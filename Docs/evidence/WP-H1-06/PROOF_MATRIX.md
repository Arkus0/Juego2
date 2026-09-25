# WP-H1-06 proof matrix

Status: **SECOND REPAIR INTEGRATION CORRECTED AFTER PHYSICAL UNITY FALSE RED; FINAL EXACT-SHA PHYSICAL-LOCAL RECEIPT REQUIRED BEFORE FREEZE**

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
FOUNDATIONAL_PROOF_VERDICT: READY (subject to the final exact-SHA physical-local receipt before freeze)
UNRESOLVED_PROOF_OBLIGATIONS: 0 within the tested H1-06 relationship claim after remote repair
KNOWN_UNDETECTED_DEFECT_CLASSES: 0 within the declared H1-06 relationship universe after closing both inequality directions
TRUST_BOUNDARY: accepted H1-04 Quaternius Source slice and catalogue, accepted H1-05 scene publication, pinned Unity Editor/project/package graph, and H1-06 managed prefab realization through the public projection path; one synthetic nested-prefab fixture is used only as a causal falsifier of the generic relationship equality rule. A real Quaternius public-path control exercises an extra relationship in an existing managed derivative and its reuse/rebuild/stage/publish recovery.

## Foundational circuit-breaker audit

The two independent FAILs are not separate relation-type bugs. They are the two directions of one incomplete equality claim:

- first FAIL (`53801ae7f9990502dd5881c73717613a0fc7abe2`): **missing / under-count** — source had two equal nested-prefab occurrences while realized had one; the old set-shaped representation lost multiplicity;
- second FAIL (`5a771e8f28bf67e3fa235a88fd2ffd2d4520f5d4`, review comment `#5826423094`): **extra / over-count** — the multiplicity repair consumed every expected source occurrence from realized but did not reject leftover realized occurrences, so `source multiset ⊂ realized multiset` could still be accepted and republished.

The repaired oracle therefore has one rule only:

> **expected source-derived relationship multiset == effective realized source-derived relationship multiset**

No nested/mesh/material/animation-specific guard is added. The normalized key already contains relationship kind, relative object path, asset path, GUID, local file ID and runtime type name, and repeated equal rows remain repeated. Exact multiset equality consequently detects missing rows, insufficient multiplicity, unexpected rows, excess multiplicity, and identity/path/type changes with the same primitive.

Strict Worker pre-review of the clean physical result at `9a25fa5217cf8dbed33c41b8cab14f381d3ae96e` found one more way for the relationship universe to shrink: `AddRelationship` silently returned when an effective non-null mesh/material/clip/prefab reference had no stable Unity asset identity or path. That would let an extra transient reference disappear from both observation and digest. It now fails closed with `projection.prefab-reference-unresolved`. A transient material reference on the nested fixture's effective scene instance makes the public observer RED for that code, after which the original fixture is reloaded and the remaining equality controls run. This closes an identified omission in the same source-derived oracle; it does not add a new relation-kind policy.

## Relationship universe and wrapper invariants

`variant-base` is deliberately **not** part of the source-derived multiset. It is a managed-wrapper/lineage invariant: H1-06 requires the derivative to be a `PrefabAssetType.Variant`, to live at the deterministic managed generation/path, to carry exact `H1ManagedPrefabLineage`, to retain the accepted source dependency, and public observation to expose the exact source `variant-base` row.

The source-derived multiset is the remainder of H1-06's declared observed relationship universe:

- `nested-prefab`;
- `mesh-reference`;
- `material-reference`;
- `animation-clip-reference`.

`CollectRelationships` produces the normalized rows used by observation/digest. `ValidateSourceRelationships` filters only the wrapper `variant-base` row and compares those same normalized source-derived rows as an exact multiset. `RelationshipDigest` orders by the same `RelationshipKey`, so validation and convergence do not maintain divergent definitions of relationship identity.

For a scene instance, `CollectRelationships` stops at each descendant `H1ManagedMarker`: that descendant is a separate canonical scene node whose prefab source relationships are observed and validated under its own node. It still traverses every unmarked prefab internal under the current node. This boundary is needed for the accepted four-node Quaternius scene: counting `bar.potes` and `workshop.potes` as extra relationships of their parent `market.potes` made exact equality reject a valid projection. The physical run on clean `96775f1daaab3252362c5fac36dfd76122493316` exposed that false red. The corrected public-path trial passed on the working repair, including the nested canonical children, before the final exact-SHA receipt.

