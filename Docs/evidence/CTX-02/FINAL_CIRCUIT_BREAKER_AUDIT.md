# CTX-02 Final Circuit-Breaker Audit

Status: **DURABLE FINAL-AUDIT EVIDENCE / PRE-FREEZE**  
PR: `#113`  
WP: `WP-CTX-02`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Trigger reviewed candidate: `210d42cc41199716e46d38c23ed7a550b8038a1c`  
Trigger review: `#5274609459`  
`fail_cycle: 5`

This audit closes the recurring false-green class rather than only the latest reported example. The governing question is:

> Can an audited capsule/index, configuration supplied by it, coordinated absence, semantic substitution or manipulable selection reduce/change what CTX-02 believes it must check and still produce GREEN?

The answer after the repairs and controls below is **no for every mechanical claim CTX-02 makes automatically**. Natural-language completeness that cannot be proved mechanically is explicitly **ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES** rather than silently treated as GREEN.

## 1. Final security invariants

1. An audited artifact never defines the universe whose completeness it proves.
2. An audited artifact never selects or replaces its own mechanical oracle.
3. Deleting object + evidence + index entry together cannot shrink an externally defined expected set.
4. Every canonical completeness universe is rooted in checker-owned convention or an independent external authority.
5. Completeness selectors, canonical roots, filename derivation, track/mode derivation and result templates are checker-owned or exact-bound.
6. Shape-valid but materially altered representative semantics RED through an independent test-only semantic oracle.
7. Every mandatory defect control exercises the actual promised path/oracle; integration defects use the real `--audit-index` CLI.
8. Material absence cannot become implicit “not applicable” unless the external contract permits it.
9. Reviewer/source reconstruction remains available regardless of capsule/index `ACCEPTED` metadata.
10. CTX-02-derived artifacts cannot certify other CTX-02-derived artifacts as predecessor authority.
11. Subject-supplied configuration is either descriptive metadata or exact-compared against checker-owned/external constants.
12. Every automatic claim has an identifiable oracle independent of the field being validated.
13. Canonical capsule/index paths are checker-owned; alternate valid-looking objects cannot be rebound through index metadata.
14. Canonical workpack path, track and content mode are derived from capsule ID, so a capsule cannot disable track-specific validation by relabelling itself.
15. PA disposition table section/key/status-column selectors are checker-owned; a capsule cannot point at another internally coherent table in the same canonical result.
16. A future PA capsule without a reviewed checker-owned disposition selector fails closed to authoritative reconstruction; silence is not coverage.
17. Representative authoritative-source inventories are independently pinned where CTX-02 claims downstream reconstruction from those pointers.
18. Any future coordinated change to capsule prose plus its test-only semantic oracle is not mechanically certified as semantic truth; it requires authoritative-source reconstruction and independent Reviewer judgment.

## 2. Trust-boundary map

