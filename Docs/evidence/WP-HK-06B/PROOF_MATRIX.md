# WP-HK-06B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-06B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted H0 authored-world model, HK06B compares semantic authored resources independently of serializer text/order and exports/imports a versioned snapshot that reconstructs the exact canonical authored state/hash. Snapshot import validates the complete artifact before publication, uses optimistic current-state reconciliation and keyed idempotency, then performs a **canonical rebase**: it atomically establishes the imported state as a new local lineage root, emits machine-readable rebase evidence, and leaves the HK06A mutation journal empty because no HK06A mutation occurred. Runtime-only observations remain outside the snapshot.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| added/removed/changed authored resources are reported semantically | finite HK02/HK02A object+extension resource model | test-owned projections enumerate object identity/type/container/references and extension identity/payload/dependencies independently of production diff; existence is exercised for both resource kinds | focused field-class cases plus exact extension add/remove resource sets | omission mutant returns `missing:<resource>`; false-extra mutant returns `extra:<resource>` | PASS | future canonical field additions must extend HK06B semantics under review |
| diff identity/order are deterministic and serializer-order independent | accepted canonical identities only | production indexes by accepted object/extension identity and sorts resource/relation output ordinally; oracle derives identities independently | repeated diff output stable; reversed input order has same hash and empty diff | reorder-only case would fail on any emitted change | PASS | revision is lineage metadata, not a resource field |
| snapshot is complete enough to reconstruct canonical hash | current canonical `WorldState` codec/hash | artifact embeds the exact unique canonical bytes plus independently checked anchor and state-format/version identifiers | export -> clean import yields identical canonical hash/revision for MicroWorld and Potes-shaped content | canonical-byte corruption and anchor corruption are rejected | PASS | durable storage/framing outside H0 |
| snapshot format/version validation fails closed | `arkus.authoring.snapshot@1` / current world-state format | parser admits only declared fields, canonical Base64, supported snapshot/state versions and canonical deserialization | exported version 1 parses through compare/import | version 2, boundary flags, malformed/corrupt state and false anchor reject before replacement | PASS | future migrations require explicit reviewed semantics |
| invalid import cannot partially replace current authored state | one `PortableWorldAuthoringSession` aggregate | full artifact/state validation precedes session gate; CAS rechecks current revision/hash; successful publication is one inner-session reference swap | accepted clean import replaces state once | each rejection asserts unchanged target revision/hash/journal | PASS | arbitrary process failure is trusted-base/out-of-scope |
| import history semantics are truthful | local HK06A mutation lineage at imported anchor | imported snapshot carries state only; source/target journal data is not embedded; new `TransactionalWorldAuthoringSession` uses imported state as base | source with one mutation imports to target journal count 0; base/current equal imported anchor; later mutation creates local entry 1 | any retained/imported journal entry would fail focused assertions | PASS | history transfer/merge is not claimed |
| public classification matches the actual writer authority | HK06B-owned snapshot-import integration seam | contract meta-model has a distinct `CanonicalRebase` side-effect and transaction requirement; effective import handler has a distinct rebase marker and is not an HK04 transactional-mutation handler | `Hk06BRebaseContractTests` asserts definition/handler split while inherited `MutationSurfaceConformance` remains GREEN | relabel import as `CanonicalMutation`, restore `CanonicalTransaction`, or mark import handler `ITransactionalMutationHandler` and the focused control turns RED | PASS | future canonical whole-root writers must explicitly choose/justify their authority class |
| required evidence is truthful without fabricating HK06A mutation provenance | successful canonical rebase only | accepted result schema requires `arkus.authoring.snapshot-rebase-evidence@1`, binding request identity, snapshot anchor, previous/current anchors and explicit empty-new-lineage mutation-journal disposition | first import emits schema-valid evidence; exact retry returns the same evidence with `replayed=true`; local mutation journal remains 0 | removing `rebaseEvidence` makes the public success schema invalid; non-empty inherited mutation history fails existing lineage assertions | PASS | rebase-receipt durability beyond process lifetime is not claimed by H0 |
| snapshot rebase respects keyed idempotency | public `CanonicalRebase` route | receipt store is keyed to authoritative outer session; fingerprint covers the forwarded semantic request and is separate from HK06A mutation provenance | exact retry returns same result/evidence with `replayed=true` and no second state effect | same key + different request semantics returns `world.snapshot.idempotency_conflict` | PASS | receipt durability outside process lifetime not claimed |
| canonical discovery/projection understands the new rebase semantics | HK01 canonical meta-model integration | production projection and independent projection oracle both explicitly represent `canonicalrebase`; no route-specific projection exception exists | full HK01/HK03/HK04 conformance regression GREEN | omitting the new enum token from the independent oracle caused the repair observation to fail closed before correction | PASS | future metadata classes must extend both production and independent projection paths |
| authored/live boundary is preserved | canonical authored snapshot only | artifact has explicit false provenance/runtime flags and contains only canonical `WorldState` bytes | Potes runtime surrogate advances twice while exported authored bytes stay equal | runtime flag true is rejected as `world.snapshot.boundary_violation` | PASS | actual gameplay/runtime schemas are later work |
| public surfaces are versioned/discoverable | canonical HK01 composed inventory | definitions and handlers are added through the accepted composer; no portability-owned registry defines discovery | system contract exposes compare/export/import `1.0`; success schemas validate outputs | inherited route-universe/conformance tests turn RED on definition/handler drift | PASS | transport projection belongs HK07A |
| accepted predecessor guarantees remain composed | HK01-HK06A integration seams | ordinary authored edits remain `CanonicalMutation + CanonicalTransaction` and continue through unchanged HK04/HK06A mutation authority; snapshot import is explicitly a different whole-root writer owned by HK06B | full regression 128/128 GREEN; ordinary apply remains mutation-classified and mutation conformance stays exact | predecessor behavioural/conformance tests fail on ordinary mutation/read/validation drift; focused HK06B control fails if rebase masquerades as mutation | PASS | concrete contrary evidence would reopen predecessor; none observed |
| representative Juego2 content shape fits the claim | approved fictional Potes/Liébana H2 slice | bounded plaza/building/NPC/extension scenario exercises only HK06B-owned diff/snapshot/live-boundary semantics | Potes shop edit diffs exactly one resource and round-trips to same hash in clean holder | transient surrogate step does not alter snapshot bytes | PASS | gameplay transforms/schedules/Unity state intentionally unclaimed |
| forbidden scope remains absent | baseline-to-candidate diff | implementation is limited to portability contract/engine/bindings, canonical metadata support, focused integration tests, scripts and evidence | complete PR diff audited in Worker pre-review | no replay, deterministic simulation, Unity serialization, GUI, cloud persistence, Git truth or transport implementation present | PASS | none |

