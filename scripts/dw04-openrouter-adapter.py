#!/usr/bin/env python3
"""Real OpenRouter adapter for the frozen DW-04 model campaign.

Reads one canonical DW-04 request JSON object from stdin and writes one JSON
response to stdout. Expected answers/oracles are intentionally unavailable here.
The exact DeepSeek model and DeepSeek provider route are supplied by the frozen
protocol, with provider fallback disabled for matched-run reproducibility.
"""

import hashlib
import json
import os
import sys
import urllib.error
import urllib.request
import uuid

ENDPOINT = "https://openrouter.ai/api/v1/chat/completions"
FORMAT_NAME = "dw04_answer_v1"
MODEL = "deepseek/deepseek-v4.1-flash"


def fail(message, code=2):
    print(message, file=sys.stderr)
    raise SystemExit(code)


def validate_answer(answer, contract=None):
    if not isinstance(answer, dict) or set(answer) != {"facts", "blockers", "verdict", "evidence"}:
        fail("provider structured answer has wrong top-level shape")
    facts = answer["facts"]
    if not isinstance(facts, dict) or not all(isinstance(k, str) and isinstance(v, str) for k, v in facts.items()):
        fail("provider facts must be a string-to-string object")
    for field in ("blockers", "evidence"):
        value = answer[field]
        if not isinstance(value, list) or not all(isinstance(x, str) for x in value) or len(value) != len(set(value)):
            fail(f"provider {field} must be a unique string array")
    if not isinstance(answer["verdict"], str):
        fail("provider verdict must be a string")
    if contract is not None:
        if set(facts) != set(contract["fact_keys"]):
            fail("provider facts differ from the frozen fact-key contract")
        if not set(answer["blockers"]) <= set(contract["allowed_blockers"]):
            fail("provider emitted blocker outside frozen formatting vocabulary")
        if answer["verdict"] not in contract["allowed_verdicts"]:
            fail("provider verdict is outside frozen formatting vocabulary")
        if not set(answer["evidence"]) <= set(contract["evidence_ids"]):
            fail("provider evidence is outside frozen formatting vocabulary")


