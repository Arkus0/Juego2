# WP-HK-08 — Agent ergonomics + interaction efficiency

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-07B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Make the harness efficient for real AI use, not merely correct: minimize unnecessary round trips, token volume and ambiguous repair loops while preserving explicit contracts.

## Acceptance

- Batch authoring supports multiple related changes in one transaction without weakening atomicity or validation.
- Batch capacity/shape is evidence-driven rather than treated as an arbitrary product constant. The representative gate authoring intent must fit in one accepted atomic batch under the declared resource budget. The current 64-operation request limit is a starting constraint, not a sacred semantic boundary: if the representative flow cannot fit safely, HK08 must justify and adjust the limit/request shape before `WP-HK-GATE` rather than silently split one atomic intent into partially committed plans.
- Read APIs support bounded projections/pagination and compact machine responses. This includes the accepted HK-06A provenance journal read, which currently returns the complete session-local journal in one response and must become bounded here without changing its accepted semantics.
- Discovery distinguishes cheap reads, expensive reads and mutating operations where relevant.
- A stale-revision/hash conflict returns stable machine-readable recovery context anchored to both the request's expected base and the current authored revision/hash. For the ordinary bounded H0 case, that context includes enough deterministic semantic change/resource information for a client to preserve its intent and re-plan only what is affected rather than re-inspecting the complete world.
- Conflict recovery is explicit re-planning, not an implicit merge: the client submits a new normal dry-run/apply request against the new base and all accepted validation, atomicity, idempotency and provenance semantics still apply.
- Structured diagnostics may expose deterministic prioritization/grouping or a likely first repair when useful, but compact/priority presentation must not suppress independent HK05 diagnostics. The complete machine-actionable diagnostic set remains retrievable.
- A reference micro-world authoring benchmark measures request count, response bytes and elapsed harness time for representative create/inspect/modify/repair flows, including one stale-plan → structured recovery → successful retry flow.
- The stale-plan recovery benchmark must demonstrate that the ordinary representative conflict can be repaired without a full-world reload; if the client must reconstruct the whole world, the interaction contract is not yet sufficient.
- Baseline budgets are recorded from evidence rather than guessed; regressions above explicit thresholds fail CI or require reviewed budget update.
- No “one API call per field/property” requirement exists for common authoring flows.
- Compact/batch/recovery modes are semantically equivalent across the accepted reference transport and MCP projection. HK08 must rerun cross-transport conformance for the ergonomic primitives it changes or adds; HK07B PASS is not treated as proof for later-added interaction semantics.
- Compact/batch/recovery paths cannot skip canonical validation/provenance, invent a second semantic registry or change request/result/error meaning between transports.
- Responses do not echo unnecessary implementation/internal state by default.

## Required negative-conformance tests

RED→GREEN for: batch partially committing; a representative conceptual edit being forced into two persisted halves solely by an unjustified batch limit; compact response omitting required repair context; prioritization hiding an independent HK05 diagnostic; pagination causing duplicate/missed resources; stale conflict returning only an opaque/generic error that forces full-world reinspection; conflict recovery context anchored to the wrong expected/current revision or omitting a material changed resource; pathological per-field round-trip regression; batch/recovery path omitting provenance/validation; and reference-transport versus MCP drift for any HK08-added ergonomic primitive.

## Explicit concurrency boundary

H0 continues to use the accepted whole-world revision/hash CAS. HK08 makes stale conflicts cheap to recover from; it does **not** add per-resource locks, automatic merge of disjoint writers, distributed transactions, multi-process writer coordination or autonomous multi-agent scheduling.

Those mechanisms are deferred until after `WP-HK-GATE` unless measured HK08/GATE evidence proves that a representative single-client authoring flow cannot meet the accepted interaction budget without them. A demonstrated need must amend the owning workpack explicitly; concurrency machinery is not added speculatively.

Likewise, a logical transaction spanning several persisted batches is not part of H0 by default. Prefer a sufficiently expressive/capacious single atomic request for the representative intent. Add multi-plan atomicity only if measured product evidence proves that this is necessary rather than merely possible.

## Forbidden scope

Prompt engineering tied to one model vendor, natural-language planner, autonomous loop/orchestrator, gameplay content, per-resource/distributed locking, automatic merge of concurrent writers, multi-agent orchestration, or multi-plan transactional machinery without the explicit evidence/amendment described above.

## DoD

A measured reference client can complete representative authoring flows efficiently through stable protocol primitives, recover a stale plan through bounded structured context without full-world reconstruction in the ordinary gate case, keep one representative multi-resource intent atomic within the accepted batch/resource budget, and obtain semantically equivalent ergonomic behaviour through both reference transport and MCP; recorded performance/interaction baseline and independent PASS.
