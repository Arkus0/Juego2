# WP-PA-01 — Independent Reviewer handoff

Workpack: `WP-PA-01 — Adopt/revalidate NPC Daily Life findings`  
Execution: `REMOTE_HARVEST`  
Worker: ChatGPT GPT-5.6 Sol  
Baseline: `main@cff6d4d0d40786dd1c002a8cc46e768b478ee3cc`  
Candidate branch: `pa/wp-pa-01-daily-life-harvest`  
Freeze anchor: the exact `Frozen candidate SHA` recorded in the PR body after this file is committed. This file intentionally does not self-hash the commit that contains itself.

## Review target

Primary deliverable:

- `Docs/research/living-world/results/PA-01.md`

Worker evidence:

- `Docs/evidence/WP-PA-01/WORKER_PLAN.md`
- `Docs/evidence/WP-PA-01/WORKER_PRE_REVIEW.md`

Contract:

- `Docs/workpacks/PA/WP-PA-01.md`
- `Docs/workpacks/PA/README.md`

## Exact donor provenance

- donor repo: `Arkus0/Juego`;
- donor dossier: `Docs/living-city-research/PA-01_NPC_DAILY_LIFE.md`;
- exact donor candidate: `42f08346fbf518eb19e1158ea8448926a1a4d86b`;
- donor PR: `#32`;
- donor final independent PASS review: `#5224625310`.

The donor PASS is provenance only; this Juego2 candidate needs its own independent review.

## Worker claim

The candidate harvests only daily-life product semantics still justified: schedule as expected intent, activity/place separation, interruption cleanup + re-evaluation, causal travel, capacity/contention, expected vs actual state, bounded context variants, routine characterization without donor class authority, cheaper off-screen fidelity with causal continuity, and the explicit rule that routine is not autonomous agency.

Juego2 reconciliation makes routine a **perturbable baseline** inside the shared player/NPC causal city. Durable later material/player/institutional change may invalidate old opportunities; PA-01 cannot restore starting conditions by fiat.

## Reviewer attack surface

1. Does any rule smuggle donor M9/M10/`WorldState`, scheduler topology, AI algorithm, Unity representation or API?
2. Does stable expected routine accidentally become destiny and block PA-02 initiative, PA-09 perturbation or durable changed context?
3. Does PA-01 steal PA-08 activity-rule/outcome ownership?
4. Can persistent place/service/access change alter actual routine without a schedule force-field?
5. Does changing only route realization leave authored schedule intent semantically unchanged?
6. Is capacity/contention evidence kept distinct from the exact still-unproven validator architecture?
7. Is expected-vs-actual useful without implying omniscient actor/player knowledge?
8. Was the harvest materially cheaper than redoing donor research?

## PASS continuation

If and only if the exact frozen candidate passes:

```text
Next PA workpack: WP-PA-02 — Adopt/revalidate NPC Agency findings
Execution class: REMOTE_HARVEST
Prerequisites: satisfied
```

On FAIL, remain on `WP-PA-01` and name the causal blocker.

## Worker state

After the PR records the exact final HEAD and is marked Ready, state is `FROZEN_FOR_REVIEW`. No further Worker writes unless independent review returns FAIL and opens a repair cycle.
