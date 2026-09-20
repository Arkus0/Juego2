# WP-CITY-00 — Worker pre-review (circuit-breaker re-audit)

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7
fail_cycle: 3 — this is a Worker-initiated re-audit, not a fourth FAIL

Run from scratch on the reconciled candidate. Every previous `CLEAN` is void. A **quality gate, not
an independent review**.

`FOUNDATIONAL_PROOF_STANDARD.md` is not binding: `Docs/workpacks/README.md` places `CITY/` outside
the H0 DAG and exempts it from the foundational standard and from exact-SHA evidence.

## 1. Why this is not cycle 4

Three independent FAILs landed in one causal family:

| Cycle | Claim | Refuted by | Distance |
|---|---|---|---|
| 1 | the landing needs a navigability premise | the setting already licensed a landing needing none | same document |
| 2 | closing the Puente Viejo cuts the south side off | the barca, added in the same repair | same table |
| 3 | the south bank is never on a single crossing | the row directly above it | 8 lines |

The protocol is explicit that at this point local patching stops. The freeze at
`814c8c959285535bae13251bd0a5bc3474b711e1` was **withdrawn**, not handed to a fourth reviewer.

The defect was never any of the three sentences. It was that connectivity semantics lived in six
surfaces at once — constitution prose, crossing table, ladder, graph, invariants, dossier — each free
to drift. Patching one surface per cycle could only ever produce the next contradiction, and did,
three times.

## 2. What the audit built

`Docs/evidence/WP-CITY-00/CONNECTIVITY_MATRIX.md`: one authoritative source covering areas, the
complete crossing set with availability, the two states, an availability ladder for the río **and one
for the arroyo**, route counts and plaza dependence per adjacent pair, port approaches, loops, and the
seam count. Every other surface now cites it and none paraphrases it.

## 3. Findings

Six, all found by the audit rather than by review, all fixed before freeze.

### F13 — a live contradiction the reviews had not yet reached

§2.4.1 opened with *"The south bank is therefore reached two ways in both states"* — **seven lines
above** its own ladder showing one crossing at high water and zero in a flood. This was a fourth
instance of the failing family, already in the candidate. Replaced with a design claim: each state
*designs* two crossings; availability is counted separately and owned by the matrix.

### F14 — CSI-03 promised twice what the evidence shows

It required "at least two plaza-free routes" between adjacent families. The §2.6 table demonstrates
**one** per pair. It also quantified over pairs containing the plaza itself, where the question is
meaningless. Restated as a design invariant over the correct set, at the count the evidence supports.

### F15 — CSI-03 was also false in high water, and that is content

Casco ↔ Ensanche's only plaza-free route is the pasos, which are low water only. So that pair **does**
route through the plaza when the water rises. Rather than invent a lane to rescue the invariant, the
condition is named in the matrix, in the §2.6 table's new *Always available?* column, and in CSI-03
itself. A town where one quarter gets harder to reach in high water is better than one where nothing
ever changes — this is the audit's one finding with real spatial content, and it is kept, not fixed.

### F16 — CSI-07 promised an availability count

"At least three" arroyo crossings is false in flood, when the Vega bridge is closable and the pasos
are under: two remain. Four exist by design; two are permanent. Restated accordingly.

### F17 — the port was described as reachable three ways

Three land approaches, but the ferry is a fourth way in, from the south bank. The dossier already
said "three land approaches plus the ferry"; the constitution said three. Now four in both, in both
states.

### F18 — two summaries omitted the south bank's second crossing

The dossier's required-ingredients row 6 and the handoff's one-line summary both listed "one historic
river crossing plus four stream crossings", silently dropping the barca and the Puente del Muelle —
the precise omission that caused cycle 2. Both now derive from the matrix.

Also carried: the State-1 loop in the walk-time table was unlabelled and is now **L1** with its
State-2 counterpart **L1′**; seam 6 now records that it is the only seam that would promote a
transitional area into fabric.

