# WP-HK-09B Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-09B/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-09B`.
- Baseline: `ada532f99282c96db15813eb17963bc9cb6d08fb`.
- Branch: `wp/hk-09b-resource-persistence`.
- Direct predecessor: accepted + DocSynced `WP-HK-09A`.
- First frozen candidate: `dcea0901a4fe78549d81271a7f192bbae77e58b7`.
- Independent Reviewer FAIL: review `#5261028914`, blocker limited to unversioned HK07A JSONL framing/error drift.
- Post-review repair checkpoint challenged: `7762d4ef0da2bbce5b5b5a1cc008c56b2bc9cd78`.
- GitHub Actions exact-SHA validation: run `35522382987`, canonical frozen-candidate verifier GREEN.
- Code-only repair checkpoint `dadb0168ab014cb5155aef28315fd6dc7a8a2dba` also passed exact-SHA validation in run `35522285009` before evidence reconciliation.
- Fresh independent Reviewer is required after the final refreeze; this document is Worker quality-gate evidence only.

## Scope/predecessor challenge

The pre-review consumes HK04 transaction authority, HK05 validation/diagnostics, HK06A provenance, HK06B snapshot semantics, HK06C replay, HK07 neutral transports, HK08A interaction shapes, HK08B recovery and HK09A host-capability containment. It reopens those only where HK09B's new resource/publication seam can concretely contradict them.

The first review supplied exactly such evidence for HK07A: the candidate had kept protocol id `arkus.reference.jsonl@1` while widening its frame ceiling from 1 MiB to 2 MiB, changing the inherited oversized-frame machine code, and modifying the predecessor regression oracle to follow the new behavior. That finding is owned and repaired by HK09B; HK07A itself is not redesigned.

No new mutation/rebase/replay authority was found. The resource layer rejects/advises; actual canonical publication remains in the existing Authoring authorities. HK09B remains process-local and does not claim WAL/fsync/power-loss, cloud/distributed persistence, engine/editor persistence or multi-writer coordination.

## Finding 1 — independent review caught an unversioned HK07A framing regression

The original HK09B implementation coupled `ReferenceTransportHost.MaximumFrameBytes` to the new `H0ResourceEnvelope`, widened the frozen `arkus.reference.jsonl@1` frame to 2 MiB and changed the oversized-frame error from `transport.frame_too_large` to `resource.request_bytes_exceeded`. It also changed the inherited HK07A test to derive its oracle from the new HK09B constant. That made the suite falsely green while changing a predecessor public transport contract in place.

Repair:

- commit `6b35791049bbfe4a3fcda7f65562337671504d95` restores `src/Arkus.Harness.Cli/ReferenceTransport.cs` exactly to its accepted baseline blob;
- the same commit restores `tests/Arkus.Harness.Tests/Hk07AReferenceTransportTests.cs` exactly to its accepted baseline blob, including the literal 1 MiB + 1 probe and `transport.frame_too_large` oracle;
- as a result, neither file remains in the effective PR changed-file set;
- commit `dadb0168ab014cb5155aef28315fd6dc7a8a2dba` instead fits the new neutral envelope under the inherited transport: 896 KiB canonical arguments and 640 KiB canonical world/snapshot bytes, while machine-readable compatibility reports the inherited 1 MiB frame boundary;
- 640 KiB expands to at most 873,816 base64 characters, leaving 43,688 bytes inside the canonical-argument cap for import metadata before framing overhead;
- the existing external-process JSONL/MCP byte-limit comparison now reaches the neutral `resource.request_bytes_exceeded` boundary while still fitting inside the frozen JSONL frame, so moving the canonical byte probe beyond framing would turn the conformance test red.

This is a version/compatibility repair, not a new transport version and not a redesign of persistence.

## Finding 2 — inherited HK01 inventory tests encoded the old one-base-capability count

Once `system.resource-envelope.describe@1.0` was added, full regression exposed three HK01 assertions that assumed `system.describe` was the only base definition. The production contract was correct; the old proof literals were stale.

Repair:

- base-contract test explicitly requires both `system.describe@1.0` and `system.resource-envelope.describe@1.0`;
- synthetic scoped composition expects the additional base definition and still requires the scoped capability;
- scoped discovery explicitly requires all accepted fixture/base versions including the resource-envelope capability;
- the tests retain their real invariant: canonical inventory/projection must expose every accepted route/version.

## Finding 3 — HK04 effective non-writer self-check had no request vector for the new base read route

