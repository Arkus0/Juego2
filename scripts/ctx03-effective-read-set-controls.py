#!/usr/bin/env python3
"""Independent class controls for CTX-03 canonical effective mandatory-read discovery.

The control creates route authorities and mutations, then challenges the production
oracle in ctx03-dynamic-context-check.py. It does not implement a second discovery
algorithm or maintain a parallel expected universe.
"""
from __future__ import annotations

import importlib.util
import json
import sys
import tempfile
from pathlib import Path

TARGET = Path("scripts/ctx03-dynamic-context-check.py")


def load_target(root: Path):
    spec = importlib.util.spec_from_file_location("ctx03_effective_read_target", root / TARGET)
    if spec is None or spec.loader is None:
        raise RuntimeError("cannot load production effective-read oracle")
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


def write(root: Path, rel: str, text: str) -> None:
    path = root / rel
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


def policy(per_source: int = 80, aggregate: int = 400) -> dict:
    return {"process_envelope": {"dynamic_repository_envelope": {
        "per_source_ceiling_estimate": per_source,
        "aggregate_route_ceiling_estimate": aggregate,
        "policy_revision": 1,
        "rationale": "class-control fixture",
    }}}


def base_profiles() -> dict:
    return {"profiles": {
        "worker": {
            "initial_reads": ["AGENTS.md", "<EXACT_WP>"],
            "conditional_reads": {"direct_dependencies": "accepted predecessor sources"},
            "live_state": [],
        },
        "reviewer": {
            "initial_reads": [
                "AGENTS.md",
                "<EXACT_WP>",
                "<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>",
                "<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>",
            ],
            "conditional_reads": {},
            "live_state": [],
        },
    }}


def paths(report: dict) -> set[str]:
    return {row["path"] for row in report["sources"]}


