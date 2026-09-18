# WP-HK-09 — Safety + capability boundary

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-08`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Constrain what the harness can affect so an AI can author aggressively inside the game model without gaining accidental arbitrary host powers.

## Acceptance

- Canonical harness kernel has no generic shell/process execution capability.
- Network access is absent by default from the kernel/host and cannot be triggered by protocol payloads.
- Filesystem access is limited to explicit harness-owned persistence/evidence locations; path traversal and symlink escapes fail closed.
- Input size, batch size, query page size, recursion/depth and execution-time/resource limits are explicit and tested.
- Capability metadata identifies mutating vs read-only operations and any privileged persistence/export operation.
- Deserialization does not instantiate arbitrary runtime types from user-controlled names.
- Import/snapshot paths validate format/version/size before replacing canonical state.
- Crash/interruption during persistence cannot leave a half-committed canonical world.
- Security/safety policy is enforced below transport-specific adapters so a future MCP/GUI cannot bypass it.

## Required self-attacks

RED→GREEN for: `../` path traversal, symlink escape, oversized/deep payload, arbitrary type hint, attempted shell/process invocation route, attempted network-dependent behaviour, batch/resource exhaustion boundary and interrupted persistence.

## Forbidden scope

Authentication/multi-user cloud security, anti-cheat, Unity sandboxing, OS container orchestration.

## DoD

Adversarial protocol inputs cannot escape the declared game-authoring/persistence boundary; limits are machine-tested and independently reviewed.
