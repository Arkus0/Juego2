# Recovered research — testable material from the reference archive

Version: 0.1 — 2026-09-20

Status: **NON-BINDING reference.** This file creates no acceptance criterion, reopens no accepted H0 guarantee, alters no workpack contract and commits the roadmap to nothing. It is transcription with citation, organised for use.

## 0. Scope and standing

`Arkus0/Juego` remains accessible, so this file does not try to duplicate it. It carries the part that is **expensive to rebuild and cheap to lose**: preregistered scenarios, named failure modes, reference fixtures and instrumentation budgets. Prose, argument and prior-art surveys stay in the archive and are cited by path.

What was recovered, and what it cost the original research to produce:

| | Count | Source |
|---|---|---|
| Named acceptance scenarios | **31** | 7 in `PA-01`, 10 in `PA-03`, 6 in `PA-04`, 8 in `PA-05` |
| Named failure modes | **62** | 12 + 16 + 14 + 8 + 12 across the same five tracks |
| Preregistered scenarios from unstarted research | **6** | `PA-09`, transcribed because it is the counterpart of blueprint §6.3 |
| Reference fixtures | 3 | Antonio's 24-hour routine with 6 perturbations; the social-action vocabulary; the recurring cast |
| Budget dimensions | 10 | `PA-02` |

### No verdict is inherited

`PA-02`, `PA-03` and `PA-04` passed independent review — under **the archive's own proof standard, bound to the archive's SHAs**. Juego2 has its own `FOUNDATIONAL_PROOF_STANDARD.md` and `EXECUTION_RECEIPT_PROTOCOL.md`, and `AGENTS.md:66` forbids building on a predecessor claim that was never accepted here.

Transcribing a scenario is not importing its PASS. Everything below is **cited design input**. A future workpack that adopts one of these scenarios owns proving it, from scratch, under Juego2's standard.

### How to use it

A scenario here is a candidate PASS criterion, not a requirement. A failure mode here is a thing to watch for, with the signal that makes it observable. Neither obliges anyone to build the system it describes.

Identifiers are preserved exactly as the archive wrote them — including its inconsistent styles (`F01`, `F1`, `F04-1`, `F05-1`) — so any line can be checked against the source rather than trusted here.

---

# 1. Acceptance scenarios

## 1.1 Routine and schedules

Source: `Juego/Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md`. Status in the archive: `DELTAS_READY`.

| ID | Scenario |
|---|---|
| **A** | Normal day + travel. GIVEN a baseline schedule and POI travel estimates, WHEN 24 h is simulated headlessly, THEN every required interval resolves to an activity plus semantic location/state, transitions consume non-zero modeled travel where applicable, no impossible overlap or occupancy remains, and the timeline is deterministic for the same inputs |
| **B** | Interruption crossing a boundary. GIVEN an actor is working at 13:55 and a lunch slot starts at 14:00, WHEN an interruption owns the actor from 13:58 to 14:08, THEN work cleans up or suspends safely, the current schedule and context are re-evaluated after release, the actor does **not** resume stale pre-14:00 work merely because execution was interrupted, and the trace explains the resume/skip/replan result |
| **C** | Preferred resource unavailable. GIVEN lunch resolves to a bar with finite capacity, WHEN the preferred valid slot is unavailable, THEN the resolver selects a declared fallback or an explicit failure, no double occupancy occurs, and no hidden teleport occurs |
| **D** | Calendar/context override. GIVEN a valid weekday schedule, WHEN an explicit holiday or context override applies, THEN the selected normal plan changes **without editing each base entry**, and removing the override restores the baseline deterministically |
| **E** | Expected versus actual. GIVEN repeated baseline days, WHEN tooling queries the actor at time T, THEN it can return the *expected* activity and location independently of actual current state, and a deviation can be represented without mutating historical baseline data |
| **F** | Time skip and abstract reconciliation. GIVEN the actor leaves full simulation at T1, WHEN headless time advances across multiple routine slots to T2, THEN abstract state reaches a semantically valid activity and location, re-entry does not replay every missed animation or action, and expected/actual state and trace remain coherent |
| **G** | Static validator. GIVEN authored schedule and POI data, WHEN the proposed validator runs without an engine, THEN invalid windows, disallowed overlaps and unresolved POI/affordance requirements are reported |

