# WP-H1-GATE — content-shape probe

The WP requires the probe to be "identical to the deterministic reference scenario", so the probe **is** the Gate scenario (`scripts/h1-gate-scenario.py`, `NODES`). This file records the probe's shape and how each finding is classified. It is not a completeness oracle, and it grants no gameplay or art scope.

## Shape (non-keeper conformance assembly shaped by the Cuña de Confluencia)

| Parity-gate requirement | Realized by | Source (H1-11 representative manifest) |
| --- | --- | --- |
| Wedge-tip sample root with a plaza/market edge | `wedge.tip` (corner module) with `market.cart`, `market.crate` | Corner_Exterior_Wood, Prop_Wagon, Prop_Crate |
| facade hierarchy | `facade.front`, `window.front` → `shutters.front`, `wall.side`, `wall.door` → `door.main`, `roof.house` → `chimney.house`, `vine.front` | the eight kit modules plus the H1-04 facade |
| separate bridge/river-edge marker | `river.edge`, a second scene root with a fence asset and a material override | Prop_WoodenFence_Single |
| wall, roof, door and window/shutter modules | yes | yes |
| bench/table/chair class, lamp, tree and rock | **absent from the admitted slice** (see finding G1) | — |
| one humanoid with idle/walk/sit clip references | `civilian.idle`, `civilian.walk`, `civilian.sit` (seated on the crate) | UAL1 + three clips |
| at least one nested/source prefab relationship | every prefab node is realized as a managed derivative of a read-only source prefab | H1-06 lineage |
| shared material and one allowed override | `market.crate` and `river.edge` share the single-slot renderer override | the H1-11 shared material |
| at least one canonical-object reference | `civilian.idle` faces `door.main`; `civilian.walk` walks toward `market.cart` | canonical-link component |
| several catalogue dependencies | 14 sources + 1 material + 3 clips (18 logical IDs), all resolved publicly at S05; the fixed managed scene is the projection target, not a catalogued source | — |
| multiple allowlisted components and a three-level hierarchy | transform, mesh-renderer, animator, canonical-link; `wedge.tip → window.front → shutters.front` and `wedge.tip → market.crate → civilian.sit` | — |

There is no player controller, AI, navmesh, physics behaviour, schedule, dialogue, CITY seed, route graph or keeper geometry.

## Findings and classification

| # | Finding | Classification |
| --- | --- | --- |
| G1 | The admitted H1-11 slice contains no bench, table or chair model. This adds to H1-11 F1 (no lamp, tree or rock in the adopted distributions). The crate serves as the seat for `civilian.sit`. | Named future ART/H2 adoption decision. The Gate adopts nothing and may not widen the admitted slice, so this is not a Gate blocker and not a predecessor reopen. It is carried in `RECONCILIATION.json` with H1-11 F1. |
| G2 | A materialize request over an invalid canonical binding is refused before any Editor launch. On the pre-reopen base, the public error was the generic `unity.lifecycle.corrupt-result` ("typed worker request encoder rejected the canonical request"). Only the public preflight `unity.projection.plan` carried the actionable code. | Initially classified as accepted predecessor semantics. Trial 1 (R4) showed it blocks a fresh client's recovery path. **Resolved by WP-H1-04 reopen 1** (PR `#239`, merge `4ab82fb1c0a0d8654aaa44ebc66452a3507ef347`): the refusal now carries `projection.source-missing` / `projection.reference-missing` with a plan hint. Gate S12 requires those exact codes on both transports (`S12.*.materialize-refusal-not-actionable`), and the C6 control `materialize-refusal-generic` proves the check is sensitive. |
| G3 | Stage 13 needs a "supported managed Unity edit". No public capability performs a Unity-side edit, by design, because canonical truth never originates in Unity. The Gate simulates the external human edit by changing one managed `Transform` in the published scene's serialized text (Force Text), without launching Unity. | Gate-owned stimulus. It is not a product path and not a private Unity launch. Only the public drift and import-proposal capabilities interpret it. |
| G4 | Observe and drift are plan-relative. While the canonical binding is invalid, they are refused before any Editor launch. Since WP-H1-04 reopen 1, the refusal carries the actionable `projection.source-missing` code, which Gate S12 requires (`S12.*.observe-refusal-not-actionable`). The effective Unity state therefore cannot be observed during the invalid interval. The Gate proves the absence of a false publication in two ways. First, the refused requests launch no Editor process: the ledger audit counts every launch. Second, the first observation after the canonical repair, taken before any new materialization, returns the unchanged S11 generation with `current=false`. | Accepted predecessor semantics (H1-05/H1-09 plan-relative observation). An observation mode that reports the last publication independent of the canonical plan is a named future diagnostics decision. This is not a reopen, because the accepted guarantee (no false active generation) holds. |

The Gate itself confirms H1-11 F10: realization digests are per import. The isolated MCP project copy is therefore taken **after** the shared baseline import. The copy is empty of canonical state and projection output, and it shares the imported Library and material GUIDs. The normalized reconstruction digests stay comparable across transports. No cross-import portability is claimed.
