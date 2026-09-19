# WP-HK-03 completeness map

Claim: every currently accepted HK02 semantic state field needed to reconstruct the canonical world is reachable through the HK03 discovered read surface without using a second state model.

The independent universe is the accepted HK02 public state-bearing property surface (`WorldState`, `WorldObject`, `WorldReference`, `WorldExtensionData`). `Hk03SelfAttackTests.AssertCurrentSemanticSurfaceIsExplicitlyMapped` reflects that universe independently of the HK03 command registry; if the HK02 public state surface grows, the completeness proof turns red until this map and reconstruction oracle are updated.

## State-to-read map

| Canonical HK02 state | HK03 reconstruction path | Boundedness / interpretation |
|---|---|---|
| `WorldState.Id` | `world.summary` and every successful read: `world.worldId` | fixed scalar |
| `WorldState.SchemaVersion` | `world.summary` and every successful read: `world.schemaVersion` | fixed scalar |
| `WorldState.Revision` | `world.summary` and every successful read: `world.revision`; state-dependent requests are anchored to it | fixed scalar |
| canonical content identity | every successful read: `world.hash`; state-dependent requests are anchored to it | fixed 64-char SHA-256 identity |
| `WorldState.Objects` membership | exhaust `world.object.query` pages | at most 100 fixed-size object rows per page |
| `WorldObject.Id` | `world.object.get` / `world.object.query`: `id` | always present in an object row |
| `WorldObject.TypeId` | full/default `world.object.get` / `world.object.query`: `typeId` | optional only when the caller explicitly projects it away |
| `WorldObject.ContainerId` | full/default object read: `containerId`; absence means canonical null/no container | optional only for null or explicit projection |
| `WorldObject.References` membership | exhaust `world.reference.query`; group rows by `sourceId` | at most 100 fixed-size relationship rows per page; deliberately not nested in object responses |
| `WorldReference.Kind` | `world.reference.query`: `kind` | fixed scalar per relationship row |
| `WorldReference.TargetId` | `world.reference.query`: `targetId` | fixed scalar per relationship row |
| `WorldState.Extensions` membership | exhaust `world.extension.query` | at most 100 fixed-size descriptors per page |
| `WorldExtensionData.Owner` | `world.extension.query`: `owner` | fixed scalar |
| `WorldExtensionData.SchemaVersion` | `world.extension.query`: `schemaVersion` | fixed scalar |
| `WorldExtensionData.PayloadLength` | `world.extension.query`: `payloadLength` | fixed scalar; also determines completion of payload reconstruction |
| opaque extension payload bytes | repeated `world.extension.read(owner, schemaVersion, offset, limit)` | at most 768 raw bytes per response; concatenate chunks until no `nextOffset`; a zero-length payload is fully determined by its descriptor |

## Reconstruction oracle

`Hk03SelfAttackTests.HiddenAuthorableStateMutantBreaksIndependentReadReconstructionHash` reconstructs a complete accepted micro-world using only the six composed public read commands. It does not read `WorldState` directly while rebuilding semantics. It then compares the canonical HK02 content hash of the original and reconstructed worlds. A deliberately hidden/mutated extension payload changes the canonical hash and turns the oracle red.

Relationships are reconstructed exclusively from `world.reference.query`, not from nested object output. This is intentional: HK02 places no fixed cardinality bound on an object's references, so embedding all edges in `world.object.get/query` would create an unbounded response path. `UnboundedNestedRelationshipMutantIsSplitIntoBoundedReferencePages` exercises a source object with 130 references and proves the object response remains fixed-size while the complete relationship set is returned as 100 + 30 rows over two deterministic pages.

## Query/projection semantics

The query language is finite and typed by command schema: object filters can constrain IDs, types, containers, reference kinds and targets; reference filters constrain source IDs, kinds and targets; extension filters constrain owners and schema versions. Runtime semantic validation additionally restricts identifier tokens to the same conservative lower-case stable-token alphabet used by current canonical state and rejects selector values outside the declared grammar. No expression, callback, code, reflection selector, dynamic property path or natural-language evaluator is accepted.

Object projection may remove `typeId` and/or `containerId`; it never substitutes alternate semantics. Full/default object reads expose both fields where semantically present. Relationship and extension payload data use their own bounded canonical channels rather than projection-dependent aliases.

## Output-bound inventory

- `world.summary`: fixed metadata + three counts.
- `world.object.get`: one fixed-size object row.
- `world.object.query`: maximum 100 fixed-size object rows/page.
- `world.reference.query`: maximum 100 fixed-size relationship rows/page.
- `world.extension.query`: maximum 100 fixed-size extension descriptors/page.
- `world.extension.read`: maximum 768 raw payload bytes/chunk plus fixed metadata.

There is no public HK03 response containing an unbounded nested authorable collection.
