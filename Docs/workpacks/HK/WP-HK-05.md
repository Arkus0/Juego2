# WP-HK-05 — Validation + repairable diagnostics

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-02A`
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make invalid world states and invalid authoring intents detectable through stable, machine-actionable diagnostics rather than opaque exceptions.

## Acceptance

- Canonical validator covers structural validity, identity uniqueness, reference integrity and every invariant owned by the micro-world model.
- Validation runs before commit and can also be invoked explicitly against current/proposed state.
- Diagnostics have stable machine code, severity, resource/path context, invariant identity and concise remediation context when safe.
- Multiple independent violations can be reported deterministically without hiding earlier failures behind exceptions.
- Validation result schema is discoverable and versioned.
- Runtime exceptions are not used as the public expected-error contract.
- Validator inventory is mechanically enumerable and mapped to owned invariants; unclassified invariants/rejection sites are zero.
- A known-invalid change cannot enter canonical state through any public mutation route.

## Required negative-conformance tests

RED→GREEN for: validation being skipped on a public mutation path, missing invariant registration, exception escaping as public error, nondeterministic diagnostic order, ambiguous path/context, and mutation route accepting state rejected by explicit validation.

## Forbidden scope

AI natural-language repair generation, Unity validation, final gameplay invariants.

## DoD

Invalid micro-world edits are rejected consistently and a machine client has enough structured information to choose a corrected request; independent PASS.
