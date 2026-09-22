# H0S — Post-GATE scale + concurrent-authoring / viability track

Status: **PARALLEL / NON-BLOCKING BY DEFAULT**

H0S begins from measured or falsifiable evidence. It may run alongside H1 and later product work and becomes blocking only when evidence shows that a required product assumption cannot be met safely with the current architecture.

H0S must not invent shipping-scale constants before representative runtime/hardware exists, and must not weaken accepted H0 correctness, validation, provenance, replay or deterministic-state guarantees merely to improve throughput.

## Current sequence

| Order | Workpack | Status | Purpose |
|---:|---|---|---|
| 1 | `WP-H0S-00` | PLANNED / NOT_STARTED | early structural scalability viability spike: bounded candidate work + bounded ABSTRACT→FULL reconciliation |

Additional H0S workpacks are evidence-driven rather than predeclared. Candidate follow-up areas include incremental/indexed authoring state, validation/hash cost, writer coordination, scoped preconditions, change feeds, memory growth and other scale/concurrency changes justified by H1/H0S measurements.

## Two carried viability invariants

`WP-H0S-00` records two product-risk proposals that later runtime work must preserve or replace only with stronger reviewed evidence:

1. **SV-1 — irrelevant-population independence / bounded candidate surfaces.** Local/bounded decisions may not become global scans or unbounded all-pairs work as unrelated population or local density grows.
2. **SV-2 — bounded ABSTRACT→FULL reconciliation.** Off-screen simulation may not defer an unbounded debt of omitted micro-ticks that must later be replayed on promotion.

The initial synthetic planning evidence is recorded at `Docs/evidence/H0S-00/INITIAL_SYNTHETIC_SPIKE.md`. It is deliberately not a claim about final CPU, memory, population or frame/tick budgets.

`WP-PA-13` consumes these invariants on the living-world research side and must carry them into later H3/H4/H7 runtime acceptance fixtures.
