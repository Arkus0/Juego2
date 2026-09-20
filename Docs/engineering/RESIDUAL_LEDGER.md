# Residual Ledger — declared residuals across accepted H0 workpacks

Version: 1.3 — 2026-09-20

Status: **NON-BINDING inventory.** This document creates no acceptance criterion, reopens no accepted guarantee and alters no workpack contract. It records what accepted workpacks already declared.

## Purpose

`WP-HK-10` must establish zero known undetected defect classes **inside the declared boundary**, and its residual-risk audit needs a universe of declared residuals that it did not compose itself. `FOUNDATIONAL_PROOF_STANDARD.md` forbids a completeness proof whose universe can be silently shrunk by the thing under proof; an audit that invents its own list of residuals has exactly that shape.

This ledger is that independently obtained universe. It exists so the classification work happens incrementally, while each residual is fresh and before any session has an interest in a short list.

It is an inventory, not a verdict. Accepted evidence under `Docs/evidence/**` remains authoritative for what each workpack actually claimed.

## What belongs here

One entry per residual that an accepted workpack **declared as outside its claim, deferred, or deliberately not proved**.

Risks a workpack recorded as closed inside its own claim do not belong here — they are proven, not residual.

## Status vocabulary

- `IN-BOUNDARY` — inside the declared H0 boundary, so `WP-HK-10` must seed a causal control for it or its zero is false.
- `OUT-BOUNDARY` — outside the boundary and staying there, so `WP-HK-GATE` must name it in its residual statement.
- `CLOSED-BY` — a later accepted workpack closed it; the closing evidence is cited.
- `DEFERRED` — a named workpack or milestone owns it and has not run yet.
- `UNCLASSIFIED` — not yet classified. **This is the default and carries no judgement.**

Version 1.0 was the initial transcription pass. Version 1.1 records accepted HK06B. Version 1.2 records accepted HK06C: it closes only the replay/compatibility residuals that HK06C explicitly owned, folds repeated lifetime/concurrency/runtime/history/scale seams into their existing entries, and transcribes HK06C-specific residuals without assigning new classifications or owners not present in accepted evidence. Version 1.3 re-points only planned ownership after the pre-implementation `HK08 → HK08A/HK08B` and `HK09 → HK09A/HK09B` split; it makes no new residual classification.

## A. Trusted base

