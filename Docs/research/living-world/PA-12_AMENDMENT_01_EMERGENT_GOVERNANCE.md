# PA-12 AMENDMENT 01 — EMERGENT GOVERNANCE, MACRO↔MICRO PLAY & INTENTIONAL TRANSFORMATION

Amendment status: **FROZEN**  
Canonical parent: **PA-12 — Governance as Intervention in Simulation**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> **Product thesis:** the player should not choose a labelled type of town from a strategy screen. The player should be able to **turn the town into that kind of place by governing and acting inside it**, while citizens retain their own beliefs, interests, relationships and decisions.
>
> Governance changes rules, resources and opportunities. It does not directly change minds.

This amendment strengthens PA-12. It does not implement politics, prescribe an ideology, reopen H0, or commit Juego2 to a full municipal/economic simulator.

---

## 0. Why this amendment exists

PA-12 already owns the question of how mayoral intervention changes normal simulation rather than applying abstract buffs. The wider Living World programme now adds two requirements that make the governance question more ambitious:

1. the player is a first-class causal actor (PA-09), capable of changing the town through ordinary embodied/social interaction as well as dialogue;
2. the town should be stable by default but capable of **meaningful deliberate transformation**, including severe destabilisation, when the player repeatedly chooses to push it that way.

Without an explicit governance model, Juego2 could still fall into one of two weak designs:

```text
A) mayoral strategy island
   menu/overhead layer -> percentages change -> third-person town barely matters

B) purely local mayor fantasy
   player talks to people -> occasional quest flags -> office has no systemic leverage
```

The intended design is a loop:

```text
institutional decision
        ↓
rule / resource / access / service / schedule / capacity changes
        ↓
NPCs encounter changed conditions
        ↓
NPCs perceive, interpret, support, resist, exploit, adapt or ignore
        ↓
relationships / beliefs / routines / opportunities / events evolve through their owners
        ↓
player experiences consequences in the third-person town
        ↓
player responds through further macro decisions AND ordinary micro action
        ↓
new equilibrium, reform, backlash, escalation or deliberate destabilisation
```

The mayoral role therefore becomes a high-leverage interface to the same Living World, not a second game sitting above it.

---

## 1. Exact amendment question

> **What minimal set of municipal levers and institutional interactions lets the player deliberately reshape the town at high leverage, while keeping the consequences inside the same causal Living World and preserving independent NPC agency?**

A secondary question is equally important:

> **How can the game protect a normal player from accidental systemic collapse while still allowing a player who deliberately and persistently seeks radical change or chaos to achieve it?**

Observable win:

- two players can begin from the same town and, through materially different governance + micro behaviour over time, produce recognisably different social/institutional equilibria;
- those outcomes are explainable from concrete rules, opportunities, resources, social reactions and player actions rather than a hidden ideology-state machine;
- the town remains functional under ordinary play but can be substantially transformed or destabilised through sustained intentional choices.

---

## 2. Core invariants

### GOV-01 — No ideology button

The product should not require high-level mode switches such as:

```text
CommunistTown = true
CultTown = true
AuthoritarianTown = true
ChaosNight = true
```

Terms such as "collectivist", "deregulated", "clientelist", "sect-like", "authoritarian", "communal" or "chaotic" may be useful **descriptions of an emergent result**, not semantic authorities that force citizen behaviour.

If a particular story beat requires an authored named movement or organisation, that content still acts through explicit rules, beliefs, relationships, resources and participation rather than replacing them.

### GOV-02 — Governance changes conditions, not minds

A mayoral action may change:

- access;
- permissions/prohibitions;
- public-space use;
- service availability;
- schedules/opening windows;
- capacity;
- municipal priorities;
- funding/resource allocation;
- fees/costs where useful;
- staffing/opportunities;
- public events;
- obligations;
- enforcement posture within product scope;
- ownership/use arrangements where explicitly modelled.

It MUST NOT directly set private belief, trust, fear, loyalty, friendship or future action.

NPC owners consume the new conditions and decide what they mean.

### GOV-03 — Macro and micro are one causal loop

A municipal decision must be capable of producing consequences that the player encounters in normal third-person play.

Likewise, micro behaviour can alter the success, legitimacy, interpretation or downstream consequences of macro policy.

