# WP-PA-02 — Worker pre-review

Workpack: `WP-PA-02`  
Candidate branch: `pa/wp-pa-02-npc-agency-harvest`  
Baseline: `main@c6e97600fa68afbe1de7fe46121880ffe8be917b`  
Repair cycle: `fail_cycle: 1`  
Transfer SHA: `0ab469b640535d92c73c617e0d944c0b1f45033f`  
Superseded independent FAIL: review `#5269143202` on `0ab469b640535d92c73c617e0d944c0b1f45033f`  
Worker verdict: **CLEAN / READY TO FREEZE**  
Date: 2026-09-21

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 0
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-PA-02/WORKER_PRE_REVIEW.md
INDEPENDENT_FAIL_BLOCKERS_REPAIRED: 1
PROOF_BUDGET_VERDICT: N/A — WP-PA-02 is RESEARCH / NON-FOUNDATIONAL
```

## Repair provenance / exact delta

Independent review `#5269143202` found one blocker: the initial cycle omitted the mandatory v1.7 `PREDECESSOR_CONTRACT_CHECK` and canonical handoff fields. The same review explicitly found the PA-02 research content strong against the contract and required no semantic redesign unless this fresh pre-review discovered a new defect.

Repair actions before this pre-review:

1. persisted the repair-cycle `PREDECESSOR_CONTRACT_CHECK` in `WORKER_PLAN.md`, binding exact accepted PA-01 evidence and the inherited/new/consumed/reopen split;
2. normalized `HANDOFF.md` with the predecessor evidence, repair history, transfer SHA and `fail_cycle`;
3. left `Docs/research/living-world/results/PA-02.md` unchanged.

The PA-02 result blob is `1218c3d896fef28b9293075b3ea6f99aed6025e1` both at failed candidate `0ab469b640535d92c73c617e0d944c0b1f45033f` and on the repaired branch before this pre-review evidence commit. Therefore the repair is process/evidence-only, not a semantic rewrite of the harvest.

Current `main` is `55d919ea6cea7784431b628343b1cde46f2e3863`. The only baseline→main drift after branch baseline `c6e97600fa68afbe1de7fe46121880ffe8be917b` remains PR `#95`, the unrelated H1 Quaternius Source timing/ownership documentation correction already recorded in the original handoff. It does not change PA-01, PA-02 workpacks, PA programme inputs or candidate paths.

## PREDECESSOR_CONTRACT_CHECK verification

The fresh pre-review re-read the persisted check against current accepted evidence.

| Check | Result |
|---|---|
| Direct dependency identified | PASS — `WP-PA-01` |
| Accepted candidate exact SHA | PASS — `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715` |
| Independent PASS bound | PASS — review `#5268013445` |
| Merge bound | PASS — PR `#89`, merge `3688b7b9a27355b0fda160c20e57a385e40c6814` |
| DocSync completion bound | PASS — PR `#91`, merge `50bbb95d261ff1964f2e920ce0380868eb2c59fd` |
| Completion metadata re-read | PASS — `Docs/workpacks/PA/WP-PA-01.md` is `COMPLETE` and names the accepted SHA/review/merge |
| Accepted result re-read | PASS — `Docs/research/living-world/results/PA-01.md` is `ACCEPTED / REVIEWED` |
| DocSync re-read | PASS — `Docs/evidence/WP-PA-01/DOCSYNC.md` is `DOCSYNC_COMPLETE` and names PA-02 next |
| Inherited guarantees stated | PASS |
| Newly PA-02-owned guarantees stated | PASS |
| Consumed-not-reproved guarantees stated | PASS |
| Concrete reopen condition stated | PASS |
| Timing represented honestly | PASS — check is explicitly repair-cycle evidence performed before repair branch writes; it does not claim to have existed in the failed initial cycle |

Relevant inherited PA-01 guarantees are consumed rather than duplicated: routine is expected semantic intent, routine is not agency, expected != actual, route realization does not rewrite schedule intent, interruption recovery re-evaluates current context, routine is a perturbable shared-world baseline, and PA-08/PA-09 retain their accepted ownership boundaries.

PA-02 owns the new agency seam only: actor-originated choice, bounded discovery, authorized inputs, explainable decisions, optional receiver-choice ownership, commitment/replanning/failure guardrails and player-independent continuation.

No concrete evidence was found that reopens PA-01.

## Contract / scope

