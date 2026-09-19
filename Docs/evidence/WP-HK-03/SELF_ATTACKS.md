# WP-HK-03 causal self-attacks

Implementation observation SHA: `05e25995d5c0ad91954bded22ab65e7c49097ea2`  
GitHub Actions run: `35438474195`

Observed result: 7/7 HK03 positive inspection tests GREEN; 7/7 HK03 causal controls GREEN; 70/70 total regression GREEN; Release build 0 warnings / 0 errors.

The dedicated self-attack gate is `Hk03SelfAttackTests`. Each control protects a concrete acceptance criterion or an observed false-green class.

| Required defect class | Causal control | RED oracle / protected cause |
|---|---|---|
| hidden authorable field | `HiddenAuthorableStateMutantBreaksIndependentReadReconstructionHash` | independent HK02 property universe + public-read reconstruction must reproduce canonical hash; hidden extension bytes change it |
| nondeterministic query order | `NondeterministicQueryOrderMutantCannotLeakInputOrdering` | semantically identical worlds with opposite input ordering must produce the same first row and opaque cursor |
| unbounded result path | `UnboundedNestedRelationshipMutantIsSplitIntoBoundedReferencePages` | 130 references cannot escape nested through one object response; canonical relationship output is 100 + 30 rows |
| unbounded declared limit | `UnboundedDeclaredLimitsFailClosed` | object page size >100 and extension chunk size >768 return `world.invalid_selector` at `$.limit` |
| stale cursor/revision | `StaleRevisionAndCursorAreRejectedAfterSourceAdvances` | old revision returns `world.stale_revision`; old cursor used with the current anchor returns `world.stale_cursor` |
| selector escapes grammar | `SelectorCannotEscapeDeclaredStableTokenGrammar` | expression-like identifier `node.child || true` is rejected as `world.invalid_selector` rather than evaluated/interpreted |
| discovered read missing schema / route drift | `EveryEffectiveWorldReadIsDiscoveredAndMissingSchemaMutantTurnsRed` | independent reflected `ICanonicalCapabilityHandler` route universe must conform to canonical composition; a capability definition with missing request schema produces `contract.missing_schema` |

## Material defect found and removed during Worker implementation

The initial object projection included each object's entire `References` list. HK02 intentionally does not impose a fixed relationship cardinality, so object pagination alone did not bound that response path: one object could still return arbitrarily many relationship rows. The Worker treated this as a class defect, not a one-off limit bug.

Repair: object responses now contain only fixed-cardinality identity/type/container fields. All relationship reconstruction goes through `world.reference.query`, whose page size is capped at 100 and whose deterministic cursor binds command, selector/projection context, world revision/hash and offset. The 130-edge self-attack above is the regression oracle for this specific false-green class.

## Effective-route completeness

HK01's independent `RouteUniverse` proof was intentionally kept active over the expanded production assembly. The first real HK03 CI pass exposed all six new `world.*` handlers as unclassified extras against the old base-only composition. The repair changed the effective canonical composition used by the route-universe proof to include the HK03 definitions; it did not narrow reflection roots or ignore the new handlers. Thus route additions cannot disappear merely by omitting themselves from the HK03 definition list.
