# WP-PA-02 — Worker plan

Workpack: `Docs/workpacks/PA/WP-PA-02.md`  
Execution: `REMOTE_HARVEST`  
Baseline: `main@c6e97600fa68afbe1de7fe46121880ffe8be917b`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-21

## Repair-cycle note

The initial candidate `0ab469b640535d92c73c617e0d944c0b1f45033f` received independent **FAIL** in review `#5269143202` solely because the v1.7 predecessor-contract/handoff protocol had not been persisted. The Reviewer explicitly found the PA-02 research content strong against the contract and required no semantic redesign unless the repair pre-review found a new defect.

This repair cycle therefore starts from transfer SHA `0ab469b640535d92c73c617e0d944c0b1f45033f`, remains `WP-PA-02` only, and changes process evidence/handoff only. `Docs/research/living-world/results/PA-02.md` is intentionally unchanged.

## Contract interpretation

PA-02 is an adoption/revalidation gate, not a fresh research study. Read the exact donor dossier and final review, strip donor runtime/milestone authority, reconcile current Juego2 amendments plus accepted PA-01, and publish a compact agency requirement set.

The result must demonstrate semantic support for a deterministic bounded actor-originated decision that affects another actor/world state with no privileged player trigger and no global population scan. It must not select Utility AI, GOAP, rules, planners or LLM control as universal authority.

The workpack blocks `WP-PA-03`, so this branch does not advance PA-03..14.

## PREDECESSOR_CONTRACT_CHECK

Repair-cycle timing: **completed before any repair-cycle branch write**. The initial failed cycle omitted this mandatory v1.7 evidence; this section records the check honestly rather than pretending it existed before the superseded candidate.

### Accepted direct predecessor / exact evidence

Direct dependency: `WP-PA-01 — Adopt/revalidate NPC Daily Life findings`.

- accepted candidate: `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715`;
- independent PASS review: `#5268013445`;
- implementation/research PR: `#89`;
- merge commit: `3688b7b9a27355b0fda160c20e57a385e40c6814`;
- DocSync PR: `#91`;
- DocSync merge commit: `50bbb95d261ff1964f2e920ce0380868eb2c59fd`;
- completion metadata: `Docs/workpacks/PA/WP-PA-01.md`;
- canonical accepted finding: `Docs/research/living-world/results/PA-01.md`;
- completion evidence: `Docs/evidence/WP-PA-01/DOCSYNC.md`.

PA-01 is `COMPLETE`, its canonical result is `ACCEPTED / REVIEWED`, and its DocSync names `WP-PA-02` as the dependency-valid next workpack.

PA-01 is a non-foundational research harvest, so there is no separate foundational proof-matrix obligation to inherit here. Its accepted result and DocSync explicitly carry the relevant deferred empirical proof and ownership boundaries.

### Inherited guarantees relevant to PA-02

PA-02 inherits and treats as binding:

1. **Routine is expected semantic intent, not executable choreography.** A schedule may provide context but does not become a waypoint screenplay or autonomous chooser.
2. **Routine != agency.** PA-01 explicitly leaves actor-originated action/goal choice to PA-02.
3. **Expected != actual.** Real outcomes may diverge from routine because of opportunity, interruption, player/world perturbation or later autonomous choice without rewriting the expected routine to hide that divergence.
4. **Activity intent and opportunity/place resolution are distinct and fallible.** Scarcity/unavailability may affect actual resolution without changing authored intent by fiat.
5. **Interruption/recovery re-evaluates current context rather than blindly resuming stale execution.** PA-02 may add commitment/replanning semantics without weakening that inherited recovery boundary.
6. **Travel/path realization does not own schedule intent.** Changing only route realization cannot rewrite the accepted routine meaning.
7. **Routine is a perturbable baseline in one shared player/NPC causal world.** Legitimate player/material/institutional changes may alter actual conditions; schedule cannot restore the initial town by fiat.
8. **Cheaper off-screen fidelity may not discard required causal meaning.** Exact FULL↔ABSTRACT runtime proof remains deferred.
9. **Ownership remains split.** PA-08 owns activity rules/outcomes, PA-09 owns generic player-intervention semantics, and later PA owners retain relationships, beliefs, information, memory, work, events, governance and anti-chaos.

### Guarantees newly owned by PA-02

PA-02 newly owns only the research/product requirement set for autonomous agency, including:

