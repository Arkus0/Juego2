# WP-PA-B1 — PA-09 embodied-action semantic fixtures

Status: **WORKER EVIDENCE / REVIEW PENDING**  
Unit: PA-09  
Source amendment: `PA-09_AMENDMENT_01_EMBODIED_ACTIONS.md`  
Date: 2026-09-26

## 0. Proof boundary

These fixtures close the **research oracle** for embodied A11B-13..21. They show that the proposed ownership model can represent each required action family without dialogue-only causality, player-only quest magic or downstream ownership theft.

They do **not** claim that final controls, animation, combat feel, crime/governance rules or playable quality have been implemented. Those are explicitly deferred consumers.

## CF-E13 — Non-dialogue social causality

Player helps Carmen physically unload/return `bar-service-stock` at Bar F01 without first resolving the action through dialogue.

Required semantic path:

```text
player HELP/CARRY action
 -> PA-07 material dependency restored
 -> Carmen directly experiences/perceives the help if present
 -> that actor-accessible experience may become PA-03/PA-06 input
 -> PA-03/PA-06 keep authority over relationship/memory consequence
```

A non-witness receives no automatic social attribution.

**Pass:** useful embodied help changes shared state and can become social cause without direct `relationship += N` or dialogue-as-adapter.

**Covers:** A11B-13.

## CF-E14 — Follow / accompany is a decision, not puppeteering

Antonio asks or signals for accompaniment to a bounded destination/purpose. Player may accept/refuse. Conversely, the player may invite Antonio to accompany them and Antonio owns the accept/refuse decision via PA-02.

If accepted:

- shared travel consumes real time/opportunity;
- separation, arrival or higher-priority interruption terminates/re-evaluates the activity coherently;
- neither participant's entire schedule becomes owned by a `FollowSystem`;
- later social meaning, if any, passes through normal owners.

**Fail:** `player selects FOLLOW -> NPC forced to trail indefinitely`.

**Covers:** A11B-14.

## CF-E15 — Context-sensitive signal / gesture

Use the same physical signal in at least two contexts, for example a wave/beckon/show gesture:

- familiar actor in a calm public setting;
- unknown or unwelcome actor at a restricted/private threshold.

Required:

```text
same physical family
 + different role/relationship/place/consent/urgency context
 -> potentially different interpretation/response through owners
```

Forbidden baseline:

```text
WAVE -> affinity +1
BECKON -> follow = true
```

**Covers:** A11B-15.

## CF-E16 — Controlled force versus serious assault

This is a semantic counterfactual for the later combat consumer, not combat implementation.

Two future physical interventions may share a broad force family but differ in:

- severity;
- target condition;
- consent/authority;
- threat context;
- injury/property outcome;
- witnessed conduct.

A shove to separate an immediate altercation and a sustained severe attack must not collapse to one universal `violent=true -> reputation -N` result. The future action result must emit sufficiently typed owned facts for downstream systems to distinguish them.

**Covers:** A11B-16.

## CF-E17 — Restraint / proportionality can create gameplay

Future physical-conflict semantics must leave room for outcomes such as:

- stop/block;
- restrain;
- disable/disengage;
- protect/escort away;
- serious injury/lethal result where later design permits.

These are not required controller verbs. They are consequence distinctions required so “not escalating” can be a meaningful option instead of every confrontation collapsing to maximum force or dialogue.

All outcomes rejoin normal material/social/knowledge owners.

**Covers:** A11B-17.

## CF-E18 — Intervene without stealing NPC agency

Player changes the **current condition**, not an NPC's autonomous next action.

Example:

```text
player clears a blocked service access / returns a needed object / separates an immediate obstruction
 -> shared condition changes through its owner
 -> affected NPC perceives/encounters new opportunity
 -> PA-02 chooses what to do next
```

Forbidden:

```text
playerIntervened = true
NPC.nextAction = THANK_AND_GO_HOME
```

**Covers:** A11B-18 and reinforces the PA-09 hard negative.

## CF-E19 — Mayoral authority changes rules, not minds

Future governance owner PA-12 may accept a legitimate player policy/authority action such as changing a public access/service rule.

Required semantic path:

```text
legitimate player authority action
 -> PA-12-owned rule changes
 -> PA-07/08 opportunities may change
 -> actors perceive/learn relevant rule through legitimate channels
 -> PA-02 actors choose responses using their own state
```

Forbidden:

```text
mayorPolicyChosen -> allCitizens.supportMayor = true
mayorPolicyChosen -> allCitizens.nextAction = COMPLY_HAPPILY
```

This fixture does not implement governance; it reserves a compatible ownership seam.

**Covers:** A11B-19.

## CF-E20 — Embodied free-play chain

Combine ordinary high-value actions without a bespoke quest script:

```text
player helps restore Bar F01 stock without dialogue
 -> service condition changes
 -> player later occupies/releases a card-table slot or joins an existing session
 -> another actor independently accepts/refuses participation
 -> player leaves
 -> later NPC activity continues from shared state
```

At least one useful consequence survives beyond the immediate interaction, and the chain still uses PA-07/08 plus inherited owners rather than a private “free-play” state machine.

**Covers:** A11B-20.

## NC-E21 — Prop sandbox false positive

Candidate build allows the player to pick up/push dozens of decorative props. None changes:

- a meaningful service/material state;
- access or capacity;
- an actor's legitimate opportunity;
- a witnessed/socially interpretable event;
- an activity state;
- any later owned consequence.

**Expected:** this does **not** satisfy embodied agency merely because physical interaction count is high.

The product target is **affordance density**: fewer actions with reusable causal yield.

**Covers:** A11B-21.

## Acceptance matrix

| Amendment scenario | Research fixture | Research disposition | Deferred proof |
|---|---|---|---|
| A11B-13 non-dialogue social causality | CF-E13 | SATISFIED semantically | animation/control feel + runtime owner routing |
| A11B-14 follow/accompany | CF-E14 | SATISFIED semantically | navigation/formation/interruption UX |
| A11B-15 context-sensitive gesture | CF-E15 | SATISFIED semantically | gesture set, perception radius, animation/readability |
| A11B-16 controlled force vs assault | CF-E16 | SATISFIED as required semantic distinction | combat/crime implementation and balance |
| A11B-17 restraint creates gameplay | CF-E17 | SATISFIED as future product constraint | combat controls/AI/feel |
| A11B-18 intervention without agency theft | CF-E18 | SATISFIED | runtime shared-owner proof |
| A11B-19 mayor changes rules not minds | CF-E19 | SATISFIED as governance seam | PA-12 governance semantics/runtime |
| A11B-20 embodied free-play | CF-E20 | SATISFIED at research-model level | bounded playable session proof |
| A11B-21 no prop-sandbox false positive | NC-E21 | SATISFIED negative | later playable affordance inspection |

## Result

Every embodied amendment scenario now has an explicit falsifiable research fixture. Where the scenario necessarily belongs to later combat, governance, controls or playable-quality work, B1 preserves the semantic oracle and names the deferred proof rather than falsely claiming implementation.

`PA-09 EMBODIED FIXTURE RESULT: PASS_CANDIDATE`
