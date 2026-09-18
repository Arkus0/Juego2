# Foundational Proof Standard

Version: 1.1 — 2026-09-18

This standard binds every `HK-*` workpack through `WP-HK-GATE`.

Green CI is necessary but insufficient. A foundational candidate must show why its central architectural claim is complete enough to trust downstream work.

Required sequence:

```text
implementation
→ independently defined/evaluated universe where completeness is claimed
→ mechanically enumerable inventory where possible
→ proof-obligation matrix
→ causal self-attacks / negative controls
→ independent or evaluated oracle where self-confirmation is possible
→ residual-risk audit
→ exact-SHA CI
→ freeze
→ fresh independent review
```

Each WP evidence must contain:

```text
FOUNDATIONAL_PROOF_VERDICT: READY | NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: <integer>
KNOWN_UNDETECTED_DEFECT_CLASSES: <integer>
```

`READY` requires both counts to be zero.

## Proof matrix

For every acceptance criterion record:

| Proof obligation | Completeness argument | Positive evidence | Negative control / attack | Result | Residual risk |
|---|---|---|---|---|---|

Representative happy-path tests are not a completeness argument.

## Independent-universe rule

A completeness proof is invalid if the same registry, manifest, root list, dispatcher list, validator list or configuration under test can silently shrink the universe that the proof then declares complete.

When the claim is “all X are classified/discovered/validated/compiled/exposed”, the candidate must identify how the universe of X is obtained independently of the classification/discovery/validation declaration being checked, or demonstrate an effective/evaluated oracle that makes omission observable.

Examples:

- repository/project/source completeness begins from an independent repository/effective-build universe, not only a manifest-declared scan root;
- command discovery completeness cannot use discovery itself as the only command inventory;
- validator completeness cannot use only the validator registry to decide which invariants exist;
- transport/adaptor completeness cannot use only one adapter's exported surface as the canonical capability inventory.

If deleting, relocating, unregistering or hiding a material object can make both the object and the proof obligation disappear while CI stays green, the proof boundary is self-shrinking and the central claim is false.

## Self-attack rule

For each material defect class that could leave CI green while the claim is false:

1. inject a defect;
2. demonstrate the intended guard/oracle turns red for the intended reason;
3. revert the defect;
4. demonstrate green again;
5. preserve exact commands and evidence.

A compile failure for an unrelated reason is not valid evidence.

Attacks should target omission classes, not merely syntax variants. If multiple bypasses share one causal class, fix/prove the class rather than accumulating case-specific checks.

## Effective-behaviour rule

When a claim concerns what a compiler, serializer, dispatcher, transport, validator or runtime actually consumes, prefer an oracle over the **evaluated/effective result** rather than a growing list of forbidden syntax. Static guards remain defence in depth.

Independent discovery and effective evaluation are complementary: an evaluated oracle proves what a known participant actually did, while an independent-universe oracle proves that no material participant disappeared before evaluation.

## External-component rule

Adopting a library/framework transfers implementation work, not Arkus proof obligations. For every external component on a foundational path, record:

- exact version/upstream identity and license;
- which Arkus guarantee it helps implement;
- which Arkus guarantees remain outside its scope;
- a conformance/replacement boundary so the dependency cannot silently become the product's semantic authority.

Do not claim an upstream project's tests or popularity as evidence of Arkus completeness.

## Freeze blockers

Do not freeze while any of these is true:

- an acceptance claim is UNKNOWN/PARTIAL;
- an enumerable inventory has unclassified entries;
- the universe behind a completeness claim can self-shrink without detection;
- a material self-attack remains unexercised;
- a negative control fails to turn red;
- a known defect class inside the claim would escape detection;
- exact evidence and candidate SHA do not match;
- residual risk can still falsify the central claim.

## Circuit breaker

Two independent Reviewer FAILs exposing the same foundational defect class require architecture re-audit before another repair cycle.

A Reviewer finding that demonstrates a self-shrinking proof universe, circular completeness oracle or equivalent false proof boundary triggers architecture re-audit immediately; do not spend another cycle adding a case-specific exception first.

## Reviewer duty

The independent Reviewer must reconstruct scope from the repository, challenge the Worker's completeness argument, inspect causal negative controls, and look for at least one omission class not highlighted by the Worker. Reviewer PASS names the exact candidate SHA and does not mean merely that Worker tests are green.
