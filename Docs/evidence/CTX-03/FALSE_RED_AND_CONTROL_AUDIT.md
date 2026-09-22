# CTX-03 — False-red and causal control audit

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Goal

Move deterministic protocol defects left **without reducing false-green detection power or semantic Reviewer authority**. A red check must not be promoted to “the WP is wrong” unless a reviewed deterministic verifier causally owns that claim, but unknown/infra red state also must not be silently ignored to start review.

Repair cycle 1 additionally circuit-breaks the three classes raised by independent review `#5275757245`: exact-candidate pre-review ordering, effective mandatory-context growth, and retry-safe same-SHA terminal closure.

## Mechanical outcome contract

| Outcome | Meaning | WP defective? | May independent semantic review start? |
|---|---|---:|---:|
| `PASS` | registered deterministic condition satisfied | no | yes, subject to other terminal predicates |
| `FAIL` | registered deterministic verifier causally proves its owned candidate condition false | **yes, mechanically** | no; Worker repairs first |
| `REVIEW_BLOCKED` | lifecycle/handoff/freeze metadata is incomplete, pending, stale or incoherent | no semantic defect asserted | no |
| `NOT_APPLICABLE` | verifier does not apply to this candidate/mode | no | neutral |
| `INFRA_ERROR` | runner/API/tool/unregistered-red/unclassified failure gives no causal candidate defect proof | **no** | no until triaged/rerun |

This explicitly closes the false-red failure mode where a missing handoff field, a runner crash or an experimental check consumes an independent Reviewer FAIL round.

## Registry boundary

`Docs/engineering/mechanical-verifier-registry.json` is the reviewed mapping from GitHub check names to deterministic ownership/failure class.

- `Worker handoff lint` -> `REVIEW_BLOCKED`;
- `Freeze exact-SHA validation` -> `REVIEW_BLOCKED`;
- `CTX process envelope` -> structured causal result, allowed to emit mechanical `FAIL` for its exact process claim;
- `Review-ready terminal closure` -> `REVIEW_BLOCKED`.

An unregistered red check has `affects_wp_decision=false` and `counts_as_wp_fail=false`, but the classifier returns overall `INFRA_ERROR` so review remains fail-closed while the unknown red is triaged. A structured causal verifier that crashes or disagrees with its GitHub conclusion also becomes `INFRA_ERROR`, never a fabricated `FAIL`.

## Exact-candidate Worker pre-review ordering

The failed candidate `1421f1690f1bd20b578ba8f70ee8ee5deb90b67a` exposed a sequencing defect: the complete Worker pre-review covered parent `460f997c8e70720356f285583977530cc2175c76`, then a repository evidence file claiming final cleanliness was committed afterward. Exact-SHA CI on the child did not retroactively make the complete pre-review cover the child's bytes.

Repair cycle 1 removes that self-invalidating pattern:

1. all implementation and repository/evidence bytes are finalized and pushed while Draft + ACTIVE;
2. writers stop and the exact resulting HEAD is read;
3. the complete Worker pre-review inspects that exact HEAD and the full baseline→candidate diff;
4. only if clean, a durable GitHub PR issue comment is created containing the clean marker, exact Candidate SHA, findings count and evidence pointers;
5. `derive-worker-review-metadata.py` accepts that durable issue-comment URL as the final pre-review evidence pointer and rejects a repository-local final-clean pointer;
6. Ready/freeze metadata may then change without changing candidate bytes.

The final clean record is therefore durable but external to the Git tree. Any subsequent repository/evidence mutation changes HEAD and invalidates the clean result; the complete pre-review must be repeated before another freeze.

## Effective mandatory-context envelope

The original CTX-03 envelope bounded only `initial_reads`. That was insufficient because concrete routes can make repository-owned conditional sources mandatory. Repair cycle 1 separates two budget layers:

- **base profile** — canonical `initial_reads`;
- **route-effective** — checker-owned H1/CITY/PA minimum and escalated source sets, including route-forced conditional/capsule/non-compressible/authoritative sources.

The route universe and a minimum effective-required-source oracle live in `scripts/context-envelope-check.py`, not in the budget config under test. `context-envelope.json` supplies the reviewed numeric baseline/ceiling for every checker-owned route and both minimum/escalated modes; it cannot remove a route without `route_budget_universe_errors` turning red.

The circuit-breaker controls deliberately grow real repository sources across their applicable effective-route ceilings:

