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
- Local and remote validation already proved both accepted distribution hashes, four accepted SourceSlice content hashes, and the four frozen Unity GUID sidecars.

## Worker pre-review

The candidate remains bounded to:

- exact-SHA checkout of the public candidate;
- exact-SHA checkout of the private asset vault;
- source/distribution/hash/licence/GUID binding back to H1-04;
- fail-closed private-vault negative controls before Unity execution;
- temporary mount only under the ignored H1 SourceSlice boundary;
- effective GitHub-hosted Unity proof of the frozen H1-04 identities;
- derivation of closed public receipts from ephemeral raw Unity output;
- exact two-file retained evidence allowlisting;
- deterministic cleanup back to the exact public candidate, including Docker-created root-owned derivatives.

No private asset bytes are added to the public repository.

## Known setup boundary

`ARKUS_ASSET_VAULT_TOKEN` is a repository Actions setup secret that grants read-only access to the private asset repository. Absence of that credential is setup-blocked state, not semantic evidence against the product/workpack claim.

SETUP_BLOCKED must not be interpreted as a semantic product/workpack FAIL; acceptance still requires a later exact-SHA effective run once the credential exists.

## Previously accepted effective path

Credentialed exact-SHA runs already established the substantive remote-source path:

- exact private vault SHA checkout passed;
- accepted H1-04 distribution/file/GUID verification passed;
- all four source mutation/rebinding negative controls passed;
- effective Unity `6000.3.24f1` EditMode execution completed `12/12`;
- `H1AssetCloudTests.PrivateVaultMount_RecreatesAcceptedH104UnityIdentities` passed;
- deterministic GameCI package drift was reconciled;
- root-owned temporary import derivatives were removed safely;
- the public tracked tree returned to the exact candidate.

Those guarantees are consumed unchanged by this repair.

## fail_cycle 2 circuit breaker — retained-evidence / private-content oracle

Independent review `#5313597522` of candidate `ce31619b822ccd4d8996a797c71c81a4eca1c603` identified a foundational weakness in the retained-evidence privacy proof. The byte scanner fingerprinted aligned 96-byte private blocks and therefore only guaranteed detection of an arbitrary contiguous leak once it was large enough to contain a fingerprinted block. Extending that scanner with more offsets, encodings, thresholds, or heuristics would keep the proof dependent on predicting representations of private content.

The circuit breaker therefore changes the proof model rather than extending the heuristic oracle.

### New retained-evidence model

Raw Unity/GameCI products are ephemeral execution inputs only. In particular, `artifacts/h1-asset-cloud/unity/editmode-results.xml` is parsed on-runner and is never part of the retained upload surface.

After effective Unity execution, `Tools/AssetVault/derive_h1_unity_receipt.py`:

1. parses the exact ephemeral NUnit XML;
2. requires the root suite to be `Passed` with exactly `12` concrete tests, `12` passed, `0` failed, `0` skipped, and `0` inconclusive;
3. requires exactly one exact H1 asset-cloud identity probe and requires it to be `Passed`;
4. requires the exact effective Unity-version probe to be `Passed`;
5. requires `EditMode` platform evidence;
6. writes a new JSON object from scratch using only closed enums, fixed counters, exact candidate/vault SHAs, and the pinned Unity version.

The Unity receipt does not copy test messages, stack traces, stdout/stderr, XML properties, arbitrary test names, timestamps, paths, or asset content. The required probe is normalized to the closed enum `H1_04_NATIVE_IDENTITY` only after the exact full probe name has matched and passed.

The retained public surface is exactly:

- `H1_ASSET_CLOUD_RECEIPT.json`;
- `H1_ASSET_CLOUD_UNITY_RECEIPT.json`.

`Tools/AssetVault/scan_h1_public_evidence.py` is no longer a private-byte scanner. It is a structural retained-surface validator. It proves:

- exactly those two files exist and no third file exists;
- both files are bounded-size UTF-8 JSON objects;
- every top-level and nested key is exact;
- all fixed strings are closed enums or explicit allowlisted public identities;
- candidate and vault SHAs are exact lowercase 40-hex and equal the effective inputs;
- distribution SHA-256 values and SourceSlice hash/GUID identities equal the already accepted H1-04 values;
- Unity counters and result enums are exact;
- no free-text/payload field exists;
- the workflow uploader has exactly the two allowed receipt paths and cannot retain the raw Unity XML.

This removes the previous minimum-leak threshold entirely. Privacy now follows primarily from eliminating every arbitrary-text upload channel rather than recognizing possible encodings of private bytes.

### Causal negative controls for the closed surface

The workflow falsifies the new model directly:

- third retained file -> RED;
- unexpected receipt key -> RED;
- free-text `payload` field -> RED;
- changed candidate SHA -> RED;
- changed vault SHA -> RED;
- changed normalized probe enum -> RED;
- uploader path changed from the Unity receipt to raw `editmode-results.xml` -> RED;
- exact probe renamed in a copy of the ephemeral XML -> Unity receipt cannot be minted;
- exact probe marked failed in a copy of the ephemeral XML -> Unity receipt cannot be minted.

After the receipt gate succeeds, the raw Unity/GameCI evidence directory is explicitly deleted before final upload. The final uploader remains conditioned on overall success plus the explicit retained-evidence gate outcome and uses `if-no-files-found: error`.

## Reviewer falsification targets

FAIL the candidate if any of the following holds after setup is available:

1. A changed/missing distribution can reach Unity.
2. A changed SourceSlice byte or rebound GUID can reach Unity.
3. The vault manifest can widen/rebind H1-04 source authority.
4. The effective Unity run does not recreate the accepted H1-04 native identities.
5. Raw Unity/GameCI output, free text, or any third file can reach the retained artifact surface.
6. A Unity receipt can be minted without exact passing probe evidence and exact passing suite summary.
7. Candidate/vault identity can be rebound in either receipt.
8. The uploader can retain anything other than the two closed receipts.
9. Cleanup can leave tracked public-tree drift.
10. Later H1 use depends on moving vault `main` rather than the frozen vault SHA.

WORKER_PRE_REVIEW_FINDINGS_FIXED: 6  
WORKER_PRE_REVIEW_EVIDENCE: pinned private baseline + accepted vault/hash/GUID/Unity path + circuit-breaker closed receipts + exact two-file uploader allowlist + structural negative controls + raw Unity evidence deletion + root-safe cleanup
