# WP-HK-04 causal self-attack matrix

The mandatory HK04 defect classes are covered by executable controls in `Hk04SelfAttackTests*`. Repair cycle 2 deliberately stops treating structural object-graph traversal as a completeness oracle.

| Required defect class | Causal mutant / attack | Independent/effective observable | Retained GREEN control |
|---|---|---|---|
| partial apply after mid-operation failure | first operation would create `node.should-not-persist`, later operation makes final world invalid | canonical hash + inventory before/after failed apply | `PartialApplyAfterLaterFailureMutantCannotChangeCommittedHash` |
| stale revision overwrite | second writer reuses old revision/hash after another writer commits | structured stale error + accepted hash remains unchanged | `StaleRevisionOverwriteMutantFailsBeforeReplacingAcceptedState` |
| duplicate retry | exact idempotency key/request is resent after accepted commit | revision advances at most once; replay flag true | `DuplicateRetryMutantCannotAdvanceRevisionTwice` |
| hidden mutation bypass | exact cycle-2 shape: a ReadOnly external handler holds the authoritative session behind `List<TransactionalWorldAuthoringSession>` and uses only public API. Its fixture dynamically invokes public `Apply` if one exists | the fixture is allowed to publish without collection-specific traversal; invocation must leave real revision/hash unchanged. If `Apply` becomes public again, the fixture invokes the real commit and the state assertions turn RED. Separately, every current non-mutation production route is successfully invoked against the same live session and must preserve revision/hash | `IndirectReadOnlyHandlerCannotCommitThroughPublicSessionApi`; `EveryEffectiveNonMutationRouteIsEvaluatedAndCannotChangeCanonicalState` |
| dry-run differs semantically from apply | construct divergent candidate after dry-run prediction | accepted HK02 canonical hash must equal predicted result after real apply | `DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash` |
| command effect absent from plan/change set | change `node.peer.typeId` while declaring empty changes | independent base→candidate semantic diff reports missing effect | `OmittedChangeEffectMutantTurnsIndependentCoverageOracleRed` |

Additional positives exercise all four finite operation kinds (`put-object`, `remove-object`, `put-extension`, `remove-extension`) and an actual two-writer concurrent race from one anchor; exactly one writer may commit.

`HiddenMutationBypassMutantBreaksMutationSurfaceEquality` remains a supplementary declaration/transaction-marker consistency control. It is not the causal proof for hidden write authority.

## Hidden-mutation RED→GREEN lineage

- Frozen candidate `28695c9d…`: the negative control was non-causal; it relabelled metadata and did not actually mutate authoritative state. Independent FAIL #1 exposed the false proof.
- Frozen candidate `dc81b054…`: a real direct-session mutator was added, but public `TransactionalWorldAuthoringSession.Apply` plus structural authority walking left ordinary `List<>`/array/foreign-holder indirection undetected. Independent FAIL #2 exposed that the proof method still depended on object shape.
- Repair cycle 2: public commit authority is removed. The exact `List<>` shape is now retained as executable fixture without adding collection traversal. Its `Invoke` looks up only a **public** `Apply`; if such a regression exists it performs the real commit and changes canonical hash/revision, making the control RED. In the repaired candidate it can only call `Plan`, so state stays GREEN.
- The separate whole-surface evaluated oracle closes the Runtime friend-boundary concern by exercising every current public non-mutation route, not by inspecting fields or call syntax.

Latest implementation observation before documentation reconciliation: SHA `914ab13f9fcb28f6d62974bd22eaf6dae1ccb6fe`, Actions run `35450750854`: Release build 0 warnings / 0 errors; transactional positives 9/9; HK04 causal controls 8/8; full regression 87/87; receipt `Result: GREEN`.
