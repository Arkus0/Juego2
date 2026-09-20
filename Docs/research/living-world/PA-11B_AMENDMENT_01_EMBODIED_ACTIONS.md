# PA-11B PLAN AMENDMENT 01 — EMBODIED PLAYER ACTIONS, SOCIAL SIGNALS & NON-COMBAT AFFORDANCES

Amendment status: **FROZEN**  
Parent plan: `PA-11B_PLAYER_CAUSAL_AGENCY.md`  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> **Reason for amendment:** the frozen PA-11B correctly establishes the player as a first-class causal actor, but its acceptance surface can still be misread as “dialogue plus combat plus minigames.” That is not sufficient for the intended product.
>
> **Added product thesis:** the player must be able to act on people, places and shared situations through a bounded repertoire of embodied and social affordances. Dialogue is one action family. Combat is another. Neither may monopolize player agency.

This amendment does not reopen H0, implement gameplay, promise a specific verb list, or require a full immersive-sim object ontology. It strengthens PA-11B so that a future Living World cannot PASS while the protagonist is effectively a talking quest cursor who occasionally fights.

---

## 1. Exact amendment question

> **Which small set of reusable embodied, social and physical interaction primitives lets the player meaningfully perturb a living town outside dialogue, authored quests and full combat, while preserving NPC agency, causal ownership, bounded consequences and a coherent third-person videogame?**

**Observable win:** the player can spend a session changing social and material situations using a mixture of speech and non-speech actions, and NPCs can perceive, interpret, respond to and remember those actions according to context rather than through bespoke scene scripts.

The target is closer to an immersive-sim philosophy than a giant interaction wheel:

```text
few strong verbs + shared world semantics + context
                     ↓
             many useful situations
```

Not:

```text
hundreds of bespoke prompts
        ↓
content-specific reactions
```

---

## 2. New invariants

### PCA-09 — Dialogue is not the universal interaction adapter

If a meaningful town interaction can only be expressed by selecting a dialogue line, PA-11B should ask whether the underlying action deserves a reusable non-dialogue representation.

Examples include giving, taking, following, helping, occupying, blocking, signalling, inviting, carrying, intervening and physically protecting.

Dialogue may negotiate or explain an action. It must not be required merely because the game lacks another verb.

### PCA-10 — Embodied action can carry social meaning

NPC interpretation should be allowed to depend on what the player physically did, to whom, where, in front of whom, and under what circumstances.

A wave, refusal to move, helping carry stock, sitting at somebody's table, following somebody, stepping between two people, returning a lost object or knocking somebody down can all be socially meaningful without becoming dialogue branches.

### PCA-11 — Context determines meaning

The same input or physical action may have different social meaning according to consent, role, ownership, relationship, current conflict, location, witness state and severity.

Examples:

- following a friend after an invitation is accompaniment;
- following a stranger repeatedly may become suspicious;
- taking a chair from an unused terrace is not equivalent to taking a personal object from a home;
- grabbing someone during agreed martial practice is not equivalent to attacking them in the street;
- blocking a doorway during an emergency is not equivalent to blocking it for amusement.

### PCA-12 — Martial mastery has a graded action space

The protagonist being a kung-fu master must create **more** interaction possibilities, not collapse physical agency into `fight / do nothing`.

Research must distinguish, where useful:

- posture / warning / de-escalatory presence;
- avoidance and disengagement;
- protective positioning;
- separating participants;
- parry / evade / block;
- disarm;
- controlled push / off-balance / sweep;
- restraint / hold / immobilization;
- controlled knockdown;
- sparring / demonstration / teaching;
- defensive force;
- serious violence.

These categories are research targets, not final controls or move lists.

### PCA-13 — Physical conflict does not automatically equal game-ending crime

Consequences should be capable of depending on severity, context, witnesses, perceived intent, prior events, self-defence/defence-of-others, consent, injury, property damage, role and local rules.

The design must be able to represent a spectrum such as:

```text
social friction
 -> warning / demand to stop
 -> relationship consequence
 -> removal/refusal/service denial
 -> restitution/fine/obligation
 -> local authority intervention
 -> detention or serious legal consequence
 -> exceptional story-level consequence
```

This is **not** a commitment to model criminal law. It is an anti-binary requirement: `one punch -> universal lethal wanted state -> campaign effectively over` is not an acceptable default model for a protagonist whose physical competence is part of ordinary play.

### PCA-14 — NPCs may initiate embodied interaction too

