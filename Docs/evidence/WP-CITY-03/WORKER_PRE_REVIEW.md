# WP-CITY-03 — Strict Worker pre-review

WP: `WP-CITY-03 — Retained product seed + exact scenario specification`  
Contract: `Docs/workpacks/CITY/WP-CITY-03.md`  
Baseline SHA: `1b503cfabeca340c42055aaf31df90d72ca28e68`  
PR: `#94`  
Pre-review semantic candidate HEAD before this report: `f2485725d480b3d51be2428e4aaa97d14f528e48`  
Prior failed frozen candidate: `68dafdc555be0732f31200aa95d8533c4fad2142`  
Independent FAIL review: `#5268624526`  
Worker: `ChatGPT GPT-5.6 Sol`  
fail_cycle: **1**

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED_THIS_CYCLE: 1 reviewer blocker`  
`WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-03/WORKER_PRE_REVIEW.md`

This is Worker quality-gate evidence only. It is not an independent PASS and does not authorize merge.

## 1. Repair scope reconstructed

The Reviewer accepted the prior candidate's seed selection, three alternatives, X5 conditionality, absence of a dry Puerto edge, CITY-05 roles, CITY-06 depth/discovery mix, scenario pack and expansion seams. The single independent blocker was causal and precise: CITY-03 claimed an exact hard playable boundary while leaving Río/Arroyo bank limits to CITY-04.

The repair therefore changes only:

- `Docs/production/CITY_PRODUCT_SEED.md`;
- `Docs/evidence/WP-CITY-03/BOUNDARY_AND_HANDOFF_AUDIT.md`.

Comparison `68dafdc555be0732f31200aa95d8533c4fad2142..f2485725d480b3d51be2428e4aaa97d14f528e48` is exactly two commits ahead and touches only those two files. No accepted predecessor, workpack contract, seed-comparison file, runtime/code/Unity surface or unrelated product document changed.

Result: **REPAIR BOUNDARY CONTAINED**.

## 2. Predecessor-contract check

Direct accepted dependency remains CITY-06:

- candidate `c73cbb19f8b152bd4b97ce3154eb18a78deaec3c`;
- independent PASS review `#5268249668`;
- implementation merge PR #90 / `acb84ec9ba7aa94e962bf7b35d7e53c0549ba758`;
- DocSync PR #93 / Worker baseline `1b503cfabeca340c42055aaf31df90d72ca28e68`.

The repair consumes rather than re-proves CITY-00 geography/seed band, CITY-01 mobility/access, CITY-02 place/depth/capacity, CITY-05 exterior/access-role grammar and CITY-06 interior/discovery grammar.

`PREDECESSOR_CONTRACT_CHECK: VALID`

## 3. Reviewer blocker — exact hard playable boundary

### Prior defect

The failed candidate froze only the outer envelope and stated that water remained non-traversable, but `mask.rio` / `mask.arroyo` had no metric geometry. The audit explicitly deferred bank geometry to CITY-04. A LOCAL Worker therefore still had to decide:

- where Wedge/Orilla-sur/Ensanche land ended;
- how wide each water exclusion was;
- where X1/X5 cut the banks;
- how much receiving-stub land existed;
- whether site regions touched water/no-build.

That contradicted the CITY-03 DoD.

### Repair

`CITY_PRODUCT_SEED.md` v0.3 now freezes:

- exact 19-vertex Río polygon;
- exact 18-vertex Arroyo polygon;
- confluence meeting at `(-20,-8)`;
- Arroyo termination exactly on hard edge `B07→B06`;
- 3 m Río / 2 m Arroyo Euclidean bank no-build derivation;
- exact X1 and X5 crossing polygons;
- exact Wedge/Orilla/Ensanche crossing stubs;
- explicit `dry_land`, `permanent_playable`, `low_water_playable` set algebra.

CITY-04 may still falsify widths/grades/bends/bank treatment, but a material measured correction must be reported to CITY-03; it cannot silently choose the first bank limit.

Disposition: **FIXED**.

## 4. Mechanical geometry challenge

`BOUNDARY_AND_HANDOFF_AUDIT.md` was rerun against the same definitions.

### A. Outer envelope

- area = `44,817.5 m² = 0.0448175 km²`;
- remains inside inherited `0.03–0.06 km²` band.

Result: **CLEAN**.

### B. Water masks

- `mask.rio` area = `3,505.0 m²`;
- `mask.arroyo` area = `713.6875 m²`;
- combined water = `4,218.6875 m²`;
- masks meet at confluence cut and Arroyo reaches the upper hard edge.

Result: **CLEAN**.

### C. Landmass negative control

Outer minus exact water yields exactly three dry connected components:

- Wedge `31,704.125 m²`;
- Orilla-sur `3,200.0 m²`;
- Ensanche receiving component `5,694.6875 m²`.

Required anchors fall into the correct component. Removing either water mask or leaving a confluence gap destroys this property and therefore fails the intended control.

Result: **CLEAN / CAUSAL**.

### D. X1

Exact crossing overlay joins W.X1 to O.X1 and pierces only Río. With X1 present and X5 unavailable, Wedge+Orilla are connected while Ensanche remains separate.

Result: **CLEAN**.

### E. X5

