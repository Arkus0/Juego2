# ADR-H1-003 — Unity host authority is explicit and project-scoped

Status: PROPOSED; accepted with the H1 planning PR
Date: 2026-09-20

## Decision

The accepted H0 host policy remains unchanged. H1 introduces a separately versioned Unity host policy that admits only reviewed editor/project effects inside a bootstrapped project and fixed managed roots. The policy is enforced below JSONL/MCP at the projection-to-Unity seam.

No public request supplies an arbitrary filesystem root, executable, network endpoint or runtime type name. Unity `AssetDatabase`, scene and prefab operations are reached only through typed bridge capabilities and allowlisted adapters.

## Why

Silently weakening HK09A would redefine an accepted predecessor and recreate its effective-path defect. Unity genuinely needs project filesystem/editor authority, so that authority must be explicit, bounded and independently reviewable rather than disguised as H0 behavior.

## Consequences

- H0 can continue running without Unity or editor authority;
- H1 capabilities carry truthful `ExternalReversible`/read-only semantics;
- a future transport cannot skip policy by composing providers directly;
- package installation and ProjectSettings changes remain bootstrap operations, not ordinary public authoring calls.
