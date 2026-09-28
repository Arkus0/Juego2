# Visual production factory amendment

Status: **PROPOSED / NOT ACCEPTED**

## Owner decision

Before Juego2 expands into the deeper GC2 gameplay chain, H2 must stop being only a small visual benchmark and become a **production-industrialization phase**.

The goal is not merely to produce one attractive keeper scene. The goal is to leave Juego2 able to produce additional streets, interiors, inhabitants, animation-rich interactions and presentation at the approved visual quality without returning to dressed-greybox improvisation or bespoke per-scene plumbing.

The owner priority is explicit:

> **Lock the look and industrialize visual/content production before adding deeper gameplay systems.**

`WP-GC2-02` and later gameplay WPs remain blocked until the expanded `WP-H2-GATE` passes. `WP-GC2-00`, the H2F-admitted GC2 Core shell and the bounded `WP-GC2-DIALOGUE-00` strategy checkpoint are permitted before that Gate only because they are inputs to visual/presentation production rather than new systemic gameplay. The checkpoint does **not** require buying Dialogue 2: Core/local presentation remains a valid H2 path.

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
- `WP-GC2-DIALOGUE-00` first resolves availability/materiality and may legitimately finish `ADOPTED`, `NOT_MATERIAL`, `REJECTED` or `DEFERRED_NOT_ACQUIRED`;
- Dialogue 2 may be adopted before deep dialogue gameplay only when the owner has a lawful package and its material work saving or quality gain justifies acquisition; before `ADOPTED`, the exact version must pass the binding `DEPENDENCY_IP_POLICY` record and composed H1 lifecycle proof;
- if Dialogue 2 is not adopted, `WP-ART-UI-01` uses the admitted Core/local presentation surface and must prove the same typography, speaker treatment, choices, prompts, no-voice pacing, acting/camera readability, Juego2 visual language and presentation reuse;
- `WP-GC2-03` later consumes the accepted disposition and may not independently buy, adopt or upgrade Dialogue 2;
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
- `WP-GC2-DIALOGUE-00` — dialogue-presentation strategy checkpoint and single optional Dialogue 2 adoption authority; no investigation-system claim and no purchase requirement.
- `WP-ART-UI-01` — Juego2 UI/dialogue visual-language benchmark for a no-voice game, valid on either the adopted Dialogue 2 seam or Core/local path.
- `WP-ART-03` — integrated visual-production lock on the actual port-town keeper block.

Existing `ART-01`, `H2-01`, `H2-02`, `H2-03` and `ART-02` remain useful, but they become inputs to the stronger final lock rather than the complete industrialization claim by themselves.

## Revised H2 ordering

The intended causal DAG after `WP-H2F-GATE` is shown with explicit multi-predecessor joins; `A + B -> C` means all named predecessors are required for the downstream PASS.

```text
H2F-GATE -> ART-01
H2F-GATE -> ART-CHAR-01 -> ART-CHAR-02
H2F-GATE -> ART-ANIM-01
ART-ANIM-01 + ART-CHAR-02 -> ART-ANIM-02
H2F-GATE -> GC2-DIALOGUE-00
GC2-DIALOGUE-00 + ART-CHAR-02 + H2F-GATE -> ART-UI-01

ART-01 + CITY-URBAN-00 -> ART-ENV-02
ART-01 + ART-ENV-02 + CITY-URBAN-00 -> ART-URBAN-01
CITY-URBAN-00 + ART-URBAN-01 -> CITY-URBAN-01

H2F-GATE -> GC2-00
H2F-GATE + ART-01 + CITY-07 -> H2-01
H2F-GATE + CITY-07 + GC2-00 -> H2-02
H2-02 + ART-CHAR-02 + ART-ANIM-02 -> H2-03
CITY-07 + H2-02 + H2-03 + ART-01 -> ART-02

ART-02 + ART-ENV-02 + ART-CHAR-02 + ART-ANIM-02 + ART-UI-01
  + ART-URBAN-01 + CITY-URBAN-01 -> ART-03

H2F-GATE + H2-01 + H2-03 + ART-02 + ART-03 + CITY-URBAN-01 -> H2-GATE
H2-GATE -> GC2-02+
```

