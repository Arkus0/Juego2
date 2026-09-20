# WP-CITY-00 — Worker handoff

WP: WP-CITY-00 — Keeper City spatial constitution + scale envelope
Contract: `Docs/workpacks/CITY/WP-CITY-00.md`
Baseline SHA: 290f92e9c21f1e454e0d6924b29f4d778b9875f8
Branch: `claude/city-urban-topologies-ehn6qf`
Active Worker: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Worker history: Claude Code — session 01KC1S5dCRLuMq6qeht4n34L
Transfer SHA: NONE
fail_cycle: 3

## Repair cycle 3

Frozen candidate `daefc6a1e4959c1cffacd662f5578a4c13a642d3` received independent **FAIL**, review
[#5261488049](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261488049).

A summarising sentence written directly beneath the crossing table claimed the south bank is never
left on a single crossing — when the barca-suspended row is exactly that — and counted the
double-failure case as one crossing when it is zero. CSI-06 carried the claim into the permanent
contract.

Repaired by restating CSI-06 as a **design obligation** rather than a promise the weather can break,
giving §2.4.1 an explicit crossing-count ladder (2 → 1 → 0 per state), deleting the dossier's
paraphrase, and correcting the seam-count drift in four places plus a broken section reference. No
spatial decision changed.

**Three FAILs, one shape:** a summarising absolute claim written beside the artefact it summarises
and never checked against it — each time with the contradiction under twenty lines away, and this
time in the same commit. "Read more carefully" has been tried three times. The pre-review now runs a
mechanical **absolute-quantifier audit** instead (`WORKER_PRE_REVIEW.md` §3), and `WORKER_PLAN.md`
adds the corollary that an evidence document must not restate an invariant in its own words.

## Repair cycle 2

Frozen candidate `6091584313488bcfb1840132f22adcc9b1b5de4f` received independent **FAIL**, review
[#5261418045](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261418045).

The cycle-1 repair held, but it had added the barca as a crossing without reconciling what that edge
falsifies: both crossing tables still said closing the Puente Viejo cut the south side off, and
neither semantic graph drew the ferry — so the district graph and the crossing strategy described
two different cities.

Reconciled around two explicit connectivity states (`CITY_SPATIAL_CONSTITUTION.md` §2.4.1), with
closure consequences derived from the dimensional sketch instead of asserted, both graphs redrawn,
and both tables restructured to separate availability from closure. No spatial decision changed.

**Both failures share one shape:** closing over something without checking what else depended on it.
The pre-review now carries a cross-check for it (`WORKER_PRE_REVIEW.md` §4) and `WORKER_PLAN.md`
carries the rule.

## Repair cycle 1

Frozen candidate `b9473f04da72462619e9561c097083afdc6c436d` received independent **FAIL**, review
[#5261386416](https://github.com/Arkus0/Juego2/pull/65#pullrequestreview-5261386416).

The constitution had justified the landing on a premise it itself called invented and unaccepted by
ART, and left it open while admitting its refusal would reopen the topology. The same defect had
inverted the CITY→ART authority direction in §2.5 and CSI-05.

Repaired by removing the need for the premise rather than arguing or deferring it: the landing now
rests on confluence hydrology plus timber rafting at high water, áridos worked off the bars, a roped
ferry and break-bulk for the arriving valley road — all inside the small-landing/working-boats
direction `Docs/art/SETTING.md` already gives. The spatial work the review found sound is unchanged.
No ART decision is required or pending.

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
3. **The landing after repair.** Whether §2.5's four reasons really need nothing beyond the inherited
   setting, and whether the district still earns its place now that the city is not on a trade route.
   The honest weak point is seasonality: on its river side the landing works only at high water, so
   CSI-04 carries more weight than it looks.
4. **The refutation.** Whether Topology C was rejected on a sound reading of the river criterion, and
   whether Topology A's rejection really is coupled to the size decision rather than to preference.
5. **The blueprint edit.** Whether v0.3 stays inside "sharpen or explicitly propose amendments" and
   changes nothing it should not.
6. **Whether the new audit's scope is right.** Three cycles failed on one shape, and the
   countermeasure (`WORKER_PRE_REVIEW.md` §3) is unproven on a fresh candidate. A fourth instance
   would most likely live where the grep pattern does not reach — a claim phrased without an absolute
   quantifier, or one inside the evidence files rather than the deliverable.
7. **The State 1 → State 2 transition.** A real change in the city's connectivity that no workpack
   yet owns the timing of (residual 6). Whether CITY-00 was right to leave it unscheduled is a fair
   challenge.
8. **The seasonal single-crossing state.** For part of the year the south bank runs on the Puente
   Viejo alone (§2.4.1). It is deliberate, but whether the constitution should say more about what
   may and may not be sited across the river is arguable.

## What is deliberately left undone

- `Docs/workpacks/CITY/WP-CITY-00.md` status is untouched; transitions belong to post-PASS DocSync.
- `Docs/engineering/RESIDUAL_LEDGER.md` is untouched; it declares itself as covering accepted H0
  workpacks. CITY residuals live as open questions in the constitution §8.
- No `WP-CITY-01` work has begun, and none may begin before an independent PASS on this candidate.

## Next dependency-valid step

`WP-CITY-01 — Mobility, district graph + walk-time topology`, which depends on `WP-CITY-00` PASS and
blocks only `WP-CITY-02`. It is non-foundational and cannot affect `WP-HK-GATE`, which remains the
next dependency-valid H0 workpack independently of anything in this track.
