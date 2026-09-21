# ADR-H1-003 — Unity host authority is explicit and project-scoped

Status: ACCEPTED — H1 planning PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`
Date: 2026-09-21

## Decision

The accepted H0 host policy remains unchanged. H1 introduces a separately versioned Unity host policy that admits only reviewed editor/project effects inside a bootstrapped project and fixed managed roots. The policy is enforced below JSONL/MCP at the projection-to-Unity seam.

No public request supplies an arbitrary filesystem root, executable, network endpoint or runtime type name. Unity `AssetDatabase`, scene and prefab operations are reached only through typed bridge capabilities and allowlisted adapters.

This ADR decides **which authority is admitted**. `ADR-H1-004` separately freezes **how** an admitted composed call crosses from the external Arkus host into the Unity Editor process. Its fixed internal launcher is bootstrap authority, never a public generic process capability.

## Why

Silently weakening HK09A would redefine an accepted predecessor and recreate its effective-path defect. Unity genuinely needs project filesystem/editor authority, so that authority must be explicit, bounded and independently reviewable rather than disguised as H0 behavior.

## Consequences

- H0 can continue running without Unity or editor authority;
- H1 capabilities carry truthful `ExternalReversible`/read-only semantics;
- a future transport cannot skip policy by composing providers directly;
- the H1-03A execution seam must consume this policy and cannot mint launcher authority inside an adapter;
- package installation and ProjectSettings changes remain bootstrap operations, not ordinary public authoring calls.
