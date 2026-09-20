# WP-HK-07B Worker plan

Baseline SHA: `6173471245da081b21cf66fd164cbb3a30dcba1d`
Branch: `wp/hk-07b-mcp-projection`
Worker state: ACTIVE

## Objective

Implement HK07B only: add MCP over local stdio as a second first-party transport projection of the already accepted `arkus.neutral-projection@1` contract, and prove cross-transport semantic conformance against the deterministic HK07A reference transport. MCP framing/tool metadata may adapt canonical definitions to MCP protocol shapes, but it may not redefine capability identity, request/result/error meaning, mutation authority, discovery completeness, or the canonical inventory.

This Worker will not change HK07A neutral projection semantics merely to fit MCP and will not add HTTP/cloud, Unity/editor integration, model-vendor orchestration, HK08 batching/pagination/compact-output semantics, or HK09 host-policy/resource-governance work.

## Ownership and baseline

- Current `main` and Worker baseline: `6173471245da081b21cf66fd164cbb3a30dcba1d`.
- HK07A implementation and accepted post-PASS DocSync are present in that baseline.
- The baseline also contains the separately merged production-blueprint PR #45; it is documentation/product planning and does not change HK07A transport semantics.
- No open HK07B pull request or remote HK07B branch existed when this Worker started.
- Canonical implementation branch: `wp/hk-07b-mcp-projection`.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-07A`.

- Reviewed frozen candidate: `f2ef88980b38482a1a635f4eeb0582ee49d735ec`.
- Independent review: PASS (`#5259732343`).
- Exact-SHA validation: Actions `35492562005` GREEN; artifact `10599004891`.
- Implementation merge SHA: `f70a81b5115cd2a6b0c8b1cb9c5dd657d5f3792b`.
- Post-PASS DocSync merge: PR `#46`, contained in the exact baseline above.

Inherited guarantees consumed rather than re-proved:

1. `arkus.neutral-projection@1` is the accepted transport-neutral request/outcome/admission contract above Runtime; HK07B must conform to it without semantic amendment.
2. `ComposedContract.Definitions` is the single capability/schema inventory and `ComposedContract.Dispatch` is the public invocation authority. A transport does not decide which capabilities exist.
3. HK07A already proves generic projection of accepted base + scoped canonical capabilities, including a synthetic scoped capability, with no adapter registry.
4. Canonical result/error meaning and admission-only cancellation/timeout semantics are already frozen by HK07A. Once canonical dispatch begins, its truthful outcome wins.
5. The deterministic JSONL reference transport is accepted as the first projection/oracle, including its process-local host composition and fresh-process read/validation/mutation/provenance/snapshot/diff/replay behavior.
6. HK01-HK06C guarantees beneath that projection (canonical schemas/versioning, state/inspection, transaction/validation, provenance, snapshot/diff/import/replay) are consumed through the accepted dispatcher and are not re-proved here.

HK07B newly owns:

1. A first-party MCP stdio adapter using a pinned approved MCP implementation behind an Arkus-owned conformance boundary.
2. Mechanical MCP tool discovery from the composed canonical capability definitions, including collision-safe transport naming/version mapping and canonical request/success/error schema projection; no hand-maintained command/schema registry.
3. MCP call dispatch through `NeutralProjectionService` only, with canonical structured success/error preserved inside MCP structured results and MCP framing differences kept transport-local.
4. Cross-transport conformance for representative discovery/read/validation/mutation/provenance/diff/snapshot/replay outcomes and errors.
5. Completeness against an independently obtained canonical inventory, including synthetic scoped contribution visibility and rejection/detection of missing, duplicate or adapter-only MCP capabilities.
6. Clean cancellation/error mapping at the MCP boundary without creating a post-dispatch cancellation lie or an alternate mutation path.
7. Dependency adoption evidence and replaceability: replacing/removing the MCP SDK may change only the MCP adapter/integration, not Runtime, canonical contracts, or `arkus.neutral-projection@1`.
8. A negative-conformance control for an MCP-shaped requirement that would require amending the accepted neutral semantics: the adapter must reject/contain the mismatch rather than move transport concerns upstream.

Predecessor reopen trigger: concrete effective evidence that `arkus.neutral-projection@1` cannot faithfully represent an already accepted canonical request/outcome needed by both transports, or that the accepted HK07A projection itself changes canonical meaning/omits a composed capability. An MCP SDK naming/framing/schema limitation is HK07B-owned adapter work and is not by itself evidence to reopen HK07A. No predecessor contradiction is currently present.

## Architecture decision

MCP is a sibling transport of `arkus.reference.jsonl@1`, never a parent of or replacement for the neutral projection. The MCP adapter obtains `NeutralProjectionService.Capabilities`, deterministically maps each `CapabilityDefinition` to an MCP Tool, invokes only `NeutralProjectionService.InvokeAsync`, and translates the resulting neutral outcome to MCP structured content. Any MCP-specific metadata/name encoding stays in the adapter and remains mechanically traceable to the canonical key.

The official C# MCP SDK is linked only by the MCP adapter/test surface. Protocol/Runtime/Projection do not reference MCP assemblies or types. Conformance tests compare normalized neutral semantic outcomes rather than raw transport frames, so the SDK cannot become the semantic oracle.

## Proof plan

Positive/effective evidence:

- official MCP client over stdio discovers exactly the canonical inventory projected as MCP tools;
- representative capabilities invoked through MCP normalize to the same neutral outcomes as the accepted JSONL reference transport;
- a synthetic scoped canonical provider appears automatically without MCP registry edits;
- canonical validation/version/domain errors remain canonically identifiable in MCP structured results;
- cancellation before dispatch does not invoke the synthetic canonical handler;
- project/assembly graph keeps MCP SDK references outside Protocol/Runtime/Projection.

Causal negative-conformance classes map directly to WP acceptance:

- omit one canonical capability from MCP discovery;
- alter request/result/error meaning during MCP projection;
- publish an undeclared/adapter-only tool or mutation path;
- use an adapter-owned fixed registry as the completeness oracle;
- omit a synthetic scoped canonical contribution;
- drift cancellation/error classification;
- make SDK replacement require a canonical/projection change;
- impose an MCP-shaped requirement that would amend `arkus.neutral-projection@1` rather than remain framing/adapter metadata.

Independent/evaluated oracle:

Expected capability identities come from independently composed canonical `ComposedContract.Definitions`; MCP discovery is obtained through the actual SDK/client path. Semantic comparisons normalize both transport outputs to the already accepted neutral outcome shape. Static project/assembly checks independently enforce dependency direction. The external MCP SDK itself is trusted only for documented protocol mechanics, not for Arkus semantic completeness.

## Proof/trust boundary

Inside the claim: repository-owned MCP adapter/project, canonical-to-MCP mapping, SDK configuration, stdio process integration, dependency boundary, and cross-transport conformance controls. Trusted base remains exact Git/.NET/MSBuild/NuGet behavior, normal OS stdio/process primitives, documented MCP SDK 2.2.0 behavior at its protocol boundary, and accepted HK01-HK07A semantics.

Outside HK07B: HTTP/cloud transport, authentication/tenancy, hosted external services, UI, Unity/editor behavior, model-vendor orchestration, HK08 efficiency/pagination/batching, HK09 resource policy, and arbitrary out-of-contract SDK/runtime corruption.

Initial `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
