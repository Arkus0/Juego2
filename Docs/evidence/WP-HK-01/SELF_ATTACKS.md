# WP-HK-01 causal self-attacks

Repair-cycle-2 implementation observation: `cfcb9ab25977a1f58cc85554c5150d98574e01e3`  
GitHub Actions run: `35435242336`

Observed result: 29/29 HK01 causal negative controls GREEN; 9/9 positive contract tests GREEN; 39/39 total regression GREEN; Release build 0 warnings / 0 errors.

The attacks inject invalid contract/runtime variants against the same production guards used by the canonical composer, projection conformance and dispatcher. RED means the injected defect is rejected/detected for the intended causal reason; the valid baseline remains GREEN in the same canonical run.

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
| duplicate scoped scope with disjoint namespaces | `DuplicateScopedScopeWithDisjointNamespacesFailsClosed` | `composition.scope_conflict` |
| permissive `Any` admits CLR/engine object | `AnySchemaCannotAdmitClrOrEngineObjectAtCanonicalBoundary` | `portable.non_json_value`, handler not invoked |
| mandatory policy metadata missing | `MissingMandatoryPolicyMetadataFailsComposition` | `contract.missing_policy` |
| semantic contract changes without version increment | `SemanticChangeWithoutVersionIncrementIsBreaking` | compatibility `Breaking` |
| delimiter-bearing preconditions collide in serialized comparison | `DelimiterBearingMetadataCannotCollideInSemanticComparison` | structural mismatch / distinct diagnostic fingerprints |
| delimiter-bearing schema enum values collide | `DelimiterBearingSchemaValuesCannotCollideInSemanticComparison` | structural compatibility `Breaking` |
| emitted discovery artifact changes semantic metadata | `EmittedDiscoveryArtifactCannotAlterSemanticMetadata` | `projection.semantic_mismatch` |
| optional semantic field disappears from emitted artifact | `ProjectionOracleDetectsOptionalSemanticFieldOmittedFromArtifact` | independent oracle `projection.semantic_mismatch` |
| allowed logical-reference format carries CLR/engine class+assembly identity | `AllowedLogicalReferenceFormatCannotCarryClrOrEngineTypeNamespace` | `contract.schema.non_portable_logical_reference_namespace`; composition fails |
| logical-reference namespace belongs to another provider | `LogicalReferenceNamespaceMustBelongToContributingProvider` | `composition.logical_reference_namespace_mismatch` |
| logical reference encodes implementation choices as enum values | `LogicalReferenceSchemaCannotEncodeImplementationChoicesAsEnumValues` | `contract.schema.logical_reference_enum_forbidden` |
| typed optional property added to previously open object narrows accepted old requests | `TypingAPropertyOnPreviouslyOpenObjectIsBreaking` | compatibility `Breaking` |
| equivalent narrowing hidden inside nested open object | `CompatibilityRelationRecursesThroughNestedOpenObjects` | compatibility `Breaking` |
| conservative relation accidentally rejects a true open-object widening | `OpenObjectMayAddExplicitAnyPropertyWithoutNarrowingAcceptance` | compatibility `Additive` (positive control on the relation) |
| breaking same-major definitions still become negotiable | `ComposerRejectsBreakingSameMajorVersionChainBeforeNegotiation` | `composition.breaking_same_major_version`; no `ComposedContract` |

## Repair-cycle-2 portability/evolution boundary

The second independent FAIL showed that the earlier portability proof was token-oriented rather than semantic at the logical-reference boundary, and that compatibility existed as a classifier without governing composition/negotiation. The repair therefore changes both causal boundaries:

- logical references now occupy `ref.<provider-id>.<domain>` canonical identity space; schema validation rejects non-portable namespaces and implementation-choice enums, while composition recursively checks provider ownership across request/success/error schemas;
- request compatibility is a recursive acceptance-preservation relation over the supported schema model. A same-major edge is admitted only when the newer schema accepts every value the previous schema accepted; uncertainty is breaking;
- composition evaluates every adjacent same-major edge before constructing the public contract. Dispatch therefore cannot select a higher minor version from a chain already known to be breaking.

During Worker pre-review, the seven new controls were found to be passing only under full regression because their original class name did not match the canonical `Hk01SelfAttackTests` filter. This was repaired before freeze; Actions `35435242336` proves all 29 controls are now inside the causal gate itself.

## Earlier proof-boundary regressions retained

Repair cycle 1 removed fingerprints from correctness decisions and bound conformance to the actual portable `system.describe` artifact through an independent expected-data oracle. Earlier HK01 work also removed provider-ID filtering from effective route enumeration so registry metadata cannot shrink its own proof universe. Those controls remain in the same 29-test causal gate.
