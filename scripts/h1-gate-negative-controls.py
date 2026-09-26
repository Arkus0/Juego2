#!/usr/bin/env python3
"""WP-H1-GATE gate-owned causal negative-conformance controls.

Each control removes or corrupts one real Gate obligation while the declaration stays in place. It then requires
`h1-gate-verify.py` to go RED with the specific reason code for that omission. A control that stays GREEN, or goes RED
for an unrelated reason, fails the Gate. The WP's seven causal classes are covered:

  C1 a real mandatory gate execution removed while declaration remains
  C2 one inherited H1 stage/residual/dependency omitted from reconciliation
  C3 Unity artifact substituted as canonical recovery truth
  C4 normalized parity stage skipped while screenshot remains
  C5 AI trial artifact/SHA mismatch or hidden/private call
  C6 gate-owned proof universe self-shrinks
  C7 a required H1 capability absent from composed discovery while a private Unity path keeps the scenario green

`static` needs no Unity: reconciliation, workflow, universe, scope and synthetic trial controls.
`evidence --dir DIR --candidate-sha SHA` mutates the real effective evidence bundle of this run.
`trial --dir DIR --candidate-sha SHA` mutates the real AI-trial artifacts of this run.
"""

import argparse
import copy
import hashlib
import json
import os
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
VERIFY = ROOT / "scripts/h1-gate-verify.py"
results = []


def run_verifier(arguments, env=None):
    completed = subprocess.run([sys.executable, str(VERIFY), *arguments], cwd=ROOT, text=True,
                               stdout=subprocess.PIPE, stderr=subprocess.PIPE, env=env)
    return completed.returncode, completed.stdout + completed.stderr


def expect_red(label, klass, arguments, code, env=None):
    rc, output = run_verifier(arguments, env)
    ok = rc != 0 and f"H1_GATE_VERIFY_RED {code}" in output
    results.append((klass, label, ok))
    marker = "H1_GATE_NEGATIVE_CONTROL_GREEN" if ok else "H1_GATE_NEGATIVE_CONTROL_RED"
    print(f"{marker} {klass} {label} expected={code} rc={rc}")
    if not ok:
        print(output[-3000:], file=sys.stderr)


def expect_green(label, arguments, env=None):
    rc, output = run_verifier(arguments, env)
    ok = rc == 0 and "H1_GATE_VERIFY_GREEN" in output
    results.append(("baseline", label, ok))
    print(f"{'H1_GATE_NEGATIVE_BASELINE_GREEN' if ok else 'H1_GATE_NEGATIVE_BASELINE_RED'} {label}")
    if not ok:
        print(output[-3000:], file=sys.stderr)


def finish(scope):
    failed = [f"{klass}:{label}" for klass, label, ok in results if not ok]
    if failed:
        print(f"H1_GATE_NEGATIVE_CONTROLS_RED {scope} failed={failed}", file=sys.stderr)
        sys.exit(1)
    classes = sorted({klass for klass, _, _ in results if klass != "baseline"})
    print(f"H1_GATE_NEGATIVE_CONTROLS_GREEN {scope} controls={len(results)} classes={','.join(classes)}")


# ----- helpers -------------------------------------------------------------------------------------------------------

def read_jsonl(path):
    return [json.loads(line) for line in Path(path).read_text(encoding="utf-8").splitlines() if line.strip()]


def write_jsonl(path, records):
    Path(path).write_text("".join(json.dumps(record, sort_keys=True, separators=(",", ":")) + "\n" for record in records), encoding="utf-8")


def mutated_copy(source, mutate):
    target = Path(tempfile.mkdtemp(prefix="h1-gate-negative-"))
    shutil.copytree(source, target, dirs_exist_ok=True)
    mutate(target)
    return target


