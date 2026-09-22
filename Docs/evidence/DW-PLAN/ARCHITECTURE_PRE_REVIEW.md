# DW architecture pre-review — PROCESS_ONLY

Status: COMPLETE for the repaired planning candidate; not an implementation PASS
Planning base: `main` at `13332b738626b43ace6047a9c141a840406bfed7`
Date: 2026-09-22

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 8
WORKER_PRE_REVIEW_RERUN_AFTER_REVIEW_FAIL: YES
WORKER_PRE_REVIEW_REVIEW_FAILS_REPAIRED: `#5274609395`, `#5274691431`
WORKER_PRE_REVIEW_RERUN_SCOPE: full DW architecture/workpack/gate set + foundational standard adoption + DW-04 paired-agent/task-selection contract + top-level ROADMAP milestone/gate authority + H2 interlock reconciliation
WORKER_PRE_REVIEW_EVIDENCE: `Docs/evidence/DW-PLAN/ARCHITECTURE_PRE_REVIEW.md`

## Scope

This is the architect/Worker's strict pre-review of the complete DW plan before any `WP-DW-*` is activated. It checks authority, workpack ownership, causal proof, foundational-proof adoption, representative product probing, cross-track sequencing, context-quality measurement, actual agent-quality evidence, task-universe independence, top-level roadmap/gate registration and H2 interlock. It is not an independent Reviewer PASS and cannot authorize implementation by itself.

## Findings fixed during planning

### F1 — generalization test was initially too early

The first sketch placed a generic-abstraction pressure test immediately after the minimal CITY slice. That could only distinguish CITY from a synthetic fixture and would make later PA pressure arrive after the alleged generality decision.

**Repair:** generic-boundary stress moved to `DW-05`, after both real consumers and the context trial. It now uses CITY + PA evidence and only a tiny neutral falsification fixture; arbitrary-domain universality remains an explicit non-claim.

### F2 — attractive downstream uses were becoming active scope

The initial idea set included design↔Unity drift, generated art briefs, content coverage, QA replay and narrative knowledge checks. Pulling those into the validation track would create H1/H2 ownership conflicts and delay the second-consumer proof.

**Repair:** they are preserved in architecture/README as downstream adoption candidates only. No implementation WP exists until DW-GATE and the relevant H1/H2 owner/prerequisite exist.

### F3 — token saving could false-PASS on a weaker baseline or lost facts

A vague 'uses fewer tokens' claim could compare against an obsolete bloated prompt, cherry-pick tasks, omit fallback source cost or accept a plausible answer that missed a material blocker.

**Repair:** `DW-04` depends on accepted `CTX-03`, counts all source fallback, preserves deterministic expected facts/blockers/verdicts, and requires at least 30% lower median injected source-context bytes. F7 below further hardens this from context-package completeness into an actual paired-agent quality claim and prevents post-tuning task selection.

### F4 — PA projection risked repeating the current false-green class

A typed PA index could appear complete while silently dropping an entire disposition/fixture surface, recreating the same omission family the repository is actively repairing elsewhere.

**Repair:** `DW-03` makes the PA source universe independent of projected rows and explicitly requires semantic RED controls for whole-surface disposition omission, finding omission and fixture/evidence-edge omission. It begins only after PA-01..05 are accepted locally.

### F5 — CITY architecture wording allowed a cherry-picked proof universe

The architecture initially described `DW-01` as using a "small independently selected POI universe" while the workpack correctly required the complete accepted universe relevant to each invariant. The looser architecture wording could let a later implementation prove the access invariant on a friendly subset.

**Repair:** architecture requires the complete accepted source universe relevant to every invariant; for the initial access-role proof that means the full accepted CITY-02 A/B functional-POI universe. The same repair also makes explicit that DW-04 compares against accepted CTX-03 and therefore does not invalidate CTX.

### F6 — foundational-proof adoption and DW-00 product probe were inconsistent

Independent review found that the DW plan marked all seven workpacks `FOUNDATIONAL` and named `FOUNDATIONAL_PROOF_STANDARD.md` as binding, while accepted v1.4 normatively adopted only H1. Merely adding DW metadata would still leave `WP-DW-00` contradictory because its content-shape probe was explicitly domain-neutral, whereas the foundational standard requires a bounded slice of the approved Juego2 target whenever a foundational WP defines/changes authorable-state or public-contract semantics.

