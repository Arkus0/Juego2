# WP-H1-00 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 9
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-H1-00/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-H1-00 — Engine-neutral projection contract + reference materializer`.
- Baseline: `a9ff655e5bf2319690d919b88bd57389a32483f3`.
- Branch: `wp-h1-00-engine-neutral-projection`.
- Direct accepted predecessor: `WP-HK-GATE` PASS and DocSync.
- Previously frozen candidate: `ed2f0532c4f2107f697728a7fa7774323445a307`.
- Previously frozen exact-SHA validation: Actions run `35574350759` GREEN.
- Independent review on that SHA: FAIL, review `#5264468475`, because ambiguous effective inventories with duplicate `ResourceId` values could leak caller enumeration order into `EffectiveDigest` / `ObservationDigest`.
- The failed freeze was withdrawn and PR #75 returned to Draft before any repair write.
- This document is Worker quality-gate evidence only. Fresh independent Reviewer PASS is still required on the final repaired frozen SHA.

## Scope / authority challenge

The complete baseline→candidate diff was challenged against `Docs/workpacks/H1/WP-H1-00.md`, the accepted H1 architecture/ADRs and `FOUNDATIONAL_PROOF_STANDARD` v1.4.

The candidate owns only a dependency-free `netstandard2.1` `Arkus.EngineBridge` assembly, its deterministic fileless reference materializer, portable contract records, focused proof/evidence and the exact-SHA observation/verification routes required to validate H1-00. It does not add Unity/editor implementation, asset-catalogue authority, gameplay, CITY keeper content, public H0 mutation routes, canonical state ownership, host-to-Editor process lifecycle or transport-specific bridge semantics.

Canonical truth remains H0-owned. The bridge consumes supplied canonical anchor/snapshot bytes defensively and emits derived plan/generation/observation/receipt evidence only. A bridge receipt is not an HK06 mutation entry and materialization failure cannot rewrite or advance canonical truth.

## Findings fixed during Worker cycle

### Finding 1 — H1-00 had no canonical exact-SHA observation route

The first CI attempt could not validate the WP because `scripts/arkus-observe-exact-sha.sh` had no H1-00 dispatch. The candidate now registers `WP-H1-00` and delegates to a bounded H1-00 observer. This is validation routing only; Automation V2 semantics were not redefined.

### Finding 2 — locked restore did not include the new production project dependency

Adding the test-project reference to `Arkus.EngineBridge` initially made locked restore fail because `tests/Arkus.Harness.Tests/packages.lock.json` did not declare the new project reference. The lockfile was updated through the normal project dependency shape; no package was added.

### Finding 3 — `Arkus.EngineBridge` was absent from the Release solution graph

A draft run built the new assembly only incidentally through the test project, leaving no effective Release production assembly for the inherited HK01 production-assembly oracle. The solution now includes `Arkus.EngineBridge` with Debug/Release configurations. The inherited oracle was preserved and correctly detected this integration omission.

### Finding 4 — multiline negative-control seeds were not literal

The first defect-injection helper treated escaped newlines as literal text for multiline replacements. The helper now expands those escaped newlines before replacement and rejects compiler/MSBuild/NuGet failure as invalid causal evidence.

### Finding 5 — portable receipt/observation semantics were initially too implicit

The first implementation exposed deterministic domain objects and receipt digests but did not provide a sufficiently explicit machine-readable/versioned record for observation/receipt diagnostics. `ProjectionPortableData` now emits only primitives, ordered lists and dictionaries, carries `arkus.engine-projection@1`, exposes the canonical anchor structurally (`worldId`, `revision`, `stateHash`), diagnostics and publication status, and fails closed if receipt, plan or observation identities disagree.

### Finding 6 — dependency neutrality needed proof in both directions

It was not enough to prove that `Arkus.EngineBridge` does not reference Runtime/transport/Unity. The proof also inspects effective Protocol and Runtime assembly references and verifies they do not acquire an upward dependency on `Arkus.EngineBridge`. The neutral bridge therefore remains downstream rather than entering the canonical kernel.

### Finding 7 — the deterministic scenario needed an explicit drift→delete→rebuild closure

A fresh-materializer rebuild was already deterministic, and drift was independently detectable, but the WP scenario explicitly calls for drift followed by deletion/rebuild from the same inputs. The focused suite links those steps: changed effective content is observed as `engine-drift`; an empty/fresh generated scope is `absent`; rematerializing the same plan restores the original normalized in-sync observation/generation.

