# implement-workpack

Implement exactly one explicitly authorized Juego2 workpack.

Juego2 / Arkus Harness is a game-development and software-verification project. Work is limited to repository-owned game-authoring code, fixtures, tests, CI and documentation. Legacy terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` refer only to internal negative/conformance testing; use the neutral terminology defined in `AGENTS.md` for new work.

## Trigger

Requests such as `Worker H1-02`, `Worker WP-H1-02`, `Worker CITY-04` or equivalent are explicit authorization to resolve that exact WP and execute this skill. The short command does not weaken any dependency, ownership, evidence, local-engine, freeze or review rule.

For a local session, reconstruct PR/ownership/check state from live GitHub through authenticated `gh` (or an equivalent live GitHub surface) rather than inferring it from the local Git checkout alone. Before GitHub mutations verify `gh repo view --json nameWithOwner` resolves to `Arkus0/Juego2` and that exactly one canonical implementation PR/branch owns the requested WP.

For `WP-H1-02` through `WP-H1-GATE`, the default orchestration is **remote-first staged execution** under `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`: the Worker performs `REMOTE_PREP`, delegates only the exact Unity/Windows-dependent slice through a persisted `LOCAL_EXECUTION.md`, then resumes as the same Worker for `REMOTE_CLOSEOUT`. `LOCAL_UNITY_REQUIRED` and `HYBRID` continue to require real local evidence; they do not require the whole Worker reasoning session to run locally.

## Context bootstrap

Start with `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `worker` profile in `Docs/engineering/context-bootstrap-profiles.json`. The profile is a minimum starting pack, not a context ceiling. For an exact WP, full `Docs/ROADMAP.md` is conditional on unresolved cross-track/order/gate meaning; `FOUNDATIONAL_PROOF_STANDARD.md` is loaded when the exact claim binds it. After CTX-02 adoption, a validated accepted-contract capsule may be the starting representation of an accepted predecessor under `Docs/engineering/CONTEXT_CAPSULE_V1.md`; it never replaces an authoritative read when a capsule escalation trigger, non-compressible source, direct proof/architecture binding or concrete reopen question is material. If any compact state is stale/missing/contradictory for a hint being used, ignore that hint and deepen to live GitHub + authoritative contracts.

After CTX-03 adoption, `Docs/engineering/CONTEXT_ENVELOPE_V1.md` is the narrow terminal-closure and mechanical-outcome amendment. It does not add a normal bootstrap read: the deterministic closure epilogue below is sufficient unless a classification dispute or envelope failure makes the full protocol material.

## Preconditions

- Read `AGENTS.md`, `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`, the exact WP and `WORKER_REVIEW_PROTOCOL.md`; load ROADMAP/foundational/architecture surfaces when the worker profile or exact claim triggers them.
- For `WP-H1-02` through `WP-H1-GATE`, also read `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md` before planning execution.
- Reconstruct current `main`, dependency satisfaction, open ownership and write-set conflicts from live GitHub.
- Before CTX-02 adoption, or for a direct dependency without a valid capsule, read the direct accepted dependency WP plus accepted completion/PASS evidence, relevant proof matrix/residual risk and binding architecture/invariants. After CTX-02 adoption, a mechanically valid capsule + independently confirmed accepted identity may instead start that reconstruction; follow its exact source pointers whenever the current claim needs a material detail, the capsule is lossy/suspect, a source is non-compressible, or concrete evidence could reopen the inherited guarantee. Follow transitive predecessors only when their invariants are material to the current WP.
- Persist a short `PREDECESSOR_CONTRACT_CHECK` in Worker plan/evidence identifying capsule(s) used when applicable, any source escalations, inherited guarantees, current-WP-owned guarantees, guarantees intentionally consumed rather than re-proved, and the concrete trigger that would justify reopening an accepted predecessor boundary.
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
12. Finish **all** repository/evidence bytes needed by the candidate while still Draft + ACTIVE, including pre-review inputs/checklists, and commit/push them. After CTX-03, the final `CLEAN` record itself is not a repository byte.
13. Stop writers and read the exact 40-character HEAD that will be challenged. Perform the mandatory strict Worker pre-review against that exact resulting HEAD:
   - re-read the complete WP/DoD and binding protocol/proof rules;
   - verify the predecessor contract check still matches the accepted dependency state and avoid duplicating inherited proof unless concrete evidence invalidates it;
   - inspect the complete baseline→candidate diff, not only the latest change;
   - independently challenge acceptance, negative/error paths, scope, completeness, fail-closed behaviour and handoff requirements;
   - search for omission classes/effective behaviour inside the declared claim;
   - respect the proof budget and trusted base instead of expanding proof into arbitrary toolchain-pathology scenarios;
   - classify findings as local/trivial versus causal architectural/proof-boundary defects.
