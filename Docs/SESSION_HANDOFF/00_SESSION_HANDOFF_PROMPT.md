# Juego2 — Session Handoff Router

This file is a compact resumption router, not a state/semantic authority.

## Start here

1. Read `AGENTS.md`.
2. Read `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`.
3. Identify the explicitly requested role and exact WP/gate/task.
4. Use `Docs/engineering/context-bootstrap-profiles.json` for that role's minimum starting pack.
5. Query live GitHub for the mutable state required by the profile: current `main`, PR/branch ownership, HEAD, review/check/merge state as applicable.
6. If using mutable hints from `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json`, first require exact equality:

   ```text
   generated_from_main_sha == current live main SHA
   ```

   `scripts/context-bootstrap-check.py --current-main-sha <LIVE_MAIN_SHA> --require-fresh` is the mechanical check. A stale/missing/malformed index invalidates its mutable hints; reconstruct from live GitHub + authoritative contracts instead.
7. Read the exact contract(s) selected by the profile and make the routing decision observable as `CONTEXT_CLOSED`, `ESCALATE:<reason>` or `BLOCKED:<reason>`.
8. Escalate monotonically whenever the profile/protocol requires it. Missing context is never permission to infer from silence.

## Current-state navigation

Use `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` only as a derived navigation projection. It points to current H0/H1/CITY/PA/CTX state and cross-track hints, with authoritative source pointers.

Do **not** duplicate accepted history here. Exact accepted contracts/evidence, ROADMAP where materially required, and live GitHub remain the sources used to make decisions.

## Role boundaries preserved

- Exact Worker/repair/Reviewer sessions still obey `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` and mandatory predecessor/review duties.
- `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is loaded when the exact claim binds it, not merely because foundational history exists elsewhere.
- Full `Docs/ROADMAP.md` is conditional for exact-WP sessions and mandatory when cross-track/order/gate meaning is not closed by the exact/direct sources. Planner/milestone work normally starts with ROADMAP.
- H1 local execution remains the narrow mechanical role defined by `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`; it returns `REMOTE_DECISION_REQUIRED` rather than acquiring missing Worker authority.
- Derived index/handoff content is never independent Reviewer proof.

## Update rule

After an accepted merge, DocSync reconstructs live state, regenerates `ACCEPTED_STATE_INDEX.json` from authoritative sources with the exact live `generated_from_main_sha`, updates this router only if routing rules changed, emits normal `DOCSYNC_COMPLETE`, and names the dependency-valid next action.

If `main` advances afterward, the exact SHA mismatch intentionally makes mutable index hints stale until a later refresh.