**E is the one worth noticing.** Expected-versus-actual is what makes an anomaly legible — it is the mechanism behind "finding the facility open and Antonio absent at 19:30 can carry meaning". Without it, a deviation is invisible.

## 1.2 Relationships

Source: `Juego/Docs/living-city-research/PA-03_SOCIAL_GRAPH.md`. Status in the archive: independent PASS, merged.

| ID | Scenario |
|---|---|
| **A1a** | Trust has independent behavioural value. One edge differing changes which target is selected for an "ask for cover" action |
| **A1b** | Affinity has independent behavioural value. The same structure for a "take a break with" action |
| **A1c** | Fear has independent behavioural value. Holding affinity and trust fixed and changing only fear flips the choice between confronting and seeking mediation |
| **A2** | Directionality and receiver ownership. GIVEN A chooses `ASK_FAVOR(B)`, WHEN B evaluates the proposal, THEN the receiver decision uses **B→A**, not A→B, and the response changes as the fixture specifies |
| **A3** | Structural role changes eligibility without changing affection. GIVEN affinity, trust and fear are identical in both runs, WHEN an official-report action is considered, THEN it is eligible in one and ineligible in the other |
| **A4** | Obligation lifecycle changes choice **once**. GIVEN one active favour owed, WHEN the creditor asks and the accepted outcome fulfils it, THEN the obligation becomes fulfilled with source and outcome trace — and the leverage is spent |
| **A5** | Sourced relation delta is explainable. GIVEN trust has a material active negative modifier sourced from event E, WHEN a trust-sensitive action is rejected, THEN inspection names current trust **and E** as a material contributor |
| **A6** | Graph locality. WHEN the same relation-backed target query runs in a base fixture and a stress fixture, THEN the decision is unchanged |
| **A7** | Private third-party edge is not a free decision input. GIVEN a canonical trust edge between two other actors exists, WHEN an actor's rule attempts to use it, THEN the query is unavailable or rejected for decision input |
| **A8** | Save and diff keep semantic identity. GIVEN directed affect, a paired structural tie and an active obligation, WHEN snapshot → serialize → load, THEN endpoint direction, roles, obligation identity and source refs are preserved |

**A1a/A1b/A1c are a set.** Three axes, three different actions, each proving the axis is not decoration. **A4** is the one that kills the eternal-favour failure: leverage is consumed.

## 1.3 Knowledge and belief

Source: `Juego/Docs/living-city-research/PA-04_KNOWLEDGE.md`. Status in the archive: independent PASS, merged. All six are marked `PASS (DESIGN PROOF)` there — a design argument, not an execution.

| ID | Scenario |
|---|---|
| **A04-1** | Belief counterfactual. Same world, same relationships, same opportunity; only epistemic state differs. The actor holding the belief is eligible for the action; the one without it fails as `UNKNOWN` and the candidate is absent. Explain output lists the held belief and its source, or `UNKNOWN` — never canonical truth |
| **A04-2** | No omniscient fallback. GIVEN a canonical fact is true and an actor has no belief, no perception, no received claim and no explicit public acquisition, THEN evaluating the condition returns `FAIL_UNKNOWN`. *Any implementation returning true by consulting the world fact fails, regardless of how the gameplay looks* |
| **A04-3** | False or stale belief. An actor perceives a shop open at T1; the fact changes at T2 while the actor is absent; at T3 the belief still reads open with `acquiredAt=T1`, and the action may remain eligible. Only privileged tooling may report the mismatch |
| **A04-4** | Provenance trace. Tooling can distinguish immediate origin, acquisition or revision time, and certainty — without needing a complete lifetime or an entire rumour chain |
| **A04-5** | Deception representable. A false claim accepted by a receiver produces a false belief whose source names the speaker; canonical truth is untouched. Whether the speaker *chose* to lie, and any onward propagation, belong to `PA-05` |
| **A04-6** | Persistence and determinism. After save, load and headless replay: same belief map, values, certainty bands, immediate provenance refs and acquisition times ⇒ same knowledge-query results |

