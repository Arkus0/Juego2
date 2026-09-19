# WP-HK-02 Worker plan

Baseline SHA: `82ad746961145c2c779f5086cbc6bfde2b348c3c`  
Worker: `ChatGPT / GPT-5.6 Sol`  
State: `ACTIVE`

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-01 — Canonical contract model + capability discovery`.

- Reviewed candidate SHA: `c16c0a7bbe4afe440252b921516b5e9b4635e082`
- Independent Reviewer verdict: `PASS` (PR #16 review `#5255264042`)
- HK01 merge SHA: `24d761ba0bc33a70fc06e5ea351054b5d3c51488`
- HK01 DocSync completion is reflected on current `main`; HK02 is the dependency-valid next WP.

### Inherited guarantees consumed by HK02

HK02 consumes, and will not redundantly re-prove, the accepted HK01 guarantees that:

1. Arkus has one transport-neutral canonical public capability inventory and one Arkus-owned composition/admission path.
2. Public request/success/error contracts are portable machine-readable data, not C#/engine implementation types.
3. `system.describe`/projection is driven by the canonical inventory and its emitted artifact is already covered by HK01 conformance.
4. Scoped providers cannot bypass canonical composition and same-major breaking request evolution fails before negotiation.
5. HK01 completeness over the current canonical public route/discovery/schema surface is already accepted inside its finite trust boundary.

HK02 therefore does **not** reopen dispatcher/discovery completeness, provider namespace ownership, JSON-schema compatibility, or the HK00 project/source universe merely for defence-in-depth.

### Guarantees newly owned by HK02

HK02 owns only the canonical world-state boundary:

- stable typed world/object identities independent of runtime object references, scene IDs and file paths;
- explicit schema version + world revision;
- deterministic canonical serialization and content hashing;
- semantic round-trip fidelity;
- canonical ordering independent of caller/collection iteration order;
- explicit referential-integrity rules for containment/reference edges;
- explicit unknown/forward-compatible payload policy;
- a deliberately tiny non-gameplay micro-world fixture;
- causal negative controls for the six WP-mandated defect classes.

### Reopen condition for inherited guarantees

An inherited HK01 boundary is reopened only if HK02 produces concrete evidence that the accepted HK01 guarantee is false or inapplicable to an effective path HK02 actually uses—for example, if implementing HK02 required publishing a public capability outside the canonical composer, or using an implementation/runtime type in a canonical contract. No such evidence is currently present.

## Claim and trust boundary

Claim: for the finite HK02 `WorldState` semantic model, every semantic field admitted by the model participates in a deterministic canonical byte representation; valid references resolve inside that state; deserialization either preserves the state exactly or fails closed; and SHA-256 over canonical bytes is therefore a stable content identity for the same semantic state.

Trusted base: exact Git checkout; pinned .NET SDK/runtime and documented BCL behavior; UTF-8 and SHA-256 implementations; normal filesystem/CI infrastructure. HK02 does not prove cryptographic primitives, compiler/runtime integrity, HK00 project completeness, or HK01 canonical-contract completeness.

## Design

- Keep the product model in `Arkus.Game.World`; no Unity/DFU/gameplay dependency.
- Use immutable/value-based typed IDs and immutable-ish world records/collections copied at construction.
- Use one Arkus-owned canonical textual codec with culture-invariant ASCII framing and Base64-encoded UTF-8 payload fields, avoiding serializer ordering/runtime-version ambiguity.
- Canonicalize objects, references and extension blocks by ordinal semantic keys before encoding.
- Preserve forward-compatible extension data as opaque bytes only when it is explicitly namespaced and version-owned; reject unknown structural record kinds and unsupported world schema versions.
- Validate duplicate IDs, dangling containment/reference targets and containment cycles before canonicalization/hash.
- Hash only canonical bytes with SHA-256.

## Proof strategy

Positive tests cover repeatability, insertion-order/culture independence, round-trip exactness, stable hashing, extension preservation and the micro-world fixture.

Required RED→GREEN controls cover:

1. unstable ordering;
2. duplicate ID;
3. dangling reference;
4. serialization-order noise changing hash;
5. current semantic fields omitted from serialization (field-mutation matrix must change canonical bytes/hash);
6. unsupported schema/version payload.

Additional causal controls cover dangling containment, containment cycles and unknown structural records because they directly protect the explicit referential-integrity / unknown-data acceptance criteria.

## Proof budget

No external package is planned. Proof code stays in the existing test project plus compact evidence/scripts. Do not build a generic serialization framework or gameplay schema system in HK02.
