# WP-HK-08A Worker plan

Baseline SHA: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`
Branch: `wp/hk-08a-efficient-interaction`
Worker state: ACTIVE

## Objective

Implement HK08A only: make ordinary AI authoring efficient through the already accepted canonical/neutral transport stack by proving and, where required, extending atomic multi-operation authoring, compact/bounded inspection, deterministic pagination and useful cost discovery. Preserve HK04 atomicity, HK05 validation, HK06A provenance and HK07A/HK07B transport neutrality.

HK08A will not add stale-CAS recovery semantics, automatic merge, per-resource/distributed locking, agent orchestration, final interaction budgets, model-vendor prompt logic, gameplay systems or speculative logical transactions across persisted batches. Those boundaries remain HK08B/post-GATE ownership.

## Ownership and baseline

- Current `main` and Worker baseline: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`.
- The baseline contains accepted HK07B plus its post-PASS DocSync (`#49`).
- No open HK08A PR existed when this Worker started.
- Canonical implementation branch: `wp/hk-08a-efficient-interaction`.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-07B`.

- Reviewed frozen candidate: `37ea185f28dd28e41b406f2c9407fdfe6f9b752b`.
- Independent review: PASS (`#5259854121`).
- Exact-SHA freeze validation: Actions `35495563546` GREEN; artifact `10600393826`.
- Implementation merge SHA: `58b6571b96eac4c73e5c3cf28a42a6630ea13505`.
- Post-PASS DocSync: PR `#49`, contained in the exact baseline above.

Inherited guarantees consumed rather than re-proved:

1. `arkus.neutral-projection@1` is the accepted transport-neutral invocation/outcome boundary; JSONL and MCP are sibling projections and cannot redefine canonical semantics.
2. `ComposedContract.Definitions` remains the single composed capability/schema inventory and canonical dispatcher authority; scoped providers project generically.
3. HK07B already proves MCP discovery/invocation completeness, bounded MCP naming, SDK isolation and representative equivalence with JSONL for the semantics that existed before HK08A.
4. HK03 already provides bounded, revision/hash-anchored world queries, deterministic cursors and compact object field projection. HK08A consumes those primitives unless concrete effective evidence shows a defect in their accepted boundary.
5. HK04 already provides one canonical transaction containing multiple mutation operations; candidate validation precedes publication and publication is all-or-nothing.
6. HK05 validation, HK06A journal provenance, HK06B snapshot/diff/rebase and HK06C replay remain accepted semantic authorities below the transport layer.

HK08A newly owns:

1. Evidence-driven atomic batch capacity/shape for a representative coherent multi-resource authoring intent. The current 64-operation cap is treated as provisional, not as a product semantic constant.
2. Preservation of one persisted transaction/journal entry for that coherent edit; HK08A may adjust the single-request envelope but will not introduce multi-plan transaction machinery.
3. Bounded/paginated HK06A journal reading with deterministic ordering/completeness and fail-closed stale-anchor/cursor behavior while preserving journal meaning.
4. Explicit machine-discoverable interaction-cost classification derived from canonical metadata, alongside canonical side-effect semantics, so clients can distinguish cheap reads, expensive reads and mutations without source-code knowledge.
5. Cross-transport conformance for every HK08A-added/changed primitive, including batching, compact/bounded reads, journal pagination and discovery metadata.
6. Causal negative-conformance controls for partial commit, unjustified batch splitting, validation/provenance omission, compact required-field loss, pagination duplicate/omission/stale continuation, pathological ordinary per-field request shape and JSONL/MCP drift.
7. A bounded representative content-shape probe under `FOUNDATIONAL_PROOF_STANDARD.md` because HK08A changes public interaction semantics.

Predecessor reopen trigger: concrete effective evidence that an accepted HK03/HK04/HK05/HK06A/HK07 guarantee does not hold on the effective HK08A path (for example an accepted cursor duplicates/omits at one anchor, the existing multi-operation mutation path can partially publish, or a transport changes newly added canonical meaning). A desire for duplicate defence-in-depth is not a reopen condition. No such contradiction is currently present.