def main():
    if len(sys.argv) == 2 and sys.argv[1] == "--self-test":
        contract = {"fact_keys": ["f"], "allowed_blockers": ["b"], "allowed_verdicts": ["REPORT", "REJECT"], "evidence_ids": ["e"]}
        validate_answer({"facts": {"f": "v"}, "blockers": [], "verdict": "REPORT", "evidence": ["e"]}, contract)
        print("DW-04 OpenRouter adapter self-test: GREEN")
        return
    if len(sys.argv) != 1:
        fail("usage: dw04-openrouter-adapter.py [--self-test]")

    api_key = os.environ.get("OPENROUTER_API_KEY")
    campaign_id = os.environ.get("DW04_CAMPAIGN_ID")
    if not api_key:
        fail("OPENROUTER_API_KEY is required for real DW-04 provider execution")
    if not campaign_id:
        fail("DW04_CAMPAIGN_ID is required to bind provider calls to one durable campaign")

    try:
        request = json.load(sys.stdin)
    except json.JSONDecodeError as exc:
        fail(f"invalid DW-04 request JSON: {exc}")
    if not isinstance(request, dict):
        fail("DW-04 request must be an object")

    options = request.get("provider_options")
    if request.get("provider") != "openrouter-chat-completions" or request.get("model") != MODEL or not isinstance(options, dict):
        fail("request is not frozen for the reviewed OpenRouter DeepSeek route")
    if options.get("endpoint") != ENDPOINT or options.get("structured_output") != FORMAT_NAME or options.get("store") is not False:
        fail("provider options differ from the reviewed adapter contract")
    routing = options.get("routing")
    expected_routing = {"order": ["deepseek"], "allow_fallbacks": False, "require_parameters": True}
    if routing != expected_routing:
        fail("provider routing differs from the frozen DeepSeek-only route")
    if request.get("tool_policy") != "none" or request.get("thinking") is not None:
        fail("DW-04 DeepSeek calls must use no tools and no extra reasoning control")

    response_contract = request.get("response_contract")
    if not isinstance(response_contract, dict) or set(response_contract) != {"fact_keys", "allowed_blockers", "allowed_verdicts", "evidence_ids"}:
        fail("DW-04 response contract shape is invalid")
    if not isinstance(response_contract["fact_keys"], list) or not all(isinstance(x, str) for x in response_contract["fact_keys"]):
        fail("DW-04 response contract fact_keys are invalid")
    for field in ("allowed_blockers", "allowed_verdicts", "evidence_ids"):
        if not isinstance(response_contract[field], list) or not all(isinstance(x, str) for x in response_contract[field]):
            fail(f"DW-04 response contract {field} is invalid")

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
            "facts": {
                "type": "object",
                "properties": {key: {"type": "string"} for key in response_contract["fact_keys"]},
                "required": response_contract["fact_keys"],
                "additionalProperties": False,
            },
            "blockers": {"type": "array", "items": {"type": "string", "enum": response_contract["allowed_blockers"]}},
            "verdict": {"type": "string", "enum": response_contract["allowed_verdicts"]},
            "evidence": {"type": "array", "items": {"type": "string", "enum": response_contract["evidence_ids"]}},
        },
        "required": ["facts", "blockers", "verdict", "evidence"],
        "additionalProperties": False,
    }
    user_text = (
        "TASK\n" + request.get("task_prompt", "")
        + "\n\nRESPONSE CONTRACT (formatting vocabulary only; allowed does not mean true)\n"
        + json.dumps(response_contract, ensure_ascii=False, sort_keys=True)
        + "\n\nAUTHORITATIVE CONTEXT\n" + "\n\n".join(rendered)
    )
    body = {
        "model": MODEL,
        "messages": [
            {"role": "system", "content": request.get("system_prompt", "")},
            {"role": "user", "content": user_text},
        ],
        "max_tokens": budget["max_output_tokens"],
        "temperature": request.get("temperature", 0),
        "stream": False,
        "response_format": {
            "type": "json_schema",
            "json_schema": {"name": FORMAT_NAME, "strict": True, "schema": schema},
        },
        "provider": routing,
    }

    canonical_request = json.dumps(request, sort_keys=True, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
    client_request_id = str(uuid.uuid5(uuid.NAMESPACE_URL, campaign_id + ":" + hashlib.sha256(canonical_request).hexdigest()))
    http_request = urllib.request.Request(
        ENDPOINT,
        data=json.dumps(body, separators=(",", ":"), ensure_ascii=False).encode("utf-8"),
        headers={
            "Authorization": "Bearer " + api_key,
            "Content-Type": "application/json",
            "X-Title": "Juego2 DW-04 frozen context trial",
        },
        method="POST",
    )
    timeout = int(budget.get("timeout_seconds", 120))
    try:
        with urllib.request.urlopen(http_request, timeout=timeout) as http_response:
            response_bytes = http_response.read()
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")[:2000]
        fail(f"OpenRouter provider HTTP {exc.code}: {detail}", 3)
    except (urllib.error.URLError, TimeoutError) as exc:
        fail(f"OpenRouter provider transport failure: {exc}", 3)

    try:
        response = json.loads(response_bytes)
    except json.JSONDecodeError as exc:
        fail(f"OpenRouter provider returned non-JSON: {exc}", 3)
    if not response.get("id") or not isinstance(response.get("choices"), list) or len(response["choices"]) != 1:
        fail("OpenRouter response lacks one auditable completion", 3)
    message = response["choices"][0].get("message", {})
    answer_text = message.get("content")
    if not isinstance(answer_text, str):
        fail("OpenRouter completion has no textual structured answer", 3)
    try:
        answer = json.loads(answer_text)
    except json.JSONDecodeError as exc:
        fail(f"OpenRouter structured completion is not JSON: {exc}", 3)
    validate_answer(answer, response_contract)

    result = {
        "provider_request_id": response["id"],
        "provider_response_id": response["id"],
        "client_request_id": client_request_id,
        "model": request.get("model"),
        "resolved_model": response.get("model"),
        "resolved_provider": response.get("provider"),
        "usage": response.get("usage"),
        "raw": {"answer": answer, "provider_response": response},
    }
    json.dump(result, sys.stdout, ensure_ascii=False, separators=(",", ":"))
    sys.stdout.write("\n")


if __name__ == "__main__":
    main()