| Check | Result |
|---|---|
| Exact `WP-PA-02` acceptance/DoD re-read | PASS |
| Accepted PA-01 prerequisite audited | PASS — exact reviewed/merge/DocSync evidence above |
| Exact donor dossier audited | PASS — `dfe8a2b10831774f846274143c58dc41227d6231` |
| Donor final PASS audited | PASS — PR #36 / review `#5225572191` |
| Donor failed-review repairs considered | PASS — fake boundedness, receiver ownership, certainty, population-axis, determinism and causal-axis defects preserved as regression targets |
| Complete baseline→candidate changed-file surface inspected | PASS — only `PA-02.md` + three `Docs/evidence/WP-PA-02/*` files |
| Broad prior-art re-research avoided | PASS |
| Donor runtime/milestone architecture stripped | PASS |
| Frozen plans/amendments untouched | PASS |
| PA-03+ ownership preserved | PASS |
| Runtime/Unity/tuning/performance claims deferred | PASS |
| Universal AI algorithm avoided | PASS |
| Foundational proof/content-shape probe obligations | N/A — research/non-foundational WP |

## Required deliverables

| Deliverable | Result |
|---|---|
| Canonical Juego2 PA-02 finding | PASS — `Docs/research/living-world/results/PA-02.md` |
| Mechanism disposition matrix | PASS — result §3 |
| Bounded discovery / anti-global-scan requirements | PASS — result §4 |
| Actor→actor positive scenario | PASS — P1 in result §9 |
| No-player-trigger negative/control scenario | PASS — NC-01 |
| Anti-global-scan causal negative control | PASS — NC-02, including >=10,000 irrelevant decoys |
| Receiver-ownership falsifier | PASS — NC-03 |
| Future H3/H4/H7 consumer notes | PASS — result §12 |
| Deferred empirical proof list | PASS — result §13 |
| PersistentActor-grade vs ambient distinction | PASS — result §11 without freezing donor profile taxonomy |

## Acceptance attack

The WP requires support for a deterministic bounded scenario where an NPC initiates a meaningful action for an explainable reason and affects another actor/world state without a privileged player trigger or global population scan.

Candidate proof shape:

```text
actor-accessible state / pressure / opportunity
 -> actor-originated problem or goal
 -> bounded action family + bounded target/opportunity source
 -> cheap eligibility
 -> explainable choice
 -> commitment / attempted action
 -> receiver-owned response when optional
 -> structured shared-world outcome
 -> bounded resolve/fail/replan
```

P1 instantiates that shape with ordinary coordination between Antonio and Manolo while the player is absent. The goal does not preselect `ASK`; at least two valid initiator alternatives exist. If the chosen social action has a meaningful response choice, Manolo owns it.

NC-01 fails if player/quest presence is required to produce the decision. NC-02 fails if a scoped query enumerates the global persistent-actor collection before filtering. Together they directly attack the two required acceptance dimensions.

The fresh repair pre-review specifically challenged whether the new predecessor check had accidentally weakened the PA-02 seam into “schedule causes action.” It does not: PA-01 schedule is consumed only as context/expected intent and PA-02 still requires an actor-owned problem/goal plus alternatives before the meaningful action is chosen.

## Bounded-discovery causality

The result does not merely say “filter before scoring”. It requires:

- an explicit bounded target/opportunity source before expensive eligibility/comparison;
- discovery work independent of unrelated global population growth for the tested scope;
- deterministic/explainable cap behavior;
- future instrumentation equivalent to enumeration, cheap-filter and comparison/scoring counters;
- no fallback to a global scan when the bounded source yields no candidate.

The >=10k-decoy control changes global population while preserving the relevant scope. A false implementation such as `AllPersistentActors -> Where(local)` therefore fails even if the final scored set stays tiny.

## Receiver ownership causality

The result distinguishes:

```text
Initiator decision: proposal/action/target + initiator reasons
        ↓
Receiver decision: eligible responses + receiver reasons + receiver choice
```

when the receiver genuinely has a choice. A future executor may apply an already-owned receiver result, but cannot select `ACCEPT/REFUSE/...` on the receiver's behalf and still claim independent receiver agency.

NC-03 changes only an authorized receiver input while keeping the initiator decision semantically fixed. If the receiver output cannot change independently, the claimed boundary is false.

## Juego2 delta / player-first audit

