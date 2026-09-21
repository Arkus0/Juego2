#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "H1-00 negative conformance requires a clean candidate" >&2; exit 2; }

TMP_ROOT="$(mktemp -d)"
WORKTREE="${TMP_ROOT}/mutant"
LOG_DIR="${TMP_ROOT}/logs"
mkdir -p "${LOG_DIR}"
cleanup() {
  git worktree remove --force "${WORKTREE}" >/dev/null 2>&1 || true
  rm -rf "${TMP_ROOT}"
}
trap cleanup EXIT

git worktree add --detach "${WORKTREE}" "${CANDIDATE}" >/dev/null
cd "${WORKTREE}"
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers >/dev/null

replace_once() {
  local file="$1" old="$2" new="$3"
  python3 - "${file}" "${old}" "${new}" <<'PY'
import pathlib, sys
path = pathlib.Path(sys.argv[1])
old = sys.argv[2].replace("\\n", "\n")
new = sys.argv[3].replace("\\n", "\n")
text = path.read_text(encoding="utf-8")
count = text.count(old)
if count < 1:
    raise SystemExit(f"seed target missing: {path}: {old!r}")
path.write_text(text.replace(old, new, 1), encoding="utf-8")
PY
}

restore_candidate() {
  git reset --hard "${CANDIDATE}" >/dev/null
}

expect_red() {
  local name="$1" filter="$2"
  local log="${LOG_DIR}/${name}.log"
  set +e
  DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
    --configuration Release --no-restore -m:1 --disable-build-servers \
    --filter "${filter}" >"${log}" 2>&1
  local status=$?
  set -e

  if grep -Eq 'error (CS|MSB|NU)[0-9]+' "${log}"; then
    echo "H1-00 defect control ${name}: infrastructure/build failure instead of causal test RED" >&2
    cat "${log}" >&2
    exit 2
  fi
  if [[ ${status} -eq 0 ]] || ! grep -Fq 'Failed!' "${log}"; then
    echo "H1-00 defect control ${name}: FALSE GREEN or non-test failure" >&2
    cat "${log}" >&2
    exit 1
  fi
  echo "H1_00_NEGATIVE_RED ${name}"
  restore_candidate
}

# Receipt provenance: a published receipt must truthfully anchor the catalogue member of the input tuple.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  'CatalogueFingerprint = plan.Input.CatalogueFingerprint;' \
  'CatalogueFingerprint = "h1-seeded-wrong-catalogue"; // H1-00 SEEDED DEFECT'
expect_red receipt-anchor 'FullyQualifiedName~H1EngineBridgeReferenceTests.EveryFullInputAxisChangesIdentityAndReceiptAnchorsTheTuple'

# Determinism: plan normalization must not depend on caller resource enumeration order.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  '.OrderBy(value => value.ResourceId, StringComparer.Ordinal)\n                .ToArray();' \
  '.ToArray(); // H1-00 SEEDED DEFECT: caller order leaks into plan identity'
expect_red deterministic-plan 'FullyQualifiedName~H1EngineBridgeReferenceTests.SameFullInputNormalizesPlanAndRetryHasNoSecondSemanticDelta'

# Generational publication: pre-publication failure must not advance the active generation.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  'if (failurePoint == ReferenceFailurePoint.BeforePublication)\n                return Fail(plan, "reference.failure.before-publication");' \
  'if (failurePoint == ReferenceFailurePoint.BeforePublication)\n            {\n                _activePlan = plan; // H1-00 SEEDED DEFECT: failed staging leaks into active generation\n                return Fail(plan, "reference.failure.before-publication");\n            }'
expect_red failed-staging-publication 'FullyQualifiedName~H1EngineBridgeReferenceTests.FailedStagingNeverMovesTheActiveGeneration'

# Canonical immutability: callers must never receive the bridge-owned canonical byte buffer itself.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  'public byte[] GetCanonicalSnapshotCopy() => (byte[])_canonicalSnapshot.Clone();' \
  'public byte[] GetCanonicalSnapshotCopy() => _canonicalSnapshot; // H1-00 SEEDED DEFECT'
expect_red canonical-byte-isolation 'FullyQualifiedName~H1EngineBridgeReferenceTests.BridgePlanningAndMaterializationCannotMutateCanonicalSnapshotBytes'

# Drift completeness: effective extra/missing/changed managed resources must not collapse to in-sync.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  'if (!StringComparer.Ordinal.Equals(expectedShape, effectiveShape))' \
  'if (false && !StringComparer.Ordinal.Equals(expectedShape, effectiveShape)) // H1-00 SEEDED DEFECT'
expect_red drift-visibility 'FullyQualifiedName~H1EngineBridgeReferenceTests.DriftOracleDetectsExtraMissingAndChangedManagedResources'

# Engine neutrality: a public engine/editor-specific type in the neutral assembly must be rejected.
replace_once \
  src/Arkus.EngineBridge/ProjectionContract.cs \
  'internal static class StableEncoding' \
  'public sealed class UnityEditorLeak { } // H1-00 SEEDED DEFECT\n\n    internal static class StableEncoding'
expect_red engine-type-leak 'FullyQualifiedName~H1EngineBridgeReferenceTests.NeutralBridgeAssemblyHasNoRuntimeUnityOrTransportDependency'

restore_candidate
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Disposable H1-00 mutation worktree did not restore cleanly" >&2; exit 2; }
echo 'H1_00_NEGATIVE_CONFORMANCE GREEN red_controls=6'
