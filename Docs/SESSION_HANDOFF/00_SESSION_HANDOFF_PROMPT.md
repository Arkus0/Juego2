# Juego2 — Session Handoff Router

This file is a compact resumption router, not a state/semantic authority.

## Start here

1. Read `AGENTS.md`.
2. Read `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`.
3. Identify the explicitly requested role and exact WP/gate/task.
4. Use `Docs/engineering/context-bootstrap-profiles.json` for that role's minimum starting pack.
5. Query live GitHub for mutable state required by the profile.
6. Treat `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` as derived navigation only. Mutable main-derived hints are usable only when:

   ```text
   projection_phase == DOCSYNC_PERSISTED
   AND
   generated_from_main_sha == first_parent(current live main)
   ```

   `generated_from_main_sha` is the source `main` commit that DocSync reconstructed **before** the commit/merge that persists the projection. It is not the containing commit SHA.

   Mechanical live check:

   ```bash
   python3 scripts/context-bootstrap-check.py --current-main-sha <LIVE_MAIN_SHA> --current-main-parent-sha <LIVE_MAIN_FIRST_PARENT_SHA> --require-fresh
   ```

   A `CANDIDATE`, stale, missing, malformed or contradictory index invalidates mutable hints; reconstruct from live GitHub + authoritative contracts instead.
7. Read exact contracts selected by the profile and make routing observable as `CONTEXT_CLOSED`, `ESCALATE:<reason>` or `BLOCKED:<reason>`.
8. Escalate monotonically whenever required. Missing context is never permission to infer from silence.

## Role boundaries preserved

- Exact Worker/repair/Reviewer sessions still obey `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` and mandatory predecessor/review duties.
- `FOUNDATIONAL_PROOF_STANDARD.md` is loaded when the exact claim binds it.
- Full `Docs/ROADMAP.md` is conditional for exact-WP sessions and mandatory when cross-track/order/gate meaning remains unresolved.
- H1 local execution remains the narrow mechanical role defined by `H1_REMOTE_LOCAL_EXECUTION.md`.
- Derived index/handoff content is never independent Reviewer proof.

## Update rule

After accepted implementation merge, DocSync reconstructs live `main` as `SOURCE_MAIN_SHA`, regenerates the index with `projection_phase=DOCSYNC_PERSISTED` and `generated_from_main_sha=SOURCE_MAIN_SHA`, and persists it in a commit/merge whose first parent is exactly `SOURCE_MAIN_SHA`.

If `main` moves before persistence, regenerate from the new source. After persistence, run the live first-parent freshness check above before emitting `DOCSYNC_COMPLETE`. Any later main advance makes the projection stale until refreshed.
