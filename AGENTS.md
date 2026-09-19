# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

Arkus Harness is also being designed as a commercially viable, engine-agnostic AI-native game-authoring platform. Juego2 is its proving ground, not a reason to narrow the platform to one game, one engine, one model vendor, or one transport.

## Operating model

Juego2 uses **Automation V2** for mechanical GitHub Actions validation and state transitions, but has no automation bootstrap, role leases, dependency-routing daemon or automatic AI-session spawning.

The human still explicitly starts each reasoning role/session: Worker, independent Reviewer and DocSync. Every role reconstructs current GitHub state before acting and stops at the next role boundary.

Automation may run canonical validation, persist handoff markers and merge an exact reviewed SHA after a valid PASS. It never substitutes for Worker pre-review or independent Reviewer judgment.

If the user gives only a generic request such as `Ponte a trabajar en Arkus0/Juego2`, reconstruct current state, identify the next dependency-valid role, and do not silently cross from Worker to independent Reviewer or from Reviewer to repair Worker in the same context.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md` — exact-SHA execution and evidence binding.
5. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` — ownership, Worker pre-review, exact-SHA freeze and independent review.
6. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
7. `Docs/engineering/PRODUCT_ARCHITECTURE.md` + `DEPENDENCY_IP_POLICY.md` — product ownership, adapter boundaries and external-dependency rules.
8. `Docs/engineering/AUTOMATION_V2.md` — minimal replaceable GitHub Actions orchestration.
9. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` — compact resumption summary only; never outranks current evidence.

## Product rules

- One active Worker per WP candidate and one canonical implementation PR.
- Foundational work is not done because one execution is green; completeness, self-attacks and residual-risk evidence are required within the accepted trust boundary.
- Validation is exact-SHA and executor-neutral. Automation V2 normally supplies hosted execution; Worker, independent Reviewer or capable local environments remain valid fallbacks.
- GitHub Actions workflow YAML is orchestration only. Canonical scripts/contracts own validation semantics.
- Standard GitHub-hosted runners are allowed. Do not introduce larger/paid runners or paid CI as a normal dependency without explicit human approval.
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

## Worker → Reviewer flow

- GitHub is the persistent repository, PR and evidence truth; sessions are disposable.
- Draft + ACTIVE: Worker may write; Automation V2 runs candidate observation on relevant updates.
- Before freeze, Worker performs the required adversarial pre-review and repairs any in-claim blocker while still Draft + ACTIVE.
- `WORKER_PRE_REVIEW: CLEAN` is readiness evidence, never independent PASS.
- Worker stops all writers, binds exact HEAD as `Frozen candidate SHA`, records `Candidate HEAD SHA`, sets `FROZEN_FOR_REVIEW`/`Branch frozen: YES`, and marks the PR Ready.
- Automation V2 runs frozen exact-SHA verification. Only after it is green and metadata agrees does it persist `REVIEW_READY`.
- The human then starts a fresh independent Reviewer. The Reviewer reconstructs state and tries to falsify the frozen candidate; it never repairs implementation.
- PASS/FAIL binds the exact Frozen candidate SHA.
- FAIL produces `REPAIR_REQUIRED`; a fresh repair Worker is started manually on the same WP.
- PASS plus exact-SHA green preflight may be merged automatically by Automation V2.
- Merge produces `DOCSYNC_REQUIRED`; DocSync is started manually and must complete before the next WP is selected.
- Automation never proves role independence. That remains a session/process obligation under `WORKER_REVIEW_PROTOCOL.md`.

## Skills / profiles

Project skills under `.agents/skills/` and OpenCode role profiles under `.opencode/agents/` are optional manual helpers, not orchestration. Use the profile/skill appropriate to the current explicitly invoked role; never reuse Worker context as independent Reviewer.
