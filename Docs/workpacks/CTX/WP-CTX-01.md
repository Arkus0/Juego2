# WP-CTX-01 — Role-specific bootstrap + accepted-state navigation

Status: **FROZEN PLAN / NOT_STARTED**
Class: **PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL**
Execution: **REMOTE_OK**
Depends on: accepted CTX plan
Blocks: `WP-CTX-02` only

## Objective

Make fresh Juego2 agent sessions start from a small, role-specific, lossless context pack instead of mechanically loading broad project history, while preserving every existing semantic/proof/review obligation and forcing deeper reads whenever the current claim requires them.

This is the intentionally quick first CTX workpack so later sessions begin saving context immediately.

## Required inputs

- accepted Worker/Reviewer protocol;
- accepted H1 remote/local execution protocol from PR #99;
- current role skills (`implement-workpack`, `repair-workpack`, `validate-workpack`, planner/gate/DocSync helpers);
- current `AGENTS.md`, session handoff and track/workpack state surfaces;
- live GitHub examples of state drift / DocSync lag.

## Work

- define a binding context-bootstrap protocol with explicit authority ordering;
- define machine-readable minimum boot profiles for Worker, repair Worker, Reviewer, planner/gate, DocSync and H1 local executor;
- add a compact derived accepted-state navigation index with exact `generated_from_main_sha` stale detection;
- route role skills through those profiles;
- make full `Docs/ROADMAP.md` conditional for exact-WP sessions rather than mechanically mandatory when the exact WP/direct dependencies already close order/gate meaning;
- make `FOUNDATIONAL_PROOF_STANDARD.md` conditional on the current claim actually binding it;
- keep predecessor reconstruction, Worker pre-review, Reviewer independence and exact-SHA obligations unchanged;
- compact the session handoff into a router rather than a broad fixed read list;
- teach DocSync to refresh the derived state index without making it a second semantic authority.

## Forbidden

- no product/runtime semantics;
- no accepted-contract capsules yet;
- no structured proof/residual migration yet;
- no bulk ROADMAP/history rewrite yet;
- no deletion of historical evidence;
- no weakening of predecessor reads when materially required;
- no use of compact state/Worker prose as Reviewer proof authority;
- no broadening of the H1 local executor's discretion.

## Required process controls

At minimum prove these cases:

1. non-foundational PA harvest does not load unrelated foundational proof material by default, but does load it if the exact claim later binds it;
2. a cross-track dependency not closed by the exact WP triggers track/ROADMAP deepening before eligibility is decided;
3. stale README/index state cannot override later live GitHub state;
4. state index cannot create product semantics;
5. Reviewer may navigate through compact state but independently opens authoritative predecessor evidence when material;
6. H1 local executor remains narrower than remote Worker/Reviewer roles;
7. `generated_from_main_sha != current main` invalidates mutable next/block hints until reconciliation;
8. missing compact context causes escalation, never inference from silence.

## Acceptance

- each supported role has an explicit minimum boot profile and escalation rules;
- live GitHub vs semantic contracts vs derived summaries have unambiguous authority ordering;
- exact-WP Worker/Reviewer sessions can omit unrelated full-history/ROADMAP/proof surfaces at initial bootstrap without losing any material obligation;
- any material omission/ambiguity has a fail-safe path to authoritative deeper context;
- Reviewer independence is unchanged or stronger;
- existing H1 local-execution boundary remains unchanged;
- a fresh representative dry-run for `Worker PA-04`, `Worker H1-02`, `Reviewer <exact WP>` and blocked `CITY-04` demonstrates correct initial selection + escalation.

## Definition of Done

Context Bootstrap v1 is independently reviewed and merged, DocSync has refreshed its derived index/handoff, and subsequent exact-WP sessions can use the new role profiles immediately. Reviewer PASS names `WP-CTX-02` next.