- H1 Worker: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`;
- H1 Reviewer: `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`;
- CITY Worker + Reviewer: `Docs/ROADMAP.md`;
- cumulative PA Worker + Reviewer escalation: `Docs/research/living-world/results/PA-03.md`.

Each must turn the route-effective envelope RED. The existing unrelated-100KB control remains GREEN because unrelated bytes are outside the derived effective set. Ceiling increases in either the base or route layer require an incremented revision plus explicit justification.

This is a class repair: conditionally mandatory H1/CITY/PA context can no longer grow outside every reviewed ceiling merely because it is absent from `initial_reads`.

## Retry-safe REVIEW_READY_CLOSED

The failed workflow woke only on creation of a new Automation V2 `REVIEW_READY` comment. Automation V2 deduplicates that marker by PR+SHA, so a first closure attempt blocked by a red/pending freeze gate could become permanently unreachable after a same-SHA metadata/gate correction.

After repair, `.github/workflows/review-ready-closure.yml` can wake from either:

1. the original bot `issue_comment` containing the durable `REVIEW_READY` marker; or
2. completion of `Arkus Candidate Validation`.

The workflow-run path resolves the exact PR and candidate SHA, searches the canonical PR comments for an **already-existing** Automation V2 `REVIEW_READY` marker targeting that same SHA, and then reruns the normal handoff/freeze/final-live-HEAD closure. Persistence remains idempotent by `review-ready-closed:<PR>:<SHA>`.

`scripts/review-ready-closure.py --self-test` reproduces the exact Reviewer case: same HEAD + same marker + freeze failure is blocked, then changing only the gate observation to SUCCESS on the same SHA turns GREEN without a second marker. `ctx03-process-controls.py` separately requires both workflow triggers and the idempotent CLOSED key to remain present.

No semantic authority is added. `REVIEW_READY_CLOSED` is lifecycle evidence only.

## Causal false-green controls preserved or strengthened

| Claim | Independent/derived universe | Causal negative control | Required result |
|---|---|---|---|
| base role context ceiling | effective `initial_reads` from accepted role profile | grow a real unconditional required source across base ceiling | `FAIL` |
| effective H1 foundational/local ceiling | checker-owned H1 routes + effective-required-source oracle | grow real foundational/local source across route ceiling | `FAIL` |
| effective CITY cross-track ceiling | checker-owned CITY routes | grow real `Docs/ROADMAP.md` across route ceiling | `FAIL` |
| effective PA escalated ceiling | checker-owned cumulative PA routes | grow canonical PA-03 result across escalated route ceiling | `FAIL` |
| unrelated repo growth should not false-red | derived base read set | add 100KB unrelated file | remains GREEN |
| ceiling increase reviewability | prior config from base ref | increase profile/route ceiling without revision + justification | `FAIL` |
| escalation completeness | accepted profile `must_escalate_if` | delete one real predicate from escalation record | `FAIL` |
| PA material-source reachability | production route generator + real PA capsule source selectors | remove canonical PA-03 result from compact route selectors | quality replay RED |
| CITY non-compressible source | production route generator + real CITY capsule mandatory read | remove `CITY_PRODUCT_SEED.md` mandatory read | quality replay RED |
| DocSync current-state consistency | accepted-state index + normal current-state docs | representative accepted CTX-02→CTX-03 reconciliation | contradictory/missing next state RED |
| history separation | accepted profile initial reads | any `Docs/history/**` normal initial read | RED |
| exact pre-review ordering | final Git HEAD vs durable external clean record | repository-local final-clean pointer rejected; any later byte mutation changes SHA | cannot false-bind parent review to child bytes |
| terminal REVIEW_READY transaction | live-state-shaped closure oracle | remove actual marker / move HEAD / break gate | `REVIEW_BLOCKED` |
| same-SHA terminal retry | existing marker + later Candidate Validation completion | freeze RED -> same-SHA freeze GREEN with same marker | blocked -> PASS |

The quality replay checks routing/discoverability only. It does **not** claim that a model will notice a semantic blocker once the source is open. Independent Reviewer reasoning remains mandatory.

## Historical corpus classification

`HISTORICAL_CLASSIFICATION.json` was re-derived from durable GitHub/repository evidence. The adopted mechanical families remain limited to deterministic causal conditions. Generic natural-language semantic equivalence is explicitly `REJECT` as a new mechanical gate.

The repair does not reinterpret the independent FAIL as infrastructure noise: all three reviewed blockers are treated as real current-WP process defects and repaired at their causal boundaries.

## Validation rule for the repaired candidate

Earlier GREEN runs belong to earlier candidate SHAs and remain historical evidence only. After **the last repository/evidence mutation**, the Worker must rerun the complete canonical CTX-03 surface on the exact resulting HEAD and then perform the complete Worker pre-review against that same HEAD before creating the external clean record.

Required surface:

```text
python3 scripts/context-envelope-check.py --self-test
python3 scripts/mechanical-verifier-classifier.py --self-test
python3 scripts/review-ready-closure.py --self-test
python3 scripts/derive-worker-review-metadata.py --self-test
python3 scripts/ctx03-quality-replay.py --self-test
python3 scripts/ctx03-process-controls.py --self-test
python3 scripts/ctx03-docsync-history-check.py --self-test
python3 scripts/context-envelope-check.py --audit --base-ref <BASE_SHA> --escalations Docs/evidence/CTX-03/CONTEXT_ESCALATIONS.json
python3 scripts/ctx03-quality-replay.py --negative-controls
python3 scripts/ctx03-process-controls.py
python3 scripts/ctx03-docsync-history-check.py
```

No historical green in this document is a substitute for that final exact-SHA rerun or fresh independent semantic review.
