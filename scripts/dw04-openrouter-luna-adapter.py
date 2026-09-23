#!/usr/bin/env python3
"""Real OpenRouter adapter for the frozen DW-04 Luna protocol.

The adapter owns only transport plus task-independent canonical output formatting.
Expected task answers/oracles are intentionally unavailable here.
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
MODEL = "openai/gpt-5.6-luna-20260709"
EXPECTED_PROVIDER = "OpenAI"
FACT_DOMAINS = {
    "importance": ["A", "B", "C", "D"],
    "spatial_depth": ["S0", "S1", "S2", "S3", "S4"],
    "interior": ["I0", "I1", "I2", "I3"],
    "fixture": ["NC-01", "NC-02", "DL-11", "DL-12", "DL-13", "DL-14"],
    "player_trigger_required": ["YES", "NO"],
    "actor_decision_without_player": ["REQUIRED", "NOT_REQUIRED"],
    "A_to_B_implies_B_to_A": ["YES", "NO"],
    "B_to_A_stays_LOW": ["YES", "NO"],
}


def fail(message, code=2):
    print(message, file=sys.stderr)
    raise SystemExit(code)


def retryable_http_status(status):
    return status in (408, 425, 429) or 500 <= status <= 599


def fact_schema(key):
    domain = FACT_DOMAINS.get(key)
    if domain is None:
        return {"type": "string"}
    return {"type": "string", "enum": domain}


def validate_answer(answer, contract=None):
    if not isinstance(answer, dict) or set(answer) != {"facts", "blockers", "verdict", "evidence"}:
        fail("provider structured answer has wrong top-level shape")
    facts = answer["facts"]
    if not isinstance(facts, dict) or not all(isinstance(k, str) and isinstance(v, str) for k, v in facts.items()):
        fail("provider facts must be a string-to-string object")
    for key, value in facts.items():
        domain = FACT_DOMAINS.get(key)
        if domain is not None and value not in domain:
            fail(f"provider fact {key} is outside canonical field domain")
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
        contract = {"fact_keys": ["importance"], "allowed_blockers": ["b"], "allowed_verdicts": ["REPORT", "REJECT"], "evidence_ids": ["e"]}
        validate_answer({"facts": {"importance": "A"}, "blockers": [], "verdict": "REPORT", "evidence": ["e"]}, contract)
        try:
            validate_answer({"facts": {"importance": "A / S1"}, "blockers": [], "verdict": "REPORT", "evidence": ["e"]}, contract)
            raise AssertionError("noncanonical fact value unexpectedly accepted")
        except SystemExit as exc:
            assert exc.code == 2
        assert retryable_http_status(408) and retryable_http_status(429) and retryable_http_status(500) and retryable_http_status(503)
        assert not retryable_http_status(400) and not retryable_http_status(401) and not retryable_http_status(402) and not retryable_http_status(404)
        print("DW-04 OpenRouter Luna canonical-schema adapter self-test: GREEN")
        return
    if len(sys.argv) != 1:
        fail("usage: dw04-openrouter-luna-adapter.py [--self-test]")

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
        fail("request is not frozen for the reviewed OpenRouter Luna route")
    if request.get("version") != "openrouter-model-id:openai/gpt-5.6-luna-20260709":
        fail("request version differs from the frozen Luna version")
    if set(options) != {"endpoint", "structured_output", "routing"}:
        fail("provider options contain an undeclared or non-effective dimension")
    if options.get("endpoint") != ENDPOINT or options.get("structured_output") != FORMAT_NAME:
        fail("provider options differ from the reviewed adapter contract")
    routing = options.get("routing")
    expected_routing = {"order": ["openai"], "allow_fallbacks": False, "require_parameters": True}
    if routing != expected_routing:
        fail("provider routing differs from the frozen OpenAI-only serving route")
    if request.get("temperature") is not None:
        fail("frozen Luna route omits unsupported temperature control")
    if request.get("tool_policy") != "none" or request.get("thinking") is not None:
        fail("DW-04 Luna calls must use no tools and no explicit reasoning override")

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
                "properties": {key: fact_schema(key) for key in response_contract["fact_keys"]},
                "required": response_contract["fact_keys"],
                "additionalProperties": False,
            },
            "blockers": {
                "type": "array",
                "items": {"type": "string", "enum": response_contract["allowed_blockers"]},
            },
            "verdict": {
                "type": "string",
                "enum": response_contract["allowed_verdicts"],
                "description": "Disposition of the reviewed claim: REJECT when blockers is non-empty because a causal blocker invalidates it; REPORT when blockers is empty.",
            },
            "evidence": {"type": "array", "items": {"type": "string", "enum": response_contract["evidence_ids"]}},
        },
        "required": ["facts", "blockers", "verdict", "evidence"],
        "additionalProperties": False,
    }
    user_text = (
        "TASK\n" + request.get("task_prompt", "")
        + "\n\nRESPONSE CONTRACT (formatting vocabulary only; allowed does not mean true)\n"
        + json.dumps(response_contract, ensure_ascii=False, sort_keys=True)
        + "\n\nCANONICAL FACT DOMAINS FOR REQUESTED KEYS\n"
        + json.dumps({key: FACT_DOMAINS.get(key, "string") for key in response_contract["fact_keys"]}, ensure_ascii=False, sort_keys=True)
        + "\n\nAUTHORITATIVE CONTEXT\n" + "\n\n".join(rendered)
    )
    body = {
        "model": MODEL,
        "messages": [
            {"role": "system", "content": request.get("system_prompt", "")},
            {"role": "user", "content": user_text},
        ],
        "max_tokens": budget["max_output_tokens"],
        "stream": False,
        "response_format": {
            "type": "json_schema",
            "json_schema": {"name": FORMAT_NAME, "strict": True, "schema": schema},
        },
        "provider": routing,
    }
    seed = request.get("seed")
    if seed is not None:
        if not isinstance(seed, int) or isinstance(seed, bool):
            fail("frozen matched-run seed must be an integer or null")
        body["seed"] = seed

    canonical_request = json.dumps(request, sort_keys=True, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
    client_request_id = str(uuid.uuid5(uuid.NAMESPACE_URL, campaign_id + ":" + hashlib.sha256(canonical_request).hexdigest()))
    provider_body = json.dumps(body, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
    http_request = urllib.request.Request(
        ENDPOINT,
        data=provider_body,
        headers={
            "Authorization": "Bearer " + api_key,
            "Content-Type": "application/json",
            "X-OpenRouter-Title": "Juego2 DW-04 frozen context trial",
        },
        method="POST",
    )
    timeout = int(budget.get("timeout_seconds", 120))
    try:
        with urllib.request.urlopen(http_request, timeout=timeout) as http_response:
            response_bytes = http_response.read()
    except urllib.error.HTTPError as exc:
        detail = exc.read().decode("utf-8", errors="replace")[:2000]
        code = 3 if retryable_http_status(exc.code) else 2
        fail(f"OpenRouter provider HTTP {exc.code}: {detail}", code)
    except (urllib.error.URLError, TimeoutError) as exc:
        fail(f"OpenRouter provider transport failure: {exc}", 3)

    try:
        response = json.loads(response_bytes)
    except json.JSONDecodeError as exc:
        fail(f"OpenRouter provider returned non-JSON: {exc}", 3)
    if not response.get("id") or not isinstance(response.get("choices"), list) or len(response["choices"]) != 1:
        fail("OpenRouter response lacks one auditable completion", 3)
    if response.get("provider") != EXPECTED_PROVIDER:
        fail(f"OpenRouter resolved unexpected provider: {response.get('provider')!r}")
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
        "provider_request_body_sha256": hashlib.sha256(provider_body).hexdigest(),
        "usage": response.get("usage"),
        "raw": {"answer": answer, "provider_request_body": body, "provider_response": response},
    }
    json.dump(result, sys.stdout, ensure_ascii=False, separators=(",", ":"))
    sys.stdout.write("\n")


if __name__ == "__main__":
    main()
