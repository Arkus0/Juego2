# WP-GC2-DIALOGUE-00 — Dialogue presentation strategy + optional Dialogue 2 adoption

Status: **PROPOSED / NOT_STARTED**
Class: PRODUCT TOOLING / DIALOGUE STRATEGY + CONDITIONAL LICENSED MODULE ADOPTION
Depends on: `WP-H2F-GATE` PASS
Blocks: `WP-ART-UI-01`
Binding decisions/policies: `Docs/product/VISUAL_PRODUCTION_FACTORY_AMENDMENT.md` + `Docs/engineering/DEPENDENCY_IP_POLICY.md`

## Claim

Juego2 has an explicit, reviewable dialogue-presentation strategy before the H2 visual lock. Dialogue 2 may be admitted as a **presentation/directing surface** only when its material value justifies acquisition and the owner provides a lawful package; otherwise the project may continue through the accepted Core/local presentation path without treating non-acquisition as a failure.

This WP is the **single adoption authority** for Dialogue 2. `WP-ART-UI-01` and later `WP-GC2-03` consume its accepted disposition. They may not independently buy, install, upgrade, repin or adopt Dialogue 2.

## First step — availability + materiality decision

The first action is to record whether Dialogue 2 is actually available and whether adopting it now is materially justified. The WP must end with exactly one explicit disposition:

- `ADOPTED` — the owner has provided a lawful package and the exact admitted version has completed every dependency/IP and H1 lifecycle requirement below;
- `NOT_MATERIAL` — evaluation found no material work saving or quality gain that justifies adopting the module for the current presentation need;
- `REJECTED` — the evaluated module/version is unsuitable because of a concrete compatibility, licensing, authority, quality or replacement concern;
- `DEFERRED_NOT_ACQUIRED` — the owner has not acquired Dialogue 2 and there is not yet sufficient material reason to buy it.

`NOT_MATERIAL`, `REJECTED` and `DEFERRED_NOT_ACQUIRED` are legitimate PASS dispositions for this checkpoint. They authorize **no retained Dialogue 2 dependency** and hand `WP-ART-UI-01` the accepted Core/local presentation path.

The project policy remains: do not buy a paid asset/plugin merely to satisfy this WP. Acquisition is justified only when the real need shows a material work saving or important quality gain.

## Scope

This is deliberately narrower than `WP-GC2-03`.

If Dialogue 2 is being considered for `ADOPTED`, evaluate/admit only surfaces needed for visual presentation, such as:

- actor/speaker presentation;
- dialogue/subtitle UI skins;
- choices presentation;
- text reveal/typewriter or equivalent no-voice pacing;
- expression/gesture/state callbacks;
- bounded camera/presentation hooks where available;
- one tiny dummy conversation used only as an adoption/visual fixture.

If the disposition is non-adopted, do **not** manufacture equivalent Dialogue 2 evidence. Record the decision and the Core/local presentation handoff instead.

Do **not** build the real investigation conversation graph, story quest flow, Ink integration policy or persistent narrative state here.

## Dependency/IP adoption gate — binding only for `ADOPTED`

Dialogue 2 is a material separately licensed dependency introduced after the H2F foundation freeze. **Before and only before this WP may declare `ADOPTED`**, it must re-run the complete exact-version adoption gate from `Docs/engineering/DEPENDENCY_IP_POLICY.md` for the owner-supplied package.

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

Unknown, ambiguous or incompatible terms fail closed **for adoption**. They may lead to `REJECTED` rather than forcing the whole checkpoint or H2 to wait for an impossible import. Owning the package is not sufficient evidence. If the proposed version materially conflicts with the frozen H2F baseline, it cannot be `ADOPTED` unless the conflict is resolved through the proper reviewed foundation process.

## Host + H1 lifecycle classification — required only for `ADOPTED`

Before `ADOPTED`, publish a complete Dialogue-selected-state lifecycle matrix equivalent in rigor to the selected-foundation matrix required by `WP-H2F-02`. Every material state family actually used must choose a concrete host/lifecycle class, source of truth, observation/reconciliation behavior and rebuild expectation.

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

If accepted H1 observation/reconciliation would erase required Dialogue state, classify it as unsupported drift, or require undocumented hand repair after materialization/rebuild, `ADOPTED` is blocked until a lawful retained host/boundary or deterministic reconstruction path exists.

