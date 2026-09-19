# WP-HK-04 causal self-attack matrix

The mandatory HK04 defect classes are challenged by executable negative controls in `Hk04SelfAttackTests`. They are not checklist assertions against the implementation: each test constructs or models the forbidden mutant and requires an independent observable to turn red.

| Required defect class | Causal mutant / attack | Independent observable | Retained GREEN control |
|---|---|---|---|
| partial apply after mid-operation failure | first operation would create `node.should-not-persist`, later operation makes the final world invalid | canonical state hash + object inventory before/after failed apply | `PartialApplyAfterLaterFailureMutantCannotChangeCommittedHash` |
| stale revision overwrite | second writer reuses the old revision/hash after another writer committed | structured stale error + accepted canonical hash remains unchanged | `StaleRevisionOverwriteMutantFailsBeforeReplacingAcceptedState` |
| duplicate retry | exact same idempotency key/request is sent after accepted commit | revision may advance exactly once; replay flag is true | `DuplicateRetryMutantCannotAdvanceRevisionTwice` |
| hidden mutation bypass | a public definition is changed to `CanonicalMutation`/`CanonicalTransaction` while its effective handler does not implement the transactional marker | set equality between canonical mutation definitions and effective transactional handlers | `HiddenMutationBypassMutantBreaksMutationSurfaceEquality` |
| dry-run differs semantically from apply | construct a divergent candidate after obtaining dry-run's predicted canonical hash | accepted HK02 canonical content hash must equal the dry-run prediction after real apply | `DryRunDivergenceMutantIsDetectedByPredictedCanonicalHash` |
| command effect absent from plan/change set | change `node.peer.typeId` while declaring an empty change set | independent base→candidate semantic diff reports the missing effect | `OmittedChangeEffectMutantTurnsIndependentCoverageOracleRed` |

Additional positive controls exercise all four finite micro-world operation kinds (`put-object`, `remove-object`, `put-extension`, `remove-extension`) and an actual two-writer concurrent race from one anchor. Exactly one concurrent writer may commit.

Observed on SHA `1922fcb7e43729326b9860fb27b13e90d8a2b552`, GitHub Actions run `35439895690`: Release build 0 warnings / 0 errors; HK04 transactional positives 9/9; HK04 causal controls 6/6; full regression 85/85; canonical receipt `Result: GREEN`.
