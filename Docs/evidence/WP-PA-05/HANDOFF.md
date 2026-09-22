# WP-PA-05 — Worker handoff surface

Workpack: `WP-PA-05 — Adopt/revalidate Rumours & Information Flow findings`  
Execution: `REMOTE_HARVEST`  
Canonical PR: `#132`  
Branch: `pa/wp-pa-05-rumours-harvest`  
Original baseline: `58e417f357caac387f84333766f61c2fb4d9f461`  
Final reconciled baseline: `cd8440938f98dcf51c0a56204bb14e8634a31926`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-22  
Fail cycle: `0`

This file is repository-resident review navigation only. The exact final `Candidate HEAD SHA`, external `WORKER_PRE_REVIEW: CLEAN` record, freeze state and terminal automation markers are authoritative on the live PR surface after writers stop. This file deliberately does not attempt to contain its own final commit SHA.

## Candidate claim

PA-05 harvests/revalidates the corrected donor Rumours & Information Flow finding into a compact Juego2 product-semantic transport contract while stripping donor runtime/M9/M10/schema authority.

Central flow:

```text
sender-owned action/motive
 -> valid communication opportunity/channel
 -> explicit sender -> receiver asserted-claim transfer
 -> receiver-owned interpretation from actor-accessible inputs
 -> PA-04 COMMUNICATION acquisition/revision
 -> receiver may later choose a NEW retell action — or not
```

Central authority split:

```text
privileged engine lineage != actor-accessible provenance
```

Hidden root/parent/hop/lineage may support tooling, budgets and technical guards, but cannot change receiver belief/confidence/corroboration/dialogue/action unless the relevant provenance was legitimately acquired.

## Candidate surface

Only:

- `Docs/evidence/WP-PA-05/WORKER_PLAN.md`;
- `Docs/evidence/WP-PA-05/PREDECESSOR_CONTRACT_CHECK.md`;
- `Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md`;
- `Docs/evidence/WP-PA-05/HANDOFF.md`;
- `Docs/research/living-world/results/PA-05.md`.

No runtime/code/Unity/H0/H1/CITY/DW/frozen-PA-plan/amendment file is intentionally changed by PA-05. The branch also contains the merge reconciliation with accepted `main@cd8440938f98dcf51c0a56204bb14e8634a31926`; those predecessor repair bytes are already in main and therefore are not PA-05 diff scope.

## Exact predecessor closure

`WP-PA-04` is accepted and DocSync-complete after the corrective capsule repair:

- research candidate `5d38ea38b983cd5227f57afa1d880d24746f9249`;
- independent research PASS `#5280879115`;
- research PR #129 merge `d6041b719292f24c4481dea28727e2cfd5f7ed5b`;
- original DocSync PR #131 candidate `b9a884c9c8f0dfc6afae59ef33fa58c3a8997904`;
- original DocSync merge `58e417f357caac387f84333766f61c2fb4d9f461`;
- PA-05 exact-HEAD validation then exposed the missing accepted PA capsule-chain coverage for COMPLETE `WP-PA-04`;
- corrective PR #133 candidate `ef665c48d43e93dc294fdeb9f9b2443197872480`;
- corrective independent FAIL `#5281292455` found only handoff/lifecycle metadata defects, not a PA-04 semantic defect;
- corrective independent PASS `#5281331377` bound to the same exact repository candidate after metadata repair;
- corrective merge / final PA-05 baseline `cd8440938f98dcf51c0a56204bb14e8634a31926`;
- `Docs/engineering/context-capsules/WP-PA-04.json` now participates in the fail-closed accepted PA chain;
- accepted-chain coverage is COMPLETE for PA-01..04 and grants no semantic authority to the capsule.

Canonical machine-consumable evidence:

`Docs/evidence/WP-PA-05/PREDECESSOR_CONTRACT_CHECK.md`

`WORKER_PLAN.md#predecessor_contract_check` remains the chronology-preserving first-write record; the standalone predecessor check records the later reopen and final closure.

## Exact donor regression lineage

- donor repo: `Arkus0/Juego`;
- plan: `Docs/living-city-research/PA-05_PLAN_RUMOURS.md`;
- dossier: `Docs/living-city-research/PA-05_RUMOURS.md`;
- failed candidate: `3a81811df3755e6ed7427911e7edba89efd279b2`;
- independent FAIL: `#5230788897`;
- corrected candidate: `1c8ec93b52d828744ca5b6504ca211b446be73c7`;
- independent PASS: `#5230865571`;
- donor PR #43 merge `cf22cbd8f5ec4e2a9f82818b9b2c93a0ab27a2ea`.

The prior FAIL is materially preserved: engine-known causal lineage cannot substitute for receiver-accessible source knowledge.

## Required causal controls

Canonical fixture surface:

`Docs/evidence/WP-PA-05/TRANSFER_AND_FIXTURES.md`

