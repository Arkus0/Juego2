#!/usr/bin/env python3
"""Real OpenAI Responses adapter for the frozen DW-04 model campaign.

Reads one canonical DW-04 request JSON object from stdin and writes one JSON
response to stdout. Expected answers/oracles are intentionally unavailable here.
"""

import hashlib
import json
import os
import sys
import urllib.error
import urllib.request
import uuid

ENDPOINT = "https://api.openai.com/v1/responses"
FORMAT_NAME = "dw04_answer_v1"


def fail(message, code=2):
    print(message, file=sys.stderr)
    raise SystemExit(code)


def validate_answer(answer):
    if not isinstance(answer, dict) or set(answer) != {"facts", "blockers", "verdict", "evidence"}:
        fail("provider structured answer has wrong top-level shape")
    facts = answer["facts"]
    if not isinstance(facts, dict) or not all(isinstance(k, str) and isinstance(v, str) for k, v in facts.items()):
        fail("provider facts must be a string-to-string object")
    for field in ("blockers", "evidence"):
        value = answer[field]
        if not isinstance(value, list) or not all(isinstance(x, str) for x in value) or len(value) != len(set(value)):
            fail(f"provider {field} must be a unique string array")
    if answer["verdict"] not in ("REPORT", "REJECT"):
        fail("provider verdict must be REPORT or REJECT")


def output_text(response):
    texts = []
    for item in response.get("output", []):
        if item.get("type") != "message":
            continue
        for part in item.get("content", []):
            if part.get("type") == "output_text" and isinstance(part.get("text"), str):
                texts.append(part["text"])
    if len(texts) != 1:
        fail("provider response did not contain exactly one structured output_text")
    return texts[0]


def main():
    if len(sys.argv) == 2 and sys.argv[1] == "--self-test":
        validate_answer({"facts": {"f": "v"}, "blockers": [], "verdict": "REPORT", "evidence": ["e"]})
        print("DW-04 OpenAI adapter self-test: GREEN")
        return
    if len(sys.argv) != 1:
        fail("usage: dw04-openai-adapter.py [--self-test]")

    api_key = os.environ.get("OPENAI_API_KEY")
    campaign_id = os.environ.get("DW04_CAMPAIGN_ID")
    if not api_key:
        fail("OPENAI_API_KEY is required for real DW-04 provider execution")
    if not campaign_id:
        fail("DW04_CAMPAIGN_ID is required to bind provider calls to one durable campaign")

    try:
        request = json.load(sys.stdin)
    except json.JSONDecodeError as exc:
        fail(f"invalid DW-04 request JSON: {exc}")
    if not isinstance(request, dict):
        fail("DW-04 request must be an object")

    options = request.get("provider_options")
    if request.get("provider") != "openai-responses" or not isinstance(options, dict):
        fail("request is not frozen for the OpenAI Responses provider")
    if options.get("endpoint") != ENDPOINT or options.get("store") is not False or options.get("structured_output") != FORMAT_NAME:
        fail("provider options differ from the reviewed adapter contract")
    if request.get("tool_policy") != "none":
        fail("DW-04 provider calls must not enable tools")

    contexts = request.get("context_fragments")
    if not isinstance(contexts, list) or not contexts:
        fail("DW-04 request has no context fragments")
    rendered = []
    for fragment in contexts:
        if not isinstance(fragment, dict) or not isinstance(fragment.get("text"), str):
            fail("DW-04 context fragment shape is invalid")
        rendered.append(
            f"--- fragment {fragment.get('id', '<missing>')} | source {fragment.get('source_path', '<missing>')} ---\n"
            + fragment["text"]
        )

    budget = request.get("execution_budget")
    if not isinstance(budget, dict) or not isinstance(budget.get("max_output_tokens"), int):
        fail("execution budget is missing max_output_tokens")

    schema = {
        "type": "object",
        "properties": {
            "facts": {"type": "object", "additionalProperties": {"type": "string"}},
            "blockers": {"type": "array", "items": {"type": "string"}},
            "verdict": {"type": "string", "enum": ["REPORT", "REJECT"]},
            "evidence": {"type": "array", "items": {"type": "string"}},
        },
        "required": ["facts", "blockers", "verdict", "evidence"],
        "additionalProperties": False,
    }
    user_text = "TASK\n" + request.get("task_prompt", "") + "\n\nAUTHORITATIVE CONTEXT\n" + "\n\n".join(rendered)
    body = {
        "model": request.get("model"),
        "instructions": request.get("system_prompt"),
        "input": [{"role": "user", "content": [{"type": "input_text", "text": user_text}]}],
        "max_output_tokens": budget["max_output_tokens"],
        "store": False,
        "tools": [],
        "text": {"format": {"type": "json_schema", "name": FORMAT_NAME, "schema": schema, "strict": False}},
    }
    thinking = request.get("thinking")
    if thinking is not None:
        body["reasoning"] = {"effort": thinking}
    if request.get("temperature") is not None:
        body["temperature"] = request["temperature"]

    canonical_request = json.dumps(request, sort_keys=True, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
    client_request_id = str(uuid.uuid5(uuid.NAMESPACE_URL, campaign_id + ":" + hashlib.sha256(canonical_request).hexdigest()))
    http_request = urllib.request.Request(
        ENDPOINT,
        data=json.dumps(body, separators=(",", ":"), ensure_ascii=False).encode("utf-8"),
        headers={
            "Authorization": "Bearer " + api_key,
            "Content-Type": "application/json",
            "X-Client-Request-Id": client_request_id,
        },
        method="POST",
    )
    timeout = int(budget.get("timeout_seconds", 120))
    try:
        with urllib.request.urlopen(http_request, timeout=timeout) as http_response:
            response_bytes = http_response.read()
            provider_request_id = http_response.headers.get("x-request-id")
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")[:2000]
        fail(f"OpenAI provider HTTP {exc.code}: {detail}", 3)
    except (urllib.error.URLError, TimeoutError) as exc:
        fail(f"OpenAI provider transport failure: {exc}", 3)

    try:
        response = json.loads(response_bytes)
    except json.JSONDecodeError as exc:
        fail(f"OpenAI provider returned non-JSON: {exc}", 3)
    if response.get("status") != "completed" or not response.get("id"):
        fail("OpenAI provider response is not completed", 3)
    if not provider_request_id:
        fail("OpenAI x-request-id header is missing", 3)

    resolved_model = response.get("model")
    answer_text = output_text(response)
    try:
        answer = json.loads(answer_text)
    except json.JSONDecodeError as exc:
        fail(f"structured output_text is not JSON: {exc}", 3)
    validate_answer(answer)

    result = {
        "provider_request_id": provider_request_id,
        "provider_response_id": response["id"],
        "client_request_id": client_request_id,
        "model": request.get("model"),
        "resolved_model": resolved_model,
        "usage": response.get("usage"),
        "raw": {"answer": answer, "provider_response": response},
    }
    json.dump(result, sys.stdout, ensure_ascii=False, separators=(",", ":"))
    sys.stdout.write("\n")


if __name__ == "__main__":
    main()
