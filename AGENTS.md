# AGENTS.md

## Prime directive

Juego2 is harness-first. No serious gameplay, Unity scene production, Quaternius integration, vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-HK-GATE` passes.

The harness exists to let an AI agent create, inspect, modify, validate, replay, and test the game world through stable machine-readable contracts without knowing C# implementation details.

Arkus Harness is also being designed as a commercially viable, engine-agnostic AI-native game-authoring platform. Juego2 is its proving ground, not a reason to narrow the platform to one game, one engine, one model vendor, or one transport.

## Project domain and testing terminology

Juego2 / Arkus Harness is a **game-development and software-verification project, not a cybersecurity project**. Repository work is limited to the game-authoring harness, its own source code, fixtures, tests, CI and documentation. Do not infer or introduce work against external systems, networks, accounts, services or third-party infrastructure.

All robustness work in this repository means ordinary software-quality verification of repository-owned code. Use clear software-testing language in new prompts, evidence and documentation:

- `negative conformance test` or `defect-injection test` instead of `self-attack`;
- `negative test fixture` instead of `attack fixture`;
- `undeclared/alternate code path` instead of `bypass` when describing an internal contract path;
- `strict pre-review` / `independent strict review` instead of `adversarial pre-review/reviewer`;
- `reproduction case` instead of `exploit` when the intent is only to reproduce a software defect.

Legacy names may still exist in historical evidence, old commits or frozen candidates. Interpret them according to the software-testing meanings above; do not expand their meaning beyond repository-owned conformance testing. Terminology cleanup must never weaken the underlying proof obligation.

## Operating model

Juego2 uses **Automation V2** for mechanical GitHub Actions validation and state transitions, but has no automation bootstrap, role leases, dependency-routing daemon or automatic AI-session spawning.

The human explicitly starts Worker and fresh independent Reviewer sessions. Every reasoning session reconstructs current GitHub state before acting.

A failed Reviewer stops at FAIL and never becomes the repair Worker. A successful Reviewer, however, should normally close the accepted cycle without another human handoff: after persisting exact-SHA PASS, the same session may transition one-way into finalization/DocSync, merge the exact reviewed SHA (or observe Automation V2 doing so), reconcile documentation only, emit `DOCSYNC_COMPLETE`, resolve the dependency-valid next WP, and stop. This post-PASS continuation may not modify implementation bytes or repair the reviewed candidate.

Automation may run canonical validation, persist handoff markers and merge an exact reviewed SHA after a valid PASS. It never substitutes for Worker pre-review or independent Reviewer judgment, and it does not perform semantic DocSync reasoning by itself.

If the user gives only a generic request such as `Ponte a trabajar en Arkus0/Juego2`, reconstruct current state, identify the next dependency-valid role, and do not silently cross from Worker to independent Reviewer or from Reviewer FAIL to repair Worker in the same context.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md` — exact-SHA execution and evidence binding.
5. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` — ownership, Worker pre-review, exact-SHA freeze, independent review and successful finalization.
6. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
7. `Docs/engineering/PRODUCT_ARCHITECTURE.md` + `DEPENDENCY_IP_POLICY.md` — product ownership, adapter boundaries and external-dependency rules.
8. `Docs/engineering/AUTOMATION_V2.md` — minimal replaceable GitHub Actions orchestration.
9. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` — compact resumption summary only; never outranks current evidence.

## Product rules

- One active Worker per WP candidate and one canonical implementation PR.
- Foundational work is not done because one execution is green; completeness, causal negative-conformance tests and residual-risk evidence are required within the accepted trust boundary.
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
- Accepted predecessor guarantees compose forward. A downstream WP consumes binding guarantees already accepted upstream and must not re-prove them merely as defence-in-depth unless the current WP explicitly owns that guarantee or concrete evidence shows the predecessor claim is false/inapplicable.
- Do not add gameplay semantics merely to make harness tests convenient; use a deliberately tiny micro-world fixture.
- Before freezing a foundational WP that defines or changes authorable-state or public-contract semantics, run one bounded content-shape probe against the currently approved representative Juego2 target. The probe is an exploratory omission detector, not a completeness oracle and not permission to add gameplay scope. Classify each finding as a current-WP blocker, concrete predecessor reopen condition, named future/residual decision, or out-of-boundary observation.

