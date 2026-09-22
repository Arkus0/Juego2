# Token budget re-measurement — NON-CONTRACTUAL

Measured at `main` = `f7b4f1e8247dfc927203dca6754b71aaa53938f3` (2026-09-22).
Prior artifact: `TOKEN_BUDGET_BASELINE.md` on `claude/token-savings-project-owqkzj`,
pinned to `a57afa3d3fc60b3e1c59d04149fe982f387c2048`.

External-analyst input only. This document defines no contract, binds no workpack,
weakens no acceptance criterion, and is not accepted evidence. It has not been
through independent review. Where it appears to conflict with `AGENTS.md`,
`Docs/ROADMAP.md`, `Docs/workpacks/**` or accepted evidence, those win.

Tokenizer: Claude tokenizer bundled in `anthropic==0.21.3`. Close proxy, not exact.
**Treat every figure as ±10%.** Reproduce with `scripts/token-budget/`.

## What changed since the anchor

`measure.py` reading sets are deliberately PRE-CTX: they are the "before" side and
were not edited. `after.py` is new and reads the derived role profiles that CTX-01
made authoritative (`Docs/engineering/context-bootstrap-profiles.json`), so both
sides stay comparable.

| set | a57afa3 | f7b4f1e | delta |
|---|---:|---:|---:|
| worker-foundational (WP-H1-02) | 50,414 | 53,382 | +2,968 |
| reviewer-foundational | 50,123 | 52,597 | +2,474 |
| worker-harvest (PA) | 53,000 | 55,430 | +2,430 |
| reviewer-harvest (PA) | 52,709 | 54,645 | +1,936 |

The `+427` linear boot term per accepted workpack reproduces exactly over the full
32-revision ROADMAP history. The boot set rose **17,937 → 22,497 (+4,560, +25.4%)**
across 2026-09-20→21 at a constant 19 accepted workpacks — growth with zero
accepted product work, during the token-savings effort itself.

Across `a57afa3..f7b4f1e`: **+49,691 tokens added, 1,390 removed** (a single
trimmed handoff prompt). The binding corpus is strictly additive so far.

## What CTX-01 delivers, as merged

CTX-01 is routing, not deletion. It removes nothing; it permits skipping. The
delivered saving is therefore a range, not a number, and the width of that range is
decided by whether the `must_escalate_if` predicates fire.

WP-H1-02 worker cold start, measured at `f7b4f1e`:

| | tokens | vs 53,382 |
|---|---:|---:|
| pre-CTX rules | 53,382 | — |
| CTX-01 **MIN** (profile pack, no escalation) | 36,952 | −30.8% |
| CTX-01 **ESC** (architecture + hk00 escalate) | 47,222 | −11.5% |

Reviewer incl. the mean candidate diff (32,968, read in full — closed):
85,565 → **69,135 MIN / 79,405 ESC** (−19.2% / −7.2%).

For WP-H1-02 specifically, the ESC column is the likely one: the WP names an
architecture/authority boundary, declares HK00/HK00A inherited guarantees, and
owns Unity dependency/IP adoption records — three separate triggers for
`the current claim binds an architecture/proof source outside the initial pack`.

**Nothing records which branch was taken.** The 10,270-token spread is resolved by
an unaudited judgement made by the session that benefits from skipping, and no
artifact of that decision survives into review.

Measured end-to-end from the anchor, the worst case improved by 3,192 tokens
(50,414 → 47,222, −6.3%); roughly half of CTX-01's worst-case saving was consumed
by corpus growth that landed while CTX-01 was in flight.

## The cost CTX-01 does not touch

`direct_dependencies` (12,709 for WP-H1-02) is untouched by design — CTX-01 §4
preserves the full predecessor check and assigns capsules to CTX-02.

The dominant term project-wide is the cumulative PA chain, which is contractually
cumulative in workpack text (`WP-PA-13`: "all accepted PA-01..12 findings"):

| | tokens |
|---|---:|
| PA results measured (PA-01/02/03) | 3,444 / 5,735 / 7,281 |
| PA-01..12 findings, flat @ PA-03 size | 87,372 |
| PA-01..12 findings, linear continuation (+1,919/WP) | 167,982 |
| both crosscutting amendments (binding to every PA WP) | 7,707 |
| **WP-PA-13 worker cold start, post-CTX-01, flat** | **124,396** |

That is 2.6× the foundational kernel session. Independently measured: CITY
production corpus 67,509; CITY accepted evidence 110,249.

## Growth vs. the one-time saving

65 workpack files exist, 19 accepted. H2 has no workpacks yet.

| term | tokens |
|---|---:|
| ~46 remaining planned WPs × 427 | 19,642 |
| H2 + H3 phase steps (2 × 4,325) | 8,650 |
| H2 + H3 binding architecture (2 × 6,826) | 13,652 |
| **further boot growth to H4** | **~41,944** |

CTX-01 best case saves 16,430 once. Remaining growth adds ~41,944 — **2.6× the
entire CTX-01 saving**. CTX changes the intercept; nothing in CTX-01/02/03 changes
the slope.

## Reproduce

```bash
pip install tokenizers
pip download anthropic==0.21.3 --no-deps -d /tmp/a
unzip -o -j /tmp/a/*.whl anthropic/tokenizer.json -d scripts/token-budget/
git fetch --unshallow origin main          # growth.py needs full history
python3 scripts/token-budget/measure.py    # before (PRE-CTX sets, do not edit)
python3 scripts/token-budget/after.py      # after  (reads CTX-01 role profiles)
python3 scripts/token-budget/growth.py     # growth curve over ROADMAP history
```

## Not re-derived here

The 0.92 foundational repair rate (11 FAILs in 12 WPs) is carried from the prior
artifact and was not independently re-derived in this pass.
