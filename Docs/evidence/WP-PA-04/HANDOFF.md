# WP-PA-04 — Worker handoff surface

Workpack: `WP-PA-04 — Adopt/revalidate Knowledge & Belief findings`  
Execution: `REMOTE_HARVEST`  
Canonical PR: `#129`  
Branch: `pa/wp-pa-04-knowledge-harvest`  
Recorded baseline: `2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-22  
Fail cycle: `0`

This file is repository-resident review navigation only. The exact final `Candidate HEAD SHA`, external `WORKER_PRE_REVIEW: CLEAN` record, freeze state and Automation V2 terminal markers are authoritative on the live PR handoff surface after the Worker stops writers. This file deliberately does not attempt to contain its own final commit SHA.

## Candidate claim

PA-04 harvests/revalidates the independently reviewed donor Knowledge/Belief findings into a compact Juego2 product-semantic epistemic contract while stripping donor runtime/M9/M10/schema authority.

Central boundary:

```text
canonical world truth != actor-accessible belief
missing actor belief/access -> UNKNOWN
canonical truth mutation -> no automatic actor learning
privileged truth comparison -> debug/test only
```

Two actors in the same canonical world can therefore make different explainable actor-owned decisions because their legitimate epistemic state differs. Hidden/debug truth must not make an actor smarter without a valid acquisition path.

## Candidate surface

Only:

- `Docs/evidence/WP-PA-04/PREDECESSOR_CONTRACT_CHECK.md`;
- `Docs/evidence/WP-PA-04/WORKER_PLAN.md`;
- `Docs/evidence/WP-PA-04/HANDOFF.md`;
- `Docs/evidence/WP-PA-04/PLAYABLE_CAUSAL_CITY_PROOF.md`;
- `Docs/research/living-world/results/PA-04.md`.

No runtime/code/Unity/H0/H1/CITY/frozen-PA-plan/amendment file is intentionally changed.

## Exact predecessor closure

Canonical predecessor evidence is now the standalone file required by the post-CTX-03 handoff generator:

`Docs/evidence/WP-PA-04/PREDECESSOR_CONTRACT_CHECK.md`

`WP-PA-03` is accepted and DocSync-complete:

- candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927`;
- independent PASS `#5270421825`;
- PR #100 merge `2f3862b601b0521a6a3d5a57afe54f182d037e97`;
- DocSync PR #101 candidate `1b3005ac9118aae3043efa204c3d1d8127681f7d`;
- DocSync merge `e388f6e3c9d42e89418bd8877ed36fcbcf6d2aaa`;
- `Docs/evidence/WP-PA-03/DOCSYNC.md` ends `DOCSYNC_COMPLETE` and names PA-04 next.

The Worker opened the authoritative PA-03 result and DocSync directly rather than using a capsule as semantic authority because PA-04 materially consumes PA-03's rule that private third-party relationship truth is not ambient actor knowledge.

`WORKER_PLAN.md#predecessor_contract_check` retains the original first-write planning record; the standalone file is the canonical machine-consumable handoff evidence.

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
- sustained player action may change information distribution/source availability rather than trigger automatic truth synchronization;
- no quest/player-only `knows_X` universe substitutes for shared epistemic state;
- PA-03 canonical relationship truth remains separate from an actor's belief about a relationship;
- PA-05 retains communication motive, retransmission, distortion and corroboration;
- PA-06 retains autobiographical memory, salience, forgetting and compaction;
- PA-09 later owns generic player-action semantics;
- PA-12 later owns governance rules/resources/opportunities, not mind control.

The mandatory Playable Causal City PA-04 proof is persisted separately in `PLAYABLE_CAUSAL_CITY_PROOF.md` so the witness/non-witness/later-source acceptance surface is explicit and falsable without pretending perception or PA-05 runtime already exists.

## Required positive/counterfactual fixtures

### CF-01 — same world, different belief

Antonio and Manolo use a paired fixture profile: every non-epistemic semantic input is cloned/equal. Their distinct actor IDs exist only as ownership keys; identity-specific authored rules, bonuses/penalties and identity-based tie breaking are forbidden in the fixture.

Antonio legitimately holds `meeting_location=town_hall`; Manolo is `UNKNOWN`.

Required material outcomes:

```text
Antonio -> TRAVEL_TO_TOWN_HALL
Manolo  -> SEEK_MEETING_INFORMATION
```

Same material outcome, correlated input differences, actor-ID-specific behaviour, canonical fallback or tie-break dependence is a FAIL.

### CF-02 — stale belief survives real hidden truth change

Carmen's actor-safe belief remains `shop_open=true@T1` in both runs while only canonical truth changes from true to false without an acquisition/revision event.

The belief-gated decision input/result remains the same. Privileged tooling may change `MATCH -> MISMATCH`; actor-safe state may not.

### CF-03 — deceptive claim creates receiver epistemic state

