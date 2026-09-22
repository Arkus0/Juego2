# CTX-02 Worker Pre-Review — Final Circuit-Breaker

WP: `WP-CTX-02`  
PR: `#113`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
`fail_cycle: 5`

`WORKER_PRE_REVIEW: CLEAN`

`UNSAFE: 0`

`PROOF_BUDGET: WITHIN_BUDGET`

This is the complete Worker pre-review rebuilt from zero after review `#5274609459`. It is not a patch-only receipt. The Worker re-audited the complete CTX-02 trust boundary, all five historical Reviewer FAIL classes, the complete validation/oracle surface, adoption wiring, current live-main movement, scope and a 15-attack hostile Reviewer pre-mortem before declaring CLEAN.

Durable final-audit authority: `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`.  
Claim→source→oracle→mutation matrix: `Docs/evidence/CTX-02/CONTROL_MATRIX.md`.  
Representative/integration runs: `Docs/evidence/CTX-02/DRY_RUNS.md`.  
Proof proportionality: `Docs/evidence/CTX-02/PROOF_BUDGET.md`.

## 1. Historical Reviewer FAIL lineage — all preserved

1. `8db17036ad65ca03540f14e71711d806179c3e1c`, review `#5274094804`: whole accepted-PA `disposition_source` + `dispositions` omission. **Preserved RED**.
2. `504b4ff25f67720be9adb6b948dff859413419f3`, review `#5274163927`: malformed reopen/escalation plus review ID without explicit PASS. **Preserved RED**.
3. `66ccbeccf699f98c591809c739a3118f1b376e9c`, review `#5274257119`: same-ID/source-pointer/fingerprint semantic substitution. **Preserved RED** through independent bounded semantic oracles.
4. `1afc4135e4eda8cfab8e85325dcafcf123978915`, review `#5274389905`: one-of-many omission control did not exercise the semantic oracle; PA universe was result-first. **Preserved repair**: actual representative-oracle RED + COMPLETE-workpack-side discovery.
5. `210d42cc41199716e46d38c23ed7a550b8038a1c`, review `#5274609459`: PA `workpack_glob`/`result_template` still index-controlled; DocSync full-surface list omitted omission controls. **Closed at class boundary**, not only examples.

## 2. Final circuit-breaker defect class closure

The final audit asked whether audited data/configuration, coordinated absence, source rebinding or semantic substitution could change what the checker believed it had to prove and still produce GREEN.

The repair makes all mechanically consequential selectors checker-owned or exact-bound:

- canonical index and protocol paths;
- canonical capsule path from capsule ID;
- canonical workpack path, track and content mode from capsule ID;
- PA COMPLETE-workpack discovery glob;
- PA canonical result template/path;
- current PA-01/02/03 disposition section/key/status-column selectors.

The audited index/capsule may repeat those values only as assertions. It cannot select a different universe/oracle.

Additional same-class false-greens found and repaired in cycle 5:

1. CITY `track` could suppress the CITY-specific oracle when changed together with mandatory-read deletion;
2. `content_mode` remained subject-controlled;
3. PA table section/key/status columns selected their own completeness oracle;
4. index entry could redirect an ID to an alternate valid-looking capsule JSON;
5. `--audit-index` could audit an alternate index file;
6. accepted identity could rebind to a same-name workpack under another root;
7. representative authoritative-source inventory could be silently narrowed while remaining fingerprints stayed valid;
8. DocSync validation-surface drift omitted `context-capsule-omission-controls.py`.

All eight are now either `RED_AUTOMATIC` or `RED_SEMANTIC_ORACLE`. No `UNSAFE` remains.

## 3. Trust/oracle boundary re-review

Final oracle classes remain deliberately separated:

- **A / deterministic production:** canonical roots/selectors, exact accepted identity/PASS, external-source boundary, recomputed blob fingerprints, COMPLETE PA inventory, canonical result existence, checker-owned disposition selector, exact key/status equality, exact CITY seed, structural asymmetry;
- **B / bounded independent semantic tests:** material HK/CITY/PA representative exports, exclusions, reopen/escalation values, PA direction values, representative capsule/source inventories and structurally valid one-of-many omissions;
- **C / explicit human escalation:** arbitrary future prose completeness/equivalence, contested interpretation, coordinated rewrite of both capsule and test-only semantic fixture, and authenticity/provenance of an externally supplied live-state assertion.

Class C is never converted to automatic PASS. The required outcome is `ESCALATE_HUMAN / RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`.

The production checker does not import an approved-prose registry and no product semantic source of truth moved into CTX-02.

## 4. Hostile Reviewer pre-mortem — 15 attacks investigated