The hidden-writer circuit breaker intentionally executes every effective public non-writer route against a live authoritative session and proves revision/hash cannot change. Adding a legitimate read-only base capability increased that independently enumerated universe; without a request vector, the guard correctly failed rather than silently shrinking.

Repair:

- added an empty-request vector for `system.resource-envelope.describe`;
- preserved the exact `nonWriterNames.SetEquals(requests.Keys)` completeness assertion;
- preserved before/after canonical revision/hash checks for every effective non-writer route.

The fix expands the evaluated universe; it does not weaken the HK04 guard.

## Finding 4 — evidence originally described the regressed transport as intentional

The original `RESOURCE_ENVELOPE.md`, `RESIDUAL_RISK.md` and Worker plan described a 2 MiB JSONL ceiling as part of HK09B, which would have perpetuated the same compatibility error even after code repair.

Repair:

- evidence now states that HK07A remains authoritative for `arkus.reference.jsonl@1` framing/error semantics;
- the 896 KiB request and 640 KiB snapshot/world limits are derived to fit below that inherited framing boundary;
- residual risk explicitly distinguishes physical transport failures from successfully framed neutral resource semantics;
- the Worker plan records the Reviewer FAIL and repair commits instead of erasing the failed cycle.

## Architecture challenge

The repaired surface was challenged for:

- another attempt to make adapter framing a second semantic authority;
- public resource metadata diverging from runtime constants or the inherited JSONL contract;
- request limits that cannot actually traverse JSONL @1 while remaining advertised as transport-neutral;
- snapshot size whose base64 representation cannot fit the canonical request/frame envelope;
- old 64-operation capacity returning and splitting the accepted 96-op coherent edit;
- request/depth/page/payload limits returning generic schema errors or partial effects;
- materialized world/snapshot bypassing adapter estimates;
- execution expiry/cancellation occurring after state but before receipt/journal evidence;
- snapshot import publishing state before idempotency receipt/rebase evidence;
- replay publishing staged state before staged HK06A provenance;
- post-commit cancellation falsely reporting that no commit occurred;
- import receipt/session-history growth remaining unbounded;
- JSONL/MCP emitting different resource semantics for successfully framed requests;
- new resource-discovery route escaping route-universe/hidden-writer controls;
- the product-shaped Potes slice fitting abstract tests but not one real public transaction/checkpoint;
- accidental scope growth into power-loss durability, distributed storage, auth/security or H1 engine authority.

The focused negative matrix, product-shaped probe, inherited HK07A regression and residual-risk audit cover those owned classes. No in-scope Worker blocker remains.

## Validation reconciliation

Post-review repair validation is GREEN:

- `dadb0168ab014cb5155aef28315fd6dc7a8a2dba` — code repair exact-SHA run `35522285009`: GREEN;
- `7762d4ef0da2bbce5b5b5a1cc008c56b2bc9cd78` — code + reconciled repair evidence exact-SHA run `35522382987`: GREEN;
- both runs executed the canonical frozen-candidate verifier, including Release build, focused HK09B resource/persistence/transport/content-shape gates, full regression, foundational proof and evidence reconciliation;
- effective PR diff no longer contains `ReferenceTransport.cs` or `Hk07AReferenceTransportTests.cs`, proving the predecessor implementation/oracle were restored rather than edited into a new meaning.

Required final evidence is mutually consistent:

- `PROOF_MATRIX.md` — READY / 0 unresolved / 0 known-undetected / WITHIN_BUDGET;
- `NEGATIVE_CONFORMANCE_MATRIX.md` — required resource/import/interruption/transport negatives;
- `RESOURCE_ENVELOPE.md` — inherited JSONL compatibility plus enforced neutral ceilings;
- `PERSISTENCE_INTERRUPTION.md` — exact process-local stage/publish proof boundary;
- `CONTENT_SHAPE_PROBE.md` — approved `Docs/art/VISUAL_BIBLE.md` Potes slice, 96 operations, checkpoint rebase;
- `RESIDUAL_RISK.md` — explicit non-claims/reopen conditions.

## Handoff condition

This pre-review makes the branch eligible for a new evidence-only refreeze. The final evidence-bearing HEAD must itself receive GREEN exact-SHA validation, then the PR body must record that exact SHA as Candidate/Frozen candidate, set `Branch frozen: YES`, and point to the new validation run. No Worker commit may follow that refreeze.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary Worker blocker remains; fresh independent review is still required.