## Independent/effective universes

1. **Authored-resource universe:** accepted HK02/HK02A `WorldState` object and extension fields/identities, independently projected by tests rather than discovered from the production diff.
2. **Snapshot truth oracle:** canonical codec deserializes the embedded bytes; canonical hash and declared anchor are recomputed from that state.
3. **Effective publication:** the outer authoring session exposes one current inner aggregate under its gate; failed import is compared against independently captured target revision/hash/journal.
4. **Public capability universe:** inherited HK01 route enumeration/discovery reconciles compare/export/import definitions and handlers.
5. **Canonical-writer classes:** inherited HK04 conformance owns `CanonicalMutation`; HK06B independently asserts the `CanonicalRebase` definition/handler seam. Neither class is recognized by route-name exceptions.
6. **Projection semantics:** HK01's independent projection oracle has an explicit canonical-rebase token; it does not derive expected metadata from production serialization.
7. **Validation integration:** invalid snapshot/state/version/boundary controls execute the effective rebase route and assert no partial replacement.
8. **Content-shape universe:** one bounded approved Potes/Liébana scenario, explicitly representative rather than a completeness oracle.

## Implementation observation

Implementation/test SHA `6f700ad4c9a00328f61d7c317a28fdbbe303ca95` passed candidate observation run `35473449876` on Ubuntu 24.04 with pinned .NET SDK 8.0.425:

- Release build: 0 warnings / 0 errors;
- focused `Hk06B*`: 9/9 GREEN;
- full regression: 128/128 GREEN;
- exact-SHA clean-before/clean-after observation receipt: GREEN;
- artifact `10593592078` contains the observation log and receipt.

Two earlier repair observations failed closed and are not counted as green evidence: `4240e72…` exposed one nullable-analysis error in the new test, and `c28ea84…` exposed the independent HK01 projection oracle's missing canonical-rebase token. Both were corrected before the GREEN observation above.

The subsequent evidence-reconciliation commit is documentation-only. Its resulting exact branch SHA must receive the canonical frozen-candidate verifier unchanged before handoff.

## Proof-budget verdict

The repair adds one explicit canonical rebase metadata class, one rebase handler marker/evidence schema and two focused regression tests. Those pieces map directly to the independent Reviewer's classification ↔ authority ↔ provenance/history blocker and do not expand HK06B into replay, transport or a second mutation system. HK04 mutation conformance itself was not weakened or given a snapshot-import exception.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