def mirror_root(changes):
    """A copy of the governing documents with `changes` applied; the verifier reads it through its root override."""
    target = Path(tempfile.mkdtemp(prefix="h1-gate-root-"))
    for relative in ("Docs/engineering", "Docs/architecture", "Docs/evidence", "Docs/workpacks/H1", ".github/workflows"):
        shutil.copytree(ROOT / relative, target / relative, dirs_exist_ok=True)
    for relative, transform in changes.items():
        path = target / relative
        path.write_text(transform(path.read_text(encoding="utf-8")), encoding="utf-8")
    env = dict(os.environ, H1_GATE_VERIFY_ROOT=str(target))
    return env


# ----- static controls ---------------------------------------------------------------------------------------------

def static_controls():
    reconciliation = ROOT / "Docs/evidence/WP-H1-GATE/RECONCILIATION.json"
    expect_green("reconciliation-baseline", ["reconcile"])
    expect_green("workflow-baseline", ["workflow"])
    data = json.loads(reconciliation.read_text(encoding="utf-8"))

    def reconcile_mutation(label, mutate, code):
        clone = copy.deepcopy(data)
        mutate(clone)
        path = Path(tempfile.mkdtemp()) / "RECONCILIATION.json"
        path.write_text(json.dumps(clone), encoding="utf-8")
        expect_red(label, "C2", ["reconcile", "--file", str(path)], code)

    reconcile_mutation("residual-row-omitted", lambda d: d["residuals"].pop(37), "RECONCILE.inherited-residual-omitted")
    reconcile_mutation("h1-11-owner-residual-omitted", lambda d: d.__setitem__("residuals", [r for r in d["residuals"] if not r.get("externalSource")]),
                       "RECONCILE.h1-11-owner-residual-omitted")
    reconcile_mutation("dependency-omitted", lambda d: d["dependencies"].pop(10), "RECONCILE.dependency-omitted")
    reconcile_mutation("gate-stage-omitted", lambda d: d["stages"].pop(12), "RECONCILE.gate-stage-omitted")
    reconcile_mutation("residual-unclassified", lambda d: d["residuals"][5].__setitem__("disposition", "IGNORED"), "RECONCILE.row-unclassified")
    # A new inherited residual that appears upstream must be reconciled; the universe is discovered, not declared.
    env = mirror_root({"Docs/evidence/WP-H1-10/RESIDUAL_RISK.md": lambda text: text + "- A newly declared H1-10 residual.\n"})
    expect_red("new-upstream-residual-unreconciled", "C2", ["reconcile", "--file", str(reconciliation)], "RECONCILE.inherited-residual-omitted", env)

    # C6: the stage universe comes from the accepted gate contract. A stage without a Gate oracle fails closed,
    # and removing stage rows from the reconciliation cannot shrink the declared universe.
    env = mirror_root({"Docs/engineering/H1_UNITY_PARITY_GATE.md":
                       lambda text: text.replace(
                           "\n## Parity tuple", "18. an additional deterministic stage added upstream;\n\n## Parity tuple", 1)})
    expect_red("stage-universe-grows-without-oracle", "C6", ["reconcile", "--file", str(reconciliation)], "RECONCILE.gate-stage-omitted", env)
    env = mirror_root({"Docs/architecture/ADR-H1-004-PUBLIC-EDITOR-EXECUTION-SEAM.md":
                       lambda text: text.replace("| H1-10 | project checkpoint", "| H1-10 | project checkpoint and extra role", 1)})
    fake = Path(tempfile.mkdtemp())
    expect_red("adr-role-table-changed", "C6", ["evidence", "--dir", str(fake), "--candidate-sha", "0" * 40], "UNIVERSE.adr-role-table-changed", env)

    # C7 (workflow half): a private Unity path added to the gate workflow is rejected even if everything else stays.
    workflow = (ROOT / ".github/workflows/h1-gate-validation.yml").read_text(encoding="utf-8")
    for label, injected, code in [
        ("gameci-test-helper", '\n      - name: private\n        run: "${PINNED_GAMECI_BIN}" test --docker Unity/ArkusUnity --testPlatforms=editmode\n',
         "WORKFLOW.private-unity-path.unity-test-helper"),
        ("direct-entry-point", '\n      - name: private\n        run: docker run image /opt/unity/Editor/Unity -batchmode -executeMethod Arkus.H1.Editor.H1EditorWorker.Run\n',
         "WORKFLOW.private-unity-path.direct-editor-entry-point"),
        ("predecessor-private-stage", '\n      - name: private\n        run: bash scripts/h1-11-unity-stage.sh baseline StageA\n',
         "WORKFLOW.private-unity-path.predecessor-private-stage"),
    ]:
        env = mirror_root({".github/workflows/h1-gate-validation.yml": lambda text, extra=injected: text + extra})
        expect_red(label, "C7", ["workflow"], code, env)
    # C1 (workflow half): the real MCP execution removed while its step name/declaration stays.
    env = mirror_root({".github/workflows/h1-gate-validation.yml":
                       lambda text: text.replace("h1-gate-scenario.py scenario --transport mcp", "true # scenario declared")})
    expect_red("mcp-execution-removed", "C1", ["workflow"], "WORKFLOW.stage-not-executed.mcp-scenario", env)
    env = mirror_root({".github/workflows/h1-gate-validation.yml":
                       lambda text: text.replace("dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj", "echo dotnet-test-declared")})
    expect_red("remote-validation-removed", "C1", ["workflow"], "WORKFLOW.stage-not-executed.S02-remote-validation", env)

    # C5 on synthetic trial artifacts: proves verifier sensitivity before the real trial exists.
    synthetic_trial_controls()


