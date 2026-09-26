# WP-PA-B1 — Batch causal fixtures and acceptance matrix

Status: **WORKER EVIDENCE / REVIEW PENDING**  
Execution class: `RESEARCH_BATCH`  
Units: PA-07 + PA-08 + PA-09  
Date: 2026-09-26

## 0. Evaluation method and proof boundary

This file is the compact falsifiable evidence surface for the PA-B1 research candidate. Evaluation method is **research argument against preregistered semantic fixtures and negative controls**.

It does **not** claim:

- a Unity/runtime implementation exists;
- actual NPC decisions have been executed;
- activity mechanics are fun;
- save/load/performance budgets are proven;
- final schemas, algorithms, constants or content are selected.

Those are explicitly deferred by the three unit contracts.

A batch PASS requires:

1. PA-07's minimum work/material model can make one bounded perturbation alter at least two later actor choices through shared state;
2. PA-08's activity model exists in normal town life without requiring player activation and emits only bounded downstream-owned outcomes;
3. PA-09's player action can enter the same owners as an equivalent NPC action and produce downstream consequences after player absence;
4. all three compose without private causality systems or ownership theft.

## 1. Shared fixture vocabulary

Names are scenario labels, not shipping content commitments.

### Actors

- **Carmen** — worker associated with Bar F01 for the fixture.
- **Antonio** — ordinary service/activity user.
- **Manolo** — ordinary actor and possible activity participant.
- **Player** — first-class causal actor; no privileged semantic write access.

### Place/opportunities

- **Bar F01** — compact business/social fixture.
- **Service counter/opportunity** — finite service capacity.
- **Card table** — finite activity capacity.
- **Fishing opportunity** — low-social contrasting activity.

### Coarse material dependency

- **bar-service-stock** — deliberately coarse named dependency. It exists only because missing/restored state changes service opportunity in the fixture. It does not imply per-bottle inventory simulation.

### Normal owners inherited

- PA-01 expected routine;
- PA-02 actor-owned decisions/receiver choice;
- PA-03 relationship/obligation;
- PA-04 belief/knowledge;
- PA-05 communication;
- PA-06 selected bounded memory.

## 2. CF-B1-01 — Worker absence causes a real multi-actor consequence

### Baseline

```text
Bar F01 service window active
Carmen available for expected work
service capacity available
Antonio intends to use service before later commitment
Manolo has another bounded opportunity that may contend with Antonio's fallback
```

### Perturbation

Carmen becomes legitimately unavailable for the window.

### Required result

```text
PA-07 changes actual service capacity/state
 -> service is degraded/unavailable, not magically staffed
 -> Antonio encounters changed opportunity
 -> PA-02 chooses wait/fallback/other valid action
 -> that choice changes a second bounded opportunity OR an eligible second worker chooses whether to cover
 -> at least one additional actor's later PA-02 choice differs
```

### Pass oracle

- shared service/material state carries the cause;
- two actor choices can differ from baseline;
- no bespoke quest branch carries the chain;
- PA-07 does not choose the actors' responses.

### Fails if

- business keeps full service with no explanation;
- missing worker is instantaneously replaced invisibly;
- a work script directly assigns Antonio/Manolo's reactions.

**Covers:** PA-07 A07-1, A07-2, A07-5 and WP acceptance.

## 3. CF-B1-02 — Material dependency, non-dialogue player restoration, autonomous continuation

### Start

```text
Bar F01 open window
bar-service-stock = MISSING
service = DEGRADED/UNAVAILABLE for affected use
Carmen has expected work context
Antonio later has leisure/service opportunity
Manolo may participate in cards
```

### Player action

Player physically carries/returns the delayed stock to the valid service point through an allowed non-dialogue action.

### Immediate owned result

```text
bar-service-stock: MISSING -> RESTORED
service condition recalculates from owned state
```

No private quest flag is needed.

### Continuation after player leaves

```text
Carmen encounters restored state
 -> PA-02 may select normal service work instead of fallback
 -> PA-07/08 expose valid bar/card opportunity
 -> Antonio later encounters it
 -> PA-02 may choose cards
 -> Antonio may invite Manolo
 -> Manolo owns accept/refuse
 -> session may resolve and emit PA-08 structured outcome
```

