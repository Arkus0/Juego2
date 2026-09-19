# WP-HK-06A trust boundary and residual risk

## Claim boundary

HK06A claims truthful deterministic authored mutation provenance for the current finite,
single-process `TransactionalWorldAuthoringSession` and accepted composed H0 capability surface.
The journal lineage begins at an explicit initial authored-state anchor. Each entry after that anchor
represents one new canonical state published by the accepted apply path.

Authored state means the accepted canonical `WorldState`: world identity/schema, revision, objects,
references and extension data/dependencies covered by its canonical codec/hash. Runtime observation
means transient data outside that model. The only runtime artifact introduced is a separately named
stamp carrying the authored base; the bounded scheduled-NPC object is test-owned.

## Trusted / inherited base

Per `FOUNDATIONAL_PROOF_STANDARD.md`, normal pinned .NET/MSBuild/NuGet behavior, Git checkout/object
semantics, filesystem/runner behavior and SHA-256 used according to contract remain trusted.

Accepted predecessor guarantees consumed:

- HK01: canonical composed definition/route/discovery completeness;
- HK02/HK02A: finite canonical authored state, identity, deterministic codec/hash and dependency data;
- HK03: state-neutral inspection;
- HK04: sole current commit authority, atomic plan/apply boundary, CAS, idempotency and complete
  semantic change set;
- HK05: complete pre-commit candidate validation and structured rejection.

Concrete evidence of a public writer outside HK04, a false persisted result, a state field outside
HK02 hash identity or an invalid HK05 commit would reopen that predecessor. No such evidence was
found.

## In-boundary residuals

None known after implementation challenge and causal controls.

The session publishes `WorldState`, idempotency receipts and journal entries through one immutable
holder reference under the existing commit lock. Entry construction, continuity checks and result
hash recomputation complete before publication; a mismatch returns structured failure and does not
publish either state or provenance.

## Non-blocking residuals outside HK06A

- **Process durability:** state, receipts and journal share the current in-memory session lifetime.
  Durable storage/recovery is not claimed; future persistence must atomically preserve the same
  aggregate boundary.
- **Snapshot/history interaction:** HK06B must state whether imported snapshots start a new local
  lineage or retain external evidence. HK06A does not fabricate/import history.
- **Replay/compatibility:** HK06C owns interpretation, missing/reordered/tampered entry behavior and
  compatibility across journal/snapshot versions. HK06A owns the artifact it will consume.
- **Semantic diff:** affected-resource identity is journal metadata, not HK06B field-level semantic
  diff.
- **Journal growth/pagination:** H0 exposes the complete session-local journal in one deterministic
  response. Compact/paged interaction budgets belong to HK08.
- **Runtime observation content:** clock sources, schedules, positions, physics, animations, AI and
  simulation determinism are not modeled or validated. Future observation routes must use the
  separate stamp contract and remain outside authored commit authority.
- **Transport/storage framing:** JSONL, MCP, files and databases are later adapters and cannot
  redefine journal meaning.
- **Arbitrary trusted-infrastructure failure:** out-of-contract runtime/memory/hash/toolchain
  behavior is not recursively proved.

## Proof-budget assessment

The implementation adds one journal contract/read binding and integrates one entry construction
into the existing commit section. One immutable holder publishes current state, receipts and journal
together, which simplifies the atomicity argument. The tests use one bounded transition audit
and one runtime-boundary oracle tied directly to required defect classes. No generalized event
store, replay engine, simulation framework or redundant predecessor verifier exists.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
