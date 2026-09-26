#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"
# Candidate Validation receipt files (VALIDATION_CONTEXT.json, validation.log) are tolerated, as in the exact-SHA verifiers;
# the mutations run in a disposable worktree of the committed candidate, so they never see those files.
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "H1-01 negative conformance requires a clean candidate" >&2; exit 2; }

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
    echo "H1-01 defect control ${name}: infrastructure/build failure instead of causal test RED" >&2
    cat "${log}" >&2
    exit 2
  fi
  if [[ ${status} -eq 0 ]] || ! grep -Fq 'Failed!' "${log}"; then
    echo "H1-01 defect control ${name}: FALSE GREEN or non-test failure" >&2
    cat "${log}" >&2
    exit 1
  fi
  echo "H1_01_NEGATIVE_RED ${name}"
  restore_candidate
}

# Structured canonical references must all become HK02A dependency metadata.
# Use a runtime expression that is true for every real component without making the
# following code compiler-unreachable: the causal oracle must fail as a test, not as CS0162.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs \
  'if (!string.Equals((string)component["kind"]!, "canonical-link", StringComparison.Ordinal)) continue;' \
  'if (component.Count >= 0) continue; // H1-01 SEEDED DEFECT: canonical structured references are omitted'
expect_red omitted-canonical-reference 'FullyQualifiedName~H1UnityAuthoringProducerTests.PotesFacadeProbeDerivesEveryStructuredReferenceExactlyOnceAndRoundTrips'

# Catalogue references are typed provider-owned data; silently changing a material into an asset is semantic corruption.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs \
  'AddCatalogue(result, "material", (string)component["materialId"]!);' \
  'AddCatalogue(result, "asset", (string)component["materialId"]!); // H1-01 SEEDED DEFECT: material mis-typed as asset'
expect_red mistyped-catalogue-reference 'FullyQualifiedName~H1UnityAuthoringProducerTests.PotesFacadeProbeDerivesEveryStructuredReferenceExactlyOnceAndRoundTrips'

# A caller assertion may cross-check derived truth but may never override or omit it.
# Keep the branch dynamically reachable to avoid converting the seeded semantic defect into compiler RED.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs \
  'if (!EqualCanonical(supplied, derived))' \
  'if (supplied.Count < 0 && !EqualCanonical(supplied, derived)) // H1-01 SEEDED DEFECT: contradictory caller truth accepted'
expect_red contradictory-caller-dependency 'FullyQualifiedName~H1UnityAuthoringProducerTests.CallerMaintainedDependencyTruthFailsClosedOnOmissionContradictionAndDuplicates'

# Duplicate caller dependency claims must fail instead of being silently coalesced into derived truth.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs \
  'if (!seen.Add(dependency.Key))\n                    throw Error("unity.binding.duplicate-dependency-assertion", path + "[" + index + "]", "Caller dependency assertions may not contain duplicates.");' \
  'if (!seen.Add(dependency.Key))\n                    continue; // H1-01 SEEDED DEFECT: duplicate caller assertion silently accepted'
expect_red duplicate-caller-dependency 'FullyQualifiedName~H1UnityAuthoringProducerTests.CallerMaintainedDependencyTruthFailsClosedOnOmissionContradictionAndDuplicates'

# One transport dropping a newly composed route must be observable despite the canonical inventory remaining complete.
replace_once \
  src/Arkus.Harness.Mcp/McpProjectionAdapter.cs \
  'var definitions = _projection.Capabilities\n                .OrderBy(value => value.Key.Name, StringComparer.Ordinal)' \
  'var definitions = _projection.Capabilities\n                .Where(value => value.Key.Name != "unity.binding.compile") // H1-01 SEEDED DEFECT: MCP-only omission\n                .OrderBy(value => value.Key.Name, StringComparer.Ordinal)'
expect_red mcp-route-omission 'FullyQualifiedName~H1UnityAuthoringTransportTests.UnityScopedCapabilitiesAndPotesCompileAreEquivalentAcrossJsonlAndMcp'

# Keep capability/schema IDs at v1 while changing the admitted component vocabulary: this is true same-version semantic drift.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityAuthoringProvider.cs \
  '["kind"] = SchemaNode.String(new[] { "canonical-link", "renderer", "animator" }),' \
  '["kind"] = SchemaNode.String(new[] { "canonical-link", "renderer", "animator", "light" }), // H1-01 SEEDED DEFECT: same-version schema semantics widened'
expect_red same-version-schema-drift 'FullyQualifiedName~H1UnityAuthoringContractPinTests.VersionOnePublicIdentityCannotDriftWithoutExplicitVersionChange'

# The portable provider assembly may not grow public Unity runtime/editor types.
replace_once \
  src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs \
  'namespace Arkus.EngineBridge.UnityAuthoring\n{\n    public sealed class UnityBindingException' \
  'namespace Arkus.EngineBridge.UnityAuthoring\n{\n    public sealed class UnityEditorPortableLeak { } // H1-01 SEEDED DEFECT\n\n    public sealed class UnityBindingException'
expect_red unity-public-type-leak 'FullyQualifiedName~H1UnityAuthoringContractPinTests.PortableProviderPublicSurfaceContainsNoUnityRuntimeOrEditorTypes'

restore_candidate
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Disposable H1-01 mutation worktree did not restore cleanly" >&2; exit 2; }
echo 'H1_01_NEGATIVE_CONFORMANCE GREEN red_controls=7'