### CF-01 — SELECTIVE_DELAYED_CHAIN

Fixed PA-02-owned sender decisions plus explicit opportunities require:

```text
Antonio -> Manolo at first opportunity
Manolo  -> Carmen only after a separate later decision/opportunity
Paco    -> UNKNOWN throughout because no acquisition path exists
```

The result is mandatory, not merely allowed. Receiving T1 cannot auto-create T2, Carmen receiving T2 cannot auto-create T3, and the fixture ends with no later sender-owned retell decision plus no valid further opportunity: the chain therefore terminates for explicit causal reasons.

### NC-01 — NO_GLOBAL_TRUTH_SYNC

Change only canonical truth while providing no perception/public/communication acquisition.

Required: zero ActorBelief deltas and zero transfer records caused solely by the truth mutation.

### CF-02 — FALSE_ASSERTION

Canonical truth remains true while Antonio explicitly asserts false to Manolo through a valid transfer and the declared receiver fixture accepts/revises.

Required: `WorldFact(Z)=true` remains unchanged and `Manolo ActorBelief(Z)=false via COMMUNICATION/T1`. No hidden `isLie/isFalse` may leak into actor-safe state.

### NC-02 — HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE

Carmen receives the same two actor-visible reports from Manolo/Paco in both runs. Both reports are delivered in both runs; channel/order/timestamps/relationships/rule/seed and all actor-visible provenance remain equal.

Only privileged engine roots differ:

```text
Run A: L1 + L1
Run B: L1 + L2
```

Required: receiver belief/confidence/corroboration/actor-facing explanation/dialogue/action output is identical. The fixture explicitly disables any technical suppression that would change actor-visible evidence; a dedup guard cannot fake the result by dropping report #2.

### CF-03 — REPORTED_SOURCE_POSITIVE

Hold hidden lineage/non-provenance inputs fixed and change only communicated actor-visible provenance:

```text
Run A -> source independence unknown
Run B -> Manolo and Paco explicitly report Antonio as their source
```

Required fixture labels:

```text
Run A -> SOURCE_INDEPENDENCE_UNKNOWN
Run B -> COMMON_REPORTED_SOURCE_ANTONIO
```

Run B must derive Antonio from the communicated payload, not hidden lineage.

### CF-04 — BOUNDED_LOOP

A->B and B->C are valid separate relays. A later C->A relay candidate is rejected by a configured technical repeat/loop guard.

Required: finite transfer count + explicit engine stop reason; no automatic relay; hidden lineage does not become actor-facing epistemic evidence.

### CF-05 — PLAYER_ORIGINATED_FLOW

Player -> Carmen uses the normal communication transfer/acquisition path. After the player leaves, Carmen may independently choose a later Carmen -> Manolo relay. Paco remains UNKNOWN without an acquisition path.

No quest-only `playerRevealed_X` / `heardRumour_X` authority or town-wide broadcast is permitted. Player conceal/expose remain normal source/opportunity operations, and investigation may surface only legitimately player-accessible evidence/provenance through its future owner — never privileged engine root/parent/hop/lineage or canonical debug truth merely because tooling knows it.

## Ownership guards

- PA-02 owns sender motive/action/retell choice.
- PA-03 owns relationship semantics.
- PA-04 owns receiver belief/access/revision.
- PA-06 owns autobiographical memory/forgetting/compaction.
- PA-09 owns generic player-action semantics.
- PA-11 later owns investigation/player-facing trace presentation.
- Future H4/H7 consumers own runtime representation, persistence, APIs, numeric tuning and scaling proof.

## Independent Reviewer focus

Try especially to show that:

1. CF-01 only permits rather than requires the selective chain;
2. CF-01 can continue after Carmen without a new retell decision/opportunity;
3. Paco can learn without a valid acquisition path;
4. receive still auto-relays under another name;
5. truth mutation syncs beliefs;
6. hidden lineage changes receiver epistemic output;
7. NC-02 accidentally changes delivery count or another actor-visible variable;
8. technical dedup is being confused with corroboration;
9. reported source is inferred from engine lineage rather than communicated;
10. receiver acceptance is sender/global-manager owned;
11. ABSTRACT opportunity is unexplained teleportation;
12. capped graph diffusion is being called bounded agency;
13. false assertion mutates truth or leaks a privileged lie/truth label;
14. player origin broadcasts or uses parallel quest knowledge state;
15. player investigation reads privileged debug lineage rather than a legitimate player-facing evidence/source path;
16. transfer history steals PA-06 memory ownership;
17. dialogue state can diverge from PA-04 ActorBelief;
18. bounded delivery hides global population/social-graph enumeration;
19. runtime/persistence/performance claims exceed the research evidence.

A fresh independent Reviewer must bind any verdict to the exact frozen SHA published on PR #132 after the Worker stops writers and completes exact-HEAD pre-review.
