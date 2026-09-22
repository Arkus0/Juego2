# WP-PA-05 — Worker plan

Workpack: `Docs/workpacks/PA/WP-PA-05.md`  
Execution: `REMOTE_HARVEST`  
Baseline: `main@58e417f357caac387f84333766f61c2fb4d9f461`  
Candidate branch: `pa/wp-pa-05-rumours-harvest`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-22

## Contract interpretation

PA-05 is a compact Juego2 adoption/revalidation of the corrected independently reviewed donor study on rumours and information flow. It must not repeat the donor's broad prior-art research, import donor M9/M10/runtime architecture, or treat donor PASS as automatic Juego2 authority.

The workpack owns only the product-semantic transport boundary between an actor deciding to communicate and PA-04 receiver-side belief acquisition/revision:

```text
sender-owned action / motive
        ↓
real communication opportunity / channel
        ↓
explicit sender -> receiver asserted-claim transfer
        ↓
receiver-owned interpretation using actor-accessible inputs only
        ↓
PA-04 acquisition / revision
        ↓
receiver may later choose to retell — or not
```

The crucial donor review correction is binding attack surface, not optional history: privileged causal/debug lineage may support replay, technical dedup/loop guards and bounded propagation, but hidden engine lineage must not alter receiver belief, confidence, corroboration, dialogue or action unless the receiver legitimately acquired the relevant provenance.

## PREDECESSOR_CONTRACT_CHECK

Completed before any PA-05 result/fixture write. This Worker-plan commit is the first PA-05 branch write.

### Accepted direct predecessor / exact evidence

Direct dependency: `WP-PA-04 — Knowledge, Belief, Ignorance & Deception`.

- accepted candidate: `5d38ea38b983cd5227f57afa1d880d24746f9249`;
- independent PASS review: `#5280879115`;
- research PR: `#129`;
- research merge: `d6041b719292f24c4481dea28727e2cfd5f7ed5b`;
- DocSync PR: `#131`;
- DocSync candidate: `b9a884c9c8f0dfc6afae59ef33fa58c3a8997904`;
- DocSync merge / PA-05 baseline: `58e417f357caac387f84333766f61c2fb4d9f461`;
- canonical accepted finding: `Docs/research/living-world/results/PA-04.md`;
- completion evidence: `Docs/evidence/WP-PA-04/DOCSYNC.md` with `DOCSYNC_COMPLETE` and PA-05 named as next.

No compact navigation projection is used as semantic authority for this material seam. PA-05 directly consumes the accepted PA-04 result and DocSync because the transport contract terminates at PA-04 receiver-owned acquisition/revision and must preserve PA-04's actor-safe/privileged boundary.

### Inherited guarantees relevant to PA-05

PA-05 consumes as binding:

1. **Canonical truth is not actor belief.** A transfer carries an assertion; repetition never writes or repairs canonical truth.
2. **Missing actor knowledge fails closed.** A receiver cannot infer hidden facts or provenance merely because the engine knows them.
3. **Acquisition needs a legitimate cause.** Communication is one PA-04 acquisition category; public/perception acquisition remain separate paths.
4. **Receiver ownership.** Sender/transport code may deliver an assertion, but receiver-side acquisition/revision owns the epistemic result.
5. **False/stale/partial beliefs are representable.** PA-05 may transport a false or partial assertion without changing canonical truth.
6. **Actor-safe state is segregated from privileged diagnostics.** Hidden truth/staleness/debug fields cannot enter actor-facing decisions. PA-05 extends this same rule to hidden transfer lineage.
7. **Immediate provenance is bounded and causal.** PA-05 may supply communication provenance without turning transfer history into PA-06 autobiographical memory.
8. **Private relationship truth remains non-ambient.** PA-03 relationship/context may be an authorized receiver/sender input, but hidden third-party relationship state cannot be smuggled through propagation.
9. **Actor-owned agency remains authoritative.** PA-02 owns why/when an actor chooses to communicate or retell; receiving a belief is not itself permission for automatic forwarding.
10. **Bounded discovery remains binding.** A rumour path cannot hide graph-wide population enumeration behind a small delivered result.
11. **Playable causal city.** Player-originated and NPC-originated information must enter compatible owners/paths; no privileged quest-rumour universe is needed.

