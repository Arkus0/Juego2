# WP-HK-06A negative-conformance matrix

These controls are bounded to HK06A's owned seam: truthful authored mutation provenance at the
accepted HK04 commit boundary and separation of test-owned runtime observation from authored state.
They do not re-prove HK04 transaction/route completeness, HK05 validation completeness or future
simulation behavior.

| Required defect class | Controlled defect / mutant | Oracle expected RED | Reverted/effective GREEN evidence |
|---|---|---|---|
| missing journal entry after accepted mutation | remove the second entry from a two-transition effective journal | independent transition audit returns `entry-count-mismatch` | two accepted applies produce exactly two ordered entries |
| entry emitted for rejected/dry-run/read-only work | present a real entry to an audit whose independently observed persisted-transition set is empty | audit returns `entry-count-mismatch` | plan, dry-run, inspection, current/proposed validation, journal read, malformed/rejected/stale apply all leave count unchanged |
| idempotent response mistaken for a new commit | invoke exact accepted request twice | a second append would make journal count 2 and violate the persisted-transition count | second response is `replayed=true`; revision/hash and journal remain at one transition |
| wrong before revision/hash | replace first entry base hash with 64 zeroes | `base-anchor-mismatch:0` | effective base anchor equals independently captured initial state |
| wrong after revision/hash | replace first result revision with the prior revision | `result-anchor-mismatch:0` | effective result anchor equals independently captured committed state |
| wrong affected-resource set | replace the covered object+extension set with `world.object:invented` | `affected-resource-mismatch:0` | sorted effective set equals the independent representative expectation |
| provenance claims a transition that did not persist | evaluate the two-entry journal against zero independently observed persisted transitions | audit detects extra entries through `entry-count-mismatch` | rejected/non-persisting paths publish neither state nor entry |
| corrupt/nondeterministic entry identity | replace deterministic SHA-256 ID with a noncanonical string; independently rerun the same sequence in a clean session | audit returns `entry-id-invalid:0`; cross-session comparison would diverge for nondeterminism | both clean sessions produce identical ordered IDs |
| concurrent commit/journal divergence | race two requests from the same anchor | more/fewer than one entry or an entry whose request ID differs from the accepted writer fails focused assertions | inherited HK04 winner and HK06A entry agree exactly; losing stale writer produces no entry |
| runtime observation changes authored state/history | advance scheduled-NPC surrogate twice | any revision/hash/journal change yields boundary issue | transient step/position changes while all authored observables remain equal |
| runtime observation path acquires commit authority | unsafe test-owned surrogate invokes a real canonical apply on step | boundary oracle returns `authored-revision-changed`, `authored-hash-changed` and `authored-journal-changed` | production stamp/safe surrogate receive no mutation service or internal committer |

## Result

Every HK06A-required defect class has one causal bounded oracle. The controls compare effective
canonical state and public journal data; none treats the journal's own contents as the sole universe
of mutations that should exist. No syntax-path enumeration or arbitrary toolchain behavior is added.
