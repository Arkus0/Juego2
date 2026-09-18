# WP-HK-01 — Canonical contract model + capability discovery

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-00A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Define the stable, transport-neutral machine contract an AI/client can use without implementation knowledge, and make every public capability discoverable from one canonical contract source.

## Canonical contract ownership

The Arkus contract model is upstream of transports. MCP/JSONL/future HTTP/SDK/Creator surfaces project the same canonical capability definitions and may not become independent semantic registries.

A capability definition must be able to describe, where applicable:

- stable identity and contract version;
- request schema;
- success-result schema;
- structured error schema;
- side-effect class;
- determinism class;
- preconditions/postconditions;
- concurrency/idempotency semantics;
- batching semantics;
- repair/retry metadata;
- measured/declared cost or privilege metadata when useful.

Exact representation may differ if it preserves these ownership properties and remains mechanically projectable.

## Acceptance

- Versioned canonical request/response semantics exist independently of any one transport framing.
- Stable structured error model includes machine code, human message, path/context and retry/repair metadata where applicable.
- `system.describe` (or equivalent canonical operation) discovers every public capability.
- Discovery exposes machine-readable request, success-result and error schemas for every capability, plus side-effect/determinism and relevant precondition metadata.
- Schemas use a documented standard representation and are generated/validated from one canonical contract source; no hand-maintained second truth.
- The canonical source can drive later transport projections and generated client metadata without requiring C# implementation/type knowledge.
- Unknown capability, unsupported contract version and schema-invalid payload fail closed at the canonical runtime boundary.
- Public contract contains no requirement to know C# type names, namespaces, source paths, MCP-specific type names or engine-specific object types.
- Contract compatibility rules are explicit: additive vs breaking changes and version negotiation.
- A completeness check proves canonical dispatcher surface == canonical discovered surface == request/success/error schema surface.
- The completeness universe is independently/effectively enumerable: removing or unregistering a public route cannot make both the capability and its proof obligation disappear.
- Contract projection tests prove a generated/projection artifact cannot silently omit a canonical capability or alter semantic schema meaning.

## Required self-attacks

RED→GREEN for:

- registered/dispatchable-but-undiscovered capability;
- discovered capability with missing success/error schema;
- schema/dispatcher mismatch;
- unsupported contract version;
- unknown extra route;
- accidental C# implementation detail leakage;
- transport-specific metadata leaking into canonical semantics;
- canonical capability omitted from a generated/projection inventory;
- self-shrinking discovery proof where deleting registry metadata would otherwise erase the proof obligation.

## Forbidden scope

World semantics, mutation behaviour, Unity/DFU, production network server, implementing the full MCP host, gameplay commands.

## DoD

A standalone canonical contract conformance suite can independently enumerate and validate the whole public capability surface and its request/success/error contracts; exact-SHA evidence and independent PASS.
