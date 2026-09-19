# update-handoff

Keep `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` compact and reconstructible.

Update only from accepted GitHub evidence. Include:

- current `main` SHA;
- active milestone and next contractual target;
- open WP/PR ownership and state;
- frozen/reviewed candidate SHA when applicable;
- last accepted merges/gates;
- durable blocks/human decisions;
- exact files a fresh session must read first.

When this skill is used for post-merge DocSync/finalization, reconstruct current `main`, open ownership and dependencies first. After reconciliation is complete, persist exactly one `ARKUS_AUTOMATION_V2` comment on the merged implementation PR with:

- `State: DOCSYNC_COMPLETE`;
- an idempotent `Key: docsync-complete:<PR>:<reconciled-main-sha>`;
- `WP: <WP-ID>`;
- `Next WP: <dependency-valid next WP, or NONE>`;
- a short `Detail:` describing the reconciliation result.

The marker is notification/handoff metadata, not authority. Never emit it before DocSync is actually complete.

Do not copy private reasoning, transient chat history, long logs or stale implementation details. Handoff summarizes; it never outranks ROADMAP, WP contracts, code or evidence.