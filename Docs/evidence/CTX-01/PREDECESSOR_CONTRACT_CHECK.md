# CTX-01 — PREDECESSOR_CONTRACT_CHECK

Baseline main SHA: `02016ba5a3d0a344525835652bbd84c9a2e9cc49`
Workpack: `WP-CTX-01 — Role-specific bootstrap + accepted-state navigation`
Mode: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`

## Accepted predecessor

The direct predecessor is the accepted CTX programme plan:

- contract family: `Docs/workpacks/CTX/README.md` + `Docs/workpacks/CTX/WP-CTX-01.md`;
- frozen plan candidate: `c9ff3605048e05fded4d58a1de27450661d9fe0e`;
- independent PASS: PR #102 review `#5270686380`;
- plan merge: `f3c8362b3d76fd4f78107d8142e07e476985f973`;
- post-PASS DocSync: PR #103, merge `609ac464e0b5aed2da0a369b6f2d368f72dd35a3`.

The historical CTX-01 baseline remains `02016ba5a3d0a344525835652bbd84c9a2e9cc49`.

## Main drift reconciled during the Worker cycle

Before final pre-review/freeze, current `main` advanced to `3f1247a2289351d32a5d8c1f3bbbcbee32db6983` through PROCESS_ONLY hotfix PR #107. The CTX-01 branch explicitly merged that current main; compare now reports the candidate branch `behind_by: 0` with merge-base `3f1247a2289351d32a5d8c1f3bbbcbee32db6983`.

PR #107 changed Automation V2's PROCESS_ONLY validation behavior only: when no canonical product/runtime command is executed, it no longer synthesizes `EXECUTION_RECEIPT_V1`, `Result: GREEN`, or equivalent execution-proof claims. PROCESS_ONLY still performs applicable cheap mechanical checks such as exact-checkout verification and Worker handoff lint where the Worker lifecycle applies.

CTX-01 consumes that accepted maintenance as current process state. It does **not** redesign Automation V2, add receipt semantics, or treat absence of an execution receipt as failure for this PROCESS_ONLY WP. The bootstrap profiles/router/index concern context selection and authority only.

## Inherited guarantees relevant to CTX-01

1. Priority order is fixed: product/architecture correctness, proof/review quality, context precision, then token/local-agent cost.
2. CTX-01 owns **context selection, authority ordering, stale detection and escalation only**. It must not introduce accepted-contract capsules (CTX-02) or evidence/history migration (CTX-03).
3. Compact indexes/summaries are navigation projections, never semantic/proof authority. Missing, stale, contradictory or materially ambiguous compact context must force deeper authoritative reads.
4. Live GitHub remains authoritative for mutable PR/branch/review/check/ownership state; exact accepted repository contracts/evidence remain authoritative for semantics and proof.
5. Worker predecessor reconstruction, strict Worker pre-review, exact freeze/review SHA binding and fresh independent Reviewer judgment remain unchanged.
6. The H1 local executor remains a bounded context-poor mechanical executor under `H1_REMOTE_LOCAL_EXECUTION.md`; CTX-01 may route it but may not broaden its discretion.
7. The accepted CTX-plan Reviewer explicitly expects CTX-01 dry-run evidence to make the **escalation decision itself observable**, so a false `context closed` decision cannot hide a required ROADMAP/proof read.
8. Current PROCESS_ONLY lifecycle semantics include the PR #107 rule that no synthetic execution receipt/GREEN claim is produced where no canonical execution occurred.

## Guarantees newly owned by CTX-01

- a binding Context Bootstrap v1 protocol with question-specific authority ordering;
- machine-readable minimum boot profiles for Worker, repair Worker, Reviewer, planner/gate, DocSync and H1 local executor;
- a compact derived accepted-state index with exact `generated_from_main_sha` stale detection;
- routing of project role skills through those profiles;
- conditional rather than mechanical initial loading of full ROADMAP/foundational proof surfaces for exact-WP sessions;
- a compact session-handoff router;
- DocSync instructions for refreshing the derived index without promoting it to authority;
- representative dry-runs and negative controls proving escalation and stale/missing-context fail-closed behavior.

## Accepted guarantees intentionally consumed rather than re-proved

- the CTX programme's three-WP causal split and its quality-first priority;
- the semantic content of the existing Worker/Reviewer protocol;
- the H1 remote/local SHA-chain and executor-ownership model;
- Worker handoff lint's mechanical lifecycle checks;
- PR #107's PROCESS_ONLY non-receipt behavior;
- accepted H0/H1/CITY/PA product semantics unrelated to context selection.

CTX-01 may point to those authorities and test correct routing to them. It does not reopen or duplicate their substantive proof.

## Concrete predecessor reopen condition

Reopen the CTX-plan boundary only if implementation evidence shows that safe role-specific routing cannot preserve a material existing obligation without changing the plan's accepted authority/escalation model, or if current accepted process state contradicts an inherited plan assumption. Mere desire for broader default reads, redundant proof or extra summaries is not a reopen condition.

`PREDECESSOR_CONTRACT_CHECK: COMPLETE`
