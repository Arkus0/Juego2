# PRODUCT_SHA and non-material closure

Status: binding anti-loop clarification for Automation V2 after merge to `main`.

## Problem

The repository historically used one exact Git SHA for both product/proof identity and every final handoff step. PR-body reconciliation can therefore trigger `pull_request.edited`, which starts Candidate Validation again even though no repository byte changed. A mechanically red handoff could force the Worker to repeat an already-green exact-SHA product verifier, then rewrite metadata again, producing a protocol loop whose cost is unrelated to product change.

This clarification preserves exact-SHA integrity while separating immutable candidate execution from mutable closure metadata.

## PRODUCT_SHA

For an implementation/repair cycle, `PRODUCT_SHA` is the exact Git commit that the Worker freezes for independent review. In the existing handoff schema it is the value carried by `Candidate HEAD SHA` and `Frozen candidate SHA`; no new required PR-body field is introduced by this hotfix.

`PRODUCT_SHA` remains strict:

- it must be an exact 40-character Git SHA;
- it must equal the live PR HEAD at freeze/review;
- any Git commit after freeze creates a new product candidate and invalidates prior candidate validation, Worker pre-review, `REVIEW_READY`, and any Reviewer verdict for the previous SHA;
- changing effective `WP`, `PROCESS_ONLY`, or accepted proof class changes the validation context and forbids reuse even when Git HEAD is unchanged.

## Non-material closure

A change is `NON_MATERIAL_CLOSURE` only when all of the following are true:

1. no Git commit is created and live PR HEAD remains the same `PRODUCT_SHA`;
2. effective WP, process mode and proof class remain unchanged;
3. the change is limited to GitHub-side lifecycle metadata such as PR body handoff fields, issue comments, check/status records, review-ready markers or equivalent closure bookkeeping;
4. current mechanical handoff validation is rerun against the live metadata and exact `PRODUCT_SHA`;
5. any reused product validation comes from an immutable prior run whose exact-SHA verify job is GREEN and whose recorded validation context matches the current context exactly.

A non-material closure correction does **not** invalidate the exact-SHA product execution or require another semantic Worker pre-review. It must not be represented by a repository commit. If a repository/evidence byte must change, the correction is material and the normal new-SHA cycle applies.

## Candidate Validation behavior

For a Ready PR-body `edited` event, Candidate Validation still reruns the cheap live gates (`Worker handoff lint` and validation-context binding). The expensive `Freeze exact-SHA validation` job may reuse prior immutable exact-SHA validation only when it can prove all of the following:

- source workflow is `Arkus Candidate Validation`;
- source run targets the same exact `PRODUCT_SHA`;
- source `Freeze exact-SHA validation` job concluded `success`;
- source `Validation context binding` job concluded `success`;
- source `VALIDATION_CONTEXT.json` still matches the current PR + SHA + WP + process/proof class;
- when an `EXECUTION_RECEIPT_V1` is present, its WP and candidate SHA match the current request.

The source workflow does not need an overall `success` conclusion: it may have been red solely because the old mutable handoff metadata was invalid. The immutable product verify job is the reusable fact; the current run independently rechecks the mutable handoff.

If no qualifying source exists, Candidate Validation runs the full verifier exactly as before.

## Reviewer boundary

Independent review remains exact-SHA and is not weakened. A Reviewer must review `PRODUCT_SHA`, and any Git commit invalidates that verdict.

Mechanical closure defects should be caught before a Reviewer is started. Fixing only `NON_MATERIAL_CLOSURE` metadata on the same `PRODUCT_SHA` is not a new product candidate and must not create a new semantic review cycle. A protocol defect is a material Reviewer blocker when it breaks candidate identity, provenance, acceptance evidence, role independence, or another integrity guarantee—not merely because derivable GitHub metadata required same-SHA reconciliation.

## Intent

The invariant is now:

`product/proof bytes changed -> new PRODUCT_SHA -> full validation + pre-review + independent review`

`only GitHub closure metadata changed -> same PRODUCT_SHA -> cheap live gates + proven reuse -> continue closure`

This is an anti-loop rule, not a relaxation of exact-SHA review.