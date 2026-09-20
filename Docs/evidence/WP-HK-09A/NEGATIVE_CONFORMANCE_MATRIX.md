# WP-HK-09A — Negative conformance matrix

This matrix covers every negative class required by WP-HK-09A. The tests act only against the repository-local H0 host and canonical protocol; they are not external-system security tests.

| Required negative | Causal executable oracle | Expected fail-closed boundary | Status |
|---|---|---|---|
| Path traversal | `Hk09AHostCapabilityContainmentTests.ParentTraversalFileAttemptFailsBeforeProtocolOutput` | Production CLI rejects `--file` with usage failure before protocol framing or filesystem open | PASS |
| Symlink escape | `Hk09AHostCapabilityContainmentTests.SymlinkEscapeFileAttemptFailsBeforeProtocolOutput` | A real symlink path is rejected by the same pre-open production boundary | PASS |
| Generic process / shell execution | `Hk09AHostCapabilityContainmentTests.UnsupportedGenericProcessCapabilityCannotBeMintedByProtocolPayload` plus effective-source inventory | Invented `host.process.execute` remains unknown; production source contains no admitted process-launch primitive | PASS |
| Network-shaped payload | `Hk09AHostCapabilityContainmentTests.NetworkShapedPayloadCannotAcquireNetworkAuthority` | URL-shaped data is canonical input only; a loopback listener observes no connection | PASS |
| Runtime type selector | `Hk09AHostCapabilityContainmentTests.RuntimeTypeSelectorCannotAcquireRuntimeActivationAuthority` plus effective-source inventory | `$type`-shaped data is rejected by schema and no arbitrary runtime activation primitive is present | PASS |
| Adapter-only privileged capability | `Hk09AHostCapabilityContainmentTests.AdapterOnlyPrivilegedCapabilityCannotAppearOutsideCanonicalInventory` | Production discovery contains no external/elevated capability and invented filesystem authority remains unknown | PASS |
| Transport path skipping host policy | `Hk09ATransportPolicyBoundaryTests.TransportPathCannotProjectCompositionThatSkippedHostCapabilityAdmission` | A direct `ContractComposer` result containing H0-forbidden external authority may exist at the generic canonical layer, but `NeutralProjectionService` re-applies H0 admission before any transport can expose capabilities or dispatch | PASS |

## Causal RED→GREEN for the below-transport boundary

The independent review of candidate `1c85a64a5d3930ad2e39451fb8db2c1fac6ae78b` found a real alternate path: `ContractComposer.Compose(...)` could create a canonical `ComposedContract` and the public `NeutralProjectionService(ComposedContract)` constructor accepted it without crossing `H0HostCapabilityPolicy`. A transport could therefore remain a faithful HK07 projection while still skipping the new HK09A host-policy admission.

`TransportPathCannotProjectCompositionThatSkippedHostCapabilityAdmission` isolates that defect causally. The fixture first proves that generic canonical composition succeeds with a valid scoped capability whose declared side effect is `ExternalReversible`; the test then attempts the exact composition → neutral-projection seam. The repaired constructor rejects with `host-policy.external-effect-forbidden` before a projection instance exists and before the fixture handler can run. Under the reviewed pre-repair constructor, the same composed contract would have been accepted by the projection and the negative would fail.

## Independent effective-surface oracle

`EffectiveProductionSourceSurfaceHasNoShellNetworkOrRuntimeActivationPrimitive` enumerates production C# source independently of the canonical capability registry and fails if reviewed shell/process, network or arbitrary runtime-activation primitives appear. It separately inventories filesystem primitives and pins the only legacy occurrence to `ReferenceTransport.cs`, while asserting the production executable rejects `--file` before delegation. The MCP production entrypoint is also checked to compose `ProductionHarnessHost.Create()` and not add IO/network authority.

This oracle exists specifically to avoid the self-referential false-green class “policy registry says no dangerous capability because the dangerous path is not registered.”

## Policy metadata negative controls

`PolicyRejectsExternalEffectsElevatedPrivilegeAndContradictoryMetadata` supplies deliberately invalid canonical definitions and verifies that H0 admission rejects external side effects, elevated privilege and contradictory side-effect/transaction metadata.

NEGATIVE_CONFORMANCE_VERDICT: PASS
UNRESOLVED_REQUIRED_NEGATIVES: 0