### Finding 8 — Ready/freeze verification had no H1-00 canonical verifier route

The first Ready transition exposed a process omission: `scripts/arkus-verify-exact-sha.sh` could not resolve `WP-H1-00`, so frozen validation failed before product verification. The repair added `scripts/h1-00-verify-exact-sha.sh` and registered it in the canonical verifier router. The failed freeze was withdrawn before those tracked-file writes.

### Finding 9 — ambiguous observation normalization was not deterministic inside duplicate-ID groups

Independent review of frozen SHA `ed2f0532c4f2107f697728a7fa7774323445a307` identified a real false green. `ReferenceMaterializer.EffectiveDigest` sorted effective resources only by `ResourceId`. When two resources shared the same ID but differed in normalized content, stable ordering preserved caller enumeration order. `[A,B]` and `[B,A]` therefore produced the same `Ambiguous` state and diagnostics but different `EffectiveDigest` / `ObservationDigest` values.

The repair stays inside the already-owned H1-00 deterministic-observation guarantee:

- `EffectiveDigest` now orders first by `ResourceId` and then by the full normalized resource representation using ordinal comparison;
- fully identical duplicates remain harmless because their normalized encodings are equal;
- `AmbiguousObservationDigestIsIndependentOfDuplicateEnumerationOrder` constructs distinct duplicate-ID resources and asserts `[A,B]` ↔ `[B,A]` equality for state, diagnostics, `EffectiveDigest` and `ObservationDigest`;
- `scripts/h1-00-negative-conformance.sh` adds `ambiguous-observation-order`, removes only the secondary ordering, and requires the new regression test to turn RED for that causal reason;
- the control count is therefore seven: the six workpack-named classes plus this reviewer-discovered in-boundary determinism defect.

No H0 guarantee, H1 architecture decision, public version or Unity boundary is changed by this repair.

## Acceptance / false-green challenge

The repaired pre-review challenges the following material classes:

- each member of the full canonical-anchor/snapshot/binding/catalogue/toolchain tuple independently changes projection identity;
- caller resource order cannot change normalized plan/generation identity;
- equivalent ambiguous effective-resource multisets cannot change normalized effective/observation digests when duplicate entries are enumerated in a different order;
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
- all seven causal controls turn RED for their intended test reason and the unmutated candidate then returns GREEN;
- the full accepted H0 regression remains green after H1-00 composition;
- Draft observation and Ready/frozen verification are both routable for `WP-H1-00` and bind the same exact candidate SHA.

## Foundational proof / residual reconciliation

`PROOF_MATRIX.md` reports `FOUNDATIONAL_PROOF_VERDICT: READY`, `UNRESOLVED_PROOF_OBLIGATIONS: 0`, `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` and `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` inside the declared fileless neutral claim. It now contains a dedicated row for duplicate-ID ambiguity order invariance and the seventh causal control.

`CONTENT_SHAPE_PROBE.md` remains complete and found no H1-00 blocker or H0 reopen condition. `RESIDUAL_RISK.md` still classifies real catalogue completeness, Unity serialization/native identity/effective observation, project restart durability, public batch lifecycle, supported Unity→canonical proposals and gameplay/CITY realization to their downstream owners. `UNCLASSIFIED_RESIDUALS: 0`; `PREDECESSOR_REOPEN_TRIGGERED: NO`.

No external runtime/package dependency or new IP adoption is introduced by this WP.

## Concurrent-main reconciliation

The PR remains mergeable. Concurrent CITY work is outside the H1-00 write set and does not alter the accepted H1-00 engine-neutral contract, H0 authority model or build/runtime graph. No causal rebase is required solely for the reviewer-discovered determinism repair.

## Validation reconciliation and handoff condition

The prior frozen candidate `ed2f0532c4f2107f697728a7fa7774323445a307` had exact-SHA GREEN but is invalidated by independent Reviewer FAIL. Its GREEN result is therefore historical evidence only, not authority for the repaired candidate.

The repaired implementation/tests/control/evidence change HEAD. The final evidence-finalized Draft HEAD must receive a fresh canonical exact-SHA GREEN. Expected focused delta from the prior candidate is one additional reference regression test and one additional causal mutant (`15` focused H1 tests total if no unrelated test-count change; `7/7` causal RED controls). Only after Draft GREEN may that exact HEAD be recorded as Candidate/Frozen SHA and the PR become Ready. The Ready transition must then receive GREEN frozen exact-SHA verification. No tracked-file write is permitted after the successful freeze.

No known in-boundary Worker blocker remains.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET