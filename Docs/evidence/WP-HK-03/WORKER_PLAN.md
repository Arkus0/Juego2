# WP-HK-03 Worker plan

Baseline SHA: `39b0660193af53575aab26d3e5ff567a48ea86f3`  
Worker: `ChatGPT / GPT-5.6 Sol`  
State: `ACTIVE`

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-02 — Canonical world state + deterministic identity`.

- Reviewed candidate SHA: `f23fba9ab81566e682237183cf92618bbce504bd`
- Independent Reviewer verdict: `PASS` (PR #17 review `#5255346033`)
- Exact-SHA validation: GREEN (`Arkus Candidate Validation` run `35436836349`)
- Merge SHA: `1605f922b74d4f0b71bba7896c311a914ffed3f7`
- Current `main`/HK03 baseline: `39b0660193af53575aab26d3e5ff567a48ea86f3`

### Inherited guarantees consumed by HK03

HK03 consumes, and does not defensively re-prove, these accepted HK02 guarantees for the current finite canonical state surface:

1. `WorldState`, `WorldObject`, `WorldReference` and `WorldExtensionData` are the current canonical state-bearing model with typed stable identity and explicit schema/revision.
2. `CanonicalWorldStateCodec` defines one deterministic canonical byte form and SHA-256 content identity; object/reference/extension input ordering and process culture are non-semantic.
3. The accepted state enforces same-world referential integrity and explicit namespaced/versioned opaque extension preservation.
4. The HK02 semantic-surface proof covers the current state-bearing public fields. HK03 will not add new authorable state fields; it will expose reads over that accepted state.

HK03 also relies transitively on accepted HK01 only at the already-binding public-contract boundary: new public reads must enter the single Arkus-owned canonical composed inventory and therefore inherit canonical request/success/error schema validation and `system.describe` projection. HK03 does not re-prove HK01's generic composition/discovery completeness unless concrete contradictory evidence appears.

### Guarantees newly owned by HK03

HK03 owns the inspection/query semantics layered over the accepted state:

- a complete public read surface for world metadata, object lookup/query, flattened relationships and opaque extension bytes;
- an explicit typed selector grammar with no arbitrary expression/code execution;
- bounded result production, including bounded pagination and chunked opaque extension payload reads;
- deterministic ordering and deterministic opaque cursors stable for an unchanged world revision/hash;
- stale revision/hash/cursor rejection with canonical structured errors;
- every success response binding itself to world id, schema version, revision and canonical content hash;
- projection/field selection that only removes response fields and never creates a second semantic truth;
- side-effect-free repeatability against the same state;
- a completeness map from every current HK02 semantic state field to at least one discovered read path.

### Reopen conditions

An inherited HK02/HK01 boundary is reopened only if HK03 produces concrete evidence that the accepted guarantee is false or inapplicable to the effective path—for example, a current canonical state field that cannot be reconstructed without changing HK02 semantics, or a new HK03 public route that cannot pass through the accepted canonical composer/discovery path. A desire for duplicate proof is not sufficient.

## Implementation boundary

Keep the HK00 module direction intact:

- `Arkus.Game.Authoring` owns pure world-inspection/query semantics and may read `Arkus.Game.World` plus canonical protocol data.
- `Arkus.Harness.Runtime` owns canonical handler bindings/routes and delegates to the Authoring inspection service; Runtime does not take a new direct dependency on `Arkus.Game.World`.
- Base canonical composition gains the `world` namespace and discovered read definitions through the existing composer; no secondary registry is introduced.
- No mutation/apply, Unity/engine code, gameplay semantics, transport host or natural-language query interpretation is added.

## Planned public read surface

The finite v1 read surface is intentionally small and composable:

- `world.summary@1.0` — world identity/schema/revision/hash plus bounded counts.
- `world.object.get@1.0` — exact typed object lookup with optional field projection.
- `world.object.query@1.0` — typed filters, bounded deterministic page size, projection and cursor.
- `world.reference.query@1.0` — flattened source/kind/target relationships with typed filters and cursor.
- `world.extension.query@1.0` — bounded extension descriptors (`owner`, `schemaVersion`, `payloadLength`).
- `world.extension.read@1.0` — exact extension lookup plus bounded byte-range payload chunks so opaque authorable bytes remain reconstructible without an unbounded response path.

All state-dependent reads accept an explicit expected revision/hash anchor. Cursors additionally bind command + selector/projection fingerprint + world revision/hash + offset, so changing state or selector context fails closed rather than silently continuing another result set.

## Foundational proof approach

The HK03 claim is finite: the six canonical read commands above over the current HK02 state model and their canonical contract definitions/bindings. Completeness will use an independent semantic-state inventory (the accepted HK02 public state field universe, checked independently of the read registry) mapped to concrete reconstruction paths. Public-read schema completeness will be checked against effective canonical routes/definitions rather than discovery projecting its own list.

Required causal RED→GREEN controls will cover the six WP defect classes: hidden authorable field, nondeterministic query order, unbounded result path, stale cursor/revision, selector escaping its grammar and discovered read command missing schema. Any additional attack must map to a concrete acceptance gap or realistic false-green class.