Where a verb represents normal town behaviour, NPCs should be able to initiate a compatible form when product scope calls for it: beckon, wave, invite, accompany, hand over an object, ask for help, occupy a place, block access, intervene in a dispute, challenge to a game, request sparring, follow briefly, carry something or physically assist.

The town should not look as if every physical affordance exists only after the player presses an interaction button.

### PCA-15 — Mayor authority is a gameplay affordance, not omnipotence

Being mayor should create additional legitimate ways to change the city — request, authorize, prioritize, open/close, schedule, mediate, allocate, convene, inspect, promise, deny, intervene — without giving the player direct mind-control over citizens or arbitrary state mutation.

Institutional actions should change rules/opportunities and then let the Living World react.

### PCA-16 — Affordance density matters more than prop interactivity percentage

The target is not “every cup can be picked up.” Research should prioritize actions that create reusable causal consequences across many situations.

A small set of high-value affordances across people, POIs, resources and social situations is preferable to thousands of inert physics interactions that do not matter to anyone.

---

## 3. Interaction families to investigate

These are **families**, not promised controller buttons.

### 3.1 Social signalling without full dialogue

Candidate probes:

- greet / wave / nod;
- beckon / call attention;
- point / indicate place or person;
- invite / signal “come with me”;
- refuse / dismiss;
- thank / apologize through lightweight expression where appropriate;
- celebrate / congratulate;
- threaten / warn without initiating combat;
- show / present an object;
- offer / hand over / return something.

Research question: which signals deserve actual world semantics, and which are better left as animation/flavour?

A signal becomes valuable when context and NPC state can change its interpretation or downstream result.

### 3.2 Follow, accompany and shared movement

`NPC follows player` should not be a one-off Shenmue-style trick or quest escort primitive.

Questions:

- Can the player ask an NPC to accompany them for a bounded reason?
- Can an NPC invite the player to follow them?
- Can two actors walk and talk while preserving navigation and schedule consequences?
- Can accompaniment end naturally because of destination, time, relationship, danger or changed priorities?
- Can persistent following become socially meaningful or suspicious rather than being ignored?
- Can a third actor encounter the pair and infer something appropriate without omniscience?

Candidate consequences:

- shared travel creates conversational opportunity;
- actor arrives late elsewhere;
- witness sees two people together;
- player gains access because accompanied by someone authorized;
- actor abandons accompaniment due to conflict/priority;
- invitation/failure/refusal affects relationship or future opportunity.

### 3.3 Help, hinder and intervene

Candidate verbs/situations:

- carry or move a relevant item;
- hold/open access for someone;
- help unload/setup/repair/clean where represented;
- give up or reserve a place;
- fetch/bring an item through normal world ownership;
- physically help somebody up;
- intervene in an argument/fight;
- protect somebody's route or work;
- obstruct, distract or occupy a needed resource;
- refuse assistance after being asked.

The system should care primarily about changed opportunity/state and social interpretation, not a generic `helpfulness +1` meter.

### 3.4 Shared-space behaviour

A living town needs meaningful use of space:

- sit / stand / wait at socially relevant locations;
- join or leave a group;
- occupy a chair/table/game/fishing spot;
- queue or cut into a queue where modeled;
- enter a private/semi-private/public area;
- stay after closing;
- block or clear access;
- place/leave something in a meaningful location.

These can affect capacity, social context, visibility, invitation, refusal, staff behaviour and routine without requiring full sandbox physics.

### 3.5 Possession, exchange and small material choices

Candidate actions:

- give;
- lend / borrow where justified;
- return;
- take;
- pay / owe;
- leave for someone;
- hide / reveal;
- consume / use a scarce relevant resource;
- repair / damage / disable a bounded class of useful objects.

Ownership and knowledge matter. The same transfer can be gift, loan, theft, recovery or evidence movement depending on accepted state.

### 3.6 Non-lethal martial/social physicality

The protagonist's skill can enrich ordinary encounters:

- agreed sparring with locals;
- demonstration/teaching;
- helping break up a fight;
- defending a weaker NPC;
- controlling an aggressive drunk without hospitalizing them;
- disarming someone;
- preventing passage or protecting a location;
- choosing restraint when stronger force is possible;
- deliberately escalating when the player accepts consequences.

The important gameplay is not only winning. It is **how the player chooses to use superior capability** and how witnesses/participants interpret that choice.

### 3.7 Mayoral / civic intervention

