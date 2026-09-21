# WP-CITY-01 — Worker pre-review

WP: `WP-CITY-01 — Mobility, district graph + walk-time topology`  
Contract: `Docs/workpacks/CITY/WP-CITY-01.md`  
Baseline SHA: `7fe44840076eba05f1b67a7633cd33fc67b9023d`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**  
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7

This is a Worker quality gate, **not** independent review. CITY-01 is non-foundational; `FOUNDATIONAL_PROOF_STANDARD.md`, exact-SHA foundational proof machinery and foundational negative-conformance requirements do not apply. The predecessor check, complete-diff challenge, clean pre-review and frozen independent-review handoff do apply.

The exact Frozen candidate SHA is recorded in PR #73 after this report and `HANDOFF.md` are committed.

---

## 1. Predecessor contract check revalidated

`Docs/evidence/WP-CITY-01/WORKER_PLAN.md` records the pre-implementation `PREDECESSOR_CONTRACT_CHECK`.

Direct accepted dependency remains `WP-CITY-00`, accepted on candidate `f5c684461b525841487158d873a9b135008df3ab`, independent review `#5261672961`, PR #65 merge `69bbba2603e67cb233a3e1cc36a4173bed41cfc4`, with the accepted CITY Programme v2 and its DocSync already on the Worker baseline.

While CITY-01 was active, `main` advanced to `b02f9dfdc0fae1e38f66d7a527584a2ea80a055f` through H1 planning DocSync PR #74. That later main change touches H1/ROADMAP/session/workpack-index surfaces, not CITY-00, the CITY-01 contract or any CITY production/evidence source consumed by this candidate. PR #73 remains mergeable. No accepted direct dependency changed, so the predecessor check remains valid without rebasing the semantic candidate onto unrelated H1 documentation.

Inherited guarantees still consumed rather than re-proved:

- CITY-00 landmasses and planar relation;
- complete crossing set X1..X7 and State-1/State-2 availability;
- no dry Wedge→Puerto or Ensanche→Orilla-sur connection;
- three Wedge longitudinal route families;
- Plaza independence by design;
- CITY-00 permanent `CSI-*` invariants;
- adopted scale/walk-time values as planning hypotheses;
- CITY-03 exact-seed ownership and CITY-04 bounded local-greybox scope.

Result: **PASS.**

---

## 2. Claim and trust boundary

Claim under review: inside the accepted CITY-00 geography, CITY-01 provides one coherent semantic movement/access graph with differentiated route/access/elevation classes, mobility-profile assumptions, falsifiable planning costs, meaningful loops/alternates, a practical municipal closure lever, all mandatory route scenarios and a scope-correct measurement handoff.

Trusted predecessor inputs:

- `Docs/production/CITY_SPATIAL_CONSTITUTION.md`;
- `Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md`;
- `Docs/evidence/WP-CITY-00/PLANAR_EMBEDDING.md`;
- `Docs/evidence/WP-CITY-00/SCALE_ENVELOPE.md`;
- accepted CITY Programme v2 ownership split.

Outside claim:

- final metres/grades/geometry;
- Unity/navmesh behaviour;
- measured full-city traversal;
- final CITY-03 seed boundary;
- CITY-02 location/systemic programme;
- interior topology;
- runtime route-planning AI, beliefs or schedules;
- bus/vehicle simulation;
- State-1→State-2 production timing.

Result: **PASS — the candidate does not convert planning topology into runtime/engine authority.**

---

## 3. Complete baseline→candidate semantic diff

Before this pre-review/process file, the candidate changed exactly three semantic/evidence surfaces:

1. `Docs/production/CITY_MOBILITY_TOPOLOGY.md` — authoritative CITY-01 semantic owner;
2. `Docs/evidence/WP-CITY-01/WORKER_PLAN.md` — process/predecessor evidence;
3. `Docs/evidence/WP-CITY-01/GRAPH_AUDIT.md` — derived graph/scenario audit, explicitly non-authoritative.

No CITY-00 source, workpack contract, Unity scene, asset, runtime code, test, H0/H1 contract, ART source or CITY-02/03/04 contract is modified.

The production document intentionally makes §5's edge ledger the **only** semantic graph owner. The district-family table is labelled a projection, and evidence files are forbidden from defining new edges. This addresses the specific multi-surface semantic-drift failure class learned during CITY-00.

Result: **PASS.**

---

## 4. CITY-00 compatibility / hidden-edge challenge

Recomputed from the authoritative edge ledger:

| Relation | Candidate inter-landmass edges | Expected |
|---|---|---|
| Wedge ↔ Ensanche | X2, X3, X4, X5 | exact match |
| Wedge ↔ Orilla sur | X1 + X6/X7 by state | exact match |
| Ensanche ↔ Orilla sur | none | none |
| dry Wedge ↔ Puerto | none | none |

Counts remain seven crossing IDs across the constitution and six coexisting in either state. `X6` and `X7` are mutually exclusive. Internal W/E/O edges stay inside their named landmass.

The district-family projection was challenged for accidental implied edges; every row decomposes into §5 ledger edges, including Casco↔Puerto via `W12-X1-O01` and landing↔Puerto only via X6/X7.

