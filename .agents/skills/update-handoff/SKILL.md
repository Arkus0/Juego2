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

1. reconstruct current live `main` as `SOURCE_MAIN_SHA`, accepted merge/review, exact frozen candidate/validation run and dependency-valid next action;
2. for a standard accepted `WP-*` transition, run the checker-owned mechanical core once:

   ```text
   python3 scripts/docsync.py prepare \
     --wp <WP-ID> \
     --candidate <FROZEN_SHA> \
     --review <REVIEW_ID> \
     --pr <PR_NUMBER> \
     --merge <MERGE_SHA> \
     --validation-run <RUN_ID> \
     --source-main <SOURCE_MAIN_SHA>
   ```

   This command may update only the accepted WP Status/Acceptance identity, its generated `Docs/evidence/<WP>/DOCSYNC.md` core, and the derived accepted-state index. It does not synthesize semantic accepted claims or free-form README prose. If the transition is nonstandard, fail closed to manual reconstruction rather than weakening the command;
3. reconcile only authoritative/current-state docs whose effective accepted meaning actually changed. Do not copy the same accepted transition into ROADMAP, handoff, track README and evidence merely to preserve chronology; long closure chronology belongs under exact evidence or `Docs/history/` with stable pointers back to the accepted PR/review/SHA;
4. run `python3 scripts/docsync.py check --wp <WP-ID>` for standard WP transitions after reconciliation. A failed identity/index check blocks `DOCSYNC_COMPLETE`;
5. if the accepted transition creates/changes a canonical accepted PA result, create/update its one accepted-contract capsule from the exact accepted identity/result and update `Docs/engineering/context-capsules/index.json`; preserve every disposition key/status and never infer missing rows. `WP-PA-06+` authoritative results use the checker-owned `Docs/engineering/PA_DISPOSITION_TABLE_V1.md` convention; PA-01..05 retain their frozen reviewed selectors;
6. whenever capsule/index state is affected run the full CTX-02 validation surface: `python3 scripts/context-capsule-check.py --self-test`, `python3 scripts/context-capsule-controls.py`, `python3 scripts/context-capsule-omission-controls.py`, `python3 scripts/context-capsule-pa-semantic-controls.py`, and `python3 scripts/context-capsule-check.py --audit-index --repo-root .`; a missing/invalid accepted PA capsule, failed standard selector, or failed independent semantic control means capsule-chain/navigation coverage is incomplete, not that the authoritative PA result is invalid;
7. keep `Docs/history/**` out of normal role bootstrap. Historical material is read only when a current claim, contradiction or audit needs it. Preserve accepted evidence; never delete it for context savings;
8. persist DocSync in a direct child/merge whose first parent is exactly `SOURCE_MAIN_SHA`;
9. if main moved before persistence, stop and regenerate from the new source;
10. query the resulting live main SHA and first parent and run `python3 scripts/context-bootstrap-check.py --current-main-sha <DOCSYNC_MAIN_SHA> --current-main-parent-sha <SOURCE_MAIN_SHA> --require-fresh`;
11. only after the mechanical check, capsule checks when applicable, semantic reconciliation and live-main freshness check pass, emit exactly one `ARKUS_AUTOMATION_V2` comment on the merged implementation PR with `State: DOCSYNC_COMPLETE`, `Key: docsync-complete:<PR>:<reconciled-main-sha>`, `WP: <WP-ID>`, `Next WP: <dependency-valid next WP, or NONE>`, and a short `Detail:`.

The marker is notification/handoff metadata, not authority. Never emit it before DocSync is actually complete.

Accepted-contract capsules are also navigation only. They use contract-scoped identity/source freshness from `Docs/engineering/CONTEXT_CAPSULE_V1.md`, not the accepted-state index's first-parent freshness. Do not rewrite an unchanged capsule merely because unrelated `main` advanced.

Reconcile affected accepted milestone/track state, open WP/PR ownership relevant to next action, frozen/reviewed/merged exact SHA, durable blocks/human decisions, and role-routing changes only when the accepted contract changed them.

Read full `Docs/ROADMAP.md` when the next-action decision contains a cross-track/order/gate question not closed by exact accepted/direct dependency contracts. Missing/stale compact context causes escalation, never inference from silence.

Do not copy private reasoning, transient chat history, long logs or stale implementation details. Handoff/index/capsules summarize and navigate; they never outrank live GitHub, ROADMAP when materially required, exact WP contracts, code or accepted evidence.
