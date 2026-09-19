# WP-HK-08 — Agent ergonomics + interaction efficiency

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-07B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make the harness efficient for real AI use, not merely correct: minimize unnecessary round trips, token volume and ambiguous repair loops while preserving explicit contracts.

## Acceptance

- Batch authoring supports multiple related changes in one transaction without weakening atomicity or validation.
- Read APIs support bounded projections/pagination and compact machine responses. This includes the accepted HK-06A provenance journal read, which currently returns the complete session-local journal in one response and must become bounded here without changing its accepted semantics.
- Discovery distinguishes cheap reads, expensive reads and mutating operations where relevant.
- Structured errors include enough local context for repair without forcing full-world reinspection in ordinary cases.
- A reference micro-world authoring benchmark measures request count, response bytes and elapsed harness time for representative create/inspect/modify/repair flows.
- Baseline budgets are recorded from evidence rather than guessed; regressions above explicit thresholds fail CI or require reviewed budget update.
- No “one API call per field/property” requirement exists for common authoring flows.
- Compact/batch modes are semantically equivalent to canonical single operations and cannot skip validation/provenance.
- Responses do not echo unnecessary implementation/internal state by default.

## Required negative-conformance tests

RED→GREEN for: batch partially committing, compact response omitting required repair context, pagination causing duplicate/missed resources, pathological per-field round-trip regression, and batch path omitting provenance/validation.

## Forbidden scope

Prompt engineering tied to one model vendor, natural-language planner, autonomous loop/orchestrator, gameplay content.

## DoD

A measured reference client can complete representative authoring flows efficiently through stable protocol primitives, with recorded performance/interaction baseline and independent PASS.
