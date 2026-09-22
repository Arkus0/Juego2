# CTX-02 Worker Pre-Review

WP: `WP-CTX-02`
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`
Original pre-review subject before report persistence: `12c4bfaa01b74c11668a930e4cc06c30813fa02e`
First frozen handoff candidate: `975249f242650b47b96e2af0fbf57a1d30cef23a`
First independently reviewed candidate: `8db17036ad65ca03540f14e71711d806179c3e1c`
Independent FAIL evidence: PR review `5274094804`
Repair-cycle pre-review subject before this report persistence: `d520768ff37189ed847d23259ce45aa0a77d893e`
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 10`

This report records the complete strict Worker pre-review for CTX-02, including the fresh rerun performed after independent Reviewer FAIL on candidate `8db17036ad65ca03540f14e71711d806179c3e1c`. The PR returned to Draft + ACTIVE before repair. The prior clean pre-review/freeze was treated as stale, the causal false-green was reproduced, the complete baseline-to-repaired-candidate surface was re-challenged, and the affected validation/control set was rerun. Persisting this updated report is the final planned repository-byte mutation; the resulting SHA must receive the same complete no-write validation and a green Worker handoff/freeze check before it can be handed back to an independent Reviewer.

## Contract and inherited boundary

Re-read and challenged:

