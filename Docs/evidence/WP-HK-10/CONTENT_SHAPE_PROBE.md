# WP-HK-10 representative content-shape probe

HK10 is closure-only and does not change the authored-world object/extension model. The exact candidate does contain a public canonical-dispatch failure amendment discovered during closure, but that semantic change is now explicitly owned by the reopened `WP-HK-01` contract and is reviewed on the same candidate. Because the combined foundational candidate changes public-contract semantics, the Foundational Proof Standard v1.3 still requires a bounded representative content-shape probe before freeze.

## Approved slice

The approved source remains `Docs/art/VISUAL_BIBLE.md`. HK10 re-executes the already reviewed Potes hero-slice probe `Hk09BContentShapeProbeTests.PotesHeroSliceFitsOneBoundedTransactionAndCheckpointRebase` through the current combined candidate surface rather than inventing a second product fixture.

The slice represents plaza, market, bar-terrace and workshop authored content as 72 canonical objects plus 24 opaque extensions in one accepted 96-operation transaction. It then exercises public summary inspection, snapshot export and checkpoint import/rebase into a fresh session, requiring matching canonical revision/hash.

## Why reuse is appropriate

HK10 owns quality closure, not a new content model. A new bespoke content vocabulary would add proof machinery without increasing representativeness. Re-running the accepted product-shaped slice under the combined candidate is sufficient to detect whether the separately owned HK01 dispatcher amendment or HK10 proof changes accidentally break ordinary authored-content representability or the inspect/mutate/snapshot boundary it traverses.

The probe is an omission detector, not the completeness oracle. HK10's completeness claims remain supported by the property/robustness suite, the 12 causal negative-conformance controls, the independent Protocol v1 corpus and residual-ledger reconciliation. HK01's dispatcher-failure semantics are owned and directly proved by `Hk01DispatchFailureContractTests`, not by this content fixture.

## Findings and classification

- Representability: PASS. The bounded Potes slice remains representable through accepted object/extension semantics.
- Identity/granularity: PASS. HK10 introduces no new identity or granularity rule.
- Mutation/inspection/snapshot/rebase path: PASS under the executable probe.
- Canonical dispatcher amendment: no content-specific mismatch found; the change affects failure definition, not successful product-shaped authoring semantics, and is causally owned by HK01.
- Gameplay transforms, schedules, simulation and other H1/runtime semantics remain outside H0 and are not promoted by this probe.
- No authored-state predecessor guarantee requires reopening from this slice. The separate HK01 dispatcher reopen was triggered by the independent review's ownership finding, not by a content-shape mismatch.

The canonical HK10 observation runs the HK01 owner proof and this representative probe before causal defect injection and full regression.

CONTENT_SHAPE_PROBE_VERDICT: PASS
APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md
UNRESOLVED_IN_SCOPE_FINDINGS: 0
