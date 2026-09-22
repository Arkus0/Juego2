# CTX-02 Worker Pre-Review

WP: `WP-CTX-02`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
`fail_cycle: 4`

Historical independently reviewed candidates and FAILs:

- `8db17036ad65ca03540f14e71711d806179c3e1c` -> review `#5274094804`: **FAIL** — accepted-PA structured-disposition whole-surface omission;
- `504b4ff25f67720be9adb6b948dff859413419f3` -> review `#5274163927`: **FAIL** — malformed reopen/escalation items plus accepted identity not requiring explicit PASS;
- `66ccbeccf699f98c591809c739a3118f1b376e9c` -> review `#5274257119`: **FAIL** — representative semantic-content false-green with IDs/source pointers/fingerprints preserved while guarantee/exclusion statement text is inverted or invented;
- `1afc4135e4eda8cfab8e85325dcafcf123978915` -> review `#5274389905`: **FAIL** — one-of-many omission control proved only that a row was deleted, not a real semantic RED; PA-chain universe was seeded from result files, allowing future COMPLETE result+capsule whole-item omission to disappear from coverage.

Circuit-breaker re-audit remains authoritative: `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`, first persisted at `0b676eae0b4b0977113c96eb5b4e4d3aa626af29`.

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 23`

`PROOF_BUDGET: WITHIN_BUDGET`

This report records the complete strict Worker pre-review after cycle 4. PR #113 was returned to Draft before repository mutation. The full baseline→candidate surface, all four historical Reviewer FAIL classes, cycle-4 causal closure, adoption wiring, live-main movement, representative consumer boundaries and proof proportionality were re-reviewed. Persisting this report is the final planned repository-byte mutation before exact-SHA no-write CI validation and freeze.

Pre-report subject reviewed: `dc202c9cda8baf3dc711c715dd8d2795ef2d209b`.

## 1. Cycle-4 blocker closure

### B1 — one-of-many omission now proves a real RED

The old synthetic regression was insufficient: it kept production validation GREEN and merely asserted that an expected ID was absent.

The repair adds `scripts/context-capsule-omission-controls.py`, wired into the required GitHub Actions validation surface. It uses the actual indexed `WP-HK-GATE` capsule and the same independent representative semantic oracle as the main CTX-02 controls.

Causal sequence for exported guarantees:

1. validate the real baseline capsule under production validation and the representative oracle;
2. remove exactly one of several exports: `unity-work-authorized`;
3. require production shape/source validation to remain GREEN, proving the mutation is not caught incidentally by a structural error;
4. require the independent representative oracle to RED with `exported_guarantees material content mismatch`.

The symmetric exclusion control removes only `no-new-concurrency-claim` while another exclusion remains, keeps production validation GREEN, and requires `exclusions_nonclaims material content mismatch` from the same oracle.

Verdict: the mandatory omission control now proves CTX-02 detects semantic narrowing; it no longer treats “the mutation removed an ID” as detection evidence.

### B2 — COMPLETE workpacks now define the future PA universe

`validate_pa_chain()` no longer globs result files and asks whether their workpacks happen to be COMPLETE.

The index now declares:

- `workpack_glob = Docs/workpacks/PA/WP-PA-[0-9][0-9].md`;
- `result_template = Docs/research/living-world/results/PA-{NN}.md`.

The checker first discovers canonical `WP-PA-NN.md` files and selects every `Status: COMPLETE` workpack. Each selected workpack independently requires its canonical `PA-NN.md` result to exist; only then may matching capsule/index/disposition coverage be checked.

The self-test retains `WP-PA-04.md` as COMPLETE while jointly omitting `PA-04.md` and its capsule/index entry. `validate_pa_chain()` must RED with `accepted PA canonical result missing for COMPLETE workpack(s): WP-PA-04`.

Verdict: result+capsule+index can no longer disappear together and erase themselves from the completeness universe.

## 2. Prior FAIL regressions preserved

Cycle-4 code retains the previous causal closures:

- accepted PA capsule cannot omit the complete `disposition_source` + `dispositions` surface;
- disposition rows remain exact source-derived key/status coverage;
- malformed/blank reopen and escalation elements RED;
- accepted identity requires explicit independent PASS plus review id;
- same-ID/pointer/fingerprint guarantee/exclusion statement inversion REDs through bounded representative semantic oracles;
- representative reopen/escalation substitutions and changed-but-still-distinct PA directional values RED;
- identity/authority sources cannot self-confirm from capsule/CTX-02-generated evidence;
- accepted PA disposition source must be the canonical result derived from the independent chain inventory;
- `WP-CITY-03` requires the exact non-compressible `CITY_PRODUCT_SEED.md` mandatory read;
- PA-01/02/03 remain the bounded cumulative PA semantic representative surface.

No prior repair was removed to close cycle 4.

## 3. Contract / scope review

Re-reviewed:

- `Docs/workpacks/CTX/WP-CTX-02.md` objective, Work, Forbidden, Required controls, Acceptance and DoD;
- accepted CTX-01 predecessor evidence;
- `AGENTS.md`, Context Bootstrap, Worker Review Protocol and role skills;
- Context Capsule protocol/schema/index and all five indexed representative capsules;
- HK-GATE, CITY-03 and PA-01/02/03 accepted source bindings;
- all four independent FAIL reviews on their exact candidate SHAs;
- complete baseline→pre-report diff and cycle-4 failed-candidate→pre-report diff.

Baseline `f7b4f1e8247dfc927203dca6754b71aaa53938f3` → pre-report `dc202c9cda8baf3dc711c715dd8d2795ef2d209b` is 72 commits ahead, 0 behind, across 28 CTX-02 process/context paths. The cycle-4 delta from failed candidate `1afc4135e4eda8cfab8e85325dcafcf123978915` is 9 commits and only 9 paths: checker, index, workflow, protocol, four CTX-02 evidence files, and the new omission-control harness.

No product/runtime implementation, Unity project, CITY geometry/result, canonical PA accepted result, accepted predecessor workpack, or architecture/product semantic source is modified.

Scope verdict: **WITHIN WP-CTX-02**.

## 4. Live-main movement

Live `main` remains `13332b738626b43ace6047a9c141a840406bfed7` during this pre-review. Its baseline movement concerns CTX-03 and PA-13 scalability evidence/workpack surfaces already reviewed in the previous cycle. It does not overlap CTX-02's write set or the immutable accepted HK-GATE/CITY-03/PA-01/02/03 source bindings used by the representative controls.

No predecessor reopen/rebase is required.

## 5. Oracle / authority boundary

The cycle-4 repair preserves the durable three-way split:

- **A / production:** deterministic identity, source fingerprints, exact mandatory-read identity, completion-side PA inventory, canonical result existence, structured disposition equality and shape invariants;
- **B / bounded representative tests:** exact material semantics for the five selected current consumer capsules, including real one-of-many omission defects;
- **C / human escalation:** arbitrary future prose equivalence/completeness and disputed interpretation.

The new PA completeness rule does not auto-enroll future PA prose into semantic fixtures. The new omission control reuses the existing test oracle. No production semantic registry, fuzzy/NLP equivalence engine, bulk historical migration or generalized theorem proving was added.

`PROOF_BUDGET: WITHIN_BUDGET` remains justified by `Docs/evidence/CTX-02/PROOF_BUDGET.md`.

## 6. Required exact-candidate validation

After this report commit, no repository bytes may change before freeze unless a required check finds a defect. The exact final candidate must run:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

These commands are wired into `.github/workflows/context-capsule-validation.yml` against the exact PR candidate checkout. Arkus Candidate Validation must also be GREEN on the same SHA.

## 7. Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`PROOF_BUDGET: WITHIN_BUDGET`

The candidate is ready for an exact-SHA no-write CI rerun. Only after those checks are GREEN may the PR be marked Ready and the resulting SHA declared the new frozen candidate for a fresh independent Reviewer. No merge or DocSync is authorized by this Worker pre-review.
