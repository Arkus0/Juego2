# WP-H1-01 Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 8
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-H1-01/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-H1-01 — Unity scoped authoring producer + dependency derivation`.
- Baseline: `0f4dec86c7f9a5ab10db6479b995521d5f47cc78`.
- Branch: `wp-h1-01-unity-authoring-producer`.
- PR: `#79`.
- Direct accepted predecessor: `WP-H1-00` PASS, merge and DocSync.
- Pre-review input HEAD: `47fa48bab95d832eee9b0878d7c0b332701eaf41`.
- This document is Worker quality-gate evidence only. Independent Reviewer PASS is still required on the final frozen SHA.

## Scope / authority challenge

The complete baseline→candidate diff was challenged against `Docs/workpacks/H1/WP-H1-01.md`, H1 architecture, inherited H0 authority and the foundational proof standard.

The candidate owns one portable `netstandard2.1` Unity-scoped producer/provider, the generic scoped-contribution composition seam required to admit it, one shared production-host composition point, focused tests/evidence and exact-SHA validation routing. It does not contain `UnityEngine`/`UnityEditor`, native paths/GUID authority, Editor execution/materialization, scene/prefab writes, gameplay schema or an alternative public provider registry.

Canonical authority remains H0-owned. `unity.binding.compile` returns an ordinary `put-extension` mutation fragment and never commits canonical state. The representative integration path still passes that fragment through accepted H0 `plan/dry-run/apply`.

Catalogue identity remains provider-owned logical identity. Catalogue references are not promoted into canonical world-object identity and therefore do not alter HK02A semantics. Structured canonical-object references are emitted as accepted HK02A `kind + targetId` dependency metadata.

## Findings fixed during Worker cycle

### Finding 1 — H1-01 had no canonical exact-SHA validation route

The Automation router did not know `WP-H1-01`. The candidate now has bounded H1-01 observer/verifier scripts and registers them in the canonical observation/verification dispatchers. Automation semantics are not otherwise changed.

### Finding 2 — locked restore was stale after adding the provider project references

The first remote attempt failed with `NU1004`: the test and MCP lockfiles did not contain the new project-reference closure. Both lockfiles were reconciled without adding an external package.

### Finding 3 — initial provider schemas used formats outside the canonical portable subset

Early Worker inspection found `world-object-id` / `base64` format labels that the accepted canonical schema validator does not admit, and the catalogue logical-reference schema lacked the required `arkus-logical-reference` format. The candidate now uses the accepted schema subset and provider-owned logical-reference namespace; stronger object/base64 semantics remain handler validation rather than invented protocol formats.

### Finding 4 — one focused versioning fixture supplied a `WorldState` where an `IWorldStateSource` was required

The remote Release build exposed the test-only type mismatch. The fixture now uses `FixedWorldStateSource`; no production behavior changed.

### Finding 5 — the first canonical-reference defect seed failed compilation instead of the intended oracle

`if (true) continue` made later code compiler-unreachable (`CS0162`). Since compiler failure is not valid causal evidence, the negative harness rejected it. The seed now uses a runtime expression that remains true for real components while preserving compile reachability, and the named Potes test becomes RED for the omitted dependency itself. The contradiction mutant follows the same discipline.

### Finding 6 — the new production assembly was absent from the Release solution graph

The inherited HK01 production-assembly oracle found no effective Release `Arkus.EngineBridge.UnityAuthoring.dll`; the project had been built only incidentally through references. `Juego2.sln` now includes the project with Debug/Release configurations, preserving rather than weakening the independent assembly-universe proof.

### Finding 7 — inherited HK07/HK01 expected inventories still described H0-only production

Once the shared production host correctly composed the three Unity routes, inherited tests identified them as unexpected extras. The repair does not hardcode `23`: HK07A/HK07B independently compose the accepted base plus the explicit Unity scoped contribution and compare the full canonical definitions/schemas, while HK01 still scans every production project under `src/` and now requires `unity.binding.*` routes to belong to `arkus.unity-authoring` and all other production routes to remain `arkus.base`.

### Finding 8 — the initial same-version negative seed changed an explicit schema-version string

