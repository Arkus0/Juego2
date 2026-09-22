# CTX-02 Final Circuit-Breaker Trust Audit

Status: **ACTIVE CIRCUIT-BREAKER EVIDENCE — INVARIANTS FROZEN BEFORE CYCLE-5 CODE REPAIR**  
PR: `#113`  
Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Trigger candidate: `210d42cc41199716e46d38c23ed7a550b8038a1c`  
Trigger review: `#5274609459`  
`fail_cycle: 5`

This document is the cycle-5 design authority. It is intentionally persisted before the code repair. It closes a **class** of false-greens, not only the latest `workpack_glob` example. No freeze, merge or DocSync is authorized by this audit.

## 1. Circuit-breaker question

The audit asks one question for every CTX-02 mechanical claim:

> Can an audited artefact, configuration supplied by it, coordinated absence, semantic substitution, alternate source, or selectable oracle reduce/change what the validator believes it must prove and still produce GREEN?

If yes, the surface is `BUG` until repaired. If a safe automatic oracle cannot be owned outside the audited artefact, the only allowed result is explicit `ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`, never implicit “not applicable”.

## 2. Safety invariants — frozen before repair

1. **No self-defined completeness universe.** An audited capsule/index never defines the inventory whose completeness it proves.
2. **No self-selected oracle.** An audited artefact never selects/replaces the validator, source table, column mapping, canonical object path or semantic oracle used to validate it.
3. **Paired absence cannot narrow expectation.** Deleting object + evidence + index entry cannot silently remove an item that an independent canonical source says should exist.
4. **Canonical roots are checker/external-owned.** Any root/convention needed to complete a universe comes from checker constants or a source independently authoritative for that claim.
5. **Selectors are not free metadata.** Completeness globs, result templates, canonical roots, ID→filename rules, track/mode dispatch and structural parsing selectors are checker-owned or must equal checker-owned constants exactly.
6. **Shape-valid semantic substitution is not semantic proof.** Where CTX-02 claims representative semantic preservation, same-ID/same-pointer/same-fingerprint material substitution must RED through an independent test-only oracle.
7. **Defect controls exercise the promised path.** A control that claims a defect is detected must invoke the causal production/semantic oracle that fails; proving only that a mutation occurred is insufficient.
8. **Absence is never implicit N/A.** Missing material fields/surfaces/items are RED or explicit human escalation unless an external contract expressly makes them optional.
9. **Reviewer reopening stays external.** The Reviewer can reconstruct/reopen from live GitHub + authoritative sources even when capsule/index says `ACCEPTED`.
10. **No derived-on-derived self-confirmation.** CTX-02 protocol/evidence/index/capsules cannot be used as predecessor authority to validate another CTX-02 derivative.
11. **Audited configuration is descriptive only unless exactly bound.** Any config value that can alter coverage/oracle behavior must equal a checker-owned/external contract; otherwise RED.
12. **Every automatic claim names an independent oracle.** “Covered by validator” is not evidence unless the function and independent expected source are identified.
13. **Canonical identity path is independently derived.** `capsule_id` determines its canonical workpack root/path; the capsule cannot choose another same-named workpack.
14. **Canonical capsule path is independently derived.** An index entry for `WP-X` must point to the checker-derived `context-capsules/WP-X.json`; an alternate valid-looking JSON cannot replace it.
15. **Track/mode cannot select validation away.** Track and v1 content mode are derived from canonical ID family, not trusted from capsule fields.
16. **PA structural extraction is an oracle.** `section`, `key_column` and `status_column` are checker-owned for accepted PA-01/02/03. A future COMPLETE PA with no checker-owned extraction spec escalates/reconstructs instead of self-selecting a parser.
17. **Representative source inventory is independent.** Current HK/CITY/PA representative authoritative source identities are checker/test-owned; an audited capsule cannot silently drop a proof/source and still claim the same navigation contract.
18. **Live-state JSON is comparison input, not provenance authority.** The checker may compare an independently supplied live-state assertion, but authenticity/live GitHub provenance remains external to repository JSON and Reviewer process.
19. **Unknown future semantics fail safely.** CTX-02 does not manufacture a production prose registry; semantic completeness beyond bounded fixtures remains human/source reconstruction.
20. **One canonical full command surface.** Any role/workflow claiming “full CTX-02 validation” must execute the same required command set, including all independent controls.

## 3. Oracle classes

- **A — `RED_AUTOMATIC`:** checker-owned deterministic invariant or source-derived comparison.
- **B — `RED_SEMANTIC_ORACLE`:** independent test-only fixture over the bounded actual representative surface.
- **C — `ESCALATE_HUMAN`:** generic natural-language completeness, live GitHub provenance or future structure for which CTX-02 has no safe automatic oracle.
- **BUG / `UNSAFE`:** a mutation can currently remain GREEN inside a CTX-02 claim. There must be zero before freeze.