14. If pre-review finds an in-claim blocker, resume Draft + ACTIVE, repair the causal defect boundary, rerun affected validation/evidence, repeat the required local round when the repair can affect local evidence, commit/push the changed bytes, then repeat steps 12–13 on the new exact HEAD. The old cleanliness is invalid.
15. Only when the exact HEAD is clean, persist a durable **GitHub PR issue comment** containing at minimum `WORKER_PRE_REVIEW: CLEAN`, `Candidate SHA: <exact HEAD>`, findings-fixed count and evidence pointers. This GitHub record is created after the complete pre-review and does not mutate candidate bytes. It is Worker readiness evidence, never independent PASS.
16. Keep writers stopped. Generate the derivable Ready metadata with `scripts/derive-worker-review-metadata.py --pre-review-evidence <that issue-comment URL>` so the canonical handoff points to the exact external CLEAN record; validate the PR body with the existing handoff lint.
17. Freeze that same HEAD, set `FROZEN_FOR_REVIEW`, `Branch frozen: YES`, and mark the PR Ready. No repository/evidence byte may change after the final pre-review; any byte mutation requires returning to Draft + ACTIVE and repeating the complete sequence.
18. Classify mechanical results under `mechanical-verifier-registry.json`: a registered causal `FAIL` returns to Worker repair; `REVIEW_BLOCKED` means repair/derive lifecycle metadata and rerun before Reviewer; `INFRA_ERROR` means diagnose/rerun infrastructure without calling the WP defective; `NOT_APPLICABLE` is neutral. An unregistered verifier cannot by itself count as WP FAIL. None of these outcomes substitutes for semantic Reviewer judgment.
19. Do **not** report “ready for Reviewer” merely because CI is green. Observe a durable Automation V2 `State: REVIEW_READY` marker targeting the exact frozen SHA. After CTX-03 adoption, require the automatic `State: REVIEW_READY_CLOSED` marker for the same SHA; closure may reuse an already-existing REVIEW_READY marker when a later same-SHA metadata/gate rerun makes the terminal predicates green. If the closure workflow is not yet adopted for the current transition, perform the equivalent final live HEAD read after observing `REVIEW_READY` and record it outside repository bytes.
20. Only when the terminal invariant is closed may the Worker tell the human to start a fresh independent Reviewer. Then STOP. Never act as that Reviewer or start the next WP.

Any implementation/evidence mutation after a clean pre-review invalidates that cleanliness and requires the complete pre-review/freeze sequence to be rerun on a new exact SHA. Metadata-only correction on the same SHA may reuse the exact-SHA CLEAN record and durable REVIEW_READY marker only when the corrected gates are rerun and closure re-observes the terminal invariant.

Missing required evidence, skipped mandatory tests, unavailable proof tooling or unresolved material doubt are FAIL/NOT_READY, never green by omission. Mechanical protocol/infra failures are classified before independent review; they are never promoted into a semantic Reviewer FAIL merely because a check is red.
