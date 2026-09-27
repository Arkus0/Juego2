# H2F — Pre-H2 product/toolchain foundation freeze

Status: **ACCEPTED PLAN / NOT STARTED**  
Class: PRODUCT / TOOLCHAIN + CONTENT INTEGRATION  
Execution policy: `RESEARCH_BATCH` for survey; `PRODUCT_CHECKPOINT` for Unity spikes/integration  
Plan date: 2026-09-27  
Accepted: PR `#250`, candidate `d128aef60c951f3b19daae1d57cddae55c2519ac`, independent PASS review `#5330069733`, merge `9d3bb90ebfae8837155576c13b705fc1c72c2579`

## 1. Phase claim

H2F answers one question before keeper production continues:

> **Have we deliberately surveyed, selected, integrated and frozen the rendering, worldbuilding, character/animation and production-tool baseline that CITY-07 and H2 are expected to build on, so that keeper work does not discover foundational visual/tooling choices piecemeal after content has already been authored around weaker assumptions?**

H2F is not a new canonical/platform foundation. It does not reopen H0 or H1, does not make Unity semantic authority, and does not replace ART or CITY ownership. It is a bounded pre-production freeze over the Unity/product toolchain and admitted external content sources used to realize the game.

The intended execution order is:

```text
H1-GATE accepted
      +
ART-01 KEEPER_READY
      ↓
H2F-00 capability survey + candidate register
      ↓
H2F-01 decision spikes + stack selection
      ↓
H2F-02 exact adoption + URP/toolchain bootstrap
      ↓
H2F-03 integrated compatibility/authoring benchmark
      ↓
H2F-GATE foundation freeze
      ↓
CITY-07 keeper game-space realization
      ↓
H2-01 / H2-02 / H2-03 / ART-02 / H2-GATE
```

`H2F-00` may be researched remotely before ART-01 closes because it does not mutate the project or approve dependencies. No adoption, migration or keeper claim may use that overlap to pretend ART-01 has already passed.

## 2. Why H2F must precede CITY-07

CITY-07 owns the first retained keeper game-space and may make material choices about roads, terrain, local vertical realization, walls/edges, vegetation, collision/navigation and Unity-native geometry. If the project selected URP, spline/road tooling, terrain/vegetation rules, water/shoreline treatment, character retargeting or other retained implementation mechanisms only after CITY-07, the first keeper slice could be authored around assumptions that immediately require rework.

H2F therefore consumes ART-01 and blocks CITY-07. It does not ask ART-01 to redo its production-kit claim, and it does not let H2F redesign CITY semantics.

## 3. Exhaustive-by-capability discovery rule

H2F interprets “search everything useful” as **exhaustive capability coverage**, not indiscriminate installation or enumeration of every Asset Store listing.

For every capability category that can materially affect Juego2 keeper construction, visual quality, authoring cost or later replacement cost, H2F-00 must inspect:

1. the relevant Unity/URP/native capability where one exists;
2. credible free/open-source/content-library alternatives;
3. credible commercial alternatives when they could materially change the decision;
4. already-owned/adopted Quaternius Source capabilities and source projects before adding redundant tools;
5. whether the correct answer is deliberately `NO_ADOPT`.

The survey closes only when every named category has an explicit disposition and rationale. It is not enough to document only the packages already known to the team.

### Mandatory capability categories

1. render pipeline, shader/material migration and Shader Graph;
2. lighting, GI/probes, shadows and baked/realtime strategy;
3. post-processing, tone/color, SSAO, decals and wetness/weathering presentation;
4. sky, fog, atmosphere and weather;
5. water, river and shoreline treatment;
6. terrain/elevation/sculpting;
7. roads, paths, splines, kerbs, retaining edges, walls, fences, rails and repeated linear features;
8. vegetation, grass, trees, scatter, ivy/moss and exclusion/authoring controls;
9. architecture, modular assembly, windows, fake interiors and interior/exterior transitions;
10. source-asset/DCC adaptation and derivative-asset workflow;
11. player controller, camera and input;
12. character bodies, clothing/wardrobe and customization scope;
13. animation libraries, Humanoid retargeting, root motion, IK/rigging and interaction animation;
14. facial presentation and lip-sync;
15. navigation/pathfinding and bounded local behaviour execution;
16. dialogue/narrative/quest authoring;
17. cinematics/sequencing;
18. audio, spatial audio and environmental sound;
19. VFX/particles;
20. UI and localization;
21. asset loading, scenes, streaming and Addressables-like concerns;
22. performance: LOD, culling, instancing, batching and profiling;
23. tests, performance benchmarks, capture/recording and regression evidence;
24. editor/level-design productivity tooling;
25. Astra/AI-native authoring integration and wrappers/presets;
26. save/persistence consumers that could constrain H2 assets/tooling;
27. networking/multiplayer or other consciously out-of-scope runtime systems, recorded explicitly as `NO_ADOPT`/`DEFER` rather than silently ignored.

