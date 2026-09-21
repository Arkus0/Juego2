# WP-PA-02 — Independent Reviewer handoff

Workpack: `WP-PA-02 — Adopt/revalidate NPC Agency findings`  
Execution: `REMOTE_HARVEST`  
Worker: ChatGPT GPT-5.6 Sol  
Baseline: `main@c6e97600fa68afbe1de7fe46121880ffe8be917b`  
Candidate branch: `pa/wp-pa-02-npc-agency-harvest`  
Repair cycle: `fail_cycle: 1`  
Transfer SHA: `0ab469b640535d92c73c617e0d944c0b1f45033f`  
Superseded independent FAIL: review `#5269143202` on `0ab469b640535d92c73c617e0d944c0b1f45033f`  
Freeze anchor: the exact `Frozen candidate SHA` recorded in the PR body after all repair evidence is committed. This file intentionally does not self-hash the commit that contains itself.

## Repair boundary

The previous Reviewer found the PA-02 research content strong against the workpack and failed only the mandatory v1.7 predecessor-contract/handoff obligation. This repair therefore changes process evidence/handoff only. `Docs/research/living-world/results/PA-02.md` remains byte-for-byte unchanged from the failed candidate unless the fresh Worker pre-review discovers a new semantic defect.

The repair-cycle `PREDECESSOR_CONTRACT_CHECK` is persisted in:

- `Docs/evidence/WP-PA-02/WORKER_PLAN.md#predecessor_contract_check`

It was completed before the first repair-cycle branch write and explicitly records that the initial failed cycle had omitted it.

## Accepted predecessor — exact evidence

Direct dependency: `WP-PA-01`.

- accepted candidate: `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715`;
- independent PASS review: `#5268013445`;
- PR: `#89`;
- merge: `3688b7b9a27355b0fda160c20e57a385e40c6814`;
- DocSync PR: `#91`;
- DocSync merge: `50bbb95d261ff1964f2e920ce0380868eb2c59fd`;
- accepted result: `Docs/research/living-world/results/PA-01.md`;
- completion evidence: `Docs/evidence/WP-PA-01/DOCSYNC.md`.

Inherited PA-01 guarantees are consumed rather than redundantly re-proved: routine remains expected semantic intent rather than agency; expected and actual state remain distinct; interruption recovery re-evaluates current context; route realization does not rewrite schedule intent; routine is a perturbable baseline compatible with durable world change; and PA-08/PA-09 retain their accepted ownership boundaries.

PA-02 newly owns actor-originated choice, bounded discovery/anti-global-scan semantics, authorized decision inputs, explainable decision traces, optional receiver-choice ownership, commitment/replanning/failure guardrails and player-independent autonomous continuation. The exact inherited/new/consumed/reopen split is in the predecessor check.

## Review target

Primary deliverable:

- `Docs/research/living-world/results/PA-02.md`

Worker evidence:

- `Docs/evidence/WP-PA-02/WORKER_PLAN.md`
- `Docs/evidence/WP-PA-02/WORKER_PRE_REVIEW.md`

Contract:

- `Docs/workpacks/PA/WP-PA-02.md`
- `Docs/workpacks/PA/README.md`

Accepted prerequisite:

- `Docs/workpacks/PA/WP-PA-01.md`
- `Docs/research/living-world/results/PA-01.md`
- independent PASS review `#5268013445`
- `Docs/evidence/WP-PA-01/DOCSYNC.md`

## Exact donor provenance

- donor repo: `Arkus0/Juego`;
- donor dossier: `Docs/living-city-research/PA-02_NPC_AGENCY.md`;
- exact donor candidate: `dfe8a2b10831774f846274143c58dc41227d6231`;
- donor PR: `#36`;
- prior failed donor candidate: `be0fad74b337ddae247891eac43ad7347e754fb6` / review `#5225337797`;
- donor final independent PASS review: `#5225572191`.

The donor PASS is provenance only. This Juego2 candidate needs its own independent review.

## Worker claim

The candidate adopts a compact agency requirement set:

