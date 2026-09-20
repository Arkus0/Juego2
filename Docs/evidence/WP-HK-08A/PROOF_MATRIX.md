# WP-HK-08A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-08A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK08A makes the accepted H0 authoring surface materially less chatty without creating a second semantic system. One representative mixed-resource authoring intent fits in one accepted HK04 transaction; compact reads reuse accepted HK03 projection semantics; the accepted HK06A `authoring.journal.read@1.0` identity remains an empty-request complete-journal read; and HK08A bounded deterministic pagination is exposed only through explicit breaking-version negotiation as `authoring.journal.read@2.0`. Canonical cost/side-effect metadata remains sufficient for economical discovery, and every changed public interaction shape is exercised through both JSONL and MCP.

The claim is narrower than HK08B/HK09B: it does not claim stale-CAS recovery, repair prioritization, final interaction budgets, resource quotas, durable storage, hostile-client authorization or multi-plan atomicity.

## Proof obligations

| Proof obligation | Completeness argument | Positive/effective evidence | Causal negative control | Result |
|---|---|---|---|---|
| representative coherent edit fits one atomic request | the product-shaped probe uses 96 mixed operations, deliberately above the former provisional 64 ceiling; mutation authority remains HK04 | `RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry` advances one revision and writes 72 objects + 24 extensions in one request | restoring 64 or splitting the intent makes the probe red | PASS |
| batching cannot weaken HK04/HK05/HK06A | HK08A changes capacity, not candidate validation, commit authority or provenance authority | invalid later operation and invalid containment candidate leave revision/hash/journal unchanged; successful 96-op batch produces one complete provenance entry | operation-level, candidate-level and provenance-delta controls | PASS |
| accepted journal v1 semantics are preserved | v1 definition is restored to the HK06A request/success/metadata contract and is bound directly to the complete journal authority | with 55 entries, exact v1 + `{}` returns all 55 under `arkus.authoring.journal@1`, with no page cursor; inherited replay remains GREEN | the same test necessarily turns red if default-50 pagination is reintroduced under v1 | PASS |
| HK08A pagination is versioned and complete | v2 is a distinct major version of the same canonical capability; the composer permits breaking evolution across majors and both routes are independently enumerable | v2 `{}` gives default 50 + cursor for a 55-entry journal; explicit 20/20/15 pages reconstruct sequence 1..55; max-sized complete page can recover the exact journal artifact | duplicate/gap controls plus explicit v1/v2 identity assertions; HK01 route-universe oracles require both versions | PASS |
| cursor continuation is bound to one authored journal context | cursor binds base/current anchors, entry count, limit and offset, then receives deterministic integrity framing | valid v2 cursor resumes sequence 2; stale cursor/anchor fail closed | altered inner offset with old checksum -> `world.provenance.invalid_cursor` | PASS |
| compact reads retain required interpretation data | HK03 field selection is reused rather than adding a compact semantic registry | `fields=[]` retains object identity + world anchor while omitting optional fields | deleting required `world` produces schema issues | PASS |
| discovery exposes economical choices without transport policy | cost, side-effect and batching data remain canonical `CapabilityDefinition` metadata | `system.describe` shows cheap reads vs mutation and now enumerates journal v1/v2 | cross-transport discovery comparison catches metadata/version drift | PASS |
| ordinary modify is not one request per property | one `put-object` expresses coherent replacement | type, container and references change in one operation/request and one provenance entry | one-field-per-request oracle turns red for four requests / four conceptual properties | PASS |
| JSONL/MCP remain equivalent for changed primitives | both transports consume the same canonical inventory/neutral dispatch and are launched as real Release processes | flow covers 96-op batch, coherent modify, v1 complete journal read, v2 page+continuation, compact read and discovery | semantic-drift injection turns the independent comparison oracle red | PASS |
| HK08A does not pull HK08B/09B scope forward | changed semantics remain batch envelope, read projection/pagination and conformance only | no recovery planner, merge policy, resource quota or multi-plan transaction exists | any such public handler/contract would be visible outside this matrix | PASS |

## Independent/effective universes

1. **Product shape:** `Docs/art/VISUAL_BIBLE.md` supplies scale/context only; the 72-object + 24-extension probe is engine-neutral fixture vocabulary.
2. **Mutation authority:** revision/hash and journal effects are observed from the accepted session, independently of mutation success metadata.
3. **Pagination sequence:** completeness is checked against persisted sequence numbers and entry IDs across v2 pages, not against cursor arithmetic alone.
4. **Compatibility/version universe:** v1 is exercised with a journal larger than the v2 default page size, so a same-version semantic regression cannot hide behind a small fixture. The canonical inventory and independently enumerated route universe both require v1 and v2 identities.
5. **Transport universe:** JSONL and MCP run as separate external processes and compare neutral outcomes after removing only request correlation.
6. **Replay universe:** the accepted complete HK06A artifact/schema and inherited HK06C replay regression remain the compatibility oracle; HK08A does not redefine replay.

## Reviewer FAIL repair and convergence

The independent Reviewer rejected candidate `0b835891d70666dab41846017e79eb3f7c3b311a` because it changed `authoring.journal.read@1.0` from complete-journal semantics to default-50 pagination without a version increment. The repair does not reinterpret HK06A or HK06C: it restores v1 exactly at the public boundary and adds v2 for the breaking paged contract.

The first repaired code SHA `21b13bf571c7295d2907d972f2eaa95e9a4c5031` built cleanly and passed focused HK08A 13/13, but full regression exposed two HK01 tests whose route-count literals assumed the old inventory size. Those tests were repaired at their actual invariant: independent routes must equal canonical definitions, and both journal versions must be present. No production semantics changed in that follow-up.

Exact implementation/test SHA `300c4b65bcbe1b547837dffc76e241c9500a9e0c` then passed GitHub Actions run `35499408516` on the pinned SDK:

- Release build: 0 warnings / 0 errors;
- focused `Hk08A*`: 13/13 GREEN;
- full regression: 173/173 GREEN;
- cross-transport: GREEN;
- foundational proof / evidence reconciliation / Worker pre-review gates: GREEN;
- candidate clean before and after: YES;
- artifact: `10601643061`.

## Proof-budget verdict

The fail-cycle adds only the version boundary, its second route and causal compatibility/conformance coverage. It does not add an alternate semantic registry, replay format, generic hardening framework or HK08B recovery machinery. The earlier three Worker pre-review repairs (cursor integrity, multi-property modify evidence and candidate-level validation negative) remain intact.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
