#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

BASELINE_SHA="159b85f8352ca5df2aa5b3c714ad5dc94b76d64d"
PROJECTION_DIGEST="516f51930124e0e77e6f767af0c8074c8384723c809c0bd50571ff44d80b1858"
PROJECTION_IDENTITY="ctx-dw-h1-01-adapter-v1:${PROJECTION_DIGEST}"

actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: expected ${expected}, observed ${actual}" >&2; exit 2; }

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}
[[ -z "$(candidate_dirty_status)" ]] || { echo "CTX-DW-H1-02 candidate is not clean before verification" >&2; candidate_dirty_status >&2; exit 2; }

for required in \
  Docs/workpacks/CTX/WP-CTX-DW-H1-02.md \
  Docs/evidence/WP-CTX-DW-H1-02/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_CASES.json \
  Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_DISPOSITION.md \
  Docs/evidence/WP-CTX-DW-H1-02/PROOF_MATRIX.md \
  Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md \
  Docs/evidence/WP-H1-05/PROOF_MATRIX.md \
  Docs/evidence/WP-H1-06/PROOF_MATRIX.md \
  Docs/workpacks/H1/WP-H1-07.md \
  Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md \
  src/Arkus.DesignWorld/H1CatalogueProjection.cs \
  src/Arkus.DesignWorld/H1ProjectionLifecycle.cs \
  tests/Arkus.Harness.Tests/CtxDwH102SelectiveAdoptionTests.cs \
  scripts/ctx-dw-h1-02-proof.py; do
  test -f "${required}" || { echo "Missing CTX-DW-H1-02 verification input: ${required}" >&2; exit 2; }
done

