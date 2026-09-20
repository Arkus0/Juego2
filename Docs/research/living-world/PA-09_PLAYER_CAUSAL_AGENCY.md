# PA-11B PLAN — PLAYER CAUSAL AGENCY & WORLD INTERVENTION

Plan status: **FROZEN**  
Study status at freeze: **NOT_STARTED**  
Owner mode: **REMOTE**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> This PA exists because a living city is not the product by itself. Juego2 is a videogame. The player must be able to perturb the same world that NPCs inhabit and later observe understandable consequences of those interventions.
>
> **Product thesis:** the player is a first-class causal actor, not a privileged quest-script source.

This plan does not modify H0 contracts, reopen an accepted H0 guarantee, start runtime implementation, or make an old `Arkus0/Juego` PASS authoritative in Juego2. It defines a research question that must be resolved before the Living World is considered product-complete.

---

## 0. Why this PA exists

The Living World work already asks for proactive actors, Actor→Actor causality, beliefs, relationships, memory, schedules, autonomous events, legibility and bounded simulation. That is necessary but insufficient.

A city can be technically impressive and still play like a museum if the player mainly:

- watches NPC routines;
- consumes authored dialogue;
- starts quests through privileged triggers;
- fights in a separate combat dimension;
- launches isolated minigames whose results do not matter to anybody;
- receives bespoke scripted reactions instead of changing shared world state.

The desired product is stronger:

```text
player chooses / acts
        ↓
material, social or institutional state changes
        ↓
people may perceive, miss, misinterpret or learn about it
        ↓
beliefs / relationships / memories / opportunities change through their owners
        ↓
NPCs choose new actions for their own reasons
        ↓
new events and consequences occur, including with player absent
        ↓
the player later sees, exploits, suffers or investigates the aftermath
```

The city is therefore not scenery around the story. It is a **playable causal system**.

---

## 1. Exact research question

> **How can meaningful player actions alter material, social or institutional world state and produce later consequences through the same causal contracts used by the simulation, without relying on quest-specific magic, hardcoded per-NPC reactions or a parallel “player state” universe?**

**Observable win:** in a bounded town scenario, the player can spend a session primarily interacting with people, places and activities — without advancing the main story or fighting — and still generate distinct, understandable state changes that alter later NPC behaviour, opportunities and events.

A stronger win is that a causal chain can outlive the initiating interaction:

```text
player action
 -> Antonio reacts
 -> Carmen learns or witnesses something
 -> Carmen changes a later choice
 -> Manolo is affected while player is elsewhere
 -> player encounters the aftermath later
```

The chain must not require a hidden quest script to keep moving.

---

## 2. Core invariants to test

### PCA-01 — Player is a first-class causal actor

Meaningful player actions enter the causal world through explicit action/outcome/event/state mechanisms rather than arbitrary privileged mutations.

This does **not** require NPC and player control schemes to be identical. It requires semantically equivalent world actions to have compatible consequences regardless of who originated them.

### PCA-02 — Same world, not two semantic universes

There must not be one rich simulation for NPC-originated actions and a separate quest/script path for player-originated actions that bypasses normal consequence ownership.

If an NPC can create a debt, reserve a resource, insult someone, damage an object, spread information or alter access, the player doing the semantically equivalent thing should not need a second bespoke state model.

### PCA-03 — Consequence owners remain authoritative

A player action may cause inputs to belief, relationship, memory, schedule, event, material or policy systems. It does not directly own their internal state.

Examples:

- player lies → does not directly set `ActorBelief=true`; perception/communication decides what is believed;
- player insults Carmen → does not arbitrarily set every observer's relationship score;
- player blocks a doorway → does not script each NPC's new route;
- player wins at cards → does not hardcode dialogue flags for the whole bar.

### PCA-04 — Presence is not required for continuation

A player-originated consequence may continue through Actor→Actor/world causality after the player leaves.

The simulation must not secretly mean “nothing consequential happens unless the camera is here.”

### PCA-05 — Reactions are conditional, not universal

Different actors may react differently — or not at all — according to perception, knowledge, relationship, role, current goals, opportunity and material state.

### PCA-06 — Persistent effects need bounded state

