# WP-HK-08A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-08A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK08A makes the accepted H0 authoring surface materially less chatty without creating a second semantic system. One representative mixed-resource authoring intent now fits in one accepted HK04 transaction, compact reads reuse the accepted HK03 projection semantics, the HK06A journal is exposed through bounded deterministic pages while complete small reads retain the exact HK06A replay artifact, and existing canonical cost/side-effect metadata is sufficient for cheap-path discovery. Every changed public interaction shape is exercised through both the accepted JSONL process and MCP projection.

The claim is deliberately narrower than HK08B/HK09B: it does not claim stale-CAS recovery, repair prioritization, final interaction budgets, resource quotas, durable storage, hostile-client authorization, or multi-plan atomicity.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| representative coherent edit fits one atomic request | canonical `authoring.change.apply` request shape | the Potes/Liébana content probe uses 96 mixed operations, deliberately above the former provisional 64 ceiling; the same canonical mutation authority still performs one candidate validation and one commit | `RepresentativeNinetySixOperationIntentRemainsOneAtomicTransactionAndOneJournalEntry`: dry-run is non-persisting; apply advances one revision and writes 72 objects + 24 extensions in one request | the test requires `operations.Count > 64` and `operations.Count == MaximumOperations`; restoring the old 64 limit turns the representative probe red rather than silently splitting it | PASS | 96 is an H0 interaction envelope, not the final resource budget; HK09B owns measured limits |
| batching cannot weaken HK04 atomicity, HK05 validation or HK06A provenance | mutation candidate/commit seam | HK08A changes the capacity constant, not mutation authority; validation/provenance remain on the accepted transaction path | invalid later operation leaves revision/hash/journal unchanged; candidate-level invalid containment also leaves state/journal unchanged; successful 96-op batch produces exactly one journal entry containing all 96 normalized operations | `ParsedBatchThatViolatesWorldValidationCannotPublishOrAppendProvenance`; `BatchIntegrityOracleTurnsRedIfSuccessfulBatchOmitsProvenance`; partial-commit test | PASS | predecessor correctness remains inherited; HK08A proves the changed batch shape still traverses those seams |
| journal read is bounded/paginated without changing journal meaning | public `authoring.journal.read` over accepted complete HK06A authority | paging is a read-only view over the complete accepted artifact; entries retain persisted sequence order; a page that covers the whole journal is converted back to the exact HK06A artifact shape | 55 entries reconstruct as 20/20/15 with sequences 1..55 and unique entry IDs; a max-sized complete read validates against `JournalResultSchema`; inherited replay regression stays green | duplicate and sequence-gap injections turn the reconstruction oracle red; complete-vs-page assertions fail if a partial page masquerades as the complete artifact | PASS | public replay of histories larger than one page requires deterministic client reassembly; no new replay meaning is introduced |
| cursor continuation is bound to one valid authored journal context | page cursor / authored anchor | cursor contains current/base anchors, entry count, limit and offset; public envelope checks deterministic integrity before the inner cursor is accepted; current journal must still match all bound fields | valid continuation resumes exactly at pageOffset 1 / sequence 2; stale current state rejects old cursor and explicit stale anchor | `AlteredCursorOffsetFailsClosedInsteadOfSkippingJournalEntries` changes only the encoded offset while retaining the old integrity checksum and must return `world.provenance.invalid_cursor`; stale state returns `stale_cursor`/`stale_anchor` | PASS | cursor envelope is deterministic integrity framing, not authentication; clients are required to treat cursors as opaque |
| compact reads retain contractually required interpretation data | inherited HK03 bounded read surface consumed by HK08A | HK08A reuses field selection rather than inventing another compact registry; the canonical success schema remains the authority for required fields | object read with `fields=[]` returns object identity + world anchor while omitting optional type/container fields | deleting required `world` from the observed compact result makes the canonical success-schema oracle red | PASS | larger/query pagination semantics remain inherited HK03 behavior |
| discovery exposes economical choices without transport-specific policy | canonical `CapabilityDefinition` inventory | side-effect class and canonical relative cost are already machine-readable; no new per-transport tier table is needed | `system.describe` shows summary cheaper than object query/apply, readonly versus canonical mutation, and batch maximum 96 | cross-transport discovery comparison would fail on any metadata drift | PASS | final measured agent-call budgets belong to HK08B |
| ordinary modify flow is not one request per property | canonical mutation grammar | one `put-object` operation already represents a coherent object replacement; HK08A proves that several changed fields travel together rather than adding field setters | `OneCoherentMutationRequestUpdatesSeveralObjectPropertiesTogether` changes type, container and references in one canonical mutation request and one journal entry | `OrdinaryFlowShapeOracleRejectsOneMutationRequestPerProperty` turns red for four mutation requests used for four conceptual properties | PASS | more ergonomic high-level planning remains future/client work, not a new H0 semantic layer |
| every HK08A-changed public primitive remains JSONL/MCP equivalent | two independent external process projections | both transports consume the same canonical definitions/neutral dispatcher; the conformance test launches real Release JSONL and MCP processes and compares normalized neutral outcomes | external flow: 96-op batch, coherent modify, journal page + continuation, compact object read, and `system.describe` metadata | `CrossTransportOracleTurnsRedWhenAnHk08AResultDrifts` mutates one observed page result and reports semantic drift | PASS | vendor-specific clients remain outside the accepted transport pair |
| HK08A does not pull HK08B/09B scope forward | changed-file and semantic boundary | no recovery planner, conflict merge, resource quota, timeout budget or multi-plan transaction is added | implementation is limited to batch envelope, provenance paging/integrity view, runtime binding, exact-SHA routing and conformance tests | any new recovery/merge/resource-budget semantics would be visible as a new public contract/handler outside this proof matrix | PASS | the next workpacks intentionally consume these primitives |

