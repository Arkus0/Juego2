# ADR-H1-001 — Canonical identity and Unity catalogue separation

Status: ACCEPTED — H1 planning PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`
Date: 2026-09-20

## Decision

Canonical resource identity remains `WorldObjectId`/`WorldExtensionIdentity`. Unity assets, prefabs, scenes, clips and component schemas use stable Arkus logical catalogue IDs mapped by the bridge to Unity-native locators. The catalogue is bridge/project-owned and publicly discoverable through canonical scoped capabilities; it is not serialized into `WorldState` merely to make it visible.

Unity `GUID + local file ID`, `GlobalObjectId` and asset paths are locators/evidence only. They never become the canonical game identity or a direct canonical hash input.

## Why

Putting the Unity asset database into `WorldState` would make engine inventory churn rewrite canonical authored truth and contaminate the kernel with Unity lifecycle. Using paths or native object IDs as game identity would make moves/reimports/reconstruction redefine the game. A logical catalogue preserves stable authored choices while keeping the external inventory observable and replaceable.

## Consequences

- catalogue drift can invalidate a materialization without altering the canonical hash;
- canonical binding intent refers to logical catalogue IDs;
- moves preserving Unity identity preserve mapping; copies require distinct logical IDs;
- generic cross-engine asset-reference semantics remain unclaimed;
- completeness must be checked against effective Unity inventory, not the catalogue manifest alone.
