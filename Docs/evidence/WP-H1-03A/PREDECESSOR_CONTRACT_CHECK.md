# WP-H1-03A — PREDECESSOR_CONTRACT_CHECK

Status: ACTIVE Worker evidence
Baseline main: `50461f813ecef55f832fc5d26eba2fde2e829b6a`

## Direct accepted dependency

`WP-H1-03` is COMPLETE / ACCEPTED.

- frozen candidate: `6d78ebc07e48419e279d4a17b93eeb099307316b`
- canonical implementation PR: `#168`
- independent PASS review: `#5299906832`
- implementation merge: `b52fe8f67bb74880af8ed2d734c1293869fd9f86`
- exact-SHA Candidate Validation: Actions `35956034910` GREEN
- H1-03 Unity Host Policy: Actions `35955397660` GREEN
- Arkus Main Safety: Actions `35955397574` GREEN

Authoritative sources opened for this Worker: `Docs/evidence/WP-H1-03/DOCSYNC.md`, `Docs/workpacks/H1/WP-H1-03A.md`, `src/Arkus.Harness.Projection/H1UnityHostPolicy.cs`, and `src/Arkus.Harness.Projection/NeutralProjection.cs`.

## Effective Unity substrate consumed

`WP-H1-UNITY-CI` is COMPLETE / ACCEPTED.

- exact candidate: `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf`
- independent PASS: `#5299167558`
- PR: `#166`
- implementation merge: `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`
- accepted pilot run: `35936805407`
- exact Unity: `6000.3.24f1 (4e7b9b5b6244)`

The accepted hosted substrate may be used for H1-03A because this workpack's acceptance oracle is fully machine-verifiable and requires neither Quaternius source bytes nor visual/interactive judgment.

## Inherited guarantees consumed, not re-proved

- H0 remains the canonical session/composition and transport-neutral dispatch authority.
- `arkus.unity-host-policy@1` is the separate H1 admission boundary; raw Unity-host effects remain rejected by the public H0 admission path.
- H1 admission is bound to the bootstrapped `ArkusUnity` project/workspace and reviewed per-capability resource/time grants; public transport data cannot choose executable, project root, endpoint or ambient host authority.
- the admitted `NeutralProjectionService` retains the exact accepted H1 admission object containing the composed contract, workspace and immutable reviewed grant snapshot specifically for this lifecycle layer.
- JSONL/reference and MCP remain projections over the same neutral/composed capability surface and cannot mint adapter-only Unity power.
- H1-02 fixes the Unity project/editor substrate; H1-03A does not reopen editor selection or package adoption.

## Guarantees newly owned by H1-03A

- a fixed versioned launch profile and separately versioned H1 Unity operation ceilings;
- project-bound invocation/result envelopes and a bounded durable invocation ledger used only for lifecycle recovery;
- mechanically complete pairing between admitted Editor-bound composed handlers and typed worker executors;
- exact short-lived Unity batch launch through one non-public worker entry point;
- one same-project operation lease and Unity-main-thread execution;
- truthful pre-launch cancellation versus post-launch interruption/indeterminate outcomes;
- structured busy/crash/timeout/missing-result/corrupt-result/wrong-profile failures without leaking raw process text as public contract;
- restart recovery by invocation identity and deterministic repeated read-only inspection;
- public project/profile inspection and operation-status capabilities traversing the same composed handler path through reference and MCP;
- protocol stdout isolation from Unity diagnostics;
- the required H1-00 hierarchy-shaped diagnostic context carried through the lifecycle as a bounded test probe.

## Reopen conditions

Reopen H1-03 only if concrete implementation/effective evidence shows its accepted admission cannot authorize the fixed launcher without caller-selectable ambient authority, or the retained admission token does not actually preserve the workspace/grants needed by the effective path.

Reopen HK07A/HK07B only if the effective seam cannot preserve the already accepted neutral request/outcome meaning across both transports.

Launcher, envelope, executor completeness, lease, lifecycle mapping, result integrity, restart recovery and Unity-worker defects remain H1-03A-owned and do not reopen predecessors.

H1-04 catalogue/materialization/source semantics are explicitly out of scope and remain untouched.
