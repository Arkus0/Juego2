# WP-CTX-03 — Structured evidence, quality-preserving context envelope, DocSync and history separation

Status: **FROZEN PLAN / NOT_STARTED**
Class: **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**
Execution: **REMOTE_OK**
Depends on: `WP-CTX-02` PASS + merge + DocSync
Blocks: none

## Objective

Reduce repeated state/proof/history prose and bound future context growth **without reducing the ability of a fresh independent Reviewer to locate, reconstruct and challenge any material semantic/proof source**. Make DocSync/state drift easier to detect mechanically and close the CTX programme with reproducible before/after measurements of both context cost and review-quality preservation.

CTX-03 optimizes **causal information density**, not token count in isolation. A smaller bootstrap that hides a material source, turns a summary into proof, suppresses escalation or weakens independent review is a regression, not a success.

## Non-negotiable quality invariant

No CTX-03 mechanism may make a conforming fresh Reviewer less able to discover and challenge a real blocker than the pre-CTX process.

Savings are invalid when they are obtained by:

- deleting accepted evidence or making authoritative sources unreachable;
- treating a capsule/index/table as semantic or proof authority;
- disabling or discouraging escalation to an exact source when a claim makes it material;
- measuring success only by lower token count, lower FAIL rate or fewer Reviewer source reads;
- training Workers to satisfy a mechanical gate while leaving the causal defect intact.

The full authoritative source remains available whenever escalation is required. Independent Reviewer behavior, exact-SHA review and predecessor-reopen rules remain unchanged.

## Required inputs

- accepted CTX-01 role/bootstrap authority rules;
- accepted CTX-02 contract-capsule rules and exact post-DocSync `main` identity;
- current proof/residual matrices, PR handoff conventions, ROADMAP history and DocSync flow;
- representative H1/CITY/PA Worker and Reviewer session measurements;
- the accepted PA result chain and transverse PA amendments actually required by live PA contracts;
- live GitHub history of independent FAILs/repaired candidates used for the pre-freeze-gate classification;
- live GitHub history of incomplete or invalid Worker→Reviewer handoffs, including the CTX-01 family and the CTX-02 handoff-lint/freeze-repair cycle, re-derived from exact repository/PR state rather than chat memory;
- current role/bootstrap profiles and all documents that every normal role must read.

Any external/token-budget analysis is **input only**. Re-derive figures from the exact accepted repository state before using them as CTX-03 evidence; do not inherit historical counts, repair rates or projected token values as facts without remeasurement.

## Work

### 1. Reproducible same-snapshot context measurement

- version a reproducible measurement harness that can calculate the mandatory starting read set for each representative role/profile on one exact repository SHA;
- compare pre-CTX and post-CTX routing on the **same frozen snapshot** so product/history growth is not confused with routing savings;
- record total tokens, initial mandatory tokens, escalated-source tokens, effective source set and declared tokenizer/measurement uncertainty;
- measure both minimum-route and materially escalated routes where CTX routing permits both;
- persist enough inputs/commands to reproduce the numbers without private chat context;
- a claimed `material` saving must exceed the declared measurement uncertainty; otherwise record it as no demonstrated material improvement.

### 2. Auditable context escalation

Extend the already-required predecessor/reviewer reconstruction evidence with a compact durable escalation record (for example `CONTEXT_ESCALATIONS`) that states:

- which conditional/escalation predicates were evaluated;
- which authoritative sources were opened because a predicate triggered;
- which candidate predicates were evaluated as non-material to the current claim;
- any capsule/structured representation used only as navigation.

This record is audit/navigation evidence only. It may not become a semantic oracle or require verbose restatement of source contents.

### 3. Cumulative PA/research composition

Measure the accepted PA chain as a real cumulative consumer, not merely as a direct-predecessor case.

First determine from the **accepted CTX-02 result** whether its capsule family materially solves the cumulative composition cost. Do not duplicate CTX-02 machinery when it already closes the need.

If a material cumulative term remains, introduce a versioned structured result surface with stable normative section/role identifiers (or an equivalent mechanically addressable representation) so a downstream PA consumer can request exactly the accepted material it needs, such as canonical finding, failure modes, provenance, disposition and explicitly required scenarios/fixtures.

