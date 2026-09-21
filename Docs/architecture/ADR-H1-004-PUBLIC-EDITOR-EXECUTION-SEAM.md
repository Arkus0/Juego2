# ADR-H1-004 — Public host to Unity Editor execution seam

Status: ACCEPTED — H1 planning PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`
Date: 2026-09-21

## Decision

H1 uses one fixed external-host topology:

1. the ordinary Arkus .NET host owns the process-local canonical authored session, canonical composition, `arkus.neutral-projection@1`, and the reference/MCP adapters;
2. after H1 host-policy admission, an editor-bound composed capability creates a versioned invocation envelope under the bootstrapped project workspace;
3. the host launches the exact pinned Unity Editor in batchmode with a fixed project path and one Arkus batch entry point;
4. that short-lived Unity worker executes Unity API work on the Editor main thread, writes one atomic structured result/receipt, and exits; and
5. the original composed capability maps that result back through the same neutral outcome contract used by reference and MCP.

The Unity worker owns Editor API execution only. It does not own canonical state, public capability discovery, MCP framing, or another command/schema registry. Its internal envelope carries the already-dispatched canonical capability identity/version, invocation/idempotency identity, project/toolchain fingerprint, relevant canonical/bridge anchors, and the typed execution plan produced by the capability owner. An Editor-bound provider owns its composed definition/handler and its typed worker executor together; bootstrap mechanically reconciles admitted Editor-bound handlers against effective executors, so a separate hand-maintained launcher list cannot shrink the public universe.

H1 does not host Arkus or MCP inside the Unity Editor, does not introduce a long-lived editor daemon or network IPC service, and does not allow a public caller to choose an executable, project path, entry point, environment variable, or raw Unity command.

## Process and session ownership

| Concern | Owner | Binding rule |
|---|---|---|
| canonical authored session, mutation, journal and snapshot/replay calls | external Arkus .NET host | accepted H0 semantics remain process-local until explicit H1-10 checkpoint/reconstruction |
| public capability/schema inventory and dispatch | canonical composed contract through neutral projection | reference and MCP cannot add an editor-only route |
| Unity project/editor binding | out-of-band H1 launch profile accepted by H1-02/H1-03 | exact editor identity, fixed project identity/root, managed workspace and batch entry point are not request fields |
| Unity API execution and serialization | short-lived Unity batch worker | Unity APIs run on the Editor main thread; imports, scene/prefab saves and refresh are serialized |
| projection/checkpoint authority | owning H1 capability and its accepted generation/checkpoint contract | a process exit or output file alone is never success evidence |

Reference and MCP hosts may be separate processes, as in accepted H0. They must build the same H1 composed inventory and dispatch implementation. Cross-transport proof runs sequentially against isolated project copies or against a checkpoint-restored baseline; two public hosts never concurrently write the same project.

## Bootstrap and single-instance lifecycle

The operator selects an H1 launch profile out of band. The profile binds a stable project identity to the exact Unity editor/toolchain fingerprint, project root, managed workspace roots, platform, fixed batch entry point, operation ceilings and diagnostic/log locations. Public inspection exposes the effective profile fingerprint, not unrestricted native paths or launch authority. Unity operation ceilings are a separately versioned H1 profile derived from measured batch startup/execution; they do not silently widen or relabel HK09B's accepted H0 5-second cooperative canonical-operation envelope.

Only one editor-bound invocation may hold the project-operation lease. A second invocation or another host targeting the same project fails closed with a structured busy/lease diagnostic; H1 claims neither multi-host coordination nor parallel Unity mutation. Worker stdout/stderr is captured as diagnostic evidence and never shares JSONL/MCP protocol stdout.

Every editor-bound invocation is restartable because the Unity worker is intentionally short-lived. The managed workspace keeps an atomic invocation ledger containing the request identity, phase transitions and final result/receipt. On startup the worker validates the request identity, profile/toolchain fingerprint and project lease before touching Unity state. On completion it flushes owned serialization/import effects, writes the result atomically, releases the lease and exits. A restarted host can query/reconcile that ledger by invocation identity without treating it as canonical state.

## Cancellation, interruption and recovery

- Cancellation before editor launch retains accepted H0 admission semantics and produces no Unity invocation.
- After editor launch, cancellation is cooperative at capability-owned phase boundaries. The host may not report a no-effect cancellation after an Editor effect may have occurred.
- If a truthful completion receipt is recovered, that completion wins. Otherwise termination, timeout, missing/corrupt result, editor crash or lost worker maps to a stable structured `unity.execution.*` failure with invocation identity, last verified phase and `outcome: indeterminate` where effect publication cannot yet be excluded.
- Retry/reconciliation uses the same invocation/idempotency identity plus the owning capability's accepted receipt/publication contract. A bounded public operation-status read exposes completed/interrupted/indeterminate lifecycle state without acquiring mutation authority. Staging files, exit code zero, console text or an orphaned output are never sufficient to infer success.
- Before H1-05, the seam proves lifecycle with read-only project inspection only. H1-05 owns recovery against generational materialization; H1-10 owns host restart from a project checkpoint and clean Unity rebuild. H1-03A does not pretend to provide WAL/fsync or canonical crash recovery.

## Public capability ownership

The execution seam is internal, but the operations that use it are public composed capabilities:

| Workpack | Minimum public composed capability roles |
|---|---|
| H1-03A | project/profile inspection that executes through the real Editor worker, plus bounded operation-status recovery |
| H1-04 | bounded catalogue query/get and fingerprint inspection |
| H1-05 | projection plan, materialize and normalized observe; Editor-bound materialize/observe phases use H1-03A |
| H1-08 | current/proposed Unity validation |
| H1-09 | drift inspection, rematerialization and supported import-proposal compilation; Editor-bound observation/rematerialization uses H1-03A |
| H1-10 | project checkpoint and clean rebuild/reconstruction; Editor-bound checkpoint/rebuild phases use H1-03A |

Each owner freezes its exact versioned capability keys/schemas under HK01 rules. Acceptance requires the capability to enter canonical composition and therefore project generically through the reference transport and MCP. A private editor menu, test helper, direct batch script or adapter-only MCP tool cannot satisfy the role. `WP-H1-GATE` only discovers and composes these accepted operations; it may not add a missing public route.

## Why

The accepted H0 host is process-local and explicitly excludes Unity. Leaving H1 free to choose later between in-Editor hosting, an external batch launcher and a long-lived IPC service would leave session authority, main-thread rules, framing, cancellation, restart and error meaning undecided until feature implementation or the closure Gate.

The external host plus short-lived batch worker preserves the accepted H0 process and transport boundaries, keeps Unity dependencies out of the canonical host, uses Unity's deterministic batch surface, prevents MCP stdout from mixing with Editor logs, and avoids making a new daemon/IPC registry foundational. The cost is editor startup latency, which H1 records as a residual rather than hiding behind an unreviewed long-lived service.

## Consequences

- H1-03 continues to own which Unity powers are admitted; H1-03A independently owns how admitted public calls reach and survive the Editor process boundary.
- H1-04 and every later Unity feature consume H1-03A instead of choosing another topology.
- Feature WPs own their public capability semantics and effect reconciliation; H1-03A owns only the shared execution/lifecycle boundary and structured process failure mapping.
- H1 has no concurrent multi-host, remote-editor, long-lived daemon, arbitrary executable, or hostile-plugin isolation claim.
- Replacing batch-per-operation later requires a new ADR and compatibility proof; it cannot be an invisible implementation swap if lifecycle/error semantics change.
