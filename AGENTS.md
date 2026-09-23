# AGENTS.md

## Prime directive

Juego2 is harness-first. `WP-HK-GATE` has passed. No gameplay, keeper realization (`CITY-07+`), vertical-slice content, DFU integration, or Creator GUI work may begin before `WP-H1-GATE` passes. The bounded `CITY-04` greybox may start after its own CITY chain and `WP-H1-08`; that exception validates accepted CITY geometry and authorizes neither H2 nor keeper content. H1 itself may create only the bounded Unity project, fixtures, generated projections and representative real-asset slice explicitly owned by its bridge workpacks; those are proof inputs, not CITY/H2 production.

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

After the `PRODUCT_SHA` anti-loop clarification is merged, the exact `Frozen candidate SHA` is also the cycle's material `PRODUCT_SHA`. GitHub-side PR body/comments/checks/status reconciliation that creates no Git commit and does not change effective WP/process/proof class is `NON_MATERIAL_CLOSURE`: rerun cheap live handoff/context gates and reuse an already-GREEN exact-SHA product validation only when its recorded context still matches. Any Git commit remains material, creates a new candidate SHA and invalidates the old validation/pre-review/review as before. See `Docs/engineering/PRODUCT_SHA_CLOSURE.md`.

After `WP-CTX-01` independently passes, merges and completes DocSync, role sessions use `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` plus `Docs/engineering/context-bootstrap-profiles.json` to select the minimum **starting** context and explicit escalation path. This is routing only: live GitHub remains authoritative for mutable PR/branch/review/check state, exact workpack/evidence/architecture sources remain authoritative for semantics and proof, and mandatory predecessor/reviewer duties below are unchanged. `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` is derived navigation only and never proof authority.

After `WP-CTX-02` independently passes, merges and completes DocSync, `Docs/engineering/CONTEXT_CAPSULE_V1.md` adds a second non-authoritative navigation layer for accepted predecessor contracts. A mechanically valid capsule may satisfy the **initial reconstruction** of an accepted predecessor boundary without loading every historical predecessor narrative, but it never becomes semantic/proof authority. Missing, stale, lossy or contradictory capsules fail closed to the authoritative sources; material non-compressible details, concrete contradiction/reopen questions and Reviewer verdicts that depend on an inherited guarantee still deepen to the exact sources named by the capsule/protocol.

A failed Reviewer stops at FAIL and never becomes the repair Worker. A successful Reviewer, however, should normally close the accepted cycle without another human handoff: after persisting exact-SHA PASS, the same session may transition one-way into finalization/DocSync, merge the exact reviewed SHA (or observe Automation V2 doing so), reconcile documentation only, emit `DOCSYNC_COMPLETE`, resolve the dependency-valid next WP, and stop. This post-PASS continuation may not modify implementation bytes or repair the reviewed candidate.

Automation may run canonical validation, persist handoff markers and merge an exact reviewed SHA after a valid PASS. It never substitutes for Worker pre-review or independent Reviewer judgment, and it does not perform semantic DocSync reasoning by itself.

If the user gives only a generic request such as `Ponte a trabajar en Arkus0/Juego2`, reconstruct current state, identify the next dependency-valid role, and do not silently cross from Worker to independent Reviewer or from Reviewer FAIL to repair Worker in the same context.

## Shorthand role commands

Treat short role requests as explicit routing instructions, not as abbreviated acceptance criteria:

- `Worker <WP-ID>` (for example `Worker H1-02` or `Worker CITY-04`) means resolve that exact workpack under `Docs/workpacks/**` and execute `.agents/skills/implement-workpack/SKILL.md` for it. Reconstruct current `main`, dependencies and canonical ownership first; if the requested WP is blocked, report the blocker and STOP rather than selecting another WP.
- `Corrige el FAIL de <WP-ID>` / `Repair <WP-ID>` means start a **fresh repair Worker** for that same WP and execute `.agents/skills/repair-workpack/SKILL.md`. Locate the canonical PR and latest independent FAIL from live GitHub state, preserve the failed candidate/history, repair the causal blocker, rerun required evidence plus the complete Worker pre-review, freeze a new exact SHA, mark Ready and STOP for a fresh independent Reviewer. Exception: a correction proven to be `NON_MATERIAL_CLOSURE` under `Docs/engineering/PRODUCT_SHA_CLOSURE.md` creates no Git commit and therefore keeps the same `PRODUCT_SHA`; it needs current handoff/context gates, not another product execution or semantic Worker pre-review.
- These Worker/repair shorthand commands never authorize independent review, merge, DocSync, a different WP, or scope beyond the requested WP.
- For `LOCAL_UNITY_REQUIRED` or `HYBRID` work, use the exact local Unity/toolchain/evidence required by the WP. Missing mandatory local engine evidence is never silently treated as PASS-ready.
- A local Git checkout is not sufficient to reconstruct PR/review/check state. Local Worker/repair sessions must use authenticated GitHub CLI (`gh`) or an equivalent live GitHub surface. Before GitHub mutations verify the repository is `Arkus0/Juego2` and that canonical WP/PR ownership is unambiguous.
- Never put GitHub passwords, PATs/tokens, Unity credentials or other secrets in prompts, repository files, committed `.env` files or evidence. Use the OS/authentication store managed by the relevant tool.

