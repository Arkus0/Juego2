# WP-HK-07A negative-conformance matrix

All controls execute either the public neutral service, the compiled reference process, or the independently bootstrapped repository/project oracle. They target HK07A-owned defect classes rather than duplicating accepted canonical semantics.

| Required defect class | Causal control | Expected RED condition | GREEN result |
|---|---|---|---|
| protocol-stream log contamination | `CleanOneShotProcessDiscoversCanonicalInventoryAndKeepsDiagnosticsOffStdout` launches `--diagnostics` and parses stdout independently | any readiness/diagnostic text enters stdout or a second stdout line appears | one valid protocol frame only; readiness text exists only on stderr |
| truncated frame | `MalformedTruncatedInvalidUtf8AndOversizedFramesFailWithStableTransportErrors` sends both no one-shot bytes and an incomplete `{` value | incomplete JSON is mislabeled malformed, dispatched, or crashes/fatals | exit 2 with `failureKind=transport`, `transport.truncated_frame` |
| malformed frame | same process test sends structurally impossible `{"value":}` | malformed JSON is accepted, conflated with truncation, or throws outside the protocol | exit 2 with `transport.malformed_json` |
| oversized frame | same process test sends 1,048,577 bytes plus terminator | unbounded buffering, partial dispatch or unstable fatal behavior | line is drained; exit 2 with `transport.frame_too_large`; no canonical dispatch |
| cancellation | `CancellationAndExpiredAdmissionNeverInvokeCanonicalHandler` supplies an already-cancelled token | canonical handler runs or result is mislabeled after cancellation | `projection.cancelled`; synthetic invocation count remains 0 |
| timeout | same neutral test plus `TimeoutAndCanonicalFailuresRemainDistinctMachineReadableOutcomes` uses an expired admission deadline | canonical handler runs or timeout is confused with canonical rejection | `projection.timeout`, `failureKind=timed-out`; invocation count remains 0 |
| unexpected environment changes semantics | `UnrelatedEnvironmentCannotChangeCanonicalProcessSemantics` sets world/registry/default-capability-looking `ARKUS_*` values | host world, inventory, default route or output bytes change | exact stdout bytes and empty stderr match baseline |
| alternate public host path skips canonical runtime | `ProductionHostProjectsTheCompleteCanonicalWorldComposition` compares production host to an independently paired canonical session; external client then needs all accepted authorities | base-only/direct adapter path omits canonical inventory or fails provenance/snapshot/replay | exact production discovery equivalence; full external flow GREEN |
| transport-owned registry is sole completeness oracle | `CompletenessOracleRejectsTransportOwnedBaseOnlyList` deliberately projects fixed base keys against a larger composed contract | fixed list is accepted as complete | `projection.omitted_capability` for `engine.observe@1.0` |
| synthetic scoped capability omitted | `SyntheticScopedCapabilityIsProjectedAndInvokedWithoutAdapterRegistry` composes a scoped provider unknown to CLI | neutral layer/adapter needs a hand-maintained route | scoped capability is discovered and invoked with no adapter change |
| framing concern leaks into kernel/neutral semantics | `NeutralContractContainsNoReferenceTransportFramingFields`, assembly-reference test and HK00 project graph | neutral fields/dependencies include JSONL/frame/stdin/stdout, Runtime depends on Projection/CLI, or Projection references JSON serializer | exact six neutral fields; Runtime has no Projection/CLI; Projection has no CLI/System.Text.Json and depends only on Protocol + Runtime |

## Additional fail-closed controls

- invalid strict UTF-8 returns `transport.invalid_utf8`;
- a second one-shot frame returns `transport.multiple_frames` and neither frame is dispatched;
- unknown/duplicate envelope fields return stable invalid-frame/request diagnostics;
- unknown launch options return exit 64 with empty stdout;
- unknown capabilities remain canonical `contract.unknown_capability`, distinct from transport and deadline failures;
- canonical response objects are emitted with recursively ordinal-sorted keys;
- the independent repository/static oracle turns RED for unowned projects, undeclared graph edges or candidate/index divergence.

Implementation/test/evidence SHA `9632884059abf483254e7f15eb01e1083c6e1762`: focused HK07A 15/15 GREEN, full regression 151/151 GREEN, exact-SHA receipt GREEN.