### Pass oracle

- player changed a PA-07-owned shared state via an ordinary semantic action;
- >=2 subsequent NPC decisions can occur after player departure;
- downstream choices are not commanded by the player action;
- activity consequence enters through PA-08 rather than a quest continuation script.

**Covers:** PA-07 player-intervention deliverable; PA-09 required player→NPC→NPC chain; A11B-2, A11B-4, A11B-13, A11B-20.

## 4. CF-B1-03 — Player/NPC semantic-equivalence counterfactual

Run CF-B1-02 twice from equivalent material/context state.

### P

Player validly returns `bar-service-stock`.

### N

Eligible NPC validly returns the same stock under equivalent authority/capability/context.

### Required equality

The immediate PA-07 semantic state transition is compatible/equivalent:

```text
MISSING -> RESTORED
service availability/capacity follows the same owner rules
```

### Legitimate later divergence

Actor reactions may differ if initiator identity becomes legitimately known and relationship/role/context differs.

### Forbidden divergence

```text
NPC action -> shared material owner
player action -> player-only quest flag / direct NPC mutation
```

**Covers:** PA-09 A11B-1 and required semantic-equivalence counterfactual; LW-X01.

## 5. CF-B1-04 — NPC-only social activity

### Start

```text
Bar F01 open
card table available
eligible NPCs have compatible time/context
player absent
```

### Result

Antonio and Manolo may independently choose to form/join a card session through PA-02. The session consumes real table capacity and time.

### Pass oracle

- no player interaction prompt is required to instantiate the town activity;
- participant choice remains actor-owned;
- current participants/capacity are coherent;
- session can end and actors re-enter current-context routine/decision flow.

**Covers:** PA-08 A07B-1; PA-09 A11B-7.

## 6. CF-B1-05 — Player joins an existing activity without resetting it

### Start

NPC card session already exists with valid open slot/transition.

### Player action

Player requests/chooses to join.

### Pass oracle

- capacity/participation conditions apply to player as they do to an eligible participant;
- existing participant/session history is not erased merely to center the player;
- player joins through PA-08 session boundary;
- later structured outcome may include the player without granting PA-08 authority over relationships/beliefs/schedule.

**Covers:** PA-08 A07B-2; PA-09 activity integration.

## 7. CF-B1-06 — Relationship changes participation, not game rules

Two runs differ only in one PA-03 relationship input relevant to an invitation/response.

```text
Antonio invites Player to cards
 -> PA-02 receiver decision may differ because PA-03 input differs
 -> if session begins, identical activity rules govern legal play/result
```

The minigame does not read a global friendship score to decide card legality or winner.

**Covers:** PA-08 A07B-3; preserves PA-02/03 ownership.

## 8. CF-B1-07 — Structured outcome changes a later opportunity through an owner

A card session ends with a bounded meaningful outcome, for example:

```text
participants: Antonio, Manolo
completed: YES
duration: real time consumed
result: winner/partner only if game needs it
stake: NONE by default
visibility: participants + actual witnesses
```

A later change is valid only through an owning system, e.g.:

- PA-01 actual routine is late because time was consumed;
- PA-03 may consume a legitimately defined social input;
- PA-06 may select the experience if it passes memory policy;
- PA-04/05 may carry witnessed/told facts.

No `wonCardsQuestFlag` is required.

**Covers:** PA-08 A07B-4, A07B-8.

## 9. CF-B1-08 — Non-witness ignorance

Player and Antonio participate in a notable activity result. Pilar is absent and receives no communication/public evidence.

### Required

Pilar's actor-facing decision state cannot use the result or player authorship as known fact merely because debug lineage knows it.

A later legitimate PA-05 transfer may change that state.

**Covers:** PA-08 A07B-5; PA-09 A11B-3/A11B-9; PA-04/05 inherited negative boundary.

## 10. CF-B1-09 — Interruption and recovery

Card activity is in progress. Closing time or participant priority invalidates continuation.

### Required

The activity declares one valid disposition for this content:

- cancel;
- settle/forfeit early;
- pause/resume only if explicitly supported.

Activity slots are released coherently. Participants return to PA-01/02 current-context evaluation.

### Forbidden

- stranded table reservation;
- actor remains “playing” forever;
- stale pre-interruption routine blindly resumes when current time/context no longer supports it.

**Covers:** PA-08 A07B-6; PA-01 inherited interruption semantics.

## 11. CF-B1-10 — Capacity contention stays bounded

Card table has four participant slots. More eligible actors seek participation than available slots.

### Required

- no exclusive slot is double-owned;
- rejected/waiting actor can return to a bounded PA-02 alternative;
- activity does not globally scan every actor/activity in town to recover;
- contention reason is explainable.

**Covers:** PA-08 A07B-7; PA-02 bounded discovery; PA-07 capacity semantics.

## 12. CF-B1-11 — Low-social activity is not forced into group semantics

Fishing opportunity exists at a real compatible place/time.

### Required

An NPC can choose it alone without the player. A player may independently fish nearby or join as companion only if content supports that mode. A catch crosses into PA-07 material state only when a future decision actually depends on it.

**Covers:** WP-PA-08 individual/low-social deliverable and anti-bar monoculture requirement.

## 13. CF-B1-12 — Household/work conflict uses existing owners

An actor has:

- expected work obligation/context;
- incompatible social/material obligation in the same window.

PA-07 exposes real work/service consequence; PA-01 exposes routine context; PA-03 exposes relevant obligation/tie; PA-02 chooses among eligible responses.

No work-specific bespoke script decides for that NPC.

**Covers:** PA-07 A07-4.

## 14. CF-B1-13 — Off-screen perturbation remains legible precursor

A worker/resource/service perturbation occurs while player is away.

Required structured trace can identify:

```text
what service/material state changed
which bounded opportunity became unavailable/degraded
which later actor decision consumed that changed input
```

This is engine/research trace, not automatic player omniscience. Future PA-11/integration owns player-facing investigation/legibility.

**Covers:** donor PA-07 A07-6 and LW-X12 precursor.

## 15. CF-B1-14 — Quiet-town compatibility / positive causality

Several ordinary leisure sessions occur without conflict.

### Required

- they consume time/place normally;
- most produce no durable event/rumour/memory;
- at least one can create a positive/neutral later opportunity such as invitation, gratitude, companionship or routine variation through normal owners;
- absence of drama remains a valid coherent state.

**Covers:** PA-08 A07B-8, A07B-9, A07B-12; LW-X04/X07/X08.

## 16. NC-B1-01 — Hidden macroeconomy deletion test

Candidate additions:

- town GDP;
- global demand index;
- market-clearing price curve;
- per-object commodity ledger;
- broad industrial production chain.

Delete each from CF-B1-01/02/12.

### Negative oracle

If removal changes no service opportunity, actor eligibility/choice, consequence or player-visible/traceable state in the target town scenarios, the variable is rejected from PA-07 minimum.

**Covers:** PA-07 A07-3 and unit negative gate.

## 17. NC-B1-02 — Decorative job / magic service

A worker is absent but business presentation continues exactly as normal and service is fully available with no substitute/capacity/resource state.

**Expected:** FAIL PA-07.

Reason: the job is decorative and absence is causally irrelevant.

## 18. NC-B1-03 — Minigame island

A polished cards minigame:

```text
player presses prompt
private score screen starts
NPCs never play without player
venue/time/capacity irrelevant
reward currency stays private
no bounded structured outcome crosses to town owners
```

**Expected:** FAIL PA-08 even if mechanics are excellent.

**Covers:** A07B-10 and WP negative gate.

## 19. NC-B1-04 — Integration cannot rescue bad gameplay

Suppose a future playable cards/fishing implementation is structurally integrated but bounded human play inspection finds the core game tedious/unusable.

**Expected:** REJECT activity for product adoption until its gameplay is good; Living World integration alone cannot PASS the fun claim.

This negative remains **deferred empirical**, because B1 research cannot execute the final game.

**Covers:** A07B-11.

## 20. NC-B1-05 — Quest-magic player path

