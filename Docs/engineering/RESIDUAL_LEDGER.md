# Residual Ledger — declared residuals across accepted H0 workpacks

Version: 1.8 — 2026-09-20

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

Version 1.0 was the initial transcription pass. Version 1.1 records accepted HK06B. Version 1.2 records accepted HK06C: it closes only the replay/compatibility residuals that HK06C explicitly owned, folds repeated lifetime/concurrency/runtime/history/scale seams into their existing entries, and transcribes HK06C-specific residuals without assigning new classifications or owners not present in accepted evidence. Version 1.3 re-points only planned ownership after the pre-implementation `HK08 → HK08A/HK08B` and `HK09 → HK09A/HK09B` split; it makes no new residual classification. Version 1.4 records accepted HK07A: it closes the deterministic JSONL/reference-projection half of the transport seam, leaves MCP cross-transport parity to HK07B, folds repeated process-lifetime/concurrency/gameplay/scale seams into existing entries, and transcribes HK07A-specific host/resource residuals only where the accepted downstream owner is explicit. Version 1.5 records accepted HK07B: it closes the named MCP/JSONL projection-parity and local journal/snapshot framing obligations, records the actual MCP dependency-adoption revalidation, folds repeated process-lifetime/resource/remote-host seams into existing entries, and transcribes only the HK07B residuals that remain beyond the accepted local-stdio projection claim. Version 1.6 records accepted HK08A: it closes the named batching/compact/pagination half of the interaction-efficiency seam, records bounded journal paging as explicit v2 while preserving complete v1 compatibility, leaves stale-plan recovery and measured end-to-end interaction budgets to HK08B, and folds repeated process-lifetime/resource/cursor-authentication seams into their existing entries. Version 1.7 records accepted HK08B: it closes the named stale-CAS recovery/repair/benchmark residual, records exact-lineage fail-closed recovery and executable reference-client interaction budgets, folds repeated durability/concurrency/resource-limit seams into existing entries, and adds the explicitly declared unknown-future-material-resource vocabulary residual without assigning a speculative owner. Version 1.8 records accepted HK09A: it closes the caller-selected `--file PATH`/host-capability residual by removing that production authority and enforcing H0 admission at the neutral-projection boundary, folds repeated resource/persistence and remote/OS-security exclusions into their existing entries, and leaves HK09B resource/interruption ownership and H1/external-system authority boundaries unchanged.

## A. Trusted base

