# WP-HK-GATE proof matrix

FOUNDATIONAL_PROOF_VERDICT: NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: 1
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
TRUST_BOUNDARY: exact Git checkout/object semantics; pinned .NET/MSBuild/NuGet documented behavior; normal documented GitHub-hosted runner process/filesystem behavior; BCL/JSON/SHA-256 primitives; accepted HK01-HK10 predecessor guarantees unless contradicted by concrete effective-path evidence

## Fixed readiness universe

The GATE readiness universe is fixed by `WP-HK-GATE` and the declarative `HkGateReadinessTests.GateStepUniverseIsExplicitAndCannotSilentlyOmitARequiredScenarioStage`. Executable wiring is independently checked by `scripts/hkgate-proof-infrastructure-check.sh`, so retaining a stage label while deleting the corresponding stage-14 execution cannot remain green.

| Gate obligation | Evidence/oracle | Status |
|---|---|---|
| 1. public discovery + schemas | clean-process reference JSONL `system.describe`; required capability keys; schema presence; independent canonical↔MCP inventory oracle | GREEN |
| 2. representative multi-resource micro-world | `HkGateReadinessTests` + `CONTENT_SHAPE_PROBE.md` | GREEN |
| 3. bounded authorable-state reconstruction | object query/get + typed reference query + opaque extension query/read | GREEN |
| 4. plan/dry-run/change set/atomic apply | representative four-operation coherent request | GREEN |
| 5. invalid diagnostics + repair/no partial commit | `world.change.invalid_candidate`, structured validation context, anchor equality before repair | GREEN |
| 6. stale same-lineage bounded recovery | HK08B recovery context; returned affected-resource inspection descriptors; no summary/object-query/extension-query reload in recovery slice; explicit retry | GREEN |
| 7. HK08A coherent batch within HK09B envelope/budget | 96-operation plan/dry-run/apply; HK08B interaction benchmark; HK09B resource tests | GREEN |
| 8. snapshot + provenance/journal | public snapshot export; journal v1 complete read; v2 bounded pagination | GREEN |
| 9. clean restart replay/import identical state | second clean reference process; import base; replay accepted journal; final hash equality | GREEN |
| 10. semantic diff + provenance chain | initial→final semantic diff plus final↔replayed zero diff | GREEN |
| 11. reference↔MCP semantic equivalence | accepted HK07B/HK08A/HK08B/HK09B cross-transport conformance suite, including independent canonical inventory | GREEN |
| 12. HK09A authority + HK09B resource/persistence | complete HK09A/HK09B test surfaces re-executed by GATE | GREEN |
| 13. bounded endurance | accepted HK10 512-transaction inspect/validate/mutate/journal/snapshot/restart surface | GREEN |
| 14. full headless validation | real unfiltered full `Arkus.Harness.Tests` execution in `hkgate-observe-exact-sha.sh`, with executable-wiring oracle | GREEN |
| 15. causal false-green controls | G1 removes the real stage-14 execution while leaving its declarative label intact and must RED for that omission; twelve accepted HK10 causal RED controls run in disposable worktrees | GREEN |
| 16. residual reconciliation + dependency/IP + H0→Engine Bridge audit | `RESIDUAL_RISK.md` explicitly consumes all 53 accepted HK10 residual rows; exact-SHA verifier checks the fixed ID universe; `DEPENDENCY_IP_INVENTORY.md` | GREEN |
| 17. fresh independent AI-agent public-client trial | `AI_AGENT_TRIAL_BRIEF.md`; must be external, MCP, exact frozen SHA, no implementation-source access | **PENDING / BLOCKING** |

## Independent-universe checks

Transport readiness does not compare MCP to its own registry. `Hk07BMcpConformanceTests.McpDiscoveryMatchesIndependentCanonicalInventoryAndSchemas` independently composes the effective canonical contract, then requires every MCP tool identity/schema to match it exactly. Its causal controls separately demonstrate missing, extra, duplicate and schema-drift detection.

The representative GATE client likewise requires a fixed public capability set that includes discovery, object/reference/extension inspection, validation, mutation, journal v1/v2, snapshot, diff and replay. For integration-proof completeness, the declaration and execution are intentionally separate oracles: `GateStepUniverse` fixes the named 14-stage universe, while G1 mutates the actual stage-14 runner command and `hkgate-proof-infrastructure-check.sh` must detect its absence.

Residual completeness is also checked against an independent fixed universe rather than only the GATE document's own `COMPLETE/0` headers. `hkgate-verify-exact-sha.sh` requires the exact 53 residual IDs handed off by accepted HK10 evidence, preserving `CLOSED-BY`, `HK10-COVERED`, `DEFERRED`, trusted-base and `OUT-BOUNDARY` outcomes without re-proving predecessor semantics.

## Trust-boundary challenge

GATE does not claim arbitrary OS/toolchain correctness, power-loss durability, automatic merge, per-resource locking, multi-agent throughput, cloud/auth/tenancy, Unity/gameplay semantics or model-vendor quality. Those are outside this bounded H0 readiness claim. Within the claim, product behavior is observed through public process/transport boundaries, not privileged in-process calls.

## Current blocker

The deterministic Worker evidence cannot substitute for the mandated independent second client because this Worker has read implementation source and authored the candidate. The prior trial PASS is bound to its superseded exact SHA; after this tracked reviewer repair, a fresh agent trial must target the final frozen repaired SHA. Until it posts a qualifying exact-SHA PASS transcript, the correct foundational verdict remains `NOT_READY`.
