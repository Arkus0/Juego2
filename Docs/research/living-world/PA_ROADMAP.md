# Juego2 Living World PA Roadmap

Version: 1.1 — 2026-09-20  
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

## Current canonical amendments

The following amendments are part of the Juego2 research programme and must be consumed by the relevant PA work rather than treated as optional notes:

- `LIVING_WORLD_CROSSCUTTING_AMENDMENT_01_PLAYABLE_CAUSAL_CITY.md` — Living World must be playable by the player, not merely autonomous NPC simulation; integrates embodied action, activities/minigames, recovery and anti-chaos requirements.
- `LIVING_WORLD_CROSSCUTTING_AMENDMENT_02_INTENTIONAL_TRANSFORMATION.md` — ordinary play should be stable/recoverable, but sustained intentional player pressure may materially transform or destabilise the town; anti-chaos budgets regulate propagation rather than protect the default equilibrium.
- `PA-12_AMENDMENT_01_EMERGENT_GOVERNANCE.md` — governance must operate as high-leverage intervention into the same Living World, with macro decisions producing third-person consequences and no ideology/mind-control shortcut.

## Authority / migration boundary

The old `Arkus0/Juego` PA results are research/reference inputs, not automatically accepted Juego2 evidence. Their questions, findings, failure modes and scenarios may be continued or revalidated under the canonical Juego2 IDs, but old PASS/DELTAS_READY status does not silently acquire authority in this repository.

The Juego2 PA programme is also non-binding until reviewed/adopted into the relevant future runtime/workpack/ADR. It does not reopen H0.

## Product spine

All future PA work must preserve the cross-cutting product thesis:

> The town is not a museum and not a procedural-drama machine. It is a playable causal system: NPCs live and act proactively; the player can perturb people, places, activities and institutions through dialogue and embodied action; consequences propagate only through legitimate perception/state owners; ordinary life remains the baseline; important chains are bounded, legible and capable of recovery.

Activities/minigames are part of this spine where accepted: they should be fun as games, able to belong to ordinary town life, and capable of exposing bounded outcomes to the same Living World without becoming isolated score screens or private semantic universes.

The stability rule is deliberately asymmetric:

> **The simulation protects the player from accidental collapse, not from intentional consequences.**

`Normality is an attractor` means ordinary low-severity noise tends to settle through repair, adaptation, de-escalation and bounded propagation. It does **not** mean that Juego2 preserves the starting town against sustained coherent player action. A player who deliberately keeps supplying strong causes may produce a radically different, damaged, reorganised or improved town.

## PA-12 governance product direction

PA-12 now carries a stronger product requirement:

> **The player should not choose a labelled type of town from a strategy screen; the player should turn the town into that kind of place by governing and acting inside it.**

Research should therefore favour a small reusable vocabulary of institutional levers — access, permission, schedule, capacity, allocation, service, cost/support, obligation, public commitment and related bounded concepts — whose consequences are consumed by normal Living World owners.

Descriptions such as communal/collectivised, deregulated, clientelist, sect-like, highly regulated, authoritarian or chaotic may describe an emergent trajectory. They are not permitted to substitute for explicit rules/opportunities plus independent actor response.

### PA-12 presentation default

The default hypothesis is **not** a separate overhead/strategy game.

Mayoral decisions should first be tested through:

- conversations with an interventor/secretary/relevant official;
- meetings;
- desk/documents;
- compact diegetic or conventional menus where clarity requires them;
- direct third-person inspection and follow-up in the town.

A dedicated top-down management layer may be reconsidered only if a later concrete usability/scale problem demonstrates that accepted governance gameplay cannot be expressed clearly without it.

### Macro↔micro requirement

PA-12 must demonstrate both directions:

```text
macro decision
 -> rule/resource/opportunity changes
 -> NPCs react through their own systems
 -> player encounters consequences in third person
 -> player can locally help/resist/mediate/exploit
 -> later town state reflects both macro and micro causes
```

A mayoral layer that changes only dashboard numbers fails even if the numbers are sophisticated.

## PA-13 anti-chaos clarification

PA-13 must prove two things at once:

1. ordinary/clumsy play does not naturally spiral into systemic anarchy;
2. sustained intentional destabilisation can overcome normal recovery mechanisms and produce materially different state.

If (1) fails, the simulation is unstable.

If (2) fails, the simulation is over-damped and player freedom is cosmetic.

PA-13 budgets may bound rate, scope, persistence and computational cost. They may not silently decide which town outcomes are morally/designer-approved or force restoration of the default equilibrium.

## PA-14 final integration additions

PA-14 must now include integrated proofs for:

- **govern → walk out → see it:** a municipal decision visibly changes normal third-person town play;
- **macro↔micro composition:** later local player action can alter the implementation/aftermath without bypassing subsystem ownership;
- **divergent governance trajectories:** equivalent starting towns can become materially different under sustained different policy/action histories without global ideology-mode flags;
- **clumsy vs deliberate chaos:** ordinary mistakes remain recoverable while repeated intentional destabilisation can materially transform/degrade the town;
- **constructive transformation:** systemic freedom can also build stronger cooperation/services/activities rather than only destroy;
- **repeal is not a time machine:** reversing a rule does not automatically erase memories, relationships, debts, moved resources or other legitimate consequences it already caused.

These proofs join, rather than replace, the earlier Living World requirements for autonomous NPC causality, player causal agency, integrated activities/minigames, legibility and bounded simulation.
