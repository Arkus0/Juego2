# Juego → Juego2 Knowledge Ledger

Version: 1.0 — 2026-09-20  
Status: **NON-BINDING REFERENCE / PROCESS_ONLY HARVEST.**

This document changes no H0 guarantee, acceptance criterion, workpack contract, roadmap ordering or `WP-HK-GATE` precondition. It does not authorize H1/gameplay implementation. `Arkus0/Juego` remains reference material only under `AGENTS.md`; Juego2 contracts and accepted evidence remain authoritative.

## 0. Purpose

`Arkus0/Juego` contains substantial product knowledge that should not be lost merely because its implementation path was abandoned. The safe unit of migration is **not a file, class, workpack or architecture**. It is a reusable decision, invariant, failure mode, behavioral specification, acceptance scenario, production lesson or anti-goal.

The harvest rule is:

```text
Juego source
  -> extract knowledge
  -> strip donor architecture / old milestone routing / DFU-Shenmue authority
  -> reconcile with accepted Juego2 boundaries
  -> classify
  -> preserve as non-binding input or future fixture
```

Nothing receives authority merely because it was `ACCEPTED`, `PASS` or implemented in Juego. Old review verdicts bind old SHAs and old contracts only. Likewise, an old license observation is research, not Juego2 adoption evidence; `Docs/engineering/DEPENDENCY_IP_POLICY.md` requires exact-version re-verification at adoption time.

## 1. Classification

| Class | Meaning in Juego2 |
|---|---|
| `ABSORBED` | The useful lesson is already represented more strongly or more cleanly in accepted Juego2 contracts or the current Production Blueprint. No migration work is needed. |
| `CARRY` | Preserve the knowledge explicitly as planning/test input for a future phase. It does not become binding until a reviewed Juego2 WP/ADR adopts it. |
| `REWRITE` | The mechanism/problem is useful, but the donor representation conflicts with accepted Juego2 architecture. Preserve intent, redesign the representation. |
| `BENCHMARK` | Observation/checklist only. It may guide measurement or behavioral specs; it is not a dependency or authority. |
| `DROP` | Do not migrate. The item is donor-specific, superseded, contaminated by an abandoned foundation, or creates more risk than value. |

Contamination labels used below:

- `DFU` — assumes Daggerfall Unity as product foundation.
- `SHENMUE` — assumes Shenmue formats/runtime/data as implementation authority.
- `OLD_RUNTIME_STATE` — assumes the donor's ticking gameplay `WorldState`; conflicts with the authored/live boundary accepted by `WP-HK-06A`.
- `OLD_WP_ROUTING` — donor `M*`, `REMOTE-DONE/PREP`, local/cloud routing or old dependency graph.
- `STALE_ADOPTION` — old price/license/version/adoption observation that must be rechecked before actual use.

A contaminated wrapper does not poison a reusable fact. We keep the fact and discard the wrapper.

---

# 2. Executive harvest result

The high-value archive divides into five veins:

1. **Agentic authoring / product semantics** — mostly already absorbed by H0: stable semantic IDs, self-describing contracts, bounded inspection, plan/dry-run, deterministic validation, structured errors, provenance, replay and presentation/domain separation.
2. **Production content pipeline** — substantial net-new detail worth carrying: transformation recipes, semantic sockets, presentation profiles as validators, pilot-batch gates, wardrobe/animation/building lanes and measured batch throughput.
3. **Gameplay-system design** — strong reusable semantics: GameFlow authority, structured outcomes, conditions/explanations, persistent actor identity, schedule-as-intent, truth-vs-belief, directed relationships, event visibility, FULL/ABSTRACT continuity and bounded autonomous agency.
4. **Fixture/failure-case bank** — especially valuable. The old M5–M14 plans contain concrete tests for double authority, QTE boundaries, third-person camera assumptions, false beliefs, witness rules, save migration, batch failure and shipping provenance.
5. **Prior-art / failed-foundation lessons** — use as a question bank and anti-regression record, never as architecture. DFU/OpenShenmue/old runtime state do not return through this document.

The current `Docs/production/PRODUCTION_BLUEPRINT.md` has already harvested a large portion correctly: keeper-town planning, the 13-character foundation, authored/live separation, the Living City and Combat charters, phase ordering, vertical-slice intent and major rabbit-hole guards. This ledger therefore focuses on **what remains useful around and underneath that blueprint** rather than duplicating it.

---

# 3. Source ledger

| Donor source | Disposition | What survives | What does not travel | Likely Juego2 use |
|---|---|---|---|---|
| `Docs/AUTHORING_FOUNDATION.md` | `ABSORBED` | one canonical model consumed by runtime/harness/future Creator; inspectable/versioned data; deterministic validation; bounded authoring loop | old HS/AS milestone topology | H0 already embodies the stronger form; use as historical rationale only |
| `Docs/AGENTIC_AUTHORING.md` | `ABSORBED` + `CARRY` | stable IDs; inspect-before-mutate; dry-run/diff; machine-readable errors; provenance; domain families for world/actor/schedule/knowledge/sequence | exact command names and donor milestone ownership | capability ideas when a real Hx consumer appears |
| `Docs/adr/ADR-014-HARNESS-FIRST-CONTRACT-CORE.md` | `ABSORBED` + **anti-regression** | if discovery must reverse-engineer private implementation rules, semantic ownership is wrong; fix the source-of-truth boundary instead of adding the next exception | old HS01/M4C/AS01 plan | H1 Unity bridge must not grow a bridge-local semantic registry or private rejection dialect |
| `Docs/engineering/WORLD_REPLACEMENT_SCHEMA.md` | `REWRITE` | identity independent of presentation; slot-level provenance; lineage + transformation pipeline; deterministic validation; replacement intent separate from execution | donor schema/ID grammar and DFU source categories as canonical model | H1 presentation/catalogue/binding design |
| `Docs/ASSET_FACTORY.md` | **`CARRY`** | source manifest; transformation recipe; presentation profile; semantic sockets; validation report; animation semantic IDs; wardrobe grammar; pilot batch before mass production | DFU foundation, old milestone numbers, old purchase timing/prices | H1 representative real assets; H2/H3 production kit; post-VS batch pipeline |
| `Docs/DEV_REFERENCE_TOWN.md` | `ABSORBED` / `DROP` | compact observability; 30–90 s POI test travel; semantic POI identity independent of placeholder presentation; local smoke checklist | Chesterwark/Aldingwall/Gothway and DFU location identity | already translated into keeper-town/product-seed planning |
| `Docs/GAMEFLOW_RUNTIME.md` + `ADR-010` | **`CARRY` / `REWRITE`** | shared authority for player/camera/actors; enter/exit/rollback; structured results; small composable sequences; skip/cancel/failure cleanup | old `WorldState` consequence sink and DFU framing | H5 GameFlow/cinematics/QTE and H6 combat integration |
| `Docs/LIVING_WORLD_RUNTIME.md` + `ADR-011/013` | `ABSORBED` + `CARRY` | PersistentActor vs AmbientPopulation; schedule = intent; truth != belief; directed relationships; reason traces; bounded agency; FULL/ABSTRACT same causal meaning | donor runtime-state store and old M9/M10 ownership | H3/H4/H7 fixtures and future runtime/save design |
| `Docs/Architecture/GAMEPLAY_SYSTEMS_ARCHITECTURE.md` | `REWRITE` | one condition vocabulary; deterministic phase order; no reentrant event mutation; seeded randomness; Why/explain traces; presentation behind ports | donor core topology and ticking canonical `WorldState` | runtime architecture questions only after consumers exist |
| `Docs/Architecture/GAMEPLAY_DEPENDENCY_GRAPH.md` | `CARRY` as design hygiene | DAG mindset; solve backward needs through ports/data; explicit anti-cycle patterns | exact donor nodes/build graph | future gameplay architecture review checklist |
| `Docs/PRIOR_ART_HARVEST.md` + `ADR-012` | `CARRY` as research index; adoption rules `ABSORBED/SUPERSEDED` | candidate catalogue, behavioral questions, `whatWeTake/whatWeDoNotTake`, adapter/replacement thinking | old license conclusions and DFU-first posture | use candidates only when a future WP has a concrete question; adoption obeys Juego2 IP policy |
| `Docs/SHENMUE_KEEP.md` | **`BENCHMARK` firewall** | camera/staging/control-transition questions; QTE timing/branching; combat spacing/targeting/hit/crowd questions; interaction feel; observable NPC-life questions | renderer, formats, scheduler, world port, OpenShenmue runtime | behavioral specs/playtest references only; never architecture |
| `Docs/TRANSITION_DFU_20260916.md` | `DROP` foundation, `CARRY` meta-lesson | research must answer a concrete production question, have a budget and feed an immediate feature/test/decision | DFU-as-foundation decision | research discipline only |
| `Docs/living-city-research/*` | largely `ABSORBED` by Production Blueprint | LC charter, completed findings, failure registers, acceptance scenarios, anti-goals | `PROJECT_DELTAS` as deltas; old PASSes; old runtime-state targets | H3/H4/H7 design and fixtures |
| `Docs/combat-research/*` | charter `ABSORBED`; plans `CARRY` as question backlog | 20 combat laws + preregistered research questions | pretending the 12 unrun tracks are findings | H6 research/playtest plan when combat becomes current |
| `Docs/workpacks/M5..M14/*` | **`CARRY` as fixture bank; `DROP` as WPs** | acceptance scenarios, failure cases, integration gates, production/shipping checks | milestone numbering, dependency chain, remote/local classes, old contracts/verdicts | future Hx WP authoring after re-derivation |

