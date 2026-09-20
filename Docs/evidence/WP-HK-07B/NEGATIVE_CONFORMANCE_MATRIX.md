# WP-HK-07B negative-conformance matrix

The controls below target defect classes newly owned by HK07B. Accepted canonical/state semantics beneath HK07A are consumed, not re-proved.

| Required defect class | Causal control / oracle | Expected RED condition | GREEN result |
|---|---|---|---|
| canonical capability missing from MCP | `CausalNegativeControlsDetectInventoryAndSchemaDrift` removes one actual MCP canonical key and compares against the independently expected inventory | omission is accepted as complete | exact `missing:<canonical-key>` issue is produced |
| MCP changes request/result/error meaning | production discovery compares canonical input/full definitions; cross-transport flow normalizes outcomes; test mutates `successSchema` in one observed definition | schema drift or normalized result/error drift is accepted | actual definitions equal canonical; injected schema differs; invalid canonical request retains canonical failure meaning |
| MCP exposes undeclared mutation path | MCP project references only Projection + Protocol and calls only `NeutralProjectionService`; inventory oracle injects `adapter.only@1.0` | adapter-only path/tool is accepted as canonical | injected capability is reported `extra:adapter.only@1.0`; real mutations traverse neutral/canonical provenance |
| transport-owned registry is the sole completeness oracle | expected production universe is independently composed from `CanonicalWorldContract`, while actual universe comes from real MCP `tools/list` | MCP can shrink both its export and proof universe | any missing/extra identity is visible against the independent universe |
| synthetic scoped canonical capability omitted | `SyntheticScopedProviderProjectsWithoutMcpRegistry` supplies `engine.observe@1.0` through accepted composition without MCP edits | scoped contribution disappears unless added to adapter code | scoped capability appears automatically and projected count equals composed count |
| canonical-valid scoped capability exceeds MCP name limit | `OverLimitScopedCapabilitiesRemainDiscoverableUniqueAndInvokable` composes two distinct valid 123-character capability names through a scoped provider | composition succeeds but MCP discovery throws, a name exceeds 128, two identities alias, or invocation cannot recover the exact canonical route | both identities compose; both receive distinct bounded `arkus-long-*` names <=128 chars; both invoke successfully through neutral dispatch |
| bounded surrogate relies on hash uniqueness for collision safety | same long-name control exercises two distinct over-limit keys; production allocator includes the unique deterministic canonical ordinal before the full SHA-256 fingerprint | two distinct accepted long identities can receive the same transport handle if their fingerprints collide/truncate alike | ordinal is unique in the deterministically sorted composed inventory, so fingerprint collision cannot alias two inventory entries; runtime uniqueness guard remains fail-closed |
| adapter-only capability published without canonical composition | extra-key defect injection | MCP-only capability passes completeness | exact extra identity is rejected by oracle |
| cancellation/error semantic drift | `PreCancelledMcpAdmissionReturnsNeutralCancellationWithoutCanonicalDispatch` plus timeout/invalid-request cross-transport assertions | MCP throws framing cancellation, runs canonical handler, or changes failure class/code | `projection.cancelled`, handler count 0; timeout is `projection.timeout`; canonical invalid request stays canonical |
| SDK replacement requires canonical/projection semantic change | `SdkReplacementBoundaryOracleRejectsCanonicalLayerDependencyLeak` checks Protocol/Runtime/Projection and injects an SDK PackageReference into Projection text | upstream SDK dependency is accepted in neutral/canonical layer | real canonical projects have no SDK reference; injected Projection reference yields exact dependency-leak issue |
| MCP-shaped requirement forces neutral amendment | `McpToolNameConstraintStaysTransportFramingWithoutCanonicalIdentityAmendment` encodes canonical `@` only in MCP name, then injects `toolName` into the neutral field universe | MCP framing field can be promoted into accepted neutral request shape | canonical key remains `world.summary@1.0`, MCP name is `world.summary_401.0`, injected neutral `toolName` is rejected |

## Additional fail-closed/effective controls

- duplicate MCP canonical identity injection is detected separately from missing/extra;
- short canonical identities retain the existing reversible byte encoding, while over-limit identities switch to bounded transport-local handles only after the reversible form exceeds 128 characters;
- exact canonical identity is retained on every projected descriptor/tool metadata and invocation resolves the descriptor before constructing the neutral request;
- direct short names cannot alias the `arkus-long-*` surrogate family under the accepted canonical identifier syntax because every direct key includes the escaped `@` version separator `_40`;
- MCP `_meta` timeout is absent from canonical request `inputSchema` and from neutral projection source;
- full canonical definition metadata accompanies each projected tool, so success/error schema reconciliation is not inferred from input schema alone;
- unknown MCP tool names fail at the adapter framing boundary rather than becoming an alternate canonical route;
- MCP process initialization/list/call is exercised through real stdio JSON-RPC, not by calling SDK DTO builders only;
- the inherited HK01 production-assembly universe remains unchanged by the new long-name fixture: runtime-emitted attributed handlers live in a separate dynamic assembly rather than being added to HK01's explicitly enumerated test assembly.

Repair implementation/test SHA `b8e7f1a628e65effa790f810519a721454a6c980`: focused HK07B 9/9 GREEN, full regression 160/160 GREEN, Actions `35495362976`, artifact `10600483330`.
