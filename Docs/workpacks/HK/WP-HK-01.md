# WP-HK-01 — Canonical contract model + capability discovery

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-00A` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`
Implementation PR: `#16`

Completion:
- Reviewed candidate SHA: `c16c0a7bbe4afe440252b921516b5e9b4635e082`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5255264042`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35435429932`)
- Merge SHA: `24d761ba0bc33a70fc06e5ea351054b5d3c51488`
- Completed: `2026-09-19`

## Objective

Define the stable, transport-neutral machine contract an AI/client can use without implementation knowledge, make every public capability discoverable from one canonical contract system, and define the engine-neutral composition mechanism that later reviewed scoped providers use without creating parallel registries.

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
- measured/declared cost or privilege metadata when useful;
- provider/scope/namespace metadata needed for later accepted scoped extensions.

Exact representation may differ if it preserves these ownership properties and remains mechanically projectable.

## Scoped extension composition

HK01 must define an engine-neutral canonical contract composer. It owns the rules by which the base Arkus capability set and later reviewed scoped contributions become one canonical public inventory.

A scoped provider may contribute canonical capability definitions plus implementation-binding metadata through the Arkus-owned registration/composition boundary. The composer must fail closed on namespace/identity conflicts, invalid or incomplete schemas, missing mandatory policy/side-effect/transaction/provenance metadata, or a public route that has no accepted canonical definition.

The H0 base capability set remains engine-neutral and contains no engine implementation object model. Later engine-scoped capability definitions may express genuinely engine-specific domain semantics only as portable contract data and Arkus-owned or namespaced logical references; clients must never need engine runtime/editor class names, assemblies or C# implementation types to discover or invoke them.

No engine bridge, transport adapter, SDK or Creator surface may make a capability public by maintaining an independent registry. Discovery and projection consume the single composed canonical inventory. Provider source material is not a second public truth: it becomes public only after canonical composition accepts it.

HK01 does not implement Unity or choose concrete engine APIs. Its obligation is to make the ownership/composition path real and testable with synthetic scoped-provider fixtures so H1 can instantiate it without redesigning the kernel.

## Acceptance

- Versioned canonical request/response semantics exist independently of any one transport framing.
- Stable structured error model includes machine code, human message, path/context and retry/repair metadata where applicable.
- `system.describe` (or equivalent canonical operation) discovers every public capability in the composed canonical inventory.
- Discovery exposes machine-readable request, success-result and error schemas for every capability, plus side-effect/determinism and relevant precondition metadata.
- Schemas use a documented standard representation and are generated/validated from one canonical contract source; no hand-maintained second truth.
- The canonical source can drive later transport projections and generated client metadata without requiring C# implementation/type knowledge.
- The canonical model supports base + scoped-provider composition without depending on any engine runtime/editor type system.
- A scoped provider cannot expose a public capability unless its definition has been accepted into the canonical inventory; duplicate/conflicting scope or identity fails closed.
- Unknown capability, unsupported contract version and schema-invalid payload fail closed at the canonical runtime boundary.
- Public H0/base contract contains no requirement to know C# type names, namespaces, source paths, MCP-specific type names or engine-specific object types; later scoped definitions likewise use portable contract data rather than engine implementation types.
- Contract compatibility rules are explicit: additive vs breaking changes and version negotiation, including scoped extensions.
- A completeness check proves canonical dispatcher surface == composed canonical discovered surface == request/success/error schema surface.
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
- self-shrinking discovery proof where deleting registry metadata would otherwise erase the proof obligation;
- synthetic scoped provider attempting to expose a public capability without canonical composition;
- duplicate/conflicting scoped capability identity or namespace;
- scoped capability schema attempting to require an implementation/runtime type rather than portable contract data.

## Forbidden scope

World semantics, mutation behaviour, concrete Unity/DFU APIs or engine packages, production network server, implementing the full MCP host, gameplay commands.

## DoD

A standalone canonical contract conformance suite can independently enumerate and validate the whole public composed capability surface and its request/success/error contracts, including synthetic scoped-provider composition and rejection of parallel registries; exact-SHA evidence and independent PASS.
