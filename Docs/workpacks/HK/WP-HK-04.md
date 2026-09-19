# WP-HK-04 — Planning + transactional mutation

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-03` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`  
Baseline SHA: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`  
Implementation PR: `#19`

Completion:
- Reviewed candidate SHA: `849ed68e41d674ab0d50883ccd9af394e9d2e456`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5256156672`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35450965678`)
- Merge SHA: `b1810a5f7c5378ff06272718a11e08720d714a65`
- Completed: `2026-09-19`
- Review history: two earlier candidates were rejected for a circular/effectively incomplete hidden-mutation proof boundary; the accepted repair closed public commit authority rather than extending structural wrapper enumeration.

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
- No public command may use an undeclared alternate path to change authorable state outside the canonical mutation pipeline.
- Mutation dispatcher surface == discovered mutation surface == transactional surface is mechanically checked.

## Accepted architecture / proof boundary

- `TransactionalWorldAuthoringSession` exposes current state plus plan/dry-run publicly, but no public commit method.
- Canonical commit authority is internal to the Authoring→Runtime boundary through `ICanonicalWorldMutationCommitter`; only the canonical apply binding receives it.
- `MutationAuthorityInspector` is defence in depth only, not a completeness oracle and not a wrapper/container enumeration strategy.
- HK04 consumes the accepted HK01 independently enumerable public route universe rather than re-proving it.
- Every current public non-`CanonicalMutation` route is effectively invoked against one live authoritative session and must preserve canonical revision/hash; the request-vector set must exactly match the non-mutation definition set.
- Mutation definitions, canonical-transaction policy, effective `ITransactionalMutationHandler` routes and dispatcher bindings are mechanically reconciled.
- Atomicity, stale-writer rejection, idempotent retry semantics, plan/dry-run/apply parity and independent semantic change-set coverage are executable and GREEN on the reviewed SHA.

## Required negative-conformance tests

RED→GREEN for: partial apply after mid-operation failure, stale revision overwrite, duplicate retry, undeclared canonical mutation path, dry-run differing semantically from apply, and command whose effects are absent from its plan/change set.

Accepted evidence also retains the exact ordinary `List<TransactionalWorldAuthoringSession>` indirection class that broke the prior proof: no container-specific traversal was added; the authoritative session simply no longer grants public commit power.

## Forbidden scope

Undo history UI, Unity scene writes, arbitrary filesystem edits, gameplay-specific authoring commands beyond the micro-world fixture.

## DoD

The micro-world can be created and modified transactionally with deterministic plans and zero undeclared mutation paths; independent PASS. ✅
