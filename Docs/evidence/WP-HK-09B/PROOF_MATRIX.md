# WP-HK-09B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-09B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK09B adds one explicit H0 resource envelope below transport-specific adapters and makes the canonical mutation, snapshot-import and replay publication seams fail closed under the limits/interruption modes H0 actually claims. It does not change the accepted HK04/HK05/HK06 semantic authorities, does not split the accepted HK08A 96-operation coherent edit, and does not claim crash/power-loss durability beyond the process-local checkpoint model.

The machine-readable envelope is exposed as `system.resource-envelope.describe@1.0`. Neutral projection admission enforces portable request bytes/depth, mutation batch/payload/relation limits, page size and snapshot size before dispatch. Authoring authority independently validates materialized world/snapshot limits before authoritative publication. Mutation, snapshot import and replay stage complete aggregates and perform one publication swap only after `InvocationResourceBudget.TryBeginPublication` succeeds.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| limits are explicit and machine-readable | H0 canonical/neutral boundary only | one `H0ResourceEnvelope` is used by canonical metadata, neutral admission and authoring resource checks | `EnforcedH0EnvelopeIsMachineReadableAndMatchesCanonicalMetadata` validates discovered schema/data and mutation batching metadata | schema/constant drift makes the test fail | PASS | values are H0 ceilings, not production-scale guarantees |
| representative coherent HK08A edit remains atomic | accepted 96-op HK08A shape | the envelope sets `maximumOperations=96`, exactly preserving the accepted product-shaped 72-object + 24-extension intent | `RepresentativeNinetySixOperationIntentFitsMeasuredEnvelopeAndCommitsOnce`; content-shape probe repeats 96 operations | restoring the former 64 cap or forcing a split turns the representative test red | PASS | larger future coherent intents require a reviewed envelope change |
| oversize/deep/batch/page work fails before canonical effect | neutral admission + canonical authoring checks | each owned dimension has a stable resource machine code and state/journal anchor is observed independently | `OversizeDeepBatchPayloadAndPageRequestsFailBeforeCanonicalEffect` | requests exceed bytes, depth, operation count, decoded payload and page size; any accepted effect makes revision/hash/journal assertions red | PASS | cooperative execution is not a hard CPU preemption mechanism |
| materialized world limits cannot be bypassed by transport shape | authoring candidate/import state | `WorldResourceLimits.ValidateState` independently rechecks resource count, relation count, extension payload and canonical world bytes after parsing/materialization | mutation/import focused tests + full regression | oversized snapshot and candidate resource violations are rejected before swap | PASS | arbitrary production-scale streaming remains outside H0 |
| invalid/oversized/unsupported import cannot replace state/history/evidence | process-local snapshot rebase authority | snapshot parses/validates before lock publication; imported state + fresh lineage + receipt + rebase evidence are one immutable `PortableSessionState` | `InvalidOversizedAndInterruptedImportCannotReplaceStateHistoryOrEvidence` | unsupported version, oversized state and publication interruption must preserve prior revision/hash/journal | PASS | no power-loss durability claim |
| persistence interruption cannot create half-commit or false evidence | in-process authoritative publication seam | mutation, import and replay build next aggregate before a single reference swap; receipt/journal/evidence are inside that aggregate | mutation/import/replay interruption tests | `InterruptedAtPublication(...)` forces failure immediately before swap; state and evidence must remain old | PASS | OS/process death during memory write is trusted infrastructure/outside claimed durability |
| execution/cancellation limits do not create another writer | canonical dispatcher + existing authorities | `InvocationResourceBudget` is passed into existing HK04/HK06B/HK06C authorities; it grants no state mutation API | expired/interrupted mutation test; inherited HK04 authority conformance remains GREEN | expired budget and publication denial return resource diagnostics without state/journal effect | PASS | long computation can consume CPU until a cooperative check |
| reference JSONL and MCP expose equivalent resource semantics | two real Release processes | both transports consume the same neutral projection; comparison removes request correlation only | `ResourceEnvelopeAndLimitDiagnosticsAreEquivalentAcrossReferenceJsonlAndMcp` | request bytes, page and depth errors are compared end-to-end; drifted dimension injection produces `semantic-drift` | PASS | transport framing may reject malformed/non-frame input outside canonical request semantics |
| accepted snapshot/replay semantics remain intact while publication becomes atomic | HK06B/HK06C integration seam only | no new snapshot format, journal format, replay algorithm or writer authority is introduced | inherited portability/replay regression + interrupted replay test | staged replay is denied at publication and target state/provenance must stay unchanged | PASS | distributed/multi-process writers remain outside H0 |
| bounded long-session growth has an explicit H0 ceiling | process-local H0 session | transaction history and snapshot import receipt counts are bounded in the same envelope and enforced at authority | machine-readable `maximumSessionTransactions=10000`, `maximumImportReceipts=1024`; full regression covers accepted history behavior | authority returns stable resource diagnostics when a ceiling would be crossed | PASS | HK10 still owns measured long-session behavior and may justify tuning |
| product-shaped content is not hidden by abstract fixtures | representative approved Juego2 slice | `Docs/art/VISUAL_BIBLE.md` independently supplies Potes/plaza/market/bar/workshop context; candidate surface represents it with existing generic IDs/extensions | `Hk09BContentShapeProbeTests.PotesHeroSliceFitsOneBoundedTransactionAndCheckpointRebase` | exact 96-op fit, export/import hash equality and fresh-lineage evidence are asserted | PASS | transforms/gameplay/schedules are not promoted into H0 schemas |

