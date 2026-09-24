# WP-H1-03A — Public Unity Editor execution seam + lifecycle

Status: COMPLETE / ACCEPTED
Class: FOUNDATIONAL PROCESS BOUNDARY
EXECUTION_REQUIREMENT: HYBRID / GITHUB_HOSTED_UNITY_ELIGIBLE
Depends on: `WP-H1-03` PASS
Blocks: `WP-H1-04`

Acceptance: frozen candidate `24d24526487af0d32e69799960f4251e44f0b5ad`; PR `#176`; independent PASS review `#5303016012`; merge `90428b803948820663abfebaa3fe21eb37596247`; Arkus Candidate Validation `35985858123` GREEN; H1-03A Unity Lifecycle `35985225199` GREEN; Arkus Main Safety `35985225228` GREEN; post-acceptance DocSync `Docs/evidence/WP-H1-03A/DOCSYNC.md`.

## Execution interpretation

`HYBRID` means remote contract/process tests plus effective pinned Unity worker evidence. After accepted `WP-H1-UNITY-CI`, that effective Unity evidence may run on the reviewed GitHub-hosted Unity substrate when the claim is fully machine-verifiable and does not require human visual inspection, interactive authoring, peripherals, private local source bytes or other physical/local-machine state.

The current H1-03A contract is eligible for that hosted path. Its acceptance oracle is process/lifecycle truth: launch, main-thread execution, normalized result equality, lease contention, cancellation, crash/timeout/result-corruption handling and restart recovery. It does not require Quaternius Source assets, scene appearance judgment or interactive Editor work. A physical owner PC is therefore **not** an acceptance prerequisite for H1-03A under the current contract.

This does not turn GitHub Actions into a product remote-editor topology and does not generalize later H1 claims that depend on local-only Quaternius source bytes or human visual/interactive evidence.

Binding process record: `Docs/evidence/WP-H1-UNITY-CI/DOCSYNC.md`.

## Objective and central claim

Implement the external Arkus-host → short-lived Unity batch-worker topology from `ADR-H1-004`, proving that the same admitted composed capability can be discovered and invoked through reference and MCP, reach Unity's main-thread Editor API boundary, and return truthful structured lifecycle outcomes without transferring canonical session or public-registry authority into Unity.

## WHY_THIS_BOUNDARY

H1-03 answers which host powers may exist; this WP answers how an admitted public call crosses the process boundary and what cancellation, interruption, restart and failure mean. Either claim can pass while the other is false. Catalogue and materialization behavior remain later feature owners, but neither may redesign this topology.

## Inherited guarantees

HK07A/HK07B neutral projection and generic cross-transport composition, HK09A host admission, HK09B process-local publication/resource semantics, H1-00 invocation/receipt vocabulary, H1-02 exact Unity launch substrate and H1-03 Unity host policy.

## New guarantees owned

- external .NET host ownership of the process-local canonical session and public composition;
- versioned project-bound invocation/result envelopes and fixed bootstrap profile;
- atomic project-local invocation ledger plus bounded public operation-status recovery;
- provider-owned typed worker executors mechanically reconciled with the admitted Editor-bound composed-handler universe;
- exact pinned Unity batch launch through one non-public entry point;
- serialized Unity main-thread execution under one project-operation lease;
- protocol stdout isolation from Unity diagnostics/logs;
- truthful pre-launch cancellation versus post-launch interruption/indeterminate-outcome semantics;
- structured editor crash, timeout, missing/corrupt result, wrong-profile and busy-project failures;
- public composed project/profile inspection and operation-status capabilities, with inspection exercising the real Editor worker through both reference and MCP.

## Explicitly not re-proved

H0 canonical operation semantics, generic MCP/JSONL projection correctness, Unity catalogue completeness, generation publication, checkpoint durability, multi-process coordination or OS/plugin sandboxing.

## Allowed scope

H1 host bootstrap/profile, project-operation lease, fixed Unity batch launcher, internal invocation/result envelope and ledger, Editor main-thread entry point, process/lifecycle diagnostics, lasting public project/profile inspection and operation-status capabilities, remote test double plus exact effective Unity evidence. For the current claim, the effective Unity evidence may use the accepted GitHub-hosted execution substrate.

## Forbidden scope

Hosting Arkus/MCP inside the Editor, long-lived daemon or network IPC, adapter-only Unity tools, parallel public registry, caller-selected executable/path/arguments/environment, catalogue feature behavior, scene/prefab/component materialization, canonical mutation authority, gameplay or remote editor service.

## Architecture / authority boundary

