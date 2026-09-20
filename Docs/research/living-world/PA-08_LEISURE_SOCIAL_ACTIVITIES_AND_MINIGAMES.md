# PA-07B PLAN — LEISURE, SOCIAL ACTIVITIES & MINIGAMES AS LIVING WORLD SYSTEMS

Plan status: **FROZEN**  
Study status at freeze: **NOT_STARTED**  
Owner mode: **REMOTE**  
Date: 2026-09-20  
Repository: `Arkus0/Juego2`  
Process class: **RESEARCH / PRODUCT-DESIGN INPUT — NON-BINDING UNTIL REVIEWED**

> A minigame is not automatically a Living World feature just because it happens inside the town. The research question is whether leisure and playable activities can belong to ordinary town life, attract NPC participation, consume real time/place/capacity, and produce bounded consequences that matter later.

This PA owns the **activity integration boundary**. It does not own the rules of mus, billiards, fishing, bolos or any other individual game.

---

## 0. Why this PA exists

The inherited Living City research has owners for:

- daily life and schedules;
- autonomous agency;
- relationships;
- belief/knowledge;
- rumours;
- memory;
- work/material dependencies;
- autonomous events;
- investigation;
- governance;
- simulation budgets.

Juego2 PA-11B additionally establishes the player as a first-class causal actor.

But no single PA owns this product question:

> **How do leisure, games and repeatable town activities become part of the same living city instead of isolated “press X to launch minigame” attractions?**

Without a dedicated owner, integration can fall between systems:

- schedule assumes an activity exists;
- minigame owns a private score/reward;
- relationship system never sees the outcome;
- NPCs never play without the player;
- activity ignores real time/capacity;
- nobody remembers or talks about anything;
- an interrupted activity cannot resume;
- the bar/table/river is just a stage loaded for the protagonist.

PA-07B exists to prevent that seam.

---

## 1. Exact research question

> **What minimal shared activity model lets NPCs and the player participate in leisure/social activities as part of ordinary town life, while preserving schedule, capacity, agency and consequence ownership and keeping each minigame independently fun?**

**Observable win:** at least one accepted social activity and one accepted individual/low-social activity can exist in the town without the player, can be joined or affected by the player when appropriate, consume real world time/opportunity, and emit bounded structured outcomes that at least one later Living World system may use.

A stronger product proof is:

```text
NPCs decide to play / participate
        ↓
real place + time + capacity
        ↓
player may join / refuse / watch / interrupt / affect context
        ↓
activity resolves by its own rules
        ↓
small structured outcome crosses activity boundary
        ↓
selected social/material consequences belong to existing owners
        ↓
normal town life continues
```

---

## 2. Core invariants

### ACT-01 — Activity exists independently of player activation

If an activity represents ordinary town life, eligible NPCs should be able to participate without the player where product scope requires.

A table is not a dormant UI portal waiting for Juan.

### ACT-02 — Activity rules and Living World semantics are separate owners

The mus implementation decides cards, legal plays and winner. The Living World decides what the result may mean socially/materially.

The fishing implementation decides whether/how a fish is caught. The Living World decides whether the catch becomes resource, gift, shared experience or irrelevant detail.

### ACT-03 — Real time and opportunity matter

Participation should be capable of consuming time, place, capacity and actor availability.

An NPC cannot simultaneously play cards in the bar and perform another incompatible activity elsewhere.

### ACT-04 — Participation is a decision

Where NPC autonomy is in scope, joining/refusing/leaving an activity should be capable of consuming normal decision inputs such as:

- schedule;
- relationship;
- current need/goal;
- role;
- money/resource where useful;
- invitation;
- fatigue/mood only if accepted elsewhere;
- available slot;
- current conflict/commitment.

The activity must not directly own general NPC agency.

### ACT-05 — Stakes are explicit and bounded

Potential stakes may include:

- money/resource;
- favour/debt;
- pride/status;
- relationship input;
- information opportunity;
- access/invitation;
- time;
- scarce slot/resource;
- material output.

An activity should use only the stakes that create product value. No requirement to attach every system to every minigame.

