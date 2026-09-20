# WP-CITY-00 — Worker pre-review

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7

This is the mandatory strict pre-review required before freeze. It is a **quality gate, not an
independent review**. `WORKER_PRE_REVIEW: CLEAN` never means PASS, does not satisfy the Reviewer
obligation, and gives the independent Reviewer no reason to trust any conclusion below.

`FOUNDATIONAL_PROOF_STANDARD.md` is **not** binding here: `Docs/workpacks/README.md` places `CITY/`
outside the H0 DAG and exempts it from the foundational standard and from exact-SHA evidence. The
pre-review duty, the predecessor contract check and the freeze/handoff rules still apply.

## 1. What was inspected

- The complete baseline→candidate diff (`git diff 290f92e..HEAD`), not only the last repair.
- The exact WP acceptance criteria, Definition of Done, required ingredients and negative gates.
- The inherited direction documents named in `WORKER_PLAN.md`'s predecessor contract check.
- Internal numeric consistency across the constitution, the three dossiers, the matrix and the scale
  document.
- Every cross-referenced path, mechanically.

## 2. Findings, all repaired while Draft

Seven in-claim findings. None were discovered by reading the last commit; all came from re-reading
the candidate against the contract as if writing a FAIL.

### F1 — material: the content-cost argument was wrong and leaned on the wrong cap

`SCALE_ENVELOPE.md` justified shrinking the fabric partly by claiming "~0.40 km² … needs on the order
of 300–450 authored building frontages", citing the visual bible's ≤120-mesh cap.

Both halves were defective. The figure was undefended and low by roughly a factor of three; and the
mesh cap counts **distinct meshes**, which a modular kit exists precisely to decouple from building
count, so it was not the constraint the argument needed.

This mattered because it was load-bearing: it was one of only two independent checks supporting the
falsification of the area hypothesis. A reviewer entitled to reject the number would have been
entitled to reject the conclusion.

Repaired at the causal level rather than by adjusting the number: the claim is now derived from the
constitution's own district breakdown with stated coverage ratios and footprints (≈830 buildings at
0.365 km², ≈1,800–2,800 at the original hypothesis), the assumptions are declared as assumptions, and
the argument is moved to the ratio that actually binds — systemic Tier A/B locations against ambient
fabric (5–7% here versus about 2% at 1.0 km²).

### F2 — material: loops were asserted, not tested

The constitution claimed loops and a non-mandatory plaza without applying the workpack's own test.
Added §2.6's explicit table: five representative trips, each with a plaza route and a plaza-free
route, plus the structural reason (CSI-02 and CSI-07) rather than a decorative one.

### F3 — material: an acceptance criterion had no answer in the deliverable

*"Long-route scale is sufficient that an actor can genuinely be 'elsewhere in town'"* was implied by
the walk-time table and argued nowhere. Added §2.7, framed as verification cost — a player who sees
somebody at the plaza cannot cheaply confirm where they are half an hour later — rather than as raw
distance, because distance alone does not satisfy the criterion.

### F4 — material: the seed guarantee was in the evidence but not in the deliverable

*"Demo/product seed can occupy a real retained part of the selected constitution"* was argued in the
Topology B dossier and absent from the constitution the Reviewer reads. Added §2.8, stating where a
retained seed can sit and what it contains, while leaving the boundary to `WP-CITY-03`.

### F5 — consistency: district family count

The Topology B dossier said "four of the eight district families"; the constitution defines nine
(seven substantial plus two edge). Corrected in the dossier.

### F6, F7 — consistency: a superseded scale band survived in two places

`REFUTATION_LOG.md` and `COMPARISON_MATRIX.md` still cited "≈0.35–0.45 km²" after the adopted band
became 0.30–0.45 km², and the derived half-town figure moved with it. Both corrected; a repository
sweep confirms no stale band reference remains.

```text
WORKER_PRE_REVIEW_FINDINGS_FIXED: 7
```

## 3. Acceptance criteria, walked one at a time

