# implement-workpack

Implement exactly one explicitly authorized Juego2 workpack.

## Preconditions

- Read `AGENTS.md`, `Docs/ROADMAP.md`, the exact WP, `WORKER_REVIEW_PROTOCOL.md`, and `FOUNDATIONAL_PROOF_STANDARD.md` when bound.
- Reconstruct current `main`, dependency satisfaction, open ownership and write-set conflicts.
- Before editing, read each direct accepted dependency WP plus its accepted completion/PASS evidence, relevant proof matrix/residual risk, and binding architecture/invariants. Follow transitive predecessors only when those inherited invariants are material to the current WP.
- Persist a short `PREDECESSOR_CONTRACT_CHECK` in Worker plan/evidence identifying inherited guarantees, current-WP-owned guarantees, guarantees intentionally consumed rather than re-proved, and the concrete trigger that would justify reopening an accepted predecessor boundary.
- Do not auto-route to another workpack. If the requested WP is blocked, report the blocking prerequisite and STOP so the human can choose the next Worker task.

## Workflow

1. Record baseline SHA and ownership.
2. Complete the mandatory predecessor contract check before implementation.
3. Work only while Draft + ACTIVE.
4. Implement only Allowed scope.
5. Run exact positive tests through the canonical validation entrypoint.
6. Build a completeness inventory for finite in-claim surfaces where required.
7. For foundational claims, self-attack material defect classes inside the declared trust boundary and prove causal RED→GREEN.
8. Prefer an independent/effective oracle where self-confirmation is possible.
9. Record exact commands, outputs, residual risk and candidate SHA in evidence.
10. Perform the mandatory adversarial Worker pre-review while still Draft + ACTIVE:
   - re-read the complete WP/DoD and binding protocol/proof rules;
   - verify the predecessor contract check still matches the accepted dependency state and avoid duplicating inherited proof unless concrete evidence invalidates it;
   - inspect the complete baseline→candidate diff, not only the latest change;
   - try to falsify acceptance, negative/error paths, scope, completeness, fail-closed behaviour and handoff requirements;
   - search for omission classes/effective behaviour inside the declared claim;
   - respect the proof budget and trusted base instead of expanding proof into arbitrary toolchain subversion;
   - classify findings as local/trivial versus causal architectural/proof-boundary defects.
11. If pre-review finds an in-claim blocker, keep Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence, and repeat step 10.
12. Record `WORKER_PRE_REVIEW: CLEAN`, findings fixed count and evidence path only when no known in-claim blocker remains. `CLEAN` is Worker readiness evidence, never independent `PASS`.
13. Stop all writers.
14. Freeze exact 40-char candidate SHA; set `FROZEN_FOR_REVIEW`; mark Ready.
15. STOP. Never act as the independent Reviewer or start the next WP.

Any implementation/evidence mutation after a clean pre-review invalidates that cleanliness and requires the pre-review to be rerun before freeze.

Missing required evidence, skipped mandatory tests, unavailable proof tooling or unresolved material doubt are FAIL/NOT_READY, never green by omission.
