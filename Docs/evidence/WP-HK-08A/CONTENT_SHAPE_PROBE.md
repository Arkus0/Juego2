# WP-HK-08A representative content-shape probe

## Source and purpose

The bounded product source is `Docs/art/VISUAL_BIBLE.md` v0.1.3: fictional Potes/Liébana, modular reuse, and an H2 hero target of at most roughly 120 meshes. HK08A does not adopt art/gameplay schemas; it uses that approved-draft scale only to avoid validating batching against a toy two-object fixture.

The probe asks one HK08A question: can a plausible coherent authored sub-scene plus metadata cross the former 64-operation boundary and still remain one validated/provenanced canonical transaction, while the resulting state can be inspected compactly through both accepted transports?

## Bounded slice

`RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry` constructs:

- 72 structural object records representing a modular micro-block/sub-scene;
- 24 opaque scoped extension records attached to 24 of those objects, representing bridge/future metadata without inventing engine or gameplay fields;
- total: 96 canonical mutation operations.

The numbers are deliberately bounded rather than presented as a shipping-content law. Ninety-six is below the visual bible's approximate 120-mesh H2 hero envelope while leaving room to exercise mixed object/extension authoring and, importantly, exceeding the provisional pre-HK08A 64-operation request ceiling. It does **not** claim that a whole H2 scene should be one transaction or that 96 is the final resource limit.

Fixture identifiers such as `fixture.hk08a.micro-block-item` are test vocabulary only. No transform, asset, Unity, schedule, simulation or gameplay semantics are promoted into H0.

## Probe path

1. Create an empty portable authored world.
2. Build the 96-operation mixed-resource intent.
3. Dry-run the whole request and require no persistence.
4. Apply the same whole request once.
5. Require exactly one revision advance, 72 objects, 24 extensions and one HK06A journal entry containing all 96 normalized operations.
6. Exercise a separate coherent object modification changing several properties in one mutation request.
7. Inspect the changed world through compact object reads.
8. Exercise bounded journal continuation.
9. Repeat the HK08A public interaction shapes through real JSONL and MCP external processes and require equivalent neutral outcomes.

## Findings

- **Representability — GREEN.** The accepted H0 object + opaque extension grammar can carry the mixed structural/metadata shape without new game-specific schemas.
- **Atomic granularity — GREEN.** The 96-operation intent is one revision and one provenance entry; no logical multi-batch transaction machinery is required for this representative slice.
- **Former 64 ceiling — concrete HK08A finding.** The old provisional ceiling would reject this probe as one request. HK08A therefore raises the bounded implementation envelope to 96 rather than teaching the client to persist two halves.
- **Validation/provenance — GREEN.** A parseable but globally invalid batch cannot publish; a successful batch leaves exactly one provenance entry.
- **Inspectability — GREEN.** Compact read can omit optional object fields while retaining object identity and the authored-world anchor needed to interpret it.
- **Modify chattiness — GREEN.** One coherent object replacement changes type, containment and references together; no field-setter API is introduced.
- **Transport neutrality — GREEN.** JSONL and MCP produce equivalent neutral outcomes for the changed batch/page/compact/discovery shapes.
- **Predecessor reopen condition — none observed.** HK03/HK04/HK05/HK06/HK07 seams represented the product-shaped slice once HK08A supplied the larger bounded request and journal paging view.

## Classification of future decisions

- Exact transforms, asset/catalogue identity, Unity realization and real geometry belong after H0/GATE.
- Final bytes/time/depth/page/batch resource envelopes belong to HK09B and must measure the accepted HK08A/HK08B shapes rather than redefine them silently.
- Stale-CAS recovery and agent correction workflow belong to HK08B.
- The visual-bible H2 `<=~120 meshes` figure is product planning context, not a reason to raise H0 mutation capacity to 120 without measured evidence.

## Result

The content-shaped interaction probe closes the concrete product mismatch behind the old 64-operation ceiling while keeping HK08A narrow: one larger atomic request, bounded inspection/provenance reads and transport-neutral interaction, with no engine/gameplay model pulled into H0.