| Surface | Producer | Expected universe / authority | Oracle | Disappearance / coordinated omission | Selector/config mutation | Shape-valid semantic mutation | Final class |
|---|---|---|---|---|---|---|---|
| canonical index path | caller/workflow | checker constant `Docs/engineering/context-capsules/index.json` | `audit_index()` exact path equality | alternate index cannot replace canonical audit input | RED | n/a | SAFE |
| index protocol path | index | checker constant `Docs/engineering/CONTEXT_CAPSULE_V1.md` | `audit_index()` exact equality | missing/redirected protocol RED | RED | descriptive prose still non-authoritative | SAFE |
| index entries / capsule paths | index | checker derives `context-capsules/<capsule_id>.json` | `audit_index()` | PA omissions also caught from COMPLETE-workpack universe; representative H1/CITY inventory caught by independent oracle | alternate valid-looking path RED | capsule semantic mutation handled below | SAFE |
| capsule ID | capsule + index | canonical workpack/result naming and representative inventory | index/id equality + canonical binding + PA completion universe / representative oracle | coordinated rename removes expected ID -> PA or representative RED | cannot choose alternate canonical path | n/a | SAFE |
| identity source path | capsule | checker-derived canonical `Docs/workpacks/<track>/<capsule_id>.md` | `validate_identity()` | missing canonical workpack RED | alternate same-name external path RED | workpack itself remains external authority | SAFE |
| track | capsule | checker derives from capsule ID/workpack root | `validate_basic_shape()` | cannot delete CITY obligations by relabelling | relabel RED | n/a | SAFE |
| content mode | capsule | checker derives `PA -> structured_disposition`, others boundary summary | `validate_basic_shape()` | whole structured PA surface cannot become “not applicable” by mode switch | mode switch RED | n/a | SAFE |
| accepted reviewed SHA / merge SHA / review ID | capsule | external canonical workpack completion metadata | `parse_completion()` + `validate_identity()`; optional live accepted-state exact match | missing identity RED | capsule cannot choose identity source | PASS->FAIL / mismatch RED | SAFE; live truth may additionally reopen |
| live state | external caller/live reconstruction | external accepted state | `validate_identity(..., live_state)` / Reviewer process | missing live input means no live claim is made | subject cannot supply via capsule | `REOPENED` or SHA mismatch RED when required | SAFE / HUMAN when live input not supplied |
| authoritative source paths + blob SHAs | capsule | external repository bytes; representative inventory pin for selected boundaries | `validate_sources()` + representative oracle | representative whole-source omission RED; arbitrary future source sufficiency escalates | CTX-02/capsule self-source forbidden | changed bytes RED; arbitrary semantic sufficiency HUMAN | SAFE + HUMAN for future semantic sufficiency |
| mandatory source reads | capsule | exact CITY seed where contract names it; otherwise explicit noncompressible source | `validate_mandatory_reads()` + CITY exact path + representative oracle | CITY omission RED; cannot hide by track change | alternate valid file RED for CITY | reason adequacy HUMAN after non-empty shape check | SAFE / HUMAN reason semantics |
| exported guarantees | capsule | representative test-only oracle for selected H1/CITY/PA; future source judgment | semantic oracle exact ID/statement/pointer | one-of-many and whole representative inventory omissions RED | subject cannot select oracle | same-ID invented/opposite statement RED | RED_SEMANTIC_ORACLE; future prose HUMAN |
| exclusions/nonclaims | capsule | same as guarantees | same independent oracle | omission RED | subject cannot select oracle | same-ID inversion RED | RED_SEMANTIC_ORACLE; future prose HUMAN |
| reopen conditions | capsule | representative oracle + shape; future source judgment | production non-empty shape + semantic oracle | representative deletion/substitution RED | no subject-selected oracle | opposite non-empty text RED | RED_SEMANTIC_ORACLE; future completeness HUMAN |
| escalation conditions | capsule | representative oracle + shape; future source judgment | same | representative deletion/substitution RED | no subject-selected oracle | opposite non-empty text RED | RED_SEMANTIC_ORACLE; future completeness HUMAN |
| PA workpack discovery | index used only as asserted metadata | checker constant `Docs/workpacks/PA/WP-PA-[0-9][0-9].md` | `validate_pa_chain()` | COMPLETE WP remains expected if result/capsule/index disappear | narrow/broad/none selector RED before discovery | n/a | SAFE |
| PA result discovery | index used only as asserted metadata | checker constant `Docs/research/living-world/results/PA-{NN}.md` | `validate_pa_chain()` | missing canonical result for COMPLETE WP RED | alternate template RED | n/a | SAFE |
| PA disposition source path | capsule | checker-derived canonical PA result | `validate_canonical_pa_selector()` + chain source equality | whole surface required | rebind to another result RED | n/a | SAFE |
| PA table section/key/status columns | capsule | checker-owned selector keyed by accepted PA capsule ID | `validate_canonical_pa_selector()` | selector absence for future PA RED/reconstruct | alternate table/columns RED | n/a | SAFE |
| PA dispositions | capsule | canonical result table parsed with checker-owned selector | `validate_dispositions()` exact key/status equality | entire surface or one row omission RED | subject cannot change source/selector to hide it | reclassification RED | SAFE |
| directional/asymmetric semantics | capsule | representative PA oracle for material values | production shape/distinctness + independent PA semantic oracle | material representative omission/mutation RED via oracle | no oracle selection by subject | changed-but-distinct direction RED | SAFE for representative / HUMAN future semantics |
| representative H1/CITY inventory | index | CTX-02 independent test oracle set | `assert_representative_inventory()` | coordinated index+capsule omission RED | index cannot redefine required set | semantic fields handled independently | SAFE |
| workflow command surface | docs/workflow | canonical command set in independent control code | `validate_adoption_wiring()` | omitted harness RED | “full CTX-02 validation surface” guides must contain all commands | n/a | SAFE |
| test-only semantic oracle code | CTX-02/reviewer-reviewed code, not capsule/index | authoritative source reconstruction + independent review | review-visible fixed fixtures | capsule cannot delete oracle through data | subject cannot select different oracle at runtime | coordinated oracle+capsule semantic rewrite requires HUMAN reconstruction | HUMAN-ESCALATION for coordinated oracle rewrite |

