# WP-H1-ASSET-CLOUD — DocSync

DOCSYNC_COMPLETE

## Accepted implementation

- Workpack: `WP-H1-ASSET-CLOUD`
- Canonical implementation PR: `#201`
- Frozen candidate: `e4d18002e6ca81f302daa645e306e9373debac92`
- Independent PASS review: `#5313800159`
- Autopilot review ID: `1c70c88639e0484890b63bba998f3400`
- Implementation merge: `fe6ecd5e14ef2581fd8fbe8f87166d13d02109bd`
- Pinned private vault: `Arkus0/Juego2-assets@ae782c5f08cc4144a7ff0c3d4af67451d4d86bb6`

## Effective evidence

- `H1 Asset Cloud Validation` run `36098270707`: GREEN on the exact frozen candidate.
- `Arkus Main Safety` run `36098270716`: GREEN on the exact frozen candidate.
- `Arkus Candidate Validation` run `36098270700`: aggregate RED only because `Worker handoff lint` failed; validation-context binding and exact-SHA freeze verification were GREEN.
- The accepted run executed real Unity `6000.3.24f1`, recreated the accepted H1-04 identity probe, validated the pinned vault/source/hash/GUID boundary, deleted raw Unity/GameCI evidence before upload and retained only the two closed public receipts.

The Candidate Validation aggregate RED is recorded as historical process state, not rewritten as GREEN. Final acceptance is the exact-SHA PASS review above plus the implementation merge authorized by the owner.

## Accepted threat model

The fail-cycle-2 circuit breaker is accepted as the correct retained-evidence boundary:

- raw Unity/GameCI output is ephemeral and is not part of the retained uploader surface;
- the uploader is constrained to the two derived receipt files;
- those receipts contain only approved public identities, exact hashes/GUIDs, counters, SHAs and closed result enums;
- the boundary protects against accidental retention/propagation of private source material through normal Unity/GameCI execution and evidence collection;
- it does not attempt to defend against deliberate hostile modification of the reviewed pipeline itself to exfiltrate private assets.

This supersedes the earlier overdefensive requirement to prove absence of every possible byte representation inside arbitrary retained evidence. No further heuristic private-byte scanner hardening is required by this WP.

## Preserved authority and scope

- `WP-H1-04` remains source/licence/catalogue/Unity-identity authority.
- `WP-H1-05` and `WP-H1-06` remain closed and are not reopened by this infrastructure acceptance.
- The private vault is execution storage only and may not broaden the accepted H1-04 source universe.
- No H1 product semantics are changed by this DocSync.
- Physical-local Unity remains required only when a consuming claim materially depends on visual, interactive, GPU, peripheral or other physical-local evidence.

## H1 consequence

The accepted H1-04 Quaternius baseline now has a reviewed hosted-input path. Source-dependent H1 proofs may use GitHub-hosted Unity when the consuming claim is otherwise machine-verifiable and the frozen private-vault input is sufficient.

`WP-H1-07 — Allowlisted component projection` remains the next default H1 product workpack, dependency-valid from accepted `WP-H1-06` and not dependent on this process-infrastructure WP for semantic authority.

DOCSYNC_COMPLETE
Next H1 WP: `WP-H1-07`.
