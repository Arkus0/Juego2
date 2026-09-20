# WP-HK-08A Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-08A/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-08A`.
- Baseline: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`.
- Branch: `wp/hk-08a-efficient-interaction`.
- Direct predecessor: accepted `WP-HK-07B`.
- Reviewer-failed candidate: `0b835891d70666dab41846017e79eb3f7c3b311a`.
- Reviewer review: `5260042576` on PR #50.
- Repaired implementation/test SHA challenged here: `300c4b65bcbe1b547837dffc76e241c9500a9e0c`.
- GREEN Actions observation: `35499408516`; artifact `10601643061`.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Scope and predecessor check

The repair was constrained to the public provenance-read version boundary, its runtime routes, affected conformance tests and evidence. It does not change HK06A journal truth, HK06C replay semantics or HK08B conflict recovery. Existing batching, validation, provenance authority, compact reads and JSONL/MCP projection remain on their accepted paths.

## Findings 1–3 retained from the original Worker pre-review

The earlier Worker pre-review found and repaired three in-scope defects before the first freeze:

1. cursor offset context could be edited without an integrity guard;
2. ordinary modify evidence changed too little to prove the anti-chattiness claim;
3. an operation-level failure did not independently prove candidate-level world validation remained in the batch path.

Their causal tests remain GREEN: altered v2 cursor context fails closed; one coherent mutation changes multiple object properties in one request; and a parseable but globally invalid candidate cannot publish state or provenance.

## Finding 4 — Reviewer FAIL: v1 public semantics changed without a version bump

The independent Reviewer correctly identified that candidate `0b835891...` kept `authoring.journal.read@1.0` while changing its accepted HK06A meaning from `{}` -> complete journal to a default-50 paged read. A distinct page `schemaId` did not preserve the contract for existing v1 clients.

Repair:

- `authoring.journal.read@1.0` is restored to its accepted empty request schema, complete `arkus.authoring.journal@1` success shape and direct binding to the complete provenance authority;
- HK08A pagination is published as the breaking major version `authoring.journal.read@2.0` with revision/hash/limit/cursor request fields and the bounded page success contract;
- v2 retains cursor integrity, stale-anchor behavior and complete-page compatibility shaping;
- JSONL and MCP conformance clients now negotiate v1 or v2 explicitly and require both canonical tool identities;
- a 55-entry causal regression proves v1 `{}` returns all 55 while v2 default paging returns 50 + continuation;
- independently enumerated HK01 route-universe tests require both journal versions and compare route count to canonical definitions instead of freezing a pre-v2 magic total.

## False-green challenge after repair

The repaired surface was challenged for:

- accidentally routing v1 through the bounded service;
- allowing v1 paging fields despite the restored empty request schema;
- returning only the v2 default page under v1 with a large journal;
- exposing v2 in definitions without an independently discoverable route or MCP tool;
- letting JSONL and MCP negotiate different journal semantics;
- confusing a partial v2 page with a complete HK06A replay artifact;
- weakening stale cursor/anchor or cursor-integrity behavior while splitting routes;
- reopening HK06A journal meaning or HK06C replay;
- hard-coding predecessor route totals so a legitimate version addition causes a false regression.

No such blocker remains in the implementation/test SHA.

## Validation convergence

First repaired SHA `21b13bf571c7295d2907d972f2eaa95e9a4c5031` produced the desired build and focused result, but full regression found two stale HK01 numeric inventory assertions (expected 18/21, actual 19/22). This was a test-oracle maintenance issue caused by the legitimate new version identity, not a production contract failure. The repair replaced those magic totals with the actual completeness invariant and explicit v1/v2 presence checks.

Exact implementation/test SHA `300c4b65bcbe1b547837dffc76e241c9500a9e0c` then passed:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk08A*`: 13/13 GREEN;
- full regression: 173/173 GREEN;
- cross-transport: GREEN;
- foundational-proof/evidence/Worker-pre-review verifier gates: GREEN;
- candidate clean before/after: YES;
- Actions run `35499408516`: GREEN;
- artifact `10601643061`.

## Proof-budget conclusion

The fail-cycle changes only the version boundary that the Reviewer identified and the conformance needed to prove it. No generic hardening, second registry, replay reinterpretation, conflict-recovery machinery or speculative transport behavior was added. HK08B/HK09B boundaries remain intact.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary blocker remains. The resulting evidence-reconciliation commit is eligible for exact-SHA verification and freeze; a fresh independent Reviewer is still required after handoff.
