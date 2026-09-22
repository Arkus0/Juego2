# CTX-02 Worker Pre-Review

WP: `WP-CTX-02`
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`
Original pre-review subject before report persistence: `12c4bfaa01b74c11668a930e4cc06c30813fa02e`
First frozen handoff candidate: `975249f242650b47b96e2af0fbf57a1d30cef23a`
Class: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`

`WORKER_PRE_REVIEW: CLEAN`

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 9`

This report records the complete strict Worker pre-review performed while PR #113 remained Draft + ACTIVE, plus the mechanical Ready-handoff defect found before any independent Reviewer began. The first Ready transition was returned to Draft + ACTIVE before repository repair. Persisting this updated report is the final planned repository-byte mutation; the resulting SHA must receive the same complete no-write rerun and a green Worker handoff lint before it can freeze.

## Contract and inherited boundary

Re-read and challenged:

- `Docs/workpacks/CTX/WP-CTX-02.md` complete Work/Forbidden/Required controls/Acceptance/DoD;
- accepted CTX-01 completion and PASS lineage;
- `AGENTS.md`;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`;
- representative H1/CITY/PA consumer/predecessor contracts and canonical PA result documents.

The predecessor check remains valid. CTX-01's authority ordering, fail-closed escalation, Worker pre-review, Reviewer independence and H1 local-executor boundary are consumed rather than re-proved. CTX-02 newly owns only accepted-contract capsule navigation/compression and its validation/adoption mechanics.

While CTX-02 was active, live `main` first advanced from the recorded Worker baseline through plan-only PR #114. That PR changed only `Docs/workpacks/CTX/WP-CTX-03.md`, explicitly kept CTX-03 blocked on CTX-02 and did not touch CTX-01, CTX-02 or PR #113's write set. A later accidental empty root-file add on `main` was immediately reverted; comparison of the resulting current-main tree against the post-#114 tree reports zero file differences. These non-causal history advances do not invalidate the CTX-02 predecessor check or require importing future CTX-03 planning bytes into this candidate.

## Complete diff / scope verdict

The complete baseline→candidate diff was inspected, not only the latest fixes. Changed surfaces are limited to:

- context/process authority and role-routing docs;
- capsule protocol/schema/index and representative capsule data;
- capsule validation/control scripts and CI orchestration;
- CTX-02 Worker evidence.

No product/runtime contract, canonical harness implementation, gameplay, Unity project, CITY production geometry/result, or accepted PA source result is mutated. CITY's exact product seed remains authoritative and non-compressible; the candidate adds only a boundary capsule that points to it.

Scope verdict: **WITHIN WP-CTX-02**.

## Acceptance challenge

### H1

`WP-HK-GATE` capsule binds exact accepted reviewed/merge identity and points directly to accepted gate verdict, proof matrix and residual risk. It compresses the inherited H0/H1 ownership boundary without bulk-migrating H0 or replacing proof. `WP-H1-02` remains responsible for Unity/toolchain guarantees and its local execution protocol.

### CITY

`WP-CITY-03` capsule is deliberately boundary-only. `CITY_PRODUCT_SEED.md` is mechanically required as `noncompressible=true`; removing that exact read is a validation failure. CITY-04 eligibility still depends on H1-08 and cannot be granted by a capsule.

### PA

The accepted cumulative PA chain is explicit. PA-01/02/03 each have one capsule bound to exact accepted identity and canonical result bytes. Future PA chain coverage is discovered from canonical `PA-NN.md` results whose matching workpack is `COMPLETE`, rather than trusting the capsule index to define its own universe.

The PA source disposition table is parsed mechanically and compared as an exact key→status map. Compound/deferred/exclusion states such as `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT`, `REJECT as authority` and `REJECT baseline` cannot silently collapse to binary summary state.

PA-03 explicitly protects the inherited PA-02 exclusion `default global/N-hop social traversal to discover targets = REJECT`, plus distinct forward/reverse trust/affinity/fear semantics.

Current PA-01..03 canonical result files total 73,039 bytes versus 14,011 bytes for their capsule JSON files: 59,028 fewer bytes, about 80.8% less repeated accepted-result payload before tokenizer effects. This is a byte measurement only, not a model-token claim.

## Findings found and repaired before independent review

