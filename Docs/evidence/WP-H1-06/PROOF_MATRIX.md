# WP-H1-06 proof matrix

Status: **REPAIR VERIFIED PHYSICALLY AFTER INDEPENDENT FAIL; FINAL EXACT-SHA READ-ONLY RECEIPT REQUIRED BEFORE FREEZE**

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
FOUNDATIONAL_PROOF_VERDICT: READY (subject to the final exact-SHA read-only receipt before freeze)
UNRESOLVED_PROOF_OBLIGATIONS: 0 within the tested H1-06 claim
KNOWN_UNDETECTED_DEFECT_CLASSES: 0 within the tested H1-06 claim
TRUST_BOUNDARY: accepted H1-04 Quaternius Source slice and catalogue, accepted H1-05 scene publication, pinned Unity Editor/project/package graph, and H1-06 managed prefab realization through the public projection path; synthetic nested prefab fixture only for causal relationship controls.

Independent review of frozen candidate `53801ae7f9990502dd5881c73717613a0fc7abe2` found a material false-green: two sibling instances of the same nested prefab with the same GameObject name collapsed to one normalized relationship because the relationship collection was keyed as a set. Removing one sibling could therefore leave source/realized comparison and the relationship/realization digest unchanged. The repair preserves relationship multiplicity as repeated normalized rows, compares source relationships as a multiset, and extends the Unity conformance fixture with two same-name sibling instances plus a one-sibling removed-override control after save/reload. The negative control also requires the raw relationship digest to change before the product observer rejects the loss with `projection.prefab-nested-lineage-missing`.

This matrix does not claim independent PASS. The complete physical-local command returned GREEN on repair SHA `e1e34da7d9625309bcdcd8c09d6f4296dcd84c73` after the local evidence gate was aligned with the repaired fixture's exact result fields. That run included the two-sibling positive, one-sibling-loss negative, flattened-child negative, approved Source and content-shape checks, locked build, focused .NET tests and public Quaternius conformance. Earlier physical-local receipts remain historical evidence for superseded SHAs. Because this evidence reconciliation changes the candidate SHA, `scripts/h1-06-local-evidence.ps1` must run read-only from a clean checkout on the final exact repair SHA before Worker pre-review/freeze. The final receipt is a durable PR comment bound to that SHA.

| Claim | Mechanism | Causal/negative control | Current state |
|---|---|---|---|
| Accepted source prefab is read-only | `H1SceneProjection.ResolveSource`, exact source path/GUID/local-file-id/content SHA, before/after source hashing | local proof re-verifies H1-04 owner Source pins after all materialization/rebuild activity | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Prefab source relationship is not flattened | source instance is saved as a Unity `PrefabAssetType.Variant`; `H1ManagedPrefabLineage` records exact source identity; normalized observation requires `variant-base` | a regular/flattened asset cannot satisfy `PrefabAssetType.Variant` + exact source dependency + lineage checks | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Nested/source dependency shape and multiplicity remain inspectable | normalized relationship rows retain every occurrence of `nested-prefab`, mesh, material and animation references; source comparison consumes matching rows as a multiset; derivative dependencies must contain every accepted source dependency | harness source contains two sibling instances of the same nested prefab with the same GameObject name; positive observation must expose two nested rows after save/reload; deleting exactly one sibling as a removed-GameObject override must change the relationship digest and make product `ObserveRealization` fail with `projection.prefab-nested-lineage-missing`; separate flattened-child control remains RED | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Managed derivatives cannot escape bridge ownership | deterministic path `Assets/Arkus/H1/ManagedPrefabs/generations/<generation>/<source-identity>.prefab` | observation rejects any derivative path outside the exact generation root with `projection.prefab-derivative-scope-escape` | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Managed derivative has stable bridge identity and source lineage | prefab generation ID is deterministic from projection input/catalogue fingerprint; derivative path is deterministic from generation + source identity; lineage marker binds source logical/native/content identity | delete/rebuild must reproduce the same path, generation and normalized relationship profile | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Missing logical source fails closed | host plan maps `catalogue.missing-reference` to `projection.source-missing` | focused .NET regression + public local conformance with `prefab.absent` | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Wrong asset type fails closed | host plan maps `catalogue.incompatible-reference` to `projection.source-wrong-type`; Editor independently checks effective Unity type | focused regression binds prefab kind to accepted mesh logical ID | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Rebound/stale source fails closed | accepted mapping/content identity is revalidated in host and effective Editor | local proof temporarily corrupts the accepted prefab mapping content hash and requires `projection.source-rebound`, then restores exact bytes | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Catalogue remapping cannot alter canonical world identity | `CanonicalHash` is computed only from `WorldState`; catalogue fingerprint is separate input evidence | .NET regression remaps an unrelated accepted catalogue logical ID and requires equal canonical hash but different catalogue fingerprint/input digest | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Same input is semantically idempotent | existing active scene is reused only when graph and realization both match | local public proof materializes twice and requires identical generation + normalized realization | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Deleted generated derivative converges | missing derivative causes active observation to fail and a fresh staged scene/variant rebuild | local proof deletes managed derivative bytes/meta, rematerializes, and compares normalized source/relationship profile | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Canonical state/journal remain authoritative | public projection path never applies H0 canonical mutations | local proof snapshots `world.summary` and `authoring.journal.read` around materialize/rebuild and diagnostics | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |
| Representative positive shape is real Quaternius, not a primitive | exact H1-04-pinned Medieval Village archive and accepted wall/window prefab/material source are used | bounded archive probe reports wall/roof/door/window/prop candidate shape without adopting new source | REPAIR SHA GREEN; FINAL SHA RECEIPT PENDING |

## Trust boundary

The H1-06 claim is bounded to the existing H1-04 catalogue/source authority and H1-05 managed-scene publication boundary. Native Unity GUIDs/local file IDs are observed locators, not canonical game identity. The stable bridge identity for a managed derivative is the deterministic managed generation/path plus exact source lineage; deleting and rebuilding may legitimately allocate a new Unity-native GUID while the normalized source/relationship identity remains equal. Relationship equality is multiplicity-sensitive: repeated equal rows are material and are included repeatedly in `relationshipDigest`.

## Residual-risk and proof-budget audit

The positive game-shaped realization is the accepted Quaternius wall/window source with effective mesh/material references. A separate ignored harness fixture tests nested-prefab mechanics because the accepted H1-04 Source slice has no nested prefab. The first independent FAIL exposed one concrete completeness gap in that fixture and comparator: duplicate equal relationship instances were collapsed. The targeted repair changes only relationship multiplicity semantics and the causal nested fixture; it does not reopen H1-04 catalogue/source authority, H1-05 publication semantics, arbitrary component fidelity, art adaptation or broader content adoption. Complex importer adaptation, arbitrary component properties, broad Cantabrian art conversion, wardrobe, vehicles, missing production props/animations and shipping delivery remain named future work outside H1-06.

## Required physical-local command

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha <exact-40-char-head>
```

The command must run from a clean checkout at the exact PR head, with physical local Unity `6000.3.24f1 (4e7b9b5b6244)`. Until that receipt is GREEN, this PR remains Draft and must not be frozen for Reviewer.
