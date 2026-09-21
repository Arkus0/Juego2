# Residual Ledger — declared residuals across accepted workpacks

Version: 1.15 — 2026-09-21

Status: **NON-BINDING inventory.** This document creates no acceptance criterion, reopens no accepted guarantee and alters no workpack contract. It records what accepted workpacks already declared.

## Purpose

`WP-HK-10` completed the strict classification pass over this independently maintained universe. Its accepted `Docs/evidence/WP-HK-10/RESIDUAL_RISK.md` records `UNCLASSIFIED_RESIDUALS: 0`. Accepted `WP-HK-GATE` then consumed that handoff exactly: `Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md` reproduces all 53 inherited IDs with their accepted classifications, and exact-SHA verification mechanically checks ID + classification equality against HK10.

HK-GATE did not close or reclassify the accepted out-of-boundary product/scale/durability/security/lifecycle classes and produced no measured evidence requiring one of them to block H1. H0 is complete; named H1/H0S/future owners remain responsible for later work where already declared.

This ledger remains the independently obtained universe. It exists so residual inventory is maintained incrementally while each declaration is fresh, rather than being composed by the closure or gate that later audits it.

It is an inventory, not a verdict. Accepted evidence under `Docs/evidence/**` remains authoritative for what each workpack actually claimed.

## What belongs here

One entry per residual that an accepted workpack **declared as outside its claim, deferred, or deliberately not proved**.

Risks a workpack recorded as closed inside its own claim do not belong here — they are proven, not residual.

## Status vocabulary

- `IN-BOUNDARY` — inside the declared H0 boundary, so accepted closure must have causal evidence or its zero is false.
- `OUT-BOUNDARY` — outside the accepted H0 boundary; HK-GATE has named/reconciled this class and later work needs a new explicit owner/evidence to change it.
- `CLOSED-BY` — a later accepted workpack closed it; the closing evidence is cited.
- `DEFERRED` — a named workpack or milestone owns it and has not run yet.
- `UNCLASSIFIED` — not yet classified. **This is the default and carries no judgement.**

Version 1.0 was the initial transcription pass. Version 1.1 records accepted HK06B. Version 1.2 records accepted HK06C: it closes only the replay/compatibility residuals that HK06C explicitly owned, folds repeated lifetime/concurrency/runtime/history/scale seams into their existing entries, and transcribes HK06C-specific residuals without assigning new classifications or owners not present in accepted evidence. Version 1.3 re-points only planned ownership after the pre-implementation `HK08 → HK08A/HK08B` and `HK09 → HK09A/HK09B` split; it makes no new residual classification. Version 1.4 records accepted HK07A: it closes the deterministic JSONL/reference-projection half of the transport seam, leaves MCP cross-transport parity to HK07B, folds repeated process-lifetime/concurrency/gameplay/scale seams into existing entries, and transcribes HK07A-specific host/resource residuals only where the accepted downstream owner is explicit. Version 1.5 records accepted HK07B: it closes the named MCP/JSONL projection-parity and local journal/snapshot framing obligations, records the actual MCP dependency-adoption revalidation, folds repeated process-lifetime/resource/remote-host seams into existing entries, and transcribes only the HK07B residuals that remain beyond the accepted local-stdio projection claim. Version 1.6 records accepted HK08A: it closes the named batching/compact/pagination half of the interaction-efficiency seam, records bounded journal paging as explicit v2 while preserving complete v1 compatibility, leaves stale-plan recovery and measured end-to-end interaction budgets to HK08B, and folds repeated process-lifetime/resource/cursor-authentication seams into their existing entries. Version 1.7 records accepted HK08B: it closes the named stale-CAS recovery/repair/benchmark residual, records exact-lineage fail-closed recovery and executable reference-client interaction budgets, folds repeated durability/concurrency/resource-limit seams into existing entries, and adds the explicitly declared unknown-future-material-resource vocabulary residual without assigning a speculative owner. Version 1.8 records accepted HK09A: it closes the caller-selected `--file PATH`/host-capability residual by removing that production authority and enforcing H0 admission at the neutral-projection boundary, folds repeated resource/persistence and remote/OS-security exclusions into their existing entries, and leaves HK09B resource/interruption ownership and H1/external-system authority boundaries unchanged. Version 1.9 records accepted HK09B: it closes the named H0 payload/dependency/world/request/batch/page/depth/session resource-cap obligations, fixes the accepted 96-operation coherent edit inside a machine-readable envelope, closes process-local interruption integrity at mutation/import/replay publication, preserves frozen JSONL @1 framing rather than redefining it, and leaves HK10 bounded-session endurance plus power-loss/general crash recovery, hard preemptive CPU/memory governance, shipping SLOs and arbitrary large-world scaling outside the accepted HK09B claim. Version 1.10 records accepted HK10: it closes `R-06A-05` with bounded 512-transaction endurance/recovery evidence inside the HK09B envelope, records the accepted closure classification of every ledger entry with zero unclassified residuals, and leaves the accepted out-of-boundary product/scale/durability/security/lifecycle classes for HK-GATE to name rather than reinterpret. Version 1.11 records accepted HK-GATE: it mechanically reconciles all 53 inherited residual IDs/classifications, closes no out-of-boundary class by assertion, and confirms that no remaining residual is required to block the representative H0→H1 authoring contract. Version 1.12 changes no accepted classification: it assigns the three explicit H1-owned deferred rows to their planned causal workpacks so later DocSync has a precise owner. Version 1.13 records the downstream residual boundary declared by accepted `WP-CITY-02`; it adds no H0 classification and does not reinterpret the accepted CITY-02 Q6 reopen condition as a residual. Version 1.14 records accepted `WP-CITY-05`: it closes the reusable exterior-grammar/host-fit planning portion of `R-CITY02-01`, re-points the remaining exact-placement/realized-dimension work downstream, and transcribes CITY-05's asset/discovery-capability/keeper-realization/authoring-proof residuals without changing H0 classifications. Version 1.15 records the empirical/runtime boundary explicitly deferred by accepted `WP-PA-03`; it adds no H0 classification and does not convert PA-03's accepted semantic constraints into runtime proof.

