# WP-HK-09B H0 resource envelope

RESOURCE_ENVELOPE_VERDICT: PASS

## Enforced envelope

The machine-readable contract is `arkus.h0-resource-envelope@1`, discoverable through `system.resource-envelope.describe@1.0`.

| Dimension | H0 ceiling | Enforcement seam |
|---|---:|---|
| transport frame bytes | 2 MiB | reference JSONL framing before neutral request dispatch |
| canonical argument bytes | 1792 KiB | transport-neutral `H0ResourcePolicy` |
| portable nesting depth | 32 | portable measurement before canonical dispatch |
| execution budget | 5000 ms | cooperative budget plus authoritative publication check |
| mutation operations | 96 | neutral admission + canonical mutation parser/metadata |
| decoded mutation payload | 512 KiB | neutral admission + canonical mutation parser |
| relations per resource | 256 | request admission and materialized state validation |
| extension payload | 256 KiB | request admission and materialized state validation |
| query page size | 100 | neutral admission; accepted paged read contracts remain authoritative below it |
| canonical world bytes | 1 MiB | materialized world/snapshot validation |
| canonical world resources | 10,000 | materialized world validation |
| session mutation transactions | 10,000 | canonical mutation authority before publication |
| snapshot import receipts | 1,024 | snapshot rebase authority before publication |

Persistence metadata is also explicit: `durabilityLevel=process-local-checkpoint`, `publicationBoundary=validate-stage-aggregate-publish`, and `powerLossDurabilityClaimed=false`.

## Why these limits do not redefine HK08A/HK08B

The operation ceiling is fixed by accepted HK08A evidence rather than a pre-existing convenience constant. HK08A already proved a representative coherent edit of 72 objects + 24 extensions = **96 operations** in one transaction, explicitly above the former provisional 64-operation ceiling. HK09B therefore sets the enforced maximum to 96 and tests that the same shape still advances exactly one revision and one journal entry.

The product-shaped HK09B probe independently maps the current approved Potes/Liébana hero slice into the same 96-operation envelope. This prevents resource policy from making the accepted authoring shape smaller after the fact.

HK08B's accepted five-flow external-process benchmark measured 12 requests, 12,051 serialized response bytes and 185 ms, with regression guards at 12 requests / 15,064 bytes / 1,480 ms. HK09B's 5 s execution ceiling is an outer fail-closed publication budget, not a relaxed performance target; it does not replace or raise HK08B's interaction-regression budgets.

The remaining ceilings are H0 bounding guards, not claims of arbitrary production scale. They sit above the accepted representative shapes and exist so HK10 has explicit finite session/world bounds to measure against. If HK10 or later approved product content demonstrates that one coherent valid intent cannot fit, the reviewed envelope must change; clients must not silently split required atomic work to satisfy a legacy cap.

## Layering

- `H0ResourcePolicy` performs transport-neutral admission for portable bytes/depth, batch count/payload/relations, page size and snapshot size.
- `WorldResourceLimits` validates the materialized canonical state for world bytes/resource count, relations and extension payloads. This second check means adapter-side estimates are not the semantic authority.
- Canonical mutation/import authorities enforce bounded transaction/receipt growth and publication budgets at the existing write authorities.
- Adapters may reject malformed or impossible framing earlier, but valid framed requests converge on the same neutral resource semantics.

## Evidence

`EnforcedH0EnvelopeIsMachineReadableAndMatchesCanonicalMetadata` verifies the public data against constants and canonical batching metadata. `OversizeDeepBatchPayloadAndPageRequestsFailBeforeCanonicalEffect` covers bytes/depth/batch payload/count/page negatives. `RepresentativeNinetySixOperationIntentFitsMeasuredEnvelopeAndCommitsOnce` proves the accepted coherent edit still fits atomically. Cross-transport tests compare the same resource outcomes over JSONL and MCP.

The implementation checkpoint `e370606e4278da3e08602a3167c1cb6513bea93d` passed focused HK09B 9/9 and full regression 205/205 in Actions run `35521120979`.
