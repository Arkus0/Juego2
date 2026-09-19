# WP-HK-06B Worker plan

Baseline SHA: `5281160ce4eff601ab52dfb568fd4e4976560383`
Branch: `wp/hk-06b-semantic-diff-snapshot`
Worker state: ACTIVE

## Objective

Implement HK06B only: deterministic semantic diff plus versioned canonical snapshot export/import for the accepted authored `WorldState`, without journal replay, transport framing, Unity serialization, gameplay state, or runtime-observation capture.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-06A`.

- Reviewed candidate: `4ed9a925791ae14b0b5c0d92625161504021542e`.
- Independent review: PASS (`#5257682288`).
- Implementation merge SHA: `8089a52e8a7bbdde46e97705df6906c0d38d593a`.
- Post-merge DocSync SHA: `ab5872879d07103d71a7010738ba881bdf3e6d7e`.

Inherited guarantees consumed rather than re-proved:

1. HK01/HK04 expose one accepted canonical mutation route/commit authority for ordinary authored changes, with plan/dry-run/apply, CAS and idempotency.
2. HK02/HK02A define the finite canonical authored `WorldState`, stable object/extension identities, canonical serialization/hash, references, opaque extension payloads and typed dependencies.
3. HK03 inspection is state-neutral and HK05 validates complete candidate state before ordinary mutation commit.
4. HK06A publishes truthful mutation provenance from an explicit lineage base; state, receipts and journal are one immutable aggregate under the session gate; runtime observations are explicitly outside authored state.
5. The HK06A journal schema belongs to HK06A and is not redefined here. Replay interpretation remains HK06C.

HK06B newly owns:

1. Semantic diff over accepted authored resources, independent of canonical serializer text/order and deterministic by accepted resource identity.
2. Field-level change reporting for object type/container/references and extension payload/dependencies, including added/removed resources.
3. A separately versioned snapshot artifact that is complete enough to reconstruct the exact canonical authored hash.
4. Fail-closed snapshot import: validate artifact schema/version, embedded canonical state and declared anchor/hash before any replacement.
5. Explicit import/history semantics: successful import starts a new local lineage rooted at the imported authored state, with empty local mutation receipts/journal; neither imported nor prior mutation history is fabricated as continuity.
6. Canonical discoverability for diff/snapshot surfaces while preserving the authored/live boundary.

Predecessor reopen trigger: concrete effective evidence that a field included in accepted canonical `WorldState` is not covered by HK02/HK02A serialization/hash, that ordinary public mutation can persist outside HK04/HK06A authority, or that HK06A journal publication is not the accepted aggregate described by its PASS evidence. No such evidence is currently present.

## Architecture decision

Snapshot import is a **session rebase**, not a synthetic `authoring.change.apply` mutation and not replay. It validates a complete external authored-state artifact first, then atomically replaces the session aggregate and establishes the imported state as the new local provenance base with empty receipts/journal. Subsequent ordinary edits continue through the accepted HK04 mutation authority. This avoids inventing an HK06A journal entry whose schema cannot truthfully describe snapshot import.

Diff ignores revision-only/serialization-order differences because revision is lineage/anchor metadata rather than an authorable resource value; snapshot preservation still retains revision because the canonical hash includes it.

## Implementation surfaces

Expected product changes:

- `src/Arkus.Game.Authoring/WorldPortability*.cs`: semantic diff, snapshot artifact parsing/export/import contract and service boundary.
- `src/Arkus.Game.Authoring/WorldMutationService.cs`: narrow session-rebase integration under the existing aggregate gate and dynamic lineage base.
- `src/Arkus.Harness.Runtime/*`: canonical definitions/routes/discovery bindings for HK06B.
- `tests/Arkus.Harness.Tests/Hk06B*.cs`: positive, negative-conformance, independent-oracle and content-shape evidence.
- exact-SHA observation/verification scripts and evidence under `Docs/evidence/WP-HK-06B/`.

## Proof plan

Positive/effective proof:

- deterministic semantic added/removed/changed resources across nontrivial states;
- reorder-only reference/dependency/object/extension representation yields no semantic diff;
- object and extension field changes remain visible and deterministically ordered;
- export → clean import reconstructs same canonical hash and semantic state;
- successful import resets local lineage base and leaves the local journal empty;
- public canonical discovery exposes versioned diff/export/import schemas.

Causal negative-conformance classes:

- hidden authored resource change omitted from diff;
- serializer/order-only change falsely reported;
- altered snapshot payload or declared anchor accepted as same state;
- unsupported snapshot schema/version accepted;
- invalid snapshot partially replaces current state;
- runtime-only observation leaks into snapshot;
- import fabricates or retains misleading local mutation history.

Independent oracle:

A test-owned semantic projection will compare accepted authored resource fields directly, without calling the production diff builder. Snapshot truth will be checked by independently deserializing the embedded canonical state and recomputing its canonical hash before/after import.

## Proof/trust boundary

Trusted base remains the accepted foundational standard: pinned .NET/MSBuild/NuGet behavior, Git checkout/object semantics, normal process memory/locking and SHA-256 used according to contract. HK06B does not claim durable storage, transport framing, replay, arbitrary filesystem/network isolation or deterministic simulation.

Initial `PROOF_BUDGET_VERDICT`: `WITHIN_BUDGET`; one semantic comparator and one snapshot/session-rebase seam are sufficient for the WP claim. If support machinery begins expanding beyond these owned seams without a concrete acceptance gap, re-audit before freeze.
