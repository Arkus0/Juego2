# WP-HK-02A — Object-scoped extension data + typed dependencies

Status: COMPLETE
Class: FOUNDATIONAL
Depends on: `WP-HK-04` ✅ COMPLETE
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `5a07c55aeb79406a84bff579b34c09459707b713`
Implementation PR: `#25`

Completion:
- Reviewed candidate SHA: `f39a1994524c42213dafb63d440faaf9de7c040f`
- Independent Reviewer verdict: `PASS`, review `#5256593405`
- Final exact-SHA observation: GREEN, Actions `35456397714`
- Exact-SHA freeze validation: GREEN, Actions `35456445373`
- Focused controls: 11/11; full regression: 98/98; Release build: 0 warnings/errors (implementation run `35456331653`)
- Merge SHA: `ac7ce1180b462f093cf0ee02bbbd6f853938e27c`
- Completed: `2026-09-19`
- Review history: frozen `9d0dc3739f31bcc6a7f8df12b5f6e876837efacc` independently FAILed on global/object `"-"` fingerprint collision and missing multi-page dependency evidence; both corrected in the accepted candidate. Prior FAIL not retroactively superseded.
- Next dependency-valid WP: `WP-HK-05`.

## Why this workpack exists

The accepted HK02 model intentionally provides a small structural world plus opaque, world-global extension payloads. Modelling the approved H2 hero target showed a concrete downstream gap before HK05/HK06: NPC, placement, business and other per-object data has no canonical object anchor, and object references used by opaque extension semantics have no typed surface that validation can inspect.

This is an additive evolution at the current sequence point. It does not invalidate the accepted HK02–HK04 claims and does not amend their merged evidence. HK02A must preserve their guarantees while widening the state vocabulary through a separately reviewed candidate.

## Objective

Give every extension an optional canonical subject object and an explicit typed dependency-reference surface, then propagate that semantic shape through deterministic state identity, complete inspection and transactional mutation before validation diagnostics are built on top.

## Canonical semantics

- `WorldExtensionData` remains an opaque, namespaced/versioned byte payload.
- An extension may be global or may declare one `SubjectId` identifying the object whose data it extends.
- Its canonical identity is `(owner, schemaVersion, subjectId-or-global)`.
- An extension may declare zero or more typed dependency references `(kind, targetId)` whose targets must resolve in the same world.
- Any object identity semantically used by an opaque payload must also be declared on this typed dependency surface. Arkus validates the declaration; it does not pretend to parse arbitrary payload bytes.
- Global extensions remain supported for genuinely world-wide data.
- Whole-world revision/hash CAS remains unchanged. Object scoping improves semantic resource/change/rebase granularity; it does **not** claim automatic merging of concurrent disjoint writes.

## Acceptance

- The canonical model represents both global and object-scoped extensions without engine/runtime types.
- Extension identity/uniqueness is `owner + schemaVersion + subjectId-or-global`; two different subjects may carry the same owner/version.
- A non-null extension subject must resolve to a canonical object.
- Every typed extension dependency target must resolve to a canonical object and duplicate dependency edges are rejected.
- Removing an object fails candidate validation while any extension subjects or dependency references still point to it.
- Canonical schema/format version advances explicitly; deterministic serialization, hashing, ordering and round-trip identity cover subject and dependency semantics.
- Changing subject or any dependency changes canonical identity/hash; input ordering of extensions/dependencies remains non-semantic.
- HK03 extension discovery/read addressing exposes global versus subject identity and typed dependencies, remains bounded/deterministic, and can reconstruct the exact canonical hash.
- HK04 put/remove-extension operations address the composite identity and put carries typed dependencies; plan/dry-run/apply, CAS, idempotency and atomicity semantics remain unchanged.
- HK04 change sets identify the object-scoped extension resource and dependency additions/removals; payload bytes remain one field at that object-scoped resource.
- Existing global-extension call sites remain source-compatible where practical and retain their prior semantics.
- The accepted HK01 route universe and HK04 commit-authority boundary remain consumed, not re-proved.
- A bounded H2 Liébana content-shape probe demonstrates the claimed extension identity/dependency granularity and explicitly classifies adjacent time, behavior, asset and world-partition questions without adding those schemas here.

## Required negative-conformance tests

RED→GREEN for:

- serializer/hash omitting `SubjectId`;
- serializer/hash omitting a typed extension dependency;
- extension uniqueness still using only owner/version;
- dangling subject accepted;
- dangling or duplicate extension dependency accepted;
- HK03 reconstruction omitting subject/dependencies;
- HK04 extension key or remove operation ignoring subject;
- HK04 change coverage omitting dependency effects.

## Forbidden scope

- payload-specific schema interpreters or arbitrary payload introspection;
- a general ECS/component framework;
- concrete transform, schedule, NPC, business, Unity or gameplay schemas;
- per-resource concurrency/automatic merge semantics;
- HK05 public diagnostics, AI repair generation, HK06 journal/replay implementation.

## DoD

The widened extension semantics round-trip deterministically and remain completely inspectable/mutable through the accepted canonical surfaces; every required causal control is demonstrated, proof budget is within bounds, exact-SHA evidence is green and a fresh independent Reviewer issues PASS.
