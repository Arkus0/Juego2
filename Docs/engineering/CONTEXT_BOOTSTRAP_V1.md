# Context Bootstrap v1

Status: **CTX-01 ACCEPTED / CTX-02 CAPSULE AMENDMENT CANDIDATE**

## Purpose

Give each Juego2 role the smallest safe starting context without turning compact context into a semantic authority or a substitute for predecessor/review proof.

This protocol changes initial selection/navigation only. It does not change product/runtime contracts, workpack meaning, proof threshold, Worker ownership, Reviewer independence, exact-SHA obligations, DocSync semantics, or H1 local-executor discretion.

Machine-readable boot profiles: `Docs/engineering/context-bootstrap-profiles.json`  
Derived navigation index: `Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json`  
Mechanical freshness/profile check: `scripts/context-bootstrap-check.py`  
Post-CTX-02 accepted-predecessor navigation: `Docs/engineering/CONTEXT_CAPSULE_V1.md`

## 1. Authority is question-specific

### A. Mutable repository/process state

For current `main`, open PR/branch ownership, PR HEAD, Draft/Ready state, checks, reviews, merge state and automation markers, **live authenticated GitHub state is authoritative**.

A local checkout, session handoff or derived index may navigate to that state but may not override it. The index never caches transient PR/branch/review/check/ownership facts.

### B. Semantic / proof / acceptance state

For product/process semantics, scope/DoD, inherited guarantees, accepted proof and architecture, the authoritative sources are the exact repository contracts, code/tests and accepted evidence named by `AGENTS.md` and the applicable workpack/protocol.

Accepted-contract capsules, once CTX-02 is accepted, remain navigation only. They may reduce the initial predecessor read but never replace an authoritative source when the exact material detail, proof, contradiction or reopen question requires it.

### C. Derived accepted-state navigation

`Docs/SESSION_HANDOFF/ACCEPTED_STATE_INDEX.json` and `00_SESSION_HANDOFF_PROMPT.md` are navigation projections only. Every hint carries source pointers to authoritative contracts.

The index uses the non-self-referential freshness contract `docsync-first-parent-v1`:

```text
projection_phase == DOCSYNC_PERSISTED
AND
index.generated_from_main_sha == first_parent(current live main)
```

`generated_from_main_sha` is the accepted `main` commit from which DocSync reconstructed the projection **before** the commit/merge that persists that projection. It is deliberately not the SHA of the commit containing the index.

A `CANDIDATE` projection is never live-fresh. A persisted projection becomes stale as soon as `main` advances again, because the current first parent then changes. Missing, malformed, stale or contradictory projections have `mutable_hints_usable=false`; roles reconstruct from live GitHub plus authoritative contracts and never infer from silence.

This avoids the impossible self-reference `file contains SHA of commit that contains file` while preserving deterministic stale detection.

## 2. Minimum profile means start here, not stop here

The role profile selects an initial pack, not a maximum-read rule. Deepen whenever exact dependency/order/gate meaning is unresolved, the current claim binds proof/architecture outside the pack, live GitHub contradicts compact state, a repair touches inherited guarantees, semantic/proof conclusions would rely only on a summary/index/capsule, a capsule escalation trigger fires, or material ambiguity remains.

Escalation is monotonic: read authoritative context until the material question is closed or stop blocked/not-ready.

## 2A. Bounded inspection reduces output, not verification

When locating repository facts, default to the smallest **sufficient** inspection surface rather than emitting large files, logs or API payloads before they are needed. Prefer targeted search (`rg -n` when available, otherwise an equivalent such as `grep -n`), ranged reads around relevant matches, path-scoped `git diff`, and PR/SHA/file/job-scoped GitHub queries. After an edit, prefer the complete relevant diff plus targeted rereads over mechanically dumping the whole file when the unchanged remainder is not material.

This is tool-output/context hygiene only. It MUST NOT:

- narrow the effective mandatory read set selected by the role profile, exact WP, route, predecessor contract, proof standard or authoritative-escalation rules;
- replace a required complete WP/DoD read, complete baseline→candidate diff, strict pre-review, independent causal review, canonical test/gate execution or exact-SHA evidence check;
- treat a search hit, summary, capsule, truncated result or absence from a bounded query as proof that a material fact/path does not exist;
- stop at a bounded result when it is ambiguous, contradictory, truncated, incomplete for a completeness claim, or otherwise insufficient to close the material question.

If a bounded inspection is insufficient, widen monotonically until the authoritative question is closed or stop blocked/not-ready. The operating rule is: **minimize incidental output; never minimize required verification**.

## 3. ROADMAP and foundational proof are conditional for exact-WP sessions

For exact Worker, repair Worker and exact-WP Reviewer sessions, full `Docs/ROADMAP.md` is not mechanically required at initial bootstrap when the exact WP/direct accepted dependencies close eligibility, ownership and ordering. Read it whenever cross-track/order/gate meaning remains material.

`Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md` is loaded when the current WP/gate is foundational or another binding contract invokes it.

Planner/milestone roles normally start with ROADMAP because milestone ordering/gates are their claim.

## 4. Mandatory predecessor/reviewer behavior remains intact

Workers still perform the complete `PREDECESSOR_CONTRACT_CHECK` required by `AGENTS.md` / `WORKER_REVIEW_PROTOCOL.md`.

