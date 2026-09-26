# WP-PA-B1 — PREDECESSOR_CONTRACT_CHECK

Worker state: `ACTIVE_WORKER`
Baseline: `main@caacfbf2706aa4c41c076dfe5e4f58a6c35ce7e0`
Execution class: `RESEARCH_BATCH`
Date: 2026-09-26

## Dependency identity

`WP-PA-B1` consumes the accepted PA-01..06 research spine and the still-binding unit contracts `WP-PA-07`, `WP-PA-08`, and `WP-PA-09`.

| Predecessor | Accepted candidate | Independent PASS | Merge / accepted lineage |
|---|---|---|---|
| PA-01 | `87c4cbe81f8195770eb23ba7f2a5d5ca2a237715` | `#5268013445` | PR #89 / `3688b7b9a27355b0fda160c20e57a385e40c6814` |
| PA-02 | `015bb28ddc9facc46459c2d1dd87a89740b6c9ef` | `#5270038879` | PR #97 / `85d23489a6478da6bc9f33c3f017640e46ab05e9` |
| PA-03 | `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927` | `#5270421825` | PR #100 / `2f3862b601b0521a6a3d5a57afe54f182d037e97` |
| PA-04 | `5d38ea38b983cd5227f57afa1d880d24746f9249` | `#5280879115` | PR #129 / `d6041b719292f24c4481dea28727e2cfd5f7ed5b` |
| PA-05 | `99890f1af10691ef7e38f8722830dd0f66529665` | `#5281462911` | PR #132 / `31f8258cf2873e9080d9dacad2cfe956f0e2fa2e` |
| PA-06 corrected | `9ce17952e257587c0dcec5dc4b854170365545ca` | `#5300025658` | repair PR #172 / `fdfec568844bc0d006959c3cd207795544eca33d` |

The PA track README on the baseline records PA-01..06 as accepted and names `WP-PA-B1` as the current next PA workpack. The batch amendment preserves the three unit result artefacts and requires one batch review only after all three unit oracles are satisfied.

## Authoritative escalations used

The accepted result artefacts for PA-01..06 were opened directly because B1 composes their ownership boundaries. No predecessor implementation/runtime proof is being re-run.

PA-07's required donor preregistration is not copied into Juego2. It was opened read-only from:

- repository: `Arkus0/Juego`;
- branch: `master`;
- path: `Docs/living-city-research/PA-07_PLAN_ECONOMY_WORK.md`;
- plan-declared preregistration baseline: `c55b0486ba05347638a3c83ada1f6b0fdf618b2a`.

The donor is research input only. Its old milestone/runtime destinations are not Juego2 authority.

PA-08 and PA-09 consume their frozen Juego2 plans plus the PA-09 embodied-actions amendment. The two cross-cutting Living World amendments remain product constraints, not runtime architecture.

## Inherited guarantees consumed

1. **PA-01 — routine:** normal schedule is semantic expected intent; actual execution may deviate when opportunity/capacity/material state changes; interruption re-evaluates current context. Routine is not actor agency.
2. **PA-02 — agency:** actors own their decisions from bounded, authorized opportunity/state; receiver owns optional responses; failure/replanning is explicit. A work/activity/player system may expose causes and opportunities but may not choose the actor's later action for it.
3. **PA-03 — relationships:** directed trust/affinity/fear, structural ties and bounded obligations are causal inputs owned by PA-03; downstream systems may propose relationship-relevant outcomes but may not maintain a parallel social score.
4. **PA-04 — knowledge/belief:** canonical world truth is not actor belief. Actors may use only legitimately acquired information; missing knowledge remains unknown.
5. **PA-05 — information flow:** communication is an explicit sender→receiver opportunity/action; engine/debug lineage is not actor knowledge and information does not globally broadcast.
6. **PA-06 — memory:** selected actor-accessible experience can become a bounded causal witness, under finite per-actor capacity and causal-time selection. Memory does not own current truth, belief, relationship or action choice.

## Guarantees newly owned by B1

### PA-07

Define the minimum work/service/material state that makes staffing, opening, capacity and a small number of explicit dependencies alter human opportunities and at least two later actor choices. Reject hidden macroeconomy, decorative jobs, instant replacement and material variables with no visible behavioural consequence.

### PA-08

Define the minimum activity-integration boundary so an activity can exist without player activation, consume real time/place/capacity, accept/refuse/interruption through existing owners, and emit a small structured outcome without becoming authority over schedules, relationships, beliefs or memories. Integration must not rescue a bad minigame.

### PA-09

Define how player-originated semantic actions enter the same material/activity/consequence universe as semantically equivalent NPC actions, including useful embodied/non-dialogue intervention. Consequences must continue after player departure without private quest flags or omniscient reactions.

### Cross-unit composition

B1 owns the consistency rule:

```text
shared material/service opportunity (PA-07)
    -> actor-owned decision (PA-02)
    -> activity opportunity/session/outcome when applicable (PA-08)
    -> player or NPC semantic intervention uses the same owned state (PA-09)
    -> normal downstream owners consume bounded causes
```

No PA-07/08/09 private causality universe is permitted.

## Guarantees intentionally not re-proved

- schedule semantics and actor decision algorithm;
- relationship, belief, communication and memory internals;
- CITY geography;
- Unity/runtime implementation, save format, performance, numerical tuning or H2 character behaviour;
- autonomous event orchestration, investigation UX and governance semantics owned by later PA units.

## Concrete reopen conditions

An inherited guarantee is reopened only if B1 produces concrete evidence that the accepted guarantee is false or inapplicable to the exact composed path. Examples: a necessary work/activity decision cannot be expressed without the work system choosing on behalf of PA-02; an accepted information boundary cannot represent an essential witness case; or bounded PA-06 memory semantics make a required in-scope consequence impossible. Mere desire for a richer model or another defensive layer is not a reopen condition.

`PREDECESSOR_CONTRACT_CHECK: PASS`
