# WP-PA-04 — Playable Causal City amendment proof

Workpack: `WP-PA-04`  
Execution: `REMOTE_HARVEST`  
Source requirement: `Docs/research/living-world/LIVING_WORLD_CROSSCUTTING_AMENDMENT_01_PLAYABLE_CAUSAL_CITY.md`, PA-04 added proof  
Date: 2026-09-22

## Why this evidence exists

The cross-cutting Playable Causal City amendment adds a mandatory PA-04 acceptance surface beyond the base harvest WP:

> A visible player action must be able to produce different beliefs for witness and non-witness actors, while a later legitimate information source may update the non-witness.

The original Worker candidate described player-caused information asymmetry but did not freeze that amendment requirement as a falsable fixture. Strict Worker pre-review classified that omission as an in-claim blocker and added this proof before final freeze.

This is a research-level semantic fixture, not a claim that perception, dialogue, runtime belief storage or PA-05 propagation already exists.

## CF-04 — visible player action: witness / non-witness / later allowed source

### Fixed world/action surface

```text
canonical starting world = W
player action = GIVE_SEALED_LETTER(player, Antonio)
location = Plaza
semantic action outcome = identical in all comparisons
canonical action fact = player_gave_letter_to_Antonio = true
actor profiles / relationships / routine / seed = fixed where relevant
```

The player action itself is a normal shared-world cause. It does not directly write every actor's belief and does not toggle a quest-global `everyone_knows` flag.

### T1 — immediate aftermath

Carmen is a legitimate witness under the fixture's declared perception surface:

```text
Carmen perception includes the player action
```

Required Carmen epistemic result:

```text
query(Carmen, player_gave_letter_to_Antonio)
  = HELD(true)
sourceKind = PERCEPTION
sourceRef = exact player action / witnessed event
```

Paco is a non-witness and has no other acquisition path at T1:

```text
Paco not in perception surface
no communication received
no accessed public source
no authored knowledge grant
no bounded inference rule with sufficient actor-accessible inputs
```

Required Paco epistemic result:

```text
query(Paco, player_gave_letter_to_Antonio) = UNKNOWN
```

The fixture **FAILS at T1** if:

- Paco learns merely because the canonical action occurred;
- Paco learns because the action was player-originated;
- a quest/global flag broadcasts the fact;
- Carmen fails to acquire the witnessed fact despite the declared valid perception event;
- Carmen's provenance cannot identify perception of the relevant action/event.

### T2 — later legitimate information source

PA-04 does not own why/when Carmen chooses to speak or how rumours propagate. Therefore the later transfer is supplied as an **exogenous PA-05-compatible communication input seam**:

```text
communication event:
  sender = Carmen
  receiver = Paco
  asserted claim = player_gave_letter_to_Antonio = true
  fixture declares Paco accepts/revises from this received claim
```

Required Paco epistemic result after that allowed acquisition:

```text
query(Paco, player_gave_letter_to_Antonio)
  = HELD(true)
sourceKind = COMMUNICATION
sourceRef = Carmen / received communication event
```

The fixture **FAILS at T2** if:

- Paco remains forced to `UNKNOWN` despite the explicitly accepted valid acquisition event;
- Paco's provenance is rewritten to `PERCEPTION` even though he did not witness the action;
- Paco's belief is sourced from hidden canonical truth instead of the received communication;
- the fixture has to specify sender motive, retransmission policy, distortion/corroboration rules or a rumour cascade in order for PA-04 to represent the receiver belief.

## Negative pair — hide the player action from the witness

Keep the canonical player action, actor profiles, relationships and all non-perception inputs fixed. Change only Carmen's legitimate perception access so she no longer witnesses the action and receives no other source.

Required:

```text
query(Carmen, player_gave_letter_to_Antonio) = UNKNOWN
query(Paco, player_gave_letter_to_Antonio) = UNKNOWN
```

If Carmen still knows because the player performed the action or because canonical truth is globally visible, PA-04 fails.

## Ownership preserved

This fixture proves only PA-04-owned epistemic semantics:

```text
player action / world owner
        ↓
legitimate perception event
        ↓
PA-04 actor belief

later exogenous communication event
        ↓
PA-04 receiver acquisition/revision
```

It deliberately does **not** own:

- generic player-action semantics → PA-09;
- perception geometry/range/Unity implementation → later runtime work;
- why Carmen chooses to communicate → PA-02/PA-05;
- retransmission, distortion, corroboration or propagation budgets → PA-05;
- autobiographical memory/forgetting/compaction → PA-06.

## Acceptance statement

The required amendment proof is closed at research-contract level only when all three states are distinguishable and causally justified:

```text
T1 witness Carmen     = HELD via PERCEPTION
T1 non-witness Paco   = UNKNOWN
T2 Paco after allowed communication = HELD via COMMUNICATION
```

No step may read hidden canonical truth merely because the engine or player knows that the action happened.