**A04-2 is the load-bearing one.** It is the negative case that makes the whole epistemic model real rather than decorative, and it is stated as a disqualifier.

## 1.4 Rumours and information transfer

Source: `Juego/Docs/living-city-research/PA-05_RUMOURS.md`. Status in the archive: candidate, independent review never completed. Each carries `RESULT: PASS AS SPECIFICATION` — explicitly a falsifiable contract, with no runtime execution claimed.

| ID | Scenario |
|---|---|
| **A05-1** | Selective chain. One actor knows a claim; others do not; communication opportunities differ. A transfer may occur, a later **independent** decision may produce a second, an actor with no opportunity stays `UNKNOWN`, and **no transfer is created solely because a belief changed** |
| **A05-2** | No global sync. GIVEN canonical truth changes and nobody perceives it, reads a public source or receives a transfer, THEN zero beliefs change and zero transfers are synthesized from the mutation alone |
| **A05-3** | False claim coexistence. A receiver may hold a belief contradicting canonical truth; provenance names the speaker and the communication event; the receiver's decision code **cannot** consume an omniscient false/stale label or hidden causal ancestry |
| **A05-4** | Relationship/context counterfactual. Two otherwise equivalent receivers, same claim, sender, seed and opportunity, one declared input differing ⇒ outcomes may diverge, and explain attributes the divergence to that input. Guarded explicitly: this must **not** canonize "higher trust always means belief" |
| **A05-5** | Bounded loop. In a cycle, receiving never auto-generates a relay; the engine may stop a technical loop using hidden lineage, but hidden lineage **must not** reach an actor belief query or appear in actor-facing explain |
| **A05-6** | Temporal delay. With no opportunity and no independent acquisition path, an actor stays `UNKNOWN` for the whole interval |
| **POST_HOC A05-7** | Hidden common origin must not become actor knowledge. Two deliveries share a hidden root; neither speaker reports it. Engine tooling may see it; the receiver's accessible provenance is only the two speakers; **changing only hidden lineage while holding accessible inputs fixed must not change the epistemic result.** If both speakers *do* report the origin, it may then be used |
| **POST_HOC A05-8** | Witness is not rumour. Two actors independently witnessing an event create **no** transfer; only a later explicit tell, report or warn does |

**A05-7 is the sharpest thing in the whole archive.** It is a counterfactual over *engine metadata*: hold everything the actor can reach fixed, change only what the engine knows, and the answer must not move. That is a precise, testable statement of non-omniscience, and it was written as a post-hoc strengthening after the original six.

---

# 2. Failure registers

62 named failure modes. Kept verbatim in identifier and substance, because a failure mode with a detection signal is worth more than a principle.

## 2.1 Routine — `PA-01`

| ID | Failure | Guard |
|---|---|---|
| F01 | Schedule encodes paths/animations | semantic intent + activity/runtime separation |
| F02 | Template clones — many actors share an identical rhythm | archetype + at least one meaningful per-actor delta |
| F03 | Zero travel semantics | coarse travel estimator + dynamic runtime path |
| F04 | Capacity ignored | slot/resource claim + static and dynamic validation + fallback |
| F05 | Interruption resumes stale execution | cleanup + re-evaluate current schedule/context |
| F06 | No expected-versus-actual distinction | expected-routine query + actual-state trace |
| F07 | Exact off-screen simulation | abstract time/location/activity progression |
| F08 | Visible teleport reconciliation | observed/full continuity rule; reconcile only under controlled conditions |
| F09 | Routine mistaken for agency | an explicit actor-originated goal/action layer |
| F10 | Calendar conditions duplicated in every row | schedule/context override selector |
| F11 | Hyper-individualization | actor tiers + reusable activities/archetypes |
| F12 | Smart-object overmodeling | affordance only when it changes choice, contention or legibility |

## 2.2 Agency — `PA-02`