```text
onPlayerRestoresStock:
    questFlags["fixed_bar"] = true
    Carmen.relationship += 10
    Antonio.nextAction = CARDS
    Manolo.dialogueBranch = THANK_PLAYER
```

Equivalent NPC helper uses normal PA-07 state instead.

**Expected:** FAIL PA-09.

Reasons:

- two semantic worlds;
- relationship ownership stolen;
- later actor choices commanded;
- player-specific script carries the chain.

**Covers:** PA-09 unit negative gate, A11B-1/5/12.

## 21. NC-B1-06 — Omniscient aftermath

Player secretly removes/restores a material object without observers. Unrelated actors immediately react to *who did it* because causal debug lineage names the player.

**Expected:** FAIL PA-09/PA-04/05 composition.

Actors may react to independently perceptible changed material state. Attribution requires legitimate information access.

## 22. NC-B1-07 — Downstream ownership theft

Activity or player action directly writes:

```text
belief
relationship
autonomous next action
memory
expected schedule
```

without the accepted owner consuming an input/outcome.

**Expected:** FAIL B1 cross-unit consistency.

## 23. NC-B1-08 — Consequence explosion

Every routine card hand, fishing attempt, purchase and service interaction becomes a permanent memory + rumour + event record.

**Expected:** FAIL B1 composition.

PA-06 finite selected-memory semantics and LW-X08 require most low-salience ordinary state to remain transient/bounded.

**Covers:** PA-08 A07B-9; PA-09 bounded-state invariant.

## 24. NC-B1-09 — Prop sandbox false positive

Player can pick up or push many decorative props, but these interactions never affect a meaningful opportunity, person, access condition or shared state.

**Expected:** does **not** satisfy PA-09 embodied agency.

**Covers:** embodied A11B-21.

## 25. PA-07 acceptance map

| Frozen requirement | Evidence in candidate | Status |
|---|---|---|
| A07-1 worker absence multi-actor | CF-B1-01 | SATISFIED at research-model level |
| A07-2 closed means unavailable | PA-07 service state + CF-B1-01 | SATISFIED |
| A07-3 no useless macro-state | NC-B1-01 | SATISFIED |
| A07-4 household/work conflict | CF-B1-12 | SATISFIED |
| A07-5 bounded cascade | CF-B1-01/02; propagation through explicit opportunity/decisions | SATISFIED semantically; numeric runtime budget deferred |
| A07-6 legibility precursor | CF-B1-13 | SATISFIED |
| WP minimum vocabulary/ownership | PA-07 §§2–3 | SATISFIED |
| player intervention scenario | CF-B1-02 | SATISFIED |
| hidden-macroeconomy negative | NC-B1-01 | SATISFIED |

## 26. PA-08 acceptance map

| Frozen requirement | Evidence | Status |
|---|---|---|
| A07B-1 NPC-only | CF-B1-04 | SATISFIED |
| A07B-2 player joins existing | CF-B1-05 | SATISFIED |
| A07B-3 relationship affects participation not rules | CF-B1-06 | SATISFIED |
| A07B-4 outcome feeds later decision | CF-B1-07 | SATISFIED |
| A07B-5 non-witness ignorance | CF-B1-08 | SATISFIED |
| A07B-6 interruption/recovery | CF-B1-09 | SATISFIED |
| A07B-7 capacity contention | CF-B1-10 | SATISFIED |
| A07B-8 positive chain | CF-B1-14 | SATISFIED |
| A07B-9 no consequence explosion | CF-B1-14 + NC-B1-08 | SATISFIED |
| A07B-10 minigame island negative | NC-B1-03 | SATISFIED |
| A07B-11 integration not fun-proof | NC-B1-04 | BOUNDARY PRESERVED; playable proof deferred |
| A07B-12 quiet town | CF-B1-14 | SATISFIED |
| social + low-social scenario | CF-B1-04/05 + CF-B1-11 | SATISFIED |

## 27. PA-09 core acceptance map

