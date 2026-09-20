# WP-HK-08A — Efficient interaction primitives

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-07B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Completion metadata

- implementation PR: `#50`;
- baseline SHA: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`;
- reviewed frozen candidate: `324102d40fa0b7e36c7320216914f9d3fddadcc8`;
- independent Reviewer verdict: `PASS` (review `#5260092080`);
- exact-SHA freeze validation: GREEN, Actions `35499569973`, artifact `10601679693`;
- implementation merge SHA: `eb9d3df58df1114abafaca435da64683fdb1480e`.

Accepted semantics: representative coherent authoring can carry 96 mixed object/extension operations in one accepted HK04 transaction, with one revision advance and one HK06A provenance entry; compact reads reuse accepted projection semantics while retaining required anchors; `authoring.journal.read@1.0` preserves the accepted HK06A empty-request complete-journal contract; bounded deterministic journal pagination is exposed only through explicit breaking-version negotiation as `authoring.journal.read@2.0`, with continuation bound to the authored journal context and altered/stale cursors failing closed; canonical discovery exposes relative cost/side-effect/batching information; and every HK08A-changed public interaction shape is semantically equivalent through the accepted JSONL reference transport and MCP projection.

The first frozen candidate `0b835891d70666dab41846017e79eb3f7c3b311a` failed independent review `#5260042576` because it changed `authoring.journal.read@1.0` from complete-journal semantics to default-bounded pagination without a version increment. The accepted repair restores v1 exactly at the public boundary and introduces v2 for the breaking paged contract, without reopening HK06A journal truth, HK06C replay semantics or HK08B recovery ownership.

## Objective

Make ordinary AI authoring efficient through stable protocol primitives: atomic batching, compact/bounded reads, pagination and cheap capability discovery, while preserving the accepted canonical semantics and transport neutrality.

## Acceptance

- Batch authoring supports multiple related changes in one canonical transaction without weakening HK04 atomicity, HK05 validation or HK06A provenance.
- Batch capacity/shape is evidence-driven rather than treated as an arbitrary semantic constant. A representative coherent multi-resource authoring intent must fit in one accepted atomic batch under the later HK09B resource envelope. The current 64-operation request limit is a starting implementation constraint, not a permanent product boundary.
- If the representative coherent edit cannot fit safely, HK08A must justify and adjust the request shape/limit rather than silently split one atomic intent into independently persisted halves.
- Read APIs support bounded projections/pagination and compact machine responses where the accepted complete read semantics permit it.
- The accepted HK06A provenance journal read becomes bounded/paginated without changing journal meaning, ordering or completeness semantics.
- Pagination/cursors do not duplicate or omit resources when the accepted read anchor remains valid and fail closed under the accepted stale-anchor rules when it does not.
- Discovery distinguishes cheap reads, expensive reads and mutating operations where relevant so a client can choose an economical path without source-code knowledge.
- Common authoring flows do not require one API call per field/property when one coherent request can express the intent.
- Compact/batch/pagination behaviour is semantically equivalent across the accepted reference transport and MCP projection. HK08A reruns cross-transport conformance for every interaction primitive it changes or adds; HK07B PASS is not treated as proof for later-added semantics.
- Compact/batch/pagination paths cannot skip canonical validation/provenance, invent a second semantic registry or change request/result/error meaning between transports.
- Responses avoid echoing unnecessary implementation/internal state by default while retaining all contractually required data.

## Required negative-conformance tests

RED→GREEN for:

- batch partially committing;
- a representative conceptual edit being forced into two persisted halves solely by an unjustified batch limit;
- batch path omitting validation or provenance;
- compact response omitting a contractually required field needed to interpret the result;
- pagination causing duplicate/missed resources or journal entries;
- stale pagination continuing against a mismatched anchor rather than failing closed;
- pathological one-field-per-request behaviour for the representative ordinary authoring flow; and
- reference-transport versus MCP semantic drift for any HK08A-added primitive.

## Explicit boundary

HK08A does **not** own stale-CAS recovery semantics, diagnostic repair prioritization or final interaction budgets; those belong to `WP-HK-08B`, which consumes these primitives in the complete measured workflow.

A logical transaction spanning several persisted batches is not part of H0 by default. Prefer a sufficiently expressive/capacious single atomic request for the representative intent. Multi-plan atomicity requires separate measured product evidence and explicit roadmap amendment.

## Forbidden scope

Structured stale-conflict recovery, automatic merge of concurrent writers, per-resource/distributed locking, autonomous multi-agent orchestration, final agent benchmark/budget setting, model-vendor prompt engineering, natural-language planner/orchestrator, gameplay content, or speculative multi-plan transaction machinery.

## DoD

A client can perform representative ordinary create/inspect/modify flows using bounded, compact and batched stable primitives without per-field chattiness, with one representative coherent multi-resource edit remaining atomic and with equivalent semantics through reference transport and MCP; independent PASS.