Examples:

- change closing rules -> venue adapts/resists -> player encounters affected people -> player mediates or escalates;
- fund repair -> workers/resources/schedules change -> player can personally inspect/help/obstruct;
- grant a group access to a public space -> others react based on their own interests/knowledge -> later meeting/activity/event becomes possible;
- make a public promise -> later failure/success becomes social evidence.

### GOV-04 — No dedicated strategy layer by default

The default interaction hypothesis is:

- conversations with an interventor/secretary/official;
- meetings;
- documents/desk interactions;
- compact diegetic or conventional menus where needed for clarity;
- direct inspection and follow-up in the third-person town.

A separate overhead/city-management view is **not a baseline requirement**.

It may only be introduced later if a concrete research/usability need proves that the mayor cannot understand or execute an accepted governance task through a simpler interface.

This is an anti-duplication/default rule, not a permanent ban on camera/UI experimentation.

### GOV-05 — High leverage is allowed

Mayoral actions are intentionally capable of affecting many citizens faster than ordinary micro interactions.

That is a feature, not a violation of first-class causal agency, provided that:

- the action uses legitimate institutional authority;
- the changed rule/resource/opportunity is explicit;
- affected actors remain decision owners;
- consequences remain inspectable and bounded enough to understand.

The mayor should feel meaningfully different from an ordinary citizen without becoming a mind-controlling player character.

### GOV-06 — Deliberate transformation must be possible

The simulation must not silently restore the same social order no matter what the player does.

Sustained, coherent player choices may create a materially different town equilibrium.

Examples to research, not shipping promises:

- unusually communal/shared provision;
- extensive market/private latitude;
- clientelist favour networks;
- highly regulated public life;
- strong local mutual-aid structures;
- a personality/organisation acquiring sect-like social influence;
- concentrated mayoral authority;
- weakened institutions and widespread opportunism;
- recurring collective festivities/associations dominating town rhythms.

These outcomes should emerge from composable systems and actor responses, not bespoke end-state scripts.

### GOV-07 — Normality is an attractor, not a prison

Under ordinary play, minor mistakes, isolated disputes and low-severity rule changes should have recovery paths.

The simulation should naturally support:

- correction;
- apology/restitution;
- adaptation;
- replacement;
- reopening;
- rescheduling;
- de-escalation;
- forgetting/compaction;
- institutional repair.

But repeated intentional pressure may overcome those stabilisers.

### GOV-08 — Protect against accidental collapse, not intentional consequences

> **The simulation protects the player from accidental collapse, not from intentional consequences.**

A player should not destroy a long campaign through a trivial misclick or one ambiguous physical interaction.

Conversely, a player who repeatedly chooses severe, destabilising actions after clear feedback should not be stopped by invisible safeguards merely to preserve the default town.

Research should distinguish:

```text
accident / misunderstanding
 -> easy correction / limited propagation

deliberate transgression
 -> meaningful social/material/institutional consequence

sustained destabilisation campaign
 -> systemic transformation / severe consequences / new equilibrium
```

### GOV-09 — Consequences remain gameplay

Except for genuinely necessary authored fail states, severe governance failure or deliberate chaos should prefer **changed play conditions** over arbitrary campaign termination.

Possible consequences include:

- services becoming unavailable;
- actors refusing cooperation;
- officials challenging/resigning/withdrawing support where represented;
- Guardia Civil / legal or administrative intervention within product scope;
- businesses changing behaviour;
- people organising in support/opposition;
- altered access/availability;
- displacement of routines;
- social fragmentation or new alliances;
- later reconciliation/reconstruction opportunities.

The player may create a worse town and continue playing in it.

### GOV-10 — Citizens retain exit, resistance and opportunism

Municipal authority should not imply universal compliance.

Where product scope supports it, actors may:

- comply;
- oppose;
- petition;
- seek exceptions;
- evade;
- exploit loopholes;
- leave an activity/place;
- organise with others;
- help implementation;
- adapt routines;
- reinterpret information;
- pursue self-interest under the new rule.

Which response occurs remains an agency/relationship/belief/opportunity question.

### GOV-11 — Positive transformation is as important as collapse

The same systemic freedom that allows the player to destabilise the town should support constructive transformations:

- stronger community activities;
- services that work better;
- new cooperation;
- repair of conflicts;
- easier access;
- successful collective projects;
- thriving venues;
- stronger associations;
- reciprocal favour networks that remain bounded and legible.

Living World freedom must not become a destruction simulator by default.

### GOV-12 — Narrative can react; it must not erase

Authored story may react strongly to the town the player has produced.

It should avoid silently resetting systemic state merely to restore the expected script.

When story constraints require limits, GameFlow should block/defer/substitute/replan explicitly and preserve relevant causal history according to accepted owners.

---

## 3. Municipal primitive families to research

These are research categories, **not promised UI buttons or APIs**.

### 3.1 Access and permission

- allow/deny use of public space;
- permits;
- opening/closure conditions;
- temporary restrictions;
- exemptions where justified;
- priority access.

### 3.2 Time and schedule

- service hours;
- venue/event windows;
- municipal staffing;
- market/festival timing;
- temporary schedule changes.

### 3.3 Capacity and allocation

- limited public resource allocation;
- prioritised repair/service;
- activity/event capacity;
- municipal use of places/resources;
- scarce appointment/slot distribution where useful.

### 3.4 Cost, subsidy and obligation

Only where behaviourally useful:

- fee/tax/charge abstraction;
- subsidy/support;
- compensation;
- municipal purchase/provision;
- obligation/duty;
- fine/restitution if accepted by later legal design.

Avoid hidden macroeconomics for its own sake.

### 3.5 Institutional sponsorship

- recognise/support a club/activity/event;
- provide venue/time/resource;
- call a meeting;
- launch a public project;
- publicly endorse/oppose a proposal.

This can interact strongly with PA-08 activities and PA-03..06 social systems without granting direct control over them.

### 3.6 Enforcement posture

Research only to the depth needed for gameplay:

- warning/education;
- selective enforcement risk and its fairness/legibility problems;
- temporary closure/removal;
- referral to another authority;
- consequences for mayoral abuse of authority.

Do not build a complete legal simulator unless future evidence justifies it.

### 3.7 Public commitment

Promises are valuable because they connect macro and micro play:

```text
player publicly commits
 -> actors may hear/believe/remember
 -> resources/opportunities later permit or block fulfilment
 -> fulfilment/failure becomes evidence
 -> relationships and future cooperation may change
```

The promise itself does not directly edit loyalty.

---

## 4. Emergent political/social configurations

PA-12 should explicitly research whether a small set of institutional primitives plus Living World owners can support **recognisably different town trajectories without ideology-specific engines**.

The research may use provocative labels as stress-test descriptions, for example:

- "the town has become highly collectivised";
- "the mayor has built a patronage network";
- "a social group now behaves almost like a sect";
- "public order has been deliberately hollowed out";
- "the town has become heavily regulated";
- "the municipality barely intervenes and private actors fill the gaps".

For every such description, the study must answer:

1. What concrete rules/resources/opportunities changed?
2. Which actors supported/resisted/adapted and why?
3. What information did they actually possess?
4. What persisted because of memory/relationship/material owners?
5. What could reverse the trajectory?
6. Could the same primitives produce a different outcome under different actor state?

If the answer reduces to a global ideology/reputation flag, the design has failed the research question.

---

## 5. Macro↔micro interface research

### 5.1 Office/interventor loop

Candidate default:

```text
player enters office / meets interventor or relevant official
 -> sees pending decisions/problems/options
 -> requests explanation / projected direct effects
 -> makes one or more legitimate decisions
 -> time/state advances as appropriate
 -> player returns to town
 -> implementation and reaction occur through Living World
```

The official is an interface/presentation role, not necessarily semantic authority.

### 5.2 Explainability before decision

Where useful, the player should understand direct intended effects:

- who/what is covered;
- start/end conditions;
- cost/resource implication if represented;
- known capacity/service changes;
- obvious conflicts.

The game should **not** promise exact downstream NPC reactions. Those are part of the Living World.

### 5.3 Physical follow-up

Some macro choices should create opportunities to:

- inspect a site;
- attend a meeting;
- talk to affected citizens;
- help implementation;
- mediate resistance;
- enforce or waive a decision within authority;
- see unintended consequences;
- reverse/amend policy.