| Frozen requirement | Evidence | Status |
|---|---|---|
| A11B-1 equivalent action/shared spine | CF-B1-03 | SATISFIED |
| A11B-2 player material change alters NPC decisions | CF-B1-02 | SATISFIED |
| A11B-3 witnessed social / non-witness | CF-B1-08 | SATISFIED |
| A11B-4 player leaves, chain continues | CF-B1-02 | SATISFIED |
| A11B-5 same action different context | PA-09 §§2/7 | SATISFIED semantically |
| A11B-6 activity consequence | CF-B1-05/07 | SATISFIED |
| A11B-7 NPC activity absent player | CF-B1-04 | SATISFIED |
| A11B-8 low-stakes session still gameplay | CF-B1-02/05 plus PA-09 action families | RESEARCH MODEL SATISFIED; fun later |
| A11B-9 no omniscient retaliation | NC-B1-06 | SATISFIED |
| A11B-10 story/systemic composition | PA-09 finding; story may constrain, not erase causal history | SATISFIED as requirement |
| A11B-11 save/load continuity | PA-09 future consumer | REQUIRED / RUNTIME PROOF DEFERRED by WP |
| A11B-12 anti-museum | PA-09 high-value action families + NC-B1-09 | SATISFIED as research boundary |

## 28. PA-09 embodied amendment map

| Added scenario | B1 disposition |
|---|---|
| A11B-13 non-dialogue social causality | CF-B1-02 gives non-dialogue material causality; social interpretation boundary is defined; runtime/play proof later |
| A11B-14 follow/accompany | retained high-value family with time/opportunity + PA-02 response; concrete implementation later |
| A11B-15 context-sensitive gesture | context rule adopted; no fixed global relationship delta |
| A11B-16 controlled force vs assault | requirement retained for later combat; no combat mechanics claimed |
| A11B-17 restraint creates gameplay | future combat consumer requirement retained |
| A11B-18 intervention without ownership theft | NC-B1-07 + PA-09 ownership rules |
| A11B-19 mayor changes rules not minds | institutional seam retained; PA-12 owns policy semantics |
| A11B-20 embodied free-play | CF-B1-02 demonstrates a non-dialogue persistent consequence chain; richer playable session deferred |
| A11B-21 no prop sandbox false positive | NC-B1-09 |

These amendment scenarios deliberately include future action families beyond what B1 can empirically prove. B1's responsibility is to preserve the semantic requirement and avoid a research model that would make them impossible; it does not pull combat, governance or final controls into PA-09.

## 29. Cross-unit consistency matrix

| Cause/state | Canonical owner | PA-07 role | PA-08 role | PA-09 role |
|---|---|---|---|---|
| service/resource availability | PA-07 | owns | consumes as activity context if relevant | player/NPC action may validly change through PA-07 |
| activity opportunity/session | PA-08 | may supply venue/service/material precondition | owns integration/session/outcome | player/NPC can participate/interfere through same seam |
| actor choice | PA-02 | exposes opportunity only | exposes participation modes only | initiates player action; does not command NPC follow-up |
| relationship | PA-03 | may create input candidate | may create outcome candidate | action may create observed social input; never direct write |
| belief/knowledge | PA-04/05 | state may be perceptible | witness facts available | attribution only via legitimate access |
| selected memory | PA-06 | experience candidate | activity experience candidate | player-caused experience candidate |
| expected routine | PA-01 | work/service is context | activity consumes time/context | intervention can perturb actual opportunity |
| governance | future PA-12 | consumes changed rule | consumes changed rule | player may invoke legitimate policy channel, not mind-control |

No cell requires a PA-07/08/09 private duplicate of an inherited owner.

## 30. Batch pass candidate

All three unit research results preserve their prewritten acceptance/negative gates, and their shared scenario composes without private causality systems:

```text
PA-07 shared world opportunity
 -> PA-02 actor decision
 -> PA-08 activity opportunity/session/outcome where applicable
 -> PA-09 player/NPC actions enter those same shared owners
 -> PA-03/04/05/06 and later owners consume bounded causes
```

Allowed residuals remain implementation/play/tuning questions explicitly deferred by the WPs.

`PA-B1 WORKER EVIDENCE RESULT: PASS_CANDIDATE`