## A. Trusted base

One class, declared by `WP-HK-00`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B`, `WP-HK-09A`, `WP-HK-09B`, consumed/classified by accepted `WP-HK-10`, reconciled by accepted `WP-HK-GATE`, and authorised by the trusted-base rule of `FOUNDATIONAL_PROOF_STANDARD.md`.

| ID | Residual | Status |
|---|---|---|
| `R-TB-01` | Git implementation misreporting candidate bytes | `OUT-BOUNDARY` |
| `R-TB-02` | Pinned SDK/MSBuild/C# compiler violating documented behaviour, including assembly loading and reflection used for effective-route enumeration | `OUT-BOUNDARY` |
| `R-TB-03` | NuGet implementation or service violating lock/restore semantics | `OUT-BOUNDARY` |
| `R-TB-04` | Runner OS, hypervisor or Actions service compromise | `OUT-BOUNDARY` |
| `R-TB-05` | SHA-256/BCL correctness and collision resistance | `OUT-BOUNDARY` |

Arkus checks identity, configuration and effective observations at these boundaries; it does not recursively prove the implementations. Accepted HK-GATE named/reconciled this trusted-base class without pulling it inside H0.

## B. Deferred to a named owner

| ID | Declared by | Residual | Owner | Status |
|---|---|---|---|---|
| `R-00A-02` | HK-00A, HK-07B | External upstream audit snapshot must be revalidated at the actual adoption point | adoption WP/ADR | `CLOSED-BY` — HK07B PASS `#5259854121`; the actual MCP adoption pins `ModelContextProtocol.Core` 2.2.0, records exact upstream/license identity and preserves an explicit Arkus conformance/replacement boundary |
| `R-00A-05` | HK-00A | Concrete H1 engine abstractions and Unity capability definitions not selected | H1-00 + H1-01, confirmed at H1-GATE | `DEFERRED` |
| `R-01-07` | HK-01 | No external plugin loader; a future reviewed loader must extend the independent universe rather than trust registration metadata | future loader WP | `DEFERRED` |
| `R-01-08` | HK-01, HK-07A, HK-07B | Concrete MCP/JSONL projection parity | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121`; MCP is accepted as a genuine second projection of `arkus.neutral-projection@1`, with external stdio conformance across the representative accepted H0 surface and canonical composition remaining semantic authority |
| `R-01-09` | HK-01 | Unity/editor types absent; H1 instantiates the scoped-provider boundary | H1-01 | `DEFERRED` |
| `R-02A-04` | HK-02A, HK-09B | Total payload bytes, dependency count and world size were not capped before HK09B | HK-09B | `CLOSED-BY` — HK09B PASS `#5261068513`; the accepted machine-readable envelope caps decoded mutation payload, per-resource relations/payload, canonical world bytes and world resource count, with materialized Authoring state rechecked before publication |
| `R-04-04` | HK-04, HK-08A, HK-09B | Byte quota on opaque extension payloads and final operation/request resource envelope were not fixed by the accepted representative 96-operation HK08A shape | HK-09B | `CLOSED-BY` — HK09B PASS `#5261068513`; the 96-operation coherent edit remains one transaction inside explicit canonical-argument, decoded-payload, relation, extension, world and session ceilings |
| `R-05-05` | HK-05 | Unity/engine validation: scene serialization, prefab/component rules, editor constraints | H1-08 | `DEFERRED` |
| `R-05-06` | HK-05, HK-08A, HK-08B, HK-09B | Whole-world size and product latency/throughput/resource budgets are not shipping guarantees; HK09B now supplies reviewed H0 content/resource ceilings but not production SLOs | later product evidence for shipping SLOs | `DEFERRED` — HK08B PASS `#5260337340` supplies the measured reference-client regression baseline; HK09B PASS `#5261068513` closes the enforced H0 size/resource-cap half; neither is a shipping latency/throughput SLO |
| `R-06A-02` | HK-06A | Whether an imported snapshot starts a new local lineage or retains external evidence | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; snapshot import is an explicit canonical rebase that starts `new-local-lineage` and emits truthful rebase evidence |
| `R-06A-03` | HK-06A | Replay interpretation, missing/reordered/tampered entry behaviour, journal/snapshot version compatibility | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; deterministic replay, fail-closed evidence handling and explicit accepted-version compatibility are now canonical capabilities |
| `R-06A-04` | HK-06A | Field-level semantic diff; affected-resource identity is journal metadata only | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; deterministic semantic diff covers the complete accepted authored-resource model |
| `R-06A-05` | HK-06A, HK-07A, HK-08A, HK-08B, HK-09B, HK-10 | Session-local provenance/history growth inside the finite H0 session envelope required bounded endurance evidence | HK-10 | `CLOSED-BY` — HK10 PASS `#5261301248`; the accepted 512-transaction endurance case periodically inspects, validates, reads journal, exports snapshots and imports the final snapshot into a fresh session with matching revision/hash while remaining below HK09B transaction/import/world limits and recording process/session growth |
| `R-06A-07` | HK-06A, HK-07A, HK-07B | Transport/storage framing may not redefine journal meaning | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121`; JSONL and MCP preserve the accepted journal/replay semantics through the same neutral/canonical path. Process/storage lifetime remains separately tracked in `R-04-01` |
| `R-06B-02` | HK-06B | Deterministic journal replay and policy for accepted journal/snapshot version combinations are not implemented by snapshot portability | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; replay is a distinct canonical authority that consumes HK06A/HK06B artifacts and reports explicit supported/unsupported compatibility |
| `R-06B-03` | HK-06B, HK-07A, HK-07B | File/JSONL/MCP/network/cloud transport framing and persistence may not redefine snapshot semantics | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121` closes the named local file/JSONL/MCP transport-parity obligation: snapshot export/import/diff/replay retain equivalent accepted meaning through MCP. Durability and remote/cloud semantics remain separately recorded in `R-04-01` and `R-07A-04` |
| `R-07A-01` | HK-07A, HK-07B, HK-08A, HK-08B, HK-09A, HK-09B | Admission-only cancellation/timeout and transport/interaction framing lacked an accepted H0 post-dispatch/resource envelope | HK-09B | `CLOSED-BY` — HK09B PASS `#5261068513`; canonical request bytes/depth, batch count/payload/relations, page size, world/snapshot bytes/resources, session growth and a cooperative 5 s execution/publication budget are now explicit and machine-readable. Hard preemptive CPU/memory governance remains separately recorded as `R-09B-01` |
| `R-07A-02` | HK-07A | `--file PATH` accepts an explicit caller path and HK07A does not claim filesystem sandbox/allow-list, path-containment or broader host-capability policy | HK-09A | `CLOSED-BY` — HK09A PASS `#5260496498`; the production H0 executable rejects `--file` before the legacy framing host can open any caller-selected path, and H0 host-capability admission is enforced at neutral-projection construction below conforming transports |
| `R-07A-03` | HK-07A, HK-07B, HK-08A, HK-08B | Structured stale-revision recovery, repair ergonomics and measured end-to-end interaction budgets remain outside the earlier accepted interaction primitives | HK-08B | `CLOSED-BY` — HK08B PASS `#5260337340`; `arkus.world-conflict-recovery@1` now provides exact-lineage fail-closed recovery, affected-only reinspection, normal retry authority, executable reference-client budgets and JSONL↔MCP conformance |