| ID | Failure | Guard |
|---|---|---|
| F01 | thrashing | commitment + explicit interrupts + hysteresis/cadence |
| F02 | omniscience | belief and authorized-known inputs only |
| F03 | social spam | initiative budgets + cooldowns + salience threshold |
| F04 | ping-pong loop | lineage + duplicate-family/depth limits |
| F05 | samey agents | a declared fixture modifier must change ranking or response |
| F06 | hidden drama | structured changes + lineage |
| F07 | story sabotage | central flow authority + defer/expiry/re-evaluate |
| F08 | hidden global scan | enumeration counters + a 10 000-decoy invariant stress proof |
| F09 | utility soup | few named reason terms + golden and counterfactual tests |
| F10 | fake agency | actor origin + the goal must not preselect the action + ≥2 eligible actions |
| F11 | resource cascade | eligibility/social/legal cost + resource authority |
| F12 | full/abstract divergence | bridge experiment; explicit preserve/resolve/cancel rule |
| F13 | replan storm | max replans + backoff + abandon outcome |
| F14 | opaque decision | normalized semantic trace with alternatives and reasons |
| F15 | fake receiver independence | explicit receiver-decision boundary owned by the receiver resolver |
| F16 | fake constitutional chain | ≥3 **material** changes; duplicate debug or log representations do not count |

**F08 and F16 are unusually good.** F08 turns "no hidden global scan" into a measurable stress test. F16 anticipates a proof being gamed by counting a log line, a debug trace and an event wrapper for one mutation as three changes.

## 2.3 Relationships — `PA-03`

| ID | Failure | Guard |
|---|---|---|
| F1 | Friendship super-score | separate affect, structural ties and obligations |
| F2 | Implicit symmetry — writing A→B mutates B→A | directed by default; explicit direction mode |
| F3 | Formal-role affection leak | structural ties separate from affect |
| F4 | Eternal favour — debt stored without lifecycle | first-class sparse obligation state |
| F5 | Threshold cliff | thresholds only where semantic; continuous ranking; hysteresis for labels |
| F6 | Universal trust sign — every action adds `+trust` | action-specific named influences |
| F7 | Infinite social history | bounded active modifiers + source refs |
| F8 | Global graph traversal | indexed local query providers + caps |
| F9 | Omniscient relationship read | access boundary through belief and perception |
| F10 | Initiator-owned response | receiver uses receiver-owned edge and state |
| F11 | Dialogue-only relationship | mandatory behavioural counterfactual |
| F12 | Dimension inflation | add a field only with an independent acceptance scenario |
| F13 | Hidden coefficient soup | small named reasons + rejected alternatives |
| F14 | Popularity as graph substitute | global reputation, if ever needed, stays a distinct input |

## 2.4 Knowledge — `PA-04`

| ID | Failure | Status and guard |
|---|---|---|
| F04-1 | Omniscient fallback | `BLOCKED BY CONTRACT` — a missing belief means `UNKNOWN`; a world-fact query is not a fallback path |
| F04-2 | Global truth sync | `BLOCKED BY CONTRACT` — a truth mutation emits no automatic belief rewrite; revision needs an explicit acquisition cause |
| F04-3 | Flags duplicated | `BLOCKED BY DELTA` — dialogue and decisions use a common knowledge query; quest flags cannot duplicate epistemic truth |
| F04-4 | Confidence theatre | `BLOCKED CONDITIONALLY` — certainty must affect eligibility, scoring or revision and appear in explain; otherwise omit it |
| F04-5 | Provenance explosion | `BLOCKED BY BOUNDARY` — immediate source, time and ref only |
| F04-6 | Deception only textual | `BLOCKED BY CONTRACT` — an accepted false claim creates a false belief with source provenance |
| F04-7 | Inference oracle | `BLOCKED BY CONTRACT` — named deterministic local rules only; no implicit truth read, general reasoner or model authority |
| F04-8 | Knowledge-graph global scan | `BLOCKED BY CONTRACT` — actor and proposition indexed queries |

## 2.5 Rumours — `PA-05`