One class, declared by `WP-HK-00`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-05`, `WP-HK-06A`, `WP-HK-06B`, `WP-HK-06C`, `WP-HK-07A`, `WP-HK-07B`, `WP-HK-08A`, `WP-HK-08B` and `WP-HK-09A`, and authorised by the trusted-base rule of `FOUNDATIONAL_PROOF_STANDARD.md`.

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
| `R-00A-02` | HK-00A, HK-07B | External upstream audit snapshot must be revalidated at the actual adoption point | adoption WP/ADR | `CLOSED-BY` — HK07B PASS `#5259854121`; the actual MCP adoption pins `ModelContextProtocol.Core` 2.2.0, records exact upstream/license identity and preserves an explicit Arkus conformance/replacement boundary |
| `R-00A-05` | HK-00A | Concrete H1 engine abstractions and Unity capability definitions not selected | H1 | `DEFERRED` |
| `R-01-07` | HK-01 | No external plugin loader; a future reviewed loader must extend the independent universe rather than trust registration metadata | future loader WP | `DEFERRED` |
| `R-01-08` | HK-01, HK-07A, HK-07B | Concrete MCP/JSONL projection parity | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121`; MCP is accepted as a genuine second projection of `arkus.neutral-projection@1`, with external stdio conformance across the representative accepted H0 surface and canonical composition remaining semantic authority |
| `R-01-09` | HK-01 | Unity/editor types absent; H1 instantiates the scoped-provider boundary | H1 | `DEFERRED` |
| `R-02A-04` | HK-02A | Total payload bytes, dependency count and world size not capped | HK-09B | `DEFERRED` |
| `R-04-04` | HK-04, HK-08A | Byte quota on opaque extension payloads and final operation/request resource envelope are not fixed by the accepted representative 96-operation HK08A shape | HK-09B | `DEFERRED` |
| `R-05-05` | HK-05 | Unity/engine validation: scene serialization, prefab/component rules, editor constraints | H1 | `DEFERRED` |
| `R-05-06` | HK-05, HK-08A, HK-08B | Whole-world size and product latency/throughput/resource budgets are not product guarantees; HK08B supplies only reviewed H0 reference-client regression budgets | HK-09B for enforced content/resource caps; later product evidence for shipping SLOs | `DEFERRED` — HK08B PASS `#5260337340` closes the measured H0 interaction-baseline/regression-budget half, but its 8x elapsed guard is explicitly not a product latency SLO |
| `R-06A-02` | HK-06A | Whether an imported snapshot starts a new local lineage or retains external evidence | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; snapshot import is an explicit canonical rebase that starts `new-local-lineage` and emits truthful rebase evidence |
| `R-06A-03` | HK-06A | Replay interpretation, missing/reordered/tampered entry behaviour, journal/snapshot version compatibility | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; deterministic replay, fail-closed evidence handling and explicit accepted-version compatibility are now canonical capabilities |
| `R-06A-04` | HK-06A | Field-level semantic diff; affected-resource identity is journal metadata only | HK-06B | `CLOSED-BY` — HK06B PASS `#5258130916`; deterministic semantic diff covers the complete accepted authored-resource model |
| `R-06A-05` | HK-06A, HK-07A, HK-08A, HK-08B | Session-local provenance journal growth remains unbounded internally; HK08A bounds the explicit v2 public read and HK08B may consume the complete current local journal for ancestry proof, but neither claims incremental storage/indexing or long-session growth control | HK-10 for bounded-session growth evidence inside the HK-09B envelope | `DEFERRED` — accepted paging/recovery semantics leave storage growth open to the named owner |
| `R-06A-07` | HK-06A, HK-07A, HK-07B | Transport/storage framing may not redefine journal meaning | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121`; JSONL and MCP preserve the accepted journal/replay semantics through the same neutral/canonical path. Process/storage lifetime remains separately tracked in `R-04-01` |
| `R-06B-02` | HK-06B | Deterministic journal replay and policy for accepted journal/snapshot version combinations are not implemented by snapshot portability | HK-06C | `CLOSED-BY` — HK06C PASS `#5259530509`; replay is a distinct canonical authority that consumes HK06A/HK06B artifacts and reports explicit supported/unsupported compatibility |
| `R-06B-03` | HK-06B, HK-07A, HK-07B | File/JSONL/MCP/network/cloud transport framing and persistence may not redefine snapshot semantics | HK-07B | `CLOSED-BY` — HK07B PASS `#5259854121` closes the named local file/JSONL/MCP transport-parity obligation: snapshot export/import/diff/replay retain equivalent accepted meaning through MCP. Durability and remote/cloud semantics remain separately recorded in `R-04-01` and `R-07A-04` |
| `R-07A-01` | HK-07A, HK-07B, HK-08A, HK-08B, HK-09A | Admission-only cancellation/timeout and current transport/interaction framing/budgets are not general post-dispatch execution-time, CPU/memory, request/output-size or resource-governance guarantees | HK-09B | `DEFERRED` — HK09A constrains which host powers exist but explicitly leaves final input/batch/page/depth/time/size/resource limits and persistence/interruption integrity to HK09B |
| `R-07A-02` | HK-07A | `--file PATH` accepts an explicit caller path and HK07A does not claim filesystem sandbox/allow-list, path-containment or broader host-capability policy | HK-09A | `CLOSED-BY` — HK09A PASS `#5260496498`; the production H0 executable rejects `--file` before the legacy framing host can open any caller-selected path, and H0 host-capability admission is enforced at neutral-projection construction below conforming transports |
| `R-07A-03` | HK-07A, HK-07B, HK-08A, HK-08B | Structured stale-revision recovery, repair ergonomics and measured end-to-end interaction budgets remain outside the earlier accepted interaction primitives | HK-08B | `CLOSED-BY` — HK08B PASS `#5260337340`; `arkus.world-conflict-recovery@1` now provides exact-lineage fail-closed recovery, affected-only reinspection, normal retry authority, executable reference-client budgets and JSONL↔MCP conformance |

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
| `R-02A-02` | HK-02A, HK-08B | Whole-world revision/hash CAS; no per-resource concurrency and no automatic merge of disjoint edits | HK08B PASS `#5260337340` closes the cheap same-lineage recovery problem under whole-world CAS, but deliberately does not add per-resource locking/CAS, hidden merge or conflict elimination. Finer concurrent-writer semantics remain an open post-GATE product decision unless measured evidence promotes them. Measured cost context below |
| `R-03-01` | HK-03 | Query evaluation is bounded in output, not in scan cost; no index, sublinear complexity or world-size-independent CPU claim | HK-03 defers this to "future scale work", which does not exist. H0 gates on a micro-world, so this is a candidate for `OUT-BOUNDARY`; the post-GATE H0S track may measure it but is not a binding owner |
| `R-03-02` | HK-03, HK-08A | Cursors are deterministic/integrity-framed continuation tokens, not authenticated capabilities, and are not an authorization boundary | HK08A detects altered v2 journal continuation context but explicitly does not claim a secret MAC/auth boundary; accepted HK09A constrains host authority without turning cursors into authentication capabilities |
| `R-03-03` | HK-03 | No multi-command snapshot lease; a state source may advance between calls | stale anchors fail closed rather than mixing revisions |
| `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A | Canonical state, mutation idempotency receipts/journal, snapshots, snapshot-rebase receipts/evidence, replay staging and the accepted JSONL/MCP production hosts remain process-local; HK08B recovery proves ancestry only from complete current local history and HK09A deliberately does not add crash-consistent persistence mechanics | fresh-process reconstruction remains explicit snapshot import/journal replay; HK09B owns interruption/import integrity at the persistence boundary it accepts, while general automatic crash/restart recovery and durable recovery ancestry remain outside the current accepted claim |
| `R-04-02` | HK-04, HK-06C, HK-07A, HK-07B, HK-08B | Multi-process/distributed writers, cross-process locking and multi-agent merge/coordination | HK08B handles local same-lineage stale recovery but explicitly does not add multi-process coordination; the post-GATE H0S track remains the default measurement venue, not a binding owner |
| `R-04-05` | HK-04 | Asymptotic performance for very large worlds | split out of `R-04-04`: HK-04 defers it to "later harness budget/guardrail work", which does not exist. Same candidate as `R-03-01`; H0S may measure it after GATE |
| `R-05-01` | HK-05 | Secondary diagnostics whose own source or dependency traversal is ambiguous are deferred and not visible in the same validation pass; the contract is iterative for those | HK-05 declares this as claimed semantics, not a gap. Open question for classification: `arkus.world-validation-result/v1` carries no field indicating the report is partial |
| `R-06B-04` | HK-06B, HK-06C, HK-07A, HK-07B | Gameplay/runtime state such as transforms, clocks, schedules, physics, animation, AI and simulation remains outside canonical authored `WorldState`/replay and the accepted local hosts unless a later reviewed WP promotes it | no specific owner named by HK06B/HK06C/HK07A/HK07B |
| `R-06B-05` | HK-06B, HK-06C | Snapshot import/replay uses explicit local lineage roots; merging, rebasing or reconciling independent authored histories would require a future explicit contract | HK06C rejects a replay target with pre-existing local mutation history rather than splicing histories |
| `R-06B-06` | HK-06B, HK-06C, HK-07A, HK-08A, HK-08B | Whole-state snapshot payloads, full semantic comparisons, staged replay and process-local journal storage are accepted for H0; streaming/chunking/large-history or large-world throughput is not claimed | HK08A bounds v2 journal responses and HK08B consumes current local history for recovery, but neither claims incremental storage/indexing or large-history throughput; H0S may measure larger-scale needs after GATE |
| `R-06C-01` | HK-06C | Lost-response replay retry is not durable/idempotent: after successful replay the local journal is non-empty, so an identical blind retry is rejected rather than acknowledged from a durable replay receipt | no owner named by HK06C |
| `R-06C-02` | HK-06C | Future journal/snapshot versions currently fail as unsupported; no migration capability or cross-version replay path exists until separately reviewed | no owner named by HK06C |
| `R-07A-04` | HK-07A, HK-07B, HK-09A | Authentication, remote tenancy, encrypted/network transport semantics and hostile OS/process isolation are not claimed by the local stdin/stdout/stdio hosts | HK09A PASS `#5260496498` explicitly keeps authentication/multi-user cloud security, external network service security and OS sandboxing outside its repository-local H0 containment claim; no accepted H0 owner is named for these external/remote concerns |
| `R-07B-01` | HK-07B | For canonical identities requiring bounded MCP surrogates, the transport tool handle is deterministic only for a given composed inventory and may change when capabilities are added/removed | exact canonical identity remains the stable semantic identity and clients are required to rediscover; no later owner is named |
| `R-07B-02` | HK-07B | The accepted adapter pins `ModelContextProtocol.Core` 2.2.0 and MCP protocol `2025-11-25`; later SDK/protocol revisions are not covered by the accepted candidate | any upgrade requires dependency/license review and rerunning HK07B conformance; no separate owner is named |
| `R-07B-03` | HK-07B | Standards-compatible MCP JSON-RPC over local stdio is exercised directly, but every vendor/client product is not certified | vendor-specific orchestration/configuration remains outside HK07B; no later owner is named |
| `R-08A-01` | HK-08A | `authoring.journal.read@1.0` intentionally remains a complete, potentially large response for compatibility even though v2 provides bounded paging | removing, deprecating or changing v1 requires a later explicit contract-lifecycle decision; no later owner is named |
| `R-08B-01` | HK-08B | Unknown future material-resource vocabularies are preserved visibly by recovery but are not silently given a fabricated inspection contract | current HK06A material resources are object/extension keys; extending that vocabulary requires a reviewed semantic change and no later owner is named |

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

