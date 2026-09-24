# WP-H1-06 proof matrix

Status: **PHYSICAL-LOCAL PROOF GREEN ON IMPLEMENTATION SHA; FINAL EVIDENCE-BEARING SHA RECEIPT PENDING**

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: accepted H1-04 Quaternius Source slice and catalogue, accepted H1-05 scene publication, pinned Unity Editor/project/package graph, and H1-06 managed prefab realization through the public projection path; synthetic nested prefab fixture only for a causal relationship control.

This matrix does not claim independent PASS. `scripts/h1-06-local-evidence.ps1` completed GREEN on implementation SHA `1404da8b312b86046a036fde0f5fc179b466f7d0`, including the added nested-prefab save/reload and flattened-child control; `LOCAL_EXECUTION_RESULT.md` records the earlier and current observations. Since this evidence update changes the candidate SHA, the command must run again read-only on the final exact SHA before freeze. The final receipt is a durable PR comment bound to that SHA.

| Claim | Mechanism | Causal/negative control | Current state |
|---|---|---|---|
| Accepted source prefab is read-only | `H1SceneProjection.ResolveSource`, exact source path/GUID/local-file-id/content SHA, before/after source hashing | local proof re-verifies H1-04 owner Source pins after all materialization/rebuild activity | OBSERVED GREEN; FINAL SHA PENDING |
| Prefab source relationship is not flattened | source instance is saved as a Unity `PrefabAssetType.Variant`; `H1ManagedPrefabLineage` records exact source identity; normalized observation requires `variant-base` | a regular/flattened asset cannot satisfy `PrefabAssetType.Variant` + exact source dependency + lineage checks | OBSERVED GREEN; FINAL SHA PENDING |
| Nested/source dependency shape remains inspectable | normalized relationship rows record `nested-prefab`, mesh, material and animation references where present; derivative dependencies must contain every accepted source dependency and each source relationship | real Quaternius mesh/material observation plus harness-only nested variant saved/reloaded through product `ObserveRealization` with a `nested-prefab` row; flattened-child fixture returns `projection.prefab-nested-lineage-missing` | OBSERVED GREEN; FINAL SHA PENDING |
| Managed derivatives cannot escape bridge ownership | deterministic path `Assets/Arkus/H1/ManagedPrefabs/generations/<generation>/<source-identity>.prefab` | observation rejects any derivative path outside the exact generation root with `projection.prefab-derivative-scope-escape` | CODED; LOCAL EXECUTION PENDING |
| Managed derivative has stable bridge identity and source lineage | prefab generation ID is deterministic from projection input/catalogue fingerprint; derivative path is deterministic from generation + source identity; lineage marker binds source logical/native/content identity | delete/rebuild must reproduce the same path, generation and normalized relationship profile | OBSERVED GREEN; FINAL SHA PENDING |
| Missing logical source fails closed | host plan maps `catalogue.missing-reference` to `projection.source-missing` | focused .NET regression + public local conformance with `prefab.absent` | OBSERVED GREEN; FINAL SHA PENDING |
| Wrong asset type fails closed | host plan maps `catalogue.incompatible-reference` to `projection.source-wrong-type`; Editor independently checks effective Unity type | focused regression binds prefab kind to accepted mesh logical ID | OBSERVED GREEN; FINAL SHA PENDING |
| Rebound/stale source fails closed | accepted mapping/content identity is revalidated in host and effective Editor | local proof temporarily corrupts the accepted prefab mapping content hash and requires `projection.source-rebound`, then restores exact bytes | OBSERVED GREEN; FINAL SHA PENDING |
| Catalogue remapping cannot alter canonical world identity | `CanonicalHash` is computed only from `WorldState`; catalogue fingerprint is separate input evidence | .NET regression remaps an unrelated accepted catalogue logical ID and requires equal canonical hash but different catalogue fingerprint/input digest | FOCUSED .NET GREEN; FINAL SHA PENDING |
| Same input is semantically idempotent | existing active scene is reused only when graph and realization both match | local public proof materializes twice and requires identical generation + normalized realization | OBSERVED GREEN; FINAL SHA PENDING |
| Deleted generated derivative converges | missing derivative causes active observation to fail and a fresh staged scene/variant rebuild | local proof deletes managed derivative bytes/meta, rematerializes, and compares normalized source/relationship profile | OBSERVED GREEN; FINAL SHA PENDING |
| Canonical state/journal remain authoritative | public projection path never applies H0 canonical mutations | local proof snapshots `world.summary` and `authoring.journal.read` around materialize/rebuild and diagnostics | OBSERVED GREEN; FINAL SHA PENDING |
| Representative positive shape is real Quaternius, not a primitive | exact H1-04-pinned Medieval Village archive and accepted wall/window prefab/material source are used | bounded archive probe reports wall/roof/door/window/prop candidate shape without adopting new source | OBSERVED GREEN; FINAL SHA PENDING |

## Trust boundary

The H1-06 claim is bounded to the existing H1-04 catalogue/source authority and H1-05 managed-scene publication boundary. Native Unity GUIDs/local file IDs are observed locators, not canonical game identity. The stable bridge identity for a managed derivative is the deterministic managed generation/path plus exact source lineage; deleting and rebuilding may legitimately allocate a new Unity-native GUID while the normalized source/relationship identity remains equal.

## Residual-risk and proof-budget audit

The positive game-shaped realization is the accepted Quaternius wall/window source with effective mesh/material references. A separate ignored harness fixture tests the nested-prefab mechanism because the accepted H1-04 Source slice has no nested prefab. The archive probe found broader wall/roof/door/window/prop shapes but does not adopt them. Complex importer adaptation, arbitrary component properties, broad Cantabrian art conversion, wardrobe, vehicles, missing production props/animations and shipping delivery remain named future work outside H1-06. No concrete predecessor contradiction was observed. The added nested control closes an explicit H1-06 acceptance gap, so its proof cost remains within budget.

## Required physical-local command

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha <exact-40-char-head>
```

The command must run from a clean checkout at the exact PR head, with physical local Unity `6000.3.24f1 (4e7b9b5b6244)`. Until that receipt is GREEN, this PR remains Draft and must not be frozen for Reviewer.
