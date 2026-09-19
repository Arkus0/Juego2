# WP-HK-02A Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 5
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-02A

Baseline: `5a07c55aeb79406a84bff579b34c09459707b713`

Latest implementation observation: Actions run `35454985416` on exact SHA `f3c9a8f918a4389298cbb4a9515e4cd701f990ad` — Release build 0 warnings / 0 errors, focused HK02A 8/8 GREEN, full regression 95/95 GREEN and canonical receipt `Result: GREEN`.

This report and the proof/residual reconciliation are documentation-only additions to the reviewed worktree. Their resulting exact SHA must receive a fresh canonical observation before freeze.

## Contract and predecessor re-check

Re-read `WP-HK-02A`, `AGENTS.md`, `WORKER_REVIEW_PROTOCOL.md`, `FOUNDATIONAL_PROOF_STANDARD.md`, `PRODUCT_ARCHITECTURE.md`, `EXECUTION_RECEIPT_PROTOCOL.md`, the complete accepted HK04 WP/evidence and independent PASS review `#5256156672`.

- HK04 reviewed candidate `849ed68e41d674ab0d50883ccd9af394e9d2e456` PASSed on PR #19, exact-SHA run `35450965678`, and merged as `b1810a5f7c5378ff06272718a11e08720d714a65`.
- HK02A continues to consume HK01 route composition, HK02 stable object/world identity, HK03 bounded side-effect-free reads and HK04 atomic/CAS/idempotent internal-commit authority.
- No observation showed an accepted predecessor guarantee false or inapplicable. Updating the accepted codec/read/mutation semantics for new fields is the current WP's explicit ownership.

## Complete baseline-to-candidate audit

The complete diff from `5a07c55…` was inspected. Product changes are limited to:

- `Arkus.Game.World`: composite extension identity, optional subject, declared dependencies, referential validation and V2 canonical format/hash;
- `Arkus.Game.Authoring`: composite extension selectors/results, dependency paging, mutation grammar/key/fingerprint and dependency-aware semantic change coverage;
- tests: direct acceptance/negative controls and extension of accepted HK02/HK03 semantic/reconstruction inventories;
- canonical HK02A observation/verification entrypoints and Worker evidence.

No Unity/engine type, transform/schedule/NPC/business schema, ECS framework, payload interpreter, journal/replay implementation, HK05 diagnostic system, package dependency, public route, new commit authority or concurrency relaxation is introduced.

## Strict falsification

### Identity and hash omission

Challenge: subject or dependency semantics could exist in memory but disappear from canonical identity/hash.

Result: valid states differing only in subject or dependency target must have different hashes; exact V2 round trip preserves both; caller ordering is intentionally reversed and remains non-semantic. The pre-review found that the original reverse-input setup changed dependency order after immutable constructor copy and therefore did not exercise that branch. It was corrected before CLEAN.

### Proof-universe growth

Challenge: the new public `WorldExtensionIdentity` type could grow outside the accepted semantic-property inventory.

Result: the pre-review found and fixed that omission. Exact constructor/property inventories now include both `WorldExtensionData` and `WorldExtensionIdentity`, while HK03 reconstruction proves the effective public data rebuilds the canonical hash.

### Referential validation and atomic removal

Challenge: a valid world could become dangling when removing a subject or dependency target.

Result: the final candidate is constructed and validated before inherited HK04 commit. The pre-review expanded the removal control to exercise both the subject and dependency-target cases; neither reaches apply and canonical revision/hash remain unchanged.

### Composite mutation address

Challenge: HK04 could still key/remove extensions only by owner/version.

Result: two subjects with the same owner/version coexist; put fingerprints and dictionary keys include optional subject; removing one composite key leaves the other. Global omission of subject retains the accepted global key.

### Change-set completeness

Challenge: dependency changes could affect effective state without appearing in the plan.

Result: planner and independent `WorldMutationCoverage` each derive dependency field/add/remove effects from before/after states. A real dependency replacement with an empty declared change list turns the oracle RED.

### HK03 reconstruction and bounds

Challenge: query/read may expose only descriptors or one page and still claim completeness.

Result: descriptors carry scope/subject/count; reads separately page payload and sorted dependency edges; the inherited full reconstruction loops over extension, payload and dependency pages and must reproduce the exact canonical hash.

### Global compatibility and inherited guarantees

Challenge: additive parameters could break global extensions or accidentally alter HK04 transaction semantics.

Result: constructor/request fields are optional, global identity is explicit and the complete 95-test regression is GREEN. Commit authority, whole-world CAS, idempotency and atomic replacement code paths are unchanged and intentionally not re-proved beyond regression.

## Findings fixed

1. Missing `Arkus.Harness.Runtime` import prevented the focused test class from compiling.
2. Two successful-dispatch test paths lacked explicit nullable-flow assertions; warnings-as-errors caught both.
3. Dependency order was reversed after constructor copy, leaving the intended ordering control ineffective.
4. The new public `WorldExtensionIdentity` property/input universe was omitted from the reflection completeness inventory.
5. Transactional object removal tested a dependency target but not the extension subject; the control now covers both.

## Content-shape, residual risk and proof budget

The bounded Potes/Liébana probe was rerun through the completed inspect/mutate surfaces and found no in-scope blocker or predecessor reopen condition. Opaque-payload declaration honesty, whole-world CAS, V1 migration, total-size budgets and content-specific semantics are explicit non-blocking residuals outside the claim.

Proof converges on existing effective oracles and one focused test class. No external framework, parallel semantic registry or repeated predecessor hardening was added.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

## Handoff readiness

No known in-claim blocker remains in the complete intended candidate tree. `WORKER_PRE_REVIEW: CLEAN` is justified. After this documentation reconciliation is committed, that exact SHA must receive GREEN canonical observation; only then may PR metadata bind the same Candidate/Frozen SHA, set `FROZEN_FOR_REVIEW`/`Branch frozen: YES`, and mark the PR Ready. This Worker does not issue the independent PASS/FAIL verdict.
