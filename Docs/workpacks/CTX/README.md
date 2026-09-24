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

## Standalone CTX sequence — COMPLETE

```text
WP-CTX-01  Role-specific bootstrap + derived accepted-state navigation
    ↓
WP-CTX-02  Accepted-contract capsules + predecessor inheritance compression
    ↓
WP-CTX-03  Structured evidence / DocSync / history separation + measured closure
```

All three are PROCESS_ONLY. None may change product/runtime semantics, accepted workpack meaning, or the Worker → fresh independent Reviewer boundary.

`NON-PRODUCT-FOUNDATIONAL` means CTX does not invoke the product `FOUNDATIONAL_PROOF_STANDARD.md` merely by existing. It does **not** mean a lighter causal standard: compact context cannot become authoritative merely because it is convenient.

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

## Accepted standalone adoption

`WP-CTX-01 — Role-specific bootstrap + accepted-state navigation` is **COMPLETE** on repaired frozen candidate `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39` (independent PASS review `#5273801466`, PR `#110`, implementation merge `fbd3e5526e760efc89f54e7c12a274af10d4765f`). Its accepted `docsync-first-parent-v1` contract keeps the derived accepted-state projection non-authoritative and stale-detected.

`WP-CTX-02 — Accepted-contract capsules + predecessor inheritance compression` is **COMPLETE** on final circuit-breaker candidate `e5053b778e050cff83e2443fef888c64883c88ca` (independent PASS review `#5274937744`, PR `#113`, implementation merge `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`). The accepted capsule layer is navigation only, preserves authoritative-source escalation, and its PA completeness/oracle selectors are checker-owned or exact-bound so the audited compact surface cannot silently shrink its own universe.

`WP-CTX-03 — Structured evidence, quality-preserving context envelope, DocSync and history separation` is **COMPLETE** on final circuit-breaker candidate `8851f3a295c848be5724d5d9b796e00d2037d6e8` (independent PASS review `#5277502546`, PR `#121`, implementation merge `d2cf7145b4ec83c3935286f2d9faeabbcd032148`). Its accepted effective-read-set oracle derives repository-backed mandatory context from profile/route authority, caller omission cannot silently shrink that universe, and new unclassified read surfaces/classes fail closed.

The reviewed CTX-01 → CTX-02 → CTX-03 compression programme is **COMPLETE**. There is no further standalone CTX compression WP planned.

## CTX↔DW composition — GATE ACCEPTED

`WP-CTX-DW-GATE — CTX↔DW discoverability and selective-adoption gate` is **COMPLETE / ACCEPTED** on candidate `7912383c806171aea101907c05a3b008bc7a5e3b`, independent PASS review `#5298141688`, PR `#165`, implementation merge `8b7b7715c7365acfb94670b179f21668a0562534`.

The composition principle is: **all material information must be discoverable from CTX, but not all material information belongs in DW**. For represented semantic facts, authoritative source > derived DW projection > CTX summary/navigation. For routing/process, accepted CTX effective-read-set, mandatory-read and escalation rules remain authoritative; DW only advises materiality and can never shrink those obligations. A material contradiction forces source-open/rebuild/fail-closed handling.

The Gate intentionally did **not** create a real H1 projection because H1-04 had not yet accepted the Quaternius/catalogue universe from which that projection must be derived.

## H1 CTX↔DW adoption sequence

The two missing H1 implementation/adoption boundaries are now explicit workpacks:

```text
H1-04 PASS — establishes accepted source/catalogue authority
    ↓
WP-CTX-DW-H1-01 — H1 projection bootstrap + lifecycle
    ↓
H1-05 — first eligible real consumer / managed-scene shape
    ↓
H1-06 — second distinct real consumer / asset-prefab shape
    ↓
WP-CTX-DW-H1-02 — selective-adoption validation + H1-07+ disposition
```

### `WP-CTX-DW-H1-01`

Runs only after H1-04 PASS. It owns the real H1→DW projection lifecycle: independently enumerated source universe, versioned adapter/schema, completeness, source-open provenance, staleness, deterministic rebuild, corruption controls and routing admission. It does not own H1 product semantics and does not block H1-05+; if unavailable, CTX routes directly to authority.

### `WP-CTX-DW-H1-02`

Runs after accepted H1-05 and H1-06 observations. It validates whether the projection is actually useful across two materially different real consumer shapes without hiding blockers or mandatory reads, then publishes bounded `USE / OPTIONAL / NOT_MATERIAL` guidance for H1-07+.

It must consume accepted H1-05/H1-06 evidence rather than creating shadow duplicate Workers merely to produce nicer adoption metrics.

Neither H1 CTX↔DW WP is allowed to contaminate the mandatory fresh public-client trial owned by `WP-H1-GATE`.

Detailed H1 integration remains recorded in `Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md`.

## Future H2 trigger — do not pre-author yet

Future H2 planning must explicitly decide whether CTX/DW becomes part of any public/external **knowledge portability** boundary. If the accepted H2 plan needs such a claim, it must author a separate `CTX-DW-H2-*` workpack then, against the real H2 consumer/public boundary.

Do not create that WP now: H1-05/H1-06 and `WP-CTX-DW-H1-02` have not yet produced the evidence needed to know what H2 should preserve, change or reject.

Current H2 planning input is `Docs/workpacks/H2/CTX_DW_PLANNING_INPUT.md`.