Requirements:

- the canonical PA result remains authoritative and fully reconstructible;
- mixed disposition/status semantics remain exact and are never flattened;
- causal scenarios/fixtures stay mandatory whenever the current claim needs them;
- a consumer cannot define completeness solely from the compact registry it is consuming;
- omission of a material finding/disposition/required section must be detected or force reconstruction from authoritative sources;
- record the measured before/after cost for at least one cumulative PA consumer.

### 4. Structured proof/state and compact handoff

- introduce structured representations for naturally tabular evidence such as proof obligations, negative controls, residual rows, accepted SHAs and durable state;
- keep causal explanation in prose where reasoning itself is evidence;
- compact PR handoff bodies to state/scope/freeze/findings/pointers instead of duplicating complete evidence;
- ensure a fresh Reviewer can still locate the complete candidate evidence, exact diff and authoritative sources from the compact handoff alone plus live GitHub;
- no generated/derived registry may define its own proof universe without an independent discovery/check path.

### 4a. Transactional Worker review-ready closure

Treat Worker→Reviewer closure as a first-class state transition, not as prose appended after implementation. `WORKER_PRE_REVIEW: CLEAN` is permission to begin closure; it is **not** completion and must never by itself justify telling the user or Reviewer that the WP is ready.

A Worker may claim `REVIEW_READY` / “pass to Reviewer” only after one exact candidate satisfies the complete terminal invariant:

- the final repository/evidence byte mutation happened before the final complete Worker pre-review;
- that pre-review is `WORKER_PRE_REVIEW: CLEAN` for the exact resulting candidate bytes;
- one exact 40-character HEAD is recorded consistently as `Candidate HEAD SHA` and `Frozen candidate SHA`;
- `Worker state: FROZEN_FOR_REVIEW`, `Branch frozen: YES`, `Worker verdict: IN_REVIEW` and `Reviewer verdict: PENDING` are present in the canonical handoff;
- the PR is Ready rather than Draft and remains mergeable;
- the Ready-state Worker handoff lint is GREEN on that same exact SHA;
- the Ready-state freeze exact-SHA validation is GREEN on that same exact SHA;
- a final live-state read confirms PR HEAD still equals the frozen SHA after those gates complete.

The Worker role/repair instructions must end with this small deterministic closure epilogue. They must not report “done”, “ready for Reviewer” or equivalent while any terminal predicate is false, pending or stale.

If a Ready-state gate discovers a defect that requires any repository/evidence byte mutation, the prior clean/freeze is invalid: return to Draft + ACTIVE, repair, rerun the **complete** Worker pre-review after the last mutation, freeze the new exact SHA and repeat Ready-state gates. A metadata-only correction that leaves repository bytes and exact HEAD unchanged does not invent a new candidate, but the affected Ready-state gates must still be rerun and observed GREEN before handoff completion.

Reproduce representative historical premature/incomplete handoffs from live GitHub, including at minimum:

- the CTX-02 class where implementation and pre-Ready validations were GREEN but Ready-state handoff lint rejected a missing canonical predecessor-check marker; and
- at least one independently reconstructed CTX-01 or other pre-CTX-03 handoff case where substantive work was complete but canonical Worker→Reviewer closure state was incomplete/stale.

The reproduction must prove the terminal invariant prevents handoff until the real missing condition is restored. Do not encode chat-specific incident text as authority; derive the fixture from durable GitHub/repository evidence.

### 5. ROADMAP/history separation and DocSync

- separate current ROADMAP/gate planning from long accepted closure narrative while preserving exact history/provenance under `Docs/history/` or equivalent;
- make DocSync update one compact derived accepted-state/index surface plus only affected authoritative docs rather than manually restating the same fact broadly;
- add stale/consistency checks where practical;
- prove on a representative accepted transition that compact state is updated once and that normal bootstrap surfaces do not retain contradictory next-WP/current-state claims;
- preserve every accepted milestone/gate pointer needed to reconstruct historical closure.

### 6. Process-envelope: bound the growth slope

Add a small versioned **CI-only** process-budget surface and deterministic checker that bounds mandatory context growth by role/profile.

The process-envelope must:

- derive the required read set from the accepted role/bootstrap profile rather than a manually duplicated file list;
- tokenize that effective set reproducibly and fail closed when a profile exceeds its reviewed ceiling;
- keep the budget/config small and out of normal role bootstrap so the remedy does not add recurring context cost;
- establish initial ceilings from measured post-CTX required context plus explicitly justified bounded headroom, with the formula/rationale persisted;
- make any ceiling increase an explicit reviewed diff with justification rather than a silent side effect of adding prose elsewhere;
- be implemented as a sibling validation concern, not by broadening the narrow semantic scope of the existing Worker handoff lint;
- include self-tests/negative conformance fixtures showing that a synthetic required-context increase crossing a ceiling fails while ordinary unrelated repository growth does not;
- expose enough output to identify which required source/profile caused the overage without requiring a model to inspect all source prose.

### 7. Historical FAIL classification and bounded pre-freeze quality gate

Re-derive the historical independent FAIL corpus from live GitHub; do not assume a previously reported count or repair rate.

Classify each relevant FAIL as:

- **mechanical/deterministic** — a repository-owned check can detect the defect without semantic Reviewer judgment;
- **semantic/reasoning** — independent Reviewer judgment is the intended detector;
- **mixed** — a deterministic precondition is checkable but semantic judgment remains necessary.

Classify incomplete Worker→Reviewer closure incidents alongside the FAIL corpus even when no independent Reviewer verdict was emitted, because a deterministic handoff defect that correctly prevents review is still a process defect worth moving left.

Only deterministic portions may be moved into a pre-freeze/closure gate. Do not build a gate merely to lower the observed Reviewer FAIL rate.

For each adopted mechanical check, include a negative conformance/reproduction fixture that removes or corrupts the **real required condition**, not just the declaration consumed by the gate. The fixture must turn red for the causal defect and green after the actual condition is restored.

Persist the classification and the explicit `ADOPT / DEFER / REJECT` decision for each candidate gate family.

### 8. Reviewer quality-preservation replay

Create a bounded quality-preservation control using representative historical blockers/reproduction cases.

For every selected case, when starting from the post-CTX compact context, the process must do one of the following before the affected claim can be treated as closed:

1. expose the material authoritative source in the required starting set; or
2. deterministically trigger an escalation path that leads to the authoritative source; or
3. fail closed and require reconstruction from authoritative sources.

A compact route that can reach an apparently complete conclusion while the historical causal blocker/source is omitted is a CTX-03 failure.

This control tests routing/discoverability, not whether a particular model happens to notice every semantic defect. Independent semantic review remains mandatory.

### 9. Future consumer-repo handoff

Leave the process-envelope/checker reusable by a future clean consumer repository so H2/H3 can adopt a context budget before a new corpus accumulates. Do not create H2 product workpacks, move PA/CITY ownership, or decide the H3 repository boundary inside CTX-03; record those as explicit future planning inputs only.

## Forbidden

- no deletion of accepted evidence for token savings;
- no conversion of causal reasoning into opaque tables when prose is needed to understand why proof is valid;
- no single generated file may become semantic authority merely because it is easy to parse;
- no compact registry may be the sole source defining the universe whose completeness it claims to prove;
- no retroactive bulk rewrite of every H0 artefact unless required by a live consumer or consistency check;
- no budget whose normal operation requires every Worker/Reviewer to read the budget file;
- no success claim based solely on average tokens, lower Reviewer FAIL rate, fewer Reviewer source reads or a single un-escalated happy path;
- no deterministic gate for a semantic judgment that cannot be checked causally;
- no Worker completion claim based solely on `WORKER_PRE_REVIEW: CLEAN`, pre-Ready CI, local validation or intended metadata; live Ready-state terminal predicates must actually be GREEN;
- no weakening of independent Reviewer authority, exact-source access, predecessor reopen rules or exact-SHA review;
- no product/runtime change;
- no CTX-03 implementation before CTX-02 has PASSed, merged and completed DocSync.

## Required controls