def synthetic_trial(directory, candidate, mutate=None):
    events = [
        {"event": "discovery", "canonicalKeys": ["authoring.change.apply@1.0", "unity.host.projection.materialize@1.0",
                                                 "unity.host.projection.observe@1.0", "unity.host.checkpoint.capture@1.0",
                                                 "unity.host.checkpoint.restore@1.0"],
         "offeredToolNames": ["t_apply", "t_materialize", "t_observe", "t_capture", "t_restore"]},
        {"event": "model-message", "toolCalls": [{"id": f"c{i}"} for i in range(6)]},
    ]
    for i, (name, key) in enumerate([("t_apply", "authoring.change.apply@1.0"), ("t_materialize", "unity.host.projection.materialize@1.0"),
                                     ("t_materialize", "unity.host.projection.materialize@1.0"), ("t_observe", "unity.host.projection.observe@1.0"),
                                     ("t_capture", "unity.host.checkpoint.capture@1.0"), ("t_restore", "unity.host.checkpoint.restore@1.0")]):
        events.append({"event": "tool-call", "callId": f"c{i}", "toolName": name, "canonicalKey": key})
        events.append({"event": "tool-result", "callId": f"c{i}", "status": "error" if i == 1 else "success",
                       "machineCode": "projection.source-missing" if i == 1 else None})
    record = {"candidateSha": candidate, "transport": "MCP_STDIO", "implementationSourceAccess": "NONE", "verdict": "PASS",
              "final": {"canonicalHash": "a" * 64, "activeGenerationId": "g", "graphDigest": "b" * 64}}
    if mutate:
        mutate(events, record)
    transcript = directory / "trial-transcript.jsonl"
    write_jsonl(transcript, events)
    record["transcriptSha256"] = hashlib.sha256(transcript.read_bytes()).hexdigest()
    (directory / "trial-record.json").write_text(json.dumps(record), encoding="utf-8")


