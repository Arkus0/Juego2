# CITY-07 shared elevation-profile amendment

Status: **PROPOSED / NOT ACCEPTED**  
Class: DOCS_ONLY / PRODUCT-SCOPE CLARIFICATION  
Scope: `WP-CITY-07` keeper realization  
Causal owner amended: CITY-07 local physical realization/evidence boundary  
Companions: `CITY_07_ART_READINESS_AMENDMENT.md`, `ART_ENVIRONMENT_ASSEMBLY_GRAMMAR.md`, `ART_01_DIMENSIONAL_PROFILE.md`

## Purpose

Prevent a new dressed-greybox failure mode: every individual building/street assembly can be locally coherent while the district as a whole has no authored elevation structure.

CITY-04's measured greybox remains physical evidence, not keeper elevation authority. CITY-07 must therefore establish one **shared district elevation frame** before final keeper assembly so Casco, Plaza, bridge, landing, river edge, thresholds and route profiles are not solved independently with unrelated local Y values.

This is not permission to invent new hills, stairs, shortcuts or elevation classes. The profile must remain inside accepted CITY topology, route/elevation classes, grade/clearance constraints and the differential revalidation obligations already owned by CITY-07.

## Shared datum

CITY-07 must declare one local vertical reference for the retained seed and express keeper-relevant elevations against it.

The datum may be an arbitrary project-local `Y=0` reference. What matters is that all retained elevations derive from the same frame and can be compared.

At minimum the profile records effective keeper elevations for the accepted anchor/control points relevant to the retained seed and representative chain, including where applicable:

- Río / water reference used by the retained scene;
- `W.LANDING`;
- X1 / Puente Viejo approach controls, abutments and bridge crest;
- S02 bridgehead / immediate receiving ground;
- `W.X1` / W12 approach control;
- `W.CASCO` and the Casco square/micro-route controls;
- accepted Bar F01 public threshold level;
- `W.PLAZA` / Plaza reference level;
- `W.X5` / ford approach reference where it participates in retained evidence;
- any accepted expansion seam whose vertical continuation is visible from the keeper slice.

If an identifier above is represented by several physical points in Unity, the sheet records the relevant point set or range rather than pretending it is one scalar.

## Route longitudinal profiles

For every retained route segment materially realized by CITY-07, record a compact longitudinal profile containing:

- start/end control elevation;
- meaningful grade breakpoints;
- landing/step/retaining transitions if already authorized;
- effective grade between controls;
- relation to the accepted route/elevation class;
- any local tuning relative to CITY-04 evidence and the affected-conclusion revalidation it triggers.

The minimum positive proof covers the representative chain:

`Orilla sur -> Puente Viejo -> S02 bridgehead -> W12 -> Casco -> casco micro-route B -> Bar F01`.

If PR `#228` P8 later becomes authoritative, its accepted new lanes/paths also receive profiles before they are keeper-realized; this amendment does not adopt P8 itself.

## Cross-sections

Retain at least **three** small cross-sections where vertical relationships materially affect third-person reading. Recommended controls:

1. Puente Viejo / S02 bridgehead;
2. W12 climb with adjacent building/retaining relationship;
3. Casco / Bar F01 threshold and street-edge relationship.

A cross-section need only show the surfaces/levels needed to judge route support, threshold/base contact, retaining/edge condition and human scale. It is not an architectural construction drawing.

## Building/site elevation binding

A keeper building may use its own local transform origin, but its physical levels cannot be invented independently.

At minimum:

- principal exterior threshold derives from the shared route/site profile;
- plinth/base/retaining response derives from the difference between building floor level and adjacent ground/street levels;
- visible floor/storey/eave relationships consume the ART-01 dimensional profile or a reviewed exception;
- interior floor level at a public entrance must connect coherently to the exterior threshold;
- nearby buildings sharing a street must not each flatten or re-slope the same public ground independently.

The key rule is: **local datum is a coordinate convenience, not a separate world elevation truth**.

## Relationship to ART-01

ART-01 still precedes CITY-07 keeper realization and therefore does not depend on the final CITY-07 keeper profile.

For its kit/paintover benchmark ART-01 may consume the accepted CITY-04 measured/blockout elevations as design input. CITY-07 later freezes and, where lawful, tunes the final keeper elevation profile while preserving accepted CITY conclusions.

This avoids a dependency cycle:

```text
CITY-04 measured seed -> ART-01 kit/visual target
                 \-> CITY-07 final shared elevation profile + keeper assembly
```

ART-01 owns shared dimensions and kit compatibility. CITY-07 owns how the actual retained district sits in one vertical frame.

## Rapid spatial iteration without review-per-click

CITY-07 is a level-design/keeper realization WP. It must permit quick local iteration inside accepted boundaries.

The active Worker may repeatedly adjust local realization details — route surface interpolation, kerb/retaining placement, lawful threshold/base response, local facade setback, small grade smoothing inside the accepted class/bands — **without starting a new independent review cycle for every adjustment**.

The Worker keeps compact checkpoints instead:

1. **elevation baseline** — shared datum + anchor/profile sheet before keeper massing is frozen;
2. **massing/traversal checkpoint** — representative chain walkable with neutral/simple materials, profiles updated;
3. **keeper candidate** — final profile/cross-sections, third-person evidence and all affected CITY-04 conclusions discharged through the existing differential ledger/campaign.

A micro-iteration that remains inside existing authority is ordinary Worker execution. A change that needs a new route, crossing, elevation class, stair route, hard-boundary change, site/programme mutation or weakened clearance/access obligation is not iteration freedom: route it to the causal owner before implementation.

This process rule changes **review cadence**, not acceptance rigor. Independent review examines the frozen candidate, retained checkpoints and final measurements rather than every exploratory tweak.

## Deliverables added to CITY-07

- shared vertical datum declaration;
- anchor/control elevation sheet for the retained seed;
- longitudinal profiles for materially realized retained routes;
- at least three representative cross-sections;
- mapping from player-facing building thresholds/interior floors to the shared route/site elevation frame;
- compact iteration checkpoints described above;
- differential revalidation links for any keeper elevation/grade change that can affect accepted CITY-04 conclusions.

## Acceptance additions

CITY-07 cannot PASS unless:

- the retained district has one coherent shared elevation frame;
- the representative chain has a reviewable longitudinal profile;
- the bridgehead, W12/Casco transition and Bar F01 threshold can be understood from the retained elevation evidence;
- keeper buildings derive their public ground/threshold contact from the shared district profile rather than arbitrary per-building local Y choices;
- no final keeper slope/step/retaining solution silently changes accepted route/elevation/access semantics;
- material elevation changes are covered by the existing differential revalidation mechanism;
- the final third-person walkthrough confirms that the authored vertical rhythm reads coherently at human scale.

## Negative gates

FAIL if:

- each building/street segment is locally plausible but uses an unrelated vertical datum with no shared district profile;
- the accepted greybox plane is copied blindly as final relief merely because it was measurable;
- the Worker flattens public ground independently under each building to make asset placement easy;
- a threshold or interior floor visually aligns from one camera but is disconnected from the route/site profile;
- a new stair/hill/shortcut/elevation class is introduced as a local polish decision;
- or iteration speed is achieved by skipping final profile evidence or affected-conclusion revalidation.

## Result

CITY-07 now owns not only coherent local assemblies but a coherent **vertical game-space**. ART-01 can supply compatible pieces and construction grammar; CITY-07 must place them on one authored terrain/route/elevation frame so the keeper district reads as a connected place rather than a set of individually grounded dioramas.