**Repair:** `FOUNDATIONAL_PROOF_STANDARD.md` is advanced to v1.5 with an explicit DW adoption boundary. It applies to DW foundational cycles only after the accepted DW planning commit reaches `main`, does not bind this `PROCESS_ONLY` planning PR, and does not retroactively alter accepted H0/H1 evidence. `WP-DW-00` keeps its neutral independently enumerable fixture for mechanics/completeness/negative controls **and separately** requires one bounded approved-Juego2 content-shape probe through the same candidate surface. The real-content probe tests only representability, identity/granularity, provenance and DW-00-owned public boundaries; CITY semantic correctness/invariants remain exclusively DW-01's first-real-consumer claim.

### F7 — DW-04 semantically substituted context-package completeness for actual agent quality, and could self-select friendly tasks

Independent review of frozen candidate `dfb179e6876eb75294b2188022a6a2220f9811b9` found two coupled false-green paths. First, the workpack claimed preservation/improvement of Worker/Reviewer quality but allowed actual agent executions only "if available", so a smaller context package containing the expected facts could PASS even if a real agent failed to detect the required blocker or verdict. Second, the six tasks only had to be frozen before final comparison, allowing the DW-04 Worker to tune the route and then choose a friendly task set.

**Repair:** architecture v0.3, `WP-DW-04`, DW README and DW-GATE now make actual paired agent executions mandatory for every selected task. Each CTX/DW pair must use the same declared model/version, system/task prompt, decoding settings, tool permissions, execution budget and matched run-count policy; outputs are scored mechanically against pre-frozen expected facts/blockers/verdicts, so model prose is execution evidence rather than its own oracle. Before any route-specific tuning or result observation, DW-04 must durably freeze either the complete eligible historical-task universe plus deterministic selection/stratification rule or the exact independently reviewed task manifest, along with task prompts, authority anchors, expected facts/blockers/verdicts, scoring rules and run policy. Any later change invalidates prior tuning/results and restarts the trial. Deterministic context assembly remains a separate structural oracle and cannot substitute for the paired-agent layer.

### F8 — the new DW→H2 milestone gate was absent from the top-level roadmap authority

Independent review also found that lower-level DW/H2 planning surfaces declared a real conditional gate on final H2 public/external-boundary acceptance, but `Docs/ROADMAP.md` — the repository's top-level milestone/gate authority — still jumped directly from H1 to H2. That allowed milestone bootstrap/planning to ignore a gate that lower-level documents treated as binding.

**Repair:** `Docs/ROADMAP.md` advances to v1.31 and registers the proposed `DW-00 -> DW-01 -> DW-02 -> DW-03 -> DW-04 -> DW-05 -> DW-GATE` sequence, the PA/CTX cross-track prerequisites, H1 independence, and the conditional final-H2-boundary interlock. It explicitly preserves existing ownership: H1-GATE remains the Unity/gameplay prerequisite, DW does not authorize or generally block H2 implementation, and if the DW plan is accepted, final H2 public/external-boundary acceptance must consume accepted DW-GATE evidence or explicitly review/disposition the interlock. Architecture/README/GATE now point back to ROADMAP as the top-level authority.

## Per-workpack split review

| WP | Dual-claim challenge | Authority / predecessor challenge | Universe + causal-control challenge | Immediate-next / temporary-contract challenge | Scope/leakage challenge | Result |
|---|---|---|---|---|---|---|
| DW-00 | generic envelope + neutral mechanics fixture + representative Juego2 shape are one non-speculative public-surface contract claim, with the real probe explicitly non-semantic | H0 consumed unchanged; source docs remain authority; v1.5 applies only after planning merge | independent neutral source universe owns omission/stale provenance/rebuild RED controls; real Juego2 probe cannot substitute for that universe | DW-01 consumes final generic envelope and remains first CITY semantic/invariant owner | approved product content may traverse the probe, but no CITY/PA/process vocabulary may enter H0/public generic semantics | KEEP |
| DW-01 | CITY mapping + two real invariants form one first-real-consumer claim | accepted CITY truth encoded, not redesigned | complete relevant source universe independent of projection; entire-surface and role omission must RED through semantic oracle | DW-02 can expand data without changing authority model | no Unity/full CITY conversion | KEEP |
| DW-02 | production query completeness + content-shape report share one useful-CITY-projection claim | source universes stay CITY-owned | independent expected sets/counts; omitted source record must RED | PA receives stable generic boundary, not CITY contract | no invented hours/euros or final art tooling | KEEP |
| DW-03 | PA typed corpus + lossless query are one composition claim | only accepted PA-01..05 become initial authority inputs | source manifest independent of index; finding/disposition/fixture omission controls must RED | extension rule handles later accepted PA without pre-accepting it | no new research or destructive summaries | KEEP |
| DW-04 | structural context correctness + actual paired-agent quality preservation + context reduction are jointly necessary efficiency claim | accepted CTX-03 is baseline owner; PA/CITY remain task truth owners | pre-tuning eligible universe/selection rule or exact reviewed manifest; frozen expected facts/blockers/verdicts/scorer; every task gets matched CTX/DW executions and any lost required item fails regardless of savings | DW-05 receives measured evidence bounded to exact task/model/config/run policy, not a universal token promise | no process state imported into DW; no model allowed to self-score | KEEP |
| DW-05 | domain-neutrality audit + H2 impact classification are one post-consumer boundary assessment | cannot repair H0; only route causal generic contradictions | neutral probe and independent limitation inventory prevent shared hidden assumption/omission | gate consumes final residual/H2 input state | synthetic third shape stays a falsification fixture, not product | KEEP |
| DW-GATE | closure/composition/H2 handoff only | all semantics owned by predecessors; H1-GATE remains separate; ROADMAP owns milestone/gate order | executed stage + residual universe; omission controls attack actual gate execution; verifies DW-04 paired-run and task-freeze evidence | no H2 implementation authorized by gate | no new domains/Unity/productization | KEEP |

