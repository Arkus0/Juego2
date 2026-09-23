#!/usr/bin/env python3
"""Execute the frozen DW-04 acceptance schedule once with only frozen objective retries."""

import argparse
import datetime
import hashlib
import json
import os
import pathlib
import subprocess
import sys

from importlib.machinery import SourceFileLoader

ROOT = pathlib.Path(__file__).resolve().parents[1]
trial = SourceFileLoader("dw04_trial", str(ROOT / "scripts/dw04-trial.py")).load_module()


def now():
    return datetime.datetime.now(datetime.timezone.utc).isoformat()


def invalid_receipt(slot, attempt, completed, request, freeze_commit, freeze_sha, assembly_commit, assembly_sha):
    stderr = completed.stderr.decode("utf-8", errors="replace")
    return {
        "slot": slot,
        "attempt": attempt,
        "state": "RUN_INVALID",
        "exit_code": completed.returncode,
        "retryable_objective_failure": completed.returncode == 3,
        "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
        "stderr_excerpt": stderr[:1200],
        "request_sha256": trial.digest(trial.canonical(request)),
        "freeze_commit": freeze_commit,
        "freeze_sha256": freeze_sha,
        "assembly_commit": assembly_commit,
        "assembly_sha256": assembly_sha,
    }


def compact_response(response):
    raw = response.get("raw", {})
    return {
        "provider_request_id": response.get("provider_request_id"),
        "provider_response_id": response.get("provider_response_id"),
        "client_request_id": response.get("client_request_id"),
        "model": response.get("model"),
        "resolved_model": response.get("resolved_model"),
        "resolved_provider": response.get("resolved_provider"),
        "provider_request_body_sha256": response.get("provider_request_body_sha256"),
        "usage": response.get("usage"),
        "raw": {"answer": raw.get("answer")},
    }


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--pre-commit", required=True)
    parser.add_argument("--freeze-commit", required=True)
    parser.add_argument("--assembly-commit", required=True)
    parser.add_argument("--output", required=True, help="new, nonexistent audit transcript path")
    parser.add_argument("--raw-output", help="optional raw provider evidence path; defaults beside --output")
    args = parser.parse_args()

    pre, _ = trial.frozen_file(args.pre_commit, trial.PRE)
    freeze, freeze_sha = trial.frozen_file(args.freeze_commit, trial.FREEZE)
    trial.ancestor(args.pre_commit, args.freeze_commit)
    selected = trial.precheck(pre, pre["baseline_sha"])
    trial.require(freeze["selected"] == selected and freeze["slots"] == trial.make_slots(selected),
                  "acceptance schedule not frozen from the independent universe")
    trial.require(freeze["scorer_sha256"] == trial.digest((ROOT / "scripts/dw04-trial.py").read_bytes()),
                  "scorer changed after freeze")
    trial.calibration_check(pre, args.pre_commit, freeze)

    trial.ancestor(args.freeze_commit, args.assembly_commit)
    assembly, assembly_sha = trial.frozen_file(args.assembly_commit, trial.ASSEMBLY)
    trial.require(assembly["freeze_commit"] == args.freeze_commit and assembly["freeze_sha256"] == freeze_sha,
                  "assembly must descend from the selection freeze before acceptance calls")

    adapter = freeze["provider_adapter"]
    command = adapter["command"]
    script_path = adapter["script_path"]
    trial.require(isinstance(command, list) and len(command) == 2 and all(isinstance(x, str) for x in command),
                  "provider command must be a frozen two-element Python argument vector")
    trial.require(command[1] == script_path, "provider command/script identity mismatch")
    script = (ROOT / script_path).resolve(strict=True)
    trial.require(script.is_file() and hashlib.sha256(script.read_bytes()).hexdigest() == adapter["script_sha256"],
                  "provider adapter script changed after freeze")

    output_path = pathlib.Path(args.output)
    raw_path = pathlib.Path(args.raw_output) if args.raw_output else output_path.with_name("ACCEPTANCE_PROVIDER_RAW.jsonl")
    trial.require(not output_path.exists() and not raw_path.exists(), "transcript paths already exist; no overwrite/rerun")
    trial.require(trial.context_check(freeze, assembly, pre, selected),
                  "structural source/fallback completeness RED before any model call")

    audit_fd = os.open(output_path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    raw_fd = os.open(raw_path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    records = []
    with os.fdopen(audit_fd, "w", encoding="utf-8") as log, os.fdopen(raw_fd, "w", encoding="utf-8") as raw_log:
        for slot in freeze["slots"]:
            task, route = slot["task"], slot["route"]
            context = assembly["contexts"][task][route]
            task_freeze = freeze["tasks"][task]
            request = dict(freeze["model_config"])
            request.update({
                "task_prompt": task_freeze["prompt"],
                "response_contract": task_freeze["response_contract"],
                "system_prompt": freeze["system_prompt"],
                "slot": slot,
                "context_fragments": context,
                "seed": freeze["pair_seeds"][slot["pair"]],
            })
            invalids = []
            response = None
            started = now()
            for attempt in (1, 2):
                completed = subprocess.run(command, input=trial.canonical(request), cwd=ROOT,
                                           stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                           timeout=freeze["timeout_seconds"], check=False)
                if completed.returncode == 0 and completed.stdout:
                    try:
                        response = json.loads(completed.stdout)
                    except json.JSONDecodeError as exc:
                        raise trial.ProtocolError("provider returned non-JSON with exit 0; semantic retry forbidden") from exc
                    raw_log.write(json.dumps({"slot": slot, "attempt": attempt, "response": response}, ensure_ascii=False) + "\n")
                    raw_log.flush()
                    break
                receipt = invalid_receipt(slot, attempt, completed, request, args.freeze_commit, freeze_sha,
                                          args.assembly_commit, assembly_sha)
                invalids.append(receipt)
                raw_log.write(json.dumps({"slot": slot, "attempt": attempt, "invalid": receipt}, ensure_ascii=False) + "\n")
                raw_log.flush()
                if completed.returncode == 3 and attempt == 1:
                    continue
                terminal = {"slot":slot,"state":"RUN_INVALID_TERMINAL","invalid_attempts":invalids,
                            "request":request,"request_sha256":trial.digest(trial.canonical(request)),
                            "freeze_commit":args.freeze_commit,"freeze_sha256":freeze_sha,
                            "assembly_commit":args.assembly_commit,"assembly_sha256":assembly_sha}
                log.write(json.dumps(terminal, ensure_ascii=False) + "\n")
                log.flush()
                raise trial.ProtocolError("provider failed outside the single frozen objective-invalid replacement allowance")

            trial.require(response is not None, "provider produced no scorable response")
            trial.require(response.get("provider_request_id") and response.get("model") == freeze["model_config"]["model"] and
                          isinstance(response.get("raw", {}).get("answer"), dict),
                          "provider response lacks actual structured model execution evidence")
            record = {
                "slot": slot,
                "attempt": 1 + len(invalids),
                "invalid_attempts": invalids,
                "request": request,
                "request_sha256": trial.digest(trial.canonical(request)),
                "response": compact_response(response),
                "injected_source_bytes": sum(len(f["text"].encode("utf-8")) for f in context),
                "started": started,
                "ended": now(),
                "freeze_commit": args.freeze_commit,
                "freeze_sha256": freeze_sha,
                "assembly_commit": args.assembly_commit,
                "assembly_sha256": assembly_sha,
            }
            records.append(record)
            log.write(json.dumps(record, ensure_ascii=False) + "\n")
            log.flush()

    trial.require(len(records) == 36, "incomplete execution campaign")
    print("36 scorable provider executions recorded; compact audit transcript and raw provider evidence are both preserved")


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, OSError, subprocess.TimeoutExpired) as exc:
        print("DW04 EXECUTION BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
