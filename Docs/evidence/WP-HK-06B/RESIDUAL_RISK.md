# WP-HK-06B trust boundary and residual risk

## Claim boundary

HK06B claims deterministic semantic comparison of the current accepted authored `WorldState` resource model and a separately versioned, fail-closed snapshot artifact that can reconstruct the identical canonical authored hash in the current single-process H0 session.

The semantic resource universe is finite and inherited from HK02/HK02A: canonical objects identified by object ID, with type/container/references, and opaque extensions identified by owner/schema-version/subject, with payload/dependencies. World identity/schema are compatibility gates. Revision/hash are lineage/anchor metadata: snapshot export/import preserves them exactly, while a revision-only difference is not reported as an authored-resource field change.

Successful snapshot import is a **canonical session rebase**, not an HK04/HK06A canonical mutation. It replaces the complete inner authored session only after artifact/state/anchor validation and optimistic current-state reconciliation, starts a new local mutation lineage at the imported state, emits schema-bound rebase evidence, and keeps snapshot-import idempotency receipts separate from HK06A mutation provenance so no mutation history is invented.

## Trusted / inherited base

Per `FOUNDATIONAL_PROOF_STANDARD.md`, pinned .NET/MSBuild/NuGet behavior, Git checkout/object semantics, normal CI/runner/process-memory behavior and SHA-256 used according to contract remain trusted.

Accepted predecessor guarantees consumed:

- HK01: one canonical composed capability inventory and effective route/discovery completeness;
- HK02/HK02A: finite canonical authored-state fields, identities, validation and deterministic canonical codec/hash;
- HK03: state-neutral inspection;
- HK04: canonical **mutation** authority, atomic mutation commit, CAS and keyed idempotency semantics for ordinary authored mutations;
- HK05: complete candidate validation and stable structured rejection;
- HK06A: truthful mutation journal, explicit lineage anchor and authored/live separation.

Concrete evidence of a material canonical field outside HK02/HK02A identity/codec, an ordinary canonical mutation outside HK04 authority, invalid state accepted by the relevant validation boundary, or false HK06A mutation-journal evidence would justify reopening the predecessor boundary. None was observed. HK06B itself owns the distinct snapshot-rebase writer seam introduced here.

## In-boundary residuals

None known after repair-cycle Worker pre-review. The original extension-existence proof gap and the independent Reviewer's classification ↔ authority ↔ history/evidence blocker are both covered by causal controls.

## Non-blocking residuals outside HK06B

- **Durability/recovery:** snapshots and rebase-idempotency receipts/evidence storage are process-local in H0. Durable storage, crash recovery and cross-process receipt persistence are not claimed.
- **Replay/compatibility:** HK06C owns deterministic journal replay and the policy for replaying accepted journal/snapshot versions. HK06B only validates its own snapshot version and establishes a clean imported base; it does not replay or reinterpret HK06A entries.
- **Transport framing:** files, JSONL, MCP, network/cloud persistence and external framing belong to later adapters and cannot redefine snapshot semantics.
- **Gameplay/runtime state:** transforms, clocks, schedules, physics, animation, AI and simulation state remain outside canonical authored `WorldState` unless a later reviewed WP promotes them.
- **History migration:** import deliberately starts `new-local-lineage`; it does not merge source and target journals. A future history-transfer feature would require a new explicit contract.
- **Large-world interaction budget:** whole-state snapshot payloads and full semantic comparisons are acceptable for H0; streaming/chunking/compact interaction belongs to later ergonomics/performance work.
- **Trusted-infrastructure failure:** arbitrary out-of-contract runtime, memory, hash, toolchain or runner behavior is not recursively proved.

## Proof-budget assessment

Product machinery remains limited to one semantic comparator, one snapshot artifact parser/exporter, one narrow canonical-rebase authority, keyed rebase receipts/evidence, and the minimum canonical metadata support required to describe that writer truthfully. HK04 mutation conformance was not weakened and no route-specific exemption was added. Proof additions map directly to HK06B acceptance, the extension-existence proof gap, and the independent Reviewer's rebase-seam blocker. No replay engine, second mutation system, second state model, transport framework or duplicate predecessor validator was added.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
