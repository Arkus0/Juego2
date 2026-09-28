# WP-GC2-DIALOGUE-00 — Dialogue 2 adoption + presentation seam

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT TOOLING / LICENSED MODULE ADOPTION
Depends on: `WP-H2F-GATE` PASS + owner-provided lawful Dialogue 2 package
Blocks: `WP-ART-UI-01`
Binding decisions/policies: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md` + `Docs/engineering/DEPENDENCY_IP_POLICY.md`

## Claim

The exact owner-supplied Dialogue 2 version can be lawfully and reproducibly admitted into Juego2 as a **presentation/directing surface** without changing Arkus authority, creating a second narrative source of truth or prematurely implementing the investigation/dialogue gameplay WP.

This WP is the **single adoption authority** for Dialogue 2. `WP-GC2-03` may later consume the accepted disposition and admitted adapter/presentation surface, but it may not independently decide, install or adopt a different Dialogue 2 version.

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

## Dependency/IP adoption gate — binding

Dialogue 2 is a material separately licensed dependency introduced after the H2F foundation freeze. Before it can be treated as retained production, this WP must re-run the complete exact-version adoption gate from `Docs/engineering/DEPENDENCY_IP_POLICY.md` for the owner-supplied package.

The adoption record must include, at minimum:

- package/provider identity and exact pinned version/package identity or hash where practical;
- exact EULA/license/commercial terms applying to that acquired version;
- lawful source/provider/acquisition and provisioning path, keeping restricted vendor bytes out of the repository when required;
- linkage/distribution mode and whether the module is shipped runtime, editor/build tooling or both;
- notices/attribution obligations;
- package/module dependencies and compatibility with the accepted H2F/GC2 Core baseline;
- the Juego2/Arkus guarantee the module assists and every guarantee explicitly outside its authority;
- the public Juego2 adapter/conformance boundary around Dialogue 2;
- update/security owner;
- replacement/uninstall strategy and exit path;
- explicit confirmation that Dialogue-private IDs/Variables/state do not become canonical NPC, clue, world, transaction or persistence identity.

Unknown, ambiguous or incompatible terms fail closed. Owning the package is not sufficient evidence. If the owner-supplied version materially conflicts with the frozen H2F baseline, this WP fails/blocks rather than silently upgrading the foundation or Core.

## Host + H1 lifecycle classification

Before PASS, publish a complete Dialogue-selected-state lifecycle matrix equivalent in rigor to the selected-foundation matrix required by `WP-H2F-02`. Every material state family actually used must choose a concrete host/lifecycle class, source of truth, observation/reconciliation behavior and rebuild expectation.

At minimum classify where applicable:

| Dialogue state family | Required lifecycle decision |
|---|---|
| exact package/version + package dependencies | pinned/reproducibly provisioned external dependency; never inferred from local cache |
| Dialogue project/editor settings used by Juego2 | retained project state with explicit host and deterministic restoration |
| retained Dialogue conversation/presentation assets used by the fixture | retained realization/content state; must survive accepted H1 observation/reconciliation without false unsupported drift |
| Juego2-owned adapter, skin/style and callback bindings | retained Juego2-owned realization state behind the public adapter boundary |
| Dialogue-local Variables/private identifiers allowed only for local execution | explicitly noncanonical local state; never canonical Arkus/Juego2 identity |
| generated/import/cache/editor/runtime outputs | generated/transient; disposable and reproducible from admitted inputs |
| licensed vendor bytes not lawfully stored in repo | external/manual source input with documented lawful provisioning before reconstruction |

If accepted H1 observation/reconciliation would erase required Dialogue state, classify it as unsupported drift, or require undocumented hand repair after materialization/rebuild, the WP is blocked until a lawful retained host/boundary or deterministic reconstruction path exists.

## Required composed lifecycle witness

Classification alone is not sufficient. Before Dialogue 2 is considered retained production, run one bounded composed witness with the exact admitted version and the actual dummy-presentation fixture present:

`materialize -> observe -> reconcile -> rematerialize -> clean rebuild`

The evidence must show that:

1. retained Dialogue/Juego2 presentation state remains present or is deterministically reconstructed from its declared host;
2. `observe` does not misclassify admitted retained state as unsupported canonical drift;
3. `reconcile` does not promote Dialogue-private state into Arkus/Juego2 canonical authority and does not erase admitted retained realization state;
4. rematerialization preserves the declared authority/adapter boundary;
5. generated/transient outputs can be deleted and rebuilt;
6. licensed/manual inputs can be lawfully reprovisioned from the documented source rather than from an undeclared machine-local cache;
7. after clean rebuild the project compiles/imports and the same dummy text + choice + expression/gesture callback fixture still works without hand repair.

The witness may reuse accepted H1/H2F tooling and does not require H1 to understand arbitrary Dialogue internals. It proves only the bounded state families actually admitted by this WP.

## Adoption work

- complete the exact dependency/IP adoption record above;
- record package/module dependencies and compatibility with the accepted H2F/GC2 Core version;
- publish the complete host/lifecycle classification matrix;
- define the Juego2 adapter boundary so plugin-private IDs/Variables do not become canonical story/world identity;
- define lawful clean-workstation restoration steps;
- prove compile/import with Core and the frozen H2F baseline;
- run and retain the composed H1 lifecycle witness;
- record uninstall/replacement boundary.

## Disposition and later reopening rule

The WP must record one explicit disposition: `ADOPTED`, `NOT_MATERIAL`, or `REJECTED`.

- `ADOPTED`: the exact version, adapter boundary, adoption record and lifecycle witness are frozen outputs consumed by `WP-ART-UI-01` and later `WP-GC2-03`.
- `NOT_MATERIAL` / `REJECTED`: no Dialogue 2 runtime/module adoption is authorized. Later gameplay WPs use the accepted local/Core presentation path unless the owner explicitly reopens this decision.
- A later reopen must occur through a separately reviewed amendment/reopened `WP-GC2-DIALOGUE-00` adoption checkpoint that again satisfies the complete dependency/IP + H1 lifecycle gate for the proposed exact version **before** any gameplay WP uses it. `WP-GC2-03` itself may never perform an implicit second adoption.

## Authority boundary

- Arkus/Juego2 owns persistent world truth and consequential state.
- Dialogue 2 owns local dialogue presentation/execution only inside its admitted scope.
- Dialogue assets/IDs may locate presentation content but do not become canonical NPC identity or persistent world facts.
- Whether Ink becomes an authored narrative source is a later explicit decision; this WP neither rejects nor canonizes Ink.

## PASS-before-work acceptance contract

**Mandatory evidence:** complete `DEPENDENCY_IP_POLICY` exact-version adoption record; exact module/provisioning record; compile/import proof; complete retained/generated/manual-input lifecycle classification matrix; bounded `materialize -> observe -> reconcile -> rematerialize -> clean rebuild` witness with the actual admitted Dialogue fixture; authority/replacement boundary; one no-voice dummy conversation showing text + choice + one expression/gesture callback; explicit `ADOPTED` / `NOT_MATERIAL` / `REJECTED` disposition; and a clean removal/replacement note.

**FAIL if:** package version floats; exact license/EULA or distribution/linkage mode is unresolved; vendor bytes are committed contrary to license; update/security/replacement ownership is missing; Dialogue state silently becomes Arkus authority; material Dialogue state has no declared host/lifecycle behavior; the composed H1 lifecycle loses retained state, reports it as unsupported canonical drift, requires hand repair, or relies on undeclared local cache; the WP expands into real story/investigation content; a required foundation package is silently replaced; or the presentation cannot coexist with the accepted GC2 Core/H2F baseline.

## Non-claims

No `GC2-03` investigation-dialogue PASS, no final UI skin, no final narrative pipeline, no Ink decision and no voice acting.
