# Content Shape Backlog — forms of authorable state

Version: 1.2 — 2026-09-20

Status: **NON-BINDING reference.** This document creates no acceptance criterion, reopens no accepted guarantee, alters no workpack contract and commits the roadmap to nothing.

## Purpose

The bounded representative content-shape probe required by `FOUNDATIONAL_PROOF_STANDARD.md` is deliberately one scenario per applicable workpack, explicitly cheaper than the proof it informs and explicitly not a completeness oracle. That design leaves a standing question the probe is not built to answer: which *forms* of authorable state will the representative Juego2 target eventually need, and which of them no accepted probe has exercised yet.

This file keeps that list finite and auditable so that:

- an applicable workpack can select the probe slice that most stresses its own claim instead of repeating an earlier slice;
- `WP-HK-GATE` can assess readiness against a known inventory rather than a single scenario;
- unexercised shapes are tracked deliberately instead of rediscovered once per cycle.

It is input for probe selection. It is not a schema catalogue and not a requirement that H0 support every listed shape.

## Source and boundary

Entries were derived from `Docs/art/SETTING.md`, `Docs/art/VISUAL_BIBLE.md` and design material in the `Arkus0/Juego` reference archive.

Per `AGENTS.md` and `CLAUDE.md`, `Arkus0/Juego` is reference material only. Nothing here authorizes importing its architecture, systems or code. Each row names a *form of state a future target may need to author*, never a component to build. Any decision to support a shape belongs to a workpack, an ADR or `WP-HK-GATE` — never to this file.

## Status vocabulary

- `COVERED` — an accepted workpack contract represents this shape and at least one accepted probe or conformance fixture exercised it.
- `ASSIGNED` — a named workpack already owns the question.
- `OPEN` — no workpack owns it; the modeling decision has not been taken.
- `OUT` — outside the harness boundary (engine, presentation, runtime).

A row moves to `COVERED` on accepted evidence, never on this file's assertion.

## Inventory

| # | Shape | Representative example | Status | Owner / note |
|---|---|---|---|---|
| 1 | Object with opaque authored document | building, prop, shop | `COVERED` | HK-02A; probed by HK-02A and HK-05 |
| 2 | Containment hierarchy | plaza contains bar | `COVERED` | HK-02; probed by HK-05 |
| 3 | Typed reference between objects | NPC works at a building | `COVERED` | HK-02A; probed by both |
| 4 | Object-scoped extension with declared typed dependencies | NPC profile document | `COVERED` | HK-02A |
| 5 | Several subjects sharing owner/schema version | two NPC profiles | `COVERED` | HK-02A composite identity |
| 6 | Set/tag-valued reference | "any object carrying tag X" | `OPEN` | Typed dependencies resolve to one canonical object; a set-valued target has no declaration form |
| 7 | State keyed by namespaced string, not by object | world flags, progression stages, counters | `OPEN` | Extension subjects must resolve to a canonical object; a global extension payload loses per-key identity and diff granularity |
| 8 | State attached to a pair of objects | relationship axes between two NPCs | `OPEN` | Expressible as a relation object plus two references; not yet a deliberate decision |
| 9 | References embedded in opaque payload | an authored condition naming other state | `ASSIGNED` | Kernel remains deliberately opaque. H1-01 owns a schema-aware Unity producer that mechanically derives typed dependencies from structured input; arbitrary raw opaque payloads remain outside that guarantee. |
| 10 | Ordered interval data with wrap-around | daily schedule blocks crossing midnight | `OPEN` | Payload-opaque today; no kernel time semantics |
| 11 | Authored versus live/runtime state separation | ticking clock, transient position | `COVERED` | `WP-HK-06A`, accepted with its own content-shape probe |
| 12 | Provenance and journal of authored change | who changed what, from which revision | `COVERED` | `WP-HK-06A` |
| 13 | Per-agent derived state | beliefs, current abstract location | `OPEN` | The authored/live boundary is closed by `WP-HK-06A`; representation of the derived state itself is undecided |
| 14 | Concurrency granularity and world partition | many independent edits in one valley | `OPEN` | H0 deliberately retains whole-world CAS and accepted HK08B makes same-lineage stale recovery bounded. H0S remains non-blocking unless measured H1 evidence proves otherwise. |
| 15 | Localization of authored text | one string, several locales | `OPEN` | Not modeled |
| 16 | Asset and engine binding | prefab, mesh, material | `ASSIGNED` | H1-01 owns binding intent/dependency derivation; H1-04..11 own catalogue, materialization and representative Unity proof. It becomes `COVERED` only on accepted evidence. |
| 17 | Dialogue and narrative runtime content | branching conversation | `OUT` | Later reviewed architecture decision |
| 18 | Navigation, physics and animation state | paths, colliders, clips | `OUT` | Presentation and runtime |

## Probe coverage as of WP-HK-06B acceptance

Rows 6–10 have not been exercised by any accepted probe. The accepted HK-02A and HK-05 probes both modeled direct object-identifier references only, the HK-06A probe used a test-owned runtime observation surrogate rather than authored schedule data, and HK-06B exercised semantic diff/snapshot portability over already accepted object/reference/extension shapes rather than introducing new payload semantics.

This is a statement about probe coverage, not a defect claim against any accepted workpack. HK-02A's accepted boundary is explicit that opaque payload meaning is not inferred, and HK-05's invariant set operates on declared structure. Neither guarantee is weakened by a shape no probe has visited; per `AGENTS.md`, reopening an accepted guarantee requires concrete evidence that it is inapplicable or false, which nothing in this file supplies.

## How to use it

When a foundational workpack that defines or changes authorable-state or public-contract semantics selects its probe slice:

1. read this inventory;
2. prefer an `OPEN` or `ASSIGNED` shape that the workpack's own claim actually touches, over repeating an already-probed slice;
3. classify each finding under the standard — current-WP blocker, concrete predecessor reopen condition, named future/residual decision, or out-of-boundary observation;
4. if the probe resolves or reframes a shape, update its row and cite the accepted evidence that moved it.

Keeping a shape `OPEN` is a valid outcome. This file exists to make that choice visible, not to force closure.
