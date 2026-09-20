# WP-HK-08B residual risk and trust boundary

## In-boundary claim

HK08B claims only that a stale whole-world revision/hash CAS failure can expose truthful, bounded recovery information for the current local authored lineage; that an ordinary same-lineage client can use that information to re-plan without a full-world reload; that unprovable lineage/history fails closed without a fabricated delta; and that the representative public-client workflow stays within reviewed interaction budgets with equivalent JSONL/MCP meaning.

## Trusted inherited base

HK08B consumes rather than re-proves these accepted guarantees unless an effective HK08B path contradicts them:

- HK04 canonical mutation authority, atomic commit, whole-world revision/hash CAS and idempotency;
- HK05 complete deterministic machine-actionable diagnostics;
- HK06A journal base/result anchors and material `affectedResources` provenance;
- HK06B snapshot import as a new local lineage with a reset local mutation journal;
- HK07A/HK07B canonical meaning above JSONL/MCP transports;
- HK08A coherent batching, compact reads and explicitly versioned journal pagination.

## Known residuals outside the HK08B claim

1. **Durable ancestry across restart is not claimed.** Recovery uses the complete current local HK06A journal available from the bound authoring session. If the necessary history is unavailable, the correct result is `bounded-reinspection-required`, not reconstructed ancestry.
2. **Automatic merge is not claimed.** Even when writers touch disjoint resources, the accepted H0 model remains whole-world CAS. Recovery helps a client re-plan; it does not silently merge concurrent intent.
3. **Per-resource locking/CAS is not claimed.** HK08B does not weaken or replace the accepted whole-world concurrency boundary.
4. **Multi-process/distributed writer coordination is not claimed.** The proof is for the H0 local authored lineage and accepted public host surfaces.
5. **The elapsed budget is a coarse regression guard, not an SLO.** The 8x headroom intentionally tolerates hosted-runner noise. Product latency and throughput belong to later product/resource-envelope work.
6. **Response-byte budget permits bounded additive growth.** The 25% headroom can accommodate richer truthful diagnostics/recovery data; it does not permit extra public round trips or suppression of required information.
7. **A further write may race a client's recovery/retry.** That is handled by the same optimistic CAS: the retry can become stale again and repeat recovery. HK08B does not promise conflict elimination.
8. **Unknown future material-resource vocabularies are not silently treated as inspectable.** The current HK06A mutation vocabulary produces object/extension resource keys. Runtime preserves an unknown key visibly rather than fabricating an inspection contract; extending that vocabulary requires a reviewed semantic change.
9. **Host quotas, cancellation and resource exhaustion are not finalized here.** HK09B owns final resource-budget/resource-envelope proof.

## Why these residuals do not reopen predecessors

None of the residuals demonstrates that an accepted predecessor guarantee fails on the HK08B path. They are either explicitly deferred concurrency/resource mechanisms or conservative failure modes. In particular, inability to prove an ancestor after history loss/rebase is expected and produces the fail-closed disposition required by this WP.

## Proof-budget conclusion

HK08B adds one read-only recovery decorator/source contract, its machine-readable data vocabulary, focused causal controls, cross-transport conformance and one external-process benchmark. It does not add a second mutation authority, semantic registry, merge engine, scheduler or distributed lock layer.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET
