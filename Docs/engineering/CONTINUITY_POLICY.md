# WP Continuity Policy — Juego2

Version: 1.3 — 2026-09-19

Complements `WORKER_REVIEW_PROTOCOL.md` with post-review/merge continuity.

## After PASS

A WP is merge-ready only when:

1. independent Reviewer validated the exact Frozen candidate SHA;
2. exact-SHA PASS is persisted;
3. implementation has not changed after the reviewed SHA;
4. mandatory validation/evidence is complete;
5. any strictly allowed reviewer finalization is documentation-only.

Once those conditions hold, routine continuity should **not** require another human/session handoff. The successful independent Reviewer session may immediately switch to finalization/DocSync mode and perform:

```text
PASS exact SHA
→ exact-SHA merge preflight
→ merge
→ documentation-only DocSync/reconciliation
→ DOCSYNC_COMPLETE + dependency-valid Next WP
```

This post-PASS continuation does not weaken Reviewer independence because the review verdict is already fixed before finalization begins. It may not alter implementation bytes, repair the reviewed candidate or manufacture missing evidence. If any implementation change is required, stop and reopen the Worker → Reviewer cycle instead.

Automation V2 may perform the mechanical exact-SHA merge before the Reviewer session reaches that step. If so, the same session continues directly with DocSync from current `main`.

Do not churn ROADMAP for every commit. Update global planning only when accepted WP/gate results materially change milestone/dependency state.

## After merge / DocSync

Before selecting another WP, inspect only the surfaces actually affected by the accepted result:

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

`DOCSYNC_COMPLETE` is also the normal high-value Telegram completion transition when notification secrets are configured.

## Compact handoff

Keep only information a fresh session needs: latest accepted WP/SHA, current milestone, active ownership, durable blockers and next dependency-satisfied action. Never use handoff as a session diary.

## New WP ownership

A merged WP reserves no future WP for its Worker or Reviewer. After `DOCSYNC_COMPLETE`, reconstruct current `main`, resolve dependencies again, then start the next WP as a fresh Worker context.

## Future local/editor evidence

When H1 introduces Unity, local editor evidence belongs to the exact environment that actually executed it. Cloud actors may prepare but never fabricate local/editor validation.