Exact crossing overlay joins W.X5 to E.X5 and pierces only Arroyo. It is added to the playable set only under the inherited low-water condition. With X5 unavailable there is no substitute cut; with X5 available the Ensanche component joins the Wedge.

Result: **CLEAN**.

### F. Bank/no-build

The no-build bands are deterministic mathematical derivatives of the two masks, clipped to the hard outer polygon and subtracting only named crossing+stub exemptions. They do not create a graph edge.

Result: **CLEAN**.

### G. Site containment

F01–F08 and S01–S03:

- 11/11 inside hard outer polygon;
- 11/11 outside water polygons;
- 11/11 outside effective bank no-build;
- S02's proximity to Río is justified only because S02 is itself the exact named X1 Wedge stub.

F01–F08 remain in inherited CITY-05 family bands.

Result: **CLEAN**.

## 5. Contract acceptance challenge after repair

### Three genuine alternatives

Unchanged. `SEED_COMPARISON.md` still compares compact Casco/civic, selected civic-commercial and work-edge alternatives with materially different content/cost/yield.

Result: **CLEAN**.

### Geography/crossing truth

Selected crossings remain exactly X1 and conditional X5. No X6/X7 geometry, dry Puerto continuation, Ensanche↔Orilla-sur link or eighth crossing is introduced. The new exact masks make these prohibitions geometrically testable instead of merely semantic.

Result: **CLEAN**.

### Route choice without graph mutation

W04/W05/W06/W12/W13 + X1/X5 unchanged. `casco.micro.A/B` remain a W.CASCO-local split/rejoin and S03 remains semi-private, not a fallback edge.

Result: **CLEAN**.

### Place/depth/hero mix

Unchanged:

- bar A/S4/I3, sole I3 hero;
- ayuntamiento A/S3/I2;
- everyday shop B/S2/I1;
- I0 market/bridgehead/shared-court and ordinary closed frontages;
- D/S0 remains soft scenic context.

Result: **CLEAN**.

### CITY-05 access roles

Unchanged and still fail-closed:

- bar `{public,service,semi-private}`;
- ayuntamiento `{public,service,private}`;
- everyday shop `{public,service}`;
- shared court public passage + semi-private court.

No form qualifier substitutes for a role.

Result: **CLEAN**.

### CITY-06 depth/discovery boundary

Unchanged. Future PA/runtime routes remain `FUTURE_OWNER_CONDITIONAL`; no repair text claims runtime proof.

Result: **CLEAN**.

### Scenario pack

All thirteen scenarios remain. Two are strengthened without changing their ownership:

- SCN-07 now names `x1.crossing` and exact Río exclusion;
- SCN-13 explicitly requires X5's water intersection to become non-traversable when unavailable.

Result: **CLEAN**.

### Two materially different everyday trips

SCN-03 residential-threshold→shop→bar and SCN-04 Orilla→X1→civic/market remain materially different.

Result: **CLEAN**.

### Expansion seams

All five seams remain. Exact water geometry now proves they do not hide an extra dry seam around Río/Arroyo.

Result: **CLEAN**.

### CITY-04 measurement handoff

CITY-04 now receives boundary instantiation checks before route measurement, including three-landmass subtraction, X1 permanent connectivity, X5 conditional connectivity, bank clearances and site-vs-water/no-build checks.

CITY-04 still owns empirical falsification of planning dimensions.

Result: **CLEAN**.

## 6. Negative/error challenge

The repaired candidate now explicitly fails if any of the following are introduced:

1. X1 removed;
2. X5 made permanent;
3. Río shifted/removed so Wedge touches Orilla-sur;
4. Arroyo shifted/removed so Wedge touches Ensanche;
5. water masks separated at confluence leaving a dry Wedge continuation;
6. W.LANDING dry-connected to Puerto;
7. unnamed bank-clearance exemption added;
8. shared court publicised as a through-route;
9. shop service role deleted;
10. ordinary I0 frontages opened for convenience;
11. future PA semantics counted as implemented;
12. selected functional slots moved during greybox;
13. another I3 hero promoted;
14. soft-envelope path counted as playable.

The first seven directly exercise the repaired blocker family rather than merely restating prose.

## 7. Residuals deliberately left to CITY-04

Not blockers:

- measured channel width/profile/grade may prove the planning geometry poor;
- retaining walls, rocks, bank mesh tessellation and visual treatment remain greybox/art realization;
- precise collision/nav implementation remains local;
- exact shell footprint inside a bounded F-region remains local so long as family/access/clearance constraints hold;
- travel times, sightlines and route widths remain measurements rather than remote assertions.

The key distinction is now explicit: **CITY-04 may falsify a frozen boundary; it no longer invents that boundary.**

## 8. Proof-budget / scope check

CITY-03 is non-foundational. No new verifier framework, runtime harness, Unity content or predecessor amendment was introduced. The repair adds only the missing site geometry and recomputed evidence.

`PROOF_BUDGET_VERDICT: NOT_APPLICABLE_NON_FOUNDATIONAL`

## 9. Final Worker pre-review verdict

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED_THIS_CYCLE: 1`  
`KNOWN_IN_CLAIM_BLOCKERS: 0`

The semantic candidate `f2485725d480b3d51be2428e4aaa97d14f528e48` is ready for a new final handoff commit and exact-SHA freeze. The next Reviewer must review the new frozen SHA, not the failed `68dafdc...` candidate.