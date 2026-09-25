# WP-H1-09 — PREDECESSOR_CONTRACT_CHECK

Status: WORKER ACTIVE

## Accepted direct predecessor

`WP-H1-08 — Integrated Unity validation` is accepted.

- Frozen candidate / PRODUCT_SHA: `8b7ea7b9ad74d67c5b300180b79833fe89350f4d`
- Canonical implementation PR: `#216`
- Independent PASS review: `#5319609389`
- Implementation merge: `f22fa99e4427849b8a7202bf24bbae83fb90ee83`
- Post-PASS DocSync PR: `#219`
- DocSync merge / H1-09 baseline: `73cdbdacd3c4b4adff4cd9086b55356405c23370`

The H1 remaining-scope compression amendment is also binding:

- Candidate: `46e679986e4e482a1a29f3326ff20e2522d0b409`
- PR: `#209`
- Independent PASS review: `#5315077514`
- Merge: `8cc921719861f5e3dc4affa6bc2c07b9c22fdb34`

## Authoritative escalations used

The Worker read the exact current `WP-H1-09`, the binding H1 remaining compression amendment, the accepted H1-08 lifecycle/validation implementation surfaces, H1-00 drift vocabulary, H1 managed-scene plan/observation implementation, H0 mutation contract and the accepted Unity binding compiler. This escalation was necessary because H1-09 consumes concrete observation, drift and canonical-proposal semantics that are not safely reconstructed from status metadata alone.

## Inherited guarantees consumed

H1-09 consumes rather than re-proves:

1. H0 `authoring.change.plan`, `authoring.change.dry-run` and `authoring.change.apply` remain the only canonical mutation path, with expected revision/hash CAS, idempotency, atomic validation, provenance and HK08B recovery semantics.
2. H1-00 owns the shared projection drift vocabulary and normalized deterministic projection identities.
3. H1-04 owns accepted catalogue identity/resolution; Unity native path/GUID/local-file-id remain bridge locators, never canonical identity.
4. H1-05 owns canonical -> Unity managed-scene planning, effective observation and rematerialization direction.
5. H1-06/07 own prefab lineage and the admitted component schemas/realization fidelity used by the effective observation.
6. H1-08 owns integrated preflight/postflight validation and structured expected-invalidity handling for materialization/current validation.
7. H1-03A owns bounded Unity Editor execution lifecycle, project lease, typed worker dispatch and result identity.

No second transaction engine, commit authority, recovery protocol, provenance route or implicit merge is introduced by H1-09.

## Guarantees newly owned by H1-09

H1-09 owns exactly the compressed scope:

1. a complete deterministic expected-vs-effective oracle for the managed Unity scope, including normalized classification of missing, extra, changed, ambiguous, catalogue-drift and canonical-ahead states while exposing unrelated unmanaged state without contaminating the managed digest; and
2. an allowlisted Unity-edit -> canonical-mutation proposal compiler that emits ordinary H0 mutation input with expected revision/hash and mechanically derived dependencies, never mutating canonical state by inspection or proposal generation.

Canonical -> Unity repair remains rematerialization through the accepted H1-05/H1-08 path. Unity -> canonical is proposal only and must re-enter H0 plan/dry-run/apply.

## Concrete reopen conditions

An inherited guarantee is reopened only if effective evidence shows its accepted boundary is inapplicable or false, for example:

- the accepted materializer/observer omits a state surface that H1-09 must compare and therefore cannot supply the inherited normalized effective facts;
- H0 plan/dry-run/apply cannot represent the mechanically compiled extension mutation or fails to enforce the expected-base CAS claimed by its accepted contract;
- H1-08 validation admits a materialization state that H1-09 cannot safely observe because the accepted validation claim itself is false; or
- accepted catalogue/component identity cannot map an otherwise supported effective component edit back to a unique canonical logical reference.

Theoretical possibility or a desire for duplicate proof is not a reopen condition.
