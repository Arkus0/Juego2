# WP-HK-09B H0 resource envelope

RESOURCE_ENVELOPE_VERDICT: PASS

## Enforced envelope

The machine-readable contract is `arkus.h0-resource-envelope@1`, discoverable through `system.resource-envelope.describe@1.0`.

| Dimension | H0 ceiling | Enforcement seam |
|---|---:|---|
| transport frame bytes | 1 MiB | inherited `arkus.reference.jsonl@1` framing contract; HK09B mirrors rather than redefines it |
| canonical argument bytes | 896 KiB | transport-neutral `H0ResourcePolicy` |
| portable nesting depth | 32 | portable measurement before canonical dispatch |
| execution budget | 5000 ms | cooperative budget plus authoritative publication check |
| mutation operations | 96 | neutral admission + canonical mutation parser/metadata |
| decoded mutation payload | 512 KiB | neutral admission + canonical mutation parser |
| relations per resource | 256 | request admission and materialized state validation |
| extension payload | 256 KiB | request admission and materialized state validation |
| query page size | 100 | neutral admission; accepted paged read contracts remain authoritative below it |
| canonical world bytes | 640 KiB | materialized world/snapshot validation |
| canonical world resources | 10,000 | materialized world validation |
| session mutation transactions | 10,000 | canonical mutation authority before publication |
| snapshot import receipts | 1,024 | snapshot rebase authority before publication |

Persistence metadata is also explicit: `durabilityLevel=process-local-checkpoint`, `publicationBoundary=validate-stage-aggregate-publish`, and `powerLossDurabilityClaimed=false`.

## Compatibility derivation from the frozen HK07A transport

HK07A already froze `arkus.reference.jsonl@1` at **1,048,576 UTF-8 bytes per frame**, with larger input rejected as `transport.frame_too_large`. HK09B does not own or version that transport contract, so the inherited framing implementation and inherited HK07A regression oracle remain unchanged.

The transport-neutral canonical-argument ceiling is therefore **896 KiB**, leaving 128 KiB of framing headroom for protocol/request/version/correlation metadata. The cross-transport oversized-request probe sends `MaximumCanonicalRequestBytes + 1` through both real JSONL and MCP processes; because that request still fits the inherited JSONL frame, both adapters reach the same neutral `resource.request_bytes_exceeded` semantic boundary instead of relying on a transport error.

The canonical-world/snapshot ceiling is **640 KiB** so a maximally sized decoded canonical state expands to at most **873,816 base64 characters (~853.3 KiB)**. That leaves **43,688 bytes** inside the 896 KiB canonical-argument ceiling for snapshot/import metadata before transport framing overhead. This keeps the advertised snapshot envelope representable through the inherited JSONL @1 path rather than advertising a state size that only MCP could import.

These ceilings are compatibility bounds, not a claim that all future production worlds must remain this size. If later approved product evidence requires more, the envelope and/or an explicitly versioned transport must be revised together rather than silently changing @1.

## Why these limits do not redefine HK08A/HK08B

The operation ceiling is fixed by accepted HK08A evidence rather than a pre-existing convenience constant. HK08A already proved a representative coherent edit of 72 objects + 24 extensions = **96 operations** in one transaction, explicitly above the former provisional 64-operation ceiling. HK09B therefore sets the enforced maximum to 96 and tests that the same shape still advances exactly one revision and one journal entry.

The product-shaped HK09B probe independently maps the current approved Potes/Liébana hero slice into the same 96-operation envelope. This prevents resource policy from making the accepted authoring shape smaller after the fact.

HK08B's accepted five-flow external-process benchmark measured 12 requests, 12,051 serialized response bytes and 185 ms, with regression guards at 12 requests / 15,064 bytes / 1,480 ms. HK09B's 5 s execution ceiling is an outer fail-closed publication budget, not a relaxed performance target; it does not replace or raise HK08B's interaction-regression budgets.

The remaining ceilings are H0 bounding guards, not claims of arbitrary production scale. They sit above the accepted representative shapes and exist so HK10 has explicit finite session/world bounds to measure against. If HK10 or later approved product content demonstrates that one coherent valid intent cannot fit, the reviewed envelope must change; clients must not silently split required atomic work to satisfy a legacy cap.

## Layering

- `H0ResourcePolicy` performs transport-neutral admission for portable bytes/depth, batch count/payload/relations, page size and snapshot size.
- `WorldResourceLimits` validates the materialized canonical state for world bytes/resource count, relations and extension payloads. This second check means adapter-side estimates are not the semantic authority.
- Canonical mutation/import authorities enforce bounded transaction/receipt growth and publication budgets at the existing write authorities.
- `arkus.reference.jsonl@1` keeps its inherited 1 MiB framing/error semantics. Successfully framed requests then converge on the same neutral resource semantics as MCP; malformed or physically oversized frames remain transport failures.

## Evidence

`EnforcedH0EnvelopeIsMachineReadableAndMatchesCanonicalMetadata` verifies the public data against constants and canonical batching metadata. `OversizeDeepBatchPayloadAndPageRequestsFailBeforeCanonicalEffect` covers bytes/depth/batch payload/count/page negatives. `RepresentativeNinetySixOperationIntentFitsMeasuredEnvelopeAndCommitsOnce` proves the accepted coherent edit still fits atomically. `Hk07AReferenceTransportTests.MalformedTruncatedInvalidUtf8AndOversizedFramesFailWithStableTransportErrors` remains the unchanged predecessor oracle for 1 MiB + `transport.frame_too_large`, while HK09B cross-transport tests compare neutral resource outcomes over real JSONL and MCP processes below that inherited framing boundary.

Post-review repair validation supersedes the original implementation checkpoint; the final GREEN run is recorded in `WORKER_PRE_REVIEW.md` before refreeze.
