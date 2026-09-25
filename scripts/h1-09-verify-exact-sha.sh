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
clean_candidate || { echo 'H1-09 candidate is not clean' >&2; exit 2; }

for required in \
  Docs/workpacks/H1/WP-H1-09.md \
  Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md \
  Docs/evidence/WP-H1-09/PREDECESSOR_CONTRACT_CHECK.md \
  tools/Arkus.H1.UnityHost/H1ProjectionReconciliation.cs \
  tools/Arkus.H1.UnityHost/H1ProjectionReconciliationCapability.cs \
  tools/Arkus.H1.UnityHost/H1ProjectionReconciliationComposition.cs \
  tests/Arkus.Harness.Tests/H1ProjectionReconciliationTests.cs \
  tests/Arkus.Harness.Tests/H1ProjectionCatalogueDriftTests.cs \
  tests/Arkus.Harness.Tests/H1ProjectionReconciliationTransportTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Reconciliation.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1ProjectionReconciliationTests.cs \
  .github/workflows/h1-09-unity-validation.yml; do
  test -f "${required}" || { echo "Missing H1-09 verification input: ${required}" >&2; exit 2; }
done

reconciliation=tools/Arkus.H1.UnityHost/H1ProjectionReconciliation.cs
capability=tools/Arkus.H1.UnityHost/H1ProjectionReconciliationCapability.cs
composition=tools/Arkus.H1.UnityHost/H1ProjectionReconciliationComposition.cs
transport=tests/Arkus.Harness.Tests/H1ProjectionReconciliationTransportTests.cs
unity_bridge=Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Reconciliation.cs
unity_test=Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1ProjectionReconciliationTests.cs
workflow=.github/workflows/h1-09-unity-validation.yml

grep -Fq 'arkus.h1-projection-drift-report@1' "${reconciliation}"
grep -Fq 'arkus.h1-projection-import-proposal@1' "${reconciliation}"
grep -Fq 'unity.host.projection.drift' "${capability}"
grep -Fq 'unity.host.projection.rematerialize' "${capability}"
grep -Fq 'unity.host.projection.import-proposal' "${capability}"
grep -Fq 'H1ProjectionReconciliationComposition' "${composition}"
grep -Fq 'unity.host.projection.drift' "${transport}"
grep -Fq 'unity.host.projection.rematerialize' "${transport}"
grep -Fq 'unity.host.projection.import-proposal' "${transport}"
grep -Fq 'ExecuteReconciliation' "${unity_bridge}"
grep -Fq 'RematerializationRestoresParity' "${unity_test}"
grep -Fq 'H1ProjectionReconciliationTests' "${workflow}"

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-09`?[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

clean_candidate || { echo 'H1-09 candidate changed during verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-09
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical static freeze verifier (hosted or local)
Canonical command: scripts/h1-09-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-SHA=GREEN; H1-09-static-contract-shape=GREEN; frozen-handoff=SEPARATE_CONTEXT_GATE; H1-09-Unity=EXTERNAL_EXACT_SHA_RECEIPT; H1-08-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-07-regression=EXTERNAL_EXACT_SHA_RECEIPT; hosted-Main-Safety=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: PR #220 exact-SHA H1-09 Unity, H1-08 regression, H1-07 regression, and Main Safety receipts
EOF
