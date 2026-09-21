# H1 plan amendment — Quaternius Source timing and downstream art ownership

Status: PROCESS_ONLY planning correction candidate
Date: 2026-09-21
Baseline: `main` after PR #92 merge (`dc5da1b722a90b5d6e878615d458a22006d87dd9`)

## Problem found

The accepted H1 plan delayed real third-party game art until `WP-H1-11`, while `WP-H1-04`, `WP-H1-05` and `WP-H1-06` already claimed catalogue, scene and prefab behavior using game-shaped content. That timing implied disposable substitute production art for the exact seams that will later consume the intended Quaternius assets.

The product decision is different and simpler:

1. acquire an exact human-approved **Quaternius Source** distribution before the first H1 WP that actually needs game-representative art;
2. use the maximum practical amount of that source directly to get the first playable/demo working;
3. do not block bridge/gameplay progress on final Cantabrian art;
4. later create separately identified Juego2-derived assets only where concrete product needs require clothing, architectural/material adaptation, missing props/meshes, variants or missing animations;
5. preserve upstream/derivative provenance and keep all art below Arkus semantic authority.

## Causal correction

`WP-H1-04` is the first point that requires effective game-representative Unity assets. It therefore becomes the **first Quaternius Source adoption/import point** under the already-binding `DEPENDENCY_IP_POLICY.md`.

No new implementation WP is added. Source qualification is a dependency/adoption obligation at the point of first use, not a new bridge semantic authority.

`WP-H1-05` and `WP-H1-06` consume that accepted source for positive game-shaped scene/prefab proof. Harness-only synthetic fixtures remain allowed for bridge internals and causal negative controls.

`WP-H1-10` proves clean reconstruction while the accepted source content remains an explicit external bridge input.

`WP-H1-11` no longer owns first adoption. It expands the already-adopted source baseline into a broader representative hierarchy/material/pivot/rig/animation conformance slice before H1-GATE.

`WP-H1-GATE` closes composed bridge readiness over that same accepted source baseline and does not become an art-production gate.

## Upstream versus derivative assets

Three identities must not be conflated:

- **Quaternius Source upstream** — purchased/adopted external source distribution; exact provenance/terms are recorded before first use. It is editable raw material for later art production, but bridge proof does not silently overwrite the pinned upstream baseline.
- **Juego2-derived art** — later authored assets based on or stylistically coherent with the source: Cantabrian architecture/material variants, clothing/outfits, missing objects, new variants and missing/retargeted animations. These receive their own provenance/catalogue identities.
- **H1 bridge-managed derivatives** — disposable generated prefab/projection artifacts used to prove bridge semantics. These are not the same thing as artist-authored Juego2 derivatives.

This separation lets the product modify Quaternius later without making source bytes immutable forever and without turning H1 into an art DCC pipeline.

## First playable / demo policy

After H1-GATE, H2 should build the first playable using **maximum practical direct Quaternius Source reuse**. Art is adapted incrementally only when the playable exposes a real need.

A player-facing character is not considered demo-ready without appropriate clothing merely because a raw humanoid source exists. Likewise, available Quaternius animations are reused first; new/adapted/retargeted clips are produced only when a concrete gameplay/demo action is uncovered.

This policy is now recorded in `Docs/art/VISUAL_BIBLE.md` rather than hidden in H1 implementation detail.

## Ownership check

- H1-04 owns catalogue identity/completeness and performs first-use dependency qualification; it does **not** own broad artistic transformation.
- H1-05..10 own their existing bridge claims and consume the accepted art baseline.
- H1-11 owns broad representative bridge conformance, not first adoption or finished art.
- H1-GATE owns closure/public-client readiness only.
- future H2/ART/CITY planning owns when/how the demo gets Cantabrianized and which missing assets/animations are created.
- H0 canonical state/semantics remain unchanged; Quaternius/Juego2 art never becomes canonical semantic authority.

## Negative planning checks

The amended plan must FAIL review if any of these are true:

- a binding H1 surface still says real third-party game art first appears at H1-11;
- H1-04+ positive game-art probes are required to fabricate substitute production assets despite an accepted Quaternius source shape being available;
- H1 is made responsible for completing wardrobe/Cantabrian art before bridge readiness;
- Quaternius upstream and Juego2-derived art are treated as the same asset identity/provenance;
- bridge-managed prefab derivatives are confused with artist-authored Juego2 derivatives;
- H2 is forced to wait for final art before a first playable can exist;
- the plan permits naked/unpresentable player-facing characters to count as demo-ready merely because the raw source model is technically valid;
- source/license terms are assumed without exact adoption-time verification under `DEPENDENCY_IP_POLICY.md`.

## Scope boundary

This amendment changes planning/contracts/documentation only. It purchases nothing, imports no binary asset, selects no exact Quaternius distribution, creates no Unity project, modifies no H1 implementation and does not authorize H2/CITY keeper work early.
