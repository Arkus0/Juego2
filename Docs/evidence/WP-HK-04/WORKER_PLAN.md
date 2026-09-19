# WP-HK-04 Worker plan

Baseline SHA: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`  
Worker: `ChatGPT / GPT-5.6 Sol (repair cycle 2)`

State: `ACTIVE`

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-03 — Inspection + query surface`.

- Reviewed candidate SHA: `8c20a380003c082fa9bd472d3233afa9654fb231`
- Independent Reviewer: `PASS` on PR #18.
- Implementation merge SHA: `d8b808450ee7863726d718a25c5756534113fcfd`
- Original HK04 dependency-valid baseline: `ecebd054821eeb388c3cf6ee4e389545d762b6d8`.
- Current `main` at repair-cycle start: `1f1805559c269b19c99a4b80eee638314024f601`; the only intervening change is PROCESS_ONLY terminology/context documentation, so no accepted predecessor semantic guarantee changed.
- HK03 proof remains `READY`, unresolved obligations `0`, known undetected defect classes `0`, proof budget `WITHIN_BUDGET`.

### Inherited guarantees consumed

1. HK02 owns the current finite canonical `WorldState` model, referential validation, immutable-by-copy semantics and deterministic canonical SHA-256 identity.
2. HK01 owns the single canonical capability definition/composition/discovery path and its independent production route universe.
3. HK03 owns complete bounded deterministic reads over current canonical state, explicit revision/hash anchors and side-effect-free inspection.
4. These guarantees remain consumed; no concrete evidence from either HK04 review invalidates them.

### Guarantees newly owned by HK04

- deterministic proposed plans/change sets before persistence;
- one semantic planning/validation path shared by plan, dry-run and apply;
- dry-run without persistence;
- atomic whole-state commit after complete candidate validation;
- optimistic expected revision + hash checks;
- explicit idempotency-key replay/conflict semantics;
- machine-readable affected resources/fields/references and evaluated conditions;
- no public command may obtain a hidden path that changes canonical authorable state outside the canonical transaction pipeline;
- mutation dispatcher surface == discovered canonical-mutation surface == transactional-handler surface.

### Reopen conditions

Reopen inherited HK01/HK02/HK03 only if concrete evidence shows that a public handler escapes canonical composition, a committed HK04 state cannot be represented by accepted HK02/HK03 semantics, or another accepted predecessor claim is factually false on the effective path.

## Circuit-breaker re-audit

Repair cycle 1 attempted to prove hidden-mutation completeness by structurally walking handler object graphs with `MutationAuthorityInspector`. The second independent FAIL on frozen SHA `dc81b054c5f2c6fdda7bf0da84e9b3f99a6457ce` showed that ordinary indirection through `List<>`, arrays or foreign-assembly holders can hide the same current public `TransactionalWorldAuthoringSession.Apply` authority from that walker.

This is the second FAIL in the same foundational class. Per `FOUNDATIONAL_PROOF_STANDARD.md`, cycle 2 does **not** add collection/array/wrapper traversal cases. The proof boundary is changed instead.

### Architecture decision

1. `TransactionalWorldAuthoringSession` will no longer expose `Apply` as public API. Public consumers may read `Current` and use `Plan`/`DryRun`; canonical commit is available only through the internal Authoring-owned `ICanonicalWorldMutationCommitter` friend boundary consumed by Runtime.
2. The current concrete session therefore ceases to be a public write capability. Wrapping it in `List<>`, an array or an arbitrary public holder cannot restore a public commit method.
3. `MutationSurfaceConformance` will return to the mechanical acceptance claim actually named by HK04: canonical mutation definitions, canonical-transaction policy, effective `ITransactionalMutationHandler` routes and dispatcher keys must agree. Structural object-graph inspection is not a completeness oracle.
4. Any retained `MutationAuthorityInspector` publication check is defence in depth only. It may recognize direct internal committer leakage, but proof/evidence will not claim it enumerates all shapes.
5. The causal hidden-mutation control will preserve the Reviewer’s exact ordinary-indirection shape: a `ReadOnly` external fixture holds the session behind a `List<TransactionalWorldAuthoringSession>`. It may invoke only public API. The control asserts no public `Apply` exists and that invoking the route cannot change revision/hash. If public `Apply` is reintroduced, this evaluated effect control turns RED regardless of the structural walker.

This resolves the causal class by capability closure rather than bypass enumeration. Hostile private reflection/runtime-toolchain subversion remains outside the standard trusted boundary; ordinary public C# access, including BCL containers, remains inside and is protected.

## Implementation boundary

`Arkus.Game.Authoring` owns engine-neutral mutation semantics and the authoritative session. `Arkus.Harness.Runtime` owns canonical binding/composition and is the only production friend assembly receiving the internal committer capability. `Arkus.Harness.Tests` may receive test-only friend access so existing direct transactional unit tests can exercise commit semantics without making commit public. The isolated external negative-control fixture is deliberately **not** a friend.

No Unity, filesystem writes, undo UI, gameplay-specific authoring, transport host or natural-language mutation work is introduced.

## Proof approach after re-audit

The finite claim universe remains the three HK04 public authoring routes, four generic operation kinds, the accepted HK02 state model and all effective public routes supplied by the inherited HK01 universe.

Independent/evaluated controls are:

1. HK01 independent route enumeration prevents public handlers from disappearing from the public capability universe.
2. HK04 mechanically reconciles discovered canonical mutations, transaction-policy declarations, effective transactional-handler route identities and dispatcher bindings.
3. Public API closure proves the authoritative session exports no public commit primitive; the ordinary-indirection fixture evaluates the exact missed `List<>` class by invoking only public API and comparing revision/hash.
4. Dry-run predicted canonical hash is compared with the actual accepted apply result.
5. Whole-state hash/inventory protects atomicity on later-invalid multi-operation requests.
6. `WorldMutationCoverage` independently diffs base/candidate semantics against declared change effects.

The existing atomicity, CAS, idempotency, dry-run/apply parity and change-coverage implementation should not be redesigned; both independent reviews found those boundaries sound.

## Proof-budget decision

`PROOF_BUDGET_VERDICT` remains conditional until implementation/pre-review, but the selected repair simplifies the completeness argument: it removes structural traversal from the proof claim and closes the public authority that made traversal necessary. Expanding `MutationAuthorityInspector` to understand more containers is explicitly rejected as whack-a-mole.
