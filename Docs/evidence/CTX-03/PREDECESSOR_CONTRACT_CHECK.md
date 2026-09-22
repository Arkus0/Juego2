# CTX-03 — PREDECESSOR_CONTRACT_CHECK

Status: **COMPLETE / IMPLEMENTATION MAY BEGIN**  
Worker: **ChatGPT remote Worker**  
Baseline main: `107694d3850a478849bffd9510dc030910fc8aa3`  
Direct dependency: `WP-CTX-02`

## Accepted predecessor identity

`WP-CTX-02` is accepted and its post-PASS reconciliation is complete:

- final reviewed candidate: `e5053b778e050cff83e2443fef888c64883c88ca`;
- independent PASS: review `#5274937744` on PR `#113`;
- implementation merge: `af63528b63ba9b3ddf2e612c0ad8dff96a57a6c2`;
- DocSync evidence: `Docs/evidence/CTX-02/DOCSYNC.md` with `DOCSYNC_COMPLETE`;
- combined DocSync persistence/main used by CTX-03 baseline: `107694d3850a478849bffd9510dc030910fc8aa3`.

Live GitHub and the accepted CTX state both resolve `WP-CTX-03` as the dependency-valid next CTX workpack.

## Capsule path and authoritative escalations

No accepted-contract capsule exists for `WP-CTX-02` in `Docs/engineering/context-capsules/index.json`, so the CTX-02 boundary was reconstructed from authoritative sources rather than inferred from compact navigation.

Authoritative sources opened for this check:

- `Docs/workpacks/CTX/WP-CTX-02.md`;
- exact independent PASS/review lineage on PR `#113`;
- `Docs/evidence/CTX-02/DOCSYNC.md`;
- `Docs/engineering/CONTEXT_CAPSULE_V1.md` because CTX-03 directly changes how accepted compact context is measured, routed and challenged;
- `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`, `context-bootstrap-profiles.json` and `WORKER_REVIEW_PROTOCOL.md` because CTX-03 directly owns their process envelope / closure behavior.

## Inherited guarantees consumed by CTX-03

CTX-03 consumes rather than re-proves these accepted CTX-02 guarantees:

1. Accepted-contract capsules are `NON_AUTHORITATIVE_NAVIGATION_ONLY`; exact source semantics, proof and independent review remain authoritative.
2. Missing, stale, contradictory, lossy or selector-mismatched compact context fails closed to authoritative reconstruction.
3. The audited artifact cannot choose the universe/oracle used to prove its own completeness; canonical capsule/index/PA selectors are checker-owned or exact-bound.
4. The accepted PA chain is discovered from canonical COMPLETE workpacks, not from result/capsule/index presence, and its current structured disposition surface preserves exact key/status semantics.
5. Representative H1/CITY/PA semantic controls catch the accepted same-ID substitution/material-omission classes while arbitrary future prose equivalence remains a human/escalation responsibility.
6. Reviewer independence, exact-SHA review, concrete predecessor reopen and full authoritative-source access were not weakened by CTX-02.

CTX-03 does not duplicate the five-cycle CTX-02 circuit-breaker proof merely for defence-in-depth. It treats those guarantees as binding unless concrete evidence below reopens them.

## Guarantees newly owned by CTX-03

CTX-03 owns, and therefore must prove independently:

- reproducible same-snapshot pre/post context measurement and declared uncertainty;
- auditable `CONTEXT_ESCALATIONS` that cannot suppress mandatory deepening;
- explicit cumulative-PA context-cost measurement and a no-duplication decision against accepted CTX-02;
- structured proof/state/handoff representations that remain derived and independently discoverable;
- transactional Worker `REVIEW_READY` closure including the real Automation V2 marker and final live-HEAD check;
- ROADMAP/history and DocSync state separation without provenance loss or contradictory bootstrap state;
- a CI-only process envelope derived from effective role profiles, with reviewed ceilings and causal negative controls;
- re-derived historical FAIL/incomplete-handoff classification with only deterministic causal portions adopted as gates;
- bounded historical-blocker quality-preservation replay;
- reusable future consumer-repo primitives without deciding H2/H3 product ownership.

## Concrete predecessor reopen condition

Reopen CTX-02 only if CTX-03 obtains concrete evidence that an accepted CTX-02 guarantee is false or inapplicable to the effective path being consumed — for example, the canonical accepted capsule audit can still return GREEN while omitting/misrouting a material source required by a CTX-03 replay, or accepted CTX-02 identity/adoption no longer agrees with live authoritative state.

A theoretical possibility, a new CTX-03 convenience format, or a desire to duplicate CTX-02 proof is not sufficient to reopen that boundary.

## Decision

`PREDECESSOR_CONTRACT_CHECK: COMPLETE`

The dependency is satisfied and no predecessor contradiction was found. CTX-03 implementation may proceed from baseline `107694d3850a478849bffd9510dc030910fc8aa3` inside its PROCESS_ONLY scope.
