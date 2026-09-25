# H1 remaining-work compression amendment

Status: PLANNING AMENDMENT / REVIEW CANDIDATE
Class: DOCS_ONLY / NON_PRODUCT
Base main SHA: `8f7c1970cfd3533908a29b556cff199d8e7689f7`
Scope: `WP-H1-08` through `WP-H1-GATE`; `WP-H1-07` remains unchanged.

## Purpose

Reduce duplicated implementation and proof work in the remaining H1 chain by consuming accepted H0/H1 guarantees instead of rebuilding them inside later workpacks. This amendment does not weaken any acceptance criterion, remove any dependency edge, redefine canonical authority, or reopen accepted H0/H1 work.

## Binding compression rule

For `WP-H1-08` through `WP-H1-11`, if an acceptance criterion can be satisfied by invoking and composing an already accepted predecessor guarantee, that part is `INTEGRATION_ONLY`. The Worker must consume the accepted public capability/oracle rather than create a parallel implementation, registry, transaction path, persistence format, recovery mechanism or second proof oracle for the same semantic claim.

A later integration/conformance workpack may reopen a predecessor only when effective evidence proves that predecessor's accepted guarantee false or inapplicable. It may not duplicate the predecessor merely to make its own evidence easier to construct.

H0 remains closed. H0/H0S may be reopened only by concrete contradictory or scale evidence under their existing contracts; no remaining H1 workpack may expand H0 speculatively.

## WP-H1-07

No compression amendment. H1-07 still owns the effective allowlisted component-adapter claim: schema-to-adapter completeness, real Unity field/reference round-trip, normalized observation and fail-closed unsupported semantics.

## WP-H1-08 — compressed ownership

H1-08 must consume HK05 diagnostic semantics and the causal validators/checkers already owned by H1-04/05/06/07.

H1-08 newly owns only:

- independently enumerable Unity validator/invariant composition;
- Unity-specific deterministic aggregation, including dependency-local ambiguity;
- truthful preflight versus post-materialization phases;
- the rule that a failed postflight cannot publish a successful active generation;
- any genuinely unowned Unity invariant required by its contract, such as non-finite transform detection.

It must not build a second generic diagnostic framework or reimplement accepted catalogue, marker, prefab or component validity checks merely to aggregate them.

## WP-H1-09 — compressed ownership

H1-09 must consume accepted H0 commit, validation, provenance, CAS and HK08B stale-recovery semantics, plus H1-00 drift vocabulary and H1-05 observation/rematerialization.

Its implementation ownership is limited to:

- a complete deterministic expected-versus-effective managed Unity drift oracle; and
- an allowlisted Unity-edit-to-canonical-mutation proposal compiler.

The proposal remains non-authoritative until ordinary H0 `plan -> dry-run -> apply` accepts it. H1-09 must not add a second transaction engine, recovery protocol, provenance path, commit authority or implicit merge mechanism.

## WP-H1-10 — compressed ownership

H1-10 must consume HK06B snapshot/rebase, HK06C replay and already accepted H1 deletion/rematerialization/rebuild guarantees.

Its new implementation ownership is limited to:

- a versioned project-checkpoint manifest referencing accepted canonical artifacts;
- atomic/current checkpoint publication at the project level;
- bridge/profile/catalogue/source/package/editor fingerprint capture and verification;
- orchestration of a truthful fresh-process restore and clean Unity rebuild;
- structured blocked-rebuild behavior for missing or incompatible required fingerprints.

H1-10 must not define a new canonical persisted-world format, replay model, lineage model, recovery store or redundant Unity rebuild algorithm when predecessor capabilities already provide those semantics.

## WP-H1-11 — conformance-first / zero-code target

H1-11 is a broad real-source conformance challenge, not a generic bridge implementation owner.

The Worker must first attempt the representative Quaternius slice using only accepted H1-04 through H1-10 capabilities. The target is `ZERO_CODE` outside selected-source manifest/import-configuration evidence unless the representative real source exposes a genuinely new in-boundary requirement.

Failures route causally:

- catalogue/source identity -> H1-04;
- scene/materialization/publication -> H1-05;
- prefab/asset relationships -> H1-06;
- component adapters -> H1-07;
- validation -> H1-08;
- drift/proposals -> H1-09;
- checkpoint/rebuild -> H1-10.

H1-11 retains ownership only of representative selected-source breadth and any concrete import/pivot/scale/rig/animation-shape compatibility requirement that is not already an accepted generic predecessor guarantee. It may not introduce a second real-asset-specific implementation of an existing bridge subsystem.

## WP-H1-GATE — proof-only boundary

H1-GATE remains mandatory and separate.

It owns composition/readiness evidence, residual/dependency reconciliation, the deterministic reference scenario, the fresh independent public-client AI-agent trial and the H2 authorization decision.

The Gate must add no product capability or replacement oracle. If the reference scenario needs a semantic bridge fix, the causal predecessor reopens. Gate-owned work is limited to orchestration/evidence and gate-specific omission controls.

## Resulting minimum chain

```text
H1-07  effective component semantics
  -> H1-08  validator composition + pre/postflight
  -> H1-09  drift oracle + reverse proposal compiler
  -> H1-10  checkpoint manifest/fingerprints + fresh-process orchestration
  -> H1-11  representative real-source conformance, zero-code target
  -> H1-GATE  composed proof + fresh-agent trial
```

The DAG and PASS dependencies remain unchanged. This amendment changes implementation/proof ownership only by forbidding redundant mechanisms and forcing accepted predecessor guarantees to be consumed where applicable.
