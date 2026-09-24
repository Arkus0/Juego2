# PA track — Living World research and adoption programme

Status: **ACTIVE / PLAN ACCEPTED / PROCESS_ONLY**  
Version: 1.6 — 2026-09-22  
Repository: `Arkus0/Juego2`

Programme plan accepted on candidate `95fc0f5935561fd61a20221f993e2532ce513795`, independent review `#5267699844`, PR `#84`, merge `4c1672db639f9d56e7bd6f837ff5e1881d789b16`.

`WP-PA-01` is **COMPLETE**. Candidate `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715` passed independent review `#5268013445` in PR `#89` and merged as `3688b7b9a27355b0fda160c20e57a385e40c6814`.

`WP-PA-02` is **COMPLETE**. Candidate `015bb28ddc9facc46459c2d1dd87a89740b6c9ef` passed independent review `#5270038879` in PR `#97` and merged as `85d23489a6478da6bc9f33c3f017640e46ab05e9`.

`WP-PA-03` is **COMPLETE**. Candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927` passed independent review `#5270421825` in PR `#100` and merged as `2f3862b601b0521a6a3d5a57afe54f182d037e97`.

`WP-PA-04` is **COMPLETE**. Candidate `5d38ea38b983cd5227f57afa1d880d24746f9249` passed independent review `#5280879115` in PR `#129` and merged as `d6041b719292f24c4481dea28727e2cfd5f7ed5b`.

`WP-PA-05` is **COMPLETE**. Candidate `99890f1af10691ef7e38f8722830dd0f66529665` passed independent review `#5281462911` in PR `#132` and merged as `31f8258cf2873e9080d9dacad2cfe956f0e2fa2e`.

`WP-PA-06` is **COMPLETE** after post-PASS repair. Corrected candidate `9ce17952e257587c0dcec5dc4b854170365545ca` passed independent correction review `#5300025658` in PR `#172` and merged as `fdfec568844bc0d006959c3cd207795544eca33d`. The next executable PA workpack is `WP-PA-07`.

## Purpose

Turn the existing Living World research programme into an executable Worker → independent Reviewer sequence without pretending that research PASS means gameplay/runtime PASS.

PA answers **what behaviour/product semantics Juego2 needs and what failure modes must be prevented**. Later H2+ phases own implementation, Unity/runtime proof, tuning and performance.

This track is intentionally usable while the user is remote.

## Authority and continuity

Canonical PA meanings/order remain owned by `Docs/research/living-world/PA_ROADMAP.md`.

The donor `Arkus0/Juego` archive is research evidence/provenance only. Its PA-01..05 dossiers received independent review, but old verdicts do not automatically acquire Juego2 authority. `Docs/research/living-world/PA_LEGACY_HARVEST.md` records what can be cheaply revalidated instead of researched from zero.

Current Juego2 cross-cutting amendments are binding inputs to every relevant PA research workpack:

- playable causal city;
- intentional transformation;
- emergent governance;
- current PA-08 leisure/activity plan;
- current PA-09 player-causal-agency plan.

No PA workpack may reopen H0/H1 semantics, make Unity canonical truth, or silently design an unowned H2+ runtime API.

## Result artefact rule

Frozen plans and amendments are immutable research inputs. Executing a PA MUST NOT overwrite them, including the current PA-08/PA-09 plan files whose historical filenames do not contain `_PLAN_`.

The canonical Juego2 result artefact for each executed workpack is:

```text
Docs/research/living-world/results/PA-XX.md
```

Supporting evidence may live beside it in a PA-specific evidence directory if needed. A result file records findings, dispositions, scenarios, provenance, deferred empirical proof and Reviewer status; it does not silently mutate its preregistration plan.

## Execution classes

### `REMOTE_HARVEST`

PA-01..05. The expensive research already exists and was independently reviewed in Juego. These WPs re-read exact donor evidence, remove donor architecture, reconcile current Juego2 product deltas and publish compact Juego2 findings.

These five WPs are **adoption/revalidation gates, not a restart of PA-01..05 research**. Unless the audit finds a concrete conflict, unsupported claim or current-product gap, the Worker should not repeat broad prior-art research or rebuild the donor dossiers from zero.

A harvest WP should be materially smaller than repeating the original study. If it turns into broad new prior-art research without an identified conflict/gap, it has failed its scope discipline.

### `REMOTE_RESEARCH`

PA-06..13. These can be researched without Unity/local hardware. Donor frozen plans are useful preregistration inputs where present, but they must be rebased onto current Juego2 ownership and amendments before research begins.

`PA-08` and `PA-09` already have frozen Juego2 plans; their workpacks execute those plans rather than inventing replacements.

### `DEFERRED_INTEGRATION`

PA-14. Final integration needs all prior PA findings **and an actually completed H2 playable-shell phase**, so it can target real H3+ consumer boundaries instead of an imagined H2 architecture.

Formal PA-14 execution therefore waits until:

1. PA-01..13 are accepted in Juego2; and
2. H2 has reached its future accepted closure/gate, whatever exact ID/name the later reviewed H2 plan assigns to that closure.

The future H2 plan owns its exact IDs/gate names; this PA plan deliberately does not invent them early.

## Execution chain

