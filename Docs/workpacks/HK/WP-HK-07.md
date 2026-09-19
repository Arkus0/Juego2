# WP-HK-07 — Headless host + transport projections

Status: SUPERSEDED — SPLIT BEFORE IMPLEMENTATION  
Class: FOUNDATIONAL UMBRELLA  
Depends on: `WP-HK-06C`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Resolution

This workpack was intentionally split before implementation because it coupled two distinct independently reviewable boundaries: freezing a real transport-neutral external host/projection contract, and then proving that MCP can conform to that already accepted contract as a second projection.

Do **not** implement or review this umbrella as a single WP.

Execution order is now:

1. `WP-HK-07A — Headless host + neutral projection contract + reference transport`
2. `WP-HK-07B — MCP as second projection + cross-transport conformance`
3. `WP-HK-08` only after `WP-HK-07B` PASS + merge + DocSync.

## Split rationale

The original HK07 objective remains unchanged in aggregate: external processes must be able to discover and exercise the complete accepted Arkus contract through a deterministic reference transport and a standards-compatible MCP projection, with both transports sharing one canonical semantic source of truth.

The split strengthens the proof of transport neutrality:

- HK07A owns process hosting, the accepted neutral projection contract, deterministic JSONL/reference framing, failure behaviour, cancellation and the no-second-registry completeness boundary. It may not pre-cook MCP-specific abstractions.
- HK07B consumes the accepted HK07A host/reference transport and must fit MCP into that already accepted projection contract without semantic amendment. Cross-transport conformance, SDK replaceability and absence of adapter-only semantics are therefore observable results rather than claims made inside one combined implementation.

If HK07B produces concrete evidence that MCP cannot conform without changing the accepted neutral projection semantics, that is evidence that HK07A was not truly transport-neutral. The correct response is to reopen/repair the predecessor boundary under the normal review protocol, not to smuggle a semantic amendment into HK07B.

Accepted predecessor guarantees from HK01–HK06C compose forward and should not be redundantly re-proved without concrete contradictory evidence.

## Original aggregate DoD

When HK07A and HK07B are both COMPLETE, external processes can discover and exercise the complete accepted H0 contract through both the deterministic reference transport and MCP, with deterministic failure behaviour, no parallel adapter registry, mechanically demonstrated semantic equivalence and a demonstrated second-projection proof of transport neutrality.
