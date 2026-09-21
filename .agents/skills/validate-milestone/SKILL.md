# validate-milestone

Validate a milestone/gate from accepted repository state.

Start with `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `planner_gate` profile in `Docs/engineering/context-bootstrap-profiles.json`. Use the derived accepted-state index only for stale-detected navigation; gate composition must be established from exact constituent contracts/evidence and live GitHub state.

1. Read the exact milestone/gate contract and all constituent WP dependencies.
2. Verify every required WP is accepted/merged at the required exact state.
3. Verify no known predecessor falsification remains open.
4. Re-run or inspect gate-level end-to-end evidence that is not reducible to merely summing unit tests.
5. For foundational gates, challenge whether constituent proofs compose into the gate claim.
6. Verify final documentation/architecture/evidence surfaces are synchronized.
7. Emit PASS/FAIL/BLOCKED naming the exact gate state and evidence.

A milestone is not complete merely because its final numbered WP merged.