def run_controls(target) -> list[str]:
    errors: list[str] = []
    with tempfile.TemporaryDirectory() as td:
        root = Path(td)
        write(root, "AGENTS.md", "agents\n")
        write(root, str(target.PROFILES), json.dumps(base_profiles()))

        dep = "Docs/workpacks/T/WP-T-01.md"
        exact = "Docs/workpacks/T/WP-T-02.md"
        exact_required = "Docs/evidence/T-02/required.md"
        dep_verdict = "Docs/evidence/T-01/VERDICT.md"
        write(root, dep, "# dependency\n")
        write(root, dep_verdict, "PASS\n")
        write(root, exact_required, "required\n")
        write(root, exact, f"# route\nDepends on: WP-T-01\nMandatory repository source: {exact_required}\n")

        # 1. Mandatory placeholder in initial_reads is counted.
        r = target.audit_resolved_route(root, policy(), "worker", {"<EXACT_WP>": exact})
        if r["errors"] or exact not in paths(r):
            errors.append(f"initial_reads placeholder was not counted: {r['errors']}")

        # 2. Same known placeholder in another reviewed read surface is counted by production.
        p = base_profiles()
        p["profiles"]["conditional_role"] = {
            "initial_reads": ["AGENTS.md"],
            "conditional_reads": {"direct_dependencies": "<EXACT_WP>"},
            "live_state": [],
        }
        write(root, str(target.PROFILES), json.dumps(p))
        r = target.audit_resolved_route(root, policy(), "conditional_role", {"<EXACT_WP>": exact})
        if r["errors"] or exact not in paths(r):
            errors.append(f"conditional read-surface placeholder was not counted: {r['errors']}")

        # 3. Direct dependency derives contract plus repository-backed accepted context.
        r = target.audit_resolved_route(root, policy(), "worker", {"<EXACT_WP>": exact})
        if r["errors"] or not {dep, dep_verdict} <= paths(r):
            errors.append(f"direct dependency context was not independently derived: {r['errors']}")

        # 4. Reviewer predecessor navigation cannot stop at the dependency contract.
        cap = "Docs/engineering/context-capsules/WP-T-01.json"
        cap_source = "Docs/evidence/T-01/PROOF_MATRIX.md"
        write(root, str(target.CAPSULE_PROTOCOL), "capsule protocol\n")
        write(root, cap_source, "proof\n")
        write(root, cap, json.dumps({
            "identity_source": {"path": dep},
            "authoritative_sources": [{"path": cap_source}],
            "mandatory_source_reads": [],
        }))
        write(root, str(target.CAPSULE_INDEX), json.dumps({"entries": [{"capsule_id": "WP-T-01", "path": cap}]}))
        reviewer_bindings = {
            "<EXACT_WP>": exact,
            "<LIVE_CANONICAL_PR_AND_COMPLETE_DIFF>": "external:pr+diff",
        }
        r = target.audit_resolved_route(root, policy(), "reviewer", reviewer_bindings)
        must = {dep, str(target.CAPSULE_PROTOCOL), str(target.CAPSULE_INDEX), cap, cap_source}
        if r["errors"] or not must <= paths(r):
            errors.append(f"Reviewer predecessor navigation omitted capsule/evidence context: {r['errors']}")

        # 5. Caller omission of the derived predecessor binding cannot shrink the universe.
        without_binding = paths(r)
        with_binding = dict(reviewer_bindings)
        with_binding["<DIRECT_PREDECESSOR_ACCEPTED_EVIDENCE_OR_VALIDATED_CAPSULE_NAVIGATION>"] = [cap]
        r2 = target.audit_resolved_route(root, policy(), "reviewer", with_binding)
        if r2["errors"] or not without_binding <= paths(r2) or not must <= without_binding:
            errors.append(f"caller omission changed/reduced derived predecessor universe: {r2['errors']}")

        # 6. A new role using known surfaces/classes inherits the same production resolver.
        p = json.loads((root / target.PROFILES).read_text(encoding="utf-8"))
        p["profiles"]["future_role"] = {
            "initial_reads": ["<EXACT_WP>"],
            "conditional_reads": {"direct_dependencies": "accepted predecessor sources"},
            "live_state": [],
        }
        write(root, str(target.PROFILES), json.dumps(p))
        r = target.audit_resolved_route(root, policy(), "future_role", {"<EXACT_WP>": exact})
        if r["errors"] or not {exact, dep, cap} <= paths(r):
            errors.append(f"new role did not inherit known resolver semantics: {r['errors']}")

        # 7a. A new slot class fails closed.
        p["profiles"]["future_role"]["initial_reads"].append("<UNREVIEWED_SLOT_CLASS>")
        write(root, str(target.PROFILES), json.dumps(p))
        r = target.audit_resolved_route(root, policy(), "future_role", {"<EXACT_WP>": exact})
        if not any("UNREVIEWED_SLOT_CLASS" in e for e in r["errors"]):
            errors.append("new slot class did not fail closed")

        # 7b. A new read surface fails closed rather than silently escaping discovery.
        p["profiles"]["future_role"]["initial_reads"] = ["<EXACT_WP>"]
        p["profiles"]["future_role"]["supplemental_reads"] = [exact_required]
        write(root, str(target.PROFILES), json.dumps(p))
        r = target.audit_resolved_route(root, policy(), "future_role", {"<EXACT_WP>": exact})
        if not any("unreviewed read surface" in e for e in r["errors"]):
            errors.append("new read surface did not fail closed")

        # 8. Growth of any derived mandatory source beyond the per-source ceiling is RED.
        write(root, str(target.PROFILES), json.dumps(base_profiles()))
        r = target.audit_resolved_route(root, policy(per_source=60, aggregate=1000), "worker", {"<EXACT_WP>": exact})
        if r["errors"]:
            errors.append(f"growth-control baseline is not GREEN: {r['errors']}")
        else:
            baseline = r["aggregate_estimate"]
            write(root, exact_required, "x" * 300)
            over = target.audit_resolved_route(root, policy(per_source=60, aggregate=1000), "worker", {"<EXACT_WP>": exact})
            if not any(exact_required in e and "per-source ceiling" in e for e in over["errors"]):
                errors.append("derived mandatory source growth above ceiling did not turn RED")

            # 9. Growth of a repository file outside the mandatory set stays GREEN.
            write(root, exact_required, "required\n")
            write(root, "Docs/random/not-mandatory.md", "z" * 20000)
            neutral = target.audit_resolved_route(root, policy(per_source=60, aggregate=1000), "worker", {"<EXACT_WP>": exact})
            if neutral["errors"] or neutral["aggregate_estimate"] != baseline:
                errors.append(f"non-mandatory repository growth affected envelope: {neutral['errors']}")

    return errors


def self_test() -> None:
    assert paths({"sources": [{"path": "a"}]}) == {"a"}
    print("ctx03-effective-read-set-controls self-test: PASS")


def main() -> int:
    if "--self-test" in sys.argv:
        self_test()
        return 0
    root = Path(".").resolve()
    try:
        target = load_target(root)
        errors = run_controls(target)
    except Exception as exc:
        print(f"CTX03_EFFECTIVE_READ_SET_CONTROLS: INFRA_ERROR\n{exc}", file=sys.stderr)
        return 23
    if errors:
        print("CTX03_EFFECTIVE_READ_SET_CONTROLS: FAIL", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 20
    print("CTX03_EFFECTIVE_READ_SET_CONTROLS: PASS")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
