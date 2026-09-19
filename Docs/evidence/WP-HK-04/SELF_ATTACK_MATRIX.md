# WP-HK-04 causal self-attack matrix

The mandatory HK04 defect classes are challenged by executable negative controls in `Hk04SelfAttackTests*`. They are not checklist assertions against the implementation: each test constructs or models the forbidden mutant and requires an independent observable to turn red.

| Required defect class | Causal mutant / attack | Independent observable | Retained GREEN control |
|---|---|---|---|
| partial apply after mid-operation failure | first operation would create `node.should-not-persist`, later operation makes the final world invalid | canonical state hash + object inventory before/after failed apply | `PartialApplyAfterLaterFailureMutantCannotChangeCommittedHash` |
| stale revision overwrite | second writer reuses the old revision/hash after another writer committed | structured stale error + accepted canonical hash remains unchanged | `StaleRevisionOverwriteMutantFailsBeforeReplacingAcceptedState` |
| duplicate retry | exact same idempotency key/request is sent after accepted commit | revision may advance exactly once; replay flag is true | `DuplicateRetryMutantCannotAdvanceRevisionTwice` |
| hidden mutation bypass | isolated public `engine.observe@1.0` handler stays contractually `ReadOnly`, holds the real authoritative session and calls `Apply`; direct invocation proves canonical hash/revision change | publication-time object-graph authority guard rejects the non-transactional handler; metadata-independent assembly inspection reports `engine.observe@1.0` as extra effective write authority | `RealReadOnlyHandlerMutationIsBlockedAndTurnsIndependentAuthorityOracleRed` |
| dry-run differs semantically from apply | construct a divergent candidate after obtaining dry-run's predicted canonical hash | accepted HK02 canonical content hash must equal the dry-run prediction after real apply | `DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash` |
| command effect absent from plan/change set | change `node.peer.typeId` while declaring an empty change set | independent base→candidate semantic diff reports the missing effect | `OmittedChangeEffectMutantTurnsIndependentCoverageOracleRed` |

Additional positive controls exercise all four finite micro-world operation kinds (`put-object`, `remove-object`, `put-extension`, `remove-extension`) and an actual two-writer concurrent race from one anchor. Exactly one concurrent writer may commit.

The original `HiddenMutationBypassMutantBreaksMutationSurfaceEquality` remains as a supplementary declaration/marker consistency control. It is not counted as the causal evidence for a real hidden mutation bypass.

Observed on repair SHA `7da26b0e45847aba5f63d0a079001ce550460c4d`, GitHub Actions run `35443591856`: the isolated attack fixture compiled under `Release`; build 0 warnings / 0 errors; HK04 transactional positives 9/9; HK04 causal controls 7/7; full regression 86/86; canonical receipt `Result: GREEN`.
