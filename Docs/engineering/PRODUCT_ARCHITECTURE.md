# Arkus Product Architecture

Version: 1.2 — 2026-09-19
Status: candidate contract for `WP-HK-00A`; becomes binding only after that WP is independently accepted.

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
(base + accepted scoped extensions)
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

Canonical world/game state, transaction semantics, invariant identity, provenance/replay meaning and persisted canonical formats are also Arkus-owned semantic authorities. No transport, engine adapter, SDK or third-party framework may become the only definition of any of those semantics.

Transport adapters project the canonical contract. They may add framing or transport metadata, but may not invent a second semantic command model.

## Scoped capability extension rule

The canonical Arkus Contract is an extensible contract system, not a closed H0-only list of commands. H0 defines the engine-neutral contract model and base capability set; later reviewed packages may contribute scoped capabilities without creating another semantic authority.

Engine-scoped public capabilities enter the product only through an Arkus-owned contract composition path.

A scoped contribution must use the canonical Arkus capability-definition model and declare a stable namespace/scope/provider identity, request/success/error schemas and the same side-effect, determinism, policy, concurrency, transaction/provenance and compatibility metadata required of comparable base capabilities. The canonical contract composer validates namespace ownership, capability identity/version, schemas, metadata, implementation binding, conflicts and required policy/transaction/provenance declarations before a contribution becomes public. Invalid, conflicting or incomplete contributions fail closed.

H0 base capability definitions remain engine-neutral. A later engine extension may describe genuinely engine-scoped domain semantics using portable contract data and Arkus-owned or namespaced logical references, but the canonical contract meta-model and kernel may not depend on engine runtime/editor classes, assemblies or implementation type names. Engine-specific vocabulary in an accepted scoped capability does not make the engine implementation type system part of the canonical kernel.

An engine bridge may contribute capability definitions and implementation bindings, but it may not publish a parallel public capability registry, discovery surface or transport-only schema authority. Registration is upstream of transports: once accepted by the Arkus composer, the capability appears in the single composed canonical inventory and is discovered/projected through the same canonical path as base capabilities.

Every public scoped invocation enters the canonical dispatch/policy envelope. A scoped capability may not bypass accepted validation, policy, provenance or transaction rules merely because its implementation is engine-specific. Canonical-state mutation still goes through the canonical authoring transaction pipeline. Engine-owned side effects that do not mutate canonical state must still declare their side-effect/rollback-or-irreversibility semantics and produce canonical provenance/evidence according to the later accepted contract.

H1 must instantiate this extension boundary for Unity; it may define Unity-specific implementations and scoped semantics, but it may not reopen the ownership question by creating a Unity-owned public registry or by moving Unity runtime types into the H0 contract/kernel.

## Mutation authority

Transport and engine adapters may not mutate canonical state directly. Every canonical mutation must enter through the accepted Arkus authoring transaction pipeline and its validation/concurrency/provenance rules before downstream realization occurs.

An adapter may reject, translate, observe or realize accepted canonical intent. It may not create a hidden side door whose state changes bypass canonical plan/apply, validation, journal or replay semantics.

If an engine realization fails after a canonical decision, the failure must be represented explicitly at the adapter/projection boundary; the adapter must not silently rewrite canonical meaning to match engine state.

## Transport rule

MCP is a first-class interoperability adapter because it gives Arkus access to a broad AI-client ecosystem. It is not the canonical semantic authority.

The reference deterministic transport remains useful as an oracle for conformance, replay and CI. Future transports must prove semantic equivalence against the same canonical contract.

No kernel project may depend on MCP/stdio/HTTP-specific types.

## Engine rule

Engine bridges are downstream projections/implementations of accepted canonical intent plus accepted scoped capability bindings.

The first production bridge is Unity, but H0 canonical contracts may not depend on `GameObject`, `Scene`, `Prefab`, `MonoBehaviour` or analogous concepts from any engine unless a later reviewed contract proves a genuinely engine-neutral abstraction.

An engine bridge may own:

- realization/projection of canonical state into engine artifacts;
- asset resolution and engine-specific references;
- runtime/editor execution;
- engine observation/evidence;
- engine diagnostics;
- parity verification;
- visual/playtest evidence;
- implementation bindings for accepted engine-scoped capability contributions.

It may not silently become the owner of canonical gameplay state, authoring semantics or the public capability inventory.

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

Wholesale adoption of an engine harness is specifically rejected when required Arkus semantics are absent. Shorter implementation is not sufficient justification for inheriting a narrower state model, transaction model, discovery model, replay model or engine boundary.

## One-source contract rule

Request/result/error schemas, discovery metadata, generated SDK types, Creator-form metadata and transport projections must derive from one canonical contract source or from mechanically proven equivalent generated artifacts.

The single source is the composed canonical inventory produced by the Arkus-owned contract system: base definitions plus only those scoped contributions accepted by the canonical composer. A bridge/provider may own source material for its contribution and its implementation binding, but it cannot expose that material as a second public registry that bypasses canonical composition.

Hand-maintained parallel schemas are forbidden as a normal architecture. Adapter-only framing metadata may be authored separately only when it cannot change canonical semantic meaning and conformance proves that relationship.

A projection cannot decide which canonical capabilities exist. Canonical capability inventory is upstream; projection completeness is checked against it.

## Superset rule

External harnesses are capability benchmarks, not architectural masters. Arkus may reuse their strong generic ideas, but a dependency or adapter is not accepted if the result becomes narrower than the strongest relevant practical capabilities without a deliberate reviewed tradeoff.

The external benchmark snapshot lives in `EXTERNAL_HARNESS_ADOPTION_AUDIT.md` and must be refreshed before H1/Unity bridge planning.

## H0 dependency route

The product boundary is intentionally frozen before protocol/runtime expansion:

- `WP-HK-00` owns the portable module/build boundary only.
- `WP-HK-00A` owns this product/adoption boundary, including the ownership path for later scoped capabilities.
- `WP-HK-01` must define the canonical contract/composition model and independently/effectively prove the complete canonical capability/schema surface; it may not make MCP, JSONL, an engine bridge or any discovery registry the source of semantic truth.
- `WP-HK-02` through `WP-HK-06` build state, inspection, transaction, validation and provenance/replay semantics below transports and engines.
- `WP-HK-07` projects the already-accepted composed canonical contract through the deterministic reference transport and MCP, and proves adapter completeness/equivalence against the canonical inventory.
- `WP-HK-GATE` must prove the full H0 product boundary before any engine bridge can unblock.
- H1 defines engine bridge abstractions and Unity as the first implementation only after H0 acceptance, and must register any public engine-scoped capability through the accepted Arkus-owned composition path.

No downstream WP may patch around a transport- or engine-owned semantic shortcut that contradicts this route; the predecessor contract must be corrected and independently reviewed instead.

## Commercial portability

The product must be able to ship as a self-contained local developer tool without mandatory cloud/model-vendor service. Cloud services may be added later as optional products.

Critical persisted formats and canonical evidence must be documented/versioned enough that users are not trapped behind a proprietary opaque state store.

## Non-goals for H0

H0 does not need to implement:

- multiple game engines;
- concrete Unity capability definitions or Unity APIs;
- cloud multi-tenancy;
- authentication SaaS;
- Creator GUI;
- final gameplay systems;
- natural-language planning/orchestration.

H0 must only leave clean boundaries so those can be added without redesigning the canonical kernel.