One class, declared by `WP-HK-00`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B` and `WP-HK-06C`, and authorised by the trusted-base rule of `FOUNDATIONAL_PROOF_STANDARD.md`.

| ID | Residual | Status |
|---|---|---|
| `R-TB-01` | Git implementation misreporting candidate bytes | `OUT-BOUNDARY` |
| `R-TB-02` | Pinned SDK/MSBuild/C# compiler violating documented behaviour, including assembly loading and reflection used for effective-route enumeration | `OUT-BOUNDARY` |
| `R-TB-03` | NuGet implementation or service violating lock/restore semantics | `OUT-BOUNDARY` |
| `R-TB-04` | Runner OS, hypervisor or Actions service compromise | `OUT-BOUNDARY` |
| `R-TB-05` | SHA-256/BCL correctness and collision resistance | `OUT-BOUNDARY` |

Arkus checks identity, configuration and effective observations at these boundaries; it does not recursively prove the implementations. `WP-HK-GATE` should name this class once rather than per workpack.

## B. Deferred to a named owner

| ID | Declared by | Residual | Owner | Status |
|---|---|---|---|---|
| `R-00A-02` | HK-00A | External upstream audit snapshot must be revalidated at the actual adoption point | adoption WP/ADR | `DEFERRED` |
| `R-00A-05` | HK-00A | Concrete H1 engine abstractions and Unity capability definitions not selected | H1 | `DEFERRED` |
| `R-01-07` | HK-01 | No external plugin loader; a future reviewed loader must extend the independent universe rather than trust registration metadata | future loader WP | `DEFERRED` |
| `R-01-08` | HK-01 | Concrete MCP/JSONL projection parity | HK-07A, HK-07B | `DEFERRED` |
| `R-01-09` | HK-01 | Unity/editor types absent; H1 instantiates the scoped-provider boundary | H1 | `DEFERRED` |
| `R-02A-04` | HK-02A | Total payload bytes, dependency count and world size not capped | HK-09B | `DEFERRED` |
| `R-04-04` | HK-04 | Byte quota on opaque extension payloads, beyond the 64-operation request limit | HK-09B | `DEFERRED` |
| `R-05-05` | HK-05 | Unity/engine validation: scene serialization, prefab/component rules, editor constraints | H1 | `DEFERRED` |
| `R-05-06` | HK-05 | Whole-world size and latency budgets are not product guarantees | HK-09B for content-size caps; HK-08B for measured latency/interaction baselines | `DEFERRED` |
| `R-06A-02` | HK-06A | Whether an imported snapshot starts a new local lineage or retains external evidence | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; snapshot import is an explicit canonical rebase that starts `new-local-lineage` and emits truthful rebase evidence |
| `R-06A-03` | HK-06A | Replay interpretation, missing/reordered/tampered entry behaviour, journal/snapshot version compatibility | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; deterministic replay, fail-closed evidence handling and explicit accepted-version compatibility are now canonical capabilities |
| `R-06A-04` | HK-06A | Field-level semantic diff; affected-resource identity is journal metadata only | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; deterministic semantic diff covers the complete accepted authored-resource model |
| `R-06A-05` | HK-06A | Journal growth and pagination; the journal read currently returns the complete session-local journal in one response | HK-08A for bounded journal read/pagination; HK-10 for bounded-session growth evidence inside the HK-09B envelope | `DEFERRED` |
| `R-06A-07` | HK-06A | Transport/storage framing may not redefine journal meaning | HK-07A, HK-07B | `DEFERRED` |
| `R-06B-02` | HK-06B | Deterministic journal replay and policy for accepted journal/snapshot version combinations are not implemented by snapshot portability | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; replay is a distinct canonical authority that consumes HK06A/HK06B artifacts and reports explicit supported/unsupported compatibility |
| `R-06B-03` | HK-06B | File/JSONL/MCP/network/cloud transport framing and persistence may not redefine snapshot semantics | HK-07A, HK-07B | `DEFERRED` |

A `DEFERRED` entry closes when its owner is accepted. The owner's DocSync should move it to `CLOSED-BY` with the accepted evidence, or restate it.

## C. Open — no named owner

These are the entries that decide whether `WP-HK-10` can claim zero, and they are the substance of the classification pass.

| ID | Declared by | Residual | Note |
|---|---|---|---|
| `R-00-05` | HK-00 | Upstream package/tool vendor compromise that preserves expected identity and lock semantics | HK-00 points to "later architecture/gate work" without naming a workpack |
| `R-00-06` | HK-00 | Semantic game-content code added inside otherwise owned product source | governed only by "later WPs govern semantic growth" |
| `R-01-01` | HK-01, HK-03, HK-04 | Canonical schema vocabulary is a deliberate subset: no min/max/maxItems facets, no discriminated unions; semantic limits are enforced by service validation instead | declared three times; HK-04 says "a later schema-vocabulary WP may tighten representation" and none exists |
| `R-01-03` | HK-01 | Cross-provider or shared logical-reference vocabulary is not modelled | must be modelled explicitly rather than by weakening provider ownership |
| `R-01-05` | HK-01 | Cross-major migration guidance beyond "majors are explicit breaking boundaries" | |
| `R-02-01` | HK-02, HK-02A | Persisted-world format migration; V1 fails closed and no accepted migration path exists | HK-02A restated it when the format advanced to V2 |
| `R-02-04` | HK-02 | Richer display names and localization layered over the narrow stable identity syntax | matches `CONTENT_SHAPE_BACKLOG.md` row 15 |
| `R-02-05` | HK-02 | Cross-world and external asset references | touches `CONTENT_SHAPE_BACKLOG.md` row 16 |
| `R-02A-01` | HK-02A, HK-05 | A producer that embeds an object identity in opaque payload bytes without declaring it cannot be detected generically | matches `CONTENT_SHAPE_BACKLOG.md` row 9 |
| `R-02A-02` | HK-02A | Whole-world revision/hash CAS; no per-resource concurrency and no automatic merge of disjoint edits | HK08B owns cheap same-lineage recovery under the accepted whole-world CAS, while finer concurrent-writer semantics remain an open post-GATE product decision unless measured evidence promotes them. Measured cost context below |
| `R-03-01` | HK-03 | Query evaluation is bounded in output, not in scan cost; no index, sublinear complexity or world-size-independent CPU claim | HK-03 defers this to "future scale work", which does not exist. H0 gates on a micro-world, so this is a candidate for `OUT-BOUNDARY`; the post-GATE H0S track may measure it but is not a binding owner |
| `R-03-02` | HK-03 | Cursors are deterministic continuation tokens, not authenticated capabilities, and are not an authorization boundary | HK09A constrains host authority but deliberately does not turn cursors into authentication/authorization capabilities; classification remains open |
| `R-03-03` | HK-03 | No multi-command snapshot lease; a state source may advance between calls | stale anchors fail closed rather than mixing revisions |
| `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C | Canonical state, mutation idempotency receipts/journal, snapshots, snapshot-rebase receipts/evidence and replay staging remain process-local; no durable WAL/recovery or cross-process persistence claim | HK06C restates the lifetime seam for replay and explicitly excludes process-crash recovery |
| `R-04-02` | HK-04, HK-06C | Multi-process/distributed writers, cross-process locking and multi-agent merge/coordination | HK06C uses the accepted whole-world CAS + session gate and makes no richer concurrency claim; the post-GATE H0S track is the default measurement venue, not a binding owner |
| `R-04-05` | HK-04 | Asymptotic performance for very large worlds | split out of `R-04-04`: HK-04 defers it to "later harness budget/guardrail work", which does not exist. Same candidate as `R-03-01`; H0S may measure it after GATE |
| `R-05-01` | HK-05 | Secondary diagnostics whose own source or dependency traversal is ambiguous are deferred and not visible in the same validation pass; the contract is iterative for those | HK-05 declares this as claimed semantics, not a gap. Open question for classification: `arkus.world-validation-result/v1` carries no field indicating the report is partial |
| `R-06B-04` | HK-06B, HK-06C | Gameplay/runtime state such as transforms, clocks, schedules, physics, animation, AI and simulation remains outside canonical authored `WorldState`/replay unless a later reviewed WP promotes it | no specific owner named by HK06B/HK06C |
| `R-06B-05` | HK-06B, HK-06C | Snapshot import/replay uses explicit local lineage roots; merging, rebasing or reconciling independent authored histories would require a future explicit contract | HK06C rejects a replay target with pre-existing local mutation history rather than splicing histories |
| `R-06B-06` | HK-06B, HK-06C | Whole-state snapshot payloads, full semantic comparisons and staged replay are accepted for H0; streaming/chunking/large-history throughput for larger worlds is not claimed | HK06C restates that arbitrary-long journal performance/endurance is outside its correctness claim; H0S may measure larger-scale needs after GATE |
| `R-06C-01` | HK-06C | Lost-response replay retry is not durable/idempotent: after successful replay the local journal is non-empty, so an identical blind retry is rejected rather than acknowledged from a durable replay receipt | no owner named by HK06C |
| `R-06C-02` | HK-06C | Future journal/snapshot versions currently fail as unsupported; no migration capability or cross-version replay path exists until separately reviewed | no owner named by HK06C |

