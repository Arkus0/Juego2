# Residual Ledger — declared residuals across accepted H0 workpacks

Version: 1.0 — 2026-09-19

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

Version 1.0 is a transcription pass. `OUT-BOUNDARY` and `DEFERRED` are recorded only where the declaring workpack states them explicitly; everything requiring judgement stays `UNCLASSIFIED` on purpose. Classification is deliberate work and is not done here.

## A. Trusted base

One class, declared by `WP-HK-00`, `WP-HK-01`, `WP-HK-02`, `WP-HK-03`, `WP-HK-05` and `WP-HK-06A`, and authorised by the trusted-base rule of `FOUNDATIONAL_PROOF_STANDARD.md`.

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
| `R-02A-04` | HK-02A | Total payload bytes, dependency count and world size not capped | HK-08 | `DEFERRED` |
| `R-03-01` | HK-03 | Query evaluation is bounded in output, not in scan cost; no index or asymptotic claim | future scale work | `DEFERRED` |
| `R-04-04` | HK-04 | Resource/cost bounds beyond the 64-operation request limit | HK-08, HK-09 | `DEFERRED` |
| `R-05-05` | HK-05 | Unity/engine validation: scene serialization, prefab/component rules, editor constraints | H1 | `DEFERRED` |
| `R-05-06` | HK-05 | Whole-world size and latency budgets are not product guarantees | HK-08 | `DEFERRED` |
| `R-06A-02` | HK-06A | Whether an imported snapshot starts a new local lineage or retains external evidence | HK-06B | `DEFERRED` |
| `R-06A-03` | HK-06A | Replay interpretation, missing/reordered/tampered entry behaviour, journal/snapshot version compatibility | HK-06C | `DEFERRED` |
| `R-06A-04` | HK-06A | Field-level semantic diff; affected-resource identity is journal metadata only | HK-06B | `DEFERRED` |
| `R-06A-05` | HK-06A | Journal growth and pagination; compact interaction budgets | HK-08 | `DEFERRED` |
| `R-06A-07` | HK-06A | Transport/storage framing may not redefine journal meaning | HK-07A, HK-07B | `DEFERRED` |

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
| `R-02A-02` | HK-02A | Whole-world revision/hash CAS; no per-resource concurrency and no automatic merge of disjoint edits | candidate to fall inside HK-09, which claims deterministic stale-revision and conflicting-writer tests |
| `R-03-02` | HK-03 | Cursors are deterministic continuation tokens, not authenticated capabilities, and are not an authorization boundary | candidate to fall inside HK-09's capability boundary |
| `R-03-03` | HK-03 | No multi-command snapshot lease; a state source may advance between calls | stale anchors fail closed rather than mixing revisions |
| `R-04-01` | HK-04, HK-06A | Canonical state, idempotency receipts and the journal share one in-memory session lifetime; no durable replay protection across restart | both declaring WPs require a future durable substrate to preserve the aggregate atomically |
| `R-04-02` | HK-04 | Multi-process and distributed writers | requires a later authoritative persistence/locking substrate |
| `R-05-01` | HK-05 | Secondary diagnostics whose own source or dependency traversal is ambiguous are deferred and not visible in the same validation pass; the contract is iterative for those | HK-05 declares this as claimed semantics, not a gap. Open question for classification: `arkus.world-validation-result/v1` carries no field indicating the report is partial |

## D. Entries declared more than once

A residual restated by several workpacks is a signal that it sits on a seam rather than inside one contract.

| Theme | Entries | Declared by |
|---|---|---|
| Canonical schema vocabulary subset | `R-01-01` | HK-01, HK-03, HK-04 |
| In-memory lifetime and durability | `R-04-01` | HK-04, HK-06A |
| Undeclared identity inside opaque payloads | `R-02A-01` | HK-02A, HK-05 |
| Size, byte and latency budgets | `R-02A-04`, `R-04-04`, `R-05-06`, `R-06A-05` | HK-02A, HK-04, HK-05, HK-06A |

The budget theme is the densest: four accepted workpacks have each deferred a different budget to HK-08 without anyone owning the aggregate.

## E. Deliberate non-goals already stated as such

Recorded so they are not rediscovered as findings: conservative same-major compatibility that prefers false-breaking over false-compatible (HK-01); no semantic denylist guessing at engine allusions in opaque tokens (HK-01); semantic fingerprints as diagnostics rather than correctness authorities (HK-01); forward compatibility as extension-envelope compatibility rather than unknown-structure guessing (HK-02); completeness tied to the current accepted state universe rather than predicted future shapes (HK-03, HK-05); caller ownership of idempotency keys (HK-04); `MutationAuthorityInspector` as defence in depth and explicitly not a completeness argument (HK-04); ordinary argument exceptions for direct CLR misuse outside the dispatcher contract (HK-05); runtime observation content not modelled (HK-06A); prose not mechanically proving every future semantic interpretation, and unusual license terms failing closed to human review (HK-00A).

## Maintenance

DocSync for an accepted workpack appends that workpack's declared residuals here and updates any entry the workpack closed. A residual that exists only in `Docs/evidence/**` and never reaches this ledger is invisible to `WP-HK-10`'s audit.

Adding an entry is cheap and carries no obligation. Removing one requires citing the accepted evidence that closed it.