- actor-originated pressure/problem/goal leading to a meaningful action not already selected by schedule/quest/event;
- bounded action/target/opportunity discovery before expensive filtering/comparison, including the anti-global-scan requirement;
- actor decisions based only on authorized inputs;
- deterministic/explainable semantic decision traces without opaque global-ID equality;
- initiator/receiver semantic ownership when a social response is genuinely optional;
- commitment, cooldown/backoff, bounded replanning and explicit failure/recovery requirements;
- player-independent autonomous continuation inside the same shared causal world;
- PersistentActor-grade agency remaining distinct from cheaper ambient presence;
- deferred rather than preselected Utility AI/GOAP/rules/planner/LLM/runtime architecture.

### Inherited guarantees intentionally consumed, not re-proved

This WP **consumes rather than re-proves** PA-01's route-realization invariant, expected-vs-actual distinction, routine-as-intent meaning, interruption/current-context recovery, activity/place separation, durable-world-change compatibility and PA-08/PA-09 ownership split.

PA-02 tests only the new composition seam it actually owns: routine/context may feed an autonomous decision, but schedule/quest/event must not preselect that decision. `NC-04` is therefore a PA-02 agency-boundary control, not a duplicate proof of all PA-01 routine semantics.

### Concrete reopen condition

Reopen an inherited PA-01 guarantee only on **concrete contradictory evidence** that the accepted guarantee is false or inapplicable on the effective PA-02 composition path—for example, an actual path where route realization rewrites routine intent, interruption recovery necessarily resumes stale execution, schedule necessarily restores a durable invalidated opportunity, or the only viable agency integration requires schedule itself to select the supposedly autonomous action.

A theoretical possibility, implementation preference, or desire for duplicate defence-in-depth proof is not a reopen condition.

## Inputs audited

- `Docs/workpacks/PA/WP-PA-02.md` and PA track README;
- accepted `Docs/workpacks/PA/WP-PA-01.md`, `Docs/research/living-world/results/PA-01.md`, independent PASS `#5268013445`, PR `#89`, and `Docs/evidence/WP-PA-01/DOCSYNC.md` / PR `#91`;
- `PA_ROADMAP.md` and `PA_LEGACY_HARVEST.md`;
- playable-causal-city and intentional-transformation amendments;
- current PA-09 player-causality direction and track-level governance/activity ownership boundaries;
- donor `Arkus0/Juego/Docs/living-city-research/PA-02_NPC_AGENCY.md` at exact candidate `dfe8a2b10831774f846274143c58dc41227d6231`;
- donor PR `#36`, failed review `#5225337797`, repaired final independent PASS review `#5225572191`.

## Donor defects explicitly preserved as regression constraints

The donor final revision repaired several ways an agency proof can be faked. This harvest therefore attacks:

1. “bounded scoring” that still globally enumerates actors before filtering;
2. initiator/action code that secretly chooses the receiver response;
3. source evidence being mislabeled as proof of a Juego2 runtime adaptation;
4. AmbientPopulation and persistent agency tiers being mixed into one enum axis;
5. deterministic replay being defined by opaque global-ID equality;
6. causal initiator identity being conflated with domain/topic tags;
7. a scripted schedule/event selecting the claimed autonomous action;
8. logs/debug wrappers being confused with meaningful downstream causality.

## Execution

1. Reconstruct exact donor findings and review repairs.
2. Revalidate only the useful product semantics; do not repeat broad prior-art research.
3. Classify mechanisms `ADOPT / ADAPT / LATER / REJECT` for Juego2.
4. Remove donor M9/M10/`WorldState`/`BehaviourResolver`/API/schema authority.
5. Reconcile actor initiative with accepted PA-01 routine, shared player/NPC causality, intentional transformation and governance/activity ownership.
6. Define bounded discovery and anti-global-scan requirements before scoring.
7. Preserve explicit initiator/receiver ownership without freezing an implementation API.
8. Define actor→actor positive proof plus no-player-trigger and anti-global-scan negative controls.
9. State H3/H4/H7 consumer notes and deferred empirical proof.
10. Run Worker pre-review and freeze exact candidate for independent review.

## Ownership guards

No runtime/Unity/code changes; no H0/H1/CITY contract changes; no frozen PA-plan/amendment edits; no automatic donor acceptance; no universal AI algorithm; no final relationship/belief/memory semantics; no final GameFlow authority order; no exact agency profile taxonomy; no performance/tuning claims; no PA-03+ completion.

The donor `>=3` material linked changes rule is retained only as a useful stronger stress-fixture shape. This WP does not import the donor's old constitutional count as a universal Juego2 runtime law.

## Candidate surface

Only:

- `Docs/research/living-world/results/PA-02.md`;
- `Docs/evidence/WP-PA-02/*`.
