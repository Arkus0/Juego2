# CTX-03 — Worker pre-review

`WORKER_PRE_REVIEW: CLEAN`

Status: **READY_FOR_FINAL_EXACT_SHA_VALIDATION_THEN_FREEZE**  
Baseline: `107694d3850a478849bffd9510dc030910fc8aa3`  
Complete implementation diff reviewed through pre-evidence candidate: `460f997c8e70720356f285583977530cc2175c76`  
Worker pre-review findings fixed: **10**  
Scope: **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**

This file is the final repository-byte persistence of the completed pre-review. It changes evidence only and does not change the process mechanisms evaluated below. No repository/evidence bytes may change after this commit unless CTX-03 returns to Draft + ACTIVE and the complete pre-review is rerun. The exact resulting evidence-bearing HEAD must rerun all registered validation before freeze/Ready.

## 1. Preconditions and predecessor boundary

- `WP-CTX-02` is independently accepted: candidate `e5053b778e050cff83e2443fef888c64883c88ca`, PASS review `#5274937744`, implementation merge `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`, successful combined DocSync main `107694d3850a478849bffd9510dc030910fc8aa3`.
- `Docs/evidence/CTX-03/PREDECESSOR_CONTRACT_CHECK.md` was persisted before implementation and remains factually valid.
- CTX-03 consumes rather than re-proves CTX-02 capsule semantics, checker-owned PA completeness/selectors, accepted semantic controls and Reviewer/source escalation authority.
- Live `main` advanced from the CTX-03 baseline only through accepted DW-04 plan refinement to `26327b6519456aea222f254f8476f40c35372224`; the baseline→live-main compare changes only `Docs/workpacks/DW/WP-DW-04.md`. It does not change CTX-02 acceptance, CTX-03 dependency validity or the CTX-03 write set. No merge-from-main is needed merely to import unrelated plan prose.

## 2. Complete baseline→candidate scope review

The full `107694d... -> 460f997...` compare was inspected, not only the latest commits. It contains 25 changed files and no product/runtime implementation:

- four role/DocSync skills;
- two process-only GitHub workflows;
- current ROADMAP history separation;
- one CTX-03 envelope protocol/config and one mechanical verifier registry;
- CTX-03 escalation, classification, measurement/control evidence;
- non-bootstrap history files;
- seven repository-owned deterministic process/checker scripts.

No H0/H1 runtime semantics, Unity product behavior, PA/CITY authority, accepted capsule payload, accepted-state index, foundational proof threshold or independent Reviewer authority is changed.

## 3. Acceptance / quality challenge

### Same-snapshot measurement

- Pre/post measurements use one candidate snapshot.
- The representative route universe is checker-owned.
- The canonical role-profile source and calibration substitutions are checker-owned.
- The pre-CTX direct-predecessor baseline is checker-owned and independent of the compact capsules being measured.
- Dynamic live PR/diff inputs remain mandatory but are held outside both sides of the static delta.
- Material saving requires exceeding declared combined uncertainty.

Hardened measured result: PA minimum route is materially smaller (`38389 -> 22855`, `40.4647%`); H1 (`35104 -> 29896`, `14.8359%`) is **not** labelled material; CITY intentionally grows because its non-compressible seed and unresolved cross-track gate remain mandatory. No universe was tuned to make every route look cheaper.

### Auditable escalation and quality preservation

- The escalation completeness universe comes from the canonical role profile rather than the record being audited.
- Every current Worker `must_escalate_if` predicate is represented in `CONTEXT_ESCALATIONS.json` with either opened authoritative source(s) or an explicit non-material reason.
- Quality replay uses the production route generator and checks H1 proof-source, CITY non-compressible seed and cumulative PA material-source reachability.
- Integration mutations remove the real PA/CITY source selectors and must make replay RED; the control does not merely assert that a fixture changed.
- The replay proves routing/discoverability only. It does not automate semantic equivalence or replace independent review.

### Cumulative PA

The accepted CTX-02 capsule family materially closes the measured cumulative starting-cost problem. CTX-03 therefore explicitly **does not** add a second PA compact registry. Material escalation still opens the authoritative PA results even when doing so is more expensive than pre-CTX.

### Process envelope

- Effective static read sets are derived from canonical accepted profiles.
- Config cannot remove a profile, redirect profile source, redirect calibration placeholders or shrink the representative route universe.
- Real required-source growth over a reviewed ceiling turns RED; 100KB unrelated repository growth stays GREEN.
- A ceiling increase requires an explicit revision increment plus justification.
- Final-shape profile baselines/ceilings are calibrated, including `planner_gate=19955/23946` after ROADMAP history separation.

### ROADMAP / DocSync / history separation

