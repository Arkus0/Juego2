# WP-CITY-00 — Worker pre-review (repair cycle 2)

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7
fail_cycle: 2

This supersedes the cycle-1 pre-review, whose `CLEAN` was invalidated by the FAIL and by the repair
mutation. It remains a **quality gate, not an independent review**.

`FOUNDATIONAL_PROOF_STANDARD.md` is not binding: `Docs/workpacks/README.md` places `CITY/` outside
the H0 DAG and exempts it from the foundational standard and from exact-SHA evidence.

## 1. The FAIL, and why it was right

Frozen candidate `6091584313488bcfb1840132f22adcc9b1b5de4f` received independent FAIL, review
[#5261418045](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261418045).

The cycle-1 repair was confirmed sound. The new blocker was **produced by that repair**: adding the
barca as a second Puerto ↔ south-bank crossing falsified consequences elsewhere that were never
reconciled. The crossing tables still asserted that closing the Puente Viejo cut the south side off —
untrue while the ferry runs, and untrue again once the Puente del Muelle replaces it — and neither
semantic graph drew the ferry, which is exactly what cycle 1 had said it did not want to leave
implicit. The selected district graph and the crossing strategy had stopped describing the same city,
so `WP-CITY-01` would have had to choose one.

**Accepted in full.** And it is the second instance of one shape of error: closing over something
without checking what else depended on it. Cycle 1 closed over a setting premise without asking
whether the landing needed it; cycle 2 added an edge without asking what that edge falsified. That is
why the repair puts a rule in `WORKER_PLAN.md` and a check in §4 below, rather than only fixing the
two sentences the Reviewer named.

## 2. The repair

**Two connectivity states, named** (`CITY_SPATIAL_CONSTITUTION.md` §2.4.1). The crossing set changes
exactly once across production — State 1, the barca era, with a conditional ferry; State 2, the
Puente del Muelle era, with two unconditional south-bank crossings. Leaving that implicit is how the
table came to contradict the graph.

**Closure consequences derived, not asserted.** From the §3 dimensional sketch: casco to the
ermita over the Puente Viejo is ≈80 m; with that bridge closed in State 1 the south bank costs
≈500 m plus the ferry wait, a detour of order 6× bounded by hours, fare and high water. The genuine
cut-off case is the **double failure**, which one flood can cause — better governance and event
material than the flat claim it replaces, and unlike that claim, true.

**Both graphs redrawn** so the barca and the south-bank return are visible, and the loop is seen
rather than described.

**Both tables restructured** to separate Availability from closure consequence. Worth recording
honestly: four of the seven rows had been carrying availability in the closure column since the
first draft — "seasonal, flood-closable", "disappears when the water rises". The column was broken
before the barca existed; the barca only made it visible.

**Propagated** to CSI-06 (which now spans both states and fixes that the south bank is never left on
a single crossing), the walk-time hypotheses, expansion seam 6 and Q12.

## 3. Findings from this pre-review

Three, all produced by the new cross-check in §4, all fixed before freeze.

### F10 — closure consequences did not name the crossing that absorbs the detour

"carts detour ~200 m" is unverifiable: a reader cannot tell which crossing takes them or whether
200 m is right. Three arroyo rows had this shape. Each now names its detour target — carts upstream
to the Puente de la Vega, the pasarela to the Puente del Mercado, the huerta route downstream —
which is what makes the number checkable against the graph rather than trusted.

### F11 — the Puente del Muelle's closure said what stops, not what continues

"freight cannot cross at all" is true and incomplete: in State 2 people still cross at the casco. A
consequence that omits what survives reads as a cut-off, which is precisely the error this cycle
exists to remove. Corrected in both tables.

### F12 — the redrawn graph created a tenth district family by accident

Labelling `ORILLA SUR` as a graph node put it alongside the nine district families, while §4
classifies the camino sur as *transitional* fabric — traversable, deliberately low-intensity, and
carrying no CITY-02 programming promise. The graph already marks the scenic envelope `[escénico]`, so
the south bank is now marked `[transitional]` using the graph's own convention. Left unmarked it
would have handed CITY-02 a district to programme that the constitution never promised.

```text
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
CUMULATIVE_FINDINGS_FIXED_ACROSS_CYCLES: 12
```

## 4. The cross-check this cycle adds

For **every crossing in every table**, verify its closure consequence against the *complete* crossing
set in the graph. This is the check that would have caught the cycle-1 defect, and it is now part of
the pre-review rather than of a reviewer's reading.

| Crossing | Closure consequence | Compatible with the full crossing set? |
|---|---|---|
| Puente Viejo | south bank via barca (S1) or Puente del Muelle (S2), ≈500 m + wait | yes — both alternatives exist in the graph |
| Puente del Mercado | carts upstream to the Puente de la Vega, ~200 m | yes — the Vega bridge is the only other cart-capable arroyo crossing |
| Pasarela del Lavadero | two minutes to the Puente del Mercado | yes |
| Puente de la Vega | huerta route downstream to the Puente del Mercado | yes |
| Pasos / vado | nothing on its own | yes — three other arroyo crossings remain |
| La barca (S1) | south bank on the Puente Viejo alone | yes — and CSI-06 marks this as the single-crossing state |
| Puente del Muelle (S2) | people cross at the casco; freight cannot cross | yes — the barca no longer exists in S2, and the Puente Viejo is handcart-only |

No row asserts an isolation the graph contradicts, and no row omits an alternative the graph
provides.

## 5. Mechanical verification

| Check | Result |
|---|---|
| no unconditional isolation claim survives (`grep "cut off"`) | pass — three hits, two explicit negations and one quotation of the superseded claim |
| the barca appears in both graphs and both crossing tables | pass |
| every Markdown table has consistent column counts | pass |
| diff contains only `.md` under `Docs/` | pass |
| twelve CSI defined, twelve cited, no orphan | pass |
| open questions contiguous and in order | pass |
| worktree clean before freeze | pass |

## 6. Acceptance criteria

Only the rows this repair touches are re-argued.

| # | Criterion | Verdict | Where |
|---|---|---|---|
| — | **Definition of Done**: a constitution stable enough for CITY-01 to calculate movement and topology without inventing a different city | **met** — this was the failed criterion | the graph, the crossing table and the closure consequences now describe one city across both connectivity states |
| 2 | Loops, not a hub with spokes | met, and strengthened | §2.6's remove-the-plaza table is untouched; the south-bank loop is now drawn and costed rather than latent |
| 3 | River shapes movement rather than scenery | met | two río crossings with genuinely different characters, and closure states that matter |
| 1, 4–10 | topology alternatives, port, district distinctness, expansion, long-route scale, size justification, retained seed, no engine work | unchanged by this repair | walked in earlier cycles; the review challenged none of them |

Negative gates: unchanged.

## 7. Residual risks

1. Every walk time and area remains a hypothesis; 1.15 m/s effective is a declared assumption for
   `WP-CITY-04`.
2. Verticality remains the selected topology's declared weakness, named for CITY-04.
3. The coverage ratios behind the ~830-building estimate are planning assumptions.
4. The rejection of Topology A stays coupled to the size decision, not to its quality.
5. The landing is seasonal on its river side, so CSI-04 is load-bearing.
6. **New:** the State 1 → State 2 transition is a real change in the city's connectivity, and no
   workpack yet owns *when* it happens. CITY-03 chooses the seed and CITY-01 the mobility graph;
   neither is asked to schedule the Puente del Muelle. Recorded rather than resolved, because
   deciding it here would be CITY-00 taking a production-sequencing decision it has no basis for.

## 8. Verdict

No known blocking defect remains inside the claim.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
```

Two cycles have now failed on the same shape of error, so the fresh independent Reviewer should
assume a third instance is likelier than a clean sheet: look for anything else this candidate closes
over without checking what depends on it. This report is not a checklist and its boundary is not the
only place a defect can live.
