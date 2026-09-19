# WP-HK-04 — Independent Reviewer verdict (repair cycle 1)

Reviewer verdict: `FAIL`
Reviewed candidate SHA: `dc81b054c5f2c6fdda7bf0da84e9b3f99a6457ce`
PR: #19
Reviewer: independent session, no Worker/implementation role on this candidate
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.5
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.2
fail_cycle after this verdict: `2`

## Handoff preconditions (all verified, all GREEN)

| Check | Observation |
|---|---|
| PR HEAD == `Frozen candidate SHA` | `dc81b054c5f2c6fdda7bf0da84e9b3f99a6457ce` on both |
| Branch frozen / `FROZEN_FOR_REVIEW` / `Worker verdict: IN_REVIEW` | recorded in PR #19 body |
| `Worker pre-review: CLEAN` | recorded, evidence `Docs/evidence/WP-HK-04/WORKER_PRE_REVIEW.md` |
| `PREDECESSOR_CONTRACT_CHECK` recorded before implementation | `Docs/evidence/WP-HK-04/WORKER_PLAN.md`, commit `9ce4b7f` (first candidate commit) |
| Transfer/repair lineage preserved | Transfer SHA `28695c9d…`, Worker history preserved, `fail_cycle: 1` not reset |
| Frozen exact-SHA verification | run `35448483093`, job `Freeze exact-SHA validation` = success on the frozen SHA |
| `REVIEW_READY` persisted for exact SHA | PR comment `review-ready:19:dc81b054…` |
| Canonical observation on frozen SHA | run `35448423288`: locked restore, Release build 0 warnings / 0 errors, positives 9/9, causal controls 7/7, regression 86/86, receipt `Result: GREEN` |

Process state is valid. This FAIL is a substantive finding, not a handoff defect.

## Predecessor contract reconstruction (independent)

HK-03 reviewed SHA `8c20a380003c082fa9bd472d3233afa9654fb231`, merged `d8b808450ee7863726d718a25c5756534113fcfd`, baseline `ecebd054…`. Inherited and consumed, not re-proved: HK02 canonical state model and deterministic content hash; HK01 single canonical composition/discovery path and the filesystem-derived production-assembly route universe (`LoadProductionAssemblies()` in `Hk01CanonicalContractTests.cs`); HK03 bounded deterministic reads and revision/hash anchors.

The finding below is **not** on an inherited boundary. "No public command may bypass the canonical mutation pipeline to change authorable state privately" is a guarantee HK04 newly owns (WP-HK-04 Acceptance; Worker plan, "Guarantees newly owned by HK04"). No predecessor guarantee covers it: HK03's `ReadsRemainSideEffectFreeAndRepeatableAgainstSameState` exercises one read route against a fixed state source and is not a universal effective oracle over public read routes.

## BLOCKING FINDING — effective write-authority oracle omits objects by construction

### Violated criteria

- WP-HK-04 Acceptance: *"No public command may bypass the canonical mutation pipeline to change authorable state privately."*
- WP-HK-04 Acceptance: *"Mutation dispatcher surface == discovered mutation surface == transactional surface is mechanically checked."*
- `FOUNDATIONAL_PROOF_STANDARD.md` — Independent-universe rule, and `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` as asserted in `PROOF_MATRIX.md`.

### Claim under test

`PROOF_MATRIX.md`, obligation *"public canonical mutation cannot bypass transactional handler surface"*, claims completeness through two enforcement points sharing one oracle:

1. `CapabilityRoute` constructor (`src/Arkus.Harness.Runtime/RuntimeBindings.cs:50-56`) rejects a handler that carries write authority but is not `ITransactionalMutationHandler`;
2. `MutationSurfaceConformance` (`src/Arkus.Harness.Runtime/MutationSurfaceConformance.cs:160-176`) independently enumerates authority-bearing handler types without reading `SideEffect`.

Both delegate the decision to `MutationAuthorityInspector`.

### Defect

`MutationAuthorityInspector` only recognises authority that is reachable through fields whose **runtime type (object walk) or declared type (type walk) lives in the same assembly as the handler**, with a single exception for `Delegate`:

