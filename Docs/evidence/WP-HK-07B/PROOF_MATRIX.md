# WP-HK-07B foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-07B/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK07B proves that MCP is a genuine second first-party projection of the already accepted `arkus.neutral-projection@1` boundary. The MCP process obtains capability identity and schemas from the composed canonical inventory, invokes only the accepted neutral service, and keeps MCP naming, JSON-RPC correlation and timeout metadata inside the adapter. No canonical, Runtime or neutral-projection source was amended to make MCP fit.

The claim covers the repository-owned MCP stdio adapter, its canonical-to-MCP mapping, dependency boundary and semantic conformance with the accepted HK07A JSONL projection for the accepted H0 surface. The mapping is total over the canonical capability identities accepted by composition: canonical names that fit MCP retain the reversible byte encoding, while over-limit identities receive a bounded adapter-local surrogate without changing canonical identity.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| MCP is a second projection, not a second semantic registry | `Arkus.Harness.Mcp` discovery and call path | every `tools/list` and `tools/call` rebuilds from `NeutralProjectionService.Capabilities`; no adapter command table exists | real MCP stdio discovery matches an independently composed canonical inventory | missing, extra and duplicate canonical-key injections make the inventory oracle RED | PASS | future protocol features outside tools are unclaimed |
| accepted neutral/canonical semantics were not amended for MCP | baseline→candidate source boundary | no Protocol, Runtime, Projection, Game authoring/state/validation source changed; MCP depends on Projection + Protocol only | diff audit plus project dependency checks | injected MCP-only `toolName` in the neutral field universe is detected as `mcp-framing-field:toolName` | PASS | a future incompatible MCP revision could require a new adapter/version decision |
| discovery identity and schemas are mechanically canonical | composed `CapabilityDefinition` universe | expected definitions are independently composed; actual tools carry exact canonical key, input schema and full canonical definition metadata | `McpDiscoveryMatchesIndependentCanonicalInventoryAndSchemas` compares every production definition | deliberate canonical-definition success-schema drift fails deep semantic equality | PASS | MCP presentation fields such as description/title are transport-local |
| scoped providers appear without MCP edits, including over-limit canonical names | accepted HK01 composition seam plus MCP naming boundary | adapter enumerates the supplied neutral projection; tool-name allocation runs over the deterministically sorted complete inventory rather than a base-only/fixed list | ordinary synthetic `engine.observe@1.0` plus two canonical-valid 123-character scoped capabilities appear automatically; both long capabilities remain invokable | `OverLimitScopedCapabilitiesRemainDiscoverableUniqueAndInvokable` requires composition success, distinct bounded MCP names and successful dispatch of both long identities | PASS | provider installation lifecycle itself remains predecessor/future scope |
| MCP naming is deterministic, bounded and collision-safe for the composed inventory | MCP framing only | short keys use the existing injective byte escape; over-limit keys use `arkus-long-<canonical-ordinal>-<sha256>` where the ordinal comes from the deterministic canonical sort and therefore separates every distinct accepted key even if fingerprints collide; direct short names always contain the escaped version separator `_40`, while surrogate names do not | two different 123-character canonical names receive distinct MCP names of at most 128 characters while exact canonical keys remain on their definitions/metadata and drive neutral dispatch | same long-name test would fail if either capability throws during discovery, exceeds the limit, aliases the other tool name, or dispatches to the wrong route | PASS | transport-local tool handles may change if the composed inventory itself changes; clients must rediscover and canonical identity remains the stable semantic identity |
| MCP cannot publish an adapter-only capability or mutation authority | MCP project/code dependency and effective call path | MCP has no Authoring/World/Runtime project reference and every call resolves a projected descriptor then invokes neutral service | representative mutations produce accepted journal/snapshot/replay artifacts through MCP | injected `adapter.only@1.0` is reported extra; unknown MCP name cannot enter canonical dispatch | PASS | reflection/native code outside repository conventions is outside the claim |
| JSONL and MCP preserve representative accepted H0 meaning | two fresh external process projections | both transports are normalized to the accepted neutral outcome shape and compared across the same semantic classes | summary, validation, apply, object read, journal, snapshot, diff, import and replay produce equivalent normalized outcomes and identical reconstructed authored state | invalid canonical request and zero-timeout paths must retain the same failure class/code | PASS | representative flow is bounded, not a proof of every payload value |
| cancellation/timeout remain accepted admission semantics | MCP call → neutral admission seam | MCP forwards cancellation/deadline to `NeutralProjectionService`; it does not preempt with a framing exception | zero timeout cross-transport equivalence; pre-cancelled synthetic call returns `projection.cancelled` | pre-cancelled synthetic handler invocation count remains exactly zero | PASS | cancellation after canonical dispatch begins remains admission-only by inherited HK07A contract |
| SDK is replaceable and not semantic authority | external component boundary | only MCP/test integration references `ModelContextProtocol.Core`; canonical projects are independently checked | exact package 2.2.0 + lockfile + dependency adoption record; canonical project texts contain no SDK reference | injected SDK PackageReference in Projection is detected by dependency-leak oracle | PASS | documented SDK protocol behavior is trusted infrastructure |
| MCP-specific representation pressure stays adapter-local | tool-name/metadata framing | canonical keys are never shortened or rewritten upstream; short names encode `@` as `_40`, and long names are represented by adapter-local bounded handles while exact canonical keys remain in metadata/descriptor lookup | `world.summary@1.0` remains canonical while its short MCP name is `world.summary_401.0`; long scoped keys remain exact canonically while receiving bounded MCP handles | deliberate `toolName` neutral-field amendment turns the framing-leak oracle RED; long-name causal test fails if adapter framing cannot represent an accepted canonical key | PASS | canonical semantics remain independent of MCP naming rules |
| non-interactive local boundary remains in scope | MCP executable | executable accepts no CLI mode and runs SDK stdio transport over `ProductionHarnessHost.Create()` | external tests launch real Release MCP process and exchange initialize/list/call JSON-RPC | unexpected CLI args fail before starting MCP server | PASS | HTTP/cloud/auth/vendor clients are explicitly outside HK07B |
| predecessor guarantees are consumed, not redundantly rebuilt | accepted HK07A/HK01-HK06C semantics | candidate adds no alternate state/validation/mutation/provenance/diff/replay implementation | inherited full regression remains GREEN after the naming repair | concrete cross-transport mismatch would be the reopen condition; none is observed | PASS | predecessor correctness remains governed by its accepted evidence |

