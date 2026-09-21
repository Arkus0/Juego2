# WP-PA-03 — Independent Reviewer handoff

Workpack: `WP-PA-03 — Adopt/revalidate Social Graph findings`  
Execution: `REMOTE_HARVEST`  
Worker: ChatGPT GPT-5.6 Sol  
Baseline: `main@a57afa3d3fc60b3e1c59d04149fe982f387c2048`  
Candidate branch: `pa/wp-pa-03-social-graph-harvest`  
Repair cycle: `fail_cycle: 1`  
Transfer SHA: `NONE`  
Freeze anchor: the exact `Frozen candidate SHA` recorded in PR #100 after all Worker evidence is committed. This file intentionally does not self-hash the commit that contains itself.

## Accepted predecessor — exact evidence

Direct dependency: `WP-PA-02`.

- accepted candidate: `015bb28ddc9facc46459c2d1dd87a89740b6c9ef`;
- independent PASS review: `#5270038879`;
- PR: `#97`;
- merge: `85d23489a6478da6bc9f33c3f017640e46ab05e9`;
- DocSync PR: `#98`;
- DocSync merge / PA-03 baseline: `a57afa3d3fc60b3e1c59d04149fe982f387c2048`;
- accepted result: `Docs/research/living-world/results/PA-02.md`;
- completion evidence: `Docs/evidence/WP-PA-02/DOCSYNC.md`.

The mandatory v1.7 predecessor check was persisted before the PA-03 result write in `Docs/evidence/WP-PA-03/WORKER_PLAN.md#predecessor_contract_check`.

Inherited PA-02 guarantees are consumed rather than redundantly re-proved: actor-owned choice, authorized inputs, bounded discovery/anti-global-scan semantics, explainable semantic decisions, receiver-owned optional response, bounded commitment/replanning and shared player/NPC causal ownership.

PA-03 newly owns only the relationship semantics/product constraints at the composition seam: directionality/asymmetry, separation of affect vs structural ties vs obligations, minimal vocabulary disposition, action-specific relationship use, bounded current-state provenance vs biography, cross-system ownership seams, relationship-dependent explanation and the required counterfactual/asymmetry fixtures.

## Review target

Primary deliverable:

- `Docs/research/living-world/results/PA-03.md`

Worker evidence:

- `Docs/evidence/WP-PA-03/WORKER_PLAN.md`
- `Docs/evidence/WP-PA-03/WORKER_PRE_REVIEW.md`

Contract:

- `Docs/workpacks/PA/WP-PA-03.md`
- `Docs/workpacks/PA/README.md`

Accepted prerequisite:

- `Docs/workpacks/PA/WP-PA-02.md`
- `Docs/research/living-world/results/PA-02.md`
- independent PASS review `#5270038879`
- `Docs/evidence/WP-PA-02/DOCSYNC.md`

## Exact donor provenance

- donor repo: `Arkus0/Juego`;
- donor dossier: `Docs/living-city-research/PA-03_SOCIAL_GRAPH.md`;
- exact donor candidate: `205aab2ae2cdd7cfd8b5fda987cbf3bbd6cba41c`;
- donor PR: `#38`;
- prior failed candidate: `f9d50a26f28e8fd77673cf916690f95dea93bc45` / review `#5226094801`;
- final donor independent PASS: `#5226298241`.

The donor PASS is provenance only. This Juego2 candidate needs fresh independent review.

## Repair-cycle provenance

The first Juego2 candidate `3c93ca888144c7b292ad100bed3679aa93195e4e` received independent `FAIL` in review `#5270307195`.

The sole causal blocker was the anti-inflation proof for adopted `affinity` and `fear`: `CF-02` and `CF-03` previously allowed a material result to remain unchanged because their expectations used “may change / may be chosen”. The repair does not broaden PA-03. It makes those two fixtures falsable in the same way as the accepted donor repair:

- `CF-02`: with exactly one changed relationship input, `affinity(Antonio -> Manolo)` HIGH → LOW must flip the selected social target `Manolo -> Paco`; same material outcome, correlated changes or tie-break dependence fail.
- `CF-03`: with exactly one changed relationship input, `fear(Antonio -> Paco)` LOW → HIGH must flip `DIRECT_CONFRONT -> SEEK_MEDIATION`; same material outcome, third-response substitution, correlated changes or tie-break dependence fail.

No research expansion, donor runtime architecture, schema choice or additional relationship dimension was introduced.

## Worker claim

The candidate reduces PA-03 to three conceptual relationship layers:

```text
1. directed actor stance: trust / affinity / fear
2. structural ties / endpoint roles: household-family, employment, authority/status-bearing roles
3. identifiable obligations: favour/debt/duty-like current commitments with lifecycle
```

It rejects one universal relationship/opinion score, implicit symmetry, permanent unsourced obligation tags, unbounded biography inside each edge, default global/N-hop graph traversal and a player-exclusive bond subsystem.

