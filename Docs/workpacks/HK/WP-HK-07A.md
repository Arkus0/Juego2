# WP-HK-07A — Headless host + neutral projection contract + reference transport

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06C`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Turn the accepted canonical Arkus runtime into a production-quality non-interactive process and freeze a transport-neutral projection contract, proven implementable by one deterministic reference transport, without pre-shaping that contract for MCP or any future second adapter.

## Acceptance

- Canonical host launches non-interactively from a clean checkout in CI.
- HK07A defines the **accepted neutral projection contract** between canonical Arkus semantics and transport adapters: discovery/capability inventory, request/result/error meaning, cancellation/failure mapping and completeness rules live above any specific transport framing.
- Deterministic reference transport is provided as JSON Lines over stdin/stdout plus one-shot file/stdin mode or an explicitly reviewed equivalent and implements that neutral projection contract.
- Reference transport projects the composed canonical capability inventory generically; it does not own a second command/schema registry.
- Protocol output is isolated from diagnostic logging so incidental logs cannot corrupt machine-readable frames.
- Framing, malformed/truncated/oversized input behaviour, fatal errors and exit codes are stable and documented.
- Request timeout/cancellation semantics are explicit and map through the neutral projection contract into canonical cancellation/failure semantics.
- Process startup for the local canonical path requires no Unity, editor state, network access or user prompts.
- Runtime kernel has no dependency on CLI/stdin/stdout/JSONL-specific types; host and reference transport remain adapters around the accepted canonical service surface.
- Effective process inputs/environment relevant to behaviour are explicit enough to reproduce CI execution.
- The projection completeness oracle is the canonical composed capability inventory, not a transport-owned list, so scoped canonical capabilities cannot disappear silently.
- A fresh external reference client can discover and exercise representative accepted read, validation, mutation, provenance, diff, snapshot and replay capabilities through this host without reading implementation source.
- Every abstraction introduced in HK07A must be justified by canonical semantics or by the reference transport that actually exercises it; no abstraction may exist solely because a future MCP adapter might need it.

## Required negative-conformance tests

RED→GREEN for: protocol-stream log contamination, truncated/malformed/oversized frame, cancellation/timeout, unexpected process environment changing semantics, alternate public host path that skips the canonical runtime, transport-owned registry being used as the only completeness oracle, synthetic scoped canonical capability omitted by a fixed/base-only transport registry, and a transport-specific framing concern leaking into canonical kernel or neutral projection semantics.

## Forbidden scope

MCP adapter, MCP-specific request/tool/schema abstractions, speculative second-transport abstraction added only in anticipation of MCP, HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration, agent ergonomics/batching work owned by HK08.

## DoD

A clean external process can discover and exercise the complete accepted canonical H0 surface through a deterministic reference transport, while an independently reviewable neutral projection contract is frozen above that transport with no second semantic registry and no speculative MCP pre-cooking; independent PASS.