## 4. Complete trust-boundary map

| Surface | Supplied by | Independent expected source / oracle | On delete / paired delete | On selector/config mutation | Shape-valid semantic mutation | Pre-repair classification |
|---|---|---|---|---|---|---|
| index schema/authority | index | checker constants | RED | RED | n/a | SAFE/A |
| index protocol path | index | canonical protocol path should be checker-owned | currently may redirect | currently may redirect | n/a | **BUG** |
| index entries inventory | index | bounded representative oracle + PA COMPLETE workpacks | PA protected; HK/CITY pair deletion detected only by independent rep controls | index can choose path | n/a | **BUG class: path selection** |
| capsule ID | capsule + index | index↔capsule equality; canonical ID grammar | duplicate/mismatch RED | ID currently helps choose track/path but fields remain self-controlled | n/a | PARTIAL |
| capsule canonical path | index | checker-derived `{capsule_id}.json` | current paired deletion can evade production for non-PA | **alternate valid-looking path currently accepted** | n/a | **BUG** |
| identity source path | capsule | matching canonical external workpack | missing RED | only basename/root-family-prefix loosely checked; same-name wrong root possible | external completion semantics | **BUG** |
| source blob SHA | capsule | recomputed Git blob SHA from external source bytes | missing source RED | source redirection constrained away from CTX-02 generated layer | coordinated mutation of authoritative bytes is external accepted-source/review concern | SAFE/A + C for source legitimacy |
| accepted reviewed SHA / merge SHA / review ID | capsule | parsed matching canonical COMPLETE workpack; optional independently sourced live-state comparison | missing/mismatch RED | workpack path must become canonical | workpack/live GitHub legitimacy is external | SAFE/A once path fixed; C provenance |
| PASS identity | completion source | parser requires explicit PASS + review id | RED | n/a | spoofing accepted source itself is outside capsule trust boundary and Reviewer/live-state concern | SAFE/A+C |
| live state | external caller | authenticated live GitHub/independent adapter, not repo JSON itself | absence means live confirmation not claimed; Reviewer still queries live state | caller-selected JSON cannot become authority | fake assertion cannot prove GitHub | HUMAN/C; must be documented honestly |
| track | capsule | canonical ID family (`HK/H1/CITY/PA/CTX`) | n/a | **currently capsule-controlled and selects track-specific validator** | n/a | **BUG** |
| content mode | capsule | v1 family contract | missing/malformed RED | currently self-selected except PA/CITY secondary checks | n/a | **BUG class / close checker-owned** |
| mandatory reads | capsule | exact source + CITY checker-owned seed identity | list shape RED only for missing field; CITY empty RED | **CITY check currently depends on self-controlled `track`** | reason semantics human | **BUG via track hiding** |
| exported guarantees IDs/statements | capsule | bounded independent representative semantic oracle; generic future semantics human | empty whole list RED; one-of-many current reps B RED | n/a | same-ID substitutions current reps B RED | SAFE A+B+C |
| exclusions/nonclaims | capsule | same split as guarantees | whole list RED; one-of-many current reps B RED | n/a | same-ID substitutions current reps B RED | SAFE A+B+C |
| reopen conditions | capsule | shape checker + bounded semantic fixtures; generic completeness human | whole list/invalid element RED; representative omission/substitution B | n/a | opposite nonempty text B RED | SAFE A+B+C |
| escalation conditions | capsule | same | same | n/a | opposite nonempty text B RED | SAFE A+B+C |
| authoritative source inventory | capsule | external paths/fingerprints; current representative provenance should be independently pinned | nonempty only; can silently drop one valid source | can choose different valid external source | source sufficiency affects navigation | **BUG for current representative provenance** |
| structured dispositions | capsule | canonical accepted PA result table | whole surface historical fix RED; row omission RED | canonical result path checked in chain | exact status/key comparison | SAFE except extraction selector bug |
| disposition source path | capsule | completion-derived canonical PA result | missing RED | alternate result path chain RED | n/a | SAFE/A |
| disposition section / key column / status column | capsule | should be checker-owned reviewed structural spec | malformed may RED | **currently capsule chooses table/columns used as oracle** | syntactically valid alternate table can self-confirm | **BUG** |
| directional/asymmetric structure | capsule | shape/distinctness checker | optional whole surface is caught for PA-03 by representative oracle | n/a | invented-but-distinct values B RED | SAFE A+B |
| CITY product seed binding | capsule | exact `Docs/production/CITY_PRODUCT_SEED.md` checker constant + blob | missing RED when CITY validator runs | **track mutation can currently avoid validator** | capsule cannot replace seed semantics | **BUG via track hiding** |
| PA workpack discovery | index selector + repo | COMPLETE canonical `WP-PA-NN.md` workpacks | completion-side historical fix catches result/capsule/index paired omission | **`workpack_glob` currently supplied by audited index** | n/a | **BUG — latest Reviewer** |
| PA accepted-result derivation | index template + repo | canonical `PA-{NN}.md` should be checker-owned | missing result RED only after derivation | **`result_template` currently index-controlled** | n/a | **BUG — latest Reviewer** |
| PA capsule discovery | index | COMPLETE workpack + canonical result define expected ID | missing capsule RED | entry path currently redirectable | n/a | **BUG path selection** |
| representative semantic oracle | test code | code-reviewed constants outside capsule/index | missing representative should RED controls | capsule cannot select fixture | material substitutions RED | SAFE/B |
| adoption/role wiring | docs/profiles/skills | independent wiring control | missing role path RED | current control checks discovery but not exact full command set everywhere | prose meaning human | PARTIAL; command drift **BUG** |
| workflow command surface | workflow | canonical command set should be independently asserted | omitted control currently possible | latest update-handoff already drifted | n/a | **BUG** |
| self-tests | checker code | explicit expected failures | test omission visible in code review | helper-only tests insufficient for integration | n/a | SAFE only when integration controls use real CLI |
| independent controls | separate test scripts | bounded oracles / real CLI | omission must break workflow/wiring control | control list must be exact | semantic controls independent of capsule | PARTIAL until full-surface equality control |
| omission controls | independent script | actual indexed HK representative + semantic oracle | one-of-many RED proven | n/a | n/a | SAFE/B |
| `--audit-index` path | CLI caller | canonical index path should be checker-owned | missing RED | **caller can currently choose alternate index** | n/a | **BUG** |

