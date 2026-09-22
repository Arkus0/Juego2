# CTX-02 Control Matrix

WP: `WP-CTX-02`  
Circuit-breaker lineage: `fail_cycle: 5`  
Final trust-boundary audit: `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`

CTX-02 uses three oracle classes:

- **A — production/source-derived invariant:** deterministic facts proved from checker-owned repository conventions or independent authoritative bytes;
- **B — representative semantic control:** bounded test-only oracles over the actual selected H1/CITY/PA boundaries;
- **C — human/escalation-only:** arbitrary future natural-language completeness/equivalence, coordinated semantic-oracle changes or contested interpretation; reconstruct authoritative sources and require independent judgment.

`VALID_NAVIGATION_ONLY` requires production audit plus all required independent controls. Neither component grants semantic authority.

| Claim / control | Authoritative universe/source | Independent oracle | Representative mutation | Required result | Current result |
|---|---|---|---|---|---|
| canonical audit input | checker constant `Docs/engineering/context-capsules/index.json` | `audit_index()` exact path | pass alternate index | RED | RED_AUTOMATIC |
| index protocol | checker constant `CONTEXT_CAPSULE_V1.md` | exact equality | redirect protocol path | RED | RED_AUTOMATIC |
| capsule object identity | checker-derived `<capsule_id>.json` | `audit_index()` | redirect entry to alternate valid capsule | RED | RED_AUTOMATIC |
| capsule ID / canonical workpack | checker-derived track root + filename | `canonical_workpack_binding()` + `validate_identity()` | same-name workpack under alternate root | RED | RED_AUTOMATIC |
| track | checker derives from capsule ID | `validate_basic_shape()` | CITY→H1 while deleting CITY mandatory read | RED | RED_AUTOMATIC via real `--audit-index` |
| content mode | checker derives from canonical track | `validate_basic_shape()` | boundary→structured or PA→boundary | RED | RED_AUTOMATIC |
| accepted identity | canonical external workpack completion | `parse_completion()` + exact comparison | mutate reviewed/merge SHA | RED | RED_AUTOMATIC |
| independent PASS | canonical external workpack completion | explicit PASS parser | PASS→FAIL while keeping review ID | RED | RED_AUTOMATIC via real `--audit-index` |
| live accepted state | external accepted-state input / Reviewer | live-state exact state/SHA check | `ACCEPTED`→`REOPENED` or SHA mismatch | RED/reconstruct | RED_AUTOMATIC when supplied; otherwise HUMAN reconstruction |
| source self-confirmation | external repository authority boundary | `require_external_authority_path()` | source points into capsule/CTX-02 layer | RED | RED_AUTOMATIC |
| source bytes | external repository bytes | recomputed Git blob SHA | mutate bound source bytes | RED | RED_AUTOMATIC |
| representative authoritative-source inventory | accepted HK/CITY/PA source set | independent representative oracle | delete HK proof-matrix source only | RED | RED_SEMANTIC_ORACLE |
| mandatory reads | exact named external source where contract requires it | source validator + CITY exact-path rule + rep oracle | substitute CITY seed with another valid source | RED | RED_AUTOMATIC |
| exported guarantees | representative expected material map | independent semantic oracle | delete one of many exports | RED | RED_SEMANTIC_ORACLE |
| guarantee semantic substitution | representative expected material map | same | preserve ID/pointer/fingerprints, invert statement | RED | RED_SEMANTIC_ORACLE |
| exclusions/nonclaims | representative expected material map | independent semantic oracle | delete one of many exclusions | RED | RED_SEMANTIC_ORACLE |
| exclusion semantic substitution | representative expected material map | same | preserve ID/pointer/fingerprints, invert statement | RED | RED_SEMANTIC_ORACLE |
| reopen-condition shape | declared machine shape | production validator | null/blank element | RED | RED_AUTOMATIC via real `--audit-index` |
| reopen-condition semantics | representative expected list | independent semantic oracle | opposite non-empty rule | RED | RED_SEMANTIC_ORACLE |
| escalation shape | declared machine shape | production validator | null/blank element | RED | RED_AUTOMATIC via real `--audit-index` |
| escalation semantics | representative expected list | independent semantic oracle | opposite non-empty rule | RED | RED_SEMANTIC_ORACLE |
| future arbitrary prose completeness | original accepted sources | independent Reviewer | material ambiguity/possible omission not in bounded fixture | never auto-PASS | ESCALATE_HUMAN |
| PA workpack universe | checker constant `Docs/workpacks/PA/WP-PA-[0-9][0-9].md` + COMPLETE status | `validate_pa_chain()` | delete result+capsule+index but retain COMPLETE WP | RED | RED_AUTOMATIC |
| PA workpack selector integrity | checker constant glob | exact equality before discovery | narrow selector excluding 04 | RED | RED_AUTOMATIC via real `--audit-index` |
| PA workpack selector integrity | checker constant glob | exact equality | selector matching nothing | RED | RED_AUTOMATIC via real `--audit-index` |
| PA workpack selector integrity | checker constant glob | exact equality | broaden to `*.md` | RED | RED_AUTOMATIC via real `--audit-index` |
| PA result derivation | checker constant `Docs/research/living-world/results/PA-{NN}.md` | exact equality + existence | redirect result template | RED | RED_AUTOMATIC via real `--audit-index` |
| accepted PA capsule coverage | COMPLETE workpack→canonical result | chain join to canonical capsule ID/path | omit capsule/index entry only | RED | RED_AUTOMATIC |
| PA structured surface | canonical PA result and canonical mode | production validator | remove entire `disposition_source` + `dispositions` | RED | RED_AUTOMATIC |
| PA disposition source | checker-derived canonical PA result | canonical path/fingerprint equality | rebind to PA-02/alternate result | RED | RED_AUTOMATIC |
| PA table selector | checker-owned section/key/status columns | `validate_canonical_pa_selector()` | choose second valid-looking table | RED | RED_AUTOMATIC via real `--audit-index` |
| PA table selector | checker-owned columns | same | switch status column | RED | RED_AUTOMATIC via real `--audit-index` |
| future PA selector availability | reviewed checker-owned selector map | fail-closed selector lookup | new COMPLETE PA ID with no checker selector | no coverage claim | RED_AUTOMATIC / RECONSTRUCT |
| PA disposition completeness | canonical table parsed with checker selector | exact key→status equality | omit material REJECT | RED | RED_AUTOMATIC |
| PA status preservation | same | exact key→status equality | LATER→ADOPT or other reclassification | RED | RED_AUTOMATIC |
| directional structure | declared asymmetric object | production shape/distinctness | collapse A→B/B→A | RED | RED_AUTOMATIC |
| directional semantic value | representative PA expected map | independent semantic oracle | change to different still-distinct direction | RED | RED_SEMANTIC_ORACLE |
| representative H1/CITY inventory | independent test expected set | `assert_representative_inventory()` | remove capsule + index entry together | RED | RED_SEMANTIC_ORACLE |
| workflow command surface | canonical five-command set in independent control code | adoption-wiring exact text checks | guide says “full” but omits omission harness | RED | RED_AUTOMATIC |
| coordinated capsule + semantic-oracle rewrite | original accepted sources | independent Reviewer, not CTX-derived data | change subject and fixture together | do not treat CI as semantic truth | ESCALATE_HUMAN |