A `DEFERRED` entry closes when its owner is accepted. The owner's DocSync should move it to `CLOSED-BY` with the accepted evidence, or restate it.

## C. Open product/lifecycle residuals — accepted H0 classification after HK-GATE

Accepted HK10 evidence classified every entry below as `OUT-BOUNDARY` for H0 closure unless an already named later owner is shown elsewhere. Accepted HK-GATE then reproduced the complete 53-row handoff without reclassification and found no representative-flow evidence requiring any of these classes to block H1. They therefore remain explicit post-H0 residuals, not hidden green guarantees.

| ID | Declared by | Residual | Note |
|---|---|---|---|
| `R-00-05` | HK-00 | Upstream package/tool vendor compromise that preserves expected identity and lock semantics | HK10: `OUT-BOUNDARY`; repository-local H0 checks pinned identity/locked restore rather than recursively proving vendors |
| `R-00-06` | HK-00 | Semantic game-content code added inside otherwise owned product source | HK10: `OUT-BOUNDARY`; product/H1 semantic growth is not generic harness closure |
| `R-01-01` | HK-01, HK-03, HK-04 | Canonical schema vocabulary is a deliberate subset: no min/max/maxItems facets, no discriminated unions; semantic limits are enforced by service validation instead | HK10: `OUT-BOUNDARY`; expanding the public schema vocabulary is a reviewed semantic feature, not closure |
| `R-01-03` | HK-01 | Cross-provider or shared logical-reference vocabulary is not modelled | HK10: `OUT-BOUNDARY`; requires explicit semantic modelling |
| `R-01-05` | HK-01 | Cross-major migration guidance beyond "majors are explicit breaking boundaries" | HK10: `OUT-BOUNDARY`; future contract-lifecycle work |
| `R-02-01` | HK-02, HK-02A | Persisted-world format migration; V1 fails closed and no accepted migration path exists | HK10: `OUT-BOUNDARY`; future persisted-format lifecycle work |
| `R-02-04` | HK-02 | Richer display names and localization layered over the narrow stable identity syntax | HK10: `OUT-BOUNDARY`; content/UI semantics |
| `R-02-05` | HK-02 | Cross-world and external asset references | HK10: `OUT-BOUNDARY`; requires a reviewed reference model |
| `R-02A-01` | HK-02A, HK-05 | A producer that embeds an object identity in opaque payload bytes without declaring it cannot be detected generically | HK10: `OUT-BOUNDARY`; opaque-extension contract deliberately requires declared identity/reference surfaces |
| `R-02A-02` | HK-02A, HK-08B | Whole-world revision/hash CAS; no per-resource concurrency and no automatic merge of disjoint edits | HK10: `OUT-BOUNDARY`; closure stress-tests deterministic stale rejection/replan but does not invent finer concurrency/merge semantics. H0S remains the default post-GATE measurement venue |
| `R-03-01` | HK-03 | Query evaluation is bounded in output, not in scan cost; no index, sublinear complexity or world-size-independent CPU claim | HK10: `OUT-BOUNDARY`; post-GATE scale/asymptotic work |
| `R-03-02` | HK-03, HK-08A | Cursors are deterministic/integrity-framed continuation tokens, not authenticated capabilities, and are not an authorization boundary | HK10: `OUT-BOUNDARY`; authentication/authorization is not an H0 claim |
| `R-03-03` | HK-03 | No multi-command snapshot lease; a state source may advance between calls | HK10: `OUT-BOUNDARY`; a lease would be a new consistency capability; stale anchors fail closed |
| `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A, HK-09B | Canonical state, receipts/journal, snapshots/rebase evidence, replay staging and the accepted JSONL/MCP production hosts remain process-local; HK09B proves fail-closed in-process aggregate publication under owned rejection/expiry/interruption but explicitly claims no process-kill/power-loss durability | HK10: `OUT-BOUNDARY`; WAL/fsync/process-kill/power-loss durability is not claimed; fresh-session recovery uses accepted snapshot import |
| `R-04-02` | HK-04, HK-06C, HK-07A, HK-07B, HK-08B | Multi-process/distributed writers, cross-process locking and multi-agent merge/coordination | HK10: `OUT-BOUNDARY`; H0 proves local deterministic stale recovery, not distributed coordination |
| `R-04-05` | HK-04 | Asymptotic performance for very large worlds | HK10: `OUT-BOUNDARY`; post-GATE scale evidence |
| `R-05-01` | HK-05 | Secondary diagnostics whose own source or dependency traversal is ambiguous are deferred and not visible in the same validation pass; the contract is iterative for those | HK10: `OUT-BOUNDARY`; accepted iterative semantics remain under regression and adding a partial-report marker would change public semantics |
| `R-06B-04` | HK-06B, HK-06C, HK-07A, HK-07B | Gameplay/runtime state such as transforms, clocks, schedules, physics, animation, AI and simulation remains outside canonical authored `WorldState`/replay and the accepted local hosts unless a later reviewed WP promotes it | HK10: `OUT-BOUNDARY`; H1/runtime/gameplay semantics |
| `R-06B-05` | HK-06B, HK-06C | Snapshot import/replay uses explicit local lineage roots; merging, rebasing or reconciling independent authored histories would require a future explicit contract | HK10: `OUT-BOUNDARY`; new lineage/merge semantics |
| `R-06B-06` | HK-06B, HK-06C, HK-07A, HK-08A, HK-08B, HK-09B | Whole-state snapshots, full semantic comparisons, staged replay and process-local journal storage are accepted only inside finite H0 bounds; streaming/chunking/arbitrary large-history or large-world throughput is not claimed | HK10: `OUT-BOUNDARY`; bounded endurance is accepted, arbitrary streaming/scale is not |
| `R-06C-01` | HK-06C | Lost-response replay retry is not durable/idempotent: after successful replay the local journal is non-empty, so an identical blind retry is rejected rather than acknowledged from a durable replay receipt | HK10: `OUT-BOUNDARY`; durable replay receipts would add persistence/idempotency semantics |
| `R-06C-02` | HK-06C | Future journal/snapshot versions currently fail as unsupported; no migration capability or cross-version replay path exists until separately reviewed | HK10: `OUT-BOUNDARY`; future lifecycle/migration work |
| `R-07A-04` | HK-07A, HK-07B, HK-09A | Authentication, remote tenancy, encrypted/network transport semantics and hostile OS/process isolation are not claimed by the local stdin/stdout/stdio hosts | HK10: `OUT-BOUNDARY`; external/remote security is not repository-local H0 |
| `R-07B-01` | HK-07B | For canonical identities requiring bounded MCP surrogates, the transport tool handle is deterministic only for a given composed inventory and may change when capabilities are added/removed | HK10: `OUT-BOUNDARY`; canonical identity is stable and clients rediscover transport handles |
| `R-07B-02` | HK-07B | The accepted adapter pins `ModelContextProtocol.Core` 2.2.0 and MCP protocol `2025-11-25`; later SDK/protocol revisions are not covered by the accepted candidate | HK10: `OUT-BOUNDARY`; future upgrade requires dependency/license/conformance review |
| `R-07B-03` | HK-07B | Standards-compatible MCP JSON-RPC over local stdio is exercised directly, but every vendor/client product is not certified | HK10: `OUT-BOUNDARY`; vendor-specific certification is not H0 |
| `R-08A-01` | HK-08A | `authoring.journal.read@1.0` intentionally remains a complete, potentially large response for compatibility even though v2 provides bounded paging | HK10: `OUT-BOUNDARY`; changing/deprecating v1 is an explicit compatibility-lifecycle decision |
| `R-08B-01` | HK-08B | Unknown future material-resource vocabularies are preserved visibly by recovery but are not silently given a fabricated inspection contract | HK10: `OUT-BOUNDARY`; future resource vocabularies require reviewed semantics |
| `R-09B-01` | HK-09B | The 5 s execution envelope is cooperative at dispatch/publication seams, not a hard scheduler that forcibly aborts arbitrary managed CPU or enforces an exact memory/output ceiling | HK10: `OUT-BOUNDARY`; hard preemptive CPU/memory governance and shipping SLOs remain outside H0 |

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

Reading it: the cost is per **transaction**, not per edited object, because identity is whole-world. A batched authoring flow pays it once; an unbatched one pays it per edit. `WP-HK-08A` supplies the accepted representative batching primitive, `WP-HK-08B` supplies the measured reference-client interaction/recovery baseline, `WP-HK-09B` supplies the finite H0 resource envelope, `WP-HK-10` supplies bounded-session closure inside that envelope, and accepted `WP-HK-GATE` found the representative public authoring contract usable without promoting finer concurrency into an H1 blocker. None changes the open finer-concurrency or arbitrary large-world scaling decision.

Limits of the measurement, which is **not evidence**: it is not bound to a candidate SHA, was not produced by canonical validation, and ran on shared container CPU. It covers `Arkus.Game.Core` and `Arkus.Game.World` only, so it is a lower bound — a real commit also pays HK-04 plan/change-set work, HK-05 diagnostic aggregation and the HK-06A journal entry. The synthetic world's reference density is uniform and real content will differ.

What would make this a live decision rather than recorded context: a representative target whose authored object count approaches the high tens of thousands, or a requirement for concurrent disjoint writers, which batching/recovery do not eliminate and whole-world CAS rejects by design.

## D. Entries declared more than once

A residual restated by several workpacks is a signal that it sits on a seam rather than inside one contract.

| Theme | Entries | Declared by |
|---|---|---|
| Canonical schema vocabulary subset | `R-01-01` | HK-01, HK-03, HK-04 |
| In-memory lifetime and durability | `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A, HK-09B |
| Undeclared identity inside opaque payloads | `R-02A-01` | HK-02A, HK-05 |
| Size, byte, latency and resource budgets | `R-02A-04`, `R-04-04`, `R-05-06`, `R-06A-05`, `R-07A-01`, `R-09B-01` | HK-02A, HK-04, HK-05, HK-06A, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A, HK-09B, HK-10 |
| Large-world scaling with no owner | `R-03-01`, `R-04-05`, `R-06B-06` | HK-03, HK-04, HK-06B, HK-06C, HK-07A, HK-08A, HK-08B, HK-09B |
| Replay/compatibility | `R-06A-03`, `R-06B-02`, `R-06C-02` | HK-06A, HK-06B, HK-06C |
| Transport framing/projection parity | `R-01-08`, `R-06A-07`, `R-06B-03` | HK-01, HK-06A, HK-06B, HK-07A, HK-07B |
| Cross-process/concurrent writers | `R-04-02`, `R-02A-02` | HK-04, HK-06C, HK-07A, HK-07B, HK-02A, HK-08B |

