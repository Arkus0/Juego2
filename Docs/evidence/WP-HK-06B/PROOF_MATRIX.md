# WP-HK-06B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-06B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted H0 authored-world model, HK06B compares semantic authored resources independently of serializer text/order and exports/imports a versioned snapshot that reconstructs the exact canonical authored state/hash. Import validates the complete artifact before publication, uses optimistic current-state reconciliation and keyed idempotency, then starts a truthful new local HK06A lineage rooted at the imported state with no fabricated source/target mutation history. Runtime-only observations remain outside the snapshot.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| added/removed/changed authored resources are reported semantically | finite HK02/HK02A object+extension resource model | test-owned projections enumerate object identity/type/container/references and extension identity/payload/dependencies independently of production diff; existence is exercised for both resource kinds | focused field-class cases plus exact extension add/remove resource sets | omission mutant returns `missing:<resource>`; false-extra mutant returns `extra:<resource>` | PASS | future canonical field additions must extend HK06B semantics under review |
| diff identity/order are deterministic and serializer-order independent | accepted canonical identities only | production indexes by accepted object/extension identity and sorts resource/relation output ordinally; oracle derives identities independently | repeated diff output stable; reversed input order has same hash and empty diff | reorder-only case would fail on any emitted change | PASS | revision is lineage metadata, not a resource field |
| snapshot is complete enough to reconstruct canonical hash | current canonical `WorldState` codec/hash | artifact embeds the exact unique canonical bytes plus independently checked anchor and state-format/version identifiers | export -> clean import yields identical canonical hash/revision for MicroWorld and Potes-shaped content | canonical-byte corruption and anchor corruption are rejected | PASS | durable storage/framing outside H0 |
| snapshot format/version validation fails closed | `arkus.authoring.snapshot@1` / current world-state format | parser admits only declared fields, canonical Base64, supported snapshot/state versions and canonical deserialization | exported version 1 parses through compare/import | version 2, boundary flags, malformed/corrupt state and false anchor reject before replacement | PASS | future migrations require explicit reviewed semantics |
| invalid import cannot partially replace current authored state | one `PortableWorldAuthoringSession` aggregate | full artifact/state validation precedes session gate; CAS rechecks current revision/hash; successful publication is one inner-session reference swap | accepted clean import replaces state once | each rejection asserts unchanged target revision/hash/journal | PASS | arbitrary process failure is trusted-base/out-of-scope |
| import history semantics are truthful | local HK06A lineage at imported anchor | imported snapshot carries state only; source/target journal data is not embedded; new `TransactionalWorldAuthoringSession` uses imported state as base | source with one mutation imports to target journal count 0; base/current equal imported anchor; later mutation creates local entry 1 | any retained/imported journal entry would fail focused assertions | PASS | history transfer/merge is not claimed |
| snapshot import respects canonical mutation idempotency semantics | public `CanonicalMutation` route | route declares OptimisticVersioned + IdempotentWithKey; separate receipt store is keyed to authoritative outer session and does not masquerade as HK06A provenance | exact retry returns same result with `replayed=true` and no second effect | same key + different request semantics returns `world.snapshot.idempotency_conflict` | PASS | receipt durability outside process lifetime not claimed |
| authored/live boundary is preserved | canonical authored snapshot only | artifact has explicit false provenance/runtime flags and contains only canonical `WorldState` bytes | Potes runtime surrogate advances twice while exported authored bytes stay equal | runtime flag true is rejected as `world.snapshot.boundary_violation` | PASS | actual gameplay/runtime schemas are later work |
| public surfaces are versioned/discoverable | canonical HK01 composed inventory | definitions and handlers are added through the accepted composer; no portability-owned registry defines discovery | system contract exposes compare/export/import `1.0`; success schemas validate outputs | inherited route-universe/conformance tests turn RED on definition/handler drift | PASS | transport projection belongs HK07A |
| accepted predecessor guarantees remain composed | HK01-HK06A integration seams | read routes use accepted attenuation; import remains a canonical mutation with inherited semantics; HK05 test now requires a validation oracle for every public canonical mutation class | full regression GREEN, including updated HK01/HK04/HK05 controls | predecessor behavioural/conformance tests would fail on write/read/validation drift | PASS | concrete contrary evidence would reopen predecessor; none observed |
| representative Juego2 content shape fits the claim | approved fictional Potes/Liébana H2 slice | bounded plaza/building/NPC/extension scenario exercises only HK06B-owned diff/snapshot/live-boundary semantics | Potes shop edit diffs exactly one resource and round-trips to same hash in clean holder | transient surrogate step does not alter snapshot bytes | PASS | gameplay transforms/schedules/Unity state intentionally unclaimed |
| forbidden scope remains absent | baseline-to-candidate diff | implementation is limited to portability contract/engine/bindings, focused predecessor integration tests, scripts and evidence | complete PR diff audited in Worker pre-review | no replay, deterministic simulation, Unity serialization, GUI, cloud persistence, Git truth or transport implementation present | PASS | none |

## Independent/effective universes

1. **Authored-resource universe:** accepted HK02/HK02A `WorldState` object and extension fields/identities, independently projected by tests rather than discovered from the production diff.
2. **Snapshot truth oracle:** canonical codec deserializes the embedded bytes; canonical hash and declared anchor are recomputed from that state.
3. **Effective publication:** the outer authoring session exposes one current inner aggregate under its gate; failed import is compared against independently captured target revision/hash/journal.
4. **Public capability universe:** inherited HK01 route enumeration/discovery reconciles compare/export/import definitions and handlers.
5. **Mutation semantics:** inherited HK04 conformance identifies all `CanonicalMutation` routes; HK06B import satisfies optimistic versioning and keyed idempotency rather than being exempted.
6. **Validation integration:** HK05's all-public-mutations test dispatches a route-specific invalid-state control for snapshot import as well as mutation apply.
7. **Content-shape universe:** one bounded approved Potes/Liébana scenario, explicitly representative rather than a completeness oracle.

## Implementation observation

Implementation/test SHA `02775314155dde7d62f99e48456238a07e227848` passed candidate observation run `35472319015` on Ubuntu 24.04 with pinned .NET SDK 8.0.425:

- Release build: 0 warnings / 0 errors;
- focused `Hk06B*`: 7/7 GREEN;
- full regression: 126/126 GREEN;
- exact-SHA clean-before/clean-after observation receipt: GREEN;
- artifact `10593145655` contains the observation log and receipt.

The subsequent evidence-reconciliation commit is documentation-only. Its resulting exact branch SHA must receive the canonical frozen-candidate verifier unchanged before handoff.

## Proof-budget verdict

The pre-review added one focused negative-conformance file to close an explicit extension-existence proof gap. That control maps directly to the WP's added/removed-resource criterion and does not expand product scope. The product remains one comparator + one versioned snapshot/rebase seam with no replay or transport machinery.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
