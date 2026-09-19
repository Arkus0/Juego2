# WP-HK-00 Worker adversarial pre-review

Status: **CLEAN**
Protocol: `WORKER_REVIEW_PROTOCOL.md` v1.3
Proof standard: `FOUNDATIONAL_PROOF_STANDARD.md` v1.2
Trust boundary: `Docs/evidence/WP-HK-00/TRUST_BOUNDARY_AUDIT.md`
Independent Reviewer verdict: **not attempted by this Worker**

## Result

`DESIGN_AUDIT: CLEAN`
`WORKER_PRE_REVIEW: CLEAN`
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 12`
`WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-00/WORKER_PRE_REVIEW.md`
`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`

This is the mandatory Worker-side falsification gate. It does not satisfy or replace independent review. Freeze remains conditional on the final evidence-bearing HEAD passing the exact-SHA verifier with zero evidence drift; the authoritative execution/freeze state is the PR handoff and exact-SHA receipt.

## Acceptance-focused falsification

### Canonical universe / ownership — CLEAN

- Re-read `WP-HK-00`, `FOUNDATIONAL_PROOF_STANDARD.md` and the complete baseline→candidate surface.
- Git candidate tree is the independent project/source existence universe; fixed Arkus classification is separate from generated evidence.
- Canonical solution/project membership is exact; tracked production C# maps to exactly one owner.
- The prior self-shrinking `sourceScanRoots`/classification family is repaired at the universe boundary, not by special-casing the reported example.
- Observed `projects.json` and `graph.json` are byte-identical to committed evidence; reconciled `sources.json` only adds tracked proof-oracle sources introduced by the repair lineage.

### Portable/toolchain/forbidden scope — CLEAN

- Portable projects remain `netstandard2.1`, C# 9, deterministic and warnings-as-errors; host/tests/proof use pinned .NET 8.
- SDK `8.0.425`, runtime/ref pack `8.0.31`, roll-forward disabled.
- No Unity/UnityEditor/DFU/gameplay/assets/Creator GUI scope was introduced.
- Product projects have no third-party NuGet authority; test-only package closure is exact and locked.

### Effective build / compiler boundary — CLEAN

- Evaluated `ProjectReference` graph is checked against the fixed direction and for cycles.
- Effective compiler sources, references, analyzers/configs, PDB/source checks and output identity are observed rather than inferred only from static XML.
- Full normalized compiler arguments are reconciled as `inventory/compiler-args.json` for all nine canonical projects.
- External authority is fail-closed: selected SDK, packs and `sdk-manifests` under the same selected `DOTNET_ROOT` are trusted toolchain roots; test package paths must belong to the exact committed lock closure; arbitrary external imports/references remain rejected.

### Proof integrity / causal controls — CLEAN

- Positive proof is read-only with observations below ignored `artifacts/`.
- Self-attacks execute in disposable candidate copies and reconstruct pristine state after each RED injection.
- Observation run #104 on `56919bfd71fa5f28a370e7ba03a49d07b76e722a` completed foundational proof GREEN and aggregated exactly 37 rows / 37 unique causal controls GREEN.
- The external-import false-green discovered during this Worker cycle was repaired causally: import closure no longer relies on `MSBuildAllProjects`; the existing `external-build-import` control now detects the injected external import for `HK00-MSBUILD-IMPORT-EXTERNAL-UNTRUSTED` and then returns pristine GREEN.
- Parallel sharding changes orchestration only: shards reuse canonical attack functions; the aggregate requires exactly 37 unique observations before receipt success.

## Material causal classes repaired during Worker lineage

1. self-shrinking project/source scan boundary;
2. project/build participant pre-filtering before classification;
3. Tests/Proof outside effective source/PDB observation;
4. proof oracle able to depend on its own MSBuild compile universe;
5. response/solution/import behavior not aligned with canonical observation;
6. material compiler input channels omitted from effective observation;
7. PE/PDB output substitution gap;
8. proof/evidence mutation of the candidate;
9. transitive test dependency/analyzer authority not explicitly bounded;
10. external import/reference provenance split across inconsistent policies;
11. candidate lineage behind binding process rules;
12. evaluated import-completeness oracle relying on `MSBuildAllProjects`, which could omit an injected external import; replaced with direct preprocessed import closure and retained causal regression coverage.

No known material defect class inside the HK00 claim remains undetected. Risks requiring malicious subversion of Git, the pinned .NET/MSBuild/C# toolchain, NuGet semantics, OS/filesystem or CI substrate remain outside the declared trust boundary.

## Freeze condition

`WORKER_PRE_REVIEW: CLEAN` is established for the final evidence content represented by this commit. Do **not** freeze unless the same exact evidence-bearing HEAD passes the canonical exact-SHA verifier, including committed inventory drift and committed 37-attack-summary drift. If that verifier fails or any candidate byte changes, this CLEAN result is invalid and the Worker must return to ACTIVE.