Player agency must not become “everything leaves permanent bespoke history forever.” Relevant consequences should survive where useful; trivial interactions should not explode save/state size.

### PCA-07 — Explainability follows causality

When an important downstream behaviour changed because of the player, tooling should be able to explain the causal path without pretending the NPC had omniscient knowledge of the player's action.

### PCA-08 — Product value over simulation completeness

We are not trying to simulate every possible human response. We are trying to maximize the number and quality of **playable, composable consequences** from a bounded set of strong shared systems.

---

## 3. Hypotheses ex ante

| ID | Hypothesis | What would falsify it |
|---|---|---|
| H11B-1 | Shared outcome/event semantics between player and NPC actions will create more emergent gameplay than quest-specific reaction scripting for the same content budget. | Bounded scenarios show bespoke scripting is materially cheaper while preserving composition, persistence and variation. |
| H11B-2 | Most useful player causality can be expressed as changes to existing world concepts: material state, access, availability, ownership, knowledge, relationship inputs, events, commitments and rules. | Key gameplay repeatedly requires a separate player-only semantic model. |
| H11B-3 | Perception/knowledge mediation is essential: NPCs should not react to player actions they could not reasonably know about. | Omniscient reaction produces equal believability/gameplay without contradictions. |
| H11B-4 | The most valuable downstream consequence is often a changed future opportunity or decision, not an immediate bark/animation. | Tests show delayed systemic consequences are not perceivable or enjoyable relative to immediate authored feedback. |
| H11B-5 | Activities/minigames become substantially more valuable when their participants, stakes, schedules and outcomes feed the Living World. | Integrated outcomes add complexity without producing reusable or noticeable consequences. |
| H11B-6 | The player needs meaningful low-stakes ways to perturb the city, not only quests, crime and combat. | Non-story/non-combat interventions fail to create compelling sessions in playtests. |
| H11B-7 | Systemic consequences need authored constraints and salience budgets; unconstrained “everything affects everything” will become noisy and illegible. | Broad propagation remains coherent, performant and understandable without constraints. |

---

## 4. Research axes

### 4.1 Material intervention

Questions:

- What classes of world state can the player change persistently or semi-persistently?
- Which changes should alter navigation, access, availability, work, services or schedules?
- How do NPCs discover a changed material condition: direct perception, failed action, report, routine anomaly?
- Which changes are reversible, repairable, consumable, owned or contested?
- How much world manipulation is useful before the game becomes a sandbox-builder rather than our intended product?

Candidate examples, not commitments:

- move/remove/consume a resource relevant to an activity;
- open/close/block an access route;
- damage, repair or temporarily disable a useful object;
- leave, gift, hide or take an item where ownership matters;
- change the availability of a POI or activity through an accepted interaction;
- create physical evidence of an event.

### 4.2 Social intervention

Questions:

- What player acts should produce social consequences beyond an immediate dialogue response?
- How are witnesses distinguished from later recipients of information?
- How do lies, promises, favors, insults, debts, victories, losses and public behaviour enter existing belief/relationship/memory owners?
- How do reputation-like effects emerge from individual state without requiring one magical global reputation meter?
- When is an aggregate reputation useful as a derived/indexed view rather than truth authority?

### 4.3 Opportunity intervention

A powerful class of consequences changes what actors can or want to do rather than directly commanding them.

Examples:

- a seat/table/boat/tool becomes occupied or unavailable;
- the player helps one person obtain a scarce slot;
- a business or activity opens/closes under changed conditions;
- a social invitation creates an opportunity;
- a debt or favor changes which request is acceptable;
- information changes which target/place/time an actor considers.

Research should test whether **changing opportunities** creates more believable agency than directly scripting reactions.

### 4.4 Institutional intervention

Municipal/policy gameplay is a special high-level form of player causality.

The player may eventually affect rules, schedules, access, capacity or services. Those changes should enter the same town systems and let actors react through normal decision logic rather than becoming global percentage buffs or policy-specific scripts.

PA-11B owns the generic player-causality question. Governance research owns the semantics of municipal rules themselves.

### 4.5 Activity and minigame intervention

A minigame should be evaluated as a possible **Living World interface**, not automatically as a self-contained arcade screen.

For each activity ask:

