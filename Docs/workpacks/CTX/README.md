# CTX — Agent context / process efficiency

Status: **ACCEPTED / PROCESS_ONLY**
Execution: **REMOTE_OK**
Purpose: improve agent reasoning quality first, then reduce context/token/local-execution cost without weakening any accepted product, proof or review obligation.

Accepted plan evidence:

- frozen candidate: `c9ff3605048e05fded4d58a1de27450661d9fe0e`;
- independent PASS: review `#5270686380` on PR `#102`;
- plan merge: `f3c8362b3d76fd4f78107d8142e07e476985f973`;
- post-PASS reconciliation: `Docs/evidence/CTX-PLAN/DOCSYNC.md`.

## Governing priority

```text
1. product / architecture correctness
2. proof and independent-review quality
3. context precision and cognitive load
4. token / local-agent cost
```

A saving that increases guessing, stale-state risk, hidden assumptions, Reviewer dependence on Worker prose, or omitted proof is a regression.

## Sequence

```text
WP-CTX-01  Role-specific bootstrap + derived accepted-state navigation
    ↓
WP-CTX-02  Accepted-contract capsules + predecessor inheritance compression
    ↓
WP-CTX-03  Structured evidence / DocSync / history separation + measured closure
```

All three are PROCESS_ONLY. None may change product/runtime semantics, accepted workpack meaning, or the Worker → fresh independent Reviewer boundary.

`NON-PRODUCT-FOUNDATIONAL` means CTX does not invoke the product `FOUNDATIONAL_PROOF_STANDARD.md` merely by existing. It does **not** mean a lighter handoff: every CTX WP still follows the normal Worker pre-review → exact frozen candidate SHA → fresh independent Reviewer → PASS/FAIL → merge → DocSync lifecycle.

## Why this split

`CTX-01` changes **what a fresh role loads first**. It can deliver immediate savings without changing how accepted predecessor guarantees are summarized.

`CTX-02` changes **how accepted predecessor guarantees are navigated/reused**. That is a separate risk: a compact capsule must never become a lossy replacement for authoritative accepted evidence.

`CTX-03` changes **how repeated evidence/state/history is represented and synchronized**. It depends on the first two routing/authority rules and owns measured closure.

Combining all three would make one Reviewer validate context routing, semantic compression and evidence migration at once. Splitting further would create administrative micro-WPs with no independent causal claim.

## Track invariants

- live GitHub remains authoritative for transient PR/branch/review/check/ownership state;
- exact code/workpacks/architecture/accepted evidence remain authoritative for semantics/proof;
- compact indexes/capsules are navigation/projection surfaces unless a later independently reviewed contract explicitly says otherwise;
- role boot sets are minimum starting packs, never maximum context ceilings;
- missing, stale, contradictory or materially ambiguous compact context forces deeper authoritative reads;
- Reviewer independence cannot be replaced by Worker summaries/capsules;
- history may leave normal bootstrap, but accepted evidence is never deleted merely to save tokens;
- H1's accepted context-poor local executor remains a bounded mechanical executor and is not generalized into a context-poor Worker/Reviewer.

## Adoption

The CTX programme plan is accepted, but the plan itself changes no operating rule. Each WP becomes binding only after its own fresh independent PASS + merge + DocSync.

The next CTX action is `WP-CTX-01 — Role-specific bootstrap + accepted-state navigation`. It remains `NOT_STARTED` until a human explicitly starts its Worker.

Human priority is to execute `WP-CTX-01` immediately so subsequent remote/local sessions start benefiting before H1 local work. Until CTX-01 itself passes, the existing bootstrap/read rules remain binding unchanged.
