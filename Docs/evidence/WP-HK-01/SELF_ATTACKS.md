# WP-HK-01 causal self-attacks

Implementation observation: `38783856d89a9ec7cff376086ae8702b0bb72501`

GitHub Actions run: `35434057309`

Observed result: 22/22 HK01 negative controls GREEN; 9/9 positive contract tests GREEN; 32/32 total regression GREEN.

The attacks below inject invalid contract/runtime variants in-memory against the same production guards used by the canonical composer, projection conformance and dispatcher. RED means the injected defect is rejected/detected for the intended causal reason; the valid baseline remains GREEN in the same canonical run.

| Defect class | Causal control | Intended RED oracle |
|---|---|---|
| dispatchable capability omitted from discovery/projection | `RegisteredDispatchableButUndiscoveredTurnsProjectionConformanceRed` | `projection.omitted_capability` |
| discovered capability missing success/error schema | `DiscoveredCapabilityMissingSuccessOrErrorSchemaFailsClosed` | `contract.missing_schema` |
| dispatcher/schema/binding identity mismatch | `SchemaDispatcherOrBindingMismatchFailsComposition` | binding mismatch + definition/route closure errors |
| unsupported contract version | `UnsupportedContractVersionFailsBeforeHandlerInvocation` | `contract.unsupported_version`, handler not invoked |
| extra public route outside canonical inventory | `UnknownExtraRouteIsFoundByIndependentUniverse` | `conformance.extra-public-route` |
| implementation/transport type vocabulary in canonical schema | `ImplementationTypeOrTransportSpecificSchemaMetadataIsRejected` | `contract.schema.non_portable_format` |
| transport projection attempting to become registry | `TransportProjectionCannotBecomeCanonicalProviderRegistry` | `composition.transport_registry_forbidden` |
| projection changes canonical schema meaning | `ProjectionCannotSilentlyAlterCanonicalSchemaMeaning` | `projection.semantic_mismatch` |
| deleting registry metadata shrinks discovery | `DeletingRegistryMetadataCannotShrinkIndependentRouteProofUniverse` | effective handler remains visible as extra route |
| scoped public route bypasses canonical composition | `SyntheticScopedPublicRouteWithoutCanonicalCompositionIsRejectedByConformance` | `conformance.extra-public-route` |
| overlapping scoped namespace | `DuplicateOrOverlappingScopedNamespaceFailsClosed` | `composition.namespace_conflict` |
| duplicate scoped capability identity | `DuplicateScopedCapabilityIdentityFailsClosed` | `composition.duplicate_capability` |
| scoped schema requires implementation/runtime type | `ScopedSchemaCannotRequireEngineRuntimeImplementationType` | `contract.schema.non_portable_format` |
| route metadata lies about handler contract version | `ImplementationBindingCannotLieAboutContractVersion` | `composition.binding_metadata_mismatch` |
| duplicate scoped scope with disjoint namespaces | `DuplicateScopedScopeWithDisjointNamespacesFailsClosed` | `composition.scope_conflict` independently of namespace conflict |
| permissive `Any` admits CLR/engine object | `AnySchemaCannotAdmitClrOrEngineObjectAtCanonicalBoundary` | `portable.non_json_value`, handler not invoked |
| mandatory privilege/transaction/provenance policy missing | `MissingMandatoryPolicyMetadataFailsComposition` | `contract.missing_policy` |
| semantic contract changes without version increment | `SemanticChangeWithoutVersionIncrementIsBreaking` | compatibility classification `Breaking` |
| delimiter-bearing precondition lists collide in a serialized fingerprint | `DelimiterBearingMetadataCannotCollideInSemanticComparison` | distinct diagnostic fingerprints; structural compatibility `Breaking`; projection `projection.semantic_mismatch` |
| delimiter-bearing schema enum values collide in a serialized fingerprint | `DelimiterBearingSchemaValuesCannotCollideInSemanticComparison` | distinct diagnostic fingerprints; structural compatibility `Breaking` |
| emitted discovery artifact changes semantic metadata after projection | `EmittedDiscoveryArtifactCannotAlterSemanticMetadata` | `projection.semantic_mismatch` against the actual portable artifact |
| optional semantic field disappears from the emitted artifact and its public schema alone would allow omission | `ProjectionOracleDetectsOptionalSemanticFieldOmittedFromArtifact` | independent projection oracle reports `projection.semantic_mismatch` |

## Semantic/projection boundary regression

The rejected candidate used delimiter-joined fingerprints as its correctness oracle and compared canonical definitions with definition objects retained inside `CanonicalContractProjection`. Two different valid metadata or schema lists could therefore collide, and conformance was not bound to the portable data returned by `system.describe`.

Repair cycle 1 removes fingerprints from compatibility and conformance decisions. Exact structural equality now owns model comparison; fingerprints remain diagnostic and use length/cardinality framing. Runtime conformance dispatches `system.describe` and validates that emitted artifact against an independent model-to-data oracle that deliberately does not call the production `ToData()` projectors. The four controls above cover both collision and shared-projector false-green paths.

## Independent-universe regression

The most important proof-boundary repair was removal of the route-universe `providerIds` input. Effective route enumeration now receives assemblies only and records every concrete `ICanonicalCapabilityHandler` it finds, including unknown providers. The positive production proof independently discovers production projects under `src`, locates their Release assemblies and evaluates the entire effective handler surface against canonical definitions/dispatcher/projection.

This protects the causal class where deleting or changing registry/provider metadata could previously have made both a route and its proof obligation disappear.
