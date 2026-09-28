# WP-GC2-04 — Evidence use, portable object and new lead

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT / `PRODUCT_CHECKPOINT`
Depends on: `WP-GC2-03` PASS
Blocks: `WP-GC2-05`

## Claim

The player acquires a photograph, clue or other portable object, keeps the relevant evidence across a revisit/reload, shows or uses it and unlocks a new piece of information. This WP is the first formal dependency decision for **GC2 Inventory 2** under [`IMMERSIVE_OBJECT_INTERACTION_SCOPE.md`](../../product/IMMERSIVE_OBJECT_INTERACTION_SCOPE.md): adopt it only if the exact licensed module materially reduces work for the reusable portable-object loop, with explicit version/license/provisioning and H1/H2F lifecycle compatibility before keeper use. Quests remains a separate optional dependency and is not implied by Inventory adoption.

If Inventory 2 is adopted, prove a bounded reusable pattern around a concrete Runtime Item or equivalent instance: acquire → carry/store → inspect/use/show → drop/transfer where relevant → changed lead → reload/revisit. Inventory executes/presents portability; Arkus owns investigation facts and any unique object identity/outcome whose later meaning survives scenes. Generic commodity items do not need Arkus object identity merely because they live in Inventory. GC2 item IDs/runtime GUIDs/save IDs never become canonical Juego2 identity.

This is an immersive-sim-light proof, not an RPG-backpack mandate. The interaction may use contextual UI instead of a large inventory screen. Crafting, merchant systems, equipment grids and broad loot catalogs remain out of scope unless a concrete later feature needs them.

**PASS evidence:** complete played acquire → carry/use/show → changed lead → reload path; if applicable, drop/transfer and later correct materialization; canonical receipt for the durable fact/object consequence; a visible refusal/closed branch without the clue; explicit Inventory/Quests dependency decisions and lifecycle disposition. **Evaluation:** player capture plus fact/object provenance inspection. **FAIL if** debug injection replaces acquisition, local Inventory/UI is the only persistent authority for a consequential unique object, duplicate use duplicates a unique fact, plugin-private IDs become canonical identity, restricted module bytes enter the repo, or adopting Inventory silently expands scope into generic crafting/economy. **Allowed residuals:** broad evidence catalog, complete quest journal, crafting/economy and non-consequential world-prop persistence.
