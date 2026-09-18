# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

## Sources of truth

1. `Docs/ROADMAP.md` — milestone order and gates.
2. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
3. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
4. Code + executable tests + recorded evidence — actual state.

## Rules

- One WP at a time per branch/Worker.
- Foundational work is not done because CI is green; completeness, self-attacks and residual-risk evidence are required.
- Do not copy architecture or code from `Arkus0/Juego` by default. It is reference material only. Migration requires explicit justification and review.
- DFU is not part of the critical path. It may only return later as an optional adapter after `WP-HK-GATE`, through an explicit ADR proving net value.
- The harness core must be engine-agnostic and transport-agnostic. Unity is a downstream consumer.
- The harness must fail closed: missing schemas, validators, proof tools or required evidence are failures.
- Prefer standards and evaluated behaviour over growing syntax denylists.
- Determinism, replayability, structured errors, discoverability and provenance are product requirements, not test conveniences.
- Do not add gameplay semantics merely to make harness tests convenient; use a deliberately tiny micro-world fixture.