Before CTX-02 adoption, predecessor reconstruction uses the pre-capsule authoritative read path. After CTX-02 independently passes, merges and completes DocSync, `CONTEXT_CAPSULE_V1.md` may supply a mechanically validated accepted-predecessor starting representation plus exact provenance. This changes repeated initial reads, not the inherited guarantee or its authority. Missing/stale/lossy/mismatched capsules, non-compressible source requirements, material details outside the capsule, directly binding proof/architecture and concrete reopen questions all force authoritative deepening.

Independent Reviewers may use the state index and, after CTX-02 adoption, validated capsules to navigate. They must still independently reconstruct the inherited/current ownership split and open authoritative predecessor evidence whenever the verdict materially depends on the inherited guarantee or a contradiction/reopen question. Worker prose/index/capsule hints are never review proof.

## 5. H1 local executor remains narrower

The `h1_local_executor` profile preserves `H1_REMOTE_LOCAL_EXECUTION.md`: external anchor -> anchored manifest -> exact WP -> manifest-named executable surfaces. It returns `REMOTE_DECISION_REQUIRED` rather than widening context to make semantic/design decisions.

Accepted-contract capsules do not broaden local executor discretion.

## 6. Candidate/source validation

A Worker candidate may carry:

```text
projection_phase = CANDIDATE
generated_from_main_sha = exact live main used as its reconstruction source
```

This allows deterministic source-anchor validation during Worker pre-review without claiming the candidate is already a fresh live projection:

```bash
python3 scripts/context-bootstrap-check.py \
  --expected-source-sha <LIVE_MAIN_USED_FOR_RECONSTRUCTION> \
  --require-source-match
```

`source_anchor_valid=true` proves only that the candidate was reconstructed from the declared source. It does not authorize mutable hints on live `main`.

Capsules have a different freshness model because they bind immutable accepted contract identities. Their validation is defined by `CONTEXT_CAPSULE_V1.md`; an unrelated later `main` commit does not by itself stale an accepted capsule.

## 7. DocSync persistence rule

After independent PASS and implementation merge:

1. reconstruct the exact current live `main` (`SOURCE_MAIN_SHA`) and accepted transition state;
2. regenerate `ACCEPTED_STATE_INDEX.json` from authoritative sources on `SOURCE_MAIN_SHA`;
3. set `projection_phase` to `DOCSYNC_PERSISTED`;
4. set `generated_from_main_sha` to `SOURCE_MAIN_SHA`;
5. persist the DocSync bytes in a direct child/merge whose **first parent is `SOURCE_MAIN_SHA`**;
6. if `main` moved before persistence, stop, reconstruct again and use the new source; do not force a stale projection;
7. after persistence, verify live freshness with both the current main SHA and its first parent;
8. keep transient PR/branch/review/check state out of the reusable index and accepted-contract capsules;
9. after CTX-02 adoption, maintain accepted PA result capsule coverage as defined by `CONTEXT_CAPSULE_V1.md` without rewriting unchanged capsules solely because `main` advanced;
10. emit `DOCSYNC_COMPLETE` only after reconciliation and required live/capsule checks pass.

Mechanical live check:

```bash
python3 scripts/context-bootstrap-check.py \
  --current-main-sha <DOCSYNC_PERSISTING_MAIN_SHA> \
  --current-main-parent-sha <SOURCE_MAIN_SHA> \
  --require-fresh
```

The index is fresh only for that persistence commit. Any later main advance intentionally makes it stale until a later DocSync refresh.

## 8. Fail-closed decision procedure

```text
1. Identify exact role + exact WP/gate/task.
2. Read its machine profile and minimum starting pack.
3. Query live GitHub for mutable state required by the role.
4. If using main-derived accepted-state hints, run the live first-parent freshness check.
5. After CTX-02 adoption, if using an accepted-contract capsule, validate its accepted identity/source/coverage and apply its escalation/non-compressible rules.
6. Read the exact authoritative contract(s) still required by the current material question.
7. Decide: CONTEXT_CLOSED | ESCALATE:<reason> | BLOCKED:<reason>.
8. If ESCALATE, open the named authoritative source(s) and repeat.
9. Never treat missing/stale compact context as negative evidence or permission.
```

## 9. Repair lineage and adoption boundary

Independent review of the original CTX-01 candidate `d972db98eca5527e7c30596ea069866dd878069b` exposed the self-referential old equality rule. Review FAIL `#5271197524` and revert PR `#109` returned `main` to accepted state `a85954539e0ef397009e87af322eb735d58ccc0d`.

The repaired CTX-01 candidate `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39` independently passed review `#5273801466`, merged as `fbd3e5526e760efc89f54e7c12a274af10d4765f`, and completed DocSync on live main `f7b4f1e8247dfc927203dca6754b71aaa53938f3`. Context Bootstrap v1 is therefore binding.

The old rule `generated_from_main_sha == current live main SHA` remains superseded by the first-parent contract above. No product or predecessor semantics changed in CTX-01.

The capsule amendments in this candidate become binding only after `WP-CTX-02` receives independent PASS, merges and completes successful DocSync. Until then, the accepted CTX-01 predecessor-read behavior on `main` remains authoritative and this candidate's capsules are evidence only. CTX-03 may later restructure repeated evidence/state/history representation but may not promote derived navigation to semantic authority.
