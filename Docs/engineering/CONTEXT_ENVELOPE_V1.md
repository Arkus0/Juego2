# Context Envelope v1

Status: **CTX-03 CANDIDATE / PROCESS_ONLY**

## Purpose

Close the CTX programme by measuring context savings and bounding future mandatory-context growth **without reducing semantic/proof quality**. This protocol adds derived process measurements, auditable escalation, deterministic lifecycle closure, fail-closed dynamic repository budgets and mechanical false-red classification. It does not move product/proof authority into compact context or automation.

Machine/evidence surfaces:

- role source: `Docs/engineering/context-bootstrap-profiles.json`;
- envelope/measurement config: `Docs/engineering/context-envelope.json`;
- static/representative checker: `scripts/context-envelope-check.py`;
- dynamic repository checker: `scripts/ctx03-dynamic-context-check.py`;
- independent process controls: `scripts/ctx03-process-controls.py`;
- final B1/B2 circuit breaker: `scripts/ctx03-final-circuit-breaker.py`;
- quality-preservation replay: `scripts/ctx03-quality-replay.py`;
- DocSync/history/current-state checker: `scripts/ctx03-docsync-history-check.py`;
- verifier registry: `Docs/engineering/mechanical-verifier-registry.json`;
- mechanical outcome classifier: `scripts/mechanical-verifier-classifier.py`;
- review metadata generator: `scripts/derive-worker-review-metadata.py`;
- post-marker closure oracle: `scripts/review-ready-closure.py`;
- historical classification: `Docs/evidence/CTX-03/HISTORICAL_CLASSIFICATION.json`;
- non-bootstrap history: `Docs/history/CTX_PROCESS_HISTORY.md` + `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md`;
- CI: `.github/workflows/context-envelope-validation.yml`;
- automatic post-marker closure after adoption: `.github/workflows/review-ready-closure.yml`.

All compact/config/index/control surfaces are process/navigation evidence only. Exact workpacks, authoritative architecture/proof/result sources, code/tests, live GitHub and independent Reviewer judgment keep their existing authority.

## 1. Quality invariant

CTX-03 is successful only when a compact route preserves the same ability to discover, reconstruct and challenge material sources. Token/context savings never justify:

- omitting an authoritative source that a material claim requires;
- converting a derived table/capsule/index into semantic or proof authority;
- suppressing a mandatory escalation;
- replacing independent semantic review with a mechanical PASS;
- weakening accepted CTX-02 omission/substitution/source-consistency controls;
- letting a compact artifact define the universe that the same checker declares complete.

Historical-blocker replay therefore tests **routing and causal reachability**, not whether a particular model happens to notice every semantic problem.

## 2. Same-snapshot measurement

The estimator is deliberately simple and reproducible: `ceil(UTF-8 bytes / 4)` for each unique repository source. This is a provider-neutral context estimate, not a claim about an exact vendor tokenizer.

Provider-token uncertainty is declared separately in `context-envelope.json`. A saving is called **material** only when:

```text
pre_estimate - post_estimate > pre_uncertainty + post_uncertainty
```

Pre/post bytes are always read from the **same checked-out candidate snapshot**. Repository/history growth therefore cannot masquerade as routing improvement.

The representative route universe, canonical profile source, calibration substitutions and pre-CTX direct-predecessor source universe are checker-owned. `context-envelope.json` repeats them only as reviewed assertions. The artifact being measured therefore cannot narrow its own baseline or choose a friendlier route after seeing the result.

The six H1/CITY/PA routes are calibration/quality-replay cases, **not** the complete universe of future mandatory repository context. A separate dynamic repository envelope covers route-dependent repository sources for arbitrary future WPs and roles.

Genuinely external payloads may remain outside repository corpus estimates: live GitHub metadata, API response bytes and a generated complete PR diff are examples. A payload is not external merely because its identity is selected at runtime. Exact WPs, repository dependency/evidence files, anchored repository manifests and manifest-named repository files are repository-backed and are budgeted by section 5.

