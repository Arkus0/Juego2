# WP-HK-08B representative content-shape probe

CONTENT_SHAPE_PROBE_VERDICT: PASS
APPROVED_PRODUCT_SOURCE: Docs/art/VISUAL_BIBLE.md
EXECUTABLE_PROBE: Hk08BContentShapeProbeTests.RepresentativeMarketMicroBlockStaleEditRecoversByInspectingOnlyChangedResources
UNRESOLVED_IN_SCOPE_FINDINGS: 0

## Source and purpose

The bounded approved product source is `Docs/art/VISUAL_BIBLE.md` v0.1.3. It defines the current Juego2 target as a fictional Potes/Liébana valley market town, explicitly naming market grain, a bar terrace, workshops and modular reuse, with an H2 hero target of a plaza plus one or two streets and a bar/shop interior.

HK08B does not adopt art, transform, asset, Unity, simulation or gameplay schemas. The probe uses only those approved content relationships to make the recovery contract confront a plausible authored shape rather than another abstract `node.*` micro-world.

The question is deliberately narrow: if a client forms one coherent edit spanning a market cluster and bar-front resource, then two accepted same-lineage edits make that plan stale, can HK08B identify exactly what changed, let the client inspect only those changed resources, and preserve the original coherent intent as one ordinary retry transaction?

## Bounded slice

`RepresentativeMarketMicroBlockStaleEditRecoversByInspectingOnlyChangedResources` models four generic canonical objects:

- `market.root` — structural root for the bounded slice;
- `plaza.stalls` — independently addressable market-cluster record;
- `street.bar.front` — independently addressable bar-front record;
- `street.workshop.front` — independently addressable workshop-front record.

These identifiers and `fixture.hk08b.*` type IDs are test vocabulary only. They prove identity/granularity through the existing generic H0 object grammar; they do not promote market, facade, terrace, weather, transform, asset or engine concepts into canonical schemas.

The client intent coherently updates `plaza.stalls` and `street.bar.front` in one request. Before that request is submitted, two accepted commits change `plaza.stalls` and `street.workshop.front`, making the original whole-world revision/hash base stale.

## Probe path

1. Create the bounded four-object market micro-block in one canonical authored world.
2. Accept one same-lineage change to `plaza.stalls`.
3. Accept a second same-lineage change to `street.workshop.front`.
4. Submit the original two-resource client intent against its stale revision/hash base and require stale rejection.
5. Require `same-lineage-replan` with `history-complete` over exactly two transitions.
6. Require `changedResources` to be exactly `world.object:plaza.stalls` plus `world.object:street.workshop.front`; the unchanged `street.bar.front` must not be invented as changed merely because it participates in the stale intent.
7. Require exactly two `currentResources` descriptors and inspect both through their bounded `world.object.get` requests. No summary/query/full-world reconstruction is needed.
8. Rebuild the original coherent two-resource intent against the returned current anchor.
9. Require normal `plan` and `dry-run` success, then one normal `apply` whose plan still contains exactly two resource changes.
10. Require one revision advance for that retry and one additional HK06A provenance entry, leaving three accepted journal entries total.

## Boundary analysis and findings

- **Representability — GREEN.** The approved market/bar/workshop relationships fit the existing generic object + containment grammar. HK08B needs no game-specific schema to express the recovery problem.
- **Identity/granularity — GREEN.** Market cluster, bar front and workshop front can be independently named at the resource granularity HK08B returns. The probe does not require per-field recovery, per-resource CAS or automatic merge.
- **Inspect boundary — GREEN.** Recovery exposes only the two resources changed since the stale base. The client follows the returned bounded object descriptors and does not reload the whole micro-block.
- **Mutation boundary — GREEN.** After reinspection, the client's original market-cluster + bar-front intent remains one coherent normal transaction rather than degenerating into field/resource chatter.
- **Validation boundary — GREEN.** The recovered request still traverses ordinary `plan` and `dry-run` before `apply`; HK08B introduces no alternate acceptance path.
- **Diff/recovery boundary — GREEN.** The delta is lineage-derived and exact: it includes the concurrently changed workshop even though the stale intent did not touch it, and excludes the unchanged bar front even though the intent does touch it.
- **Provenance/replay boundary — GREEN.** Recovery proof is based on the two accepted HK06A lineage transitions, and the successful retry appends exactly one ordinary provenance entry. No replay or rebase semantic is redefined by HK08B.
- **In-scope blocker — none found.** The current structured recovery shape represents this approved product-shaped stale-edit scenario without contract change.
- **Concrete predecessor reopen condition — none observed.** HK04/HK05/HK06/HK08A accepted boundaries remain sufficient for this slice.

## Named future/residual decisions

- Exact transforms, mesh/asset identity, Unity realization, schedules, simulation and gameplay remain post-H0/GATE product work.
- Whether later product authoring wants domain-specific grouping above generic resources is a future ergonomics decision; this probe does not justify changing H0 identity semantics.
- Per-resource locking/CAS, automatic merge and autonomous multi-agent coordination remain explicitly outside HK08B unless later measured product evidence justifies them.
- HK09B owns final resource-envelope limits and may measure this accepted recovery shape, but must not reinterpret its ancestry or delta truth.

## Result

The approved Juego2 market-town slice does not expose a mismatch in HK08B's recovery contract. A realistic coherent stale intent can be recovered with exact lineage-derived changed-resource identities, affected-only inspection and one ordinary two-resource retry transaction, while retaining the accepted whole-world CAS boundary.
