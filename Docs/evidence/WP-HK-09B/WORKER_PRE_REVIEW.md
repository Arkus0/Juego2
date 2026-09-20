# WP-HK-09B Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-09B/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-09B`.
- Baseline: `ada532f99282c96db15813eb17963bc9cb6d08fb`.
- Branch: `wp/hk-09b-resource-persistence`.
- Direct predecessor: accepted + DocSynced `WP-HK-09A`.
- Implementation checkpoint challenged: `e370606e4278da3e08602a3167c1cb6513bea93d`.
- GitHub Actions observation: run `35521120979`, focused HK09B 9/9, full regression 205/205, Release build 0 warnings / 0 errors.
- Fresh independent Reviewer is required after the final exact-SHA freeze; this document is Worker quality-gate evidence only.

## Scope/predecessor challenge

The pre-review consumes HK04 transaction authority, HK05 validation/diagnostics, HK06A provenance, HK06B snapshot semantics, HK06C replay, HK07 neutral transports, HK08A interaction shapes, HK08B recovery and HK09A host-capability containment. It reopens those only where HK09B's new resource/publication seam could contradict them.

No new mutation/rebase/replay authority was found. The resource layer rejects/advises; actual canonical publication remains in the existing Authoring authorities. HK09B remains process-local and does not claim WAL/fsync/power-loss, cloud/distributed persistence, engine/editor persistence or multi-writer coordination.

## Finding 1 — transport framing could pre-empt the intended neutral depth/resource semantics

During convergence, the reference JSONL framing/parser limit still reflected older framing assumptions. A transport-specific parser ceiling could therefore reject a request before the new neutral H0 depth/resource policy and make JSONL/MCP resource behavior diverge.

Repair:

- aligned reference framing allowance with the neutral H0 envelope plus protocol framing overhead;
- kept the canonical portable depth limit in `H0ResourcePolicy` as the semantic limit;
- added external-process JSONL/MCP equivalence coverage for envelope discovery, request bytes, page size and depth;
- added a deliberate resource-dimension drift injection proving the comparator turns red.

The adapter may still reject malformed/non-frame input earlier; that is outside canonical request semantics.

## Finding 2 — inherited HK01 inventory tests encoded the old one-base-capability count

Once `system.resource-envelope.describe@1.0` was added, full regression exposed three HK01 assertions that assumed `system.describe` was the only base definition. The production contract was correct; the old proof literals were stale.

Repair:

- base-contract test now explicitly requires both `system.describe@1.0` and `system.resource-envelope.describe@1.0`;
- synthetic scoped composition expects the additional base definition and still requires the scoped capability;
- scoped discovery explicitly requires all five accepted fixture/base versions including the resource-envelope capability;
- the tests retain their real invariant: canonical inventory/projection must expose every accepted route/version.

This was verified by the successful 205/205 full regression at `e370606e...`.

## Finding 3 — HK04 effective non-writer self-check had no request vector for the new base read route

The hidden-writer circuit breaker intentionally executes every effective public non-writer route against a live authoritative session and proves revision/hash cannot change. Adding a legitimate read-only base capability increased that independently enumerated universe; without a request vector, the guard correctly failed rather than silently shrinking.

Repair:

- added an empty-request vector for `system.resource-envelope.describe`;
- preserved the exact `nonWriterNames.SetEquals(requests.Keys)` completeness assertion;
- preserved before/after canonical revision/hash checks for every effective non-writer route.

The fix expands the evaluated universe; it does not weaken the HK04 guard.

## Architecture challenge

The complete changed surface was challenged for:

- adapter-only limits becoming a second semantic authority;
- public resource metadata diverging from runtime constants;
- old 64-operation capacity returning and splitting the accepted 96-op coherent edit;
- request/depth/page/payload limits returning generic schema errors or partial effects;
- materialized world/snapshot bypassing adapter estimates;
- execution expiry/cancellation occurring after state but before receipt/journal evidence;
- snapshot import publishing state before idempotency receipt/rebase evidence;
- replay publishing staged state before staged HK06A provenance;
- post-commit cancellation falsely reporting that no commit occurred;
- import receipt/session-history growth remaining unbounded;
- JSONL/MCP emitting different resource semantics;
- new resource-discovery route escaping route-universe/hidden-writer controls;
- the product-shaped Potes slice fitting abstract tests but not one real public transaction/checkpoint;
- accidental scope growth into power-loss durability, distributed storage, auth/security or H1 engine authority.

The focused negative matrix, product-shaped probe and residual-risk audit cover those owned classes. No in-scope blocker remains.

## Evidence reconciliation

Required final evidence is present and mutually consistent:

- `PROOF_MATRIX.md` — READY / 0 unresolved / 0 known-undetected / WITHIN_BUDGET;
- `NEGATIVE_CONFORMANCE_MATRIX.md` — required resource/import/interruption/transport negatives;
- `RESOURCE_ENVELOPE.md` — enforced machine-readable ceilings and HK08A/HK08B justification;
- `PERSISTENCE_INTERRUPTION.md` — exact process-local stage/publish proof boundary;
- `CONTENT_SHAPE_PROBE.md` — approved `Docs/art/VISUAL_BIBLE.md` Potes slice, 96 operations, checkpoint rebase;
- `RESIDUAL_RISK.md` — explicit non-claims/reopen conditions.

## Handoff condition

The branch is eligible for freeze only when the final evidence-bearing HEAD receives GREEN canonical observation, then the PR body records that exact SHA as both Candidate HEAD and Frozen candidate, `Branch frozen: YES`, and `scripts/hk09b-verify-exact-sha.sh <SHA>` is GREEN in the exact-SHA freeze job. No Worker commit may follow freeze.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary Worker blocker remains; fresh independent review is still required.