The budget theme uses five different mechanisms. HK-08A accepted **interaction primitives** such as representative batching and bounded v2 journal/page reads. HK-08B accepted **measured end-to-end reference-client baselines and executable regression budgets**. HK-09A constrains **which host powers exist**. HK-09B accepted **enforced finite H0 caps** for request/batch/page/depth/world/session resources plus cooperative execution and fail-closed process-local publication. HK10 accepted **strict causal closure and bounded long-session endurance inside those finite caps**. HK-GATE then composed those guarantees through the real public paths and a fresh independent MCP client without promoting the remaining scale/concurrency classes into H0.

None of those workpacks owns arbitrary large-world asymptotic optimization, shipping latency SLOs, hard real-time/preemptive CPU-memory isolation or richer concurrent-writer semantics. `R-03-01`, `R-04-05`, `R-06B-06`, `R-04-02`, `R-09B-01` and the concurrency half of `R-02A-02` therefore remain accepted H0 `OUT-BOUNDARY` classes. The roadmap's post-GATE H0S track provides a measurement venue without pretending those residuals are already closed or owned.

## E. Deliberate non-goals already stated as such

Recorded so they are not rediscovered as findings: conservative same-major compatibility that prefers false-breaking over false-compatible (HK-01); no semantic denylist guessing at engine allusions in opaque tokens (HK-01); semantic fingerprints as diagnostics rather than correctness authorities (HK-01); forward compatibility as extension-envelope compatibility rather than unknown-structure guessing (HK-02); completeness tied to the current accepted state universe rather than predicted future shapes (HK-03, HK-05, HK-06B, HK-06C); caller ownership of idempotency keys (HK-04); `MutationAuthorityInspector` as defence in depth and explicitly not a completeness argument (HK-04); ordinary argument exceptions for direct CLR misuse outside the dispatcher contract (HK-05); runtime observation content not modelled (HK-06A); snapshot import intentionally starting a new local lineage rather than merging mutation history (HK-06B); replay intentionally refusing to splice into pre-existing local mutation history (HK-06C); deterministic journal fingerprints/entry IDs are identities and consistency checks, not cryptographic signatures or provenance authentication (HK-06C); HK07A/HK07B cancellation/timeout intentionally ending at admission so canonical dispatch truth wins once execution starts; accepted host state intentionally remaining process-local and reconstructed in a new process only through accepted snapshot/journal capabilities; MCP-specific naming remaining transport framing rather than canonical identity; over-limit MCP tool names being rediscovered transport handles rather than durable semantic identifiers; HK07B not claiming every vendor MCP client or future protocol revision; HK08A journal cursor integrity framing intentionally not being authentication/authorization; HK08A v2 partial pages intentionally not being complete HK06C replay artifacts until a client reconstructs the accepted complete journal; HK08A's representative 96-operation envelope intentionally not being a permanent product/resource promise; HK08B recovery being explicit re-planning rather than automatic merge/per-resource conflict elimination; HK08B's elapsed limit being a coarse hosted-runner regression guard rather than a product latency SLO; HK08B's response-byte allowance permitting bounded truthful additive payload growth while its request-count budget forbids extra recovery chatter; HK09A host containment intentionally not being authentication, hostile-OS sandboxing, cloud tenancy/security or anti-cheat; HK09A generic `ContractComposer` intentionally remaining policy-agnostic while H0 admission is enforced when a composed inventory crosses into the accepted neutral-projection boundary; HK09B resource enforcement intentionally being a finite H0 envelope rather than arbitrary production-scale streaming/SLO guarantees; HK09B execution budgeting intentionally being cooperative rather than hard real-time/preemptive scheduling; HK09B process-local publication integrity intentionally not claiming WAL/fsync/process-kill/power-loss durability; frozen `arkus.reference.jsonl@1` physical framing remaining a predecessor transport contract rather than being silently redefined by HK09B; HK10 strict closure intentionally not adding product architecture to eliminate accepted out-of-boundary classes; HK10 bounded endurance intentionally not being a shipping performance or leak theorem; HK-GATE intentionally being closure/readiness proof rather than a new semantic architecture workpack; prose not mechanically proving every future semantic interpretation, and unusual license terms failing closed to human review (HK-00A).

