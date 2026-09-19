# WP-HK-03 residual risk

## Declared trust boundary

HK03 claims complete, deterministic, bounded public inspection of the **currently accepted finite HK02 canonical world-state model** through the single HK01 canonical contract/composition path.

Inside the claim:

- current public state-bearing properties of `WorldState`, `WorldObject`, `WorldReference` and `WorldExtensionData`;
- the six HK03 `world.*@1.0` read definitions and their effective runtime routes;
- deterministic ordering/cursor context, revision/hash anchoring, selector grammar and output bounds;
- complete reconstruction of current canonical state semantics through discovered public reads.

Inherited/trusted:

- accepted HK02 canonical state invariants, immutability-by-copy semantics, deterministic codec and SHA-256 content identity;
- accepted HK01 canonical definition/composition/schema-validation/discovery machinery, except that HK03 re-evaluates effective route-to-definition completeness for the expanded production route universe;
- pinned .NET/compiler/runtime, Git checkout/object semantics, Actions runner/filesystem and SHA-256 primitive under the Foundational Proof Standard trusted base.

Outside HK03:

- mutation/apply transactions;
- engine/Unity projection or engine-native references;
- gameplay semantics;
- transport/network host behavior;
- natural-language query interpretation;
- security/authorization guarantees stronger than the existing public-read policy contract.

## Non-blocking residual risks

### Query evaluation is bounded in output, not asymptotic scan cost

Object/reference/extension queries may scan and sort the accepted in-memory world collection before emitting a bounded page. HK03 guarantees bounded public response paths, deterministic semantics and finite typed selectors; it does not claim an index, sublinear complexity or a fixed CPU budget independent of world size. Future scale work may add indexes so long as results remain semantically identical.

### Cursors are continuation tokens, not authorization credentials

HK03 cursors are deterministic opaque transport values binding command, world revision/hash, normalized selector/projection context and offset. They are not cryptographically authenticated capabilities and are not used as an authorization boundary. A caller that deliberately manufactures another syntactically valid continuation context gains no state beyond the same public-read surface. Security-capability semantics are outside this WP.

### Cross-call snapshot lifetime

Each command captures one canonical `WorldState` value and binds its result to that state's revision/hash. A state source may advance between calls; subsequent reads using old anchors/cursors fail stale rather than silently mixing revisions. HK03 does not provide a multi-command snapshot lease or transaction; later mutation/transaction WPs own that concern.

### Schema language does not encode every semantic numeric bound

The canonical request schema describes command shape/types/enums; HK03 service validation enforces semantic limits (`page <= 100`, extension chunk `<= 768`) and stable-token grammar with structured errors. The current HK01 `SchemaNode` vocabulary has no min/max/maxItems facets. Expanding the shared schema vocabulary is not necessary to make the HK03 runtime bounded and would be cross-WP contract work; clients can discover the command schema and receive deterministic structured repair errors when exceeding semantic bounds.

### Current model only

Completeness is intentionally tied to the current accepted HK02 public state-bearing universe. If future WPs add a new authorable state field/type, the reflection-based semantic-surface oracle turns red until the inspection map and reconstruction path are deliberately extended. HK03 does not claim to predict arbitrary future state shapes.

## Blocking-risk conclusion

No known residual risk above can falsify the HK03 acceptance claim inside the declared trust boundary. Any future change to the HK02 semantic surface, effective public read routes, or bounded-output architecture is mechanically observable by the retained tests and must update the proof before a new candidate can pass.
