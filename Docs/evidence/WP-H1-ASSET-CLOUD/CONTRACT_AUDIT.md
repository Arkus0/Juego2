# WP-H1-ASSET-CLOUD — Worker contract audit

PREDECESSOR_CONTRACT_CHECK: PASS  
WORKER_PRE_REVIEW: CLEAN

## Candidate purpose

This workpack changes only the execution substrate used to supply the already accepted H1-04 Quaternius baseline to GitHub-hosted Unity. It does not adopt new art, change source/licence authority, alter H1 catalogue semantics, or reopen H1-04/H1-05.

## Predecessor contract check

- `WP-H1-UNITY-CI` already established GitHub-hosted Unity as an accepted optional effective H1 substrate for machine-verifiable claims.
- `WP-H1-04` remains the source/licence/catalogue authority for `quaternius-medieval-source` and `quaternius-ual1-source`.
- The private vault is execution storage only; its manifest cannot broaden H1-04 authority.
- The pinned private baseline is exact vault SHA `ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6`.
- Local prevalidation proved both accepted distribution hashes, four accepted SourceSlice content hashes, and the four frozen Unity GUID sidecars.

## Worker pre-review

The candidate is bounded to:

- exact-SHA checkout of the public candidate;
- exact-SHA checkout of the private asset vault;
- source/distribution/hash/licence/GUID binding back to H1-04;
- fail-closed negative controls before Unity execution;
- temporary mount only under the ignored H1 SourceSlice boundary;
- effective GitHub-hosted Unity proof of the frozen H1-04 identities;
- public-evidence allowlisting/leak rejection over the exact retained artifact surface;
- deterministic cleanup back to the exact public candidate, including Docker-created root-owned derivatives.

No private asset bytes are added to the public repository.

## Known setup boundary

`ARKUS_ASSET_VAULT_TOKEN` is a repository Actions setup secret that grants read-only access to the private asset repository. Absence of that credential is setup-blocked state, not semantic evidence against the product/workpack claim.

SETUP_BLOCKED must not be interpreted as a semantic product/workpack FAIL; acceptance still requires a later exact-SHA effective run once the credential exists.

## Repair evidence from run 36094543131

The first credentialed exact-SHA run on candidate `26e3369ff075897c7e8c32189726099f4e091ced` proved the substantive remote-source claim before failing in post-proof containment:

- exact private vault SHA checkout passed;
- accepted H1-04 distribution/file/GUID verification passed;
- all four source mutation/rebinding negative controls passed;
- effective Unity `6000.3.24f1` EditMode execution completed with `12/12` tests passed;
- `H1AssetCloudTests.PrivateVaultMount_RecreatesAcceptedH104UnityIdentities` passed.

The run then exposed two infrastructure-only defects:

1. the leak scanner inspected GameCI's raw `editmode.log` even though that log was not part of the retained upload allowlist;
2. GameCI/Docker generated imported material derivatives as `root`, so unprivileged cleanup could not remove the temporary SourceSlice.

The repair therefore:

- stages the exact retained public artifact surface in a dedicated `artifacts/h1-asset-cloud-public/` directory, scans only that surface, injects a private-byte negative control into that surface, and uploads only the staged receipt + Unity result XML;
- performs root-safe SourceSlice removal and restores package files before proving the public tracked tree is identical to the candidate SHA.

These repairs do not weaken or alter the Unity/source oracle established earlier in the run.

## Reviewer falsification targets

FAIL the candidate if any of the following holds after setup is available:

1. A changed/missing distribution can reach Unity.
2. A changed SourceSlice byte or rebound GUID can reach Unity.
3. The vault manifest can widen/rebind H1-04 source authority.
4. The effective Unity run does not recreate the accepted H1-04 native identities.
5. Private source bytes can enter retained public artifacts.
6. Cleanup can leave tracked public-tree drift.
7. Later H1 use depends on moving vault `main` rather than the frozen vault SHA.
8. Raw GameCI output can bypass the retained-evidence staging boundary.

WORKER_PRE_REVIEW_FINDINGS_FIXED: 4  
WORKER_PRE_REVIEW_EVIDENCE: pinned private baseline + local H1 vault GREEN + run 36094543131 Unity 12/12 + exact-SHA workflow + fail-closed negative controls + retained-artifact leak gate + root-safe cleanup