## 3. New false-greens found in this final audit

Beyond review `#5274609459`, the Worker found and repaired these same-class defects:

1. **Track-controlled oracle suppression.** `WP-CITY-03` could change `track` from `CITY` to another allowed value and simultaneously remove the mandatory seed read; the prior CITY-specific checker keyed on the subject-provided track. Repair: derive canonical track from capsule ID/workpack root and exact-compare.
2. **Content-mode selector remained subject-controlled.** A subject could attempt to change whether structured-disposition vs boundary-summary rules applied. Repair: mode is derived from canonical track/ID and exact-checked.
3. **PA table selector was self-authored.** `disposition_source.section`, `key_column` and `status_column` selected the very table used as the row-completeness oracle. A valid-looking alternate table in the same canonical result could therefore become self-confirming. Repair: checker-owned selector map; unknown future PA selector fails closed.
4. **Index entry could redirect a capsule ID to an alternate valid-looking file under the capsule root.** Repair: index entry path must equal `Docs/engineering/context-capsules/<capsule_id>.json` exactly.
5. **`--audit-index` could be pointed at an alternate index file.** Repair: audit mode accepts only the canonical checker-owned index path.
6. **Identity source root was not exact.** Basename matching allowed a same-name workpack under another `Docs/workpacks/**` directory. Repair: canonical workpack path is derived exactly from capsule ID.
7. **Representative authoritative-source inventory omission.** Removing one HK proof source could preserve shape/fingerprints for remaining sources and leave semantic statements unchanged. Repair: test-only representative oracle now pins the material authoritative-source inventory.
8. **DocSync command-set drift.** `.agents/skills/update-handoff/SKILL.md` called its list the full CTX-02 surface while omitting `context-capsule-omission-controls.py`. Repair: skill aligned; independent adoption control now exact-checks every command in protocol, workflow and every guide claiming the full surface.

## 4. Class-level mutation matrix

`actual outcome` below is the enforced result of the current checker/control implementation. `RED_AUTOMATIC` means the production checker / real `--audit-index` fails. `RED_SEMANTIC_ORACLE` means production shape/source may remain valid but the independent representative oracle fails. `ESCALATE_HUMAN` is intentional and explicitly non-automatic.

