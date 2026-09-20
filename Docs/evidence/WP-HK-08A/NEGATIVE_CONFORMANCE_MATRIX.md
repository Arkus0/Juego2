# WP-HK-08A negative-conformance matrix

The controls below target defect classes newly owned by HK08A. Accepted HK03/HK04/HK05/HK06/HK07 behavior is consumed unless the changed interaction shape could bypass it.

| Required defect class | Causal control / oracle | Expected RED condition | GREEN result |
|---|---|---|---|
| batch partially commits | `InvalidLaterOperationCannotPublishEarlierBatchEditsOrProvenance` places a valid edit before an operation that cannot apply | the first edit/revision/journal entry survives after the later failure | state hash/revision remain at baseline, objects remain empty and journal stays empty |
| representative coherent edit forced into two persisted halves by old batch cap | `RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry` requires 96 operations, explicitly >64, to equal the declared single-request maximum | returning `MaximumOperations` to 64 or silently splitting the request makes the test red | 96 mixed operations apply in one revision and one journal entry |
| batch bypasses candidate validation | `ParsedBatchThatViolatesWorldValidationCannotPublishOrAppendProvenance` uses two parseable operations whose resulting containment graph is invalid | mutation path persists a candidate without the accepted world validator | canonical apply fails; revision/hash/state/journal remain unchanged |
| successful batch omits provenance | `BatchIntegrityOracleTurnsRedIfSuccessfulBatchOmitsProvenance` observes a real successful two-operation batch, then defect-injects journal delta 0 into the independent effect oracle | persisted revision change is accepted without one provenance entry | real batch has revision delta 1 / journal delta 1; injected omission reports `missing-provenance` |
| compact response drops required interpretation data | `DiscoveryAndCompactProjectionExposeEconomicalPublicChoicesWithoutLosingRequiredFields` removes the required `world` anchor from a real compact object result | compactness can erase a required field and still satisfy the canonical result contract | compact result with optional fields omitted is valid; removing `world` yields schema issues |
| pagination duplicates or misses entries | actual 55-entry 20/20/15 reconstruction plus `JournalReconstructionOracleTurnsRedForDuplicateAndMissingSequence` | duplicate entry ID or sequence gap is accepted as complete | exact sequence 1..55, 55 unique entry IDs; duplicate and missing injections turn red |
| stale pagination continues after authored anchor changes | `JournalPagesReconstructOneAnchoredSequenceWithoutDuplicateOrOmissionAndFailClosedWhenStale` captures cursor/anchor, commits another change, then retries | old cursor/anchor continues against a different current state | old cursor -> `world.provenance.stale_cursor`; old explicit revision/hash -> `world.provenance.stale_anchor` |
| altered continuation offset skips entries while anchor still matches | `AlteredCursorOffsetFailsClosedInsteadOfSkippingJournalEntries` changes only the encoded inner offset and keeps the original integrity checksum | altered opaque cursor is accepted and continuation jumps over sequence 2 | altered cursor -> `world.provenance.invalid_cursor`; unchanged cursor resumes at pageOffset 1 / sequence 2 |
| partial page is mistaken for complete HK06A journal | complete-vs-page assertions in `JournalPagesReconstructOneAnchoredSequenceWithoutDuplicateOrOmissionAndFailClosedWhenStale` | partial page can carry complete journal schema or complete read changes HK06A shape | partial reads use `arkus.authoring.journal-page@1`; complete bounded read restores exact `arkus.authoring.journal@1` shape |
| ordinary mutation requires one request per property | actual `OneCoherentMutationRequestUpdatesSeveralObjectPropertiesTogether` + `OrdinaryFlowShapeOracleRejectsOneMutationRequestPerProperty` | type/container/reference change requires separate public mutation requests | one operation/request changes all three fields and produces one update/journal entry; one-field-per-request injection turns red |
| reference JSONL and MCP drift for HK08A primitives | external `Hk08AChangedInteractionPrimitivesRemainEquivalentAcrossReferenceJsonlAndMcp` and discovery test | batch/page/compact/discovery neutral outcome differs by transport | real external flows deep-equal after requestId removal; injected result difference reports `semantic-drift` |

## Effective controls and scope

- The representative capacity control would fail if the old 64-entry constraint reappeared; no runtime split helper exists to make the test green by committing halves.
- The validation control reaches the candidate-level world validator rather than relying on an operation parse/apply error.
- Provenance is checked by observing the journal after mutation, not by reading mutation metadata that merely claims provenance is enabled.
- Pagination completeness is checked from persisted sequence/entry identity, not just `pageOffset` arithmetic.
- Cursor integrity is ordinary protocol correctness. The deterministic checksum is not represented as authentication or a security boundary.
- JSONL/MCP tests launch the actual Release executables; they do not compare two in-memory adapters sharing a test fake.
- No negative control requires stale-CAS recovery, final budgets, network behavior or engine integration because those are outside HK08A.

Implementation/test SHA `7102330e9a3030d8f4f333d2715a8633312eeb7b`: focused HK08A 13/13 GREEN, full regression 173/173 GREEN, Actions `35498188203`, artifact `10600817518`.
