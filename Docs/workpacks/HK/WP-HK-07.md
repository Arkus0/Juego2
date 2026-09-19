# WP-HK-07 — Headless host + transport projections

Status: SUPERSEDED — SPLIT BEFORE IMPLEMENTATION  
Class: FOUNDATIONAL UMBRELLA  
Depends on: `WP-HK-06C`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Resolution

This workpack was intentionally split before implementation because it coupled two distinct independently reviewable boundaries: making Arkus a real deterministic external process, and projecting that accepted process contract through MCP without semantic drift.

Do **not** implement or review this umbrella as a single WP.

Execution order is now:

1. `WP-HK-07A — Headless host + deterministic reference transport`
2. `WP-HK-07B — MCP projection + cross-transport conformance`
3. `WP-HK-08` only after `WP-HK-07B` PASS + merge + DocSync.

## Split rationale

The original HK07 objective remains unchanged in aggregate: external processes must be able to discover and exercise the complete accepted Arkus contract through a deterministic reference transport and a standards-compatible MCP projection, with both transports proven to share one canonical semantic source of truth.

- HK07A owns process hosting, deterministic JSONL/reference framing, failure behaviour, cancellation and the no-second-registry completeness boundary.
- HK07B consumes the accepted HK07A host/reference transport and adds MCP plus cross-transport semantic equivalence and SDK replaceability.

This split lets the project obtain and independently validate a usable external harness process before introducing MCP SDK/projection complexity. Accepted predecessor guarantees from HK01–HK06C compose forward and should not be redundantly re-proved without concrete contradictory evidence.

## Original aggregate DoD

When HK07A and HK07B are both COMPLETE, external processes can discover and exercise the complete accepted H0 contract through both the deterministic reference transport and MCP, with deterministic failure behaviour, no parallel adapter registry and mechanically demonstrated semantic equivalence.
