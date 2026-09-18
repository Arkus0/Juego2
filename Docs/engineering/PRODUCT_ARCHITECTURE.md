# Arkus Product Architecture

Version: 1.0 — 2026-09-18
Status: proposed by `WP-HK-00A`; becomes binding only after that WP is independently accepted.

## Product position

Arkus is not a Unity MCP wrapper and not a transport-specific tool server. It is an engine-agnostic, transport-agnostic game-authoring platform whose durable value is the canonical semantic model and the guarantees around safe machine authoring.

Juego2 is the first full proving ground. Product decisions must not narrow Arkus to Juego2-specific gameplay, one engine object model, one model vendor or one transport.

## Ownership hierarchy

```text
clients / agents / Creator / SDKs
            ↓
transport projections
MCP | JSONL | future HTTP/IPC
            ↓
canonical Arkus Contract
            ↓
authoring / validation / history kernel
            ↓
canonical game/world state
            ↓
engine bridge abstractions
            ↓
Unity first | future engines
```

### Canonical authority

The canonical Arkus Contract owns:

- capability identity/version;
- request schema;
- success-result schema;
- structured error schema and repair metadata;
- side-effect classification;
- determinism classification;
- preconditions/postconditions where applicable;
- concurrency/idempotency semantics;
- batching semantics;
- policy/privilege metadata;
- cost/weight metadata when measured and useful.

Transport adapters project this contract. They may add framing/transport metadata, but may not invent a second semantic command model.

## Transport rule

MCP is a first-class interoperability adapter because it gives Arkus access to a broad AI-client ecosystem. It is not the canonical semantic authority.

The reference deterministic transport remains useful as an oracle for conformance, replay and CI. Future transports must prove semantic equivalence against the same canonical contract.

No kernel project may depend on MCP/stdio/HTTP-specific types.

## Engine rule

Engine bridges are downstream projections of accepted canonical state/intent.

The first production bridge is Unity, but H0 canonical contracts may not depend on `GameObject`, `Scene`, `Prefab`, `MonoBehaviour` or analogous concepts from any engine unless a later reviewed contract proves a genuinely engine-neutral abstraction.

An engine bridge may own:

- realization/projection of canonical state into engine artifacts;
- asset resolution and engine-specific references;
- runtime/editor execution;
- engine observation/evidence;
- engine diagnostics;
- parity verification;
- visual/playtest evidence.

It may not silently become the owner of canonical gameplay state or authoring semantics.

## Verification rule

Arkus verification should support one scenario/evidence model across headless semantic execution and engine-backed execution where practical.

Engine-specific runners may collect screenshots, profiler data, console output and runtime observations, but scenario identity, deterministic inputs, expected semantic outcomes and replay evidence should remain independent of the engine adapter.

## Adopt-versus-own rule

Adopt external infrastructure when all are true:

1. it solves a generic problem materially better than bespoke Arkus code;
2. its license/commercial terms are acceptable under `DEPENDENCY_IP_POLICY.md`;
3. it can sit behind an Arkus-owned interface/conformance boundary;
4. replacing it would not require changing canonical Arkus semantics;
5. adopting it does not reduce scope versus Arkus product requirements.

Borrow patterns or selected permissively licensed components when useful, with exact upstream provenance and notices.

Reject or isolate components that impose product-level scope ceilings, engine lock-in, transport lock-in, model-vendor lock-in, avoidable copyleft propagation, opaque hosted dependency, or semantic authority outside Arkus.

## One-source contract rule

Request/result/error schemas, discovery metadata, generated SDK types, Creator-form metadata and transport projections must derive from one canonical contract source or from mechanically proven equivalent generated artifacts.

Hand-maintained parallel schemas are forbidden as a normal architecture.

## Superset rule

External harnesses are capability benchmarks, not architectural masters. Arkus may reuse their strong generic ideas, but a dependency or adapter is not accepted if the result becomes narrower than the strongest relevant practical capabilities without a deliberate reviewed tradeoff.

The external benchmark snapshot lives in `EXTERNAL_HARNESS_ADOPTION_AUDIT.md` and must be refreshed before H1/Unity bridge planning.

## Commercial portability

The product must be able to ship as a self-contained local developer tool without mandatory cloud/model-vendor service. Cloud services may be added later as optional products.

Critical persisted formats and canonical evidence must be documented/versioned enough that users are not trapped behind a proprietary opaque state store.

## Non-goals for H0

H0 does not need to implement:

- multiple game engines;
- cloud multi-tenancy;
- authentication SaaS;
- Creator GUI;
- final gameplay systems;
- natural-language planning/orchestration.

H0 must only leave clean boundaries so those can be added without redesigning the canonical kernel.
