# H1 Engine Bridge Architecture — Unity first

Version: 1.1 — 2026-09-21
Status: ACCEPTED / BINDING — H1 `PROCESS_ONLY` planning PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`.

## 1. Definition

An **Engine Bridge** is a versioned Arkus-owned projection boundary that:

1. consumes an immutable canonical authored-state anchor plus reviewed bridge inputs;
2. plans engine-owned effects without changing canonical state;
3. materializes those effects into a bounded managed engine scope;
4. observes the effective engine result through a normalized, machine-readable model;
5. diagnoses parity or drift; and
6. can reconstruct the projection from canonical evidence and bridge inputs.

It is not a second world model, a Unity command registry, a serializer for all Unity YAML, or permission for Unity objects to enter the canonical kernel.

## 2. Authority model

| Data / decision | Authority | Consequence |
|---|---|---|
| world/resource identity, containment and canonical references | accepted H0 `WorldState` | Unity may realize but not redefine it |
| canonical mutation, validation, CAS/idempotency and journal | HK04/HK05/HK06A | all authored-state changes still use `plan -> dry-run -> apply` |
| Unity binding intent | versioned Unity-scoped opaque extension authored through a schema-aware producer | engine-specific but canonical authored data; kernel remains opaque |
| asset/prefab/component catalogue | Unity bridge/project integration | discoverable scoped data, not `WorldState` and not part of its hash |
| Unity native locators and serialization | Unity bridge | replaceable implementation detail |
| managed scenes/prefabs/GameObjects/components | derived Unity projection | disposable/rebuildable; never canonical authority |
| normalized engine observation and drift | Unity bridge | evidence about projection, not canonical mutation |
| importing a supported Unity edit | canonical mutation proposal only | no state change until ordinary H0 apply succeeds |
| live runtime/save state | later gameplay/runtime owner | excluded from H1 authored projection |

### 2.1 Public host-to-Editor execution topology

`ADR-H1-004` freezes one topology before feature work begins:

```text
reference JSONL / MCP
          |
arkus.neutral-projection@1
          |
H1 policy + canonical composed capability handler
          |
fixed project-bound invocation envelope
          |
