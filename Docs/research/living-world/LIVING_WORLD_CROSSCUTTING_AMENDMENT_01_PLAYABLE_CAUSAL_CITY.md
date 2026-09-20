# LIVING WORLD CROSS-CUTTING AMENDMENT 01 — PLAYABLE CAUSAL CITY

Status: **FROZEN CROSS-CUTTING RESEARCH AMENDMENT**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Applies to: future Juego2 continuation/integration of inherited Living City PA-01..12 plus Juego2 PA-11B/PA-07B  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> The Living World is not a museum, a screensaver, or an NPC-only simulation. It is a videogame system shared by player, NPCs, places, activities and authored story.
>
> **Cross-cutting thesis:** a good Living World continuously produces ordinary life, occasionally produces meaningful situations, allows the player to perturb those situations through dialogue and embodied action, and converts selected consequences into later behaviour without collapsing into chaos.

This amendment does not make old `Arkus0/Juego` findings authoritative in Juego2 and does not reopen H0. It records mandatory research questions that every future continuation/integration of the Living City PA lineage must consume.

---

## 1. Why this amendment exists

The inherited PA lineage is strong on autonomous NPC life, social causality, knowledge, rumours, memory, work, events, investigation, governance and simulation control. Two product risks remain if those studies are consumed independently:

1. **Living museum risk:** NPCs become sophisticated while the player mainly watches, talks, triggers quests or enters combat.
2. **Procedural anarchy risk:** advanced systems continuously amplify conflict until the town becomes noisy, unstable and unbelievable.

A third risk appears when minigames are added later:

3. **Activity island risk:** mus, billiards, fishing, bolos or other activities become isolated score/reward screens instead of part of town life.

This amendment therefore binds the research programme around one shared causal model.

```text
ordinary life / authored conditions / player interventions / world perturbations
                              ↓
                 opportunities + frictions
                              ↓
                actor-owned decisions/actions
                              ↓
                    structured outcomes
                              ↓
 perception / material state / belief / relationship / memory / access
                              ↓
               later decisions and situations
                              ↓
       player can notice, exploit, repair, worsen or ignore them
```

The simulation may run continuously. **Important drama must not.**

---

## 2. Cross-cutting invariants

### LW-X01 — Player and NPCs share one causal world

Player-originated and NPC-originated actions may differ in capability, control and authority, but meaningful equivalent outcomes should feed compatible world/consequence owners.

No PA may assume that rich causality is only for NPC->NPC actions while player actions live in quest flags, combat state or minigame rewards.

### LW-X02 — Dialogue is not the only non-combat verb

Future research must consider embodied/social/material player actions where relevant: give, take, return, accompany, follow, invite, signal, occupy, help, hinder, protect, intervene, use, reserve, block, repair, damage, mediate, authorize and other bounded high-value affordances.

No PA is required to create a giant immersive-sim verb list. Every PA must, however, avoid designs that accidentally force all meaningful interaction through dialogue.

### LW-X03 — Activities are town systems before they are player prompts

Where an activity is accepted as part of Living World scope, research must ask:

- why/when NPCs participate;
- whether they can participate without the player;
- place/capacity/reservation implications;
- social/material stakes;
- witnesses and information flow;
- how outcomes affect later opportunities/relationships/memory where justified;
- whether the player can join, influence, refuse, interrupt or alter the activity;
- whether the activity remains fun as gameplay independently of systemic hooks.

### LW-X04 — Ordinary life is the baseline

The city should spend most of its time functioning rather than producing dramatic incidents.

Normal work, travel, conversation, leisure, waiting, shopping, eating, socialising, maintenance and quiet routine are valid simulation outputs.

### LW-X05 — Normality is an attractor

After bounded perturbations, the town should usually tend toward a coherent operating state unless strong causes justify a lasting transformation.

Mechanisms may include recovery, repair, replacement, apology, reconciliation, rescheduling, reallocation, forgetting/compaction, de-escalation and explicit event termination.

This does not mean restoring the exact previous state. It means the world should be capable of stabilising.

### LW-X06 — Causality may be organic; spectacle may not be fabricated

Meaningful situations can originate from:

1. player actions;
2. NPC goals/decisions;
3. world/material/routine perturbations;
4. authored seeds/constraints.

