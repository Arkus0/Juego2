# WP-H1-06 proof matrix

Status: **PHYSICAL-LOCAL OBSERVATION GREEN ON PRE-EVIDENCE SHA; FINAL EXACT-SHA RECEIPT PENDING**

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

This matrix does not claim PASS. `scripts/h1-06-local-evidence.ps1` completed GREEN on pre-evidence SHA `f0b874d64cd2e4ef7d79a52d1a118ac23e3840c9`; `LOCAL_EXECUTION_RESULT.md` records that observation. Since this evidence update changes the candidate SHA, the command must run again read-only on the final exact SHA before freeze. The final receipt is a durable PR comment bound to that SHA.

| Claim | Mechanism | Causal/negative control | Current state |
|---|---|---|---|
| Accepted source prefab is read-only | `H1SceneProjection.ResolveSource`, exact source path/GUID/local-file-id/content SHA, before/after source hashing | local proof re-verifies H1-04 owner Source pins after all materialization/rebuild activity | OBSERVED GREEN; FINAL SHA PENDING |
| Prefab source relationship is not flattened | source instance is saved as a Unity `PrefabAssetType.Variant`; `H1ManagedPrefabLineage` records exact source identity; normalized observation requires `variant-base` | a regular/flattened asset cannot satisfy `PrefabAssetType.Variant` + exact source dependency + lineage checks | OBSERVED GREEN; FINAL SHA PENDING |
| Nested/source dependency shape remains inspectable | normalized relationship rows record `nested-prefab`, mesh, material and animation references where present; derivative dependencies must contain every accepted source dependency | missing dependency returns `projection.prefab-nested-lineage-missing` | OBSERVED REPRESENTATIVE MESH/MATERIAL; NESTED CLAIM UNDER PRE-REVIEW |
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

## Required physical-local command

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha <exact-40-char-head>
```

The command must run from a clean checkout at the exact PR head, with physical local Unity `6000.3.24f1 (4e7b9b5b6244)`. Until that receipt is GREEN, this PR remains Draft and must not be frozen for Reviewer.