| ID | Failure | Guard |
|---|---|---|
| F05-1 | Global broadcast | no graph-wide fanout; one explicit sender→receiver transfer per communication edge |
| F05-2 | Rumour-as-truth | transfer carries an assertion only; only the owning world system changes truth |
| F05-3 | Infinite echo | receiving never auto-relays; repeat/lineage guard + configurable hop and time budget |
| F05-4 | Gossip without motive | intent and willingness are agency-owned; bounded candidate topics |
| F05-5 | Telepathic delivery | explicit communication opportunity and channel with time and participants |
| F05-6 | Provenance loss | persist actor-accessible provenance when acquired; keep privileged engine lineage separately |
| F05-7 | Provenance explosion | bounded event history; belief stores only legitimately acquired provenance |
| F05-8 | Privileged-provenance leak | hard query boundary — actor revision sees actor-accessible provenance only |
| F05-9 | Rumour owns memory | transfers are causal transport; long-term memory is a separate question |
| F05-10 | Sender owns receiver result | separate the transfer from the receiver-side acquisition outcome |
| F05-11 | Hidden omniscient lie flag | deception intent is debug-only unless externally observable |
| F05-12 | Cascade budget as fake agency | every relay must originate in an actor decision; a budget only constrains |

---

# 3. Reference fixtures

## 3.1 Antonio's 24-hour routine

Source: `Juego/Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md`. A ready-made schedule fixture — the shape, not the content, is what transfers.

```text
06:45 HOME            WAKE_BREAKFAST
07:35 HOME->SPORTS    COMMUTE
08:00 SPORTS_CENTER   OPEN_FACILITY
08:15 SPORTS_CENTER   CLEAN / MAINTAIN
10:00 SPORTS_CENTER   FRONT_DESK_WORK
14:00 SPORTS->BAR     COMMUTE_LUNCH
14:10 BAR             EAT
14:50 BAR->SPORTS     COMMUTE
15:00 SPORTS_CENTER   FRONT_DESK_WORK
19:00 SPORTS_CENTER   CLOSE_FACILITY
19:20 SPORTS->BAR     COMMUTE_SOCIAL
19:30 BAR             SOCIALIZE
21:15 BAR->HOME       COMMUTE
21:30 HOME            DINNER / LEISURE
23:15 HOME            SLEEP
```

Note the commutes are **explicit rows**. That is what makes scenario A's "transitions consume non-zero modeled travel" checkable rather than aspirational.

Six required perturbations, quoted:

1. sports centre unexpectedly closed;
2. bar capacity full;
3. conversation interruption crossing a schedule boundary;
4. calendar holiday/context override;
5. off-screen time skip;
6. an autonomous goal overrides the normal day.

The workplace is a sports centre because the fixture predates the Potes/Liébana setting. Adapting the *place* is trivial; the value is the interval shape, the explicit commutes and the six ways it is meant to break.

## 3.2 Social-action vocabulary

Source: `Juego/Docs/living-city-research/PA-02_NPC_AGENCY.md:533-557`. Described there as "deliberately small".

```text
ASK(target, topic/problem)
TELL(target, belief/event)
REQUEST(target, request)
OFFER_HELP(target, problem)
CONFRONT(target, grievance)
REPORT(targetAuthority, event/actor)
INVITE(target, activity/place)
AVOID(target/place)

Receiver response families:
ACCEPT | REFUSE | DEFER | COUNTER
```

`WARN`, `GOSSIP`, `COMPLAIN`, `CHECK_ON`, `APOLOGIZE`, `VISIT` and `FOLLOW` are explicitly compositions until evidence requires a first-class primitive.

This is the vocabulary blueprint §6.3 opens to the player. As written in the archive it is actor-to-actor only.

## 3.3 The recurring cast

Antonio, Manolo, Carmen and Paco appear across every worked track, which is why they are usable as a shared fixture rather than as characters. The relationships the research leans on: Carmen is Manolo's sister and does not therefore share his opinions; Paco owes Carmen one favour, and helping once spends it.

---

# 4. Budgets and instrumentation

Source: `Juego/Docs/living-city-research/PA-02_NPC_AGENCY.md`. Ten dimensions, quoted:

```text
maxNewGoalsPerWindow
maxSocialInitiativesPerWindow
perActionCooldown
perTargetCooldown
maxTargetsEnumeratedPerDecision
maxCandidatesAfterCheapFilter
maxCandidatesScored
maxReplansPerGoal
maxCausalDepth / duplicate-family policy
nextEvaluationAt / dirty wakeup
```

