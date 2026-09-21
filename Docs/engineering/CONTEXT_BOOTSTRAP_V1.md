# Context Bootstrap v1

Status: **CTX-01 CANDIDATE / PROCESS_ONLY**

## Purpose

Give each Juego2 role the smallest safe **starting** context without turning compact context into a ceiling, a semantic authority, or a substitute for predecessor/review proof.

This protocol changes initial selection/navigation only. It does not change any product/runtime contract, workpack meaning, proof threshold, Worker ownership, Reviewer independence, exact-SHA obligation, DocSync semantics or H1 local-executor discretion.

Machine-readable boot profiles: `Docs/engineering/context-bootstrap-profiles.json`  
Derived navigation index: `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json`  
Mechanical stale/profile check: `scripts/context-bootstrap-check.py`

## 1. Authority is question-specific

A single total ordering would be misleading, because mutable process state and accepted semantics answer different questions. Use this routing order:

### A. Mutable repository/process state

For current `main`, open PR/branch ownership, PR HEAD, Draft/Ready state, checks, reviews, merge state and automation markers, **live authenticated GitHub state is authoritative**.

A local checkout, session handoff, README or derived index may navigate to that state but may not override it. The accepted-state index deliberately does **not** cache open-PR Draft/Ready/HEAD/check/review state, because those can change without a `main` commit and therefore cannot be made fresh by a `generated_from_main_sha` comparison.

### B. Semantic / proof / acceptance state

For product/process semantics, workpack scope/DoD, inherited guarantees, accepted proof and architecture, the authoritative sources are the exact repository contracts, code/tests and accepted evidence named by `AGENTS.md` and the applicable workpack/protocol.

A derived index or Worker summary may point to those sources but may not replace them.

### C. Derived accepted-state navigation

`Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` and `00_SESSION_HANDOFF_PROMPT.md` are **navigation projections only**.

Main-derived accepted-state hints (`accepted_workpacks_hint`, `next_contract_hint`, accepted dependency/block hints) are usable only when:

```text
index.generated_from_main_sha == current live main SHA
```

The comparison is exact, 40-character SHA equality. If the index is stale, missing, malformed or contradictory, those hints are invalid. The role continues from live GitHub + authoritative contracts; it never infers from silence.

Even when the index is fresh relative to `main`, transient PR/branch/review/check/ownership state must still be queried live. Freshness against `main` cannot prove facts that mutate outside `main`.

The index may never create product semantics, accepted guarantees, dependency edges or proof obligations. Every hint carries source pointers back to authoritative contracts.

## 2. Minimum profile means start here, not stop here

The role profile selects an initial pack. It is **not** a maximum-read rule.

Every role must deepen when any of these triggers fires:

- exact workpack/dependency text does not close eligibility, ownership or gate meaning;
- a direct dependency is cross-track, unresolved, contradictory or absent from trusted current state;
- the current claim binds a proof/architecture document not in the minimum pack;
- live GitHub contradicts a compact state surface;
- a Reviewer/repair finding touches an inherited guarantee that requires original evidence;
- a semantic/proof conclusion would otherwise rely only on a summary/index;
- material ambiguity remains after the initial pack;
- the index is stale/missing/malformed for a decision that would use its main-derived hints.

Escalation is monotonic: read more authoritative context until the material question is closed or stop as blocked/not-ready. Never fill a gap by inference from absence.

## 3. ROADMAP is conditional for exact-WP sessions

For exact Worker, repair Worker and exact-WP Reviewer sessions, full `Docs/ROADMAP.md` is not mechanically required at initial bootstrap when all of the following are true:

1. the exact WP is known;
2. live GitHub establishes current main/ownership/review state;
3. the exact WP plus its direct dependency contracts/evidence close eligibility and ownership;
4. no cross-track gate/order question remains;
5. no compact-state contradiction or staleness affects the decision.

Read/deepen into ROADMAP immediately when those conditions do not hold, including any cross-track dependency whose accepted meaning cannot be closed from the exact WP/direct sources.

Planner/milestone roles normally start with ROADMAP because milestone ordering/gates are their claim. Gate validation starts from the exact gate + constituent accepted state and reads ROADMAP whenever composition/order is material.

## 4. Foundational proof standard is claim-bound

`Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is loaded when the current WP/gate is marked foundational or another binding contract explicitly invokes it.

A non-foundational role does not load it merely because the repository contains foundational history. If a later material question crosses into a foundational claim, the role escalates before deciding that question.

This changes bootstrap selection only; it does not change which claims are foundational.

## 5. Mandatory predecessor/reviewer behavior remains intact

CTX-01 does **not** compress accepted predecessor semantics.

Workers still perform the complete `PREDECESSOR_CONTRACT_CHECK` required by `AGENTS.md` / `WORKER_REVIEW_PROTOCOL.md`: direct dependency WP, accepted completion/review evidence, relevant proof/residual evidence and binding invariants must be opened when material. CTX-02, not CTX-01, owns any future capsule mechanism.

Independent Reviewers may use the index to navigate, but must independently open authoritative predecessor evidence and the complete frozen candidate material required by the review. Worker prose/index hints are never review proof.

## 6. H1 local executor remains narrower

The `h1_local_executor` profile preserves `H1_REMOTE_LOCAL_EXECUTION.md` exactly: external anchor -> anchored manifest -> exact WP -> manifest-named executable surfaces. It does not inherit remote Worker predecessor/proof reasoning and must return `REMOTE_DECISION_REQUIRED` rather than widen context to make semantic/design decisions.

CTX-01 therefore generalizes safe **routing**, not the local executor's reduced authority.

## 7. DocSync refresh rule

Post-PASS DocSync must:

1. reconstruct current live `main` and accepted transition state;
2. update semantic/contract docs only when the accepted candidate requires it;
3. regenerate `ACCEPTED_STATE_INDEX.json` from authoritative accepted sources on that `main`;
4. set `generated_from_main_sha` to the exact live main SHA from which that regenerated projection was derived;
5. keep source pointers for every main-derived hint and keep transient PR/branch/review/check state out of the reusable index;
6. compact `00_SESSION_HANDOFF_PROMPT.md` as a router, not a duplicate state/history store;
7. emit normal `DOCSYNC_COMPLETE` only after reconciliation is actually complete.

If main advances after generation, that is normal: the exact SHA mismatch makes main-derived index hints stale until a later DocSync refresh. Staleness is a routing signal, not permission to rewrite accepted semantics.

## 8. Fail-closed decision procedure

For any supported role:

```text
1. Identify exact role + exact WP/gate/task.
2. Read its machine profile and minimum starting pack.
3. Query live GitHub for mutable state required by the role.
4. If using accepted-state main-derived hints, compare generated_from_main_sha to live main exactly.
5. Read the exact contract(s) in the starting pack.
6. Evaluate escalation triggers and make the decision observable:
     CONTEXT_CLOSED | ESCALATE:<reason> | BLOCKED:<reason>
7. If ESCALATE, open the named authoritative source(s) and repeat step 6.
8. Never treat missing/stale compact context as negative evidence or permission.
```

The final decision must be traceable to authoritative sources, not to the compact projection alone.

## 9. Adoption boundary

Context Bootstrap v1 becomes binding only after `WP-CTX-01` receives fresh independent PASS, merges and completes DocSync. Until that point, the pre-CTX bootstrap rules on `main` remain authoritative.

CTX-02 may later add accepted-contract capsules but may not weaken this authority/escalation model. CTX-03 may later restructure repeated evidence/state/history representation but may not promote derived navigation to semantic authority.