| Claim | Authoritative universe/source | Independent oracle | Mutation | Expected | Actual |
|---|---|---|---|---|---|
| PA chain completeness | checker-owned COMPLETE `WP-PA-NN` discovery | `validate_pa_chain()` | delete result+capsule+index, retain COMPLETE WP | RED | RED_AUTOMATIC |
| PA chain selector authority | checker constant glob | exact equality before discovery | narrow glob to exclude `04` while WP-PA-04 COMPLETE/result absent | RED | RED_AUTOMATIC via real `--audit-index` |
| PA chain selector authority | checker constant glob | exact equality | selector matches nothing | RED | RED_AUTOMATIC via real `--audit-index` |
| PA chain selector authority | checker constant glob | exact equality | broaden to `*.md` | RED | RED_AUTOMATIC via real `--audit-index` |
| PA result derivation | checker result template | exact equality | redirect `result_template` | RED | RED_AUTOMATIC via real `--audit-index` |
| audit input identity | checker canonical index path | `audit_index()` | pass alternate index file | RED | RED_AUTOMATIC via real `--audit-index` |
| index object identity | checker canonical capsule path | `audit_index()` | point entry to alternate valid-looking capsule | RED | RED_AUTOMATIC via real `--audit-index` |
| identity root | checker canonical workpack path | `validate_identity()` | same-name workpack under alternate directory | RED | RED_AUTOMATIC via real `--audit-index` |
| CITY seed binding | canonical ID->track + exact seed path | `validate_basic_shape()` + CITY validator | change track to H1 + delete mandatory read | RED | RED_AUTOMATIC via real `--audit-index` |
| content-mode rule | canonical ID->mode | `validate_basic_shape()` | CITY boundary -> structured mode | RED | RED_AUTOMATIC via real `--audit-index` |
| PA table oracle identity | checker-owned section/columns | `validate_canonical_pa_selector()` | point section at second valid-looking same-status table | RED | RED_AUTOMATIC via real `--audit-index` |
| PA table oracle identity | checker-owned section/columns | same | change status column | RED | RED_AUTOMATIC via real `--audit-index` |
| PA structured surface | canonical PA result + selector | exact source table map | remove entire `disposition_source`+`dispositions` | RED | RED_AUTOMATIC |
| PA disposition row completeness | canonical result table | exact key/status map | delete one `REJECT` | RED | RED_AUTOMATIC |
| PA disposition classification | canonical result table | exact key/status map | `LATER` -> `ADOPT` | RED | RED_AUTOMATIC |
| accepted identity | canonical external workpack | completion parser | PASS -> FAIL while fingerprint updated | RED | RED_AUTOMATIC via real `--audit-index` |
| source bytes | external repository file | recomputed Git blob SHA | mutate source bytes without updating binding | RED | RED_AUTOMATIC |
| authority self-confirmation | external-source rule | path boundary | redirect source into capsule/CTX-02 generated layer | RED | RED_AUTOMATIC |
| representative source inventory | external accepted HK/CITY/PA sources | test-only representative oracle | remove HK proof matrix source only | RED | RED_SEMANTIC_ORACLE |
| material guarantee omission | representative expected map | independent semantic oracle | delete one of many exports | RED | RED_SEMANTIC_ORACLE |
| material exclusion omission | representative expected map | independent semantic oracle | delete one of many exclusions | RED | RED_SEMANTIC_ORACLE |
| same-ID semantic substitution | representative expected map | independent semantic oracle | preserve ID/pointer/fingerprint, invert statement | RED | RED_SEMANTIC_ORACLE |
| reopen/escalation semantics | representative expected list | independent semantic oracle | replace with opposite non-empty string | RED | RED_SEMANTIC_ORACLE |
| directional semantics | representative PA expected map | production shape + semantic oracle | A→B/B→A collapse | RED | RED_AUTOMATIC |
| directional semantic value | representative PA expected map | independent semantic oracle | change A→B to different still-distinct expression | RED | RED_SEMANTIC_ORACLE |
| live acceptance | external accepted state | live-state checker/Reviewer | `ACCEPTED` -> `REOPENED` | RED/reconstruct | RED_AUTOMATIC when live state supplied; otherwise Reviewer reconstruction |
| future arbitrary prose completeness | original accepted sources | independent Reviewer | coordinated semantic omission not represented by bounded fixture | do not auto-PASS | ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES |
| coordinated capsule + semantic-oracle rewrite | original accepted sources | independent Reviewer | change subject and test fixture together | do not treat CI as semantic authority | ESCALATE_HUMAN / authoritative reconstruction |
| workflow surface | canonical command set | adoption-wiring control | omit omission harness from guide claiming “full” | RED | RED_AUTOMATIC |