---

# 4. High-value knowledge not to lose

## 4.1 Asset factory: recipe, lineage, validation, then scale

The strongest transferable idea from `ASSET_FACTORY.md` is not a particular pack. It is a production contract:

```text
legal/pinned source
 -> provenance + hash + license record
 -> normalize scale/pivot/rig/material/naming
 -> versioned transformation recipe
 -> presentation-profile validation
 -> semantic sockets / bindings
 -> engine smoke
 -> promote/register
```

Important consequences to carry:

- a transformed external seed keeps its lineage; transformation does not magically make it `ORIGINAL_OURS`;
- gameplay references semantic animation/action identity, not vendor filenames;
- style targets should become measurable validators, not adjectives in prompts;
- wardrobe variation is combinatorial data over a shared body/rig, not a demand for one unique rig/controller per NPC;
- production should **pilot 10–20 variants before mass generation** and measure rework/failure rate;
- buy/reuse generic work; author the memorable/identity-bearing work;
- reviewed good compositions should be promoted so later/weaker agents instantiate/adapt them instead of redesigning every façade from primitives.

This complements the Production Blueprint's H1 real-asset slice and should inform the eventual H1/H2 asset/catalogue WPs. It does **not** authorize any pack purchase or adoption now.

## 4.2 GameFlow is a resource-ownership problem before it is a cinematic system

The reusable GameFlow insight is that Dialogue, Cinematic, QTE and Combat compete for shared runtime authorities. Therefore future runtime design should make ownership explicit rather than allowing components to toggle one another opportunistically.

Carry these invariants as future test ideas:

- one effective owner of player-control authority at a time;
- one effective camera/director authority at a time;
- a directed actor does not simultaneously run incompatible schedule/autonomous/combat/cinematic control;
- every transition has acquire/enter, active state and release/exit or rollback;
- skip, cancel, failure, exception and load/recovery cannot strand ownership;
- returning an actor to Living World causes re-evaluation against the resulting runtime state;
- presentation executors such as Timeline/Animator/Camera are downstream executors, not narrative authority.

The donor's `WorldState` bridge is **not** carried. Runtime/save-state authority must be designed separately from HK06 authored state, exactly as the Production Blueprint already requires.

## 4.3 Conditions should be explainable and should not own state

The donor architecture repeatedly converged on a single reusable lesson: a condition system is more valuable when the same semantic predicates can be reused by dialogue, schedules, interaction, story and tests and when it can explain the result.

Preserve as a future design target, not a preselected implementation:

```text
Evaluate(condition, read-only context) -> true/false/error
Explain(condition, read-only context) -> per-leaf trace + referenced state
Validate(condition) -> authoring diagnostics
```

Do not persist a condition's result as truth merely because it was evaluated once. Do not put important knowledge only in dialogue text. Do not create a private condition dialect per subsystem unless a future reviewed tradeoff proves it necessary.

This must be reconciled with Juego2's opaque authored payload boundary: schema-aware future producers may own typed condition payloads without forcing H0 kernel semantics to understand gameplay conditions.

## 4.4 Events and outcomes are integration seams, not permission for event sourcing everywhere

Useful donor rule:

- meaningful actions return structured outcomes/events;
- a cinematic/QTE/combat system should not directly edit social state ad hoc;
- witnesses, beliefs, relationships, schedules and later narrative consume structured consequences through their own authorities;
- causal lineage is useful where it helps explain a player-visible consequence;
- none of this requires making every state mutation an event-sourced architecture.

The exact runtime event/save model remains a future H4+ decision.

## 4.5 Persistent actor, ambient population and promotion boundary

Carry the distinction:

