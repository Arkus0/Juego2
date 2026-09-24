#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: ${expected} != ${actual}" >&2; exit 2; }

clean_candidate() {
  [[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]]
}
clean_candidate || { echo 'H1-06 candidate is not clean' >&2; exit 2; }

for required in \
  Docs/evidence/WP-H1-06/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-06/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-06/CONTENT_SHAPE_PROBE.md \
  Docs/evidence/WP-H1-06/LOCAL_EXECUTION_RESULT.md \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1PrefabNestedConformance.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Runtime/H1ManagedPrefabLineage.cs \
  scripts/h1-06-local-evidence.ps1 \
  scripts/h1-06-public-conformance.py \
  scripts/h1-06-content-shape-probe.py; do
  test -f "${required}" || { echo "Missing H1-06 verification input: ${required}" >&2; exit 2; }
done

matrix=Docs/evidence/WP-H1-06/PROOF_MATRIX.md
shape=Docs/evidence/WP-H1-06/CONTENT_SHAPE_PROBE.md
local_result=Docs/evidence/WP-H1-06/LOCAL_EXECUTION_RESULT.md
grep -Fq 'FOUNDATIONAL_PROOF_VERDICT: READY' "${matrix}"
grep -Fq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' "${matrix}"
grep -Fq 'KNOWN_UNDETECTED_DEFECT_CLASSES: 0' "${matrix}"
grep -Fq 'PROOF_BUDGET_VERDICT: WITHIN_BUDGET' "${matrix}"
grep -Fq 'ObserveRealization' "${matrix}"
grep -Fq 'projection.prefab-nested-lineage-missing' "${local_result}"
grep -Fq 'b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10' "${shape}"

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^WP:[[:space:]]*\`?WP-H1-06\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

clean_candidate || { echo 'H1-06 candidate changed during verification' >&2; exit 2; }
cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-06
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical static evidence verifier (hosted or local)
Canonical command: scripts/h1-06-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-SHA=GREEN; committed-evidence-shape=GREEN; frozen-handoff=GREEN; physical-local-Unity=EXTERNAL_EXACT_SHA_RECEIPT; hosted-Main-Safety=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: Docs/evidence/WP-H1-06/; exact-SHA physical-local Unity and Main Safety receipts on PR #195
EOF