| # | Criterion | Verdict | Where |
|---|---|---|---|
| 1 | ≥3 genuine topology alternatives compared | met | three dossiers, each placing all twelve required ingredients and declaring its own weaknesses; `COMPARISON_MATRIX.md` covers the union of the eight required axes and the ten WP criteria |
| 2 | Selected topology contains loops and cannot be described as one hub with spokes | met | constitution §2.6, remove-the-plaza table over five trips |
| 3 | River shapes movement rather than functioning only as scenery | met | §2.1, §2.4, CSI-01: the stream is crossed casually and often, the river rarely and deliberately; this criterion is also what rejected Topology C |
| 4 | Port has a gameplay/material/social reason to exist and a setting-appropriate scale | met, **conditionally** | §2.5: the landing is where valley road freight changes medium below the confluence; quay 120–150 m, working craft only. The fictional navigability premise is referred to `WP-ART-00` as open question Q1 rather than assumed |
| 5 | Old quarter, civic/commercial, residential, port/work edge and rural edge spatially distinct | met | §2.3 and CSI-09; residential is deliberately two characters, Barrio Alto and Ensanche |
| 6 | At least two credible outward/expansion directions | met | five named seams plus a later sixth; §5 |
| 7 | Long-route scale sufficient for "elsewhere in town" | met | §2.7 and the §3 walk-time table |
| 8 | Size justified through density/travel/content cost, not comparison vanity | met | `SCALE_ENVELOPE.md`: no comparison to another game appears anywhere in the candidate; the argument is traversal arithmetic, a derived building count and a location-to-fabric ratio |
| 9 | Demo/product seed can occupy a real retained part | met | §2.8; it is the blueprint's existing seed plus a stream crossing and a port seam |
| 10 | No Unity scene, asset import or runtime contract created | met | the diff is nine Markdown files under `Docs/`; every occurrence of Unity/navmesh/asset-import vocabulary in the candidate is a negation or a deferral, verified mechanically |

Definition of Done — *"strong enough for CITY-01 to calculate movement/topology without inventing a
different city"*: the district graph, the crossing table with closure consequences, the three
longitudinal routes, the dimensional sketch and the eight-route walk-time table are together enough
for CITY-01 to build a route graph without choosing a city. Met.

## 4. Negative gates

| Gate | Verdict |
|---|---|
| effectively four streets plus scenery | clear — nine district families, three longitudinal routes, six crossings, two watercourses |
| chases acreage without a reactive-density argument | clear — the candidate **shrinks** the hypothesised city and states the density ratio that forced it |
| puts every meaningful route through the plaza | clear — §2.6; and CSI-02 makes route continuity a contract rather than an intention |
| uses the river/port as decorative labels only | clear on the movement half (CSI-01) and on the port's function (§2.5); the setting premise behind the port is an open question, not a hidden assumption |
| requires demolishing the first serious demo district | clear — the retained seed is the blueprint's existing seed, and no seam requires moving the plaza, the Puente Viejo, the Calle Mayor alignment or the quay (CSI-10) |
| pre-decides implementation authority owned by H1/H2 | clear — no identifier is frozen, no storage authority is named, no runtime or catalogue decision is made |

## 5. Scope decisions a Reviewer should challenge deliberately

**The candidate edits `Docs/production/PRODUCTION_BLUEPRINT.md`.** `Docs/workpacks/CITY/README.md`
authorizes CITY to "sharpen or explicitly propose amendments to it", and the project owner chose
editing over proposing so that `main` does not carry two contradictory topologies. The edit is
confined to §1.2, §1.3, §1.4, §1.5, §8.1 and §12, hands topology and scale authority to the
constitution rather than restating it, and changes nothing about assets, phases, architecture or the
H0 boundary. The blueprint remains explicitly non-binding.

**The candidate does not touch `Docs/workpacks/CITY/WP-CITY-00.md`.** Workpack status transitions
belong to post-PASS DocSync, not to the Worker.

**The candidate does not touch `Docs/engineering/RESIDUAL_LEDGER.md`.** That ledger declares itself
as covering accepted **H0** workpacks; CITY residuals are carried as open questions in the
constitution §8 instead.

## 6. Residual risks, recorded rather than hardened

1. **The port's setting premise is unresolved.** If `WP-ART-00` refuses the fictional navigability of
   the river below the confluence, the port's placement and therefore the selected topology must be
   reopened. This is deliberately left open: deciding it here would be CITY silently restyling the
   game, which the track README forbids.
2. **Every walk time and area is a hypothesis.** The 1.15 m/s effective speed is an assumption stated
   as one. `WP-CITY-04` measures; nothing here may be cited as measured.
3. **Verticality is the selected topology's declared weakness.** Three terraces plus two watercourses
   is the hardest of the three options to read on the ground; it is named for CITY-04 rather than
   argued away.
4. **The coverage ratios and footprints behind the ~830-building estimate are planning assumptions.**
   They are stated so they can be contested; they are not measured.
5. **The rejection of Topology A is coupled to the size decision, not to quality.** Above a dense band
   of roughly 0.7 km² it no longer holds. The correct response is recorded as reopening the
   constitution, never stretching it.

## 7. Scope discipline

No new process machinery, no new document class, no new registry and no new proof apparatus were
introduced. The candidate is nine Markdown files: one product deliverable, seven pieces of reasoning
evidence, and a reconciliation of a document that already existed. Nothing was added to make review
feel thorough.

## 8. Verdict

No known blocking defect remains inside the claim. The four material findings were repaired at their
causal level rather than patched at the reported example, and the three consistency findings were
swept repository-wide rather than fixed where spotted.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 7
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
```

The independent Reviewer must not treat this report as a checklist or limit its search to the risks
named here.
