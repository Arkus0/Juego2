# WP-PA-06 — Memory causal fixtures

Date: 2026-09-24
Status: post-PASS repair candidate; research-level acceptance surface; no runtime execution claimed

These fixtures specify observable semantic outcomes for later H4/H7 consumers. A paired run clones every non-target input: actor, candidate set, goal, opportunity, current world truth, PA-04 belief, PA-03 relationship, PA-05 delivery, authored rule, seed and tie-break order. `Memory` below is an actor-accessible selected experience, never a second truth, belief or relationship store. A pass requires the named outcome, not merely that an implementation could allow it.

## CF-01 — Selected past help changes a later autonomous choice

At day 1 the player and Manolo jointly help Antonio recover a dropped work parcel in a witnessed, resolved interaction. Antonio directly experiences both contributions. Under the predeclared selection policy, Manolo's voluntary help is unusual and belongs to a cooperation/reliability consumer class that can be retained without knowing which later opportunity will occur. The parcel and immediate work delay are gone by day 3. PA-03 directed trust/affinity, PA-04 beliefs, obligations, role, schedule, candidate eligibility, seed and all other inputs are frozen equal in both day-3 runs; no quest script chooses Antonio's action.

The fixture fixes a tie-free action-family rule: Antonio's present goal selects `ASK_SHIFT_COVER`; both candidates are eligible; absent a relevant selected help witness the fixed ordinary ranking puts Paco ahead of Manolo. For this action family only, a selected personally experienced act of reliable help by an eligible candidate is a declared reason to rank that candidate ahead of the ordinary order. The rule does not assign trust, compel Manolo to accept, or choose the action family for Antonio. The day-1 witness is the only changed input between paired day-3 evaluations.

```text
Day 3 common: Antonio needs shift cover; Manolo and Paco are eligible.
No-memory control: Antonio asks Paco under the declared ordinary ranking.
Selected-memory run: one actor-accessible day-1 help witness concerning Manolo
                     is supplied as the declared help-ranking input;
                     Antonio asks Manolo.
```

The player directly carries/returns the parcel while Manolo volunteers time. The selected witness names each only to the extent Antonio observed their role. Thus the fixture is player-caused and the day-3 choice has a person-specific memory reason, without silently changing PA-03 relationship state. The no-memory run differs only by removing that selected witness. The decision trace must name the eligible alternatives, declared `ASK_SHIFT_COVER` consumer, day-1 experienced help, actor access cause, causal selection checkpoint/reason, and why it changes the choice. If the result requires changing trust, candidate order, a hidden player quest flag, authored selection or knowledge of the actual day-3 opportunity at the day-1 checkpoint, this fixture fails.

The same causal path can support a player-targeted later decision when the player is an eligible participant; no player-only memory channel is permitted. A mere altered dialogue line with the same choice fails.

## NC-01 — Routine experience does not become infinite biography

Antonio has 100 low-stakes repeated ordinary greetings and minor game results across a long span. They create no unresolved commitment, strong personal stake, new belief, material relationship change or declared future action consumer. Include both witnessed player encounters and off-screen NPC encounters with valid local access; compare with a run containing no such candidates.

Required after selection/compaction: no 100 individual persistent autobiographical records; routine candidates are rejected or compressed into a bounded pattern if a genuine consumer exists. For this fixture there is none, so the later `ASK_SHIFT_COVER` decision and material reason trace equal the no-routine control. A system that writes 100 permanent records then merely hides them in UI fails. A fixed small ring that simply overwrites the salient CF-01 witness to make room for greetings also fails.

## CF-02 — Compaction, save/load and FULL/ABSTRACT continuity

Take CF-01 after its day-1 experience. Generate unrelated routine life, then transition Antonio `FULL -> ABSTRACT -> save -> load -> FULL` before day 3. Compact/prune the original detailed event stream under a declared policy. The retained selected witness or compact active-reason equivalent must still name the experienced help, participants as Antonio knew them, day/order and relevance to `ASK_SHIFT_COVER`.

Under identical semantic state, seed, eligible alternatives and declared rule, day-3 action/target and material reason are the same before and after the transition. Exact serialized bytes and debug-only IDs need not match; normalized causal meaning must. A dangling event ID, post-load default memory, hidden event-log replay, or an actor-safe summary reconstructed from privileged details fails. If a runtime chooses to preserve the detailed source event for an active reason, it must still demonstrate bounded retention for unrelated candidates.

## NC-02 — Remembering a false claim does not restore obsolete belief

On day 1 Carmen hears Manolo assert `X=false` through a valid PA-05 channel. PA-04 records that belief. Carmen may select a memory of *having heard and once believed* the claim. On day 2 she sees an authoritative accessible correction and PA-04 revises current belief to `X=true`. On day 3 memory compaction/save-load occurs.

Required: current PA-04 belief stays `X=true`; memory may report the historical hearing with correct temporal framing, but cannot write `X=false` back or mark `X=true` stale solely from its old episode. A day-3 action requiring belief uses `X=true`. Hidden canonical truth/PA-05 root lineage remain unavailable to Carmen absent an access cause. A memory selector that promotes every received transfer without salience also fails the NC-01 bound.

## NC-03 — Relationship repair does not erase history or create a second current edge

