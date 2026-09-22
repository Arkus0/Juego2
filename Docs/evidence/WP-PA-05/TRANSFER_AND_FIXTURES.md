# WP-PA-05 — Transfer ownership and causal fixtures

Workpack: `WP-PA-05`  
Date: 2026-09-22  
Purpose: falsifiable research acceptance surface for selective information flow, provenance authority and player-originated transport.

This file does not claim runtime execution. It freezes the semantic controls a future consumer must be able to realize without violating accepted PA-02/03/04 ownership.

## 1. Ownership diagram

```text
PA-02 / authored action owner
  sender motive + chosen communication action/target
                 │
                 ▼
communication opportunity/channel
  participants + time/context + channel eligibility
                 │
                 ▼
PA-05 TRANSFER
  sender
  receiver
  asserted proposition/value
  actor-visible transfer context/provenance payload
                 │
                 ▼
PA-04 RECEIVER EPISTEMIC BOUNDARY
  receiver-owned Acquire/Revise(COMMUNICATION)
  may consume receiver-accessible relationship/context/provenance
                 │
                 ▼
ActorBelief / actor-safe KnowledgeQuery
                 │
                 ├────────► later behaviour/dialogue consumers
                 │
                 └────────► later PA-02 actor decision MAY choose retell

PARALLEL PRIVILEGED ENGINE LANE
  root/parent/hop/lineage technical metadata
       ├─ replay/debug
       ├─ persistence validation
       ├─ technical loop/dedup guard
       └─ configured propagation budget

  MUST NOT feed actor belief/confidence/corroboration/dialogue/action
  unless the corresponding provenance was separately acquired by the actor.
```

Ownership exclusions:

- PA-05 does not choose sender motive; PA-02 does.
- PA-05 does not choose receiver epistemic outcome; PA-04 does.
- PA-05 does not define trust semantics; PA-03 does.
- PA-05 transfer history is not PA-06 autobiographical memory.
- PA-05 player cases do not define the generic PA-09 player-action system.
- PA-05 does not turn public/perception acquisition into communication.

## 2. Fixture discipline

For paired counterfactuals, every non-target variable MUST be cloned/equal. Actor identity may exist as an ownership key but cannot activate identity-specific authored rules or tie breaks unless the fixture explicitly makes identity the tested input.

A control FAILS if:

- the required material result is merely permitted rather than required;
- another semantic input changes with the tested variable;
- a hidden tie-break/actor-ID special case can explain the result;
- the implementation changes actor-visible delivery count in a test that claims only hidden metadata changed;
- an asserted negative control checks only that a field was deleted/changed instead of exercising the effective semantic oracle.

## 3. CF-01 — SELECTIVE_DELAYED_CHAIN

### Purpose

Prove a claim reaches some actors and not others through two explicit opportunities separated in time, without broadcast or automatic receive→relay.

### Fixed fixture

Actors:

```text
Antonio
Manolo
Carmen
Paco
```

Claim:

```text
X = sports_center_closes_early_today
```

Canonical truth:

```text
WorldFact(X) = true
```

Initial actor beliefs:

```text
Antonio = HELD(true) via already-valid PA-04 acquisition
Manolo  = UNKNOWN
Carmen  = UNKNOWN
Paco    = UNKNOWN
```

Injected PA-02-owned sender decisions/opportunities for this semantic fixture:

```text
17:05
  decision D1: Antonio chooses WARN(target=Manolo, claim=X=true)
  opportunity O1: valid direct communication Antonio <-> Manolo

18:05
  precondition: Manolo has acquired X from T1
  decision D2: Manolo chooses TELL(target=Carmen, claim=X=true)
  opportunity O2: valid direct communication Manolo <-> Carmen

Paco:
  no perception/public acquisition for X
  no valid communication opportunity with Antonio/Manolo/Carmen during fixture window
```

The fixed sender decisions deliberately avoid pretending PA-05 proves the PA-02 chooser. PA-05 must prove the transport/composition result once those authorized inputs exist.

### Required result

Exactly:

```text
T1 exists at/after 17:05: Antonio -> Manolo, asserted X=true
Manolo -> HELD(true) through PA-04 COMMUNICATION acquisition caused by T1

T2 exists at/after 18:05: Manolo -> Carmen, asserted X=true
Carmen -> HELD(true) through PA-04 COMMUNICATION acquisition caused by T2

Paco -> UNKNOWN for entire fixture window
```

Additional requirements:

- T2 cannot exist before Manolo's acquisition + the separate D2/O2 inputs.
- Manolo receiving T1 alone cannot auto-create T2.
- no relationship adjacency or canonical truth mutation may synthesize a delivery.
- engine/debug trace may connect T1/T2 through privileged lineage if causally appropriate.
- Carmen's actor-facing provenance contains only information actually communicated/acquired; hidden root/parent/hop fields are not implicitly actor-visible.

### Explicit FAILs

- Paco learns X despite no valid acquisition cause;
- Carmen learns before O2;
- T2 is emitted automatically by T1/Manolo belief change;
- one graph/broadcast operation updates Manolo/Carmen/Paco and later filtering hides the extra writes;
- the fixture only says the chain *may* happen rather than requiring T1/T2 and the exact final epistemic states above.

