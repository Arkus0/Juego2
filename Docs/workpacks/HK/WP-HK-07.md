# WP-HK-07 — Headless host + transport boundary

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Expose the accepted runtime through a production-quality headless process boundary without coupling the kernel to any one future transport.

## Acceptance

- Canonical host works non-interactively in CI from a clean checkout.
- At least one deterministic machine transport is supported (preferred baseline: JSON Lines over stdin/stdout plus one-shot file/stdin mode).
- Protocol output is isolated from diagnostic logging; stdout cannot be corrupted by incidental logs.
- Exit codes and fatal transport errors are stable/documented.
- Request timeout/cancellation semantics are explicit.
- Process startup does not require Unity, editor state, network access or user prompts.
- Runtime kernel has no dependency on CLI/stdio-specific types; future MCP/GUI adapters can wrap the same service surface.
- Restore/build/test and protocol conformance run under pinned toolchain/environment assumptions.
- Effective process inputs/environment relevant to behaviour are explicit enough to reproduce CI execution.

## Required self-attacks

RED→GREEN for: log contamination of protocol stream, truncated frame, malformed JSON, oversized frame, cancellation/timeout, unexpected process environment changing semantics, and alternate public host path bypassing canonical runtime.

## Forbidden scope

MCP server, HTTP/cloud service, Unity Editor bridge, GUI.

## DoD

An external process can discover and exercise the complete accepted harness contract headlessly with deterministic framing and failure behaviour; independent PASS.
