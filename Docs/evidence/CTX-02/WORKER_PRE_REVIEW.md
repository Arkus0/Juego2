# CTX-02 Worker Pre-Review

WP: `WP-CTX-02`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
`fail_cycle: 3`

Historical independently reviewed candidates and FAILs:

- `8db17036ad65ca03540f14e71711d806179c3e1c` -> review `#5274094804`: **FAIL** — accepted-PA structured-disposition whole-surface omission;
- `504b4ff25f67720be9adb6b948dff859413419f3` -> review `#5274163927`: **FAIL** — malformed reopen/escalation items plus accepted identity not requiring explicit PASS;
- `66ccbeccf699f98c591809c739a3118f1b376e9c` -> review `#5274257119`: **FAIL** — representative semantic-content false-green with IDs/source pointers/fingerprints preserved while guarantee/exclusion statement text is inverted or invented.

Circuit-breaker re-audit persisted before cycle-3 repair: `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`, first commit `0b676eae0b4b0977113c96eb5b4e4d3aa626af29`.  
Cycle-3 pre-report subject: `e2f14b7e043c9f53a436cc8e89340c4a80d8707c`.

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 21`

`PROOF_BUDGET: WITHIN_BUDGET`

This report records the complete strict Worker pre-review after the third consecutive CTX-02 Reviewer FAIL. The PR was returned to Draft + ACTIVE before repair, no prior frozen candidate was reused as current evidence, and the repair was intentionally delayed until an explicit claim/trust-boundary re-audit had been persisted. The full baseline→candidate surface, all three historical FAIL classes, nearby same-class false-green routes, adoption wiring, current live-main movement, representative consumer boundaries, causal controls and proof proportionality were then re-reviewed.

Persisting this report is the final planned repository-byte mutation before an exact-SHA no-write validation rerun and freeze.

## 1. Circuit-breaker trust-boundary result

The re-audit separates CTX-02 into three oracle classes rather than pretending one checker can prove all semantics.

### A — production/source-derived invariants

The production checker may prove deterministic facts whose oracle is independent of the capsule/index:

- accepted identity from the matching canonical external workpack, including explicit COMPLETE + reviewed candidate + merge + PASS review;
- exact external authoritative source paths and recomputed Git blob fingerprints;
- rejection of capsule/CTX-02-generated self-confirmation paths;
- required shape and useful element types;
- accepted PA result-chain discovery from canonical `PA-NN.md` + matching COMPLETE workpack state;
- exact PA disposition key/status equality against the canonical accepted result;
- accepted PA `disposition_source` identity bound to that same independently discovered canonical result;
- exact representative CITY mandatory read `Docs/production/CITY_PRODUCT_SEED.md`;
- structural directional distinctness and other deterministic shape checks.

### B — representative semantic controls

Open-ended natural-language equivalence is not moved into production. Independent **test-only** fixtures validate actual material semantic content for the selected current consumer surface:

- `WP-HK-GATE`;
- `WP-CITY-03`;
- cumulative `WP-PA-01` + `WP-PA-02` + `WP-PA-03` used by the representative PA-04 consumer.

Those controls bind real exported guarantees and exclusions as `id -> statement + source_pointer`, exact reopen/escalation content, and PA-03 directional values. Defect injections preserve IDs, pointers and source fingerprints while changing material values, so the RED is caused by the independent semantic oracle rather than incidental source/shape failure.

### C — human/escalation-only judgment

CTX-02 does not mechanically prove arbitrary future prose equivalence/completeness, exhaustiveness of every future semantic list, disputed source interpretation, or whether later evidence causally reopens a predecessor. Those questions fail safe to authoritative source reconstruction + independent Reviewer judgment.

This A/B/C split is durable in `TRUST_BOUNDARY_REAUDIT.md`, `DESIGN.md`, `CONTROL_MATRIX.md`, `DRY_RUNS.md` and `CONTEXT_CAPSULE_V1.md`.

## 2. Contract and inherited boundary

Re-read/challenged during the circuit-breaker cycle:

- `Docs/workpacks/CTX/WP-CTX-02.md` objective, Work, Forbidden, Required controls, Acceptance and DoD;
- accepted CTX-01 completion/PASS lineage and `Docs/evidence/CTX-02/PREDECESSOR_CONTRACT_CHECK.md`;
- `AGENTS.md`;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`, including the adoption boundary that keeps CTX-02 itself under pre-CTX-02/v1.8 predecessor mechanics;
- `Docs/engineering/CONTEXT_CAPSULE_V1.md` and `context-capsule-schema.json`;
- bootstrap profiles and Worker/Repair/Reviewer/DocSync role skills;
- actual indexed `WP-HK-GATE`, `WP-CITY-03`, `WP-PA-01`, `WP-PA-02`, `WP-PA-03` capsules;
- matching accepted H1/CITY/PA authoritative workpacks/results/proof pointers;
- all three independent FAIL reviews on their exact reviewed candidate SHAs;
- complete baseline→pre-report diff;
- current live-main movement after the original baseline.