### Guarantees newly owned by PA-05

PA-05 newly owns only the information-flow semantics required by its workpack:

- explicit actor-to-actor asserted-claim transfer after a valid sender decision/opportunity;
- no automatic graph broadcast or autonomous `RumourManager` propagation policy;
- receiving does not automatically generate a relay;
- communication opportunity/channel is causal, including structured ABSTRACT opportunities without requiring loaded GameObjects;
- selective and delayed delivery: some actors can receive while others remain `UNKNOWN`;
- false/partial assertions can propagate while canonical truth remains unchanged;
- strict dual authority between privileged causal/debug lineage and actor-accessible/reported provenance;
- hidden-lineage epistemic invariance: with all actor-visible inputs fixed, changing only hidden lineage must not change receiver epistemic output;
- positive counterpart: common-source reasoning is allowed only after common-source information is legitimately communicated/acquired;
- bounded relay guardrails and explicit technical termination reasons without pretending budgets are actor motive or epistemic evidence;
- player-originated information flow through the same communication/public/perception owners rather than player-only flags;
- future H4/H7 consumer notes with runtime/schema/tuning/persistence/scale proof explicitly deferred.

### Inherited guarantees intentionally consumed, not re-proved

This WP does not re-prove PA-01 routine semantics, PA-02 general action selection, PA-03 relationship dimensions, PA-04 truth/belief separation, generic receiver response ownership, or PA-04's perception/public-source acquisition semantics.

PA-05 exercises the new composition seam it owns: selective communication may alter receiver epistemic state only through an explicit transport plus actor-accessible receiver inputs, while privileged engine lineage remains causally useful to tooling/guards but epistemically inert by default.

### Concrete reopen condition

Reopen a predecessor guarantee only on concrete contradictory evidence that it cannot hold on the effective PA-05 path—for example, if receiver-owned belief revision can only function by reading hidden causal lineage, if a viable transfer path necessarily requires graph-wide population fanout, or if a false assertion cannot be transported without mutating canonical truth.

Future schema uncertainty, a possible implementation convenience, or desire for duplicate proof is not a predecessor reopen condition.

## Donor provenance audited

- donor repo: `Arkus0/Juego`;
- frozen donor plan: `Docs/living-city-research/PA-05_PLAN_RUMOURS.md`;
- donor dossier: `Docs/living-city-research/PA-05_RUMOURS.md`;
- prior failed donor candidate: `3a81811df3755e6ed7427911e7edba89efd279b2`;
- blocking independent FAIL: review `#5230788897`;
- corrected donor candidate: `1c8ec93b52d828744ca5b6504ca211b446be73c7`;
- donor PR: `#43`;
- final independent donor PASS: review `#5230865571` on the corrected candidate;
- donor merge: `cf22cbd8f5ec4e2a9f82818b9b2c93a0ab27a2ea`.

The FAIL/PASS lineage is materially consumed. The failed candidate leaked engine-known common-origin lineage into receiver corroboration. The corrected candidate split privileged causal/debug lineage from actor-accessible/reported provenance and added the direct counterfactual: if all actor-visible inputs are equal, changing only hidden lineage cannot change the actor's epistemic result.

The donor PASS is provenance only. Juego2 requires its own independent review.

## Current Juego2 reconciliation

PA-05 must consume current Juego2 programme amendments rather than restore donor-era architecture:

- **Playable causal city:** the player may originate, repeat, conceal, expose or investigate information using the same causal owners as NPCs; player origin is not broadcast authority.
- **Intentional transformation:** sustained player action may materially reshape information availability/distribution, but no anti-chaos or provenance shortcut may erase or globally synchronize beliefs.
- **PA-09 player causality boundary:** PA-05 can prove compatible player-originated information-flow cases, but generic player-action semantics remain PA-09-owned.
- **No omniscience:** engine lineage, hidden truth, deception intent and hidden common-source knowledge remain privileged unless acquired through an actor-accessible path.
- **Positive/neutral causality:** information sharing need not be lies/conflict; warnings, help, invitations and ordinary reports are legitimate transfer cases.
- **Salience/retention boundary:** transfer history is not automatically durable autobiographical memory; PA-06 owns memory selection/forgetting/compaction.

## Required causal controls

The candidate must make the following falsifiable rather than merely permissible:

1. `CF-01 SELECTIVE_DELAYED_CHAIN`: fixed sender intents + valid opportunities cause Antonio -> Manolo -> Carmen transfers at declared times while Paco, who has no valid opportunity, remains `UNKNOWN`.
2. `NC-01 NO_GLOBAL_TRUTH_SYNC`: canonical truth changes with no perception/public/communication acquisition; no ActorBelief and no KnowledgeTransfer changes.
3. `CF-02 FALSE_ASSERTION`: sender asserts a false value through a valid transfer; receiver may hold that false PA-04 belief while canonical truth stays unchanged.
4. `NC-02 HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE`: paired deliveries are actor-visible-identical; only privileged engine lineage differs. Receiver belief/confidence/corroboration/dialogue/action output must remain identical. Same material result is required, not optional.
5. `CF-03 REPORTED_SOURCE_POSITIVE`: change only actor-accessible reported provenance by explicitly communicating a common source; a declared receiver rule may now legitimately produce a different corroboration result.
6. `CF-04 BOUNDED_LOOP`: a configured technical guard stops a candidate relay cycle for an explicit engine reason; no automatic retransmission occurs and the hidden stop reason cannot become actor-facing epistemic evidence.
7. `CF-05 PLAYER_ORIGINATED_FLOW`: a player assertion enters the same transfer/acquisition path as an NPC assertion; witnesses/recipients and non-recipients diverge causally without quest-only `heardRumour_X` state.

## Execution

1. Persist this exact predecessor check before any result/fixture write.
2. Persist the standalone machine-consumable predecessor check required by the post-CTX-03 handoff contract.
3. Reconstruct donor findings and exact FAIL→repair→PASS lineage without copying donor runtime/API/schema authority.
4. Reconcile with accepted PA-01..04 plus current Juego2 roadmap/harvest/cross-cutting amendments.
5. Publish compact canonical `Docs/research/living-world/results/PA-05.md`.
6. Persist a dedicated transfer/ownership + causal fixture surface covering CF/NC controls and player-originated flow.
7. Classify mechanisms as `ADOPT / ADAPT / LATER / REJECT` while staying representation-neutral.
8. Preserve PA-02 sender motive, PA-04 receiver belief, PA-06 memory and PA-09 generic player-action ownership.
9. Keep runtime schema/event transport, dialogue presentation, numeric propagation constants, save compaction and large-population performance as deferred proof.
10. Perform strict Worker pre-review against the complete baseline→candidate diff while Draft; repair any in-claim defect before freeze.
11. Stop writers, bind exact HEAD as frozen candidate, publish external exact-SHA `WORKER_PRE_REVIEW: CLEAN`, and hand to a fresh independent Reviewer.

## Ownership guards

No runtime/Unity/code changes; no H0/H1/CITY/DW changes; no edits to frozen PA plans/amendments; no donor auto-acceptance; no final event/schema/API; no confidence/corroboration formula; no generic distortion engine; no global diffusion manager; no PA-06 autobiographical memory/compaction; no PA-09 generic player-action model; no PA-10 event-chain ownership; no PA-11 investigation UI; no PA-05+ completion.

## Candidate surface

Only:

- `Docs/research/living-world/results/PA-05.md`;
- `Docs/evidence/WP-PA-05/*`.
