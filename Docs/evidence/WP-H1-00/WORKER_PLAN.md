# WP-H1-00 Worker plan

WP: `WP-H1-00`
Worker state: `ACTIVE`
Baseline SHA: `a9ff655e5bf2319690d919b88bd57389a32483f3`
Baseline branch: `main`
Implementation branch: `wp-h1-00-engine-neutral-projection`
Class: `FOUNDATIONAL`
Execution requirement: `REMOTE_OK`

## Objective

Implement the engine-neutral, versioned projection state machine and deterministic non-Unity reference materializer required by `Docs/workpacks/H1/WP-H1-00.md`, while preserving H0 canonical authority and the accepted H1 identity/generational-projection decisions.

## Ownership / write set

Owned for this candidate:

- new engine-neutral bridge contract/reference implementation under `src/Arkus.EngineBridge/`;
- solution/test-project wiring and focused executable proof under `tests/Arkus.Harness.Tests/`;
- H1-00 evidence under `Docs/evidence/WP-H1-00/`;
- H1-00 status/handoff metadata only when the candidate is frozen.

Not owned: Unity/editor/runtime implementation; canonical H0 mutation semantics/public H0 contracts; asset-catalogue implementation; gameplay/CITY content; H1-01+ implementation.

CITY-01 merged to `main` immediately before Worker activation. This plan is deliberately rebased onto that merge; CITY files are outside the H1-00 write set and are consumed only as concurrent repository state.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted dependency: `WP-HK-GATE`.

- Reviewed candidate SHA: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`.
- Independent verdict: `PASS`, review `#5261636151`.
- Merge SHA: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca`.
- Accepted H1 planning/DocSync is present on the baseline; current baseline is `a9ff655e5bf2319690d919b88bd57389a32483f3`.

Inherited guarantees consumed by H1-00:

1. canonical authored state, hashing/snapshot/replay and mutation/CAS semantics remain H0 authority;
2. canonical public contracts remain engine/transport neutral;
3. rejected canonical mutation cannot partially publish canonical state;
4. H0 host/resource closure and JSONL/MCP parity are accepted and not re-proved;
5. H0 exposes a clean downstream engine-bridge boundary.

H1-00 newly owns:

1. portable/versioned materialization input tuple: canonical anchor, binding version, catalogue fingerprint, bridge/toolchain profile;
2. deterministic normalized plan, generation identity, observation, receipt and drift semantics;
3. staged publication only after validation/observation success;
4. deterministic/idempotent retry in a non-Unity reference materializer;
5. explicit `absent`, `in-sync`, `canonical-ahead`, `engine-drift`, `missing-dependency`, `ambiguous`, `failed` observations;
6. proof bridge actions do not mutate supplied canonical snapshot bytes/hash;
7. bounded plaza/market/bar/workshop content-shape probe with two logical asset dependencies.

Inherited guarantees intentionally consumed rather than re-proved: H0 validation completeness, journal/snapshot/replay, canonical atomic publication, full transport conformance, and H0 containment/resource closure.

Concrete predecessor reopen trigger: executable evidence that an accepted canonical snapshot/anchor cannot be consumed without redefining H0 semantics, or a required public projection cannot pass the neutral bridge contract. No such contradiction is currently known.

## Binding H1 decisions

- canonical identity remains canonical; catalogue IDs are bridge/project-owned logical references (`ADR-H1-001`);
- materialization is external-reversible and generational: stage, validate/observe, publish; failure preserves prior active generation (`ADR-H1-002`);
- H1-00 contains no Unity types and no Editor/process lifecycle implementation.

## Claim / trust boundary

Within the pure in-memory/fileless reference materializer and portable contracts, the complete input tuple deterministically governs plan/generation/observation/receipt semantics; failed staging cannot change active state; supplied canonical bytes are never mutated; effective managed-resource drift is diagnosed explicitly.

Trusted base: accepted H0 guarantees; normal .NET collection/string/UTF-8/SHA-256 behaviour; exact Git identity; pinned build/test toolchain; normal process memory. Unity/editor behaviour, crash/power-loss durability, real catalogue completeness and H1-01+ dispatch are outside the claim.

## Planned proof

The executable scenario covers deterministic ordering/digests, independent tuple changes, first publish + idempotent retry, failed staging preservation, all seven observation states, extra/missing/changed managed-resource drift, missing logical dependencies, canonical-byte immutability and assembly-reference/type leakage checks. The bounded content-shape probe uses plaza/market/bar/workshop only for hierarchy/dependency granularity.

## Proof budget

Keep product code to one neutral assembly plus focused tests/evidence. No new package, workflow, engine project or transport capability is planned.
