# HK01 causal amendment — canonical handler failure boundary

CAUSAL_OWNER: WP-HK-01
AMENDMENT_TRIGGER: WP-HK-10 independent review #5261225652
AMENDMENT_REVIEW_VEHICLE: PR #60
OWNER_PROOF: Hk01DispatchFailureContractTests
AMENDMENT_STATUS: CANDIDATE_PENDING_INDEPENDENT_PASS

## Why HK01 owns this

HK10 closure demonstrated a concrete baseline gap: an exception escaping `route.Handler.Invoke(...)` also escaped `ComposedContract.Dispatch`, so the canonical public runtime had no structured outcome for that branch. HK01 owns both `ComposedContract` dispatch semantics and the canonical structured-error model. The correction therefore belongs to HK01 even though HK10 discovered it.

## Amended semantics

Before authoritative publication, an escaping handler exception becomes `contract.handler_failure`, with `retryable=false` and `publicationCommitted=false`. After the invocation budget has recorded authoritative publication, an escaping handler exception becomes `contract.handler_failure_after_publication`, with `retryable=true` and `publicationCommitted=true`; the repair hint requires inspecting the current canonical anchor and using the capability's accepted idempotency/recovery contract rather than assuming no effect.

Both branches include the canonical capability identity and exception type for diagnosis. Raw exception message/stack text is not copied into the public message or repair hint.

This does not add a public capability, change a capability identity/version, alter success semantics, create a mutation authority, or change transport/persistence ownership. It defines a previously unspecified failure branch at the HK01 dispatcher boundary.

## Owner proof

`Hk01DispatchFailureContractTests` uses the already accepted `system.describe@1.0` definition and constructs a dispatcher-only test binding without declaring an extra public route. One handler throws before publication; another explicitly marks publication committed and then throws. The tests require the two distinct machine codes, retryability/publication flags, diagnostic context and absence of internal sentinel leakage.

HK10 retains thrown accepted inspection/validator fixtures only because HK10 explicitly requires handler/validator fault injection. Those closure fixtures now consume the HK01-owned semantics rather than define them.

## Review ordering

The exact-SHA HK10 observation runs the HK01 owner proof before the HK10 closure tests. Independent review of PR #60 must accept this HK01 amendment on the same exact SHA before treating HK10's handler/validator fault evidence as inherited-contract closure. If the amendment fails review, HK10 remains unresolved.

POST_PUBLICATION_MACHINE_CODE: contract.handler_failure_after_publication
PRE_PUBLICATION_MACHINE_CODE: contract.handler_failure
RAW_EXCEPTION_MESSAGE_PUBLIC: NO