A pacing/director layer may modulate opportunity, salience or concurrency, but must not secretly manufacture actor motivation and then present it as autonomous agency.

### LW-X07 — Positive and neutral causality matters

A living city cannot be built only from aggression, lies, theft, grievances and punishment.

Research should support where useful:

- helping;
- invitations;
- friendship;
- gratitude;
- cooperation;
- mentoring/teaching;
- friendly competition;
- favours;
- reconciliation;
- celebration;
- shared leisure;
- mundane work accomplished together;
- information sharing without conflict.

A simulation whose interesting outputs are mostly conflict will drift toward anarchy even if every local decision is individually plausible.

### LW-X08 — Salience controls persistence and propagation

Not every state change deserves a WorldEvent lineage, durable memory, rumour or evidence trail.

Each owning PA must distinguish:

- transient local state;
- useful short-lived consequence;
- socially/materially salient event;
- durable history.

This distinction is required for product clarity and save/runtime budgets.

### LW-X09 — Low-stakes play is first-class gameplay

The product must eventually prove a bounded session in which the player does not advance the main story and does not enter full combat, yet can still create several meaningful later consequences through social, embodied, material, civic or activity interactions.

### LW-X10 — Combat is one intervention family, not a separate universe

Combat/martial outcomes that matter to people or the town must be capable of producing Living World consequences through shared owners.

Likewise, martial mastery should allow research into restraint, defence, intervention, sparring, intimidation/de-escalation and proportional aftermath rather than only `fight / no fight`.

### LW-X11 — Authored story places pressure; simulation owns ordinary consequences

Authored narrative may seed tensions, reserve actors/places, constrain critical outcomes or inject events. It should not replace every downstream response with bespoke scripts when shared systems can own them.

### LW-X12 — Legibility is part of agency

If the player changes the town but cannot later understand that anything changed or why, systemic agency will feel random or cosmetic.

Every relevant PA must expose enough causal information for PA-09/integration tooling to make consequences perceptible without omniscience.

---

## 3. Event ecology: how the city gets a “spark” without becoming chaos

The research programme must treat **event ecology** as a composition of PA-08 and PA-11, with upstream inputs from the other PAs.

### 3.1 Sources of perturbation

A meaningful chain may begin from:

- **player seed:** the player changes material/social/institutional state;
- **actor seed:** an NPC goal/conflict/opportunity produces an action;
- **world seed:** availability, absence, capacity, schedule, resource or environmental condition changes;
- **authored seed:** designers introduce a tension, visitor, festival, breakdown, deadline, dispute or story condition.

These sources must converge into the same downstream consequence model where semantically appropriate.

### 3.2 Small variation is not automatically an event

Examples:

- Antonio chooses another chair;
- Carmen serves someone first;
- Manolo is ten minutes late;
- Pilar sees Juan walking with Antonio;
- Paco cannot use one tool and tries another.

These may remain ordinary state/decisions.

A chain becomes event-worthy only when accepted salience/causal criteria justify additional propagation, memory, trace or investigation cost.

### 3.3 Pacing layer may suppress opportunity, not steal agency

A director/storyteller may legitimately:

- limit concurrent high-salience chains;
- suppress optional opportunity injection while the town is saturated;
- select among equivalent authored seeds;
- vary timing/windows;
- promote player-visible opportunities;
- protect critical authored sequences through explicit constraints.

It must not:

- force Manolo to hate Paco because “the drama meter is low”;
- make an NPC choose an action they would reject under their own state;
- invent omniscient knowledge;
- bypass relationship/belief/material owners;
- continuously escalate because spectacle is desired.

### 3.4 Stability mechanisms to investigate

PA-08/PA-11 must jointly evaluate:

- event/concurrency budgets;
- actor cooldowns and inertia;
- commitment before replanning;
- escalation thresholds;
- de-escalation paths;
- explicit termination/stable-state criteria;
- bounded participant recruitment;
- locality/relevance filtering;
- recovery/repair/replacement;
- relationship repair/reconciliation where appropriate;
- memory/rumour decay and compaction;
- positive/neutral action availability;
- authored constraint collision handling;
- FULL/ABSTRACT continuity;
- kill switches/degradation under pathological state.

### 3.5 Target qualitative rhythm

Do not freeze numeric pacing gates without evidence. The intended qualitative shape is:

```text
many ordinary decisions
      +
several noticeable small consequences
      +
few salient chains
      +
rare memorable events
```

