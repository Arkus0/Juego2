# Foundational Proof Standard

Version: 1.0 — 2026-09-18

This standard binds every `HK-*` workpack through `WP-HK-GATE`.

Green CI is necessary but insufficient. A foundational candidate must show why its central architectural claim is complete enough to trust downstream work.

Required sequence:

```text
implementation
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

## Self-attack rule

For each material defect class that could leave CI green while the claim is false:

1. inject a defect;
2. demonstrate the intended guard/oracle turns red for the intended reason;
3. revert the defect;
4. demonstrate green again;
5. preserve exact commands and evidence.

A compile failure for an unrelated reason is not valid evidence.

## Effective-behaviour rule

When a claim concerns what a compiler, serializer, dispatcher, transport, validator or runtime actually consumes, prefer an oracle over the **evaluated/effective result** rather than a growing list of forbidden syntax. Static guards remain defence in depth.

## Freeze blockers

Do not freeze while any of these is true:

- an acceptance claim is UNKNOWN/PARTIAL;
- an enumerable inventory has unclassified entries;
- a material self-attack remains unexercised;
- a negative control fails to turn red;
- a known defect class inside the claim would escape detection;
- exact evidence and candidate SHA do not match;
- residual risk can still falsify the central claim.

## Reviewer duty

The independent Reviewer must reconstruct scope from the repository, challenge the Worker's completeness argument, inspect causal negative controls, and look for at least one omission class not highlighted by the Worker. Reviewer PASS names the exact candidate SHA and does not mean merely that Worker tests are green.
