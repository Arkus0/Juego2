# WP-HK-05 representative content-shape probe

## Approved product source

This bounded probe uses `Docs/art/SETTING.md`: fictional Potes/Liébana, with the H2 hero slice centered on plaza + streets + bar/shop. It does not add gameplay, schedules, transforms, Unity data or domain-specific schemas.

## Bounded scenario

`Hk05ContentShapeProbeTests.PotesHeroSliceFitsValidationAndBrokenAuthoredReferenceFailsClosed` models only enough current canonical structure to exercise HK05:

- world `world.potes`;
- `place.plaza`;
- `building.bar` and `building.shop`, both contained by the plaza;
- `npc.ana`, also placed in the plaza;
- one typed `works.at -> building.bar` object reference;
- one opaque `arkus.npc-profile@1@object:npc.ana` extension whose declared dependency also targets the bar.

The `fixture.*` type IDs remain opaque stable identifiers. The probe does not claim that `fixture.place`, `fixture.building`, `fixture.npc` or `works.at` are final gameplay schemas; they are representative content labels carried by the existing generic model.

## Candidate surfaces exercised

1. Construct the finite canonical state through the accepted HK02/HK02A model.
2. Invoke `world.validation.current`; the representative state must be valid with zero diagnostics.
3. Invoke inherited `world.summary`; the same state remains inspectable as four objects + one extension.
4. Propose removal of `building.bar` through the HK04 mutation grammar.
5. Invoke `authoring.change.validate`; the proposal must report the now-dangling object reference and extension dependency as structured diagnostics.
6. Invoke canonical apply with the same request; it must return `world.change.invalid_candidate` and preserve revision/hash.

This checks the HK05 validate/current/proposed/apply relationship on content shaped like the approved target rather than only on abstract `node.*` fixtures.

## Findings

- **Representability:** PASS. The current generic micro-world can represent the bounded plaza/bar/shop/NPC identity and typed dependency shape without a new schema.
- **Identity/granularity:** PASS for HK05. Individual plaza/building/NPC resources and object-scoped extension data are separately addressable and diagnosable.
- **Validation boundary:** PASS. Removing a referenced resource creates multiple independent violations and returns repairable resource/path context rather than an exception.
- **Mutation boundary:** PASS. The exact proposal rejected by explicit validation is also rejected before canonical commit.
- **Inspection boundary:** PASS for the probe. Existing summary observes the representative state; HK05 does not reopen HK03 completeness.

## Classification

- In-scope blockers: none after the validation-route authority repair found during Worker pre-review.
- Predecessor reopen conditions: none observed. HK04 commit authority remains closed; HK02A reference/dependency semantics behave as accepted.
- Named future/residual decisions: gameplay meaning of type/reference IDs, transforms, schedules/AI behavior, Unity representation, content budgets and final gameplay invariants.
- Out-of-boundary observations: visual fidelity, asset selection, river/port expansion and engine-specific validation.

The probe is representative evidence only, not a completeness oracle. Completeness for HK05 is established separately by the invariant inventory/effective fixtures and public canonical-mutation route checks.
