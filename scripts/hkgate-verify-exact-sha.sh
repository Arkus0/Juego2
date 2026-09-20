#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean before HK GATE verification" >&2; exit 2; }

# Final freeze is allowed only after both untracked-in-Git review artifacts have been attached to
# this exact candidate through immutable PR comments/metadata. This avoids a post-trial commit that
# would change the SHA and invalidate the independent trial's exact-SHA binding.
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Worker state: FROZEN_FOR_REVIEW'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Branch frozen: YES'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Worker pre-review: CLEAN'
printf '%s\n' "${PR_BODY:-}" | grep -Eq '^Worker pre-review evidence: https://github\.com/Arkus0/Juego2/pull/[0-9]+#issuecomment-[0-9]+$'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'AI trial verdict: PASS'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq "AI trial target SHA: ${actual}"
printf '%s\n' "${PR_BODY:-}" | grep -Eq '^AI trial evidence: https://github\.com/Arkus0/Juego2/pull/[0-9]+#issuecomment-[0-9]+$'

bash scripts/hkgate-observe-exact-sha.sh "${actual}"

# The committed matrix intentionally remains NOT_READY/1 on the candidate SHA: the single unresolved
# item is the external trial itself. Requiring READY/0 in a tracked file would force a post-trial
# commit, changing the SHA and invalidating the exact-SHA trial. Exact-SHA verification closes that
# one external obligation dynamically only after the PASS metadata above is proven for this SHA.
grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: NOT_READY' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 1' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Eq '^TRUST_BOUNDARY: .+$' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fq '| 17. fresh independent AI-agent public-client trial |' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fq '| **PENDING / BLOCKING** |' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'NEGATIVE_CONTROL_UNIVERSE: 13' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'GATE_OWNED_RED_CONTROLS: 1' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'INHERITED_HK10_RED_CONTROLS: 12' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'KNOWN_UNRESOLVED_FALSE_GREEN_CONTROLS: 0' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'RESIDUAL_LEDGER_RECONCILIATION: COMPLETE' Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md
grep -Fxq 'UNCLASSIFIED_RESIDUALS: 0' Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md
grep -Fxq 'HK10_RECONCILED_ENTRY_COUNT: 53' Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md

# COMPLETE/0 is accepted only when the GATE evidence explicitly consumes the full accepted HK10
# residual universe. This prevents a smaller hand-written summary from self-certifying as complete.
python3 - <<'PY'
from pathlib import Path
import re

expected = {
    "R-TB-01", "R-TB-02", "R-TB-03", "R-TB-04", "R-TB-05",
    "R-00A-02", "R-00A-05", "R-01-07", "R-01-08", "R-01-09",
    "R-02A-04", "R-04-04", "R-05-05", "R-05-06", "R-06A-02",
    "R-06A-03", "R-06A-04", "R-06A-05", "R-06A-07", "R-06B-02",
    "R-06B-03", "R-07A-01", "R-07A-02", "R-07A-03",
    "R-00-05", "R-00-06", "R-01-01", "R-01-03", "R-01-05",
    "R-02-01", "R-02-04", "R-02-05", "R-02A-01", "R-02A-02",
    "R-03-01", "R-03-02", "R-03-03", "R-04-01", "R-04-02",
    "R-04-05", "R-05-01", "R-06B-04", "R-06B-05", "R-06B-06",
    "R-06C-01", "R-06C-02", "R-07A-04", "R-07B-01", "R-07B-02",
    "R-07B-03", "R-08A-01", "R-08B-01", "R-09B-01",
}
path = Path("Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md")
rows = re.findall(r"^\| (R-[A-Za-z0-9-]+) \|", path.read_text(encoding="utf-8"), flags=re.MULTILINE)
found = set(rows)
if len(rows) != len(found):
    raise SystemExit("HK GATE residual reconciliation contains duplicate residual IDs")
if found != expected:
    missing = sorted(expected - found)
    extra = sorted(found - expected)
    raise SystemExit(f"HK GATE residual reconciliation universe mismatch missing={missing} extra={extra}")
if len(found) != 53:
    raise SystemExit(f"HK GATE residual reconciliation expected 53 entries, observed {len(found)}")
print("HK_GATE_RESIDUAL_RECONCILIATION GREEN inherited_entries=53")
PY

grep -Fxq 'DEPENDENCY_IP_INVENTORY: COMPLETE' Docs/evidence/WP-HK-GATE/DEPENDENCY_IP_INVENTORY.md
grep -Fxq 'ENGINE_BRIDGE_BOUNDARY: CLEAN' Docs/evidence/WP-HK-GATE/DEPENDENCY_IP_INVENTORY.md
grep -Fxq 'AI_AGENT_TRIAL_STATUS: EXTERNAL_EXACT_SHA_REQUIRED' Docs/evidence/WP-HK-GATE/AI_AGENT_TRIAL_BRIEF.md

[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Candidate is not clean after HK GATE verification" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-HK-GATE
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/hkgate-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: deterministic-gate-observation=GREEN; external-ai-agent-trial=GREEN; foundational-proof=GREEN; residual-reconciliation-53=GREEN; dependency-ip-boundary=GREEN; worker-pre-review=GREEN
FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
Result: GREEN
Evidence: Docs/evidence/WP-HK-GATE; PR exact-SHA Worker pre-review + AI trial comments
EOF