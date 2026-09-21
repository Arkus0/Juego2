# WP-CITY-03 — Worker → independent Reviewer handoff

WP: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Contract: `Docs/workpacks/CITY/WP-CITY-03.md`  
PR: **#94**  
Baseline SHA: `1b503cfabeca340c42055aaf31df90d72ca28e68`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**

This file is the Worker's **final tracked branch mutation before the new freeze**. The exact 40-character commit containing this handoff is read from PR #94 immediately afterward and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`.

## Prior independent FAIL

Failed frozen candidate: `68dafdc555be0732f31200aa95d8533c4fad2142`.  
Formal review: **#5268624526**.

Single blocker: CITY-03 claimed an exact hard playable boundary while `mask.rio` / `mask.arroyo` remained semantic-only and bank geometry was deferred to CITY-04. That forced the LOCAL worker to choose where land ended, how X1/X5 cut the banks and how receiving stubs/sites related to water/no-build.

The PR was returned to Draft before repair; the old freeze is retired.

## Worker state at repaired handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-03/WORKER_PLAN.md
Worker pre-review: CLEAN
Pre-review semantic candidate SHA: f2485725d480b3d51be2428e4aaa97d14f528e48
Worker pre-review evidence: Docs/evidence/WP-CITY-03/WORKER_PRE_REVIEW.md
Seed comparison: Docs/evidence/WP-CITY-03/SEED_COMPARISON.md
Boundary/handoff audit: Docs/evidence/WP-CITY-03/BOUNDARY_AND_HANDOFF_AUDIT.md
Semantic owner: Docs/production/CITY_PRODUCT_SEED.md v0.3
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING fresh independent review
```

## Direct predecessor reconstructed

Accepted `WP-CITY-06` remains unchanged:

- candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c`;
- independent PASS review `#5268249668`;
- implementation merge PR #90 / `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`;
- DocSync PR #93 / Worker baseline `1b503cfabeca340c42055aaf31df90d72ca28e68`.

No predecessor was amended or re-proved during the repair.

## Selected seed retained

Selected candidate remains `seed.confluence_civic_commercial`.

Hard outer envelope remains:

```text
B01 (-20,-60)
B02 (70,-80)
B03 (205,-48)
B04 (245,22)
B05 (220,95)
B06 (150,145)
B07 (70,132)
B08 (-20,55)
AREA = 44,817.5 m² = 0.0448175 km²
```

This remains inside the inherited `0.03–0.06 km²` band.

## Exact hard playable-boundary repair

CITY-03 now freezes exact seed-local water/site geometry before Unity:

### Río

```text
(-20,-8),(0,-10),(20,-24),(40,-42),(60,-46),(95,-45),(140,-42),(185,-40),(205,-38),
(205,-48),(185,-52),(140,-57),(95,-61),(60,-62),(40,-58),(20,-43),(0,-30),(-20,-24)
```

Area: `3,505.0 m²`.

### Arroyo

```text
(-20,-8),(0,10),(20,28),(40,48),(60,68),(78,90),(88,111),(108,132),(120,140.125),
(100,136.875),(99,130),(90,118),(82,106),(70,86),(50,60),(30,40),(8,20),(-20,4)
```

Area: `713.6875 m²`.

The masks meet at `(-20,-8)` on the confluence cut; Arroyo reaches the hard upper boundary. Dry subtraction produces exactly three components:

- Wedge `31,704.125 m²`;
- Orilla-sur `3,200.0 m²`;
- Ensanche receiving component `5,694.6875 m²`.

### Bank/no-build

```text
nb.rio_bank = ((buffer(mask.rio, 3.0 m) ∩ hard_outer) - mask.rio) - named X1 crossing/stubs
nb.arroyo_bank = ((buffer(mask.arroyo, 2.0 m) ∩ hard_outer) - mask.arroyo) - named X5 crossing/stubs
```

These bands prohibit unowned shells/crossings but do not invent public graph edges.

### X1