1. Who chooses to participate and why?
2. Where and when is it available?
3. Can NPCs participate without the player?
4. What is at stake: money, pride, debt, trust, information, access, habit, status, resource, time?
5. Who witnesses the result?
6. Can the result become conversation, memory, relationship input or future opportunity?
7. Can prior social state alter the match/activity?
8. Can the activity be interrupted and resumed coherently?
9. Does repeated participation change the town rather than only a private score?
10. Is it still fun as a game even if all Living World hooks are removed?

The last question matters: systemic integration must strengthen a good activity, not excuse a bad minigame.

---

## 5. Candidate activity probes

These are research probes, **not promised shipping features**.

### 5.1 Mus / cards in the bar

Potential Living World value is unusually high:

- recurring participants and time/place habits;
- partner/rival selection;
- money or favor stakes;
- cheating accusation / trust consequence;
- overheard information during the game;
- somebody refuses to play with the player later;
- an NPC arrives late because the game ran long;
- winning/losing becomes a memory or social anecdote;
- absent regular at the table becomes an investigable routine anomaly;
- NPC→NPC matches can occur without the player.

The card rules themselves remain a normal minigame. Living World integration owns context and aftermath, not the truth of the card game.

### 5.2 Billiards / table game

Possible value:

- social venue and recurring rivalries;
- queue/reservation/turn-taking as POI capacity;
- skill reputation or challenge invitations;
- low-stakes wagers/debts;
- witnesses and conversational context;
- a physical activity whose occupied table changes opportunity for others.

### 5.3 Fishing

Possible value:

- time/place/conditions matter;
- shared locations with limited useful spots;
- catch becomes material resource or gift;
- local knowledge can reveal better places/times;
- a companion trip can create social time;
- absence from a routine can itself matter;
- illegal/restricted fishing, if ever designed, could produce witness/institutional consequences without a bespoke mission.

### 5.4 Darts / bolos / local competitive activity

Possible value:

- cheap repeatable social competition;
- invitations/rivalries;
- small tournaments as scheduled town events;
- crowd/witness effects;
- status/pride without needing a global RPG level system.

### 5.5 Helping with ordinary work

Not necessarily presented as a traditional minigame:

- unload deliveries;
- carry stock;
- repair/prepare a venue;
- help at market close;
- assist a farmer/fisher/workshop task.

The important outcome may be changed availability, debt/favor, schedule recovery or future access rather than a score screen.

### 5.6 Food / drink / bar social play

Potential value is less in button mechanics and more in social context:

- who sits with whom;
- who pays;
- invitation/refusal;
- overheard information;
- intoxication only if it creates useful bounded gameplay, not generic stat clutter;
- closing time and staff routine;
- debts/tabs/favors if they prove worthwhile.

---

## 6. Acceptance scenarios preregistered

### A11B-1 — Equivalent action, shared consequence spine

```text
GIVEN a world action class that can be initiated by either player or PersistentActor
WHEN player and NPC variants produce semantically equivalent accepted outcomes
THEN both enter compatible world/event/consequence contracts
AND downstream systems do not require a separate player-only reaction universe
AND differences are attributable to actor identity, capability, context or authority.
```

### A11B-2 — Player material change alters NPC decisions

```text
GIVEN two NPC routines depend on a material resource, access route, slot or POI state
WHEN the player validly changes that state
THEN at least two affected actors can re-evaluate differently through normal systems
AND no per-NPC reaction script is required
AND explain tooling identifies the changed opportunity/state as causal input.
```

### A11B-3 — Witnessed social act

```text
GIVEN player performs a socially meaningful action involving Antonio
AND Carmen can perceive the action while Pilar cannot
WHEN social processing occurs
THEN Carmen may acquire relevant belief/memory/relationship inputs through accepted owners
AND Pilar MUST NOT react as direct witness
UNLESS she later learns through an allowed information path.
```

### A11B-4 — Player leaves, causality continues

```text
GIVEN player initiates a meaningful action/event and leaves the area
WHEN affected actors continue normal simulation
THEN >=2 subsequent material/social changes may occur without player presence
AND at least one later player-visible consequence can be causally traced to the initiating action
AND the chain terminates/stabilizes within accepted budgets.
```

