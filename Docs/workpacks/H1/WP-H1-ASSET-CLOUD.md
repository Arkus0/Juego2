# WP-H1-ASSET-CLOUD — Private asset vault parity for remote H1 Unity

Status: IMPLEMENTING
Class: PROCESS_INFRASTRUCTURE / NON-PRODUCT
EXECUTION_REQUIREMENT: GITHUB_HOSTED_UNITY
Depends on: `WP-H1-UNITY-CI` PASS and accepted `WP-H1-04` source baseline
Blocks: no product semantics; enables remote execution of H1-06 through H1-GATE when claims are machine-verifiable

## Objective

Make the already accepted H1-04 Quaternius source baseline available to GitHub-hosted Unity without committing or redistributing third-party source bytes in the public `Arkus0/Juego2` repository.

This workpack changes execution substrate only. It does not adopt new art, change licences, alter catalogue semantics, or weaken any H1 workpack oracle.

## Central claim

A private repository `Arkus0/Juego2-assets` can provide the exact H1-04-approved source distributions and Unity-facing SourceSlice to an exact-SHA GitHub-hosted Unity run such that:

- the private source distributions match the hashes already accepted by H1-04;
- the mounted SourceSlice files match the accepted per-file hashes;
- the original Unity `.meta` sidecars preserve the native GUID/import identity already frozen by H1-04;
- missing, changed, rebound, or incomplete source fails closed before Unity execution;
- source bytes remain private and are never retained as public workflow artifacts;
- the public repository tree remains identical to the candidate SHA after temporary source mounting and cleanup.

## Authority boundary

`Docs/evidence/WP-H1-04/SOURCE_ADOPTION.json` and the accepted H1-04 catalogue mapping remain the source/licence/Unity-identity authorities.

The private vault manifest is an execution index only. It may not broaden the accepted source universe, change hashes/licences, or mint new catalogue identity.

## One-time H1 baseline

The owner uploads the accepted H1 source baseline once, not once per workpack.

The private vault retains:

- the complete `quaternius-medieval-source` distribution already adopted by H1-04;
- the complete `quaternius-ual1-source` distribution already adopted by H1-04;
- the exact four-file H1-04 compatibility SourceSlice plus original `.meta` sidecars.

Later H1 workpacks may select a broader representative subset from those already-adopted distributions where their own oracle requires it, especially H1-11 and H1-GATE. New owner upload is required only if a later workpack explicitly adopts a genuinely new source outside the accepted H1-04 baseline.

## Private vault contract

Repository: `Arkus0/Juego2-assets` (PRIVATE)

Canonical manifest: `h1/H1_SOURCE_SLICE_MANIFEST.json`

The compatibility slice must contain:

- `Wall_Plaster_Window_Wide_Flat.fbx` + original `.meta`;
- `MI_Plaster.mat` + original `.meta`;
- `FacadeImportedMaterial.mat` + original `.meta`;
- `UAL1.fbx` + original `.meta`.

The manifest records only source IDs, accepted hashes, expected Unity GUIDs, licence IDs and private vault paths. Secrets do not belong in either repository.

## Remote credential boundary

The public workflow must receive private-vault read access through a GitHub Actions secret or other reviewed read-only credential. The credential:

- must not be committed;
- must not appear in retained logs/artifacts;
- should have read-only access limited to `Arkus0/Juego2-assets` when practical;
- is setup state, not H1 product authority.

A missing/invalid credential is `SETUP_BLOCKED`, not product FAIL.

## Acceptance criteria

1. Private vault repository is private and readable only through explicit authorization.
2. Verifier binds vault source IDs, distribution hashes and licence IDs to accepted H1-04 source adoption.
3. Both complete H1 distributions are present with exact accepted SHA-256 values.
4. The exact H1-04 four-file SourceSlice is present with exact file hashes.
5. Each SourceSlice file has its original `.meta`, and the verifier proves the expected GUID against accepted H1-04 catalogue identity.
6. Any missing file, mismatched hash, mismatched GUID, extra/rebound source ID or manifest widening fails before Unity starts.
7. The SourceSlice mounts only into the ignored `Unity/ArkusUnity/Assets/Arkus/H1/SourceSlice/` boundary.
8. A GitHub-hosted Unity execution can consume the mounted baseline at an exact public candidate SHA.
9. No private source file is uploaded as a public Actions artifact; retained evidence contains hashes/IDs/results only.
10. After cleanup/reconciliation, tracked public repository state is identical to the candidate SHA.

## Causal negative controls

- mutate one private source byte;
- substitute one `.meta` with a new GUID;
- omit one distribution payload;
- change a manifest source ID/hash/licence away from H1-04;
- attempt to mount over a non-empty unexpected SourceSlice;
- attempt to retain private source bytes as public evidence;
- allow tracked public-tree drift after the run.

Each must fail closed without weakening H1-04 or the consuming workpack.

## PASS consequence

PASS establishes a reusable remote H1 asset substrate. H1-06 through H1-GATE may consume it under the already accepted GitHub-hosted Unity policy whenever their own evidence is machine-verifiable. Local/visual execution remains required whenever a consuming claim materially depends on visual, interactive, GPU, peripheral, or other physical-local evidence.
