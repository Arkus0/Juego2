#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before H1-02 verification" >&2; candidate_dirty_status >&2; exit 2; }

for required in \
  Unity/ArkusUnity/Packages/packages-lock.json \
  Docs/evidence/WP-H1-02/effective-inventory.json \
  Docs/evidence/WP-H1-02/second-import-inventory.json \
  Docs/evidence/WP-H1-02/editmode-results.xml \
  Docs/evidence/WP-H1-02/PACKAGE_LEGAL_OBSERVATION.md \
  Docs/evidence/WP-H1-02/LOCAL_EXECUTION_RESULT.md \
  Docs/evidence/WP-H1-02/WORKER_PRE_REVIEW.md; do
  test -f "${required}" || { echo "Missing H1-02 verification evidence: ${required}" >&2; exit 2; }
done

bash scripts/h1-02-observe-exact-sha.sh "${actual}" | tee /tmp/h1-02-observe.txt
grep -Fxq 'Result: GREEN' /tmp/h1-02-observe.txt

grep -Fxq 'FOUNDATIONAL_PROOF_VERDICT: READY' Docs/evidence/WP-H1-02/PROOF_PLAN.md
grep -Fxq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-H1-02/PROOF_PLAN.md
grep -Fxq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' Docs/evidence/WP-H1-02/PROOF_PLAN.md
grep -Fxq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' Docs/evidence/WP-H1-02/PROOF_PLAN.md
grep -Fxq 'LOCAL_EXECUTION_RESULT: PASS' Docs/evidence/WP-H1-02/LOCAL_EXECUTION_RESULT.md
grep -Fxq 'WORKER_PRE_REVIEW: CLEAN' Docs/evidence/WP-H1-02/WORKER_PRE_REVIEW.md
grep -Eq '^WORKER_PRE_REVIEW_FINDINGS_FIXED: [0-9]+$' Docs/evidence/WP-H1-02/WORKER_PRE_REVIEW.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after H1-02 verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-02
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-02-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: remote-static=GREEN; causal-negative-controls=GREEN; h0-build=GREEN; local-unity-evidence=GREEN; editmode=GREEN; effective-inventory=GREEN; clean-second-import=GREEN; dependency-legal-observation=GREEN; foundational-proof=GREEN; worker-pre-review=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H1-02
EOF
