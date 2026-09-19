# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

Arkus Harness is also being designed as a commercially viable, engine-agnostic AI-native game-authoring platform. Juego2 is its proving ground, not a reason to narrow the platform to one game, one engine, one model vendor, or one transport.

## Manual operating model

Juego2 has no automation bootstrap and no GitHub Actions workflow orchestration.

The human explicitly starts each role/session. A Worker, Reviewer or documentation/finalization session must reconstruct current GitHub state before acting and must stop at the next role boundary. No repository trigger, lease, Telegram notification or bootstrap document is required to advance work.

If the user gives only a generic request such as `Ponte a trabajar en Arkus0/Juego2`, reconstruct the current state, identify the next dependency-valid manual action, and do not silently cross from Worker to independent Reviewer or from Reviewer to repair Worker in the same context.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md` — exact-SHA execution and evidence binding without hosted CI.
5. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` — ownership, Worker pre-review, exact-SHA freeze and independent review.
6. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
7. `Docs/engineering/PRODUCT_ARCHITECTURE.md` + `DEPENDENCY_IP_POLICY.md` — product ownership, adapter boundaries and external-dependency rules.
8. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` — compact resumption summary only; never outranks current evidence.

## Product rules

- One active Worker per WP candidate and one canonical implementation PR.
- Foundational work is not done because one execution is green; completeness, self-attacks and residual-risk evidence are required within the accepted trust boundary.
- Validation is exact-SHA and executor-neutral. Worker, independent Reviewer or a capable local environment may run canonical commands and persist receipts.
- GitHub Actions are not used by Juego2. Do not add or re-enable workflow automation unless the human explicitly changes this policy.
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

## Manual Worker → Reviewer flow

- GitHub is the persistent repository, PR and evidence truth; sessions are disposable.
- Draft + ACTIVE: Worker may write.
- Before freeze, the Worker performs the required adversarial pre-review against the complete candidate and repairs any in-claim blocker while still Draft + ACTIVE.
- `WORKER_PRE_REVIEW: CLEAN` is readiness evidence, never an independent PASS.
- Ready + `FROZEN_FOR_REVIEW`: no Worker writes.
- A fresh independent Reviewer reconstructs state and tries to falsify the frozen candidate; it never repairs implementation.
- PASS/FAIL binds the exact Frozen candidate SHA.
- FAIL returns to the same WP repair loop and requires a new Worker session followed by a fresh pre-review/refreeze.
- Merge is followed by a manual DocSync/reconciliation before the next WP is selected.
- No automatic role transition, role lease, notification or background runner is part of the correctness contract.

## Skills / profiles

Project skills under `.agents/skills/` and OpenCode role profiles under `.opencode/agents/` are optional manual helpers, not orchestration. Use the profile/skill appropriate to the current explicitly invoked role; never reuse Worker context as independent Reviewer.