## 4. NC-01 — NO_GLOBAL_TRUTH_SYNC

### Purpose

Prove canonical truth is not a propagation trigger.

### Paired setup

Run A and Run B are identical except canonical truth changes:

```text
Run A: WorldFact(Y)=false
Run B: WorldFact(Y)=true
```

For both runs:

- Antonio/Manolo/Carmen/Paco actor beliefs for Y are fixed at their initial values;
- there is no PERCEPTION acquisition for Y;
- there is no accessed PUBLIC source for Y;
- there is no KnowledgeTransfer/communication opportunity for Y;
- actor-visible inputs, seed, decisions and relationships are equal.

### Required result

```text
ActorBelief deltas caused solely by truth mutation = 0
InformationTransfer records caused solely by truth mutation = 0
```

Privileged debug truth comparison may change. Actor-safe epistemic state may not.

Any belief repair/sync/fanout is a FAIL.

## 5. CF-02 — FALSE_ASSERTION

### Purpose

Prove a false assertion can produce receiver state without mutating truth or leaking an omniscient false/lie label.

### Fixed setup

```text
WorldFact(Z)=true
Antonio ActorBelief(Z)=true
Manolo ActorBelief(Z)=UNKNOWN
```

Authorized fixture inputs:

```text
D1: Antonio chooses TELL Manolo asserting Z=false
O1: valid direct communication opportunity
R1: declared receiver fixture rule accepts/revises the received assertion
```

The fixture does not assert why Antonio lies; deception motive belongs to the sender/action owner.

### Required result

```text
WorldFact(Z)=true                  // unchanged
Transfer T1 asserts Z=false
Manolo ActorBelief(Z)=false via COMMUNICATION/T1
```

Manolo's actor-facing state cannot consume privileged `isLie`, `isFalse`, canonical comparison or hidden causal origin unless separately acquired.

Text/dialogue changing while Manolo's PA-04 belief remains UNKNOWN is a FAIL.

## 6. NC-02 — HIDDEN_LINEAGE_EPISTEMIC_INVARIANCE

### Purpose

Directly preserve the donor FAIL→PASS correction. This is the principal PA-05 negative regression.

### Paired setup

Carmen receives two reports in both Run A and Run B.

Actor-visible inputs are byte/semantically equal between runs:

```text
canonical WorldFact(X)
Carmen initial ActorBelief(X)
report #1: immediate speaker=Manolo, asserted X=true
report #2: immediate speaker=Paco,   asserted X=true
actor-accessible reported provenance: NONE beyond immediate speakers
channel/context visible to Carmen
PA-03 inputs exposed to Carmen's receiver rule
receiver rule/config
seed/order/timestamps visible to Carmen
both reports are DELIVERED in both runs
```

Only privileged engine metadata differs:

```text
Run A engine lineage:
  report #1 root=L1
  report #2 root=L1

Run B engine lineage:
  report #1 root=L1
  report #2 root=L2
```

Important isolation requirement:

> The technical guard configuration for this fixture MUST permit both already-declared deliveries in both runs. A same-lineage guard may not suppress report #2 in Run A, because that would change Carmen's actor-visible evidence and invalidate the claimed one-variable counterfactual.

### Required result

Carmen's complete actor-facing epistemic output is identical across Run A/B:

```text
belief proposition/value
belief-held/unknown state
confidence/corroboration result if such a fixture-local output exists
actor-facing explanation/reasons
behaviour/dialogue eligibility derived from that belief/provenance
```

If any of those differ solely because engine lineage changed, PA-05 FAILS.

Engine/debug trace is allowed to differ and report the different root topology.

### Explicit false-green guards

The control is invalid if:

- one report is missing/suppressed in one run;
- timestamp/order/channel differs;
- immediate speaker differs;
- relationship/context differs;
- receiver rule differs;
- hidden lineage is copied into actor-accessible provenance before evaluation;
- the assertion merely compares stored inputs rather than executing the same effective receiver epistemic/query surface used by the consumer.

## 7. CF-03 — REPORTED_SOURCE_POSITIVE

### Purpose

Prove the negative gate does not forbid legitimate source reasoning once provenance is actually learned.

### Paired setup

Start from NC-02 with engine lineage identical and non-provenance inputs fixed.

Run A actor-visible reports:

```text
Manolo says X=true; no source stated
Paco says X=true; no source stated
```

Run B actor-visible reports:

```text
Manolo says X=true and explicitly reports source=Antonio
Paco says X=true and explicitly reports source=Antonio
```

The fixture-local receiver rule is declared in advance to distinguish:

```text
TWO_IMMEDIATE_SPEAKERS_UNKNOWN_ORIGIN
vs
TWO_REPORTS_EXPLICITLY_NAMING_SAME_SOURCE
```

No global confidence formula is canonized.

### Required result

The fixture-local provenance/corroboration result MUST be the declared different result because the tested input is now legitimately actor-accessible common-source information.

For this frozen fixture, require the concrete labels:

