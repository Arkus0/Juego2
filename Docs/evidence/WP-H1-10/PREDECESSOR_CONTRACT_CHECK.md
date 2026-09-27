# WP-H1-10 — PREDECESSOR_CONTRACT_CHECK

Status: WORKER ACTIVE (recorded by the receiving Worker at Transfer SHA `5068096ee5fa0273d3a4c1a63b30eab02b886fe8`, before any further implementation)

## Accepted direct predecessor

`WP-H1-09 — Drift reconciliation + Unity import proposals` is accepted.

- Frozen candidate / PRODUCT_SHA: `8f2103c037d08416d70687d4fb08b3d3c62bceac`
- Canonical implementation PR: `#220`
- Independent PASS review: `#5322464433`
- Implementation merge: `9574f80aa07effc3022a3bb100a516d107965a19`
- Binding DocSync: `Docs/evidence/WP-H1-09/DOCSYNC.md`; H1-10 baseline `fb2414745c2de7fd47d9eef39daecb2c0a49c428`

The H1 remaining-work compression amendment is binding (`Docs/workpacks/H1/H1_REMAINING_COMPRESSION_AMENDMENT.md`, PR `#209`, PASS `#5315077514`, merge `8cc921719861f5e3dc4affa6bc2c07b9c22fdb34`).

`main` advanced after the baseline only through documentation (PR #229/#232 product/H2/ART/CITY planning). No H0/H1 contract, workflow or source consumed here changed, so this check is unaffected.

## Authoritative escalations used

No accepted-contract capsule covers H1; the Worker read the exact `WP-H1-10`, the compression amendment, the H1-09 DocSync, the accepted H0 portability/provenance contracts (`WorldPortabilityContract`, `WorldPortability`, `WorldProvenanceContract`), the accepted H1-05/06 materializer and observation (`H1SceneProjection.cs`, `H1SceneProjection.Reconciliation.cs`) and the accepted H1-04 asset-vault/catalogue boundary. This was necessary because H1-10 composes concrete snapshot-import, lineage and observation semantics that status metadata does not carry.

## Inherited guarantees consumed

1. HK06B: canonical snapshot export/import/compare. Import requires the expected current revision/hash, starts a truthful `new-local-lineage` and never replays foreign mutation history; `compare` reports `sameAuthorableState`.
2. HK06C: deterministic journal replay and audit consistency of the accepted journal artifact.
3. HK09B: resource/interruption integrity of accepted persistence; process-local publication boundaries.
4. H1-03A: public host -> Editor dispatch, project lease, typed worker lifecycle and structured process failures.
5. H1-04: accepted Quaternius Source baseline, catalogue identity/fingerprint and the private asset-vault mount (`ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6`).
6. H1-05/06/07/08: managed-scene planning, deterministic input digest, generation staging + atomic publication, derivative-prefab lineage, component realization and pre/postflight validation (a failed postflight never publishes).
7. H1-09: complete managed-scope effective observation and drift oracle.

H1-10 adds no persisted-world format, replay/lineage model, recovery store, transaction path or second materializer.

## Guarantees newly owned by H1-10

Exactly the compressed scope: versioned project-checkpoint manifest referencing the accepted snapshot/journal; atomic current-checkpoint publication; bridge/profile/catalogue/source/package/editor fingerprint capture and verification; truthful fresh-process restore (accepted H0 import only) plus clean generated-output Unity rebuild orchestration through public composition; structured blocked results for missing/incompatible required inputs; and the normalized reconstruction-parity comparison between the checkpointed observation and the rebuilt observation.

## Transfer finding recorded before further implementation

The accepted H1-05/06 `realizationDigest` includes generation-local native locators of generated derivative prefabs (`realizedGuid`, `realizedLocalFileId`). A clean rebuild must regenerate those assets, so comparing the raw digest can never show parity (observed in run `36202186005`). This is not an H1-05/06 defect: those locators are bridge-local and the accepted digest is used within one generation. Normalizing them out of the H1-10 parity comparison is H1-10-owned; H1-05/06 are consumed, not reopened.

## Concrete reopen conditions

- HK06B/HK06C reopen only if effective fresh reconstruction shows the accepted import/compare/replay semantics false or inapplicable (for example import cannot restore the exact canonical hash/revision under `new-local-lineage`).
- H1-04 reopens only if the accepted source/catalogue identity cannot be reproduced as claimed from the pinned vault.
- H1-05/06/08 reopen only if an accepted materialization publishes a generation whose non-locator realization facts differ for identical plan/source inputs, or a failed postflight publishes.
- A checkpoint/restore/rebuild orchestration defect is H1-10-owned.

Theoretical possibility or a desire for duplicate proof is not a reopen condition.
