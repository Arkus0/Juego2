#!/usr/bin/env python3
"""Execute the frozen DW-04 acceptance schedule once, without semantic retries.

The provider adapter is a real external model client (fixed executable + arguments
in the freeze), reading one JSON request on stdin and returning one JSON response
on stdout. It must return provider_request_id, model, raw.answer and optional usage.
No mock adapter is acceptance evidence. The adapter's command/version, provider
identity and execution environment must be recorded independently for review.
"""

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
    trial.require(isinstance(command, list) and command and all(isinstance(x, str) for x in command),
                  "provider command must be a frozen argument vector")
    executable = pathlib.Path(command[0]).resolve(strict=True)
    trial.require(executable.is_file() and hashlib.sha256(executable.read_bytes()).hexdigest() == adapter["executable_sha256"],
                  "provider adapter binary/script changed after freeze")
    trial.require(not pathlib.Path(args.output).exists(), "transcript path already exists; no overwrite/rerun")
    trial.require(trial.context_check(freeze, assembly, pre, selected),
                  "structural source/fallback completeness RED before any model call")
    # Exclusive creation happens before the first call. An interrupted campaign is
    # retained as partial evidence; never resume from selected successful slots.
    fd = os.open(args.output, os.O_WRONLY | os.O_CREAT | os.O_EXCL, 0o600)
    records = []
    try:
        with os.fdopen(fd, "w", encoding="utf-8") as log:
            for slot in freeze["slots"]:
                task, route = slot["task"], slot["route"]
                context = assembly["contexts"][task][route]
                request = dict(freeze["model_config"])
                request.update({"task_prompt": freeze["tasks"][task]["prompt"],
                                "system_prompt": freeze["system_prompt"],
                                "slot": slot, "context_fragments": context,
                                "seed": freeze["pair_seeds"][slot["pair"]]})
                started = datetime.datetime.now(datetime.timezone.utc).isoformat()
                completed = subprocess.run(command, input=trial.canonical(request), cwd=ROOT,
                                           stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                           timeout=freeze["timeout_seconds"], check=False)
                ended = datetime.datetime.now(datetime.timezone.utc).isoformat()
                if completed.returncode or not completed.stdout:
                    record = {"slot": slot, "state": "RUN_INVALID", "started": started, "ended": ended,
                              "exit_code": completed.returncode,
                              "stderr_sha256": hashlib.sha256(completed.stderr).hexdigest(),
                              "freeze_commit": args.freeze_commit, "freeze_sha256": freeze_sha,
                              "assembly_commit": args.assembly_commit, "assembly_sha256": assembly_sha}
                    log.write(json.dumps(record, ensure_ascii=False) + "\n")
                    log.flush()
                    raise trial.ProtocolError("provider failed; preserve partial evidence and apply only frozen invalid-run policy")
                try:
                    response = json.loads(completed.stdout)
                except json.JSONDecodeError as exc:
                    raise trial.ProtocolError("provider returned non-JSON; retain partial evidence") from exc
                trial.require(response.get("provider_request_id") and response.get("model") and
                              isinstance(response.get("raw", {}).get("answer"), dict),
                              "provider response lacks actual structured model execution evidence")
                record = {"slot": slot, "request": request, "request_sha256": trial.digest(trial.canonical(request)),
                          "response": response, "injected_source_bytes": sum(len(f["text"].encode("utf-8")) for f in context),
                          "started": started, "ended": ended, "freeze_commit": args.freeze_commit,
                          "freeze_sha256": freeze_sha, "assembly_commit": args.assembly_commit,
                          "assembly_sha256": assembly_sha}
                records.append(record)
                log.write(json.dumps(record, ensure_ascii=False) + "\n")
                log.flush()
    finally:
        # The exclusive JSONL file remains even when a provider/transport call
        # fails. It is never silently replaced with a favorable new campaign.
        pass
    # A JSONL execution log is preserved unchanged. The JSON array used for audit
    # is a lossless deterministic conversion, never a filtered success subset.
    trial.require(len(records) == 36, "incomplete execution campaign")
    print("36 provider executions recorded; audit accepts the unfiltered JSONL transcript")


if __name__ == "__main__":
    try:
        main()
    except (trial.ProtocolError, OSError, subprocess.TimeoutExpired) as exc:
        print("DW04 EXECUTION BLOCKED: " + str(exc), file=sys.stderr)
        sys.exit(1)
