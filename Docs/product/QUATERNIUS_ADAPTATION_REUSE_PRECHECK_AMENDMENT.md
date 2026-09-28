# Quaternius adaptation reuse-precheck amendment

Status: **PROPOSED / PROCESS_ONLY / DOCS_ONLY**  
Date: 2026-09-28  
Class: PRODUCT-ART PROCESS AMENDMENT  
Research input: `Docs/discovery/QUATERNIUS_ADAPTATION_ECOSYSTEM_AUDIT.md`  
Tooling catalog: `Docs/discovery/QUATERNIUS_TOOLING_CATALOG.md`  
WP impact map: `Docs/discovery/QUATERNIUS_WP_IMPACT_MAP.md`

## Owner decision

Future Juego2 art-industrialization work must not begin by inventing a new Quaternius adaptation/import/generation pipeline when a Quaternius-native capability, public reference pipeline or existing authoring helper may already cover a material part of the problem.

For the directly affected ART workpacks, the default implementation order becomes:

```text
accepted native/current path
-> audited existing tool / technique / Quaternius-native capability
-> bounded compatibility + value spike when material
-> adapt/wrap a lawful existing solution where cheaper and safer
-> custom tooling only for the remaining proven gap
```

This is a **reuse precheck**, not a dependency-adoption mandate. A Workpack may reject every audited candidate when they are incompatible, fragile, legally unsuitable, lower quality or more expensive than the project-owned alternative. The causal decision must simply be observable rather than rediscovered from scratch.

## Affected workpacks

This amendment changes the process interpretation of:

- `WP-ART-01`;
- `WP-ART-CHAR-01`;
- `WP-ART-ANIM-01`;
- `WP-ART-ENV-02`;
- `WP-ART-URBAN-01`;
- `WP-ART-03`.

It does not replace their existing claims, dependencies, acceptance evidence or visual-quality oracles.

### `WP-ART-01`

ART-01 remains the bounded production-kit/recovery proof. It must consult the audit before declaring a tooling-driven or source-adaptation gap, especially when recovery via `ADAPT`, `DONOR_COMPONENTS` or `CREATE_DERIVED` could be accelerated by an already-audited technique.

ART-01 must **not** expand into the final character factory, environment procedural framework or universal Blender/Unity exporter. Its existing H2F sequencing and `KEEPER_READY / PROXY_VISUAL / COVERAGE_BLOCKED` truthfulness remain binding.

### `WP-ART-CHAR-01`

Before implementing a new wardrobe/character pipeline, evaluate the admitted Quaternius-native modular/rig capabilities and relevant audited reference pipelines. Record which techniques are `USE`, `ADAPT`, `REFERENCE`, `SPIKE`, `WATCH` or `REJECT`, and why.

The workpack still owns the Juego2 character/wardrobe grammar, clipping/fit/rig quality, provenance and production recipe. An external pipeline can implement a mechanism; it cannot define the product's character identity or acceptance semantics.

### `WP-ART-ANIM-01`

Before building new import/retarget automation, evaluate the admitted UAL/shared-rig path and any relevant audited Quaternius import helpers. The workpack still must prove useful functional coverage, import/root-motion conventions and representative retarget quality on the actual accepted character bases.

### `WP-ART-ENV-02`

This is the primary owner of the environment-factory consequences of the audit.

Before creating a new building generator, material batch system, donor extractor, Blender-to-Unity exporter or collision/prefab helper, the Worker must:

1. identify the exact repeated production problem;
2. consult the Quaternius audit and tooling catalog;
3. evaluate the native/manual path plus materially relevant audited candidates;
4. run a bounded spike where candidate adoption could plausibly save substantial repeated work;
5. preserve Juego2-owned semantic component metadata, assembly grammar, provenance and visual acceptance outside any external black box;
6. implement only the remaining gap.

Automation remains optional. If a simple modular/manual authoring recipe is cheaper, more robust and passes the repeatability challenge, no generator is required.

### `WP-ART-URBAN-01`

Consume the accepted `ART-ENV-02` production surface. Port-specific needs may extend the same factory, but the WP must not create a second unreviewed adaptation/generation pipeline because a marketplace/GitHub tool looks convenient.

### `WP-ART-03`

The final visual-production lock must judge the selected production paths by actual repeatability and keeper-quality output. A tool installing successfully, a procedural graph generating geometry or an exporter producing a prefab is not production readiness unless the fresh-author challenge demonstrates lower repeated setup work and the integrated third-person result still meets Juego2's quality bar.

