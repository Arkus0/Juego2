# WP-HK-09A — Content-shape probe

APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md
EXECUTABLE_PROBE: Hk09AContentShapeProbeTests.PotesMarketSliceStillSupportsInspectAuthorSnapshotAndReplayUnderH0Policy

## Product slice

The approved Juego2 visual source describes a fictional Potes/Liébana valley-town hero slice with a plaza/market grain, bar terrace and workshops. HK09A uses only that structural content shape; it does not promote art-draft details into H0 semantic requirements.

The executable fixture represents four authored resources: the plaza as container, market stalls, bar terrace and workshop front. The child resources are contained by the plaza. A wet-weather authoring edit changes the stalls and bar-terrace types while retaining stable identities and containment.

## What the probe proves

The probe composes the production canonical world contract through `CanonicalWorldContract.Compose`, so `H0HostCapabilityPolicy` is in the exercised path. It then demonstrates that the representative product slice can still:

1. inspect the world through canonical `world.summary`;
2. mutate two product-shaped resources through ordinary `authoring.apply` semantics;
3. export an authored-state snapshot;
4. read the accepted provenance journal;
5. import the clean base snapshot into a fresh policy-bound session;
6. replay the journal through canonical replay; and
7. converge to the same canonical content hash, revision and authored snapshot bytes as the source session.

This proves containment does not require shell/process execution, ambient networking, caller-selected files or runtime-type activation for the accepted H0 authoring path.

## Representability and boundary analysis

- **Identity/granularity:** plaza, stalls, terrace and workshop are independently addressable world objects; HK09A does not collapse them into one blob.
- **Containment:** the product-shaped parent/child relationship is representable through `ContainerId`; the host-capability boundary does not reinterpret it.
- **Authoring semantics:** the wet-weather change is an ordinary canonical mutation and therefore remains under the accepted transaction/provenance path.
- **Portability/replay:** snapshot/import/replay are canonical capabilities admitted by the H0 policy, not filesystem shortcuts.
- **Host authority:** no product field is interpreted as a path, URL, process command or runtime type selector.
- **Future boundary:** Unity/editor execution and engine-specific assets remain H1+ concerns; this probe makes no claim about them.

## Result

The probe is part of the `FullyQualifiedName~Hk09A` exact-SHA observation set and the full regression suite. Freeze verification requires the executable test and this evidence marker to be present on the same candidate SHA.

CONTENT_SHAPE_PROBE_VERDICT: PASS
UNRESOLVED_IN_SCOPE_FINDINGS: 0
