# WP-HK-09A — Capability containment boundary

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-08B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Constrain which host capabilities Arkus can exercise at all so an AI can author aggressively inside the game model without acquiring accidental arbitrary host powers. This is repository-local software capability containment, not external-system testing.

## Acceptance

- Canonical harness kernel exposes no generic shell/process execution capability.
- Network access is absent by default from the kernel/host and cannot be triggered by protocol payloads.
- Filesystem access is limited to explicit harness-owned persistence/evidence locations; authority is expressed by reviewed capability boundaries rather than arbitrary path strings.
- Parent-directory and symbolic-link path escapes fail closed before any out-of-boundary effect.
- Capability metadata truthfully identifies mutating vs read-only operations and any privileged persistence/export operation needed by accepted H0 semantics.
- Deserialization does not instantiate arbitrary runtime types from user-controlled names or equivalent unchecked type selectors.
- Capability policy is enforced below transport-specific adapters so reference transport, MCP and future GUI/SDK projections cannot create or bypass host authority.
- The accepted reference transport and MCP projection expose no adapter-only host-power capability absent from canonical composition/policy.
- Ordinary canonical read/authoring/replay/snapshot flows continue to work without shell, ambient network or unrestricted filesystem authority.

## Required negative-conformance tests

RED→GREEN for:

- parent-directory path escape;
- symbolic-link boundary escape;
- request for an unsupported generic process/shell capability;
- protocol-controlled input attempting to create a network dependency;
- unsupported runtime-type hint/selector;
- adapter-only privileged capability that is absent from canonical policy; and
- a transport path attempting to skip the below-transport capability boundary.

## Explicit boundary

HK09A owns **which powers exist**, not numeric size/time quotas or crash-safe persistence mechanics. Input/batch/page/depth/time/resource limits, snapshot/import size enforcement and interrupted-persistence integrity belong to `WP-HK-09B`.

No claim is made about hostile external systems, OS-level sandbox escape, cloud tenancy/authentication or anti-cheat.

## Forbidden scope

Authentication/multi-user cloud security, anti-cheat, Unity sandboxing, OS container orchestration, penetration testing, testing external systems, numeric resource-budget tuning, or persistence crash-consistency mechanics owned by HK09B.

## DoD

All public/effective H0 paths remain inside the declared Arkus game-authoring capability boundary, no transport can manufacture additional host authority, and the canonical H0 surface remains functional without generic shell/process, ambient network or unrestricted filesystem powers; independent PASS.
