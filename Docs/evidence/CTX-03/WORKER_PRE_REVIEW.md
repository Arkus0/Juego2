# CTX-03 — Worker pre-review history / repair-cycle input

Status: **SUPERSEDED BY INDEPENDENT FAIL #5275757245 / NOT FINAL CLEAN EVIDENCE**  
Failed candidate: `1421f1690f1bd20b578ba8f70ee8ee5deb90b67a`  
Baseline: `107694d3850a478849bffd9510dc030910fc8aa3`  
Scope: **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**

This file preserves prior Worker pre-review history. It is intentionally **not** the final clean record for repair cycle 1.

The failed cycle reviewed the complete implementation diff only through parent `460f997c8e70720356f285583977530cc2175c76`, then committed this evidence file as child `1421f1690f1bd20b578ba8f70ee8ee5deb90b67a`. Exact-SHA GREEN CI on that child did not make the complete Worker pre-review retroactively cover the child's bytes. Review `#5275757245` correctly rejected that ordering.

Repair cycle 1 changes the durability rule: all repository/evidence bytes — including this historical record and every repair artifact — are finalized first. The Worker then stops writers, reads the exact resulting HEAD, performs the complete pre-review against that exact HEAD and full baseline→candidate diff, and if clean persists the final clean result as a durable GitHub PR issue comment tied to the exact SHA. That external record does not mutate candidate bytes. Any later repository/evidence mutation invalidates it and requires another complete pre-review.

## Accepted predecessor boundary retained

- `WP-CTX-02` remains independently accepted: candidate `e5053b778e050cff83e2443fef888c64883c88ca`, PASS review `#5274937744`, implementation merge `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`, successful combined DocSync main `107694d3850a478849bffd9510dc030910fc8aa3`.
- `Docs/evidence/CTX-03/PREDECESSOR_CONTRACT_CHECK.md` remains the repository predecessor-check evidence.
- CTX-03 consumes rather than re-proves CTX-02 capsule semantics, checker-owned PA completeness/selectors, accepted semantic controls and Reviewer/source escalation authority.

## Prior Worker findings preserved as history

The failed cycle had already repaired ten Worker-discovered classes:

1. invalid post-marker checkout design;
2. uncalibrated/obsolete base envelope;
3. missing causal quality replay;
4. cosmetic ROADMAP/history separation;
5. mishandling of unknown red verifier state;
6. overly generic closure outcome;
7. metadata generator able to reset lineage;
8. self-shrinking measurement baseline;
9. config-selected measurement universe;
10. unstructured FAIL-capable verifier registry entry.

## Independent Reviewer blocker classes

Review `#5275757245` added three causal classes:

11. **Exact-candidate pre-review ordering** — final clean evidence must be created after the complete pre-review without creating new repository/evidence bytes afterward.
12. **Effective mandatory-context growth** — the process envelope must bound route/profile-forced conditional/capsule/escalated repository sources, not only `initial_reads`.
13. **Same-SHA terminal retry** — `REVIEW_READY_CLOSED` must remain reachable when an existing deduplicated REVIEW_READY marker precedes a red gate and a later metadata/gate rerun turns GREEN on the same SHA.

## Repair-cycle circuit-breaker finding

Before refreezing, the Worker deliberately attempted to find the next variant of blocker 12 and found one:

14. **Non-representative fixed conditionals outside concrete route budgets** — the first repair covered the six checker-owned H1/CITY/PA routes, but fixed `conditional_reads` in `repair_worker`, `planner_gate` and `docsync` could still grow outside those concrete route ceilings. The repair was widened to a three-layer envelope with a checker-owned fixed conditional superset for **every canonical profile**, explicit-path discovery that turns RED when a new fixed conditional is introduced without oracle review, and negative controls that grow every fixed conditional source across its applicable ceiling.

Finding 14 was repaired before final freeze. It is recorded here specifically to demonstrate the requested circuit-breaker audit happened before another Reviewer round rather than leaving this variant for the Reviewer to discover.

## Final repair-cycle pre-review rule

After this file and all other repository/evidence changes are committed and pushed:

1. no writer may change repository/evidence bytes;
2. read the exact branch HEAD;
3. inspect the full `107694d3850a478849bffd9510dc030910fc8aa3 -> <exact HEAD>` candidate diff and rerun the canonical CTX-03 validation surface;
4. challenge all three envelope layers: base reads, every checker-owned fixed conditional profile source, and concrete H1/CITY/PA minimum/escalated route sources;
5. reproduce the REVIEW_READY same-SHA negative→repair→GREEN class;
6. verify no repository-local final-clean write is needed after the review;
7. only if no known in-claim blocker remains, create the durable GitHub issue-comment clean record targeting the exact HEAD;
8. derive/freeze Ready metadata for that same SHA without changing repository bytes.

Until that sequence completes, CTX-03 remains **NOT READY** for a fresh independent Reviewer.
