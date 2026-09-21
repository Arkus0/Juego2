# WP-CTX-02 — Accepted-contract capsules + predecessor inheritance compression

Status: **FROZEN PLAN / NOT_STARTED**
Class: **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**
Execution: **REMOTE_OK**
Depends on: `WP-CTX-01` PASS + merge + DocSync
Blocks: `WP-CTX-03` only

## Objective

Stop downstream Workers/Reviewers from repeatedly re-deriving the same accepted predecessor guarantees from long historical evidence while preserving exact provenance, reopening conditions and independent Reviewer judgment.

## Required inputs

- accepted Context Bootstrap v1;
- current `WORKER_REVIEW_PROTOCOL.md` predecessor-contract rules;
- representative accepted boundaries needed by upcoming H1/CITY/PA work;
- original accepted WP/PASS/proof/residual evidence for those boundaries.

## Work

- define one compact accepted-contract capsule schema;
- bind each capsule to the exact accepted reviewed candidate/merge and authoritative source pointers;
- record exported guarantees, explicit exclusions/non-claims, and concrete reopen conditions;
- make capsules discoverable from role bootstrap/predecessor reconstruction;
- define mandatory escalation triggers from capsule to original evidence;
- preserve the Worker's auditable `PREDECESSOR_CONTRACT_CHECK` while allowing it to begin from accepted capsule(s) rather than blindly re-reading every predecessor narrative;
- preserve independent Reviewer reconstruction: Reviewer may use a capsule for navigation but must challenge material inherited guarantees against authoritative sources when needed;
- create capsules first for boundaries actually consumed by the near-term active tracks rather than bulk-migrating all H0 history.

## Forbidden

- no capsule may become a new semantic source of truth;
- no invented guarantee absent from accepted evidence;
- no silent omission of exclusions/reopen conditions;
- no automatic trust in Worker-authored predecessor classifications;
- no bulk historical rewrite simply to maximize coverage;
- no change to product/runtime contracts.

## Required controls

- source/capsule consistency check on representative accepted boundaries;
- stale capsule exact-SHA mismatch must fail closed / force source reconstruction;
- deliberately omit one material exported guarantee from a test capsule and prove the validation/review process detects the loss rather than silently narrowing the inherited contract;
- demonstrate that two directional relationships or similarly asymmetric accepted semantics are not collapsed by summary compression;
- prove a Reviewer can reopen a predecessor when concrete contradictory evidence exists even if the capsule says accepted.

## Acceptance

A fresh downstream Worker can reconstruct the inherited/current ownership split for representative H1, CITY and PA consumers from compact capsules + exact pointers with materially less repeated prose, while an independent Reviewer can still reach the same accepted guarantee boundary from original evidence and detect a lossy/stale capsule.

## Definition of Done

Accepted-contract capsules are a safe navigation layer for near-term predecessor boundaries; no semantic authority moved from original accepted evidence. Reviewer PASS names `WP-CTX-03` next.