### A11B-5 — Same player action, different context

```text
GIVEN identical player input/action intent in two world states
AND witness, relationship, knowledge, role, resource or opportunity differs materially
WHEN outcomes propagate
THEN downstream consequences may differ for those reasons
AND MUST NOT depend solely on a hidden `playerDidX` quest flag.
```

### A11B-6 — Minigame has Living World consequences

```text
GIVEN a bounded social activity such as mus, billiards, fishing or another accepted probe
WHEN player participates and produces a meaningful result
THEN the activity may emit structured outcomes relevant to existing systems
AND at least one later behaviour/opportunity/dialogue/event can differ because of accepted outcome state
WITHOUT the minigame becoming authority over beliefs/relationships/schedules.
```

### A11B-7 — NPC activity exists without player

```text
GIVEN an activity is part of normal town life
WHEN player does not participate
THEN eligible NPCs can still use/schedule/contest/observe it where product scope requires
AND the venue does not exist solely as a dormant player interaction prompt.
```

### A11B-8 — Low-stakes session is still gameplay

```text
GIVEN no main-story advancement and no combat for a bounded play session
WHEN player talks, participates in activities and perturbs ordinary town state
THEN the session can produce multiple persistent or semi-persistent consequences
AND at least one new opportunity, changed behaviour or discoverable aftermath
AND the result is not merely flavour text or private minigame high scores.
```

### A11B-9 — No omniscient retaliation

```text
GIVEN player performs a consequential act without observers or information leakage
WHEN unrelated actors evaluate later decisions
THEN they MUST NOT react as if they know the act
UNLESS another accepted material/institutional state makes the consequence independently observable.
```

### A11B-10 — Story and systemic action compose

```text
GIVEN authored story reserves an actor/place/time or constrains an outcome
WHEN player systemic action would collide with that constraint
THEN the system blocks, defers, substitutes or replans through an explicit rule
AND does not silently erase the player's prior causal history
AND does not strand actor/GameFlow authority.
```

### A11B-11 — Save/load causal continuity

```text
GIVEN player causes a consequential state change
AND downstream consequences are pending or partially propagated
WHEN save/load occurs
THEN the relevant causal state survives according to its accepted owner
AND later behaviour remains explainable under equivalent state/seed
WITHOUT replaying the player's original input as a magic trigger.
```

### A11B-12 — Anti-museum negative test

```text
GIVEN a Living World demo with impressive autonomous NPC behaviour
WHEN the player attempts only interactions permitted by the demo
THEN Reviewer can identify multiple ways to change future world behaviour beyond starting authored quests or combat
ELSE the demo FAILS the product thesis even if NPC autonomy looks convincing.
```

---

## 7. Failure modes to attack actively

| ID | Failure mode | Signal | Risk |
|---|---|---|---|
| F11B-1 | Living museum | NPCs are fascinating but player influence is shallow. | Great demo, weak videogame. |
| F11B-2 | Quest magic | Player action sets bespoke flags/scripts that bypass shared systems. | Content explosion and fake systemic depth. |
| F11B-3 | Omniscient town | Everybody reacts to hidden player acts. | Breaks belief/perception model. |
| F11B-4 | Bark-only reactivity | Consequence is only immediate dialogue/animation. | No persistent gameplay effect. |
| F11B-5 | Global reputation shortcut | One number substitutes for individual knowledge/relationships. | Flattens social causality. |
| F11B-6 | Everything affects everything | Minor acts propagate indefinitely. | Noise, instability, save growth. |
| F11B-7 | Minigame island | Activity score/reward never touches town state. | Expensive disposable side content. |
| F11B-8 | Simulation excuse | Activity is systemically integrated but not fun. | Complexity without play value. |
| F11B-9 | Player puppet-master | Player directly commands NPC internal state. | NPC agency becomes cosmetic. |
| F11B-10 | NPC immunity | NPC-originated actions are systemic but equivalent player actions are disallowed/arbitrary. | Two semantic worlds. |
| F11B-11 | Consequence without legibility | State changes but player cannot reasonably notice why. | Agency feels random. |
| F11B-12 | Persistent clutter | Every conversation/game/object interaction becomes permanent history. | State/budget collapse. |
| F11B-13 | Main-story hostage | Interesting systemic actions only exist inside authored missions. | Free play feels empty. |
| F11B-14 | Combat exception | Combat outcomes bypass Living World causality. | Combat lives in separate dimension. |
| F11B-15 | Activity prompt world | NPCs never use activities unless player activates them. | Town feels staged for player. |

