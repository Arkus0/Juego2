# WP-HK-06B trust boundary and residual risk

## Claim boundary

HK06B claims deterministic semantic comparison of the current accepted authored `WorldState` resource model and a separately versioned, fail-closed snapshot artifact that can reconstruct the identical canonical authored hash in the current single-process H0 session.

The semantic resource universe is finite and inherited from HK02/HK02A: canonical objects identified by object ID, with type/container/references, and opaque extensions identified by owner/schema-version/subject, with payload/dependencies. World identity/schema are compatibility gates. Revision/hash are lineage/anchor metadata: snapshot export/import preserves them exactly, while a revision-only difference is not reported as an authored-resource field change.

Successful snapshot import is a session rebase. It replaces the complete inner authored session only after artifact/state/anchor validation and optimistic current-state reconciliation, starts a new local HK06A lineage at the imported state, and keeps snapshot-import idempotency receipts separate from mutation provenance so no history is invented.

## Trusted / inherited base

Per `FOUNDATIONAL_PROOF_STANDARD.md`, pinned .NET/MSBuild/NuGet behavior, Git checkout/object semantics, normal CI/runner/process-memory behavior and SHA-256 used according to contract remain trusted.

Accepted predecessor guarantees consumed:

- HK01: one canonical composed capability inventory and effective route/discovery completeness;
- HK02/HK02A: finite canonical authored-state fields, identities, validation and deterministic canonical codec/hash;
- HK03: state-neutral inspection;
- HK04: canonical mutation authority, atomic mutation commit, CAS and keyed idempotency semantics;
- HK05: complete candidate validation and stable structured rejection;
- HK06A: truthful mutation journal, explicit lineage anchor and authored/live separation.

Concrete evidence of a material canonical field outside HK02/HK02A identity/codec, a public writer outside accepted mutation authority, invalid state accepted by HK05, or false HK06A journal evidence would justify reopening the predecessor boundary. None was observed.

## In-boundary residuals

None known after Worker pre-review and the extension-existence proof repair.

## Non-blocking residuals outside HK06B

- **Durability/recovery:** snapshots and import-idempotency receipts are process-local in H0. Durable storage, crash recovery and cross-process receipt persistence are not claimed.
- **Replay/compatibility:** HK06C owns deterministic journal replay and the policy for replaying accepted journal/snapshot versions. HK06B only validates its own snapshot version and fails closed on unsupported versions.
- **Transport framing:** files, JSONL, MCP, network/cloud persistence and external framing belong to later adapters and cannot redefine snapshot semantics.
- **Gameplay/runtime state:** transforms, clocks, schedules, physics, animation, AI and simulation state remain outside canonical authored `WorldState` unless a later reviewed WP promotes them.
- **History migration:** import deliberately starts `new-local-lineage`; it does not merge source and target journals. A future history-transfer feature would require a new explicit contract.
- **Large-world interaction budget:** whole-state snapshot payloads and full semantic comparisons are acceptable for H0; streaming/chunking/compact interaction belongs to later ergonomics/performance work.
- **Trusted-infrastructure failure:** arbitrary out-of-contract runtime, memory, hash, toolchain or runner behavior is not recursively proved.

## Proof-budget assessment

Product machinery is limited to one semantic comparator, one snapshot artifact parser/exporter, one narrow session-rebase wrapper and keyed import receipts required by the inherited canonical-mutation contract. Proof additions map directly to HK06B acceptance and one pre-review-discovered false-green class (extension existence omission). No replay engine, second state model, semantic registry, transport framework or duplicate predecessor validator was added.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
