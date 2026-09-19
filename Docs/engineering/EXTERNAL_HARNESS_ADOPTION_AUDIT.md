# External Harness Adoption Audit

Snapshot date: 2026-09-19
Status: binding architecture/adoption input after `WP-HK-00A` acceptance; refresh before H1 planning and before copying, vendoring or directly depending on external code.

## Purpose

Avoid reimplementing generic infrastructure while refusing to inherit another harness's architectural ceiling. External projects are benchmarks and component sources, not product authorities.

This snapshot records architecture decisions, not exact-version dependency approval. Any future direct adoption must separately satisfy `DEPENDENCY_IP_POLICY.md` at the exact selected version/commit and linkage/distribution mode.

## Decision scale

- `ADOPT`: candidate for direct use behind an Arkus-owned boundary, subject to exact-version dependency review.
- `BORROW`: reuse patterns and, where license/provenance permit, selected components.
- `BENCHMARK`: use as a capability/ergonomics target; no dependency required.
- `INTEROPERATE`: preserve compatibility without making the external product an authority.
- `ISOLATE/REJECT`: do not embed in the product path without a new reviewed decision.

## 2026-09-19 research refresh

Public upstream material was rechecked for this architecture freeze. Observations below are deliberately non-adoption evidence:

| Candidate | Upstream checked | Current public license observation | Architecture note |
|---|---|---|---|
| Official Model Context Protocol C# SDK | `modelcontextprotocol/csharp-sdk` | Apache-2.0; current upstream documents it as the official C# SDK | strong standards adapter candidate; pin/review exact package only when HK07 adopts it |
| Unity Biome MCP | `german-krasnikov/unity-biome-mcp` | MIT | useful deterministic playtest/visual-diff benchmark; Unity/MCP architecture is not Arkus kernel authority |
| Coplay MCP for Unity | `CoplayDev/unity-mcp` | MIT | broad Unity/editor tooling benchmark; engine-specific surface remains downstream |
| IvanMurzak Unity-MCP | `IvanMurzak/Unity-MCP` | Apache-2.0 | useful server/plugin and remote/runtime patterns; canonical state remains Arkus-owned |
| CoderGamester MCP Unity | `CoderGamester/mcp-unity` | MIT | useful batch/rollback/tooling patterns; Unity transaction semantics cannot replace canonical Arkus transactions |
| AltTester Unity SDK | `alttester/AltTester-Unity-SDK` | GPL-3.0 | useful black-box automation ideas, but embedded commercial use requires explicit legal/architecture exception or isolation |

These observations may change upstream. They are insufficient to copy/vendor/link code without the exact-version record required by `DEPENDENCY_IP_POLICY.md`.

## Current findings

| Candidate / category | Strong work worth preserving | Arkus decision | Product constraint |
|---|---|---|---|
| Model Context Protocol + C# SDK | broad client interoperability, standard tool discovery/schema transport, mature transport plumbing | ADOPT as first-class transport projection candidate | MCP must not define Arkus canonical semantics or error/transaction model |
| Unity Biome MCP | transactional scene-change workflow, batching/checkpoints, deterministic playtest ideas, visual baseline/diff workflow, progressive discovery | BORROW + BENCHMARK | Unity/Python/MCP architecture is not Arkus kernel architecture; do not inherit Unity-only ceiling |
| Coplay Unity MCP | broad Unity/editor tooling, compatibility focus, multi-instance/remote patterns, practical agent ergonomics | BENCHMARK + BORROW | engine-specific integration remains downstream of Arkus semantics |
| IvanMurzak GameDev/Unity MCP family | engine-neutral server/plugin split, self-contained .NET packaging, remote/runtime patterns | BORROW architecture patterns | bridge/transport layer only; canonical state stays Arkus-owned |
| CoderGamester MCP Unity | batching/rollback patterns, per-project security/auth ideas, dashboard/tooling ergonomics | BORROW + BENCHMARK | do not substitute engine transaction semantics for canonical Arkus transaction semantics |
| Unity first-party AI/MCP tooling | first-party editor interoperability and future ecosystem compatibility | INTEROPERATE / BENCHMARK | optional adapter path; Arkus cannot require a proprietary editor AI layer for core authoring |
| FsCheck or equivalent property-testing framework | mature property/random testing | ADOPT if exact-version license/dependency review passes | testing mechanism only; Arkus defines properties/oracles |
| Stryker.NET or equivalent mutation-testing framework | mutation testing of guard effectiveness | ADOPT if exact-version review passes | secondary oracle, not proof of completeness by itself |
| SharpFuzz or equivalent .NET fuzzing stack | protocol/parser fuzzing with mature native fuzzers | ADOPT if exact-version review passes | must preserve reproducible seeds/corpus and Arkus failure classification |
| GPL/copy-left engine test SDKs such as AltTester SDK | useful external black-box automation ideas | ISOLATE/REJECT as embedded dependency by default | commercial distribution obligations require explicit legal/architecture exception |
| Snapshot libraries with nontrivial commercial terms | convenience for snapshot assertions | BENCHMARK/REJECT as foundational dependency by default | canonical snapshot/replay format must remain Arkus-owned and commercially predictable |

## Capability harvest

The following ideas are worth carrying into Arkus requirements even when no code is reused:

### Discovery / agent ergonomics

- progressive/bounded discovery rather than dumping a giant tool surface;
- capability categories/tags;
- batch-first authoring primitives;
- compact/projection responses;
- cost/side-effect metadata;
- multi-instance/project routing where an engine adapter supports it.

### Safe mutation

- explicit plan/preflight;
- checkpoint/rollback around engine-side realization;
- atomic canonical transactions;
- post-apply verification;
- clear distinction between canonical commit and downstream engine projection failure.

### Verification

- deterministic playtest/scenario DSL concepts;
- semantic assertions independent of screenshots;
- visual baselines/pixel-diff as engine evidence, not canonical truth;
- captured console/profiler/runtime evidence;
- replayable evidence bundles.

### Packaging / deployment

- self-contained .NET packaging where practical;
- local stdio and remote transports behind one host abstraction;
- Docker/CI-friendly execution for headless components;
- optional runtime bridge separate from editor-only bridge.

### Security / operations

- explicit per-project identity/authorization at privileged adapter boundaries;
- request/resource limits;
- no generic shell/network power in canonical authoring kernel;
- multi-project routing must never weaken state identity or provenance.

## Deliberate Arkus supersets

Arkus must exceed the common engine-MCP pattern in these areas:

1. canonical engine-agnostic world/game semantics;
2. discoverable request + success + error contracts with repair metadata;
3. deterministic plan/dry-run/atomic apply as canonical mutation model;
4. optimistic concurrency and idempotency;
5. invariant inventory and repairable structured diagnostics;
6. semantic diff, provenance, journal and hash-identical replay;
7. one scenario/evidence model capable of headless semantic and engine-backed execution;
8. transport replaceability and engine replaceability;
9. completeness proofs that cannot self-shrink with the registry being proved;
10. commercial dependency/IP discipline.

A mature external harness may be adopted or borrowed from only when these Arkus obligations remain explicit. Missing Arkus semantics are not waived because the external implementation is broader in engine tooling or easier to integrate.

## Revalidation rule

Before any external source code is copied, vendored or directly depended on, re-check the exact current repository/version, license and relevant implementation. Record the result under `DEPENDENCY_IP_POLICY.md`, including the Arkus conformance/replacement boundary and the guarantees that remain Arkus-owned.
