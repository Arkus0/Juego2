# CTX-02 Predecessor Contract Check

`PREDECESSOR_CONTRACT_CHECK`

Status: COMPLETE / REVALIDATED IN FAIL CYCLE 5  
WP: `WP-CTX-02`  
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Direct accepted dependency: `WP-CTX-01`

## Accepted predecessor identity

- CTX-01 repaired frozen/reviewed candidate: `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39`
- Independent PASS: review `#5273801466` on PR `#110`
- Implementation merge: `fbd3e5526e760efc89f54e7c12a274af10d4765f`
- Successful CTX-01 DocSync completion SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`
- CTX-01 completion metadata: `Docs/workpacks/CTX/WP-CTX-01.md`
- Binding bootstrap contract: `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`

Cycle-5 live-state refresh: `main` has advanced to `b831050e9df8b61b76744e0c5f544bd7ec2d79b5` through later accepted work, including merged DW planning. That movement does not change CTX-01's accepted reviewed/merge identity or the inherited CTX-01 guarantees consumed here, and it does not overlap CTX-02's process-only capsule write set. No predecessor reopen or rebase is required merely because unrelated `main` advanced.

## Inherited guarantees consumed by CTX-02

1. Live authenticated GitHub remains authoritative for mutable PR/branch/review/check state.
2. Exact repository contracts, code/tests and accepted evidence remain authoritative for semantics, proof and acceptance.
3. `ACCEPTED_STATE_INDEX.json` and other compact projections are navigation-only and cannot grant semantic authority.
4. CTX-01 freshness is the non-self-referential `docsync-first-parent-v1` relation; missing/stale/contradictory compact context causes escalation rather than inference from silence.
5. Worker predecessor reconstruction, Worker pre-review, exact-SHA freeze and independent Reviewer obligations remain unchanged.
6. Compact context is a minimum starting surface, never a ceiling; materially unresolved questions deepen to authoritative sources.

These guarantees are consumed rather than re-proved. CTX-02 may build on them but may not weaken or replace them.

## Guarantees newly owned by CTX-02

CTX-02 owns the accepted-contract capsule mechanism: schema/contract, explicit non-authoritative semantics, accepted-state/repository/source/predecessor bindings, integrity and freshness checks, deterministic fail-closed validation, automated positive/negative controls and CI-suitable invocation.

The final circuit-breaker makes the trust boundary explicit: an audited capsule/index may not define its own completeness universe, select its own mechanical oracle, redirect canonical roots/templates/selectors, or use coordinated omission/configuration to turn a material requirement into implicit non-applicability. Arbitrary natural-language equivalence that cannot be proved independently remains human/source escalation rather than an automatic claim.

## Reopen boundary

CTX-01 is reopened only by concrete evidence that an inherited CTX-01 guarantee is false or inapplicable to the effective CTX-02 path—for example, if the accepted bootstrap contract itself permits a compact artifact to become authoritative, or if the accepted-state projection freshness relation used by CTX-02 is demonstrably inconsistent with accepted CTX-01.

A CTX-02 capsule becoming stale, malformed, mismatched, reopened/revoked or otherwise invalid does **not** reopen CTX-01; the capsule fails closed and the consumer returns to authoritative sources.

## Ownership/scope conclusion

Dependency remains accepted and DocSync-complete. `WP-CTX-02` remains dependency-valid despite unrelated live-main movement. CTX-02 may implement only the bounded capsule contract/validator/fixtures/evidence and must not alter product/runtime semantics or accepted CTX-01 authority ordering.