Candidate actions:

- authorize or deny use of municipal space;
- alter an opening/service window through legitimate policy/process;
- call a meeting;
- prioritize a repair or service;
- mediate a dispute;
- grant/deny a bounded permission;
- make a promise with later accountability;
- inspect a problem personally;
- ask an employee/role-holder to perform something within their role rather than mind-control them;
- publicly support or oppose a proposal.

The resulting social/material changes must flow through normal schedules, opportunities, beliefs and relationships where appropriate.

---

## 4. Anti-game-ending conflict model to research

The PA must explicitly investigate how ordinary misconduct and physical conflict create gameplay **after** the incident rather than merely ending or resetting it.

Potential consequence channels to compare:

- participant fear/anger/respect/gratitude as accepted relationship inputs;
- witness beliefs that may be incomplete or wrong;
- gossip/reporting;
- being asked to leave a venue;
- temporary service refusal;
- apology/restitution opportunity;
- medical/injury consequence if severity warrants it;
- obligation/favour/debt;
- Guardia Civil/local-authority warning or intervention;
- fine, questioning or temporary detention if product design benefits;
- mayoral conflict-of-interest/reputation consequences;
- later confrontation or reconciliation;
- schedule/work disruption;
- authored story reacting to existing systemic state rather than overwriting it.

Research must reject both extremes:

1. **No consequences:** kung-fu sandbox where attacking people is trivial.
2. **Binary apocalypse:** any physical misconduct turns the rest of the game into permanent police combat or forces reload.

The interesting design space is proportional, contextual and recoverable where appropriate.

---

## 5. Added acceptance scenarios

### A11B-13 — Non-dialogue social causality

```text
GIVEN the player has not opened a dialogue interface
WHEN the player performs a supported social/embodied action toward or near an NPC
THEN that action can be perceived and interpreted through accepted context
AND may affect later behaviour/opportunity without requiring a bespoke quest trigger
AND a non-witness does not gain magical knowledge.
```

### A11B-14 — Follow/accompany is systemic

```text
GIVEN player and Antonio have a context that permits accompaniment
WHEN one invites the other to follow and the invitation is accepted
THEN they can move together toward a bounded purpose
AND the shared movement consumes real time/opportunity
AND can end/replan from schedule, danger, destination or relationship state
AND is not implemented only as a mission escort flag.
```

### A11B-15 — Gesture meaning depends on context

```text
GIVEN the same lightweight signal is performed toward two actors or in two contexts
WHEN relationship/role/current situation differs materially
THEN interpretation or response may differ for those reasons
AND the gesture MUST NOT map globally to a fixed relationship delta.
```

### A11B-16 — Controlled force vs serious assault

```text
GIVEN the player is capable of overwhelming physical force
WHEN two scenarios differ in consent, threat, severity, injury, witnesses or defensive context
THEN the Living World can represent materially different aftermath
AND MUST NOT collapse both to the same universal combat/crime outcome.
```

### A11B-17 — Restraint creates gameplay

```text
GIVEN player can resolve a physical confrontation through at least two accepted levels of force
WHEN the player chooses a lower-force successful resolution
THEN participant/witness/material aftermath may differ from a severe resolution
AND the difference can matter later through normal social/world systems.
```

### A11B-18 — Intervention without ownership theft

```text
GIVEN two NPCs are in a conflict or blocked situation
WHEN the player intervenes physically or socially
THEN the intervention may change opportunities/outcomes
BUT the player does not directly set each NPC's belief, relationship or future action
AND NPCs retain their own decision ownership after the intervention.
```

### A11B-19 — Mayor changes rules, not minds

```text
GIVEN the player uses legitimate mayoral authority to change a municipal rule, access condition or service opportunity
WHEN affected actors encounter the new condition
THEN they react through normal schedule/agency/social systems
AND the policy/action MUST NOT directly command private beliefs or relationships.
```

### A11B-20 — Embodied free-play session

```text
GIVEN no main-story advancement, no full combat encounter and no mandatory dialogue chain
WHEN player uses several supported embodied/social affordances during a bounded session
THEN the session can still alter later NPC behaviour, access, relationships, opportunities or events
AND at least one consequence persists after leaving the immediate scene.
```

### A11B-21 — No prop-sandbox false positive

```text
GIVEN the demo permits many cosmetic physics interactions
WHEN none of them meaningfully affect people, opportunities or causal world state
THEN those interactions MUST NOT satisfy PA-11B's embodied-agency requirement by quantity alone.
```

