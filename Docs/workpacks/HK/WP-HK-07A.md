# WP-HK-07A — Headless host + neutral projection contract + reference transport

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06C`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Completion metadata

- implementation PR: `#44`;
- baseline SHA: `9173dcef32f6e020b64c3db2816fe5f4d0994058`;
- reviewed frozen candidate: `f2ef88980b38482a1a635f4eeb0582ee49d735ec`;
- independent Reviewer verdict: `PASS` (review `#5259732343`);
- exact-SHA validation: GREEN, Actions `35492562005`, artifact `10599004891`;
- implementation merge SHA: `f70a81b5115cd2a6b0c8b1cb9c5dd657d5f3792b`.

Accepted semantics: one non-interactive process-local canonical host; `arkus.neutral-projection@1` as the accepted transport-neutral request/outcome/completeness boundary above canonical composition; `arkus.reference.jsonl@1` as the deterministic JSONL reference transport; generic discovery and dispatch from the composed canonical inventory with no adapter-owned registry or mutation authority; explicit admission-only cancellation/timeout semantics; strict framing/failure/exit behaviour with protocol stdout isolated from diagnostics; and fresh-process exercise of accepted read, validation, mutation, provenance, snapshot, diff and replay semantics. HK07B must fit MCP as a genuine second projection without amending these accepted neutral semantics merely to accommodate the adapter.

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

MCP adapter, MCP-specific request/tool/schema abstractions, speculative second-transport abstraction added only in anticipation of MCP, HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration, or later interaction-efficiency/recovery work owned by HK08A/HK08B.

## DoD

A clean external process can discover and exercise the complete accepted canonical H0 surface through a deterministic reference transport, while an independently reviewable neutral projection contract is frozen above that transport with no second semantic registry and no speculative MCP pre-cooking; independent PASS.