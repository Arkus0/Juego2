# repair-workpack

Repair exactly one explicitly identified Juego2 workpack after an independent Reviewer FAIL.

This is a **fresh Worker role**, never a continuation of the Reviewer session that emitted FAIL.

## Trigger

Use this skill for requests such as:

- `Corrige el FAIL de H1-02`
- `Repair WP-H1-02`
- `Fresh repair Worker CITY-04`

Resolve the exact WP ID and canonical PR from live GitHub state. Do not infer a different WP.

## Context bootstrap

Start with `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `repair_worker` profile in `Docs/engineering/context-bootstrap-profiles.json`. The profile narrows only the initial pack. Full ROADMAP/proof/architecture context is loaded when the FAIL, exact WP, direct predecessor state or cross-track gate makes it material. After CTX-02 adoption, `Docs/engineering/CONTEXT_CAPSULE_V1.md` governs validated accepted-contract capsules that may navigate unchanged accepted predecessor guarantees, but any capsule mismatch or FAIL that may reopen/touch that predecessor escalates to the exact authoritative sources. Stale/missing compact context always escalates; it never supplies a repair assumption.

After CTX-03 adoption, terminal closure and mechanical outcome classification follow `Docs/engineering/CONTEXT_ENVELOPE_V1.md`. Its deterministic epilogue below is sufficient for normal repairs; load the full protocol only when an envelope/classification dispute is material.

## Preconditions

- Read `AGENTS.md`, `Docs/engineering/WORKER_REVIEW_PROTOCOL.md`, the exact WP, the original Worker evidence, and the latest independent Reviewer FAIL bound to an exact reviewed candidate SHA, as required by the repair profile.
- Reconstruct current `main`, the canonical implementation PR/branch, current PR HEAD, frozen/reviewed SHA fields, `fail_cycle`, dependency state and any later accepted predecessor changes.
- Use live GitHub state (`gh` in a local session, or an equivalent authenticated GitHub surface). A local Git checkout alone is not sufficient to reconstruct review state.
- Verify there is exactly one canonical open implementation PR for the WP. If ownership is ambiguous, STOP rather than guessing.
- Verify the latest independent verdict is actually FAIL/`REPAIR_REQUIRED` for this WP. A mechanical `REVIEW_BLOCKED` or `INFRA_ERROR` is **not** an independent Reviewer FAIL and does not create a semantic repair cycle by itself.

## Workflow

1. Identify the violated criterion, evidence, exact reviewed candidate SHA and minimal causal correction boundary from the independent FAIL.
2. Confirm whether the finding belongs to the current WP or concretely reopens an accepted predecessor. Do not silently repair a different ownership boundary; a capsule cannot settle a concrete reopen question by itself.
3. Return the canonical PR to the repair state required by `WORKER_REVIEW_PROTOCOL.md` (normally Draft + ACTIVE) and preserve Worker history, prior reviewed SHA, `fail_cycle` and existing evidence.
4. Check out/update the canonical repair branch. Do not create a competing implementation PR unless the protocol explicitly requires a migration; if migrated, mark the old PR superseded and preserve history.
5. Refresh `PREDECESSOR_CONTRACT_CHECK` if dependency/accepted predecessor state changed since the failed candidate. After CTX-02 adoption, unchanged accepted boundaries may start from valid capsules; any predecessor-related FAIL or contradiction requires authoritative escalation.
6. Reproduce or otherwise validate the Reviewer's blocker before changing the candidate when feasible.
7. Repair the **causal defect boundary**, not merely the reported example. Stay inside Allowed scope and do not opportunistically advance later WPs.
8. Run affected positive tests, causal negative-conformance/defect-injection controls, exact local Unity evidence when required, and any canonical validation needed by the WP.
9. Update evidence truthfully. Preserve superseded/failing evidence where the protocol requires history; never rewrite history to make the prior candidate appear green.
10. Rerun the complete mandatory Worker pre-review against the full repaired baseline→candidate diff. Any implementation/evidence mutation invalidates prior `WORKER_PRE_REVIEW: CLEAN`.
11. Record the new clean pre-review only when no known in-claim blocker remains.
12. Commit and push the repaired candidate to the canonical branch.
13. Stop all writers and read the exact 40-char HEAD. After CTX-03 adoption, use `scripts/derive-worker-review-metadata.py` to generate the derivable canonical Ready fields instead of hand-transcribing SHA/state/path values; then validate the PR body with the canonical handoff lint.
14. Set `FROZEN_FOR_REVIEW` / `Branch frozen: YES`, mark the PR Ready and stop repository/evidence writes.
15. Classify mechanical results before consuming another Reviewer round: registered causal `FAIL` returns to Worker repair; `REVIEW_BLOCKED` means repair/derive lifecycle metadata and rerun; `INFRA_ERROR` means diagnose/rerun infrastructure without calling the WP defective; `NOT_APPLICABLE` is neutral. An unregistered red verifier cannot by itself count as WP FAIL. Semantic/causal Reviewer authority is unchanged.
16. Observe the durable Automation V2 `REVIEW_READY` marker for the exact frozen SHA. After CTX-03 adoption, normal closure is automatic: `REVIEW_READY_CLOSED` is emitted only after the post-marker live HEAD still equals the frozen SHA and requires no additional human ceremony. If the automatic closure is not yet adopted for the current transition, perform the equivalent final live HEAD read after the marker without mutating repository bytes.
17. Only after terminal closure may the Worker report the repaired candidate ready for a **fresh independent Reviewer**. Then STOP. Never self-review, merge, DocSync or start the next WP from this repair context.

## Local Unity rule

For `LOCAL_UNITY_REQUIRED` or `HYBRID` WPs, required effective Unity evidence must be rerun on the repaired candidate when the fix can affect that evidence. Remote-only repair may end as `READY_FOR_LOCAL_VALIDATION`, never a false PASS-ready handoff.

## GitHub CLI rule for local sessions

When running locally, prefer authenticated `gh` for PR/review/check state and `git` for repository bytes/history. Before any mutation, verify:

```text
gh repo view --json nameWithOwner
```

resolves to `Arkus0/Juego2`, then identify the canonical PR and branch from live GitHub state. Never paste or store GitHub passwords/tokens in repository files or prompts.