```text
x1.crossing = (53,-33),(45,-65),(39,-63),(47,-31)
stub.x1.wedge  = (42,-40),(58,-40),(58,-22),(42,-22)  # S02
stub.x1.orilla = (34,-69),(50,-72),(52,-62),(37,-60)
```

Permanent; Wedge↔Orilla-sur only; AH/no-cart semantics preserved.

### X5

```text
x5.crossing = (90.5,107.8),(88.5,121.8),(91.5,122.2),(93.5,108.2)
stub.x5.wedge    = (84,99),(99,99),(96,108),(87,108)
stub.x5.ensanche = (86,119),(93,124),(94,130),(84,128)
```

Low-water-only; when unavailable its water intersection remains non-traversable.

Playable-set rule:

```text
dry_land = hard_outer - (mask.rio ∪ mask.arroyo)
permanent_playable = dry_land ∪ x1.crossing
low_water_playable = permanent_playable ∪ x5.crossing
```

CITY-04 may falsify measured widths/grades/bends/bank treatment, but no longer chooses first bank limits or landmass cuts.

## Containment and negative controls

The repaired audit reports:

- 8/8 selected anchors inside the correct land component;
- 11/11 F01–F08/S01–S03 inside hard outer;
- 11/11 site regions outside water;
- 11/11 outside effective bank no-build after named crossing exemptions only;
- Wedge/Orilla/Ensanche dry components preserved;
- X1 reconnects only Wedge↔Orilla;
- X5 reconnects Ensanche only when available;
- no dry Wedge→Puerto or Ensanche→Orilla-sur route;
- no extra water crossing or upstream mask-skirt seam.

Negative controls now include removing/shifting either water mask, separating them at confluence, or adding an unnamed bank-clearance exemption; each fails for the intended causal reason.

## Unchanged accepted parts of the prior candidate

The repair does **not** change:

- three genuine seed alternatives or selected seed identity;
- W04/W05/W06/W12/W13;
- X1/X5 inherited access semantics;
- F01–F08 / S01–S03 identities and frontage-family bindings;
- bar I3, ayuntamiento I2, shop I1, ordinary I0 fabric;
- CITY-05 access-role sets;
- CITY-06 authored-vs-future discovery boundary;
- `casco.micro.A/B` node-local split/rejoin;
- five expansion seams;
- 13 CITY-04 spatial scenarios;
- CITY-08 reserved authoring-proof slice.

SCN-07 and SCN-13 are only sharpened to reference the new exact water/crossing geometry.

## CITY-04 handoff now complete

The LOCAL worker receives:

- exact outer polygon;
- exact water polygons;
- exact bank/no-build derivation;
- exact X1/X5 overlays and receiving stubs;
- target anchors in their correct landmasses;
- bounded site regions;
- selected route/place/depth/access obligations;
- scenarios, measurement duties and owner-tagged failure routing.

It can therefore **build exactly this and falsify it** without doing first-pass macro-layout/bank design.

## Worker pre-review

The full pre-review was rerun after the semantic repair, tied to semantic candidate `f2485725d480b3d51be2428e4aaa97d14f528e48`.

`WORKER_PRE_REVIEW: CLEAN`  
`KNOWN_IN_CLAIM_BLOCKERS: 0`

## Reviewer challenge targets

Fresh Reviewer should especially challenge:

- reproducibility and containment of the exact Río/Arroyo polygons;
- whether the masks truly produce the three intended landmasses;
- X1/X5 crossing/stub cuts and conditional X5 semantics;
- whether the 3 m / 2 m no-build rules accidentally consume any selected F/S region;
- whether confluence and upper Arroyo termination leave any dry unintended bypass;
- whether CITY-04 still has enough falsification authority without being asked to redesign the site;
- whether the repair touched anything beyond the failed boundary family.

## Freeze rule

After this handoff commit:

- Worker performs no further branch writes;
- exact PR HEAD is recorded in PR body as Candidate HEAD + Frozen candidate SHA;
- PR is marked Ready for review;
- next actor is a **fresh independent Reviewer**;
- any further Reviewer FAIL returns CITY-03 to Draft for another repair cycle.