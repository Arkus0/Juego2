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

`WP-CTX-01 — Role-specific bootstrap + accepted-state navigation` is **COMPLETE** on repaired frozen candidate `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39` (independent PASS review `#5273801466`, PR `#110`, implementation merge `fbd3e5526e760efc89f54e7c12a274af10d4765f`). Its accepted `docsync-first-parent-v1` contract keeps the derived accepted-state projection non-authoritative and stale-detected.

`WP-CTX-02 — Accepted-contract capsules + predecessor inheritance compression` is **COMPLETE** on final circuit-breaker candidate `e5053b778e050cff83e2443fef888c64883c88ca` (independent PASS review `#5274937744`, PR `#113`, implementation merge `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`). The accepted capsule layer is navigation only, preserves authoritative-source escalation, and its PA completeness/oracle selectors are checker-owned or exact-bound so the audited compact surface cannot silently shrink its own universe.

`WP-CTX-03 — Structured evidence, quality-preserving context envelope, DocSync and history separation` is **COMPLETE** on final circuit-breaker candidate `8851f3a295c848be5724d5d9b796e00d2037d6e8` (independent PASS review `#5277502546`, PR `#121`, implementation merge `d2cf7145b4ec83c3935286f2d9faeabbcd032148`). Its accepted effective-read-set oracle derives repository-backed mandatory context from profile/route authority, caller omission cannot silently shrink that universe, and new unclassified read surfaces/classes fail closed.

The reviewed CTX-01 → CTX-02 → CTX-03 programme is **COMPLETE**. There is no next standalone CTX compression/process workpack. CTX does not semantically block H1/CITY/PA; the CTX-03 side prerequisite used by DW is accepted.

## Post-program CTX↔DW composition

After accepted `WP-DW-GATE`, a separate cross-track `WP-CTX-DW-GATE` may test the composition of CTX as control/routing plane and DW as selective structured knowledge plane. This does not reopen CTX-01..03 and does not imply general DW adoption.

The composition principle is: **all material information must be discoverable from CTX, but not all material information belongs in DW**. For represented semantic facts, authoritative source > derived DW projection > CTX summary/navigation. For routing/process, accepted CTX effective-read-set, mandatory-read and escalation rules remain authoritative; DW only advises materiality and can never shrink those obligations. A material contradiction forces source-open/rebuild/fail-closed handling.

`WP-CTX-DW-GATE` explicitly does not block H1-03/H1-03A. It also does not assume H1 projection semantics that DW-GATE never proved. H1-04 remains source-first and establishes the real Quaternius/catalogue authority; only after H1-04 PASS may a separate non-product CTX↔DW owner build and independently validate an H1 projection with source-universe completeness, provenance, staleness and deterministic rebuild controls. H1-05 is the first eligible real consumer of that projection when current/material, and H1-06 is the second distinct asset/prefab observation. If the projection is absent or stale, H1 continues through CTX→authoritative sources rather than blocking.

The required fresh public-client AI-agent trial owned by `WP-H1-GATE` remains pure: it uses the H1 public bootstrap/discovery/schemas and is not pre-seeded with Juego2-private CTX/DW knowledge. CTX↔DW agent composition, if measured, is a separate observation and cannot repair or substitute H1-GATE public discoverability.

Detailed H1 integration is recorded in `Docs/workpacks/H1/CTX_DW_ADOPTION_PLAN.md`; future H2 planning input is recorded separately under `Docs/workpacks/H2/CTX_DW_PLANNING_INPUT.md`.
