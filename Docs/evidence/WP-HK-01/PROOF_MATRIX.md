# WP-HK-01 foundational proof matrix

Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Repair-cycle-2 implementation observation SHA: `cfcb9ab25977a1f58cc85554c5150d98574e01e3`  
Observation run: GitHub Actions `35435242336`

Binding contract: `Docs/workpacks/HK/WP-HK-01.md`  
Binding architecture: `Docs/engineering/PRODUCT_ARCHITECTURE.md` v1.2  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.2

## Claim and trust boundary

HK01 claims that every capability made public through the Arkus canonical runtime is owned by one composed canonical contract inventory, has machine-readable portable request/success/error semantics, and is equivalent across canonical dispatch, discovery/projection and the independently enumerated effective route surface.

For same-major version evolution, HK01 additionally claims that a composed negotiable chain cannot silently narrow request acceptance: every request admitted by an earlier minor schema must remain admitted by the next minor schema. The supported canonical schema subset evaluates this relation structurally and recursively; uncertainty fails closed as breaking. Composition enforces the relation before a `ComposedContract` can exist, so dispatch cannot negotiate across a known breaking same-major chain.

Portable logical references are canonical string data in an Arkus-owned identity space: `ref.<provider-id>.<domain>`. Schema validation rejects non-canonical logical-reference namespaces and implementation-choice enums; composition recursively verifies that request/success/error logical-reference namespaces belong to the contributing provider. Runtime values remain recursively JSON-compatible. The proof does not attempt to infer hidden intent from an otherwise valid opaque domain token; reviewed provider semantics are part of the admission boundary, while actual CLR/editor/runtime objects, assembly-qualified identities and foreign-provider reference namespaces fail closed mechanically.

The independent production route universe is obtained from production projects independently discovered under `src/<project>/<project>.csproj`, their effective Release assemblies, and reflection over every concrete `ICanonicalCapabilityHandler` in those assemblies. Provider IDs, discovery metadata and the canonical registry are deliberately not inputs to that enumeration. Synthetic scoped-provider conformance separately enumerates the runtime and fixture assemblies and proves the accepted base + scoped composed inventory end to end.

Trusted infrastructure: Git exact-checkout/object semantics; pinned .NET SDK/runtime; normal documented compiler/MSBuild/NuGet and reflection/assembly-loading behavior; filesystem and GitHub Actions runner. HK01 does not recursively prove those trusted components or arbitrary unsupported dynamically loaded assemblies.

No new external dependency is introduced by HK01.

| Proof obligation | Completeness argument | Positive evidence | Negative control / causal attack | Result |
|---|---|---|---|---|
| Canonical versioned contract is transport-neutral | `Arkus.Harness.Protocol` owns identity/version, portable schema and semantic metadata; no transport or engine dependency was added. | `CanonicalContracts.cs`, `JsonSchemaDocument.cs`, `SchemaNode.cs`; Release build | transport provider/format rejection; runtime-object portability attack | GREEN |
| Request/success/error schemas are machine-readable and one-source | Every accepted definition requires all three schemas; projection is generated from canonical definitions and emitted portable data is checked by an independent expected-data oracle. | canonical positive tests; emitted-artifact structural conformance | missing schema; projection omission; schema/metadata alteration; optional-field omission | GREEN |
| Structured errors are stable and repairable | One canonical `StructuredError` schema is required for every public capability. | validator + runtime error paths | noncanonical/missing schema paths | GREEN |
| `system.describe` discovers whole composed inventory | Handler returns the composed contract projection, not a separate registry. | base + synthetic scoped discovery tests | projected omission attack | GREEN |
| Base + scoped composition has one Arkus-owned admission path | Composer validates provider/scope/namespace ownership, definitions, exact bindings, logical-reference ownership and same-major evolution before constructing `ComposedContract`. | synthetic scoped composition + compatible negotiation | uncomposed route; binding lie; foreign reference namespace; breaking version chain | GREEN |
| Scope/namespace/identity conflicts fail closed | Provider IDs/scopes are unique; namespaces may not overlap; capability keys/routes may not duplicate. | composer tests | duplicate scope/namespace/capability | GREEN |
| Mandatory semantic/policy metadata fails closed | Validator rejects unknown/missing side-effect, determinism, concurrency, idempotency, batching, repair and policy metadata. | canonical positive suite | missing policy/schema | GREEN |
| Canonical runtime boundary fails closed | Dispatch negotiates only a successfully composed inventory, validates portable request data/schema before invocation and validates result/error data after invocation. | runtime + compatible negotiation tests | unknown/unsupported version; invalid request; CLR object through `Any` | GREEN |
| Engine/runtime implementation types cannot become canonical data | Formats are finite; runtime data is JSON-compatible; logical references use canonical `ref.<provider>.<domain>` identities and provider ownership. | portable logical-reference positive test | `dotnet-type`; transport format; assembly-qualified logical-reference namespace; foreign-provider namespace; logical-reference enum | GREEN |
| Same-major evolution preserves request acceptance | Structural recursive relation proves previous accepted values remain accepted by next; typed additions to previously open objects are breaking; widening to `Any`, enum widening and other proven relaxations may be additive. | compatible v1.0→v1.1 negotiation; explicit open-object `Any` widening | typed property on open object; nested open-object narrowing; unchanged-version semantic drift | GREEN |
| Compatibility governs negotiation | Composer validates every adjacent same-major version pair and refuses to construct a contract on a breaking edge; highest-version dispatch therefore operates only on admitted chains. | compatible chain composes + dispatches highest accepted | breaking same-major chain returns `composition.breaking_same_major_version` and no contract | GREEN |
| Dispatcher surface equals canonical discovered/schema surface | Definitions are compared with composed route keys; conformance dispatches `system.describe` and compares returned portable artifact against independent structural oracle. | conformance positives | binding mismatch; projection omission/alteration | GREEN |
| Completeness universe cannot self-shrink through provider/discovery metadata | Route enumeration takes assemblies only and inspects every concrete public handler; no provider-ID filter. | production route-universe conformance | deleting registry metadata; unknown provider route | GREEN |
| Synthetic scoped providers satisfy the same completeness rule | Accepted base + scoped providers produce matching definitions/routes/projected capabilities from independently enumerated assemblies. | scoped conformance | uncomposed scoped route | GREEN |
| Projection cannot silently omit or alter canonical semantics | Runtime conformance evaluates actual `system.describe`; separate model-to-data oracle does not call production `ToData()` projectors. | emitted-artifact conformance | omission, alteration, optional-field omission, delimiter collisions | GREEN |
| Canonical Linux build/test path remains healthy | Exact clean SHA, locked restore, Release build, canonical positives, causal controls and full regression. | Actions `35435242336`: 0 warnings/errors; 9/9 positives; 29/29 causal controls; 39/39 regression | exact-SHA/clean-worktree gates | GREEN |

## Circuit-breaker re-audit conclusion

Repair cycle 2 did not special-case the Reviewer examples. Portability was redefined around a canonical provider-owned logical-reference identity boundary, while compatibility was redefined as a conservative request-acceptance relation and moved into composition so negotiation is downstream of compatibility admission. Worker pre-review then found that the seven new controls were initially only in the regression set; their class was moved under the canonical causal-control filter before evidence reconciliation. The resulting proof protects the causal classes that produced both Reviewer blockers without expanding into arbitrary semantic-word denylisting or trusted-toolchain hostility.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: exact Git candidate + pinned .NET/effective production assemblies + documented reflection/CI infrastructure; canonical public routes and reviewed provider contributions are in claim
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
