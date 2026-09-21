# CTX-01 — Final rerun (repair cycle 1)

Status: **GREEN / READY_FOR_WORKER_PRE_REVIEW**

## Mechanical checker

Executed against the repaired checker implementation in the Worker environment:

```text
python3 scripts/context-bootstrap-check.py --self-test
context-bootstrap self-test: PASS
exit 0
```

The self-test covers required role-profile shape; exact source-anchor match; persisted first-parent source => FRESH; later main advance => STALE; candidate phase => STALE; mismatched source anchor => STALE; missing reviewer profile => fail closed; missing compact context => fail closed.

## Repair-specific source anchor

The candidate index declares `projection_phase=CANDIDATE`, `freshness_contract=docsync-first-parent-v1`, and `generated_from_main_sha=a85954539e0ef397009e87af322eb735d58ccc0d`.

That SHA is the exact restored accepted `main` after revert PR #109 and includes retained PR #108. Candidate/source validation therefore has a concrete non-self-referential anchor while live freshness remains false until post-PASS DocSync.

## Full CTX-01 controls

1. non-foundational conditionality — PASS;
2. cross-track deepening before eligibility — PASS;
3. stale compact state cannot override live GitHub — PASS;
4. index remains `DERIVED_NAVIGATION_ONLY` — PASS;
5. Reviewer independently opens authoritative predecessor/frozen evidence — PASS;
6. H1 local executor remains narrower / `REMOTE_DECISION_REQUIRED` — PASS;
7. first-parent freshness control is satisfiable and stales after any later main advance — PASS;
8. missing compact context escalates/fails closed — PASS.

## Scope rerun

The repaired candidate changes only context bootstrap routing/freshness, its exact workpack amendment, and Worker evidence. It does not change product/runtime semantics, accepted predecessor semantics, CTX-02/03 ownership, H1 local discretion, or unrelated future planning. PR #108's `Docs/workpacks/H2/FUTURE_PLANNING_SIGNAL.md` is retained exactly from current main.
