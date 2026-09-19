# WP-HK-06A Worker plan

Baseline SHA: `bc6241d2db2b6e15f1a9ac78c673f7ef0735ffff`
Worker: `ChatGPT / Codex`
Governing process: `FOUNDATIONAL_PROOF_STANDARD.md` v1.3 and `WORKER_REVIEW_PROTOCOL.md` v1.6
State: `ACTIVE`

No prior HK06A Worker branch or implementation PR existed at baseline. This cycle owns branch
`wp/hk-06a-provenance-journal` and only the write set required by `WP-HK-06A`.

The local Worker substrate does not currently provide the pinned .NET SDK. Missing local execution
is not treated as green: implementation iterations will use the repository's standard hosted
exact-SHA observation workflow, and the candidate cannot freeze without a compliant GREEN receipt.

## PREDECESSOR_CONTRACT_CHECK

Accepted direct predecessor: `WP-HK-05 — Validation + repairable diagnostics`.

- Predecessor baseline SHA: `dbd8121411079f22a01d5cb85345e180ff41f7e2`.
- Reviewed candidate SHA: `23a9fd4373a803187cd9391b1459cd48975177f6`.
- Independent Reviewer: `PASS`, PR `#26`, review `#5257350871`.
- Exact-SHA candidate observation: Actions `35464742544` GREEN.
- Exact-SHA freeze validation: Actions `35464834162` GREEN.
- Implementation merge SHA: `ed65661680aea2a9be79f892c96aa42bf788a842`.
- DocSync merge SHA: `2be48337721406eb77b6b66b31f7fc738d9ba08f`.
- HK06A split/adoption baseline on current `main`: `bc6241d2db2b6e15f1a9ac78c673f7ef0735ffff`.

### Inherited guarantees consumed

1. HK01 owns the composed canonical capability/schema inventory, public route completeness and
   discovery/dispatcher reconciliation. HK06A adds one canonical read surface to that inventory; it
   does not create a journal-owned command registry.
2. HK02/HK02A own the finite canonical authored `WorldState`, stable identities, deterministic
   serialization/content hash, revision semantics, object-scoped extensions and typed declared
   dependencies.
3. HK03 owns side-effect-free inspection of accepted authored state. HK06A does not rebuild the
   inspection universe.
4. HK04 owns the closed canonical commit authority, plan/dry-run/apply parity, atomic state
   replacement, whole-world revision/hash CAS, session-lifetime idempotency and complete semantic
   change sets. HK06A composes journal append at that accepted commit boundary rather than
   introducing a second writer or re-proving route/commit closure.
5. HK05 owns complete candidate validation, deterministic structured diagnostics and rejection of
   invalid state before commit. Rejected candidates remain non-persisted and therefore cannot
   create successful mutation-journal entries.

### Guarantees newly owned by HK06A

- an Arkus-owned, versioned, discoverable machine journal schema and deterministic entry identity;
- one journal entry per newly persisted authored mutation, appended in commit order at the HK04
  transaction boundary;
- entry truthfulness for canonical request identity/envelope, capability version, base/result
  authored revision+hash and the exact affected-resource set;
- no successful mutation entry for plan, dry-run, reads, validation, rejected/failed apply or an
  idempotent retry that does not persist a new state;
- an explicit journal base anchor plus the normalized accepted mutation envelope needed for HK06C
  to replay later without redefining HK06A's artifact format;
- a named runtime-observation stamp contract carrying an authored base anchor, while runtime-only
  observation values remain outside `WorldState`, authoring revision/hash and journal authority;
- a bounded test-owned scheduled-NPC observation surrogate proving that transient steps can change
  without authored-state or journal churn and that the oracle catches a surrogate granted commit
  authority.

### Guarantees intentionally not re-proved

HK06A will not duplicate HK01 route-discovery proof, HK02 codec/hash completeness, HK03 inspection
completeness, HK04 hidden-write/transaction/CAS/idempotency proof or HK05 invariant completeness.
It will test only the concrete integration seam: the accepted apply path appends truthful provenance
atomically, and every non-persisting accepted path leaves that journal unchanged.

### Reopen conditions

Reopen an inherited guarantee only if concrete evidence shows an effective public mutation can
persist outside the accepted HK04 boundary, HK04 can report `persisted=true` without the accepted
candidate being current, an accepted state field is absent from HK02 hash identity, or HK05 permits
an invalid candidate to commit. A theoretical alternate route, unsupported reflection/toolchain
behavior or desire for duplicate defence-in-depth is not a reopen condition.

## Architecture boundary

The authoritative session remains the sole owner of canonical authored state. HK06A adds an
immutable session-local journal lineage with an explicit initial authored anchor. A commit prepares
the candidate state, normalized journal entry and next journal collection before publishing them
inside the existing commit lock. Readers observe state+journal under the same lock.

Each entry records the full normalized mutation envelope as replay evidence, but HK06A does not
interpret or replay it. HK06B will decide snapshot/history interaction, and HK06C will consume this
schema to define compatibility and replay behavior.

Runtime observations use a separately named stamp containing an `authoredBase` anchor. They do not
reuse journal entries as observations and receive neither the mutation service nor internal commit
authority. The content-shape surrogate owns only synthetic step/position fields in test memory.

## Proof approach

1. Exercise a nontrivial two-commit micro-world sequence through the composed public contract and
   compare every effective journal field against independently captured before/after states and an
   independently derived affected-resource set.
2. Re-run the same sequence in a clean second session and require identical ordering and entry IDs.
3. Invoke plan, dry-run, current/proposed validation, inspection, malformed/rejected/stale apply and
   idempotent retry, proving journal count/content changes only for newly persisted state.
4. Validate the effective journal result against the schema discovered through `system.describe`.
5. Use bounded test-local defect injections for missing entries, wrong anchors, wrong affected
   resources, false non-persisted success and runtime-surrogate commit authority, demonstrating the
   intended audit oracle turns RED for the causal reason and GREEN on production output.
6. Run a Potes/Liébana plaza + bar/shop + scheduled-NPC-shaped probe. The NPC observation surrogate
   advances transient step/position values while authored state, hash, revision and journal bytes
   remain identical.

## Forbidden scope preserved

No semantic diff, snapshot export/import, replay engine, gameplay scheduler/clock/AI, deterministic
simulation claim, Unity serialization, GUI history browser, cloud persistence or Git-as-history
authority is introduced.
