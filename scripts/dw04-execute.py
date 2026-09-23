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


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--pre-commit", required=True)
    parser.add_argument("--freeze-commit", required=True)
    parser.add_argument("--assembly-commit", required=True)
    parser.add_argument("--output", required=True, help="new, nonexistent transcript path")
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
    trial.require(pathlib.Path(command[0]).name.startswith("python") or command[0] in ("python3", sys.executable),
                  "provider adapter must execute through the frozen Python script path")

    output_path = pathlib.Path(args.output)
    trial.require(not output_path.exists(), "transcript path already exists; no overwrite/rerun")
    trial.require(trial.context_check(freeze, assembly, pre, selected),
                  "structural source/fallback completeness RED before any model call")

    fd = os.open(output_path, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    records = []
    try:
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
                started = datetime.datetime.now(datetime.timezone.utc).isoformat()
                completed = subprocess.run(command, input=trial.canonical(request), cwd=ROOT,
                                           stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                           timeout=freeze["timeout_seconds"], check=False)
                ended = datetime.datetime.now(datetime.timezone.utc).isoformat()
                if completed.returncode or not completed.stdout:
                    record = {
                        "slot": slot,
                        "state": "RUN_INVALID",
                        "started": started,
                        "ended": ended,
                        "exit_code": completed.returncode,
                        "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
                        "request": request,
                        "request_sha256": trial.digest(trial.canonical(request)),
                        "freeze_commit": args.freeze_commit,
                        "freeze_sha256": freeze_sha,
                        "assembly_commit": args.assembly_commit,
                        "assembly_sha256": assembly_sha,
                    }
                    log.write(json.dumps(record, ensure_ascii=False) + "\n")
                    log.flush()
                    raise trial.ProtocolError("provider failed; preserve partial evidence and apply only frozen invalid-run policy")
                try:
                    response = json.loads(completed.stdout)
                except json.JSONDecodeError as exc:
                    raise trial.ProtocolError("provider returned non-JSON; retain partial evidence") from exc
                trial.require(response.get("provider_request_id") and response.get("model") == freeze["model_config"]["model"] and
                              isinstance(response.get("raw", {}).get("answer"), dict),
                              "provider response lacks actual structured model execution evidence")
                record = {
                    "slot": slot,
                    "request": request,
                    "request_sha256": trial.digest(trial.canonical(request)),
                    "response": response,
                    "injected_source_bytes": sum(len(f["text"].encode("utf-8")) for f in context),
                    "started": started,
                    "ended": ended,
                    "freeze_commit": args.freeze_commit,
                    "freeze_sha256": freeze_sha,
                    "assembly_commit": args.assembly_commit,
                    "assembly_sha256": assembly_sha,
                }
                records.append(record)
                log.write(json.dumps(record, ensure_ascii=False) + "\n")
                log.flush()
    finally:
        pass

    trial.require(len(records) == 36, "incomplete execution campaign")
    print("36 provider executions recorded; audit accepts the unfiltered JSONL transcript")


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, OSError, subprocess.TimeoutExpired) as exc:
        print("DW04 EXECUTION BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