# This adoption WP may not repair or expand product/projection implementation to improve its result.
allowed_change() {
  case "$1" in
    Docs/workpacks/CTX/WP-CTX-DW-H1-02.md|\
    Docs/evidence/WP-CTX-DW-H1-02/*|\
    tests/Arkus.Harness.Tests/CtxDwH102SelectiveAdoptionTests.cs|\
    scripts/ctx-dw-h1-02-proof.py|\
    scripts/ctx-dw-h1-02-verify-exact-sha.sh|\
    scripts/arkus-verify-exact-sha.sh) return 0 ;;
    *) return 1 ;;
  esac
}
while IFS= read -r changed; do
  [[ -z "${changed}" ]] && continue
  allowed_change "${changed}" || { echo "Out-of-scope CTX-DW-H1-02 change: ${changed}" >&2; exit 1; }
done < <(git diff --name-only "${BASELINE_SHA}" "${actual}")

catalogue_blob="$(git hash-object Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json)"
adoption_blob="$(git hash-object Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json)"
[[ "${catalogue_blob}" == "922dbdffbe2f0a622acc2e153ca8b181427f6ce6" ]] || { echo "Frozen H1-04 catalogue blob mismatch: ${catalogue_blob}" >&2; exit 1; }
[[ "${adoption_blob}" == "664e83e25269f345a248ce43410a28ed0a670750" ]] || { echo "Frozen H1-04 source-adoption blob mismatch: ${adoption_blob}" >&2; exit 1; }

grep -Fq -- "- Projection.Digest: \`${PROJECTION_DIGEST}\`" Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md
grep -Fq -- "- ProjectionIdentity: \`${PROJECTION_IDENTITY}\`" Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md
grep -Fq 'AcceptedH104CandidateSha = "8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5"' src/Arkus.DesignWorld/H1CatalogueProjection.cs
grep -Fq 'new DesignProjectionVersion(1, "ctx-dw-h1-01-v1")' src/Arkus.DesignWorld/H1CatalogueProjection.cs

# Product progress remains independent from this optional routing checkpoint.
grep -Fq 'Depends on: `WP-H1-06` PASS' Docs/workpacks/H1/WP-H1-07.md
! grep -Fq 'WP-CTX-DW-H1-02' Docs/workpacks/H1/WP-H1-07.md
# The mandatory gate trial must remain a private-context-free public-client test.
grep -Fq 'Do **not** pre-seed that trial with Juego2-private CTX capsules, DW projections' Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md

grep -Fq 'PREDECESSOR_CONTRACT_CHECK: PASS' Docs/evidence/WP-CTX-DW-H1-02/PREDECESSOR_CONTRACT_CHECK.md
grep -Fq 'PROOF_MATRIX_VERDICT: READY' Docs/evidence/WP-CTX-DW-H1-02/PROOF_MATRIX.md
grep -Fq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-CTX-DW-H1-02/PROOF_MATRIX.md
grep -Fq 'ADOPTION_DISPOSITION: READY_FOR_EXACT_SHA_VALIDATION' Docs/evidence/WP-CTX-DW-H1-02/ADOPTION_DISPOSITION.md

observation="${ROOT}/artifacts/observed/ctx-dw-h1-02-projection-observation.json"
summary="${ROOT}/artifacts/observed/ctx-dw-h1-02-proof-summary.json"
rm -f "${observation}" "${summary}"
mkdir -p "$(dirname "${observation}")"
export CTX_DW_H1_02_OBSERVATION_OUTPUT="${observation}"

dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release -m:1 --disable-build-servers
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --no-build --no-restore -c Release \
  --filter 'FullyQualifiedName~CtxDwH102SelectiveAdoptionTests|FullyQualifiedName~CtxDwH101ProjectionIdentityTests|FullyQualifiedName~CtxDwH101ProjectionLifecycleTests'

test -s "${observation}" || { echo "H1-02 projection observation was not emitted" >&2; exit 1; }
python3 scripts/ctx-dw-h1-02-proof.py \
  --projection-observation "${observation}" \
  --summary-output "${summary}"
test -s "${summary}" || { echo "H1-02 proof summary was not emitted" >&2; exit 1; }

summary_value() {
  local expression="$1"
  python3 - "${summary}" "${expression}" <<'PY'
import json, sys
path, expression = sys.argv[1], sys.argv[2]
value = json.load(open(path, encoding="utf-8"))
for part in expression.split('.'):
    value = value[part]
print(value)
PY
}

use_count="$(summary_value classifications.USE)"
optional_count="$(summary_value classifications.OPTIONAL)"
not_material_count="$(summary_value classifications.NOT_MATERIAL)"
max_query_bytes="$(summary_value max_use_query_bytes)"
authority_bytes="$(summary_value catalogue_authority_bytes)"
summary_projection="$(summary_value projection_identity)"
[[ "${use_count}" -ge 1 && "${optional_count}" -ge 1 && "${not_material_count}" -ge 1 ]] || { echo "Selective class coverage collapsed" >&2; exit 1; }
[[ "${max_query_bytes}" -lt "${authority_bytes}" ]] || { echo "Selective USE query did not reduce initial bytes" >&2; exit 1; }
[[ "${summary_projection}" == "${PROJECTION_IDENTITY}" ]] || { echo "Proof summary projection identity mismatch" >&2; exit 1; }

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "CTX-DW-H1-02 candidate changed during verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-CTX-DW-H1-02
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-canonical-dotnet-python-shell}
Canonical command: scripts/ctx-dw-h1-02-verify-exact-sha.sh ${actual}
Accepted H1-01 projection identity: ${PROJECTION_IDENTITY}
Accepted H1-05 candidate: 186224fbc3f53eb9c47ae528a56dcf3514af163f
Accepted H1-06 candidate: 96de260021fb28ff2cc7da8d2ef488be419568b3
Catalogue blob: ${catalogue_blob}
Source-adoption blob: ${adoption_blob}
Disposition counts: USE=${use_count}; OPTIONAL=${optional_count}; NOT_MATERIAL=${not_material_count}
Max admitted selective query bytes: ${max_query_bytes}
Catalogue authority bytes: ${authority_bytes}
Required gates: exact-checkout=GREEN; bounded-scope=GREEN; predecessor-identity=GREEN; frozen-h1-authority=GREEN; locked-restore=GREEN; release-build=GREEN; real-projection-observation=GREEN; stale-lifecycle-control=GREEN; source-open=GREEN; mandatory-read-preservation=GREEN; capability-inflation-control=GREEN; label-independence=GREEN; partial-product-oracle=GREEN; h1-07-selective-disposition=GREEN; h1-gate-isolation=GREEN; product-dependency-independence=GREEN; navigation-measure=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-CTX-DW-H1-02
EOF
