# WP-HK-01 causal self-attacks

Implementation observation: `6cf1e6c5cdd76946f57a92b9d56a6b77267fc1dd`  
GitHub Actions run: `35432812488`  
Observed result: 18/18 HK01 negative controls GREEN; 9/9 positive contract tests GREEN; 28/28 total regression GREEN.

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

## Independent-universe regression

The most important proof-boundary repair was removal of the route-universe `providerIds` input. Effective route enumeration now receives assemblies only and records every concrete `ICanonicalCapabilityHandler` it finds, including unknown providers. The positive production proof independently discovers production projects under `src`, locates their Release assemblies and evaluates the entire effective handler surface against canonical definitions/dispatcher/projection.

This protects the causal class where deleting or changing registry/provider metadata could previously have made both a route and its proof obligation disappear.