Result: **PASS — no eighth crossing or visually implied edge exists.**

---

## 5. Availability / permanent-floor challenge

The candidate distinguishes designed graph from availability:

- X2/X3 are the permanent Arroyo floor;
- X4 is seasonal/flood-closable;
- X5 is low-water only and never used as permanent redundancy;
- State 1 X6 can suspend in high water;
- the inherited rare State-1 2→1→0 Río ladder is preserved rather than “fixed” by inventing connectivity;
- State 2 X7 closure leaves pedestrians/handcarts on X1 while full cart freight waits.

The first quiet-evening scenario depended on low-water X5 and was rejected during pre-review. The repaired quiet alternative is a permanent Vega→landing low-paseo route versus the commercial middle route.

Result: **PASS.**

---

## 6. Route-cost arithmetic challenge

`GRAPH_AUDIT.md` recomputes the asserted costs from the ledger.

Key inherited targets remain inside CITY-00 planning bands:

- Plaza→Casco: 1.75 min;
- Ensanche home→Plaza: 4.0 min;
- Barrio Alto→Ribera: 6.5 min downhill, 7.5 uphill;
- Vega→Plaza: 6.5 min;
- X1 far head→Entrada: 7.5 min;
- Puerto quay→Calle Mayor shop: 9.0 min;
- landing→NE Calle Mayor: 8.0 min;
- Vega→Puerto: 15.0 min primary / 15.5 min plaza-free low route.

CITY-00-deferred costs are explicit without new topology:

- Ensanche home→Puerto State 2 primary: 12.5 min;
- permanent plaza-free: 16.0 min outward / 17.0 reverse because W11 becomes uphill;
- X6-suspended State-1 ordinary fallback: 14.25 min;
- Vega→Puerto X6-suspended ordinary fallback: 16.75 min.

A pre-review arithmetic defect was found and fixed: the first Plaza-removal example used W11's downhill 6.5-min weight while traversing it uphill. The correct public bypass cost is 16.55 min.

Result: **PASS after repair.**

---

## 7. Loops / universal-connector challenge

Meaningful loops derived from the ledger include:

- L1/L1′ cross-river loop, base motion ≈8.25 min plus State-1 ferry wait;
- permanent X2/X3 Arroyo residential loop ≈8.6 min;
- Wedge middle/low loop ≈21.5 min;
- Wedge upper/middle loop ≈10.5 min.

Under normal availability, removing X2, X3, X1, X6 or X7 individually does not disconnect ordinary pedestrian movement because the accepted differentiated alternate remains. Removing Plaza also leaves the public graph connected with restricted `W17` excluded.

No claim is made that cart freight has the same redundancy as pedestrians; the X7-closure freight interruption is intentional and inherited.

Result: **PASS — loops are meaningful and no Plaza/bridge is a forbidden universal connector.**

---

## 8. Mobility-profile challenge

All contract-requested profiles have deterministic spatial assumptions:

- ordinary pedestrian;
- slower pedestrian: E0/E1 ×1.35, E2/E3 ×1.50, non-cumulative;
- bicycle: AR/AP-E0/E1 ride, AP-E2/E3 dismount, AF inaccessible, X1 push-only, X6 not assumed, X7 rideable;
- service/delivery cart: AR/AS only; X2/X4/X7 allowed; X1/X3/X5/X6 not full-cart crossings;
- porter/carryable load: pedestrian topology including X6;
- arrival/bus: vehicle terminates at Entrada; passengers continue as pedestrians;
- following/search: public graph with AS only when legitimate access exists;
- time-sensitive: scenario cost choice only, not runtime NPC omniscience.

The first bicycle wording (“selected AP”) was underdefined and was repaired before freeze. Route-character `V` was also separated from access restriction so public `S/V + AR` edges cannot be accidentally treated as private; only `AS` is restricted.

Result: **PASS after repair.**

---

## 9. Required scenario matrix challenge

Every contract scenario is represented from the same ledger:

| Required scenario | Candidate case | Result |
|---|---|---|
| upper/residential home → workplace | Barrio Alto→Ribera | PASS |
| old quarter → port/work edge without Plaza | Casco→landing→X6/X7→quay | PASS |
| port delivery → commercial destination | State-2 cart; State-1 porter after break-bulk | PASS |
| rural/valley arrival → market/civic | both Vega→Plaza and Entrada/bus→Plaza | PASS |
| follow NPC across ≥2 district boundaries | Ensanche→X3→Barrio→Ribera→landing→X6/X7→Puerto | PASS |
| closure forces plausible alternate | X2→X4/X3 | PASS |
| late actor chooses faster/contextually different | X6 wait threshold versus X1 | PASS |
| quiet evening differs from market-day flow | permanent Vega→landing paseo versus commercial route | PASS |
| service/back route differs from public | restricted W17 | PASS |

Following/search was challenged as a corridor case. It contains real public branches at Barrio, Ribera and landing, and crosses Ensanche→Wedge→Orilla-sur movement.

Result: **PASS.**

