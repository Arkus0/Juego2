# WP-GC2-DIALOGUE-00 — Dialogue 2 adoption + presentation seam

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT TOOLING / LICENSED MODULE ADOPTION
Depends on: `WP-H2F-GATE` PASS + owner-provided lawful Dialogue 2 package
Blocks: `WP-ART-UI-01`
Binding decision: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md`

## Claim

The exact owner-supplied Dialogue 2 version can be lawfully and reproducibly admitted into Juego2 as a **presentation/directing surface** without changing Arkus authority, creating a second narrative source of truth or prematurely implementing the investigation/dialogue gameplay WP.

## Scope

This is deliberately narrower than `WP-GC2-03`.

Admit and exercise only the surfaces needed to judge visual presentation, such as:

- actor/speaker presentation;
- dialogue/subtitle UI skins;
- choices presentation;
- text reveal/typewriter or equivalent no-voice pacing;
- expression/gesture/state callbacks;
- bounded camera/presentation hooks where available;
- one tiny dummy conversation used only as a visual fixture.

Do **not** build the real investigation conversation graph, story quest flow, Ink integration policy or persistent narrative state here.

## Adoption work

- identify exact version/hash/package identity where practical;
- record license/provisioning and keep restricted vendor bytes out of the repository when required;
- record package/module dependencies and compatibility with the accepted H2F/GC2 Core version;
- classify retained project assets/settings versus generated/transient data;
- define the Juego2 adapter boundary so plugin-private IDs/Variables do not become canonical story/world identity;
- define lawful clean-workstation restoration steps;
- prove compile/import with Core and the frozen H2F baseline;
- record uninstall/replacement boundary.

If the owner-supplied version materially conflicts with the frozen H2F baseline, this WP fails/blocks rather than silently upgrading the foundation or Core.

## Authority boundary

- Arkus/Juego2 owns persistent world truth and consequential state.
- Dialogue 2 owns local dialogue presentation/execution inside its admitted scope.
- Dialogue assets/IDs may locate presentation content but do not become canonical NPC identity or persistent world facts.
- Whether Ink becomes an authored narrative source is a later explicit decision; this WP neither rejects nor canonizes Ink.

## PASS-before-work acceptance contract

**Mandatory evidence:** exact module/provisioning record, compile/import proof, retained/generated lifecycle classification, authority boundary, one no-voice dummy conversation showing text + choice + one expression/gesture callback, and a clean removal/replacement note.

**FAIL if:** package version floats; vendor bytes are committed contrary to license; Dialogue state silently becomes Arkus authority; the WP expands into real story/investigation content; a required foundation package is silently replaced; or the presentation cannot coexist with the accepted GC2 Core/H2F baseline.

## Non-claims

No `GC2-03` investigation-dialogue PASS, no final UI skin, no final narrative pipeline, no Ink decision and no voice acting.