## 3. Auditable context escalation

A `CONTEXT_ESCALATIONS` record evaluates every `must_escalate_if` predicate of the effective role profile. A triggered predicate must name at least one existing authoritative source opened because of it. A non-triggered predicate must record why it is non-material.

The checker independently obtains the required predicate universe from the canonical accepted role profile. The escalation record therefore cannot define its own completeness by deleting one predicate, replacing the collection with `[]`, removing the collection field, or supplying a syntactically valid semantically empty equivalent.

`CONTEXT_ESCALATIONS` is navigation/audit evidence only. It may not prove a semantic claim or prevent a later Worker/Reviewer from deepening further.

## 4. Cumulative PA decision

CTX-03 measures PA as a cumulative consumer rather than a direct-predecessor happy path. Accepted CTX-02 already provides one capsule per accepted PA result, completion-side discovery, exact disposition status preservation and fail-closed source reconstruction.

The hardened same-snapshot measurement showed a material minimum-route reduction for the cumulative PA case while authoritative escalation can intentionally become larger than pre-CTX. That is correct: escalation buys source fidelity, not token savings.

Decision: **KEEP CTX-02 CAPSULE CHAIN / DO NOT ADD A SECOND PA COMPACT REGISTRY IN CTX-03**. CTX-02 closes the dominant cumulative starting-cost problem and CTX-03 keeps the canonical PA result reachable on material escalation. Another compact registry would duplicate navigation state without demonstrated need.

## 5. CI process envelope — four layers

The process envelope has **four** distinct layers. They solve different completeness problems and none may be treated as a substitute for another.

### 5.1 Base profile budget

Canonical repository-owned `initial_reads` that are fixed for an accepted role profile. Profile universe and calibration substitutions are independently checker-owned.

### 5.2 Fixed conditional-profile budget

Base pack plus a checker-owned superset of every fixed repository source that canonical `conditional_reads` can make mandatory for that profile. `CANONICAL_FIXED_CONDITIONAL_SOURCES` cannot be shrunk by editing the profile being audited. If the accepted profile adds a new explicit fixed `Docs/...` source absent from the checker oracle, CI is RED pending explicit reviewed oracle/budget evolution.

This layer covers Worker / repair Worker / Reviewer foundational/H1/ROADMAP/capsule conditionals, planner foundational proof, DocSync ROADMAP/capsule conditionals, and the local executor's empty fixed-conditional set.

### 5.3 Representative route-effective budget

Concrete checker-owned H1/CITY/PA minimum and escalated routes include capsule payloads, non-compressible sources and authoritative predecessor/result escalation. They are quality-preservation and calibration cases; they do not enumerate every future WP.

### 5.4 Dynamic repository envelope

`scripts/ctx03-dynamic-context-check.py` owns the classification of every dynamic placeholder appearing in canonical role `initial_reads`.

Each slot is classified as one of:

- genuinely external;
- repository-backed exact source;
- repository-or-external, requiring an explicit concrete classification at resolution time;
- dependency-derived repository set;
- manifest-derived repository set.

The audited profile/config cannot relabel a repository slot as external. A new placeholder not known to the checker turns RED until its ownership class is explicitly reviewed. A removed placeholder also turns RED until the checker oracle is deliberately reconciled, preventing stale hidden registries.

For a concrete route, every resolved repository-backed source is subject to:

```text
per-source estimate <= reviewed per_source_ceiling_estimate
sum(unique dynamic repository source estimates) <= reviewed aggregate_route_ceiling_estimate
```

Initial reviewed dynamic policy:

- per resolved repository source ceiling: `32768` estimate units;
- aggregate dynamic route ceiling: `131072` estimate units;
- policy revision: `1`.

These are conservative policy bounds rather than a claim that one representative route calibrated all future WPs. Any future increase requires an explicit `policy_revision` increment and non-empty `ceiling_increase_justification`; the base-ref comparison rejects silent ceiling expansion.