## F. CITY planning residuals

The rows below transcribe the residual boundaries declared by accepted `WP-CITY-02` candidate `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8` (PASS `#5266113192`) and accepted `WP-CITY-05` candidate `10d1528b0354b16a614fb10a3933a25b32f15f28` (PASS `#5267776704`). They are not part of the 53-row H0 closure universe and carry no HK10/HK-GATE classification.

| ID | Declared by | Residual | Owner | Status |
|---|---|---|---|---|
| `R-CITY02-01` | CITY-02, CITY-05 | Exact parcel polygons/site placement and final realized footprint/frontage dimensions remain unproved; CITY-05 now supplies the accepted reusable exterior grammar plus bounded host/site witnesses | CITY-03 selection + later realized-geometry owner | `DEFERRED` — CITY-05 PASS `#5267776704` closes the reusable grammar/host-fit planning portion but deliberately does not freeze exact parcels or final realized dimensions |
| `R-CITY02-02` | CITY-02, CITY-05 | Exact interior layouts and layered discovery routes remain downstream of the interior-priority backlog and reviewed exterior shell/access handoff | CITY-06 | `DEFERRED` |
| `R-CITY02-03` | CITY-02, CITY-05 | Exact retained-seed boundary/selection remains unchosen | CITY-03 | `DEFERRED` |
| `R-CITY02-04` | CITY-02, CITY-05 | Realized street lengths/grades/turning/clearances, Unity geometry and blockout traversal/reactive-density measurements remain unavailable until sufficient realized geometry has an accepted owner | CITY-04 for the accepted seed; later realized-geometry owner for broader measurements | `DEFERRED` |
| `R-CITY02-05` | CITY-02, CITY-05 | Runtime schedules, access permissions over time, beliefs, dialogue, relationships, decisions, incidents and off-screen simulation are not authored by CITY planning | Living World / runtime owners | `DEFERRED` |
| `R-CITY02-06` | CITY-02, CITY-05 | Final canonical game IDs, Unity native locators, NPC bindings, narrative/backstories, minigame mechanics and State-1→State-2 production timing remain outside the accepted CITY-02/05 planning claims | later product/runtime/production owners | `DEFERRED` |
| `R-CITY05-01` | CITY-05 | Final asset/prefab/module inventory and imported dependency adoption are not selected by the exterior grammar | ART / H1 / later production owners | `DEFERRED` |
| `R-CITY05-02` | CITY-05 | Arkus public capability/schema implementation for reviewed environment discovery is not implemented by the non-canonical requirements projection | owning H1 / later authoring WPs | `DEFERRED` |
| `R-CITY05-03` | CITY-05 | Keeper realization of the accepted city grammar remains unbuilt | CITY-07 after H1-GATE | `DEFERRED` |
| `R-CITY05-04` | CITY-05 | Public authoring/reuse-cost proof for the reviewed city grammar remains unproved | CITY-08 | `DEFERRED` |

