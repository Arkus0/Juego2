# WP-HK-07 — Headless host + transport projections

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-06`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Expose the accepted runtime through a production-quality, transport-neutral host with both a deterministic reference transport and a standards-compatible MCP projection, without coupling canonical semantics to either.

## Acceptance

- Canonical host works non-interactively in CI from a clean checkout.
- Deterministic reference transport is supported (baseline: JSON Lines over stdin/stdout plus one-shot file/stdin mode or an explicitly reviewed equivalent).
- A first-party MCP adapter projects the accepted canonical Arkus contract through a pinned approved SDK/implementation rather than maintaining an independent command/schema registry.
- MCP and reference transport expose semantically equivalent capabilities for the accepted H0 surface; framing differences are allowed, semantic drift is not.
- Transport projections consume the composed canonical capability inventory generically rather than a transport-owned or engine-bridge-owned list, so later accepted scoped providers can become visible without creating a second registry or rewriting canonical semantics.
- Protocol output is isolated from diagnostic logging; machine streams cannot be corrupted by incidental logs.
- Exit codes and fatal reference-transport errors are stable/documented.
- Request timeout/cancellation semantics are explicit and map cleanly into canonical cancellation/failure semantics.
- Process startup does not require Unity, editor state, network access or user prompts for the local canonical path.
- Runtime kernel has no dependency on CLI/stdio/MCP/HTTP-specific types; future GUI/HTTP/SDK adapters can wrap the same service surface.
- Restore/build/test and transport conformance run under pinned toolchain/environment assumptions.
- Effective process inputs/environment relevant to behaviour are explicit enough to reproduce CI execution.
- The transport inventory cannot self-shrink: canonical capability inventory is the authority and adapters are checked against it, not vice versa.
- External transport SDK usage satisfies `DEPENDENCY_IP_POLICY.md` and is replaceable behind Arkus conformance tests.

## Required negative-conformance tests

RED→GREEN for:

- log contamination of reference protocol stream;
- truncated/malformed/oversized reference frame;
- cancellation/timeout;
- unexpected process environment changing semantics;
- alternate public host path that skips the canonical runtime;
- canonical capability missing from MCP projection;
- MCP projection changing request/result/error meaning;
- MCP adapter exposing an undeclared mutation path;
- transport registry being used as the only completeness oracle;
- synthetic scoped canonical capability present in the composed inventory but omitted because the transport reads a fixed/base-only registry;
- engine/transport adapter attempting to publish an adapter-only capability that was never accepted by canonical composition;
- removing/replacing the transport SDK causing canonical kernel code changes rather than adapter-only changes.

## Forbidden scope

HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration.

## DoD

External processes can discover and exercise the complete accepted harness contract through the deterministic reference transport and the MCP projection; conformance proves both are projections of the same composed canonical semantics with deterministic failure behaviour and no parallel adapter registry; independent PASS.