- `Docs/workpacks/CTX/WP-CTX-02.md` complete Work/Forbidden/Required controls/Acceptance/DoD;
- accepted CTX-01 completion and PASS lineage;
- `AGENTS.md`;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` including its explicit rule that CTX-02 itself remains governed by the pre-adoption predecessor-read mechanics;
- representative H1/CITY/PA consumer/predecessor contracts and canonical PA result documents;
- independent FAIL review `5274094804` and its exact reviewed candidate `8db17036ad65ca03540f14e71711d806179c3e1c`.

The predecessor check remains valid. The Reviewer FAIL is wholly inside CTX-02's newly owned capsule validation boundary and does not reopen CTX-01 or any accepted PA semantic result. CTX-01's authority ordering, fail-closed escalation, Worker pre-review, Reviewer independence and H1 local-executor boundary are consumed rather than re-proved. CTX-02 newly owns only accepted-contract capsule navigation/compression and its validation/adoption mechanics.

While CTX-02 was active, live `main` advanced from the recorded Worker baseline through plan-only PR #114 and a later accidental empty root-file add that was immediately reverted. Current `main` is `a2929bdf488e4fd9ffc9d59cf36886c92ef95b51`; the non-causal history advance does not alter CTX-01, CTX-02, the accepted PA source results, or PR #113's repair boundary. It therefore does not invalidate the predecessor check or require importing future CTX-03 planning bytes into this candidate.

## Complete diff / scope verdict

The complete baseline→candidate diff was re-inspected after the FAIL, not only the latest repair. Changed surfaces remain limited to:

- context/process authority and role-routing docs;
- capsule protocol/schema/index and representative capsule data;
- capsule validation/control scripts and CI orchestration;
- CTX-02 Worker evidence.

The independent-F​​AIL repair delta from `8db17036ad65ca03540f14e71711d806179c3e1c` to pre-report subject `d520768ff37189ed847d23259ce45aa0a77d893e` is deliberately narrow in final bytes: 8 added lines in `scripts/context-capsule-check.py` and 55 added lines in `scripts/context-capsule-controls.py`. An intermediate formatting-heavy edit was fully normalized away before pre-review; the effective repair diff contains no unrelated reformatting.

No product/runtime contract, canonical harness implementation, gameplay, Unity project, CITY production geometry/result, or accepted PA source result is mutated. CITY's exact product seed remains authoritative and non-compressible; the candidate adds only a boundary capsule that points to it.

Scope verdict: **WITHIN WP-CTX-02**.

## Acceptance challenge

### H1

`WP-HK-GATE` capsule binds exact accepted reviewed/merge identity and points directly to accepted gate verdict, proof matrix and residual risk. It compresses the inherited H0/H1 ownership boundary without bulk-migrating H0 or replacing proof. `WP-H1-02` remains responsible for Unity/toolchain guarantees and its local execution protocol.

### CITY

`WP-CITY-03` capsule is deliberately boundary-only. `CITY_PRODUCT_SEED.md` is mechanically required as `noncompressible=true`; removing that exact read is a validation failure. CITY-04 eligibility still depends on H1-08 and cannot be granted by a capsule.

### PA

The accepted cumulative PA chain is explicit. PA-01/02/03 each have one capsule bound to exact accepted identity and canonical result bytes. Future PA chain coverage is discovered from canonical `PA-NN.md` results whose matching workpack is `COMPLETE`, rather than trusting the capsule index to define its own universe.

For every accepted PA result discovered by that chain audit, the capsule is now required to declare `track=PA`, `content_mode=structured_disposition`, a structured `disposition_source`, and a non-empty `dispositions` list. The ordinary capsule validator still permits non-disposition capsule shapes needed by other tracks; the stronger requirement is applied exactly where the accepted PA chain claims structured disposition preservation.

The PA source disposition table is parsed mechanically and compared as an exact key→status map. Compound/deferred/exclusion states such as `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT`, `REJECT as authority` and `REJECT baseline` cannot silently collapse to binary summary state. The new whole-surface negative control additionally proves that removing both structured disposition fields cannot bypass the row-level oracle: baseline `--audit-index` passes, the mutated accepted-PA capsule deletes both fields, and the same CLI exits non-zero before PA chain coverage can be called COMPLETE.

PA-03 explicitly protects the inherited PA-02 exclusion `default global/N-hop social traversal to discover targets = REJECT`, plus distinct forward/reverse trust/affinity/fear semantics.

Current PA-01..03 canonical result files total 73,039 bytes versus 14,011 bytes for their capsule JSON files: 59,028 fewer bytes, about 80.8% less repeated accepted-result payload before tokenizer effects. This is a byte measurement only, not a model-token claim.

## Findings found and repaired before independent review / during FAIL repair

1. **Final-evidence rerun gap.** Capsule CI originally did not trigger on CTX-02 evidence changes. Repaired by adding `Docs/evidence/CTX-02/**` so the final evidence-bearing SHA reruns the capsule audit.
2. **Positive-loss false proof.** The first positive omission control could RED merely because all exports were removed and schema shape failed. Repaired with a separate independent oracle that removes one of two material positive guarantees while another remains; structural capsule validation still passes and the independent oracle must detect the loss.
3. **Exclusion-loss symmetry.** Added the corresponding one-of-many exclusion omission control so a predecessor-carrying REJECT cannot disappear while the capsule remains structurally valid.
4. **H1 proof navigation incomplete/incorrect.** HK-GATE initially lacked direct proof/residual navigation and a first repair guessed a nonexistent `FINAL_VERDICT.md` path. Real CI RED exposed the error. Rebound to canonical `VERDICT.md`, `PROOF_MATRIX.md`, and `RESIDUAL_RISK.md` with recomputed blob fingerprints; rerun GREEN.
5. **Optimization not binding at authority layer.** Profiles alone could not save context because accepted `AGENTS.md` / `WORKER_REVIEW_PROTOCOL.md` still mandated full predecessor narratives unconditionally. Repaired the post-adoption rules and role skills so a valid capsule may satisfy initial reconstruction while all material/reopen/non-compressible cases still deepen to authority.
6. **Protocol self-adoption ambiguity.** Directly changing predecessor-read mechanics could have retroactively changed CTX-02's own process. Repaired with `WORKER_REVIEW_PROTOCOL` v1.9 adoption deferred until CTX-02 independent PASS + merge + DocSync; CTX-02 itself remains governed by v1.8 and used full predecessor reconstruction.
7. **Residual unconditional Worker profile read.** `context-bootstrap-profiles.json` still phrased direct dependencies as an exact dependency-WP read even when a capsule was valid. Repaired to an explicit capsule-or-authoritative-source path with fail-closed escalation.
8. **Discoverability regression path.** Bootstrap/skills could later stop naming the capsule mechanism while capsule data remained green. Repaired `CONTEXT_BOOTSTRAP_V1`, explicit skill protocol paths and an independent wiring control that REDs if Worker/Repair/Reviewer/DocSync no longer discover the capsule protocol.
9. **Ready handoff predecessor marker missing.** The first Ready transition on frozen candidate `975249f242650b47b96e2af0fbf57a1d30cef23a` had a GREEN exact-SHA freeze verifier but Worker handoff lint RED because `PREDECESSOR_CONTRACT_CHECK.md` did not contain the literal marker `PREDECESSOR_CONTRACT_CHECK`. The file itself had been persisted before implementation and already contained all required reasoning, so the defect was durable handoff syntax rather than missing predecessor work. PR returned to Draft + ACTIVE, the canonical marker was added to that evidence file, and the full pre-review was rerun before candidate `8db17036ad65ca03540f14e71711d806179c3e1c` froze.
10. **Accepted-PA whole-disposition-surface false green.** Independent Reviewer review `5274094804` proved that deleting both `disposition_source` and `dispositions` from an accepted PA capsule bypassed `validate_dispositions()` and still let `validate_pa_chain()` call the chain COMPLETE. The PR returned to Draft + ACTIVE. The repair keeps generic non-PA disposition optionality intact but makes accepted PA chain discovery require `track=PA`, `content_mode=structured_disposition`, `disposition_source` and non-empty `dispositions`. An independent CLI defect-injection control now writes a valid accepted PA chain fixture, proves baseline `--audit-index` GREEN, deletes the entire field pair, and requires the same `--audit-index` invocation to fail. This closes the reported class rather than only the previously tested one-row omission example.

## Causal negative controls

The repaired candidate validation surface exercises:

- reviewed-candidate SHA mismatch -> FAIL closed;
- source blob fingerprint mismatch -> FAIL closed;
- external accepted state `REOPENED` -> FAIL closed;
- external accepted exact-SHA mismatch -> FAIL closed;
- one material positive export omitted while another remains -> independent control RED;
- one material exclusion omitted while another remains -> independent control RED;
- authoritative PA disposition row omitted -> FAIL;
- `LATER` reclassified -> FAIL;
- entire accepted-PA `disposition_source` + `dispositions` surface removed -> `--audit-index` FAIL;
- accepted PA capsule with wrong/non-structured content mode -> chain audit FAIL;
- A→B/B→A collapse -> FAIL;
- CITY non-compressible seed read removed -> FAIL;
- accepted PA result with matching COMPLETE WP omitted from capsule index -> coverage FAIL;
- capsule bootstrap/role wiring removed -> independent control FAIL.

The production capsule/index does not define the complete universe used to prove itself: accepted identity comes from independent completion metadata/live state, source integrity from repository bytes, PA row coverage from canonical result tables, PA chain coverage from canonical result+WP discovery, and material-loss tests from a separate test-only oracle. The Reviewer-reported wholesale-field mutation is now protected at the PA chain boundary even though a generic non-PA capsule may legitimately omit disposition fields.

## Validation history

On exact pre-report SHA `12c4bfaa01b74c11668a930e4cc06c30813fa02e`:

- `Context Capsule Validation` run `35685529557` / #24: **SUCCESS**;
- `Arkus Candidate Validation` run `35685529565` / #919: **SUCCESS**.

On first frozen candidate `975249f242650b47b96e2af0fbf57a1d30cef23a` before the handoff-marker repair:

- `Context Capsule Validation` run `35685630382` / #25: **SUCCESS**;
- `Arkus Candidate Validation` run `35685630373` / #920: **SUCCESS** while Draft;
- Ready run `35685757106` / #921: `Freeze exact-SHA validation` **SUCCESS**, `Worker handoff lint` **FAIL** solely on missing literal predecessor evidence marker.

On independently reviewed candidate `8db17036ad65ca03540f14e71711d806179c3e1c`:

- prior Worker pre-review/freeze/handoff checks were GREEN;
- independent review `5274094804`: **FAIL** on the accepted-PA whole-disposition-surface false green described above.

On repaired pre-report SHA `d520768ff37189ed847d23259ce45aa0a77d893e` while PR #113 remained Draft + ACTIVE:

- `Context Capsule Validation` run `35686845977` / #30: **SUCCESS**;
- `Arkus Candidate Validation` run `35686845959` / #932: **SUCCESS**;
- final effective repair diff versus the reviewed FAIL candidate is limited to the validator's accepted-PA chain requirements plus the independent exact-CLI regression control.

Other useful RED→GREEN evidence remains preserved in Actions: run `35684684410` RED exposed bad H1 evidence navigation; repaired run `35684803654` GREEN. Run `35685443916` RED exposed missing deterministic capsule discovery in repair skill; the later adoption-wiring run GREEN after repair.

## Residual boundary

No product/runtime semantics are claimed. Capsules remain navigation only. Token counts vary by tokenizer; only the byte reduction above is asserted here. Exact non-compressible production/proof material can still be large when the current question genuinely needs it; CTX-02 optimizes repeated inherited reconstruction, not necessary source depth.

No residual risk was found that weakens the Reviewer repair. Requiring structured dispositions only for accepted PA results discovered by the chain preserves legitimate non-PA capsule shapes while making the PA chain claim fail closed on both partial-row and wholesale-surface loss.

## Current verdict before final rerun

No known semantic/in-claim blocker remains. `WORKER_PRE_REVIEW: CLEAN` is a Worker readiness statement only, not independent acceptance. Persisting this report is the final planned repository-byte mutation for repair cycle 1. The resulting exact SHA must now pass the complete capsule audit, Candidate Validation, Worker handoff lint and Freeze exact-SHA validation before it is recorded as the new frozen candidate and returned to an independent Reviewer.