The CITY-02 Q6 class-ceiling/reserve rule and CITY-05 access/host-fit rules are **not** residuals: they are accepted fail/reopen conditions. If downstream composition exceeds a programme envelope, breaches a district cap, needs protected reserve, loses an inherited required access role or cannot satisfy the reviewed host/coverage posture, the causal claim must reopen rather than being recorded as deferred success.

## G. PA research residuals

The row below transcribes the empirical/runtime boundary explicitly deferred by accepted `WP-PA-03` candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927` (PASS `#5270421825`). It is not part of the 53-row H0 closure universe and carries no HK10/HK-GATE classification.

| ID | Declared by | Residual | Owner | Status |
|---|---|---|---|---|
| `R-PA03-01` | PA-03 | Numeric tuning/representation, final relationship-role-obligation schema, storage/index/query/API design, scale/performance, persistence/save/load/migration, UI/redaction/debug presentation, exact cross-system relationship-change tuning and taxonomy, additional affect dimensions, FULL↔ABSTRACT equivalence, authoring/content-scale/fun evidence and H4 implementation proof remain deliberately unproved | future H4/runtime consumers plus the named PA cause owners where applicable | `DEFERRED` |

PA-03's accepted semantic constraints are **not** residuals: directed A→B stance, explicit structural-role/obligation distinctions, lifecycle-bearing obligations, isolated trust/affinity/fear counterfactuals, receiver-owned relationship reasoning, bounded-discovery composition and private-third-party knowledge boundaries are accepted research requirements. A later consumer that cannot preserve them must reopen/falsify the relevant PA-03 finding rather than recording the failure as deferred success.

## Maintenance

DocSync for an accepted workpack appends that workpack's declared residuals here and updates any entry the workpack closed. HK10 closure classification lives authoritatively in `Docs/evidence/WP-HK-10/RESIDUAL_RISK.md`; accepted HK-GATE reconciliation lives authoritatively in `Docs/evidence/WP-HK-GATE/RESIDUAL_RISK.md` and proves that the same 53-row universe was consumed without reclassification. H0 is complete.

Later H1/H0S/CITY/future work should update a row only when accepted evidence actually closes it, changes its declared owner, or introduces a new residual. Adding an entry is cheap and carries no obligation. Removing one requires citing the accepted evidence that closed it.
