# CTX-02 Predecessor Contract Check

`PREDECESSOR_CONTRACT_CHECK`

Status: COMPLETE BEFORE IMPLEMENTATION
WP: `WP-CTX-02`
Baseline SHA: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`
Direct accepted dependency: `WP-CTX-01`

## Accepted predecessor identity

- CTX-01 repaired frozen/reviewed candidate: `ea92e4eab36566ab3d0367fef64fefc0b2b0ff39`
- Independent PASS: review `#5273801466` on PR `#110`
- Implementation merge: `fbd3e5526e760efc89f54e7c12a274af10d4765f`
- Successful DocSync completion: final live main `f7b4f1e8247dfc927203dca6754b71aaa53938f3`
- CTX-01 completion metadata: `Docs/workpacks/CTX/WP-CTX-01.md`
- Binding bootstrap contract: `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md`

## Inherited guarantees consumed by CTX-02

1. Live authenticated GitHub remains authoritative for mutable PR/branch/review/check state.
2. Exact repository contracts, code/tests and accepted evidence remain authoritative for semantics, proof and acceptance.
3. `ACCEPTED_STATE_INDEX.json` and other compact projections are navigation-only and cannot grant semantic authority.
4. CTX-01 freshness is the non-self-referential `docsync-first-parent-v1` relation; missing/stale/contradictory compact context causes escalation rather than inference from silence.
5. Worker predecessor reconstruction, Worker pre-review, exact-SHA freeze and independent Reviewer obligations remain unchanged.
6. Compact context is a minimum starting surface, never a ceiling; materially unresolved questions deepen to authoritative sources.

These guarantees are consumed rather than re-proved. CTX-02 may build on them but may not weaken or replace them.

## Guarantees newly owned by CTX-02

CTX-02 owns the accepted-contract capsule mechanism: schema/contract, explicit `non_authoritative` semantics, accepted-state/repository/source/predecessor bindings, integrity and freshness checks, deterministic fail-closed validation, automated positive and negative controls, and CI-suitable invocation.

In particular, CTX-02 must ensure that a capsule cannot self-certify. Validation inputs representing accepted state and predecessor state must be independent from capsule content, while source path revisions are recomputed from repository bytes.

## Reopen boundary

CTX-01 is reopened only by concrete evidence that an inherited CTX-01 guarantee is false or inapplicable to the effective CTX-02 path—for example, if the accepted bootstrap contract itself permits a compact artifact to become authoritative, or if the accepted-state projection freshness relation used by CTX-02 is demonstrably inconsistent with the accepted CTX-01 contract.

A CTX-02 capsule becoming stale, malformed, mismatched, reopened/revoked, or otherwise invalid does **not** reopen CTX-01; the capsule must fail closed and the consumer returns to authoritative sources.

## Ownership/scope conclusion

Dependency is accepted and DocSync-complete. `WP-CTX-02` is dependency-valid on the recorded baseline. CTX-02 may implement only the bounded capsule contract/validator/fixtures/evidence and must not alter product/runtime semantics or accepted CTX-01 authority ordering.