### ACT-06 — Outcome is structured, not private magic

Where an activity matters outside itself, it should expose a small accepted result/outcome rather than directly editing relationship, belief, schedule or memory internals.

Conceptual example:

```text
ActivityResult
- activity identity
- participants
- start/end/time
- place
- outcome class
- optional stakes settled
- optional notable conduct
- optional witness/visibility facts
```

This is not an API proposal. It is an ownership pattern to test.

### ACT-07 — Witness and knowledge rules still apply

The whole town does not magically know that the player won at mus, cheated, landed a large fish or lost a wager.

Knowledge must come from participation, observation, report, later evidence or another accepted channel.

### ACT-08 — Activity consequences decay into normal life

An activity may cause later consequences, but participation should usually return actors to ordinary schedules/decisions rather than spawning permanent event chains.

### ACT-09 — Interruption is a first-class scenario

Activities may be interrupted by:

- schedule deadline;
- closing time;
- player leaving;
- actor reprioritization;
- GameFlow reservation;
- conflict;
- environmental/material invalidation;
- save/load.

Each accepted activity class should declare whether it can pause, resume, forfeit, cancel or settle early.

### ACT-10 — Fun first, systemic multiplier second

A minigame must justify itself as gameplay independently of its Living World hooks.

Living World integration can turn a good activity into a richer one. It must not be used to defend an activity that is tedious or mechanically weak.

### ACT-11 — Reuse integration hooks, not identical design

Activities should share a small integration vocabulary where possible:

- availability;
- participation;
- capacity;
- reservation;
- invitation/challenge;
- stakes;
- outcome;
- interruption;
- witnesses;
- persistence.

Their internal mechanics can remain completely different.

### ACT-12 — Activities can create positive/neutral causality

Living World cannot depend on conflict for interesting consequences.

Activities are especially valuable as sources of:

- friendship;
- rivalry without violence;
- invitations;
- mentorship;
- pride;
- embarrassment;
- generosity;
- shared routine;
- favours;
- information exchange;
- recurring habits;
- community identity.

---

## 3. Hypotheses ex ante

| ID | Hypothesis | What would falsify it |
|---|---|---|
| H07B-1 | A small shared activity boundary can integrate very different minigames without creating a universal minigame framework. | Different activities require incompatible ownership/models and abstraction adds more complexity than it removes. |
| H07B-2 | NPC participation without the player materially improves town believability and creates reusable causal opportunities. | Playtests show background participation has negligible value relative to cost. |
| H07B-3 | Social activities produce higher Living World consequence density than isolated reward loops. | Integrated consequences are rarely noticed or useful despite correct implementation. |
| H07B-4 | Time/place/capacity are sufficient to connect many activities to daily life without full economic simulation. | Important activity behaviour repeatedly requires deeper resource/economy modelling. |
| H07B-5 | A structured outcome boundary can prevent each minigame from hardcoding relationship/memory/dialogue consequences. | Owners still require substantial activity-specific coupling. |
| H07B-6 | Repeated low-stakes activities can generate compelling free-play sessions without main-story progression. | Repetition becomes contentless unless heavily authored each time. |
| H07B-7 | Activity ecology provides positive/neutral causal chains that improve simulation stability and variety. | Activities mainly create noise or fail to influence later decisions in useful ways. |

---

## 4. Research axes

### 4.1 Activity identity and availability

Research:

- What makes two sessions the same activity vs distinct instance/event?
- Is availability driven by place, time, participants, equipment, rule, weather/condition or authored state?
- What minimum metadata is needed for discovery by NPC agency and player interaction?
- How are closures/unavailable equipment/full capacity represented?

Avoid creating a universal ontology larger than the activities need.

### 4.2 Participation and invitations

Research:

- self-initiated participation;
- invitation/challenge;
- partner/team requirements;
- refusal;
- waiting/queue;
- join-in-progress where appropriate;
- spectator role;
- accompaniment to the activity;
- leaving early;
- replacement/substitute participant.

Important boundary:

> The activity exposes participation opportunities. Actor agency decides whether a particular NPC takes them.