Not:

```text
important incident every minute
```

The exact distribution is a PA-11 measurement question.

---

## 4. Cross-PA amendments

The sections below are **mandatory additional questions/acceptance surfaces** for future Juego2 continuation of each inherited PA. They do not replace the original central question.

### PA-01 — NPC Daily Life

Add:

- leisure/social activities are part of daily life, not only work/idle;
- POI capacity/reservation should support both NPC-only and player-joined activities where accepted;
- accompaniment/following may alter travel/schedule coherently;
- player occupancy/help/hindrance can perturb routine through shared state;
- normal day must remain interesting without requiring constant incidents;
- activity participation must not be dormant until player arrives.

Required added scenario:

```text
GIVEN a normal day with no authored quest or major event
WHEN NPCs use work, social and leisure POIs and the player perturbs one bounded opportunity
THEN routines adapt through shared schedule/activity state
AND most actors continue ordinary life without global cascade.
```

### PA-02 — NPC Agency & Autonomous Social Action

Add:

- player presence is one possible context input, not the necessary trigger for agency;
- NPCs may initiate invitations, games, help, accompaniment, disputes, reconciliation and other positive/neutral acts;
- NPC decisions may target opportunities changed by the player without becoming player-scripted reactions;
- action vocabulary should compose with PA-11B player-equivalent outcomes where appropriate;
- anti-chaos guardrails must include positive/neutral alternatives and refusal to escalate.

Required added attack:

> Can the same social/material situation support plausible help, avoidance, negotiation or de-escalation rather than always selecting the most dramatic action?

### PA-03 — Social Graph That Changes Behaviour

Add:

- player is a participant in directed relationships, not a privileged global reputation source;
- witnessed embodied actions, shared activities, favours, victories/losses and restraint may become relationship inputs;
- NPC<->NPC and NPC<->player relationship edges use compatible semantics where justified;
- activity partner/rival/invitation/refusal choices should be able to consume relationship state;
- relationship change must include repair/reconciliation pathways, not only accumulation of grievance.

Required added proof:

> Same player action, different existing relationship/context -> different plausible response or downstream opportunity without a global `playerReputation` oracle.

### PA-04 — Knowledge, Belief, Ignorance & Deception

Add:

- NPCs can form beliefs from witnessed player actions, not only dialogue statements;
- embodied actions and activity outcomes need provenance/perception where socially relevant;
- actors who did not witness or learn an action must not react as if they did;
- player may deliberately show, conceal, demonstrate or create evidence through action;
- activity/minigame results can become known through witness/report paths, not magical town broadcast.

Required added proof:

> A visible player action creates different beliefs in witness/non-witness actors; later information flow may change the non-witness state through an allowed source.

### PA-05 — Rumours & Information Flow

Add:

- player-caused events can enter rumour/report channels;
- leisure venues/activities can be information-transfer contexts without becoming automatic gossip emitters;
- an NPC may choose to retell a game result, argument, kindness, embarrassment or mayoral decision for their own reasons;
- positive/neutral information must be possible, not only scandal;
- propagation budgets should interact with event salience and saturation.

Required added proof:

> A player-originated event propagates to some actors through explainable choices while another equally observable but low-salience event dies locally.

### PA-06 — Memory & Consequences

Add:

- selected player interactions/activities can become memories;
- most gestures, routine encounters and minor game results must not become permanent biography;
- restraint, help, betrayal, unusual generosity, humiliation, repeated shared activity or major outcome may have different salience;
- memory compaction must preserve causal minimum for later player-visible consequences;
- recovery/forgiveness/habituation should be considered where useful so memory does not create irreversible social entropy.

Required added proof:

> Repeated ordinary interaction remains bounded while one salient player-caused experience still explains a later autonomous decision after time/save-load.

### PA-07 — Work, Businesses & Material Dependencies

Add:

- player can perturb selected resources, access, capacity and work opportunities through accepted interactions;
- helping with ordinary work may be playable and have schedule/material/social consequences;
- leisure/service activities may compete with work/time/capacity where useful;
- material perturbation should create options for substitution/recovery, not automatic collapse;
- business/service state can affect minigame/activity availability without bespoke quest scripting.

Required added proof:

> A player-caused material change alters two actors differently, and the system can recover or substitute without requiring a designer-scripted reset.

