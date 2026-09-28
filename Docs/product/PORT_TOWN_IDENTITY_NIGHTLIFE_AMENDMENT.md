# Port-town identity and nightlife amendment

Status: **ACCEPTED product amendment**
Mode: `PROCESS_ONLY / DOCS_ONLY`
Date: 2026-09-28
Accepted: PR `#266`, candidate `a89276097b7247477cbede940678a131e81b2b64`, Reviewer PASS `#5334692121`, merge `70af61de17a7be9259008db279e58ddaf8b9b2ad`.
Amends: `PORT_TOWN_SCALE_AMENDMENT.md`, accepted `WP-CITY-URBAN-00` planning output and downstream ART/CITY urban consumers.

## Owner decision

The final fictional northern-Spain port town keeps its accepted scale, five-zone production structure and first-block topology, but two product-facing details are amended before visual production expands:

1. **The town's final proper name is not decided.** `Villa Bruma` is retired as a working product label and must not be treated as canon, printed into signage/UI, baked into assets or propagated into future briefs. Until a later reviewed naming decision, use neutral references such as `the port town`, `villa portuaria`, `final town`, or `NOMBRE_PENDIENTE` where an explicit placeholder is required.
2. **Nightlife is a city layer, not a sixth dedicated district and not primarily an industrial identity.** The accepted five production neighbourhoods remain sufficient. Their day/night roles are refined below without changing B0 Mercado–Muelle geometry or the current town-edge graph.

## Nightlife distribution

### `N.CASCO` — primary nightlife pole

Casco remains dense old-town fabric with civic/family institutions, slopes, landmarks and daytime errands. A bounded **lower-Casco / central nightlife strip** becomes the town's main evening and late-night social concentration.

Candidate content includes bars, taverns, restaurants, arcades/recreativos, karaoke or small performance room, small club/disco-scale venue, late cafés/food, minigame locations and dense pedestrian social circulation. This does **not** require the entire Casco to become entertainment-only; residential, civic and family uses remain present so the zone changes character with time rather than becoming a theme park.

### `N.TALLERES` — secondary worker/alternative nightlife

Talleres remains primarily repairs, small factories, workshops, warehouses and work rhythms. At night it may support a smaller, rougher and more alternative social layer: worker bars, music/rehearsal venue, cheap late food, marginal or semi-clandestine hangouts and rarer activities/minigames.

Talleres complements Casco; it is not the town's main party district.

### `N.MERCADO` — everyday evening life

Mercado remains the main repeat-visit commercial zone. Restaurants, ordinary bars, cafés and some late-opening businesses can create evening activity, but Mercado is not the principal late-night destination.

### `N.MUELLE` — night work first

Muelle remains a working port. Night presence is driven primarily by shifts, crews, deliveries, fishermen/port workers and a limited port-bar/social layer. It must not drift into a leisure-marina identity.

### `N.VIVIENDAS` — low-intensity night

Residential zones become quieter at night, with home arrivals, local corners and only sparse neighbourhood activity. They provide contrast to Casco/Mercado/Talleres rather than duplicating their nightlife intensity.

## Routine and gameplay consequence

The distribution is intended to create useful cross-town routines rather than isolated district populations. A person may live in Viviendas, work in Muelle or Talleres, pass through Mercado for errands or dinner, visit Casco at night and return home later. Named and routine-bearing NPCs may therefore change zone, activity and social context by hour without requiring every NPC to have deep authored narrative state.

Nightlife can later host minigames, substories, relationships, investigation leads, meetings, tailing, confrontations and recurring social routines. This amendment does not implement any of those systems and does not pull GC2 gameplay before its accepted gate.

## Spatial and roadmap non-changes

This amendment does **not**:

- add a sixth production neighbourhood;
- alter B0 Mercado–Muelle nodes `L/A/S/E/P/Q/O/V` or edges `B01..B10`;
- alter accepted town edges `U01..U08` merely to express nightlife;
- change H2F sequencing, H2-GATE dependencies, residency rules or NPC-count orientations;
- require Casco, Talleres or whole-town nightlife to be built before the first keeper block;
- approve final signage, architecture, lighting, assets, opening hours or exact venue locations.

Exact nightlife streets, parcels, venues, opening-hour presentation, lighting and visual language remain later CITY/ART/content decisions using the accepted scenario-production factory.

## Downstream binding consumption

- `WP-ART-ENV-02` must not hard-code `Villa Bruma` into reusable production profiles and should keep the factory capable of producing materially different day/evening/night briefs without a new scene-specific pipeline.
- `WP-ART-URBAN-01` must treat the town name as unresolved and keep permanent signage/proper-name assets placeholder-safe. Its final-town visual language should be compatible with the day/night distribution above even though its first-block proof remains Mercado–Muelle.
- `WP-CITY-URBAN-01` keeps B0 scope unchanged and must not expand to Casco/Talleres merely to prove this amendment. It preserves future seams/ownership so later district work can realize the accepted nightlife roles.
- Later `DISTRICT-*`, GC2 routine/content and story WPs consume Casco as the primary nightlife pole and Talleres as the secondary/alternative pole unless a later reviewed amendment changes that distribution.

## Acceptance test for this amendment

PASS if reviewers can confirm all of the following:

- no current keeper geometry or first-block scope is reopened;
- the final town proper name is explicitly undecided and `Villa Bruma` is non-canonical going forward;
- nightlife has a clear primary home in Casco, a secondary expression in Talleres and differentiated evening roles elsewhere;
- the five-zone model remains sufficient;
- future ART/CITY consumers are explicitly bound so the decision is not lost before asset/scenario production.

FAIL if the amendment silently creates a sixth mandatory district, turns Talleres into the sole nightlife area, converts Casco wholesale into an entertainment district, changes B0 geometry/DAG, or requires nightlife implementation before its causal production/content WPs.