### 4.3 Capacity, place and reservation

Candidate cases:

- four seats at a mus table;
- one billiards table;
- finite bolos lane/team slots;
- limited useful fishing spots;
- bounded audience/spectator space only if gameplay-relevant;
- equipment currently in use;
- business closing time.

Research must distinguish useful contention from simulation bureaucracy.

### 4.4 Stakes and settlement

Questions:

- Can wager/favour/debt be represented through existing material/social owners?
- Should social stakes be agreed before activity or derived afterward?
- How are invalid/impossible stakes rejected?
- What happens when an actor cannot pay/fulfil?
- Does non-monetary play still create worthwhile consequences?

No hidden gambling economy is implied.

### 4.5 Outcome semantics

The PA should identify the minimum outcome vocabulary useful across activities without flattening them.

Candidate classes to test:

- participated;
- completed/cancelled/forfeited;
- win/loss/draw/rank where meaningful;
- team/partner;
- agreed stake settled/unsettled;
- exceptional conduct only if the minigame can justify it semantically (e.g. cheating accusation/evidence, quitting, helping/teaching);
- material output (e.g. catch);
- duration/time cost;
- publicly observable vs private.

Do not expose every internal score to the Living World.

### 4.6 Repetition and habit

Research:

- recurring regulars;
- habitual time/place;
- rivalry/rematch invitation;
- absence as routine anomaly;
- skill recognition only if product-visible;
- saturation so NPCs do not repeat one attractive activity forever;
- player farming consequences by repeating trivial activity.

### 4.7 Activity-generated social space

An activity can create a context for other systems:

- conversation while waiting/playing;
- overheard information;
- witness relationships;
- invitations;
- challenge/refusal;
- group formation;
- introductions;
- absence/lateness;
- post-activity celebration/friction.

The activity does not own those systems. It creates a shared situation in which they may operate.

### 4.8 Positive/neutral causal chains

Research should deliberately test chains such as:

```text
Antonio invites Juan to fish
 -> Juan accepts
 -> they spend two hours together
 -> Carmen observes they are absent from usual place
 -> Antonio later offers Juan another opportunity
 -> no conflict occurred
```

or:

```text
NPC pair plays mus without player
 -> late finish
 -> one arrives late to work
 -> coworker covers task
 -> gratitude/favour opportunity emerges
 -> town stabilises
```

These chains matter because the Living World should not need aggression to become interesting.

---

## 5. Candidate activity probes

These are comparison/prototyping probes, **not shipping commitments**.

### 5.1 Mus / cards

Why strong:

- naturally social;
- teams/partners;
- invitation/refusal;
- recurrent regulars;
- conversation context;
- modest stakes;
- rivalry/rematches;
- spectator/witness potential;
- time cost;
- easy NPC-only plausibility.

Questions:

- Can NPCs form a table autonomously?
- Does team selection consume relationships?
- Can a result matter without permanent “mus reputation” state?
- How much cheating/deception is worth modelling, if any?
- Can players join an existing social context rather than always start a fresh session?

### 5.2 Billiards / darts / table activity

Why useful:

- one scarce POI;
- challenge queue;
- spectators;
- lower team complexity;
- repeatable rivalry;
- physical place occupation.

Good test for capacity and challenge/invitation without needing broad economy.

### 5.3 Bolos / local competition

Why useful:

- Cantabrian identity;
- public/social venue;
- scheduled and spontaneous play possibilities;
- individual/team competition;
- tournaments/festivals as authored seed plus systemic participation;
- witnesses and community pride.

Research should avoid turning cultural flavour into a giant sports-management subsystem.

### 5.4 Fishing

Why useful:

- low-social or companion mode;
- location/time availability;
- limited spots;
- material output;
- local knowledge;
- routine/habit;
- quiet shared time;
- player and NPC versions need not have identical control mechanics.

Good counterexample to bar-centric social games.

### 5.5 Sparring / martial practice

Boundary with PA-11B/combat:

