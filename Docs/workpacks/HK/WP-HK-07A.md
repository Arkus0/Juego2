# WP-HK-07A — Headless host + deterministic reference transport

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06C`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Turn the accepted canonical Arkus runtime into a production-quality non-interactive process that external clients can exercise deterministically without Unity, editor state or transport-specific semantics.

## Acceptance

- Canonical host launches non-interactively from a clean checkout in CI.
- Deterministic reference transport is provided as JSON Lines over stdin/stdout plus one-shot file/stdin mode or an explicitly reviewed equivalent.
- Reference transport projects the composed canonical capability inventory generically; it does not own a second command/schema registry.
- Protocol output is isolated from diagnostic logging so incidental logs cannot corrupt machine-readable frames.
- Framing, malformed/truncated/oversized input behaviour, fatal errors and exit codes are stable and documented.
- Request timeout/cancellation semantics are explicit and map to canonical cancellation/failure semantics.
- Process startup for the local canonical path requires no Unity, editor state, network access or user prompts.
- Runtime kernel has no dependency on CLI/stdin/stdout/JSONL-specific types; the host/transport remain adapters around the accepted canonical service surface.
- Effective process inputs/environment relevant to behaviour are explicit enough to reproduce CI execution.
- The transport completeness oracle is the canonical composed capability inventory, not a transport-owned list, so scoped canonical capabilities cannot disappear silently.
- A fresh external reference client can discover and exercise representative accepted read, validation, mutation, provenance, diff, snapshot and replay capabilities through this host without reading implementation source.

## Required negative-conformance tests

RED→GREEN for: protocol-stream log contamination, truncated/malformed/oversized frame, cancellation/timeout, unexpected process environment changing semantics, alternate public host path that skips the canonical runtime, transport-owned registry being used as the only completeness oracle, and a synthetic scoped canonical capability being omitted by a fixed/base-only transport registry.

## Forbidden scope

MCP adapter, HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration, agent ergonomics/batching work owned by HK08.

## DoD

A clean external process can discover and exercise the complete accepted canonical H0 surface through a deterministic reference transport, with reproducible framing/failure behaviour and no second semantic registry; independent PASS.