That seed demonstrated a version mismatch but was weaker than the contracted defect class “semantic schema change under the same version.” Worker pre-review strengthened the pin and mutant: the public binding remains `arkus.unity-binding@1`, the capability remains `1.0`, but the admitted component-kind schema is widened with `light`. The contract-pin test must turn RED specifically because v1 semantics changed without a reviewed version change.

## Acceptance / false-green challenge

The pre-review challenges these material claims:

- the provider has one explicit scoped identity, scope and namespace and enters the existing `ContractComposer` rather than a Unity-owned registry;
- v1 binding grammar is finite and pinned: target scene, asset/prefab source, normalized local transform, `canonical-link`, renderer material and animator clip;
- component caller order cannot change the normalized payload;
- every structured canonical link becomes exactly one HK02A dependency;
- target scene, source, material and animation references each appear exactly once as typed catalogue dependencies;
- caller dependency assertions are cross-checks only and fail closed on omission, contradiction or duplicates;
- compile/decode/inspect round-trip the normalized document and preserve derived dependencies;
- the representative extension fragment succeeds only through ordinary H0 plan/dry-run/apply;
- logical catalogue IDs are not paths/GUIDs and the portable assembly references no Unity runtime/editor assembly or package;
- JSONL and MCP independently discover and invoke the same new scoped routes from shared composition, without route edits in either adapter;
- the inherited HK01 route-universe oracle independently scans effective Release assemblies and includes the scoped provider rather than trusting its registry;
- same-version semantic widening is caught while all public version identifiers remain unchanged;
- all seven source mutants must fail by named tests, while compiler/MSBuild/NuGet failures are rejected as invalid negative evidence;
- full inherited regression must remain green after the scoped production composition changes.

## Content-shape / predecessor reconciliation

`CONTENT_SHAPE_PROBE.md` is complete. Its Potes facade slice independently expects one canonical `attached-to|market.potes-root` dependency plus four catalogue dependencies (`scene`, `prefab`, `material`, `animation-clip`). It exercises every initial reference-bearing location, transform normalization and three admitted component-document kinds without asserting that any Unity resource exists.

No HK02A reopen condition was triggered: its typed canonical dependency surface expresses the structured canonical reference exactly. H1-00 neutrality is preserved because no Unity runtime/editor type enters the neutral bridge. HK04 remains the only canonical commit authority. HK07 generic adapters remain untouched; only shared composition changes.

`RESIDUAL_RISK.md` classifies actual resource existence/type compatibility, logical-ID→native-locator resolution, effective Editor serialization/materialization, host-to-Editor lifecycle, project rebuild/reconciliation and future component kinds to their downstream owners. `UNCLASSIFIED_RESIDUALS: 0`; `PREDECESSOR_REOPEN_TRIGGERED: NO`.

## Foundational proof / dependency result

`PROOF_MATRIX.md` reports `FOUNDATIONAL_PROOF_VERDICT: READY`, `UNRESOLVED_PROOF_OBLIGATIONS: 0`, `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` and `PROOF_BUDGET_VERDICT: WITHIN_BUDGET` for the declared portable producer claim.

No Unity/vendor package or external runtime dependency was added. The provider project references only in-repository Arkus protocol/runtime surfaces needed for canonical definitions/routes. Native Unity locator resolution and Editor APIs are explicitly downstream.

## Validation reconciliation and handoff condition

Draft validation during the Worker cycle has already demonstrated: locked restore after lockfile repair; Release build; the nine focused H1-01 tests; and seven causal mutants reaching semantic test RED. An earlier full regression then correctly exposed the missing Release solution integration and stale H0-only inventory expectations, which have been repaired without weakening their independent oracles.

Because those integration repairs and this pre-review evidence change HEAD, historical runs are not authority for the final candidate. The final evidence-complete Draft HEAD must receive a fresh canonical exact-SHA GREEN covering locked restore, Release build, focused producer/probe/HK02A/transport tests, portable Unity boundary, all seven causal controls and the complete regression suite. Only that exact GREEN SHA may then be written into PR Candidate/Frozen metadata and transitioned Ready for Review. The Ready transition must receive the frozen exact-SHA verifier GREEN, and no tracked-file write is permitted after freeze.

No known in-boundary Worker blocker remains.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
