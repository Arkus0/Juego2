# WP-HK-02 foundational proof matrix

Baseline SHA: `82ad746961145c2c779f5086cbc6bfde2b348c3c`  
Implementation observation SHA: `58ca3eab4090c211126425be1e3950f503f48000`  
Observation run: GitHub Actions `35436688357`

Binding contract: `Docs/workpacks/HK/WP-HK-02.md`  
Binding architecture: `Docs/engineering/PRODUCT_ARCHITECTURE.md` v1.2  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.2

## Claim and trust boundary

HK02 claims that the finite public `WorldState` model defined in `Arkus.Game.World` has stable typed identity, explicit schema/revision, fail-closed referential integrity and one deterministic canonical byte representation. For every state admitted by the model, object/reference/extension input order and current process culture are non-semantic; canonical ordering and invariant framing remove those sources of serialization noise. SHA-256 over canonical bytes is the content identity.

The current semantic input/property universe is independently enumerated in the causal suite using reflection over the public constructors/properties of `WorldState`, `WorldObject`, `WorldReference` and `WorldExtensionData`; this universe does not call the production codec. A field-class mutation matrix then proves that changing each current semantic input class changes the canonical hash. Together these controls prevent a newly exposed semantic input/property from silently entering the model without updating HK02 proof and prevent a current semantic field from being omitted by the codec.

Forward-compatible data is deliberately bounded: namespaced/versioned `WorldExtensionData` payloads are opaque bytes and are preserved exactly. Unknown structural record kinds and unsupported outer world-schema versions fail closed rather than being silently discarded. This is the explicit HK02 policy; richer schema migration belongs to later reviewed work.

Trusted infrastructure: exact Git checkout; pinned .NET SDK/runtime and documented BCL behavior; UTF-8/Base64/SHA-256 implementations; normal filesystem/GitHub Actions runner. HK02 does not re-prove HK00 project completeness, HK01 contract/composition completeness, compiler/runtime integrity or cryptographic collision resistance.

No new external dependency is introduced.

| Proof obligation | Completeness argument | Positive evidence | Causal negative control | Result |
|---|---|---|---|---|
| Stable typed identity | `WorldId`, `WorldObjectId`, `WorldTypeId`, `WorldReferenceKind` are distinct ordinal value types; state contains no scene/file/runtime-object identity | typed identity positive | semantic-surface universe guard | GREEN |
| Explicit schema + revision | `WorldState.CurrentSchemaVersion`, `SchemaVersion`, `Revision`; both are canonical records | round-trip + framing positives | unsupported schema; revision validation | GREEN |
| Deterministic canonical bytes | objects sort by object ID; references by source/kind/target; extensions by owner/version; LF, invariant integers, strict UTF-8 and Base64 framing | culture/order independence + LF framing | unstable-order mutant; noncanonical ordering | GREEN |
| Stable content hash | SHA-256 is computed only from canonical bytes and rendered lowercase hex | repeated hash positive | serialization-order noise cannot change hash | GREEN |
| Round-trip semantic preservation | deserialize rebuilds typed state, validates it, reserializes and requires exact canonical-byte equality | exact byte round-trip + opaque extension preservation | unknown record and noncanonical readable payload fail closed | GREEN |
| Referential integrity | one validator independently indexes object IDs, rejects duplicate objects, dangling containment/reference edges, duplicate refs and containment cycles | micro-world valid graph | duplicate ID; dangling reference; dangling container; containment cycle | GREEN |
| Semantic fields cannot hide outside serialization | reflection independently enumerates the current public semantic constructor/property surface; mutation matrix changes every current semantic input class | semantic mutation matrix | semantic-surface growth guard | GREEN |
| Unknown / forward-compatible data policy is explicit | only namespaced/versioned extension bytes are forward-preserved; outer structural/version uncertainty is rejected | extension round-trip | unknown structural record; unsupported schema | GREEN |
| Core model stays typed | finite typed classes and lists only; no `Dictionary<string, object>` state authority | Release build + source surface | semantic-surface guard | GREEN |
| Fixture remains representative but tiny | root/child/peer exercise identity, containment, references and opaque extensions using generic fixture vocabulary | micro-world positive | full HK02 causal suite | GREEN |
| H0 engine/gameplay neutrality | implementation is confined to `Arkus.Game.World` and tests; no Unity/DFU/gameplay model or transport host | baseline-to-candidate inspection | forbidden-scope pre-review | GREEN |
| Canonical Linux path remains healthy | exact clean SHA, locked restore, Release build, filtered positives, filtered causal controls and full regression | Actions `35436688357`: 0 warnings/errors; 6/6 positives; 11/11 causal; 56/56 regression | exact-SHA/clean-worktree gates | GREEN |

## Predecessor inheritance

Accepted HK01 guarantees are consumed, not duplicated. HK02 does not publish a capability, transport or engine route, so no concrete evidence reopens HK01 canonical contract/composition/discovery guarantees. The full ownership split is recorded in `WORKER_PLAN.md`.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: exact Git candidate + pinned .NET/BCL canonical primitives + documented CI infrastructure; finite HK02 public world-state semantic surface is in claim
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