- PA-07B studies participation, invitation, schedule, spectators, stakes/context and aftermath;
- combat/martial system owns the actual physical rules;
- PA-11B studies player causal meaning of restraint/mastery.

Potential value:

- practice without crime;
- mentorship;
- demonstration;
- friendly rivalry;
- witnesses learning capability through legitimate observation rather than omniscient stats.

### 5.6 Ordinary-work playable tasks

Boundary with PA-07:

Some repeatable playable tasks may look like minigames but belong to work/material systems:

- unload delivery;
- prepare venue;
- repair/setup;
- market help;
- carry stock.

PA-07 owns material/work truth. PA-07B asks whether a playable interaction can enter/exit that work context cleanly and emit useful outcomes.

---

## 6. Acceptance scenarios preregistered

### A07B-1 — NPC-only activity

```text
GIVEN an accepted activity is available
AND eligible NPCs have compatible schedules/goals
WHEN player is absent
THEN NPCs can form/enter the activity where their agency selects it
AND the activity consumes real place/time/capacity
AND it is not instantiated solely by a player interaction prompt.
```

### A07B-2 — Player joins existing activity

```text
GIVEN NPCs are already participating in an accepted activity
WHEN player becomes eligible and chooses to join
THEN entry respects capacity/participation rules
AND does not reset participants/history merely to create a player-centric fresh session
AND activity outcome can include the player through the same boundary.
```

### A07B-3 — Relationship affects participation, not rules

```text
GIVEN two NPCs have different relationship state toward the player
WHEN player invites each to the same available activity
THEN invitation decisions may differ through normal agency/relationship inputs
BUT the minigame rules do not directly inspect general relationship internals to decide legal play/winner.
```

### A07B-4 — Outcome feeds later decision

```text
GIVEN an activity completes with a structured socially relevant outcome
WHEN participants resume normal life
THEN at least one later opportunity/decision/dialogue/event may differ through an accepted owner
AND no minigame-specific quest flag is required.
```

### A07B-5 — Non-witness ignorance

```text
GIVEN player wins/loses/does something notable in an activity
AND Pilar neither participates nor witnesses it
WHEN Pilar later decides
THEN she MUST NOT use that activity outcome as known fact
UNLESS an accepted information channel reached her.
```

### A07B-6 — Interruption and recovery

```text
GIVEN an activity is in progress
WHEN closing time, GameFlow reservation, danger or participant departure invalidates continuation
THEN the activity follows an explicit cancel/forfeit/pause/resume rule
AND actor/activity reservations are released coherently
AND schedules can recover/replan.
```

### A07B-7 — Capacity contention

```text
GIVEN one activity POI has limited capacity
AND more actors want to participate than slots allow
WHEN decisions resolve
THEN no actor double-occupies the same exclusive slot
AND rejected/waiting actors can select another valid action
AND contention does not create global scan/thrash.
```

### A07B-8 — Positive causal chain

```text
GIVEN a social activity completes without conflict
WHEN its accepted outcome affects later Living World state
THEN a positive/neutral chain such as invitation, gratitude, rivalry, habit or future opportunity can emerge
AND no aggression/scandal is required for systemic value.
```

### A07B-9 — No consequence explosion

```text
GIVEN many ordinary low-stakes activity sessions occur
WHEN persistence/rumour/memory policies apply
THEN most sessions do not create durable global history
AND selected salient sessions can still remain causally useful.
```

### A07B-10 — Minigame island negative test

```text
GIVEN an activity has polished mechanics and rewards
WHEN its participants, timing, availability and outcomes cannot affect or be affected by ordinary town state
THEN it MUST NOT count as an integrated Living World activity.
```

### A07B-11 — Integration does not rescue bad gameplay

```text
GIVEN an activity is not enjoyable/usable as a game in bounded player testing
WHEN Living World hooks are added
THEN systemic integration MUST NOT by itself qualify the activity for product adoption.
```

### A07B-12 — Quiet-town compatibility

```text
GIVEN several leisure activities run during an otherwise ordinary period
WHEN no strong causal trigger occurs
THEN they can enrich routines/social context without automatically spawning high-salience events
AND the town remains in a coherent low-drama state.
```

