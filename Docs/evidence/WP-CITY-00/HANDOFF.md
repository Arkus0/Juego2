# WP-CITY-00 — Worker handoff

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Branch: `claude/city-urban-topologies-ehn6qf`
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Worker history: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Transfer SHA: NONE
fail_cycle: 0

The exact `Frozen candidate SHA` is recorded in the PR body and equals PR HEAD at freeze. Evidence
files cannot name their own commit; the PR body is the binding record.

## What this workpack decided

A **confluence-wedge** city: two watercourses with deliberately different roles, seven substantial
district families plus two edge families, one historic river crossing plus four cheap stream
crossings, and a working fluvial landing below the confluence.

It also **shrank the city**. The track's starting size hypotheses were not merely debatable, they
were internally inconsistent: a 12–18 minute cross-city walk and 0.8–1.2 km² of dense fabric cannot
both be true. The travel hypothesis was kept because it is tied to felt experience; the area
hypothesis was replaced with 0.30–0.45 km², and the first retained seed with 0.03–0.06 km².

## Where to read it

| Document | What it carries |
|---|---|
| `Docs/production/CITY_SPATIAL_CONSTITUTION.md` | the deliverable: district graph, crossings, port, scale envelope, fabric vs scenic envelope, twelve invariants, rejected alternatives, open questions |
| `Docs/evidence/WP-CITY-00/WORKER_PLAN.md` | predecessor contract check, claim boundary, scope guard |
| `…/TOPOLOGY_A_DOS_ORILLAS.md`, `…_B_CUNA_CONFLUENCIA.md`, `…_C_RIBERA_LARGA.md` | the three candidate cities in full |
| `…/REFUTATION_LOG.md` | charges, defences and verdicts for all three |
| `…/COMPARISON_MATRIX.md` | eleven-criterion matrix plus the six negative gates |
| `…/SCALE_ENVELOPE.md` | the arithmetic, the building-count estimate, the change conditions |
| `…/WORKER_PRE_REVIEW.md` | the seven findings repaired before freeze, and the acceptance walkthrough |

## For the independent Reviewer

This session acted as **Worker**. Under `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` it may not
review its own candidate; the review must start from a fresh, independent context.

### Validation status — read this before trusting any green check

`scripts/arkus-verify-exact-sha.sh` resolves canonical validation entrypoints for `WP-HK-*` only.
There is none for CITY, and `Docs/workpacks/README.md` exempts non-foundational tracks from exact-SHA
evidence. The PR therefore declares `Mode: PROCESS_ONLY`, which makes Automation V2 emit a
**synthetic** receipt.

Consequences, stated plainly:

- the green check validates **nothing**; it records that no canonical validation applies;
- `state-transitions.yml` will **not** publish a `REVIEW_READY` marker for this PR;
- the human therefore starts the independent Reviewer manually. Its absence is expected, not a fault.

### Where a Reviewer is most likely to find something

Offered as orientation, not as a boundary — the protocol requires the Reviewer to search beyond what
the Worker highlighted.

1. **The scale arithmetic.** Everything downstream rests on an assumed 1.15 m/s and a route factor of
   1.25–1.4. Both are declared assumptions. If they are wrong, the areas move.
2. **The building-count estimate.** Coverage ratios and average footprints per district are planning
   assumptions stated so they can be contested.
3. **The port.** Its plausibility depends on a fictional premise about the river below the confluence
   that `WP-ART-00` has not confirmed (constitution §8, Q1). Whether leaving it open is correct, or
   whether it should have blocked the selection, is a fair challenge.
4. **The refutation.** Whether Topology C was rejected on a sound reading of the river criterion, and
   whether Topology A's rejection really is coupled to the size decision rather than to preference.
5. **The blueprint edit.** Whether v0.3 stays inside "sharpen or explicitly propose amendments" and
   changes nothing it should not.

## What is deliberately left undone

- `Docs/workpacks/CITY/WP-CITY-00.md` status is untouched; transitions belong to post-PASS DocSync.
- `Docs/engineering/RESIDUAL_LEDGER.md` is untouched; it declares itself as covering accepted H0
  workpacks. CITY residuals live as open questions in the constitution §8.
- No `WP-CITY-01` work has begun, and none may begin before an independent PASS on this candidate.

## Next dependency-valid step

`WP-CITY-01 — Mobility, district graph + walk-time topology`, which depends on `WP-CITY-00` PASS and
blocks only `WP-CITY-02`. It is non-foundational and cannot affect `WP-HK-GATE`, which remains the
next dependency-valid H0 workpack independently of anything in this track.
