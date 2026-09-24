#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"

actual="$(git rev-parse HEAD)"
expected="${1:-${CANDIDATE_SHA:-${actual}}}"
[[ "${expected}" =~ ^[0-9a-fA-F]{40}$ ]] || { echo "Invalid candidate SHA: ${expected}" >&2; exit 2; }
[[ "${actual}" == "${expected}" ]] || { echo "Candidate SHA mismatch: expected ${expected}, observed ${actual}" >&2; exit 2; }

candidate_dirty_status() {
  git status --porcelain --untracked-files=all | \
    grep -Ev '^\?\? (VALIDATION_CONTEXT\.json|validation\.log|EXECUTION_RECEIPT\.txt|artifacts/observed/.*)$' || true
}
[[ -z "$(candidate_dirty_status)" ]] || { echo "CTX-DW-H1-01 candidate is not clean before verification" >&2; candidate_dirty_status >&2; exit 2; }

for required in \
  Docs/evidence/WP-CTX-DW-H1-01/PREDECESSOR_CONTRACT_CHECK.md \
  Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md \
  Docs/evidence/WP-CTX-DW-H1-01/PROOF_MATRIX.md \
  src/Arkus.DesignWorld/H1CatalogueProjection.cs \
  src/Arkus.DesignWorld/H1SourceAuthorityOracle.cs \
  tests/Arkus.Harness.Tests/CtxDwH101ProjectionLifecycleTests.cs \
  tests/Arkus.Harness.Tests/CtxDwH101ProjectionIdentityTests.cs \
  Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json \
  Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json; do
  test -f "${required}" || { echo "Missing CTX-DW-H1-01 verification input: ${required}" >&2; exit 2; }
done

catalogue_blob="$(git hash-object Unity/ArkusUnity/Assets/Arkus/H1/CatalogueMapping.json)"
adoption_blob="$(git hash-object Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json)"
[[ "${catalogue_blob}" == "922dbdffbe2f0a622acc2e153ca8b181427f6ce6" ]] || { echo "Frozen H1-04 catalogue blob mismatch: ${catalogue_blob}" >&2; exit 1; }
[[ "${adoption_blob}" == "664e83e25269f345a248ce43410a28ed0a670750" ]] || { echo "Frozen H1-04 source-adoption blob mismatch: ${adoption_blob}" >&2; exit 1; }

grep -Fq 'AcceptedH104CandidateSha = "8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5"' src/Arkus.DesignWorld/H1CatalogueProjection.cs
grep -Fq 'AdapterId = "ctx-dw-h1-01-adapter-v1"' src/Arkus.DesignWorld/H1CatalogueProjection.cs
grep -Fq 'LifecycleId = "ctx-dw-h1-01-lifecycle-v1"' src/Arkus.DesignWorld/H1CatalogueProjection.cs
grep -Fq 'new DesignProjectionVersion(1, "ctx-dw-h1-01-v1")' src/Arkus.DesignWorld/H1CatalogueProjection.cs

lifecycle=Docs/evidence/WP-CTX-DW-H1-01/LIFECYCLE.md
[[ "$(grep -c '^- Projection\.Digest: `' "${lifecycle}")" -eq 1 ]] || { echo "Durable evidence must publish exactly one Projection.Digest" >&2; exit 1; }
[[ "$(grep -c '^- ProjectionIdentity: `' "${lifecycle}")" -eq 1 ]] || { echo "Durable evidence must publish exactly one ProjectionIdentity" >&2; exit 1; }
published_digest="$(sed -n 's/^- Projection\.Digest: `\([0-9a-f]\{64\}\)`$/\1/p' "${lifecycle}")"
published_identity="$(sed -n 's/^- ProjectionIdentity: `\(ctx-dw-h1-01-adapter-v1:[0-9a-f]\{64\}\)`$/\1/p' "${lifecycle}")"
[[ "${published_digest}" =~ ^[0-9a-f]{64}$ ]] || { echo "Invalid durable Projection.Digest: ${published_digest}" >&2; exit 1; }
[[ "${published_identity}" == "ctx-dw-h1-01-adapter-v1:${published_digest}" ]] || { echo "Durable ProjectionIdentity does not bind adapter to published digest" >&2; exit 1; }
! grep -Fq '<DesignWorldProjection.Digest>' "${lifecycle}" || { echo "Symbolic projection identity placeholder remains in durable lifecycle evidence" >&2; exit 1; }

identity_probe="${ROOT}/artifacts/observed/ctx-dw-h1-01-projection-identity.txt"
rm -f "${identity_probe}"
mkdir -p "$(dirname "${identity_probe}")"
export CTX_DW_H1_01_IDENTITY_OUTPUT="${identity_probe}"

dotnet restore Juego2.sln --locked-mode
dotnet build Juego2.sln --no-restore -c Release -m:1 --disable-build-servers
dotnet test tests/Arkus.Harness.Tests/Arkus.Harness.Tests.csproj --no-build --no-restore -c Release \
  --filter 'FullyQualifiedName~CtxDwH101ProjectionLifecycleTests|FullyQualifiedName~CtxDwH101ProjectionIdentityTests'

