# WP-HK-10 — Strict quality closure

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-09B` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `8651a7bd55180297c3621336e9e64a2e6a211aef`
Implementation PR: `#60`

Completion:
- Reviewed candidate SHA: `813ccf08e33fdd77a34c59da5ed766882840cbee`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5261301248`
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35527218067`)
- Merge SHA: `f893ad51d756090eeecac41b8fc7cb14f8bd359a`
- Completed: `2026-09-20`
- Repair cycles: `1`; prior frozen SHA `dabcbeb9cce071ed6fca29a8fa01030127f2ce98` failed review `#5261225652` because a dispatcher-failure semantic gap discovered by closure was initially owned by HK10 instead of reopening the causal HK01 contract. The accepted repair amends HK01 and makes HK10 consume that owner-defined behavior.

## Objective

Stress the accepted harness with broad malformed, conflicting and failure-case inputs so `WP-HK-GATE` rests on behavioural evidence rather than hand-picked examples. This remains repository-local software verification.

## Acceptance

- Property-based tests cover canonical serialization/hash, transaction atomicity, idempotency, query determinism and replay equivalence.
- Robustness testing covers malformed/truncated/unknown-version/unknown-command/schema-invalid inputs and never produces undefined public output.
- Mutation tests or equivalent defect-injection controls demonstrate that material validator/provenance/transaction guards are actually capable of catching seeded defects.
- Fault injection covers persistence interruption, thrown handler/validator failures, cancellation and restart/recovery.
- Concurrency tests exercise stale revisions and conflicting writers deterministically, including the accepted HK08B stale-plan recovery path; H0 does not claim automatic merging or multi-agent writer throughput.
- A bounded long-authoring-session endurance case repeatedly exercises representative inspect/validate/mutate/journal/snapshot flows using the accepted HK08A/HK08B interaction primitives and budgets inside the accepted HK09B resource envelope, and records process/session growth. Journal/session growth must remain within that declared H0 envelope for the bounded case; if it does not, the failure is resolved explicitly rather than assuming future compaction will save the gate.
- Capability-closure tests consume the accepted HK09A authority boundary and prove the broader quality suite does not introduce an alternate shell/network/filesystem or transport-specific privileged path.
- Compatibility corpus locks accepted Protocol v1 behaviour and schema evolution rules.
- Seeded/random tests record seeds and minimize/reproduce failures.
- Test suite distinguishes harness defect from fixture/tool failure and fails closed when proof infrastructure is missing.
- Residual-risk audit explicitly names what is outside the boundary (for example arbitrary out-of-contract OS/toolchain behaviour) rather than pretending absolute certainty.
- The residual-risk audit is reconciled against an independently obtained inventory of residuals declared by accepted predecessor workpacks (`Docs/engineering/RESIDUAL_LEDGER.md`) rather than composed from this workpack's own reading, and every entry is resolved as inside the boundary with a seeded causal control, outside the boundary and named for the gate, or closed by cited accepted evidence.

## Required negative-conformance tests

At minimum seed controlled defects in each accepted foundational layer (protocol discovery, state/hash, inspection, transaction, validation, provenance/replay, host framing, HK08A batching/efficiency, HK08B stale-conflict recovery/repair, HK09A capability containment and HK09B resource/persistence enforcement) and prove the intended oracle goes red causally. Include a stale-conflict recovery defect that loses/misanchors HK08B recovery context and a bounded-session/resource-growth defect that exceeds an accepted HK09B limit without being detected.

## Closure-only rule

HK10 is a closure workpack, not a new architecture workpack. It may add the minimum test/proof machinery needed to challenge accepted guarantees, but it must not silently invent new product semantics, new public capability families or replacement subsystems merely to make a seeded test pass. A material product-semantic gap found here reopens or amends the causal owning workpack rather than being buried inside HK10 hardening.

## Accepted closure result

The accepted candidate demonstrates twelve deterministic causal RED controls across the complete named foundational layer set, a frozen Protocol v1 compatibility corpus, bounded 512-transaction endurance with restart/import recovery inside HK09B limits, deterministic stale/conflicting writer recovery, cancellation and thrown-handler/validator fault injection, and an approved Juego2 Potes content-shape probe. `Docs/evidence/WP-HK-10/RESIDUAL_RISK.md` reconciles the independently maintained residual ledger with `UNCLASSIFIED_RESIDUALS: 0`; `PROOF_MATRIX.md` records `UNRESOLVED_PROOF_OBLIGATIONS: 0` and `KNOWN_UNDETECTED_DEFECT_CLASSES: 0`.

The only product-semantic gap discovered by closure was the canonical thrown-handler failure boundary. Consistent with this workpack's closure-only rule, the accepted candidate reopens/amends `WP-HK-01` as causal owner rather than treating the behavior as HK10 architecture. The HK01 amendment and HK10 closure were independently accepted together on the exact frozen SHA above.

## Forbidden scope

Unity/graphics/gameplay testing, model-vendor benchmarking, production cloud load testing, distributed/multi-agent load testing, penetration testing or external-system testing; new product subsystems unrelated to closing a concrete accepted H0 guarantee.

## DoD

All material H0 guarantees have strict negative-conformance evidence; the accepted HK08B stale-conflict recovery path, HK09A capability boundary, HK09B resource/persistence enforcement and bounded long-session behaviour survive closure testing; zero known undetected classes inside the declared boundary; exact-SHA CI and independent PASS.
