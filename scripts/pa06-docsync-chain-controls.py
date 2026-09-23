#!/usr/bin/env python3
"""Independent regression for DocSync status -> PA capsule-chain completeness.

This test deliberately uses the capsule checker's workpack-side discovery rather
than DocSync's own index/output, so DocSync cannot define its own completeness.
"""
from __future__ import annotations

import importlib.util
from pathlib import Path
import tempfile

ROOT = Path(__file__).resolve().parents[1]
CHECKER = ROOT / "scripts/context-capsule-check.py"
SPEC = importlib.util.spec_from_file_location("context_capsule_check", CHECKER)
if SPEC is None or SPEC.loader is None:
    raise RuntimeError(f"cannot import {CHECKER}")
mod = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(mod)


def expect_failure(fn, needle: str) -> None:
    try:
        fn()
    except mod.CapsuleError as exc:
        if needle not in str(exc):
            raise AssertionError(f"expected {needle!r}, got {exc!r}") from exc
    else:
        raise AssertionError(f"expected failure containing {needle!r}")


def main() -> int:
    with tempfile.TemporaryDirectory() as td:
        repo = Path(td)
        (repo / "Docs/workpacks/PA").mkdir(parents=True)
        (repo / "Docs/research/living-world/results").mkdir(parents=True)
        wp = repo / "Docs/workpacks/PA/WP-PA-06.md"
        # This is the exact canonical status emitted by docsync.py after Batch B.
        wp.write_text("# WP-PA-06\n\nStatus: COMPLETE\nBlocks: NONE\n", encoding="utf-8")
        index = {
            "coverage_rules": {
                "pa_accepted_result_chain": {
                    "workpack_glob": mod.PA_WORKPACK_GLOB,
                    "result_template": mod.PA_RESULT_TEMPLATE,
                }
            }
        }

        expect_failure(
            lambda: mod.validate_pa_chain(repo, index, {}),
            "accepted PA canonical result missing for COMPLETE workpack(s): WP-PA-06",
        )

        result = repo / "Docs/research/living-world/results/PA-06.md"
        result.write_text(
            "# PA-06\n\n## Canonical disposition table\n\n| ID | Status |\n|---|---|\n| X | ADOPT |\n",
            encoding="utf-8",
        )
        expect_failure(
            lambda: mod.validate_pa_chain(repo, index, {}),
            "accepted PA result-chain coverage gap: WP-PA-06",
        )

        # The old Batch-B rendering must not be treated as canonical COMPLETE by
        # this independent universe; DocSync itself now refuses to produce it.
        wp.write_text("# WP-PA-06\n\nStatus: COMPLETE / ACCEPTED\nBlocks: NONE\n", encoding="utf-8")
        got = mod.validate_pa_chain(repo, index, {})
        if got.get("accepted_pa_results_discovered"):
            raise AssertionError("non-canonical COMPLETE / ACCEPTED leaked into PA accepted universe")

    print("PA06_DOCSYNC_CHAIN_CONTROLS_GREEN")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
