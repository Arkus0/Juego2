# update-handoff

Keep `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` compact and reconstructible.

## Context bootstrap

Use `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `docsync` profile in `Docs/engineering/context-bootstrap-profiles.json`. Reconstruct live GitHub state first. `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` is derived navigation only: if its `generated_from_main_sha` differs from live `main`, its mutable hints are stale and may not decide `Next WP`.

Update only from accepted GitHub evidence. The handoff itself is now a router rather than a duplicate state/history store. DocSync must regenerate `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` from authoritative accepted sources and set `generated_from_main_sha` to the exact live main SHA from which the projection was derived. Every mutable hint keeps authoritative source pointers.

After an accepted transition, reconcile:

- current live `main` SHA;
- accepted milestone/track state affected by the merge;
- open WP/PR ownership relevant to the next action;
- frozen/reviewed/merged exact SHA where applicable;
- durable blocks/human decisions;
- dependency-valid next action;
- role-routing changes only if the accepted contract changed them.

Read full `Docs/ROADMAP.md` when the next-action decision contains a cross-track/order/gate question not closed by the exact accepted/direct dependency contracts. Missing/stale compact context causes escalation, never inference from silence.

When this skill is used for post-merge DocSync/finalization, reconstruct current `main`, open ownership and dependencies first. After reconciliation is complete, persist exactly one `ARKUS_AUTOMATION_V2` comment on the merged implementation PR with:

- `State: DOCSYNC_COMPLETE`;
- an idempotent `Key: docsync-complete:<PR>:<reconciled-main-sha>`;
- `WP: <WP-ID>`;
- `Next WP: <dependency-valid next WP, or NONE>`;
- a short `Detail:` describing the reconciliation result.

The marker is notification/handoff metadata, not authority. Never emit it before DocSync is actually complete.

Do not copy private reasoning, transient chat history, long logs or stale implementation details. Handoff/index summarize and navigate; they never outrank live GitHub, ROADMAP when materially required, exact WP contracts, code or accepted evidence.
