# CTX-02 Control Matrix

WP: `WP-CTX-02`  
Circuit-breaker lineage: `fail_cycle: 3`  
Trust-boundary design: `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`

CTX-02 intentionally uses three oracle classes:

- **A — production/source-derived invariant:** deterministic facts the production checker can prove from canonical repository structure/bytes;
- **B — representative semantic control:** a small independent **test-only** oracle over the actual indexed H1/CITY/PA representative capsules;
- **C — human/escalation-only:** arbitrary natural-language completeness/equivalence or contested interpretation; open authoritative sources and require independent judgment.

`VALID_NAVIGATION_ONLY` requires the production audit plus the independent controls. Neither component grants semantic authority.

| Control | Class | Oracle / mechanism | Expected defect result |
|---|---:|---|---|
| accepted identity consistency | A | parse matching external `Docs/workpacks/**/<capsule_id>.md`; compare reviewed candidate, merge SHA and review id | exact mismatch -> FAIL closed |
| independent PASS verdict | A | completion parser requires explicit `PASS`; mutate PASS→FAIL while rebinding a valid fingerprint | review id alone cannot certify acceptance -> FAIL |
| identity-source self-confirmation | A | identity source must be the matching external workpack, never capsule/CTX-02 generated evidence | redirected self-authored identity source -> FAIL |
| minimum declared capsule shape | A | production checker enforces required mode/identity/mandatory-read list and material field types | missing/malformed required shape -> FAIL |
| reopen/escalation element shape | A | inject `null`, empty and whitespace-only elements through production `--audit-index` | unusable trigger/condition -> FAIL |
| authoritative source consistency | A | external-authority path constraint + recomputed Git blob SHA | self-authored authority, missing bytes or changed bytes -> FAIL |
| representative guarantee material content | B | exact actual representative `id -> (statement, source_pointer)` oracle for HK/CITY/PA | omission, reclassification or invented substitution -> independent RED |
| same-ID guarantee inversion | B | mutate HK `h0-authoring-readiness.statement` only; preserve ID, pointer and all fingerprints | production shape/source audit stays structurally valid; representative oracle REDs |
| representative exclusion/non-claim material content | B | exact actual representative `id -> (statement, source_pointer)` oracle | omission or semantic substitution -> independent RED |
| same-ID exclusion inversion | B | mutate CITY `not-spatial-spec.statement` only; preserve ID, pointer and fingerprints | representative oracle REDs symmetrically |
| representative reopen semantics | B | exact actual representative condition list | non-empty opposite/invented condition -> representative RED |
| representative escalation semantics | B | exact actual representative escalation list | non-empty opposite/invented trigger -> representative RED |
| arbitrary future prose equivalence/completeness | C | no generic semantic proof; Reviewer/source escalation on material ambiguity/contradiction | cannot be certified from capsule; reconstruct sources |
| accepted PA-chain completeness | A | discover canonical `PA-NN.md` results whose matching WP is `COMPLETE`, independent of capsule index | accepted result without capsule -> coverage FAIL |
| accepted PA structured surface | A | every independently discovered accepted PA result must have structured disposition mode/source/non-empty rows | whole surface omission -> FAIL before chain COMPLETE |
| PA disposition source identity | A | `disposition_source.path` + fingerprint must equal independently discovered canonical accepted `PA-NN.md` binding | coherent table rebound to another result -> FAIL |
| PA disposition completeness | A | parse canonical authoritative Markdown table and compare exact key→status map | missing/extra/reclassified row -> FAIL |
| inherited PA-03 N-hop `REJECT` omitted | A | canonical PA-03 disposition parser | inherited bounded-discovery loss -> FAIL |
| `LATER` collapsed/reclassified | A | exact canonical status comparison | status change -> FAIL |
| directional structure | A | unique non-empty IDs, explicit forward/reverse, `must_remain_distinct=true`, forward != reverse | malformed/collapsed pair -> FAIL |
| representative directional semantics | B | PA-03 exact `id -> (forward, reverse)` test oracle | changed-but-still-distinct invented direction -> independent RED |
| concrete predecessor reopen | A+C | external accepted-state input plus independent Reviewer authority | `state != ACCEPTED` -> FAIL closed / source reconstruction |
| stale live accepted SHA | A | external accepted-state exact reviewed/merge identity | mismatch -> FAIL closed |
| CITY mandatory-read identity | A | representative `WP-CITY-03` must bind exact `Docs/production/CITY_PRODUCT_SEED.md`, fingerprinted and `noncompressible=true` | missing or substituted arbitrary valid read -> FAIL |
| mandatory-read reason semantics | A+C | production requires non-empty reason; adequacy is review judgment | malformed reason -> FAIL; questionable reason -> source/review escalation |
| capsule authority promotion | A | exact `NON_AUTHORITATIVE_NAVIGATION_ONLY`; output keeps `semantic_authority_granted=false` | other authority marker -> FAIL |
| adoption wiring | A | bootstrap profiles/protocol/skills independently checked | lost discoverability -> FAIL |

## Three Reviewer FAIL regressions preserved

1. review `#5274094804`, candidate `8db17036ad65ca03540f14e71711d806179c3e1c`: whole accepted-PA disposition surface deletion must RED;
2. review `#5274163927`, candidate `504b4ff25f67720be9adb6b948dff859413419f3`: malformed reopen/escalation entries and non-PASS accepted identity must RED;
3. review `#5274257119`, candidate `66ccbeccf699f98c591809c739a3118f1b376e9c`: actual representative semantic content must RED on same-ID guarantee/exclusion statement inversion/invention.

The cycle remains `fail_cycle: 3`; prior candidates are historical evidence, not erased or reset.

## Self-confirmation boundary

The capsule/index does not define the universe used to prove its own material claims:

- accepted identity comes from the matching canonical external workpack and can additionally be checked against external live accepted state;
- source integrity is recomputed from repository bytes only after sources are constrained outside capsule/CTX-02-generated authority;
- accepted PA-chain coverage is discovered from canonical result/workpack state outside the capsule index;
- PA row coverage is parsed from the independently discovered canonical PA result, and the disposition source must be that exact result;
- the representative semantic oracle is code in the independent control harness, not data read from the capsule/index;
- `WP-CITY-03` names one exact mandatory non-compressible product seed, rather than accepting any self-selected mandatory read.

## Semantic-control boundary

The representative oracle pins material content for the **selected actual CTX-02 examples**: `WP-HK-GATE`, `WP-CITY-03`, and complex `WP-PA-03`. It covers exports, exclusions, reopen conditions, escalation triggers and PA-03 directional expressions.

It is intentionally test-only. It is not imported by `context-capsule-check.py`, is not an accepted semantic source, and is not a requirement to create one prose registry entry for every future capsule. Future material semantic uncertainty remains class C: open authoritative sources and use independent Reviewer judgment.

## Required commands

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

These commands run in `.github/workflows/context-capsule-validation.yml` against the exact PR candidate checkout. Final Worker handoff records the exact successful run for the frozen candidate.
