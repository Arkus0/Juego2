# WP-HK-07A Worker plan

Baseline SHA: `9173dcef32f6e020b64c3db2816fe5f4d0994058`
Branch: `wp/hk-07a-headless-host`
Worker state: ACTIVE — pre-review clean; evidence reconciliation and remote publication pending

## Objective

Implement HK07A only: freeze a transport-neutral projection contract over the accepted composed canonical runtime, ship a non-interactive production headless host, and exercise that contract through a deterministic JSON Lines reference transport plus one-shot stdin/file input. The adapter must project every composed canonical capability generically and preserve canonical request/result/error meaning without introducing a second registry or changing accepted state, mutation, validation, provenance, snapshot, diff or replay semantics.

The implementation will not add MCP, HTTP/cloud, Unity/editor integration, model-vendor orchestration, HK08 interaction-efficiency/stale-recovery semantics, HK09 host capability policy/resource limits, or gameplay concepts.

## Ownership and baseline

- Current `main` and Worker baseline: `9173dcef32f6e020b64c3db2816fe5f4d0994058`.
- No open pull request or remote `WP-HK-07A` branch was present when this Worker started.
- Canonical implementation branch: `wp/hk-07a-headless-host`.
- Direct dependency `WP-HK-06C` is COMPLETE and its post-PASS DocSync is present on the baseline.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-06C`.

- Reviewed frozen candidate: `55fecbd8a4a5e17ce247b164cd375d652d066fdf`.
- Independent review: PASS (`#5259530509`).
- Exact-SHA validation: Actions `35488920548` GREEN; artifact `10598675153`.
- Implementation merge SHA: `e44a5e93bf0912f5b5fb80dd749e294e21a740f2`.
- Post-PASS DocSync merge SHA: `3add47d` (contained in the exact baseline above).

Material transitive accepted contract: `WP-HK-01` canonical contract/composition.

- Reviewed candidate: `c16c0a7bbe4afe440252b921516b5e9b4635e082`.
- Independent review: PASS (`#5255264042`).
- Exact-SHA validation: Actions `35435429932` GREEN.
- Implementation merge SHA: `24d761ba0bc33a70fc06e5ea351054b5d3c51488`.

Inherited guarantees consumed rather than re-proved:

1. HK01 owns the canonical capability identity/version model, portable request/success/error schemas, structured error meaning, semantic metadata, version negotiation and the single composed base-plus-scoped capability inventory.
2. HK01 owns canonical dispatch validation and its independently enumerable public-route completeness boundary. `system.describe` is generated from the composed contract; a transport may neither decide which capabilities exist nor maintain a parallel command/schema registry.
3. HK03 owns deterministic, bounded, revision/hash-anchored inspection semantics over the accepted authorable state.
4. HK04/HK05 own plan/dry-run/apply, whole-world revision/hash CAS, idempotency, complete candidate validation, atomic mutation publication and structured repairable diagnostics. A transport projection invokes the canonical dispatcher and never receives canonical write authority directly.
5. HK06A owns truthful deterministic mutation-journal evidence and the authored/live boundary. Reads, failures and dry-runs do not fabricate committed mutation history.
6. HK06B owns semantic authored-state diff and versioned snapshot export/import. Import is the distinct `CanonicalRebase` authority, starts a new local lineage and does not fabricate HK06A history.
7. HK06C owns journal replay interpretation/compatibility and audit consistency. Replay is the distinct `CanonicalReplay` authority, stages execution through the accepted mutation path, publishes only after full audit, regenerates truthful local HK06A history and proves final hash/diff equivalence.
8. All accepted capabilities above already expose canonical portable request/result/error meaning. HK07A projects those meanings; it does not redefine their schemas, authority classes, histories or persistence semantics.

HK07A newly owns:

1. A transport-neutral request/outcome contract that selects a canonical capability/version range, carries portable arguments and request correlation, and maps canonical success/error plus cancellation/timeout/fatal host outcomes without JSONL/MCP framing concepts.
2. Generic projection of the complete composed canonical inventory and dispatch surface, with the composed inventory as the completeness oracle rather than any adapter-maintained list.
3. A production non-interactive host composition rooted in one accepted portable authoring session and reachable only through the canonical runtime/projection path.
4. Deterministic JSON Lines stdin/stdout framing and one-shot stdin/file input, strict UTF-8/JSON parsing, stable malformed/truncated/oversized-frame behavior and documented exit codes.
5. Isolation of protocol output on stdout from diagnostics on stderr.
6. Explicit bounded cancellation/timeout semantics at the neutral projection boundary that cannot report cancellation after an accepted canonical effect has committed.
7. Reproducible launch configuration: supported arguments and behavior-relevant inputs are explicit, unknown options fail closed, and ambient environment values do not change canonical semantics.
8. An external reference client proof that discovers and exercises representative accepted read, validation, mutation, provenance, diff, snapshot and replay capabilities through the process without reading implementation source.

