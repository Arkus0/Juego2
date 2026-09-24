#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: ${expected} != ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "H1-05 candidate is not clean" >&2; exit 2; }

for required in \
  Docs/evidence/WP-H1-05/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-05/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-05/CONTENT_SHAPE_PROBE.md \
  Docs/evidence/WP-H1-05/PUBLIC_CONFORMANCE.json \
  tools/Arkus.H1.UnityHost/H1ManagedScenePlan.cs \
  tools/Arkus.H1.UnityHost/H1ManagedSceneCapability.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1SceneProjection.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Runtime/H1ManagedMarker.cs \
  scripts/h1-05-local-evidence.ps1 \
  scripts/h1-05-public-conformance.py; do
  test -e "${required}" || { echo "Missing H1-05 verification input: ${required}" >&2; exit 2; }
done

dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release -m:1 --disable-build-servers
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --no-build --no-restore -c Release \
  --filter 'FullyQualifiedName~H1ManagedScenePlanTests|FullyQualifiedName~H1CatalogueTests'
python3 scripts/h1-05-evidence-check.py \
  --inventory Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json \
  --public Docs/evidence/WP-H1-05/PUBLIC_CONFORMANCE.json

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "H1-05 candidate changed during verification" >&2; exit 2; }
cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-05
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical .NET/static verifier (hosted or local)
Canonical command: scripts/h1-05-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; focused-projection-and-catalogue-tests=GREEN; committed-evidence-shape=GREEN; local-Unity=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: Docs/evidence/WP-H1-05/PUBLIC_CONFORMANCE.json; Docs/evidence/WP-H1-05/PROOF_MATRIX.md; exact-SHA physical-local Unity receipt on PR #192
EOF