All three Reviewer findings are wholly inside CTX-02's newly owned compression/validation trust boundary. They do not reopen CTX-01, HK-GATE, CITY-03, PA-01, PA-02 or PA-03 accepted semantics.

## 3. Live-main movement / dependency validity

The original CTX-02 baseline remains `f7b4f1e8247dfc927203dca6754b71aaa53938f3`. During this repair cycle live `main` advanced to `13332b738626b43ace6047a9c141a840406bfed7`.

Baseline→live-main comparison shows changes only to:

- `Docs/workpacks/CTX/WP-CTX-03.md`;
- `Docs/workpacks/PA/WP-PA-13.md`;
- `Docs/evidence/PA-13/SCALABILITY_VIABILITY_SPIKE.md`.

Those changes do not overlap CTX-02's write set, CTX-01 acceptance, HK-GATE/CITY-03/PA-01/02/03 accepted source identities, or the current representative semantic oracle. They therefore do not invalidate the predecessor check or require rebasing the immutable accepted capsule source bindings.

## 4. Complete baseline→candidate scope review

The complete baseline→pre-report diff was inspected, not merely the cycle-3 patch. The pre-report subject is 62 commits ahead and 0 behind the original baseline and changes only CTX-02-owned process/context surfaces:

- role skills: implement / repair / validate / update-handoff;
- `AGENTS.md`;
- Context Bootstrap / Worker Review process docs and bootstrap profiles;
- new Context Capsule protocol/schema/index;
- five representative capsule data files (HK, CITY, PA-01/02/03);
- CTX-02 validation workflow;
- production source/structure checker;
- independent semantic/material-loss control harnesses;
- CTX-02 evidence, including re-audit and proof budget.

No product/runtime implementation, gameplay, Unity project, CITY geometry/result, canonical PA accepted result, accepted predecessor workpack, or architecture/product semantic source is modified.

Scope verdict: **WITHIN WP-CTX-02**.

## 5. Acceptance challenge by representative consumer

### H1

`WP-HK-GATE` remains a non-authoritative navigation boundary for H1-02. The capsule binds accepted identity plus canonical `VERDICT.md`, `PROOF_MATRIX.md` and `RESIDUAL_RISK.md`. The independent test-only oracle now validates actual material guarantee/exclusion text and pointers, so retaining a valid HK ID/source binding while inventing the opposite statement cannot silently remain CTX-02 GREEN.

Exact H0 proof still escalates to the named accepted evidence when material. Unity/toolchain/local execution remains H1-owned and is not compressed into capsule authority.

### CITY

`WP-CITY-03` remains boundary-only. The production checker now requires the exact `Docs/production/CITY_PRODUCT_SEED.md` mandatory read, fingerprinted and `noncompressible=true`; a different valid external file cannot self-satisfy that obligation. The independent semantic oracle also rejects the opposite `not-spatial-spec` statement even when ID, pointer and source fingerprints remain unchanged.

Geometry, routes, sites, scenarios, seams and measurement continue to require the exact seed. Capsule prose never becomes a spatial specification.

### PA

PA is cumulative. The representative PA-04 start surface is therefore PA-01 + PA-02 + PA-03 rather than PA-03 alone.

Production checks preserve exact disposition key/status values against the canonical accepted result and independently discover the accepted PA result-chain universe. A coherent but wrong table cannot certify another PA result because `disposition_source` must be the exact independently discovered canonical result.

The test-only semantic oracle covers actual exports, exclusions, reopen conditions and escalation triggers for all three current PA capsules; PA-03 additionally pins directional values. Compound/deferred/exclusion statuses remain source-derived rather than prose-fixture-derived.

This closes the cycle-3 statement-substitution class on the complete selected PA consumer surface without auto-enrolling arbitrary future PA prose into production proof machinery.

## 6. Findings found and repaired across CTX-02 pre-review/review cycles

