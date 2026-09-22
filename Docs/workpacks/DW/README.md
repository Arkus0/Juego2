# DW — Design World Second-Consumer Validation

Status: PLANNED / NOT_STARTED — becomes binding only after this PROCESS_ONLY planning PR independently passes, merges and DocSync completes
Class: FOUNDATIONAL VALIDATION TRACK
Binding architecture when accepted: `Docs/engineering/DW_DESIGN_WORLD_ARCHITECTURE.md`
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

No `WP-DW-*` implementation Worker is active or authorized by the planning branch itself.

## Outcome

DW answers whether the accepted Arkus/H0 public model can support useful typed design/research consumers distinct from the game runtime without domain leakage, authority inversion or lossy context compression.

The track is deliberately bounded. It proves two real consumers — CITY and PA — and one measured structured-context use case. It does not convert the whole repository to Arkus, model the workpack/review process, implement design↔Unity drift, or claim arbitrary-domain universality.

## Why now

H0 is accepted and H1 is actively proving the Unity bridge. H2 has not yet frozen an external product boundary. This is the cheapest point to expose the kernel/API to a materially different consumer and discover whether a future public boundary would accidentally overfit runtime/Unity use.

The project also already owns mechanically testable design rules and a growing PA corpus. Those are useful consumers in their own right: success improves the project while pressure-testing Arkus.

## DAG

```text
WP-HK-GATE
    |
    v
 DW-00 authority/projection contract
    |
    v
 DW-01 CITY invariant vertical slice
    |
    v
 DW-02 CITY production queries/content-shape projection
    |
    +-----------------------------+
    |                             |
    | requires PA-01..05 accepted |
    v                             |
 DW-03 PA typed corpus/provenance |
    |                             |
    | requires CTX-03 accepted    |
    v                             |
 DW-04 paired structured-context agent quality/token trial
    |
    v
 DW-05 second-domain/generalization stress + residual closure
    |
    v
 DW-GATE
    |
    +---- planning input / interlock ----> final H2 external-boundary acceptance

H1-02 ... H1-GATE proceed independently in parallel.
```

The PA and CTX prerequisites are cross-track evidence prerequisites only. DW does not take ownership of PA research or CTX process policy.

## Sequence summary

| Order | Workpack | Central claim | Execution |
|---:|---|---|---|
| 1 | `WP-DW-00` | authority-preserving, rebuildable typed Design World projection over public Arkus surfaces | `REMOTE_OK` |
| 2 | `WP-DW-01` | a bounded accepted CITY slice can produce causal mechanical FAILs without CITY semantics entering H0 | `REMOTE_OK` |
| 3 | `WP-DW-02` | the CITY projection is useful for real deterministic queries/content-shape measurements with complete provenance | `REMOTE_OK` |
| 4 | `WP-DW-03` | accepted PA findings/evidence/dispositions/fixtures can be projected and queried without lossy composition | `REMOTE_OK` |
| 5 | `WP-DW-04` | structured retrieval can reduce context on a pre-tuning frozen task universe while paired same-config agents preserve required task/review facts, blockers and verdicts under deterministic scoring | `REMOTE_OK` |
| 6 | `WP-DW-05` | the combined CITY+PA evidence supports a truthful generic-boundary assessment and closes/routes residuals without hidden H0 changes | `REMOTE_OK` |
| 7 | `WP-DW-GATE` | composed second-consumer readiness and explicit H2 planning consequence | `REMOTE_OK` |

All seven are foundational because a false PASS could cause H2 to freeze a public boundary around an overfit or lossy abstraction. Exact-SHA review, causal negative controls and independently justified completeness universes therefore apply.

## Split review

| Boundary | Why it stays separate |
|---|---|
| DW-00 vs DW-01 | a sound authority/projection contract can pass while the first real domain cannot express or validate its rules through it |
| DW-01 vs DW-02 | one causal CITY invariant can pass while the projection remains incomplete or useless for production queries/budgets |
| DW-02 vs DW-03 | CITY spatial/programme data and PA research/evidence composition stress different semantics and failure modes |
| DW-03 vs DW-04 | a faithful typed corpus can pass while context selection over it omits facts or preserves a context package that an actual Worker/Reviewer still fails to use correctly |
| DW-04 vs DW-05 | a successful measured retrieval trial does not itself prove the generic API stayed domain-neutral or that all discovered residuals were correctly routed |
| DW-05 vs GATE | generalization/residual analysis is an input; the gate owns only final composition, completeness and H2 consequence |

Splitting any invariant, entity family or query into its own WP would create micro-WPs. Merging the boundaries above would give one Reviewer independently rejectable claims.

## Track-wide authority rules

