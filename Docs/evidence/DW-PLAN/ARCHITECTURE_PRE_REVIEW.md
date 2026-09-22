# DW architecture pre-review — PROCESS_ONLY

Status: COMPLETE for the repaired planning candidate; not an implementation PASS
Planning base: `main` at `13332b738626b43ace6047a9c141a840406bfed7`
Date: 2026-09-22

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 5
WORKER_PRE_REVIEW_EVIDENCE: `Docs/evidence/DW-PLAN/ARCHITECTURE_PRE_REVIEW.md`

## Scope

This is the architect/Worker's strict pre-review of the complete DW plan before any `WP-DW-*` is activated. It checks authority, workpack ownership, causal proof, cross-track sequencing, context-quality measurement and H2 interlock. It is not an independent Reviewer PASS and cannot authorize implementation by itself.

## Findings fixed during planning

### F1 — generalization test was initially too early

The first sketch placed a generic-abstraction pressure test immediately after the minimal CITY slice. That could only distinguish CITY from a synthetic fixture and would make later PA pressure arrive after the alleged generality decision.

**Repair:** generic-boundary stress moved to `DW-05`, after both real consumers and the context trial. It now uses CITY + PA evidence and only a tiny neutral falsification fixture; arbitrary-domain universality remains an explicit non-claim.

### F2 — attractive downstream uses were becoming active scope

The initial idea set included design↔Unity drift, generated art briefs, content coverage, QA replay and narrative knowledge checks. Pulling those into the validation track would create H1/H2 ownership conflicts and delay the second-consumer proof.

**Repair:** they are preserved in architecture/README as downstream adoption candidates only. No implementation WP exists until DW-GATE and the relevant H1/H2 owner/prerequisite exist.

### F3 — token saving could false-PASS on a weaker baseline or lost facts

A vague 'uses fewer tokens' claim could compare against an obsolete bloated prompt, cherry-pick tasks, omit fallback source cost or accept a plausible answer that missed a material blocker.

**Repair:** `DW-04` now depends on accepted `CTX-03`, freezes at least six CITY+PA tasks and their expected facts/verdicts before comparison, requires 100% required-fact/blocker preservation, counts all source fallback, and requires at least 30% lower median injected source-context bytes. Provider token usage is supplementary; deterministic byte accounting is primary.

### F4 — PA projection risked repeating the current false-green class

A typed PA index could appear complete while silently dropping an entire disposition/fixture surface, recreating the same omission family the repository is actively repairing elsewhere.

**Repair:** `DW-03` makes the PA source universe independent of projected rows and explicitly requires semantic RED controls for whole-surface disposition omission, finding omission and fixture/evidence-edge omission. It begins only after PA-01..05 are accepted locally.

### F5 — CITY architecture wording allowed a cherry-picked proof universe

The architecture initially described `DW-01` as using a "small independently selected POI universe" while the workpack correctly required the complete accepted universe relevant to each invariant. The looser architecture wording could let a later implementation prove the access invariant on a friendly subset.

**Repair:** architecture now requires the complete accepted source universe relevant to every invariant; for the initial access-role proof that means the full accepted CITY-02 A/B functional-POI universe. The same repair also makes explicit that DW-04 compares against accepted CTX-03 and therefore does not invalidate CTX.

## Per-workpack split review

| WP | Dual-claim challenge | Authority / predecessor challenge | Universe + causal-control challenge | Immediate-next / temporary-contract challenge | Scope/leakage challenge | Result |
|---|---|---|---|---|---|---|
| DW-00 | generic envelope + neutral reference are inseparable non-speculative contract claim | H0 consumed unchanged; source docs remain authority | independent neutral source universe; omission/stale provenance/rebuild controls must RED | DW-01 consumes final generic envelope | no CITY/PA/process vocabulary permitted in H0 | KEEP |
| DW-01 | CITY mapping + two real invariants form one first-real-consumer claim | accepted CITY truth encoded, not redesigned | complete relevant source universe independent of projection; entire-surface and role omission must RED through semantic oracle | DW-02 can expand data without changing authority model | no Unity/full CITY conversion | KEEP |
| DW-02 | production query completeness + content-shape report share one useful-CITY-projection claim | source universes stay CITY-owned | independent expected sets/counts; omitted source record must RED | PA receives stable generic boundary, not CITY contract | no invented hours/euros or final art tooling | KEEP |
| DW-03 | PA typed corpus + lossless query are one composition claim | only accepted PA-01..05 become initial authority inputs | source manifest independent of index; finding/disposition/fixture omission controls must RED | extension rule handles later accepted PA without pre-accepting it | no new research or destructive summaries | KEEP |
| DW-04 | quality preservation + context reduction are jointly necessary efficiency claim | accepted CTX-03 is baseline owner | frozen expected facts/verdicts; lost fact fails regardless of savings; all fallback counted | DW-05 receives measured evidence, not a universal token promise | no process state imported into DW | KEEP |
| DW-05 | domain-neutrality audit + H2 impact classification are one post-consumer boundary assessment | cannot repair H0; only route causal generic contradictions | neutral probe and independent limitation inventory prevent shared hidden assumption/omission | gate consumes final residual/H2 input state | synthetic third shape stays a falsification fixture, not product | KEEP |
| DW-GATE | closure/composition/H2 handoff only | all semantics owned by predecessors; H1-GATE remains separate | executed stage + residual universe; omission controls attack actual gate execution | no H2 implementation authorized by gate | no new domains/Unity/productization | KEEP |

