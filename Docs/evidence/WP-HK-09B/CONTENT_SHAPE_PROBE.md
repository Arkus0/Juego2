# WP-HK-09B representative content-shape probe

CONTENT_SHAPE_PROBE_VERDICT: PASS
APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md
UNRESOLVED_IN_SCOPE_FINDINGS: 0

## Approved source and bounded slice

The approved product source is `Docs/art/VISUAL_BIBLE.md` v0.1.3. It anchors Juego2 in a fictional Potes/Liébana valley town and explicitly names a hero-town vocabulary that includes plaza/market grain, bar terrace and workshops. Its H2 hero target is plaza + 1–2 streets + bar/shop interior with a bounded mesh/atlas/animation budget.

HK09B does not own transforms, asset schemas, gameplay schedules, NPC simulation or engine/editor representation. The probe therefore uses only the amount of product vocabulary needed to test HK09B's own public-contract/resource/persistence claim:

- four logical districts: `plaza`, `market`, `bar-terrace`, `workshop`;
- 18 generic authored objects per district = 72 objects;
- 24 small opaque visual extension records;
- total coherent mutation shape = 96 operations.

The `fixture.hk09b.*` type/owner identifiers are deliberately generic test vocabulary. They demonstrate identity/granularity and extension capacity without promoting example art concepts into new H0 canonical schemas.

## Executable scenario

`Hk09BContentShapeProbeTests.PotesHeroSliceFitsOneBoundedTransactionAndCheckpointRebase` performs the following through the public neutral projection:

1. builds the full 96-operation Potes hero slice as one coherent mutation request;
2. measures portable depth/serialized argument bytes against the enforced H0 envelope;
3. applies it once and requires revision `1`, 72 objects and 24 extensions;
4. inspects `world.summary` and requires the expected object count;
5. exports one canonical snapshot and requires decoded authored state below the H0 world-byte envelope;
6. measures the full snapshot-import request against the same canonical argument/depth envelope;
7. imports into a separate target session;
8. requires target revision/hash to equal the source;
9. requires the imported lineage to start with zero local mutation journal entries and truthful `new-local-lineage-empty` rebase evidence.

## Representability and identity/granularity findings

**Representability — PASS.** The bounded approved town slice fits the existing generic `WorldObject` + extension model without adding product-specific schemas. HK09B therefore has no evidence that its resource envelope forces the current approved hero slice into multiple transactions or checkpoints.

**Identity/granularity — PASS for the WP claim.** One generic authored object per test item plus small extension records is sufficient to exercise the resource/persistence boundary. This is not a claim that final art assets must map 1:1 to `WorldObject`; H1/H2 may refine engine/editor representation while preserving canonical contracts.

**Atomic authoring boundary — PASS.** All 96 operations commit under one expected revision/hash, one canonical publication and one resulting revision. This directly protects the accepted HK08A coherent-edit granularity from a resource-policy regression.

**Inspection boundary — PASS.** The resulting world is inspectable through the existing summary capability and reports the expected object population without a special HK09B inspection model.

**Validation boundary — PASS by composition.** The mutation goes through the accepted canonical mutation planner/candidate validator before publication. HK09B adds resource checks around that accepted validator; it does not bypass or replace it.

**Checkpoint/import boundary — PASS.** A snapshot representing the product-shaped slice fits the H0 world/request envelope and is rebased atomically into a fresh target with source-equivalent content hash and truthful empty-local-lineage evidence.

**Diff/replay boundary — no new product semantic finding.** HK09B does not change semantic diff or replay data models. Its owned replay concern is publication interruption, exercised separately by `InterruptedReplayCannotPublishStagedStateOrProvenance`; inherited HK06B/HK06C regressions remain GREEN.

## Classified observations

| Observation | Classification | Disposition |
|---|---|---|
| approved plaza/market/bar/workshop slice fits one 96-op mutation | in-scope | PASS; locks the operation envelope to an accepted coherent shape |
| snapshot for that slice fits canonical world/request bounds and imports atomically | in-scope | PASS |
| final mesh/component/transform granularity is not represented by this fixture | named future decision | H1/H2 engine/editor bridge concern; do not expand HK09B schema |
| visual-bible hero target allows up to ~120 meshes while this probe uses 72 generic objects | non-blocking observation | the probe is a bounded slice, not a complete H2 scene capacity certification |
| power-loss recovery is not exercised | out-of-boundary | H0 explicitly claims no power-loss durability |

No approved product-shaped evidence requires widening HK09B scope or reopening a predecessor.
