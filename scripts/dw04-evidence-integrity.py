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


def compact(response):
    """Must match the durable scored projection emitted by dw04-acceptance-execute.py."""
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
        projected = compact(provider["response"])
        req(scored["response"] == projected, f"scored/provider response mismatch at record {index}")
        transcript_ids.append(scored["response"].get("provider_request_id"))
        raw_ids.append(provider["response"].get("provider_request_id"))

    req(all(transcript_ids), "missing provider request id in scored transcript")
    req(len(set(transcript_ids)) == 36, "provider request ids are not unique")
    req(transcript_ids == raw_ids, "provider request id ordering differs between transcript and raw evidence")
    req(receipt.get("provider_request_ids") == transcript_ids, "campaign receipt provider_request_ids do not match evidence")


def validate():
    transcript_bytes = TRANSCRIPT.read_bytes()
    receipt = json.loads(RECEIPT.read_text(encoding="utf-8"))
    expected_digest = receipt.get("transcript_sha256")
    actual_digest = hashlib.sha256(transcript_bytes).hexdigest()
    req(expected_digest == actual_digest, "campaign receipt transcript_sha256 does not match scored transcript bytes")

    transcript = load_jsonl(TRANSCRIPT)
    raw = load_jsonl(RAW)
    compare_records(transcript, raw, receipt)

    # Causal negative control: changing only a scored answer must be rejected even
    # when the provider raw evidence and receipt remain untouched.
    tampered = copy.deepcopy(transcript)
    answer = tampered[0]["response"]["raw"]["answer"]
    answer["verdict"] = "REPORT" if answer.get("verdict") != "REPORT" else "REJECT"
    try:
        compare_records(tampered, raw, receipt)
    except IntegrityError:
        pass
    else:
        raise IntegrityError("negative control failed: scored-answer tampering was accepted")


if __name__ == "__main__":
    try:
        validate()
    except (IntegrityError, OSError, json.JSONDecodeError, KeyError, TypeError) as exc:
        print(f"DW04 EVIDENCE INTEGRITY RED: {exc}", file=sys.stderr)
        sys.exit(1)
    print("DW04 EVIDENCE INTEGRITY GREEN")
