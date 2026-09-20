#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${ROOT}"
CANDIDATE="$(git rev-parse HEAD)"
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "HK10 negative conformance requires a clean candidate" >&2; exit 2; }

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
old = sys.argv[2]
new = sys.argv[3]
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
    echo "HK10 defect control ${name}: infrastructure/build failure instead of causal test RED" >&2
    cat "${log}" >&2
    exit 2
  fi
  if [[ ${status} -eq 0 ]] || ! grep -Fq 'Failed!' "${log}"; then
    echo "HK10 defect control ${name}: FALSE GREEN or non-test failure" >&2
    cat "${log}" >&2
    exit 1
  fi
  echo "HK10_NEGATIVE_RED ${name}"
  restore_candidate
}

# Protocol discovery: a route without its canonical definition must not survive composition/discovery proof.
replace_once \
  src/Arkus.Harness.Runtime/CanonicalWorldContract.cs \
  'definitions.AddRange(WorldInspectionContract.CreateDefinitions());' \
  '// HK10 SEEDED DEFECT: inspection definitions omitted while routes remain.'
expect_red protocol-discovery 'FullyQualifiedName~Hk03InspectionTests'

# Canonical state/hash: input order must not alter canonical bytes/hash.
replace_once \
  src/Arkus.Game.World/CanonicalWorldStateCodec.cs \
  'objects.Sort(CompareObjects);' \
  'objects.Reverse(); // HK10 SEEDED DEFECT: input-order-dependent canonicalization'
expect_red state-hash 'FullyQualifiedName~Hk10PropertyRobustnessTests.SeededCanonicalSerializationHashAndQueriesAreDeterministicAndReproducible'

# Inspection: result ordering must remain canonical and deterministic.
replace_once \
  src/Arkus.Game.Authoring/WorldInspectionService.cs \
  'matched.Sort((left, right) => left.Id.CompareTo(right.Id));' \
  'matched.Reverse(); // HK10 SEEDED DEFECT: non-canonical query order'
expect_red inspection 'FullyQualifiedName~Hk10InspectionOrderingTests.ObjectQueryOrderMatchesIndependentOrdinalIdOracle'

# Transaction/idempotency: force the accepted receipt lookup to miss an existing request key.
replace_once \
  src/Arkus.Game.Authoring/WorldMutationService.cs \
  '_state.Receipts.TryGetValue(parsed.IdempotencyKey, out var existing)' \
  '_state.Receipts.TryGetValue(parsed.IdempotencyKey + ".hk10-seeded-miss", out var existing)'
expect_red transaction-idempotency 'FullyQualifiedName~Hk04TransactionalMutationTests'

# Validation: suppress owned violations at the engine boundary.
replace_once \
  src/Arkus.Game.Validation/WorldValidation.cs \
  'var violations = WorldStateValidator.ValidateCandidate(candidate);' \
  'var violations = new List<WorldStateViolation>(); // HK10 SEEDED DEFECT: validator blind spot'
expect_red validation 'FullyQualifiedName~Hk05ValidationDiagnosticsTests'

# Provenance/replay: stop binding the supplied journal current anchor to the final chain result.
replace_once \
  src/Arkus.Game.Authoring/WorldReplay.cs \
  'if (!SameAnchor(expectedBase, currentAnchor!))' \
  'if (false && !SameAnchor(expectedBase, currentAnchor!)) // HK10 SEEDED DEFECT'
expect_red provenance-replay 'FullyQualifiedName~Hk06CReplayTests'

# Host framing: drift the frozen JSONL v1 frame ceiling.
replace_once \
  src/Arkus.Harness.Cli/ReferenceTransport.cs \
  'internal const int MaximumFrameBytes = 1024 * 1024;' \
  'internal const int MaximumFrameBytes = (1024 * 1024) + 1; // HK10 SEEDED DEFECT'
expect_red host-framing 'FullyQualifiedName~Hk07AReferenceTransportTests|FullyQualifiedName~Hk09BTransportConformanceTests'

# HK08A batching/efficiency: drift the accepted coherent transaction shape away from 96 operations.
replace_once \
  src/Arkus.Harness.Protocol/H0ResourceEnvelope.cs \
  'public const int MaximumBatchOperations = 96;' \
  'public const int MaximumBatchOperations = 95; // HK10 SEEDED DEFECT'
expect_red hk08a-batch-shape 'FullyQualifiedName~Hk08AInteractionShapeTests|FullyQualifiedName~Hk10EnduranceCompatibilityTests.ProtocolV1CompatibilityCorpusMatchesAcceptedRuntimeAndResourceEnvelope'

# HK08B stale-plan recovery: misanchor the request's expected lineage to the current state.
replace_once \
  src/Arkus.Harness.Runtime/WorldConflictRecovery.cs \
  '["expected"] = ExpectedData(expectedRevision, expectedHash),' \
  '["expected"] = ExpectedData(current.Revision, current.Hash), // HK10 SEEDED DEFECT'
expect_red hk08b-stale-recovery-anchor 'FullyQualifiedName~Hk08BConflictRecoveryTests'

# HK09A capability containment: deliberately admit elevated privilege.
replace_once \
  src/Arkus.Harness.Runtime/H0HostCapabilityPolicy.cs \
  'if (policy.Privilege == PrivilegeClass.Elevated)' \
  'if (false && policy.Privilege == PrivilegeClass.Elevated) // HK10 SEEDED DEFECT'
expect_red hk09a-capability-containment 'FullyQualifiedName~Hk09AHostCapabilityContainmentTests'

# HK09B persistence/fault boundary: ignore a publication-interruption permit.
replace_once \
  src/Arkus.Harness.Protocol/H0ResourceEnvelope.cs \
  'if (_publicationPermit != null && !_publicationPermit(boundary))' \
  'if (false && _publicationPermit != null && !_publicationPermit(boundary)) // HK10 SEEDED DEFECT'
expect_red hk09b-publication-interruption 'FullyQualifiedName~Hk09BResourcePersistenceTests'

# HK09B bounded-session/resource growth: allow the public session ceiling to drift upward undetected by runtime callers.
replace_once \
  src/Arkus.Harness.Protocol/H0ResourceEnvelope.cs \
  'public const int MaximumSessionTransactions = 10000;' \
  'public const int MaximumSessionTransactions = 10001; // HK10 SEEDED DEFECT'
expect_red hk09b-session-envelope-drift 'FullyQualifiedName~Hk10EnduranceCompatibilityTests.ProtocolV1CompatibilityCorpusMatchesAcceptedRuntimeAndResourceEnvelope'

# The real candidate must still be exactly the original tree after all disposable mutations.
restore_candidate
[[ -z "$(git status --porcelain --untracked-files=all)" ]] || { echo "Disposable mutation worktree did not restore cleanly" >&2; exit 2; }
echo 'HK10_NEGATIVE_CONFORMANCE GREEN red_controls=12'