- `src/Arkus.Harness.Runtime/MutationAuthorityInspector.cs:85` — `if (!root && type.Assembly != handlerAssembly) return false;`
- `src/Arkus.Harness.Runtime/MutationAuthorityInspector.cs:106` — recursion requires `nested.GetType().Assembly == handlerAssembly || nested is Delegate`
- `src/Arkus.Harness.Runtime/MutationAuthorityInspector.cs:52` — type walk requires `field.FieldType.Assembly == handlerAssembly`

`IsDirectAuthorityType` (`:133`) matches only a field/object that *is* `ICanonicalWorldMutationCommitter` or `TransactionalWorldAuthoringSession`. Consequently one level of ordinary indirection removes the handler from the authority universe.

`TransactionalWorldAuthoringSession` is `public sealed` (`src/Arkus.Game.Authoring/WorldMutationService.cs:16`) with a `public Apply` (`:66`), and the WP itself declares that public `Apply` to be effective write authority. So the payload is reachable without any private access.

### Failure scenario (no reflection, no toolchain subversion, ordinary C#)

A public handler declared `ReadOnly` in the canonical contract, holding the authoritative session behind any BCL container or any type defined in another assembly:

```csharp
[PublicCapabilityRoute("arkus.base", "engine.observe", "1.0")]
public sealed class IndirectBypassHandler : ICanonicalCapabilityHandler
{
    // TransactionalWorldAuthoringSession[] -> Type.Assembly is Arkus.Game.Authoring
    // List<TransactionalWorldAuthoringSession> -> Type.Assembly is System.Private.CoreLib
    // Either way: != handler assembly, so the walk stops before reaching the session.
    private readonly List<TransactionalWorldAuthoringSession> _sessions;
    private readonly IReadOnlyDictionary<string, object?> _mutation;

    public CapabilityInvocationResult Invoke(
        CapabilityInvocationContext context,
        IReadOnlyDictionary<string, object?> request)
        => _sessions[0].Apply(_mutation);   // commits canonical state
}
```

Traced against the candidate bytes:

1. `CapabilityRoute` construction — object walk, root handler: field type is `List<…>`, not a direct authority type; `nested.GetType().Assembly` is `System.Private.CoreLib` (or `Arkus.Game.Authoring` for the array form), `!= handlerAssembly`, and it is not a `Delegate`, so line 106 skips recursion. No authority detected, **publication succeeds**.
2. `MutationSurfaceConformance` — type walk, line 52 skips the field for the same reason, so the handler never enters `effectiveWriteAuthority`. `discoveredMutation` is unchanged (the route stays `ReadOnly`), so **no `mutation-surface.extra` issue is raised and the report is conformant**.
3. HK01 route-universe conformance stays green, because the route is declared in the composed contract as an ordinary read capability.
4. Invoking the route really replaces `_current` and advances revision/hash, exactly as the accepted fixture `HiddenCanonicalMutationBypassHandler` demonstrates for the direct-field form.

The existing causal control passes only because its fixture holds the session in a **direct field of the same assembly** (`tests/Arkus.Harness.MutationAttackFixture/HiddenCanonicalMutationBypassHandler.cs:12`). The mutant class is therefore proven for one field shape, not for the acceptance claim.

### Why this is in-claim and in-boundary

It requires no private reflection, no non-canonical build path and no subversion of the declared trusted base (`RESIDUAL_RISK.md` excludes exactly those, and this is none of them). It uses only public types, the public `Apply` the WP itself classifies as write authority, and `System.Collections.Generic`. `RESIDUAL_RISK.md` → "Future authority-bearing implementations" defers *new writer abstractions*; this defect uses the **current** writer, so that deferral does not cover it.

Accordingly `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` and `FOUNDATIONAL_PROOF_VERDICT: READY` are not supported on this candidate, and freeze blocker *"a known defect class inside the claim would escape detection"* applies.

### Minimal correction boundary — causal, not local

`FOUNDATIONAL_PROOF_STANDARD.md` circuit breaker: this is the **second** independent FAIL on the same foundational class (hidden-mutation-bypass / effective write-authority classification; cycle 1 FAIL on `28695c9d…` was the circular `SideEffect`-derived membership). Per that rule and global stop rule 3, **re-audit the proof boundary before another repair cycle. Do not add a third traversal rule to `MutationAuthorityInspector`.** Deepening the structural walk (unwrap collections, cross assemblies, follow statics) is the "endless bypass enumeration" the standard prohibits, and each added rule has its own next shape.