The resolver independently re-discovers repository paths named by the exact route contract. For Reviewer dependency navigation it also reconstructs exact dependency contracts/evidence from the exact contract's `Depends on:` authority. For `h1_local_executor`, manifest-named repository files are derived from the anchored repository manifest rather than from a caller-maintained list. Thus deleting a source from a route binding cannot stop it being counted while another routing authority still requires it.

A totally unrelated repository file that is not mandatory for the resolved route remains outside that route's dynamic estimate.

### 5.5 External boundary

The following can remain outside repository corpus ceilings when they are genuinely external at route resolution: live GitHub metadata, an API response, a current Reviewer FAIL stored only as a GitHub review/comment, a generated complete PR diff, or an external handoff anchor.

If the same logical input is persisted as a repository file and used as mandatory context, it becomes repository-backed for that route and is budgeted. “Dynamic” is not an exemption.

### 5.6 Future extension rule

A new role profile using existing reviewed dynamic slot classes is automatically covered. A new fixed conditional source is automatically discovered or fails closed under the fixed-conditional oracle. A new dynamic placeholder class fails closed pending explicit oracle review. A new future WP, including one outside H1/CITY/PA, is covered by the dynamic per-source + aggregate envelope when its route is resolved.

## 6. Mechanical false-red audit

CTX-03 separates mechanical outcomes so protocol/runner noise does not consume independent Reviewer rounds.

- `FAIL` — a **registered deterministic verifier** causally proved a repository-owned condition required by this WP is false. Worker repair before review; never an automated semantic Reviewer verdict.
- `REVIEW_BLOCKED` — handoff/freeze/metadata/lifecycle state is incomplete, pending, stale or incoherent.
- `NOT_APPLICABLE` — verifier does not apply. Neutral; never synthetic GREEN proof and never FAIL.
- `INFRA_ERROR` — runner/tool/API/checkout or unknown/unclassified verifier failed without causal candidate-defect evidence.
- `PASS` — registered deterministic condition satisfied. Semantic independent review remains mandatory.

A crash or unregistered red check cannot be promoted to “the WP is wrong” merely because GitHub paints a check red.

## 7. Derivable review metadata and exact pre-review durability

The terminal sequence is intentionally non-self-referential:

1. while Draft + ACTIVE, finish and commit/push every repository/evidence byte belonging to the candidate;
2. stop writers and read the exact resulting HEAD;
3. perform the complete Worker pre-review against that exact HEAD and complete baseline→candidate diff;
4. if clean, create a durable GitHub PR issue comment containing `WORKER_PRE_REVIEW: CLEAN`, `Candidate SHA: <exact HEAD>`, findings-fixed count and evidence pointers;
5. do not mutate repository/evidence bytes after that review; derive/freeze only metadata for the same SHA.

A repository file written **after** the pre-review cannot serve as the final CLEAN record. Repository pre-review notes/checklists may exist as inputs, but the final exact-SHA CLEAN record lives on durable GitHub metadata after the last candidate-byte mutation.

`derive-worker-review-metadata.py` preserves non-derivable lineage. `validate-worker-handoff.py` resolves the external clean pointer and verifies same repository/PR, exact live candidate SHA, findings count and evidence pointer. Neither decides semantic adequacy.

## 8. Transactional REVIEW_READY closure

A Worker may tell the human to start a Reviewer only after final repository/evidence bytes, exact-SHA complete Worker pre-review, durable external CLEAN evidence, coherent Ready/freeze metadata, required exact-SHA gates, a durable `REVIEW_READY` marker for the same SHA, and a post-marker live HEAD read that still equals that SHA.

After CTX-03 adoption, `.github/workflows/review-ready-closure.yml` wakes from either the original bot `issue_comment` or completion of `Arkus Candidate Validation`. The workflow-run path reuses an already-existing durable REVIEW_READY marker for the same SHA. Therefore a first closure attempt can correctly block while a gate is red, then a same-SHA metadata/gate repair can rerun Candidate Validation and reach `REVIEW_READY_CLOSED` without requiring a duplicate marker.

