# CTX — Agent context / process efficiency

Status: **PLAN CANDIDATE / PROCESS_ONLY**
Execution: **REMOTE_OK**
Purpose: improve agent reasoning quality first, then reduce context/token/local-execution cost without weakening any accepted product, proof or review obligation.

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

The CTX plan itself changes no operating rule. Each WP becomes binding only after its own fresh independent PASS + merge + DocSync.

If this plan passes, its documentation-only DocSync registers `CTX/` in the root workpack index and names `WP-CTX-01` as the next CTX action; it must not implement any CTX-01 operating rule by implication.

Human priority is to execute `WP-CTX-01` immediately after plan DocSync so subsequent remote/local sessions start benefiting before H1 local work.