Predecessor reopen trigger: concrete effective evidence that the production host reaches a public capability or canonical write effect absent from the accepted composed inventory/dispatcher; that the accepted dispatcher returns request/result/error data contradicting its canonical schema; or that invoking an accepted HK03-HK06C capability through the generic dispatcher violates its accepted authority/state/history guarantee. No such evidence is currently present. JSON/framing, host construction, correlation, projection completeness, cancellation mapping and process behavior are HK07A-owned defects, not reasons to re-prove predecessor internals.

## Architecture decision

The neutral projection will be a separate transport-agnostic assembly above `Arkus.Harness.Runtime`. It will reference the accepted `ComposedContract`, accept only portable data plus canonical version selection, and return a single typed neutral outcome. It will contain no stdin/stdout, JSON Lines, file or MCP types. The runtime/kernel will not reference this assembly.

The production host will create one `PortableWorldAuthoringSession` with an explicit empty authored-world root, compose `CanonicalWorldContract` once, wrap it in the neutral projector and then run the selected reference-transport mode. The JSON Lines adapter will parse/serialize framing only; it will not enumerate commands, duplicate schemas or invoke authoring services directly.

Cancellation is an admission guarantee, not an unsafe thread-abort mechanism: a request whose cancellation/deadline is already reached before canonical dispatch returns a neutral cancelled/timed-out error and is never dispatched. Once synchronous canonical dispatch starts, its truthful canonical success/error wins; the adapter never reports cancellation for an effect that may already have committed. This is explicit, deterministic and compatible with the current bounded synchronous H0 runtime. Later resource/time enforcement remains HK09B-owned.

## Expected implementation surfaces

- `src/Arkus.Harness.Projection/*`: neutral request/outcome model, generic canonical dispatcher adapter, projection completeness and host-factory boundary.
- `src/Arkus.Harness.Cli/*`: strict JSONL codec/frame reader, stdin/stdout/file host modes, deterministic serialization, diagnostics/exit-code isolation.
- `tests/Arkus.Harness.Tests/Hk07A*`: neutral-contract, completeness, process, external-client, content-shape and required causal negative-conformance coverage.
- `scripts/hk07a-*` plus canonical script routing: exact-SHA observation/verification.
- `Docs/reference/HK07A_REFERENCE_TRANSPORT.md` and `Docs/evidence/WP-HK-07A/*`: public launch/protocol documentation and foundational evidence.

## Proof plan

Positive/effective evidence:

- clean Release build launches the real CLI non-interactively;
- byte-stable `system.describe` output enumerates exactly the composed canonical inventory, including a synthetic scoped contribution through the neutral projector;
- an external process client discovers and invokes representative read, validation, mutation, journal, snapshot, diff and replay flows using only public frames and discovered schemas;
- protocol stdout contains frames only while diagnostics remain on stderr;
- stdin stream, one-shot stdin and one-shot file modes have documented deterministic behavior;
- launch output remains semantically identical under unrelated environment changes.

Causal negative-conformance classes:

- diagnostic log routed to protocol stdout;
- truncated/malformed/oversized or invalid-UTF-8 frame;
- already-cancelled request and expired/invalid timeout;
- ambient environment variable changing projection semantics;
- alternate public host factory/path that omits or directly skips the accepted canonical runtime;
- adapter-owned/fixed registry accepted as the only completeness oracle;
- synthetic scoped canonical capability omitted by a base-only projection;
- JSONL framing type/field leaking into the neutral projection or runtime kernel.

Independent/evaluated oracle:

Tests derive the expected capability universe from the accepted `ComposedContract.Definitions` and compare it structurally with neutral discovery and emitted external-process discovery. Separate source/project dependency checks require that Runtime/Protocol/Authoring do not reference CLI or JSONL types. Process tests parse stdout independently with `System.Text.Json`, inspect stderr separately and drive the compiled CLI as an external process rather than calling its internal codec.

## Proof/trust boundary

The accepted foundational trusted base remains: exact Git checkout/object semantics, pinned .NET/MSBuild/NuGet behavior, normal OS process/stdin/stdout/filesystem primitives used according to their documented contracts, and BCL UTF-8/JSON implementation. HK07A puts the repository-owned production host, neutral projection, JSONL framing, launch configuration, cancellation admission mapping and composed-inventory projection completeness inside its claim.

HK07A does not claim hostile OS/process isolation, authentication, network service behavior, durable persistence/crash recovery, arbitrary large-input resource policy, interruptible canonical execution after dispatch begins, MCP equivalence, multi-process writers or large-world performance. Those are outside this WP or owned by later workpacks.

Initial `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`. Product code is one neutral projection boundary plus one exercised reference transport/host. Proof machinery maps directly to HK07A acceptance and its eight required negative-conformance classes; inherited HK01/HK03-HK06C semantics are consumed rather than duplicated.
