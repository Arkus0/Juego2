# WP Continuity Policy — Juego2

Version: 1.0 — 2026-09-18

Complements `WORKER_REVIEW_PROTOCOL.md` with post-review/merge continuity.

## After PASS

A WP is merge-ready only when:

1. independent Reviewer validated the exact Frozen candidate SHA;
2. exact-SHA PASS is persisted;
3. implementation has not changed after the reviewed SHA;
4. mandatory evidence/checks are complete;
5. any strictly allowed reviewer finalization is documentation-only.

Do not churn ROADMAP for every commit. Update global planning only when accepted WP/gate results materially change milestone/dependency state.

## After merge

Implementation merge creates `DOCSYNC_PENDING`.

DocSync must inspect and reconcile the surfaces actually affected by the accepted result:

- exact WP status/contract metadata;
- `Docs/ROADMAP.md` when milestone/dependency state changed;
- architecture/ADR when the accepted architecture changed;
- evidence indexes when applicable;
- `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md`.

If no changes are needed, persist `DOCSYNC_NOOP` naming checked surfaces.

Only `DOCSYNC_COMPLETE`/`DOCSYNC_NOOP` releases the flow back to `DISCOVER` for the next WP.

## Compact handoff

Keep only information a fresh session needs: latest accepted WP/SHA, current milestone, active ownership, durable blockers and next dependency-satisfied action. Never use handoff as a session diary.

## New WP ownership

A merged WP reserves no future WP for its Worker. Reconstruct current `main`, resolve dependencies again, then claim a fresh branch/WP.

## Future local/editor evidence

When H1 introduces Unity, local editor evidence belongs to the exact environment that actually executed it. Cloud actors may prepare but never fabricate local/editor validation.
