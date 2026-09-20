# WP-HK-07A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-07A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

HK07A turns the accepted composed canonical H0 contract into a non-interactive process through one transport-neutral projection and one deterministic JSON Lines reference adapter. The composed `ComposedContract.Definitions` inventory remains the only capability/schema authority; all invocations reach `ComposedContract.Dispatch`; canonical request/result/error meaning is preserved; adapter framing, diagnostics, process failure and admission cancellation/timeout behavior are explicit and isolated. A fresh external client can discover and exercise the accepted read, validation, mutation, provenance, diff, snapshot and replay surface without linking to implementation code.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| canonical host launches non-interactively | pinned local .NET process and empty process-local H0 session | canonical exact-SHA script restores, builds and starts the real Release CLI through process tests | real one-shot and persistent stream processes launch with no prompt/network/Unity and return canonical frames | unknown option fails before stdout; process harness has a 30-second fail-closed bound | PASS | hostile OS/process behavior is trusted/out of scope |
| neutral projection is above transport framing | `arkus.neutral-projection@1` request/outcome, inventory and admission semantics | neutral assembly references only Protocol + Runtime and contains no CLI/JSON framing dependency | neutral request/outcome exercised directly; Runtime produces the empty portable composition | exact neutral request fields plus assembly-reference checks turn RED on JSONL/stdin/stdout leakage | PASS | second-transport neutrality is intentionally tested by HK07B |
| every composed canonical capability is projected generically | exact `ComposedContract.Definitions` universe | expected universe is composed independently; projection returns those definitions directly and dispatches through the same contract | production composition matches an independently composed canonical contract; current process discovery exposes 18 capabilities | fixed base-only key list omits synthetic `engine.observe@1.0` and completeness oracle returns `projection.omitted_capability` | PASS | no claim that current production composition contains an engine-scoped provider |
| scoped capabilities cannot disappear silently | canonical HK01 scoped contribution | synthetic scoped definition/route enters through accepted composer, without transport changes or registry entries | neutral discovery publishes and invokes `engine.observe@1.0` | base-only transport-registry simulation is observably incomplete | PASS | provider installation/lifecycle is not HK07A scope |
| canonical request/result/error meaning is preserved | portable data and canonical version negotiation | neutral service accepts canonical capability/range/arguments and wraps the exact canonical invocation result/error | success discovery matches canonical projection data; external client exercises all representative semantic classes | unknown capability remains `contract.unknown_capability` with `failureKind=canonical` | PASS | predecessor semantic correctness is consumed, not re-proved |
| reference transport is deterministic and supports required modes | strict UTF-8 JSONL stream plus one-shot stdin/file | a single codec parses exact envelopes and a single ordinal serializer writes every outcome; no per-capability adapter code exists | stdin one-shot, file one-shot and persistent external clients pass; repeated discovery bytes have identical SHA-256 | ambient `ARKUS_*` changes produce byte-identical stdout; unknown/duplicate envelope fields fail closed | PASS | reference frame/resource policy is not a general operational SLO |
| stdout cannot be contaminated by diagnostics | process stdout/stderr boundary | transport writes frames only to supplied output stream and all lifecycle/usage/fatal text only to diagnostics | `--diagnostics` yields exactly one parseable stdout frame and readiness text on stderr | test fails if `arkus-host` appears on stdout or if stdout has more than one frame | PASS | arbitrary native/runtime writes outside repository code are trusted |
| framing/failure/exit behavior is stable | documented reference adapter boundary | frame reader bounds every line and codec requires strict UTF-8/JSON/exact fields; Program owns fatal mapping | valid EOF-without-newline works; errors are framed structured outcomes | distinct malformed, truncated, invalid-UTF8, oversized and multiple-frame paths plus usage failure | PASS | fatal hardware/runtime termination cannot guarantee a final frame |
| cancellation/timeout cannot lie about committed effects | neutral admission before synchronous canonical dispatch | one gate owns admission; cancellation/deadline is checked before dispatcher entry; after entry the canonical outcome wins | ordinary request dispatch succeeds; timeout and cancellation produce distinct neutral kinds/codes | pre-cancelled token and zero deadline invoke the synthetic canonical handler zero times; process timeout is `timed-out`, not canonical | PASS | handlers are not forcibly interrupted after dispatch starts |
| process behavior inputs are reproducible | arguments, stdin/file bytes and fixed initial session | launch parser accepts only documented explicit options; production world ID/state is fixed; no environment-selected registry/world exists | unchanged request returns exact same bytes under three hostile-looking `ARKUS_*` values | environment mutation would turn exact stdout comparison RED | PASS | normal pinned SDK/OS locale/runtime contracts remain trusted |
| Runtime kernel remains transport-independent | tracked project/assembly graph | fixed project graph, source search and runtime/projection assembly references independently bound the layer direction | HK00 repository/static phases GREEN; Release assembly boundary test GREEN | Runtime→Projection/CLI and Projection→CLI/System.Text.Json references make tests/proof RED | PASS | canonical schemas retain historical `JsonSchemaDocument` naming but no serializer dependency |
| production path cannot skip canonical runtime | `ProductionHarnessHost.Create` and Runtime composition seam | Runtime owns the only empty portable-session pairing; projection wraps returned `ComposedContract`; CLI calls that host factory | production discovery equals independently composed canonical discovery and external writes retain accepted journal/replay semantics | replacing production composition with base-only or direct adapter handling omits routes/results and turns completeness/external-client tests RED | PASS | custom embedding code outside the production factory is unsupported |
| fresh external client exercises representative accepted H0 surface | JSONL process boundary only | client starts with `system.describe`, uses only JSON frames/portable artifacts and links to no product assembly | two real processes complete summary, validation, plan/apply, get, journal, snapshot, diff, import and replay to identical final hash/empty diff | any adapter-only shortcut, lost provenance or alternate mutation path breaks process-only assertions | PASS | representative content is bounded, not a universe oracle |
| representative Juego2 content shape fits the host/projection claim | accepted Potes/Liébana authored concepts | plaza, contained building, NPC relation and opaque extension traverse generic public frames and fresh-process portability | content-shape probe completes mutation and reconstructs identical authored state in a second host | accepted validation/hash/diff/replay controls expose containment/reference/provenance drift | PASS | gameplay schedules/transforms/runtime state remain intentionally unclaimed |
| no speculative future adapter abstraction or forbidden scope entered | baseline-to-candidate diff | product additions are one neutral service, one Runtime composition helper and one exercised JSONL adapter | diff audit finds no MCP SDK/type, HTTP/cloud, Unity/editor, GUI or vendor orchestration | assembly/source search and review scope audit would block freeze on such additions | PASS | HK07B/HK08/HK09 retain their declared work |
| accepted predecessor guarantees remain consumed | accepted HK01-HK06C public dispatcher/state semantics | external path calls accepted dispatcher and public artifacts; no alternate state, mutation, journal, snapshot, diff or replay implementation exists | full inherited regression 151/151 GREEN at implementation/test SHA | inherited route, validation, mutation, provenance, portability and replay tests turn RED on drift | PASS | concrete contradictory evidence would reopen a predecessor; none observed |