### PA-07B — Leisure, Social Activities & Integrated Minigames

New dedicated owner. See `PA-07B_LEISURE_SOCIAL_ACTIVITIES_AND_MINIGAMES.md`.

Owns:

- activity ecology;
- NPC autonomous participation;
- participation/stakes/outcome boundary;
- player join/leave/interfere semantics;
- reusable Living World hooks for minigames;
- anti-island validation.

Does not own individual game rules, relationship model, belief model or schedule architecture.

### PA-08 — Autonomous Events & Causal Chains

Strengthen substantially:

- distinguish **ordinary state change** from **salient event chain**;
- permit player/NPC/world/authored seeds to enter compatible downstream causal machinery;
- preserve a separate proof of genuinely NPC-originated chain;
- investigate event ecology rather than event generation volume;
- require escalation, de-escalation and stable termination;
- include positive/neutral chains, not only conflict;
- director/storyteller may modulate opportunity/concurrency but not fabricate actor motivation;
- chains may recruit activity/POI participants through bounded local rules;
- player-caused chain may continue after player leaves.

Required added scenarios:

```text
A) quiet baseline -> no salient event despite many ordinary decisions;
B) NPC-originated chain -> >=3 changes -> terminates;
C) player-originated seed -> autonomous continuation -> terminates;
D) same opportunity under high saturation -> optional new chain suppressed/deferred without falsifying actor state;
E) positive/cooperative chain produces lasting useful consequence without conflict.
```

### PA-09 — Investigation, Legibility & Traces

Add:

- player should be able to perceive downstream consequences of their own earlier actions even when intermediate steps occurred off-screen;
- activities can generate testimony/routine anomalies/records/physical traces where justified;
- no trace spam for trivial interactions;
- the player can sometimes discover “I caused this” without a UI declaring the complete chain;
- embodied actions may themselves create evidence;
- positive/neutral changes should also be legible when gameplay-relevant.

Required added proof:

> Player performs action A, leaves, returns after autonomous chain B/C, and can reasonably infer at least part of the relationship between A and current state through accepted channels.

### PA-10 — Governance as Intervention in Simulation

Add:

- mayoral action is a first-class player causal channel;
- policies alter rules/opportunities/capacity and actors react through normal systems;
- mayor can also use embodied/social actions outside policy menus: inspect, attend, mediate, accompany, publicly support, refuse, help;
- civic festivals/events may host PA-07B activities without becoming bespoke cutscenes;
- policy consequences need recovery/reversal and saturation handling;
- authority must not equal direct mind-control.

Required added proof:

> A mayoral decision changes opportunity state; different citizens react differently; at least one leisure/work/social activity changes; reversal does not require hand-resetting each NPC.

### PA-11 — Simulation Control, Failure Modes & Budgets

This PA becomes the principal owner of **stability controls** for the playable causal city.

Add explicit measurements/attacks for:

- salient-chain concurrency;
- actor action frequency/cooldown/inertia;
- rumour/memory/event retention growth;
- player-caused cascade amplification;
- minigame/activity event spam;
- repeated aggression vs de-escalation;
- positive/neutral action balance;
- recovery time to coherent stable state;
- locality of participant recruitment;
- director suppression/defer behaviour;
- authored story collision rate;
- FULL/ABSTRACT divergence;
- pathological player griefing without save/world collapse;
- NPC-NPC autonomous chain pressure with player absent.

Required negative gate:

> A test that runs representative ordinary life plus adversarial player perturbations for an extended simulated period must not converge toward universal hostility, permanent venue failure, infinite rumour/memory growth or endless concurrent incidents unless the test deliberately injects catastrophic causes.

### PA-11B — Player Causal Agency & World Intervention

PA-11B remains owner of player-originated causal agency.

This amendment reinforces:

- dialogue + embodied + material + civic + martial + activity channels;
- causal continuation after player leaves;
- minigames as possible Living World interfaces;
- proportional consequences rather than binary impunity/apocalypse;
- player freedom constrained by real state/authority rather than invisible quest walls;
- no requirement that every possible prop/action be simulated.

PA-11B must consume PA-08/11 findings for pacing and containment rather than inventing its own global event budgets.

### PA-12 — Integration Review

PA-12 or its Juego2 successor **MUST NOT** declare the Living World concept integrated unless accepted inputs cover all of the following:

1. NPC daily life;
2. autonomous NPC agency;
3. relationships changing decisions;
4. belief/ignorance;
5. bounded information flow;
6. bounded memory/continuity;
7. material/work causality;
8. leisure/activity/minigame ecology;
9. autonomous and player-seeded causal chains;
10. investigation/legibility;
11. governance/player institutional intervention;
12. simulation stability/budgets;
13. player causal agency through dialogue **and non-dialogue** action.

Added integration scenarios:

```text
A — Quiet hour:
Town runs coherently with many ordinary actions and no forced drama.

B — Player-free autonomy:
Player absent; NPC-originated chain occurs, remains legible and terminates.

C — Player perturbation:
Player changes a shared material/social state; autonomous consequences continue after departure.

D — Integrated activity:
NPCs participate in a leisure activity without player; player later joins/affects one; outcome feeds at least one later system without bespoke quest flags.

E — Recovery:
Conflict/perturbation causes disruption; town reaches a coherent later state through normal recovery/replanning rather than global reset.

F — Low-stakes session:
No main story and no full combat; player still creates meaningful persistent/semi-persistent consequences.

G — Anti-anarchy soak:
Extended representative simulation does not degenerate into universal conflict, event saturation or unbounded state growth.
```

---

## 5. Integrated-minigame rule

A future minigame/activity is not required to use every Living World system. It must have a deliberate integration profile.

Recommended template:

```text
Activity identity:
Location / time / availability:
Who can initiate:
Who can participate:
Capacity/reservation:
Player entry/exit:
NPC-only operation:
Stakes:
Structured outcome:
Potential witnesses:
Potential relationship input:
Potential belief/rumour input:
Potential memory input:
Material/work dependency:
Schedule consequence:
Failure/interruption/resume:
Persistence budget:
What remains purely minigame-local:
Why it is fun without Living World hooks:
```

No activity should receive fake depth by checking every box. The point is explicit ownership and composability.

---

## 6. Product-level negative tests

The integrated research programme must eventually be able to reject each of these false positives:

### Living museum
NPCs act brilliantly; player can mainly watch/talk/fight.

### Procedural soap opera
Something dramatic happens every minute because systems constantly seek salience.

### Anarchy attractor
Minor conflicts only escalate; relationships deteriorate faster than they repair; services progressively fail.

### Theme-park activity island
Minigames wait for player prompts and disappear from NPC life.

### Quest-script causality
Interesting player actions only matter when an authored quest explicitly handles them.

### Prop sandbox
Hundreds of physics interactions exist but few affect people/opportunities.

### Omniscient town
NPCs react to player actions they neither witnessed nor learned.

### Permanent biography
Every wave, conversation and game becomes durable memory/event state.

### Director puppetry
A hidden storyteller forces irrational behaviour because drama level is low.

### Combat dimension
Violence produces combat-specific state but does not feed relationships, knowledge, work, governance or later behaviour where relevant.

---

## 7. Research execution order implication

This amendment does not require blindly executing PA-01..12 again.

For already-completed inherited PA-01..05, future Juego2 integration should perform a **bounded amendment review** against the added questions instead of discarding valid prior research.

For not-yet-started PA-06..11, the added surfaces should be incorporated before execution.

Recommended remote research sequence when available:

```text
PA-05 independent closure if still useful
 -> PA-06 Memory
 -> PA-07 Work/Material
 -> PA-07B Leisure/Activities/Minigames
 -> PA-08 Event Ecology/Causal Chains
 -> PA-09 Legibility
 -> PA-10 Governance
 -> PA-11 Simulation Control/Budgets
 -> PA-11B Player Causal Agency (can overlap where evidence is useful)
 -> PA-12 Integration Review
```

Exact ordering may change when H1/H2 production dependencies are known. PA-11 collects failure findings continuously.

---

## 8. Freeze rule

**AMENDMENT STATUS: FROZEN**

Future findings may narrow mechanics and may reject candidate features. They may not silently return the programme to any of these weaker definitions:

- Living World = sophisticated NPC behaviour only;
- player agency = dialogue + quests + combat;
- events = constant procedural spectacle;
- minigames = isolated side content by default;
- stability = manually resetting the town after systems misbehave.

The target is one **playable, causal, bounded and recoverable living city**.