On day 1 Manolo betrays Antonio; an authorized consequence changes PA-03 `trust(Antonio -> Manolo)` and Antonio retains a selected experience with a bounded reason. On day 2 restitution/reconciliation through normal owners changes the directed current trust again. At day 3, the relationship consumer reads PA-03's **current** value. The memory may remain a historical reason or, if declared relevant, an additional distinct caution input; it cannot claim that day-1 trust remains the current edge or overwrite day-2 repair.

Paired day-3 runs keep the historical memory fixed and change only the PA-03 current relationship input through the authorized day-2 effect: a relationship-dependent action must follow its declared current-state semantics. A second paired run keeps PA-03 current state fixed and changes only memory eligibility: any memory-specific choice change must be named as such rather than relabelled a trust change. No `grudgeScore` parallel to PA-03 is accepted.

## NC-04 — Actor access, hidden lineage and off-screen equality

An event occurs off-screen. Antonio directly experiences it during ABSTRACT execution; Paco is absent and receives no perception, public-source or PA-05 communication path. Antonio may form a selected memory under exactly the same salience rule used on-screen. Paco must not. In a paired Antonio run, change only privileged event/root/transfer lineage while preserving his accessible experience, rule, seed and every actor-visible input. His selected memory, later decision and actor-facing reason must be identical. A later legitimate report to Paco can create a candidate of *being told*, not a synthetic eyewitness memory.

## CF-03 — Severe consequence persists; recovery remains causal

Run A: the player repeatedly blocks a service after visible feedback, and Antonio directly experiences separate high-impact lost opportunities. Active material/commitment state remains altered. The relevant selected summary and owner state may continue to affect an autonomous decision after a long interval; a generic normality timer must not erase them.

Run B: from an equivalent initial state, the player instead helps restore service, provides restitution and ceases disruption. Authorized owners resolve material state and any relationship/obligation effects. Antonio's later decision may change because those causes changed current state and memory relevance. A bounded witness may still explain prior caution, but memory cannot indefinitely force Run A's choice after the reasons have genuinely been resolved. Neither run uses `playerWantsChaos` or automatically resets to the initial town. This compares persistent harm with causal repair; exact severity and retention windows remain empirical.

## NC-05 — Distinct salient experiences still have a finite total bound

Configure any finite selected-memory semantic capacity `K` chosen by the future implementation. Give Antonio more than `K` actor-accessible experiences that are deliberately **distinct rather than repeat-groupable**: different participants, subjects and relevance classes, each initially eligible under the same declared policy, with no hidden duplicates and no trivial-event shortcut. At the pressure checkpoint, none may be made invisible merely by moving it to an unbounded overflow/history list owned by PA-06.

Required:

```text
selected-memory surface <= K
pressure outcome is deterministic/seedable under the declared policy
at least one over-capacity candidate is retired, demoted, merged or summarized
no unbounded PA-06 overflow side list appears because the candidates are distinct
active effect reasons, if any, remain explainable only through bounded memory capacity
or an explicitly bounded representation owned by the active effect's real owner
```

A system that passes NC-01 by grouping repeated greetings but stores one permanent witness for every unique salient episode fails NC-05. A system that calls all over-capacity records “active reasons” to escape the cap also fails. The exact value of `K`, weighting and replacement algorithm remain empirical H4/H7 choices; the existence of a finite bound and deterministic saturation behavior do not.

## NC-06 — Selection cannot read the future

Create two histories, A and B, identical through the end of day 1: same Antonio state, actor-accessible help experience from Manolo, current goals/roles/relationships/beliefs, declared selection policy, consumer classes, seed and selection checkpoint. The histories diverge **only after** that checkpoint:

- A later produces the day-3 `ASK_SHIFT_COVER` opportunity from CF-01.
- B later produces an unrelated opportunity for which the day-1 help will not be consumed.

At the day-1 selection checkpoint, A and B must produce the **same** retain/reject/promotion decision and the same causal selection reason. The selector may know that reliable help belongs to a declared cooperation/reliability consumer class; it may not know whether `ASK_SHIFT_COVER` will actually occur in A rather than B.

Required:

```text
same prefix through checkpoint -> same selection result at checkpoint
future-only divergence -> cannot alter the already-made day-1 selection decision
no privileged replay may resurrect a rejected day-1 episode because A later needs it
```

A later legitimate actor-accessible reminder, new report or new related experience may create a new candidate prospectively, with its own provenance and checkpoint. For this control, no such reminder exists. If A retains the help only because the implementation can see the future shift-cover decision while B drops it, PA-06 fails as post-hoc fixture fitting.

## Failure criteria across the set

The research claim fails if any future realization requires complete biography replay for a later choice; one record per trivial event; unbounded growth from distinct salient episodes; post-hoc retention selected with knowledge of a later opportunity; an unbounded overflow/active-reason side list; decorative memories without material choice effect; actor memory from inaccessible events/hidden lineage; memory assignment to PA-04 or PA-03 current state; silent loss of the last reason for an active consequence; unbounded off-screen inflation; uniform expiry that erases severe active causes; or eternal grievance despite genuine repair. A future H4/H7 implementation must execute corresponding causal controls and measure actual save/CPU budgets; these documents alone are not execution receipts.