```text
actor-originated pressure/problem/opportunity
+ bounded discovery before scoring
+ authorized actor inputs
+ explainable decision
+ commitment / bounded failure and replanning
+ receiver-owned response when response is genuinely optional
+ structured shared-world outcome
+ player absence tolerated
```

It preserves the donor's strongest anti-fake-agency lessons—especially anti-global-scan discovery and receiver ownership—without importing donor M9/M10 routing, `BehaviourResolver`, API/schema decisions, old constitutional authority or a universal Utility/GOAP/rules/planner algorithm.

Juego2-specific reconciliation makes NPC autonomy one side of a **shared playable causal world**. Player-created circumstances may legitimately change actor opportunities/inputs, but do not directly command actor decisions; equivalent player/NPC world actions should not require incompatible consequence universes.

The donor `>=3` linked material changes proof is retained as a useful stronger stress-fixture shape, not declared a universal Juego2 runtime law.

## Required proof surfaces

### Actor→actor positive case

Result §9 P1 uses ordinary coordination between Antonio and Manolo while the player is absent. Antonio's problem does not preselect `ASK`; at least two valid initiator alternatives exist. If Manolo has a genuine response choice, his response is separately owned.

### Required no-player-trigger control

NC-01 removes player presence and player/quest trigger dependence while holding actor-accessible state constant. If the autonomous decision disappears because the camera/player/quest was the real initiator, PA-02 fails.

### Required anti-global-scan control

NC-02 adds >=10,000 irrelevant distant PersistentActor records outside the authorized target source. Relevant discovery/comparison and semantic result must remain invariant/bounded. A path that scans all actors and filters afterward fails even when only a few candidates are finally scored.

## Reviewer attack surface

Please try to falsify:

1. **Protocol repair:** does the persisted predecessor check bind exact accepted PA-01 evidence and distinguish inherited/new/consumed/reopen guarantees without pretending the initial cycle had done so?
2. **Fake actor origin:** does any schedule, quest, authored event or fixture already select the claimed autonomous action?
3. **Fake boundedness:** can a target provider satisfy small scored-candidate counts while first enumerating the whole population?
4. **Fake receiver independence:** can initiator/action execution still choose the receiver response while appearing to expose a second object/trace?
5. **Architecture smuggling:** did the result accidentally freeze Utility AI, GOAP, rules, planner, target-index API, GameFlow priority order, donor actor profiles or `BehaviourResolver`?
6. **PA-01 ownership theft:** does routine itself become the chooser instead of context for agency?
7. **PA-03/04 theft:** are relationship sign or knowledge acquisition semantics silently fixed?
8. **NPC-only museum:** can the result compose with player-created causes/shared outcomes, or does it build a rich semantic world available only to NPCs?
9. **Omniscient player reaction:** may an NPC consume hidden player/quest/world truth it has no authorized path to know?
10. **Positive-causality gap:** is agency accidentally synonymous with confrontation/drama rather than ordinary cooperation/help/coordination too?
11. **Donor constitution leakage:** is `>=3` material changes treated only as a stress fixture, or has an obsolete donor constitutional rule silently become Juego2 authority?
12. **Ambient/persistent conflation:** can cheap ambient presence satisfy the persistent agency proof merely by flipping a profile enum?
13. **False determinism:** does semantic determinism avoid requiring opaque global IDs to match?
14. **Harvest scope:** is this materially a compact adoption/revalidation rather than a repeat of broad research?

## PASS continuation

If and only if the exact frozen candidate passes:

```text
Next PA workpack: WP-PA-03 — Adopt/revalidate Social Graph findings
Execution class: REMOTE_HARVEST
Prerequisites: satisfied
```

On FAIL, remain on `WP-PA-02` and name the causal blocker.

## Worker state

After the fresh `WORKER_PRE_REVIEW: CLEAN` is committed, the PR records the exact final HEAD and is marked Ready, state is `FROZEN_FOR_REVIEW`. No further Worker writes unless independent review returns FAIL and opens another repair cycle.