This is where mayor gameplay reconnects to third-person play.

### 5.4 No spreadsheet requirement

A systemic town does not require the player to become a spreadsheet operator.

Research should prefer a small number of legible, consequential decisions over hundreds of sliders.

Depth should come primarily from **downstream composition**, not menu density.

---

## 6. Intentional chaos vs accidental ruin

This boundary must be preregistered rather than improvised later.

### 6.1 Accidental-safety techniques to compare

- reversible first offences;
- clear contextual feedback;
- proportional consequences;
- confirmation only for genuinely high-impact/irreversible institutional choices;
- grace/restitution paths;
- delayed implementation for major rules when useful;
- visible severity/context cues;
- bounded propagation from low-salience incidents;
- autosave/recovery policy as UX, not as substitute for coherent simulation.

Avoid confirmation-dialogue spam for ordinary embodied actions.

### 6.2 Deliberate escalation

A player who keeps applying high-severity pressure should be able to exceed normal stabilisers.

Conceptually:

```text
one perturbation
 -> system absorbs/adapts

repeated coherent perturbations
 -> recovery capacity consumed
 -> actors reorganise
 -> services/routines/relationships change
 -> new equilibrium or crisis
```

PA-13 owns the final budgets/anti-chaos mechanisms. PA-12 owns the requirement that those mechanisms **must not make deliberate transformation impossible**.

### 6.3 Fire/destruction boundary

Literal large-scale destruction such as burning buildings is not automatically required by this amendment; that depends on future world/destruction scope.

The product requirement is broader and more important: if the player deliberately seeks systemic disorder, the town must have meaningful channels through which order can degrade and citizens/institutions react.

If physical destruction is later accepted, it should enter the same material/event/perception/recovery model rather than become a detached chaos minigame.

---

## 7. Acceptance scenarios preregistered

### A12-1 — Same policy, different citizens

```text
GIVEN one municipal rule changes a real opportunity/constraint
AND multiple actors differ in role, relationship, resources, knowledge or goals
WHEN they encounter the changed condition
THEN they may react differently for those reasons
AND the policy MUST NOT directly set a common private attitude.
```

### A12-2 — Macro decision becomes third-person gameplay

```text
GIVEN player adopts a municipal decision through an office/meeting/menu interface
WHEN implementation begins
THEN at least one material or social consequence becomes observable in normal town play
AND player can meaningfully respond through ordinary third-person actions
AND the consequence is not confined to a strategy dashboard.
```

### A12-3 — Micro behaviour changes macro outcome

```text
GIVEN a policy is active
WHEN player personally helps, obstructs, mediates, intimidates, persuades or otherwise intervenes through accepted actions
THEN implementation or social response may change through normal owners
WITHOUT editing a hidden policy-success meter as sole authority.
```

### A12-4 — Divergent town trajectories

```text
GIVEN two runs start from equivalent town state
WHEN players make materially different sustained governance and micro-intervention choices
THEN after a bounded long horizon the towns exhibit materially different rules/opportunities/routines/social alignments
AND the differences can be causally explained
AND no global ideology-mode switch is required.
```

### A12-5 — Emergent collectivist-style outcome without ideology flag

```text
GIVEN player repeatedly favours shared provision/access/municipal coordination through accepted levers
WHEN citizens encounter those changes
THEN the town may develop recognisably more collective patterns
BUT individual actors retain support/resistance/adaptation decisions
AND no `collectivist=true` flag directly controls private behaviour.
```

This scenario tests representation, not political endorsement.

### A12-6 — Sect-like social configuration remains actor-owned

```text
GIVEN one group/person gains repeated access, resources, visibility, social proof and willing participants
WHEN influence grows
THEN sect-like or highly cohesive behaviour may emerge only through accepted belief/relationship/agency mechanisms
AND municipal support alone MUST NOT directly create belief or loyalty.
```

### A12-7 — Accidental mistake is recoverable

```text
GIVEN player makes one low/medium-severity mistaken governance or embodied action
AND does not continue escalating
WHEN normal recovery mechanisms operate
THEN the campaign is not silently driven into irreversible systemic collapse
AND player has a legible correction/restitution/adaptation path where appropriate.
```

### A12-8 — Deliberate chaos can overcome recovery

