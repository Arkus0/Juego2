# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

Arkus Harness is also being designed as a commercially viable, engine-agnostic AI-native game-authoring platform. Juego2 is its proving ground, not a reason to narrow the platform to one game, one engine, one model vendor, or one transport.

## Minimal agent entry

For a configured backend, `Ponte a trabajar en Arkus0/Juego2` means: read `AUTOMATION_BOOTSTRAP.md`, reconstruct GitHub state, resolve the first safe eligible contractual action, execute one role transition, persist it, and stop at the next role boundary.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md` — provider-neutral exact-SHA execution, evidence binding and zero-budget runner policy.
5. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` — ownership, Worker pre-review, exact-SHA freeze and independent review.
6. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
7. `Docs/engineering/PRODUCT_ARCHITECTURE.md` + `DEPENDENCY_IP_POLICY.md` — product ownership, adapter boundaries and external-dependency rules.
8. `Docs/automation/DEPENDENCY_ROUTING.md` + `AGENTIC_PROTOCOL_DURABILITY.md` — backend-neutral orchestration.
9. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` — compact resumption summary only; never outranks current evidence.

## Product rules

- One WP at a time per branch/Worker.
- Foundational work is not done because one execution is green; completeness, self-attacks and residual-risk evidence are required.
- Exact-SHA validation is provider-neutral. GitHub-hosted Actions are optional convenience, not a contractual dependency; follow `EXECUTION_RECEIPT_PROTOCOL.md`.
- Do not copy architecture or code from `Arkus0/Juego` by default. It is reference material only. Migration requires explicit justification and review.
- Process lessons from `Juego` may be reused when they are engine/game independent.
- DFU is not part of the critical path. It may only return later as an optional adapter after `WP-HK-GATE`, through an explicit ADR proving net value.
- Arkus canonical contracts and semantics are transport-neutral. MCP, JSONL, HTTP, SDKs and future Creator GUI are projections/adapters, never the source of truth.
- The harness core must be engine-agnostic. Unity is the first engine bridge, not the architectural ceiling; engine-specific types may not leak into canonical kernel contracts.
- External tools/libraries may be adopted when they solve a generic problem better, but no adopted component may reduce Arkus scope, become an irreplaceable semantic authority, or create avoidable commercial/IP restrictions.
- The harness must fail closed: missing schemas, validators, proof tools or required evidence are failures.
- Prefer standards and evaluated behaviour over growing syntax denylists.
- Determinism, replayability, structured errors, discoverability, transactions and provenance are product requirements, not test conveniences.
- Completeness claims may not rely solely on an inventory/registry/configuration controlled by the thing being proved; the universe under proof must be independently discoverable or checked against effective behaviour.
- Do not add gameplay semantics merely to make harness tests convenient; use a deliberately tiny micro-world fixture.

## Agentic flow rules

- GitHub is the persistent queue and operational truth; provider sessions are disposable.
- Draft + ACTIVE: Worker may write.
- Before freeze, the Worker must perform the protocol's adversarial pre-review against the complete candidate and repair any finding while still Draft + ACTIVE.
- `WORKER_PRE_REVIEW: CLEAN` is required readiness evidence but is never an independent PASS; the fresh Reviewer must reconstruct and falsify from scratch without being bounded by Worker conclusions.
- Ready + FROZEN_FOR_REVIEW: no Worker writes.
- A Reviewer must be independent of the Worker context and tries to falsify the candidate; it never repairs implementation.
- PASS/FAIL binds an exact Frozen candidate SHA.
- FAIL returns to the same WP/PR repair loop; it does not authorize skipping ahead, and the repaired candidate must pass Worker pre-review again before refreeze.
- Two independent FAILs exposing the same foundational class trigger architecture re-audit.
- A FAIL showing that the proof universe can self-shrink or omit material objects by construction is immediately architectural: re-audit the proof boundary before another local patch.
- Merge is followed by DocSync before the next WP is selected.
- Backend outage/quota/host-down is recoverable backpressure, not contractual FAIL. Switch to another compliant execution substrate rather than weakening validation.
- Role leases prevent duplicate actors; never steal a live lease.
- Telegram is low-noise notification only. It never owns workflow state and is never a completion gate.

## Skills / profiles

Project skills live under `.agents/skills/`: `plan-milestone`, `implement-workpack`, `validate-workpack`, `validate-milestone`, `update-handoff`.

OpenCode role profiles live under `.opencode/agents/`: `architect`, `worker`, `reviewer`.

Use the profile/skill appropriate to the role; never use the Worker context as independent Reviewer.