Reference and MCP both enter `arkus.neutral-projection@1`, H1 host-policy admission and the same canonical composed capability handler. Only that handler may create an internal Editor invocation. The Unity worker executes typed plans and returns evidence; it cannot discover or dispatch canonical capabilities, mutate canonical state, or decide that staging output is authoritative.

## Acceptance criteria

- one documented launch profile binds exact editor/toolchain, project identity/root, managed workspace, fixed entry point, platform and ceilings outside public request data;
- its H1 Unity operation ceilings are separately versioned/observable and leave the accepted HK09B H0 canonical-operation envelope unchanged;
- the external Arkus host remains the sole owner of canonical session/composition and Unity assemblies do not enter H0/canonical dependencies;
- reference and MCP discover the same project/profile inspection definition from composition and invoke it through the same handler into a real pinned Unity worker;
- the worker validates profile/request identity, acquires one project lease, enters Unity on the main thread, emits a normalized project/profile result atomically and exits;
- a second same-project invocation/host is serialized or fails with the same structured busy result across transports;
- pre-launch cancellation produces no worker; post-launch cancellation/crash/timeout/missing-or-corrupt result cannot become false no-effect or success evidence;
- a restarted host recovers completed/interrupted/indeterminate lifecycle status by invocation identity from the bounded ledger without using it as canonical state;
- worker stdout/stderr cannot corrupt JSONL/MCP framing, and raw Unity exceptions/process text do not become the public error contract;
- restarting the Unity worker and repeating the read-only inspection yields the same normalized result for the same effective project/profile;
- a synthetic admitted scoped capability traverses the seam without an adapter edit, while a capability absent from composition or rejected by H1-03 cannot launch Unity.
- removing an effective worker executor or its composed handler fails bootstrap/completeness proof instead of silently shrinking either universe.

## Deterministic proof / evidence

Remote tests exercise profile binding, composition, launcher substitution, lease/cancellation/result mapping, host-restart status recovery and reference↔MCP normalized conformance. Effective Unity evidence uses the exact H1-02 editor to launch two fresh workers, proves main-thread execution and normalized equality, forces nonzero exit, timeout, result corruption and concurrent lease contention, and records the exact launch/profile fingerprints without exposing them as caller authority. Under the accepted remote-Unity policy, this evidence may execute on GitHub-hosted Unity because every asserted outcome is machine-verifiable and no third-party game-asset or visual oracle is required.

## Causal negative-conformance classes

- MCP or reference adapter launches Unity without composed dispatch;
- Unity worker maintains a second public command/schema list;
- an Editor-bound composed handler or effective worker executor exists without its mechanically paired counterpart;
- public payload changes executable, project root, entry point or raw arguments;
- two workers mutate/read the same project concurrently despite the lease;
- Unity API call occurs off the Editor main thread;
- cancellation/crash reports no effect or success without a truthful result;
- host restart loses or misreports a completed/interrupted invocation;
- wrong invocation/profile result is accepted;
- Editor log output contaminates protocol stdout;
- canonical session/state is moved into or reconstructed from the Unity worker.

## Content-shape probe

Required because this WP adds public process/lifecycle semantics. Carry the H1-00 approved hierarchy-shaped diagnostic context through a test-scoped admitted invocation and the real project-inspection lifecycle to challenge envelope size, anchors, structured failures and restart/status behavior. It authorizes no catalogue or scene behavior and is not a substitute for later effective content proofs. It does not require the H1-04 Quaternius Source adoption.

## Dependency / IP implications

Consumes only the exact editor/packages accepted by H1-02. No IPC framework, daemon, hosted service or additional dependency may be adopted without its own reviewed record. No third-party game-asset source is required by this workpack.

## Residual risks

Batch-per-operation startup latency, editor licensing/availability, OS process failure, power loss, hostile plugins and multi-host concurrency remain outside this WP. A CI runner is only an evidence substrate; long-lived remote/editor-farm product execution remains outside scope. Feature-specific reconciliation remains with H1-05/H1-10.

## Exact predecessor reopen condition

Reopen HK07A/HK07B only if an admitted composed capability cannot preserve its neutral request/outcome meaning through both transports. Reopen H1-03 only if the accepted policy cannot admit the fixed launcher without caller-selectable ambient authority. Launcher/envelope/lifecycle defects remain H1-03A-owned.

## PASS consequence / next dependency

Accepted H1-03A freezes the only H1 public host-to-Editor topology and unblocks `WP-H1-04`. Every later Editor-bound public capability must consume this seam; changing the topology requires an explicit reviewed ADR/compatibility cycle.