---

## 6. Added failure modes

| ID | Failure mode | Signal | Risk |
|---|---|---|---|
| F11B-16 | Talk-or-hit design | Almost every meaningful interaction is dialogue or combat. | Living World remains narrow despite strong AI. |
| F11B-17 | Emote toybox | Gestures exist but NPC interpretation is fixed/cosmetic. | Fable-like surface without deeper causality. |
| F11B-18 | Escort-script follow | Following only exists in authored missions. | Useful social affordance fails to generalize. |
| F11B-19 | Kung-fu apocalypse | Any physical intervention becomes maximum crime/combat state. | Martial identity discourages experimentation and free play. |
| F11B-20 | Kung-fu impunity | Physical dominance has no meaningful social/material consequence. | World loses credibility. |
| F11B-21 | Binary violence | Only attack or abstain; no restraint/intervention spectrum. | Mastery has less agency than it should. |
| F11B-22 | Mayor mind control | Office lets player directly set citizen behaviour/opinion. | Institutional gameplay bypasses Living World. |
| F11B-23 | Everything is grabbable | Huge object interaction surface with negligible causal value. | Expensive immersive-sim cosplay, low gameplay yield. |
| F11B-24 | NPC passivity | NPCs never initiate compatible social/embodied interactions. | Town still stages itself around the player. |
| F11B-25 | Context blindness | Same act always has same social meaning. | Relationships, roles and situations become decorative. |

---

## 7. Research comparison targets

The study should use concrete questions rather than feature imitation.

Useful comparison families include:

- immersive sims for reusable verbs, object/world affordances and consequences;
- `Fable` for lightweight gestures/social signalling, specifically asking what a deeper stateful interpretation layer would add;
- `Shenmue` / `Shenmue II` for follow/accompany, mundane embodiment, spatial routine and activity texture, without importing its content architecture;
- social RPGs for trespass, ownership, service refusal, witnesses and recoverable consequences;
- games with non-lethal or graded confrontation for how restraint becomes meaningful rather than merely a score bonus;
- systemic sandboxes for distinguishing high-value affordances from “interact with everything” noise.

The research question is never “which game's feature do we copy?” It is “which reusable causal primitive earns its complexity in Juego2?”

---

## 8. Amendment impact on the parent PA

The following parent sections are strengthened:

- **Exact research question:** “player actions” explicitly includes verbal, embodied, material, social, institutional and graded physical actions.
- **PCA-01/PCA-02:** equivalent player/NPC semantics apply beyond speech and combat.
- **Research axes:** embodied/social signalling and shared-space intervention become first-class axes.
- **A11B-8 / A11B-12:** a low-stakes session and anti-museum gate cannot PASS solely through dialogue choices plus minigames.
- **Research method:** candidate primitives must include non-verbal and physical/social affordances.
- **Reviewer attack surface:** Reviewer must explicitly attempt to reduce the game to `talk / quest / fight` and FAIL the plan if embodied agency is merely cosmetic.

Parent stop condition 8 is therefore interpreted as requiring a no-story/no-combat session with **mixed interaction modalities**, not dialogue-only world perturbation.

---

## 9. Reviewer attack surface added by this amendment

Reviewer should actively try to prove that:

- every meaningful social action still opens a dialogue box;
- gestures are cosmetic animations with fixed responses;
- follow/accompany exists only for missions;
- NPCs cannot invite, beckon, accompany or physically help without a script;
- kung-fu mastery only expands lethal/hostile choices;
- any punch creates an unrecoverable wanted-state spiral;
- alternatively, violence has no credible consequence;
- self-defence, intervention, sparring and assault are semantically identical;
- mayoral authority bypasses citizen agency;
- object interactivity is being counted instead of causal affordances;
- non-dialogue actions cannot seed delayed consequences;
- a player can only meaningfully alter the town through dialogue, quests or combat.

**Strong negative question:**

> Can the player walk out of the house, never start a quest, never open a full dialogue branch and never enter a full combat encounter, yet still do several understandable things in the town that other people notice, respond to and remember?

If the truthful answer is “not really,” embodied player agency is not solved.

---

## Freeze rule

**AMENDMENT STATUS: FROZEN**

This amendment extends the PA-11B preregistered surface. Findings may reject individual candidate verbs or conclude that fewer affordances are needed, but they may not silently weaken the requirement that meaningful player agency span dialogue **and** embodied action.