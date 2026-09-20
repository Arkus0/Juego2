# WP-HK-08B Worker plan

Baseline SHA: `dc8b6ee3d4ea62df70e28605268526c726de69d8`
Branch: `wp/hk-08b-conflict-recovery`
Worker state: ACTIVE

## Objective

Implement HK08B only: structured stale whole-world CAS recovery, complete diagnostic-preserving repair ergonomics, and a measured public-client interaction benchmark that consumes accepted HK08A batching/compact/pagination primitives. Preserve the accepted HK04 transaction authority, HK05 complete diagnostics, HK06A provenance truth, HK06B rebase lineage boundary and HK07A/HK07B transport neutrality.

HK08B will not add automatic merge, per-resource locking/CAS, distributed transactions, multi-process writer coordination, autonomous multi-agent scheduling, model-vendor prompting, natural-language orchestration, gameplay, or speculative multi-plan atomicity.

## PREDECESSOR_CONTRACT_CHECK

Direct accepted predecessor: `WP-HK-08A`.

- Reviewed frozen candidate: `324102d40fa0b7e36c7320216914f9d3fddadcc8`.
- Independent review: PASS (`#5260092080`).
- Exact-SHA freeze validation: Actions `35499569973` GREEN; artifact `10601679693`.
- Implementation merge SHA: `eb9d3df58df1114abafaca435da64683fdb1480e`.
- Post-PASS DocSync: PR `#51`; current baseline `dc8b6ee3d4ea62df70e28605268526c726de69d8`.

Inherited guarantees consumed rather than re-proved:

1. HK04 owns the sole ordinary canonical mutation commit authority, whole-world revision+hash CAS, atomicity and idempotency semantics.
2. HK05 returns the complete deterministic set of independently reportable validation diagnostics; HK08B may prioritize/group for repair but must never suppress that accepted set.
3. HK06A journal entries bind each accepted mutation to base/result authored anchors and a deterministic `affectedResources` set.
4. HK06B snapshot import is a canonical rebase that creates `new-local-lineage`, swaps in a fresh transactional session and resets the local HK06A journal; pre-import history is therefore not proof of ancestry in the new lineage.
5. HK08A preserves complete `authoring.journal.read@1.0`, adds explicitly versioned bounded `@2.0`, proves a representative 96-operation coherent edit remains one HK04 transaction, and proves changed interaction shapes equivalent through JSONL and MCP.
6. HK07A/HK07B keep canonical meaning above transports: JSONL and MCP project the same neutral/canonical result and may not invent recovery semantics independently.

Predecessor reopen trigger: concrete effective evidence that one of those guarantees does not hold on the HK08B path. Defensive duplication alone is not a reopen condition.

## Recovery truth contract — frozen before budgets

Structured recovery is attached to stale `authoring.change.plan`, `authoring.change.dry-run` and `authoring.change.apply` outcomes at the canonical public-route boundary. Retry remains an ordinary new plan/dry-run/apply against the returned current anchor; recovery never commits, merges or mutates.

Every stale recovery context contains:

- `schemaId = arkus.world-conflict-recovery@1`;
- `expected`: the request's exact `expectedRevision` + `expectedHash`;
- `current`: authoritative `worldId`, state schema version, current revision and current hash;
- `disposition`: exactly `same-lineage-replan` or `bounded-reinspection-required`;
- `historyProof`: stable machine-readable reason and the journal-lineage base used by the proof;
- `changedResources`: deterministic sorted union of material HK06A `affectedResources` after the expected base when and only when recoverability is proven;
- `currentResources`: deterministic current presence/read descriptors for those changed resources when and only when recoverability is proven.

### Recoverable same-lineage condition

The expected revision+hash is recoverable iff all are true:

1. it equals either the current local journal base or the result anchor of one entry in the current complete local HK06A journal;
2. every journal transition from that anchor to current is present, sequence-contiguous and anchor-contiguous (`entry[i].base == previous result`, then final result == current);
3. world identity and state schema identity remain consistent across the proven chain.

The proof is positive: matching revision alone, matching hash alone, an older revision outside the current local journal, a broken/gapped chain, or a pre-rebase anchor is insufficient.

### Required-history failure / lineage fallback

If ancestry cannot be positively proven, or any required transition is unavailable/inconsistent, recovery returns `bounded-reinspection-required`, an empty `changedResources/currentResources` delta, and a stable reason such as `expected-base-not-in-current-lineage` or `required-history-unavailable`. It must not fabricate an approximate delta.

A successful HK06B snapshot import is the causal lineage negative: because it establishes a fresh journal base, an old pre-import expected anchor cannot be treated as an ancestor merely because its revision is lower.

### Current-resource information

Recovery does not require a full-world reload. For a proven same-lineage stale conflict it identifies only the union of resources materially affected since the expected base and supplies current presence plus a canonical bounded inspection descriptor for each resource. The client may reinspect only those resources before rebuilding its mutation. Removed resources are represented explicitly as absent rather than causing discovery of the whole world.

## Diagnostic repair policy

HK05's complete `diagnostics` collection remains authoritative and unchanged. HK08B benchmark/client ergonomics may compute a stable likely-first/grouped repair view from that complete collection, but the original set remains retrievable and every independent diagnostic must remain represented. No validation contract version is silently repurposed.

## Benchmark contract

Budgets are set only after the recovery-truth implementation is green. The reference micro-world exercises public canonical routes only and records request count, serialized response bytes and elapsed harness time for:

1. create;
2. bounded/compact inspect;
3. coherent multi-resource modify;
4. invalid request -> complete diagnostics -> repair;
5. same-lineage stale rejection -> structured recovery -> affected-only reinspection -> explicit re-plan/dry-run/apply success.

The stale workflow must not use a full-world enumeration/reload. A coherent multi-resource edit remains one mutation transaction. Baseline thresholds will be derived from measured evidence with explicit bounded headroom, not chosen to excuse a failing recovery proof.

## Causal negative-conformance plan

RED→GREEN controls will cover:

- opaque stale error that forces whole-world reconstruction;
- wrong expected/current anchors;
- omission of one material resource from the journal-derived changed-resource union;
- non-ancestor/pre-rebase expected base falsely classified as recoverable;
- unavailable/gapped history falsely classified as recoverable;
- recovery retry attempting a merge/alternate mutation path or bypassing HK05/HK06A;
- repair prioritization that drops an independent HK05 diagnostic;
- benchmark path using a private/non-public surface or omitting one representative flow;
- full-world reload/per-field chattiness or request/response regression above the recorded budget;
- JSONL/MCP drift in HK08B recovery context/disposition.

## Proof/trust boundary

Inside the claim: repository-owned recovery decoration at canonical public mutation routes, current local HK06A lineage/journal truth, current authored-state anchor, deterministic affected-resource derivation, public reference/MCP projections, benchmark client and causal controls.

Trusted base: accepted predecessor semantics listed above, exact Git/.NET/MSBuild behavior, normal process/stdio primitives, documented MCP SDK behavior at the accepted HK07B boundary and normal cryptographic/hash primitives.

Outside claim: durable history across process restart, distributed writers, hostile-client authorization, automatic merge, per-resource concurrency and HK09B final host/resource envelopes.

Initial `PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.