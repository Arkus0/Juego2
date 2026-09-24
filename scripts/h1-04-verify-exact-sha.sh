#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: expected ${expected}, observed ${actual}" >&2; exit 2; }
[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "H1-04 candidate is not clean" >&2; exit 2; }

for required in \
  Docs/evidence/WP-H1-04/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-H1-04/SOURCE_ADOPTION.md \
  Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json \
  Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json \
  Docs/evidence/WP-H1-04/EFFECTIVE_MUTATION.json \
  Docs/evidence/WP-H1-04/PUBLIC_CONFORMANCE.json \
  Docs/evidence/WP-H1-04/EFFECTIVE_VALIDATION.json \
  Docs/evidence/WP-H1-04/PROOF_MATRIX.md \
  Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1CatalogueInventory.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/Editor/H1CatalogueMutationProof.cs \
  scripts/h1-04-local-evidence.ps1; do
  test -e "${required}" || { echo "Missing H1-04 verification input: ${required}" >&2; exit 2; }
done

dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release -m:1 --disable-build-servers
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --no-build --no-restore -c Release \
  --filter 'FullyQualifiedName~H1CatalogueTests|FullyQualifiedName~H1_04_catalogue_public_inventory_is_visible_in_jsonl_and_mcp'

mkdir -p .h1-04-effective
python3 scripts/h1-04-evidence-summary.py \
  --inventory Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json \
  --mutation Docs/evidence/WP-H1-04/EFFECTIVE_MUTATION.json \
  --public Docs/evidence/WP-H1-04/PUBLIC_CONFORMANCE.json \
  --output .h1-04-effective/static-summary.json
python3 scripts/h1-04-evidence-check.py \
  --inventory Docs/evidence/WP-H1-04/EFFECTIVE_INVENTORY.json \
  --mutation Docs/evidence/WP-H1-04/EFFECTIVE_MUTATION.json \
  --public Docs/evidence/WP-H1-04/PUBLIC_CONFORMANCE.json \
  --summary .h1-04-effective/static-summary.json

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(git status --porcelain --untracked-files=all | grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true)" ]] || { echo "H1-04 candidate changed during verification" >&2; exit 2; }
cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-H1-04
Candidate SHA: ${actual}
Executor role: WORKER
Execution environment: canonical .NET/static verifier (hosted or local)
Canonical command: scripts/h1-04-verify-exact-sha.sh ${actual}
Candidate clean before: YES
Candidate clean after: YES
Required gates: locked-restore=GREEN; release-build=GREEN; catalogue-contract-and-negative-tests=GREEN; JSONL-MCP-discovery=GREEN; committed-local-evidence-consistency=GREEN; local-Unity=EXTERNAL_EXACT_SHA_RECEIPT
Result: GREEN
Evidence: Docs/evidence/WP-H1-04/EFFECTIVE_VALIDATION.json; Docs/evidence/WP-H1-04/PROOF_MATRIX.md; exact-SHA local Unity receipt on PR #185
EOF
