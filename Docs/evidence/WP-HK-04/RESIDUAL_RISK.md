# WP-HK-04 residual risk / trust boundary

## Trust boundary

HK04 proves transactional semantics for the finite engine-neutral in-memory canonical `WorldState` authoring session introduced by this WP and for public capability handlers composed through the accepted HK01 route boundary. Planning access is attenuated from the internal commit capability; route publication and conformance inspect the current finite set of Authoring write-authority types using normal .NET reflection. It does not claim durable database/engine persistence, distributed transactions, Unity scene writes, undo history, authorization policy enforcement, arbitrary filesystem mutation, gameplay-specific authoring, hostile private-reflection access or subversion of the trusted runtime/compiler.

## Accepted residuals

### Session-lifetime idempotency receipts

The accepted mutation state and its idempotency receipts have the same in-memory session lifetime. Exact retries during that lifetime are safe and different semantics reusing a key fail closed. HK04 does **not** claim durable replay protection across process/session restart because HK04 does not yet persist canonical authorable state across that boundary either.

A future bridge that makes canonical state durable must not keep receipts volatile while continuing to claim HK04 idempotency semantics. It must couple durable state commit and durable idempotency outcome atomically (or provide an equivalent durable transaction identity mechanism).

### In-process atomicity only

The compare-and-swap critical section is process-local. This is sufficient for the current single canonical authoring session. Multi-process/distributed writers are outside the HK04 trust boundary and require a later authoritative persistence/locking substrate preserving the same expected revision/hash semantics.

### Canonical schema subset cannot express discriminated unions or numeric collection bounds

The accepted HK01 `SchemaNode` vocabulary exposes the operation-kind enum and possible typed fields, while operation-specific required/forbidden fields, stable-token grammar, 1..64 operation count and canonical Base64 are enforced by deterministic semantic validation. The canonical `BatchingSemantics` separately advertises maximum 64 operations. Invalid combinations return structured errors rather than being accepted silently.

Extending HK01's schema language solely to encode `oneOf`/min/max would be disproportionate to HK04 and would reopen an accepted predecessor boundary. A later schema-vocabulary WP may tighten this representation without changing HK04 mutation semantics.

### Resource/cost bounds beyond operation count

HK04 bounds a request to 64 operations but does not impose a byte quota on opaque extension payloads or prove asymptotic performance for very large worlds. Resource budgets belong to later harness budget/guardrail work. This does not weaken atomicity, determinism or completeness of the current semantic plan.

### Idempotency key ownership

The caller allocates stable idempotency keys. Reusing one key for different expected anchor or operations is a hard conflict. Retrying the same logical intention after deliberately rebasing onto a newer world is a new request and therefore requires a new key.

### Future authority-bearing implementations

The current canonical writer is the sealed `TransactionalWorldAuthoringSession`; the internal `ICanonicalWorldMutationCommitter` is the only capability issued to Runtime for commit. The inspector recognizes both by assignability and follows handler-local object graphs/delegate targets at publication. If a later persistence WP introduces a different direct canonical writer or an authority broker outside those types, that WP must route it through the internal committer or extend the authority oracle and its causal control. HK04 does not claim to pre-classify future writer abstractions that do not yet exist.

## Explicitly not residual / protected invariants

The following are inside the claim and therefore are not deferred: partial apply, stale overwrite, duplicate accepted apply within the authoritative session, dry-run/apply semantic divergence, unreported effective state changes, and a current effective public handler carrying canonical write authority while escaping the transactional mutation surface. Each has a retained causal negative control; the hidden-bypass control performs and observes a real state mutation before proving the publication/conformance boundary turns red.

No known undetected defect class remains inside the stated HK04 boundary.
