#!/usr/bin/env python3
"""Execute the frozen DW-04 CTX-only calibration campaign."""

import argparse
import datetime as dt
import hashlib
import json
import os
import pathlib
import subprocess
import sys

from importlib.machinery import SourceFileLoader

ROOT = pathlib.Path(__file__).resolve().parents[1]
trial = SourceFileLoader("dw04_trial", str(ROOT / "scripts/dw04-trial.py")).load_module()
CAL_CONTEXT = "Docs/evidence/WP-DW-04/CALIBRATION_CONTEXT.json"
CAL_PROTOCOL = "Docs/evidence/WP-DW-04/CALIBRATION_PROTOCOL.json"


def now():
    return dt.datetime.now(dt.timezone.utc).isoformat()


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def invoke(command, request, timeout):
    started = now()
    try:
        completed = subprocess.run(
            command,
            input=trial.canonical(request),
            cwd=ROOT,
            stdout=subprocess.PIPE,
            stderr=subprocess.PIPE,
            timeout=timeout,
            check=False,
        )
    except subprocess.TimeoutExpired as exc:
        return None, {
            "state": "RUN_INVALID", "kind": "timeout", "started": started, "ended": now(),
            "stderr_sha256": hashlib.sha256((exc.stderr or b"")).hexdigest(),
        }, True
    ended = now()
    if completed.returncode:
        invalid = {
            "state": "RUN_INVALID", "kind": "provider_or_adapter_failure", "exit_code": completed.returncode,
            "started": started, "ended": ended, "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
        }
        return None, invalid, completed.returncode == 3
    try:
        response = json.loads(completed.stdout)
    except json.JSONDecodeError:
        return None, {
            "state": "RUN_INVALID", "kind": "provider_non_json", "started": started, "ended": ended,
            "stdout_sha256": hashlib.sha256(completed.stdout).hexdigest(),
            "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
        }, False
    if not response.get("provider_request_id") or not isinstance(response.get("raw", {}).get("answer"), dict):
        return None, {
            "state": "RUN_INVALID", "kind": "provider_missing_structured_evidence", "started": started, "ended": ended,
            "response_sha256": trial.digest(trial.canonical(response)),
        }, False
    return response, {"started": started, "ended": ended}, False


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--pre-commit", required=True)
    parser.add_argument("--protocol-commit", required=True)
    parser.add_argument("--output", required=True)
    parser.add_argument("--campaign-id", default=os.environ.get("DW04_CAMPAIGN_ID"))
    parser.add_argument("--repository", default=os.environ.get("GITHUB_REPOSITORY"))
    parser.add_argument("--pr-number", default=os.environ.get("DW04_PR_NUMBER"))
    parser.add_argument("--candidate-sha", default=os.environ.get("DW04_CANDIDATE_SHA"))
    parser.add_argument("--run-id", default=os.environ.get("GITHUB_RUN_ID"))
    args = parser.parse_args()

    trial.require(args.campaign_id, "durable campaign id required")
    trial.require(args.repository and args.pr_number and args.candidate_sha and args.run_id,
                  "repository/PR/candidate/run identity required")
    trial.require(args.candidate_sha == trial.git("rev-parse", "HEAD").decode().strip(),
                  "calibration candidate SHA differs from exact checkout")
    trial.require(args.protocol_commit == args.candidate_sha,
                  "calibration protocol commit must equal the exact campaign candidate")

    output = pathlib.Path(args.output)
    trial.require(not output.exists(), "calibration output already exists; no overwrite/rerun")

    pre, _ = trial.frozen_file(args.pre_commit, trial.PRE)
    protocol, _ = trial.frozen_file(args.protocol_commit, CAL_PROTOCOL)
    context, _ = trial.frozen_file(args.protocol_commit, CAL_CONTEXT)
    oracle_commit = protocol["calibration_oracle_commit"]
    trial.ancestor(args.pre_commit, oracle_commit)
    trial.ancestor(oracle_commit, args.protocol_commit)
    oracles, _ = trial.frozen_file(oracle_commit, trial.CAL_ORACLES)
    trial.precheck(pre, pre["baseline_sha"])

    trial.require(protocol["schema"] == "dw04-calibration-protocol-v1", "calibration protocol schema")
    trial.require(protocol["precalibration_commit"] == args.pre_commit, "protocol pre-calibration identity")
    trial.require(oracles["precalibration_commit"] == args.pre_commit and oracles["route"] == "CTX_ONLY",
                  "calibration oracle lineage/route")
    trial.require(context["precalibration_commit"] == args.pre_commit and context["route"] == "CTX",
                  "calibration context lineage/route")
    tasks = pre["calibration_policy"]["run_order"]
    trial.require(set(protocol["tasks"]) == set(oracles["tasks"]) == set(context["contexts"]) == set(tasks),
                  "calibration task universe mismatch")

    adapter = ROOT / protocol["provider_adapter"]
    trial.require(adapter.is_file(), "frozen provider adapter missing")
    command = [sys.executable, str(adapter)]
    timeout = int(protocol["model_config"]["execution_budget"]["timeout_seconds"])
    slots = [{"task": task, "run": run, "route": "CTX"} for task in tasks for run in (1, 2)]

    result = {
        "schema": "dw04-calibration-results-v1",
        "precalibration_commit": args.pre_commit,
        "calibration_oracle_commit": oracle_commit,
        "calibration_protocol_commit": args.protocol_commit,
        "campaign": {
            "campaign_id": args.campaign_id,
            "repository": args.repository,
            "pr_number": str(args.pr_number),
            "candidate_sha": args.candidate_sha,
            "github_run_id": str(args.run_id),
        },
        "model_config": protocol["model_config"],
        "runs": [],
        "invalid_attempts": [],
        "readiness": "NOT_READY",
    }
    write_json(output, result)

    for slot in slots:
        task_id = slot["task"]
        task = protocol["tasks"][task_id]
        request = dict(protocol["model_config"])
        request.update({
            "task_prompt": task["semantic_question"],
            "response_contract": task["response_contract"],
            "system_prompt": protocol["system_prompt"],
            "slot": slot,
            "context_fragments": context["contexts"][task_id],
            "seed": None,
        })
        trial.check_request(request, protocol["model_config"], slot, task["semantic_question"],
                            task["response_contract"], protocol["system_prompt"], None)
        attempts = 0
        while True:
            attempts += 1
            response, meta, retryable = invoke(command, request, timeout)
            if response is not None:
                trial.require(response.get("model") == protocol["model_config"]["model"],
                              "provider response model differs from frozen calibration model")
                answer = response["raw"]["answer"]
                record = {
                    "slot": slot,
                    "route": "CTX",
                    "request": request,
                    "request_sha256": trial.digest(trial.canonical(request)),
                    "provider_request_id": response["provider_request_id"],
                    "provider_response_id": response.get("provider_response_id"),
                    "client_request_id": response.get("client_request_id"),
                    "model": response.get("model"),
                    "resolved_model": response.get("resolved_model"),
                    "resolved_provider": response.get("resolved_provider"),
                    "usage": response.get("usage"),
                    "answer": answer,
                    "response": response,
                    "injected_source_bytes": sum(len(f["text"].encode("utf-8")) for f in request["context_fragments"]),
                    **meta,
                }
                result["runs"].append(record)
                write_json(output, result)
                break
            invalid = {"slot": slot, "attempt": attempts, "request_sha256": trial.digest(trial.canonical(request)), **meta}
            result["invalid_attempts"].append(invalid)
            write_json(output, result)
            if retryable and attempts == 1:
                continue
            raise trial.ProtocolError("calibration provider call invalid; preserved evidence and no further adaptive retry allowed")

    trial.require(len(result["runs"]) == 8, "calibration campaign incomplete")
    scoring = []
    all_green = True
    for run in result["runs"]:
        task_id = run["slot"]["task"]
        scored = trial.score(oracles["tasks"][task_id]["oracle"], run["answer"])
        scoring.append({"slot": run["slot"], **scored})
        all_green = all_green and scored["pass"]
    result["scoring"] = scoring
    result["readiness"] = "READY" if all_green else "NOT_READY"
    write_json(output, result)
    if not all_green:
        raise trial.ProtocolError("CTX calibration completed but semantic readiness criterion is RED; no acceptance freeze allowed")
    print("DW04_CALIBRATION_READY")


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, KeyError, TypeError, OSError) as exc:
        print("DW04 CALIBRATION BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