- Current ROADMAP v1.33 retains current H0 state, ordering, architecture consequences and gates while verbose accepted H0 closure chronology moved to `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md`.
- Exact representative historical PR/review/SHA pointers are mechanically protected.
- Normal role `initial_reads` contain no `Docs/history/**` source.
- The representative CTX-02 accepted transition has one compact CTX current-state row and consistent next-WP surfaces.
- DocSync instructions now regenerate one compact accepted-state projection and update only affected current/authoritative docs rather than restating chronology everywhere.

## 4. Mechanical false-red audit

The requested false-red review is implemented without weakening semantic/causal controls:

- `FAIL`: only a registered deterministic causal verifier may establish a mechanical candidate defect;
- `REVIEW_BLOCKED`: handoff/freeze/lifecycle/derivable metadata incomplete or stale;
- `NOT_APPLICABLE`: neutral, never synthetic proof;
- `INFRA_ERROR`: runner/API/tool/unknown-red state without candidate-defect proof; operationally blocks review without labelling the WP defective.

The registry itself is validated fail-closed: unique check names, no semantic authority, and every verifier capable of mechanical `FAIL` must require a structured outcome. An unregistered red check cannot count as WP FAIL, but also cannot be silently ignored: it produces operational `INFRA_ERROR` until triaged/registered.

Derivable handoff generation preserves non-derivable lineage from the existing canonical PR body (`Baseline SHA`, Worker identity/history, Transfer SHA, prior reviewed SHA, `fail_cycle`) and can derive only final HEAD/freeze/Ready pointers after predecessor-check + CLEAN evidence exist. The existing handoff lint remains the independent oracle.

`REVIEW_READY_CLOSED` is automatic after adoption: the existing Automation V2 bot marker triggers a post-marker workflow that rereads gates/HEAD and persists CLOSED only if the exact frozen SHA remains live. There is no second human click/comment/metadata entry. For this adoption candidate, because the new issue-comment workflow is not yet on default main, the exact equivalent terminal invariant must be closed by observing the real `REVIEW_READY` marker and then performing a direct post-marker live HEAD read with no repository mutation.

## 5. Findings fixed during complete pre-review

1. **Invalid post-marker checkout design** — early workflow tried to consume a local `pr.json` as an expression input. Repaired by fetching the canonical PR first, exporting exact live HEAD, checking it out, then rereading state.
2. **Uncalibrated/obsolete envelope** — calibrated exact role baselines and recalibrated planner after ROADMAP closure-history separation.
3. **Missing causal quality replay** — added production-route source-reachability replay plus real PA/CITY material-source removal controls.
4. **History separation initially cosmetic** — moved verbose H0 accepted closure chronology out of current ROADMAP and protected reconstruction pointers mechanically.
5. **Unknown red verifier could be mishandled** — unregistered red now cannot WP-FAIL and cannot authorize review; it becomes `INFRA_ERROR` until triaged.
6. **Closure outcome too generic** — terminal closure now emits structured `REVIEW_BLOCKED` versus `INFRA_ERROR` rather than one undifferentiated failure.
7. **Metadata generator could reset lineage** — removed caller-selected baseline/history/fail-cycle inputs; those values are preserved from the existing canonical handoff.
8. **Self-shrinking measurement baseline** — pre-CTX source inventory originally depended on current capsule sources. Replaced with checker-owned canonical representative predecessor inventories.
9. **Config could choose its own measurement universe** — canonical route set, canonical profile source, full profile universe and calibration substitutions are now checker-owned/exact-bound; narrowing/redirection controls RED.
10. **FAIL-capable registry entry did not require structured result** — `ctx-process-envelope` now requires a structured outcome and registry validation rejects any unstructured FAIL-capable or semantic-authority mechanical verifier.

These are causal boundary repairs, not example-only patches.

## 6. Validation observed before final evidence persistence

Exact candidate `460f997c8e70720356f285583977530cc2175c76`:

- Context Capsule Validation run `35704295647`: **SUCCESS**;
- Arkus Candidate Validation run `35704295700`: **SUCCESS**;
- CTX Process Envelope run `35704295690`: **SUCCESS**.

The CTX job completed every self-test, causal suite, false-red classification and artifact upload. The final evidence-bearing commit created by this file must receive the same exact-SHA GREEN surface before freeze.

## 7. Residual risk / explicit non-claims

- The byte/4 estimator is a declared provider-neutral approximation, not an exact vendor tokenizer. Materiality therefore uses a conservative uncertainty rule.
- Quality replay proves material-source routing/reconstruction reachability, not model semantic competence. Independent Reviewer challenge remains mandatory.
- `REVIEW_READY_CLOSED` automation becomes available to future candidates only after CTX-03 reaches default `main`; this candidate uses the same terminal invariant directly without fabricating its own default-branch trigger.
- Process ceilings govern repository-owned static bootstrap sources only; dynamic PR diff/live GitHub/local-manifest payload size remains mandatory and observable but outside this static corpus budget.
- CTX-03 does not decide H2/H3 repository split or product ownership.

No known in-claim blocker remains after the complete pre-review.

`WORKER_PRE_REVIEW: CLEAN`
