# WP-CITY-00 Worker plan

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Worker state: ACTIVE
Class: PRODUCT / SPATIAL PREPRODUCTION (NON-FOUNDATIONAL)

## PREDECESSOR_CONTRACT_CHECK

`WP-CITY-00` declares three inputs: the `WP-ART-00` seed, `Docs/production/PRODUCTION_BLUEPRINT.md`,
and the current Living World PA roadmap **as non-binding product input**.

None of them is an independently PASSed foundational workpack, and this check does not pretend
otherwise. The honest inherited surface is: *documents merged on `main` at the baseline SHA*.

### Dependency 1 — `WP-ART-00` (art/setting direction seed)

- Status in `Docs/workpacks/ART/WP-ART-00.md`: **SEED MERGED** (direction docs on `main` via PR).
- No independent Reviewer PASS, no reviewed candidate SHA and no exact-SHA evidence exist for it.
  It is explicitly outside `FOUNDATIONAL_PROOF_STANDARD.md` and gates no `HK-*`.
- Inherited state as merged at `290f92e9c21f1e454e0d6924b29f4d778b9875f8`:
  `Docs/art/SETTING.md`, `Docs/art/VISUAL_BIBLE.md` v0.1.3,
  `Docs/evidence/WP-ART-00/TRIAGE_DRY_RUN.md`.

Guarantees consumed rather than re-argued by CITY-00:

1. The town is a **fictionalized Potes / Liébana mountain-valley market town**, not a 1:1 rebuild of
   real Potes and **not** primarily a coastal harbour town.
2. Harbour / open sea / fishing-port identity as the hero read is a declared **No**
   (`VISUAL_BIBLE.md` §2). A later river feature is permitted only as a **small port / landing on
   that river, working boats only**, keeping inland-waterway identity (`SETTING.md`).
3. Street/plaza/frontage/floor anchors (streets 4–6 m, plaza ~25×20 m order, 1 m grid) come from the
   visual bible, not from CITY.
4. The H2 hero budget (≤120 meshes, ≤6 atlases, ~20 clips, 6 NPCs) is a constraint to test, not a
   promise CITY may quietly inflate (`PRODUCTION_BLUEPRINT.md` §8.3 restates this).

`WP-ART-00` owns visual/setting direction. CITY consumes it and must not silently restyle the game
(`Docs/workpacks/CITY/README.md`). Where CITY-00 needs a setting fact that ART has not stated — the
fictional navigability premise that makes a fluvial landing plausible — it is recorded as an **open
question addressed to ART**, not decided here.

### Dependency 2 — `Docs/production/PRODUCTION_BLUEPRINT.md` v0.2

- Declared **NON-BINDING, owner-reviewed production proposal**. It creates no acceptance criterion
  and authorizes nothing.
- Inherited product decisions consumed by CITY-00: the demo is the first piece of the game (§0.1);
  the river/bridge/loops product-seed direction supersedes the earlier radial test hub (§12); the
  first serious build must be small enough to finish and good enough to keep (§1.4); reactive
  density and retained-first take priority over acreage; §10 rabbit-hole guards, in particular *do
  not model the whole valley before the keeper town core is fun and useful*.
- Explicitly **not** inherited as settled: the §1.2 semantic topology diagram. It predates the
  fluvial-port and city-size direction, contains no port, and `Docs/workpacks/CITY/README.md`
  authorizes CITY to sharpen or propose amendments to the blueprint. CITY-00 reconciles it (WP Work
  item 1) rather than treating it as fixed.

### Dependency 3 — Living World PA programme (non-binding product input)

- `Docs/research/living-world/PA_ROADMAP.md` v1.1 is `PROCESS_ONLY` and explicitly non-binding until
  reviewed/adopted. Old `Arkus0/Juego` PASS status carries no authority here.
