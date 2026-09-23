#!/usr/bin/env python3
"""Execute the frozen DW-04 acceptance schedule once, without semantic retries."""

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


def write_json(path, value):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")


def invoke(command, request, timeout):
    started = now()
    try:
        completed = subprocess.run(command, input=trial.canonical(request), cwd=ROOT,
                                   stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                   timeout=timeout, check=False)
    except subprocess.TimeoutExpired as exc:
        return None, {
            "state": "RUN_INVALID", "kind": "timeout", "started": started, "ended": now(),
            "stderr_sha256": hashlib.sha256(exc.stderr or b"").hexdigest(),
        }, True
    ended = now()
    if completed.returncode:
        return None, {
            "state": "RUN_INVALID", "kind": "provider_or_adapter_failure", "exit_code": completed.returncode,
            "started": started, "ended": ended, "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
        }, completed.returncode == 3
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
    parser.add_argument("--freeze-commit", required=True)
    parser.add_argument("--assembly-commit", required=True)
    parser.add_argument("--output", required=True, help="new, nonexistent transcript path")
    parser.add_argument("--invalid-output", required=True, help="new invalid-attempt evidence path")
    args = parser.parse_args()

    pre, _ = trial.frozen_file(args.pre_commit, trial.PRE)
    freeze, freeze_sha = trial.frozen_file(args.freeze_commit, trial.FREEZE)
    trial.ancestor(args.pre_commit, args.freeze_commit)
    selected = trial.precheck(pre, pre["baseline_sha"])
    trial.require(freeze["selected"] == selected and freeze["slots"] == trial.make_slots(selected),
                  "acceptance schedule not frozen from the independent universe")
    trial.require(freeze["scorer_sha256"] == trial.digest((ROOT / "scripts/dw04-trial.py").read_bytes()),
                  "scorer changed after freeze")
    trial.require(freeze.get("invalid_run_policy") == "one_objective_invalid_replacement_per_slot_no_semantic_retry",
                  "acceptance invalid-run policy is not the reviewed bounded policy")
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
    trial.require(pathlib.Path(command[0]).name.startswith("python") or command[0] in ("python3", sys.executable),
                  "provider adapter must execute through the frozen Python script path")

    output_path = pathlib.Path(args.output)
    invalid_path = pathlib.Path(args.invalid_output)
    trial.require(not output_path.exists() and not invalid_path.exists(), "acceptance evidence path already exists; no overwrite/rerun")
    trial.require(trial.context_check(freeze, assembly, pre, selected),
                  "structural source/fallback completeness RED before any model call")

    fd = os.open(output_path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    invalid_attempts = []
    write_json(invalid_path, invalid_attempts)
    records = []
    with os.fdopen(fd, "w", encoding="utf-8") as log:
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
            trial.check_request(request, freeze["model_config"], slot, task_freeze["prompt"],
                                task_freeze["response_contract"], freeze["system_prompt"],
                                freeze["pair_seeds"][slot["pair"]])
            attempt = 0
            while True:
                attempt += 1
                trial.require(attempt <= 2, "objective invalid-run replacement budget exceeded before provider call")
                response, meta, retryable = invoke(command, request, freeze["timeout_seconds"])
                if response is not None:
                    trial.require(response.get("model") == freeze["model_config"]["model"],
                                  "provider response model differs from frozen acceptance model")
                    record = {
                        "slot": slot,
                        "attempt": attempt,
                        "request": request,
                        "request_sha256": trial.digest(trial.canonical(request)),
                        "response": response,
                        "injected_source_bytes": sum(len(f["text"].encode("utf-8")) for f in context),
                        "freeze_commit": args.freeze_commit,
                        "freeze_sha256": freeze_sha,
                        "assembly_commit": args.assembly_commit,
                        "assembly_sha256": assembly_sha,
                        **meta,
                    }
                    records.append(record)
                    log.write(json.dumps(record, ensure_ascii=False) + "\n")
                    log.flush()
                    break
                invalid = {
                    "slot": slot, "attempt": attempt,
                    "request_sha256": trial.digest(trial.canonical(request)),
                    "freeze_commit": args.freeze_commit,
                    "assembly_commit": args.assembly_commit,
                    **meta,
                }
                invalid_attempts.append(invalid)
                write_json(invalid_path, invalid_attempts)
                if retryable and attempt == 1:
                    continue
                raise trial.ProtocolError("provider call invalid; preserved evidence and no further adaptive retry allowed")

    trial.require(len(records) == 36, "incomplete execution campaign")
    print(f"36 provider executions recorded; objective invalid attempts retained: {len(invalid_attempts)}")


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, OSError, subprocess.TimeoutExpired) as exc:
        print("DW04 EXECUTION BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
