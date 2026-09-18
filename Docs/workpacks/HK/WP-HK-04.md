# WP-HK-04 — Planning + transactional mutation

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-03`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Give the AI a safe mutation model based on plan → validate/dry-run → atomic apply, rather than opaque direct edits.

## Acceptance

- Mutating requests produce a deterministic proposed change set before commit when requested.
- Dry-run executes the same semantic planning/validation path as real apply, without persisting state.
- Apply is atomic: either the whole accepted change set commits or state remains unchanged.
- Optimistic concurrency uses explicit expected revision/hash (or equivalent) so stale writers fail rather than overwrite silently.
- Request/idempotency semantics are explicit and tested; retries cannot accidentally duplicate accepted mutations.
- Change sets identify resources/fields/references affected before apply.
- Preconditions/postconditions are machine-readable where practical.
- No public command may bypass the canonical mutation pipeline to change authorable state privately.
- Mutation dispatcher surface == discovered mutation surface == transactional surface is mechanically checked.

## Required self-attacks

RED→GREEN for: partial apply after mid-operation failure, stale revision overwrite, duplicate retry, hidden mutation bypass, dry-run differing semantically from apply, and command whose effects are absent from its plan/change set.

## Forbidden scope

Undo history UI, Unity scene writes, arbitrary filesystem edits, gameplay-specific authoring commands beyond the micro-world fixture.

## DoD

The micro-world can be created and modified transactionally with deterministic plans and zero hidden mutation paths; independent PASS.