No mutation remains `UNSAFE`.

## 5. Reviewer pre-mortem attacks

The Worker then acted as a hostile pre-mortem only (not as the independent Reviewer) and investigated these distinct next-FAIL hypotheses against the implementation:

1. **Narrow the PA workpack selector after deleting a future accepted item.** Closed: exact checker-owned glob; real CLI RED.
2. **Redirect the PA result template to a valid shadow result tree.** Closed: exact checker-owned template; real CLI RED.
3. **Point `--audit-index` at a friendlier alternate index.** Closed: canonical index path exact-bound.
4. **Keep the same index ID but redirect its path to another valid capsule file.** Closed: canonical `<capsule_id>.json` path exact-bound.
5. **Relabel CITY as H1 and delete `CITY_PRODUCT_SEED.md` mandatory read.** Closed: ID-derived track; real CLI RED.
6. **Switch `content_mode` to disable a track-specific validator.** Closed: ID-derived mode; real CLI RED.
7. **Point PA dispositions at another table/column set in the same canonical result while keeping rows internally coherent.** Closed: checker-owned section/key/status selector; real CLI RED.
8. **Use a same-name workpack under another directory as identity authority.** Closed: exact canonical workpack path.
9. **Delete one HK proof source but retain every guarantee ID/statement/pointer and valid remaining fingerprints.** Closed: representative source-inventory semantic oracle RED.
10. **Delete a representative index entry and its capsule together.** Closed: PA by COMPLETE-workpack universe; H1/CITY by independent representative inventory oracle.
11. **Spoof acceptance with review ID but FAIL verdict.** Closed: explicit PASS parser; real CLI RED.
12. **Mutate reopen/escalation to non-empty opposite prose.** Closed for representative surfaces by independent semantic oracle; future arbitrary prose explicitly HUMAN, never auto-PASS.
13. **Change one PA direction to another still-asymmetric expression.** Closed for representative PA by semantic oracle; collapse also production RED.
14. **Make a required defect test prove only that mutation happened, not that the system failed.** Closed: class-level integration controls invoke subprocess CLI `--audit-index`; omission semantics invoke the same independent representative oracle used by the validation surface.
15. **Create a future COMPLETE PA capsule with no checker-owned disposition selector.** Closed fail-closed: no selector => `RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`, never chain COMPLETE.
16. **Drift a role guide’s “full CTX-02 validation surface”.** Closed: adoption-wiring control checks exact canonical command set wherever that claim appears.
17. **Change capsule prose and its test-only oracle together.** Not mechanically decidable without creating a semantic source of truth; explicitly classified HUMAN. Protocol requires original-source reconstruction and independent review rather than CI self-certification.

Result: **0 plausible in-claim UNSAFE paths remain**.

## 6. Scope discipline

The repair changes only CTX-02 process/context validation, tests, role wiring and evidence. It does not change product/runtime contracts, canonical CITY product data, accepted PA results, Unity/runtime code, or accepted predecessor semantics. No bulk migration is introduced. Capsules remain navigation only.

Where automatic proof is unsafe (arbitrary future natural-language completeness or coordinated semantic-oracle rewrite), the result is explicit `ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`, not a weakened claim.

## 7. Exit gate before freeze

A new freeze is permitted only after all of the following are true on the final immutable SHA:

- full validation surface GREEN;
- baseline real `--audit-index` GREEN;
- all historical defect controls still exercise RED;
- all class-level selector/oracle/configuration controls exercise RED;
- Arkus Candidate Validation exact-SHA GREEN;
- complete Worker pre-review rerun from zero against full baseline→candidate diff;
- durable `WORKER_PRE_REVIEW: CLEAN` bound to the final candidate;
- PR body advances to `fail_cycle: 5`, `FROZEN_FOR_REVIEW`, exact Frozen candidate SHA, `Branch frozen: YES`;
- fresh durable `REVIEW_READY` targets that exact SHA.

No merge or DocSync is authorized by this audit.
