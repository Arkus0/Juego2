# WP-HK-06C residual risk

## Claim boundary

HK06C claims deterministic replay of accepted HK06A mutation-journal evidence from the exact accepted authored base, with explicit compatibility, canonical HK04/HK05 execution, fail-closed sequence/result checks, atomic outer publication and HK06B semantic/hash equivalence.

Within that boundary, no known material defect class remains undetected after the focused positive/negative controls and full regression at implementation/test SHA `0d7e22be092d8291d6f7e239e73ed7e91217bd3a`.

## Non-blocking residuals outside the claim

### Durable journal storage and crash recovery

HK06C consumes an in-memory accepted journal artifact. It does not define durable WAL/storage, process-crash recovery, fsync semantics or recovery after host loss. The atomicity claim is the H0 single-process aggregate publication boundary under the foundational trusted base.

### Cross-process / multi-agent concurrency

Replay is guarded by the existing whole-world revision/hash CAS and the session gate. Cross-process locking, multi-agent merge and per-resource concurrency are not claimed here. The accepted post-HK06B process boundary deliberately defers richer concurrency/batching ergonomics beyond this WP.

### Lost-response replay retry

`authoring.journal.replay@1.0` is explicitly non-idempotent at the route level: after success the local journal is no longer empty, so an identical blind retry is rejected rather than silently replayed twice. Durable replay receipts / retry-after-lost-ack semantics are not part of HK06C.

### History merge across lineage roots

Replay requires a clean local replay base and refuses to splice a source journal into pre-existing local mutation history. Merging, rebasing or reconciling two independent authored histories is intentionally outside scope.

### Artifact authenticity / signatures

HK06A deterministic fingerprints and entry IDs provide machine-readable identity and self-consistency; HK06C verifies that accepted framing and the effective persisted result. They are not signatures or an authentication system. A wholly regenerated alternate valid journal represents different evidence and, if it deterministically persists its own declared result, replay is truthful to that supplied artifact. Cryptographic provenance authenticity is not a game-authoring requirement of HK06C.

### Future journal/snapshot versions

Only the accepted `arkus.authoring.journal@1` + `arkus.authoring.journal-entry@1` combination is executable, with HK06B `arkus.authoring.snapshot@1` as a supported base-establishment format. Future versions currently return `unsupported`; a `migration-required` disposition becomes meaningful only when a separately reviewed migration capability exists.

### Large-journal performance/endurance

HK06C proves deterministic semantics, not a throughput/latency SLO for arbitrarily long histories. The staged replay approach duplicates one authored session aggregate during execution. Bounded endurance/performance work can be added when a later WP owns operational scale; this is not a correctness blocker for the declared H0 scope.

### Runtime/gameplay replay

No claim is made for deterministic NPC simulation, scheduler/clock state, physics, animation, Unity scene state, networking or other transient observations. HK06C is authored-state replay only.

## Trusted base

As permitted by `FOUNDATIONAL_PROOF_STANDARD.md`, HK06C trusts:

- exact Git object/checkout semantics;
- pinned .NET/MSBuild/NuGet documented behavior;
- normal single-process memory/locking and reference-swap behavior;
- SHA-256 and UTF-8 primitives used according to contract;
- normal CI/OS/filesystem operation.

Arbitrary out-of-contract corruption of those trusted layers is not a freeze blocker.

## Blocking-risk conclusion

No residual listed above can falsify the stated HK06C acceptance claim without leaving its declared boundary. No concrete evidence requires reopening HK06A or HK06B.

KNOWN_UNDETECTED_DEFECT_CLASSES: 0
UNRESOLVED_PROOF_OBLIGATIONS: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET