# WP-PA-B3 — SIMULATION CONTROL FIXTURES

Status: `WORKER CANDIDATE EVIDENCE`  
Batch: `WP-PA-B3`  
Unit: PA-13  
Date: 2026-09-26

This file makes the PA-13 acceptance surface falsifiable without claiming current runtime implementation, final numeric budgets, shipping scale or Unity proof.

## 0. Shared fixture rules

All fixtures preserve the accepted PA ownership model.

Simulation control may select only execution-control outcomes such as execute, defer, degrade, cap, reject-unrepresentable or terminate. It may not directly choose actor goals/actions/targets, rewrite belief/relationship/memory semantics, invent material truth or restore the starting town.

Every future executable version must record enough structured evidence to identify:

- semantic owner requesting/producing work;
- source/scope used for candidate discovery where applicable;
- eligible/candidate/expensive-comparison counts where applicable;
- control decision and reason;
- cap/degradation hit if any;
- causal state before/after;
- fidelity transition/reconciliation result where applicable;
- terminal/deferred status rather than silent disappearance.

Numeric values below are **fixture magnitudes for falsification**, not shipping budgets, except the `>=10,000` irrelevant-decoy requirement already frozen by SV-1.

---

# F13-01 — Anti-thrashing under unchanged semantic inputs

## Purpose

Closes donor-remapped A11-1 and the PA-02 commitment/stability failure class.

## Given

Antonio has one current actor-owned problem and two valid alternatives `A` and `B`. Their ranking is close enough that incidental resolver noise/order could otherwise flip the winner. No material actor-accessible input changes across repeated resolver opportunities.

## Stress

Request repeated reconsideration across many scheduler opportunities while preserving:

- same snapshot/state;
- same seed;
- same semantic inputs;
- same eligible alternatives;
- no invalidation event;
- no newly available opportunity.

## PASS

- the actor does not alternate `A/B/A/B...` merely because resolution runs again;
- a commitment/inertia/hysteresis/cooldown/event-driven equivalent explains why reconsideration is suppressed or why a change is legitimate;
- if the commitment expires, the reason is explicit rather than implicit tick count hidden from diagnostics;
- the mechanism does not freeze the actor forever when a real invalidation/input change occurs.

## Negative

FAIL if repeated calls with unchanged semantic inputs can cause persistent oscillation through incidental iteration/timing order.

---

# F13-02 — Deterministic semantic replay

## Purpose

Closes donor-remapped A11-2 and non-reproducible-ordering risk.

## Given

A frozen semantic snapshot containing:

- a small set of persistent actors;
- one service contention;
- one actor decision;
- one bounded communication opportunity;
- one causal chain continuation;
- one deferred operation;
- fixed ordered external inputs and seed.

## Run

Execute the future headless/runtime fixture multiple times from the exact same accepted snapshot/inputs.

## PASS

Normalized semantic outputs are equivalent:

- selected semantic decisions/targets where the action contract is deterministic/seeded;
- owned material deltas;
- accepted/deferred/terminated chain steps;
- reservation winner where contention semantics require one;
- budget/cap/degradation outcomes;
- final causal state.

Presentation frame timing, animation sampling or non-authoritative diagnostic timestamps may differ if they cannot feed back into semantic authority.

## Negative

FAIL if hidden hash/container/thread timing changes a semantic winner with all declared inputs fixed.

---

# F13-03 — SV-1 irrelevant-population independence

## Purpose

Makes PA-13 SV-1 executable for a future local/known/role/place/opportunity-scoped query.

## BASE

- Antonio has a local actor-owned problem.
- Authorized candidate source `R` contains two relevant candidates.
- Expensive comparison is permitted only after scope/source and cheap eligibility.
- All semantic inputs are frozen.

## STRESS

Add **at least 10,000** persistent actors that are distant and outside the authorized source/scope. Preserve `R` exactly.

## Required instrumentation

Record at minimum:

- source/scope identity;
- records enumerated to obtain relevant candidates;
- cheap-eligibility survivors;
- expensive comparisons/scoring operations;
- whether any cap/degradation path fired;
- semantic result.

## PASS

