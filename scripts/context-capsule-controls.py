#!/usr/bin/env python3
"""Independent defect-injection and adoption-wiring controls for Context Capsule v1.

The material IDs below are a test oracle, not a production semantic registry. The
production validator must not let a capsule define the universe used to prove its
own completeness. The wiring checks likewise protect discoverability of the
accepted capsule path without making test prose a semantic source of truth.
"""
from __future__ import annotations

import copy
import hashlib
import importlib.util
import json
from pathlib import Path
import tempfile

ROOT = Path(__file__).resolve().parents[1]
CHECKER_PATH = ROOT / "scripts" / "context-capsule-check.py"
spec = importlib.util.spec_from_file_location("context_capsule_check", CHECKER_PATH)
if spec is None or spec.loader is None:
    raise RuntimeError("could not load context-capsule-check.py")
checker = importlib.util.module_from_spec(spec)
spec.loader.exec_module(checker)


def blob_sha(data: bytes) -> str:
    return hashlib.sha1(f"blob {len(data)}\0".encode() + data).hexdigest()


def assert_required_statement_ids(capsule: dict, *, exports: set[str], exclusions: set[str]) -> None:
    actual_exports = {row.get("id") for row in capsule.get("exported_guarantees", []) if isinstance(row, dict)}
    actual_exclusions = {row.get("id") for row in capsule.get("exclusions_nonclaims", []) if isinstance(row, dict)}
    missing_exports = sorted(exports - actual_exports)
    missing_exclusions = sorted(exclusions - actual_exclusions)
    if missing_exports:
        raise checker.CapsuleError(f"independent control: missing required exported guarantee(s): {missing_exports}")
    if missing_exclusions:
        raise checker.CapsuleError(f"independent control: missing required exclusion(s): {missing_exclusions}")


def expect_failure(fn, needle: str) -> None:
    try:
        fn()
    except checker.CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected failure containing {needle!r}")


def require_text(path: Path, needles: list[str]) -> None:
    try:
        text = path.read_text(encoding="utf-8")
    except FileNotFoundError as exc:
        raise checker.CapsuleError(f"adoption wiring missing file: {path.relative_to(ROOT)}") from exc
    for needle in needles:
        if needle not in text:
            raise checker.CapsuleError(
                f"adoption wiring missing {needle!r} in {path.relative_to(ROOT)}"
            )


def validate_adoption_wiring(repo_root: Path) -> None:
    profile_path = repo_root / "Docs/engineering/context-bootstrap-profiles.json"
    try:
        profiles = json.loads(profile_path.read_text(encoding="utf-8"))
    except (FileNotFoundError, json.JSONDecodeError) as exc:
        raise checker.CapsuleError("adoption wiring cannot read context-bootstrap-profiles.json") from exc
    if profiles.get("capsule_protocol") != "Docs/engineering/CONTEXT_CAPSULE_V1.md":
        raise checker.CapsuleError("bootstrap profile root must discover CONTEXT_CAPSULE_V1.md")
    role_map = profiles.get("profiles")
    if not isinstance(role_map, dict):
        raise checker.CapsuleError("bootstrap profiles missing profiles object")
    required_conditional = {
        "worker": "accepted_contract_capsules",
        "repair_worker": "accepted_contract_capsules",
        "reviewer": "accepted_contract_capsules",
        "docsync": "accepted_pa_result_after_ctx02",
    }
    for role, key in required_conditional.items():
        role_data = role_map.get(role)
        conditional = role_data.get("conditional_reads") if isinstance(role_data, dict) else None
        if not isinstance(conditional, dict) or key not in conditional:
            raise checker.CapsuleError(f"bootstrap profile {role} no longer discovers capsule path {key}")

    require_text(repo_root / "Docs/engineering/CONTEXT_BOOTSTRAP_V1.md", ["Docs/engineering/CONTEXT_CAPSULE_V1.md", "WP-CTX-02"])
    require_text(repo_root / "Docs/engineering/WORKER_REVIEW_PROTOCOL.md", ["Version: 1.9", "CONTEXT_CAPSULE_V1.md", "validated capsule"])
    require_text(repo_root / "AGENTS.md", ["CONTEXT_CAPSULE_V1.md", "WP-CTX-02"])
    for relative in (
        ".agents/skills/implement-workpack/SKILL.md",
        ".agents/skills/repair-workpack/SKILL.md",
        ".agents/skills/validate-workpack/SKILL.md",
        ".agents/skills/update-handoff/SKILL.md",
    ):
        require_text(repo_root / relative, ["CONTEXT_CAPSULE_V1.md"])


