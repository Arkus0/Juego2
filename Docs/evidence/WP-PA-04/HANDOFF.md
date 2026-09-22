# WP-PA-04 — Worker handoff surface

Workpack: `WP-PA-04 — Adopt/revalidate Knowledge & Belief findings`  
Execution: `REMOTE_HARVEST`  
Canonical PR: `#129`  
Branch: `pa/wp-pa-04-knowledge-harvest`  
Recorded baseline: `2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-22  
Fail cycle: `0`

This file is repository-resident review navigation only. The exact final `Candidate HEAD SHA`, final external `WORKER_PRE_REVIEW: CLEAN` record, freeze state and Automation V2 terminal markers are authoritative on the live PR handoff surface after the Worker stops writers. This file deliberately does not attempt to contain its own final commit SHA.

## Candidate claim

PA-04 harvests/revalidates the independently reviewed donor Knowledge/Belief findings into a compact Juego2 product-semantic epistemic contract while stripping donor runtime/M9/M10/schema authority.

Central authority boundary:

```text
canonical world truth != actor-accessible belief
missing actor belief/access -> UNKNOWN
canonical truth mutation -> no automatic actor learning
privileged truth comparison -> debug/test only
```

Two actors in the same canonical world can therefore make different explainable actor-owned decisions because their legitimate epistemic state differs. Hidden/debug truth must not make an actor smarter without a valid acquisition path.

## Candidate surface

Only:

- `Docs/evidence/WP-PA-04/WORKER_PLAN.md`;
- `Docs/evidence/WP-PA-04/HANDOFF.md`;
- `Docs/research/living-world/results/PA-04.md`.

No runtime/code/Unity/H0/H1/CITY/frozen-PA-plan/amendment file is intentionally changed.

## Exact predecessor closure

`WP-PA-03` is accepted and DocSync-complete:

- candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927`;
- independent PASS `#5270421825`;
- PR #100 merge `2f3862b601b0521a6a3d5a57afe54f182d037e97`;
- DocSync PR #101 candidate `1b3005ac9118aae3043efa204c3d1d8127681f7d`;
- DocSync merge `e388f6e3c9d42e89418bd8877ed36fcbcf6d2aaa`;
- `Docs/evidence/WP-PA-03/DOCSYNC.md` ends `DOCSYNC_COMPLETE` and names PA-04 next.

The Worker opened the authoritative PA-03 result and DocSync directly rather than using a capsule as semantic authority because PA-04 materially consumes PA-03's rule that private third-party relationship truth is not ambient actor knowledge.

Full inherited/new/consumed/reopen classification is in `WORKER_PLAN.md#predecessor_contract_check`.

## Exact donor provenance

- donor repo: `Arkus0/Juego`;
- frozen plan: `Docs/living-city-research/PA-04_PLAN_KNOWLEDGE.md`;
- dossier: `Docs/living-city-research/PA-04_KNOWLEDGE.md`;
- exact donor candidate: `672dcfa46dc1212d43f5302b4937ddd345bf249a`;
- donor PR: `#41`;
- final independent donor PASS: review `#5230673388`.

The donor PASS is provenance only. This Juego2 candidate still requires fresh independent review.

## Juego2 reconciliation

The candidate keeps donor findings only where they fit current Juego2 authority:

- player/NPC information asymmetry belongs to the same playable causal city;
- sustained player action may change information distribution and source availability rather than triggering automatic truth synchronization;
- no quest/player-only `knows_X` universe substitutes for shared epistemic state;
- PA-03 canonical relationship truth remains separate from an actor's belief about a relationship;
- PA-05 retains communication motive, retransmission, distortion and corroboration;
- PA-06 retains autobiographical memory, salience, forgetting and compaction;
- PA-09 later owns generic player-action semantics;
- PA-12 later owns governance rules/resources/opportunities, not mind control.

## Required positive/counterfactual fixtures

### CF-01 — same world, different belief

All non-epistemic inputs are fixed. Antonio legitimately holds `meeting_location=town_hall`; Manolo is `UNKNOWN`.

Required material outcomes:

```text
Antonio -> TRAVEL_TO_TOWN_HALL
Manolo  -> SEEK_MEETING_INFORMATION
```

Same material outcome, correlated input differences, canonical fallback or tie-break dependence is a FAIL.

### CF-02 — stale belief survives hidden truth change

Carmen's actor-safe belief remains `shop_open=true@T1` in both runs while only canonical truth changes from true to false without an acquisition/revision event.

The belief-gated decision input/result remains the same. Privileged tooling may change `MATCH -> MISMATCH`; actor-safe state may not.

### CF-03 — deceptive claim creates receiver epistemic state

Paco begins `UNKNOWN`. A fixture-supplied communication from Carmen asserts false `alibi_valid=true`, and the fixture declares Paco accepts the claim.

Required result: Paco holds `alibi_valid=true` with immediate `COMMUNICATION` provenance from Carmen. If only rendered dialogue changes, PA-04 fails.

## Required negative controls

- `NC-01`: missing belief/access cannot fall back to canonical `WorldFact`.
- `NC-02`: changing only inaccessible privileged/debug truth metadata cannot alter actor-safe input/decision.
- `NC-03`: a public source is not automatically known before access/distribution.
- `NC-04`: private canonical PA-03 relationship truth remains `UNKNOWN` to an unrelated actor absent acquisition.
- `NC-05`: knowing one facet cannot reveal the hidden complete object.
- `NC-06`: a future knowledge-backed source must preserve accepted bounded discovery rather than hide global actor/fact enumeration.

## Main drift observed during Worker execution

The Worker branch started from `main@2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`.

During execution, main advanced to `0523b34a021d7c0a263b661ad5380a372fa2d61b` through DW-00 DocSync. The complete `2da4b6fd... -> 0523b34a...` changed-file set is limited to:

- `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json`;
- `Docs/evidence/WP-DW-00/DOCSYNC.md`;
- `Docs/workpacks/DW/README.md`;
- `Docs/workpacks/DW/WP-DW-00.md`;
- `Docs/workpacks/README.md`.

No PA-04 workpack/result/evidence, PA-03 accepted result/evidence, PA roadmap/harvest/amendment or donor input changed in that drift. Final pre-review must reconstruct live main again and reclassify any later drift before freeze.

## CTX-03 closure used by this handoff

CTX-03 is accepted and DocSync-complete on current main. Therefore the final Worker cleanliness record is external to repository bytes: after all candidate/evidence bytes are committed and writers stop, the Worker challenges the exact HEAD and, only if clean, publishes the exact-SHA `WORKER_PRE_REVIEW: CLEAN` record on PR #129. A later repository-byte mutation invalidates that cleanliness.

## Scope guards

The candidate does not claim or authorize:

- runtime/Unity implementation;
- final belief database/schema/index/API;
- perception/hearing/visibility implementation;
- dialogue UI/authoring implementation;
- numeric probability/confidence tuning;
- generic inference engine or recursive Theory of Mind;
- LLM/general-reasoner epistemic authority;
- PA-05 rumour propagation/motive/corroboration;
- PA-06 autobiographical memory/forgetting/compaction;
- persistence/save/load implementation or scale/performance proof;
- PA-05+ completion.

## Independent Reviewer focus

Try especially to show that:

1. `UNKNOWN` can still read canonical truth;
2. actor-facing truth/staleness labels leak privileged state;
3. CF-01 does not actually require a material A/B outcome or changes another input;
4. hidden truth mutation repairs or changes stale belief without acquisition;
5. public availability has become ambient knowledge;
6. deception remains text-only;
7. PA-05 or PA-06 ownership has leaked into PA-04;
8. private PA-03 relationship truth becomes ambient;
9. partial knowledge exposes a complete hidden object;
10. normal lookup can hide a global scan;
11. certainty is mandatory despite no behavioral/revision consumer;
12. donor runtime/M9/M10 architecture has become Juego2 authority;
13. a research-level finding is presented as runtime/performance/persistence proof;
14. player-caused information uses a privileged quest-only knowledge copy.

A fresh independent Reviewer must bind its verdict to the exact frozen SHA published in PR #129 after terminal handoff closure.
