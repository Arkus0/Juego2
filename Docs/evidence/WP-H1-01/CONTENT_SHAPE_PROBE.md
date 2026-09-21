# WP-H1-01 bounded content-shape probe

Probe status: `COMPLETE`
Probe owner: `WP-H1-01`
Approved target use: bounded Juego2/Potes authoring shape only; omission detector for the initial Unity binding vocabulary, not proof that Unity resources exist.
Executable witness: `H1UnityAuthoringProducerTests.PotesFacadeProbeDerivesEveryStructuredReferenceExactlyOnceAndRoundTrips`

## Slice

The probe models one facade binding in Potes with the smallest shape that exercises every initial reference domain owned by H1-01:

- canonical subject `building.potes-facade`;
- target logical scene `scene.potes-market`;
- logical prefab source `prefab.potes-facade`;
- normalized local transform using integer millimetres, milli-degrees and parts-per-million scale under the explicit `unity-local-left-handed-y-up-z-forward` convention;
- one `canonical-link` component from the facade to canonical market root `market.potes-root` with relation `attached-to`;
- one renderer document with logical material `material.potes-stone`;
- one animator document with logical clip `animation.potes-shutter`.

No Unity scene, prefab, material or clip is opened or resolved. No native path, GUID, `.meta`, `AssetDatabase`, `GlobalObjectId`, `UnityEngine` or `UnityEditor` type participates in the probe.

## Independent reference walk

The test oracle is authored independently from the producer's dependency registries/codec output. From the fixture shape it expects exactly:

- canonical/HK02A: `attached-to|market.potes-root`;
- catalogue: `scene|scene.potes-market`, `prefab|prefab.potes-facade`, `material|material.potes-stone`, `animation-clip|animation.potes-shutter`.

The producer output must contain those sets exactly once, and the ordinary extension mutation fragment must carry the same canonical dependency set. Reversing component enumeration must produce the same normalized payload. Decode must reconstruct the same normalized binding and dependencies; inspect must reconcile the payload against supplied HK02A dependency metadata.

## Questions challenged

1. Can the initial binding represent hierarchy/reference intent without using a Unity-native object identity as canonical identity?
2. Can target scene, prefab, material and animation clip remain typed logical catalogue references rather than paths/GUIDs?
3. Can every structured canonical reference become exactly one HK02A dependency without caller-maintained duplicate truth?
4. Can the normalized transform and allowlisted component documents survive deterministic encode/decode?
5. Can the compiled result enter accepted H0 `plan/dry-run/apply` as an ordinary extension mutation fragment rather than bypassing canonical authority?

## Result

The bounded shape is representable. The independent walk and producer derivation agree exactly for all five references; component-order reversal leaves the encoded normalized payload unchanged; decode/inspect reconstruct the authored shape; and the generated extension fragment succeeds through the accepted H0 plan/dry-run/apply path in the focused integration test.

This probe found no need to reopen HK02A. Its typed `kind + targetId` dependency surface truthfully expresses the structured canonical reference used by this binding.

## Finding classification

| Finding | Classification | Owner / consequence |
|---|---|---|
| Canonical market-root link maps cleanly to HK02A dependency metadata | supported H1-01 shape | current producer + HK02A integration proof |
| Scene/prefab/material/animation references remain provider-owned logical catalogue IDs | supported H1-01 shape | current binding contract |
| Actual existence and Unity type compatibility of those catalogue resources is not proven | downstream | later Unity catalogue/materialization WPs |
| Exact effective component serialization in the Editor is not proven | downstream | H1-05/materialization parity |
| Native logical-ID ↔ GUID/path resolution is not part of the public binding | downstream/private implementation concern | later Unity provider/materializer, without changing canonical identity |
| Gameplay/CITY semantics are not inferred from the Potes names used by the fixture | out of boundary | H2+/CITY owners |

The probe is therefore an omission detector for reference, transform and component-document granularity, not evidence of a materialized Unity project.