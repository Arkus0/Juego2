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
clean_candidate || { echo 'H1-11 candidate is not clean' >&2; exit 2; }

for required in \
  Docs/workpacks/H1/WP-H1-11.md \
  Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md \
  Docs/evidence/WP-H1-11/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json \
  Docs/evidence/WP-H1-11/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-11/RESIDUAL_RISK.md \
  Docs/evidence/WP-H1-11/EFFECTIVE_EVIDENCE.md \
  Docs/evidence/WP-H1-11/SUPPLEMENTARY_CAPTURE.jpg \
  Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json \
  Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json \
  Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json \
  Tools/AssetVault/h1_representative_slice.py \
  Tools/AssetVault/verify_h1_asset_vault.py \
  tests/Arkus.Harness.Tests/H1RepresentativeSliceScenario.cs \
  tests/Arkus.Harness.Tests/H1RepresentativeSliceContractTests.cs \
  tests/Arkus.Harness.Tests/H1RepresentativeSliceEffectiveProofTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1RepresentativeSliceTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1RepresentativeSliceTests.cs.meta \
  scripts/h1-11-unity-stage.sh \
  .github/workflows/h1-11-unity-validation.yml; do
  test -f "${required}" || { echo "Missing H1-11 verification input: ${required}" >&2; exit 2; }
done

# Public-only representative-slice contract: manifest <-> H1-04 admission record <-> catalogue, and the
# synthetic fail-closed controls of the parser/mount barriers. Private bytes are checked by the Unity workflow.
PYTHONDONTWRITEBYTECODE=1 python3 Tools/AssetVault/h1_representative_slice.py self-test
PYTHONDONTWRITEBYTECODE=1 python3 Tools/AssetVault/h1_representative_slice.py static --public-root .
grep -Fq 'h1_representative_slice.verify_for_vault' Tools/AssetVault/verify_h1_asset_vault.py

workflow=.github/workflows/h1-11-unity-validation.yml
unity=Unity/ArkusUnity/Assets/Arkus/H1/Tests/Editor/H1RepresentativeSliceTests.cs
effective=tests/Arkus.Harness.Tests/H1RepresentativeSliceEffectiveProofTests.cs
for stage in \
  Stage0_EffectiveImportAndCatalogueMatchTheAdoptedSource \
  StageA_MaterializeSaveReloadAndInspectTheStreetCorner \
  StageA2_SupplementaryRenderedCaptureIsNotTheParityOracle \
  StageB_CleanGeneratedOutputRebuildOfTheRealSliceInFreshEditor \
  StageC_RemovedOrReplacedSelectedAssetYieldsNamedDiagnosticsNotSubstitution; do
  grep -Fq "public void ${stage}()" "${unity}"
  grep -Fq "${stage}" "${workflow}"
done
for stage in \
  StageA_SeedStreetCornerAndPlanTheCompleteRepresentativeUniverse \
  StageB_CaptureCheckpointFromTheEffectiveRepresentativeBaseline \
  StageC_FreshProcessRestoresRepresentativeSliceOnlyThroughAcceptedH0Import \
  StageD_RebuiltRepresentativeObservationProvesNormalizedParity; do
  grep -Fq "${stage}" "${effective}"
  grep -Fq "${stage}" "${workflow}"
done
grep -Fq 'StageE_RemovedOrReplacedSelectedAssetIsNamedByTheLiveCatalogue' "${effective}"
grep -Fq 'StageE_RemovedOrReplacedSelectedAssetBlocksPublicRestore' "${effective}"
grep -Fq 'H1RepresentativeSliceEffectiveProofTests.StageE_' "${workflow}"
grep -Fq 'h1_representative_slice.py sources' "${workflow}"
grep -Fq 'h1_representative_slice.py sources' scripts/h1-11-unity-stage.sh

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq '^WP:[[:space:]]*`?WP-H1-11`?[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

clean_candidate || { echo 'H1-11 candidate changed during verification' >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-11
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical static freeze verifier (hosted or local)
Canonical command: scripts/h1-11-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: exact-SHA=GREEN; representative-slice-static-contract=GREEN; mount-barrier-self-test=GREEN; H1-11-stage-shape=GREEN; frozen-handoff=SEPARATE_CONTEXT_GATE; H1-11-Unity=EXTERNAL_EXACT_SHA_RECEIPT; H1-10-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-09-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-08-regression=EXTERNAL_EXACT_SHA_RECEIPT; H1-07-regression=EXTERNAL_EXACT_SHA_RECEIPT; hosted-Main-Safety=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: PR exact-SHA H1-11 representative real-source run, H1-10/H1-09/H1-08/H1-07 regression and Main Safety receipts; Docs/evidence/WP-H1-11/
EOF
