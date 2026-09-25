# H1-07 compression amendment

Status: PLANNING AMENDMENT / REVIEW CANDIDATE
Class: DOCS_ONLY / NON_PRODUCT
Base main SHA: `aec8c28cf7cd2452d117d18ee05a91c1da87c1e4`
Scope: `WP-H1-07` only.

## Purpose

Reduce duplicated implementation and proof work inside H1-07 without weakening its component-adapter contract, removing any acceptance criterion, changing the H1 DAG, or reopening accepted predecessor semantics.

H1-07 is an allowlisted component-semantics workpack. It is not an owner for generic Unity generation, asset transport, whole-project regression, web publication, predecessor catalogue/prefab correctness, or final H1 validation aggregation.

## Binding ownership rule

An H1-07 failure is causally owned here only when evidence directly falsifies one of the following component-adapter claims:

1. every declared H1 component schema has exactly one effective adapter and every effective adapter has exactly one composed schema;
2. supported scalar/vector/enum/reference values round-trip through the existing materialize -> save/reload -> inspect path;
3. canonical-object and catalogue references resolve back to the exact intended logical targets;
4. normalized component observation is deterministic and excludes volatile implementation-only fields;
5. unsupported component, field, type or reference fails closed without a generic reflection fallback;
6. the new component capability is bound into the existing canonical discovery and both transports without creating a parallel registry or transport implementation.

These are the complete semantic blockers owned by H1-07.

## Consumed / integration-only guarantees

H1-07 must consume, rather than rebuild or independently re-prove:

- H1-04 catalogue, source and component-schema identity;
- H1-05 scene/materialization/publication semantics;
- H1-06 prefab and asset-relationship semantics;
- H0 mutation/validation authority and existing canonical discovery/transport machinery;
- accepted source-slice / asset-distribution integrity evidence used to make the representative H1 assets available to Unity;
- generic Unity serialization behavior beyond the bounded supported fields exercised by the H1-07 adapters.

The Worker may add only the minimal glue needed to bind the H1-07 component capability into those accepted mechanisms. It must not create a replacement catalogue, materializer, prefab oracle, transport registry, publication path, generic serializer or broad asset oracle.

## Minimum sufficient proof

H1-07 PASS requires a bounded proof set, not a broad regression campaign:

- independently enumerate declared component schemas and effective adapter bindings and prove exact 1:1 equality;
- exercise one representative object carrying the required bounded component mix, including Renderer/material, Animator/controller-or-clip reference pressure, and one canonical-object relationship;
- prove the supported value/reference subset survives materialize -> save/reload -> inspect with exact normalized observations;
- prove canonical/catalogue references return the intended target identities;
- prove one or more representative unsupported component/field/type/reference cases fail closed before successful active-generation publication;
- prove the component capability is visible through the existing canonical discovery and both transports, without a second registry.

No additional whole-project, web-publication, broad asset-scanning, animation-playback, visual-quality, gameplay, physics/navmesh or unrelated subsystem regression is required to establish the H1-07 semantic claim.

## Failure routing

Classify failures before repairing:

### `H1_07_OWNED_FAIL`

Evidence directly falsifies one of the six owned component-adapter claims above. H1-07 remains blocked and the repair stays in H1-07.

### `PREDECESSOR_CONTRADICTION`

Evidence shows an accepted H1-04/H1-05/H1-06/H0 guarantee is false or inapplicable. Reopen the causal predecessor under its existing reopen rule. Do not patch a duplicate mechanism into H1-07.

### `EVIDENCE_OR_INFRA_BLOCKED`

The required H1-07 proof did not execute or persist because of runner, workflow, logging, checkout, unrelated GitHub Pages/IngestPortal regression, unrelated asset-transport infrastructure, or another non-semantic execution failure.

This state does not prove H1-07 semantically false. It may prevent PASS until the bounded H1-07 proof successfully runs, but it must not trigger speculative component-adapter repair or expansion of the H1-07 proof surface.

## Content-shape boundary

The required facade material and humanoid Animator/clip pressure exists only to exercise component field/reference shape. It does not prove animation playback, import quality, visual quality, art direction, gameplay behavior or the broader real-source conformance owned later by H1-11.

## Effect on the accepted remaining-H1 compression amendment

PR #209 remains historically correct for the state it accepted: it explicitly left H1-07 unchanged while compressing H1-08 through H1-GATE. This amendment extends the same predecessor-reuse discipline one step earlier to H1-07.

The previously accepted H1-08..Gate ownership rules remain unchanged.

## Result

```text
H1-07  bounded component-adapter semantics + minimal Unity round-trip proof
  -> H1-08  validator composition + pre/postflight
  -> H1-09  drift oracle + reverse proposal compiler
  -> H1-10  checkpoint manifest/fingerprints + fresh-process orchestration
  -> H1-11  representative real-source conformance, zero-code target
  -> H1-GATE  composed proof + fresh-agent trial
```

No H1 acceptance criterion or dependency edge is removed. This amendment changes only implementation/proof ownership and causal failure routing so H1-07 cannot accumulate unrelated regression obligations.