Correct at the boundary the standard already prescribes (Effective-behaviour rule: *"prefer an oracle over the evaluated/effective result rather than a growing list of forbidden syntax. Static guards remain defence in depth"*). Recommended boundary:

- **Evaluated oracle**: for every route in the composed contract whose definition is not `CanonicalMutation`, invoke it against a live `TransactionalWorldAuthoringSession` and assert the canonical content hash and revision are unchanged. This is assembly-independent, indirection-independent, reflection-independent, and it decides authority by *effect* rather than by object shape. The route set comes from the already-accepted HK01 independent universe, so it cannot self-shrink.
- Keep `MutationAuthorityInspector` and the `CapabilityRoute` publication gate as declared defence in depth, not as the completeness argument.
- Add the causal control for the class actually missed: a `ReadOnly` handler holding authority behind a `List<>`/array/foreign-assembly holder must turn the oracle RED, and GREEN again on revert.
- Alternatively, if the team prefers to spend no further proof budget here, **narrow the declared claim** in `PROOF_MATRIX.md` / `RESIDUAL_RISK.md` to "authority held in a handler-local direct field or delegate closure", record the indirection class as an accepted, named residual risk, and stop asserting `KNOWN_UNDETECTED_DEFECT_CLASSES: 0` for this obligation. That is a legitimate, standard-compliant resolution and is cheaper than another hardening cycle.

Note the proof-budget rule while choosing: the evaluated oracle is *smaller* than the structural machinery already present and would let the next cycle simplify rather than grow.

## Verified as correct (no further action required)

These were attacked and held; the repair Worker should not rework them.

| Acceptance criterion | Evidence inspected | Result |
|---|---|---|
| Deterministic proposed change set before commit | `BuildPlan` single construction path; plan id/hash binding | OK |
| Dry-run same semantic path, no persistence | `Plan`/`DryRun`/`Apply` all call `ParseRequest` + `BuildPlan`; `_current` is assigned only in `Apply` (`WorldMutationService.cs:98`) | OK |
| Atomic apply | complete `WorldState` candidate validated before one reference assignment inside `lock (_gate)`; no partial write path exists | OK |
| Optimistic concurrency | anchor checked at plan time and **re-checked under the commit lock** against live revision + recomputed hash (`:91-96`) before replacement | OK |
| Idempotency | receipt keyed by caller key, request fingerprint compared, exact retry replays the stored plan without advancing revision, different semantics under the same key fail closed (`ReplayOrConflict`, `:517-536`) | OK |
| Change set identifies resources/fields/references | `WorldMutationCoverage.FindMismatches` diffs base vs candidate semantically and fails the plan on *missing or extra* declared effects (`:221-233`); it does not trust operation declarations | OK — genuinely independent |
| Machine-readable conditions | `{code,path,satisfied}` arrays plus revision/hash anchors | OK |
| Forbidden scope absent | baseline→candidate diff confined to Authoring/Runtime/tests/scripts/evidence; no Unity, filesystem writes, undo UI or gameplay commands | OK |
| Engine neutrality | `Arkus.Game.Authoring` carries no engine types; generic `put/remove object|extension` grammar only | OK |

The planner/commit attenuation (`WorldMutationPlannerView`, internal `ICanonicalWorldMutationCommitter`, friend boundary) is a real product improvement over cycle 1 and should be kept.

## Reviewer execution note

The frozen candidate was inspected from an exact-SHA worktree at `dc81b054c5f2c6fdda7bf0da84e9b3f99a6457ce`. This reviewer environment has no .NET SDK and the network policy blocks `builds.dotnet.microsoft.com`, so no local build was executed and none is claimed. Execution evidence is the hosted exact-SHA runs `35448423288` (observation) and `35448483093` (frozen verify), read directly from the GitHub Actions API for this SHA. The blocking finding is a code-level trace of `MutationAuthorityInspector` against the candidate bytes and does not depend on local execution; the repair Worker must convert it into an executable RED→GREEN control.

Per `WORKER_REVIEW_PROTOCOL.md`, this reviewer stops at FAIL and does not repair. A fresh repair Worker owns the next action on the same WP, starting from the proof-boundary re-audit required by the circuit breaker.