Reading it: the cost is per **transaction**, not per edited object, because identity is whole-world. A batched authoring flow pays it once; an unbatched one pays it per edit. `WP-HK-08A` supplies the accepted representative batching primitive, and `WP-HK-08B` now supplies the measured reference-client interaction/recovery baseline. Neither result changes the open finer-concurrency decision.

Limits of the measurement, which is **not evidence**: it is not bound to a candidate SHA, was not produced by canonical validation, and ran on shared container CPU. It covers `Arkus.Game.Core` and `Arkus.Game.World` only, so it is a lower bound — a real commit also pays HK-04 plan/change-set work, HK-05 diagnostic aggregation and the HK-06A journal entry. The synthetic world's reference density is uniform and real content will differ.

What would make this a live decision rather than recorded context: a representative target whose authored object count approaches the high tens of thousands, or a requirement for concurrent disjoint writers, which batching/recovery do not eliminate and whole-world CAS rejects by design.

## D. Entries declared more than once

A residual restated by several workpacks is a signal that it sits on a seam rather than inside one contract.

| Theme | Entries | Declared by |
|---|---|---|
| Canonical schema vocabulary subset | `R-01-01` | HK-01, HK-03, HK-04 |
| In-memory lifetime and durability | `R-04-01` | HK-04, HK-06A, HK-06B, HK-06C, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A |
| Undeclared identity inside opaque payloads | `R-02A-01` | HK-02A, HK-05 |
| Size, byte, latency and resource budgets | `R-02A-04`, `R-04-04`, `R-05-06`, `R-06A-05`, `R-07A-01` | HK-02A, HK-04, HK-05, HK-06A, HK-07A, HK-07B, HK-08A, HK-08B, HK-09A |
| Large-world scaling with no owner | `R-03-01`, `R-04-05`, `R-06B-06` | HK-03, HK-04, HK-06B, HK-06C, HK-07A, HK-08A, HK-08B |
| Replay/compatibility | `R-06A-03`, `R-06B-02`, `R-06C-02` | HK-06A, HK-06B, HK-06C |
| Transport framing/projection parity | `R-01-08`, `R-06A-07`, `R-06B-03` | HK-01, HK-06A, HK-06B, HK-07A, HK-07B |
| Cross-process/concurrent writers | `R-04-02`, `R-02A-02` | HK-04, HK-06C, HK-07A, HK-07B, HK-02A, HK-08B |

