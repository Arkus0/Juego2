#!/usr/bin/env python3
"""Generate the derivable part of the canonical Worker -> Reviewer handoff.

This tool does not decide CLEAN, PASS/FAIL, ownership or semantic readiness.  It
only removes error-prone transcription for values already fixed by repository
state and the Worker lifecycle.
"""
from __future__ import annotations

import argparse
import re
import sys
from pathlib import Path

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)


def evidence_root(root: Path, wp: str) -> str:
    candidates = [f"Docs/evidence/{wp}", f"Docs/evidence/{wp.removeprefix('WP-')}"]
    existing = [p for p in candidates if (root / p).is_dir()]
    if len(existing) != 1:
        raise ValueError(f"expected exactly one evidence root for {wp}, found {existing}")
    return existing[0]


def contract_path(root: Path, wp: str) -> str:
    m = re.fullmatch(r"WP-([A-Z0-9]+)-.+", wp)
    if not m:
        raise ValueError(f"invalid WP id: {wp}")
    path = f"Docs/workpacks/{m.group(1)}/{wp}.md"
    if not (root / path).is_file():
        raise ValueError(f"contract path missing: {path}")
    return path


def require_sha(name: str, value: str) -> str:
    value = value.lower()
    if not SHA_RE.fullmatch(value):
        raise ValueError(f"{name} must be an exact 40-character SHA")
    return value


def derive(root: Path, wp: str, baseline: str, head: str, worker: str, history: str,
           fail_cycle: int, transfer: str, reviewed: str) -> str:
    baseline = require_sha("baseline", baseline)
    head = require_sha("head", head)
    transfer = transfer.upper() if transfer.upper() == "NONE" else require_sha("transfer", transfer)
    reviewed = reviewed.upper() if reviewed.upper() == "NONE" else require_sha("reviewed", reviewed)
    contract = contract_path(root, wp)
    evidence = evidence_root(root, wp)
    predecessor = f"{evidence}/PREDECESSOR_CONTRACT_CHECK.md"
    prereview = f"{evidence}/WORKER_PRE_REVIEW.md"
    if not (root / predecessor).is_file():
        raise ValueError(f"predecessor check missing: {predecessor}")
    if not (root / prereview).is_file():
        raise ValueError(f"pre-review evidence missing: {prereview}")
    text = (root / prereview).read_text(encoding="utf-8")
    if not re.search(r"WORKER_PRE_REVIEW:\s*CLEAN", text, re.I):
        raise ValueError("pre-review evidence is not CLEAN; review metadata cannot be derived yet")
    return "\n".join([
        f"WP: {wp}",
        f"Contract: {contract}",
        f"Baseline SHA: {baseline}",
        f"Active Worker: {worker}",
        "Worker state: FROZEN_FOR_REVIEW",
        f"Worker history: {history}",
        f"Transfer SHA: {transfer}",
        f"Predecessor contract check: {predecessor}",
        f"Candidate HEAD SHA: {head}",
        "Worker pre-review: CLEAN",
        f"Worker pre-review evidence: {prereview}",
        f"Frozen candidate SHA: {head}",
        "Branch frozen: YES",
        "Worker verdict: IN_REVIEW",
        "Reviewer verdict: PENDING",
        f"Reviewed candidate SHA: {reviewed}",
        f"Evidence: {evidence}/",
        f"fail_cycle: {fail_cycle}",
    ]) + "\n"


def self_test():
    import tempfile
    sha = "a" * 40
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        (root / "Docs/workpacks/CTX").mkdir(parents=True)
        (root / "Docs/workpacks/CTX/WP-CTX-03.md").write_text("Depends on: WP-CTX-02\n", encoding="utf-8")
        (root / "Docs/evidence/CTX-03").mkdir(parents=True)
        (root / "Docs/evidence/CTX-03/PREDECESSOR_CONTRACT_CHECK.md").write_text("PREDECESSOR_CONTRACT_CHECK\n", encoding="utf-8")
        (root / "Docs/evidence/CTX-03/WORKER_PRE_REVIEW.md").write_text("WORKER_PRE_REVIEW: CLEAN\n", encoding="utf-8")
        out = derive(root, "WP-CTX-03", sha, sha, "worker", "worker", 0, "NONE", "NONE")
        assert f"Candidate HEAD SHA: {sha}" in out and "Worker state: FROZEN_FOR_REVIEW" in out
        (root / "Docs/evidence/CTX-03/WORKER_PRE_REVIEW.md").write_text("WORKER_PRE_REVIEW: NOT_READY\n", encoding="utf-8")
        try:
            derive(root, "WP-CTX-03", sha, sha, "worker", "worker", 0, "NONE", "NONE")
            raise AssertionError("NOT_READY unexpectedly generated review metadata")
        except ValueError:
            pass
    print("derive-worker-review-metadata self-test: PASS")


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--wp")
    p.add_argument("--baseline-sha")
    p.add_argument("--head-sha")
    p.add_argument("--worker", default="ChatGPT remote Worker")
    p.add_argument("--worker-history", default="ChatGPT remote Worker")
    p.add_argument("--fail-cycle", type=int, default=0)
    p.add_argument("--transfer-sha", default="NONE")
    p.add_argument("--reviewed-candidate-sha", default="NONE")
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    if not args.wp or not args.baseline_sha or not args.head_sha:
        p.error("--wp, --baseline-sha and --head-sha are required")
    try:
        print(derive(args.repo_root.resolve(), args.wp, args.baseline_sha, args.head_sha,
                     args.worker, args.worker_history, args.fail_cycle,
                     args.transfer_sha, args.reviewed_candidate_sha), end="")
    except ValueError as exc:
        print(f"REVIEW_BLOCKED: {exc}", file=sys.stderr)
        return 21
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
