# WP-HK-10 — Adversarial quality closure

Status: PLANNED  
Class: FOUNDATIONAL  
Depends on: `WP-HK-09`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Stress the accepted harness as a hostile client/runtime would, so `WP-HK-GATE` rests on broad behavioural evidence rather than hand-picked examples.

## Acceptance

- Property-based tests cover canonical serialization/hash, transaction atomicity, idempotency, query determinism and replay equivalence.
- Protocol fuzzing covers malformed/truncated/unknown-version/unknown-command/schema-invalid inputs and never produces undefined public output.
- Mutation tests or equivalent self-attacks demonstrate that material validator/provenance/transaction guards are actually capable of catching seeded defects.
- Fault injection covers persistence interruption, thrown handler/validator failures, cancellation and restart/recovery.
- Concurrency tests exercise stale revisions and conflicting writers deterministically.
- Compatibility corpus locks accepted Protocol v1 behaviour and schema evolution rules.
- Seeded/random tests record seeds and minimize/reproduce failures.
- Test suite distinguishes harness defect from fixture/tool failure and fails closed when proof infrastructure is missing.
- Residual-risk audit explicitly names what is outside the boundary (for example hostile OS/toolchain installation) rather than pretending absolute certainty.

## Required self-attacks

At minimum seed defects in each accepted foundational layer (protocol discovery, state/hash, inspection, transaction, validation, provenance/replay, host framing, batching/efficiency and safety) and prove the intended oracle goes red causally.

## Forbidden scope

Unity/graphics/gameplay testing, model-vendor benchmarking, production cloud load testing.

## DoD

All material H0 guarantees have adversarial evidence; zero known undetected classes inside the declared boundary; exact-SHA CI and independent PASS.