Paco begins `UNKNOWN`. A fixture-supplied communication from Carmen asserts false `alibi_valid=true`, and the fixture declares Paco accepts the claim.

Required result: Paco holds `alibi_valid=true` with immediate `COMMUNICATION` provenance from Carmen. If only rendered dialogue changes, PA-04 fails.

### CF-04 — player action witness / non-witness / later allowed source

The same canonical player action occurs. Carmen is a declared legitimate witness; Paco is not.

Required immediate state:

```text
Carmen -> HELD(true) via PERCEPTION
Paco   -> UNKNOWN
```

A later exogenous PA-05-compatible communication from Carmen to Paco is then supplied and the fixture declares Paco accepts/revises from it.

Required later state:

```text
Paco -> HELD(true) via COMMUNICATION
```

The fixture fails if player origin broadcasts the fact, the non-witness reads canonical truth, provenance lies about perception, or PA-04 must own sender motive/rumour propagation to represent the receiver belief. Full fixture and negative pair: `Docs/evidence/WP-PA-04/PLAYABLE_CAUSAL_CITY_PROOF.md`.

## Required negative controls

- `NC-01`: missing belief/access cannot fall back to canonical `WorldFact`.
- `NC-02`: with canonical truth, actor belief and every actor-visible input fixed, defect-inject only a privileged diagnostic projection/metadata value; actor-safe input and material decision must remain unchanged.
- `NC-03`: a public source is not automatically known before access/distribution.
- `NC-04`: private canonical PA-03 relationship truth remains `UNKNOWN` to an unrelated actor absent acquisition.
- `NC-05`: knowing one facet cannot reveal the hidden complete object.
- `NC-06`: a future knowledge-backed source must preserve accepted bounded discovery rather than hide global actor/fact enumeration.
- `CF-04 negative pair`: hide the player action from Carmen while keeping canonical action and non-perception inputs fixed; Carmen and Paco must both remain `UNKNOWN` until an allowed source exists.

`NC-02` is intentionally distinct from `CF-02`: `CF-02` changes real canonical truth to test stale belief; `NC-02` changes only forbidden debug/test metadata to prove privileged diagnostics causally non-authoritative.

## Main drift observed during Worker execution

The Worker branch started from `main@2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`.

During execution, main advanced to `0523b34a021d7c0a263b661ad5380a372fa2d61b` through DW-00 DocSync. The complete `2da4b6fd... -> 0523b34a...` changed-file set was limited to:

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

## Worker pre-review repairs before freeze

The Worker challenge found and repaired four in-candidate quality/process issues before final cleanliness:

1. donor PASS provenance in the first Worker-plan draft used a GitHub GraphQL node identifier instead of stable numeric review `#5230673388`; the evidence was corrected;
2. the central counterfactual/negative fixtures were tightened so actor identity cannot explain CF-01 and a pure privileged-diagnostic defect injection cannot be conflated with CF-02's real truth change;
3. the mandatory Playable Causal City PA-04 witness/non-witness/later-source proof was only described conceptually, not frozen as a falsable acceptance fixture; `PLAYABLE_CAUSAL_CITY_PROOF.md` now closes that surface;
4. the first planning record embedded `PREDECESSOR_CONTRACT_CHECK` in `WORKER_PLAN.md`, but the accepted post-CTX-03 metadata generator requires the canonical standalone `Docs/evidence/WP-PA-04/PREDECESSOR_CONTRACT_CHECK.md`; that machine-consumable evidence now exists without erasing the original first-write plan record.

These repairs are part of the final candidate bytes and must be included in the exact-HEAD pre-review rerun.

## Independent Reviewer focus

Try especially to show that:

1. `UNKNOWN` can still read canonical truth;
2. actor-facing truth/staleness labels leak privileged state;
3. CF-01 does not require a material A/B outcome, changes another semantic input, uses actor identity, or relies on tie break;
4. real hidden truth mutation repairs stale belief without acquisition;
5. pure privileged/debug metadata can alter actor-safe input/decision;
6. a player-originated visible action broadcasts to non-witnesses or the CF-04 later-source step steals PA-05 ownership;
7. public availability has become ambient knowledge;
8. deception remains text-only;
9. PA-05 or PA-06 ownership has leaked into PA-04;
10. private PA-03 relationship truth becomes ambient;
11. partial knowledge exposes a complete hidden object;
12. normal lookup can hide a global scan;
13. certainty is mandatory despite no behavioural/revision consumer;
14. donor runtime/M9/M10 architecture has become Juego2 authority;
15. a research-level finding is presented as runtime/performance/persistence proof;
16. player-caused information uses a privileged quest-only knowledge copy;
17. canonical handoff metadata points anywhere other than the standalone predecessor check required by the generator.

A fresh independent Reviewer must bind its verdict to the exact frozen SHA published in PR #129 after terminal handoff closure.
