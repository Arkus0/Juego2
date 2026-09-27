# PA track — Living World research and adoption programme

Status: **ACTIVE / NON-FOUNDATIONAL RESEARCH**  
Version: 2.0 — 2026-09-25  
Repository: `Arkus0/Juego2`

## Accepted state

`PA-01` through `PA-13` are accepted in Juego2. PA-07..09 were accepted through `WP-PA-B1`; PA-10..12 through `WP-PA-B2`; PA-13 through `WP-PA-B3`. Their result artefacts, independent reviews and binding DocSync records remain authoritative inputs.

PA answers **what behaviour/product semantics Juego2 needs and what failure modes later runtime work must prevent**. PA research does not itself prove Unity/runtime behaviour.

## New execution policy

Remaining research uses `RESEARCH_BATCH` under `Docs/workpacks/PRODUCT_EXECUTION_POLICY.md` and the binding `PA_BATCH_EXECUTION_AMENDMENT.md`.

This changes execution packaging, not the meaning of PA-07..13.

The original WP files `WP-PA-07.md` through `WP-PA-13.md` remain binding **unit specifications**. They are no longer seven separate Worker -> review -> merge -> DocSync cycles after the batch amendment is accepted.

Each canonical result file remains separate under:

`Docs/research/living-world/results/PA-XX.md`

## Execution chain

```text
PA-01 ✅ Daily Life
 -> PA-02 ✅ NPC Agency
 -> PA-03 ✅ Social Graph
 -> PA-04 ✅ Knowledge/Beliefs
 -> PA-05 ✅ Rumours
 -> PA-06 ✅ Memory & Consequences
 -> PA-B1 ✅ { PA-07 Work/Material + PA-08 Activities + PA-09 Player Agency }
 -> PA-B2 ✅ { PA-10 Events + PA-11 Legibility + PA-12 Governance }
 -> PA-B3 ✅ { PA-13 Simulation Control + H3/H4 handoff index }
 -> PA-14    Integration Review — DORMANT until H2-GATE
```

The three remote batches are now accepted. PA-14 remains intentionally deferred until the product has an accepted H2 closure boundary.

## Batch contracts

- `WP-PA-B1.md` consumes PA-07/08/09.
- `WP-PA-B2.md` consumes PA-10/11/12.
- `WP-PA-B3.md` consumes PA-13 and publishes a traceable H3/H4 pre-integration handoff.
- `WP-PA-14.md` remains deferred and is not silently folded into B3.

A batch PASS requires every included original unit to PASS its already-written acceptance contract plus cross-unit consistency.

## What a PA PASS means

A PA PASS means a reviewed semantic/research answer is good enough to become later implementation input.

It does **not** mean:

- the runtime exists;
- Unity demonstrates the behaviour;
- final tuning constants are validated;
- CPU/save budgets are proven;
- H2 proxies are persistent NPCs;
- future consumers may skip their own executable proof.

Each PA result continues to distinguish:

```text
RESEARCH FINDING
FUTURE CONSUMER
DEFERRED EMPIRICAL PROOF
```

## Product-shape requirements

The existing track-wide negative gates remain binding, including:

- no living museum;
- no procedural soap opera;
- no omniscient actors;
- no talk-or-hit world;
- no private minigame universe;
- no quest-script causality disguised as simulation;
- no combat consequence island;
- no unbounded biography/state growth;
- no accidental-anarchy attractor;
- no designer force-field against coherent player transformation;
- no governance mind control;
- no premature runtime architecture.

## Relationship to H2/H3/H4

The accepted H2 plan deliberately keeps behaviour shallow:

- H2 characters are visual/presentation proxies: idle, walk/bounded movement and minimal dialogue/bark only.
- H3 owns persistent actor identity, time, POIs/smart objects, schedules/routines and readable daily life.
- H4 owns deeper Living World causality: beliefs/knowledge, relationships, memory, events, structured outcomes and runtime/save minimum.

PA-B1/B2/B3 prepare those later phases but do not pull their implementation into H2. The accepted PA-B3 handoff additionally requires first bounded local-query/fidelity proofs in H3 where those seams exist and deeper PA-13 control/reconciliation proof in H4, with numeric thresholds deferred to representative profiling.

## Relationship to CITY/H1

PA does not own geography or the Unity bridge. Accepted CITY places/routes may be used as scenario context; H1 remains bridge authority. PA may continue remotely in parallel where its batch prerequisites are satisfied.

## Current next PA workpack

`WP-PA-14 — Living World integration review`.

PA prerequisite: SATISFIED by accepted PA-B3.  
H2 prerequisite: **NOT YET SATISFIED**.  
Execution status: **DORMANT / BLOCKED until accepted H2-GATE**.
