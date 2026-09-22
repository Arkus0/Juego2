#!/usr/bin/env python3
"""Generate only the derivable part of the canonical Worker -> Reviewer handoff.

Non-derivable lineage fields are preserved from the existing canonical PR body;
the tool cannot reset fail_cycle, Worker history, transfer history or baseline.
It does not decide CLEAN, PASS/FAIL, ownership or semantic readiness.

After CTX-03, the final CLEAN record must be durable GitHub metadata created only
after the last repository/evidence byte mutation and the complete pre-review of
the exact resulting HEAD. Keeping that record outside repository bytes avoids a
self-invalidating "commit the CLEAN evidence after reviewing its parent" cycle.
"""
from __future__ import annotations

import argparse
import os
import re
import sys
from pathlib import Path

SHA_RE = re.compile(r"^[0-9a-f]{40}$", re.I)
PRE_REVIEW_URL_RE = re.compile(r"^https://github\.com/[^/]+/[^/]+/pull/\d+#issuecomment-\d+$", re.I)


def field(body: str, name: str) -> str:
    m = re.search(rf"^{re.escape(name)}:\s*`?([^`\r\n]+?)`?\s*$", body, re.M | re.I)
    if not m:
        raise ValueError(f"existing canonical handoff field missing: {name}")
    return m.group(1).strip()


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


def require_pre_review_pointer(value: str) -> str:
    value = value.strip()
    if not PRE_REVIEW_URL_RE.fullmatch(value):
        raise ValueError("final pre-review evidence must be a durable GitHub PR issue-comment URL")
    return value


def preserve_lineage(existing_body: str) -> dict:
    baseline = require_sha("existing Baseline SHA", field(existing_body, "Baseline SHA"))
    worker = field(existing_body, "Active Worker")
    history = field(existing_body, "Worker history")
    transfer_raw = field(existing_body, "Transfer SHA")
    transfer = transfer_raw.upper() if transfer_raw.upper() == "NONE" else require_sha("existing Transfer SHA", transfer_raw)
    reviewed_raw = field(existing_body, "Reviewed candidate SHA")
    reviewed = reviewed_raw.upper() if reviewed_raw.upper() == "NONE" else require_sha("existing Reviewed candidate SHA", reviewed_raw)
    try:
        fail_cycle = int(field(existing_body, "fail_cycle"))
    except ValueError as exc:
        raise ValueError("existing fail_cycle must be an integer") from exc
    if fail_cycle < 0:
        raise ValueError("existing fail_cycle must be non-negative")
    if not worker or not history:
        raise ValueError("existing Worker identity/history must be non-empty")
    return {
        "baseline": baseline,
        "worker": worker,
        "history": history,
        "transfer": transfer,
        "reviewed": reviewed,
        "fail_cycle": fail_cycle,
    }


def derive(root: Path, wp: str, head: str, existing_body: str, pre_review_evidence: str) -> str:
    head = require_sha("head", head)
    pre_review_evidence = require_pre_review_pointer(pre_review_evidence)
    lineage = preserve_lineage(existing_body)
    contract = contract_path(root, wp)
    evidence = evidence_root(root, wp)
    predecessor = f"{evidence}/PREDECESSOR_CONTRACT_CHECK.md"
    if not (root / predecessor).is_file():
        raise ValueError(f"predecessor check missing: {predecessor}")
    return "\n".join([
        f"WP: {wp}",
        f"Contract: {contract}",
        f"Baseline SHA: {lineage['baseline']}",
        f"Active Worker: {lineage['worker']}",
        "Worker state: FROZEN_FOR_REVIEW",
        f"Worker history: {lineage['history']}",
        f"Transfer SHA: {lineage['transfer']}",
        f"Predecessor contract check: {predecessor}",
        f"Candidate HEAD SHA: {head}",
        "Worker pre-review: CLEAN",
        f"Worker pre-review evidence: {pre_review_evidence}",
        f"Frozen candidate SHA: {head}",
        "Branch frozen: YES",
        "Worker verdict: IN_REVIEW",
        "Reviewer verdict: PENDING",
        f"Reviewed candidate SHA: {lineage['reviewed']}",
        f"Evidence: {evidence}/",
        f"fail_cycle: {lineage['fail_cycle']}",
    ]) + "\n"


def example_body(sha: str, fail_cycle: int = 3) -> str:
    return "\n".join([
        "WP: WP-CTX-03",
        f"Baseline SHA: {sha}",
        "Active Worker: Worker A",
        "Worker history: Worker A -> Worker B",
        "Transfer SHA: NONE",
        "Reviewed candidate SHA: NONE",
        f"fail_cycle: {fail_cycle}",
    ])


def self_test():
    import tempfile
    sha = "a" * 40
    pointer = "https://github.com/Arkus0/Juego2/pull/121#issuecomment-123"
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        (root / "Docs/workpacks/CTX").mkdir(parents=True)
        (root / "Docs/workpacks/CTX/WP-CTX-03.md").write_text("Depends on: WP-CTX-02\n", encoding="utf-8")
        (root / "Docs/evidence/CTX-03").mkdir(parents=True)
        (root / "Docs/evidence/CTX-03/PREDECESSOR_CONTRACT_CHECK.md").write_text("PREDECESSOR_CONTRACT_CHECK\n", encoding="utf-8")
        out = derive(root, "WP-CTX-03", sha, example_body(sha, 3), pointer)
        assert f"Candidate HEAD SHA: {sha}" in out and "Worker state: FROZEN_FOR_REVIEW" in out
        assert "fail_cycle: 3" in out and "Worker history: Worker A -> Worker B" in out
        assert f"Worker pre-review evidence: {pointer}" in out

        try:
            derive(root, "WP-CTX-03", sha, example_body(sha), "Docs/evidence/CTX-03/WORKER_PRE_REVIEW.md")
            raise AssertionError("repository-local final CLEAN pointer unexpectedly accepted")
        except ValueError:
            pass
    print("derive-worker-review-metadata self-test: PASS")


def main():
    p = argparse.ArgumentParser()
    p.add_argument("--repo-root", type=Path, default=Path("."))
    p.add_argument("--wp")
    p.add_argument("--head-sha")
    p.add_argument("--pre-review-evidence")
    p.add_argument("--existing-body-file", type=Path)
    p.add_argument("--self-test", action="store_true")
    args = p.parse_args()
    if args.self_test:
        self_test(); return 0
    if not args.wp or not args.head_sha or not args.pre_review_evidence:
        p.error("--wp, --head-sha and --pre-review-evidence are required")
    try:
        if args.existing_body_file:
            existing = args.existing_body_file.read_text(encoding="utf-8")
        else:
            existing = os.environ.get("PR_BODY", "")
        if not existing.strip():
            raise ValueError("existing PR body is required via --existing-body-file or PR_BODY")
        print(derive(args.repo_root.resolve(), args.wp, args.head_sha, existing, args.pre_review_evidence), end="")
    except (OSError, ValueError) as exc:
        print(f"REVIEW_BLOCKED: {exc}", file=sys.stderr)
        return 21
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
