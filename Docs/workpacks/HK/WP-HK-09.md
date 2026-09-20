# WP-HK-09 — Safety + capability boundary

Status: SUPERSEDED — split before implementation into `WP-HK-09A` and `WP-HK-09B`  
Class: FOUNDATIONAL UMBRELLA RECORD  
Depends on: `WP-HK-08B`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Historical objective

Constrain what the harness can affect and consume so an AI can author aggressively inside the game model without gaining accidental arbitrary host powers or leaving canonical persistence in an invalid partial state.

This umbrella workpack was intentionally split before implementation because it combined two independently reviewable claims:

1. **authority/capability containment** — what host powers are available at all; and
2. **resource/persistence limits** — how much accepted work may consume and how interruption/import boundaries fail closed.

## Executable replacement chain

`WP-HK-09A → WP-HK-09B`

- `WP-HK-09A` owns repository-local capability containment: no generic shell/process authority, no network authority by default, filesystem authority restricted to explicit harness-owned locations, safe type handling, truthful privileged-capability metadata and enforcement below transport adapters.
- `WP-HK-09B` owns explicit input/batch/page/depth/time/resource limits, import/snapshot size validation and persistence/interruption integrity. Its limits consume the measured/representative interaction shapes accepted by HK08A/HK08B rather than choosing arbitrary caps that break them.

The aggregate intent of this record remains binding only through those replacement workpacks. **Do not implement or review `WP-HK-09` directly.**

## Preserved boundary

This split remains repository-local software containment for the Arkus game-authoring harness. It does not add authentication/multi-user cloud security, anti-cheat, Unity sandboxing, OS container orchestration, penetration testing or testing of external systems.
