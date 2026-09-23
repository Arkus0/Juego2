#!/usr/bin/env python3
"""Bind DW-04 scored transcript to its immutable provider evidence and receipt."""
import copy
import hashlib
import json
import pathlib
import sys

ROOT = pathlib.Path(__file__).resolve().parents[1]
EVIDENCE = ROOT / "Docs/evidence/WP-DW-04"
TRANSCRIPT = EVIDENCE / "ACCEPTANCE_TRANSCRIPT.jsonl"
RAW = EVIDENCE / "ACCEPTANCE_PROVIDER_RAW.jsonl"
RECEIPT = EVIDENCE / "CAMPAIGN_RECEIPT.json"


class IntegrityError(ValueError):
    pass


def req(condition, message):
    if not condition:
        raise IntegrityError(message)


def load_jsonl(path):
    return [json.loads(line) for line in path.read_text(encoding="utf-8").splitlines() if line.strip()]


def canonical_sha256(value):
    """Reproduce the adapter's provider-body serialization before hashing."""
    encoded = json.dumps(value, separators=(",", ":"), ensure_ascii=False).encode("utf-8")
    return hashlib.sha256(encoded).hexdigest()


def validate_provider_raw(response, index):
    """Bind promoted fields and scored answer to the preserved provider HTTP evidence."""
    raw = response.get("raw")
    req(isinstance(raw, dict), f"missing raw provider envelope at record {index}")

    provider_body = raw.get("provider_request_body")
    req(isinstance(provider_body, dict), f"missing provider_request_body at record {index}")
    req(
        canonical_sha256(provider_body) == response.get("provider_request_body_sha256"),
        f"provider request body/hash mismatch at record {index}",
    )
    req(
        provider_body.get("model") == response.get("model"),
        f"provider request model mismatch at record {index}",
    )

    provider_response = raw.get("provider_response")
    req(isinstance(provider_response, dict), f"missing provider_response at record {index}")
    response_id = provider_response.get("id")
    req(response_id == response.get("provider_response_id"), f"provider response id mismatch at record {index}")
    req(response_id == response.get("provider_request_id"), f"provider request id mismatch at record {index}")
    req(
        provider_response.get("model") == response.get("resolved_model"),
        f"resolved model mismatch at record {index}",
    )
    req(
        provider_response.get("provider") == response.get("resolved_provider"),
        f"resolved provider mismatch at record {index}",
    )
    req(provider_response.get("usage") == response.get("usage"), f"provider usage mismatch at record {index}")

    choices = provider_response.get("choices")
    req(isinstance(choices, list) and len(choices) == 1, f"provider response choice cardinality mismatch at record {index}")
    message = choices[0].get("message") if isinstance(choices[0], dict) else None
    req(isinstance(message, dict), f"provider response message missing at record {index}")
    answer_text = message.get("content")
    req(isinstance(answer_text, str), f"provider response content missing at record {index}")
    try:
        provider_answer = json.loads(answer_text)
    except json.JSONDecodeError as exc:
        raise IntegrityError(f"provider response content is not JSON at record {index}: {exc}") from exc
    req(provider_answer == raw.get("answer"), f"provider response/raw answer mismatch at record {index}")


def compact(response, index):
    """Must match the durable scored projection emitted by dw04-acceptance-execute.py."""
    validate_provider_raw(response, index)
    return {
        "provider_request_id": response.get("provider_request_id"),
        "provider_response_id": response.get("provider_response_id"),
        "client_request_id": response.get("client_request_id"),
        "model": response.get("model"),
        "resolved_model": response.get("resolved_model"),
        "resolved_provider": response.get("resolved_provider"),
        "provider_request_body_sha256": response.get("provider_request_body_sha256"),
        "usage": response.get("usage"),
        "raw": {"answer": response.get("raw", {}).get("answer")},
    }


def compare_records(transcript, raw, receipt):
    req(len(transcript) == 36, "scored transcript must contain exactly 36 records")
    req(len(raw) == 36, "provider raw evidence must contain exactly 36 records")
    req(receipt.get("execution_count") == 36, "campaign receipt execution_count drift")

    transcript_ids = []
    raw_ids = []
    for index, (scored, provider) in enumerate(zip(transcript, raw), start=1):
        req(scored.get("slot") == provider.get("slot"), f"slot mismatch at record {index}")
        req("response" in scored and "response" in provider, f"missing response at record {index}")
        projected = compact(provider["response"], index)
        req(scored["response"] == projected, f"scored/provider response mismatch at record {index}")
        transcript_ids.append(scored["response"].get("provider_request_id"))
        raw_ids.append(provider["response"].get("provider_request_id"))

    req(all(transcript_ids), "missing provider request id in scored transcript")
    req(len(set(transcript_ids)) == 36, "provider request ids are not unique")
    req(transcript_ids == raw_ids, "provider request id ordering differs between transcript and raw evidence")
    req(receipt.get("provider_request_ids") == transcript_ids, "campaign receipt provider_request_ids do not match evidence")


def must_reject(transcript, raw, receipt, message):
    try:
        compare_records(transcript, raw, receipt)
    except IntegrityError:
        return
    raise IntegrityError(message)


def validate():
    transcript_bytes = TRANSCRIPT.read_bytes()
    receipt = json.loads(RECEIPT.read_text(encoding="utf-8"))
    expected_digest = receipt.get("transcript_sha256")
    actual_digest = hashlib.sha256(transcript_bytes).hexdigest()
    req(expected_digest == actual_digest, "campaign receipt transcript_sha256 does not match scored transcript bytes")

    transcript = load_jsonl(TRANSCRIPT)
    raw = load_jsonl(RAW)
    compare_records(transcript, raw, receipt)

    # Negative 1: changing only the scored answer must be rejected while raw and
    # receipt remain untouched.
    tampered_scored = copy.deepcopy(transcript)
    answer = tampered_scored[0]["response"]["raw"]["answer"]
    answer["verdict"] = "REPORT" if answer.get("verdict") != "REPORT" else "REJECT"
    must_reject(
        tampered_scored,
        raw,
        receipt,
        "negative control failed: scored-answer tampering was accepted",
    )

    # Negative 2: changing only the actual provider completion must be rejected
    # even if the derived raw.answer and scored transcript are left untouched.
    tampered_response = copy.deepcopy(raw)
    provider_message = tampered_response[0]["response"]["raw"]["provider_response"]["choices"][0]["message"]
    provider_answer = json.loads(provider_message["content"])
    provider_answer["verdict"] = "REPORT" if provider_answer.get("verdict") != "REPORT" else "REJECT"
    provider_message["content"] = json.dumps(provider_answer, ensure_ascii=False, separators=(",", ":"))
    must_reject(
        transcript,
        tampered_response,
        receipt,
        "negative control failed: provider-response-only tampering was accepted",
    )

    # Negative 3: changing only the preserved provider request body must be
    # rejected against the promoted hash captured during execution.
    tampered_request = copy.deepcopy(raw)
    provider_body = tampered_request[0]["response"]["raw"]["provider_request_body"]
    provider_body["max_tokens"] = provider_body.get("max_tokens", 0) + 1
    must_reject(
        transcript,
        tampered_request,
        receipt,
        "negative control failed: provider-request-body-only tampering was accepted",
    )


if __name__ == "__main__":
    try:
        validate()
    except (IntegrityError, OSError, json.JSONDecodeError, KeyError, TypeError) as exc:
        print(f"DW04 EVIDENCE INTEGRITY RED: {exc}", file=sys.stderr)
        sys.exit(1)
    print("DW04 EVIDENCE INTEGRITY GREEN")