- accepted CITY and PA documents remain semantic authority for their facts;
- DW state is derived/rebuildable and cannot silently become canonical design authority;
- every proof-relevant projected fact retains source provenance and staleness/ambiguity is fail-closed;
- H0 remains generic: CITY/PA vocabulary belongs to provider/domain layers;
- no accepted H0 guarantee is silently weakened or rewritten to make DW work;
- a genuine generic contradiction/deficiency stops the causal DW claim and is routed to an explicit reviewed delta/reopen decision;
- the repository Worker/Reviewer/freeze/DocSync/residual process is excluded from DW v1;
- creative/semantic judgement is not converted into a mechanical invariant without a separately reviewed deterministic oracle.

## Track-wide proof rules

- deterministic expected manifests/scorers are authoritative; model-generated prose is never allowed to judge itself;
- DW-04 additionally requires actual paired agent executions for every selected task, under the same declared model/configuration, because context-package completeness alone cannot prove preserved Worker/Reviewer quality;
- DW-04's eligible task universe/selection rule or exact reviewed manifest, task prompts, expected facts/blockers/verdicts and scoring rules are frozen before route-specific tuning/results so the trial cannot choose its own success universe;
- each completeness-sensitive proof uses an independent/effective universe rather than a registry proving itself;
- every material guarantee has at least one causal negative control that changes the effective claimed thing and must turn proof RED;
- projection rebuild from identical authority anchors and schema/rule version must produce equal normalized facts/relations/results;
- source provenance must be inspectable from compact records and remain sufficient to open the accepted authority when semantic depth is required;
- token/context efficiency is measured only after correctness; any demonstrated lost required fact/blocker/verdict in structural or paired-agent scoring fails DW-04 regardless of savings;
- no universal token-saving percentage is claimed from a bounded trial;
- no full H0 suite is repeated per WP: delta proof covers the public seam used by the current claim.

## Cross-track interlocks

### H1

H1 remains unchanged and proceeds independently. DW does not use Unity as its first proof surface and does not delay H1-02..GATE.

Later design↔Unity drift is explicitly deferred until H1 owns normalized effective Unity observation/reconciliation. DW-GATE may preserve that as a downstream candidate; it does not pull H1-09 semantics into DW.

### CITY

DW consumes accepted CITY facts only. It cannot redefine the retained seed, greybox, keeper realization or CITY ownership. `WP-DW-01/02` use a bounded design-data slice, not CITY-04 Unity evidence.

### PA

`WP-DW-03` begins only after PA-01..05 are accepted in Juego2 so the initial typed corpus is based on accepted local truth. Later PA research remains PA-owned. DW may index newly accepted outputs but cannot pre-accept them.

### CTX

CTX remains owner of role/context process policy. `WP-DW-04` waits for CTX-03 so its baseline route is the accepted context-efficient process rather than an obsolete strawman. DW then tests whether structured retrieval can improve on that baseline without structural or actual paired-agent quality loss.

### H2

DW does not authorize H2 implementation. `Docs/ROADMAP.md` is the top-level milestone/gate authority and now records the same conditional interlock as this track: if this plan is accepted, final H2 public/external-boundary acceptance must explicitly consume accepted DW-GATE evidence or review and disposition the DW interlock. H1-GATE remains the Unity/gameplay prerequisite and DW does not replace it.

## Planned downstream opportunities — not active scope

A PASS may justify later work for:

- design ↔ Unity semantic drift checks;
- generated art/content briefs;
- catalogue coverage/content-production planning;
- replay-backed QA fixtures;
- narrative knowledge-boundary validation; and
- external non-game product experiments.

The plan intentionally gives none of these an implementation WP now. Their correct owners depend on accepted H1/H2 capabilities and evidence at that future point.

## Pre-mortem result

The plan is designed against the following predictable false-success modes:

1. **authority inversion** — projection/index becomes easier to edit than sources and silently becomes truth;
2. **domain leakage** — H0 gains CITY/PA concepts to make the demo convenient;
3. **self-shrinking completeness** — only projected rows are checked, so missing source facts disappear from the proof universe;
4. **non-causal controls** — tests prove a row was deleted but never require validation/retrieval to fail;
5. **lossy PA compression** — dispositions, negative findings, fixtures or exceptions disappear from compact context;
6. **token vanity metric** — smaller input is called success despite a missed blocker/fact;
7. **semantic substitution** — a complete context package is called preserved agent quality without requiring paired actual Worker/Reviewer-like executions;
8. **friendly-task self-selection** — the trial chooses or edits its success universe after seeing/tuning DW results;
9. **LLM-as-oracle** — subjective model output substitutes for a deterministic external scorer;
10. **process circularity** — Arkus becomes load-bearing for the protocol that validates Arkus;
11. **H2 premature freeze** — the external/public boundary is fixed before second-consumer evidence can influence it; and
12. **scope explosion** — attractive Unity/QA/art/legal/enterprise ideas are pulled into the validation track before the core claim passes.

Every workpack below owns a subset of these risks explicitly.

## Start rule

After this planning PR independently PASSes, merges and DocSync completes, `WP-DW-00` is the sole default DW start. No later DW workpack is implicitly authorized. H1, PA and CTX continue under their own accepted DAGs.
