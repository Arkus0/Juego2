# implement-workpack

Implement exactly one authorized Juego2 workpack.

## Preconditions

- Read `AGENTS.md`, `Docs/ROADMAP.md`, the exact WP, `WORKER_REVIEW_PROTOCOL.md`, and `FOUNDATIONAL_PROOF_STANDARD.md` when bound.
- Reconstruct current `main`, dependency satisfaction, open ownership and write-set conflicts.
- Do not start a blocked downstream WP; apply `DEPENDENCY_ROUTING.md`.

## Workflow

1. Record baseline SHA and ownership.
2. Work only while Draft + ACTIVE.
3. Implement only Allowed scope.
4. Run exact positive tests.
5. Build a completeness inventory for finite surfaces.
6. For foundational claims, self-attack material defect classes and prove causal RED→GREEN.
7. Prefer an independent/effective oracle where self-confirmation is possible.
8. Record exact commands, outputs, residual risk and candidate SHA in evidence.
9. Perform the mandatory adversarial Worker pre-review while still Draft + ACTIVE:
   - re-read the complete WP/DoD and binding protocol/proof rules;
   - inspect the complete baseline→candidate diff, not only the latest change;
   - try to falsify acceptance, negative/error paths, scope, completeness, fail-closed behaviour and handoff requirements;
   - search for omission classes or effective behaviour outside any claimed proof universe;
   - for foundational WPs, attack material completeness/oracle/false-green assumptions with causal self-attacks;
   - classify findings as local/trivial versus causal architectural/proof-boundary defects.
10. If pre-review finds anything blocking, keep Draft + ACTIVE, repair the causal defect boundary, rerun affected tests/evidence, and repeat step 9. Do not freeze a knowingly defective candidate.
11. Record `WORKER_PRE_REVIEW: CLEAN`, findings fixed count and evidence path only when no known blocker remains. `CLEAN` is Worker readiness evidence, never independent `PASS`.
12. Stop all writers.
13. Freeze exact 40-char candidate SHA; set `FROZEN_FOR_REVIEW`; mark Ready.
14. STOP. Never act as the independent Reviewer or start the next WP.

Any implementation/evidence mutation after a clean pre-review invalidates that cleanliness and requires the pre-review to be rerun before freeze.

Missing required evidence, skipped mandatory tests, unavailable proof tooling or unresolved material doubt are FAIL/NOT_READY, never green by omission.