- a stale structured row/index cannot override live GitHub or exact accepted evidence;
- deleting one required proof/residual/material PA reference from a structured registry is detected or forces deeper reconstruction rather than producing a false complete view;
- at least one material-loss fixture proves that a compact representation cannot silently omit a material inherited/research fact;
- a historical-blocker quality-preservation replay proves that compact starting context still exposes, escalates to or fails closed toward the causal authoritative source;
- a capsule/structured summary that disagrees with an authoritative source loses and forces authoritative reconstruction;
- the process-envelope goes red when a required-context fixture crosses a reviewed role ceiling and remains green for unrelated repository growth outside the role read set;
- increasing a ceiling cannot happen as an unreviewed side effect of editing another protocol/source;
- a pre-freeze gate fixture removes/corrupts the real required condition and turns red, rather than merely deleting the declaration the gate parses;
- the Worker review-ready closure fails closed when `WORKER_PRE_REVIEW: CLEAN` exists but the PR is still Draft/ACTIVE, the frozen SHA is absent/stale, Ready-state handoff lint is RED/pending, freeze exact-SHA is RED/pending, or live HEAD moved;
- a repository/evidence byte mutation after CLEAN/freeze invalidates review-ready state until the complete pre-review and freeze sequence is rerun on the new exact SHA;
- the CTX-02 missing-predecessor-marker handoff reproduction turns RED before the real marker is restored and GREEN only after the complete closure sequence succeeds;
- at least one CTX-01 or other independently reconstructed historical incomplete-handoff case is likewise prevented from reaching review-ready state until its causal terminal condition is restored;
- ROADMAP-history separation preserves every accepted milestone/gate pointer needed to reconstruct past closure;
- compact PR handoff still lets a fresh Reviewer locate complete candidate evidence and full diff without private chat context;
- DocSync on a representative accepted transition updates compact state once and does not leave contradictory next-WP state in normal bootstrap surfaces;
- recorded `CONTEXT_ESCALATIONS` is reproducible against the effective claim/profile and cannot suppress a mandatory escalation by omission.

## Acceptance

On an exact accepted post-CTX-02 repository state:

1. same-snapshot pre/post measurements show a **material** reduction in irrelevant/duplicated mandatory context for the representative roles where CTX claims savings; a claimed material improvement must exceed declared measurement uncertainty;
2. the cumulative PA consumer is explicitly measured and either CTX-02 already closes its dominant accumulation cost or CTX-03 provides a measured structured-consumption improvement without loss of material findings/dispositions/required scenarios;
3. every selected historical quality-preservation case remains causally reachable from compact context through direct read, deterministic escalation or fail-closed reconstruction;
4. independent Reviewer behavior and exact authoritative-source access are unchanged; no PASS/FAIL claim may use compact material as proof authority;
5. the role/profile process-envelope fails closed on reviewed ceiling overage and turns future mandatory-context growth into an explicit reviewed decision;
6. historical FAIL/incomplete-handoff classification is re-derived and any adopted pre-freeze/closure gate is limited to deterministic causal checks with negative conformance fixtures;
7. representative DocSync/history/handoff transitions remain reconstructible and free of contradictory compact state;
8. a Worker cannot reach or report review-ready state until the exact terminal invariant is satisfied on live GitHub, including Ready state, exact frozen HEAD, final post-mutation CLEAN, GREEN handoff lint and GREEN freeze exact-SHA validation; the CTX-02 handoff defect and at least one CTX-01/other historical incomplete-handoff family are reproduced as fail-closed controls.

If token usage falls but any quality-preservation control fails, CTX-03 is **FAIL**. If review quality is preserved but a claimed token saving does not exceed measurement uncertainty, record that route as **no demonstrated material saving** rather than overstating the result.

Any remaining duplication must be classified as intentional authority, causal narrative or future residual rather than accidental repetition.

## Definition of Done

The three-WP CTX programme has independently reviewed role routing, safe predecessor compression, structured evidence/state/history handling, auditable escalations, cumulative-PA handling, a bounded context-growth envelope, transactional Worker review-ready closure and measured quality-preserving closure.

No additional CTX gate is required unless CTX-03 evidence itself reveals an unresolved process-quality defect. A future clean consumer repository can reuse the process-envelope and review-ready closure primitives without inheriting Juego2 historical corpus, but CTX-03 does not itself decide the H2/H3 repository split.