def synthetic_trial_controls():
    candidate = "c" * 40
    base = Path(tempfile.mkdtemp())
    synthetic_trial(base, candidate)
    expect_green("synthetic-trial-baseline", ["trial", "--dir", str(base), "--candidate-sha", candidate])
    other = Path(tempfile.mkdtemp())
    synthetic_trial(other, "d" * 40)
    expect_red("trial-sha-mismatch", "C5", ["trial", "--dir", str(other), "--candidate-sha", candidate], "TRIAL.sha-mismatch")

    def hidden(events, record):
        events.append({"event": "tool-call", "callId": "c0", "toolName": "private_unity_menu", "canonicalKey": "arkus.private.menu@1.0"})
    hidden_dir = Path(tempfile.mkdtemp())
    synthetic_trial(hidden_dir, candidate, hidden)
    expect_red("trial-hidden-private-call", "C5", ["trial", "--dir", str(hidden_dir), "--candidate-sha", candidate], "TRIAL.hidden-or-private-call")

    def injected(events, record):
        events.append({"event": "tool-call", "callId": "worker-supplied", "toolName": "t_apply", "canonicalKey": "authoring.change.apply@1.0"})
    injected_dir = Path(tempfile.mkdtemp())
    synthetic_trial(injected_dir, candidate, injected)
    expect_red("trial-worker-supplied-call", "C5", ["trial", "--dir", str(injected_dir), "--candidate-sha", candidate], "TRIAL.call-not-issued-by-model")

    tampered = Path(tempfile.mkdtemp())
    synthetic_trial(tampered, candidate)
    with (tampered / "trial-transcript.jsonl").open("a", encoding="utf-8") as handle:
        handle.write('{"event":"note"}\n')
    expect_red("trial-artifact-tampered", "C5", ["trial", "--dir", str(tampered), "--candidate-sha", candidate], "TRIAL.transcript-digest-mismatch")


# ----- effective evidence controls ---------------------------------------------------------------------------------

def evidence_controls(directory, candidate):
    directory = Path(directory)
    expect_green("evidence-baseline", ["evidence", "--dir", str(directory), "--candidate-sha", candidate])

    def mutate_transcript(transport, edit):
        def apply(target):
            path = target / f"transcript-{transport}.jsonl"
            write_jsonl(path, edit(read_jsonl(path)))
        return apply

    def mutate_ledger(transport, edit):
        def apply(target):
            path = target / f"ledger-{transport}.json"
            path.write_text(json.dumps(edit(json.loads(path.read_text(encoding="utf-8")))), encoding="utf-8")
        return apply

    def control(label, klass, mutation, code):
        target = mutated_copy(directory, mutation)
        expect_red(label, klass, ["evidence", "--dir", str(target), "--candidate-sha", candidate], code)
        shutil.rmtree(target, ignore_errors=True)

    # C1: the second same-input materialization really executed; delete its frames, keep the stage declared in facts.
    control("second-materialization-removed", "C1",
            mutate_transcript("reference", lambda rs: [r for r in rs if not (r.get("stage") == "S11" and r.get("capability") == "unity.host.projection.materialize@1.0")]),
            "S11.reference.second-materialization-not-executed")
    control("stale-conflict-removed", "C1",
            mutate_transcript("mcp", lambda rs: [r for r in rs if r.get("label") != "plan:stale-proposal"]),
            "S13.mcp.stale-conflict-not-forced")

    # C3: canonical recovery reconstructed from Unity-side data (an authoring write in the fresh process before
    # restore) instead of the checkpoint's H0 snapshot import.
    def unity_as_truth(records):
        restore = next(r for r in records if r.get("capability") == "unity.host.checkpoint.restore@1.0" and r.get("session") == "reference-b")
        forged = dict(restore, seq=restore["seq"] - 0.5, capability="authoring.change.apply@1.0", label="apply:from-unity-observation",
                      arguments={"operations": [{"kind": "put-object", "id": "river.edge"}]}, outcome={"status": "success", "result": {}})
        return sorted(records + [forged], key=lambda r: r["seq"])
    control("unity-observation-as-canonical-recovery", "C3", mutate_transcript("reference", unity_as_truth),
            "S14.reference.canonical-recovered-outside-checkpoint")

    def lineage(records):
        for r in records:
            if r.get("capability") == "unity.host.checkpoint.restore@1.0" and r["outcome"].get("status") == "success":
                r["outcome"]["result"]["lineageDisposition"] = "reconstructed-from-unity"
        return records
    control("restore-not-snapshot-import", "C3", mutate_transcript("reference", lineage), "S14.reference.restore-not-h0-snapshot-import")

    # C4: the normalized parity comparison after rebuild is skipped; the stage-16 Editor capture remains.
    control("parity-skipped-capture-remains", "C4",
            mutate_transcript("reference", lambda rs: [r for r in rs if not (r.get("session") == "reference-b" and r.get("stage") == "S14" and
                                                                              r.get("capability") in ("unity.host.projection.observe@1.0", "unity.host.checkpoint.capture@1.0"))]),
            "S14.reference.rebuild-graph-digest")

    # C6: the MCP half of the universe disappears (the comparison artifact is removed too).
    def shrink(target):
        (target / "transcript-mcp.jsonl").unlink()
        (target / "transport-equivalence.json").unlink()
    control("mcp-universe-removed", "C6", shrink, "EVIDENCE.mcp.transcript-or-ledger-missing")

    # C7: a required capability absent from composed discovery while the scenario still ran green.
    def undiscovered(records):
        for r in records:
            if r.get("event") == "discovery" and r.get("session") == "reference-a":
                r["keys"] = [k for k in r["keys"] if k != "unity.host.projection.materialize@1.0"]
        return records
    control("materialize-absent-from-discovery", "C7", mutate_transcript("reference", undiscovered), "S04.reference.required-capability-absent")
    control("private-unity-launch-in-ledger", "C7",
            mutate_ledger("reference", lambda rows: rows + [dict(rows[0], invocationId="h1u-private", capability="arkus.private.editor-menu@1.0")]),
            "LEDGER.reference.non-public-unity-capability")
    control("unaccounted-unity-launch", "C7",
            mutate_ledger("mcp", lambda rows: rows + [dict(rows[-1], invocationId="h1u-extra")]),
            "LEDGER.mcp.unity-launch-not-reconciled")