The Worker may add categories discovered during research. Removing a category requires an explicit reason.

## 4. Candidate disposition vocabulary

Every material candidate receives one final disposition:

- `ADOPT_NOW` — part of the frozen H2/CITY keeper baseline;
- `AVAILABLE_ASSET` — admitted content/source available to the production pipeline but not a mandatory runtime/tool dependency;
- `SPIKE_REQUIRED` — plausible winner whose fit cannot be decided honestly from documentation alone;
- `DEFER` — useful later, but adoption now would add more integration cost than value;
- `REDUNDANT` — its useful capability is already covered by the selected stack/source;
- `REJECT` — incompatible, unnecessary, legally/operationally unsuitable or contrary to project authority/visual goals;
- `NO_ADOPT` — the capability category intentionally requires no extra dependency.

No `SPIKE_REQUIRED` may remain at H2F-GATE for a capability required by CITY-07 or the representative H2 slice.

## 5. Required candidate register fields

At minimum record for every material candidate:

- identity/provider/source;
- capability category;
- native/free/open-source/paid/content classification;
- current observed version/commit and exact adopted version when applicable;
- exact license/EULA at adoption time and required notices;
- Unity 6.3 / URP evidence or compatibility status;
- runtime vs editor/build-only vs source-content role;
- source bytes/reproducibility implications;
- aesthetic fit with the accepted Juego2 visual bible;
- AI/Astra automation fit;
- manual account/acquisition/import step, if any;
- canonical-authority/mutation-boundary risk;
- integration and replacement risk;
- overlap/redundancy with Quaternius Source or another selected tool;
- spike required and spike result, if applicable;
- final disposition and rationale.

Research observations are not adoption evidence. Exact-version/license claims are reverified at `ADOPT_NOW`/`AVAILABLE_ASSET` admission under `Docs/engineering/DEPENDENCY_IP_POLICY.md`.

## 6. Authority and replacement rules

H2F consumes the accepted H0/H1 boundary:

- Arkus owns canonical world state and authoring semantics;
- CITY owns spatial/topological/game-space constraints;
- ART owns visual direction and production-content truth;
- Unity packages/assets implement or assist downstream realization;
- no third-party package may create a hidden canonical mutation path or become the only definition of a Juego2 semantic contract.

Examples:

- Arkus/CITY may author a typed road/path intent; a selected spline/road tool may realize the geometry.
- Arkus/H3 may decide where/why an NPC moves; AI Navigation or another admitted executor may realize the local path.
- ART may define an accepted character presentation; Humanoid retargeting, Animation Rigging or an admitted animation library may execute the presentation.

The selected package API is not itself the stable Juego2 semantic contract when a small Arkus/Unity adapter or preset boundary can keep it replaceable.

## 7. Workpacks

### `WP-H2F-00 — Capability ecosystem survey + candidate register`

Remote research batch. Close every mandatory capability category, create the longlist and gap map, record native/free/commercial alternatives that could change the decision, and emit the bounded spike queue. No project mutation or dependency approval occurs here.

### `WP-H2F-01 — Decision spikes + final stack selection`

Run only the Unity/source-content spikes necessary to distinguish serious candidates whose choice is expensive to reverse. Produce the final disposition matrix and one recommended baseline with explicit rejected/deferred alternatives.

### `WP-H2F-02 — Exact adoption + URP/toolchain bootstrap`

