#!/usr/bin/env python3
"""Test-only semantic controls for PA-01/02 in CTX-02's representative PA chain.

PA-03's richer representative controls live in context-capsule-controls.py. This
file closes the same statement-substitution class for the other two accepted PA
capsules consumed by the representative PA-04 start surface. It is not imported
by the production validator and is not a production semantic registry.
"""
from __future__ import annotations

import copy
import importlib.util
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CHECKER_PATH = ROOT / "scripts" / "context-capsule-check.py"
spec = importlib.util.spec_from_file_location("context_capsule_check", CHECKER_PATH)
if spec is None or spec.loader is None:
    raise RuntimeError("could not load context-capsule-check.py")
checker = importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)

ORACLE = {
    "WP-PA-01": {
        "exports": {
            "schedule-is-intent": (
                "Schedule owns normal semantic intent and is distinct from route/animation/concrete realization.",
                "PA-01 / Required semantic invariants 1-2",
            ),
            "re-evaluate-after-interruption": (
                "After interruption/release the actor re-evaluates current time/context rather than blindly resuming stale execution.",
                "PA-01 / Required semantic invariant 3",
            ),
            "expected-vs-actual": (
                "Expected routine and actual state remain distinct; deviations do not rewrite authored normal intent.",
                "PA-01 / Required semantic invariant 5",
            ),
            "routine-not-agency": (
                "Routine does not prove autonomous initiative; PA-02 owns actor-originated choice.",
                "PA-01 / Required semantic invariant 6",
            ),
            "offscreen-causal-continuity": (
                "Off-screen fidelity may be cheaper but cannot discard causal meaning required for coherent re-entry.",
                "PA-01 / Required semantic invariant 8",
            ),
        },
        "exclusions": {
            "no-exact-hidden-route-requirement": (
                "Exact physical off-screen path simulation is rejected as a PA-01 requirement.",
                "PA-01 / DL-11",
            ),
            "no-unique-day-screenplay": (
                "A unique executable 24h script per visible citizen is rejected.",
                "PA-01 / DL-12",
            ),
            "no-donor-runtime-authority": (
                "Donor M9/M10/WorldState/schema/API ownership is rejected as authority.",
                "PA-01 / DL-14",
            ),
        },
        "reopen": [
            "Concrete accepted evidence falsifies a PA-01 semantic invariant or shows it inapplicable to the effective consumer path."
        ],
        "escalate": [
            "The consumer needs the exact meaning/recommendation behind a disposition row.",
            "A later accepted result or live state contradicts this capsule.",
            "A material PA-01 detail is absent from exported guarantees.",
        ],
    },
    "WP-PA-02": {
        "exports": {
            "actor-originated-agency": (
                "Autonomous proof requires actor-owned pressure/problem/opportunity; quest/director/timetable cannot preselect the meaningful action.",
                "PA-02 / AG-01",
            ),
            "bounded-discovery": (
                "Target/action discovery is scoped before scoring; unrelated global population growth cannot become the discovery mechanism.",
                "PA-02 / Bounded discovery requirements",
            ),
            "receiver-owns-response": (
                "Where a receiver has a meaningful choice, the receiver owns its response decision.",
                "PA-02 / Social action ownership",
            ),
            "explainable-deterministic-trace": (
                "Important decisions expose explainable deterministic/seedable semantic reasons and alternatives.",
                "PA-02 / AG-04",
            ),
            "bounded-replanning": (
                "Commitment/inertia/cooldowns and explicit failure prevent global tick-by-tick thrashing.",
                "PA-02 / AG-06",
            ),
        },
        "exclusions": {
            "no-universal-chooser": (
                "PA-02 rejects one universal Utility AI/GOAP/rules/planner architecture as a conclusion.",
                "PA-02 / AG-12",
            ),
            "no-deep-psych-prerequisite": (
                "Deep psychology is rejected as a prerequisite without material behavioral value.",
                "PA-02 / AG-13",
            ),
            "no-llm-state-authority": (
                "LLM output cannot be the hidden source of canonical decision/state authority.",
                "PA-02 / AG-14",
            ),
            "no-global-scan-fallback": (
                "Failure to find a bounded candidate cannot fall back to accidental world-wide discovery.",
                "PA-02 / Bounded discovery requirement 6",
            ),
        },
        "reopen": [
            "Concrete accepted evidence falsifies the PA-02 bounded/explainable agency boundary or proves it inapplicable to the effective consumer path."
        ],
        "escalate": [
            "The exact provider/scope semantics of a decision matter.",
            "A later consumer appears to require a global scan or universal decision authority.",
            "Live accepted identity/source bytes contradict the capsule.",
        ],
    },
}