def trial_controls(directory, candidate):
    directory = Path(directory)
    expect_green("trial-baseline", ["trial", "--dir", str(directory), "--candidate-sha", candidate])

    def control(label, mutation, code):
        target = mutated_copy(directory, mutation)
        expect_red(label, "C5", ["trial", "--dir", str(target), "--candidate-sha", candidate], code)
        shutil.rmtree(target, ignore_errors=True)

    def other_sha(target):
        record = json.loads((target / "trial-record.json").read_text(encoding="utf-8"))
        record["candidateSha"] = "0" * 40
        (target / "trial-record.json").write_text(json.dumps(record), encoding="utf-8")
    control("real-trial-sha-mismatch", other_sha, "TRIAL.sha-mismatch")

    def hidden_call(target):
        path = target / "trial-transcript.jsonl"
        records = read_jsonl(path)
        issued = next(c["id"] for r in records if r.get("event") == "model-message" for c in r.get("toolCalls", []))
        records.append({"event": "tool-call", "callId": issued, "toolName": "arkus_private_editor_menu", "canonicalKey": "arkus.private.menu@1.0"})
        write_jsonl(path, records)
        record = json.loads((target / "trial-record.json").read_text(encoding="utf-8"))
        record["transcriptSha256"] = hashlib.sha256(path.read_bytes()).hexdigest()
        (target / "trial-record.json").write_text(json.dumps(record), encoding="utf-8")
    control("real-trial-hidden-call", hidden_call, "TRIAL.hidden-or-private-call")


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    sub = parser.add_subparsers(dest="command", required=True)
    sub.add_parser("static")
    e = sub.add_parser("evidence"); e.add_argument("--dir", required=True); e.add_argument("--candidate-sha", required=True)
    t = sub.add_parser("trial"); t.add_argument("--dir", required=True); t.add_argument("--candidate-sha", required=True)
    args = parser.parse_args()
    if args.command == "static":
        static_controls()
    elif args.command == "evidence":
        evidence_controls(args.dir, args.candidate_sha)
    else:
        trial_controls(args.dir, args.candidate_sha)
    finish(args.command)


if __name__ == "__main__":
    main()