---

## 8. Ownership and boundaries

### PA-11B may decide

- minimum semantics required for player-originated actions to enter Living World causality;
- classes of player intervention worth supporting in the product;
- requirements for equivalence/composition between player- and NPC-originated outcomes;
- requirements for downstream continuation with player absent;
- activity/minigame integration requirements at the Living World boundary;
- anti-museum product gate/acceptance scenarios;
- requirements that player consequences remain perceptible and explainable.

### PA-11B may not decide

- exact belief representation;
- rumor propagation algorithm;
- memory retention/compaction algorithm;
- relationship dimensional model;
- general NPC decision algorithm;
- autonomous event escalation/termination internals;
- final evidence/investigation UX;
- municipal policy model;
- final simulation budgets;
- combat mechanics;
- rules of individual minigames;
- Unity input/controller implementation;
- final narrative content.

### Boundary with inherited PA lineage

The useful old `Arkus0/Juego` PA plans remain reference inputs only until explicitly continued/reviewed in Juego2.

PA-11B should compose with their conceptual ownership as follows:

| Area | Consumer/neighbor | Boundary |
|---|---|---|
| Belief/knowledge | PA-04 lineage | Player actions provide perceptible/communicated inputs; belief owner decides belief state. |
| Rumours | PA-05 lineage | Player can originate information; relay remains a receiver decision. |
| Memory/consequence | PA-06 lineage | Player-caused experiences may become selected memories; PA-11B does not define memory storage. |
| Work/material | PA-07 lineage | Player may perturb resources/opportunities; material/work semantics belong there. |
| Autonomous events | PA-08 lineage | PA-11B can seed a chain; autonomous continuation/escalation/termination belongs there. |
| Legibility/traces | PA-09 lineage | PA-11B requires consequences to be perceivable; trace/evidence design belongs there. |
| Governance | PA-10 lineage | Player policy choices are a high-level intervention; policy semantics belong there. |
| Simulation control | PA-11 lineage | Budgets/scale/continuity validate the cost of propagation. |
| Integration review | PA-12 lineage/successor | MUST consume accepted PA-11B findings before Living World implementation plan is frozen. |

**Integration rule:** PA-12 (or its Juego2 successor) cannot claim a complete Living World integration if it proves only autonomous NPC causality and lacks an accepted path for player-originated causal agency.

---

## 9. Research references and comparison targets

External research must answer concrete mechanisms/questions, not become a feature-shopping exercise.

Candidate families to study:

- immersive sims: systemic player actions and reusable world affordances;
- `Shadows of Doubt`: player investigation/intervention inside a systemic city;
- `The Sims`: low-stakes social/activity loops and persistent social consequences;
- `Dwarf Fortress` / `RimWorld`: world consequences and actor reactions, while separating colony-control assumptions from direct-player assumptions;
- `Kenshi`: persistent world/player intervention patterns and their limits;
- `Kingdom Come: Deliverance` / similar social RPGs: routine, crime, reputation and activity integration, distinguishing global shortcuts from actor-level causality;
- `Shenmue`: mundane activities, time/place/routine and social texture, while preserving the existing anti-RE firewall;
- strong minigame-in-world examples where participation changes relationships/opportunities rather than merely awarding currency.

Claims about internals require primary evidence before becoming architecture. Observation may establish player-facing behaviour but must not be presented as proof of hidden implementation.

---

## 10. Research method

For each candidate mechanic/reference:

```text
exact player-facing question
 -> observed/reference mechanism
 -> authority/causal interpretation
 -> what state must exist
 -> which existing owner should hold it
 -> downstream consequence possibilities
 -> failure/abuse case
 -> smallest Juego2 scenario
 -> KEEP / ADAPT / BENCHMARK / REJECT / LATER
```

Preference is for **small causal primitives with many compositions**, not a giant verb list.

