#!/usr/bin/env bash
set -euo pipefail
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
EXPECTED_SHA="${1:-${CANDIDATE_SHA:-}}"
cd "${ROOT}"

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}

actual="$(git rev-parse HEAD)"
if [[ -z "${EXPECTED_SHA}" ]]; then EXPECTED_SHA="${actual}"; fi
[[ "${EXPECTED_SHA}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid expected SHA: ${EXPECTED_SHA}" >&2; exit 2; }
[[ "${actual}" == "${EXPECTED_SHA}" ]] || { echo "SHA mismatch: expected ${EXPECTED_SHA}, observed ${actual}" >&2; exit 2; }
[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean before DW GATE observation" >&2; candidate_dirty_status >&2; exit 2; }

for path in \
  scripts/dwgate-closure-check.sh \
  scripts/dwgate-proof-infrastructure-check.sh \
  scripts/dwgate-dw04-accepted-check.sh \
  scripts/dwgate-negative-conformance.sh \
  scripts/dwgate-evidence-audit.py \
  Docs/evidence/WP-DW-GATE/PROOF_MATRIX.md \
  Docs/evidence/WP-DW-GATE/H2_HANDOFF_V1.json; do
  test -f "${path}"
done

bash scripts/dwgate-closure-check.sh "${actual}"
bash scripts/dwgate-proof-infrastructure-check.sh

test -f src/Arkus.Harness.Mcp/packages.lock.json
DOTNET_NOLOGO=1 dotnet restore Juego2.sln --locked-mode -m:1 --disable-build-servers
DOTNET_NOLOGO=1 dotnet build Juego2.sln --configuration Release --no-restore -m:1 --disable-build-servers

GATE_STAGES=(
  dw00-generic
  dw01-city-invariants
  dw02-city-queries
  dw03-pa-losslessness
  dw04-paired-agent-evidence
  dw05-generic-boundary
  gate-evidence-audit
  gate-negative-conformance
  full-regression
)
run_gate_stage() {
  local stage="$1"; shift
  echo "DW_GATE_STAGE_START ${stage}"
  "$@"
  echo "DW_GATE_STAGE_GREEN ${stage}"
}

# Focused predecessor surfaces are re-executed after one shared restore/build.
# DW-04 is special: its accepted audit is intentionally historical/exact-SHA,
# so the gate verifies that frozen candidate in a disposable worktree instead
# of weakening chronology or pretending the current descendant is DW-04.
run_gate_stage dw00-generic dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers --filter 'FullyQualifiedName~Dw00'
run_gate_stage dw01-city-invariants dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers --filter 'FullyQualifiedName~Dw01'
run_gate_stage dw02-city-queries dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers --filter 'FullyQualifiedName~Dw02'
run_gate_stage dw03-pa-losslessness dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers --filter 'FullyQualifiedName~Dw03'
run_gate_stage dw04-paired-agent-evidence bash scripts/dwgate-dw04-accepted-check.sh
run_gate_stage dw05-generic-boundary dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers --filter 'FullyQualifiedName~Dw05'
run_gate_stage gate-evidence-audit python3 scripts/dwgate-evidence-audit.py
run_gate_stage gate-negative-conformance bash scripts/dwgate-negative-conformance.sh
run_gate_stage full-regression dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --configuration Release --no-build --no-restore -m:1 --disable-build-servers

digest_a="$(python3 scripts/dwgate-evidence-audit.py --digest-only)"
digest_b="$(python3 scripts/dwgate-evidence-audit.py --digest-only)"
[[ "${digest_a}" == "${digest_b}" ]] || { echo "DW GATE deterministic evidence digest mismatch" >&2; exit 1; }

[[ -z "$(candidate_dirty_status)" ]] || { echo "Candidate is not clean after DW GATE observation" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-DW-GATE
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-worker-or-remote-shell}
Canonical command: scripts/dwgate-observe-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: closure-only-diff=GREEN; proof-infrastructure=GREEN; locked-restore=GREEN; release-build=GREEN; dw00=GREEN; dw01-city-invariants=GREEN; dw02-city-queries=GREEN; dw03-pa-losslessness=GREEN; dw04-accepted-historical-evidence=GREEN; dw05-generic-boundary=GREEN; gate-evidence-reconciliation=GREEN; gate-negative-controls=GREEN; single-full-regression=GREEN; deterministic-evidence-digest=GREEN
Evidence digest: ${digest_a}
Result: GREEN
Evidence: Docs/evidence/WP-DW-GATE
EOF
