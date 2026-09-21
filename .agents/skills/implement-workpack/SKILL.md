# implement-workpack

Implement exactly one explicitly authorized Juego2 workpack.

Juego2 / Arkus Harness is a game-development and software-verification project. Work is limited to repository-owned game-authoring code, fixtures, tests, CI and documentation. Legacy terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` refer only to internal negative/conformance testing; use the neutral terminology defined in `AGENTS.md` for new work.

## Trigger

Requests such as `Worker H1-02`, `Worker WP-H1-02`, `Worker CITY-04` or equivalent are explicit authorization to resolve that exact WP and execute this skill. The short command does not weaken any dependency, ownership, evidence, local-engine, freeze or review rule.

For a local session, reconstruct PR/ownership/check state from live GitHub through authenticated `gh` (or an equivalent live GitHub surface) rather than inferring it from the local Git checkout alone. Before GitHub mutations verify `gh repo view --json nameWithOwner` resolves to `Arkus0/Juego2` and that exactly one canonical implementation PR/branch owns the requested WP.

For `WP-H1-02` through `WP-H1-GATE`, the default orchestration is **remote-first staged execution** under `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`: the Worker performs `REMOTE_PREP`, delegates only the exact Unity/Windows-dependent slice through a persisted `LOCAL_EXECUTION.md`, then resumes as the same Worker for `REMOTE_CLOSEOUT`. `LOCAL_UNITY_REQUIRED` and `HYBRID` continue to require real local evidence; they do not require the whole Worker reasoning session to run locally.

## Preconditions

- Read `AGENTS.md`, `Docs/ROADMAP.md`, the exact WP, `WORKER_REVIEW_PROTOCOL.md`, and `FOUNDATIONAL_PROOF_STANDARD.md` when bound.
- For `WP-H1-02` through `WP-H1-GATE`, also read `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md` before planning execution.
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
7. For foundational claims, exercise material defect classes inside the declared trust boundary with causal negative-conformance / defect-injection tests and prove causal RED→GREEN.
8. Prefer an independent/effective oracle where self-confirmation is possible.
9. Record exact commands, outputs, residual risk and candidate SHA in evidence.
10. For H1-02..H1-GATE when effective local Unity/Windows execution remains necessary, do **not** hand the whole Worker role to the local agent. Commit the round's remote product/code/configuration state first as `EXECUTION_BASE_SHA`; then persist `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md` in a separate manifest-only direct-child commit. Push that manifest commit, verify the canonical remote branch HEAD equals it, and only then anchor its actual `MANIFEST_COMMIT_SHA` outside the manifest on the durable PR handoff surface. The manifest declares exact actions, expected outputs, allowed mutation paths, stop conditions and result destination; it never attempts to contain its own commit SHA.
11. After local execution, reconstruct live GitHub state and verify the complete non-self-referential SHA chain required by the H1 overlay: `EXECUTION_BASE_SHA -> MANIFEST_COMMIT_SHA -> PRODUCT_RESULT_SHA -> EVIDENCE_COMMIT_SHA` (with the permitted no-product-mutation equality case), both external anchors, mutation allowlist and exact result-summary-only evidence commit. Interpret the effective evidence and perform any required design/code repair as the remote Worker. If a repair can invalidate local evidence, issue a new numbered local round rather than extrapolating stale evidence.
12. Perform the mandatory strict Worker pre-review while still Draft + ACTIVE:
   - re-read the complete WP/DoD and binding protocol/proof rules;
   - verify the predecessor contract check still matches the accepted dependency state and avoid duplicating inherited proof unless concrete evidence invalidates it;
   - inspect the complete baseline→candidate diff, not only the latest change;
   - independently challenge acceptance, negative/error paths, scope, completeness, fail-closed behaviour and handoff requirements;
   - search for omission classes/effective behaviour inside the declared claim;
   - respect the proof budget and trusted base instead of expanding proof into arbitrary toolchain-pathology scenarios;
   - classify findings as local/trivial versus causal architectural/proof-boundary defects.
13. If pre-review finds an in-claim blocker, keep Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence, and repeat the required local round when the repair can affect local evidence before repeating step 12.
14. Record `WORKER_PRE_REVIEW: CLEAN`, findings fixed count and evidence path only when no known in-claim blocker remains. `CLEAN` is Worker readiness evidence, never independent `PASS`.
15. Stop all writers.
16. Freeze exact 40-char candidate SHA; set `FROZEN_FOR_REVIEW`; mark Ready.
17. STOP. Never act as the independent Reviewer or start the next WP.

Any implementation/evidence mutation after a clean pre-review invalidates that cleanliness and requires the pre-review to be rerun before freeze.

Missing required evidence, skipped mandatory tests, unavailable proof tooling or unresolved material doubt are FAIL/NOT_READY, never green by omission.
