# Urban expansion decision — post-PASS DocSync

Status: DOCSYNC_COMPLETE
Mode: PROCESS_ONLY / DOCUMENTATION_ONLY
Date: 2026-09-28

## Accepted identity

- Product/roadmap PR: `#257`.
- Reviewed candidate: `fdfc9be334a750217f2152c76d17bfcd20ba5ded`.
- Reviewer PASS: `#5333313032`.
- Owner waiver: the sole observed red Candidate Validation was the non-semantic PR-body classifier mismatch `Mode: PROCESS_ONLY / PRODUCT_ROADMAP_AMENDMENT` versus the validator's exact `PROCESS_ONLY` token; no product/architecture/roadmap blocker remained.
- Merge: `6bf2d6e74215be51e73d85f659a2a734752ea34c`.

## Accepted product direction

Juego2 keeps Arkus active as canonical persistent/causal authority while Game Creator 2 accelerates immediate local gameplay and presentation: **GC2 ejecuta. Arkus recuerda y conecta.** The final product direction is a fictional northern-Spain port city built by compact districts, with selective NPC depth, investigation, activities/minigames, pursuit and combat.

The accepted small-town/Liébana material remains truthful history and a retained inland pilot. It is no longer the final scale/geography ceiling. New port topology and keeper art require their explicit `CITY-URBAN-*` / `ART-URBAN-*` owners; accepted inland CITY geometry is not silently relabeled as coast.

## Synced canonical navigation

The following documents now carry accepted status and PR/review/merge identity:

- `Docs/product/URBAN_EXPANSION_DECISION.md`;
- `Docs/architecture/GC2_ARKUS_RUNTIME_SPLIT.md`;
- `Docs/design/NPC_DEPTH_TIERS.md`;
- `Docs/product/SHENMUE_URBAN_SLICE_TARGET.md`;
- `Docs/roadmap/POST_H2F01A_ROADMAP.md`;
- `Docs/roadmap/POST_H2F01A_WP_AUDIT.md`.

Any remaining wording such as “prospective urban expansion” inside individual pending WP addenda records how the amendment entered those contracts; it does not revert the accepted canonical status above. The actual WP status remains `PROPOSED / NOT_STARTED` until that WP is executed and accepted.

## Dependency disposition

PR `#257` did not reopen `WP-H2F-01A`; it finished under its pre-existing contract and subsequently received Reviewer PASS and merged via PR `#256` as `2276dc1c2b429f273b7715fda023539feefdb086`. Its own post-PASS DocSync remains a separate process obligation. Once that 01A DocSync is complete, the immediate next executable Worker is `WP-H2F-02`, consuming the accepted 01A handoff and ART-01 `PREFOUNDATION_INPUT`. No additional **product replanning** gate is required.

The accepted downstream route is:

`H2F-01A → H2F-02/03/GATE → GC2-00 + retained H2/ART/CITY pilot → GC2-02..06 + CITY/ART urban block → melee/pursuit → GC2-SLICE → DISTRICT-01`.

This DocSync starts no implementation WP and grants no downstream gameplay, port-geometry, visual, package-installation or module-adoption PASS.

DOCSYNC_COMPLETE