test -s "${identity_probe}" || { echo "Projection identity probe was not materialized by the focused rebuild test" >&2; exit 1; }
projection_digest="$(sed -n 's/^Projection\.Digest=//p' "${identity_probe}")"
projection_identity="$(sed -n 's/^ProjectionIdentity=//p' "${identity_probe}")"
reverse_projection_identity="$(sed -n 's/^ReverseProjectionIdentity=//p' "${identity_probe}")"
[[ "${projection_digest}" =~ ^[0-9a-f]{64}$ ]] || { echo "Invalid computed projection digest: ${projection_digest}" >&2; exit 1; }
[[ "${projection_identity}" == "ctx-dw-h1-01-adapter-v1:${projection_digest}" ]] || { echo "Computed ProjectionIdentity does not bind adapter to digest" >&2; exit 1; }
[[ "${reverse_projection_identity}" == "${projection_identity}" ]] || { echo "Reverse enumeration changed concrete ProjectionIdentity" >&2; exit 1; }

projection_identity_matches() {
  local expected_digest="$1"
  local expected_identity="$2"
  local computed_digest="$3"
  local computed_identity="$4"
  [[ "${expected_digest}" == "${computed_digest}" && "${expected_identity}" == "${computed_identity}" ]]
}

projection_identity_matches "${published_digest}" "${published_identity}" "${projection_digest}" "${projection_identity}" || {
  echo "Published projection identity does not match deterministic rebuild" >&2
  exit 1
}

# Causal falsification: changing only the published expectation must make the same rebuild RED.
tampered_published_identity="tampered:${published_identity}"
if projection_identity_matches "${published_digest}" "${tampered_published_identity}" "${projection_digest}" "${projection_identity}"; then
  echo "Unilateral published-identity tamper did not turn verification RED" >&2
  exit 1
fi

# H1 meaning must remain adapter-owned, never promoted into the generic DW/H0 kernel.
! grep -Eqi 'quaternius|h1-catalogue|h1-source' src/Arkus.DesignWorld/DesignWorldContracts.cs src/Arkus.DesignWorld/DesignWorldProjection.cs

# The next H1 product work remains source-authoritative and cannot be blocked by this optional projection.
grep -Fq 'Depends on: `WP-H1-04` PASS' Docs/workpacks/H1/WP-H1-05.md
! grep -Fq 'Depends on: `WP-CTX-DW-H1-01`' Docs/workpacks/H1/WP-H1-05.md
grep -Fq 'Canonical state remains authoritative' Docs/workpacks/H1/WP-H1-05.md

# Durable proof/evidence contracts must describe a complete fail-closed lifecycle.
grep -Fq 'PREDECESSOR_CONTRACT_CHECK: PASS' Docs/evidence/WP-CTX-DW-H1-01/PREDECESSOR_CONTRACT_CHECK.md
grep -Fq 'LIFECYCLE_VERDICT: READY' "${lifecycle}"
grep -Fq 'PROOF_MATRIX_VERDICT: READY' Docs/evidence/WP-CTX-DW-H1-01/PROOF_MATRIX.md
grep -Fq 'UNRESOLVED_PROOF_OBLIGATIONS: 0' Docs/evidence/WP-CTX-DW-H1-01/PROOF_MATRIX.md

if [[ -n "${PR_BODY:-}" ]]; then
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Candidate HEAD SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq "^Frozen candidate SHA:[[:space:]]*\`?${actual}\`?[[:space:]]*$"
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Worker pre-review:[[:space:]]*CLEAN[[:space:]]*$'
  printf '%s\n' "${PR_BODY}" | grep -Eq '^Branch frozen:[[:space:]]*YES[[:space:]]*$'
fi

[[ -z "$(candidate_dirty_status)" ]] || { echo "CTX-DW-H1-01 candidate changed during verification" >&2; candidate_dirty_status >&2; exit 2; }

cat <<EOF
EXECUTION_RECEIPT_V1
WP: WP-CTX-DW-H1-01
Candidate SHA: ${actual}
Executor role: ${ARKUS_EXECUTOR_ROLE:-WORKER}
Execution environment: ${ARKUS_EXECUTION_SUBSTRATE:-canonical-dotnet-shell}
Canonical command: scripts/ctx-dw-h1-01-verify-exact-sha.sh ${actual}
Accepted H1-04 candidate: 8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5
Catalogue blob: ${catalogue_blob}
Source-adoption blob: ${adoption_blob}
Adapter: ctx-dw-h1-01-adapter-v1
Projection schema: ctx-dw-h1-01-v1
Lifecycle: ctx-dw-h1-01-lifecycle-v1
Published projection digest: ${published_digest}
Published projection identity: ${published_identity}
Computed projection digest: ${projection_digest}
Computed projection identity: ${projection_identity}
Reverse projection identity: ${reverse_projection_identity}
Required gates: exact-checkout=GREEN; frozen-h1-authority=GREEN; locked-restore=GREEN; release-build=GREEN; focused-lifecycle-and-negative-tests=GREEN; published-projection-identity=GREEN; identity-tamper-control=GREEN; reverse-identity=GREEN; generic-kernel-boundary=GREEN; h1-05-authority-fallback=GREEN; durable-proof=GREEN; frozen-metadata=GREEN
Result: GREEN
Evidence: Docs/evidence/WP-CTX-DW-H1-01
EOF