1. **Final-evidence rerun gap.** Context capsule CI triggers on `Docs/evidence/CTX-02/**`.
2. **Positive-loss false proof.** One-of-many material export omission is detected by an independent test oracle.
3. **Exclusion-loss asymmetry.** One-of-many material exclusion omission is independently detected.
4. **H1 proof navigation error.** HK-GATE navigation was rebound to canonical `VERDICT.md`, `PROOF_MATRIX.md`, `RESIDUAL_RISK.md` with exact fingerprints.
5. **Optimization not binding at authority layer.** Adoption wiring permits capsule-start navigation without promoting capsules over accepted semantic/proof sources.
6. **Protocol self-adoption ambiguity.** v1.9 becomes binding only after CTX-02 independent PASS + merge + DocSync; this PR remains governed by v1.8 predecessor mechanics.
7. **Residual unconditional predecessor/profile read.** Bootstrap roles now express capsule-or-authoritative-source initial reconstruction with fail-closed escalation.
8. **Discoverability regression path.** Bootstrap/Worker/Repair/Reviewer/DocSync capsule wiring is independently guarded.
9. **Ready-handoff predecessor marker gap.** Durable predecessor evidence gained the canonical marker and was rerun before prior reviewed candidates.
10. **Accepted-PA whole structured-surface false green** — review `#5274094804`. Accepted PA-chain capsules require structured mode + canonical `disposition_source` + non-empty exact dispositions; deleting the whole pair makes chain audit RED.
11. **Malformed reopen/escalation item false green** — review `#5274163927`. Every element must be a non-empty useful string; null/empty/whitespace injections are RED through production `--audit-index`.
12. **Review ID without explicit PASS weakness** — review `#5274163927`. Completion parsing requires an explicit independent PASS verdict as well as matching review id; PASS→FAIL with recomputed valid fingerprint is RED.
13. **Nearby published-schema shape gaps.** Because CI intentionally does not rely on a separate schema engine as semantic authority, the production checker now closes required content mode, exact accepted-identity fields, mandatory-read shape, consumer hints, source-pointer types and directional structure.
14. **Stale dry-run H1 proof pointer.** Durable evidence was corrected from nonexistent `FINAL_VERDICT.md` to canonical `VERDICT.md`.
15. **Same-ID/same-source semantic statement false green** — review `#5274257119`. Actual representative `id -> statement + source_pointer` content is now validated by external test-only fixtures; same-ID/pointer/fingerprint guarantee and exclusion inversion controls are RED.
16. **Accepted-identity/source self-confirmation path found by re-audit.** `identity_source` must be the matching canonical external workpack; authoritative sources cannot point into the capsule/CTX-02 generated authority layer and certify themselves.
17. **PA disposition oracle source could be internally coherent but wrong.** Accepted PA chain now requires `disposition_source.path` and fingerprint to equal the independently discovered canonical accepted `PA-NN.md`; rebinding PA-03 to PA-02 rows is RED at chain validation.
18. **CITY mandatory-read identity could be substituted.** Representative CITY now requires exact `Docs/production/CITY_PRODUCT_SEED.md`, not merely any valid fingerprinted non-compressible read.
19. **Same semantic-substitution class existed in other claimed fields.** Test-only representative controls now RED on structurally valid but opposite/invented reopen conditions, escalation triggers and changed-but-still-distinct PA directional values.
20. **PA cumulative representative semantic gap found during complete review.** PA-04 consumes PA-01/02/03, so covering only PA-03 left the same statement-substitution class open in PA-01/02. A bounded PA-01/02 test-only oracle closes those actual current capsules without creating a production/future registry.
21. **DocSync validation wiring omitted the independent semantic harness.** Complete baseline review found `update-handoff` instructed future capsule DocSync to run checker self-test + index audit but not the semantic controls. It now requires the full four-command validation surface.

## 7. Causal negative controls on the repaired boundary

The final control surface includes:

- reviewed-candidate identity mismatch -> production RED;
- completion verdict PASS→FAIL with valid rebound fingerprint -> production RED;
- identity source redirected to valid-fingerprinted CTX-02 evidence -> production RED;
- authoritative source redirected to valid-fingerprinted capsule JSON -> production RED;
- source byte/fingerprint mismatch -> production RED;
- malformed/blank reopen and escalation elements -> production RED;
- one-of-many exported guarantee omitted -> independent semantic/material-loss RED;
- one-of-many exclusion omitted -> independent RED;
- HK guarantee statement inverted while ID + source pointer + fingerprints remain -> production structure/source remains valid; independent semantic RED;
- CITY exclusion statement inverted symmetrically while ID + pointer + fingerprints remain -> independent semantic RED;
- PA-01 guarantee statement inverted with same ID/pointer/source bindings -> independent semantic RED;
- PA-02 exclusion inverted with same ID/pointer/source bindings -> independent semantic RED;
- representative reopen condition replaced by opposite non-empty text -> independent semantic RED;
- representative escalation trigger replaced by opposite non-empty text -> independent semantic RED;
- PA-03 directional forward value invented while forward/reverse remain structurally distinct -> independent semantic RED;
- omitted authoritative PA disposition row -> production RED;
- `LATER`/compound/reject disposition reclassified -> production RED;
- whole accepted-PA disposition surface removed -> production PA-chain RED;
- PA-03 disposition source/rows rebound coherently to PA-02 -> production PA-chain RED;
- accepted PA result omitted from capsule index -> externally discovered coverage RED;
- CITY mandatory read omitted -> production RED;
- CITY mandatory read substituted by another valid fingerprinted file -> production RED;
- external accepted state `REOPENED` -> production fail closed;
- external accepted exact-SHA mismatch -> production fail closed;
- capsule/bootstrap/adoption wiring removed -> independent control RED.

The production checker intentionally stays GREEN on some structurally/source-valid invented prose mutations; that is evidence of the designed A/B separation, not a hole, because CTX-02's repository validation requires the independent representative control harness as a second gate.

## 8. Validation history

Historical reviewed FAILs remain durable and are not reset by this cycle.

Relevant cycle-3 checkpoints:

- `0f39db5ede64336e49c1adf7f2348cee6018f59c`: initial re-audited checker + actual HK/CITY/PA-03 semantic controls; Context Capsule Validation run `35689452473` / #41 — **SUCCESS**;
- `2c95cb613113d8bcf285f61f708234f19409d744`: full cumulative PA semantic controls + corrected DocSync semantic-harness wiring; Context Capsule Validation run `35689811659` / #48 — **SUCCESS**;
- `022e72fed579a1967b3332fd384cb5209aeaf59a`: protocol/evidence aligned to complete PA-01/02/03 representative surface; Context Capsule Validation run `35689942517` / #51 — **SUCCESS**; Arkus Candidate Validation run `35689942360` / #967 — **SUCCESS**;
- exact pre-report subject `e2f14b7e043c9f53a436cc8e89340c4a80d8707c`: explicit `PROOF_BUDGET`; Context Capsule Validation run `35690030838` / #52 — **SUCCESS**; Arkus Candidate Validation run `35690030840` / #968 — **SUCCESS**.

The final report commit itself still requires a fresh exact-SHA no-write rerun before freeze/Ready.

## 9. Proof budget

Durable verdict: `Docs/evidence/CTX-02/PROOF_BUDGET.md`.

`PROOF_BUDGET: WITHIN_BUDGET`

Why:

- production additions are deterministic source/identity/structure invariants only;
- semantic fixtures are test-only and bounded to the five actual current consumer capsules;
- PA dispositions use one generic canonical-table oracle rather than duplicated prose machinery;
- no production semantic registry, fuzzy/NLP equivalence engine, automatic historical/future fixture generation or theorem-style narrative proof was added;
- no bulk H0 migration was introduced;
- arbitrary future natural-language semantics remain explicit escalation/independent-review territory.

If future closure requires a production prose registry/general language-equivalence engine or automatic enrollment of every capsule into semantic fixtures, CTX-02 must stop/re-scope rather than expand that machinery.

## 10. Residual boundary

CTX-02 still does not prove arbitrary narrative semantics mechanically. That is intentional and documented. A capsule remains `NON_AUTHORITATIVE_NAVIGATION_ONLY`; concrete contradiction, source mismatch, missing required content, non-compressible material, disputed interpretation or material semantic uncertainty causes authoritative-source deepening and independent review.

The representative semantic fixtures themselves are review-visible test evidence, not accepted semantic sources. A future accepted capsule outside the selected representative surface does not become magically semantically proved by passing production shape/source validation.

No product/runtime semantic residual is created by this process-only change.

## 11. Worker verdict before final exact-SHA rerun

No known in-claim semantic, process, source-self-confirmation, cumulative-PA, adoption-wiring or proof-budget blocker remains after the complete baseline→pre-report review.

`WORKER_PRE_REVIEW: CLEAN`

`PROOF_BUDGET: WITHIN_BUDGET`

This report is the final planned repository-byte mutation. The resulting exact SHA must now receive fresh successful Context Capsule Validation + Arkus Candidate Validation with no further writes. Only after those exact-SHA checks pass may the PR be frozen/marked Ready and handed to a **fresh independent Reviewer**. Worker CLEAN is readiness evidence only; it is not Reviewer PASS and authorizes neither merge nor DocSync.