## Independent/effective universes

1. **Resource-limit universe:** the public machine-readable envelope is compared with actual runtime constants/metadata and with independent observed state effects; success metadata cannot hide a canonical state change.
2. **Mutation/publication universe:** authoritative state is observed through revision/hash and HK06A journal after forced budget failure, not through the failing operation's own response.
3. **Import universe:** pre-existing target state and journal are captured before malformed/oversized/interrupted imports and required unchanged after rejection.
4. **Replay universe:** a real source journal is staged against a separate target and publication is denied after staging; target state/provenance is then observed independently.
5. **Transport universe:** reference JSONL and MCP run as distinct Release processes and compare complete neutral structured outcomes after removing only request IDs.
6. **Product-shape universe:** `Docs/art/VISUAL_BIBLE.md` is independent of harness fixtures; the bounded slice uses its plaza/market/bar/workshop vocabulary without changing canonical schemas.
7. **Route/inventory universe:** inherited HK01 route/conformance tests and HK04 effective non-writer-route execution now include the new base resource-envelope capability, preventing the discovery proof from silently shrinking around it.

## Implementation convergence before freeze

The implementation checkpoint `e370606e4278da3e08602a3167c1cb6513bea93d` passed GitHub Actions run `35521120979`:

- Release build: 0 warnings / 0 errors;
- focused `Hk09B*`: 9/9 GREEN;
- full regression: 205/205 GREEN;
- candidate clean before/after: YES;
- observation receipt reports resource-envelope, persistence-interruption, transport-conformance, representative-content-shape and regression gates GREEN.

Before that checkpoint, full regression correctly exposed four inherited oracles whose literal assumptions treated `system.describe` as the only base capability. The repair updated those oracles to include `system.resource-envelope.describe@1.0` while preserving their actual invariants: complete canonical inventory and every effective non-writer route executed without canonical mutation.

The final frozen evidence-only SHA must rerun `scripts/hk09b-verify-exact-sha.sh <SHA>` through the exact-SHA workflow before independent review.

## Proof-budget verdict

Proof additions map directly to HK09B acceptance: one neutral resource policy, one authoring state-limit helper, publication-budget hooks on existing authorities, three focused test classes, and bounded evidence. No WAL, filesystem durability layer, distributed transaction mechanism, generic load framework or alternate semantic registry was added.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
