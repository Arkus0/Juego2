# WP-HK-06A negative-conformance matrix

These controls are bounded to HK06A's owned seam: truthful authored mutation provenance at the
accepted HK04 commit boundary and separation of test-owned runtime observation from authored state.
They do not re-prove HK04 transaction/route completeness, HK05 validation completeness or future
simulation/replay behavior.

| Required defect class | Controlled defect / mutant | Oracle expected RED | Reverted/effective GREEN evidence |
|---|---|---|---|
| missing journal entry after accepted mutation | remove the second entry from a two-transition effective journal | independent transition audit returns `entry-count-mismatch` | two accepted applies produce exactly two ordered entries |
| entry emitted for rejected/dry-run/read-only work | present a real entry to an audit whose independently observed persisted-transition set is empty | audit returns `entry-count-mismatch` | plan, dry-run, inspection, current/proposed validation, journal read, malformed/rejected/stale apply all leave count unchanged |
| idempotent response mistaken for a new commit | invoke exact accepted request twice | a second append would make journal count 2 and violate the persisted-transition count | second response is `replayed=true`; revision/hash and journal remain at one transition |
| wrong before revision/hash | replace first entry base hash with 64 zeroes | `base-anchor-mismatch:0` | effective base anchor equals independently captured initial state |
| wrong after revision/hash | replace first result revision with the prior revision | `result-anchor-mismatch:0` | effective result anchor equals independently captured committed state |
| wrong affected-resource set | replace the covered object+extension set with `world.object:invented` | `affected-resource-mismatch:0` | sorted effective set equals the independent representative expectation |
| provenance claims a transition that did not persist | evaluate the two-entry journal against zero independently observed persisted transitions | audit detects extra entries through `entry-count-mismatch` | rejected/non-persisting paths publish neither state nor entry |
| schema-valid normalized replay envelope drifts from the accepted request | change valid `typeId`, object identity/container/reference fields, extension owner/version/subject/dependency/payload, remove-operation identity, expected revision/hash or idempotency key in the journal-ready envelope while keeping the parsed mutation identity unchanged | production provenance binding guard rejects construction before publication; test-owned accepted-request oracle returns `request-envelope-mismatch` for representative valid `typeId` and payload corruption | effective entry request is deeply equal to an independently constructed accepted request; guard independently re-derives the v2 fingerprint from the machine-readable envelope and matches it to parsed identity/base anchor |
| corrupt/nondeterministic entry identity | replace the ID with a different 64-character lowercase-hex value; independently rerun the same accepted request in a clean session | test-owned oracle returns `entry-identity-mismatch`; production entry guard rejects the canonical-looking but semantically false ID | two clean sessions produce the same ID, and constructor independently verifies the length-framed identity against request fingerprint, anchors and resources |
| concurrent commit/journal divergence | race two requests from the same anchor | more/fewer than one entry or an entry whose request ID differs from the accepted writer fails focused assertions | inherited HK04 winner and HK06A entry agree exactly; losing stale writer produces no entry |
| runtime observation changes authored state/history | advance scheduled-NPC surrogate twice | any revision/hash/journal change yields boundary issue | transient step/position changes while all authored observables remain equal |
| runtime observation path acquires commit authority | unsafe test-owned surrogate invokes a real canonical apply on step | boundary oracle returns `authored-revision-changed`, `authored-hash-changed` and `authored-journal-changed` | production stamp/safe surrogate receive no mutation service or internal committer |

## Repair-cycle note

Independent review of candidate `046134fd3acd9641798dbad180406688fb5d31aa` identified the
schema-valid replay-envelope false-green class above. Repair cycle 1 closes the class at the HK06A
boundary rather than special-casing `typeId`: every replay-relevant field is covered by the same
fingerprint binding rule, all four operation kinds have causal semantic-corruption controls, and a
separate test-owned oracle compares the complete journal request with an independently constructed
accepted request.

## Result

Every HK06A-required defect class has a causal bounded oracle. The controls compare effective
canonical state and public journal data against independent expectations; the journal does not define
its own truth set. The added replay-envelope guard is fail-closed and does not implement replay,
semantic diff or duplicate HK04/HK05 proof.
