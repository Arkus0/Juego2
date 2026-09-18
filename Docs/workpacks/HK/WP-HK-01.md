# WP-HK-01 — Protocol v1 + capability discovery

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-00`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Define the stable machine contract an AI can use without implementation knowledge.

## Acceptance

- Versioned request/response envelopes with request ID, protocol version, command identity and structured status.
- Stable structured error model with machine code, human message, path/context and retry/repair metadata where applicable.
- `system.describe` (or equivalent) discovers every public command.
- Discovery exposes machine-readable request, success-result and error schemas for every command, plus side-effect class, determinism classification and relevant preconditions.
- Schemas use a documented standard format and are generated/validated from one canonical contract source; no hand-maintained second truth.
- Unknown command, unsupported version, malformed envelope and schema-invalid payload fail closed.
- Public contract contains no requirement to know C# type names, namespaces or source paths.
- Contract compatibility rules are explicit: additive vs breaking changes and version negotiation.
- A completeness check proves public dispatcher surface == discovered surface == schema surface.

## Required self-attacks

RED→GREEN for: registered-but-undiscovered command, discovered command with missing output/error schema, schema/dispatcher mismatch, unsupported protocol version, unknown extra command route and accidental C# implementation detail leakage.

## Forbidden scope

World semantics, mutation behaviour, Unity/DFU, network server, MCP adapter, gameplay commands.

## DoD

A standalone protocol conformance suite can enumerate and validate the whole public surface; exact-SHA evidence and independent PASS.