The budget theme now uses three different mechanisms. HK-08A has accepted **interaction primitives** such as representative batching and bounded v2 journal/page reads. HK-08B has accepted **measured end-to-end reference-client baselines and executable regression budgets**. HK-09A constrains **which host powers exist**, but intentionally does not convert those regression budgets into quotas. HK-09B owns **enforced caps** such as input/batch/page/depth/time/resource limits that fail closed. Journal/session growth itself is exercised by HK10 against the accepted HK09B resource envelope.

None of those workpacks owns arbitrary large-world asymptotic optimization, shipping latency SLOs or richer concurrent-writer semantics. `R-03-01`, `R-04-05`, `R-06B-06`, `R-04-02` and the concurrency half of `R-02A-02` therefore remain open unless deliberately classified. The roadmap's post-GATE H0S track provides a measurement venue without pretending those residuals are already closed or owned.

## E. Deliberate non-goals already stated as such

Recorded so they are not rediscovered as findings: conservative same-major compatibility that prefers false-breaking over false-compatible (HK-01); no semantic denylist guessing at engine allusions in opaque tokens (HK-01); semantic fingerprints as diagnostics rather than correctness authorities (HK-01); forward compatibility as extension-envelope compatibility rather than unknown-structure guessing (HK-02); completeness tied to the current accepted state universe rather than predicted future shapes (HK-03, HK-05, HK-06B, HK-06C); caller ownership of idempotency keys (HK-04); `MutationAuthorityInspector` as defence in depth and explicitly not a completeness argument (HK-04); ordinary argument exceptions for direct CLR misuse outside the dispatcher contract (HK-05); runtime observation content not modelled (HK-06A); snapshot import intentionally starting a new local lineage rather than merging mutation history (HK-06B); replay intentionally refusing to splice into pre-existing local mutation history (HK-06C); deterministic journal fingerprints/entry IDs are identities and consistency checks, not cryptographic signatures or provenance authentication (HK-06C); HK07A/HK07B cancellation/timeout intentionally ending at admission so canonical dispatch truth wins once execution starts; accepted host state intentionally remaining process-local and reconstructed in a new process only through accepted snapshot/journal capabilities; MCP-specific naming remaining transport framing rather than canonical identity; over-limit MCP tool names being rediscovered transport handles rather than durable semantic identifiers; HK07B not claiming every vendor MCP client or future protocol revision; HK08A journal cursor integrity framing intentionally not being authentication/authorization; HK08A v2 partial pages intentionally not being complete HK06C replay artifacts until a client reconstructs the accepted complete journal; HK08A's representative 96-operation envelope intentionally not being a permanent product/resource promise; HK08B recovery being explicit re-planning rather than automatic merge/per-resource conflict elimination; HK08B's elapsed limit being a coarse hosted-runner regression guard rather than a product latency SLO; HK08B's response-byte allowance permitting bounded truthful additive payload growth while its request-count budget forbids extra recovery chatter; HK09A host containment intentionally not being authentication, hostile-OS sandboxing, cloud tenancy/security or anti-cheat; HK09A generic `ContractComposer` intentionally remaining policy-agnostic while H0 admission is enforced when a composed inventory crosses into the accepted neutral-projection boundary; prose not mechanically proving every future semantic interpretation, and unusual license terms failing closed to human review (HK-00A).

## Maintenance

DocSync for an accepted workpack appends that workpack's declared residuals here and updates any entry the workpack closed. A residual that exists only in `Docs/evidence/**` and never reaches this ledger is invisible to `WP-HK-10`'s audit.

Adding an entry is cheap and carries no obligation. Removing one requires citing the accepted evidence that closed it.