def run() -> None:
    validate_adoption_wiring(ROOT)

    with tempfile.TemporaryDirectory() as td:
        repo = Path(td)
        (repo / "Docs/workpacks/PA").mkdir(parents=True)
        (repo / "Docs/research/living-world/results").mkdir(parents=True)

        wp = repo / "Docs/workpacks/PA/WP-PA-03.md"
        result = repo / "Docs/research/living-world/results/PA-03.md"
        wp.write_text(
            "# WP-PA-03\n\nStatus: **COMPLETE**\n"
            "Accepted candidate: `1111111111111111111111111111111111111111`\n"
            "Independent review: **PASS**, review `12345`\n"
            "Merged: PR `#1`, merge commit `2222222222222222222222222222222222222222`\n",
            encoding="utf-8",
        )
        result.write_text(
            "# PA-03\n\n## 4. Minimal relationship vocabulary recommendation\n\n"
            "| Relationship/mechanism | Status | Juego2 recommendation |\n"
            "|---|---|---|\n"
            "| directed trust | **ADOPT** | material decision input |\n"
            "| default global/N-hop social traversal to discover targets | **REJECT** | inherited PA-02 bounded-discovery intent |\n"
            "\n## 5. Next\n",
            encoding="utf-8",
        )
        source = {
            "path": "Docs/research/living-world/results/PA-03.md",
            "git_blob_sha": blob_sha(result.read_bytes()),
        }
        capsule = {
            "schema": checker.SCHEMA,
            "authority": checker.AUTHORITY,
            "capsule_id": "WP-PA-03",
            "track": "PA",
            "content_mode": "structured_disposition",
            "accepted_identity": {
                "reviewed_candidate_sha": "1" * 40,
                "merge_sha": "2" * 40,
                "review_id": "12345",
            },
            "identity_source": {
                "path": "Docs/workpacks/PA/WP-PA-03.md",
                "git_blob_sha": blob_sha(wp.read_bytes()),
            },
            "authoritative_sources": [source],
            "exported_guarantees": [
                {"id": "directed-affect", "statement": "A->B is independent from B->A."},
                {"id": "behavioral-effect", "statement": "Relationship state can change action or target choice."},
            ],
            "exclusions_nonclaims": [
                {"id": "no-global-n-hop", "statement": "Default global/N-hop target discovery is rejected."},
                {"id": "no-implicit-symmetry", "statement": "Actor stance is not implicitly symmetric."},
            ],
            "reopen_conditions": ["concrete contradictory evidence"],
            "escalate_if": ["material exact source semantics are needed"],
            "disposition_source": {
                **source,
                "section": "## 4. Minimal relationship vocabulary recommendation",
                "key_column": 0,
                "status_column": 1,
            },
            "dispositions": [
                {"source_key": "directed trust", "status": "ADOPT"},
                {"source_key": "default global/N-hop social traversal to discover targets", "status": "REJECT"},
            ],
            "directional_semantics": [
                {"id": "trust", "forward": "trust(A -> B)", "reverse": "trust(B -> A)", "must_remain_distinct": True}
            ],
            "mandatory_source_reads": [],
        }

        # Positive control: a structurally valid capsule that also satisfies an
        # independent material-claim oracle.
        checker.validate_capsule(repo, capsule)
        expected_exports = {"directed-affect", "behavioral-effect"}
        expected_exclusions = {"no-global-n-hop", "no-implicit-symmetry"}
        assert_required_statement_ids(capsule, exports=expected_exports, exclusions=expected_exclusions)

        # Required CTX-02 defect: omit ONE material positive guarantee while
        # another remains. Basic shape still passes; the independent oracle must RED.
        one_positive_missing = copy.deepcopy(capsule)
        one_positive_missing["exported_guarantees"] = [one_positive_missing["exported_guarantees"][0]]
        checker.validate_capsule(repo, one_positive_missing)
        expect_failure(
            lambda: assert_required_statement_ids(one_positive_missing, exports=expected_exports, exclusions=expected_exclusions),
            "missing required exported guarantee",
        )

        # Symmetric negative counterpart: omit ONE material exclusion while
        # another remains. This protects loss of a predecessor-carrying REJECT.
        one_exclusion_missing = copy.deepcopy(capsule)
        one_exclusion_missing["exclusions_nonclaims"] = [one_exclusion_missing["exclusions_nonclaims"][1]]
        checker.validate_capsule(repo, one_exclusion_missing)
        expect_failure(
            lambda: assert_required_statement_ids(one_exclusion_missing, exports=expected_exports, exclusions=expected_exclusions),
            "missing required exclusion",
        )

    print("context-capsule independent controls: PASS")


if __name__ == "__main__":
    run()