`UNSAFE: 0`

## Five Reviewer FAIL regressions preserved

1. `#5274094804` / `8db17036ad65ca03540f14e71711d806179c3e1c`: whole accepted-PA disposition surface deletion REDs.
2. `#5274163927` / `504b4ff25f67720be9adb6b948dff859413419f3`: malformed reopen/escalation and non-PASS identity RED.
3. `#5274257119` / `66ccbeccf699f98c591809c739a3118f1b376e9c`: same-ID semantic substitutions RED through bounded independent oracles.
4. `#5274389905` / `1afc4135e4eda8cfab8e85325dcafcf123978915`: one-of-many omission proves a real semantic-oracle RED; COMPLETE workpacks define future PA inventory.
5. `#5274609459` / `210d42cc41199716e46d38c23ed7a550b8038a1c`: PA workpack/result selectors are checker-owned/exact-bound; DocSync skill contains the full command surface.

## Final self-confirmation boundary

The audited capsule/index cannot define or select the mechanical universe used to prove itself:

- canonical index/capsule/workpack/result roots and naming rules are checker-owned;
- track/content mode derive from canonical identity rather than subject metadata;
- PA workpack glob and result template are constants; index copies are assertions only;
- PA disposition path/table/columns are checker-owned;
- accepted identity comes from the exact canonical external workpack;
- source fingerprints are recomputed only after external-authority constraints;
- representative semantic and source-inventory expectations live in independent test code, not capsule/index data;
- arbitrary semantics that cannot be independently automated remain explicit HUMAN escalation.

## Required full validation surface

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

The same command set is asserted in the protocol, DocSync skill and Context Capsule Validation workflow. Final handoff must bind successful exact-SHA workflow runs to the frozen candidate.
