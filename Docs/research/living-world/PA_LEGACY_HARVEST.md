# Juego → Juego2 PA legacy harvest

Version: 1.0 — 2026-09-21  
Status: **PROCESS_ONLY / NON-BINDING HARVEST INPUT**  
Source repository: `Arkus0/Juego`  
Target repository: `Arkus0/Juego2`

## 0. Purpose

Preserve the research value already paid for in the old `Arkus0/Juego` Living City Research Track without importing its abandoned runtime architecture, milestone graph or authority model.

This document is deliberately a **harvest**, not an acceptance shortcut:

```text
old independently reviewed research
        ↓
extract product finding / failure mode / fixture
        ↓
strip old M9/M10/WorldState/DFU routing
        ↓
reconcile with current Juego2 PA amendments
        ↓
WP-PA-XX independent review
        ↓
Juego2 research finding may become accepted input
```

Old `PASS`, `DELTAS_READY` and merge history prove that the donor research was seriously reviewed. They do **not** make the result authoritative in Juego2. That boundary remains exactly as stated by `PA_ROADMAP.md` and `JUEGO_KNOWLEDGE_LEDGER.md`.

## 1. What actually existed in Juego

The donor archive is materially stronger than a set of notes.

| Juego2 PA | Donor artefact | Donor review state | Juego2 disposition |
|---|---|---|---|
| PA-01 Daily Life | `Juego/Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md` | independent PASS on candidate `42f08346fbf518eb19e1158ea8448926a1a4d86b`, PR #32 review `5224625310` | **HARVEST / REVALIDATE**, do not re-research by default |
| PA-02 NPC Agency | `PA-02_NPC_AGENCY.md` | independent PASS on `dfe8a2b10831774f846274143c58dc41227d6231`, PR #36 review `5225572191` | **HARVEST / REVALIDATE** |
| PA-03 Social Graph | `PA-03_SOCIAL_GRAPH.md` | independent PASS on `205aab2ae2cdd7cfd8b5fda987cbf3bbd6cba41c`, PR #38 review `5226298241` | **HARVEST / REVALIDATE** |
| PA-04 Knowledge / Beliefs | `PA-04_KNOWLEDGE.md` + frozen plan | independent PASS on `672dcfa46dc1212d43f5302b4937ddd345bf249a`, PR #41 | **HARVEST / REVALIDATE** |
| PA-05 Rumours | `PA-05_RUMOURS.md` + frozen plan | first candidate FAIL; corrected candidate `1c8ec93b52d828744ca5b6504ca211b446be73c7` independent PASS, PR #43 review `5230865571` | **HARVEST / REVALIDATE** |
| PA-06 Memory | `PA-06_PLAN_MEMORY_CONSEQUENCES.md` | frozen plan only; no completed dossier | **RESEARCH** |
| PA-07 Work / Material Dependencies | `PA-07_PLAN_ECONOMY_WORK.md` | frozen plan only | **RESEARCH** |
| PA-08 Leisure / Activities / Minigames | no donor equivalent with this ownership | Juego2 already has a frozen plan, study `NOT_STARTED` | **RESEARCH IN JUEGO2** |
| PA-09 Player Causal Agency | no donor equivalent with this ownership | Juego2 already has a frozen plan, study `NOT_STARTED` | **RESEARCH IN JUEGO2** |
| PA-10 Autonomous Events | Juego `PA-08_PLAN_AUTONOMOUS_EVENTS.md` | frozen plan only | **REBASE PLAN + RESEARCH** |
| PA-11 Investigation / Legibility | Juego `PA-09_PLAN_INVESTIGATION_LEGIBILITY.md` | frozen plan only | **REBASE PLAN + RESEARCH** |
| PA-12 Governance | Juego `PA-10_PLAN_GOVERNANCE.md` + Juego2 governance amendment | donor plan only; Juego2 direction materially strengthened | **REBASE + RESEARCH** |
| PA-13 Simulation Control | Juego `PA-11_PLAN_SIMULATION_CONTROL.md` + Juego2 cross-cutting amendments | donor plan only | **REBASE + RESEARCH** |
| PA-14 Integration Review | Juego `PA-12_PLAN_INTEGRATION_REVIEW.md` | donor plan only; old M9/M10 targets obsolete | **REWRITE AFTER CURRENT PHASE BOUNDARIES EXIST** |

## 2. High-value findings to carry from PA-01..05

These are **candidate Juego2 findings** until their corresponding `WP-PA-01..05` passes. They are intentionally stated without donor implementation ownership.

### PA-01 — Daily Life

Carry:

- schedule expresses **intent / expected routine**, not a waypoint screenplay;
- activity and destination/affordance are distinct: an actor wants to do something and needs a compatible place/resource;
- travel time and route realization belong downstream of schedule intent;
- interruptions require cleanup and later re-evaluation rather than blindly resuming a stale script;
- calendar/context overrides must be bounded and explainable;
- capacity/contention matters where several actors compete for the same affordance;
- inspection should distinguish **what the actor was expected to do** from **what actually happened and why**;
- FULL/ABSTRACT fidelity may differ without changing the causal meaning of a committed routine.

Do not carry old M9 ownership or a preselected scheduler implementation.

### PA-02 — NPC Agency

Carry:

- NPCs must be valid **initiators**, not merely responders to the player or authored triggers;
- candidate action/target discovery must be bounded rather than a global scan over every actor/object;
- decision inputs may include needs/pressures/goals, relationships, beliefs, opportunity, cooldown/inertia and current commitments;
- initiator and receiver decision ownership must remain explicit for Actor→Actor actions;
- failure/replanning and commitment/inertia are necessary anti-thrashing concerns;
- off-screen agency must preserve meaningful causal continuity;
- important decisions need deterministic/explainable traces suitable for debugging and player-facing explanation where appropriate;
- the research did not justify freezing Utility AI, GOAP, rules or another single algorithm as universal authority.

### PA-03 — Social Graph

Carry:

- relationships are **directed** and can be asymmetric;
- relationships must alter choices/targets/opportunities, not merely flavour dialogue;
- household/family, affinity, trust, fear, authority/status, debts/favours/obligations, employment and rivalry are useful semantic families, not a requirement for one giant score vector;
- relationship history may matter, but unbounded biography is not required;
- a core regression is: same external situation + materially different relationship edge ⇒ potentially different explainable decision.

### PA-04 — Knowledge, Belief, Ignorance & Deception

Carry:

- canonical truth is not actor belief;
- actors only acquire information through legitimate perception/communication/public-source paths;
- belief needs provenance sufficient to explain how an actor came to hold it;
- stale, uncertain, partial and false beliefs are valid states without changing canonical truth;
- secrets and deception require information asymmetry, not hidden omniscient flags available to every decision;
- two actors in the same world may make different correct-for-them decisions because they hold different beliefs;
- decision/dialogue conditions should query actor-accessible knowledge rather than global truth unless the mechanic explicitly owns omniscient access.

### PA-05 — Rumours & Information Flow

Carry the corrected post-FAIL model:

```text
sender chooses/gets opportunity to communicate
        ↓
explicit KnowledgeTransfer(sender, receiver, asserted claim, channel)
        ↓
receiver-owned interpretation using actor-accessible inputs
        ↓
belief acquisition/revision belongs to PA-04 owner
        ↓
receiver may later choose to retransmit — or not
```

Additional invariants:

- no autonomous `RumourManager` should mechanically walk a social graph and broadcast claims;
- repetition never changes canonical truth;
- sender motive/opportunity belongs to action/agency ownership, not the rumour subsystem;
- propagation needs bounded rate/scope/cooldown/termination dimensions, but exact constants remain empirical;
- **privileged causal/debug lineage** may support replay, budgets and loop guards;
- **actor-accessible/reported provenance** is separate and may affect belief only when the actor has a legitimate path to know it;
- hidden engine lineage must never make an actor epistemically smarter than its accessible information.

That last distinction was the causal blocker in the first PA-05 review and is therefore a particularly important negative regression to preserve.

## 3. Current Juego2 deltas that donor research did not own

Every harvested PA must be reconciled with the current programme rather than copied verbatim.

### Playable causal city

NPC autonomy is not enough. The player is a first-class causal actor whose embodied/dialogue/institutional actions enter the same consequence ecology. Harvested findings must not recreate a museum that is fascinating to watch but weak to play.

### Activities / minigames

Leisure and repeatable activities may be ordinary town systems with real time/place/capacity and bounded outcomes. They must not become isolated score-screen universes or exist only when the player launches them.

### Intentional transformation

`Normality is an attractor` protects ordinary play from accidental runaway cascades; it does not protect the initial town from sustained coherent player action. PA-13 must therefore study both anti-chaos recovery and the ability for deliberate pressure to overcome it.

### Emergent governance

Governance changes rules/resources/opportunities and lets normal actors react. It must not puppet minds or set a global ideology mode. Macro decisions must become third-person consequences that later local action can help, resist, exploit or modify.

## 4. Donor material explicitly not migrated as authority

The following may be cited historically but cannot become Juego2 truth merely through this harvest:

- donor `WorldState` runtime ownership;
- M9/M10 milestone numbers or dependency graph;
- DFU/Shenmue runtime/format assumptions;
- exact old C# schemas, command names or private registries;
- old save/event-storage architecture;
- old numeric tuning constants;
- old package/license/adoption conclusions;
- `PASS` as automatic Juego2 acceptance.

## 5. What counts as successful harvest

A `WP-PA-01..05` harvest/revalidation PASS should be cheap relative to redoing the donor study. The Worker must:

1. read the exact donor dossier and review history;
2. extract only product semantics, failure modes, scenarios and evidence still relevant;
3. reconcile them with current Juego2 cross-cutting amendments;
4. mark donor mechanisms `ADOPT / ADAPT / LATER / REJECT` for Juego2 research purposes;
5. produce a compact canonical Juego2 finding document rather than copying donor architecture wholesale;
6. identify concrete future consumer phases without editing their implementation contracts prematurely;
7. preserve at least one causal negative/control scenario that could falsify the finding;
8. receive independent Reviewer PASS.

If a donor claim is no longer adequately supported or conflicts with current product direction, the correct result is to downgrade/research that claim — not to protect the old PASS.

## 6. Relationship to implementation

PA research does not implement the game.

Current production intent places:

- H2 around the keeper playable shell, navigation, player movement/camera/interaction;
- H3 around actors, animation, clock, schedules, POIs and readable daily life;
- H4 around Living World Core — beliefs, relationships, events, memory minimum, outcomes and runtime/save minimum;
- H5 around GameFlow/cinematics/QTE;
- H6 around combat feeding consequences back into the same world;
- H7 around deeper actor-to-actor agency, abstract simulation and population scaling.

The PA track should therefore finish useful **research decisions before their consumer implementation is designed**, while deliberately deferring runtime proof, tuning and performance claims to those later phases.
