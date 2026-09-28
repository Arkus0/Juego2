# Visual production factory amendment

Status: **PROPOSED / NOT ACCEPTED**

## Owner decision

Before Juego2 expands into the deeper GC2 gameplay chain, H2 must stop being only a small visual benchmark and become a **production-industrialization phase**.

The goal is not merely to produce one attractive keeper scene. The goal is to leave Juego2 able to produce additional streets, interiors, inhabitants, animation-rich interactions and presentation at the approved visual quality without returning to dressed-greybox improvisation or bespoke per-scene plumbing.

The owner priority is explicit:

> **Lock the look and industrialize visual/content production before adding deeper gameplay systems.**

`WP-GC2-02` and later gameplay WPs remain blocked until the expanded `WP-H2-GATE` passes. `WP-GC2-00`, the H2F-admitted GC2 Core shell and a bounded Dialogue 2 presentation/adoption proof are permitted before that Gate only because they are inputs to visual/presentation production rather than new systemic gameplay.

## What “industrialized” means

H2 must establish and prove reusable production lanes for all of the following.

### Environment / scenario production

- reviewed modular architecture, street, threshold, interior, prop, vegetation and material vocabulary;
- composition metadata, dimensions, connection rules and provenance;
- reusable street/building/interior profiles and lawful generation/authoring helpers where they save repeated work;
- no scene-specific code required merely to assemble ordinary new streets;
- a fresh author/agent can produce a new bounded composition from a brief using the accepted kit and public rules;
- repeated production demonstrably reuses the established pipeline rather than rebuilding it from scratch.

### Character / wardrobe production

- a repeatable recipe from accepted source character to clothed, visually coherent, Humanoid/GC2-ready presentation;
- reusable wardrobe families, palettes and accessories suitable for a northern-Spain port town;
- provenance and source/derived identity remain traceable;
- ordinary population characters can be produced as variants from the factory rather than bespoke one-offs;
- important characters may still receive bespoke work later.

### Animation production

- a broad reusable library spanning locomotion, conversation/acting, ambient/social actions, work/activity, object handling and reactions;
- explicit retarget/import conventions and GC2 Gesture/State or equivalent mapping where applicable;
- a documented intake path for adding future animations without redesigning the runtime architecture;
- breadth is judged by useful functional coverage, not by raw clip count alone.

### UI / dialogue presentation

- a coherent Juego2 UI language including typography, dialogue/subtitle treatment, choices, prompts and interaction readability;
- the game assumes **no spoken voice acting** as the normal case;
- Dialogue 2 may be adopted before deep dialogue gameplay as a presentation/directing tool if the exact owner-supplied version passes the normal dependency/adoption boundary;
- one presentation benchmark must prove text, speaker identity, choices, acting/gesture timing and camera coexist cleanly without requiring finished narrative content.

### Final setting transfer

The old inland Puente/Casco/Bar keeper pilot remains useful production evidence, but it is not sufficient to lock the final product look. Before H2-GATE, the visual production system must also be exercised on the **actual first keeper block of the fictional large port town / villa portuaria**.

The final H2 visual approval therefore cannot be inferred from the inland pilot alone.

## New visual-production workpacks

This amendment introduces the following bounded WPs. They may run in parallel where their dependencies allow.

- `WP-ART-CHAR-01` — character/wardrobe source grammar and production specification.
- `WP-ART-CHAR-02` — repeatable character factory + representative population batch.
- `WP-ART-ANIM-01` — animation source audit, admission, retarget and coverage plan.
- `WP-ART-ANIM-02` — reusable animation vocabulary + runtime mapping proof.
- `WP-ART-ENV-02` — scenario-production factory and fresh-author repeatability proof.
- `WP-GC2-DIALOGUE-00` — bounded Dialogue 2 adoption/presentation seam; no investigation system claim.
- `WP-ART-UI-01` — Juego2 UI/dialogue visual-language benchmark for a no-voice game.
- `WP-ART-03` — integrated visual-production lock on the actual port-town keeper block.

Existing `ART-01`, `H2-01`, `H2-02`, `H2-03` and `ART-02` remain useful, but they become inputs to the stronger final lock rather than the complete industrialization claim by themselves.

## Revised H2 ordering

The intended causal graph after `WP-H2F-GATE` is:

```text
H2F-GATE
  |
  +--> ART-01 effective closure ---------> ART-ENV-02 -----------+
  |                                                              |
  +--> CITY-URBAN-00 --------------------> ART-URBAN-01 ----------+--> CITY-URBAN-01 --+
  |                                                              |                     |
  +--> ART-CHAR-01 --> ART-CHAR-02 -------------------------------+                     |
  |                                                              |                     |
  +--> ART-ANIM-01 -----> ART-ANIM-02 ----------------------------+                     |
  |                           ^                                  |                     |
  |                           +---- ART-CHAR-02                    |                     |
  |                                                                                    |
  +--> GC2-00 --> CITY-07/H2-01/H2-02 --> H2-03 --> ART-02 ----------------------------+
  |                                                                                    |
  +--> GC2-DIALOGUE-00 --> ART-UI-01 --------------------------------------------------+
                                                                                       |
                                                                                       v
                                                                                    ART-03
                                                                                       |
                                                                                       v
                                                                                    H2-GATE
                                                                                       |
                                                                                       v
                                                                                   GC2-02+
```

The graph is intentionally not fully serial. Environment, character, animation, final-setting planning and Dialogue/UI presentation can overlap once their exact predecessors are ready.

## H2-GATE meaning after this amendment

H2-GATE may PASS only when all of the following are true:

- the approved Juego2 look is stable at third-person scale;
- the actual port-town first keeper block expresses that look, not merely the historical inland pilot;
- required environment roles are `KEEPER_READY`, not proxy geometry hidden by dressing;
- ordinary character variants can be produced repeatably from the accepted factory;
- a useful animation vocabulary is already available and extendable;
- no-voice dialogue/UI presentation has an approved visual pattern;
- a fresh author/agent can build a new bounded environment composition and a small group of presentable inhabitants from the accepted production surfaces without adding a new bespoke framework;
- reuse is evidenced by separately observable first-build vs repeated-build work, with stated counting assumptions and a real reduction in repeated setup/authoring operations;
- residuals are future breadth/polish/content, not an undecided fundamental art language or missing production pipeline.

## Non-goals

This amendment does **not** require before H2-GATE:

- finished story or final dialogue content;
- persistent schedules, memories, social graphs or autonomous Living World systems;
- Inventory 2, Behavior 2, Perception, Melee or other separately licensed gameplay modules;
- all 80–120 planned inhabitants;
- final town-wide asset breadth;
- final combat/chase/minigame content;
- bespoke hero-character polish for every important NPC.

It also does not turn H2 into an endless asset-collection phase. Each production lane must prove a reusable factory and leave an explicit residual/gap ledger. Additional content after the factory exists is produced by need.

## Purchase / module timing

Acquiring a module early does not authorize premature gameplay integration. Dialogue 2 is the only separately licensed module explicitly eligible for pre-H2-GATE adoption under this amendment because its bounded use contributes directly to the presentation/UI lock. All later modules remain owned by their causal GC2 gameplay WPs unless separately amended.
