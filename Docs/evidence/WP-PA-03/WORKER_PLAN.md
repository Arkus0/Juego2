# WP-PA-03 — Worker plan

Workpack: `Docs/workpacks/PA/WP-PA-03.md`  
Execution: `REMOTE_HARVEST`  
Baseline: `main@a57afa3d3fc60b3e1c59d04149fe982f387c2048`  
Candidate branch: `pa/wp-pa-03-social-graph-harvest`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-21

## Contract interpretation

PA-03 is a compact adoption/revalidation of the independently reviewed donor Social Graph study. It does not repeat broad prior-art research and does not import donor M9/M10/runtime schemas. The current WP owns only the research/product semantics needed to constrain later H4 consumers: materially different directed relationship state must be capable of changing an explainable action, target, eligibility, willingness or receiver response rather than only dialogue flavour.

The acceptance control is intentionally asymmetric: changing only `A -> B` must not silently mutate or semantically substitute for `B -> A`.

## PREDECESSOR_CONTRACT_CHECK

Completed before any PA-03 implementation/result write. This Worker-plan commit is the first branch write.

### Accepted direct predecessor / exact evidence

Direct dependency: `WP-PA-02 — Adopt/revalidate NPC Agency findings`.

- accepted candidate: `015bb28ddc9facc46459c2d1dd87a89740b6c9ef`;
- independent PASS review: `#5270038879`;
- implementation/research PR: `#97`;
- merge commit: `85d23489a6478da6bc9f33c3f017640e46ab05e9`;
- DocSync PR: `#98`;
- DocSync merge / current PA-03 baseline: `a57afa3d3fc60b3e1c59d04149fe982f387c2048`;
- canonical accepted finding: `Docs/research/living-world/results/PA-02.md`;
- completion evidence: `Docs/evidence/WP-PA-02/DOCSYNC.md`.

PA-02 is complete and its DocSync names `WP-PA-03` as the next dependency-valid workpack.

### Inherited guarantees relevant to PA-03

PA-03 consumes as binding:

1. **Actor-owned agency.** A relationship is an input to an actor-owned decision, not permission for a relationship system, quest or timetable to preselect the action.
2. **Authorized information only.** An actor may consume only relationship information it is legitimately entitled to know/use; canonical world truth is not automatically actor knowledge.
3. **Bounded discovery before expensive comparison.** Relationship-backed target discovery cannot hide a global population scan behind a small returned/scored set.
4. **Explainable semantic choice.** Material decision inputs/reasons must be inspectable at the semantic level; opaque implementation scores are insufficient evidence.
5. **Receiver ownership where response is genuinely optional.** An initiator may choose a social proposal/target, but the receiver owns its own response decision from receiver-authorized state.
6. **Commitment/replanning/failure stay bounded.** Relationship changes do not license tick-level global reevaluation or thrashing.
7. **Shared player/NPC causal world.** Player-created circumstances may legitimately alter later actor inputs/opportunities without directly commanding NPC decisions or requiring a player-only consequence universe.
8. **No universal AI algorithm was selected.** PA-03 must remain chooser/engine/schema neutral.

Accepted PA-01 routine guarantees are inherited transitively only where PA-02 relies on them: routine is context/expected intent, not the autonomous chooser; expected and actual state remain distinct; durable world change may perturb actual behavior.

### Guarantees newly owned by PA-03

PA-03 newly owns only the relationship-semantics product constraints needed by the workpack:

- directed/asymmetric relationship state as the default for actor stance;
- separation of affective stance, structural ties/roles and lifecycle-bearing obligations so they cannot collapse into one universal opinion scalar;
- a minimal useful vocabulary recommendation classified `ADOPT / ADAPT / LATER / REJECT` without freezing runtime representation;
- action-specific consumption of relationship semantics rather than a universal monotonic social formula;
- current relationship state plus bounded/sourced explanation, while unbounded autobiographical history remains outside PA-03;
- relationship changes may be caused by owned external systems, but PA-03 does not absorb memory, belief, rumour, work/activity or governance ownership;
- same-world/different-edge counterfactual proof and the required A→B/B→A negative control;
- minimum explanation fields for a relationship-dependent decision;
- future H4 consumer notes and explicitly deferred empirical/runtime proof.