---

## 7. Failure modes to attack actively

| ID | Failure mode | Signal | Risk |
|---|---|---|---|
| F07B-1 | Minigame island | Private score/reward, no town context. | Expensive side content with no Living World leverage. |
| F07B-2 | Player activation dependency | NPCs never participate without player. | Theme-park town. |
| F07B-3 | Universal framework | One giant abstraction tries to model every activity rule. | Complexity and loss of game-specific design. |
| F07B-4 | Relationship hardcoding | Minigame directly edits social model. | Duplicate authority. |
| F07B-5 | Omniscient result | Everyone knows result instantly. | Breaks knowledge model. |
| F07B-6 | Activity spam | NPCs constantly play because activity scores too highly. | Routine collapse. |
| F07B-7 | Event inflation | Every match/catch becomes persistent event/rumour. | State and narrative noise. |
| F07B-8 | Reservation leak | Interruptions strand table/slot/actor. | Deadlocks. |
| F07B-9 | Fake stakes | Money/favour system added only to make minigame seem systemic. | Scope inflation. |
| F07B-10 | Content treadmill | Every repeat requires bespoke dialogue/quest content. | Poor scalability. |
| F07B-11 | No fun | Simulation hooks hide weak core gameplay. | Bad minigame with expensive integration. |
| F07B-12 | Leisure monoculture | One activity dominates all free time. | Identical NPCs and repetitive town. |
| F07B-13 | Player always protagonist | NPC activity reorganises around player arrival. | World waits for player. |
| F07B-14 | Irrecoverable interruption | Leaving/canceling breaks schedules or state. | Fragility. |
| F07B-15 | Conflict-only payoff | Activity only matters when cheating/fighting occurs. | Missed positive/neutral causality. |

---

## 8. Ownership boundaries

### PA-07B may decide/recommend

- minimum shared activity integration model;
- activity availability/participation/capacity/stakes/outcome concepts;
- NPC-only participation requirements;
- player join/leave/interruption boundary;
- integration profile template;
- anti-island gates;
- which activity probes best exercise Living World seams;
- candidate structured output surface from activities.

### PA-07B may not decide

- exact schedule architecture (PA-01 lineage);
- NPC decision algorithm (PA-02);
- relationship representation (PA-03);
- belief/provenance schema (PA-04);
- rumour propagation (PA-05);
- memory compaction (PA-06);
- work/material truth (PA-07);
- autonomous event escalation (PA-08);
- investigation UX (PA-09);
- governance policy model (PA-10);
- global budgets (PA-11);
- player causal interaction repertoire (PA-11B);
- combat mechanics;
- internal rules of each minigame;
- final Unity UI/control presentation.

### Neighbor composition

| Neighbor | PA-07B provides | Neighbor owns |
|---|---|---|
| PA-01 | activity availability/participation requirements | schedule/travel/routine |
| PA-02 | opportunities/invitations/results | actor decision |
| PA-03 | participant/result facts | relationship update semantics |
| PA-04 | observable activity facts/provenance | belief state |
| PA-05 | tellable outcomes/context | propagation decision |
| PA-06 | candidate salient experiences | retention/compaction |
| PA-07 | playable participation boundary | work/material state |
| PA-08 | activity-created opportunity/friction | chain escalation/termination |
| PA-09 | potential evidence/testimony/routine anomaly | reconstructibility/trace policy |
| PA-10 | civic event/activity integration surface | policy/governance |
| PA-11 | load/retention/activity spam observations | budgets/stability |
| PA-11B | integrated activity as player action channel | player causal agency |

---

## 9. Research references and questions

This PA should research **mechanisms**, not copy feature lists.

Candidate families:

- `Shenmue` — mundane playable activities embedded in time/place; distinguish atmosphere from actual systemic integration;
- `Yakuza / Like a Dragon` — strong minigame quality and social/content integration as contrast, while checking which consequences are authored rather than systemic;
- `The Sims` — autonomous leisure selection, social objects and participation;
- life/social simulation games with shared activity objects;
- immersive sims where reusable world interactions create social/material consequences;
- sports/social games only where they answer participation, scheduling, witness or stakes questions;
- real Cantabrian social/leisure practices for cultural fit, researched separately from implementation claims.

