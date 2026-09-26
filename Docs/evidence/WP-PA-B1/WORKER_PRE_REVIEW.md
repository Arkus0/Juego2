# WP-PA-B1 — Strict Worker pre-review

Status: **WORKER_PRE_REVIEW: CLEAN**  
Workpack: `WP-PA-B1`  
Units: `WP-PA-07` + `WP-PA-08` + `WP-PA-09`  
Execution class: `RESEARCH_BATCH`  
Date: 2026-09-26

## 0. Review boundary

This is a strict Worker pre-review of the complete PA-B1 research candidate against the accepted batch contract and the three binding unit contracts. It is readiness evidence only; it is **not** an independent Reviewer PASS.

The candidate remains research/product-design output. It does not claim Unity/runtime implementation, final schemas/algorithms/constants, activity fun, save/load execution, performance budgets, combat, governance implementation, or H2 Living World completion.

## 1. Contract reconstruction

Batch PASS requires all four conditions simultaneously:

1. PA-07 satisfies its own work/material acceptance and negative gate;
2. PA-08 satisfies its own shared-activity acceptance and minigame-island negative gate;
3. PA-09 satisfies its own player-causal acceptance and quest-magic/equivalence negative gate;
4. PA-07/08/09 compose through compatible owners rather than three private causality systems.

The canonical result artefacts remain separate:

- `Docs/research/living-world/results/PA-07.md`;
- `Docs/research/living-world/results/PA-08.md`;
- `Docs/research/living-world/results/PA-09.md`.

The falsifiable evidence surfaces are:

- `Docs/evidence/WP-PA-B1/BATCH_CAUSAL_FIXTURES.md`;
- `Docs/evidence/WP-PA-B1/PA09_EMBODIED_FIXTURES.md`.

## 2. PA-07 pre-review

**Result: CLEAN.**

The candidate defines the minimum work/material vocabulary around service availability/degradation, staffing/operating requirements, bounded capacity, deliberately coarse named dependencies, local request pressure and explicit substitution/recovery. It rejects global macroeconomy state, per-object commodity bookkeeping and invisible replacement unless a concrete behaviour-facing requirement later justifies them.

The decisive acceptance fixture is `CF-B1-01`. Its normative `Required result` is stronger than the shorthand wording in its later pass-oracle bullet: the perturbation must end with **at least one additional actor's later PA-02 choice differing**, so research acceptance is not satisfied merely because two choices *could* differ in principle. Any review interpretation that stops at possibility is invalid; the complete fixture requires the two-actor causal witness.

`CF-B1-02` supplies the required player-intervention seam without giving PA-07 authority over actor decisions. `NC-B1-01` is a deletion test for hidden macroeconomy state and `NC-B1-02` rejects decorative jobs/magic service.

No duplicate schedule, social, belief, memory or player-only economy owner is introduced.

## 3. PA-08 pre-review

**Result: CLEAN.**

The candidate keeps activity opportunity/session/outcome responsibilities narrow: real place/time/capacity and participation context enter the activity; local game rules remain activity-owned; only a bounded structured outcome may cross into normal downstream owners.

The frozen positive classes are represented by the batch fixtures: NPC-only participation, player joining an existing session, relationship affecting participation rather than game legality, structured outcome routing, non-witness ignorance, interruption/recovery, bounded capacity contention, positive/neutral causality, low-social activity and quiet-town compatibility.

`NC-B1-03` rejects the protagonist-only private-score-screen island. `NC-B1-04` explicitly prevents systemic integration from being misrepresented as proof that the eventual minigame is fun; playable-quality proof remains deferred.

No universal minigame engine, activity-owned relationship/belief/memory state, or requirement that every ordinary session become permanent history is introduced.

## 4. PA-09 pre-review

**Result: CLEAN with strict causal-witness interpretation recorded here.**

The candidate adopts origin-neutral semantic world actions: player and NPC variants may differ in capability/authority/context, but semantically equivalent accepted actions must enter compatible immediate consequence owners. Player causality cannot directly write downstream beliefs, relationships, memories or autonomous next actions.

`CF-B1-02` is the bounded player -> NPC -> NPC witness. For B1 PASS, its phrase `>=2 subsequent NPC decisions can occur after player departure` is read as a **required constructed research witness**, not as mere architectural possibility: the reviewed scenario must contain two downstream actor-owned decisions carried by shared state/opportunity after the player leaves. The decisions need not be runtime-executed in B1, but the causal chain must be fully specified without a bespoke continuation script.

`CF-B1-03` supplies player/NPC immediate semantic equivalence. `NC-B1-05` rejects quest-magic routing, `NC-B1-06` rejects omniscient attribution, and `NC-B1-07` rejects downstream ownership theft.

`PA09_EMBODIED_FIXTURES.md` covers the embodied amendment A11B-13..21 while preserving later ownership of controls, animation, combat, crime/governance semantics and playable-quality proof.

## 5. Cross-unit consistency

**Result: CLEAN.**

The composed causal path has one shared spine rather than three private ones:

```text
semantic action / external perturbation
 -> PA-07 material/service opportunity state
 -> PA-02 actor-owned decision
 -> PA-08 activity opportunity/session when applicable
 -> bounded PA-08 structured outcome
 -> accepted downstream owners consume only their own inputs
```

PA-09 supplies the initiator/action seam but does not become the material/activity/downstream owner. PA-08 owns only local session/game state plus a bounded outcome boundary. PA-07 owns work/service/material truth but not schedule or choice. Existing PA-01..06 ownership remains intact.

The candidate therefore does not create parallel work AI, minigame-world state or player quest causality.

## 6. Adversarial falsification ledger

The Worker challenged the candidate with the following false-PASS shapes:

| Attack | Required failure | Candidate closure |
|---|---|---|
| Worker absent but business behaves identically | FAIL PA-07 | `CF-B1-01` + `NC-B1-02` require service/opportunity consequence |
| Add GDP/global demand/per-item ledger with no behaviour effect | REJECT from minimum | `NC-B1-01` deletion test |
| Activity only exists after player prompt | FAIL PA-08 | `CF-B1-04` + `NC-B1-03` |
| Activity integration used to excuse bad gameplay | REJECT product adoption | `NC-B1-04`; playable fun remains empirical |
| Player action writes private quest flag while NPC action uses shared state | FAIL PA-09 | `CF-B1-03` + `NC-B1-05` |
| Debug lineage leaks player authorship to non-witnesses | FAIL composition | `CF-B1-08` + `NC-B1-06` |
| Player/activity directly mutates belief/relationship/next action/memory/schedule | FAIL composition | `NC-B1-07` |
| Every ordinary interaction becomes permanent history | FAIL boundedness | `NC-B1-08` |
| Many movable props but no meaningful shared consequence | does not satisfy embodied agency | `NC-B1-09` / `NC-E21` |
| “Two actors can differ” without a concrete two-actor fixture | FAIL PA-07 | strict reading of `CF-B1-01 Required result` recorded above |
| Player consequence merely *could* continue off-screen | FAIL PA-09 | strict two-downstream-decision witness interpretation recorded above |

No unresolved blocker remains inside the B1 research acceptance boundary.

## 7. Mechanical state observed before final handoff

On the pre-review candidate before this evidence commit:

- `Arkus Main Safety`: GREEN;
- `Context Capsule Validation`: GREEN;
- `Arkus Candidate Validation`: skipped while PR remained Draft/ACTIVE, so it is not claimed as final exact-SHA evidence.

This pre-review commit changes only Worker evidence and therefore requires the final exact candidate SHA to be re-read after commit before the PR handoff is frozen.

## 8. Residuals allowed

The following remain intentionally deferred and are not B1 blockers:

- runtime execution of the research fixtures;
- exact service/material quantities and economic tuning;
- final minigame mechanics, controls, animation and fun proof;
- persistence/save-load implementation;
- population/performance budgets;
- final semantic action API/action catalogue;
- combat/crime/governance implementation;
- H3/H4 Living World implementation.

## 9. Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`UNRESOLVED_MATERIAL_FINDINGS: 0`

`BATCH_UNITS_READY_FOR_INDEPENDENT_REVIEW: PA-07, PA-08, PA-09`

The next step is exact-SHA handoff/freeze for a fresh independent Reviewer. No Worker PASS is asserted.
