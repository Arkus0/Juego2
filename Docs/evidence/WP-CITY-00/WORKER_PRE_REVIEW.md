# WP-CITY-00 — Worker pre-review (repair cycle 3)

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7
fail_cycle: 3

Supersedes the cycle-2 pre-review, whose `CLEAN` was invalidated by the FAIL and by the repair
mutation. A **quality gate, not an independent review**.

`FOUNDATIONAL_PROOF_STANDARD.md` is not binding: `Docs/workpacks/README.md` places `CITY/` outside
the H0 DAG and exempts it from the foundational standard and from exact-SHA evidence.

## 1. The FAIL, and the pattern behind three of them

Frozen candidate `daefc6a1e4959c1cffacd662f5578a4c13a642d3` received independent FAIL, review
[#5261488049](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261488049).

The defect, in one sentence written directly beneath the crossing table it contradicts:

> "The south bank is never left on a single crossing except when the Puente Viejo and the barca fail
> together, which one flood can cause."

Two errors. The barca's own row says that when the ferry is suspended the south bank "depends on the
Puente Viejo alone" — that **is** a single-crossing state, so `never` is false. And the double-failure
case leaves **zero** crossings, not one. CSI-06 carried the same claim into the permanent contract
while §2.4.1 listed the contradicting case explicitly.

**Accepted.** It is blocking for the reason the Reviewer gives: `WP-CITY-01` must consume this
crossing strategy and reason about closures and alternate routes, and it cannot do that while the
invariant and the table disagree.

### The pattern

| Cycle | The claim | What refuted it | Distance |
|---|---|---|---|
| 1 | the landing needs a navigability premise | the setting already licensed a landing that needs none | same document |
| 2 | closing the Puente Viejo cuts the south side off | the barca, added in the same repair | same table |
| 3 | the south bank is never on a single crossing | the row directly above it | 8 lines |

One root: **a summarising absolute claim, written beside the artefact it summarises, never checked
against it.** Cycle 3 removes the comfortable reading — that sentence was written in the *same commit*
as the table refuting it, immediately after cycle 2 added a cross-check. "Read more carefully" has now
been tried three times and has failed three times.

## 2. The repair

**CSI-06 becomes a design obligation, not a promise the weather can break.** The previous wording was
refutable by a flood, which is not what an invariant is for. It now states what the document can
actually guarantee: the core has one río bridge; the landing *always carries a designed second
south-bank crossing* — barca in State 1, Puente del Muelle in State 2; no third río crossing exists in
either state; the transition makes the second crossing unconditional and never removes it. Degrading
to one crossing, or to none, is a bounded condition owned by the §2.4.1 ladder, not a breach. What the
invariant forbids is a constitution where the south bank is *structurally* served by one crossing.

**§2.4.1 now counts instead of describing.** The situations table became a ladder with an explicit
crossing count per state, because describing effects without counting is exactly how a zero-crossing
state got called a single-crossing one. Two rows carry the weight: **0** (a flood taking both) gives
flood, repair priority and ferry policy a real stake; **1** (State 1 high water) is the ordinary
seasonal case, and is content rather than a defect.

**The dossier stops paraphrasing.** It points at §2.4.1 and CSI-06 instead of restating them. Two
copies of a rule are two things that can disagree, and that is how this defect entered.

**Seam-count drift corrected** in all four sites, plus CSI-10's wrong section reference (§6 → §5).
The Reviewer flagged this without basing the FAIL on it.

**§2.5 qualified.** "The only way to the south bank other than the Puente Viejo" was true in State 1
and false in State 2. Nobody reported it; the audit below caught it.

## 3. The absolute-quantifier audit

This replaces "read the candidate carefully" with something that runs. Every `never / always / only /
no other / exactly / <count>` in the deliverable is listed, and each one that is a **factual claim
about this document's own artefacts** is verified against the table or graph it describes. Policy
prohibitions — "no production phase may…", "must never be cited as…" — are not factual claims and are
exempt.

### Factual claims, each checked

| Claim | Location | Checked against | Result |
|---|---|---|---|
| "the crossing set changes exactly once across production" | §2.4.1 | the two states | true — State 1 → State 2, one change |
| "the core has exactly one río bridge" | CSI-06 | §2.4 table | true — the Puente del Muelle is at the landing, not the core |
| "the landing always carries a second south-bank crossing" | CSI-06 | §2.4.1 ladder | true as a design claim; the ladder owns weather degradation, and CSI-06 now says so |
| "no third río crossing exists in either state" | CSI-06 | §2.4 table | true — State 1 has 2, State 2 has 2 |
| "six named seams exist (§5)" | CSI-10 | §5 list | true — six items, and §5 is the right section |
| "six named directions" ×2 | dossier B §3, §6 | dossier §8 | true — six items |
| "six directions" | comparison matrix | dossier §8 | true |
| "four of the nine district families" | §2.8 | §2.3 table | true — nine rows; the seed touches casco, plaza, the Ensanche edge and the river edge |
| "seven substantial families plus two edge families" | §2.3 | §2.3 table | true — nine rows, two marked low-intensity |
| "five spatial characters stay distinct" | CSI-09 | its own enumeration | true — old quarter, civic/commercial, residential, work/port, rural |
| "six zones, no port" (describing blueprint v0.2) | §7 | the superseded blueprint table | true — six rows |
| "the only option whose charges are answerable by structure" | §6 | `REFUTATION_LOG.md` verdicts | true — A loses on A-1/A-3, C on C-1, neither structurally answerable |
| "a hub-and-spokes city fails every row; this one fails none" | §2.6 | the five-trip table | true — every trip has a plaza-free route |

### Mechanical counts

| Declared | Actual | Result |
|---|---|---|
| six expansion seams | 6 numbered items, both documents | pass |
| nine district families | 9 table rows | pass |
| twelve CSI invariants | 12 defined, 12 cited, no orphan | pass |
| seven crossings | 7 table rows | pass |

### Section references

Every `§n` citation resolved against actual headings rather than memory. The document's own
references (§2, §2.2, §2.3, §2.4.1, §2.5, §3, §5, §8) all exist; the §1.2/§1.3/§1.4/§1.5/§12
citations belong to `PRODUCTION_BLUEPRINT.md` in the §7 reconciliation table and all five exist
there. CSI-10's §6 was the one broken reference and is fixed.

**Findings produced by this audit:** the §2.5 `only` claim (unreported), and confirmation of CSI-10's
count *and* its broken section reference. That is the justification for the step existing — it finds
what rereading does not.

```text
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
CUMULATIVE_FINDINGS_FIXED_ACROSS_CYCLES: 14
```

## 4. Other mechanical verification

| Check | Result |
|---|---|
| no absolute claim contradicted by its own table (§3 audit) | pass |
| crossing counts agree across §2.4.1, CSI-06 and both crossing tables | pass |
| every Markdown table has consistent column counts | pass |
| diff contains only `.md` under `Docs/` | pass |
| open questions contiguous and in order | pass |
| worktree clean before freeze | pass |

## 5. Acceptance criteria

| # | Criterion | Verdict |
|---|---|---|
| — | **Definition of Done** — a constitution stable enough for CITY-01 to calculate movement and topology without inventing a different city | **met**: the invariant, the ladder and both crossing tables now agree on how many crossings serve the south bank in every state |
| 3 | River shapes movement rather than scenery | met — and the ladder makes the río's states usable by CITY-01 rather than merely stated |
| 1–2, 4–10 | topology alternatives, port, loops, district distinctness, expansion, long-route scale, size justification, retained seed, no engine work | unchanged by this repair; challenged by none of the three reviews |

Negative gates: unchanged.

## 6. Residual risks

1–5 as recorded in cycle 2 (walk times and areas are hypotheses; verticality is a CITY-04 risk;
building-count ratios are assumptions; Topology A's rejection is coupled to the size decision; the
landing is seasonal, so CSI-04 is load-bearing).

6. The State 1 → State 2 transition has no owner for *when* it happens. Unchanged from cycle 2.
7. **New:** for part of the year the south bank genuinely runs on one crossing (§2.4.1, State 1 high
   water). That is deliberate content, but CITY-01 should treat it as a recurring seasonal state and
   not an exception, and CITY-02 should not site something the town cannot do without on the far side.

## 7. Verdict

No known blocking defect remains inside the claim.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
```

Three cycles have failed on one shape of error, and the countermeasure in §3 is new and unproven on a
fresh candidate. The independent Reviewer should not treat this report as a checklist, and should
weight the possibility that the audit's *scope* is wrong — that a fourth instance lives in a claim the
grep pattern does not match, or in the evidence files rather than the deliverable.
