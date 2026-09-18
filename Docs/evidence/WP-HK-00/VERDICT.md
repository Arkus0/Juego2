# WP-HK-00 Worker foundational verdict

FOUNDATIONAL_PROOF_VERDICT: NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: 1
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: `Docs/evidence/WP-HK-00/TRUST_BOUNDARY_AUDIT.md`
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Worker statement

The implementation covers the material acceptance claims of `WP-HK-00` within the finite trust boundary documented in `TRUST_BOUNDARY_AUDIT.md` and reconciled with `FOUNDATIONAL_PROOF_STANDARD.md` v1.2.

The proof system no longer treats arbitrary hostile subversion of Git, the pinned .NET/MSBuild/C# toolchain, NuGet or OS/CI infrastructure as an HK00 obligation. Existing guards against realistic repository-controlled false-green paths remain as defence in depth because they are already implemented and useful.

`NOT_READY` is temporary and procedural/evidentiary, not a known implementation defect. The remaining obligation is to obtain an actual exact-SHA CI execution, reconcile generated observation artifacts with committed evidence, then perform the mandatory Worker adversarial pre-review on that complete exact candidate.

## Proof-budget statement

The earlier repair line materially expanded proof machinery while closing a real self-shrinking-universe defect class. The v1.2 trust-boundary audit stops that expansion and classifies the existing machinery by acceptance value. No further proof/self-attack family will be added unless it directly falsifies an HK00 acceptance criterion inside the declared trust boundary.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`

## Required freeze preconditions

Before freeze, one exact candidate SHA must have all of the following:

- exact candidate/toolchain preflight GREEN;
- read-only foundational proof pipeline GREEN;
- committed inventory equals observed inventory, including normalized `compiler-args.json`;
- all 37 retained causal RED→GREEN attacks GREEN;
- committed self-attack summary equals observation;
- clean restore/build/test on Linux under the locked dependency graph;
- `WORKER_PRE_REVIEW: CLEAN` under Worker/Reviewer Protocol v1.3;
- no later branch mutation.

Only after the evidence-bearing candidate satisfies those conditions may the foundational verdict be changed to `READY` with both counters zero and the same exact HEAD frozen for independent review.

Independent Reviewer remains mandatory and must not treat this Worker verdict or pre-review as a PASS.
