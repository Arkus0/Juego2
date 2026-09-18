# Dependency / IP Policy

Version: 1.0 — 2026-09-18
Status: proposed by `WP-HK-00A`; binding after independent acceptance.

## Goal

Reuse excellent external engineering without allowing a dependency to become a hidden product ceiling, legal surprise or semantic source of truth.

## Default admissible classes

Direct runtime/tooling dependencies are preferred when their current license is commercially permissive and compatible with distribution, such as MIT, Apache-2.0 or BSD-family licenses.

Other licenses require an explicit reviewed exception. Copyleft components that could impose distribution obligations on Arkus code must not be embedded into shipped product artifacts without legal/architecture review; when useful, prefer external-tool isolation with a clean optional boundary.

License claims must be reverified at the exact adopted version/commit. A README statement seen during research is not enough for final adoption evidence.

## Required record for every material external dependency

Record:

- package/repository identity;
- exact pinned version or upstream commit;
- license at that version;
- source URL;
- whether code is linked, vendored, copied, generated from, or only invoked externally;
- Arkus interface/conformance boundary around it;
- replacement strategy;
- notices/attribution obligations;
- security/update owner.

Material vendored/copied code must retain required license headers/notices and identify local modifications.

## Semantic-authority rule

No external component may be the only definition of:

- canonical game/world state;
- Arkus capability semantics;
- transaction semantics;
- validation/invariant identity;
- provenance/replay meaning;
- persisted canonical format.

External components may implement generic mechanisms behind Arkus-owned contracts.

## Replaceability rule

Critical dependencies require an Arkus conformance boundary. Replacing a transport SDK, serializer helper, fuzzing framework or engine integration should not require redesigning canonical product semantics.

## Supply-chain rule

Before a commercial release:

- produce an SBOM for shipped artifacts;
- preserve `THIRD_PARTY_NOTICES` or equivalent generated notice bundle;
- pin dependencies/lock files where ecosystems support it;
- scan known vulnerabilities under a documented policy;
- distinguish build/test-only dependencies from shipped runtime dependencies.

## Hosted-service rule

Mandatory hosted third-party services are forbidden for canonical local authoring unless separately approved as an explicit product dependency. Optional cloud integrations may exist, but local canonical state, validation, replay and evidence must not become unusable when the service is absent.

## Adoption gate

A dependency is rejected or isolated when any of these is true without an explicit reviewed exception:

- its license/commercial terms are incompatible or materially ambiguous;
- it forces engine/transport/model-vendor lock-in into the kernel;
- it narrows required Arkus capability scope;
- it stores canonical state in an opaque/nonportable form with no accepted exit path;
- it cannot be pinned/audited sufficiently for reproducible proof;
- removing it would require changing Arkus semantic contracts rather than only the adapter/implementation.
