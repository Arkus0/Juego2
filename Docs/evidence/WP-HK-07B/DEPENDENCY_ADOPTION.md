# WP-HK-07B dependency adoption record

## Model Context Protocol C# SDK

- Package family: official `modelcontextprotocol/csharp-sdk`.
- Intended package: `ModelContextProtocol.Core`.
- Exact pinned version: `2.2.0`.
- Upstream release: `v2.2.0` (`6fa3825` release commit shown by upstream release metadata; released 2026-08-13).
- Source: `https://github.com/modelcontextprotocol/csharp-sdk` and NuGet package `ModelContextProtocol.Core`.
- Linkage: direct NuGet runtime dependency of the MCP adapter only; not vendored/copied/generated into canonical Arkus code.
- Exact-version license check: tag `v2.2.0` includes the project's Apache-2.0/MIT transition license text; both are commercially permissive classes allowed by `Docs/engineering/DEPENDENCY_IP_POLICY.md`. Documentation is CC-BY-4.0 and is not copied into the product.
- Arkus guarantee helped: standards-compatible MCP JSON-RPC/stdin-stdout transport and client/server protocol mechanics.
- Explicitly outside dependency authority: canonical capability identity/version, canonical request/success/error schemas, composed inventory completeness, state meaning, mutation/transaction authority, validation identity, provenance/replay meaning, snapshots/diffs, and `arkus.neutral-projection@1` semantics.
- Arkus conformance boundary: MCP-specific code receives only accepted `CapabilityDefinition`/`NeutralProjectionService` inputs and produces/consumes SDK protocol DTOs; cross-transport tests normalize results back to the accepted neutral semantic shape and compare against canonical/reference-transport expectations.
- Replacement strategy: replace SDK transport/protocol binding inside the MCP project while retaining the canonical-to-MCP mapper and conformance suite contract; Protocol, Runtime and Projection projects must not reference `ModelContextProtocol*` assemblies.
- Notices/attribution: preserve/generate applicable Apache-2.0/MIT/third-party notices in the product notice bundle before commercial release; no upstream source is copied by this WP.
- Security/update owner: Arkus harness dependency maintenance; version is centrally pinned and future updates require rerunning MCP conformance plus license review.
- Classification: shipped/local runtime dependency of the MCP adapter; no hosted service and no network dependency required for the stdio path.

## Adoption gate result before implementation

The intended dependency is admissible under the current policy provided implementation keeps it isolated to the MCP adapter and the conformance/replacement controls above remain green. If the SDK requires changing canonical or HK07A neutral semantics, adoption fails for this WP; the Worker must redesign/isolate the adapter rather than silently alter the accepted product boundary.
