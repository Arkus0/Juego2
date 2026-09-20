# WP-HK-10 Worker plan

WP: WP-HK-10 — Strict quality closure
Baseline SHA: 8651a7bd55180297c3621336e9e64a2e6a211aef
Active Worker: ChatGPT GPT-5.6 Sol
Worker state: ACTIVE

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-09B`.

- Reviewed candidate SHA: `8ed02586da9a5b6e159e1cdc76a47ae7ca89c763`.
- Independent PASS: PR #56 review `#5261068513`.
- Implementation merge SHA: `2e7a258fdcec2e26492d308c3cb199ab62201dcd`.
- Post-acceptance DocSync/main baseline: `8651a7bd55180297c3621336e9e64a2e6a211aef`.
- Exact-SHA accepted validation: Actions run `35522581043` GREEN.

### Inherited guarantees consumed by HK10

HK10 consumes rather than redesigns or redundantly re-proves these accepted guarantees:

1. `arkus.h0-resource-envelope@1` is the authoritative H0 resource envelope below transport-specific adapters.
2. Frozen `arkus.reference.jsonl@1` remains a 1,048,576-byte UTF-8 frame with `transport.frame_too_large` above that boundary; HK09B's canonical argument ceiling is 896 KiB beneath it.
3. The accepted coherent HK08A transaction shape is 96 operations; HK09B preserves that shape atomically rather than silently splitting it.
4. Portable depth is 32, decoded mutation payload is 512 KiB, query pages are at most 100 items, canonical world/snapshot state is at most 640 KiB, world resources are at most 10,000, local mutation transactions are at most 10,000 and snapshot-import receipts are at most 1,024.
5. The 5 s H0 execution budget is cooperative and publication-gating, not a hard real-time preemption guarantee.
6. Mutation/import/replay publish complete process-local aggregates only after authoritative budget/interruption checks; rejected/expired/interrupted work cannot truthfully publish partial canonical state or provenance/evidence.
7. Persistence durability is explicitly `process-local-checkpoint`; power-loss/WAL/fsync durability is not claimed.
8. HK08B same-lineage stale-plan recovery returns bounded, anchored recovery context and fails closed to bounded reinspection when exact lineage/history is unavailable.
9. HK09A keeps generic shell/process, ambient network and caller-selected filesystem authority outside the H0 public capability surface and enforces admission below conforming transports.

### Guarantees newly owned by HK10

HK10 owns closure evidence, not new product semantics:

- deterministic property-style coverage for canonical serialization/hash, transaction atomicity, idempotency, query determinism and replay equivalence;
- malformed/truncated/unknown-version/unknown-command/schema-invalid robustness with defined public outcomes;
- causal seeded defect controls across every accepted foundational layer named by the WP;
- persistence/handler/validator/cancellation/restart-recovery fault coverage;
- deterministic stale-revision/conflicting-writer and HK08B stale-recovery stress without automatic-merge or multi-agent claims;
- a bounded representative long-authoring-session measurement using accepted HK08A/HK08B primitives inside HK09B limits, including journal/session growth and observational process-memory telemetry;
- closure tests proving the broader suite introduces no alternate privileged host path beyond the accepted HK09A boundary;
- a Protocol v1 compatibility corpus;
- reproducible recorded seeds and fail-closed proof-infrastructure checks;
- independent reconciliation of the predecessor residual inventory in `Docs/engineering/RESIDUAL_LEDGER.md`.

### Concrete reopen condition

An accepted predecessor boundary is reopened only if HK10 produces concrete in-boundary evidence that the effective accepted path falsifies that predecessor guarantee — for example, a 96-operation accepted intent no longer fits atomically, interrupted publication changes authoritative state/evidence, HK08B recovery loses or misanchors accepted recovery context, a public H0 route gains forbidden ambient authority, or bounded-session evidence shows an accepted HK09B ceiling prevents the representative H0 workflow. A theoretical possibility or desire for duplicate defense-in-depth is not sufficient.

## Claim and trust boundary

HK10 claims that the already accepted H0 harness behaves deterministically and fails closed across the WP's finite quality-closure surfaces under repository-owned tests, controlled defects and bounded representative sessions. The default foundational trusted base remains exact Git checkout, pinned .NET/MSBuild/NuGet behavior, documented process/thread/filesystem/OS runner behavior and standard BCL/hash primitives.

Outside the claim are arbitrary toolchain/OS compromise, external penetration testing, distributed/multi-process writers, automatic merge, multi-agent throughput, production cloud load, engine/editor persistence, hard real-time preemption and power-loss durability.

## Planned implementation/evidence

1. Add deterministic property/robustness tests with explicit seeds and reproducible failure messages.
2. Add fault/concurrency/endurance tests reusing accepted canonical/session authorities; no replacement subsystem.
3. Add compatibility/capability-closure checks rooted in accepted canonical composition and HK09A admission.
4. Add a bounded Protocol v1 compatibility corpus owned by tests.
5. Add HK10 exact-SHA observation/verification entrypoints and causal negative-conformance controls; destructive controls run only in disposable copies and must demonstrate RED causally before GREEN.
6. Produce `PROOF_MATRIX.md`, `NEGATIVE_CONFORMANCE_MATRIX.md`, `ENDURANCE.md`, `COMPATIBILITY_CORPUS.md`, `RESIDUAL_RISK.md` and final Worker pre-review evidence.
7. Reconcile every entry in the independently maintained residual ledger as inside-HK10 with a causal control, outside boundary/named for gate, or already closed by cited accepted evidence.

## Proof-budget guard

No new public capability family, semantic authority, durability subsystem, concurrency model, load framework or architecture registry will be introduced merely to satisfy closure. If a material product-semantic gap appears, HK10 will record the causal predecessor reopen condition rather than special-case a test.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
