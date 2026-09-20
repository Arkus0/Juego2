# WP-HK-08 — Agent ergonomics + interaction efficiency

Status: SUPERSEDED — split before implementation into `WP-HK-08A` and `WP-HK-08B`  
Class: FOUNDATIONAL UMBRELLA RECORD  
Depends on: `WP-HK-07B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Historical objective

Make the harness efficient for real AI use, not merely correct: minimize unnecessary round trips, token volume and ambiguous repair loops while preserving explicit contracts.

This umbrella workpack was intentionally split before implementation because it combined two independently reviewable claims:

1. efficient protocol primitives for ordinary authoring; and
2. efficient recovery/repair behaviour plus the benchmark that measures the complete client workflow.

## Executable replacement chain

`WP-HK-08A → WP-HK-08B`

- `WP-HK-08A` owns atomic batching, compact/bounded projections, pagination (including journal paging), discovery cost metadata and cross-transport equivalence for those interaction primitives.
- `WP-HK-08B` owns structured stale-CAS recovery, diagnostic repair ergonomics, the representative client benchmark, measured interaction budgets and the explicit H0 concurrency boundary.

The aggregate intent of this record remains binding only through those replacement workpacks. **Do not implement or review `WP-HK-08` directly.**

## Preserved boundary

The split does not authorize per-resource locks, automatic merge of disjoint writers, distributed transactions, multi-process writer coordination, autonomous multi-agent scheduling or speculative multi-plan transaction machinery. Whole-world revision/hash CAS remains the accepted H0 concurrency model; `WP-HK-08B` makes ordinary same-lineage conflicts cheap to recover from rather than replacing that model.

## Historical downstream relation

Resource and persistence limits formerly described as part of the broad ergonomics/safety sequence are owned by the separately split `WP-HK-09A` / `WP-HK-09B` chain after `WP-HK-08B`.
