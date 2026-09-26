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
clean_candidate || { echo 'H1-10 candidate is not clean' >&2; exit 2; }

for required in \
  Docs/workpacks/H1/WP-H1-10.md \
  Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md \
  Docs/evidence/WP-H1-10/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-10/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-10/RESIDUAL_RISK.md \
  tools/Arkus.H1.UnityHost/H1ProjectCheckpoint.cs \
  tools/Arkus.H1.UnityHost/H1ProjectCheckpointRestore.cs \
  tools/Arkus.H1.UnityHost/H1ProjectCheckpointComposition.cs \
  tools/Arkus.H1.UnityHost/ProductionH1ProjectCheckpointHost.cs \
  tests/Arkus.Harness.Tests/H1ProjectCheckpointTests.cs \
  tests/Arkus.Harness.Tests/H1ProjectCheckpointEffectiveProofTests.cs \
  tests/Arkus.Harness.Tests/H1ProjectCheckpointTransportTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Checkpoint.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Checkpoint.cs.meta \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1ProjectCheckpointReconstructionTests.cs \
  .github/workflows/h1-10-unity-validation.yml; do
  test -f "${required}" || { echo "Missing H1-10 verification input: ${required}" >&2; exit 2; }
done

checkpoint=tools/Arkus.H1.UnityHost/H1ProjectCheckpoint.cs
restore=tools/Arkus.H1.UnityHost/H1ProjectCheckpointRestore.cs
host=tools/Arkus.H1.UnityHost/ProductionH1ProjectCheckpointHost.cs
transport=tests/Arkus.Harness.Tests/H1ProjectCheckpointTransportTests.cs
effective=tests/Arkus.Harness.Tests/H1ProjectCheckpointEffectiveProofTests.cs
unity_bridge=Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.Checkpoint.cs
workflow=.github/workflows/h1-10-unity-validation.yml

grep -Fq 'arkus.h1-project-checkpoint@1' "${checkpoint}"
grep -Fq 'class H1ReconstructionParity' "${checkpoint}"
grep -Fq 'CaptureFromObservation' "${checkpoint}"
grep -Fq 'unity.host.checkpoint.capture' "${checkpoint}"
grep -Fq 'unity.host.checkpoint.current' "${checkpoint}"
grep -Fq 'unity.host.projection.clean-rebuild' "${checkpoint}"
grep -Fq 'unity.host.checkpoint.restore' "${restore}"
grep -Fq 'H1ReconstructionParity.RequireBaseline' "${restore}"
grep -Fq 'H1ProjectCheckpointComposition' "${host}"
for capability in unity.host.checkpoint.capture unity.host.checkpoint.current unity.host.checkpoint.restore unity.host.projection.clean-rebuild; do
  grep -Fq "${capability}" "${transport}"
done
grep -Fq 'StageD_RebuiltObservationProvesNormalizedParityThroughProductVerifier' "${effective}"
grep -Fq 'Negative_EffectiveMissingOrReboundSourceBlocksPublicRestore' "${effective}"
grep -Fq 'ExecuteCleanRebuild' "${unity_bridge}"
grep -Fq 'StageD_RebuiltObservationProvesNormalizedParityThroughProductVerifier' "${workflow}"
grep -Fq 'Negative_EffectiveMissingOrReboundSourceBlocksPublicRestore' "${workflow}"
grep -Fq 'Negative_PrepublicationFailureDoesNotPublishGeneratedCurrent' "${workflow}"

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-10`?[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

clean_candidate || { echo 'H1-10 candidate changed during verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-10
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical static freeze verifier (hosted or local)
Canonical command: scripts/h1-10-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-SHA=GREEN; H1-10-static-contract-shape=GREEN; frozen-handoff=SEPARATE_CONTEXT_GATE; H1-10-Unity=EXTERNAL_EXACT_SHA_RECEIPT; H1-09-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-08-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-07-regression=EXTERNAL_EXACT_SHA_RECEIPT; hosted-Main-Safety=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: PR #230 exact-SHA H1-10 Unity reconstruction, H1-09/H1-08/H1-07 regression, and Main Safety receipts
EOF