## 5. New false-greens discovered by this final audit

These are in addition to Reviewer `#5274609459`:

1. **Track-assisted oracle hiding:** change `WP-CITY-03.track` from `CITY` to `H1` and remove mandatory seed reads; track-specific CITY validation is skipped.
2. **Self-selected PA table oracle:** keep canonical PA result bytes but change `disposition_source.section` / columns to another valid-looking table and align `dispositions`; the subject selects what table defines completeness.
3. **Alternate capsule rebinding:** change an index entry path to another valid JSON under the capsule root with the same `capsule_id`; canonical object identity is not enforced.
4. **Same-name identity-root rebinding:** the identity source check constrains `Docs/workpacks/**/<cid>.md` but does not derive the exact family directory from ID.
5. **Representative source-inventory narrowing:** current validation proves every listed source exists/fingerprints, but the audited capsule can remove one source from its own list and thereby shrink what is checked.
6. **Audit-index substitution:** CLI accepts an alternate index path; a caller can validate a different inventory while claiming index audit.
7. **Validation-surface drift:** the workflow/protocol/control matrix include omission controls, while `update-handoff` claims “full” validation without them.

All seven are repair scope for cycle 5. No claim will be reduced to obtain PASS.

## 6. Repair boundary authorized by this audit

Allowed repair is deliberately structural/process-only:

- derive canonical family/track/mode/workpack/capsule paths in checker code;
- enforce exact canonical index path/protocol and exact PA glob/result template mirrors;
- make current PA table extraction specs checker-owned and fail human/reconstruct for a future PA with no reviewed structural oracle;
- pin current representative source identities without creating a prose semantic registry;
- add real `--audit-index` class-level defect injections for selectors, paired omissions, alternate paths and configuration-assisted hiding;
- retain bounded independent semantic fixtures for prose/directionality;
- enforce one canonical full validation command surface across workflow/protocol/DocSync skill/evidence;
- update durable evidence and rerun complete pre-review.

Forbidden expansion remains unchanged: no product/runtime contract changes, no bulk migration, no NLP equivalence engine, no capsule semantic authority, no rewriting accepted predecessor sources.

## 7. Exit gate

No new freeze is permitted until:

- every `BUG` above is `RED_AUTOMATIC`, `RED_SEMANTIC_ORACLE` or explicit `ESCALATE_HUMAN`;
- the durable mutation matrix contains zero `UNSAFE`;
- all historical Reviewer defect controls still exercise their actual causal oracle;
- full command surface is identical wherever called “full”;
- baseline real `--audit-index` and all independent controls are GREEN;
- 10+ hostile Reviewer pre-mortem hypotheses are investigated against repaired code;
- complete Worker pre-review is rerun from zero and persisted `WORKER_PRE_REVIEW: CLEAN`;
- only then may exact-SHA workflows run, branch freeze and `REVIEW_READY` occur.

`PROOF_BUDGET_PRE_REPAIR: WITHIN_BUDGET` — the added authority is structural selector/provenance binding and test-only bounded semantic evidence, not a new production semantic source of truth.
