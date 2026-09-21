# WP-H1-00 bounded content-shape probe

Probe status: `COMPLETE`
Probe owner: `WP-H1-00`
Approved target use: bounded Juego2 shape only; not a completeness oracle and not production CITY content.
Executable witness: `H1EngineBridgeReferenceTests.ContentShapeProbeRepresentsPlazaMarketBarWorkshopWithTwoLogicalAssets`

## Slice

The probe uses exactly one small neutral hierarchy:

- `world/plaza` — projection root;
- `world/plaza/market` — child with logical dependency `asset.market-stall`;
- `world/plaza/bar` — child without an external logical asset dependency;
- `world/plaza/workshop` — child with logical dependency `asset.workshop-kit`.

No Unity scene, prefab, component schema, native path, GUID, gameplay rule or CITY keeper geometry is introduced. The names are representative content shapes only.

## Questions challenged

1. Can the neutral contract represent a small parent/child hierarchy without engine object identity?
2. Can two different children carry stable logical asset dependencies without turning those assets into canonical world-object identity?
3. Is the plan granularity sufficient to distinguish hierarchy, content digest and external logical dependency changes?
4. Does the materializer need Unity-specific types merely to express this representative shape?

## Result

The slice is representable with stable projection resource IDs, parent IDs, resource kind/content digests and logical dependency IDs. The plan normalizes resource order independently of caller enumeration, and dependency existence is evaluated separately from the canonical snapshot anchor. The neutral assembly requires no Unity or H0 Runtime reference.

This probe found no H1-00 blocker and no evidence requiring an H0 predecessor reopen.

## Finding classification

| Finding | Classification | Owner / consequence |
|---|---|---|
| Logical asset IDs are sufficient for the two representative dependencies | H1-00 supported shape | covered by current neutral contract/tests |
| Real logical-ID ↔ Unity-native locator mapping and effective catalogue completeness are not proven here | named future decision | `WP-H1-01` / downstream Unity catalogue work |
| Unity scene/prefab/component realization is not represented here | out of H1-00 boundary | downstream Unity materialization WPs, principally `WP-H1-05` |
| CITY geometry/keeper content is not encoded by this probe | out of boundary | CITY programme retains ownership |

The probe therefore acts only as an omission detector for identity/dependency granularity. It does not establish completeness of future Unity schemas or Juego2 content.
