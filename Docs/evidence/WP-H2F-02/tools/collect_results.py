"""Copies the curated WP-H2F-02 evidence into Docs/evidence/WP-H2F-02/{results,captures/owner}.

  python collect_results.py --workspace C:\\Juego2-H2F02-W --scratch <dir with real-project reports>

Sources: the evidence workspace run (workspace.py run) and the real-project batch reports of this Worker session
(material migration, final verify/tests, and the local H1 suite comparison main vs branch).
"""

import argparse
import json
import shutil
import xml.etree.ElementTree as ET
from pathlib import Path

EV = Path(__file__).resolve().parents[1]
WORKSPACE_FILES = ["summary.json", "workspace.json", "content.json", "a1_clean_absent_report.json", "a2_clean_absent_tests.xml",
                   "b0_gc2_provisioning_receipt.json", "b1_gc2_report.json", "b2_gc2_tests.xml", "representative.json",
                   "c2_gc2_lint.json", "player_build.json"]


def cases(xml: Path) -> dict:
    run = ET.parse(xml).getroot()
    return {"result": run.get("result"), "total": run.get("total"), "passed": run.get("passed"), "failed": run.get("failed"),
            "cases": {c.get("fullname"): c.get("result") for c in run.iter("test-case")}}


def main() -> None:
    parser = argparse.ArgumentParser()
    parser.add_argument("--workspace", type=Path, required=True)
    parser.add_argument("--scratch", type=Path, required=True)
    args = parser.parse_args()
    results, captures = EV / "results", EV / "captures/owner"
    results.mkdir(parents=True, exist_ok=True)
    captures.mkdir(parents=True, exist_ok=True)
    for name in WORKSPACE_FILES:
        shutil.copyfile(args.workspace / "results" / name, results / name)
    for png in (args.workspace / "results/captures").glob("*.png"):
        shutil.copyfile(png, captures / png.name)
    shutil.copyfile(args.scratch / "r08_migration.json", results / "real_project_material_migration.json")
    shutil.copyfile(args.scratch / "r11_report.json", results / "real_project_foundation_report.json")
    (results / "real_project_tests.json").write_text(json.dumps(cases(args.scratch / "t09_j2.xml"), indent=2) + "\n", encoding="utf-8")
    main_run, branch_run = cases(args.scratch / "keep/t00_h1_main.xml"), cases(args.scratch / "keep/t01_h1.xml")
    differ = sorted(k for k in set(main_run["cases"]) | set(branch_run["cases"]) if main_run["cases"].get(k) != branch_run["cases"].get(k))
    comparison = {
        "note": "Arkus.H1.Baseline.Tests.Editor on this workstation without the private vault mount or host-prepared plans; "
                "main e5322c84 (built-in) vs this branch (URP) with identical local ignored state",
        "main": {k: main_run[k] for k in ("result", "total", "passed", "failed")},
        "branch": {k: branch_run[k] for k in ("result", "total", "passed", "failed")},
        "differingCases": {k: {"main": main_run["cases"].get(k), "branch": branch_run["cases"].get(k)} for k in differ},
    }
    (results / "h1_local_suite_comparison.json").write_text(json.dumps(comparison, indent=2) + "\n", encoding="utf-8")
    print(f"H2F02_RESULTS_COLLECTED results={len(list(results.iterdir()))} captures={len(list(captures.iterdir()))} differing={differ}")


if __name__ == "__main__":
    main()
