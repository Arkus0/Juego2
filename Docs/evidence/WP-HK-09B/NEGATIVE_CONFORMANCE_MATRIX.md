# WP-HK-09B negative-conformance matrix

NEGATIVE_CONFORMANCE_VERDICT: PASS

The controls below are restricted to resource and process-local publication defects owned by HK09B. Accepted HK04/HK05/HK06/HK08 semantics are reused rather than re-proved except where the new limit/publication seam could falsify them.

| Required defect class | Causal control / oracle | Expected RED condition | GREEN result |
|---|---|---|---|
| oversized canonical request body | `OversizeDeepBatchPayloadAndPageRequestsFailBeforeCanonicalEffect` sends `system.describe` arguments above `MaximumCanonicalRequestBytes` | request reaches canonical behavior or returns a non-resource semantic result | `resource.request_bytes_exceeded`; canonical revision/hash/journal unchanged |
| excessive nesting/depth | same test builds nested portable data beyond `MaximumPortableDepth` | over-deep request reaches handler semantics | `resource.nesting_depth_exceeded` with `dimension=portableDepth` |
| excessive operation count | same test sends `MaximumBatchOperations + 1` operations | batch is accepted, silently truncated or partially applied | `resource.batch_operations_exceeded`; no canonical effect |
| excessive decoded mutation payload | same test supplies three 200 KiB extension payloads | payload volume is accepted or partially committed | `resource.batch_payload_exceeded`; no canonical effect |
| excessive query page size | same test asks above `MaximumPageSize` | oversized page reaches query behavior | `resource.page_size_exceeded` with stable resource context |
| representative coherent edit rejected by legacy cap | `RepresentativeNinetySixOperationIntentFitsMeasuredEnvelopeAndCommitsOnce` and the Potes content probe require 96 operations | restoring the old 64-op ceiling, or splitting the intent, fails exact one-request/one-revision expectations | all 96 operations fit one transaction, one revision, one journal entry |
| execution budget expires before mutation publication | `ExpiredAndInterruptedMutationBudgetsPublishNeitherStateReceiptNorJournal` uses `InvocationResourceBudget.ExpiredForTesting()` | state/receipt/journal becomes visible despite expired budget | `resource.execution_budget_exceeded`; old revision/hash/journal preserved |
| interruption after staging but before mutation publication | same test uses `InterruptedAtPublication("canonical-mutation")` | staged candidate or provenance becomes authoritative | `resource.persistence_interrupted`; retry later commits once and replays idempotently |
| unsupported snapshot replaces state | `InvalidOversizedAndInterruptedImportCannotReplaceStateHistoryOrEvidence` changes `snapshotVersion` | target state/history changes before format/version rejection | `world.snapshot.unsupported_version`; prior anchor and journal unchanged |
| oversized snapshot replaces state | same test supplies encoded state beyond H0 world envelope | any part of imported state/history/evidence becomes authoritative | `resource.snapshot_bytes_exceeded`; prior anchor and journal unchanged |
| interrupted import leaves mixed state/receipt/evidence | same test denies `snapshot-import` publication after full staging | imported world appears without matching fresh lineage/receipt, or receipt appears without state | old aggregate remains authoritative; successful retry publishes one coherent new aggregate |
| interrupted replay leaves staged state/provenance | `InterruptedReplayCannotPublishStagedStateOrProvenance` denies `journal-replay` publication after real staged replay | target state or HK06A provenance changes despite failure | `resource.persistence_interrupted`; target remains at original anchor/journal until normal retry |
| reference JSONL and MCP disagree on resource semantics | `ResourceEnvelopeAndLimitDiagnosticsAreEquivalentAcrossReferenceJsonlAndMcp` invokes both real Release hosts | capability/result/resource code/dimension differs between transports | normalized complete outcomes deep-equal for envelope, bytes, page and depth cases |
| transport comparison oracle is self-confirming | `TransportDriftOracleTurnsRedForDifferentResourceDimension` changes only one resource dimension in a cloned response | comparator still declares equality | injected `pageItems -> batchOperations` produces `semantic-drift` |
| new read-only resource capability escapes hidden-writer oracle | `EveryEffectiveNonWriterRouteIsEvaluatedAndCannotChangeCanonicalState` derives current non-writer definitions and requires a valid request vector for each | a new public non-writer route is unexercised or mutates canonical state | `system.resource-envelope.describe@1.0` is executed and revision/hash must remain unchanged |

## Effective controls and boundary

- Resource errors are asserted through stable machine codes and dimensions, while revision/hash/journal are observed separately so an error response cannot mask a partial effect.
- The 96-operation capacity control is inherited from accepted HK08A product-shaped evidence; the limit cannot be justified by making that coherent request smaller after the fact.
- Import interruption attacks the real H0 publication seam: a fully staged `PortableSessionState` containing state, fresh local lineage, keyed receipt and rebase evidence is denied immediately before the single authoritative reference swap.
- Replay interruption similarly denies the final aggregate swap only after a complete staged replay and provenance audit, proving staged state does not leak.
- Cross-transport conformance launches real JSONL and MCP processes. The comparator removes only request correlation and has its own deliberate semantic-drift injection.
- No power-loss/process-kill fixture is required because the discovered envelope explicitly says `powerLossDurabilityClaimed=false`; inventing one would expand the WP beyond its accepted durability claim.

Implementation checkpoint `e370606e4278da3e08602a3167c1cb6513bea93d`: focused HK09B 9/9 GREEN and full regression 205/205 GREEN in Actions run `35521120979`.
