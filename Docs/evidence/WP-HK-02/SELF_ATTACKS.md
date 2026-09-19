# WP-HK-02 causal self-attacks

Implementation observation SHA: `58ca3eab4090c211126425be1e3950f503f48000`  
GitHub Actions run: `35436688357`

Observed result: 11/11 HK02 causal controls GREEN; 6/6 positive state tests GREEN; 56/56 total regression GREEN; Release build 0 warnings / 0 errors.

These controls inject invalid state/payload variants or an intentionally order-sensitive comparison against the production validator/codec. A GREEN control means the injected defect is rejected/detected for the intended causal reason while the valid baseline remains accepted.

| Defect class | Causal control | RED oracle / protected cause |
|---|---|---|
| unstable ordering | `UnstableOrderingMutantTurnsRedWhileCanonicalCodecStaysStable` | naive order fingerprint differs while canonical bytes remain equal |
| duplicate ID | `DuplicateIdentityFailsClosed` | `world.duplicate_id` |
| dangling reference | `DanglingReferenceFailsClosed` | `world.dangling_reference` |
| serialization-order noise changes hash | `SerializationOrderNoiseCannotChangeContentHash` | reordered input has identical canonical hash |
| hidden/nonserialized semantic state | `EveryCurrentSemanticFieldClassChangesCanonicalHashWhenMeaningChanges` | every current semantic input-class mutation changes hash |
| semantic surface grows outside proof universe | `SemanticInputAndPropertyUniverseCannotGrowWithoutUpdatingCanonicalProof` | reflection surface differs from independent expected constructor/property universe |
| unsupported schema/version payload | `UnsupportedSchemaPayloadFailsClosed` | `world.unsupported_schema` |
| unknown structural forward data silently dropped | `UnknownStructuralRecordFailsClosedInsteadOfBeingSilentlyDropped` | `world.unknown_record` |
| dangling containment | `DanglingContainmentFailsClosed` | `world.dangling_container` |
| containment cycle | `ContainmentCycleFailsClosed` | `world.containment_cycle` |
| semantically readable but noncanonical byte ordering | `NonCanonicalPayloadOrderingFailsClosedEvenWhenSemanticsCouldBeRecovered` | `world.noncanonical_payload` |

## Forward-compatible policy exercised

The positive round-trip fixture carries opaque `future.alpha@2` and `future.beta@1` extension bytes. Those payloads are not interpreted by HK02 and must round-trip byte-for-byte. By contrast, unknown outer structural records and unsupported outer world-schema versions fail closed. This deliberately distinguishes an extension envelope Arkus promises to preserve from structure whose semantics Arkus does not know.

## Worker pre-review repair

The semantic-surface control was initially present only in full regression because its class name did not match the canonical `Hk02SelfAttackTests` filter. Worker pre-review detected the false-green risk and renamed the class under that filter. Actions `35436688357` proves 11/11 causal controls are now executed by the dedicated gate itself.