Examples of useful primitive categories to test:

- transfer/give/take/owe;
- tell/ask/accuse/promise/invite/refuse;
- use/reserve/occupy/release;
- open/close/enable/disable;
- help/hinder/repair/damage where product-appropriate;
- compete/win/lose/wager;
- witness/report/conceal;
- policy/rule change at institutional scale.

These are research categories, not API names.

---

## 11. Product metrics / evidence targets

Do not invent numeric gates before observation. The study should nevertheless measure or characterize:

- number of downstream state changes per selected intervention;
- proportion of consequences that require bespoke content vs shared systems;
- delayed vs immediate consequence mix;
- ability to explain why a downstream actor changed behaviour;
- false omniscience rate in adversarial scenarios;
- persistence/state growth from repeated low-stakes play;
- number of reusable activity hooks shared across minigames;
- whether NPC-only and player-originated variants use the same semantic owners;
- whether a no-story/no-combat session yields meaningful different world state;
- how often systemic freedom collides with authored narrative constraints.

The goal is not “maximum branching.” The goal is **high consequence density per authored/systemic primitive**.

---

## 12. Expected project deltas

PA-11B findings may eventually justify deltas in:

- future Living World runtime contracts;
- player interaction/action outcome contracts;
- event/causal lineage;
- perception/witness interfaces;
- POI/activity participation and reservation;
- dialogue/social action input surface;
- GameFlow reservations/constraints;
- combat aftermath integration;
- save/runtime state boundaries;
- scenario runner / explainability tooling;
- H4/H6/H7/vertical-slice workpacks when they are authored;
- the final Living World integration review.

No delta is accepted merely because this plan lists the destination.

---

## 13. Stop conditions

PA-11B may stop when:

1. A11B-1..12 are answered with evidence or explicitly narrowed/rejected;
2. player action can seed causal chains without privileged quest magic;
3. material, social and at least one activity/minigame intervention are represented by bounded scenarios;
4. player absence does not stop downstream causality;
5. perception/knowledge prevents omniscient social reactions;
6. consequences remain bounded and explainable;
7. the design distinguishes shared semantics from player/NPC capability differences;
8. at least one low-stakes no-story/no-combat session model demonstrates meaningful world change;
9. remaining questions are implementation/detail rather than a missing product model.

Do **not** continue into exhaustive sandbox-verb research, full economy design, complete crime simulation, every possible minigame or procedural story generation.

---

## 14. Reviewer attack surface

Reviewer should actively try to prove that:

- the city is still mostly observational;
- player agency is quest flags wearing systemic names;
- NPCs know player actions omnisciently;
- consequences are immediate barks but no changed future behaviour;
- global reputation is hiding missing actor-level state;
- player actions mutate belief/relationship/memory directly;
- NPC and player variants use incompatible state universes;
- minigames are isolated reward dispensers;
- “integration” makes a weak minigame acceptable without fun;
- chains cannot continue after the player leaves;
- consequences cannot survive save/load coherently;
- systemic freedom silently breaks authored GameFlow;
- trivial interactions create unbounded persistent history;
- a demo can PASS while the player has no meaningful way to perturb the future town outside quests/combat.

The strongest negative question is deliberately simple:

> **If the NPC AI vanished, would the player still have a normal scripted RPG; and if the player vanished, would the town still have a simulation? If both are true but their causal systems barely touch, PA-11B has failed.**

The target is not two good systems side by side. It is one playable causal world.

---

## 15. Timing

PA-11B is intentionally **REMOTE-capable** research.

Recommended execution window:

- after H0/GATE planning is stable;
- opportunistically when H1 reaches a genuine LOCAL/Unity boundary during the owner's remaining travel/vacation period;
- early enough that accepted findings can shape H4 Living World work and the eventual integration review;
- before declaring the Living World conceptual model complete.

The PA should not block H1 remote work merely because it exists.

---

## Freeze rule

**PLAN STATUS: FROZEN**

This file freezes the research question, minimum scenarios, boundaries and reviewer attack surface. Findings may falsify the hypotheses and may conclude that fewer mechanics are necessary. They may not silently weaken the anti-museum thesis or redefine PA-11B into generic NPC simulation research.
