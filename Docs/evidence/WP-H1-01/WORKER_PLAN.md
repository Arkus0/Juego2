# WP-H1-01 Worker plan

WP: `WP-H1-01`
Worker state: `ACTIVE`
Baseline SHA: `0f4dec86c7f9a5ab10db6479b995521d5f47cc78`
Baseline branch: `main`
Implementation branch: `wp-h1-01-unity-authoring-producer`
Class: `FOUNDATIONAL`
Execution requirement: `REMOTE_OK`

## Objective

Implement the portable Unity-scoped authoring producer required by `Docs/workpacks/H1/WP-H1-01.md`: one versioned binding document for the initial admitted Unity vocabulary, deterministic normalization/encoding and decode/inspect, mechanical derivation of canonical-object and catalogue dependencies, and canonical scoped-provider composition without Unity CLR/editor dependencies.

## Ownership / write set

Owned for this candidate:

- portable Unity authoring contract/producer code under `src/Arkus.EngineBridge/`;
- only the minimum H0 runtime/protocol project references needed to contribute through the already-accepted canonical composer, without changing canonical semantics;
- focused H1-01 tests under `tests/Arkus.Harness.Tests/`;
- bounded H1-01 validation/negative-conformance support under `scripts/` where needed;
- H1-01 evidence under `Docs/evidence/WP-H1-01/`;
- PR handoff metadata.

Not owned: `UnityEngine`/`UnityEditor`, native paths/GUID authority, catalogue inventory acquisition from Unity, asset existence/materialization, scene/prefab writing, public host-to-Editor lifecycle, gameplay/CITY schemas, H0 mutation semantics or a Unity-owned public registry.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted dependency: `WP-H1-00`.

- Reviewed candidate SHA: `3dc513dd963b77116fd45b5af8d800ae8993dd34`.
- Independent verdict: `PASS`, review `#5264661860` on PR `#75`.
- Implementation merge SHA: `c02cf54c89c13db43edda4a602b0c1620baa3fa2`.
- H1-00 DocSync completed by PR `#78`; baseline `main` is `0f4dec86c7f9a5ab10db6479b995521d5f47cc78` and authorizes `WP-H1-01`.

Inherited guarantees relevant to H1-01:

1. canonical identity/authority remains H0-owned; bridge contracts consume canonical anchors and may not become canonical write authority;
2. H1 projection inputs/observations/receipts have a portable versioned engine-neutral foundation and deterministic normalization semantics;
3. `Arkus.EngineBridge` remains free of Unity/editor/native-asset implementation dependencies;
4. accepted H0 composition/versioning, HK02A opaque extension + typed canonical dependency semantics, HK04 mutation authority and HK07 transport projection are compositional guarantees;
5. H1 identity separates canonical object IDs from Arkus logical catalogue IDs and Unity-native locators.

H1-01 newly owns:

1. one portable/versioned Unity binding document covering target logical scene, normalized local transform, logical prefab/asset binding, allowlisted component documents and structured references;
2. a schema-aware structured producer with deterministic compile/encode, decode and inspect behavior;
3. exact mechanical derivation of every understood structured canonical-object reference into HK02A dependency metadata;
4. exact mechanical derivation of every understood structured catalogue reference into provider-owned typed binding/plan data;
5. fail-closed rejection of caller-supplied dependency claims that duplicate, contradict or omit derived truth;
6. a Unity-scoped provider whose public capabilities are accepted by the canonical composer and therefore projected by existing JSONL/MCP adapters without adapter-specific route edits;
7. compatibility/version behavior for the binding/provider surface under accepted HK01 rules.

Inherited guarantees intentionally consumed rather than re-proved: generic HK01 composition completeness/version negotiation, generic HK02A opaque payload semantics, HK04 plan/dry-run/apply correctness, generic HK07 adapter parity, and H1-00 materializer/drift semantics. Focused delta tests will demonstrate that this provider actually participates in those accepted paths, not re-certify their whole universes.

Concrete predecessor reopen trigger: reopen HK02A only if effective producer evidence shows the accepted typed dependency surface cannot truthfully represent a structured canonical-object reference. Reopen H1-00 only if the portable bridge boundary itself cannot represent this provider without violating its accepted neutrality/versioning claim. Catalogue references alone are not a trigger because they are deliberately outside the canonical world-object universe.

## Binding architecture decisions

- `ADR-H1-001`: canonical `WorldObjectId`/extension identity and logical catalogue identity remain distinct; catalogue IDs are bridge/project-owned, never native Unity paths/GUIDs.
- `H1_ENGINE_BRIDGE_ARCHITECTURE` §4: structured producer derives canonical and catalogue references mechanically; raw opaque payload remains a low-level primitive and gains no automatic-reference claim.
- No Unity package/project dependency is permitted in this WP.

## Claim / trust boundary

Inside portable .NET contract data, the producer understands a finite admitted binding vocabulary and deterministically normalizes it. Every structured reference location in that vocabulary is independently walked by the producer into exactly one typed derived dependency of the correct domain, and any optional caller dependency assertion must exactly equal that derived set. The provider exposes compile/decode/inspect through canonical composition.

Trusted base: accepted H0/H1-00 guarantees, normal documented .NET/JSON/collection behavior, exact Git checkout and pinned build/test toolchain. Unity inventory, editor serialization, native locator resolution, actual asset compatibility and materialization are outside this claim.

## Planned completeness / oracle strategy

The finite reference universe is defined by the explicit typed binding/component document model, not by a mutable registry supplied by the dependency derivation itself. Tests will walk the same structured object graph through an independent test oracle and compare it with emitted canonical/catalogue dependencies, including nested component documents and duplicate references. Provider-route completeness will be compared from the composed canonical inventory to both existing transport projections rather than from either adapter's own registry.

The bounded Potes facade probe will model a facade object referencing a market root canonical object plus logical prefab, material and animation clip. It checks hierarchy/reference/material/animation representability only; existence of those resources remains a later Unity-owned proof.

## Planned causal negative-conformance classes

- omit one structured canonical reference from HK02A dependency derivation;
- omit or mis-type one structured catalogue reference;
- accept duplicate/contradictory caller dependency claims;
- hide one accepted provider route from one transport projection;
- permit same-version semantic schema drift;
- leak a Unity runtime/editor type/reference into the portable contract graph.

Each material control must fail for the intended semantic reason and return GREEN after restoration.

## Proof budget

Prefer one small producer/provider surface plus focused tests and evidence. Reuse the accepted canonical composer and transport adapters; do not introduce a parallel registry, generic Unity reflection system, catalogue implementation, editor host or transport-specific implementation. Any proof machinery must map directly to a named H1-01 acceptance or realistic false-green class.
