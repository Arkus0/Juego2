# DW-04 pre-calibration amendment 02 — PA-02 question/oracle alignment

Status: **PRE-EXECUTION / RESULT-INDEPENDENT**

Previous effective pre-calibration commit: `ad570738613c992963668be5abd89bfdeb26b7d6`  
New effective pre-calibration commit: `9cbed950a3469897cf286c9a90624c240701528f`

No DW-04 calibration or acceptance model call occurred before this amendment.

## Defect corrected

`C-PA-01` was anchored to PA-02 §9 `NC-01 — No-player-trigger / fake agency control`, and its frozen oracle expected `player_trigger_required=NO`, `actor_decision_without_player=REQUIRED`, blocker `player_trigger_fake_agency`. The question text nevertheless asked whether initiative could be manufactured "by a schedule". The accepted NC-01 does not establish a generic schedule prohibition; it establishes that player presence / player or quest triggers / scripted events may not be the hidden cause selecting the actor action.

The question is therefore corrected to ask whether actor initiative may depend on a player/quest/script trigger and to recover the autonomy requirement when the player is absent.

## Unchanged surfaces

This amendment changes **only the semantic wording of `C-PA-01` to match its already-selected accepted anchor and oracle**. It does not change:

- any task identity or partition;
- the calibration run order or count;
- any acceptance-pool task or the deterministic six-task selection rule;
- any source path/blob/anchor;
- the `C-PA-01` expected facts, blocker, verdict or evidence identity;
- CTX or DW route semantics;
- the 30% context-reduction threshold;
- the 100% paired correctness rule;
- the invalid-run or restart budget.

The earlier commits remain in history. `9cbed950a3469897cf286c9a90624c240701528f` is the sole effective pre-calibration universe for all later calibration/acceptance lineage checks.