## Independent/effective universes

1. **Capability universe:** expected identities come from an independently composed accepted `ComposedContract.Definitions`, not from a JSONL list. A synthetic scoped provider proves the universe grows without adapter edits.
2. **Effective invocation path:** the external process client can only exchange JSON frames. Its mutation journal, snapshot and replay equivalence would fail if the adapter bypassed canonical dispatch/authority.
3. **Layer universe:** tracked MSBuild project edges, Release assembly references and exact neutral-envelope fields independently constrain transport concerns to CLI. The HK00 fixed project graph now includes every solution project, including the inherited mutation negative-control fixture.
4. **Failure oracle:** transport parsing produces transport codes; neutral admission produces cancelled/timed-out codes; canonical dispatch produces canonical errors. Tests require these classes to remain distinct.
5. **Output oracle:** independent `System.Text.Json` process tests parse stdout and inspect stderr separately; diagnostics cannot be hidden inside the protocol parser under test.
6. **Representative product shape:** the bounded Potes/Liébana flow checks that the generic transport is useful for intended authored content, but does not define completeness.

## Implementation observation

Remote implementation/test/evidence SHA `acd112665db0fec32e238ed847ae18702cf02320` passed the canonical local exact-SHA observation on the pinned .NET SDK 8.0.425:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk07A*`: 15/15 GREEN;
- full regression: 151/151 GREEN;
- clean candidate before and after: YES;
- receipt command: `scripts/hk07a-observe-exact-sha.sh acd112665db0fec32e238ed847ae18702cf02320` from a clean worktree fetched from the GitHub branch.

The directly bootstrapped HK00 oracle also reported GREEN for its independent repository universe and evaluated static project graph on that clean implementation tree. This reconciled the new Projection project and two inherited graph omissions exposed during pre-review. The current managed local substrate cannot run the optional HK00 effective phase's repeated parallel rebuild/query pattern reliably; HK07A's canonical exact-SHA gate uses a single-node restore/build plus effective focused/full tests and is GREEN.

The subsequent evidence reconciliation is documentation-only. Its resulting exact branch SHA must pass `scripts/hk07a-verify-exact-sha.sh` unchanged before freeze.

## Proof-budget verdict

Product code adds one neutral projection boundary, one canonical empty-session composition helper, and one exercised reference adapter. Proof code maps directly to the eight required negative-conformance classes, external-client DoD and bounded content-shape probe. The inherited fixed-graph reconciliation restores coverage of projects/edges already in the solution; it does not add a new proof framework or re-prove predecessor semantics.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
