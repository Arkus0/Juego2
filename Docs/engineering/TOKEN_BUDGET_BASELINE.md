# Token budget baseline — NON-CONTRACTUAL

Measured at `main` = `a57afa3d3fc60b3e1c59d04149fe982f387c2048`.

This document measures the **mandatory cold-start reading cost** of Juego2 role
sessions and models what a set of representation changes would cost instead. It
is measurement input for the token-savings work. It defines no contract, binds
no workpack, and weakens no acceptance criterion. If any statement here appears
to conflict with `AGENTS.md`, `Docs/ROADMAP.md`, `Docs/workpacks/**` or accepted
evidence, those win.

## Status of this document

This is a dated measurement artifact pinned to `a57afa3d3fc60b3e1c59d04149fe982f387c2048`,
not a maintained branch. That SHA is an immutable ancestor of `main`, so the
"before" side stays reproducible indefinitely.

If `WP-CTX-03` uses this work, adopt the **machinery**, not this frozen report:

- `scripts/token-budget/measure.py` hardcodes the pre-CTX reading sets, which is
  exactly what the "before" measurement needs — leave those arrays alone;
- the "after" measurement must read the derived role profiles that `WP-CTX-01`
  makes authoritative, rather than silently editing those arrays, or the two sides
  stop being comparable;
- every figure here is an exact-SHA measurement of documents, not accepted
  evidence. Nothing in this file has been through independent review, and it must
  not become evidence by cherry-pick.

## Method

Reading sets are taken from the rules themselves, not invented: `AGENTS.md`
("Mandatory predecessor contract check"), `.agents/skills/implement-workpack/SKILL.md`
Preconditions, and `.agents/skills/validate-workpack/SKILL.md` Review steps 1–2.
Reproduce with `scripts/token-budget/measure.py` (see its docstring for the
tokenizer).

Counts use the Claude tokenizer bundled in older `anthropic` wheels. It is a
close proxy, not the exact tokenizer of current models; treat all figures as
±10%. Candidate diffs are real `git diff` output, tokenized as text.

**These numbers are a floor, not a total.** They count only the documents a
session must read before it can act. Reasoning, tool output, GitHub state
reconstruction, iteration and generated output are on top.

## Measured today

| Session | Mandatory reading |
|---|---:|
| Worker, foundational (`WP-H1-02`) | 50,414 |
| Reviewer, foundational (incl. mean real candidate diff) | 83,091 |
| Worker, non-foundational harvest (`WP-PA-04`) | 53,000 |
| Reviewer, non-foundational (`WP-PA-03`, in review now) | 68,414 |

Largest single items:

| Item | Tokens | Note |
|---|---:|---|
| Mean foundational candidate diff | 32,968 | HK-08A / HK-10 / HK-GATE, measured |
| Common boot set (6 files, every role) | 19,487 | of which `ROADMAP.md` = 8,221 |
| `WP-HK-GATE` contract + full evidence | 12,709 | read by every downstream WP |
| Accepted `WP-PA-02` result + evidence | 14,783 | of which 9,048 is process residue |
| Binding PA amendments | 12,355 | |

One 40-character SHA costs **25 tokens**. 199 files carry at least one; single
merge SHAs appear in up to 13 files.

## Empirical repair rate

Parsed from the acceptance paragraphs in `Docs/ROADMAP.md`:

- foundational (`HK-*`): 12 accepted workpacks, 11 failed candidates → **0.92 repair cycles per workpack**;
- non-foundational: **do not use the figure this document originally carried.**
  It read "0 observed" and was wrong: it came from a single ROADMAP narrative
  (`WP-H1-00`) and was not representative. Primary sources contradict it —
  `WP-PA-02` (PR #97) and `WP-PA-03` (PR #100) each record `fail_cycle: 1`, and
  PR #99 took an independent FAIL plus two voluntary reopens. The sample is too
  small and too mixed in failure class to state a rate; what it does show is that
  recent non-foundational and process candidates are not failure-free, so any plan
  that budgets clean single cycles for them has no support in this history.

A foundational workpack therefore costs ~1.92 Worker sessions **and** ~1.92
Reviewer sessions, i.e. ~256k tokens of mandatory reading before any reasoning.

## Modelled after the changes

The model applies four representation changes. Two were built and measured, not
estimated (`Docs/engineering/token-budget-prototypes/`):

1. **Machine-readable state** replaces `ROADMAP.md` + workpack index in the boot
   set: **2,220 vs 10,578 → −79%** (measured).
2. **`INHERITED_CONTRACT.md` per accepted workpack** replaces re-deriving the
   predecessor contract from its workpack plus full evidence: **496 vs 12,709 →
   −96%** (measured, for `WP-HK-GATE`).
3. **Role-tiered boot set** — `FOUNDATIONAL_PROOF_STANDARD.md` is not loaded by
   non-foundational sessions; the track index is not loaded twice.
4. **Predecessor evidence is not inherited context** — a successor reads the
   accepted *result*, not the predecessor's `WORKER_PLAN` / `WORKER_PRE_REVIEW` /
   `HANDOFF` / `DOCSYNC` (9,048 tokens of process residue in PA-02 alone).

| Session | Now | After | |
|---|---:|---:|---:|
| Worker, foundational | 50,414 | 27,399 | −46% |
| Reviewer, foundational | 83,091 | 60,076 | −28% |
| Worker, harvest | 53,000 | 33,498 | −37% |
| Reviewer, non-foundational | 68,414 | 48,912 | −29% |

Across the 27 remaining workpacks (13 foundational, 14 non-foundational), at the
measured 0.92 repair rate:

**5.03M → 3.34M tokens of mandatory reading, −34% (≈1.69M saved).**

## What does not shrink, and why that is the point

After these changes the biggest remaining item is the **candidate diff**
(32,968 tokens for a foundational review). That is irreducible: the Reviewer is
required to inspect the complete baseline→candidate diff, and cutting it would
be cutting rigor, not waste. A budget dominated by the diff is a healthy budget.

Binding PA amendments (12,355) and the H1 architecture + ADRs (6,826) are the
next candidates for inherited-contract treatment. Neither is measured here.

## An untested second-order hypothesis

Both recorded foundational failures were **ownership / inheritance** defects:
`WP-HK-10` failed because a semantic gap discovered at closure was owned by HK10
instead of reopening HK01; `WP-HK-GATE` failed because residual reconciliation
did not consume the complete accepted HK10 universe. Change 2 above formalises
exactly that boundary as a frozen artifact written by the workpack that proved
it, instead of prose re-derived by each consumer.

If it lowers the repair rate from 0.92 to 0.5, the projection improves to
**2.86M, −43% against today**. This is a hypothesis with a plausible mechanism,
**not a measured result**. It should be tested by recording the repair rate over
the next foundational workpacks, not assumed.

## Risk boundary

The savings above come from **representation** — prose to structure, duplicated
to single-source, process residue excluded from inherited context. None of them
removes evidence, relaxes exact-SHA binding, or narrows what a Reviewer must
challenge. The same rule `AGENTS.md` applies to terminology cleanup applies
here: reducing cost must never weaken the underlying proof obligation.

The measurable failure mode to watch is the one PR `#99` already names as a
review target — context minimisation hiding evidence a Worker or Reviewer needs.
Any adopted change should be paired with the repair-rate metric above, so a
saving that buys itself extra FAIL cycles is visible rather than invisible.

---

# Trajectory: what this costs by H4

Measured from history with `scripts/token-budget/growth.py`, which tokenizes the
boot set at every commit that touched `Docs/ROADMAP.md`. The boot set grew from
**2,385 tokens on day one to 22,497 across 19 accepted workpacks.**

## Two growth terms, measured

1. **Linear, per accepted workpack: +427 tokens of boot set** (of which +244 is
   the `ROADMAP.md` acceptance paragraph). Predictable and arithmetic.
2. **Step, per accepted phase: +4,325 to the boot set** — measured across the
   H1 plan merge, at zero new accepted workpacks — **plus 6,826 of binding
   architecture** (`H1_ENGINE_BRIDGE_ARCHITECTURE.md` + `ADR-H1-*`) that enters
   the read set of every foundational session and never leaves it.

Term 2 is the one that compounds. Each phase permanently adds a layer that all
later phases must read.

## Extrapolation, unchanged process

Mandatory reading before a foundational Worker writes anything. `% useful` is the
share that is the current workpack and its phase plan, rather than inherited
context.

| Phase | boot | architecture | Worker | Reviewer | % useful |
|---|---:|---:|---:|---:|---:|
| today | 22,497 | 6,826 | 46,785 | 79,753 | 8.1% |
| end of H1 | 28,475 | 6,826 | 52,763 | 85,731 | 7.1% |
| H2 | 39,632 | 13,652 | 70,746 | 103,714 | 5.3% |
| H3 | 50,789 | 20,478 | 88,729 | 121,697 | 4.2% |
| **H4** | **61,946** | **27,304** | **106,712** | **139,680** | **3.5%** |

## Extrapolation, levers applied

`INHERITED_CONTRACT.md` changes a phase's permanent export from 6,826 to ~496.
Machine-readable state changes the per-workpack marginal from 244 to ~90.

| Phase | boot | architecture | Worker | Reviewer | % useful |
|---|---:|---:|---:|---:|---:|
| today | 14,139 | 6,826 | 26,214 | 59,182 | 14.4% |
| end of H1 | 17,961 | 6,826 | 30,036 | 63,004 | 12.5% |
| H2 | 22,629 | 7,322 | 35,200 | 68,168 | 10.7% |
| H3 | 27,297 | 7,818 | 40,364 | 73,332 | 9.3% |
| **H4** | **31,965** | **8,314** | **45,528** | **78,496** | **8.3%** |

A Reviewer at H4 with the levers costs less than a Reviewer today without them.

## The binding constraint is not the bill

At ~140k tokens of mandatory intake, an H4 Reviewer does not have a cost problem.
It has nowhere left to think. It must hold the inherited contracts, the complete
candidate diff and its own adversarial reasoning at once, and reproduce material
tests. Context quality degrades well before any hard limit, so the failure mode
is qualitative: **independent review goes shallow exactly in the phase that needs
it most.** That is a proof-quality risk, not an efficiency one.

The clearest single indicator is the `% useful` column. Unchanged, the
signal-to-noise of a Worker's intake **halves between today and H4** (8.1% →
3.5%). With the levers it stays flat (14.4% → 8.3%). What the changes buy is not
a discount; it is the removal of the slope.

## Caveats

- Phase sizing for H2–H4 is assumed at 16 workpacks each, matching H1's 15 and
  H0's 19–21. No H2+ plan exists yet; these are projections, not commitments.
- The projection assumes each accepted phase keeps exporting binding
  architecture at H1's measured rate. If a future phase closes into a gate the
  way H0 did into `WP-HK-GATE`, its step would be smaller.
- The levers' effect on term 2 is modelled from one measured inherited contract
  (`WP-HK-GATE`, 496 tokens), not from a phase that has actually adopted it.
