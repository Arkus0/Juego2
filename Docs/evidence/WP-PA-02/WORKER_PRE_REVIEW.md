# WP-PA-02 — Worker pre-review

Workpack: `WP-PA-02`  
Candidate branch: `pa/wp-pa-02-npc-agency-harvest`  
Baseline: `main@c6e97600fa68afbe1de7fe46121880ffe8be917b`  
Worker verdict: **CLEAN / READY TO FREEZE**  
Date: 2026-09-21

## Contract / scope

| Check | Result |
|---|---|
| Accepted PA-01 prerequisite audited | PASS — accepted result + PA README show PA-01 COMPLETE |
| Exact donor dossier audited | PASS — `dfe8a2b10831774f846274143c58dc41227d6231` |
| Donor final PASS audited | PASS — PR #36 / review `#5225572191` |
| Donor failed-review repairs considered | PASS — fake boundedness, receiver ownership, certainty, population-axis, determinism and causal-axis defects preserved as regression targets |
| Broad prior-art re-research avoided | PASS |
| Donor runtime/milestone architecture stripped | PASS |
| Frozen plans/amendments untouched | PASS |
| PA-03+ ownership preserved | PASS |
| Runtime/Unity/tuning/performance claims deferred | PASS |
| Universal AI algorithm avoided | PASS |

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

- PA-01 retains schedule/routine semantics.
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

## Branch surface

Expected diff is PA-local docs only:

- `Docs/research/living-world/results/PA-02.md`;
- `Docs/evidence/WP-PA-02/WORKER_PLAN.md`;
- `Docs/evidence/WP-PA-02/WORKER_PRE_REVIEW.md`;
- `Docs/evidence/WP-PA-02/HANDOFF.md`.

No workpack, amendment, runtime, H0/H1/CITY or implementation file is intentionally modified.

## Worker conclusion

**CLEAN / READY TO FREEZE.**

The candidate satisfies the PA-02 harvest contract at research level, preserves the donor's difficult-to-fake boundedness/ownership lessons, reconciles them with Juego2's playable causal city, and leaves implementation/algorithm proof with future consumers.
