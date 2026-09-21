# WP-CITY-03 — Worker → independent Reviewer handoff

WP: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Contract: `Docs/workpacks/CITY/WP-CITY-03.md`  
PR: **#94**  
Baseline SHA: `1b503cfabeca340c42055aaf31df90d72ca28e68`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**

This file is the Worker's **final tracked branch mutation before freeze**. The exact 40-character commit containing this handoff is read from PR #94 immediately afterward and recorded in PR metadata as both `Candidate HEAD SHA` and `Frozen candidate SHA`. PR metadata is the authoritative exact-SHA freeze record.

## Worker state at handoff

```text
Predecessor contract check: Docs/evidence/WP-CITY-03/WORKER_PLAN.md
Worker pre-review: CLEAN
Worker pre-review findings fixed: 3
Worker pre-review evidence: Docs/evidence/WP-CITY-03/WORKER_PRE_REVIEW.md
Seed comparison: Docs/evidence/WP-CITY-03/SEED_COMPARISON.md
Boundary/handoff audit: Docs/evidence/WP-CITY-03/BOUNDARY_AND_HANDOFF_AUDIT.md
Semantic owner: Docs/production/CITY_PRODUCT_SEED.md v0.2
Branch frozen after this commit: YES
Worker verdict: IN_REVIEW after PR metadata freeze
Reviewer verdict: PENDING fresh independent review
```

## Direct predecessor reconstructed

Accepted `WP-CITY-06`:

- candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c`;
- independent PASS review `#5268249668`;
- implementation merge PR #90 / `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`;
- DocSync PR #93 / Worker baseline `1b503cfabeca340c42055aaf31df90d72ca28e68`.

CITY-03 consumes CITY-06's reviewed interior/depth/discovery constraints and the transitive CITY-00/01/02/05 guarantees. It does not reopen or re-prove them absent concrete contradiction.

## Selected seed

Selected candidate: `seed.confluence_civic_commercial`.

Hard planning envelope:

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

The area is inside the inherited `0.03–0.06 km²` band. The candidate deliberately uses this hard-envelope metric and does not invent an unsupported net-land oracle.

Selected accepted anchors inside: W.LANDING, W.X1, O.X1, W.CASCO, W.X5, E.X5, W.PLAZA, W.SHOP.

Selected playable CITY-01 edges/crossings:

- W04, W05, W06, W12, W13;
- X1 permanent Río crossing with inherited AH semantics;
- X5 Arroyo foot crossing with inherited low-water-only availability.

No X6/X7 geometry is built. W.LANDING is only the future socket. No dry Wedge→Puerto or Ensanche→Orilla-sur path exists.

## Exact place/site handoff

Selected depth set:

- `loc.casco.bar` — A/S4/I3, `if.social_house + hero.bar_layered`, roles `{public,service,semi-private}`;
- `loc.plaza.ayuntamiento` — A/S3/I2, `if.civic_office`, roles `{public,service,private}`;
- `loc.calle.everyday_shop` — B/S2/I1, `if.retail_shallow`, roles `{public,service}`;
- market, bridgehead and shared court as I0 exterior-led content;
- ordinary C/S1 closed frontages.

F01–F08 and S01–S03 have exact bounded seed-local placement regions. The boundary audit verifies all 11 regions are inside the hard polygon and F01–F08 respect their accepted CITY-05 parcel planning bands.

Hard playable classification coverage is A/B/C, S1–S4 and I0–I3. D/S0 remains soft scenic context, not playable inflation.

## Route-choice boundary

`casco.micro.A/B` form one node-local public split/rejoin inside `W.CASCO` for CITY-04 follow/search tests. They are not new CITY-01 edges, do not prove city-level redundancy and may never route through the semi-private shared court.

## CITY-04 handoff

The candidate gives CITY-04:

- exact hard polygon and target anchors;
- selected accepted route/crossing set;
- bounded frontage/open-site regions;
- retained versus temporary declaration;
- five expansion seams;
- soft-envelope restrictions;
- thirteen spatial validation scenarios covering quiet morning, market flow, home/work/social trip, a materially different bridge/civic trip, follow/search A/B, bar thresholds, bridge traversal, delivery/material change, municipal access/service change, player perturbation, leave/return, multi-route-ready discovery and blocked route/alternate;
- measurement targets for W04/W05/W06/W12/W13/X1/X5;
- physical/access/sightline checks;
- causal owner routing for deviations.

Planning sums recomputed:

```text
X1 + W12 + W05 = 3.75 min
W04 + W05 + W06 = 4.50 min
W13 + X5 = 1.40 min when X5 is available
```

No absent full-city route may be relabelled as measured.

## Expansion seams

Five named seams:

1. commercial/Vega from W.SHOP;
2. Ensanche from E.X5 while preserving X5 conditionality;
3. direct Puerto future X6/X7 socket at W.LANDING;
4. Orilla-sur→Puerto/Entrada from O.X1 via future O01;
5. upper/Barrio future continuity only through accepted graph.

None requires moving the keeper core.

## CITY-08 reserved slice

`trial.city08.civic_commercial_corner` = bounded W04/plaza edge + F03 everyday shop + F07 ordinary closed mixed frontage + S01 market edge.

This is a non-hero reusable authoring slice; CITY-08 still owns the actual Arkus public authoring/reuse proof.

## Worker pre-review repairs

Three material pre-freeze findings were repaired:

1. removed unsupported approximate net-area precision;
2. added bounded exact site regions so CITY-04 does not reselect layout;
3. made the required home/work/social spatial scenario explicit without inventing runtime schedules/jobs/interiors.

`WORKER_PRE_REVIEW: CLEAN`  
`KNOWN_IN_CLAIM_BLOCKERS: 0`

## Reviewer challenge targets

The independent Reviewer should especially challenge:

- whether the three alternatives are genuinely different and comparison preceded selection;
- whether the hard boundary/anchors/sites are coherent enough for local execution without renewed city design;
- whether X1/X5 and water masks preserve CITY-00/01 topology with no accidental crossing;
- whether node-local Casco branching remains detail rather than a hidden graph rewrite;
- whether A/S/I/access/interior/discovery obligations survive selection exactly;
- whether scenario proxies stay spatial/readiness-only and do not smuggle PA/runtime claims;
- whether seams genuinely extend the city without foreseeable demolition of core anchors;
- whether temporary Unity presentation is prevented from becoming semantic authority.

## Freeze rule

After this handoff commit:

- Worker performs no further branch writes;
- exact PR HEAD is recorded in PR body as Candidate HEAD + Frozen candidate SHA;
- PR is marked Ready for review;
- next actor is a **fresh independent Reviewer**;
- any Reviewer FAIL returns CITY-03 to Draft for a fresh repair Worker and a new pre-review/freeze cycle.