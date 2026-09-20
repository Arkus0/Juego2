# WP-HK-07A Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 4
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-07A/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-07A`.
- Baseline: `9173dcef32f6e020b64c3db2816fe5f4d0994058`.
- Branch: `wp/hk-07a-headless-host`.
- Direct accepted dependency: `WP-HK-06C`.
- Implementation/test/evidence SHA observed GREEN: `9632884059abf483254e7f15eb01e1083c6e1762`.
- Remote PR/Actions observation: pending publication; the current execution environment rejected the remote write, so this report does not claim a GitHub run or frozen handoff.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Architecture and contract challenged

The complete baseline-to-implementation diff was re-read against `WP-HK-07A`, `WORKER_REVIEW_PROTOCOL.md`, `FOUNDATIONAL_PROOF_STANDARD.md`, the accepted HK01-HK06C public contract/state/history semantics, and downstream HK07B/HK08/HK09 ownership.

The candidate was challenged specifically for:

- whether the neutral layer contained JSONL/MCP/stdin/stdout assumptions or merely renamed a transport envelope;
- whether the CLI or projection maintained a second command/schema registry that could omit scoped capabilities;
- whether production discovery and dispatch came from the accepted composed `ComposedContract` rather than a base-only or direct-authoring path;
- whether Runtime or the neutral assembly acquired a dependency on CLI/JSON serialization;
- whether reads and writes could be bound accidentally to different portable aggregates;
- whether canonical success/error/version/schema meaning survived projection unchanged;
- whether cancellation/timeout could report failure after a canonical effect committed;
- whether malformed, genuinely truncated, oversized, invalid-UTF8 and multiple-frame inputs had distinct stable fail-closed behavior;
- whether diagnostics, usage or fatal text could corrupt stdout;
- whether ambient environment values could select hidden world/registry/capability behavior;
- whether file/stdin/stream modes and exit codes were explicit and reproducible;
- whether a source-linked test was being passed off as a fresh external client;
- whether the Potes/Liébana authored slice traversed the real process boundary and a second fresh process;
- whether the project/proof universe covered every actual solution participant after adding Projection;
- whether implementation leaked into MCP, HK08 efficiency/recovery, HK09 policy/resource limits, Unity, networking or gameplay state.

## Findings fixed during Worker pre-review

**Finding 1 — incomplete JSON was conflated with generic malformed JSON.**

The first codec mapped `{` at frame end to `transport.malformed_json`; the only `transport.truncated_frame` control was empty one-shot input. That did not causally distinguish the WP's truncated-frame class. The codec now independently probes whether strict JSON parsing needs more bytes, returns `transport.truncated_frame` for incomplete values, retains `transport.malformed_json` for impossible syntax, and has separate real-process assertions for both.

**Finding 2 — readiness diagnostics were emitted before production composition completed.**

`--diagnostics` originally printed `ready` before `ProductionHarnessHost.Create()`. A composition failure could therefore emit a false readiness claim immediately before exit 70. The message now occurs only after the canonical projection service is constructed.

**Finding 3 — host composition sat too high and created a decorative proof dependency.**

The initial Projection assembly constructed `WorldState`, `PortableWorldAuthoringSession` and `WorldInspectionService` directly. The compiler required a direct Validation reference while the emitted projection assembly did not causally reference it; the effective dependency oracle correctly turned RED. More importantly, this left adapter-level code responsible for pairing read and mutation services. The repair adds one narrow Runtime-owned `ComposeEmptyPortableSession(worldId)` seam, which constructs and pairs one session through the existing canonical composer. Projection now depends only on Protocol + Runtime, and the production adapter cannot accidentally bind reads/writes to different aggregates.

**Finding 4 — the fixed repository/project universe had inherited omissions.**

Adding Projection and running the independently bootstrapped oracle exposed that the fixed HK00 graph had never been reconciled after HK04/HK05: the already-tracked mutation negative-control fixture was outside the project/source universe, the Tests→fixture edge was absent, and Authoring→Validation was undeclared. The fixed contract now includes every solution project and actual direct edge, plus the new Runtime/Projection/CLI edges. Repository and evaluated static phases are GREEN on the clean implementation tree; no guard was weakened or exclusion added.

## Strong causal controls

- The synthetic scoped provider is composed by the accepted HK01 composer, discovered and invoked generically. Comparing the same composed universe with a deliberately fixed base-only key list returns the exact omitted identity, so a transport-owned list cannot self-certify completeness.
- Pre-cancelled and zero-deadline requests leave the synthetic handler invocation count at zero. Canonical dispatch remains authoritative after admission, preventing a false cancellation report over a possible committed write.
- Diagnostics are enabled in a real process while stdout is parsed independently as exactly one JSON frame; string filtering inside the codec cannot make this control green.
- Environment controls compare exact stdout bytes between processes, not merely selected semantic fields.
- The external client links to no product assembly, starts two real processes and needs accepted journal/snapshot/diff/replay artifacts to reconstruct an identical final hash. A direct adapter mutation shortcut cannot satisfy that flow.
- Exact neutral fields, assembly references and the fixed MSBuild graph jointly guard the framing boundary; a source-name convention alone is not the oracle.

## Content-shape result

`Hk07AExternalClientTests.FreshExternalClientDiscoversAndExercisesAcceptedH0SurfaceEndToEnd` creates the bounded Potes/Liébana plaza/market/NPC/reference/extension slice through public JSON frames, reads truthful provenance, and reconstructs identical authored state in a second process using accepted snapshot import and journal replay. Final hash matches and semantic diff is empty. No gameplay-only concept was promoted into canonical authored state.

## Green implementation observation

Exact implementation/test/evidence SHA `9632884059abf483254e7f15eb01e1083c6e1762` passed:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07A*`: 15/15 GREEN;
- full regression: 151/151 GREEN;
- canonical local exact-SHA receipt: GREEN;
- candidate clean before and after: YES;
- independent HK00 repository universe: GREEN;
- independent HK00 evaluated static project graph: GREEN.

## Baseline-to-candidate scope audit

The candidate changes only:

- one portable transport-neutral projection request/outcome/service/completeness boundary;
- one narrow Runtime empty-portable-session composition seam;
- one JSONL CLI adapter/host with strict codec, deterministic serializer and documented modes/errors/exits;
- solution/project graph and locked test closure for the new portable project;
- fixed proof graph reconciliation for actual tracked projects/edges;
- focused neutral/process/external-client tests;
- exact-SHA observation/verification routing, public reference documentation and WP evidence.

No MCP SDK/type, second adapter, batching/pagination/stale recovery, host capability policy, general resource governor, HTTP/cloud, Unity/editor bridge, GUI, model-vendor orchestration, gameplay simulation or durable store was added.

## No remaining blocker found

- The canonical composed inventory is the only capability/schema oracle and dispatch authority.
- The neutral contract contains no reference-transport framing concern.
- Runtime owns canonical empty-session pairing but remains free of CLI/JSONL types.
- Production and synthetic-scoped completeness controls cannot self-shrink with a transport registry.
- Canonical, cancellation, timeout and transport failures remain machine-distinct.
- Malformed, truncated, oversized and invalid-UTF8 inputs fail closed with stable framed outcomes.
- Stdout contains protocol frames only; diagnostics remain on stderr.
- Environment and launch behavior are explicit and reproducible.
- The fresh external-client and representative content-shape flows are GREEN.
- All inherited regression tests remain GREEN; no concrete predecessor reopen condition exists.
- Remaining risks are outside the declared claim and recorded in `RESIDUAL_RISK.md`.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

The final exact-SHA reconciliation after `9632884059abf483254e7f15eb01e1083c6e1762` is documentation-only. The resulting exact HEAD must pass `scripts/hk07a-verify-exact-sha.sh` unchanged before the branch may be marked `FROZEN_FOR_REVIEW` and handed to a fresh independent Reviewer.