### Measured cost context for `R-02A-02`

An exploratory probe on 2026-09-19 measured what a whole-world commit pays as the authored world grows: candidate validation of the 17 invariants, canonical serialization and canonical content hash, over a synthetic town-shaped world (containment tree of branching factor 8, one typed reference per 10 objects, one object-scoped extension per 5).

| Objects | Validate | Serialize | Hash | Total | Canonical bytes |
|---|---|---|---|---|---|
| 1 000 | 4.2 ms | 6.3 ms | 6.6 ms | 17 ms | 92 KiB |
| 10 000 | 13.6 ms | 23.1 ms | 31.9 ms | 69 ms | 958 KiB |
| 25 000 | 34.7 ms | 87.7 ms | 69.5 ms | 192 ms | 2.4 MiB |
| 50 000 | 80.2 ms | 150.3 ms | 151.9 ms | 382 ms | 4.8 MiB |
| 100 000 | 231.2 ms | 385.1 ms | 420.4 ms | 1 037 ms | 9.6 MiB |
| 200 000 | 541.2 ms | 949.6 ms | 984.4 ms | 2 475 ms | 19.4 MiB |

Growth is roughly `O(n log n)` — 20x the objects costs 36x the time between 10 000 and 200 000 — with no discontinuity up to 200 000 objects. Per-object cost rises slowly from about 7 to 12 microseconds.