Apply the selected baseline to the real Unity project: exact package/adoption records, render-pipeline migration, project/preset/import conventions, external-source provisioning, character retarget rules and selected worldbuilding adapters/presets. For every material retained/generated state introduced by the selected stack, also freeze its H1 lifecycle class/host and its materialize/observe/reconcile/rematerialize/clean-rebuild expectation. This is the first H2F WP allowed to make the chosen dependencies project truth.

### `WP-H2F-03 — Integrated compatibility + AI-authoring benchmark`

Prove the selected stack works together rather than only in isolated package demos. Use a bounded **non-keeper integration fixture** so this work cannot bypass CITY-07. Exercise the adopted rendering/worldbuilding/player/navigation/character-animation path, execute one composed accepted H1 materialize → observe → reconcile → rematerialize → clean-rebuild cycle with the full stack present, and run one public/AI-assisted authoring operation through the approved boundary. Profiling is diagnostic except for the explicitly enumerated foundation pathologies in the WP.

### `WP-H2F-GATE — Foundation freeze`

Freeze the exact admitted baseline, demonstrate the selected foundation state is preserved or deterministically reconstructed through the accepted H1 lifecycle, demonstrate clean/reproducible project restoration with documented lawful/manual inputs, close dependency/IP records, prove the integrated fixture, retain rejected/deferred decisions and authorize CITY-07/H2 to build keeper content on the frozen baseline.

## 8. Freeze semantics after H2F-GATE

The freeze is strong enough to prevent accidental dependency drift but not a ban on all future improvement.

After H2F-GATE:

- CITY-07 and H2 use the admitted baseline by default;
- a new material dependency or replacement that changes keeper construction, rendering, character/animation handling, navigation, asset provenance or public AI authoring requires an explicit H2F amendment/adoption record before keeper content relies on it;
- ordinary assets that fit an already-admitted content/source class may enter through the existing ART/DEPENDENCY-IP process without pretending they are a new toolchain architecture;
- profiling may justify a later optimization dependency, but it is evidence-driven and does not retroactively make the H2F survey incomplete;
- future H3+ systems may adopt narrative/audio/behaviour tools that H2F intentionally deferred, using their causal owner and exact-version review.

## 9. H2F exit quality bar

H2F-GATE may PASS only when:

1. every mandatory capability category has a closed disposition;
2. every capability required by CITY-07/H2 has either an admitted solution or an explicit `NO_ADOPT` design;
3. no required capability remains `SPIKE_REQUIRED`;
4. every adopted material dependency/source satisfies the exact-version provenance/license/adoption policy;
5. the project is on the selected render pipeline and the accepted real-source assets render without silent unsupported-material fallback;
6. the selected worldbuilding tools coexist on one integrated fixture;
7. the selected player/camera/input/navigation path works on that fixture;
8. at least one representative Quaternius humanoid proves the accepted animation/retarget/rigging baseline;
9. every material selected foundation-state family has an explicit H1 lifecycle class/host and materialize/observe/reconcile/rematerialize/clean-rebuild expectation;
10. the full selected stack passes the composed accepted H1 lifecycle proof without losing required state, generating false unsupported drift or requiring undocumented hand repair;
11. a fresh author/agent can use the prescribed public/preset/adapter surfaces without making plugin-private state canonical;
12. a clean restoration/import path reproduces the accepted baseline given the documented lawful/manual asset inputs;
13. a bounded profiler snapshot is captured under a declared comparison context and none of the explicit H2F-03 foundation pathologies reproduces; ordinary numeric profiler findings are diagnostic residuals, not invented H2F thresholds;
14. the final adoption, defer, reject and replacement decisions are retained so later Workers do not repeatedly rediscover or silently overturn them.

A collection of successfully installed packages is not H2F success. The claim is a **coherent, reviewed, reproducible production baseline**.

## 10. Explicit non-claims

H2F does not prove:

- final CITY-07 keeper geometry;
- H2 visual closure;
- Living World NPC semantics;
- final dialogue/quest architecture;
- final audio pipeline;
- town-wide streaming/performance;
- combat;
- multiplayer;
- that deferred tools can never be adopted later.

Those remain owned by their causal product phases.