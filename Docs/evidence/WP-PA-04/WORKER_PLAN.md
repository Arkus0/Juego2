# WP-PA-04 — Worker plan

Workpack: `Docs/workpacks/PA/WP-PA-04.md`  
Execution: `REMOTE_HARVEST`  
Baseline: `main@2da4b6fd4a6eb7bac166c77ba8fe05796874cbbf`  
Candidate branch: `pa/wp-pa-04-knowledge-harvest`  
Worker: ChatGPT GPT-5.6 Sol  
Date: 2026-09-22

## Contract interpretation

PA-04 is a compact Juego2 adoption/revalidation of the independently reviewed donor study on knowledge, belief, ignorance and deception. It must not repeat broad prior-art research, import donor M9/M10/runtime architecture, or turn the donor PASS into automatic Juego2 authority.

The workpack owns the product-semantic epistemic boundary needed by later Living World consumers:

```text
canonical world truth != actor-accessible belief
missing belief/access != false
canonical truth mutation != automatic actor learning
```

Two actors in the same canonical world may therefore hold different justified beliefs and make different explainable decisions. Hidden/debug truth must not alter an actor-facing decision without a legitimate acquisition path.

The principal negative control is strict: changing only privileged/debug truth metadata that the actor cannot access must leave that actor's decision input unchanged.

## PREDECESSOR_CONTRACT_CHECK

Completed before any PA-04 result/implementation write. This Worker-plan commit is the first branch write.

### Accepted direct predecessor / exact evidence

Direct dependency: `WP-PA-03 — Social Graph That Changes Behaviour`.

- accepted candidate: `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927`;
- independent PASS review: `#5270421825`;
- implementation/research PR: `#100`;
- implementation merge: `2f3862b601b0521a6a3d5a57afe54f182d037e97`;
- DocSync PR: `#101`;
- DocSync candidate: `1b3005ac9118aae3043efa204c3d1d8127681f7d`;
- DocSync merge: `e388f6e3c9d42e89418bd8877ed36fcbcf6d2aaa`;
- canonical accepted finding: `Docs/research/living-world/results/PA-03.md`;
- completion evidence: `Docs/evidence/WP-PA-03/DOCSYNC.md` with `DOCSYNC_COMPLETE` and PA-04 named as the next dependency-valid workpack.

No accepted-contract capsule is relied on as authority in this check. The Worker opened the authoritative PA-03 accepted result, exact DocSync evidence and live merged PR identity directly because PA-04 materially consumes PA-03's private-third-party-relationship knowledge boundary.

### Inherited guarantees relevant to PA-04

PA-04 consumes as binding:

1. **Actor-owned decisions.** Knowledge/belief is an input to an actor-owned decision; it does not become a second chooser or an omniscient director.
2. **Authorized information only.** Canonical third-party relationship truth is not ambient actor knowledge. A private relation involving other actors requires a legitimate acquisition path before it can become an actor belief/input.
3. **Directed relationship semantics remain distinct.** A belief about a relationship does not rewrite the canonical PA-03 relationship edge or imply reciprocity.
4. **Receiver ownership remains intact.** Communication may create an epistemic input for a receiver, but the receiver still owns its optional response from receiver-authorized state.
5. **Bounded discovery remains binding.** A knowledge-backed action/target path may not hide global actor/fact enumeration behind a small result set.
6. **Explainable semantic inputs remain required.** Material decisions should be explainable in terms of named semantic inputs/reasons, not only opaque scores.
7. **Bounded current provenance vs biography.** PA-03 already keeps only bounded current relationship reasons; PA-04 must likewise avoid stealing PA-06 autobiographical memory/compaction.
8. **Shared player/NPC causal world.** Player and NPC actions may create legitimate information asymmetry through the same causal systems; no privileged player-only knowledge universe is required.

Accepted PA-01/02 guarantees are consumed transitively only where PA-03 exports them: routine is not the chooser; agency is actor-owned; discovery is bounded; information inputs are authorized; receiver choice remains receiver-owned.

### Guarantees newly owned by PA-04

PA-04 newly owns only the epistemic product constraints required by the workpack:

- strict separation between canonical truth and actor belief/access;
- ignorance/unknown as a first-class result rather than false or canonical fallback;
- legitimate acquisition categories such as perception, communication, accessed public sources and explicit authored/bounded inference seams;
- false, partial and stale belief that can persist until an explicit epistemic cause revises it;
- compact immediate provenance and acquisition/revision time sufficient for explanation without unbounded history;
- secrets/concealment/deception as access/acquisition effects rather than magic omniscient flags;
- actor-safe knowledge queries separated from privileged debug/truth comparison;
- shared knowledge state for actor decisions/dialogue rather than duplicated `knows_X` truth copies;
- two-actor same-world/different-belief counterfactual proof;
- hidden/debug-truth leakage negative fixture;
- player/NPC information asymmetry compatible with the playable causal city and intentional transformation amendments;
- future H4 consumer notes and explicit deferred empirical/runtime proof.

