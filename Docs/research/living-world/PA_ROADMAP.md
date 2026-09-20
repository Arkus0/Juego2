# Juego2 Living World PA Roadmap

Version: 1.0 — 2026-09-20  
Status: **CANONICAL EXECUTION ORDER / PROCESS_ONLY**  
Repository: `Arkus0/Juego2`

## Purpose

Juego2 uses a single integer-only PA sequence in intended execution/integration order. Historical identifiers from `Arkus0/Juego` and temporary draft identifiers created while harvesting the old programme remain provenance aliases only.

**Rule:** from this point forward, Juego2 planning, reviews, handoffs and new documents SHOULD use the canonical IDs below. Historical IDs may appear only when citing an old frozen artefact or commit.

## Canonical sequence

| Juego2 PA | Capability / research question | Provenance alias |
|---|---|---|
| PA-01 | NPC Daily Life | Juego PA-01 |
| PA-02 | NPC Agency & Autonomous Social Action | Juego PA-02 |
| PA-03 | Social Graph That Changes Behaviour | Juego PA-03 |
| PA-04 | Knowledge, Belief, Ignorance & Deception | Juego PA-04 |
| PA-05 | Rumours & Information Flow | Juego PA-05 |
| PA-06 | Memory & Consequences | Juego PA-06 |
| PA-07 | Work, Businesses & Material Dependencies | Juego PA-07 |
| PA-08 | Leisure, Social Activities & Minigames as Living World Systems | temporary Juego2 `PA-07B` |
| PA-09 | Player Causal Agency & World Intervention | temporary Juego2 `PA-11B` |
| PA-10 | Autonomous Events & Causal Chains | Juego PA-08 |
| PA-11 | Investigation, Legibility & Traces | Juego PA-09 |
| PA-12 | Governance as Intervention in Simulation | Juego PA-10 |
| PA-13 | Simulation Control, Failure Modes & Budgets | Juego PA-11 |
| PA-14 | Integration Review | Juego PA-12 |

## Why the order is canonical

The sequence is deliberately readable as one dependency story:

```text
live a normal day
 -> choose autonomously
 -> relate differently
 -> know different things
 -> transmit information
 -> remember consequences
 -> work in material systems
 -> play/socialise in shared activities
 -> player perturbs the same world
 -> causal chains emerge and terminate
 -> player can understand missed consequences
 -> mayoral/governance interventions affect normal systems
 -> simulation is bounded against spam/anarchy/cost explosion
 -> integrate only accepted findings
```

PA-13 collects failure/budget findings continuously even though its formal closure comes late. The numbering describes the main reasoning/execution spine, not a prohibition on feeding cross-cutting findings forward early.

## Renumbering rule for frozen artefacts

Some frozen documents created immediately before this roadmap contain their creation-time labels inside the text. They are not rewritten retroactively because freeze history is evidence.

Canonical file names and IDs are:

- `PA-08_LEISURE_SOCIAL_ACTIVITIES_AND_MINIGAMES.md` — canonical **PA-08**; internal `PA-07B` mentions are historical aliases.
- `PA-09_PLAYER_CAUSAL_AGENCY.md` — canonical **PA-09**; internal `PA-11B` mentions are historical aliases.
- `PA-09_AMENDMENT_01_EMBODIED_ACTIONS.md` — amendment to canonical **PA-09**; internal `PA-11B` mentions are historical aliases.
- `LIVING_WORLD_CROSSCUTTING_AMENDMENT_01_PLAYABLE_CAUSAL_CITY.md` — applies to the canonical PA-01..PA-14 programme. References to temporary `PA-07B`/`PA-11B` mean canonical PA-08/PA-09.

No research content is discarded by the renumbering.

## Authority / migration boundary

The old `Arkus0/Juego` PA results are research/reference inputs, not automatically accepted Juego2 evidence. Their questions, findings, failure modes and scenarios may be continued or revalidated under the canonical Juego2 IDs, but old PASS/DELTAS_READY status does not silently acquire authority in this repository.

The Juego2 PA programme is also non-binding until reviewed/adopted into the relevant future runtime/workpack/ADR. It does not reopen H0.

## Product spine

All future PA work must preserve the cross-cutting product thesis:

> The town is not a museum and not a procedural-drama machine. It is a playable causal system: NPCs live and act proactively; the player can perturb people, places, activities and institutions through dialogue and embodied action; consequences propagate only through legitimate perception/state owners; ordinary life remains the baseline; important chains are bounded, legible and capable of recovery.

Activities/minigames are part of this spine where accepted: they should be fun as games, able to belong to ordinary town life, and capable of exposing bounded outcomes to the same Living World without becoming isolated score screens or private semantic universes.