### Inherited guarantees intentionally consumed, not re-proved

This WP does not re-prove PA-02 actor origin, global-discovery boundedness, receiver-decision ownership, commitment semantics, authorized-input principle or shared causal-world ownership. PA-03 exercises only the new composition seam it owns: a relationship input can materially change an otherwise equivalent actor-owned decision while preserving those accepted boundaries.

Where a social-graph query is discussed, PA-02's anti-global-scan guarantee is referenced as inherited rather than re-specified as a new PA-03 performance architecture.

### Concrete reopen condition

Reopen a PA-02 guarantee only on concrete contradictory evidence that the accepted guarantee is false or inapplicable on the effective relationship-consumer path—for example, if the only viable relationship-dependent target lookup necessarily begins with global actor enumeration, if receiver-owned relationship semantics can only be implemented by the initiator choosing the receiver response, or if a relationship reason can only be consumed through actor-unauthorized omniscient state.

A theoretical implementation possibility, preference for duplicate proof, or uncertainty about a future schema is not a predecessor reopen condition.

## Donor provenance audited

- donor repo: `Arkus0/Juego`;
- donor dossier: `Docs/living-city-research/PA-03_SOCIAL_GRAPH.md`;
- exact donor candidate: `205aab2ae2cdd7cfd8b5fda987cbf3bbd6cba41c`;
- donor PR: `#38`;
- donor failed candidate: `f9d50a26f28e8fd77673cf916690f95dea93bc45` / review `#5226094801`;
- donor final independent PASS: review `#5226298241` on `205aab2ae2cdd7cfd8b5fda987cbf3bbd6cba41c`.

The donor PASS is provenance only. Juego2 needs its own independent review.

The donor repair history is retained as regression guidance: each baseline affect dimension was required to earn an isolated behavioral counterfactual; the principal fixture must change exactly one relationship input without tie-break dependence; graph-backed target discovery must not hide global enumeration inside a provider; and evidence certainty must remain separate from Juego2 adoption disposition.

## Execution

1. Reconstruct donor findings and review repairs without copying donor runtime authority.
2. Reconcile them with accepted PA-01/02 and current Juego2 player-causal / intentional-transformation direction.
3. Publish a compact canonical PA-03 result under `Docs/research/living-world/results/PA-03.md`.
4. Classify the minimal relationship vocabulary and mechanisms as `ADOPT / ADAPT / LATER / REJECT`.
5. Define the same-world/different-edge fixture plus the explicit A→B/B→A negative control.
6. Define relationship-vs-memory/belief/rumour/work/activity/governance ownership and update pathways without stealing those systems.
7. Define minimum relationship-dependent explanation requirements and H4 consumer constraints.
8. Preserve runtime storage, tuning, UI, network scale, persistence and final schemas as deferred proof.
9. Perform strict Worker pre-review against the complete baseline→candidate diff while Draft; repair any in-claim defect before declaring `CLEAN`.
10. Stop writers, bind exact HEAD as frozen candidate and mark the PR Ready for fresh independent review.

## Ownership guards

No runtime/Unity/code changes; no H0/H1/CITY changes; no edits to frozen PA plans/amendments; no automatic donor acceptance; no universal friendship/opinion score; no final schema/storage/index/API; no belief acquisition (PA-04), rumour propagation (PA-05), autobiographical memory/compaction (PA-06), economic/employment simulation (PA-07), activity dramaturgy/rules (PA-08), generic player-action semantics (PA-09), governance ownership (PA-12), anti-chaos/recovery (PA-13), or PA-04+ completion.

## Candidate surface

Only:

- `Docs/research/living-world/results/PA-03.md`;
- `Docs/evidence/WP-PA-03/*`.
