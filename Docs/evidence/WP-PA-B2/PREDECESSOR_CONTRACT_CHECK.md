# WP-PA-B2 — PREDECESSOR_CONTRACT_CHECK

Worker state: `ACTIVE_WORKER`
Baseline: `main@78a6a867d65f86a36b63db9d472324ae9dfac7d6`
Execution class: `RESEARCH_BATCH`
Date: 2026-09-26

## Dependency identity

`WP-PA-B2` is dependency-valid because `WP-PA-B1` was accepted through PR `#237`, independent PASS review `#5325533949`, merge `eb716bb4edddad8c59f416fb9f415d5eaf1845f6`, and binding post-acceptance DocSync on main.

B2 preserves the separate binding unit contracts:

- `Docs/workpacks/PA/WP-PA-10.md` — Autonomous Events & Causal Chains;
- `Docs/workpacks/PA/WP-PA-11.md` — Investigation, Legibility & Traces;
- `Docs/workpacks/PA/WP-PA-12.md` — Governance as Intervention in Simulation.

The batch contract requires all three original unit positive/negative gates plus cross-unit consistency. Internal PA-10 → PA-11 → PA-12 answers may be researched serially in this branch without intermediate merge/review because this is a `RESEARCH_BATCH`; the separate result artefacts remain individually reviewable.

## Accepted predecessor guarantees consumed

### PA-01 — routine

Expected schedule/routine is intent, not fate. Actual execution may deviate when current opportunity, capacity, material state or interruption changes, and actors re-evaluate current context.

### PA-02 — agency

Actors own meaningful decisions from bounded, authorized opportunities/state. Systems may expose choices, constraints and consequences; they may not decide the actor action/target and then attribute it to the actor.

### PA-03 — relationships

Directed stance, structural ties and bounded obligations may be causal inputs. Downstream systems do not own a parallel universal social score.

### PA-04 / PA-05 — belief and information flow

Canonical truth is not actor knowledge. Testimony and reaction must be limited to legitimately perceived/communicated information. Engine/debug lineage is never ambient actor knowledge.

### PA-06 — memory

Actor-accessible experience may become a bounded causal witness; memory does not own current truth, belief, relationship or action choice and is not an infinite event log.

### PA-07 — work/material/service

Human-facing service opportunity depends on bounded opening/staffing/capacity/named material conditions. Missing service/material state may alter later choices; no hidden macroeconomy or invisible instant replacement is required.

### PA-08 — activity integration

Activities own local session/game state only. Participation is actor-owned, capacity/time/place are real, and only a small structured outcome crosses into normal owners.

### PA-09 — player causal agency

Player and NPC semantic world actions must enter compatible immediate owners when context/capability/authority are equivalent. Player-caused consequences may continue after departure through shared state and actor-owned decisions; no player-only quest-causality universe is permitted.

The accepted B1 A11B-10 repair also establishes the story/systemic collision policy family: an authored reservation may `block / defer / substitute / replan` conflicting systemic use while preserving causal history and releasing authority cleanly rather than rewriting the cause.

## PA-10 binding research input

Canonical contract: `WP-PA-10.md`.

Read-only donor preregistration:

- repository: `Arkus0/Juego`;
- branch: `master`;
- path: `Docs/living-city-research/PA-08_PLAN_AUTONOMOUS_EVENTS.md`;
- donor plan baseline: `c55b0486ba05347638a3c83ada1f6b0fdf618b2a`.

The remapped unit must retain the donor attack surface: actor-originated 3+ causal chain, bounded termination/stable aftermath, bounded participant recruitment, explicit authored-story collision handling, a de-escalation counterfactual, and output sufficient for later legitimate legibility without making the event layer an omniscient log.

`LIVING_WORLD_CROSSCUTTING_AMENDMENT_02_INTENTIONAL_TRANSFORMATION.md` additionally requires termination to permit a genuinely changed equilibrium; termination cannot mean automatic restoration of starting conditions.

## PA-11 binding research input

Canonical contract: `WP-PA-11.md`.

Read-only donor preregistration:

- repository: `Arkus0/Juego`;
- branch: `master`;
- path: `Docs/living-city-research/PA-09_PLAN_INVESTIGATION_LEGIBILITY.md`;
- donor plan baseline: `c55b0486ba05347638a3c83ada1f6b0fdf618b2a`.

The remapped unit must retain: missed-event reconstruction from at least two independently useful legitimate channels, false/stale testimony, routine anomaly, loss/unavailability of one clue channel, separation of evidence from conclusion, and bounded trace retention. PA-11 consumes PA-10 causal output but may not rewrite upstream simulation merely to manufacture the clue required by a solution.

## PA-12 binding research input

Canonical contract: `WP-PA-12.md`.

Read-only donor preregistration:

- repository: `Arkus0/Juego`;
- branch: `master`;
- path: `Docs/living-city-research/PA-10_PLAN_GOVERNANCE.md`;
- donor plan baseline: `c55b0486ba05347638a3c83ada1f6b0fdf618b2a`.

Current Juego2 strengthening contract:

- `Docs/research/living-world/PA-12_AMENDMENT_01_EMERGENT_GOVERNANCE.md`;
- `Docs/research/living-world/LIVING_WORLD_CROSSCUTTING_AMENDMENT_02_INTENTIONAL_TRANSFORMATION.md`.

PA-12 therefore must prove more than a policy modifier. It needs macro decision → concrete rule/resource/opportunity change → independent actor response → third-person consequence → later local player response, plus divergent sustained trajectories without ideology-mode flags, recovery from ordinary mistakes, deliberate transformation that can overcome stabilizers, and repeal that does not erase already-caused history.

## B2 ownership and non-claims

B2 may decide research-level semantic responsibilities for:

- bounded autonomous chain lifecycle and recruitment/termination constraints;
- legitimate trace/evidence families, provenance/independence and reconstructibility;
- municipal lever categories and the macro↔micro governance loop;
- cross-unit compatibility between chains, traces and governance.

B2 does **not** implement or freeze:

- runtime APIs/classes/storage schemas;
- Unity systems, H2 character behaviour or CITY geometry;
- final scheduler/event budgets, propagation frequency or persistence limits (PA-13);
- final notebook/investigation UI, dialogue presentation or accessibility;
- a full economy, legal/crime, election, ideology or political simulator;
- final authored story orchestration beyond consuming the accepted reservation/collision contract;
- final integrated H3/H4 architecture.

## Reopen condition

An accepted predecessor is reopened only if this research demonstrates that its exact accepted guarantee cannot express a mandatory B2 scenario. Richer design preference, additional edge cases or alternative architecture are not reopen conditions.

`PREDECESSOR_CONTRACT_CHECK: PASS`
