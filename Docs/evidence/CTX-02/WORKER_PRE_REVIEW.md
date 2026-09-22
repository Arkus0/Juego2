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

Circuit-breaker trust-boundary re-audit remains durable at `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`.

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 24`

`PROOF_BUDGET: WITHIN_BUDGET`

This report is the complete strict Worker pre-review for cycle 4. PR #113 was returned to Draft before every repair mutation. The full baseline→candidate surface, all four Reviewer FAIL classes, the exact-SHA CI feedback produced during this cycle, adoption wiring, live-main movement, representative consumer boundaries and proof proportionality were re-reviewed. Persisting this report is the final planned repository-byte mutation before a new exact-SHA no-write validation and freeze.

## 1. Cycle-4 blocker B1 — omission control now proves a real RED

The old synthetic regression was insufficient: it kept production validation GREEN and merely asserted that an expected ID had been removed.

The repair adds `scripts/context-capsule-omission-controls.py`, wired into the required Context Capsule Validation workflow. It uses the actual indexed `WP-HK-GATE` capsule and the same independent representative semantic oracle used by CTX-02's bounded semantic controls.

Positive omission sequence:

1. validate the actual representative baseline under production validation and the independent oracle;
2. remove exactly one of several exports, `unity-work-authorized`;
3. require production shape/source validation to remain GREEN, proving the mutation is structurally valid rather than incidentally malformed;
4. require the representative oracle to RED on `exported_guarantees material content mismatch`.

Symmetric exclusion sequence removes only `no-new-concurrency-claim` while another exclusion remains, requires production validation to stay GREEN, and requires the same oracle to RED on `exclusions_nonclaims material content mismatch`.

The older synthetic one-of-many mutations remain only mutation-sanity checks and are explicitly labelled as such. They are no longer presented as the semantic RED evidence.

Verdict: **B1 CLOSED**.

## 2. Cycle-4 blocker B2 — COMPLETE workpacks define the PA universe

`validate_pa_chain()` no longer discovers existing `PA-NN.md` result files first.

The canonical chain rule is now:

- `workpack_glob = Docs/workpacks/PA/WP-PA-[0-9][0-9].md`;
- `result_template = Docs/research/living-world/results/PA-{NN}.md`.

The checker first discovers every canonical `WP-PA-NN.md` whose status is `COMPLETE`. Each COMPLETE workpack independently enters the accepted PA inventory. From that inventory the checker derives and requires the canonical `PA-NN.md` result, then requires the matching indexed capsule and structured-disposition binding.

The self-test retains a synthetic `WP-PA-04.md` as COMPLETE while jointly omitting `PA-04.md` and its capsule/index entry. `validate_pa_chain()` must RED with `accepted PA canonical result missing for COMPLETE workpack(s): WP-PA-04`.

Result, capsule and index entry therefore cannot disappear together and erase the item from the universe whose completeness is being claimed.

Verdict: **B2 CLOSED**.

## 3. Exact-SHA CI feedback during repair

After the first cycle-4 pre-review report, candidate `fb9d7265b19d4eebcc0b92b3a091785c5f9f2ef7` was marked Ready solely to trigger exact-candidate CI.

- Arkus Candidate Validation run `35691792743` / #1013: **SUCCESS**.
- Context Capsule Validation run `35691792781` / #63: **FAILURE**.

The failure was not either Reviewer blocker. `context-capsule-check.py --self-test` passed. The failure occurred inside the pre-existing synthetic regression harness because its temporary index still used the superseded result-first fields `result_glob` / `workpack_template`; the repaired production checker correctly rejected that fixture as `invalid PA chain discovery rule`.

The PR was immediately returned to Draft. Finding 24 repaired only that stale synthetic fixture:

- use the same completion-side `workpack_glob` / `result_template` contract as production;
- clarify that the synthetic one-of-many blocks are mutation sanity only;
- keep the actual semantic omission RED in `context-capsule-omission-controls.py`.

No product semantics, representative semantic oracle or production trust boundary changed in this CI-driven repair.

## 4. Prior FAIL regressions preserved

The candidate still protects all prior reviewed defect classes:

- accepted PA capsule cannot omit the complete `disposition_source` + `dispositions` surface;
- disposition rows preserve exact source-derived key/status coverage;
- malformed/blank reopen and escalation elements RED;
- accepted identity requires explicit independent PASS plus review id;
- same-ID/pointer/fingerprint guarantee/exclusion statement inversion REDs through bounded representative semantic oracles;
- representative reopen/escalation substitutions and changed-but-still-distinct PA directional values RED;
- identity/authority sources cannot self-confirm from capsule/CTX-02-generated evidence;
- PA `disposition_source` must be the canonical result derived from the independent COMPLETE-workpack inventory;
- `WP-CITY-03` requires the exact non-compressible `CITY_PRODUCT_SEED.md` mandatory read;
- PA-01/02/03 remain the bounded cumulative PA semantic representative surface.

No earlier causal repair was removed to close cycle 4.

## 5. Scope and predecessor review

Re-reviewed against the exact WP and predecessor boundary:

- `Docs/workpacks/CTX/WP-CTX-02.md` objective, Work, Forbidden, Required controls, Acceptance and DoD;
- accepted CTX-01 predecessor evidence;
- `AGENTS.md`, Context Bootstrap, Worker Review Protocol and role skills;
- Context Capsule protocol/schema/index and all five indexed representative capsules;
- HK-GATE, CITY-03 and PA-01/02/03 accepted source bindings;
- all four independent Reviewer FAILs on their exact candidate SHAs;
- complete baseline→candidate diff and cycle-4 failed-candidate→candidate diff.

The write set remains CTX-02 process/context machinery and evidence only. No product/runtime implementation, Unity project, CITY geometry/result, canonical PA accepted result, accepted predecessor workpack or architecture/product semantic source is modified.

Live `main` remains `13332b738626b43ace6047a9c141a840406bfed7` during this cycle. Its relevant movement concerns CTX-03 and PA-13 scalability surfaces and does not overlap the immutable accepted HK-GATE/CITY-03/PA-01/02/03 source bindings used here.

Scope verdict: **WITHIN WP-CTX-02**. No predecessor reopen or rebase is required.

## 6. Oracle / authority boundary

The repair preserves the durable A/B/C split:

- **A / production:** deterministic accepted identity, source fingerprints, exact mandatory-read identity, COMPLETE-workpack PA inventory, canonical result existence, structured disposition equality and shape invariants;
- **B / bounded representative tests:** exact material semantics for the five selected current consumer capsules, including real one-of-many omission defects;
- **C / human escalation:** arbitrary future prose equivalence/completeness and disputed interpretation.

The PA completeness rule does not auto-enroll future PA prose into semantic fixtures. The omission control reuses the existing test-only representative oracle. No production semantic registry, fuzzy/NLP equivalence engine, bulk historical migration or generalized semantic theorem proving was added.

Durable verdict: `Docs/evidence/CTX-02/PROOF_BUDGET.md` → `PROOF_BUDGET: WITHIN_BUDGET`.

## 7. Required final exact-candidate validation

After this report commit, no repository bytes may change before freeze unless a required check finds another defect. The exact final candidate must execute:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

These commands run in `.github/workflows/context-capsule-validation.yml` against the exact PR candidate checkout. Arkus Candidate Validation must also be GREEN on the same SHA.

## 8. Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`PROOF_BUDGET: WITHIN_BUDGET`

The branch may be marked Ready only to run/reconfirm the exact-candidate gates. Once both required workflows are GREEN on the same immutable SHA, that SHA may be declared the frozen candidate for a fresh independent Reviewer. Worker CLEAN is readiness evidence only; no merge or DocSync is authorized without Reviewer PASS.
