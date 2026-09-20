# WP-HK-07B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-07B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK07B proves that MCP is a genuine second first-party projection of the already accepted `arkus.neutral-projection@1` boundary. The MCP process obtains capability identity and schemas from the composed canonical inventory, invokes only the accepted neutral service, and keeps MCP naming, JSON-RPC correlation and timeout metadata inside the adapter. No canonical, Runtime or neutral-projection source was amended to make MCP fit.

The claim is intentionally narrower than proving MCP or the SDK in general: it covers the repository-owned MCP stdio adapter, its canonical-to-MCP mapping, dependency boundary and semantic conformance with the accepted HK07A JSONL projection for the accepted H0 surface.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| MCP is a second projection, not a second semantic registry | `Arkus.Harness.Mcp` discovery and call path | every `tools/list` and `tools/call` rebuilds from `NeutralProjectionService.Capabilities`; no adapter command table exists | real MCP stdio discovery matches an independently composed canonical inventory | missing, extra and duplicate canonical-key injections make the inventory oracle RED | PASS | future protocol features outside tools are unclaimed |
| accepted neutral/canonical semantics were not amended for MCP | baseline→candidate source boundary | no Protocol, Runtime, Projection, Game authoring/state/validation source changed; MCP depends on Projection + Protocol only | diff audit plus project dependency checks | injected MCP-only `toolName` in the neutral field universe is detected as `mcp-framing-field:toolName` | PASS | a future incompatible MCP revision could require a new adapter/version decision |
| discovery identity and schemas are mechanically canonical | composed `CapabilityDefinition` universe | expected definitions are independently composed; actual tools carry exact canonical key, input schema and full canonical definition metadata | `McpDiscoveryMatchesIndependentCanonicalInventoryAndSchemas` compares every production definition | deliberate canonical-definition success-schema drift fails deep semantic equality | PASS | MCP presentation fields such as description/title are transport-local |
| scoped providers appear without MCP edits | accepted HK01 composition seam | adapter enumerates the supplied neutral projection rather than base-only names | synthetic `engine.observe@1.0` appears from an unknown-to-MCP scoped provider | fixed/missing inventory controls expose omission | PASS | provider installation lifecycle itself remains predecessor/future scope |
| MCP cannot publish an adapter-only capability or mutation authority | MCP project/code dependency and effective call path | MCP has no Authoring/World/Runtime project reference and every call resolves a projected descriptor then invokes neutral service | representative mutations produce accepted journal/snapshot/replay artifacts through MCP | injected `adapter.only@1.0` is reported extra; unknown MCP name cannot enter canonical dispatch | PASS | reflection/native code outside repository conventions is outside the claim |
| JSONL and MCP preserve representative accepted H0 meaning | two fresh external process projections | both transports are normalized to the accepted neutral outcome shape and compared across the same semantic classes | summary, validation, apply, object read, journal, snapshot, diff, import and replay produce equivalent normalized outcomes and identical reconstructed authored state | invalid canonical request and zero-timeout paths must retain the same failure class/code | PASS | representative flow is bounded, not a proof of every payload value |
| cancellation/timeout remain accepted admission semantics | MCP call → neutral admission seam | MCP forwards cancellation/deadline to `NeutralProjectionService`; it does not preempt with a framing exception | zero timeout cross-transport equivalence; pre-cancelled synthetic call returns `projection.cancelled` | pre-cancelled synthetic handler invocation count remains exactly zero | PASS | cancellation after canonical dispatch begins remains admission-only by inherited HK07A contract |
| SDK is replaceable and not semantic authority | external component boundary | only MCP/test integration references `ModelContextProtocol.Core`; canonical projects are independently checked | exact package 2.2.0 + lockfile + dependency adoption record; canonical project texts contain no SDK reference | injected SDK PackageReference in Projection is detected by dependency-leak oracle | PASS | documented SDK protocol behavior is trusted infrastructure |
| MCP-specific representation pressure stays adapter-local | tool-name/metadata framing | canonical `world.summary@1.0` remains unchanged while transport encodes `@` as `_40`; neutral fields remain transport-neutral | tool name and canonical key are simultaneously observable and distinct | deliberate `toolName` neutral-field amendment turns the framing-leak oracle RED | PASS | canonical keys exceeding MCP's current 128-char name limit would require an adapter/version compatibility decision |
| non-interactive local boundary remains in scope | MCP executable | executable accepts no CLI mode and runs SDK stdio transport over `ProductionHarnessHost.Create()` | external tests launch real Release MCP process and exchange initialize/list/call JSON-RPC | unexpected CLI args fail before starting MCP server | PASS | HTTP/cloud/auth/vendor clients are explicitly outside HK07B |
| predecessor guarantees are consumed, not redundantly rebuilt | accepted HK07A/HK01-HK06C semantics | candidate adds no alternate state/validation/mutation/provenance/diff/replay implementation | inherited full regression 159/159 GREEN at implementation/test SHA | concrete cross-transport mismatch would be the reopen condition; none is observed | PASS | predecessor correctness remains governed by its accepted evidence |

## Independent/effective universes

1. **Capability/schema universe:** expected production identities and definitions come from an independently composed `CanonicalWorldContract`, not MCP discovery.
2. **Effective invocation path:** actual MCP stdio requests pass through SDK framing, `McpProjectionAdapter`, `NeutralProjectionService` and the accepted dispatcher. The representative mutation/provenance/replay flow cannot be satisfied by a display-only tool list.
3. **Second-transport oracle:** the accepted JSONL process is a sibling projection. Comparisons normalize each external process response to neutral semantic outcomes rather than comparing MCP to itself.
4. **Dependency universe:** Protocol/Runtime/Projection project files are checked independently for SDK leakage. MCP-specific constraints are then defect-injected into that oracle.
5. **Framing-pressure oracle:** MCP tool-name rules are deliberately exercised with a character (`@`) that requires transport encoding while the canonical key and neutral request remain unchanged.

## Implementation observation

Exact remote implementation/test SHA `abf22ba58a7645f8a74e25fb814d5613cb014e95` passed GitHub Actions run `35494482169` on the pinned .NET SDK 8.0.425:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07B*`: 8/8 GREEN;
- full regression: 159/159 GREEN;
- canonical exact-SHA receipt: GREEN;
- candidate clean before and after: YES;
- observation artifact: `10599982945`.

The evidence reconciliation commit after that observation is documentation/verification routing only. Its exact final SHA must pass `scripts/hk07b-verify-exact-sha.sh` unchanged before freeze.

## Proof-budget verdict

Product scope is one MCP adapter/executable around an already accepted neutral service. Proof is bounded to the new transport boundary: one cross-transport suite, one small causal boundary suite, dependency isolation and exact-SHA scripts. The final causal additions directly satisfy explicit HK07B negative-conformance clauses; no further proof expansion is justified.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
