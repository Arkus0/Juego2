#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"
# Candidate Validation writes its own receipt files (VALIDATION_CONTEXT.json, validation.log) before invoking this
# verifier; they are tolerated exactly as in the newer exact-SHA verifiers (for example h1-04-verify-exact-sha.sh).

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean before H1-01 observation" >&2; exit 2; }

test -f src/Arkus.EngineBridge.UnityAuthoring/Arkus.EngineBridge.UnityAuthoring.csproj
test -f src/Arkus.EngineBridge.UnityAuthoring/UnityBindingProducer.cs
test -f src/Arkus.EngineBridge.UnityAuthoring/UnityAuthoringProvider.cs
test -f tests/Arkus.Harness.Tests/H1UnityAuthoringProducerTests.cs
test -f tests/Arkus.Harness.Tests/H1UnityAuthoringTransportTests.cs
test -f tests/Arkus.Harness.Tests/H1UnityAuthoringContractPinTests.cs
test -f scripts/h1-01-negative-conformance.sh
test -f src/Arkus.Harness.Mcp/packages.lock.json

DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers \
  --filter 'FullyQualifiedName~H1UnityAuthoringProducerTests|FullyQualifiedName~H1UnityAuthoringTransportTests|FullyQualifiedName~H1UnityAuthoringContractPinTests'

if grep -R -E 'UnityEngine|UnityEditor' src/Arkus.EngineBridge.UnityAuthoring --include='*.cs' --include='*.csproj'; then
  echo "H1-01 portable provider contains a Unity runtime/editor implementation type/reference" >&2
  exit 2
fi
if grep -R -E '<PackageReference[^>]+Include="Unity|<ProjectReference[^>]+Unity[^A]' src/Arkus.EngineBridge.UnityAuthoring --include='*.csproj'; then
  echo "H1-01 portable provider depends on a Unity package/project" >&2
  exit 2
fi
if grep -R -E 'Assets/|Library/|ProjectSettings/|\.meta\b' src/Arkus.EngineBridge.UnityAuthoring --include='*.cs'; then
  echo "H1-01 portable provider contains native Unity path authority" >&2
  exit 2
fi

bash scripts/h1-01-negative-conformance.sh

DOTNET_NOLOGO=1 dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --configuration Release --no-build --no-restore -m:1 --disable-build-servers

[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "Candidate is not clean after H1-01 observation" >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-01
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-01-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-unity-producer=GREEN; potes-content-probe=GREEN; hk02a-integration=GREEN; jsonl-mcp-delta=GREEN; portable-unity-boundary=GREEN; causal-negative-controls=GREEN; regression=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H1-01
EOF
