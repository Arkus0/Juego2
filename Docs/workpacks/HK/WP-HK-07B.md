# WP-HK-07B — MCP projection + cross-transport conformance

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-07A`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Add a standards-compatible first-party MCP projection of the accepted Arkus contract and prove that MCP and the deterministic reference transport are semantically equivalent projections of one canonical capability universe.

## Acceptance

- A first-party MCP adapter projects the accepted canonical Arkus contract through a pinned approved SDK/implementation rather than maintaining an independent command/schema registry.
- MCP and the accepted HK07A reference transport expose semantically equivalent capabilities for the accepted H0 surface; framing differences are allowed, semantic drift is not.
- MCP discovery/request/result/error schemas are derived from or mechanically reconciled with the composed canonical contract rather than hand-maintained duplicates.
- Scoped canonical capability providers accepted by composition become visible through MCP without rewriting canonical semantics or a fixed/base-only adapter list.
- MCP cannot expose an adapter-only capability or mutation path that is absent from canonical composition.
- MCP timeout/cancellation and error behaviour map cleanly to the accepted canonical semantics.
- External transport SDK usage satisfies `DEPENDENCY_IP_POLICY.md` and remains replaceable behind Arkus conformance tests.
- Removing/replacing the MCP SDK must not require canonical kernel semantic changes.
- Cross-transport conformance covers representative discovery, read, validation, mutation, provenance, diff, snapshot and replay flows and compares normalized semantic results/errors.
- The MCP path remains non-interactive and does not introduce Unity, editor, hosted-service or model-vendor dependencies into canonical H0 behaviour.

## Required negative-conformance tests

RED→GREEN for: canonical capability missing from MCP projection, MCP changing request/result/error meaning, MCP exposing an undeclared mutation path, transport-owned registry acting as the only completeness oracle, synthetic scoped canonical capability omitted from MCP, adapter-only capability published without canonical composition, cancellation/error semantic drift, and removing/replacing the MCP SDK causing canonical kernel changes.

## Forbidden scope

HTTP/cloud service, Unity Editor bridge, GUI, model-vendor-specific orchestration, batching/compact/pagination efficiency work owned by HK08.

## DoD

External clients can exercise the complete accepted harness semantics through both the deterministic reference transport and MCP, and conformance proves that both are projections of the same canonical contract rather than parallel implementations; independent PASS.
