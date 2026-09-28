# WP-ART-UI-01 — Juego2 UI language + no-voice dialogue presentation

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT ART / PRESENTATION INDUSTRIALIZATION
Depends on: `WP-GC2-DIALOGUE-00` PASS + `WP-ART-CHAR-02` PASS + accepted H2F visual foundation
Blocks: `WP-ART-03`
Binding decision: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md`

## Claim

Juego2 has a coherent, reusable UI/presentation language suitable for a third-person game with no normal spoken voice acting, and Dialogue 2 can present conversations without reading as an untouched plugin skin.

## Required work

Define the smallest visual system needed for near-term production:

- typography hierarchy and readable subtitle/dialogue sizes;
- speaker/name treatment;
- dialogue panel/subtitle layout;
- choices and selected/disabled/hidden-choice visual states where applicable;
- interaction prompt/focus relationship to the dialogue language;
- continuation/advance indication;
- timing/reveal defaults appropriate to a no-voice game;
- navigation/confirm/cancel sound treatment or placeholders with a clear later audio owner;
- camera-safe margins and composition rules;
- rules for when UI is hidden/reduced during directed presentation;
- minimum accessibility/readability considerations that affect the visual foundation.

Portraits are optional. Do not force a JRPG portrait layout if the third-person acting/camera presentation reads better without it.

## Dialogue presentation benchmark

Use temporary/non-canonical text. No finished story is required.

The benchmark must show, on an accepted keeper-quality environment and factory-produced character where dependencies allow:

1. a normal NPC line;
2. a short multi-line exchange;
3. at least one set of choices;
4. at least one expression/gesture/state change from the admitted animation vocabulary when `ART-ANIM-02` is available, otherwise record final `ART-03` validation as pending;
5. camera + character + text coexistence at third-person scale;
6. no spoken voice dependency.

Typewriter/gibberish-style character sounds may be evaluated if useful, but they are not mandatory and must not substitute for later sound direction.

## Reusable skin/system rule

Start from Dialogue 2 supplied UI/presentation surfaces where they save plumbing, but produce Juego2-owned skin/presentation assets and style tokens. Do not fork or reimplement branching/dialogue runtime merely to change the look.

The UI system should be reusable for future barks, questioning and directed scenes without each conversation inventing a separate canvas/layout.

## PASS-before-work acceptance contract

**Mandatory evidence:** UI style sheet/tokens, approved Dialogue skin/prefab assets, third-person benchmark captures/video, choice/readability inspection, at least one narrow/long-text stress case, and a residual ledger for menus/HUD systems not yet needed.

**FAIL if:** the final benchmark is visibly stock/default GC2 presentation; dialogue obscures critical character acting or scene readability; no-voice pacing is unreadable/tedious in the tested presentation; each conversation needs bespoke UI layout code; or this WP expands into quest/inventory/combat HUD systems without a causal need.

**Allowed residuals:** inventory/menu UI, combat HUD, minimap, journal, final accessibility breadth and story-specific cinematic overlays.

## Non-claims

No finished narrative content, Ink runtime decision, Quest UI, Inventory UI, combat HUD or complete game menu suite.
