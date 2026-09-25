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
clean_candidate || { echo 'H1-08 candidate is not clean' >&2; exit 2; }

for required in \
  Docs/workpacks/H1/WP-H1-08.md \
  Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1ProjectionValidation.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Validation.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1ProjectionValidationTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1ProjectionContentShapeTests.cs \
  .github/workflows/h1-08-unity-validation.yml; do
  test -f "${required}" || { echo "Missing H1-08 verification input: ${required}" >&2; exit 2; }
done

validation=Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1ProjectionValidation.cs
composition=Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Validation.cs
projection=Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs
workflow=.github/workflows/h1-08-unity-validation.yml

grep -Fq 'arkus.h1-unity-validation-result@1' "${validation}"
grep -Fq 'arkus.h1-unity-validation-inventory@1' "${validation}"
grep -Fq 'unity.scene.finite-transform' "${validation}"
grep -Fq 'projection.non-finite-transform' "${validation}"
grep -Fq 'validate-proposed' "${composition}"
grep -Fq 'validate-current' "${composition}"
grep -Fq 'RunPostflight' "${projection}"
grep -Fq 'Publish(manifest)' "${projection}"
grep -Fq 'H1ProjectionValidationTests' "${workflow}"
grep -Fq 'H1ProjectionContentShapeTests' "${workflow}"

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-08`?[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

clean_candidate || { echo 'H1-08 candidate changed during verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-08
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical static freeze verifier (hosted or local)
Canonical command: scripts/h1-08-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-SHA=GREEN; H1-08-static-contract-shape=GREEN; frozen-handoff=SEPARATE_CONTEXT_GATE; H1-08-Unity=EXTERNAL_EXACT_SHA_RECEIPT; H1-07-regression=EXTERNAL_EXACT_SHA_RECEIPT; hosted-Main-Safety=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: PR #216 exact-SHA H1-08 Unity, H1-07 regression, and Main Safety receipts
EOF
