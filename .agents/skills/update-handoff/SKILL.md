# update-handoff

Keep `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` compact and reconstructible.

## Context bootstrap

Use `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `docsync` profile in `Docs/engineering/context-bootstrap-profiles.json`. Reconstruct live GitHub state first.

`Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` is derived navigation only. Its live freshness contract is non-self-referential:

```text
projection_phase == DOCSYNC_PERSISTED
AND
generated_from_main_sha == first_parent(current live main)
```

`generated_from_main_sha` names the source `main` reconstructed before the DocSync persistence commit/merge. Never attempt to store the SHA of the commit that contains the index.

After an accepted implementation transition:

1. reconstruct current live `main` as `SOURCE_MAIN_SHA`, accepted merge/review and dependency-valid next action;
2. regenerate the accepted-state index from authoritative sources;
3. set `projection_phase=DOCSYNC_PERSISTED` and `generated_from_main_sha=SOURCE_MAIN_SHA`;
4. after CTX-02 adoption, if the accepted transition creates/changes a canonical accepted PA result, create/update its one accepted-contract capsule from the exact accepted identity/result and update `Docs/engineering/context-capsules/index.json`; preserve every disposition key/status and never infer missing rows;
5. after CTX-02 adoption, whenever capsule/index state is affected run the full CTX-02 validation surface: `python3 scripts/context-capsule-check.py --self-test`, `python3 scripts/context-capsule-controls.py`, `python3 scripts/context-capsule-pa-semantic-controls.py`, and `python3 scripts/context-capsule-check.py --audit-index --repo-root .`; a missing/invalid accepted PA capsule or failed independent semantic control means capsule-chain/navigation coverage is incomplete, not that the authoritative PA result is invalid;
6. persist DocSync in a direct child/merge whose first parent is exactly `SOURCE_MAIN_SHA`;
7. if main moved before persistence, stop and regenerate from the new source;
8. query the resulting live main SHA and first parent and run `python3 scripts/context-bootstrap-check.py --current-main-sha <DOCSYNC_MAIN_SHA> --current-main-parent-sha <SOURCE_MAIN_SHA> --require-fresh`;
9. only after that check passes emit exactly one `ARKUS_AUTOMATION_V2` comment on the merged implementation PR with `State: DOCSYNC_COMPLETE`, `Key: docsync-complete:<PR>:<reconciled-main-sha>`, `WP: <WP-ID>`, `Next WP: <dependency-valid next WP, or NONE>`, and a short `Detail:`.

The marker is notification/handoff metadata, not authority. Never emit it before DocSync is actually complete.

Accepted-contract capsules are also navigation only. They use contract-scoped identity/source freshness from `Docs/engineering/CONTEXT_CAPSULE_V1.md`, not the accepted-state index's first-parent freshness. Do not rewrite an unchanged capsule merely because unrelated `main` advanced.

Reconcile affected accepted milestone/track state, open WP/PR ownership relevant to next action, frozen/reviewed/merged exact SHA, durable blocks/human decisions, and role-routing changes only when the accepted contract changed them.

Read full `Docs/ROADMAP.md` when the next-action decision contains a cross-track/order/gate question not closed by exact accepted/direct dependency contracts. Missing/stale compact context causes escalation, never inference from silence.

Do not copy private reasoning, transient chat history, long logs or stale implementation details. Handoff/index/capsules summarize and navigate; they never outrank live GitHub, ROADMAP when materially required, exact WP contracts, code or accepted evidence.