short-lived pinned Unity batch worker (Editor main thread)
```

The external Arkus .NET host owns the process-local canonical authored session, capability composition and transport projection. The Unity worker owns only Editor API execution for one admitted invocation. It has no canonical session, public discovery surface, MCP server or parallel command/schema registry.

Bootstrap binds exact editor/toolchain identity, fixed project identity/root, managed workspace roots, platform and one batch entry point outside request data. One project-operation lease serializes Editor invocations. Reference and MCP build the same H1 composition and call the same handler; cross-transport comparisons use sequential execution or isolated project copies, never concurrent public writers.

Pre-launch cancellation remains H0 admission cancellation. After launch, completion wins when a truthful result exists; otherwise cancellation, timeout, crash or a missing/corrupt result yields structured `unity.execution.*` failure and an indeterminate outcome where publication cannot yet be excluded. The owning feature contract reconciles by invocation/idempotency identity. Exit code, console text or staging output alone never proves success.

`WP-H1-03A` implements and proves this shared lifecycle with public composed read-only project inspection and bounded operation-status recovery. Its separately versioned Unity operation ceilings leave HK09B's H0 canonical-operation envelope unchanged. H1-05 must publish plan/materialize/observe capabilities and send their Editor-bound phases through it; H1-10 must do the same for checkpoint/rebuild. The Gate only consumes those accepted public routes.

## 3. Identity model

H1 uses four non-interchangeable identity layers:

| Identity | Stability and use | Forbidden interpretation |
|---|---|---|
| `WorldObjectId` / `WorldExtensionIdentity` | canonical authored identity and provenance | Unity object instance ID |
| Arkus logical catalogue ID | stable bridge/project identity for a scene, source asset, prefab, material, clip or component schema | canonical world object by default |
| projection generation/resource ID | identifies one derived managed output and receipt | authored identity |
| Unity locator (`GUID + local file ID`, `GlobalObjectId`, path) | resolves effective Unity assets/objects inside one project/editor contract | durable game identity or canonical hash input |

The bridge persists a versioned mapping from logical catalogue IDs and canonical resource IDs to native locators. Moves that preserve Unity identity preserve the logical mapping. Copies, ambiguous matches, missing `.meta` identity or locator reuse fail visibly. Recreating a managed GameObject may change its `GlobalObjectId` without changing the canonical resource it realizes.

## 4. Authored binding shape

The initial Unity-scoped authoring vocabulary is deliberately bounded:

- target logical scene;
- local transform using an explicit coordinate/unit convention;
- logical prefab/asset binding;
- allowlisted component binding documents;
- references from component data to canonical objects and logical catalogue entries;
- material/animation selections needed by the representative slice;
- projection policy/version.

The payload is stored as object-scoped opaque extension data. A schema-aware producer accepts structured input, normalizes/encodes it and derives dependencies mechanically:

- every structured canonical-object reference becomes the required HK02A typed dependency;
- every structured catalogue reference becomes a provider-owned typed catalogue dependency in the binding document/plan;
- the producer rejects a caller-supplied dependency set that disagrees with derived truth;
- raw opaque payload + dependencies remains an explicitly low-level primitive, with no claim that the kernel can inspect hidden references.

This closes the AI footgun without pretending external Unity assets are canonical world objects or silently expanding HK02A's reference model.

## 5. Admitted Unity units

H1 supports only the following reviewed scope:

| Unity unit | H1 treatment |
|---|---|
| source/imported assets | discoverable and referenceable; read-only to bridge materialization |
| source prefabs | discoverable, inspectable and instantiable; not overwritten |
| bridge-managed prefab derivatives/variants | generated projection artifacts with generation receipts |
| bridge-managed scenes | generated/materialized projection roots; arbitrary user scenes are not silently rewritten |
| managed GameObjects | mapped to canonical resources through a bridge marker/mapping record |
| Transform | first allowlisted component with explicit normalized semantics |
| allowlisted serialized components | schema adapter required; no generic arbitrary-reflection mutation endpoint |
| materials and animation clips | catalogue references; limited reviewed binding fields for the representative slice |
| packages, ProjectSettings and editor preferences | toolchain/bootstrap authority only, not public authoring units |
| runtime transient state, physics/navmesh/AI/save data | outside H1 |

Creating or modifying a scene, managed prefab, GameObject, Transform or allowlisted component therefore means changing canonical binding intent and rematerializing. Source assets/prefabs are not mutated. Unsupported or unmanaged Unity objects remain observable as unmanaged drift but are not adopted automatically.

## 6. Synchronization directions

### Canonical -> Unity (normal authority)

`canonical snapshot anchor + binding extensions + catalogue snapshot + bridge/toolchain version`
produces a deterministic projection plan. The materializer builds a new generation in a staging scope, validates and observes it, then publishes a small current-generation manifest only after success. A failure leaves canonical state and the previously active generation authoritative; incomplete staging output is non-authoritative and recoverable/cleanable.

### Unity -> canonical (explicit proposal only)

The bridge may observe supported edits inside the managed scope and compile them into a canonical mutation proposal. The proposal exposes expected revision/hash, normalized operations, derived dependencies and diagnostics. Only the accepted H0 mutation authority can commit it. Unsupported, ambiguous or unmanaged edits produce diagnostics rather than guessed mutations.

### Catalogue change

The catalogue is bridge/project-owned. Its fingerprint may change without changing the canonical world hash. A referenced logical ID becoming missing, incompatible or rebound creates validation/drift evidence and blocks a truthful materialization; it never silently rewrites canonical binding intent.

## 7. Materialization transaction and provenance

H1 does not claim a distributed transaction spanning canonical state and Unity files.

1. canonical mutation commits, if any, through H0 and receives normal H0 provenance;
2. Unity materialization is a separate external-reversible projection operation anchored to the resulting canonical hash/revision;
3. a bridge receipt records bridge contract version, Unity/toolchain fingerprint, catalogue fingerprint, plan digest, generation ID, normalized observation digest, diagnostics and publication status;
4. failed materialization leaves canonical truth intact and reports `canonical-ahead`/failed projection;
5. retry is idempotent for the same complete input tuple;
6. no bridge receipt is inserted into the HK06 mutation journal as if it were a canonical mutation.

The claimed H1 durability level is project-checkpoint/rebuild for the accepted scenario, not general WAL/fsync/power-loss durability. `WP-H1-10` owns the exact restart/rebuild boundary.

## 8. Validation ownership

| Validation | Owner |
|---|---|
| canonical identity, containment, world references, extension subject/dependencies | H0 HK05 |
| Unity binding document schema and dependency derivation | Unity scoped producer |
| logical catalogue existence/type/version compatibility | Unity catalogue/provider |
| scene loadability, unique managed markers, prefab/component/reference integrity, finite transforms and allowlisted serialized values | Unity bridge |
| gameplay rules, navigation, physics behavior, animation behavior or visual quality | later H2+ owners |

Unity diagnostics use stable machine codes and anchor to canonical resource ID, logical catalogue ID and/or normalized managed resource path. Native paths/locators may be included as evidence but are not the only repair identity.

## 9. Determinism and parity

For the same canonical snapshot hash, binding schema/version, catalogue snapshot fingerprint, bridge version, exact Unity editor/package fingerprint and declared platform, H1 requires:

- equal normalized projection plans and plan digests;
- equal managed logical hierarchy, asset/prefab relationships and allowlisted component values after observation;
- an idempotent second materialization with no semantic changes;
- deletion of generated outputs followed by rebuild to the same normalized observation digest;
- truthful, deterministic diagnostics for the same defect.

Byte-identical Unity YAML across editor versions or platforms is not claimed. Force Text is required for reviewability, but parity authority is the normalized effective observation. Rendered screenshots are supplementary evidence, not semantic truth.

## 10. Execution surfaces

- Plain .NET contract, codec, dependency-derivation and reference-projection work is `REMOTE_OK`.
- Any claim involving `AssetDatabase`, `GlobalObjectId`, scene/prefab serialization, Unity component behavior, editor package resolution or import requires the exact installed Unity Editor.
- Batchmode is both the default deterministic Unity proof surface and the sole H1 public host-to-Editor execution topology; an in-Editor Arkus/MCP host, daemon or network IPC is not an implementation option under this plan.
- A local interactive/editor or graphics-capable run is required only where the representative asset/render/animation evidence cannot be produced truthfully with `-nographics`.
- Missing required Unity evidence yields `READY_FOR_LOCAL_VALIDATION`, never PASS.

## 11. External dependencies

H1 targets the Unity 6.3 LTS family because the planning-date official support window extends through December 2027. `WP-H1-02` must select an exact patch, pin `ProjectVersion.txt`, `Packages/manifest.json` and `packages-lock.json`, record licenses/terms and freeze the render-pipeline/test package set. Editor binaries are never committed.

No third-party Unity MCP becomes bridge authority. Existing MCP remains an outer projection of the canonical composed inventory. Any real asset pack is adopted only in `WP-H1-11` with exact source/version/license and replacement boundaries.

## 12. Boundary before H2

H2/gameplay and keeper realization remain blocked until `WP-H1-GATE` proves that a fresh public client can create, inspect, modify, validate, materialize and rebuild the representative Juego2 slice with identity, dependencies, diagnostics, canonical provenance and normalized Unity parity intact.

H1 explicitly does not implement player movement, camera/gameplay systems, NPC simulation, navmesh behavior, combat, dialogue, runtime saves or the retained CITY blockout.

## 13. CITY programme interlock

Accepted CITY-00 remains the sole owner of macro geography; CITY-03 later owns the exact retained seed. H1 uses the constitution only to choose non-toy hierarchy/asset/component shapes. Its reference slice is disposable bridge evidence and cannot silently become the CITY seed.

- `CITY-01/02/05/06/03` remain independent REMOTE product planning.
- `WP-H1-08` is the earliest sufficient bridge prerequisite for `CITY-04`: managed scenes, prefab/component projection and Unity diagnostics are accepted. CITY-04 then remains a bounded CITY-owned greybox/falsification sidecar and is not an H1 proof stage.
- `WP-H1-GATE` is required before `CITY-07` turns the validated greybox into keeper realization.
- `CITY-08` owns the later keeper-slice public authoring/reuse-cost claim. H1-GATE owns only generic bridge readiness on a much smaller non-keeper slice, so neither trial substitutes for the other.
