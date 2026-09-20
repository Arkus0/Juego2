# WP-HK-06C negative-conformance matrix

All controls below execute the public/effective replay surface or the dedicated replay-conformance guard. They target the defect classes explicitly required by `WP-HK-06C`; they are not syntax-fuzzing or predecessor re-proofs.

| Required defect class | Causal control | Expected RED condition | GREEN result |
|---|---|---|---|
| reordered journal entries | `Hk06CReplayTests.ReorderedMissingAlteredAndUnsupportedJournalEvidenceFailsBeforePublishingAnything` swaps accepted entries | sequence/order guard rejects before publication | `world.replay.sequence_invalid`; target revision/hash/journal unchanged |
| missing journal entry | same test removes the second entry and reconciles `entryCount` | chain/current mismatch cannot silently claim equivalence | `world.replay.sequence_invalid`; no publication |
| altered entry data | same test changes normalized operation data without changing HK06A identity | entry identity/request binding must turn red | `world.replay.entry_invalid`; no publication |
| wrong expected before hash | `WrongBasePreconditionAndExistingLocalHistoryCannotBeSplicedIntoReplay` supplies false CAS hash | optimistic replay precondition rejects | `world.replay.concurrent_update`; target unchanged |
| wrong/false expected final hash | `IntegrityValidLateResultDivergenceCannotPublishStagedStateOrJournal` independently recomputes an HK06A-valid second `entryId` around a false result/current hash | replay reaches staged canonical execution, then persisted result differs from journal evidence | `world.replay.result_divergence`; no staged state or journal published |
| unsupported replay-version combination | compatibility test queries journal@2 / snapshot version 2; replay test submits journal@2 envelope | compatibility/replay policy, not generic schema coercion, must fail closed | disposition `unsupported`; direct replay gives `world.replay.unsupported_version` |
| replay bypasses canonical validation | `IntegrityValidSemanticallyInvalidLaterStepMustBeRejectedByCanonicalValidationWithoutPartialPublish` independently recomputes request fingerprint + entry identity for a syntactically valid second mutation that would leave containment invalid | parser accepts evidence, canonical HK04/HK05 mutation authority must reject effective state | `world.replay.step_failed` at sequence 2; target state/journal unchanged |
| false success after partial/failing replay | both late-step self-attacks execute after an accepted first staged mutation | any incremental publication or unconditional success would change target / return success | failure returned and outer target remains exact pre-replay anchor with journal count 0 |
| existing local history silently spliced/replaced | `WrongBasePreconditionAndExistingLocalHistoryCannotBeSplicedIntoReplay` uses a target whose current anchor matches source base but journal already has one local entry | replay must not discard or merge unrelated local lineage | `world.replay.target_history_not_empty`; prior state/history retained |
| replay route aliases ordinary mutation/rebase classification | `ReplayClassificationCannotAliasOrdinaryMutationOrRebaseSurface` removes/mismatches dedicated replay route | public `CanonicalReplay` definitions and dedicated handler set diverge | `ReplaySurfaceConformance` turns RED; production composition is conformant |
| new replay metadata escapes HK01 independent universe/projection | inherited HK01 route-universe/projection regression | adding two public routes without independent enumeration/token support changes exact universe and/or projection | observed initial RED at 19→21 / 16→18 and missing token integration; final regression GREEN |
| read-only compatibility route mutates state | inherited HK04 effective non-writer oracle includes `authoring.replay.compatibility` | any state revision/hash change turns RED | compatibility call succeeds with identical revision/hash |

## Effective late-failure controls

The two strongest controls deliberately do **not** fail in the replay parser:

- the result-divergence control independently recomputes the accepted entry-identity framing after forging a false result hash;
- the canonical-validation control independently recomputes both the accepted request fingerprint and entry identity after replacing the second request with a syntactically valid but semantically invalid mutation.

Therefore these tests only turn GREEN if replay actually stages through the accepted mutation authority, observes the effective result/failure, and withholds the outer aggregate publication. This directly covers the WP's partial-failure and validation-bypass classes without a test-only production executor.

Implementation/test SHA `0d7e22be092d8291d6f7e239e73ed7e91217bd3a`: focused HK06C 8/8 GREEN and full regression 136/136 GREEN in Actions `35488744134`, artifact `10598333769`.