## Independent/effective universes

1. **Product-shape universe:** `Docs/art/VISUAL_BIBLE.md` is the approved-draft Potes/Liébana art source. It calls for modular reuse and an H2 hero target of at most roughly 120 meshes. HK08A's 72 structural object records + 24 opaque metadata extensions are a bounded mixed-resource sub-scene probe below that future hero scale, not new gameplay semantics or a claim that an entire scene is one transaction.
2. **Mutation authority:** expected state/revision/hash and journal effects are observed from the accepted session, independently of the mutation request's own success flag.
3. **Pagination sequence:** expected completeness is checked against persisted sequence numbers and entry IDs across pages, not against the cursor implementation itself.
4. **Transport universe:** JSONL and MCP are separate external processes. Their results are compared after removing only transport request correlation.
5. **Discovery universe:** cost, side-effect and batching data come from canonical `CapabilityDefinition` metadata; transports only project that inventory.
6. **Compatibility universe:** the accepted HK06A complete journal schema and full inherited regression remain the oracle for replay/audit compatibility.

## Worker observation and convergence

Exact implementation/test SHA `7102330e9a3030d8f4f333d2715a8633312eeb7b` passed GitHub Actions run `35498188203` on the pinned .NET SDK 8.0.425:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk08A*`: 13/13 GREEN;
- full regression: 173/173 GREEN;
- candidate clean before and after: YES;
- observation artifact: `10600817518`.

Worker pre-review deliberately did not freeze the earlier green SHA `98b4696fe33f2cd26631249a058305993ec12a75`: it found that the initial Base64 cursor context could be edited without an integrity check. That in-claim false-green was repaired, then the ordinary multi-property modify proof and candidate-level validation negative were strengthened. The final implementation observation above is after all three pre-review findings.

## Proof-budget verdict

The final proof additions map directly to named HK08A acceptance criteria: one cursor alteration control, one multi-property interaction-shape proof and one candidate-level validation control. No generic hardening framework, alternate registry or speculative recovery machinery was added. Further hardening against fabricated opaque cursors, hostile clients, final size/time budgets or stale-CAS recovery would cross the declared boundary into security/operations/HK08B/HK09B rather than improve this proof.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
