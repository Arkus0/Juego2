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

# Freeze must be explicit in PR metadata. The mandatory AI trial is deliberately external to this
# source-reading Worker context and must target the exact candidate without changing Git history.
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Worker state: FROZEN_FOR_REVIEW'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Branch frozen: YES'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'Worker pre-review: CLEAN'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq 'AI trial verdict: PASS'
printf '%s\n' "${PR_BODY:-}" | grep -Fxq "AI trial target SHA: ${actual}"
printf '%s\n' "${PR_BODY:-}" | grep -Eq '^AI trial evidence: https://github\.com/Arkus0/Juego2/pull/[0-9]+#issuecomment-[0-9]+$'

bash scripts/hkgate-observe-exact-sha.sh "${actual}"

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Eq '^TRUST_BOUNDARY: .+$' Docs/evidence/WP-HK-GATE/PROOF_MATRIX.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-HK-GATE/WORKER_PRE_REVIEW.md
grep -Eq '^WORKER_PRE_REVIEW_FINDINGS_FIXED: [0-9]+$' Docs/evidence/WP-HK-GATE/WORKER_PRE_REVIEW.md
grep -Fxq 'NEGATIVE_CONTROL_UNIVERSE: 13' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'GATE_OWNED_RED_CONTROLS: 1' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'INHERITED_HK10_RED_CONTROLS: 12' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'KNOWN_UNRESOLVED_FALSE_GREEN_CONTROLS: 0' Docs/evidence/WP-HK-GATE/NEGATIVE_CONFORMANCE_MATRIX.md
grep -Fxq 'RESIDUAL_LEDGER_RECONCILIATION: COMPLETE' Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md
grep -Fxq 'UNCLASSIFIED_RESIDUALS: 0' Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md
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
Required gates: deterministic-gate-observation=GREEN; external-ai-agent-trial=GREEN; foundational-proof=GREEN; residual-reconciliation=GREEN; dependency-ip-boundary=GREEN; worker-pre-review=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-HK-GATE; PR exact-SHA AI trial transcript link
EOF
