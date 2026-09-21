# WP-CITY-02 — Worker pre-review

WP: `WP-CITY-02 — Systemic locations + spatial-depth/interior programme`  
Contract: `Docs/workpacks/CITY/WP-CITY-02.md`  
Baseline SHA: `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **1**  
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7

This is Worker quality-gate evidence, not independent review. Cycle 1 exists because independent review #5265810937 correctly rejected the cycle-0 Q6 capacity oracle.

---

## 1. Contract, predecessor and concurrency check

Direct dependency `WP-CITY-01` is accepted:

- reviewed candidate `3444555a983415645dcc2897b748d6c4f6294f19`;
- independent PASS review `#5263809993`;
- PR #73 merge `a9ff655e5bf2319690d919b88bd57389a32483f3`;
- DocSync PR #76 merge `96635f8d0dcf49a653ba7ec74fd112cde56a1188`.

The mandatory predecessor split is recorded in `WORKER_PLAN.md`. CITY-00 geography and CITY-01 movement/access are consumed rather than re-proved.

At cycle-1 repair time PR #81 still reports base `main` as `d20f7452ac689399b66f0601e8d0d8faa86d01a5`; no accepted CITY-00/CITY-01 semantic owner, CITY-02 contract, ART input or Production Blueprint surface consumed by this workpack changed relative to the cycle-0 predecessor check. No semantic rebase is required for this bounded repair.

Result: **PASS.**

---

## 2. Complete baseline→candidate diff / scope challenge

The Worker inspected the complete PR diff and the independent FAIL, not only the latest repair.

Product/evidence changes remain confined to:

- `Docs/production/CITY_LOCATION_PROGRAMME.md` — sole CITY-02 place-programme semantic owner;
- `Docs/evidence/WP-CITY-02/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md`;
- `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`;
- this pre-review and final handoff evidence.

Cycle 1 does **not** alter the 37-row place programme, A–D/S0–S4 assignments, CITY-00 geography or CITY-01 movement semantics. It repairs only the evidence/acceptance argument for the CITY-00 Q6 capacity delegation.

There is no code, Unity scene, prefab/asset adoption, H0/H1 contract mutation, CITY-05 parcel/shell design, CITY-06 detailed interior/discovery design or Living World runtime implementation.

Result: **PASS.**

---

## 3. Classification-independence challenge

The candidate contains 37 programmed place/family rows. Derived audit totals remain:

- importance: `A=8`, `B=15`, `C=11`, `D=3`;
- depth: `S0=2`, `S1=17`, `S2=10`, `S3=7`, `S4=1`.

The axes cannot be reconstructed from one another:

- A appears at S1, S2, S3 and S4;
- B appears at S1, S2 and S3;
- C includes both ordinary S1 fabric and the S3 shared-court case;
- D includes S0 scenic envelope and one S1 landmark shell.

Direct anti-collapse witnesses include `loc.entrada.arrival` (`A/S1`), `loc.ensanche.neighbourhood_anchor` (`A/S2`), `loc.ribera.workshop` (`A/S3`), `loc.casco.bar` (`A/S4`) and `loc.casco.shared_court` (`C/S3`).

A later rule such as A→S4 or C→S1 would contradict the candidate rather than merely simplify it.

Result: **PASS.**

---

## 4. A/B profile and interior-backlog completeness challenge

All 23 A/B rows have one profile containing the WP-required fields:

- plausible users/time bands;
- activity/material role;
- information/witness potential;
- governance/access hook;
- interior need;
- change potential;
- player-absence spatial function.

The interior backlog resolves to exactly one priority for every programme row:

- I0 NONE = 23;
- I1 SHALLOW = 7;
- I2 DEEP = 6;
- I3 HERO = 1.

### Finding discovered and repaired during cycle-0 pre-review

The first candidate wrote `loc.ribera.service_yard` as `I0/I1` in the master ledger and A/B profile even though the dedicated interior ledger already assigned I1. Commit `e6fe45d94701d4b2cec0cada6f6996287fac3874` repaired both authoritative references to **I1**.

No other slash/range interior assignment remains in the master/profile contract.

Result: **PASS AFTER 1 WORKER FINDING FIXED.**

---

## 5. Required-domain / revisitation challenge

Every required WP domain has at least one physical place home: civic/governance, food/drink/social, retail/service, work/logistics, port/waterfront, residential, leisure/activity, health/authority where needed, rural/river, quiet/private/semi-private and arrival/visitor.

Every substantial district family has a non-visual reason to revisit. A anchors are deliberately distributed across Casco, Plaza, Barrio Alto, Ensanche, Ribera, Puerto, Entrada and Vega; Casco + Plaza account for only 2 of 8 A anchors.

Puerto is not mission-only scenery: it has an A work/concession hub plus landing, warehouse/logistics and worker-social B places. No Puerto place is S4, preserving the ART rule that harbour identity does not become the hero read.

Result: **PASS.**

---

## 6. CITY-00 deferred-question / capacity challenge

CITY-00 Q6/Q7/Q8 were challenged explicitly rather than assumed closed by prose.

### Q6 — fit without crowding quiet fabric

#### Cycle-0 false green

The rejected candidate used `21 dense A/B handles / 0.365 km²` (~17,380 m² per handle), then cited selective `I0..I3` depth as reassurance. Independent review #5265810937 correctly identified that this cannot prove capacity: a handle can be a threshold, square, yard or building, and I-depth does not measure exterior/footprint/frontage demand. Two materially different programmes could therefore receive the same positive result.

That oracle has been removed. Handle count is now descriptive only.

#### Cycle-1 capacity bound

`CAPACITY_SANITY.md` assigns every A/B place a role-based `T/S/M/L` **programme envelope** with a conservative upper spatial-demand ceiling. The envelope includes shell/footprint need plus dedicated yard/open-space/threshold apron where applicable, while excluding shared streets/routes and avoiding parcel boundaries or exact dimensions.

All dense-part calculations charge the **upper** class bound:

- Wedge: 26,100 / 150,000 m² = 17.4%, below 25% A/B cap; 60% hard ordinary/quiet reserve;
- Ensanche: 5,000 / 90,000 m² = 5.6%, below 15% cap; 70% reserve;
- Barrio Alto: 7,500 / 60,000 m² = 12.5%, below 20% cap; 65% reserve;
- Puerto: 13,100 / 45,000 m² = 29.1%, below 35% cap; 50% ordinary/work reserve;
- Entrada: 5,600 / 20,000 m² = 28.0%, below 30% cap; 55% edge/ordinary reserve.

Each part also holds 15% as uncommitted/circulation margin that CITY-02 cannot spend to make the arithmetic pass. Aggregate dense A/B upper demand is 57,300 m² (15.7%) while hard reserve totals 225,500 m².

The test is falsifiable rather than automatically positive. Entrada has only 400 m² A/B-cap headroom. If `loc.entrada.depot_forecourt` cannot truthfully stay within `M` and requires `L`, Entrada becomes 8,100 m² / 40.5% and **Q6 fails/reopens**. Any class-ceiling breach, new/promoted A/B place, part-cap breach, reserve borrowing or use of named protected quiet fabric as overflow requires recomputation before acceptance.

This closes coarse programme capacity without parcelising CITY-05. CITY-05 still owns actual site/parcel/shell composition and can discover a class-ceiling or shape/access conflict; it may not hide such a conflict by consuming the hard reserve.

Result: **PASS AFTER INDEPENDENT REVIEW BLOCKER REPAIRED.**

### Q7 — two ordinary non-port services at Entrada/Puerto

`loc.entrada.fonda` and `loc.entrada.depot_forecourt` provide lodging/food and road/depot service independent of river work.

### Q8 — physical homes for ordinary activities

The bar, market terrace, workshop/yard, Puerto loading/weighing yard, Ensanche neighbourhood space and Vega supply node provide physical candidate homes. CITY-02 does not invent mechanics or turn them into UI portals.

Result: **PASS.**

---

## 7. CITY-01 access/closure challenge

The programme introduces no new movement edge or crossing. Location anchors use the accepted graph only.

Explicit challenges:

- X2 closure leaves meaningful Ensanche content on its own bank and inherited X3/X4 pedestrian alternatives where available;
- X6 suspension keeps Puerto meaningful on Orilla sur and uses inherited X1 + camino-sur fallback, not a dry shortcut;
- X7 closure preserves the accepted cart-freight interruption rather than inventing another freight crossing;
- Plaza closure does not erase the city's other A/B places or make restricted `W17` an ordinary-public shortcut;
- `W17` remains service/back relation only;
- quiet-route placement does not turn Q routes into continuous event frontage.

Result: **PASS.**

---

## 8. Reactive-density false-green challenge

The Worker challenged whether “reactive density” could be gamed by counting decorative abundance or opening everything.

