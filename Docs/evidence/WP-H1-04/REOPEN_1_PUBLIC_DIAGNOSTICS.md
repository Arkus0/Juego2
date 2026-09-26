# WP-H1-04 — Reopen 1: public catalogue discoverability diagnostics

Status: **CORRECTION / OWNER-WAIVED INDEPENDENT REVIEW**

## Trigger

This reopen comes from the WP-H1-GATE fresh independent public-client AI-agent trial 1. The trial was PR `#238`, candidate `c2dbd318843888af86543e3515a78af07a7ef4a7`, run `36238111911`, and it FAILED. The deterministic Gate passed on the same SHA (run `36238102363`). The trial failed because a client that has only the public MCP discovery and schemas could not page the catalogue or recover from its errors. `Docs/evidence/WP-H1-GATE/AI_TRIAL_HISTORY.md` on the Gate branch records the classification R1–R4. It routes R1–R3 to this WP, and routes R4 (encoder rejection mapping) to the reopen owner for triage.

Owner decision (2026-09-26):

- Reopen WP-H1-04 before retrying the trial.
- This correction does **not** need an independent Reviewer.
- The WP-H1-GATE candidate itself still requires its own fresh independent Reviewer.

## Accepted guarantee proved inapplicable

The accepted acceptance criterion reads "public bounded discovery describes each admitted unit…". It holds for a client with implementation knowledge. It is inapplicable to a fresh public client, for these reasons:

| # | Effective evidence | Correction |
| --- | --- | --- |
| R1 | `unity.host.catalogue.query` publishes `pageSize` as a bare integer, but the model enforces `1..64`. `SchemaNode` (H0 protocol) cannot express numeric bounds, and changing it is outside H1-04. | The `catalogue.page-bound` error now carries `context.minimumPageSize=1` and `context.maximumPageSize=64`, plus a code-specific hint ("Use a pageSize from 1 to 64 and page with the returned nextOffset."). An out-of-range `offset` carries `minimumOffset`, `maximumOffset` and `total`. |
| R2 | `catalogue.stale-snapshot` returned an empty context. Every catalogue error shared the hint "Repair the reviewed mapping, source input or catalogue reference before materialization.", which sends a guessed-token client towards a mapping repair. | Stale-snapshot now returns `context.currentSnapshotToken`, `fingerprint` and `restartOffset=0`, and the hint explains the token protocol ("Omit expectedSnapshotToken on the first page. On later pages, pass the snapshotToken returned by the previous page. If the snapshot changed, restart at offset 0."). Each of `unknown-kind`, `invalid-reference`, `missing-reference` and `incompatible-reference` now has its own context and hint. The generic hint remains only for mapping/adoption faults, which are what it describes. |
| R3 | The fixed managed scene `arkus.h1-05.scene.potes` is not a catalogued source. A client that tries `catalogue.get/resolve` on it receives `catalogue.missing-reference` and nothing else. | The machine code is unchanged (`catalogue.missing-reference`), so projection semantics are unchanged. The error now names the ID's public role: `context.managedProjectionTarget=true` and `targetFor=["binding.targetSceneId","sceneLogicalId"]`, with a hint that says to use it directly and that it does not need to resolve. |
| R4 (triaged here) | A `materialize` over an invalid canonical binding was rejected before launch by the typed encoder, but it was reported as `unity.lifecycle.corrupt-result` with "Repair the host/worker contract mismatch before retrying." (Gate content-shape finding G2). | `H1UnityEditorExecutionCoordinator.Invoke` now reports an `H1ProjectionException` raised by `EncodeRequest` under its own structured code (for example `projection.source-missing`, `projection.reference-missing` or `projection.scene-out-of-scope`), with retryable false and a hint to run `unity.projection.plan` and repair through `authoring.change.*`. As before, no Editor process is launched and no lease is consumed beyond the pre-launch path. Other encoder exceptions keep `unity.lifecycle.corrupt-result`. |

## Not changed

- No capability, schema, machine-code set, catalogue content, mapping, fingerprint (`e1e92d98…`), snapshot token derivation or page bound changes.
- Nothing on the Unity/Editor side changes, and no Quaternius Source adoption or asset changes.
- H0 protocol (`SchemaNode`) is not changed.
- The composed topology does not change. The only effect is on H1-05/H1-09 materialization: an encoder-level projection precondition is now reported under its own code instead of the generic lifecycle fault.

## Proof

- `H1CatalogueTests.Paging_and_reference_diagnostics_carry_the_facts_a_fresh_public_client_needs_to_recover` covers four things:
  1. page-bound context and hint;
  2. stale-snapshot `currentSnapshotToken`, and that a query with the recovered token succeeds;
  3. missing-reference, and the managed-target role;
  4. every new error serialized as a `StructuredError` is `PortableData`-valid and valid against `CanonicalContractSchemas.StructuredError()`.
- `H1ProjectionReconciliationTransportTests` (H1-09, through the composed reference transport): an out-of-scope scene reconciliation now returns `projection.scene-out-of-scope` with the `unity.projection.plan` hint instead of `unity.lifecycle.corrupt-result`.
- The full `Arkus.Harness.Tests` suite passes locally at 450/450. Exact-SHA hosted evidence (Main Safety plus the path-triggered H1 Unity workflows) is recorded on the PR.
- The effective public proof is WP-H1-GATE: its deterministic 17-stage scenario on reference and MCP, and a new fresh-agent trial on the Gate's final frozen SHA.

## Exact-SHA verifier maintenance (pre-existing breakage on `main`)

`scripts/h1-04-verify-exact-sha.sh` was already RED on `main` `78a6a867` before this reopen, independent of the diagnostics change. It hard-coded the accepted 250-row H1-04 extent and bound the current Unity package files. Later accepted work changed both inputs legitimately:

- WP-H1-11 appended 24 rows (12 asset and 12 prefab) and 12 adoption slices to the committed inventory and adoption record as a declared extension of the same distribution. The extension is recorded in `SOURCE_ADOPTION.md` and `Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json`.
- CITY-04 added `com.unity.modules.physics` to the Unity manifest and lock.

The verifier now handles each change exactly:

- `h1-04-evidence-summary.py` removes the H1-11 extension by the declared `distribution-entry` paths in that record, and it requires the declaration, rows and slices to agree. The remaining 250-row baseline must reproduce the reviewed `baselineInventorySha256` byte-for-byte.
- With `--package-revision`, it binds the package digests at the accepted H1-04 merge `4f172f7a`. Only the exact-SHA route passes that option. A fresh local round (`h1-04-local-evidence.ps1`) still binds the current tree.
- The resulting summary must still equal the committed `EFFECTIVE_VALIDATION.json` exactly. No committed evidence value was changed.
