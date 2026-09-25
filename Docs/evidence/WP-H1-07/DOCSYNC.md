# WP-H1-07 — Post-acceptance DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  
DATE: 2026-09-25

## Accepted result

- Accepted candidate SHA: `a11f9c012307ee2c1d925eb1a1be143f1d127d21`
- Canonical implementation PR: `#206`
- Final independent PASS review: `#5315878884`
- Implementation merge: `f733b2fd50ab0aed27fce99d5ca7773969f3f339`
- Arkus Main Safety: run `36116080041` GREEN on the exact accepted candidate.
- Bounded H1-07 Unity Validation: run `36116080037`, job `108010496308`.

## Accepted claim

H1-07 establishes the bounded allowlisted component-adapter boundary required by the accepted H1 slice without creating a generic reflection/SerializedProperty mutation endpoint or a parallel discovery/transport registry.

The accepted implementation provides exact declared-schema ↔ effective-adapter completeness for the initial Transform, MeshRenderer/material, Animator/clip-reference and canonical-link schemas; normalized deterministic component observation; exact catalogue/canonical reference identity; and fail-closed handling for unsupported/rebound component semantics before successful active-generation publication.

The representative Unity round-trip proved materialize -> save/reload -> inspect/rebuild determinism for the bounded component mix. The accepted UAL1 source clip is Legacy, so the adapter preserves the exact imported `AnimationClip` Unity object reference on the H1 ownership marker and type-checks it as `AnimationClip` during observation. H1-07 does not claim animation playback and does not mutate or clone the accepted source asset.

## Circuit-breaker disposition

The remaining RED in `H1-07 Unity Validation` is not accepted as a semantic H1-07 failure.

The exact bounded run executed only `H1ComponentProjectionTests`: the positive multi-component round-trip passed; the two negative controls reached the required stable fail-closed codes `projection.component-reference-rebound` and `projection.component-schema-unsupported` while retaining the prior active generation. Unity Test Framework marked those two controls FAIL because `H1SceneProjection` deliberately emitted the expected rejection through `Debug.LogError` and the tests did not declare the expected log with `LogAssert.Expect`.

Under the accepted `H1_07_COMPRESSION_AMENDMENT.md`, a failed job is an H1-07 blocker only when it directly falsifies one of the six owned component-adapter claims. This RED therefore records a proof-harness expectation defect around deliberate negative-control logging, not a product-semantic contradiction. It does not reopen H1-04, H1-05, H1-06, generic asset import, animation playback or broad projection scope.

## DocSync actions

1. Mark `WP-H1-07` COMPLETE / ACCEPTED on the exact candidate, independent review and implementation merge identities above.
2. Persist the circuit-breaker classification without rewriting the Unity job itself as GREEN.
3. Preserve the accepted H1-07 compression boundary and all consumed predecessor guarantees.
4. Advance the H1 execution spine to `WP-H1-08 — Unity-owned validation and stable diagnostics` as the next dependency-valid workpack.
5. Keep H1-08 `NOT_STARTED` until explicitly started by the human/automation controller.

## Boundary

This DocSync is documentation/current-state reconciliation only. It does not modify runtime/editor/product code, change the accepted source slice, claim animation playback, repair the negative-control logging harness, or pre-authorize H1-08 implementation.

## Next action

Next default H1 workpack: `WP-H1-08 — Unity-owned validation and stable diagnostics`.

It is dependency-valid from accepted H1-07 but remains `NOT_STARTED` until explicitly started.

`DOCSYNC_COMPLETE`