The exact oracle also runs inside `ValidateDerivativeAsset`. Therefore `RealizeDerivative` cannot reuse a deterministic pre-existing derivative merely because path/type/lineage are valid: any missing or extra source-derived row makes validation fail, the existing derivative is deleted, and the bridge rebuilds it from the unchanged source before staging/publication. Stale generated-state drift is repaired rather than adopted as new truth.

## Causal controls

The single harness fixture uses two same-name sibling instances of one nested prefab so that equal keys exercise multiplicity directly rather than depending on relation-type enumeration:

1. source=2 / realized=2 survives save/reload; public `ObserveRealization` exposes both occurrences and a relationship digest;
2. delete one sibling, save/reload: source=2 / realized=1 changes the digest and fails with `projection.prefab-nested-lineage-missing`;
3. rebuild the managed variant with all original rows plus one third identical source-derived occurrence, save/reload: source=2 / realized=3 changes the digest and fails with `projection.prefab-source-reference-unexpected` both through the equality oracle and product observation;
4. rebuild the same managed path cleanly from unchanged source: validation is GREEN and the relationship digest converges to the original positive digest;
5. the previously accepted flattened-child negative remains RED.
6. a non-null transient material reference with no asset identity is RED as `projection.prefab-reference-unresolved` rather than silently omitted from observation.

The public Quaternius control additionally mutates one existing deterministic managed variant by adding one material-reference occurrence without changing source or lineage. The next public `unity.host.projection.materialize` must reject reuse, rebuild the derivative, stage a new scene, publish it, and return the original normalized relationship profile. A subsequent public observe must report the recovered generation as current; delete/rebuild must still converge. This tests the second FAIL's actual reuse-to-publication path, not only the standalone equality predicate.

This is one falsifier of equality in both directions, not a fixture matrix per relationship kind.

Earlier physical-local GREEN receipts, including repair SHA `e1e34da7d9625309bcdcd8c09d6f4296dcd84c73`, are historical evidence for superseded candidates only. The complete run on `96775f1daaab3252362c5fac36dfd76122493316` reached the public Quaternius proof and failed with `projection.prefab-source-reference-unexpected` because canonical child nodes were counted in their parent's relationship multiset. That result is preserved as a failed integration observation, not a GREEN receipt. The corrected second repair changes production code and public proof; it requires one complete physical-local execution on the final clean exact SHA before Worker freeze/Ready. No independent PASS is claimed here.

