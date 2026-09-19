# Dependency / IP Policy

Version: 1.1 — 2026-09-19
Status: BINDING — accepted by `WP-HK-00A` independent PASS on 2026-09-19.

## Goal

Reuse excellent external engineering without allowing a dependency to become a hidden product ceiling, legal surprise or semantic source of truth.

## Default admissible classes

Direct runtime/tooling dependencies are preferred when their exact adopted version is commercially permissive and compatible with distribution, such as MIT, Apache-2.0 or BSD-family licenses.

Other licenses require an explicit reviewed exception. Copyleft components that could impose distribution obligations on Arkus code must not be embedded into shipped product artifacts without legal/architecture review; when useful, prefer external-tool isolation with a clean optional boundary.

License claims must be reverified at the exact adopted version/commit before code is embedded, copied, vendored or added as a direct shipped dependency. A README statement seen during research is not sufficient adoption evidence. Unknown, ambiguous or incompatible commercial terms are fail-closed: reject or isolate until the terms are resolved and reviewed.

Research/audit documents may record provisional observations for future candidates, but those observations do not satisfy this exact-version adoption gate.

## Required record for every material external dependency

Record before adoption:

- package/repository identity;
- exact pinned version or upstream commit;
- license at that version;
- source URL;
- whether code is linked, vendored, copied, generated from, or only invoked externally;
- which Arkus guarantee the dependency helps implement;
- which Arkus guarantees explicitly remain outside the dependency's authority;
- Arkus interface/conformance boundary around it;
- replacement strategy;
- notices/attribution obligations;
- security/update owner;
- shipped/runtime vs build/test-only classification.

Material vendored/copied code must retain required license headers/notices and identify local modifications.

## Semantic-authority rule

No external component may be the only definition of:

- canonical game/world state;
- Arkus capability semantics;
- transaction semantics;
- validation/invariant identity;
- provenance/replay meaning;
- persisted canonical format.

External components may implement generic mechanisms behind Arkus-owned contracts. Removing or replacing them must not require changing those semantic contracts merely to preserve the dependency's API or data model.

## Mutation-boundary rule

An external transport SDK, engine integration, serializer, workflow helper or host framework may not create a mutation path that bypasses the accepted Arkus authoring transaction pipeline. Adapter-specific side effects are downstream realization concerns and must remain distinguishable from canonical state mutation.

## Replaceability rule

Critical dependencies require an Arkus conformance boundary. Replacing a transport SDK, serializer helper, fuzzing framework or engine integration should not require redesigning canonical product semantics.

If replacement requires changing canonical capability identity, state meaning, transaction semantics, validation identity, replay meaning or persisted canonical format, the dependency has crossed the product boundary and is not acceptable without a separately reviewed architecture change.

## Supply-chain rule

Before a commercial release:

- produce an SBOM for shipped artifacts;
- preserve `THIRD_PARTY_NOTICES` or equivalent generated notice bundle;
- pin dependencies/lock files where ecosystems support it;
- scan known vulnerabilities under a documented policy;
- distinguish build/test-only dependencies from shipped runtime dependencies.

Foundational/H0 adoption records must preserve enough provenance for those release artifacts to be generated later without reconstructing dependency history from memory.

## Hosted-service rule

Mandatory hosted third-party services are forbidden for canonical local authoring unless separately approved as an explicit product dependency. Optional cloud integrations may exist, but local canonical state, validation, replay and evidence must not become unusable when the service is absent.

## Adoption gate

A dependency is rejected or isolated when any of these is true without an explicit reviewed exception:

- its exact-version license/commercial terms are incompatible, unknown or materially ambiguous;
- it forces engine/transport/model-vendor lock-in into the kernel;
- it narrows required Arkus capability scope;
- it stores canonical state in an opaque/nonportable form with no accepted exit path;
- it cannot be pinned/audited sufficiently for reproducible proof;
- it becomes the sole authority for Arkus semantics listed above;
- it introduces a mutation path outside the canonical transaction boundary;
- removing it would require changing Arkus semantic contracts rather than only the adapter/implementation.

## Review timing

`EXTERNAL_HARNESS_ADOPTION_AUDIT.md` is a capability/adoption research snapshot, not an approval to import code. Every actual dependency decision must re-run this policy at the point of adoption, with the exact version/commit and intended linkage/distribution mode known.
