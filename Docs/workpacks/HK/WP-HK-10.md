# WP-HK-10 — Strict quality closure

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-09`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Stress the accepted harness with broad malformed, conflicting and failure-case inputs so `WP-HK-GATE` rests on behavioural evidence rather than hand-picked examples. This remains repository-local software verification.

## Acceptance

- Property-based tests cover canonical serialization/hash, transaction atomicity, idempotency, query determinism and replay equivalence.
- Robustness testing covers malformed/truncated/unknown-version/unknown-command/schema-invalid inputs and never produces undefined public output.
- Mutation tests or equivalent defect-injection controls demonstrate that material validator/provenance/transaction guards are actually capable of catching seeded defects.
- Fault injection covers persistence interruption, thrown handler/validator failures, cancellation and restart/recovery.
- Concurrency tests exercise stale revisions and conflicting writers deterministically.
- Compatibility corpus locks accepted Protocol v1 behaviour and schema evolution rules.
- Seeded/random tests record seeds and minimize/reproduce failures.
- Test suite distinguishes harness defect from fixture/tool failure and fails closed when proof infrastructure is missing.
- Residual-risk audit explicitly names what is outside the boundary (for example arbitrary out-of-contract OS/toolchain behaviour) rather than pretending absolute certainty.
- The residual-risk audit is reconciled against an independently obtained inventory of residuals declared by accepted predecessor workpacks (`Docs/engineering/RESIDUAL_LEDGER.md`) rather than composed from this workpack's own reading, and every entry is resolved as inside the boundary with a seeded causal control, outside the boundary and named for the gate, or closed by cited accepted evidence.

## Required negative-conformance tests

At minimum seed controlled defects in each accepted foundational layer (protocol discovery, state/hash, inspection, transaction, validation, provenance/replay, host framing, batching/efficiency and capability containment) and prove the intended oracle goes red causally.

## Forbidden scope

Unity/graphics/gameplay testing, model-vendor benchmarking, production cloud load testing, penetration testing or external-system testing.

## DoD

All material H0 guarantees have strict negative-conformance evidence; zero known undetected classes inside the declared boundary; exact-SHA CI and independent PASS.
