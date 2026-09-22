# CTX-02 Control Matrix

WP: `WP-CTX-02`

| Control | Oracle / mechanism | Expected defect result |
|---|---|---|
| accepted identity consistency | parse independent `COMPLETE` WP metadata; compare reviewed candidate, merge SHA and PASS review | exact mismatch -> FAIL closed |
| source consistency | recompute Git blob SHA from repository bytes | changed/missing source -> FAIL closed |
| accepted PA-chain completeness | discover canonical `PA-NN.md` results whose matching WP is `COMPLETE`, independent of capsule index | accepted result without capsule -> coverage FAIL |
| PA disposition completeness | parse authoritative Markdown disposition table and compare exact key->status map | missing/extra/reclassified row -> FAIL |
| one material positive guarantee omitted | independent defect-control oracle expects two material export IDs; remove one while one remains | structural capsule remains valid, independent control REDs on missing export |
| one material exclusion omitted | independent defect-control oracle expects two material exclusion IDs; remove inherited exclusion while one remains | structural capsule remains valid, independent control REDs on missing exclusion |
| inherited PA-03 N-hop `REJECT` omitted | authoritative PA-03 disposition parser | loss detected; cannot silently narrow inherited PA-02 bounded discovery |
| `LATER` collapsed/reclassified | authoritative exact status comparison | status change -> FAIL |
| asymmetric semantics collapsed | explicit forward/reverse expressions with `must_remain_distinct=true` | A->B == B->A -> FAIL |
| concrete predecessor reopen | external accepted-state input independent from capsule | `state != ACCEPTED` -> FAIL closed / reconstruct sources |
| stale live accepted SHA | external accepted-state exact reviewed/merge identity | mismatch -> FAIL closed |
| CITY spec compressed away | CITY capsule requires bound `noncompressible=true` source read | missing product seed read -> FAIL |
| capsule authority promotion | exact `authority=NON_AUTHORITATIVE_NAVIGATION_ONLY`; validator emits `semantic_authority_granted=false` | other authority marker -> FAIL |

## Self-confirmation boundary

The capsule/index does not define the universe used to prove all its own completeness claims:

- accepted identity comes from completion metadata and can additionally be checked against external live accepted state;
- source integrity is recomputed from repository bytes;
- PA row coverage is parsed from canonical result tables;
- accepted PA chain coverage is discovered from result/workpack state outside the capsule index;
- the positive/exclusion omission controls use a deliberately separate test oracle.

The independent test oracle is only defect-injection evidence. It is **not** a production semantic registry and does not make its expected IDs authoritative outside the test.

## Required commands

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

These commands run in `.github/workflows/context-capsule-validation.yml` against the exact PR candidate checkout. Final Worker handoff records the exact successful run for the frozen candidate.