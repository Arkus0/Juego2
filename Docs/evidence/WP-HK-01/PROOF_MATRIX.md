# WP-HK-01 foundational proof matrix

Baseline SHA: `9db7a1ffffa02c02af7889de29960de1f19d2755`  
Implementation observation SHA: `6cf1e6c5cdd76946f57a92b9d56a6b77267fc1dd`  
Observation run: GitHub Actions `35432812488`  
Binding contract: `Docs/workpacks/HK/WP-HK-01.md`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` v1.2

## Claim and trust boundary

HK01 claims that every capability made public through the Arkus canonical runtime is owned by one composed canonical contract inventory, has machine-readable portable request/success/error semantics, and is equivalent across canonical dispatch, discovery/projection and the independently enumerated effective route surface.

The independent production route universe is obtained from production projects independently discovered under `src/<project>/<project>.csproj`, their effective Release assemblies, and reflection over every concrete `ICanonicalCapabilityHandler` in those assemblies. Provider IDs, discovery metadata and the canonical registry are deliberately not inputs to that enumeration. Synthetic scoped-provider conformance separately enumerates the runtime and fixture assemblies and proves the accepted base + scoped composed inventory end to end.

Trusted infrastructure: Git exact-checkout/object semantics; pinned .NET SDK/runtime; normal documented compiler/MSBuild/NuGet and reflection/assembly-loading behavior; filesystem and GitHub Actions runner. HK01 does not recursively prove those trusted components or arbitrary unsupported dynamically loaded assemblies.

No new external dependency is introduced by HK01.

| Proof obligation | Completeness argument | Positive evidence | Negative control / causal attack | Result |
|---|---|---|---|---|
| Canonical versioned contract is transport-neutral | `Arkus.Harness.Protocol` owns identity/version, portable schema and semantic metadata; no transport or engine dependency was added. | `CanonicalContracts.cs`, `JsonSchemaDocument.cs`, `SchemaNode.cs`; Release build | transport provider/format rejection; runtime-object portability attack | GREEN |
| Request/success/error schemas are machine-readable and one-source | Every accepted definition requires all three schemas; projection is generated directly from canonical definitions. | canonical positive tests; projection fingerprint conformance | missing schema; projection omission; semantic alteration | GREEN |
| Structured errors are stable and repairable | One canonical `StructuredError` schema is required for every public capability and includes machine code, message, path, context, retryability and optional repair hint. | validator + runtime error paths | noncanonical/missing schema paths covered by composition/runtime tests | GREEN |
| `system.describe` discovers whole composed inventory | Handler returns the composed contract projection, not a separate registry. Base and synthetic scoped inventories are invoked through dispatch. | `BaseContractComposesAndSystemDescribeDiscoversWholeSurface`; `SystemDescribeReturnsEveryAcceptedBaseAndScopedCapabilityVersion` | projected omission attack | GREEN |
| Base + scoped composition has one Arkus-owned admission path | Composer validates provider kind, provider identity, scope ownership, namespace ownership, definitions and exact handler bindings before constructing `ComposedContract`. | synthetic scoped composition + scoped conformance tests | public scoped route without composition; binding identity lie | GREEN |
| Scope/namespace/identity conflicts fail closed | Provider IDs and scopes are unique; namespaces may not overlap; capability keys/routes may not duplicate. | composer tests | duplicate scope with disjoint namespaces; overlapping namespace; duplicate capability | GREEN |
| Mandatory semantic/policy metadata fails closed | Validator rejects unknown/missing side-effect, determinism, concurrency, idempotency, batching, repair and privilege/transaction/provenance policy. | canonical definitions validate in positive suite | missing policy; missing success/error schema | GREEN |
| Canonical runtime boundary fails closed | Dispatch negotiates from discovered versions, validates portable JSON-compatible request data and schema before handler execution, then validates success/error outputs. | runtime positive and version negotiation tests | unknown capability; unsupported version; schema-invalid payload; CLR/runtime object through `Any` | GREEN |
| Engine/runtime implementation types cannot become canonical data | Contract formats are a finite portable set and runtime values are recursively restricted to JSON-compatible data. Logical references are namespaced strings. | portable logical-reference positive test | `dotnet-type`; transport format; non-JSON runtime object | GREEN |
| Compatibility/version rules are explicit | Same-version semantic drift is breaking; same-major minor increments may be compatible/additive only under conservative rules; major changes are breaking. Runtime selects highest accepted compatible version. | compatibility + negotiation tests | semantic drift without version bump | GREEN |
| Dispatcher surface equals canonical discovered/schema surface | Definitions are compared with composed route keys; every definition is schema-validated; projection fingerprints must equal canonical definitions. | `CanonicalContractConformance` positive tests | dispatcher/binding mismatch; projection omission/alteration | GREEN |
| Completeness universe cannot self-shrink through provider/discovery metadata | Route enumeration takes assemblies only and inspects every concrete public handler; it has no provider-ID filter. Production assemblies are independently discovered from `src` project directories/build outputs. | production route-universe conformance | deleting canonical registry metadata while handler remains; unknown provider route | GREEN |
| Synthetic scoped providers satisfy the same completeness rule | Accepted base + two disjoint scoped providers produce four definitions, four effective routes and four projected capabilities from independently enumerated runtime/test assemblies. | `AcceptedSyntheticScopedSurfaceIsIndependentlyEnumerableAndConformant` | uncomposed scoped route attack | GREEN |
| Projection cannot silently omit or alter canonical semantics | Projection constructor consumes canonical definitions and conformance compares identity + semantic fingerprints both directions. | projection conformance positive path | omission, extra/altered schema semantics | GREEN |
| Canonical Linux build/test path remains healthy | HK01 canonical observation uses locked restore, Release build and full regression on exact clean SHA. | Actions run `35432812488`: 0 warnings/errors; 9/9 canonical positives; 18/18 causal negative controls; 28/28 total tests | exact-SHA/clean-worktree gates in observation script | GREEN |

## Proof-budget conclusion

The proof surface maps directly to HK01 acceptance and required self-attacks. The main proof-boundary defect discovered during Worker falsification—the provider-filtered route universe—was removed rather than patched with more filter cases. Subsequent controls close concrete contract gaps (scope ownership, portable runtime data and version stability) rather than arbitrary toolchain hostility.

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: exact Git candidate + pinned .NET/effective production assemblies + documented reflection/CI infrastructure; canonical public routes and explicit synthetic scoped fixtures are in claim
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