Local workstation setup and verification is documented in `Docs/engineering/LOCAL_AGENT_WORKSTATION.md`; `scripts/local-agent-check.ps1` is the read-only readiness check.

## Sources of truth

1. Code, executable tests and recorded evidence — actual state.
2. `Docs/ROADMAP.md` — milestone order and gates.
3. `Docs/workpacks/**` — exact scope and Definition of Done for one unit of work.
4. `Docs/engineering/EXECUTION_RECEIPT_PROTOCOL.md` — exact-SHA execution and evidence binding.
5. `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` + `Docs/engineering/PRODUCT_SHA_CLOSURE.md` — ownership, Worker pre-review, exact-SHA/product identity, non-material closure, independent review and successful finalization.
6. `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` — binding proof rules for foundational WPs.
7. `Docs/engineering/PRODUCT_ARCHITECTURE.md` + `DEPENDENCY_IP_POLICY.md` — product ownership, adapter boundaries and external-dependency rules.
8. `Docs/engineering/H1_ENGINE_BRIDGE_ARCHITECTURE.md` + `Docs/architecture/ADR-H1-*` — accepted H1 identity, projection, synchronization and Unity-authority decisions after the H1 planning PR merges.
9. `Docs/engineering/AUTOMATION_V2.md` — minimal replaceable GitHub Actions orchestration.
10. `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` + `Docs/engineering/context-bootstrap-profiles.json` — role-specific initial-context selection and fail-closed escalation only; never semantic/proof authority above the applicable sources above.
11. After CTX-02 adoption, `Docs/engineering/CONTEXT_CAPSULE_V1.md` + `Docs/engineering/context-capsules/index.json` — accepted-predecessor navigation/compression only; exact accepted sources remain authority and any validation/escalation failure returns to them.
12. `Docs/SESSION_HANDOFF/00_SESSION_HANDOFF_PROMPT.md` + `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` — compact routing/navigation only; never outrank current evidence.

## Product rules

- One active Worker per WP candidate and one canonical implementation PR.
- Foundational work is not done because one execution is green; completeness, causal negative-conformance tests and residual-risk evidence are required within the accepted trust boundary.
- Validation is exact-SHA and executor-neutral. Automation V2 normally supplies hosted execution; Worker, independent Reviewer or capable local environments remain valid fallbacks.
- GitHub Actions workflow YAML is orchestration only. Canonical scripts/contracts own validation semantics.
- A same-SHA PR metadata repair is not a product mutation. Under `PRODUCT_SHA_CLOSURE.md`, automation must revalidate mutable handoff/context and may reuse immutable exact-SHA GREEN execution; it must fall back to full execution if source SHA/context/proof cannot be proven identical.
- Mechanical closure defects should be rejected before an independent Reviewer starts. Same-`PRODUCT_SHA` reconciliation of derivable lifecycle metadata must not manufacture a new semantic review cycle; protocol defects remain material blockers when they compromise identity, provenance, acceptance evidence, role independence or another integrity guarantee.
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
- H1 Unity-required evidence must bind the exact editor/package/platform/content inputs named by the workpack. Missing required local Unity evidence is `READY_FOR_LOCAL_VALIDATION`, never PASS.
- Unity native paths, GUIDs, local file IDs, `GlobalObjectId`, GameObjects and components are bridge locators/projection objects, not canonical game identity or canonical truth.
- Unity-to-canonical synchronization is always an explicit proposal that re-enters accepted H0 plan/dry-run/apply; no bridge may auto-pull editor state into canonical authored state.

## Mandatory predecessor contract check

Before editing a WP, the Worker must reconstruct the accepted contract it inherits rather than reading only the current WP.