- **Shared causal city:** PASS. NPC autonomy uses structured outcomes compatible with the same consequence owners that PA-09 will later expose to player-originated actions; there is no NPC-only rich simulation requirement.
- **Player as cause, not puppet master:** PASS. Player-created circumstances may alter inputs/opportunities but do not directly select NPC responses.
- **Player absence:** PASS. P1 and NC-01 explicitly require autonomous operation with the player absent.
- **Positive/ordinary agency:** PASS. The primary proof uses coordination/help rather than requiring conflict/drama.
- **No omniscient player reaction:** PASS. Actor inputs must be authorized; PA-04/05 retain knowledge/information ownership.
- **Intentional transformation:** PASS. Durable changes may alter opportunity surfaces; PA-02 cannot restore starting state by fiat.
- **Governance:** PASS. Policy changes rules/resources/opportunities rather than minds; actors retain decision ownership.

## Donor-review regression check

The final donor PASS repaired defects that could create false-green agency. The candidate does not reintroduce them:

1. **hidden global scan:** bounded scope precedes filtering/scoring; NC-02 attacks 10k irrelevant decoys;
2. **fake receiver independence:** explicit receiver-owned semantic boundary + NC-03;
3. **certainty overclaim:** donor mechanisms are research evidence, while Juego2 runtime proof remains deferred;
4. **AmbientPopulation confusion:** persistent-grade agency and ambient presence are separate concepts; donor profile enum names are not frozen;
5. **deterministic global IDs:** result requires normalized semantic trace, not opaque ID equality;
6. **mixed causal axes:** initiator/source and domain/topic cause are kept distinct without freezing donor enum schema;
7. **fake agency through schedule/event:** NC-04 rejects a timetable/fixture that directly selects the claimed action;
8. **fake material lineage count:** donor `3+` is preserved as an optional stronger stress fixture, not imported as a universal constitutional law.

## Algorithm / architecture anti-smuggling check

The result deliberately does **not** choose:

- Utility AI;
- GOAP;
- rules/behavior trees;
- a planner;
- `BehaviourResolver`;
- an LLM authority;
- one fixed score schema;
- a specific target index/provider API;
- an exact GameFlow priority order;
- donor `ROUTINE/SOCIAL/ACTIVE` as required Juego2 enums/classes.

What is frozen at research level is only the observable product semantics: bounded discovery, authorized inputs, explainable choice, commitment/recovery, receiver ownership where applicable, structured consequence and player-independent initiation.

## PA ownership audit

- PA-01 retains schedule/routine semantics and its accepted guarantees are consumed, not re-proved.
- PA-03 retains relationship dimensions/sign/history.
- PA-04 retains belief acquisition/truth/ignorance.
- PA-05 retains information transfer.
- PA-06 retains durable memory.
- PA-07 retains work/material systems.
- PA-08 retains activity rules/outcomes.
- PA-09 retains generic player intervention semantics.
- PA-10..14 retain event ecology, legibility, governance, anti-chaos and integration.
- H3/H4/H7 retain runtime/API/schema/implementation ownership.

## Residual / reopen signals

Future consumers should reopen/falsify the relevant finding if implementation evidence shows, for example:

- useful target/opportunity discovery cannot be bounded without unacceptable content loss;
- receiver-owned choices cannot compose cleanly with action execution;
- commitment/replan guards make agents unresponsive or fail to prevent thrashing;
- deterministic semantic traces are impractical or misleading;
- FULL↔ABSTRACT reconciliation loses meaningful commitments/outcomes;
- player/NPC shared consequence semantics require incompatible ownership;
- target-scale authoring or runtime cost invalidates the assumed product shape;
- a concrete multi-step scenario demonstrates that a stronger planner architecture is actually unavoidable.

These are deferred proof points, not reasons to select an architecture in PA-02.

## Branch surface / repair containment

Complete candidate surface remains PA-local docs only:

- `Docs/research/living-world/results/PA-02.md` — unchanged by repair;
- `Docs/evidence/WP-PA-02/WORKER_PLAN.md` — predecessor check added;
- `Docs/evidence/WP-PA-02/HANDOFF.md` — repair/predecessor handoff normalized;
- `Docs/evidence/WP-PA-02/WORKER_PRE_REVIEW.md` — this fresh complete pre-review.

No workpack, amendment, runtime, H0/H1/CITY or implementation file is modified.

## Worker conclusion

**CLEAN / READY TO FREEZE.**

The independent FAIL's single process blocker is repaired. The exact accepted PA-01 predecessor contract is now reconstructed and persisted; inherited guarantees are consumed rather than defensively re-proved; the complete candidate has been re-challenged; `PA-02.md` remains unchanged; and no new in-claim blocker was found.