---

## 10. Port / rural ordinary-movement challenge

The port is not mission-only:

- State-2 full-cart quay→Ribera workshop = 7.0 min;
- State-2 full-cart quay→Calle Mayor shop through W17 = 11.0 min;
- State-1 break-bulk porter quay→Ribera = 7.0 min + ferry wait;
- State-1 porter quay→shop = 9.0 min + ferry wait, with a 10.75-min X1 fallback.

The rural/arrival edge is also ordinary:

- Vega→Plaza = 6.5 min;
- State-2 Entrada/bus passenger→Plaza = 9.5 min;
- State-1 direct ferry = 9.5 min + wait;
- State-1 X1 fallback = 11.25 min.

The bus itself never enters the core, and State 1 never acquires a fake full-cart ferry crossing.

Result: **PASS.**

---

## 11. Municipal closure challenge

CITY-00 Q5 is resolved with X2 as the practical municipal Arroyo closure lever:

- normal Ensanche-home→Plaza: 4.0 min;
- X2 closed with X4 available: 7.8 min;
- X2 closed + flood removes X4: permanent pedestrian X3 fallback 9.0 min;
- cart crossing pauses only when both road-capable X2 and seasonal X4 are unavailable; pedestrian city remains usable via X3.

The closure therefore has visible cost and profile-specific consequences without deadlocking the city.

Result: **PASS.**

---

## 12. CITY-04 measurement-boundary challenge

This pre-review found the most important process/ownership defect in the first candidate.

The initial handoff told CITY-04 to measure every full-city benchmark. That contradicted the accepted programme:

- CITY-03 owns the exact retained seed and exact CITY-04 measurement pack;
- CITY-04 may build **only** that bounded seed;
- therefore CITY-04 cannot honestly measure a route such as Vega↔Puerto when the required geometry/endpoints are outside that seed.

`CITY_MOBILITY_TOPOLOGY.md` v1.2 repairs this by defining three evidence states:

- `MEASURED` — complete route/segment exists in the seed and was traversed;
- `CALIBRATED_COMPONENT` — represented class/edge was measured, but the full route was not built;
- `NOT_IN_SEED` — absent geometry prevents an end-to-end measured claim.

CITY-03 selects the measurement subset without changing CITY-01 topology. CITY-04 can calibrate segment/elevation/access classes but may not add them across unseen geometry and call the result a measured trip.

The remaining **FULL_CITY_ROUTE_MEASUREMENT** ownership gap is explicitly recorded as a residual. CITY-01 does not amend CITY-04, expand its Unity scope or assign a fictitious downstream owner.

Result: **PASS after causal repair.**

---

## 13. Scope / authority challenge

Forbidden-scope scan:

- no CITY-00 geography/crossing mutation;
- no CITY-02 systemic-location programme or A–D/S0–S4 assignment;
- no CITY-03 seed selection;
- no Unity/navmesh/assets;
- no runtime code/tests or NPC schedule implementation;
- no vehicle simulation or fast travel;
- no H0/H1 semantic change;
- no ART authority change.

Representative `W.SHOP`, `W.CASCO`, `W.RIBERA`, market/Plaza and home anchors are inherited planning anchors used by CITY-00/Production Blueprint; CITY-01 does not assign them systemic importance or final location programme.

Result: **PASS.**

---

## 14. Findings fixed before freeze

Six material findings were repaired during Worker pre-review:

1. directional W11 stair arithmetic in one Plaza-removal example;
2. underdefined bicycle treatment;
3. quiet-route scenario depended on seasonal X5;
4. arrival/bus profile stopped at Entrada without costing the required onward pedestrian trip;
5. `V` route-character wording conflicted with public `S/V + AR` edges;
6. CITY-04 handoff demanded impossible full-city measurement outside its bounded seed.

No known blocker remains after the repairs and the affected graph/scenario audit was rerun.

---

## 15. Residuals — not blockers / not fake proof

Explicit residuals:

1. exact metres, grades, stair counts and clearance where geometry does not yet exist;
2. actual slower-pedestrian multipliers;
3. bicycle comfort/clearance and real dismount points;
4. route readability, sightlines and followability in geometry;
5. realized X2/X6 closure costs where represented;
6. realized service-cart clearance on AR/AS edges;
7. whether the 10.5/11.0-min quiet-vs-commercial near-parity survives geometry;
8. full-city end-to-end route measurements outside the CITY-03 retained seed;
9. State 1→State 2 production timing.

These are handed downstream with ownership/boundary labels. None is silently declared measured or solved.

---

## 16. Worker verdict

The candidate satisfies the CITY-01 contract inside its declared planning boundary and no known in-claim blocker remains.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 6
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-01/WORKER_PRE_REVIEW.md
```

Next protocol step: commit the frozen handoff, read exact 40-character PR HEAD, record that SHA in PR #73 as `Candidate HEAD SHA` and `Frozen candidate SHA`, mark `Worker state: FROZEN_FOR_REVIEW`, `Branch frozen: YES`, return the PR to Ready, and stop all Worker writes pending a **fresh independent Reviewer**.