## Cross-cutting pre-mortem questions

1. **Does DW make itself source of truth?** No. Architecture and every domain WP preserve accepted source authority; derived state is rebuildable/disposable and stale provenance is fail-closed.
2. **Can a projection prove its own completeness?** No. Every completeness-sensitive WP requires an independent/effective source universe or expected set.
3. **Can a negative control merely prove it deleted data while validation stays GREEN?** No. The plan repeatedly requires the semantic validator/query/gate pipeline itself to RED on the effective omission.
4. **Does CITY/PA vocabulary enter H0?** Forbidden. Domain-specific needs remain provider-owned; a genuine generic contradiction stops the WP and routes to an explicit reviewed delta/reopen.
5. **Does the plan reopen H0 speculatively?** No. H0 is consumed as accepted. Convenience and domain needs are explicitly insufficient reopen evidence.
6. **Does PA compression lose dispositions/negative findings/fixtures?** The initial source universe is independent of the index and whole-surface omission is a required RED control.
7. **Can token savings trade away quality?** No. 100% required fact/blocker/verdict preservation is a hard condition; 30% median byte reduction is additional, not substitutive.
8. **Is CTX invalidated or compared unfairly?** No. CTX remains independently valid; DW-04 waits for CTX-03 and uses the accepted context-efficient baseline.
9. **Does DW become circular process infrastructure?** No. Worker/Reviewer/freeze/DocSync/residual process state is excluded from DW v1.
10. **Does the track delay H1/local Unity?** No. H1 proceeds independently. DW is `REMOTE_OK` and only the future H2 boundary freeze consumes DW-GATE evidence.
11. **Are design↔Unity/art/QA/narrative opportunities lost?** No. They are durably preserved as downstream candidates with explicit prerequisite ownership, but not pre-authorized.
12. **Does a tiny third probe justify 'universal Arkus'?** No. DW-GATE explicitly claims only bounded second-consumer evidence; arbitrary-domain universality and external packaging remain non-claims.
13. **Can H2 ignore the experiment after the public boundary is frozen?** If this plan passes, final H2 external/public-boundary acceptance must explicitly incorporate or disposition DW-GATE evidence. H2 exploration may occur earlier, but the boundary is not silently frozen around an untested assumption.
14. **Are WPs too small/large?** No. Entity/invariant/query families are not split into micro-WPs, while generic contract, first consumer, useful CITY projection, PA losslessness, context efficiency, generalization assessment and closure remain independently rejectable.

## Reviewer-failure forecast

| Risk | WP(s) | Why independent review is likely to find real defects there |
|---|---|---|
| source authority/provenance accidentally becomes cached-index authority | DW-00 | rebuild/index convenience can hide stale authority |
| source universe self-shrinks when a CITY row/role disappears | DW-01/02 | projection-backed enumeration is the easiest false green |
| content-shape report smuggles estimates/defaults not present in source | DW-02 | planning usefulness can tempt invented precision |
| PA disposition/negative finding/fixture lost by compact schema | DW-03 | semantic compression is exactly where absence can look plausible |
| token trial cherry-picks or counts baseline/DW context asymmetrically | DW-04 | efficiency metrics are easy to game accidentally |
| CITY/PA assumption leaks into shared provider/kernel | DW-05 | two in-project domains can still share hidden conventions |
| residual/stage omitted while gate remains GREEN | DW-GATE | closure manifests can self-shrink unless independently reconciled |

The plan expects Reviewer FAIL at these owners to be informative and bounded rather than catastrophic to one omnibus implementation.

## Mechanical planning validation

- architecture defines authority, projection, H0 reopen classification, invariant boundary, CITY/PA consumers, context trial, H1/H2 interlock, process exclusion, determinism and exit boundary;
- `Docs/workpacks/DW/README.md` defines one serial seven-step DAG with explicit PA/CTX side prerequisites and H1 independence;
- global workpack index explicitly states that CTX remains valid and that DW-04 consumes accepted CTX-03 as its comparison baseline;
- future H2 planning signal records the conditional DW-GATE evidence interlock without pre-accepting DW;
- all seven DW contract files contain central claim, boundary rationale, inherited/new guarantees, excluded re-proof, allowed/forbidden scope, authority boundary, acceptance, deterministic proof, causal negative classes, residuals, reopen condition and PASS consequence;
- all planned implementation WPs are `REMOTE_OK`; no Unity evidence is falsely claimed;
- no planned WP authorizes full Markdown replacement, process-state modeling, H0 silent modification or arbitrary-domain product claims;
- the only H2 interlock is final external/public-boundary acceptance consuming/dispositioning DW-GATE evidence; DW does not authorize H2 implementation;
- planning changes are documentation/process only; product/runtime source is outside this planning PR.

## Freeze decision

The repaired planning design is internally coherent after the five repairs above and is suitable for independent review. No DW implementation workpack is active. A Reviewer should challenge the plan itself, especially whether DW-04's quality/efficiency oracle is fair, whether the H2 interlock is causally justified, and whether DW-00 can truly consume accepted H0 rather than introducing a disguised second canonical store.
