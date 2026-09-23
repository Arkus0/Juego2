# repair-workpack

Repair exactly one explicitly identified Juego2 workpack after an independent Reviewer material FAIL.

This is a fresh Worker role. It never inherits Reviewer authority.

## Authority

Read the exact WP, latest independent review, relevant accepted predecessor contract, `AGENTS.md`, and `Docs/engineering/PRODUCT_SHA_CLOSURE.md`. The amendment governs Action cadence, same-SHA protocol repair and DocSync cost; it does not weaken the material blocker or WP acceptance criteria.

Accepted predecessor capsules remain governed by `Docs/engineering/CONTEXT_CAPSULE_V1.md`: use them only for navigation and escalate to exact authoritative sources when stale, lossy, contradictory or material to the repair.

For H1 work requiring Unity/local execution, also follow `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md` and rerun local evidence only when the repair can materially invalidate it.

## Flow

1. Reconstruct the canonical PR, latest reviewed SHA and latest independent verdict from live GitHub.
2. Distinguish **material FAIL** from `PROTOCOL_FIX` / `REVIEW_BLOCKED`. Do not edit product code for a pure lifecycle/metadata defect.
3. For a material FAIL, return the PR to Draft + ACTIVE and repair the causal blocker class, not just the reviewer's literal example.
4. Preserve accepted predecessor guarantees and previously closed blocker classes unless the new repair contradicts them.
5. Run only tests/evidence affected by the repair plus the exact WP gates needed to show the candidate as a whole still satisfies its claim. Avoid unrelated re-proof.
6. During Draft, **Arkus Main Safety** is the normal hosted .NET preflight. A same-PR, same-SHA Main Safety GREEN satisfies the hosted preflight requirement when local pinned .NET is unavailable. The dedicated Worker Candidate Preflight workflow is manual fallback only.
7. Finish all repository/evidence bytes, stop writers and select the final exact HEAD as `PRODUCT_SHA`.
8. Obtain exact-SHA preflight evidence, then perform one strict Worker pre-review against the complete repaired candidate.
9. If that pre-review finds a material blocker, repair it and repeat only from the new material SHA. If it finds only same-SHA metadata defects, repair those without another product campaign.
10. Persist CLEAN outside Git bytes, reconcile handoff once, freeze the same `PRODUCT_SHA`, mark Ready, and let `Arkus Candidate Validation` perform the final WP-specific freeze verification.
11. STOP for a fresh independent Reviewer.

## Protocol-only repair

If the Reviewer or automation identifies only malformed/stale handoff metadata while the exact `PRODUCT_SHA` and material evidence remain trustworthy:

- do not create a code/evidence commit;
- do not rerun Main Safety, Unity or WP product tests;
- do not repeat semantic Worker pre-review;
- correct all metadata in one pass and perform at most one deliberate Ready/recheck.

Any new Git commit is material and creates a new `PRODUCT_SHA`; GitHub-side comments/body/check changes do not.