Reading it: the cost is per **transaction**, not per edited object, because identity is whole-world. A batched authoring flow pays it once; an unbatched one pays it per edit. `WP-HK-08A` owns batching and is therefore the direct H0 mitigation, while `WP-HK-08B` owns the measured end-to-end interaction baseline.

Limits of the measurement, which is **not evidence**: it is not bound to a candidate SHA, was not produced by canonical validation, and ran on shared container CPU. It covers `Arkus.Game.Core` and `Arkus.Game.World` only, so it is a lower bound — a real commit also pays HK-04 plan/change-set work, HK-05 diagnostic aggregation and the HK-06A journal entry. The synthetic world's reference density is uniform and real content will differ.

What would make this a live decision rather than recorded context: a representative target whose authored object count approaches the high tens of thousands, or a requirement for concurrent disjoint writers, which batching does not help and whole-world CAS rejects by design.

## D. Entries declared more than once

A residual restated by several workpacks is a signal that it sits on a seam rather than inside one contract.

| Theme | Entries | Declared by |
|---|---|---|
| Canonical schema vocabulary subset | `R-01-01` | HK-01, HK-03, HK-04 |
| In-memory lifetime and durability | `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C |
| Undeclared identity inside opaque payloads | `R-02A-01` | HK-02A, HK-05 |
| Size, byte and latency budgets | `R-02A-04`, `R-04-04`, `R-05-06`, `R-06A-05` | HK-02A, HK-04, HK-05, HK-06A |
| Large-world scaling with no owner | `R-03-01`, `R-04-05`, `R-06B-06` | HK-03, HK-04, HK-06B, HK-06C |
| Replay/compatibility | `R-06A-03`, `R-06B-02`, `R-06C-02` | HK-06A, HK-06B, HK-06C |
| Transport framing | `R-06A-07`, `R-06B-03` | HK-06A, HK-06B |
| Cross-process/concurrent writers | `R-04-02`, `R-02A-02` | HK-04, HK-06C, HK-02A |

The budget theme now uses three different mechanisms. HK-08A owns **interaction primitives** such as batching and bounded journal/page reads. HK-08B owns **measured end-to-end baselines and regression budgets**. HK-09B owns **enforced caps** such as input/batch/page/depth/resource limits that fail closed. Journal/session growth itself is exercised by HK10 against the accepted HK09B resource envelope.

None of those workpacks owns arbitrary large-world asymptotic optimization or richer concurrent-writer semantics. `R-03-01`, `R-04-05`, `R-06B-06`, `R-04-02` and the concurrency half of `R-02A-02` therefore remain open unless deliberately classified. The roadmap's post-GATE H0S track provides a measurement venue without pretending those residuals are already closed or owned.

## E. Deliberate non-goals already stated as such

Recorded so they are not rediscovered as findings: conservative same-major compatibility that prefers false-breaking over false-compatible (HK-01); no semantic denylist guessing at engine allusions in opaque tokens (HK-01); semantic fingerprints as diagnostics rather than correctness authorities (HK-01); forward compatibility as extension-envelope compatibility rather than unknown-structure guessing (HK-02); completeness tied to the current accepted state universe rather than predicted future shapes (HK-03, HK-05, HK-06B, HK-06C); caller ownership of idempotency keys (HK-04); `MutationAuthorityInspector` as defence in depth and explicitly not a completeness argument (HK-04); ordinary argument exceptions for direct CLR misuse outside the dispatcher contract (HK-05); runtime observation content not modelled (HK-06A); snapshot import intentionally starting a new local lineage rather than merging mutation history (HK-06B); replay intentionally refusing to splice into pre-existing local mutation history (HK-06C); deterministic journal fingerprints/entry IDs are identities and consistency checks, not cryptographic signatures or provenance authentication (HK-06C); prose not mechanically proving every future semantic interpretation, and unusual license terms failing closed to human review (HK-00A).

## Maintenance

DocSync for an accepted workpack appends that workpack's declared residuals here and updates any entry the workpack closed. A residual that exists only in `Docs/evidence/**` and never reaches this ledger is invisible to `WP-HK-10`'s audit.

Adding an entry is cheap and carries no obligation. Removing one requires citing the accepted evidence that closed it.