The candidate prevents that:

- `MDT` counts distinct A/B thresholds, not props, C/D fabric, scenic envelope or UI markers;
- `RUR` requires repeated meaningful use across distinct place roles rather than repeating one interaction;
- `RCD` excludes restricted AS service routes from ordinary-public route choice and respects seasonal/State availability;
- `QSB` explicitly tracks quiet/low-intensity route/fabric rather than treating it as missing content;
- `DD` cross-tabs A–D × S0–S4 and separately I0–I3, with collapse/scope-inflation warnings;
- geometry absent from the selected seed remains `NOT_IN_SEED`, not fabricated as measured.

No metric requires every door, prop, pedestrian or location to be interactive/incident-generating.

Result: **PASS.**

---

## 9. Living World / ownership challenge

A/B profiles deliberately contain demand windows, role families, witness potential and player-absence rationale because the WP requires spatial needs for later social systems. They do **not** freeze:

- NPC schedules or opening hours;
- beliefs/knowledge state;
- dialogue;
- relationships;
- decision policy;
- incident-generation logic;
- off-screen simulation implementation.

The semantic owner repeatedly assigns those to later Living World/runtime owners. Likewise CITY-05 owns actual parcels/shells, CITY-06 owns detailed interior/discovery design, CITY-03 owns retained-seed choice and CITY-04 owns bounded physical measurement.

The new `T/S/M/L` capacity classes do not alter that ownership. They are conservative acceptance bounds, not site plans.

Result: **PASS.**

---

## 10. Negative gates / ordinary-city challenge

The explicit negative gates survive the complete candidate:

- most future ordinary fabric may remain C/S1 and I0;
- D/S0 scenic envelope remains non-playable context;
- only one I3/S4 hero interior is promised;
- quiet Vega/ravine/upper residential fabric has value without incidents;
- ordinary Puerto sheds remain C/S1 rather than becoming missions/interiors;
- no persistent-every-pedestrian contract exists;
- no “every prop smart” contract exists;
- no asset/prefab convenience is allowed to silently promote I0 fabric;
- the Q6 capacity oracle cannot pass merely because handle count is low.

Result: **PASS.**

---

## 11. Residuals deliberately not promoted to solved facts

Still downstream:

- exact parcel/site placement, exact footprint/frontage dimensions and shell composition — CITY-05;
- any discovery that a programmed place exceeds its CITY-02 capacity class ceiling — must trigger Q6 recomputation/review rather than quiet-fabric borrowing;
- exact interior layouts/discovery routes — CITY-06;
- exact retained seed — CITY-03;
- realized blockout and traversal/reactive-density measurements inside the accepted seed — CITY-04;
- full-city physical measurements outside the seed until sufficient realized geometry has an accepted owner;
- runtime schedules/beliefs/dialogue/relationships/decisions/incidents/off-screen simulation;
- final IDs, NPC bindings, narrative/backstories and minigame mechanics;
- State 1→State 2 production timing.

These are not hidden acceptance claims of CITY-02.

---

## 12. Worker verdict

Cycle 0 contained one Worker-found interior ambiguity and later received one independent-review FAIL on Q6 capacity. Both are now repaired in their causal surfaces:

- `e6fe45d...` — service-yard interior priority made unambiguous;
- `9661006d7f617dad287c7ea383763748dcb2ab6f` — false-green handle-density capacity check replaced by role-based upper demand classes, per-part caps, hard reserve, margin and explicit fail/reopen rules;
- `1f4cad98fe105f0f13a9ace591a3bec962909687` — programme audit reconciled to the repaired Q6 proof.

The complete cycle-1 candidate has been challenged against the WP, the exact reviewer blocker, inherited CITY-00/CITY-01 guarantees, scope boundary, negative gates and downstream ownership. No known in-claim blocker remains.

```text
WORKER_PRE_REVIEW: CLEAN
FAIL_CYCLE: 1
WORKER_PRE_REVIEW_FINDINGS_FIXED: 1
INDEPENDENT_REVIEW_BLOCKERS_FIXED: 1
REVIEWER_FAIL_REPAIRED: #5265810937
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-02/WORKER_PRE_REVIEW.md
PROGRAMME_AUDIT: Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md
CAPACITY_SANITY: Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md
```

Next protocol step: update the final handoff as the last branch mutation, read exact PR HEAD, record it as Candidate/Frozen SHA in PR #81, mark Ready, and stop Worker writes pending fresh independent review.