```text
Run A -> SOURCE_INDEPENDENCE_UNKNOWN
Run B -> COMMON_REPORTED_SOURCE_ANTONIO
```

If Run B needs privileged engine lineage to infer Antonio rather than the explicit report payload, FAIL.

## 8. CF-04 — BOUNDED_LOOP

### Purpose

Prove finite propagation and keep technical loop control separate from actor epistemology.

### Fixed setup

```text
A -> B transfer T1 allowed
B -> C transfer T2 allowed after a separate B decision/opportunity
C later selects a candidate relay to A under a separate C decision/opportunity
```

All three transfers/candidates are associated in engine metadata with privileged lineage `L1` where causally appropriate.

Fixture policy:

```text
repeat/loop guard: reject a relay that would re-enter an actor already visited by L1
```

The number/policy is fixture data, not a product constant.

### Required result

```text
T1 delivered
T2 delivered
candidate C -> A rejected by technical guard
stable transfer count reached; no automatic new relay appears
engine/debug reason = technical repeat/loop guard
```

Actor-facing state requirements:

- A/B/C do not automatically learn `lineageId=L1`;
- no actor belief/confidence/corroboration result changes merely because the engine detected the loop;
- actor-facing explanation cannot cite hidden `L1` unless separately acquired;
- rejection of C->A is an operational outcome, not evidence that any actor knows the common origin.

A capped automatic graph walk fails even if it terminates.

## 9. CF-05 — PLAYER_ORIGINATED_FLOW

### Purpose

Reconcile PA-05 with the Playable Causal City: player-originated information enters the same causal path as NPC information and may continue after the player leaves.

### Fixed setup

Claim:

```text
P = ferry_departure_delayed
```

Initial:

```text
Carmen = UNKNOWN
Manolo = UNKNOWN
Paco   = UNKNOWN
```

Authorized inputs:

```text
16:00 player chooses a valid communication action asserting P=true to Carmen
16:00 valid player<->Carmen opportunity/channel
receiver fixture rule allows Carmen to acquire the assertion

16:30 player is absent
16:30 Carmen independently has a PA-02-owned decision to WARN Manolo
16:30 valid Carmen<->Manolo opportunity/channel
receiver fixture rule allows Manolo to acquire

Paco has no acquisition path during the fixture
```

### Required result

```text
T1: Player -> Carmen
Carmen -> HELD(true) via COMMUNICATION

T2: Carmen -> Manolo, only because of the independent later decision/opportunity
Manolo -> HELD(true) via COMMUNICATION

Paco -> UNKNOWN
```

Requirements:

- player origin does not broadcast P to the town;
- no quest-only `heardRumour_P` / `playerRevealed_P` parallel authority is required;
- T2 can occur after the player leaves because Carmen owns her later choice;
- if the player lies, truth remains owned elsewhere exactly as in CF-02;
- any reported provenance visible to Manolo is only what Carmen actually communicates.

### Conceal / expose / investigate corollaries

The player can **conceal** by not communicating or, where another owner permits it, by affecting a normal source/opportunity. Concealment is not `erase all beliefs`.

The player can **expose** through normal communication or an authorized public-source owner. Exposure is not `set knows=true on all actors`; every actor still needs a legitimate PA-04 acquisition path.

The player can **investigate** only through an authorized player-facing evidence/source/witness path owned by the later investigation/integration layer. A successful investigation may give the player actor-accessible evidence or reported provenance that can then be used or communicated through the same normal PA-04/PA-05 paths. It MUST NOT expose `lineageId`, hidden root/parent/hop, canonical truth comparison or another privileged engine field merely because the debugger has it. PA-11/integration owns the concrete investigation verbs/UI/trace presentation; PA-05 owns only this no-privileged-shortcut information-flow seam.

## 10. Deferred consumer proof

These fixtures intentionally do not prove:

- a final runtime event bus/queue;
- Unity hearing/proximity/LOS;
- exact C# schema or storage;
- dialogue rendering;
- exact confidence math;
- final relay limits/cooldowns/TTLs;
- save compaction;
- population-scale performance;
- FULL/ABSTRACT equivalence.

A later H4/H7 implementation may choose any representation that satisfies the controls causally.

## 11. Reviewer attack checklist

Try to produce a counterexample for each:

1. CF-01 reaches Paco or reaches Carmen before O2.
2. CF-01 T2 exists without a separate Manolo decision.
3. NC-01 truth mutation creates any belief/transfer delta.
4. CF-02 mutates truth or only changes presentation.
5. NC-02 changes a second actor-visible variable together with lineage.
6. NC-02 lets a technical dedup guard remove one delivered report.
7. NC-02 compares a toy result rather than the effective receiver epistemic surface.
8. CF-03 discovers Antonio from hidden lineage rather than explicit reported provenance.
9. CF-04 terminates only because a global manager caps automatic diffusion.
10. CF-04 exposes hidden loop identity as actor-facing evidence.
11. CF-05 player origin updates non-recipients or uses a parallel quest rumour store.
12. CF-05 investigation reads privileged engine/debug lineage instead of a legitimate player-facing evidence/source path.
13. any fixture requires global population/social-graph enumeration before producing its bounded recipients.