Plus three counters the decision pipeline must expose: `targetsEnumerated`, `candidatesAfterCheapFilter`, `candidatesScored`.

Those three are what make F08 testable. Without them, "no hidden global scan" is an assertion; with them it is a measurement, and the 10 000-decoy fixture becomes a real proof rather than a claim.

---

# 5. Investigation and legibility — the other half of player agency

Source: `Juego/Docs/living-city-research/PA-09_PLAN_INVESTIGATION_LEGIBILITY.md`. Status: `NOT_STARTED`. Transcribed anyway because blueprint §6.3 designs the player **acting** and this designs the player **perceiving** — separately, each is half an answer.

Central question, quoted: *"¿Cómo puede el jugador reconstruir razonablemente un acontecimiento autónomo que no presenció usando rastros y testimonios parciales…?"*

| ID | Scenario |
|---|---|
| **A09-1** | Missed event, two independent channels. An autonomous event happens with the player absent; arriving later, the player can obtain at least two independent evidence or testimony channels, both derived from real causal state, neither requiring an omniscient quest marker |
| **A09-2** | False testimony remains possible. A witness holding a false or partial belief testifies according to its informational state and provenance, and **does not self-correct by reading canonical truth** |
| **A09-3** | Routine anomaly. The player can know a normal routine; an event alters presence or timing; comparing expectation against observation makes the anomaly a useful clue |
| **A09-4** | No single mandatory clue. If one channel becomes inaccessible, either another reasonable inference route exists or the design explicitly declares the event may stay uncertain — and no ad-hoc clue is fabricated to force a pass |
| **A09-5** | Evidence is not conclusion. Two pieces compatible with more than one explanation keep source, time and content; the inference belongs to the player |
| **A09-6** | Trace budget. Many trivial events and few relevant ones: trivial ones do not produce unbounded proportional growth, relevant ones keep enough causality to investigate |

**A09-3 depends on `PA-01`'s scenario E.** Expected-versus-actual is the mechanism; the anomaly is the payoff. They were written in different tracks and only work together.

And **A09-5 states the archive's model of player agency as cognitive** — the player's contribution is the act of inference. That is coherent, and it is exactly why blueprint §6.3 exists: inference alone leaves the world unchanged.

---

# 6. Backlog index — research never performed

19 preregistered plans. Each has ten sections including its own acceptance scenarios, so none of these is an empty folder. None is open, and opening one speculatively is the failure mode `Juego/Docs/GAME_FIRST_POLICY.md` was written to name.

## 6.1 Living city — `PA-06` … `PA-12`

| ID | Central question | File |
|---|---|---|
| `PA-06` | What must an actor keep, summarize, degrade or forget so past events change decisions? | `living-city-research/PA-06_PLAN_MEMORY_CONSEQUENCES.md` |
| `PA-07` | What minimum representation of work, services, businesses, capacity and material dependency makes labour and material change matter? | `…/PA-07_PLAN_ECONOMY_WORK.md` |
| `PA-08` | How do actor decisions, conflicts and opportunities become autonomous event chains that can escalate? | `…/PA-08_PLAN_AUTONOMOUS_EVENTS.md` |
| `PA-09` | How can the player reasonably reconstruct an autonomous event they did not witness? | `…/PA-09_PLAN_INVESTIGATION_LEGIBILITY.md` — transcribed in §5 |
| `PA-10` | *(as frozen)* How can a municipal decision change schedules, services, access, obligations or opportunities? | `…/PA-10_PLAN_GOVERNANCE.md` — see §7 |
| `PA-11` | What budgets, invariants, containment, reconciliation and degradation keep the town viable? | `…/PA-11_PLAN_SIMULATION_CONTROL.md` |
| `PA-12` | What minimum coherent set of contracts, deltas, gates and scenarios integrates the accepted findings? | `…/PA-12_PLAN_INTEGRATION_REVIEW.md` |

## 6.2 Combat — `C-PA-01` … `C-PA-12`