| Claim | Mechanism | Causal/negative control | Current state |
|---|---|---|---|
| Accepted source prefab is read-only | `H1SceneProjection.ResolveSource`, exact source path/GUID/local-file-id/content SHA, before/after source hashing | local proof re-verifies H1-04 owner Source pins after all materialization/rebuild activity | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Prefab source relationship is not flattened | source instance is saved as a Unity `PrefabAssetType.Variant`; `H1ManagedPrefabLineage` records exact source identity; normalized observation requires `variant-base` | regular/flattened asset cannot satisfy Variant + exact source dependency + lineage checks; flattened harness control remains RED | REMOTE REPAIR COMPLETE; EXACT-SHA LOCAL RECEIPT PENDING |
| Source-derived relationship multiset is exact per canonical node | one normalized `RelationshipKey` and multiplicity-preserving rows cover nested prefab, mesh, material and animation references; unresolved non-null references fail closed; `ValidateSourceRelationships` requires equality after excluding only `variant-base`; separate marked canonical child subtrees are evaluated under their own nodes | same two-sibling fixture proves source=2/realized=1 RED and source=2/realized=3 RED; transient material reference is RED; four-node Quaternius public proof requires valid parent/child scene topology GREEN | LOCAL TRIAL GREEN; FINAL EXACT-SHA RECEIPT PENDING |
| Existing managed derivative drift cannot be adopted | `ValidateDerivativeAsset` invokes the same exact relationship oracle before `RealizeDerivative` reuses an existing deterministic derivative | public Quaternius proof adds an extra material reference to an existing variant, then requires materialize to rebuild, stage and publish the original normalized profile | LOCAL TRIAL GREEN; FINAL EXACT-SHA RECEIPT PENDING |
| Relationship validation and digest share identity semantics | `CollectRelationships` + `NormalizeRelationships` + `RelationshipKey` feed exact comparison and `RelationshipDigest`; duplicates are retained | both under-count and over-count controls must change raw relationship digest; recovery must restore it | REMOTE REPAIR COMPLETE; EXACT-SHA LOCAL RECEIPT PENDING |
| Managed derivatives cannot escape bridge ownership | deterministic path `Assets/Arkus/H1/ManagedPrefabs/generations/<generation>/<source-identity>.prefab` | observation rejects derivative outside exact generation root with `projection.prefab-derivative-scope-escape` | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Managed derivative has stable bridge identity and source lineage | prefab generation ID derives from projection input/catalogue fingerprint; derivative path derives from generation + source identity; lineage marker binds logical/native/content source | clean rebuild at same managed identity must reproduce normalized source-derived multiset/digest | REMOTE REPAIR COMPLETE; EXACT-SHA LOCAL RECEIPT PENDING |
| Missing logical source fails closed | host plan maps `catalogue.missing-reference` to `projection.source-missing` | focused .NET regression + public local conformance with `prefab.absent` | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Wrong asset type fails closed | host plan maps `catalogue.incompatible-reference` to `projection.source-wrong-type`; Editor independently checks effective Unity type | focused regression binds prefab kind to accepted mesh logical ID | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Rebound/stale source fails closed | accepted mapping/content identity is revalidated in host and effective Editor | local proof corrupts accepted prefab mapping content hash, requires `projection.source-rebound`, then restores exact bytes | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Catalogue remapping cannot alter canonical world identity | `CanonicalHash` depends only on `WorldState`; catalogue fingerprint is separate input evidence | .NET regression remaps unrelated accepted catalogue logical ID and requires equal canonical hash but different catalogue fingerprint/input digest | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Same input is semantically idempotent | active scene is reused only when graph and realization both match; derivative reuse additionally requires exact source-derived multiset | public proof materializes twice and compares normalized realization | REMOTE REPAIR COMPLETE; EXACT-SHA LOCAL RECEIPT PENDING |
| Deleted/generated derivative converges | invalid or missing derivative is rebuilt from accepted source before staging | local proof deletes managed derivative and compares normalized source/relationship profile; synthetic extra control rebuilds back to original digest | REMOTE REPAIR COMPLETE; EXACT-SHA LOCAL RECEIPT PENDING |
| Canonical state/journal remain authoritative | public projection path never applies H0 canonical mutations | local proof snapshots `world.summary` and `authoring.journal.read` around materialize/rebuild and diagnostics | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |
| Representative positive shape is real Quaternius, not a primitive | exact H1-04-pinned Medieval Village archive and accepted wall/window prefab/material source are used | bounded archive probe reports wall/roof/door/window/prop shape without adopting new source | REMOTE UNCHANGED; EXACT-SHA LOCAL RECEIPT PENDING |

## Proof-budget boundary

The circuit breaker closes the broad relationship-completeness class by replacing directional consumption with equality, not by accumulating guards. There is no new relation-specific production machinery and no additional source-adoption authority. H1-04 catalogue/source guarantees and H1-05 scene-publication guarantees remain consumed, not reopened.

Component/property fidelity, complex importer adaptation, broad Cantabrian art conversion, wardrobe, vehicles, missing production props/animations, CITY/H2 and shipping asset delivery remain outside H1-06. Future relationship kinds are not silently covered by this claim: adding a new observed relationship kind requires an explicit H1-06-universe change rather than pretending the current proof covers arbitrary component semantics.

## Required physical-local command

```powershell
scripts/h1-06-local-evidence.ps1 -AssetsRoot <owner-configured> -ExpectedSha <exact-40-char-head>
```

The command must run once from a clean checkout at the exact final repair SHA with physical local Unity `6000.3.24f1 (4e7b9b5b6244)`. Until that receipt is GREEN, PR #195 stays Draft/ACTIVE and must not be frozen or marked Ready for independent Reviewer.