## Mandatory predecessor contract check

Before editing a WP, the Worker must reconstruct the accepted contract it inherits rather than reading only the current WP.

At minimum, for each direct accepted dependency it must read the dependency WP, its completion metadata/exact reviewed SHA, independent PASS evidence, relevant proof matrix/residual-risk evidence when present, and any binding architecture/invariant documents that dependency made authoritative. Follow transitive predecessors only where the direct dependency or current WP relies on their invariants; do not reread the entire project history mechanically.

The Worker must persist a short `PREDECESSOR_CONTRACT_CHECK` in its Worker plan/evidence before implementation begins. It must state:

- accepted predecessor/dependency and reviewed/merge SHA(s);
- inherited guarantees relevant to the current WP;
- guarantees newly owned by the current WP;
- predecessor guarantees intentionally consumed rather than re-proved;
- the concrete condition that would justify reopening an inherited guarantee (for example, evidence that the accepted guarantee does not cover the effective path or that the predecessor claim itself was false).

The independent Reviewer performs the mirror check. Before issuing FAIL for an apparent omission, it must determine whether that omission is already covered by a binding predecessor guarantee. If so, it is not a current-WP blocker unless the Reviewer can show with concrete evidence that the inherited guarantee is inapplicable or false. Requiring duplicate proof of an accepted predecessor claim is overdefense, not additional quality.

This rule does not make predecessor prose unquestionable. Concrete contradictory evidence may reopen the relevant causal boundary under the normal circuit-breaker rules; mere theoretical possibility or a desire for redundant proof may not.

## Worker → Reviewer → finalization flow

- GitHub is the persistent repository, PR and evidence truth; sessions are disposable.
- Draft + ACTIVE: Worker may write; Automation V2 runs candidate observation on relevant updates.
- Before implementation, Worker completes and records the mandatory predecessor contract check.
- Before freeze, Worker performs the required strict pre-review and repairs any in-claim blocker while still Draft + ACTIVE.
- `WORKER_PRE_REVIEW: CLEAN` is readiness evidence, never independent PASS.
- Worker stops all writers, binds exact HEAD as `Frozen candidate SHA`, records `Candidate HEAD SHA`, sets `FROZEN_FOR_REVIEW`/`Branch frozen: YES`, and marks the PR Ready.
- Automation V2 runs frozen exact-SHA verification. Only after it is green and metadata agrees does it persist `REVIEW_READY`.
- The human then starts a fresh independent Reviewer. The Reviewer reconstructs state and independently challenges the frozen candidate against its acceptance claims; it never repairs implementation.
- PASS/FAIL binds the exact Frozen candidate SHA.
- FAIL produces `REPAIR_REQUIRED`; a fresh repair Worker is started on the same WP.
- PASS fixes the independent verdict. Automation V2 may merge the exact SHA automatically after green preflight.
- The successful Reviewer session then continues in finalization/DocSync mode: confirm merge, reconcile affected docs/evidence/handoff from current `main`, emit `DOCSYNC_COMPLETE`, resolve `Next WP`, and stop.
- No next WP starts before `DOCSYNC_COMPLETE`.
- Automation never proves role independence. That remains a session/process obligation under `WORKER_REVIEW_PROTOCOL.md`.

## Skills / profiles

Project skills under `.agents/skills/` and OpenCode role profiles under `.opencode/agents/` are optional manual helpers, not orchestration. Use the profile/skill appropriate to the current explicitly invoked role; never reuse Worker context as independent Reviewer.