Each adopted affect dimension now has an isolated, falsable behavioural fixture with two distinct required A/B outcomes. Structural role can alter eligibility/duty without mutating affect. Obligations must be separately identifiable and capable of leaving the active set.

Relationship semantics are action-specific: PA-03 does not claim that more trust/affinity/fear universally increases or decreases every action.

## Required acceptance surfaces

### Same-world / different-edge fixtures

`CF-01` keeps actor, external world, goal, candidates, seed and every other relationship/non-relationship input fixed. It changes only `trust(Antonio -> Manolo)` and requires a target flip from Manolo to the fixed Paco alternative with no tie-break dependence and a named relationship reason.

`CF-02` now uses the same strict shape for affinity: RUN A requires Manolo; RUN B changes only `affinity(Antonio -> Manolo)` and requires Paco. A same target/behaviour result, correlated state change or tie-break dependence is an explicit FAIL.

`CF-03` now uses a strict response flip for fear: RUN A at LOW fear requires `DIRECT_CONFRONT`; RUN B changes only `fear(Antonio -> Paco)` to HIGH and requires `SEEK_MEDIATION`. A same response, third-response substitution, correlated state change or tie-break dependence is an explicit FAIL.

### Required asymmetry control

`NC-01` changes only `Antonio -> Manolo` after Antonio's proposal. `Manolo -> Antonio` must remain unchanged and receiver reasoning must use Manolo's own directed state. Any mirroring or initiator-edge substitution fails.

### Ownership / current-state controls

The result explicitly separates current relationship state plus bounded provenance from PA-06 biography; private third-party relationship knowledge remains PA-04/05-owned; activity/player/work/governance systems may cause structured relationship effects without PA-03 stealing their truth/logic.

### Explanation minimum

A material relationship-dependent decision must identify decision owner, semantic action/target/response, named relationship consumer, direction/endpoints, current semantic state, legitimate current source/reason when needed, and how the input affected eligibility/ranking/willingness/response. Counterfactual proof additionally records the declared changed input and fixed comparison surface.

## Reviewer attack surface

Please try to falsify especially:

1. **Predecessor composition:** did PA-03 accidentally redefine or weaken PA-02 actor ownership, bounded discovery, authorized information or receiver choice rather than consume them?
2. **Fake relationship causality:** can any required fixture change behavior for a second correlated input, target ordering or tie-break reason rather than exactly one edge?
3. **Affect inflation / repair regression:** do `CF-01`, `CF-02` and `CF-03` each require distinct material A/B outcomes, or can trust/affinity/fear still be marked `ADOPT` while the corresponding fixture produces no material change?
4. **Implicit symmetry:** can changing A→B still leak into or substitute for B→A?
5. **Super-score collapse:** can structural role or obligation semantics still be reduced to an unexplained friendship/opinion scalar?
6. **Role/affect conflation:** does employment/authority automatically write affection rather than altering role semantics through its own cause?
7. **Immortal obligations:** can a fulfilled favour/debt remain forever as an active causal reason?
8. **Biography theft:** does PA-03 accidentally own indefinite event history/compaction instead of only current state and bounded provenance?
9. **Knowledge leakage:** can an actor consume private third-party relationship truth with no legitimate PA-04/05 path?
10. **Hidden global scan:** does any relationship-backed target source imply permission to enumerate all actors before filtering, contrary to accepted PA-02?
11. **Cross-system ownership theft:** are rumour, memory, work, activities, player action or governance semantics silently moved into the relationship module?
12. **Universal-sign mistake:** does the candidate imply `trust/affinity/fear` have one global monotonic effect across all actions?
13. **Player exception:** does player↔NPC relationship handling require a separate privileged causal system?
14. **Premature runtime architecture:** are schema/storage/index/API/numeric ranges/tuning/UI/network-scale/persistence claimed as decided?
15. **Harvest scope:** is this still a compact revalidation/adoption result rather than a restart of broad prior-art research?

## Deferred proof

Runtime data structures, numeric tuning, thresholds/hysteresis, storage/index/query API, save/load, scale/performance, UI/redaction, final role taxonomy, long-horizon compaction, additional dimensions, authoring burden and H4 implementation proof all remain future-owned.

## PASS continuation

If and only if the exact frozen candidate passes:

```text
Next PA workpack: WP-PA-04 — Adopt/revalidate Knowledge findings
Execution class: REMOTE_HARVEST
Prerequisites: satisfied
```

On FAIL, remain on `WP-PA-03` and name the causal blocker.

## Worker state

The PR remains Draft + ACTIVE until the repaired complete candidate receives a fresh strict Worker pre-review. After `WORKER_PRE_REVIEW: CLEAN` is committed, writers stop, PR #100 records the exact final HEAD as both Candidate/Frozen SHA, and the PR is marked Ready. No further Worker writes are permitted unless fresh independent review returns FAIL and another repair cycle is opened.
