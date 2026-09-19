# WP Continuity Policy — Juego2

Version: 1.2 — 2026-09-19

Complements `WORKER_REVIEW_PROTOCOL.md` with manual post-review/merge continuity.

## After PASS

A WP is merge-ready only when:

1. independent Reviewer validated the exact Frozen candidate SHA;
2. exact-SHA PASS is persisted;
3. implementation has not changed after the reviewed SHA;
4. mandatory validation/evidence is complete;
5. any strictly allowed reviewer finalization is documentation-only.

Do not churn ROADMAP for every commit. Update global planning only when accepted WP/gate results materially change milestone/dependency state.

## After merge

Perform DocSync/reconciliation manually before selecting another WP. Inspect only the surfaces actually affected by the accepted result:

- exact WP status/contract metadata;
- `Docs/ROADMAP.md` when milestone/dependency state changed;
- architecture/ADR when accepted architecture changed;
- evidence indexes when applicable;
- `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md`.

If no documentation content change is needed, record that fact in the merge/finalization evidence.

After reconciliation, persist exactly one completion marker on the merged implementation PR:

```text
ARKUS_AUTOMATION_V2
State: DOCSYNC_COMPLETE
Key: docsync-complete:<PR>:<reconciled-main-sha>
WP: <WP-ID>
Next WP: <dependency-valid next WP, or NONE>
Detail: <short reconciliation result>
```

The marker is durable handoff/notification metadata only. It does not replace the underlying accepted GitHub evidence. `Next WP` must be resolved from current `main`, open ownership and dependency state after DocSync; never copy a stale value from an earlier session.

## Compact handoff

Keep only information a fresh session needs: latest accepted WP/SHA, current milestone, active ownership, durable blockers and next dependency-satisfied action. Never use handoff as a session diary.

## New WP ownership

A merged WP reserves no future WP for its Worker. Reconstruct current `main`, resolve dependencies again, then start a fresh branch/WP manually.

## Future local/editor evidence

When H1 introduces Unity, local editor evidence belongs to the exact environment that actually executed it. Cloud actors may prepare but never fabricate local/editor validation.