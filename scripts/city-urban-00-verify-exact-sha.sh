#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
TARGET_SHA="$1"
BASE_SHA="5d74e59d4af7738fc28450a60313f0f5932a769d"

[[ "$TARGET_SHA" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "CITYURBAN00_VERIFY_RED: exact SHA required" >&2; exit 2; }
[[ "$(git rev-parse HEAD)" == "$TARGET_SHA" ]] || { echo "CITYURBAN00_VERIFY_RED: checkout mismatch" >&2; exit 2; }
git diff --quiet && git diff --cached --quiet || { echo "CITYURBAN00_VERIFY_RED: tracked bytes changed" >&2; exit 2; }
git merge-base --is-ancestor "$BASE_SHA" "$TARGET_SHA" || { echo "CITYURBAN00_VERIFY_RED: baseline is not an ancestor" >&2; exit 2; }

python3 - "$BASE_SHA" "$TARGET_SHA" <<'PY_CHECK'
from pathlib import Path
import subprocess
import sys

base, target = sys.argv[1:]
required = {
    "Docs/production/CITY_PORT_TOWN_TRANSITION.md",
    "Docs/evidence/WP-CITY-URBAN-00/PREDECESSOR_CONTRACT_CHECK.md",
    "Docs/evidence/WP-CITY-URBAN-00/WORKER_PLAN.md",
    "Docs/evidence/WP-CITY-URBAN-00/PLANNING_EVALUATION.md",
}
changed = set(subprocess.check_output(
    ["git", "diff", "--name-only", base, target], text=True).splitlines())
assert required <= changed, f"Missing candidate planning/evidence changes: {required - changed}"
for path in changed:
    assert (
        path.startswith("Docs/evidence/WP-CITY-URBAN-00/")
        or path == "Docs/production/CITY_PORT_TOWN_TRANSITION.md"
        or path in {
            "scripts/arkus-verify-exact-sha.sh",
            "scripts/city-urban-00-verify-exact-sha.sh",
        }
    ), f"Out-of-workpack mutation: {path}"
for path in required:
    assert Path(path).is_file() and Path(path).stat().st_size, f"Missing/empty: {path}"
plan = Path("Docs/production/CITY_PORT_TOWN_TRANSITION.md").read_text(encoding="utf-8")
evaluation = Path("Docs/evidence/WP-CITY-URBAN-00/PLANNING_EVALUATION.md").read_text(encoding="utf-8")
for marker in ("source-to-new-site", "B01", "B10", "residency", "CITY-URBAN-01", "not measured"):
    assert marker.lower() in plan.lower(), f"Planning section absent: {marker}"
assert "PLANNING_GRAPH_CONSISTENT" in evaluation
print("CITYURBAN00_STRUCTURAL_CHECK_GREEN: required planning/evidence files and diff scope")
PY_CHECK

git diff --quiet && git diff --cached --quiet || { echo "CITYURBAN00_VERIFY_RED: tracked bytes changed after check" >&2; exit 2; }
cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-CITY-URBAN-00
Candidate SHA: $TARGET_SHA
Executor role: WORKER
Execution environment: worker shell or GitHub Actions
Canonical command: scripts/city-urban-00-verify-exact-sha.sh $TARGET_SHA
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-checkout=GREEN; bounded-diff=GREEN; required-planning-evidence=GREEN; semantic-product-review=INDEPENDENT_REVIEW_REQUIRED
Result: GREEN
Evidence: Docs/evidence/WP-CITY-URBAN-00/
EOF