Closure remains idempotent by PR+SHA and adds no semantic Reviewer authority.

## 9. Lifecycle controls

The canonical causal suite reproduces:

- CLEAN exact-SHA;
- repository mutation after CLEAN invalidates readiness;
- wrong-PR and wrong-SHA CLEAN pointers are RED;
- missing Ready marker blocks;
- existing marker + red gate blocks;
- same-SHA gate repair closes by reusing the durable marker;
- HEAD movement after marker blocks.

These controls exercise the real closure/handoff oracles rather than checking only that a fixture field was removed.

## 10. Derived current state, DocSync and history

`ACCEPTED_STATE_INDEX.json` is a **derived navigation projection**. It cannot tell the checker which current state should be expected.

`scripts/ctx03-docsync-history-check.py` independently discovers CTX current-state authority from numeric CTX workpack contracts under `Docs/workpacks/CTX/`:

- a contract is accepted only when its authoritative `Status` is `COMPLETE`;
- every accepted contract must have its required `Docs/evidence/CTX-XX/DOCSYNC.md` closure;
- the closure must identify the same WP, carry persisted/complete DocSync status, independent PASS provenance and implementation-PR provenance;
- accepted contracts must form a prefix; a later COMPLETE contract after an unaccepted predecessor is RED;
- the next contract is the first discovered numeric CTX contract not yet COMPLETE, or `null` when the track has no remaining contract.

Only after deriving that authority does the checker compare `ACCEPTED_STATE_INDEX` accepted/next hints. The index therefore cannot hide an accepted closure by removing its own row, invent a fictitious accepted WP, or redefine next state.

Current-state prose in CTX/root workpack indexes may reflect this authority, but it cannot define it. Explicit contradictory “next CTX” prose is rejected. Historical prose under `Docs/history/**` is deliberately not consulted to determine current state.

### 10.1 Required post-adoption simulation

Before CTX-03 freezes, the final circuit breaker must synthesize:

1. current pre-CTX-03 accepted state;
2. CTX-03 contract changed to COMPLETE plus a valid PASS/PR-bearing CTX-03 DocSync closure;
3. accepted-state projection updated to include CTX-03 and next derived correctly;
4. no checker/oracle code change.

That post-CTX-03 state must be GREEN. The same control must prove RED for a stale pre-CTX-03 projection, wrong next hint, removed accepted closure and fictitious accepted index-only WP. Editing history prose alone must not alter expected current state.

This is the adoption circuit breaker that prevents the checker from being frozen to the candidate's own pre-acceptance snapshot.

## 11. Final circuit-breaker audit

`scripts/ctx03-final-circuit-breaker.py` is independent of the two target checkers and drives their real oracles against synthetic mutations. It covers the B1 post-adoption transition and B2 future-route class, including a future invented track outside H1/CITY/PA, exact-WP growth, newly mandatory repository evidence, caller omission, non-mandatory growth, `repair_worker`, `h1_local_executor` manifest-named files and new dynamic placeholder fail-closed behavior.

The existing CTX-02 capsule controls, CTX-03 quality replay, process controls and lifecycle closure controls remain binding. The new final circuit breaker supplements them; it does not replace them.

Whole structured surfaces are challenged as collections and fields, not only row-by-row. A negative control is valid only when RED comes from the missing real condition rather than from a test-specific “field was deleted” assertion.

## 12. Adoption and future consumer

This protocol becomes binding only after `WP-CTX-03` receives independent PASS, merges and completes DocSync. It does not retroactively reinterpret accepted CTX-01/02 evidence.

The process-envelope and closure primitives are intentionally repository-generic enough for future consumers. CTX-03 does not decide the H2/H3 repository split or move product ownership.
