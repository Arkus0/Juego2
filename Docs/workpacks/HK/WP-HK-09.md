# WP-HK-09 — Safety + capability boundary

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-08`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Constrain what the harness can affect so an AI can author aggressively inside the game model without gaining accidental arbitrary host powers. This is repository-local software capability containment, not external-system testing.

## Acceptance

- Canonical harness kernel has no generic shell/process execution capability.
- Network access is absent by default from the kernel/host and cannot be triggered by protocol payloads.
- Filesystem access is limited to explicit harness-owned persistence/evidence locations; parent-directory and symbolic-link path escapes fail closed.
- Input size, batch size, query page size, recursion/depth and execution-time/resource limits are explicit and tested.
- Capability metadata identifies mutating vs read-only operations and any privileged persistence/export operation.
- Deserialization does not instantiate arbitrary runtime types from user-controlled names.
- Import/snapshot paths validate format/version/size before replacing canonical state.
- Crash/interruption during persistence cannot leave a half-committed canonical world.
- Capability policy is enforced below transport-specific adapters so a future MCP/GUI cannot skip it.

## Required negative-conformance tests

RED→GREEN for: parent-directory path escape, symbolic-link boundary escape, oversized/deep payload, unsupported runtime-type hint, request for an unsupported process-execution capability, unexpected network dependency, batch/resource-limit violation and interrupted persistence.

## Forbidden scope

Authentication/multi-user cloud security, anti-cheat, Unity sandboxing, OS container orchestration, penetration testing or testing of external systems.

## DoD

Malformed or out-of-contract protocol inputs remain inside the declared game-authoring/persistence boundary; limits are machine-tested and independently reviewed.