Before CTX-02 adoption, or whenever no valid capsule covers a direct accepted dependency, the minimum reconstruction remains: dependency WP, completion metadata/exact reviewed SHA, independent PASS evidence, relevant proof matrix/residual-risk evidence when present, and binding architecture/invariant documents made authoritative by that dependency. Follow transitive predecessors only where the direct dependency or current WP relies on their invariants; do not reread the entire project history mechanically.

After CTX-02 adoption, a direct accepted dependency with a mechanically valid accepted-contract capsule may instead begin from: the validated capsule, independently confirmed accepted identity/live state, and the exact current consumer WP. The Worker does **not** have to load the predecessor's full WP/PASS/proof/residual narratives merely to repeat guarantees already exported by the valid capsule. It must immediately deepen to the exact authoritative source(s) when a capsule validation/escalation condition fires, the current claim needs a detail not safely carried by the capsule, a source is marked non-compressible, an architecture/proof contract is directly binding, or concrete evidence could reopen the predecessor. Missing capsule content is never permission or negative evidence.

The Worker must persist a short `PREDECESSOR_CONTRACT_CHECK` in its Worker plan/evidence before implementation begins. It must state:

- accepted predecessor/dependency and reviewed/merge SHA(s);
- capsule(s) used when applicable and any authoritative escalations performed;
- inherited guarantees relevant to the current WP;
- guarantees newly owned by the current WP;
- predecessor guarantees intentionally consumed rather than re-proved;
- the concrete condition that would justify reopening an inherited guarantee (for example, evidence that the accepted guarantee does not cover the effective path or that the predecessor claim itself was false).

The independent Reviewer performs the mirror check. A validated capsule may reduce navigation/repeated history, but never limits independent judgment. Before issuing FAIL for an apparent omission, the Reviewer must determine whether that omission is already covered by a binding predecessor guarantee; if the verdict materially depends on that inherited guarantee, the capsule is lossy/suspect, or concrete contradiction may reopen it, the Reviewer opens the exact authoritative source/evidence. If the guarantee is already binding and applicable, it is not a current-WP blocker unless the Reviewer can show with concrete evidence that it is inapplicable or false. Requiring duplicate proof of an accepted predecessor claim is overdefense, not additional quality.

This rule does not make predecessor prose or capsules unquestionable. Concrete contradictory evidence may reopen the relevant causal boundary under the normal circuit-breaker rules; mere theoretical possibility, capsule acceptance, or a desire for redundant proof may not decide the question by itself.

## Worker → Reviewer → finalization flow

- GitHub is the persistent repository, PR and evidence truth; sessions are disposable.
- Draft + ACTIVE: Worker may write; Automation V2 runs candidate observation on relevant updates.
- Before implementation, Worker completes and records the mandatory predecessor contract check.
- Before freeze, Worker performs the required strict pre-review and repairs any in-claim blocker while still Draft + ACTIVE.
- `WORKER_PRE_REVIEW: CLEAN` is readiness evidence, never independent PASS.
- Worker stops all writers, binds exact HEAD as `Frozen candidate SHA`/`PRODUCT_SHA`, records `Candidate HEAD SHA`, sets `FROZEN_FOR_REVIEW`/`Branch frozen: YES`, and marks the PR Ready.
- Automation V2 runs frozen exact-SHA verification. On later PR-body-only closure edits with the same `PRODUCT_SHA` and unchanged validation context, Automation V2 reruns mutable handoff/context gates and may reuse the prior immutable GREEN exact-SHA validation instead of rerunning product execution. Only after the resulting gates are green and metadata agrees does it persist `REVIEW_READY`.
- The human then starts a fresh independent Reviewer. The Reviewer reconstructs state and independently challenges the frozen candidate against its acceptance claims; it never repairs implementation.
- PASS/FAIL binds the exact Frozen candidate SHA.
- FAIL produces `REPAIR_REQUIRED`; a fresh repair Worker is started on the same WP.
- PASS fixes the independent verdict. Automation V2 may merge the exact SHA automatically after green preflight.
- The successful Reviewer session then continues in finalization/DocSync mode: confirm merge, reconcile affected docs/evidence/handoff from current `main`, emit `DOCSYNC_COMPLETE`, resolve `Next WP`, and stop.
- No next WP starts before `DOCSYNC_COMPLETE`.
- Automation never proves role independence. That remains a session/process obligation under `WORKER_REVIEW_PROTOCOL.md`.

## Skills / profiles

Project skills under `.agents/skills/` and OpenCode role profiles under `.opencode/agents/` are optional manual helpers, not orchestration. Use the profile/skill appropriate to the current explicitly invoked role; never reuse Worker context as independent Reviewer.