- CITY-00 consumes PA material **only as spatial requirements**, and pre-accepts no PA finding:
  - `LIVING_WORLD_CROSSCUTTING_AMENDMENT_01` LW-X04/LW-X09: ordinary life and low-stakes play are
    first-class, so quiet ordinary fabric is a spatial obligation, not filler.
  - `PA-08` §4.3: activities consume real place/time/capacity, so activities need physical homes with
    scarcity (one table, limited spots, a bolera, fishing places).
  - `PA-12_AMENDMENT_01` §3 and §5.3: municipal levers (access/permission, schedule, capacity and
    allocation, cost/support, sponsorship, enforcement, public commitment) must be followable on foot
    — inspect a site, attend a meeting, talk to affected citizens. That is a requirement for
    *places*, and CITY-00 owns whether those places exist.
  - `PA-11` (Investigation, Legibility & Traces) needs branching routes and legitimate vantage, not
    an omniscient map.

### Guarantees newly owned by WP-CITY-00

CITY-00 owns, and CITY-01..04 inherit:

- the selected semantic district graph and the spatial role of each district family;
- the river/tributary/crossing/port structure and the reasons the port exists;
- the corrected scale envelope plus the explicit conditions that would shrink or expand it;
- the boundary between promised playable fabric and scenic envelope;
- the permanent spatial invariants;
- the rejected alternatives and the recorded reason each was rejected.

CITY-00 does **not** own walk-time measurement (CITY-01/04), the location/interior programme
(CITY-02), the seed boundary (CITY-03), or any engine/implementation authority.

### Concrete condition that would reopen an inherited input

- ART confirms the fictional river-navigability premise is refused, or fixes a port scale
  incompatible with a working-craft landing → the port's placement and therefore the selected
  topology must be reopened, not patched.
- An owner revision of `PRODUCTION_BLUEPRINT.md` restores a topology incompatible with the selected
  constitution → reconcile again rather than maintaining two contradictory topologies on `main`.
- A future measured traversal result from CITY-04 falsifies the travel-time arithmetic used here →
  reopen the scale envelope at its stated conditions, not by stretching the selected topology.

A theoretical preference for a different city, or a wish for a bigger one, is not such a condition.

## Claim and boundary

CITY-00 claims only this: that three materially different city topologies were constructed, that each
was attacked before any preference was formed, that the selected constitution answers the WP's
acceptance criteria, and that the resulting scale envelope follows from traversal, density and
content-cost reasoning rather than from the track's starting hypotheses.

Outside the claim: measured walk times, navmesh feasibility, final geometry, the location/interior
programme, the seed boundary, asset selection, engine behaviour and any `HK-*` guarantee.

CITY is non-foundational (`Docs/workpacks/README.md`): `FOUNDATIONAL_PROOF_STANDARD.md` is not
binding, exact-SHA validation evidence is not required, and nothing in this workpack may block,
weaken or reinterpret any `HK-*` acceptance criterion or the `WP-HK-GATE` precondition.

## Planned work and evidence

1. Build three topology dossiers, each placing all twelve required spatial ingredients.
2. Attack all three in `REFUTATION_LOG.md` **before** forming a preference.
3. Compare on the eight required axes plus the WP's six negative gates.
4. Select one constitution and record what each rejected option lost on.
5. Derive the scale envelope arithmetically and state its change conditions.
6. Write `Docs/production/CITY_SPATIAL_CONSTITUTION.md`.
7. Reconcile `PRODUCTION_BLUEPRINT.md` to the selected constitution.
8. Run the mandatory strict Worker pre-review, then freeze.

## Scope guard

No Unity scene, asset import, navmesh, runtime contract or `src/`/`tests/`/`tools/`/`scripts/`
change. No edit to `Docs/ROADMAP.md`, any `WP-HK-*`, or `Docs/engineering/RESIDUAL_LEDGER.md` (that
inventory is declared as covering accepted H0 workpacks). The starting size hypotheses are treated as
falsifiable, never as targets to justify after the fact.