- semantic result is unchanged;
- relevant enumeration is unchanged or remains under the same declared deterministic bounded query cap;
- expensive comparison is unchanged or remains under the same declared deterministic cap;
- irrelevant actors are not inspected merely to rediscover `R`;
- no failure path falls back to whole-population enumeration;
- any cap hit is explicit and reproducible.

## Negative

`AllPersistentActors -> filter local/known/relevant -> compare two` fails even if the final compared set contains only two actors.

---

# F13-04 — SV-1 dense-local candidate surface

## Purpose

Prevents “we use a spatial index” from becoming a false proof of bounded work.

## BASE

A local place/cell/neighbourhood contains the small relevant candidate surface required by one interaction class.

## STRESS

Increase unrelated or semantically ineligible co-located population sharply while holding the relevant semantic opportunity constant.

## PASS

Before expensive pairwise/scoring work, the future implementation applies an explicit deterministic bound such as:

- scoped reservation surface;
- partition/sub-index by action/role/opportunity;
- capped deterministic candidate selection;
- owner-provided bounded source;
- another mechanism with equivalent bounded work.

The expensive surface cannot become unbounded all-pairs merely because one spatial bucket is crowded.

## Negative

A spatial hash with `for each actor in cell -> compare with every other actor in cell` and no bound fails.

---

# F13-05 — SV-2 bounded ABSTRACT→FULL reconciliation

## Purpose

Makes PA-13 SV-2 executable without selecting an implementation.

## Given

A fixed set of ABSTRACT actors and a fixed amount of **material summarized change** relevant to promotion. Keep semantic material change constant across runs.

## Runs

Increase off-screen elapsed omitted micro-time by orders of magnitude, for example:

- Run A: short elapsed interval;
- Run B: much longer interval;
- Run C: orders-of-magnitude longer interval.

Do not increase the amount of material summarized change that actually needs reconciliation.

## Required instrumentation

Record:

- omitted elapsed time / hypothetical micro-steps;
- materially changed actor/domain records;
- summarized/material events/deltas considered;
- reconciliation operations by owner/domain;
- any declared catch-up cap/degradation;
- semantic reconciliation result.

## PASS

Reconciliation work remains bounded by material summarized state/change and declared reconciliation budget, rather than growing in proportion to every omitted micro-tick solely because elapsed time increased.

If a domain legitimately requires elapsed-time-dependent processing, that cost is explicit and bounded by that domain contract rather than hidden as generic tick replay.

## Negative

`for each omitted tick -> run every actor's normal FULL update` fails the fixture even if the eventual result looks correct.

---

# F13-06 — FULL↔ABSTRACT semantic continuity

## Purpose

Closes donor-remapped A11-4 independently of performance.

## Given

An actor with a representative subset of accepted state:

- identity;
- routine context + actual current state;
- current commitment;
- one directed relationship input;
- one actor-accessible belief;
- one selected memory witness;
- one material/service dependency or claim;
- one bounded event/causal reason;
- an active institutional condition.

## Transition

`FULL -> ABSTRACT -> FULL` while enough time/state change occurs to require actual reconciliation.

## PASS

- identity is preserved;
- owned semantic state is either preserved or changed only by legitimate summarized causes;
- actor-private state is not replaced with canonical/debug truth;
- material/service state is not fabricated to match the expected schedule;
- current-context re-evaluation occurs rather than replaying stale physical execution;
- any approximation is explicit in the diff/control trace;
- failure to represent a required fact causes explicit keep-full/defer/degrade/reject behaviour rather than silent semantic corruption.

---

# F13-07 — Controlled overload and truthful degradation

## Purpose

Closes donor-remapped A11-5 and the PA-13 kill-switch/degradation requirement.

## Given

A future stress scene exceeds one or more configured provisional execution budgets with a mix of:

- optional new autonomous initiatives;
- already-active commitments;
- one important material consequence;
- one reconstructible trace window;
- one low-salience background chain continuation.

## PASS

Overload handling follows an explicit priority/degradation policy that may defer optional initiation or reduce fidelity while preserving already-owned truth and required causal minimums.

The trace distinguishes at least:

- work that was not eligible;
- work deferred;
- work cap-hit/degraded;
- work rejected because required fidelity could not be represented;
- work already completed/terminal.

## Kill-switch variant

Trigger the emergency containment path.

PASS only if it:

- stops/reduces admission of new optional work;
- preserves current material/belief/relationship/obligation/causal state;
- does not mark skipped work completed;
- does not teleport/reset actors;
- surfaces a diagnostic reason;
- allows later owner-safe re-evaluation/recovery.

---

# F13-08 — Cascade cap without epistemic or causal corruption

## Purpose

Closes donor-remapped A11-6.

## Given

A legitimate communication/event/action chain where one accepted step may create another actor-owned opportunity.

## Stress

Generate enough legitimate possible continuations to hit the configured provisional propagation/concurrency envelope.

## PASS

- continuation eventually terminates or is explicitly deferred/capped;
- tooling names the controlling budget/guard;
- delivered actor belief/provenance already created remains intact;
- hidden technical lineage used for loop/dedup does not become actor evidence;
- already-caused world/material state remains;
- later fresh legitimate causes may produce new work after the relevant cooldown/pressure changes.

## Negative

FAIL if a cap simply deletes the chain record and downstream state so the incident appears never to have happened.

---

# F13-09 — Authored reservation collision and release

## Purpose

Closes donor-remapped A11-7 while preserving the accepted B1/B2 story/systemic policy.

## Given

- an accepted systemic cause already exists;
- GameFlow holds one explicit exclusive bounded reservation on actor Carmen / Bar F01 for an authored beat;
- a systemic continuation would conflict during that reservation.

## Positive policy

Choose `DEFER` for this fixture.

```text
accepted cause remains
 -> incompatible Carmen use deferred
 -> GameFlow reservation runs
 -> reservation releases exactly once
 -> current state is re-evaluated
 -> deferred work resumes, replans or terminates according to current semantics
```

## PASS

- original causal root and already-produced deltas remain;
- GameFlow authority is limited to the declared reservation;
- release does not permanently strand Carmen under GameFlow;
- release does not replay every omitted background micro-action;
- post-release work obeys current-context rules and SV-2.

Equivalent fixtures may exercise `BLOCK`, `SUBSTITUTE` or `REPLAN` when their semantics are legitimate.

---

# F13-10 — Explain the budget

## Purpose

Closes donor-remapped A11-8.

## Given

Five semantically distinct non-occurrence cases:

1. no eligible candidate exists;
2. eligible work is intentionally deferred;
3. candidate surface hits a declared cap;
4. cheaper fidelity is selected;
5. requested fidelity/state cannot be represented truthfully and is rejected/kept higher fidelity.

## PASS

A machine-readable/structured diagnostic or scenario diff lets a reviewer distinguish all five without guessing from prose logs.

No exact API is frozen. Ambiguous `skipped=true` for all five is insufficient.

---

# F13-11 — Paired clumsy player vs persistent destabiliser

## Purpose

Closes the intentional-transformation amendment's required two-sided control.

Both runs begin from the same accepted town state and use the same semantic owners.

## Run A — clumsy / isolated mistake

The player makes one low/medium-severity legitimate intervention that temporarily harms Bar F01/public activity availability, receives legible feedback and **does not reinforce** the disruption.

Expected causal possibilities include ordinary actor replanning, bounded service degradation, restitution/repair, replacement resource, changed short-lived opportunity or relationship/memory consequences if legitimately caused.

### PASS A

- the initial consequence remains real;
- propagation does not automatically self-amplify town-wide;
- normal owners have plausible finite recovery/adaptation paths;
- the bounded horizon remains broadly functional;
- recovery is caused, not a fixed reset timer.

## Run B — persistent destabiliser

From the same start, the player repeatedly applies accepted high-impact disruptive actions **after** observing consequences, for example repeatedly withdrawing/redirecting key shared access/capacity/resource support and obstructing normal recovery where those actions are legitimately available.

### PASS B

- each intervention enters the same PA-07/09/12 and downstream owners as ordinary play;
- actors/services/institutions react through their own state and choices;
- recovery capacity can be consumed or defeated;
- simulation-control caps bound concurrent work but do not erase/reverse the causal pressure;
- the run may end in a materially different stable or unstable-but-playable state.

## Paired oracle

FAIL if both runs converge to the same default town solely because simulation control heals everything.

FAIL if Run A routinely collapses without reinforcing causes.

---

# F13-12 — Constructive transformation

## Purpose

Proves anti-chaos is not a morality/destruction-only filter.

## Given

