# WP-H1-00 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 7
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-H1-00/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-H1-00 — Engine-neutral projection contract + reference materializer`.
- Baseline: `a9ff655e5bf2319690d919b88bd57389a32483f3`.
- Branch: `wp-h1-00-engine-neutral-projection`.
- Direct accepted predecessor: `WP-HK-GATE` PASS and DocSync.
- Final substantive/evidence checkpoint challenged before this pre-review record: `f0902e2631801eb1fea6ee172066fb4d11b5f3c2`.
- Exact-SHA Draft observation: Actions run `35573246626` GREEN.
- Checkpoint result: Release build `0 warnings / 0 errors`; focused H1-00 `14/14` GREEN; all six causal defect controls RED for the intended reason; full regression `234/234` GREEN; candidate clean before/after.
- This is Worker quality-gate evidence only. Fresh independent Reviewer PASS is still required on the final frozen SHA.

## Scope / authority challenge

The complete baseline→checkpoint diff was challenged against `Docs/workpacks/H1/WP-H1-00.md`, the accepted H1 architecture/ADRs and `FOUNDATIONAL_PROOF_STANDARD` v1.4.

The candidate owns only a dependency-free `netstandard2.1` `Arkus.EngineBridge` assembly, its deterministic fileless reference materializer, portable contract records, focused proof/evidence and the canonical observation route required to validate H1-00. It does not add Unity/editor implementation, asset-catalogue authority, gameplay, CITY keeper content, public H0 mutation routes, canonical state ownership, host-to-Editor process lifecycle or transport-specific bridge semantics.

Canonical truth remains H0-owned. The bridge consumes supplied canonical anchor/snapshot bytes defensively and emits derived plan/generation/observation/receipt evidence only. A bridge receipt is not an HK06 mutation entry and materialization failure cannot rewrite or advance canonical truth.

## Findings fixed during Worker pre-review

### Finding 1 — H1-00 had no canonical exact-SHA observation route

The first CI attempt could not validate the WP because `scripts/arkus-observe-exact-sha.sh` had no H1-00 dispatch. The candidate now registers `WP-H1-00` and delegates to a bounded H1-00 observer. This is validation routing only; Automation V2 semantics were not redefined.

### Finding 2 — locked restore did not include the new production project dependency

Adding the test-project reference to `Arkus.EngineBridge` initially made locked restore fail because `tests/Arkus.Harness.Tests/packages.lock.json` did not declare the new project reference. The lockfile was updated through the normal project dependency shape; no package was added.

### Finding 3 — `Arkus.EngineBridge` was absent from the Release solution graph

A draft run built the new assembly only incidentally through the test project, leaving no effective Release production assembly for the inherited HK01 production-assembly oracle. The solution now includes `Arkus.EngineBridge` with Debug/Release configurations. The inherited oracle was preserved and correctly detected this integration omission.

### Finding 4 — multiline negative-control seeds were not literal

The first defect-injection helper treated `\\n` as literal text for multiline replacements. The helper now expands those escaped newlines before replacement. The final checkpoint proves all six named defect classes cause actual test RED and explicitly rejects compiler/MSBuild/NuGet failure as invalid causal evidence.

### Finding 5 — portable receipt/observation semantics were initially too implicit

The first implementation exposed deterministic domain objects and receipt digests but did not provide a sufficiently explicit machine-readable/versioned record for observation/receipt diagnostics. `ProjectionPortableData` now emits only primitives, ordered lists and dictionaries, carries `arkus.engine-projection@1`, exposes the canonical anchor structurally (`worldId`, `revision`, `stateHash`), diagnostics and publication status, and fails closed if receipt, plan or observation identities disagree.

### Finding 6 — dependency neutrality needed proof in both directions

It was not enough to prove that `Arkus.EngineBridge` does not reference Runtime/transport/Unity. The final proof also inspects the effective Protocol and Runtime assembly references and verifies they do not acquire an upward dependency on `Arkus.EngineBridge`. The neutral bridge therefore remains downstream rather than entering the canonical kernel.

### Finding 7 — the deterministic scenario needed an explicit drift→delete→rebuild closure

A fresh-materializer rebuild was already deterministic, and drift was independently detectable, but the WP scenario explicitly calls for drift followed by deletion/rebuild from the same inputs. The final focused suite now links those steps: changed effective content is observed as `engine-drift`; an empty/fresh generated scope is `absent`; rematerializing the same plan restores the original normalized in-sync observation/generation.

## Acceptance / false-green challenge

The pre-review challenged the following material classes:

- each member of the full canonical-anchor/snapshot/binding/catalogue/toolchain tuple independently changes projection identity;
- caller resource order cannot change normalized plan/generation identity;
- same full input retries idempotently with no second semantic delta;
- pre-publication failure cannot move the active generation or receipt;
- `absent`, `in-sync`, `canonical-ahead`, `engine-drift`, `missing-dependency`, `ambiguous` and `failed` are all observably distinguishable;
- extra, missing and changed effective managed resources cannot collapse to `in-sync`;
- missing logical dependencies fail visibly rather than being silently omitted;
- supplied canonical snapshot bytes cannot be mutated through the bridge-owned input boundary;
- machine-readable records are versioned and contain no engine/native object types;
- receipt/plan/observation mismatches fail closed;
- EngineBridge has no Unity, Runtime, MCP, Projection, package or other product-project dependency;
- Protocol/Runtime do not acquire an upward EngineBridge dependency;
- the plaza/market/bar/workshop probe exercises a non-flat hierarchy and two distinct logical asset dependencies without promoting example content into Unity/CITY/gameplay schema;
- every one of the six workpack-named negative-conformance classes turns RED for the intended test reason;
- the full accepted H0 regression remains green after H1-00 composition.

The six defect controls are: receipt tuple misanchor, caller-order-dependent plan, failed staging publication leak, canonical-byte aliasing, hidden effective drift and engine/editor type leakage. Run `35573246626` proves all six RED causally, then the unmutated candidate passes the full regression.

## Foundational proof / residual reconciliation

`PROOF_MATRIX.md` reports `FOUNDATIONAL_PROOF_VERDICT: READY`, `UNRESOLVED_PROOF_OBLIGATIONS: 0`, `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` and `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` inside the declared fileless neutral claim.

`CONTENT_SHAPE_PROBE.md` is complete and found no H1-00 blocker or H0 reopen condition. `RESIDUAL_RISK.md` classifies real catalogue completeness, Unity serialization/native identity/effective observation, project restart durability, public batch lifecycle, supported Unity→canonical proposals and gameplay/CITY realization to their downstream owners. `UNCLASSIFIED_RESIDUALS: 0`; `PREDECESSOR_REOPEN_TRIGGERED: NO`.

No external runtime/package dependency or new IP adoption is introduced by this WP.

## Concurrent-main reconciliation

`main` advanced after the Worker baseline only in the CITY-01 DocSync/workpack surfaces visible in the current base comparison. Those files are outside the H1-00 write set and do not alter the accepted H1-00 engine-neutral contract, H0 authority model or build/runtime graph. The PR remains mergeable and no causal rebase is required before review.

## Validation reconciliation and handoff condition

Executable checkpoint:

- SHA `f0902e2631801eb1fea6ee172066fb4d11b5f3c2`;
- Actions run `35573246626`: GREEN;
- Release build: `0` warnings / `0` errors;
- focused H1-00: `14/14` GREEN;
- causal controls: `6/6` RED as required, runner GREEN;
- full regression: `234/234` GREEN;
- exact checkout clean before and after: YES;
- execution receipt: `Result: GREEN`.

This pre-review evidence commit necessarily changes HEAD after that executable checkpoint. Therefore the evidence-finalized HEAD must receive one final Draft exact-SHA observation. Only after that GREEN may that exact HEAD be recorded as Candidate/Frozen SHA, metadata switch to `FROZEN_FOR_REVIEW`, and the PR become Ready. The Ready transition must then receive GREEN frozen exact-SHA validation. No Worker implementation/evidence write is permitted after freeze.

No known in-boundary Worker blocker remains.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
