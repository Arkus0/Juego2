# WP-H1-06 PREDECESSOR_CONTRACT_CHECK

Bootstrap baseline: `b8724730ae7e8bfbd14bb14f58b41b64ef6909f4` (`main`, 2026-09-24).

## Accepted predecessor and authority

- Direct dependency `WP-H1-05`: canonical implementation PR `#192`, accepted candidate `186224fbc3f53eb9c47ae528a56dcf3514af163f`, independent PASS review `#5308430565`, implementation merge `7039adecac4e6e07d899247759d9b3c9fd3c2dae`; DocSync PR `#193` merged as `b8724730ae7e8bfbd14bb14f58b41b64ef6909f4` and records `DOCSYNC_COMPLETE` with `WP-H1-06` as the next dependency-valid H1 workpack.
- H1-05 consumes accepted `WP-H1-04` catalogue/source authority. H1-04's accepted identity is implementation PR `#185`, candidate `8c6ffd61d17e832ed5b9f900e8c0f7d4e85bf5f5`, independent PASS `#5306149043`, implementation merge `4f172f7aa9e7c4a0909be046d095eac5477ad765`, DocSync merge `c6db2398e780974bd4340609b2cb02e809b33a1e`.
- Binding authority for this boundary is `Docs/workpacks/H1/WP-H1-06.md`, the accepted H1 architecture/ADRs, `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`, and the accepted H1-04/H1-05 implementation/evidence. Lifecycle mechanics follow `Docs/engineering/PRODUCT_SHA_CLOSURE.md` and `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`.

## Inherited guarantees consumed

- H0 remains canonical game-state authority; Unity native paths, GUIDs, local file IDs, GameObjects and prefab contents are bridge locators/projection state rather than canonical truth.
- H1-01 owns logical asset-binding semantics and dependency identity.
- H1-02/H1-03/H1-03A own the pinned Unity project/editor, admitted Editor authority, invocation lifecycle and public composed execution path.
- H1-04 owns the accepted Quaternius Source baseline, effective catalogue universe, logical/native identity, source provenance and catalogue snapshot/staleness guarantees. H1-06 does not re-adopt or broaden that source.
- H1-05 owns managed scene graph realization, staged generation/publication, active-manifest ownership, normalized scene observation, same-input semantic idempotence and deletion/rebuild convergence for the scene projection boundary.

These accepted guarantees are consumed rather than duplicated. H1-06 may falsify them only with concrete effective-Unity evidence showing that the accepted guarantee is false or inapplicable to the path under test.

## Guarantees newly owned by H1-06

- truthful resolution and realization of logical asset/prefab bindings without mutating accepted upstream source assets;
- normalized preservation of prefab source, instance and nested-prefab relationships across save/reload;
- bridge-managed prefab derivative identity, explicit source lineage and generation receipt, constrained to managed roots;
- stable fail-closed diagnostics for missing, wrong-type and rebound logical asset/prefab identities;
- catalogue remapping changes catalogue/plan evidence while leaving canonical world identity unchanged;
- semantic idempotence and delete/rebuild convergence for managed prefab derivatives;
- one game-shaped positive proof using accepted Quaternius Source wall/roof/door/window/prop relationships, with harness-only malformed fixtures used only for causal controls.

## Intentionally not re-proved

Catalogue completeness, H1-04 source adoption/licensing, H1-05 scene publication mechanics, arbitrary component-property fidelity, Unity YAML byte stability, broad art production/Cantabrian transformation, gameplay and production-asset breadth remain outside H1-06.

## Concrete reopen conditions

- Reopen H1-04 only if effective Unity evidence proves its accepted catalogue mapping/source identity false or inapplicable to the exact Quaternius input used here.
- Reopen H1-05 only if effective execution proves its accepted generation/publication or normalized scene-observation guarantee is false for an otherwise valid H1-06 prefab realization.
- Otherwise, source mutation, flattened/omitted prefab lineage, derivative escape outside the managed root, wrong/missing/rebound asset acceptance, canonical-hash contamination, idempotence defects and derivative rebuild defects are H1-06 obligations.