- `PersistentActor`: stable identity, authored role/profile/schedule setup, runtime continuity, beliefs/relationships/memory as required, explainable decisions;
- `AmbientPopulation`: density/presence/cheap interaction, no individual persistent social truth by default;
- promotion from ambient to persistent is explicit when a story/system needs individual continuity.

This prevents a common scale failure: simulating fifty decorative people as fifty full social agents merely because they are visible.

## 4.6 Schedule is expectation, not a waypoint screenplay

Carry:

- authored schedule describes time windows, activity intent and semantic destination/POI needs;
- navigation owns the route;
- interruptions/events/goals can override normal expectation;
- fallback/resume policy is explicit;
- crossing-midnight and POI capacity are first-class edge cases;
- routine inspection should tell the user what the actor was expected to do and why reality differs.

## 4.7 Full and abstract simulation must preserve causal meaning

The old design's best off-screen rule remains useful:

> if an actor moves between FULL and ABSTRACT simulation, presentation fidelity may change but identity and meaningful causal state must not silently change category.

A significant abstract action should produce the same **kind** of semantic outcome as the visible equivalent. Streaming/LOD must not erase a committed goal, duplicate an actor or manufacture social truth.

## 4.8 Prior art should answer questions, not choose our architecture

Useful old workflow, rewritten under Juego2 policy:

```text
concrete product question
 -> find candidate/prior art
 -> inspect exact source/version/license if adoption is possible
 -> extract mechanism / edge case / behavioral spec
 -> decide OWN / ADAPT / BENCHMARK / REJECT under Juego2 rules
 -> encode the value as Arkus/Juego2 contract, fixture or measurement
```

The candidate catalogue in Juego remains useful discovery input — e.g. location-authoring tools, town-density mods, dialogue filters, population systems, third-person workarounds, Quaternius/Kenney/ALEX seeds — but **none of the old adoption/license statements are approvals**.

## 4.9 Research discipline learned from Shenmue/DFU

Keep the question whitelist; drop the implementation obsession.

Shenmue may still be useful to answer bounded observational questions about:

- entering/leaving player control;
- camera/staging rhythm;
- QTE telegraph/windows/branching/recovery;
- combat spacing, target transitions, perceived startup/recovery, hit reactions and group pressure;
- contextual interaction range/focus/feedback;
- visibility of NPC routines.

Do not revive format fidelity, renderer reconstruction, original scheduler, area porting or OpenShenmue as prerequisites. A behavioral specification or instrumented playtest is preferable when it answers the product question more cheaply.

---

# 5. Future fixture bank recovered from old workpacks

These are **test seeds**, not acceptance criteria today. A future roadmap/WP author may adopt, adapt or reject each one under current dependencies.

## H1 — Unity bridge / real asset boundary

- **H1-F01 Hidden-registry negative:** a Unity provider may contribute implementation/bindings, but a capability/semantic rule that exists only in a bridge-local registry must not become public truth. This is the H1 application of the ADR-014 lesson.
- **H1-F02 Real-asset stress:** use at least one representative asset set that exercises pivot/origin, nested hierarchy, scale, materials, collider/reference identity and missing-reference diagnostics; cubes alone do not prove the bridge.
- **H1-F03 Binding replacement:** changing a presentation asset/binding does not rename or silently rewrite the canonical semantic identity it realizes.
- **H1-F04 Missing realization:** canonical intent referencing an unavailable engine asset/composition fails visibly with structured diagnostics; the bridge does not silently mutate canonical meaning to fit what Unity happens to contain.
- **H1-F05 Lineage:** a transformed third-party seed retains exact source/license/hash/recipe evidence required by the current dependency policy.

## H2 — keeper town / world composition

- **H2-F01 Composition promotion:** build one reviewed façade/shop/bar composition from modules, validate sockets/bounds/style constraints, promote it to a reusable composition, then instantiate it again without reconstructing from primitives.
- **H2-F02 Semantic place vs realization:** swap/rework a visual building realization while stable place/building/POI references used by authored content remain intact.
- **H2-F03 Observability:** representative POIs remain easy enough to traverse/debug that a schedule scenario can be watched without turning the keeper town into a radial test diagram.

## H3 — actors, schedules and POIs

- **H3-F01 Actor identity chain:** authored actor -> stable ActorId -> presentation binding -> runtime handle -> movement/activity -> save/reload -> same identity; replacing the presentation does not reauthor gameplay.
- **H3-F02 Midnight schedule:** an ordered schedule crosses midnight and yields one unambiguous expected activity/fallback.
- **H3-F03 POI contention:** two actors compete for limited POI capacity; failure/fallback is deterministic and explainable.
- **H3-F04 Headless/runtime parity:** for a representative routine decision, headless semantic choice and realized Unity behavior agree on the chosen activity/target even though path/animation is engine-owned.

## H4 — Living World Core

- **H4-F01 Truth is not belief:** actor A can hold a false or uncertain belief without mutating canonical/runtime world truth; provenance identifies how A acquired it.
- **H4-F02 Directed relationship:** A->B and B->A may differ; a change affects an eligible later decision rather than only a dialogue number.
- **H4-F03 Dialogue explain:** a line/choice is accepted/rejected by belief/relationship/current-activity conditions and tooling can explain which predicate caused the result.
- **H4-F04 Witness pipeline:** private/public/location/participant visibility determines which persistent actors may learn an event; ambient population does not silently gain persistent quest truth.
- **H4-F05 Relay is a decision:** receiving information does not automatically broadcast it; relaying creates new provenance and may fail/be declined.
- **H4-F06 Causal before/after:** an event/information transfer changes a later routine/dialogue/action and a snapshot/diff + reason trace explains the chain.

## H5 — GameFlow, cinematics and QTE

- **H5-F01 Authority conflict:** double ownership is rejected rather than resolved by arbitrary component toggles.
- **H5-F02 Skip/cancel/failure:** sequence skip, QTE fail, missing actor, executor exception and user cancel all release/sanitize authorities.
- **H5-F03 Load sanitization:** restoring a save/checkpoint cannot recreate an impossible combination of authority owners.
- **H5-F04 QTE boundaries:** deterministic tests cover exact input-window edges, timeout, allowed failures and branching before Unity input/UI timing is playtested.
- **H5-F05 Semantic beat:** changing an animation clip/camera implementation does not change the semantic sequence definition merely because a presentation asset was replaced.

## H6 — combat

The old combat research plans were not executed; do not treat them as evidence of feel. Still preserve these test ideas:

- **H6-F01 Third-person origin:** camera offset/distance does not silently alter melee range, projectile origin or combat semantics.
- **H6-F02 Ownership transition:** Exploration/Cinematic <-> Combat acquires/releases player, camera and combatants cleanly.
- **H6-F03 Structured aftermath:** CombatResult becomes a structured outcome/event consumable by Living World; combat does not maintain a duplicate relationship/knowledge store.
- **H6-F04 Group readability:** >=2 enemies exercise pressure/spacing/turn-taking policy instead of merely increasing health/damage.
- **H6-F05 Feel requires play:** claims about responsiveness, impact, camera readability and timing require instrumented local play evidence, not headless/document confidence alone.

## H7 — deeper agency / abstract simulation / population

- **H7-F01 Player absent:** a persistent actor originates a bounded Actor->Actor/world action with the player absent; the chain terminates/stabilizes and can explain why it happened.
- **H7-F02 Counterfactual:** rerun the same seed/state with one belief/relationship changed; the reasoned behavioral difference is inspectable.
- **H7-F03 FULL->ABSTRACT->FULL:** preserve identity, meaningful state, committed action/goal policy and location semantics with no duplicate runtime actor or impossible teleport semantics.
- **H7-F04 Ambient isolation:** scale ambient density 0/low/target without accidentally multiplying persistent social agents or changing named-actor truth absent an explicit causal rule.
- **H7-F05 Budget:** measure reevaluation frequency, candidate/target count, initiative rate and CPU/state cost before increasing autonomy scope.

## Vertical slice

- **VS-F01 Integration, not reinvention:** scenario graph validates reachability, missing refs, branch endings, preconditions, failure/skip paths and aftermath using already accepted systems.
- **VS-F02 Aftermath:** at least one event leaves inspectable later social/behavioral consequences, with witness/provenance/relation/schedule or dialogue evidence as applicable.
- **VS-F03 Save/load:** persistence resumes a coherent runtime state without converting runtime history into the HK06 authored journal.
- **VS-F04 Replacement safety:** changing a presentation seed/asset near the slice does not break semantic IDs because external asset IDs were never persistent gameplay identity.

## Post-VS production / shipping

- **P-F01 Batch dry-run:** manifest set -> plan/dry-run -> generate/update -> provenance validation -> semantic validation -> diff/report; no destructive batch mutation without explicit execute.
- **P-F02 Partial failure:** a batch reports per-item failure/rework without turning successful unrelated outputs into false success/failure state.
- **P-F03 Throughput:** measure human time, agent operations, reuse ratio when meaningful, failure/rework rate, reproducibility and transformation cost. Do not invent percentages.
- **P-F04 Shipping inventory:** shipped path contains no unresolved `BENCHMARK_ONLY`, development placeholder, restricted/unknown-license or hidden load-order dependency.
- **P-F05 Save migration:** old/current/future/corrupt save fixtures exercise migration/recovery/backups with no silent deletion; presentation replacement does not break saves through vendor IDs.
- **P-F06 Clean build:** clean checkout -> build -> launch -> new game -> save/load -> representative slice -> notices/license manifest.

---

# 6. Explicit DROP list

The following must **not** re-enter Juego2 merely because adjacent knowledge was useful:

1. DFU as world/runtime foundation or required shipping dependency.
2. OpenShenmue as engine/runtime, Shenmue renderer reconstruction or original format fidelity as product work.
3. Shenmue/DFU IDs, paths, file formats, prefab/object identifiers or scheduler semantics as canonical Juego2 identity.
4. Donor `M*`, `GS*`, `AS01`, `HS01`, `M4C` workpack identities, dependency ordering and remote/local execution classes.
5. Donor independent PASSes as Juego2 evidence.
6. The donor ticking gameplay `WorldState` as an extension of Arkus authored canonical state. Gameplay runtime/save authority remains separate.
7. Old package prices, versions or license observations as current adoption facts.
8. A general GOAP/Utility/AI framework chosen before a bounded actor scenario proves what decision problem exists.
9. One executable day script per NPC, one full social agent per ambient pedestrian, or one smart-object ontology for every prop.
10. Dialogue text, LLM output, Timeline, Animator, engine scene state or an external tool as semantic authority.

---

# 7. Mapping to current post-GATE planning

| Current planning phase | Harvest material to consult before authoring WPs |
|---|---|
| **H1 Unity bridge** | ADR-014 anti-regression; Asset Factory normalization/lineage/validation; World Replacement identity-vs-presentation lessons; current Dependency/IP policy |
| **H2 keeper town** | Asset Factory building/nature lanes; composition promotion; reference-town observability lessons; prior-art density/interior edge cases as optional benchmarks |
| **H3 actors/routines** | M5 identity/binding gate ideas; Living Actor state; schedule/POI models; Quaternius-like seed questions only after exact-version adoption review |
| **H4 Living World Core** | completed Living City research; condition explanations; M9/M10 belief/relationship/witness/rumour fixtures; runtime/save authority decision |
| **H5 GameFlow/cinematics/QTE** | ADR-010/GameFlow; M6/M7 failure fixtures; Shenmue behavioral question whitelist only |
| **H6 combat** | Combat charter + twelve unrun research plans as research backlog; M8 third-person/integration checks; instrumented-play requirement |
| **H7 agency/abstract/population** | ADR-013 principles; PA-02..05 findings/failure registers; M10 continuity/density/budget fixtures |
| **VS** | M12 integration/aftermath discipline, but use the current keeper-town slice rather than inheriting the old cemetery route as authority |
| **post-VS scale/shipping** | M13 deterministic batch pipeline/throughput metrics; M14 provenance, save migration, accessibility, profiling and clean-build gates |

**Planning rule:** before designing each Hx, read the relevant rows of this ledger and the cited donor documents. Reuse knowledge, never donor authority.

---

# 8. Harvest coverage and deliberate exclusions

This pass inspected the high-value product/design spine, ADRs, architecture/dependency documents, prior-art harvest, Asset Factory, authoring foundation, Living World/GameFlow documents, Living City/Combat research inventories and the M5–M14 production plans most likely to contain future acceptance knowledge.

It deliberately does **not** migrate:

- old automation/remote-chat mechanics that Juego2 has already replaced;
- old proof/review process where Juego2 already has a stronger accepted standard;
- the full body of Shenmue reverse-engineering/Legacy evidence;
- old implementation code merely because it once worked;
- every old WP line-by-line.

Those remain searchable in `Arkus0/Juego` if a future concrete question requires forensic follow-up. This ledger is an index of **reusable knowledge**, not a mirror of the archive.

## Bottom line

Juego2 should treat Juego as a **knowledge quarry, not a parent architecture**.

The important material is now separated into:

- already absorbed knowledge that should not be re-litigated;
- concrete future design/fixture input worth carrying;
- benchmark-only behavioral knowledge;
- representations that require rewriting across the authored/live boundary;
- explicit dead ends that must stay dead.

That gives future Hx planning a prepared starting point without contaminating H0 or importing Shenmue/DFU by the back door.