No lifecycle matrix is required merely to close `NOT_MATERIAL`, `REJECTED` or `DEFERRED_NOT_ACQUIRED`, because those dispositions retain no Dialogue 2 state.

## Required composed lifecycle witness — required only for `ADOPTED`

Classification alone is not sufficient for adoption. Before Dialogue 2 is considered retained production, run one bounded composed witness with the exact admitted version and the actual dummy-presentation fixture present:

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

No import, compile proof, Dialogue fixture or lifecycle witness is required for a non-adopted disposition.

## Conditional adoption work

Only on the `ADOPTED` path:

- complete the exact dependency/IP adoption record above;
- record package/module dependencies and compatibility with the accepted H2F/GC2 Core version;
- publish the complete host/lifecycle classification matrix;
- define the Juego2 adapter boundary so plugin-private IDs/Variables do not become canonical story/world identity;
- define lawful clean-workstation restoration steps;
- prove compile/import with Core and the frozen H2F baseline;
- run and retain the composed H1 lifecycle witness;
- record uninstall/replacement boundary.

For `NOT_MATERIAL`, `REJECTED` or `DEFERRED_NOT_ACQUIRED`, record why there is no retained Dialogue 2 dependency and identify the accepted Core/local surface consumed next by `WP-ART-UI-01`.

## Disposition and later reopening rule

- `ADOPTED`: the exact version, adapter boundary, adoption record and lifecycle witness are frozen outputs consumed by `WP-ART-UI-01` and later `WP-GC2-03`.
- `NOT_MATERIAL` / `REJECTED` / `DEFERRED_NOT_ACQUIRED`: no Dialogue 2 runtime/module adoption is authorized. `WP-ART-UI-01` and later gameplay WPs use the accepted Core/local presentation path.
- If later evidence shows Dialogue 2 would save material work or create an important quality gain, adoption must be explicitly reopened through `WP-GC2-DIALOGUE-00`. Before the new exact version can become `ADOPTED`, that reopening must again satisfy exact version, license/EULA, provisioning, compatibility, authority boundary, H1 lifecycle classification, composed lifecycle witness and replacement/uninstall path.
- There is no implicit adoption from `WP-ART-UI-01`, `WP-GC2-03` or any other WP.

## Authority boundary

- Arkus/Juego2 owns persistent world truth and consequential state.
- Dialogue 2, when `ADOPTED`, owns only local dialogue presentation/execution inside its admitted scope.
- Dialogue assets/IDs may locate presentation content but do not become canonical NPC identity or persistent world facts.
- Whether Ink becomes an authored narrative source is a later explicit decision; this WP neither rejects nor canonizes Ink.

## PASS-before-work acceptance contract

**Mandatory evidence for every disposition:** explicit disposition; availability/materiality rationale; confirmation of whether any Dialogue 2 dependency is retained; the downstream presentation surface (`ADOPTED` adapter or Core/local); and the reopening rule.

**Additional mandatory evidence for `ADOPTED`:** complete `DEPENDENCY_IP_POLICY` exact-version adoption record; exact module/provisioning record; compile/import proof; complete retained/generated/manual-input lifecycle classification matrix; bounded `materialize -> observe -> reconcile -> rematerialize -> clean rebuild` witness with the actual admitted Dialogue fixture; authority/replacement boundary; one no-voice dummy conversation showing text + choice + one expression/gesture callback; and a clean removal/replacement note.

**No impossible evidence for non-adoption:** `NOT_MATERIAL`, `REJECTED` and `DEFERRED_NOT_ACQUIRED` must not be failed for lacking package bytes, import/compile proof, Dialogue fixtures, provisioning or lifecycle witnesses that only make sense after acquisition/adoption.

**FAIL if:** no explicit disposition is recorded; a downstream WP is made economically dependent on buying Dialogue 2; Dialogue 2 is used despite a non-adopted disposition; `ADOPTED` is claimed without the complete dependency/IP + lifecycle evidence; package version floats; vendor bytes are committed contrary to license; Dialogue state silently becomes Arkus authority; the WP expands into real story/investigation content; a required foundation package is silently replaced; or another WP performs an implicit adoption/upgrade.

## Non-claims

No requirement to purchase Dialogue 2, no `GC2-03` investigation-dialogue PASS, no final UI skin, no final narrative pipeline, no Ink decision and no voice acting.