```text
GIVEN player receives clear feedback that repeated actions are destabilising
WHEN player deliberately persists across multiple high-impact interventions
THEN systemic degradation or radical transformation can occur
AND stabilisation systems MUST NOT invisibly restore the default town merely to protect the campaign
AND the resulting state remains playable where product scope permits.
```

### A12-9 — Bad mayor is a playable state

```text
GIVEN sustained governance failure/abuse has produced severe social/material consequences
WHEN no authored hard fail is logically required
THEN player continues in the altered town
AND affected actors/institutions change cooperation, access, routines or organisation accordingly
RATHER THAN forcing an arbitrary reload/game-over solely because the town diverged from default.
```

### A12-10 — No top-down false requirement

```text
GIVEN an accepted mayoral task can be understood and executed through office/dialogue/document/compact UI plus in-world follow-up
WHEN evaluating presentation architecture
THEN a separate overhead strategy layer MUST NOT be required by default
UNLESS a documented usability/scale need demonstrates otherwise.
```

### A12-11 — High leverage without mind control

```text
GIVEN one mayoral decision affects many actors
WHEN downstream simulation runs
THEN the shared rule/opportunity may be applied broadly
BUT each private belief/relationship/action remains owned by the appropriate subsystem/actor decision
AND large scope alone is not treated as permission for direct social-state mutation.
```

### A12-12 — Policy rollback does not erase history

```text
GIVEN a policy was active long enough to produce social/material consequences
WHEN player reverses or amends it
THEN the rule may return/change
BUT memories, debts, relationships, moved resources and other legitimately persisted consequences do not vanish automatically
AND recovery becomes new gameplay rather than timeline reset.
```

---

## 8. Failure modes to attack

| ID | Failure | Signal | Why it fails |
|---|---|---|---|
| F12-1 | Ideology button | One choice directly converts town/NPC attitudes. | Fake emergence. |
| F12-2 | Spreadsheet mayor | Depth comes from dozens of sliders, not consequences. | Separate management game. |
| F12-3 | Decorative office | Decisions are dialogue flavour with no systemic effect. | Mayor role has no leverage. |
| F12-4 | Dashboard-only consequences | Numbers change but third-person town feels same. | Macro and Living World disconnected. |
| F12-5 | Omnipotent mayor | Policy sets beliefs/relationships directly. | NPC agency becomes cosmetic. |
| F12-6 | Powerless mayor | All decisions are tiny because systemic breadth is feared. | Role fantasy collapses. |
| F12-7 | Elastic default town | Simulation always returns to initial equilibrium regardless of sustained choices. | Player transformation is fake. |
| F12-8 | One-click apocalypse | Single ambiguous choice irreversibly ruins campaign. | Punishes experimentation/torpeza. |
| F12-9 | Invisible safety rails | Deliberate chaos is automatically cancelled/restored. | Freedom is fake. |
| F12-10 | Mandatory game-over politics | Severe divergence ends campaign rather than changing it. | Consequences cease being gameplay. |
| F12-11 | Universal opposition/support | All actors share reaction to policy. | Social model flattened. |
| F12-12 | Policy rollback time machine | Repeal deletes all caused history. | Breaks causality. |
| F12-13 | Strategy-view duplication | Separate overhead game becomes semantic authority. | Fractures product/architecture. |
| F12-14 | Destruction fetish | Freedom measured only by how much can be burned/broken. | Ignores richer social/institutional agency. |

---

## 9. Ownership boundaries

### PA-12 owns

- municipal lever vocabulary at product-semantic level;
- macro↔micro governance loop;
- requirement for high-leverage but non-mind-controlling mayoral intervention;
- emergent town transformation requirement;
- accidental-collapse vs deliberate-transformation product boundary as applied to governance;
- default presentation hypothesis for mayoral decisions;
- governance-specific acceptance/failure scenarios.

### PA-12 does not own

- general NPC decision algorithm (PA-02);
- relationship representation (PA-03);
- belief/knowledge representation (PA-04);
- rumour mechanics (PA-05);
- memory algorithms (PA-06);
- work/economy truth beyond governance inputs (PA-07);
- activity/minigame internals (PA-08);
- generic player embodied/social action model (PA-09);
- autonomous chain escalation/termination (PA-10);
- investigation/evidence UX (PA-11);
- final propagation/event/state budgets (PA-13);
- final integrated architecture/workpack routing (PA-14);
- full legal/criminal/political/economic simulation unless later evidence creates a separately reviewed need.

