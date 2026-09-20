# WP-CITY-01 — Place grammar + spatial depth tiers

Status: **FROZEN PLAN / NOT_STARTED**  
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)  
Mode: **REMOTE**  
Depends on: `WP-CITY-00` PASS + DocSync  
Blocks: `WP-CITY-02` only

## Objective

Define the shared micro-spatial vocabulary that every later CITY workpack uses: what counts as a place, shell, threshold, access route, parcel relation, private/service/vertical layer and how much production depth a location promises.

This WP deliberately happens before detailed mobility so CITY-02 can reason about more than district-to-district lines without designing interiors itself.

## Spatial depth model

Freeze a practical production-depth classification:

- `S0` — scenic envelope / inaccessible context;
- `S1` — façade or shell only;
- `S2` — shallow playable space with bounded interaction;
- `S3` — deep playable place with multiple authored spaces/thresholds/anchors;
- `S4` — hero layered place with multiple meaningful access/discovery opportunities.

Depth is a production promise, not a prestige or systemic-importance score. CITY-03 owns the separate A–D systemic-importance axis.

## Work

1. Define place/parcel/shell/interior/threshold/access terminology.
2. Define public, private, semi-private, service and vertical access classes.
3. Define when rear courts, alleys, stairs, roofs, basements, service doors or river edges are meaningful planning entities rather than decoration.
4. Define S0–S4 entry/exit expectations and what each tier explicitly does **not** promise.
5. Define minimal machine-readable facts later authoring work will need: bounds, frontage, access sides, vertical relation, sockets/anchors, constraints and dependency references.
6. Map several accepted CITY-00 anchors through the vocabulary as examples without redesigning them.

## Deliverables

- `Docs/production/CITY_PLACE_GRAMMAR.md`
- S0–S4 production-depth contract;
- access/threshold vocabulary;
- representative examples against accepted CITY-00 geography;
- explicit residuals for later street/building/interior work.

## Acceptance

- Later WPs can describe a place without inventing competing terms.
- S0–S4 distinguishes façade-only, shallow, deep-playable and hero-layered depth without implying every door opens.
- S-depth does not encode systemic importance, NPC behaviour or narrative priority.
- Access vocabulary supports main entrance, secondary/service access and verticality where justified.
- The grammar can express an ordinary house, bar, shop, warehouse, civic building, street segment and rural/river place.
- No building family, interior layout, secret, Unity object or runtime behaviour is implemented.

## Negative gates

FAIL if the vocabulary becomes an architectural CAD schema, promises ubiquitous interiors, encodes Living World behaviour, collapses into the A–D importance axis, or alters accepted CITY-00 geography.
