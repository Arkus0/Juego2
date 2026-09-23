# update-handoff

Perform post-PASS DocSync as a **bounded delta reconciliation**, not a second review.

Read `Docs/engineering/PRODUCT_SHA_CLOSURE.md` first. Its Flow Simplification V2 DocSync budget governs this skill. Accepted-contract capsule mechanics remain governed by `Docs/engineering/CONTEXT_CAPSULE_V1.md`.

## Default: zero-commit DocSync

After the accepted implementation PR merges:

1. Confirm the merged PR, exact reviewed `PRODUCT_SHA`, PASS and merge SHA from live GitHub.
2. Determine the dependency-valid next action from direct accepted contracts/live state. Read full ROADMAP only when a cross-track/order/gate question is genuinely unresolved.
3. Ask one question: **did this accepted transition change the effective accepted meaning of an authoritative document that future work relies on?**
4. If **no**, make **no repository commit**. Do not regenerate `ACCEPTED_STATE_INDEX.json`, handoff summaries, capsules, history or track docs merely to record chronology.
5. Emit one durable PR comment:
   - `ARKUS_AUTOMATION_V2`
   - `State: DOCSYNC_COMPLETE`
   - `Key: docsync-complete:<PR>:<merge-sha>`
   - `WP: <WP-ID>`
   - `Next WP: <dependency-valid next WP or NONE>`
   - short `Detail:` stating that no authoritative document meaning required reconciliation.
6. STOP.

Derived navigation is non-authoritative. If an index/handoff/cache becomes stale because it was not ceremonially rewritten, a later role escalates to live GitHub/authoritative sources. Stale navigation alone does not block an accepted transition.

## When a DocSync commit is actually required

Create one bounded documentation reconciliation only when the accepted transition really changes authoritative durable meaning, for example:

- a ROADMAP gate/order/status that future dependency resolution consumes;
- an exact workpack/track state document that is itself authoritative for subsequent scope;
- an architecture/ADR decision;
- an accepted-contract capsule whose represented accepted contract actually changed.

Then:

1. Touch only documents whose effective accepted meaning changed.
2. Do not copy the same transition into multiple summaries for chronology.
3. Run only validators applicable to the files actually changed.
4. Never rerun product/.NET/Unity tests for documentation-only DocSync.
5. Do not restart merely because unrelated `main` advanced. Rebase/reconcile only if that movement conflicts with a document you are actually changing.
6. Persist one bounded DocSync commit/PR, confirm it merged, emit one `DOCSYNC_COMPLETE` marker, and STOP.

If and only if capsule/index content is genuinely edited, run the canonical full CTX-02 validation surface required by `CONTEXT_CAPSULE_V1.md`:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

These commands are conditional on changing that surface; they are not a mandatory tax after every PASS.

## Forbidden DocSync churn

Do not require a docs commit solely to:

- refresh a derived SHA/index after every merge;
- preserve a complete chronology in multiple files;
- make a compact cache perfectly fresh when live authoritative state is available;
- re-run accepted product proof;
- prove again that PASS was valid;
- reconcile unrelated documentation.

DocSync should normally take minutes, and often seconds, after PASS. If it starts reconstructing the entire project or launching broad validation, the scope has escaped this skill.