## Mandatory precheck record

When an audited candidate is materially relevant to a directly affected WP, its evidence should record at least:

- current/native/manual path considered;
- candidate identity and exact capability being evaluated;
- provisional disposition (`USE / ADAPT / REFERENCE / SPIKE / WATCH / REJECT`);
- causal value/quality/compatibility decision;
- dependency/license boundary if actual adoption is proposed;
- bounded comparison or compatibility evidence when the decision is material;
- replacement/fallback path;
- confirmation that external tooling does not become product/visual/semantic authority.

A candidate that is obviously irrelevant does not require a ceremonial spike. The purpose is to prevent expensive reinvention, not to create research bureaucracy.

## Dependency and license gate

No repository, Blender add-on, marketplace tool or script recorded in discovery becomes an adopted Juego2 dependency merely because this amendment names or links it.

Actual adoption must follow `Docs/engineering/DEPENDENCY_IP_POLICY.md` at the exact version/commit and intended integration mode. This includes re-verifying license terms, provenance, notices, replacement path and shipped-vs-authoring classification.

Special handling:

- unknown/unverified code licenses are `REFERENCE_ONLY` until resolved;
- copyleft DCC helpers should, by default, remain isolated external authoring tools rather than copied/vendored into shipped Juego2 code unless separately reviewed;
- commercial tools require the normal owner purchase decision after a bounded material-value case exists;
- public source code may be studied as a technique even when direct code adoption is rejected.

## Purchase rule

This amendment does not authorize any purchase.

Do not buy a Quaternius Source pack, Blender add-on, marketplace generator or other external asset/tool merely because it appears promising in the audit. Acquisition remains need-driven under existing project policy.

For a paid tool, the owning WP should first have a real repeated-work problem and a credible reason that the tool will save material time or improve quality. Where practical, compare alternatives before requesting purchase.

## H2F boundary — explicit non-change

`WP-H2F-02` is **not amended, stopped, restarted or delayed by this decision**.

H2F owns the exact Unity/URP/toolchain/dependency foundation and its materialize/observe/reconcile/rematerialize/clean-rebuild guarantees. Quaternius adaptation factories are downstream consumers of that accepted foundation.

If a future ART WP proposes adopting a Unity-side Quaternius helper, that helper must prove compatibility through the normal accepted foundation/dependency boundary. This does not transfer character/environment factory ownership into H2F.

`WP-H2F-03` and `WP-H2F-GATE` likewise gain no new product claim from this amendment.

## CITY and GC2 boundaries

This process amendment does not change CITY topology, access, districts, route geometry or place semantics. CITY consumes keeper assets/factory outputs; it does not own Blender/Quaternius tooling selection.

Game Creator Hub reuse is a separate discovery/adoption track. GC2 extensions and Quaternius asset tooling may meet at ordinary prefab/content integration boundaries, but neither audit should silently become authority for the other.

## Relationship to accepted reuse-first authority

This amendment operationalizes, but does not weaken or replace, `Docs/product/ASSET_REUSE_VISUAL_DIRECTION_AMENDMENT.md`.

The accepted reuse order remains:

`DIRECT -> ADAPTABLE -> DONOR -> CREATE_DERIVED -> REJECT / BLOCKED_EXTERNAL`

and the composed third-person result remains the visual oracle. Tooling exists to reduce production cost; it does not justify salvaging an incompatible asset, hiding dressed greybox or accepting a lower-quality scene.

## Acceptance test for this amendment

PASS if a reviewer can confirm that:

- directly affected ART WPs must check audited reuse opportunities before material custom-tool invention;
- discovery documents remain non-canonical research inputs until a WP adopts a concrete solution through normal policy;
- exact-version license/dependency gates remain intact;
- paid tools are not pre-authorized;
- external tooling cannot become Juego2 visual/semantic authority;
- `WP-H2F-02` is explicitly unchanged and may continue in parallel;
- ART-01 is not expanded into the final environment/character factory;
- `ART-ENV-02`, `ART-CHAR-01` and `ART-ANIM-01` retain clear ownership of their production lanes.

FAIL if this amendment silently adopts a third-party dependency, requires tool installation/purchase before a real need exists, weakens provenance/visual-quality gates, expands H2F scope, or forces every WP to benchmark irrelevant tools.
