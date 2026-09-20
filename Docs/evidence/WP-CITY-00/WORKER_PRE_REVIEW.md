# WP-CITY-00 — Worker pre-review (repair cycle 1)

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Governing protocol: `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7
fail_cycle: 1

This pre-review supersedes the one run on the failed candidate. That `CLEAN` was invalidated by the
repair mutation and by the FAIL itself.

It remains a **quality gate, not an independent review**. `WORKER_PRE_REVIEW: CLEAN` never means
PASS and gives the fresh independent Reviewer no reason to trust anything below.

`FOUNDATIONAL_PROOF_STANDARD.md` is not binding: `Docs/workpacks/README.md` places `CITY/` outside
the H0 DAG and exempts it from the foundational standard and from exact-SHA evidence. The pre-review
duty, the predecessor contract check and the freeze/handoff rules still apply.

## 1. The FAIL, and why it was right

Frozen candidate `b9473f04da72462619e9561c097083afdc6c436d` received independent FAIL, review
[#5261386416](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261386416).

The blocker: the constitution justified the landing's placement and its road↔water transfer role on
the premise that the joined river carries loaded craft downstream — a premise the candidate itself
called invented, stated CITY was not entitled to make, and left open while admitting its refusal
would move the landing and reopen the topology. A workpack whose Definition of Done is a constitution
stable enough for `WP-CITY-01` cannot close over a condition capable of invalidating it. The same
defect produced an authority inversion: §2.5 claimed the port scale was "binding on" `SETTING.md` and
CSI-05 said it "Binds" it, when `Docs/workpacks/CITY/README.md` says ART owns setting direction and
CITY consumes it.

**The finding is accepted in full.** The previous pre-review recorded this as residual risk #1 and
argued that deciding the premise would be CITY restyling the game. That reasoning was not wrong so
much as incomplete: it treated the charge as a binary — assume the premise or defer it — and never
asked the third question, whether the landing needed the premise at all. It did not. Missing an
available third option is exactly the class of defect a pre-review exists to catch, and this one
reached an independent Reviewer instead.

## 2. The repair, and why it is causal rather than local

The Reviewer allowed two minimal boundaries: obtain an explicit binding ART decision, or make the
constitution valid under the `SETTING.md` that already exists. The project owner chose the second.

The landing's reason for existing was rewritten from scratch (`CITY_SPATIAL_CONSTITUTION.md` §2.5).
It now rests on confluence hydrology — below a junction the water is wider, slower and braided over
gravel bars, which is a fact about confluences and not a claim about navigation — plus four ordinary
reasons that coincide at one place: timber rafted on the bars and released at high water, áridos
worked under municipal concession and carted *upstream into the city*, a roped ferry to the south
bank, and break-bulk for the valley road whose junction is already adjacent under CSI-04.

This is a repair at the causal boundary, not a patch on the reported sentence:

- **CSI-05** now states that the landing depends on no premise beyond the inherited setting, and that
  the invariant is *consumed from* `SETTING.md` rather than binding on it. Both halves of the FAIL
  are closed by the same invariant.
- **`WORKER_PLAN.md`** records the rule the cycle taught, so it constrains future CITY work rather
  than this document only: *an open question is legitimate only when no answer to it can invalidate
  the constitution; if a spatial decision needs a setting premise ART has not given, redesign the
  decision to need less.*
- **§8 Q1** is no longer a validity question. It is a consumption question about visual and material
  language, and says explicitly that no answer to it can invalidate the constitution.

What the swap costs is stated in the deliverable rather than hidden: the city is no longer a node on
a long-distance trade route and visitors do not arrive by water. What it gains is a **season** — the
river works only at high water — which nothing else in the city has.

## 3. Findings from this pre-review

Two, both arising from the repair itself and both fixed before freeze.

### F8 — the barca existed in only one document

The repair introduced the roped ferry as a crossing and put it in the constitution's crossing table
and CSI-06, but not in the Topology B dossier's crossing table. A Reviewer comparing the two would
find the selected option described with a crossing its own dossier does not list. Added.

### F9 — the repair created an unstated route and left it for someone to discover

The barca and the Puente Viejo together close a **long loop along the south bank** that the
constitution did not previously have. Leaving a new route implicit is the same failure mode as
leaving a premise implicit: it hands `WP-CITY-01` a city slightly different from the one on paper.
Named in §2.4 and recorded as open question Q12 for CITY-01, framed as a question rather than as a
claim about traffic the constitution has no basis to make.

```text
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
CUMULATIVE_FINDINGS_FIXED_ACROSS_CYCLES: 9
```

## 4. Mechanical verification on the repaired candidate

| Check | Result |
|---|---|
| diff contains only `.md` under `Docs/` | pass |
| no `binds`/`binding on` claim aimed at an ART document anywhere in the candidate | pass |
| no remaining "transfer point", "toward the coast" or downstream-cargo claim | pass |
| every cross-referenced repository path resolves | pass |
| every Markdown table has consistent column counts | pass |
| twelve CSI invariants defined, and every CSI cited in prose is defined | pass |
| open questions numbered contiguously and in order | pass |
| worktree clean before freeze | pass |

## 5. Acceptance criteria, rewalked

Only the rows the repair touches are re-argued; the rest were walked on the prior candidate and the
review did not disturb them.

| # | Criterion | Verdict | Where |
|---|---|---|---|
| 4 | Port has a gameplay/material/social reason to exist and a setting-appropriate scale | **met, unconditionally** | §2.5: four reasons that need no unaccepted premise; scale consumed from `SETTING.md`; the smaller claim is stated rather than concealed |
| 3 | River shapes movement rather than functioning only as scenery | met, and strengthened | the barca makes the río a crossing at a second point, with hours and a fare, and CSI-06 keeps it from becoming a habit |
| 2 | Loops, not a hub with spokes | met, and extended | §2.6's remove-the-plaza table is unchanged; the south-bank loop is now named rather than latent |
| 1, 5–10 | topology alternatives, district distinctness, expansion, long-route scale, size justification, retained seed, no engine work | unchanged by this repair | walked on the prior candidate; the review challenged none of them |

Definition of Done — a constitution strong enough for `WP-CITY-01` to calculate movement and topology
without inventing a different city. This is the criterion the FAIL was about, and it is now met for
the reason it was not before: no pending external decision can move the landing or reopen the
topology.

Negative gates: unchanged from the prior walk, except that *"uses the river/port as decorative labels
only"* is now clear outright rather than conditional.

## 6. Residual risks

1. **Every walk time and area remains a hypothesis.** 1.15 m/s effective is a declared assumption for
   `WP-CITY-04` to measure.
2. **Verticality remains the selected topology's declared weakness**, named for CITY-04 rather than
   argued away.
3. **The coverage ratios and footprints behind the ~830-building estimate are planning assumptions**,
   stated so they can be contested.
4. **The rejection of Topology A stays coupled to the size decision**, not to its quality. Above a
   dense band of roughly 0.7 km² the constitution should be reopened rather than stretched.
5. **The landing is seasonal on its river side**, so its year-round life leans on the road junction,
   the yards and the ferry. CSI-04 is therefore load-bearing rather than a nicety.

Residual risk #1 of the prior cycle — the unresolved port premise — is **closed**, not carried
forward. It was never a residual; it was a blocker, and the Reviewer was right to say so.

## 7. Scope discipline

The repair added no document, no process machinery and no new claim. It rewrote one section, two
invariants, one open question and the corresponding passages in three evidence files, and recorded
what changed in each rather than quietly restating them. The spatial work the review found sound is
untouched.

## 8. Verdict

No known blocking defect remains inside the claim.

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-00/WORKER_PRE_REVIEW.md
```

The fresh independent Reviewer must not treat this report as a checklist, must not limit its search
to the risks named here, and must not treat the previous FAIL's boundary as the only place a defect
can live.