1. Narrow PA workpack selector after deleting future accepted item → **RED_AUTOMATIC**, exact checker glob.
2. Redirect PA result template to shadow tree → **RED_AUTOMATIC**.
3. Supply alternate index to `--audit-index` → **RED_AUTOMATIC**.
4. Redirect same index ID to alternate valid capsule file → **RED_AUTOMATIC**.
5. CITY→H1 plus delete `CITY_PRODUCT_SEED.md` mandatory read → **RED_AUTOMATIC**.
6. Switch `content_mode` to disable structural validator → **RED_AUTOMATIC**.
7. Choose another valid-looking PA table/columns in same result → **RED_AUTOMATIC**.
8. Use same-name workpack under another directory as identity authority → **RED_AUTOMATIC**.
9. Delete HK proof source while keeping guarantees and remaining hashes valid → **RED_SEMANTIC_ORACLE**.
10. Delete representative capsule + index entry together → PA **RED_AUTOMATIC** from COMPLETE universe; bounded H1/CITY **RED_SEMANTIC_ORACLE** from independent representative inventory.
11. Spoof acceptance with review ID but FAIL verdict → **RED_AUTOMATIC**.
12. Replace reopen/escalation with opposite but non-empty prose → representative **RED_SEMANTIC_ORACLE**; arbitrary future prose **ESCALATE_HUMAN**, never implicit GREEN.
13. Change PA direction to another still-asymmetric expression → representative **RED_SEMANTIC_ORACLE**; structural collapse separately automatic RED.
14. Make a defect test prove only mutation occurrence/helper failure → closed by subprocess real `--audit-index` integration controls and by the actual representative semantic oracle for omission controls.
15. Add future COMPLETE PA capsule without reviewed checker-owned disposition selector → **fail closed / RECONSTRUCT**, capsule-chain coverage cannot become COMPLETE.

Result: `PRE_MORTEM_UNSAFE: 0`.

## 5. Mutation matrix verdict

The durable `CONTROL_MATRIX.md` records every material automatic claim as:

`claim -> authoritative universe/source -> independent oracle -> mutation -> required result -> current result`.

Covered classes include delete, paired delete, rename/rebind, canonical-path change, narrow/broad/no-match selector, alternate result template, same-ID semantic substitution, blank/null surfaces, whole structured-surface removal, PASS spoofing, directional mutation, live-state mismatch, index inventory mutation and configuration-assisted hiding.

Final matrix result: `UNSAFE: 0`.

## 6. Full validation surface — preliminary repaired-head execution

On repaired pre-report SHA `00979c2c9c9589e0b686fabbfe0928b7cc6a20e9`, Context Capsule Validation run `35695822987` / #73 checked out that exact SHA and executed:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

Observed results from the workflow log:

- `context-capsule self-test: PASS`;
- `context-capsule independent controls: PASS`;
- `context-capsule representative omission controls: PASS (defect injections RED)`;
- `context-capsule PA-01/02 semantic controls: PASS`;
- real index audit: `result: PASS`, `semantic_authority_granted: false`, PA chain discovered exactly `WP-PA-01`, `WP-PA-02`, `WP-PA-03`, coverage `COMPLETE`.

Arkus Candidate Validation run `35695822985` / #1050 on the same SHA was also **SUCCESS**.

These runs prove the repaired surface before this durable pre-review report. Because this report itself changes repository bytes, **they are not the final freeze runs**. After this commit, both workflows must be GREEN again on the exact new HEAD before freeze/Ready.

## 7. Adoption and command-surface review

Canonical full CTX-02 validation commands are the five commands above. `context-capsule-controls.py` independently requires the complete set in:

- `Docs/engineering/CONTEXT_CAPSULE_V1.md`;
- `.agents/skills/update-handoff/SKILL.md`;
- `.github/workflows/context-capsule-validation.yml`;
- any other skill that explicitly claims the “full CTX-02 validation surface”.

The latest Reviewer's minor blocker is therefore closed and future same-phrase workflow drift is a control failure.

Reviewer independence is unchanged: capsules/test fixtures do not limit review, and concrete contradictory evidence routes to original authoritative sources.

## 8. Scope / current-main review

Current live `main` at this pre-review is `b831050e9df8b61b76744e0c5f544bd7ec2d79b5`, advanced by merge of planning PR #117 / DW second-consumer plan.

That main movement is outside the CTX-02 write set: it adds/changes DW/ROADMAP planning surfaces and does not modify the immutable accepted HK-GATE, CITY-03 or PA-01/02/03 source bindings consumed by CTX-02. It does not reopen CTX-01 or alter CTX-02 ownership. PR #113 remains open/mergeable.

The PR write set remains process/context machinery only: role/bootstrap protocol, capsule schema/index/capsules, CTX-02 evidence, checker/control scripts and the dedicated validation workflow. No runtime/product implementation, Unity project, CITY product seed/result, canonical accepted PA result, accepted predecessor workpack or product semantic architecture is modified.

Scope verdict: **WITHIN WP-CTX-02 / PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**. No bulk migration and no product contract reduction occurred.

## 9. Proof-budget review

`PROOF_BUDGET: WITHIN_BUDGET`.

The additional authority is mechanical: fixed repository roots, naming conventions and structural selectors that otherwise let the subject choose its own oracle. Natural-language semantics remain bounded test evidence + human escalation. No fuzzy/NLP equivalence, general semantic theorem prover or capsule-as-authority architecture was introduced.

## 10. Final no-write exit gate

After this report commit, repository bytes must remain unchanged unless an exact-candidate check exposes another defect. Freeze is authorized only if all of these hold simultaneously on one HEAD:

- Context Capsule Validation: GREEN;
- Arkus Candidate Validation: GREEN;
- baseline real `--audit-index`: GREEN;
- all historical defect controls: RED as intended;
- all cycle-5 class-level controls: RED as intended;
- `UNSAFE: 0`;
- PR HEAD equals declared Frozen candidate SHA;
- PR Ready only after exact-SHA GREEN;
- durable `REVIEW_READY` names the exact same SHA.

## Worker verdict

`WORKER_PRE_REVIEW: CLEAN`

`PROOF_BUDGET: WITHIN_BUDGET`

`UNSAFE: 0`

This CLEAN verdict authorizes exact-SHA validation/freeze/handoff only. It does **not** authorize merge or DocSync without a fresh independent Reviewer PASS.
