# WP-HK-04 residual risk / trust boundary

## Trust boundary

HK04 proves transactional semantics for the finite engine-neutral in-memory canonical `WorldState` authoring session introduced by this WP and for the current effective public capability surface composed through the accepted HK01 route boundary.

Inside the claim:

- the three HK04 `authoring.change.*@1.0` routes and four generic operation kinds;
- the current HK01 independently enumerable public route universe;
- canonical state change observable through accepted HK02 revision/content hash;
- normal documented C#/.NET public accessibility semantics;
- the Authoring→Runtime internal committer boundary actually used by the current production bindings.

The authoritative session exposes `Current`, `Plan` and `DryRun` publicly but no public commit operation. Canonical commit is available only through internal `ICanonicalWorldMutationCommitter`; the current public non-mutation route set is also executed against a live session and must leave revision/hash unchanged. Thus ordinary indirection through `List<>`, arrays, delegates, wrappers or foreign-assembly holders does not create public commit authority and is not classified by object-graph syntax.

Outside the claim/default trusted base: hostile private reflection to invoke non-public members, runtime/compiler/toolchain subversion, unsafe/native memory corruption and unsupported non-canonical execution paths. HK04 also does not claim durable database/engine persistence, distributed transactions, Unity scene writes, undo history, authorization policy enforcement, arbitrary filesystem mutation or gameplay-specific authoring.

## Accepted residuals

### Session-lifetime idempotency receipts

The accepted mutation state and its idempotency receipts have the same in-memory session lifetime. Exact retries during that lifetime are safe and different semantics reusing a key fail closed. HK04 does **not** claim durable replay protection across process/session restart because HK04 does not yet persist canonical authorable state across that boundary either.

A future bridge that makes canonical state durable must couple durable state commit and durable idempotency outcome atomically, or provide an equivalent durable transaction identity mechanism.

### In-process atomicity only

The compare-and-swap critical section is process-local. This is sufficient for the current single canonical authoring session. Multi-process/distributed writers are outside HK04 and require a later authoritative persistence/locking substrate preserving the same expected revision/hash semantics.

### Canonical schema subset cannot express discriminated unions or numeric collection bounds

The accepted HK01 `SchemaNode` vocabulary exposes the operation-kind enum and possible typed fields, while operation-specific required/forbidden fields, stable-token grammar, 1..64 operation count and canonical Base64 are enforced by deterministic semantic validation. `BatchingSemantics` separately advertises the 64-operation maximum. Invalid combinations fail with structured errors.

Extending HK01 solely to add `oneOf`/min/max would reopen an accepted predecessor boundary without changing HK04 transactional safety. A later schema-vocabulary WP may tighten representation while preserving semantics.

### Resource/cost bounds beyond operation count

HK04 bounds a request to 64 operations but does not impose a byte quota on opaque extension payloads or prove asymptotic performance for very large worlds. Resource budgets belong to later harness budget/guardrail work and do not weaken current atomicity/determinism.

### Idempotency key ownership

The caller allocates stable idempotency keys. Reusing one key for different expected anchor or operations is a hard conflict. Retrying the same logical intention after deliberately rebasing onto newer state is a new request and requires a new key.

### Future mutation implementations

HK04 proves the current effective route universe, not arbitrary future code that does not yet exist. A future public route automatically enters the inherited HK01 independent route universe; if it is non-mutating it must gain an evaluated request vector and demonstrate unchanged canonical revision/hash, and if it is a canonical mutation it must enter the mechanically equal canonical-transaction / `ITransactionalMutationHandler` surface. A future persistence writer must still enter through the Authoring-owned committer boundary or deliberately revise this contract under review.

`MutationAuthorityInspector` remains only a defence-in-depth publication check for accidental direct leakage of the internal committer. Its structural traversal is explicitly **not** a completeness argument and future container forms do not require extending it.

## Explicitly protected invariants

The following are inside the claim and are not deferred: partial apply, stale overwrite, duplicate accepted apply within the authoritative session, dry-run/apply semantic divergence, unreported effective state changes, a public session-level commit method, and any current public command classified as non-mutation that changes canonical revision/hash when effectively invoked.

No known undetected defect class remains inside the stated HK04 boundary after the circuit-breaker repair.