## Cross-cutting pre-mortem questions

1. **Does DW make itself source of truth?** No. Architecture and every domain WP preserve accepted source authority; derived state is rebuildable/disposable and stale provenance is fail-closed.
2. **Can a projection prove its own completeness?** No. Every completeness-sensitive WP requires an independent/effective source universe or expected set.
3. **Can a negative control merely prove it deleted data while validation stays GREEN?** No. The plan repeatedly requires the semantic validator/query/gate pipeline itself to RED on the effective omission.
4. **Does CITY/PA vocabulary enter H0?** Forbidden. Domain-specific needs remain provider-owned; a genuine generic contradiction stops the WP and routes to an explicit reviewed delta/reopen.
5. **Does the plan reopen H0 speculatively?** No. H0 is consumed as accepted. Convenience and domain needs are explicitly insufficient reopen evidence.
6. **Does PA compression lose dispositions/negative findings/fixtures?** The initial source universe is independent of the index and whole-surface omission is a required RED control.
7. **Can token savings trade away quality or can context-package completeness impersonate agent quality?** No. DW-04 requires both structural 100% expected-material/fallback coverage and mandatory paired same-config CTX-vs-DW agent executions mechanically scored against pre-frozen facts/blockers/verdicts. Any required miss/wrong verdict fails regardless of savings.
8. **Can DW-04 choose its own friendly success universe after tuning?** No. The eligible universe/selection rule or exact reviewed manifest plus scorer/run policy must be durably frozen before route-specific tuning/results; changing it invalidates/restarts the trial.
9. **Is CTX invalidated or compared unfairly?** No. CTX remains independently valid; DW-04 waits for CTX-03 and uses the accepted context-efficient baseline with matched agent configuration.
10. **Does DW become circular process infrastructure?** No. Worker/Reviewer/freeze/DocSync/residual process state is excluded from DW v1.
11. **Does the track delay H1/local Unity?** No. H1 proceeds independently. DW is `REMOTE_OK`; ROADMAP records only a conditional final-H2-public/external-boundary interlock after DW planning acceptance.
12. **Are design↔Unity/art/QA/narrative opportunities lost?** No. They are durably preserved as downstream candidates with explicit prerequisite ownership, but not pre-authorized.
13. **Does a tiny third probe justify 'universal Arkus'?** No. DW-GATE explicitly claims only bounded second-consumer evidence; arbitrary-domain universality and external packaging remain non-claims.
14. **Can H2 ignore the experiment after the public boundary is frozen?** Not if this plan is accepted. ROADMAP now requires final H2 public/external-boundary acceptance to consume accepted DW-GATE evidence or explicitly review/disposition the interlock. Earlier H2 exploration is not silently redefined by DW.
15. **Are WPs too small/large?** No. Entity/invariant/query families are not split into micro-WPs, while generic contract, first consumer, useful CITY projection, PA losslessness, context efficiency/agent quality, generalization assessment and closure remain independently rejectable.
16. **Is the foundational proof contract actually adopted for DW without creating a circular planning requirement?** Yes. v1.5 explicitly adopts later DW foundational Worker/review cycles only after this planning commit reaches `main`, excludes this PROCESS_ONLY planning PR, preserves H0/H1 historical evidence, and DW-00 contains both the neutral causal fixture and the separate required Juego2 representative probe.

## Reviewer-failure forecast

