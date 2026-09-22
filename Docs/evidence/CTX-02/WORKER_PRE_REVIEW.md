# CTX-02 Worker Pre-Review

WP: `WP-CTX-02`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`

Original pre-review subject before first report persistence: `12c4bfaa01b74c11668a930e4cc06c30813fa02e`  
First frozen handoff candidate: `975249f242650b47b96e2af0fbf57a1d30cef23a`  
First independently reviewed candidate: `8db17036ad65ca03540f14e71711d806179c3e1c`  
First independent FAIL: review `5274094804`  
Repair-cycle-1 pre-review subject before report persistence: `d520768ff37189ed847d23259ce45aa0a77d893e`  
Second independently reviewed candidate: `504b4ff25f67720be9adb6b948dff859413419f3`  
Second independent FAIL: review `5274163927`  
Repair-cycle-2 pre-report subject: `18d8200f689cfb5db2db4956c4bdea89f74bf4f4`

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 14`

This report records the complete strict Worker pre-review after the second independent CTX-02 FAIL. PR #113 was returned to Draft + ACTIVE before repair, the prior frozen candidate was treated as historical FAIL evidence, the causal blocker was reproduced, the complete baseline-to-current-candidate surface was re-inspected, nearby false-green paths in the same declared-schema boundary were challenged, and the affected plus canonical validation surfaces were rerun. Persisting this report is the final planned repository-byte mutation before a new exact-SHA freeze.

## Contract and inherited boundary

Re-read/challenged for this repair:

- `Docs/workpacks/CTX/WP-CTX-02.md` objective, Work, Forbidden, Required controls, Acceptance and DoD;
- accepted CTX-01 completion/PASS lineage and predecessor contract check;
- `AGENTS.md`;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`, including the explicit rule that CTX-02 itself remains governed by the pre-adoption predecessor-read mechanics;
- `Docs/engineering/CONTEXT_CAPSULE_V1.md` and `Docs/engineering/context-capsule-schema.json`;
- representative H1/CITY/PA accepted sources and capsule fixtures;
- independent FAIL reviews `5274094804` and `5274163927` on their exact reviewed candidates.

Neither FAIL reopens CTX-01 or an accepted product/PA semantic result. Both findings are wholly inside CTX-02's newly owned capsule validation/control boundary. CTX-01's authority ordering, fail-closed escalation, Worker pre-review, Reviewer independence and H1 local-executor boundary remain consumed rather than re-proved.

Live `main` is `c5a9a0d6fdd8057622068dadb5aa3f491d69964d`. Since the prior repair review, the relevant advance from `a2929bdf488e4fd9ffc9d59cf36886c92ef95b51` changes only the future `Docs/workpacks/CTX/WP-CTX-03.md` plan via PR #115. It does not alter CTX-01, CTX-02, the accepted H1/CITY/PA source identities, this PR's write set or dependency validity. The persisted predecessor check therefore remains valid.

## Complete diff / scope verdict

The complete baseline→candidate diff was re-inspected, not only the latest patch. CTX-02 remains limited to:

- context/process authority and role-routing documentation;
- capsule protocol/schema/index and representative capsule data;
- capsule validator/control scripts and CI orchestration;
- CTX-02 evidence.

No product/runtime contract, canonical harness implementation, gameplay, Unity project, CITY production geometry/result or accepted PA source result is changed.

The second-FAIL repair delta from reviewed candidate `504b4ff25f67720be9adb6b948dff859413419f3` to pre-report subject `18d8200f689cfb5db2db4956c4bdea89f74bf4f4` changes only:

- `scripts/context-capsule-check.py`;
- `scripts/context-capsule-controls.py`;
- `Docs/evidence/CTX-02/CONTROL_MATRIX.md`;
- `Docs/evidence/CTX-02/DRY_RUNS.md`.

The checker/control changes close the reported schema/identity false-green class; the two evidence edits synchronize durable evidence with the real controls and correct one stale H1 proof pointer found during the full pre-review. Scope verdict: **WITHIN WP-CTX-02**.

## Acceptance challenge

### H1

`WP-HK-GATE` remains a boundary/navigation capsule bound to accepted reviewed/merge identity and canonical `VERDICT.md`, `PROOF_MATRIX.md` and `RESIDUAL_RISK.md`. The full-pre-review inspection found that `DRY_RUNS.md` still named the already-repaired nonexistent `FINAL_VERDICT.md`; it is now corrected to `VERDICT.md`. H1-02 continues to own Unity/toolchain guarantees and its local execution protocol.

### CITY

`WP-CITY-03` remains boundary-only. `CITY_PRODUCT_SEED.md` remains mechanically mandatory and `noncompressible=true`; actual geometry/construction questions still require the exact seed. The generalized shape validation now also verifies every declared mandatory read has a valid bound source, `noncompressible=true` and a non-empty reason; CITY still additionally requires a non-empty mandatory-read set.

### PA

PA-01/02/03 remain one accepted structured-disposition capsule each. Accepted PA-chain coverage is discovered from canonical `PA-NN.md` + matching COMPLETE workpack state, not from the capsule index's own inventory. Every accepted PA capsule must remain `track=PA`, `content_mode=structured_disposition`, carry `disposition_source`, and carry a non-empty `dispositions` list whose exact key→status map matches the canonical result table. The prior whole-surface omission regression remains protected through the production `--audit-index` path.

Compound/deferred/exclusion states remain exact, including `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT`, `REJECT as authority` and `REJECT baseline`. PA-03's inherited `default global/N-hop social traversal to discover targets = REJECT` and distinct A→B/B→A trust/affinity/fear semantics remain protected.

## Findings found and repaired across CTX-02 Worker pre-review / review cycles

1. **Final-evidence rerun gap.** CI now triggers on `Docs/evidence/CTX-02/**`.
2. **Positive-loss false proof.** One-of-many material export omission is detected by an independent test-only oracle while structural validation remains otherwise valid.
3. **Exclusion-loss symmetry.** One-of-many material exclusion omission is independently detected.
4. **H1 proof navigation error.** HK-GATE capsule was rebound to canonical `VERDICT.md`, `PROOF_MATRIX.md`, `RESIDUAL_RISK.md` with exact fingerprints.
5. **Optimization not binding at authority layer.** Post-adoption role/protocol wiring permits capsule-start navigation without moving semantic/proof authority.
6. **Protocol self-adoption ambiguity.** v1.9 adoption is deferred until CTX-02 independent PASS + merge + DocSync; CTX-02 itself stays under v1.8 predecessor mechanics.
7. **Residual unconditional Worker profile read.** Bootstrap profile now expresses capsule-or-authoritative-source reconstruction with fail-closed escalation.
8. **Discoverability regression path.** Bootstrap/Worker/Repair/Reviewer/DocSync capsule protocol wiring is independently guarded.
9. **Ready handoff predecessor marker missing.** Durable predecessor evidence gained the canonical literal marker and the full pre-review was rerun before the first reviewed repair candidate.
10. **Accepted-PA whole structured surface false green** — Reviewer `5274094804`. Accepted PA-chain capsules now require structured mode + `disposition_source` + non-empty `dispositions`; independent exact-CLI control deletes the whole pair and requires `--audit-index` RED.
11. **Malformed reopen/escalation list-item false green** — Reviewer `5274163927`. `reopen_conditions` and `escalate_if` now require each element to be a non-empty useful string; independent production-path controls inject `null`, empty and whitespace-only values across both fields and require RED.
12. **Review-id without explicit PASS weakness** — Reviewer `5274163927`. Completion parsing now requires an explicit independent PASS verdict as well as the matching review id. The control mutates PASS→FAIL while recomputing the bound source fingerprint, proving the identity parser itself causes RED.
13. **Nearby published-schema shape gaps found during cycle-2 full pre-review.** Because CI intentionally does not run a separate JSON-Schema engine, the production checker now also fail-closes required `content_mode`, exact accepted-identity fields, required `mandatory_source_reads`, malformed mandatory-read objects, consumer-hint types, statement source-pointer types and malformed directional structures. Self-tests delete/malform representative fields so the reported element-type bug is not repaired as a one-off special case.
14. **Stale dry-run H1 pointer found during full pre-review.** `DRY_RUNS.md` still named `FINAL_VERDICT.md` after the capsule itself had been correctly repaired to `VERDICT.md`; durable evidence now matches the real canonical path.

## Causal negative controls on the repaired candidate

The validation surface now exercises at least:

- reviewed-candidate SHA mismatch -> FAIL closed;
- source blob fingerprint mismatch -> FAIL closed;
- missing/invalid `content_mode` or required minimum list shape -> FAIL closed;
- `reopen_conditions` containing `null` / blank / whitespace-only -> production `--audit-index` FAIL;
- `escalate_if` containing `null` / blank / whitespace-only -> production `--audit-index` FAIL;
- completion metadata changed from independent PASS to FAIL while fingerprint stays valid -> production `--audit-index` FAIL;
- external accepted state `REOPENED` -> FAIL closed;
- external accepted exact-SHA mismatch -> FAIL closed;
- one material positive export omitted while another remains -> independent control RED;
- one material exclusion omitted while another remains -> independent control RED;
- authoritative PA disposition row omitted -> FAIL;
- `LATER` reclassified -> FAIL;
- whole accepted-PA `disposition_source` + `dispositions` surface removed -> production `--audit-index` FAIL;
- accepted PA capsule wrong/non-structured content mode -> chain audit FAIL;
- malformed/non-string directional semantics or A→B/B→A collapse -> FAIL;
- CITY non-compressible seed read missing/malformed -> FAIL;
- accepted PA result with matching COMPLETE WP omitted from capsule index -> coverage FAIL;
- capsule bootstrap/role wiring removed -> independent control FAIL.

The production capsule/index still does not define the universe used to prove itself: accepted identity comes from completion metadata/live state, source integrity from repository bytes, PA row coverage from canonical result tables, PA chain coverage from canonical result+WP discovery, and material-loss tests from a separate test-only oracle.

## Validation history

Historical accepted pre-review/freeze runs and independent FAILs remain preserved in PR history. The material repair-cycle checkpoints are:

- reviewed candidate `8db17036ad65ca03540f14e71711d806179c3e1c` -> independent review `5274094804`: **FAIL** on whole accepted-PA structured-surface omission;
- reviewed candidate `504b4ff25f67720be9adb6b948dff859413419f3` -> independent review `5274163927`: **FAIL** on malformed reopen/escalation element shape plus missing explicit PASS-verdict validation;
- first cycle-2 code repair `1f675928d9efa2ae1cb5f94a5747e99d28a53cbb`: Context Capsule Validation `35687826271` / #33 **SUCCESS** and Arkus Candidate Validation `35687826259` / #940 **SUCCESS**;
- schema-boundary hardening code `2fc02325c7ddb46b7bcb48ccea31e33d1cd5fc80`: Context Capsule Validation `35688242638` / #36 **SUCCESS** and Arkus Candidate Validation `35688242691` / #943 **SUCCESS**;
- exact pre-report subject `18d8200f689cfb5db2db4956c4bdea89f74bf4f4`: Context Capsule Validation `35688268326` / #37 **SUCCESS** (self-test, independent controls and accepted-chain audit all SUCCESS) and Arkus Candidate Validation `35688268337` / #944 **SUCCESS**.

## Residual boundary

No product/runtime semantics are claimed. Capsules remain non-authoritative navigation. Token counts vary by tokenizer; CTX-02 claims reduced repeated accepted-result payload/navigation, not a guaranteed token count. Exact non-compressible production/proof material remains mandatory whenever the current question requires it.

No known residual weakens the two Reviewer repairs. The cycle-2 hardening deliberately stays inside the already-published capsule schema/validator boundary instead of adding new capsule semantics.

## Current verdict before final no-write rerun

No known semantic, process or in-claim blocker remains. `WORKER_PRE_REVIEW: CLEAN` is Worker readiness evidence only, not independent acceptance. This report is the final planned repository-byte mutation. The resulting exact SHA must now receive the complete no-write Context Capsule Validation + Arkus Candidate Validation and then a coherent Ready/frozen handoff before a fresh independent Reviewer may judge it. No merge or DocSync is authorized by this Worker.