### Important PA-13 boundary

PA-13 should keep ordinary simulation stable, bounded and recoverable.

It MUST NOT interpret `anti-chaos` as:

> preserve the default town against a player who is intentionally trying to transform or destabilise it.

The target is:

```text
accidental/noisy chaos -> damped
ordinary perturbation -> recoverable
sustained deliberate pressure -> allowed to matter
```

### Important PA-14 boundary

PA-14 cannot declare Living World + Governance integration complete if:

- mayoral actions exist only in a separate strategy layer;
- macro choices do not become third-person consequences;
- micro behaviour cannot influence implementation/response;
- radically different sustained choices converge to effectively the same town;
- ideology-specific global flags substitute for actor/state owners;
- anti-chaos controls prevent deliberate systemic transformation.

---

## 10. PA-14 integrated proof additions

The eventual integration review should include at least these combined demonstrations.

### Integrated proof G1 — Govern, walk out, see it

```text
office decision
 -> explicit world rule/opportunity change
 -> player walks into normal town
 -> affected actors encounter it
 -> at least two actors react differently
 -> player can intervene locally
 -> later state reflects both macro and micro causes
```

### Integrated proof G2 — Same town, two political trajectories

Run two bounded long-horizon scenarios from equivalent initial state with different sustained governance choices.

Reviewer must be able to identify meaningful differences in:

- access/availability;
- schedules/services;
- activity ecology;
- actor cooperation/resistance;
- social grouping/relationships where legitimately caused;
- later opportunities/events;

without relying on a hidden labelled ideology mode.

### Integrated proof G3 — Normal player vs chaos-seeking player

```text
Run A: ordinary mixed play with occasional mistakes
 -> town changes but remains broadly functional/recoverable

Run B: repeated intentional destabilising interventions despite clear feedback
 -> town can materially degrade/transform
 -> consequences remain causally legible and playable
```

If A collapses too easily, stability failed.

If B cannot substantially diverge because the simulation always self-heals, agency failed.

---

## 11. Research references / comparison questions

Research should focus on mechanisms rather than copying complete political systems.

Useful comparison questions include:

- Which games let policy change actual NPC opportunities rather than only statistics?
- Which systems allow player-created social order/disorder without bespoke faction endings?
- How do systemic games communicate high-impact choices without predicting all consequences?
- How do immersive sims protect experimentation while retaining serious consequences?
- How do management games avoid forcing every player into permanent overhead/spreadsheet interaction?
- How can social-simulation actors resist/comply/opportunistically adapt without requiring a full political-science model?

Observed behaviour may guide product research. Hidden implementation claims require appropriate evidence before architectural adoption.

---

## 12. Stop conditions

PA-12 research may stop when it can answer, with evidence or explicit rejection/narrowing:

1. which municipal levers create the most Living World consequence density;
2. how macro decisions enter shared world state without direct mind control;
3. how third-person micro play affects governance aftermath;
4. whether a dedicated overhead layer is actually necessary for any accepted task;
5. how radically different town trajectories can emerge from shared primitives;
6. how accidental collapse is prevented without preventing deliberate transformation;
7. how severe governance failure remains gameplay rather than automatic reload;
8. which governance consequences must be visible/explainable;
9. where PA-13 budgets constrain propagation without preserving the default world artificially;
10. what PA-14 must prove in the integrated vertical slice/research suite.

Do not expand PA-12 into exhaustive real-world politics, complete economics, constitutional law, policing simulation, election simulation or ideology taxonomy unless a later concrete product requirement justifies a separately scoped study.

---

## Freeze rule

**AMENDMENT STATUS: FROZEN**

This amendment freezes the macro↔micro governance thesis, emergent-transformation requirement, accidental-vs-intentional collapse boundary, presentation default and acceptance attack surface.

Findings may narrow or falsify individual levers. They may not silently redefine PA-12 back into abstract policy buffs, a detached strategy game, direct NPC mind control, or an always-self-healing town that cannot be deliberately transformed.