1. **Final-evidence rerun gap.** Capsule CI originally did not trigger on CTX-02 evidence changes. Repaired by adding `Docs/evidence/CTX-02/**` so the final evidence-bearing SHA reruns the capsule audit.
2. **Positive-loss false proof.** The first positive omission control could RED merely because all exports were removed and schema shape failed. Repaired with a separate independent oracle that removes one of two material positive guarantees while another remains; structural capsule validation still passes and the independent oracle must detect the loss.
3. **Exclusion-loss symmetry.** Added the corresponding one-of-many exclusion omission control so a predecessor-carrying REJECT cannot disappear while the capsule remains structurally valid.
4. **H1 proof navigation incomplete/incorrect.** HK-GATE initially lacked direct proof/residual navigation and a first repair guessed a nonexistent `FINAL_VERDICT.md` path. Real CI RED exposed the error. Rebound to canonical `VERDICT.md`, `PROOF_MATRIX.md`, and `RESIDUAL_RISK.md` with recomputed blob fingerprints; rerun GREEN.
5. **Optimization not binding at authority layer.** Profiles alone could not save context because accepted `AGENTS.md` / `WORKER_REVIEW_PROTOCOL.md` still mandated full predecessor narratives unconditionally. Repaired the post-adoption rules and role skills so a valid capsule may satisfy initial reconstruction while all material/reopen/non-compressible cases still deepen to authority.
6. **Protocol self-adoption ambiguity.** Directly changing predecessor-read mechanics could have retroactively changed CTX-02's own process. Repaired with `WORKER_REVIEW_PROTOCOL` v1.9 adoption deferred until CTX-02 independent PASS + merge + DocSync; CTX-02 itself remains governed by v1.8 and used full predecessor reconstruction.
7. **Residual unconditional Worker profile read.** `context-bootstrap-profiles.json` still phrased direct dependencies as an exact dependency-WP read even when a capsule was valid. Repaired to an explicit capsule-or-authoritative-source path with fail-closed escalation.
8. **Discoverability regression path.** Bootstrap/skills could later stop naming the capsule mechanism while capsule data remained green. Repaired `CONTEXT_BOOTSTRAP_V1`, explicit skill protocol paths and an independent wiring control that REDs if Worker/Repair/Reviewer/DocSync no longer discover the capsule protocol.
9. **Ready handoff predecessor marker missing.** The first Ready transition on frozen candidate `975249f242650b47b96e2af0fbf57a1d30cef23a` had a GREEN exact-SHA freeze verifier but Worker handoff lint RED because `PREDECESSOR_CONTRACT_CHECK.md` did not contain the literal marker `PREDECESSOR_CONTRACT_CHECK`. The file itself had been persisted before implementation and already contained all required reasoning, so the defect was durable handoff syntax rather than missing predecessor work. PR returned to Draft + ACTIVE, the canonical marker was added to that evidence file, and this full pre-review was rerun before a new freeze.

## Causal negative controls

The exact candidate validation surface exercises:

- reviewed-candidate SHA mismatch -> FAIL closed;
- source blob fingerprint mismatch -> FAIL closed;
- external accepted state `REOPENED` -> FAIL closed;
- external accepted exact-SHA mismatch -> FAIL closed;
- one material positive export omitted while another remains -> independent control RED;
- one material exclusion omitted while another remains -> independent control RED;
- authoritative PA disposition row omitted -> FAIL;
- `LATER` reclassified -> FAIL;
- A→B/B→A collapse -> FAIL;
- CITY non-compressible seed read removed -> FAIL;
- accepted PA result with matching COMPLETE WP omitted from capsule index -> coverage FAIL;
- capsule bootstrap/role wiring removed -> independent control FAIL.

The production capsule/index does not define the complete universe used to prove itself: accepted identity comes from independent completion metadata/live state, source integrity from repository bytes, PA row coverage from canonical result tables, PA chain coverage from canonical result+WP discovery, and one-of-many material-loss tests from a separate test-only oracle.

## Validation history

On exact pre-report SHA `12c4bfaa01b74c11668a930e4cc06c30813fa02e`:

- `Context Capsule Validation` run `35685529557` / #24: **SUCCESS**;
- `Arkus Candidate Validation` run `35685529565` / #919: **SUCCESS**.

On first frozen candidate `975249f242650b47b96e2af0fbf57a1d30cef23a` before the handoff-marker repair:

- `Context Capsule Validation` run `35685630382` / #25: **SUCCESS**;
- `Arkus Candidate Validation` run `35685630373` / #920: **SUCCESS** while Draft;
- Ready run `35685757106` / #921: `Freeze exact-SHA validation` **SUCCESS**, `Worker handoff lint` **FAIL** solely on missing literal predecessor evidence marker.

Other useful RED→GREEN evidence is preserved in Actions: run `35684684410` RED exposed bad H1 evidence navigation; repaired run `35684803654` GREEN. Run `35685443916` RED exposed missing deterministic capsule discovery in repair skill; the later adoption-wiring run GREEN after repair.

## Residual boundary

No product/runtime semantics are claimed. Capsules remain navigation only. Token counts vary by tokenizer; only the byte reduction above is asserted here. Exact non-compressible production/proof material can still be large when the current question genuinely needs it; CTX-02 optimizes repeated inherited reconstruction, not necessary source depth.

## Current verdict before final rerun

No known semantic/in-claim blocker remains. The final evidence mutation consists only of the canonical predecessor marker and this truthful pre-review update. The resulting exact SHA must now pass the complete capsule audit, Candidate Validation, Worker handoff lint and Freeze exact-SHA validation before it is recorded as the new frozen candidate.