| Risk | WP(s) | Why independent review is likely to find real defects there |
|---|---|---|
| source authority/provenance accidentally becomes cached-index authority | DW-00 | rebuild/index convenience can hide stale authority |
| representative Juego2 shape forces hidden domain assumptions into generic envelope | DW-00 | a neutral fixture can be internally coherent while real approved content exposes wrong identity/granularity |
| source universe self-shrinks when a CITY row/role disappears | DW-01/02 | projection-backed enumeration is the easiest false green |
| content-shape report smuggles estimates/defaults not present in source | DW-02 | planning usefulness can tempt invented precision |
| PA disposition/negative finding/fixture lost by compact schema | DW-03 | semantic compression is exactly where absence can look plausible |
| token trial cherry-picks tasks/runs or counts baseline/DW context asymmetrically | DW-04 | efficiency metrics are easy to game accidentally; pre-tuning freeze and matched-run protocol must remain causal |
| context package contains the blocker but actual DW agent misses it | DW-04 | structural completeness does not guarantee agent reasoning quality; paired execution/scoring is independently rejectable |
| CITY/PA assumption leaks into shared provider/kernel | DW-05 | two in-project domains can still share hidden conventions |
| residual/stage or top-level H2 interlock omitted while gate remains GREEN | DW-GATE | closure manifests and lower-level docs can self-shrink unless independently reconciled with ROADMAP |

The plan expects Reviewer FAIL at these owners to be informative and bounded rather than catastrophic to one omnibus implementation.

## Mechanical planning validation

- architecture v0.3 defines authority, projection, H0 reopen classification, invariant boundary, CITY/PA consumers, pre-tuning task-selection freeze, paired actual agent-quality trial, H1/H2 interlock, process exclusion, determinism and exit boundary;
- `Docs/workpacks/DW/README.md` defines one serial seven-step DAG with explicit PA/CTX side prerequisites, H1 independence and the same paired-agent/task-selection rules as DW-04;
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.5 explicitly adopts the accepted DW foundational track only after this planning commit reaches `main`, excludes the PROCESS_ONLY adopter and preserves accepted H0/H1 evidence;
- `WP-DW-00` keeps an independent neutral mechanics/completeness fixture **and** separately requires the bounded approved-Juego2 representative content-shape probe demanded by the foundational standard, without moving CITY semantic invariant ownership out of DW-01;
- `WP-DW-04` cannot PASS from context-package completeness alone: every selected task requires paired CTX/DW actual agent runs under the same declared model/configuration/tool/run policy, scored mechanically against pre-frozen expected facts/blockers/verdicts;
- `WP-DW-04` cannot choose a friendly task universe after tuning: the eligible universe/selection rule or exact reviewed manifest, task prompts, authority anchors, scorer and run policy are frozen before route-specific tuning/results, with invalidation/restart on later change;
- `WP-DW-GATE` explicitly verifies both DW-04 proof layers and the chronology of the task-selection freeze rather than demoting model runs to optional observations;
- global workpack index explicitly states that CTX remains valid and that DW-04 consumes accepted CTX-03 as its comparison baseline;
- `Docs/ROADMAP.md` v1.31 now registers the proposed DW sequence and conditional final-H2-public/external-boundary interlock at the top-level milestone/gate authority, while preserving H1-GATE as the gameplay/Unity prerequisite;
- future H2 planning signal remains a lower-level conditional planning record and no longer carries the interlock without matching ROADMAP authority;
- all seven DW contract files contain central claim, boundary rationale, inherited/new guarantees, excluded re-proof, allowed/forbidden scope, authority boundary, acceptance, deterministic proof, causal negative classes, residuals, reopen condition and PASS consequence;
- all planned implementation WPs are `REMOTE_OK`; no Unity evidence is falsely claimed;
- no planned WP authorizes full Markdown replacement, process-state modeling, H0 silent modification or arbitrary-domain product claims;
- the only DW→H2 gate is final external/public-boundary acceptance consuming/dispositioning DW-GATE evidence after DW plan acceptance; DW does not authorize H2 implementation or replace H1-GATE;
- planning changes are documentation/process only; product/runtime source is outside this planning PR.

## Freeze decision

The repaired planning design is internally coherent after the eight repairs above and is suitable for independent review. This pre-review was rerun across the complete planning surface after the second Reviewer FAIL: architecture v0.3, DW-04 paired-agent/task-universe contract, DW README, DW-GATE, top-level ROADMAP v1.31, foundational adoption, all unchanged DW workpack boundaries and the H2 interlock were cross-checked together. No DW implementation workpack is active. A Reviewer should challenge the plan itself, especially whether DW-04's pre-tuning universe/selection freeze is genuinely independent of the result, whether paired CTX/DW executions are treatment-matched enough to support the bounded agent-quality claim, whether the deterministic scorer can observe every required blocker/verdict without becoming self-confirming, whether ROADMAP and lower-level H2/DW surfaces now agree exactly, and whether DW-00 can truly consume accepted H0 rather than introducing a disguised second canonical store.
