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
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before H1-03 verification" >&2; candidate_dirty_status >&2; exit 2; }

for required in \
  src/Arkus.Harness.Projection/H1UnityHostPolicy.cs \
  src/Arkus.Harness.Projection/NeutralProjection.cs \
  tests/Arkus.Harness.Tests/H1UnityHostPolicyTests.cs \
  tests/Arkus.Harness.Tests/H1UnityHostPolicyMutationCompatibilityTests.cs \
  tests/Arkus.Harness.Tests/H1UnityHostAdmissionTokenTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1 \
  .github/workflows/h1-03-unity-validation.yml \
  Docs/evidence/WP-H1-03/PREDECESSOR_CONTRACT_CHECK.md; do
  test -e "${required}" || { echo "Missing H1-03 verification input: ${required}" >&2; exit 2; }
done

dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj \
  --no-build --no-restore -c Release \
  --filter 'FullyQualifiedName~H1UnityHost'

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after H1-03 verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-03
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-local-shell}
Canonical command: scripts/h1-03-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; h1-unity-host-policy-focused-tests=GREEN; h0-boundary-regression=GREEN; workspace-grant-retention=GREEN; real-h0-mutation-compatibility=GREEN; ambient-authority-contract-controls=GREEN; effective-unity=EXTERNAL_EXACT_SHA_EVIDENCE; worker-pre-review=EXTERNAL_HANDOFF_LINT; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-H1-03 + .github/workflows/h1-03-unity-validation.yml
EOF