Any claim about hidden implementation requires primary evidence before architecture adoption.

---

## 10. Activity integration profile

Every candidate activity evaluated by this PA should fill this profile:

```text
Name:
Why fun as a game:
Town role / fantasy:
Location(s):
Availability:
NPC autonomous participation: YES / NO / CONDITIONAL
Initiation modes:
Participant requirements:
Capacity / reservation:
Typical duration / time cost:
Player join/leave:
Spectator role:
Stakes:
Structured outcome crossing boundary:
Possible witness facts:
Possible material consequence:
Possible relationship input:
Possible belief/rumour input:
Possible memory candidate:
Schedule consequence:
Interruption rule:
Save/load rule:
What stays minigame-local:
Likely spam/state risk:
Reuse with other activities:
KEEP / ADAPT / EXPERIMENT / REJECT / LATER:
```

---

## 11. Expected project deltas

Accepted PA-07B findings may later justify deltas in:

- Living World activity/POI contracts;
- schedule/availability integration;
- reservation/capacity surface;
- player interaction discovery;
- NPC activity opportunity discovery;
- structured outcome contracts;
- scenario runner fixtures;
- save/runtime activity state;
- future content/minigame workpacks;
- vertical-slice acceptance;
- production authoring schema for activities.

No destination is authorized merely because it is listed here.

---

## 12. Stop conditions

PA-07B stops when:

1. A07B-1..12 are answered or explicitly narrowed/rejected;
2. one social/team activity and one contrasting activity have complete integration profiles;
3. NPC-only operation is proven or explicitly rejected with product reason;
4. participation/time/capacity/interruption ownership is unambiguous;
5. structured outcome boundary is small enough to avoid a universal minigame engine;
6. relationship/belief/memory/schedule owners are not duplicated;
7. repeated low-stakes use remains bounded;
8. at least one positive/neutral causal chain is demonstrated;
9. integration cannot hide a mechanically bad activity;
10. remaining work is game-specific implementation/content rather than a missing cross-system model.

Do not continue into designing every minigame, full sports AI, broad gambling economy, exhaustive festival system or complete cultural-content catalogue.

---

## 13. Reviewer attack surface

Reviewer should try to prove that:

- NPCs only play when the player arrives;
- activity state duplicates schedule/reservation authority;
- results directly mutate relationships/memory;
- the whole town magically knows outcomes;
- every match becomes an event/rumour;
- one attractive activity consumes every NPC's free time;
- interruptions leak reservations;
- minigame framework is overgeneralized;
- player joining resets an existing NPC context;
- stakes add systems with no visible product value;
- activity is not fun without Living World hooks;
- integration requires bespoke quest scripts;
- positive/neutral play has no downstream value;
- activity-rich periods automatically become high-drama event spam.

Strongest negative question:

> **If Juan never approaches this activity, does it still belong to the town; and if all Living World hooks are removed, is it still worth playing?**

Both answers should normally be defensible for a shipping integrated minigame.

---

## 14. Timing

PA-07B is **REMOTE-capable** research.

Recommended position:

- after enough PA-01/02/03 understanding exists to reason about schedule/agency/relationships;
- after or alongside PA-07 material/work research for resource/activity boundaries;
- before PA-08/11 final event/stability conclusions, because activity ecology is an important source of both positive causality and spam risk;
- before PA-12 integration review;
- early enough to shape which minigames are worth prototyping locally in later Unity milestones.

This PA does not block current H0/H1 work simply by existing.

---

## Freeze rule

**PLAN STATUS: FROZEN**

Findings may conclude that only a few activities deserve deep integration, that some should remain lightweight, or that NPC-only simulation is not worth the cost for certain activities.

They may not silently weaken the core product boundary into:

> “the minigame launches in town, therefore it is Living World integrated.”

The target is fewer, stronger activities that participate in one coherent living city.