| ID | Central question | File |
|---|---|---|
| `C-PA-01` | What minimum contract between input, contact resolution, reaction, audio, vibration and camera makes control feel immediate and a hit feel physical without AAA graphics? | `combat-research/C-PA-01_PLAN_FEEL_IMPACT.md` |
| `C-PA-02` | How can a compact input vocabulary produce deep, predictable kung fu through timing, direction, state and context? | `…/C-PA-02_PLAN_MARTIAL_GRAMMAR.md` |
| `C-PA-03` | How do we reward timing, phrasing and continuity until the fight feels musical, without turning it into a rhythm game? | `…/C-PA-03_PLAN_RHYTHM_FLOW.md` |
| `C-PA-04` | How is one-versus-many an aggressive, legible dance with genuinely overlapping threats? | `…/C-PA-04_PLAN_CROWD_CHOREOGRAPHY.md` |
| `C-PA-05` | How is an expert hard to beat because they deny openings and steal initiative, not because they absorb damage? | `…/C-PA-05_PLAN_DUELS_OPENINGS.md` |
| `C-PA-06` | How does the town become part of the moveset without prompt hunting or a bespoke-animation explosion? | `…/C-PA-06_PLAN_ENVIRONMENTAL_COMBAT.md` |
| `C-PA-07` | What representation of entry, contact, exit, displacement and transition lets a master flow between techniques? | `…/C-PA-07_PLAN_MOTION_TRANSITIONS.md` |
| `C-PA-08` | How do framing, distance, reframing, hit stop and slow motion make good play look like martial-arts cinema without hiding information? | `…/C-PA-08_PLAN_CINEMATIC_DIRECTION.md` |
| `C-PA-09` | What minimum behavioural variety makes different encounters demand different decisions, without more health or damage? | `…/C-PA-09_PLAN_ENEMIES_ENCOUNTERS.md` |
| `C-PA-10` | If the protagonist is already a master, what progresses across 20–30 hours? | `…/C-PA-10_PLAN_MASTERY_PROGRESSION.md` |
| `C-PA-11` | How does a fight enter from a living town and return to it leaving structured, perceptible consequences? | `…/C-PA-11_PLAN_LIVING_CITY_INTEGRATION.md` |
| `C-PA-12` | What minimum set of accepted findings becomes contracts, workpacks and tests for the bus-stop encounter? | `…/C-PA-12_PLAN_INTEGRATION_BUS_STOP.md` |

Blueprint §12.8 proposes running `C-PA-01` as a prototype rather than a document. The other eleven stay shut until it answers.

---

# 7. `PA-10` re-asked, with the player as subject

The governance question lost its subject between the roadmap and the frozen plan. Both are quoted in blueprint §6.3. Restating it is not opening the track — it is repairing a drift.

**As the roadmap asked it** (`living-city-research/ROADMAP.md:410`):

> ¿Como hacemos que **ser alcalde** modifique vidas y flujos reales del pueblo en lugar de aplicar buffs abstractos?

**As the frozen plan answers it** (`PA-10_PLAN_GOVERNANCE.md:14`):

> ¿Cómo puede **una decisión municipal** cambiar horarios, servicios, acceso, obligaciones u oportunidades de varios actores…?

**The repaired question:**

> How can a governance decision **the player makes** change schedules, services, access, obligations or opportunities for several actors — through the town's ordinary systems, rather than as abstract modifiers?

What the frozen plan already gives, and should be kept unchanged: its six scenarios `A10-1` … `A10-6`, and its boundary invariant — *"governance cambia reglas y oportunidades; los actores reaccionan mediante sistemas normales. La alcaldía no controla directamente a los NPCs."* Governance changes rules and opportunities. It never puppeteers an NPC.

That invariant is what makes player governance interesting rather than god-mode: the player changes the rules, and the town's own actors decide what to do about it. `A10-6` — off-screen governance stays legible — already assumes the player may be absent while consequences unfold, which is right whether or not the player enacted the policy.

Governance is tier 2 in blueprint §6.3's inventory. This section records the corrected question so that, whenever the tier is reached, it is not re-derived from the drifted version.
