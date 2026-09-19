# WP-HK-03 — Inspection/query surface

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-02` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `39b0660193af53575aab26d3e5ff567a48ea86f3`
Implementation PR: `#18`

Completion:
- Reviewed candidate SHA: `8c20a380003c082fa9bd472d3233afa9654fb231`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5255545669`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35438687868`)
- Merge SHA: `d8b808450ee7863726d718a25c5756534113fcfd`
- Completed: `2026-09-19`

## Objective

Make accepted world state completely inspectable by an AI through bounded, deterministic, schema-described reads.

## Acceptance

- Public read commands cover world summary, entity/resource lookup, references/relationships and filtered queries needed to reconstruct relevant state.
- Query/filter language is explicit, typed and bounded; no arbitrary code/expression execution.
- Pagination/cursors are deterministic and stable for an unchanged revision.
- Every read response identifies the world revision/hash it describes.
- Missing resources, invalid selectors and stale cursors return structured errors.
- There is a completeness argument showing no authorable state required for safe later mutation is hidden from the inspection surface.
- Large results support projection/field selection or equivalent bounded output without creating a second semantic truth.
- Reads are side-effect free and repeatable against the same revision.

## Required self-attacks

RED→GREEN for: hidden authorable field, nondeterministic query order, unbounded result path, stale cursor/revision, selector escaping its declared grammar, and discovered read command missing schema.

## Forbidden scope

Mutation/apply, Unity, gameplay implementation, natural-language query interpretation.

## DoD

A client can reconstruct the complete micro-world semantics required for future edits using only discovered read commands; independent PASS.
