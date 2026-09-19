# WP-HK-02 — Canonical world state + deterministic identity

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-01` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `82ad746961145c2c779f5086cbc6bfde2b348c3c`
Implementation PR: `#17`

Completion:
- Reviewed candidate SHA: `f23fba9ab81566e682237183cf92618bbce504bd`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5255346033`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35436836349`)
- Merge SHA: `1605f922b74d4f0b71bba7896c311a914ffed3f7`
- Completed: `2026-09-19`

## Objective

Establish a canonical in-memory/persistable world representation with stable identity and deterministic state hashing, without prematurely encoding final gameplay systems.

## Acceptance

- Stable typed identifiers are independent of object references, scene instance IDs and file locations.
- World state has an explicit revision/version and deterministic canonical serialization.
- Same semantic state produces byte-stable canonical form and identical content hash across repeated runs on supported CI platforms.
- Round-trip serialize/deserialize preserves semantic state exactly.
- Referential integrity rules for the micro-world fixture are explicit.
- Ordering is canonical; dictionary/hash iteration order cannot change state hash.
- Unknown/forward-compatible data policy is explicit and tested.
- No generic untyped `Dictionary<string, object>` becomes the core domain model merely for convenience; extensibility must retain schema/type ownership.
- A tiny representative micro-world fixture exists only to exercise identities, containment/reference edges and mutation later; it must not commit the project to gameplay design.

## Required self-attacks

RED→GREEN for: unstable ordering, duplicate ID, dangling reference, hash change from serialization-order noise, hidden nonserialized state and unsupported schema/version payload.

## Forbidden scope

Final NPC/quest/shop/combat systems, Unity object model, assets, DFU, transport host.

## DoD

Canonical-state oracle and determinism tests pass from clean checkout; proof obligations zero; independent PASS.
