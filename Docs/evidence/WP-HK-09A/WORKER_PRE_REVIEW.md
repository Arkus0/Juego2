# WP-HK-09A Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-09A/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-09A`.
- Baseline: `b2a666554e88a543013ece8751ab781637f7db55`.
- Branch: `wp/hk-09a-capability-containment`.
- Direct predecessor: accepted + DocSynced `WP-HK-08B`.
- Failed reviewed candidate repaired: `1c85a64a5d3930ad2e39451fb8db2c1fac6ae78b`, Reviewer FAIL #5260422246.
- Fresh independent Reviewer is required after a new exact-SHA freeze; this document is Worker quality-gate evidence only.

## Scope/predecessor challenge

The pre-review treats the canonical capability authority, mutation/rebase/replay semantics and neutral JSONL/MCP projection guarantees as accepted predecessors. It reopens them only if HK09A's effective host path supplies contradictory evidence. No predecessor contradiction was found.

HK09A remains limited to repository-local H0 host-capability containment. Final resource quotas and crash/interruption persistence remain HK09B; Unity/editor authority remains H1+.

## Finding 1 — HK07A reference documentation still advertised caller-selected file authority

The first implementation correctly rejected production `--file` before the framing host, but the accepted HK07A reference page still listed `--file PATH` as a supported launch mode. That would make the public host contract internally contradictory.

Repair:

- preserved the historical statement that HK07A originally proved file framing;
- added an explicit HK09A containment note;
- removed `--file` from the current production supported-mode table;
- documented usage exit 64 and stdin/stdout as the supported H0 path.

No protocol/canonical semantics were changed by this documentation repair.

## Finding 2 — initial Worker plan no longer matched the safer implemented design

The initial plan proposed both a redundant dispatch-time policy re-check and retaining one-shot file mode inside a path sandbox. Implementation deliberately chose a smaller authority surface instead: policy admission on the composed H0 surface and no production caller-selected file authority at all.

Repair:

- reconciled `WORKER_PLAN.md` with the implemented design;
- documented why policy belongs at the projection-creation boundary rather than on every dispatch;
- documented why zero production file-path authority is preferable to implementing a partial path sandbox/TOCTOU surface that H0 does not need.

## Finding 3 — initial metadata oracle could miss false privilege/provenance claims

The first `H0HostCapabilityPolicy` rejected external/elevated authority and checked side-effect/transaction consistency, but a canonical state-changing capability could still have been mislabeled `PublicRead` or published without provenance `Required` while satisfying the transaction check. That is a foundational false-green class because HK09A explicitly owns truthful mutating/read-only/privileged metadata.

Repair:

- canonical mutation/rebase/replay now require `PrivilegeClass.Authoring`;
- canonical mutation/rebase/replay now require `ProvenanceRequirement.Required`;
- unknown privilege is rejected from the effective H0 inventory;
- dedicated executable negatives cover state-changing authority masquerading as public read and dropped provenance metadata;
- no accepted canonical definition was weakened to satisfy the policy.

## Finding 4 — independent review found a public composition → projection route that skipped H0 admission

The first frozen candidate enforced `H0HostCapabilityPolicy` only in `CanonicalWorldContract.Compose`. That was insufficient because HK01's public `ContractComposer.Compose` legitimately remains generic and HK07A's public `NeutralProjectionService(ComposedContract)` accepted any successful composed contract. A transport could therefore consume canonical composition exactly as HK07 requires while omitting the new HK09A policy step.

Repair:

- `NeutralProjectionService` now runs `H0HostCapabilityPolicy.Enforce` in its constructor before retaining the contract or exposing `Capabilities`;
- `ContractComposer` remains generic and unchanged;
- `Hk09ATransportPolicyBoundaryTests.TransportPathCannotProjectCompositionThatSkippedHostCapabilityAdmission` first proves that an H0-forbidden external-side-effect fixture composes successfully, then attempts the exact public projection constructor and requires fail-closed rejection before handler invocation;
- `NEGATIVE_CONFORMANCE_MATRIX.md`, `PROOF_MATRIX.md`, `RESIDUAL_RISK.md` and `WORKER_PLAN.md` now describe the real non-skippable boundary rather than relying on happy-path production composition.

This is a local HK09A repair. It does not reopen HK07A: the accepted projection remains canonical/generic in capability identity, schemas and dispatch; HK09A adds a mandatory host-power admission check below the transport.

## False-green challenge

The reconciled candidate has been challenged for:

- policy proving only its own registry while an unregistered host-power path exists;
- direct public `ContractComposer` → `NeutralProjectionService` construction skipping host admission;
- traversal or real symlink file escape;
- shell/process capability invented by protocol payload;
- URL/network-shaped payload causing an outbound connection;
- `$type`-style selector causing runtime activation;
- adapter-only filesystem/elevated capability absent from canonical composition;
- external/elevated/unknown policy semantics entering the H0 inventory;
- mutation/rebase/replay metadata lying about transaction, privilege or provenance;
- containment breaking ordinary inspect/author/snapshot/import/replay on a representative Juego2 content slice;
- evidence claiming a stale file sandbox or per-dispatch policy mechanism that the implementation does not contain;
- accidental scope drift into HK09B quotas/crash consistency or H1 engine authority.

Required causal controls are enumerated in `NEGATIVE_CONFORMANCE_MATRIX.md`; representative product proof is in `CONTENT_SHAPE_PROBE.md`; trust-boundary assumptions and accepted out-of-scope residuals are in `RESIDUAL_RISK.md`.

## Strict repair pre-review result

The complete baseline→repair diff was re-read against WP-HK-09A acceptance and DoD. The repair is placed at the exact seam identified by independent review, does not add a second capability registry, leaves `ContractComposer` generic, and causes policy failure before transport-visible capability exposure or canonical handler dispatch. No additional in-scope alternate projection constructor or production adapter-owned authority was found in the reviewed surfaces.

## Handoff condition

The branch is eligible for freeze only when:

- canonical observation is GREEN on the exact branch HEAD with all `Hk09A` tests and full regression;
- `scripts/hk09a-verify-exact-sha.sh` is GREEN on the exact frozen SHA;
- foundational proof markers remain READY / 0 unresolved / 0 known-undetected / WITHIN_BUDGET;
- PR body records the same frozen candidate SHA as HEAD;
- no Worker commits follow freeze.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary Worker blocker remains; fresh independent review is still required.
