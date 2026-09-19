# WP-HK-06A representative content-shape probe

## Approved product source

This bounded probe uses `Docs/art/SETTING.md`: the H2 hero slice is a fictional Potes/Liébana plaza
with streets, bar/shop and social NPC presence. It does not add final gameplay schemas, transforms,
schedules, clock logic, Unity state or deterministic simulation.

## Bounded scenario

`Hk06AContentShapeProbeTests.PotesAuthoredEditIsJournaledWhileScheduledNpcObservationRemainsTransient`
models only enough content to exercise HK06A:

- world `world.potes` at authored revision 7;
- `place.plaza`, `building.bar`, `building.shop` and `npc.ana`;
- `npc.ana` has declared `works.at -> building.bar` reference/dependency data;
- one accepted authored edit changes `building.shop` through the canonical transaction pipeline;
- a test-owned scheduled-NPC observation surrogate for `npc.ana` is stamped with the resulting
  authored revision/hash and then advances transient step/position twice.

The `fixture.*` type labels remain opaque representative content. `fixture.building.refreshed` means
only an authored field change for the probe; it is not a final renovation or gameplay system.

## Candidate surfaces exercised

1. Construct the accepted canonical Potes-shaped state.
2. Apply one authored shop edit through `authoring.change.apply@1.0`.
3. Read `authoring.journal.read@1.0` and require one entry whose request, base/result anchors and
   affected set name the effective shop edit.
4. Create `arkus.runtime-observation-stamp@1` from that exact authored base.
5. Advance only test-surrogate step/position fields twice.
6. Re-read canonical revision/hash and journal entry IDs; all must remain unchanged.
7. Inject an unsafe test surrogate with an apply callback and require the same boundary oracle to
   turn RED, proving the safe result is causal rather than vacuous.

## Findings

- **Representability:** PASS. The existing generic model represents the bounded plaza/building/NPC
  identities needed for provenance without a new gameplay schema.
- **Mutation/provenance boundary:** PASS. The accepted shop edit creates exactly one truthful entry
  for `world.object:building.shop`.
- **Identity/granularity:** PASS for HK06A. The authored resource and runtime-observed NPC remain
  separately named; transient fields are not encoded into the canonical object.
- **Authored/live boundary:** PASS. The runtime stamp points to the exact authored base; surrogate
  activity changes only surrogate memory.
- **Authority boundary:** PASS. The safe surrogate has no authoring service. The injected unsafe
  callback changes revision/hash/journal and is detected immediately.

## Classification

- In-scope blockers: none observed.
- Predecessor reopen conditions: none observed; accepted HK04 apply and HK05 validation compose
  without a second writer or validator.
- Named future/residual decisions: actual schedules, clock source, simulation determinism, runtime
  observation payload schemas, snapshot/history interaction and replay compatibility.
- Out-of-boundary observations: visual fidelity, movement meaning, animation/physics, Unity scene
  state and gameplay AI.

The probe is representative evidence only. Journal truthfulness is established separately by the
effective two-transition/concurrency tests and independent transition audit.
