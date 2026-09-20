# WP-SCENE-02 — Building families + reusable composition grammar

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SCENARIO PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-SCENE-00` PASS, `WP-CITY-02` PASS  
May run in parallel with: `WP-SCENE-01`

## Objective

Define the reusable building/composition ladder that lets Arkus eventually author places by selecting and adapting reviewed compositions rather than rebuilding every façade from primitive modules.

## Composition ladder

```text
module -> assembly -> shell -> reusable building -> functional POI -> street segment -> authored zone
```

## Work

1. Define minimum metadata for a reusable composition: stable ID, bounds, entrances/sockets, floor/height envelope, allowed variants, dependencies, source lineage, style/category tags and presentation binding.
2. Define representative families: old-quarter house, mixed shop/home, bar, ordinary shop, workshop, warehouse, municipal/civic building, residential block/row, service outbuilding.
3. Define hero-vs-generic promotion rules.
4. Define what future catalogue discovery must expose to a schema-aware agent without selecting the catalogue authority implementation.
5. Define promotion workflow from reviewed ad-hoc composition to reusable composition.

## Deliverables

- `Docs/production/SCENE_BUILDING_COMPOSITIONS.md`
- composition metadata requirements;
- reusable family matrix;
- promote/reject criteria;
- minimum future catalogue queries needed by an authoring agent.

## Acceptance

- At least five materially different building families can be described without asset filenames.
- Stable semantic IDs are separate from presentation asset identity.
- A composition can be replaced visually without changing place identity where semantics are unchanged.
- A weaker agent can prefer a reviewed composition over raw module assembly.
- Missing/unavailable composition references are required to fail visibly later; silent fallback is forbidden.
- No hidden bridge-only semantic registry is accepted as the design answer.
