# WP-HK-07B — MCP as second projection + cross-transport conformance

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-07A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Implement MCP as a **second** standards-compatible projection of the already accepted HK07A neutral projection contract and prove that it conforms without amending that contract or becoming a new semantic source of truth.

## Acceptance

- A first-party MCP adapter projects the accepted canonical Arkus contract through a pinned approved SDK/implementation rather than maintaining an independent command/schema registry.
- **MCP must conform to the accepted HK07A neutral projection contract without requiring semantic amendments to that contract.** A need to change canonical/projection semantics in order to fit MCP is evidence of real transport coupling and cannot be hidden as an adapter-only fix.
- MCP and the accepted HK07A reference transport expose semantically equivalent capabilities for the accepted H0 surface; framing differences are allowed, semantic drift is not.
- MCP discovery/request/result/error schemas are derived from or mechanically reconciled with the composed canonical contract rather than hand-maintained duplicates.
- Scoped canonical capability providers accepted by composition become visible through MCP without rewriting canonical semantics or a fixed/base-only adapter list.
- MCP cannot expose an adapter-only capability or mutation path that is absent from canonical composition.
- MCP timeout/cancellation and error behaviour map cleanly to the accepted HK07A projection/canonical semantics.
- External MCP SDK usage satisfies `DEPENDENCY_IP_POLICY.md` and remains replaceable behind Arkus conformance tests.
- Removing/replacing the MCP SDK must not require canonical kernel or accepted HK07A projection-semantic changes.
- Cross-transport conformance covers representative discovery, read, validation, mutation, provenance, diff, snapshot and replay flows and compares normalized semantic results/errors.
- The MCP path remains non-interactive and does not introduce Unity, editor, hosted-service or model-vendor dependencies into canonical H0 behaviour.
- Accepted HK07A host/reference-transport behaviour and neutral projection contract remain consumed, not re-proved, unless MCP exposes concrete evidence that the supposedly neutral contract was actually transport-coupled.

## Required negative-conformance tests

RED→GREEN for: canonical capability missing from MCP projection, MCP changing request/result/error meaning, MCP exposing an undeclared mutation path, transport-owned registry acting as the only completeness oracle, synthetic scoped canonical capability omitted from MCP, adapter-only capability published without canonical composition, cancellation/error semantic drift, SDK replacement requiring canonical/projection semantic changes, and a deliberately shaped MCP requirement that would force an amendment to the accepted HK07A neutral contract.

## Forbidden scope

Changing the accepted HK07A neutral projection contract merely to accommodate MCP, HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration, batching/compact/pagination efficiency work owned by HK08.

## DoD

External clients can exercise the complete accepted harness semantics through both the deterministic reference transport and MCP, and conformance demonstrates the stronger result: MCP fits as a genuine second projection **without changing the already accepted neutral contract**, proving transport neutrality rather than merely asserting it; independent PASS.