`GC2-DIALOGUE-00` in this graph means **resolve the presentation strategy/disposition**. It does not mean “install Dialogue 2”. A non-adopted PASS (`NOT_MATERIAL`, `REJECTED` or `DEFERRED_NOT_ACQUIRED`) legally feeds `ART-UI-01` through Core/local and therefore does not create an economic dependency on the plugin.

The graph is intentionally not fully serial. Environment, character, animation, final-setting planning and Dialogue/UI presentation can overlap once their exact predecessors are ready. Individual WP predecessor sets remain binding; this causal graph must not present a weaker shortcut than those contracts.

## H2-GATE meaning after this amendment

H2-GATE may PASS only when all of the following are true:

- the approved Juego2 look is stable at third-person scale;
- the actual port-town first keeper block expresses that look, not merely the historical inland pilot;
- required environment roles are `KEEPER_READY`, not proxy geometry hidden by dressing;
- ordinary character variants can be produced repeatably from the accepted factory;
- a useful animation vocabulary is already available and extendable;
- no-voice dialogue/UI presentation has an approved visual pattern, regardless of whether its accepted runtime surface is Dialogue 2 or Core/local;
- a fresh author/agent can build a new bounded environment composition and a small group of presentable inhabitants from the accepted production surfaces without adding a new bespoke framework;
- reuse is evidenced by separately observable first-build vs repeated-build work, with stated counting assumptions and a real reduction in repeated setup/authoring operations;
- residuals are future breadth/polish/content, not an undecided fundamental art language or missing production pipeline.

## Non-goals

This amendment does **not** require before H2-GATE:

- purchasing or adopting Dialogue 2;
- finished story or final dialogue content;
- persistent schedules, memories, social graphs or autonomous Living World systems;
- Inventory 2, Behavior 2, Perception, Melee or other separately licensed gameplay modules;
- all 80–120 planned inhabitants;
- final town-wide asset breadth;
- final combat/chase/minigame content;
- bespoke hero-character polish for every important NPC.

It also does not turn H2 into an endless asset-collection phase. Each production lane must prove a reusable factory and leave an explicit residual/gap ledger. Additional content after the factory exists is produced by need.

## Purchase / module timing

The project does not buy paid assets/plugins merely because a future WP names them. Acquisition remains need-driven: buy only when the real task demonstrates material saved work or an important quality gain.

Dialogue 2 is the only separately licensed dialogue module explicitly eligible for **optional** pre-H2-GATE adoption under this amendment because its bounded use could contribute directly to the presentation/UI lock. Eligibility is not a purchase requirement.

`WP-GC2-DIALOGUE-00` owns that decision exclusively. `NOT_MATERIAL`, `REJECTED` and `DEFERRED_NOT_ACQUIRED` are valid outcomes that retain no Dialogue 2 dependency and allow `WP-ART-UI-01`, `WP-ART-03` and `WP-H2-GATE` to continue via Core/local. Only before `ADOPTED` may become retained production truth must the checkpoint run the complete exact-version `Docs/engineering/DEPENDENCY_IP_POLICY.md` record, classify all material state families against the accepted H1 host/materialize/observe/reconcile/rematerialize/clean-rebuild lifecycle, and prove the bounded composed lifecycle.

If later work demonstrates a material reason to acquire Dialogue 2 after a non-adopted disposition, the owner may provide the package and explicitly reopen `WP-GC2-DIALOGUE-00`. The reopened adoption must then execute exact version, license/EULA, provisioning, compatibility, authority boundary, H1 lifecycle classification, composed lifecycle witness and replacement/uninstall path before any downstream WP may consume Dialogue 2. `WP-ART-UI-01` and `WP-GC2-03` remain consumers only.

All later modules remain owned by their causal GC2 gameplay WPs unless separately amended.