## Independent/effective universes

1. **Capability/schema universe:** expected production identities and definitions come from an independently composed `CanonicalWorldContract`, not MCP discovery.
2. **Scoped-provider universe:** synthetic providers are accepted by the same `ContractComposer` seam consumed by MCP. The long-name control uses two independently valid canonical identities so projection completeness cannot be satisfied by special-casing the Reviewer's single example.
3. **Effective invocation path:** actual MCP stdio requests pass through SDK framing, `McpProjectionAdapter`, `NeutralProjectionService` and the accepted dispatcher. The representative mutation/provenance/replay flow and the two long-name invocations cannot be satisfied by a display-only tool list.
4. **Second-transport oracle:** the accepted JSONL process is a sibling projection. Comparisons normalize each external process response to neutral semantic outcomes rather than comparing MCP to itself.
5. **Dependency universe:** Protocol/Runtime/Projection project files are checked independently for SDK leakage. MCP-specific constraints are then defect-injected into that oracle.
6. **Framing-pressure oracle:** both character pressure (`@`) and length pressure are exercised while the canonical key and neutral semantics remain unchanged.

## Repair-cycle observation

Independent Reviewer FAIL `#5259817513` on frozen SHA `1447e56642414ce2e75219fdfcb191ada41d6378` identified the in-claim false green: a canonical-valid scoped capability could exceed MCP's 128-character tool-name limit and make discovery throw. The repair stayed entirely in HK07B adapter/test ownership; no HK07A or canonical semantic source changed.

The first repair observation at `27f0ee8cb36652b878f3e13e1ccf6d508500b6f8` correctly failed full regression because static attributed long-name test handlers entered HK01's independently enumerated test route universe. The fixture was corrected, not the predecessor oracle: the long-name handlers are now runtime-emitted in a separate dynamic assembly, still carry real `PublicCapabilityRoute` metadata, and still pass normal `ContractComposer` validation without changing HK01 production/test discovery semantics.

Exact implementation/test SHA `b8e7f1a628e65effa790f810519a721454a6c980` passed GitHub Actions run `35495362976` on the pinned .NET SDK 8.0.425:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07B*`: 9/9 GREEN;
- full regression: 160/160 GREEN;
- candidate clean before and after: YES;
- observation artifact: `10600483330`.

The evidence reconciliation commit after that observation is documentation/verification routing only. Its exact final SHA must pass `scripts/hk07b-verify-exact-sha.sh` unchanged before freeze.

## Proof-budget verdict

The repair closes one concrete false-green class already inside HK07B's declared naming/projection ownership. It adds one bounded causal test and one small adapter-local allocation rule; it does not expand canonical semantics, duplicate HK07A proof, or introduce a new registry. Further naming hardening without another realistic in-claim false-green class would exceed the present proof need.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