### Inherited guarantees intentionally consumed, not re-proved

This WP does not re-prove PA-02 actor-origin, PA-02 bounded target discovery, PA-03 directed relationship behaviour, PA-03 affect-dimension counterfactuals, receiver-response ownership, routine semantics or generic commitment/replanning behavior.

PA-04 exercises the new composition seam it owns: an actor-facing decision must use only epistemic state legitimately available to that actor, even when canonical world/relationship truth exists elsewhere.

### Concrete reopen condition

Reopen a predecessor guarantee only on concrete contradictory evidence that the accepted guarantee is false or inapplicable on the effective PA-04 path—for example, if the only viable knowledge-dependent target lookup necessarily begins with global population enumeration, if a private third-party relationship fact cannot be represented as an actor belief without granting ambient access to canonical PA-03 truth, or if receiver-owned decision semantics can only work by the sender/director selecting the receiver response.

A theoretical implementation possibility, future schema uncertainty or desire for duplicate proof is not a predecessor reopen condition.

## Donor provenance audited

- donor repo: `Arkus0/Juego`;
- frozen donor plan: `Docs/living-city-research/PA-04_PLAN_KNOWLEDGE.md`;
- donor dossier: `Docs/living-city-research/PA-04_KNOWLEDGE.md`;
- exact donor candidate: `672dcfa46dc1212d43f5302b4937ddd345bf249a`;
- donor PR: `#41`;
- donor final independent PASS: review `#5230673388` on candidate `672dcfa46dc1212d43f5302b4937ddd345bf249a`.

The donor PASS is provenance only. Juego2 requires its own independent review.

The donor study's useful research conclusion is retained only at product-semantic level: `WorldFact != ActorBelief`; actor-safe queries fail closed to `UNKNOWN`; false/stale beliefs require explicit revision causes; actor-visible truth/staleness labels are forbidden; immediate provenance is bounded; deception must be representable as receiver epistemic state; generic recursive Theory of Mind / LLM authority and global knowledge scans are rejected for baseline.

## Current Juego2 reconciliation

PA-04 must consume current Juego2 programme amendments rather than restoring donor-era product assumptions:

- **Playable causal city:** information asymmetry must create consequences the player can encounter/manipulate through the same world, not only NPC background flavour.
- **Intentional transformation:** sustained player actions may legitimately alter who knows what and what information sources remain available; the system must not silently resynchronize everyone to canonical truth in order to preserve the starting town.
- **No omniscience:** the PA track-wide negative gate directly reinforces PA-04.
- **No quest-script causality disguised as simulation:** player-triggered information changes must enter shared epistemic owners/paths rather than a privileged player-only truth flag.

PA-12 governance may later create public information or concealment conditions through explicit rules/resources/opportunities, but PA-04 does not own governance or ideology shortcuts.

## Execution

1. Reconstruct donor findings and independent review without copying donor runtime/API/schema authority.
2. Reconcile with accepted PA-01..03 and current Juego2 cross-cutting amendments.
3. Publish compact canonical `Docs/research/living-world/results/PA-04.md`.
4. Classify epistemic mechanisms as `ADOPT / ADAPT / LATER / REJECT` while remaining representation-neutral.
5. Define the required same-world/different-belief counterfactual and strict hidden/debug-truth leakage negative fixture.
6. Define acquisition/provenance/revision/secret/deception semantics while preserving PA-05 communication-propagation and PA-06 memory ownership.
7. Define actor-safe vs privileged tooling authority and the minimum explanation surface.
8. Keep runtime schema, confidence tuning, perception implementation, persistence mechanics, dialogue integration, UI and scale as deferred proof.
9. Perform strict Worker pre-review against the complete baseline→candidate diff while Draft; repair any in-claim defect before freeze.
10. Stop writers, bind exact HEAD as frozen candidate, publish canonical handoff metadata and wait for fresh independent review.

## Ownership guards

No runtime/Unity/code changes; no H0/H1/CITY changes; no edits to frozen PA plans/amendments; no donor auto-acceptance; no final belief DB/schema/API; no numerical confidence formula; no generic inference engine; no LLM epistemic authority; no PA-05 rumour motive/propagation/distortion/corroboration; no PA-06 autobiographical memory/forgetting/compaction; no PA-07+ ownership; no completion of PA-05+.

## Candidate surface

Only:

- `Docs/research/living-world/results/PA-04.md`;
- `Docs/evidence/WP-PA-04/*`.
