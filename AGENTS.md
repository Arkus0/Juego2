# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

## Minimal agent entry

For a configured backend, `Ponte a trabajar en Arkus0/Juego2` means: read `AUTOMATION_BOOTSTRAP.md`, reconstruct GitHub state, resolve the first safe eligible contractual action, execute one role transition, persist it, and stop at the next role boundary.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` — ownership, exact-SHA freeze and independent review.
5. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
6. `Docs/automation/DEPENDENCY_ROUTING.md` + `AGENTIC_PROTOCOL_DURABILITY.md` — backend-neutral orchestration.
7. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` — compact resumption summary only; never outranks current evidence.

## Product rules

- One WP at a time per branch/Worker.
- Foundational work is not done because CI is green; completeness, self-attacks and residual-risk evidence are required.
- Do not copy architecture or code from `Arkus0/Juego` by default. It is reference material only. Migration requires explicit justification and review.
- Process lessons from `Juego` may be reused when they are engine/game independent.
- DFU is not part of the critical path. It may only return later as an optional adapter after `WP-HK-GATE`, through an explicit ADR proving net value.
- The harness core must be engine-agnostic and transport-agnostic. Unity is a downstream consumer.
- The harness must fail closed: missing schemas, validators, proof tools or required evidence are failures.
- Prefer standards and evaluated behaviour over growing syntax denylists.
- Determinism, replayability, structured errors, discoverability and provenance are product requirements, not test conveniences.
- Do not add gameplay semantics merely to make harness tests convenient; use a deliberately tiny micro-world fixture.

## Agentic flow rules

- GitHub is the persistent queue and operational truth; provider sessions are disposable.
- Draft + ACTIVE: Worker may write. Ready + FROZEN_FOR_REVIEW: no Worker writes.
- A Reviewer must be independent of the Worker context and tries to falsify the candidate; it never repairs implementation.
- PASS/FAIL binds an exact Frozen candidate SHA.
- FAIL returns to the same WP/PR repair loop; it does not authorize skipping ahead.
- Two independent FAILs exposing the same foundational class trigger architecture re-audit.
- Merge is followed by DocSync before the next WP is selected.
- Backend outage/quota/host-down is recoverable backpressure, not contractual FAIL.
- Role leases prevent duplicate actors; never steal a live lease.
- Telegram is low-noise notification only. It never owns workflow state.

## Skills / profiles

Project skills live under `.agents/skills/`: `plan-milestone`, `implement-workpack`, `validate-workpack`, `validate-milestone`, `update-handoff`.

OpenCode role profiles live under `.opencode/agents/`: `architect`, `worker`, `reviewer`.

Use the profile/skill appropriate to the role; never use the Worker context as independent Reviewer.