```text
WORKER_PRE_REVIEW_FINDINGS_FIXED: 6
CUMULATIVE_FINDINGS_FIXED_ACROSS_CYCLES: 20
```

## 4. Did the audit force reopening the topology?

The circuit breaker permits it and the owner explicitly authorised it. **It does not.**

Every finding above is a surface restating connectivity and drifting. The one finding with spatial
content (F15) is a seasonal condition worth keeping. No route the constitution promises is
impossible, no crossing contradicts the graph, no state is unreachable, and the loop test still
passes on every row. **Topology B stands, and the city is not redesigned.**

## 5. Cross-surface verification

The check that patching could not do: every surface against the matrix, and against each other.

| Check | Result |
|---|---|
| every crossing named on any surface resolves to one of the matrix's seven | pass |
| no surface names a crossing the matrix does not define | pass |
| crossing counts agree: 4 arroyo, 2 río per state, 6 co-existing, 7 across the constitution's life | pass |
| CSI-03, CSI-06, CSI-07 each declare whether they bind design or availability | pass — all three bind design |
| no invariant asserts an availability count | pass |
| seam count is six on every surface that states it (CSI-10, dossier ×2, comparison matrix) | pass |
| port approaches: four on every surface | pass |
| both loops carry their state label | pass |
| `Docs/workpacks/CITY/WP-CITY-00.md`, `RESIDUAL_LEDGER.md`, ART and H0 untouched | pass |

## 6. Absolute-quantifier audit

Retained from cycle 3 and rerun. Every `never / always / only / no other / exactly / <count>` that is
a factual claim about this candidate's own artefacts, verified against the artefact. Policy
prohibitions are exempt.

Result: the claims that failed this audit are exactly F13, F14 and F16, all repaired. The remainder —
"the crossing set changes exactly once", "the core has exactly one río bridge", "no third río crossing
exists in either state", "six named seams", "nine district families", "twelve invariants", "a
hub-and-spokes city fails every row; this one fails none" — check out against their tables.

Mechanical counts: six seams (6 items, both documents), nine families (9 rows), twelve CSI (12 defined,
12 cited), seven crossings (7 rows). Section references resolved against real headings.

## 7. Acceptance criteria

| # | Criterion | Verdict |
|---|---|---|
| — | **Definition of Done** — a constitution stable enough for CITY-01 to calculate movement and topology without inventing a different city | **met**: CITY-01 now reads one connectivity source instead of reconciling six surfaces |
| 2 | Loops, not a hub with spokes | met — the table still passes every row, now with its conditionality stated |
| 3 | River shapes movement rather than scenery | met — two ladders, both waters counted |
| 1, 4–10 | topology alternatives, port, district distinctness, expansion, long-route scale, size justification, retained seed, no engine work | unchanged; no review has challenged them |

Negative gates: unchanged.

## 8. Residual risks

1–5 as recorded in cycle 2 (walk times and areas are hypotheses; verticality is a CITY-04 risk;
building-count ratios are assumptions; Topology A's rejection is coupled to the size decision; the
landing is seasonal, so CSI-04 is load-bearing).

6. The State 1 → State 2 transition has no owner for *when* it happens.
7. The south bank runs on one crossing for part of the year, by design.
8. **New:** Casco ↔ Ensanche routes through the plaza in high water. CITY-01 should treat this as a
   recurring seasonal state rather than an exception, and CITY-02 should weigh it before siting
   something the casco cannot do without on the far side of the arroyo.
9. **New:** the matrix is a governance mechanism, not a proof. It prevents drift between surfaces; it
   cannot prevent the matrix itself being wrong. A reviewer challenging this candidate should
   challenge the matrix's contents, not only the agreement between surfaces.

## 9. Verdict

No known blocking defect remains inside the claim.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 6
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
```

Three FAILs preceded this candidate and the countermeasure is structural rather than another promise.
The independent Reviewer should not treat this report as a checklist. The most productive line of
attack is residual 9: the surfaces now agree, so the remaining risk is that they agree on something
false.