## Architecture decision

### Atomic batching

Keep one canonical `authoring.change.*` request as the transaction boundary. Use the existing ordered `operations` array and accepted HK04 commit path; adjust only the bounded request capacity if the representative product-shaped probe proves the provisional 64-operation ceiling would split one coherent intent. Any accepted increase remains an implementation envelope to be bounded again by HK09B resource evidence, not a claim of unbounded work.

### Bounded journal read

Extend the existing canonical `authoring.journal.read` surface rather than inventing a second provenance registry. Pagination will use deterministic sequence order and an opaque cursor bound to the same authored revision/hash plus page context. Page responses retain total journal count and lineage anchors while returning only a bounded `entries` slice and optional `nextCursor`. Cursor/anchor mismatch fails closed and does not return a page from the wrong state.

### Discovery cost

Canonical `CostSemantics` remains the authority. HK08A will make a stable coarse interaction tier mechanically discoverable from that canonical metadata while retaining `relativeWeight`; mutation/read identity continues to come from canonical `sideEffect`. JSONL and MCP merely project the same definition data.

### Compact/bounded reads

Reuse the accepted HK03 anchored query/field-projection/chunk surfaces and prove them through the public clients. Do not add duplicate convenience APIs merely to reduce call count where an existing coherent selector/projection already expresses the intent.

## Proof plan

Positive/effective evidence:

- a representative product-shaped coherent multi-resource edit fits one accepted mutation request, dry-runs and applies atomically, advances one revision and produces exactly one matching HK06A provenance entry;
- the same representative request yields equivalent semantic outcomes through JSONL and MCP;
- journal pages are bounded, ordered and reconstruct exactly the complete accepted journal at one anchor without duplicates/omissions;
- a page cursor cannot continue after the authored anchor changes;
- compact object projections omit unrequested optional fields while retaining required identity/world anchor fields;
- discovery exposes stable coarse cost tier + relative weight + side-effect classification from the canonical definition through both transports;
- an ordinary create/inspect/modify flow uses coherent request shapes rather than one call per property.

Causal negative-conformance classes:

- force one operation in a batch to fail after earlier candidate edits and prove no partial canonical publication/journal append;
- reproduce the representative coherent edit against the old provisional 64-operation envelope and show the guard would reject/split it, then prove the accepted envelope handles it in one transaction;
- defect-inject a batch route that skips accepted validation/provenance and make the conformance oracle red;
- remove one required compact response field and make schema/conformance fail;
- alter pagination offset/order/cursor anchor and make reconstruction/stale controls red;
- replace the coherent mutation shape with per-property calls and make the ordinary-flow guard red;
- drift one HK08A definition/result between JSONL and MCP and make cross-transport comparison red.

Independent/effective oracles:

- representative operation count comes from the bounded content-shape probe, not from `MaximumOperations` itself;
- accepted canonical definitions are independently composed and compared to transport discovery;
- journal reconstruction compares page sequence/entry identity against the authoritative persisted journal semantics and final anchor, not against the cursor implementation itself;
- transaction/provenance observations inspect effective state revision/hash and HK06A entry count/identity after public dispatch;
- JSONL and MCP are separate external process projections normalized to canonical semantic outcomes.

## Proof/trust boundary

Inside the claim: repository-owned canonical interaction metadata, single-request mutation envelope, bounded journal read/pagination, accepted HK03 compact/bounded inspection usage, public JSONL/MCP projections for changed semantics, representative content-shape fixture and associated proof/conformance tests.

Trusted base: exact Git/.NET/MSBuild/NuGet behavior, normal OS process/stdio primitives, documented MCP SDK behavior at the already accepted HK07B boundary, cryptographic primitives used normally, and accepted predecessor semantics unless concrete effective evidence reopens them.

Outside HK08A: stale conflict recovery/ancestry deltas and repair prioritization (HK08B), final request/byte/time budgets (HK08B), host/resource limits (HK09A/HK09B), automatic merge, finer-grained concurrency, distributed transactions, gameplay, engine realization and vendor orchestration.

Initial `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