```text
WP-PA-01 ✅ Daily Life harvest/revalidation              REMOTE_HARVEST
   ↓
WP-PA-02 ✅ NPC Agency harvest/revalidation              REMOTE_HARVEST
   ↓
WP-PA-03 ✅ Social Graph harvest/revalidation            REMOTE_HARVEST
   ↓
WP-PA-04 ✅ Knowledge/Beliefs harvest/revalidation       REMOTE_HARVEST
   ↓
WP-PA-05 ✅ Rumours harvest/revalidation                 REMOTE_HARVEST
   ↓
WP-PA-06 ✅ Memory & Consequences                        REMOTE_RESEARCH
   ↓
WP-PA-07    Work / Material Dependencies                 REMOTE_RESEARCH
   ↓
WP-PA-08    Leisure / Activities / Minigames             REMOTE_RESEARCH
   ↓
WP-PA-09    Player Causal Agency                         REMOTE_RESEARCH
   ↓
WP-PA-10    Autonomous Events / Causal Chains            REMOTE_RESEARCH
   ↓
WP-PA-11    Investigation / Legibility                   REMOTE_RESEARCH
   ↓
WP-PA-12    Governance                                   REMOTE_RESEARCH
   ↓
WP-PA-13    Simulation Control / Anti-chaos / Budgets    REMOTE_RESEARCH
   ↓
WP-PA-14    Integration Review                           DEFERRED_INTEGRATION
```

PA-13 may collect cross-cutting failure/budget observations throughout the programme, but its formal acceptance still comes after PA-12 so it can attack the composed design.

## What a PA PASS means

A PA PASS means the research question has a reviewed Juego2 answer good enough to become input to later design/implementation.

It does **not** mean:

- the runtime exists;
- Unity demonstrates the behaviour;
- tuning constants are validated;
- CPU/save budgets are proven;
- H2/H3/H4/H7 automatically adopt every recommendation;
- a future consumer may skip its own Worker/Reviewer proof.

Each PA result must clearly separate:

```text
RESEARCH FINDING
what behaviour/invariant is justified

FUTURE CONSUMER
which later product phase probably needs it

DEFERRED EMPIRICAL PROOF
what cannot honestly be proven until playable/runtime work exists
```

## Product-shape requirements

Every accepted PA must help the actual game rather than merely making the simulation sophisticated.

Track-wide negative gates:

1. **No living museum:** NPC autonomy without meaningful player perturbation is insufficient.
2. **No procedural soap opera:** important chains need bounded initiation, escalation, termination and recovery.
3. **No omniscience:** actors cannot consume hidden engine truth/lineage as personal knowledge.
4. **No talk-or-hit world:** embodied non-dialogue action and ordinary activities must have causal room.
5. **No private minigame universe:** activities can emit bounded outcomes to normal owners.
6. **No quest-script causality disguised as simulation:** shared semantic actions/outcomes should not require privileged player-only state paths.
7. **No combat consequence island:** later combat outcomes must be consumable by the same social/material world.
8. **No infinite biography/state growth:** continuity is selective and budgeted.
9. **No accidental anarchy attractor:** ordinary/clumsy play should tend toward recoverable normality.
10. **No designer force-field around the initial town:** sustained coherent player action may genuinely transform it.
11. **No governance mind control:** policy changes rules/resources/opportunities; actors react through their own systems.
12. **No premature runtime architecture:** PA research may specify semantic needs and fixtures, not smuggle a final implementation topology into H2+.

## Relationship to current production phases

The current non-binding Production Blueprint gives useful consumer intent:

- **H2:** keeper playable shell, navigation, movement/camera/interaction;
- **H3:** actors, animation, clock, schedules, POIs and daily life;
- **H4:** Living World Core — knowledge, relationships, events, memory, structured outcomes, runtime/save minimum;
- **H5:** GameFlow/cinematics/QTE;
- **H6:** combat returning outcomes into Living World;
- **H7:** deeper agency, abstract simulation and population scaling.

PA does not freeze those future phases. It should reduce the number of product questions they have to invent while implementing.

## Relationship to CITY and H1

PA is a parallel non-foundational research track.

- It does not block H1.
- It does not block CITY-05/06/03/04/07/08.
- Accepted CITY locations/routes/depth can be used as scenario context, but PA does not own geography.
- H1 owns Unity bridge capability; PA does not invent bridge semantics.
- Later H2+ consumers decide how accepted PA findings become runtime/product contracts.

This means remote PA work can continue while local H1/CITY work is unavailable.

## Reviewer continuation rule

Every PA Reviewer verdict must end with an explicit continuation line.

On `PASS` with another PA workpack remaining:

```text
Next PA workpack: WP-PA-XX — <name>
Execution class: <REMOTE_HARVEST | REMOTE_RESEARCH | DEFERRED_INTEGRATION>
Prerequisites: satisfied | <exact unsatisfied prerequisite>
```

On final `WP-PA-14` PASS when no PA research gap remains:

```text
Next PA workpack: none — programme complete
Execution class: none
Prerequisites: n/a
```

On `FAIL`, the next workpack remains the same WP; the Reviewer names the causal blocker and does not advance the track.

This avoids making the user reconstruct the canonical sequence from memory and gives PA-14 a non-circular terminal state.

## Current next workpack

`WP-PA-07 — Work, Businesses & Material Dependencies research`.

PA-01 through PA-06 are accepted in Juego2. PA-06's post-PASS correction closed aggregate selected-memory boundedness and anti-post-hoc selection before downstream use; PA-07 is now the next dependency-valid remote research workpack.