Same initial state as F13-11.

## Run

Across a bounded horizon the player repeatedly uses accepted constructive interventions, for example:

- restoring/supporting a useful service dependency;
- increasing legitimate shared activity capacity through PA-12;
- fulfilling public commitments;
- mediating/repairing consequences through existing action/social owners;
- reinforcing a new stable community opportunity after feedback.

## PASS

- changes use the same semantic owners as destructive/ordinary actions;
- a recurring opportunity/service/social pattern differs materially from baseline;
- accepted relationships/beliefs/memories may differ only where legitimately caused;
- simulation control does not damp the town back to its initial configuration merely because the new state is unusually positive/cooperative;
- no `GoodTown` mode/score is semantic authority.

---

# F13-13 — Persistence pressure / no unbounded exception list

## Purpose

Composes PA-05/06/10/11 bounded state under one PA-13 pressure case.

## Stress

Generate over a long bounded test horizon:

- many routine communications;
- many ordinary activity outcomes;
- many distinct selected-memory candidates;
- multiple completed causal chains;
- multiple traces, only a subset inside active reconstructibility windows;
- a small set of still-active obligations/effects needing causal reasons.

## PASS

- routine repetition can compact/drop under declared rules;
- selected memories remain within a finite chosen test budget with deterministic pressure outcome;
- completed inactive lineage can compact;
- active reconstruction windows preserve their promised minimum;
- active effect reasons remain bounded and owner-safe;
- no second `importantForever` / overflow / pinned-all-history list grows without bound.

Numeric capacity is a test input and remains provisional until runtime evidence.

---

# F13-14 — H2 proxy non-claim

## Purpose

Prevents batch closure from being misread as H2 Living World proof.

## Given

An H2 character can idle, walk/bounded-move and emit minimal dialogue/bark in a keeper visual scene.

## PASS

Documentation/tooling classifies that behaviour only as H2 presentation/proxy evidence.

It does **not** satisfy:

- PA-02 autonomous choice;
- PA-04 actor-local belief;
- PA-06 selected memory;
- PA-10 causal-chain autonomy;
- PA-13 SV-1/SV-2 runtime proof;
- FULL/ABSTRACT semantic continuity.

Any future gate claiming otherwise fails the PA-B3 non-claim.

---

## 15. Future executable ownership matrix

| Fixture family | First likely executable owner | Mandatory deeper owner | Later empirical owner |
|---|---|---|---|
| F13-01 anti-thrashing | H3 only if it introduces actor-choice/reconsideration; otherwise H4 | H4 | later tuning/profile gate |
| F13-02 deterministic semantic replay | H3 for its owned routine/opportunity state | H4 for Living World composition | later regression/scale gate |
| F13-03/04 SV-1 | H3 local POI/service/activity queries | H4 actor/event recruitment | later representative performance gate / H7 if canonical |
| F13-05/06 SV-2 + continuity | H3 if first FULL/ABSTRACT seam exists | H4 Living World semantic reconciliation | later representative performance gate / H7 if canonical |
| F13-07/08 overload/cascade | H4 | H4 | later scale/profile gate |
| F13-09 authored reservation | H4/GameFlow integration owner | H4 | later authored-content stress |
| F13-10 diagnostics | H3 minimal counters where relevant | H4 structured control trace | later tooling/gate owner |
| F13-11/12 transformation | H4 semantic runtime | later playable/systemic integration | long-horizon product tuning |
| F13-13 persistence pressure | H4 | H4/save owner | later scale/save profile gate |
| F13-14 H2 non-claim | H2-GATE documentation boundary | PA-14 integration review | n/a |

This table assigns proof responsibility, not runtime architecture.

## 16. Candidate evidence conclusion

The PA-13 research model has a falsifiable path for:

- anti-thrashing;
- deterministic replay;
- irrelevant-population independence;
- dense-local boundedness;
- bounded ABSTRACT catch-up;
- FULL/ABSTRACT continuity;
- overload and kill-switch truthfulness;
- cascade caps;
- authored reservation release;
- explainable control outcomes;
- recoverable ordinary noise;
- sustained destructive and constructive transformation;
- bounded persistence;
- H2 proxy non-overclaim.

`SIMULATION_CONTROL_FIXTURES: PASS_CANDIDATE`
