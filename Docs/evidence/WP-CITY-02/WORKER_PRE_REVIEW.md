# WP-CITY-02 — Worker pre-review

WP: `WP-CITY-02 — Systemic locations + spatial-depth/interior programme`  
Contract: `Docs/workpacks/CITY/WP-CITY-02.md`  
Baseline SHA: `0b8f23fbce227bbbc710c8410dff575fcb9fcf12`  
Active Worker: `ChatGPT GPT-5.6 Sol`  
Worker history: `ChatGPT GPT-5.6 Sol`  
Transfer SHA: `NONE`  
fail_cycle: **0**  
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7

This is Worker quality-gate evidence, not independent review.

---

## 1. Contract, predecessor and concurrency check

Direct dependency `WP-CITY-01` is accepted:

- reviewed candidate `3444555a983415645dcc2897b748d6c4f6294f19`;
- independent PASS review `#5263809993`;
- PR #73 merge `a9ff655e5bf2319690d919b88bd57389a32483f3`;
- DocSync PR #76 merge `96635f8d0dcf49a653ba7ec74fd112cde56a1188`.

The mandatory predecessor split is recorded in `WORKER_PLAN.md`. CITY-00 geography and CITY-01 movement/access are consumed rather than re-proved.

While CITY-02 was active, `main` advanced from the recorded baseline `0b8f23f...` to `d20f7452ac689399b66f0601e8d0d8faa86d01a5`. The baseline→new-main comparison contains only the accepted WP-H1-01 DocSync and H1/index documentation:

- `Docs/evidence/WP-H1-01/DOCSYNC.md`;
- `Docs/workpacks/H1/README.md`;
- `Docs/workpacks/H1/WP-H1-01.md`;
- `Docs/workpacks/README.md`.

No CITY-00/CITY-01 semantic owner, CITY-02 contract, ART input or Production Blueprint surface consumed by this workpack changed. Therefore the predecessor check remains valid and no semantic rebase is required before freeze.

Result: **PASS.**

---

## 2. Complete baseline→candidate diff / scope challenge

The Worker inspected the complete PR diff rather than only the latest repair. Before this pre-review evidence itself, product/evidence changes are confined to:

- `Docs/production/CITY_LOCATION_PROGRAMME.md` — sole CITY-02 semantic owner;
- `Docs/evidence/WP-CITY-02/WORKER_PLAN.md`;
- `Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md`;
- `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`.

There is no code, Unity scene, prefab/asset adoption, H0/H1 contract mutation, CITY-00/CITY-01 mutation, CITY-05 shell design, CITY-06 detailed interior/discovery design or Living World runtime implementation.

Result: **PASS.**

---

## 3. Classification-independence challenge

The candidate contains 37 programmed place/family rows. Derived audit totals are:

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

### Finding discovered and repaired during pre-review

The first candidate wrote `loc.ribera.service_yard` as `I0/I1` in the master ledger and A/B profile even though the dedicated interior ledger already assigned I1. That was a material ambiguity because CITY-02's DoD is to give CITY-05/CITY-06 one unambiguous programme rather than forcing them to decide the commitment again.

Commit `e6fe45d94701d4b2cec0cada6f6996287fac3874` repaired the semantic owner so both authoritative references now say **I1**. `PROGRAMME_AUDIT.md` was then reconciled after the repair.

No other slash/range interior assignment remains in the master/profile contract.

Result: **PASS AFTER 1 FINDING FIXED.**

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

`CAPACITY_SANITY.md` consumes the accepted approximate dense-fabric areas without claiming parcel precision. Twenty-one A/B handles fall within the ~0.365 km² dense-fabric subtotal, a crude pressure of about one A/B handle per ~17,380 m²; two further A/B places are in Vega outside that subtotal.

This is not a parcel-fit oracle. It establishes only that the programme has no obvious area-pressure contradiction before CITY-05. The depth burden is selective: one I3 hero, six I2, seven I1, with ordinary/scenic families and I0 remaining explicit.

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

The semantic owner repeatedly assigns those to later Living World/runtime owners. Likewise CITY-05 owns parcels/shells, CITY-06 owns detailed interior/discovery design, CITY-03 owns retained-seed choice and CITY-04 owns bounded physical measurement.

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
- no asset/prefab convenience is allowed to silently promote I0 fabric.

Result: **PASS.**

---

## 11. Residuals deliberately not promoted to solved facts

Still downstream:

- exact parcel/site placement, footprint and frontage dimensions — CITY-05;
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

One material in-claim ambiguity was discovered and repaired before freeze. The complete post-repair candidate has been challenged against the WP, inherited CITY-00/CITY-01 guarantees, scope boundary, negative gates, capacity pressure and downstream ownership. No known in-claim blocker remains.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 1
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-02/WORKER_PRE_REVIEW.md
PROGRAMME_AUDIT: Docs/evidence/WP-CITY-02/PROGRAMME_AUDIT.md
CAPACITY_SANITY: Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md
```

Next protocol step: create the final handoff as the last branch mutation, read the exact PR HEAD, record it as Candidate/Frozen SHA in PR #81, mark Ready, and stop Worker writes pending a fresh independent Reviewer.
