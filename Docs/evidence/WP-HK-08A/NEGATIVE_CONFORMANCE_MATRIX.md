# WP-HK-08A negative-conformance matrix

The controls below target defect classes newly owned by HK08A. Accepted HK03/HK04/HK05/HK06/HK07 behavior is consumed unless the changed interaction shape could bypass it.

| Required defect class | Causal control / oracle | Expected RED condition | GREEN result |
|---|---|---|---|
| batch partially commits | `InvalidLaterOperationCannotPublishEarlierBatchEditsOrProvenance` places a valid edit before an operation that cannot apply | the first edit/revision/journal entry survives after the later failure | state hash/revision remain at baseline, objects remain empty and journal stays empty |
| representative coherent edit forced into two persisted halves by old batch cap | `RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry` requires 96 operations, explicitly >64, to equal the declared single-request maximum | restoring the old 64 limit or silently splitting the request makes the test red | 96 mixed operations apply in one revision and one journal entry |
| batch bypasses candidate validation | `ParsedBatchThatViolatesWorldValidationCannotPublishOrAppendProvenance` uses two parseable operations whose resulting containment graph is invalid | mutation path persists a candidate without the accepted world validator | canonical apply fails; revision/hash/state/journal remain unchanged |
| successful batch omits provenance | `BatchIntegrityOracleTurnsRedIfSuccessfulBatchOmitsProvenance` observes a real successful two-operation batch, then injects journal delta 0 into the independent effect oracle | persisted revision change is accepted without one provenance entry | real batch has revision delta 1 / journal delta 1; injected omission reports `missing-provenance` |
| public journal semantics change under the accepted v1 identity | `JournalV1RemainsCompleteWhileV2PagesAndReconstructsOneAnchoredSequence` creates 55 entries, above the v2 default page size | `authoring.journal.read@1.0` with `{}` returns only 50 entries / a cursor, or accepts the v2 request contract under v1 | v1 `{}` returns the exact complete `arkus.authoring.journal@1` artifact with all 55 entries; paging exists only at explicit v2 |
| pagination duplicates or misses entries | the same 55-entry v2 test reconstructs 20/20/15 plus `JournalReconstructionOracleTurnsRedForDuplicateAndMissingSequence` | duplicate entry ID or sequence gap is accepted as complete | exact sequence 1..55, 55 unique entry IDs; duplicate and missing injections turn red |
| stale pagination continues after authored anchor changes | `JournalV1RemainsCompleteWhileV2PagesAndReconstructsOneAnchoredSequence` captures a v2 cursor/anchor, commits another change, then retries | old cursor/anchor continues against a different current state | old cursor -> `world.provenance.stale_cursor`; old explicit revision/hash -> `world.provenance.stale_anchor` |
| altered continuation offset skips entries while anchor still matches | `AlteredCursorOffsetFailsClosedInsteadOfSkippingJournalEntries` changes only the encoded inner offset and keeps the original integrity checksum | altered opaque cursor is accepted and continuation jumps over sequence 2 | altered v2 cursor -> `world.provenance.invalid_cursor`; unchanged cursor resumes at pageOffset 1 / sequence 2 |
| partial page is mistaken for complete HK06A journal | complete-vs-page assertions in `JournalV1RemainsCompleteWhileV2PagesAndReconstructsOneAnchoredSequence` | a v2 partial page carries the complete-journal schema, or a complete v2 page cannot recover the accepted complete shape | partial reads use `arkus.authoring.journal-page@1`; a complete bounded v2 read restores exact `arkus.authoring.journal@1` shape |
| compact response drops required interpretation data | `DiscoveryAndCompactProjectionExposeEconomicalPublicChoicesWithoutLosingRequiredFields` removes the required `world` anchor from a real compact object result | compactness can erase a required field and still satisfy the canonical result contract | optional fields may be omitted; removing `world` yields schema issues |
| ordinary mutation requires one request per property | `OneCoherentMutationRequestUpdatesSeveralObjectPropertiesTogether` + `OrdinaryFlowShapeOracleRejectsOneMutationRequestPerProperty` | type/container/reference change requires separate public mutation requests | one operation/request changes all three fields and produces one update/journal entry; one-field-per-request injection turns red |
| reference JSONL and MCP drift for HK08A primitives or version negotiation | external `Hk08AChangedInteractionPrimitivesRemainEquivalentAcrossReferenceJsonlAndMcp` plus discovery test | either transport omits v1/v2, maps a version to different semantics, or changes batch/page/compact/discovery outcome | v1 complete read and v2 paged read are both exercised through real JSONL/MCP processes; normalized outcomes deep-equal |

## Effective controls and scope

- The versioning control reproduces the exact Reviewer blocker rather than merely checking a version constant: the journal contains 55 entries, so the broken v1 default-50 implementation necessarily turns red.
- HK01 route-universe tests no longer encode a magic total. They compare independently enumerated routes to canonical definitions and explicitly require both `authoring.journal.read@1.0` and `@2.0`.
- The representative capacity control fails if the old 64-entry constraint reappears; no runtime split helper can hide it by committing halves.
- The validation control reaches the candidate-level world validator rather than relying on an operation parse/apply error.
- Provenance is checked by observing the journal after mutation, not by reading mutation metadata that merely claims provenance is enabled.
- Pagination completeness is checked from persisted sequence/entry identity, not just `pageOffset` arithmetic.
- Cursor integrity is protocol correctness, not authentication or an authorization claim.
- JSONL/MCP tests launch the actual Release executables; they do not compare two in-memory adapters sharing a test fake.
- No negative control requires stale-CAS recovery, final budgets, network behavior or engine integration because those are outside HK08A.

Implementation/test SHA `300c4b65bcbe1b547837dffc76e241c9500a9e0c`: focused HK08A 13/13 GREEN, full regression 173/173 GREEN, Actions `35499408516`, artifact `10601643061`.
