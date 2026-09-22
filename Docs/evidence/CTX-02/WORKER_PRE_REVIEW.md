# CTX-02 Worker Pre-Review — Final Circuit-Breaker

WP: `WP-CTX-02`  
PR: `#113`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Pre-report repaired HEAD: `5b5660cc3c325dc4f7ad8f2fba7edff581f8adc3`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
`fail_cycle: 5`

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 33`

`UNSAFE: 0`

`PROOF_BUDGET: WITHIN_BUDGET`

This is the complete Worker pre-review rebuilt from zero after review `#5274609459` and **after** the final contract/control/evidence/predecessor refreshes. It supersedes the earlier cycle-5 CLEAN report that was invalidated by subsequent repository-byte mutations. This report commit is the final planned byte mutation before exact-SHA validation, freeze and independent handoff.

Durable final-audit authority: `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`.  
Claim→source→oracle→mutation matrix: `Docs/evidence/CTX-02/CONTROL_MATRIX.md`.  
Representative/integration runs: `Docs/evidence/CTX-02/DRY_RUNS.md`.  
Proof proportionality: `Docs/evidence/CTX-02/PROOF_BUDGET.md`.  
Predecessor refresh: `Docs/evidence/CTX-02/PREDECESSOR_CONTRACT_CHECK.md`.

## 1. Full baseline / failed-candidate / live-state reconstruction

The pre-review re-read the exact WP contract and the complete baseline→candidate surface, not only the latest patch.

- baseline→pre-report HEAD: `f7b4f1e8247dfc927203dca6754b71aaa53938f3..5b5660cc3c325dc4f7ad8f2fba7edff581f8adc3`;
- branch is 85 commits ahead of its CTX-02 baseline with 29 changed files, all inside role/bootstrap/process protocol, capsule schema/index/capsules, CTX-02 evidence, checker/controls and dedicated CI;
- latest failed candidate→pre-report HEAD: `210d42cc41199716e46d38c23ed7a550b8038a1c..5b5660cc3c325dc4f7ad8f2fba7edff581f8adc3`;
- PR remains open, Draft and mergeable during this review;
- live `main` is `b831050e9df8b61b76744e0c5f544bd7ec2d79b5`, advanced by later accepted planning work; the branch therefore diverges from current main but has no merge conflict and no accepted CTX-01 boundary change.

`PREDECESSOR_CONTRACT_CHECK.md` was refreshed after that main movement. CTX-01's reviewed candidate, PASS, merge and inherited authority rules remain unchanged. No predecessor reopen or rebase is justified solely by unrelated DW planning movement.

## 2. Historical Reviewer FAIL lineage — all preserved

1. `8db17036ad65ca03540f14e71711d806179c3e1c`, review `#5274094804`: whole accepted-PA `disposition_source` + `dispositions` omission → **preserved automatic RED**.
2. `504b4ff25f67720be9adb6b948dff859413419f3`, review `#5274163927`: malformed reopen/escalation plus review ID without explicit PASS → **preserved automatic RED**.
3. `66ccbeccf699f98c591809c739a3118f1b376e9c`, review `#5274257119`: same-ID/source-pointer/fingerprint semantic substitution → **preserved independent semantic RED**.
4. `1afc4135e4eda8cfab8e85325dcafcf123978915`, review `#5274389905`: one-of-many control did not exercise its semantic oracle; PA universe was result-first → **preserved real representative-oracle RED + COMPLETE-workpack-side discovery**.
5. `210d42cc41199716e46d38c23ed7a550b8038a1c`, review `#5274609459`: PA `workpack_glob`/`result_template` remained index-controlled; DocSync “full” surface omitted omission controls → **closed at the causal class boundary**.

No prior repair was removed to close cycle 5.

## 3. Cycle-5 defects repaired

The Reviewer-reported defects were repaired first:

- PA workpack discovery is the checker constant `Docs/workpacks/PA/WP-PA-[0-9][0-9].md`; index value must equal it and cannot narrow/broaden/empty the universe;
- PA result derivation is the checker constant `Docs/research/living-world/results/PA-{NN}.md`; index value must equal it and cannot redirect accepted results;
- `.agents/skills/update-handoff/SKILL.md` now contains all five canonical validation commands.

The final class audit then found **seven additional false-green paths** and repaired them:

1. subject-controlled `track` could suppress CITY-specific validation when combined with mandatory-read deletion;
2. subject-controlled `content_mode` could attempt to switch the validator regime;
3. PA `disposition_source.section/key_column/status_column` selected the table used as its own completeness oracle;
4. an index entry could redirect a capsule ID to an alternate valid-looking capsule JSON;
5. `--audit-index` could be pointed at an alternate index file;
6. identity could rebind to a same-name workpack under a different `Docs/workpacks/**` root;
7. representative material authoritative-source inventory could be narrowed while remaining fingerprints and capsule prose stayed valid.

Those seven plus the two Reviewer-reported cycle-5 defects account for nine cycle-5 findings; together with the previously recorded 24 fixed findings, the cumulative fixed count is 33.

## 4. Final invariants / oracle boundary

Mechanically consequential configuration is now checker-owned or exact-bound:

- canonical index/protocol/capsule paths;
- canonical workpack path from capsule ID;
- track and content mode derived from capsule ID;
- PA COMPLETE-workpack glob;
- PA canonical result template/path;
- current PA-01/02/03 disposition section/key/status-column selectors.

The audited subject may repeat these only as assertions. It cannot use metadata to select a friendlier universe/oracle.

Oracle classes remain explicit:

- **A / deterministic production:** canonical roots/selectors, exact accepted identity/PASS, external-source boundary, recomputed fingerprints, COMPLETE PA inventory, canonical result existence, checker-owned disposition selector, exact key/status equality, exact CITY seed, structural asymmetry;
- **B / bounded independent semantic controls:** actual selected HK/CITY/PA exports, exclusions, reopen/escalation semantics, PA directional values, representative capsule inventory, material representative source inventory and structurally valid one-of-many omissions;
- **C / HUMAN:** arbitrary future prose completeness/equivalence, contested interpretation and coordinated rewrite of both capsule prose and its test-only semantic fixture.

Class C outcome is explicitly `ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`, never implicit GREEN.

## 5. Hostile Reviewer pre-mortem — 17 attacks investigated

1. Narrow PA selector after deleting a future accepted item → **RED_AUTOMATIC**.
2. Redirect PA result template to shadow tree → **RED_AUTOMATIC**.
3. Supply alternate index to `--audit-index` → **RED_AUTOMATIC**.
4. Redirect index ID to alternate valid capsule file → **RED_AUTOMATIC**.
5. CITY→H1 + delete `CITY_PRODUCT_SEED.md` read → **RED_AUTOMATIC**.
6. Switch `content_mode` to disable canonical behavior → **RED_AUTOMATIC**.
7. Choose another valid-looking PA table in the same result → **RED_AUTOMATIC**.
8. Change PA key/status columns → **RED_AUTOMATIC**.
9. Use same-name workpack under another root → **RED_AUTOMATIC**.
10. Delete HK proof source while keeping guarantees/remaining hashes valid → **RED_SEMANTIC_ORACLE**.
11. Delete representative capsule + index entry together → PA **RED_AUTOMATIC**; bounded H1/CITY **RED_SEMANTIC_ORACLE** from independent expected inventory.
12. Spoof acceptance with review ID but FAIL verdict → **RED_AUTOMATIC**.
13. Replace reopen/escalation with opposite but non-empty prose → representative **RED_SEMANTIC_ORACLE**; arbitrary future prose HUMAN.
14. Change PA direction to another still-asymmetric expression → **RED_SEMANTIC_ORACLE**; collapse separately automatic RED.
15. Make defect test prove only mutation occurrence/helper failure → integration defects execute subprocess real `--audit-index`; omission semantics execute the actual representative oracle.
16. Add future COMPLETE PA capsule without reviewed checker-owned table selector → **fail closed / RECONSTRUCT**, no capsule-chain COMPLETE claim.
17. Change capsule semantics and its test-only fixture together → **ESCALATE_HUMAN to original accepted sources + independent Reviewer**; CI fixture is not semantic authority.

Result: `PRE_MORTEM_UNSAFE: 0`.

## 6. Mutation matrix result

`CONTROL_MATRIX.md` now durably records:

```text
claim
-> authoritative universe/source
-> independent oracle
-> defect mutation
-> required outcome
-> actual outcome
```

Covered classes include delete, paired delete, alternate path/source rebinding, narrow/broad/no-match selectors, alternate result template, whole structured-surface removal, blank/null substitution, same-ID semantic substitution, PASS spoofing, directional mutation, live-state mismatch, index inventory mutation, track/mode-assisted hiding and workflow drift.

Final result: `UNSAFE: 0`.

## 7. Preliminary exact repaired-head execution before this report

On pre-report repaired HEAD `5b5660cc3c325dc4f7ad8f2fba7edff581f8adc3`:

- Context Capsule Validation run `35695974725` / #75: **SUCCESS**;
- Arkus Candidate Validation run `35695974827` / #1052: **SUCCESS**.

The Context Capsule workflow executes the exact canonical surface:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

Thus the fully repaired code/contract/control/evidence surface was GREEN before this report was persisted. These runs are readiness evidence only: because this report commit changes repository bytes, both workflows must be GREEN again on the resulting exact HEAD before freeze.

## 8. Scope discipline

The complete baseline diff stays `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`. It changes no runtime/product code, Unity project, CITY product seed/result, accepted PA result, accepted predecessor workpack or product semantic architecture. It performs no bulk migration and invents no new product guarantee.

CTX-02 capsules remain navigation-only. Where safe automatic proof is unavailable, the contract escalates rather than reducing the claim or promoting a capsule/test fixture to truth.

## 9. Proof budget

`PROOF_BUDGET: WITHIN_BUDGET`.

The added authority is mechanical repository convention/selector ownership. No production prose registry, fuzzy/NLP equivalence, generalized historical capsule migration or semantic theorem prover was introduced.

## 10. Final no-write exit gate

This report commit is the last planned repository-byte mutation. From its resulting HEAD onward:

- no writer may change branch bytes unless exact-candidate validation finds a new defect;
- Context Capsule Validation must be GREEN on the exact resulting SHA;
- Arkus Candidate Validation must be GREEN on the exact same SHA;
- branch HEAD must remain that SHA;
- PR body must declare that same Candidate/Frozen SHA, `FROZEN_FOR_REVIEW`, `Branch frozen: YES`, `fail_cycle: 5`;
- PR may then be marked Ready;
- durable `REVIEW_READY` must name that exact SHA;
- a fresh independent Reviewer must review that SHA.

## Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 33`

`PROOF_BUDGET: WITHIN_BUDGET`

`UNSAFE: 0`

This CLEAN verdict authorizes exact-SHA validation/freeze/handoff only. It does **not** authorize merge or DocSync without a fresh independent Reviewer PASS.