def statement_map(capsule: dict, field: str) -> dict[str, tuple[str, str | None]]:
    return {
        row["id"]: (row.get("statement"), row.get("source_pointer"))
        for row in capsule.get(field, [])
        if isinstance(row, dict) and isinstance(row.get("id"), str)
    }


def assert_semantics(capsule: dict) -> None:
    cid = capsule.get("capsule_id")
    expected = ORACLE.get(cid)
    if expected is None:
        raise checker.CapsuleError(f"PA semantic test oracle has no fixture for {cid}")
    if statement_map(capsule, "exported_guarantees") != expected["exports"]:
        raise checker.CapsuleError(f"PA semantic test oracle: {cid} exported_guarantees mismatch")
    if statement_map(capsule, "exclusions_nonclaims") != expected["exclusions"]:
        raise checker.CapsuleError(f"PA semantic test oracle: {cid} exclusions_nonclaims mismatch")
    if capsule.get("reopen_conditions") != expected["reopen"]:
        raise checker.CapsuleError(f"PA semantic test oracle: {cid} reopen_conditions mismatch")
    if capsule.get("escalate_if") != expected["escalate"]:
        raise checker.CapsuleError(f"PA semantic test oracle: {cid} escalate_if mismatch")


def expect_semantic_red(capsule: dict, needle: str) -> None:
    # The production checker should accept structurally/source-valid invented prose;
    # the independent semantic fixture must be the causal RED.
    checker.validate_capsule(ROOT, capsule)
    try:
        assert_semantics(capsule)
    except checker.CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected semantic RED containing {needle!r}")


def load(cid: str) -> dict:
    return checker.load_json(ROOT / f"Docs/engineering/context-capsules/{cid}.json")


def run() -> None:
    pa01 = load("WP-PA-01")
    pa02 = load("WP-PA-02")

    for capsule in (pa01, pa02):
        checker.validate_capsule(ROOT, capsule)
        assert_semantics(capsule)

    # Same-ID/same-pointer/same-fingerprint positive guarantee inversion.
    pa01_invented = copy.deepcopy(pa01)
    row = next(r for r in pa01_invented["exported_guarantees"] if r["id"] == "schedule-is-intent")
    pointer = row["source_pointer"]
    row["statement"] = "Schedule directly owns exact pathfinding, animation and concrete realization."
    assert row["source_pointer"] == pointer
    expect_semantic_red(pa01_invented, "exported_guarantees mismatch")

    # Symmetric exclusion inversion on PA-02; source bindings remain untouched.
    pa02_invented = copy.deepcopy(pa02)
    row = next(r for r in pa02_invented["exclusions_nonclaims"] if r["id"] == "no-global-scan-fallback")
    pointer = row["source_pointer"]
    row["statement"] = "When bounded discovery finds nothing, world-wide target discovery is the required fallback."
    assert row["source_pointer"] == pointer
    expect_semantic_red(pa02_invented, "exclusions_nonclaims mismatch")

    # Keep the same substitution class closed for PA-01/02 reopen/escalation prose.
    pa01_reopen = copy.deepcopy(pa01)
    pa01_reopen["reopen_conditions"][0] = "Never reopen PA-01 even if accepted evidence falsifies its invariant."
    expect_semantic_red(pa01_reopen, "reopen_conditions mismatch")

    pa02_escalate = copy.deepcopy(pa02)
    pa02_escalate["escalate_if"][1] = "Do not escalate when a consumer requires a global scan."
    expect_semantic_red(pa02_escalate, "escalate_if mismatch")

    print("context-capsule PA-01/02 semantic controls: PASS")


if __name__ == "__main__":
    run()
