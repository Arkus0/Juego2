# CTX-03 — False-red and causal control audit

Status: **CANDIDATE EVIDENCE / PROCESS_ONLY**

## Goal

Move deterministic protocol defects left **without reducing false-green detection power or semantic Reviewer authority**. A red check must not be promoted to “the WP is wrong” unless a reviewed deterministic verifier causally owns that claim, but unknown/infra red state also must not be silently ignored to start review.

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

An unregistered red check has `affects_wp_decision=false` and `counts_as_wp_fail=false`, but the classifier returns overall `INFRA_ERROR` so review remains fail-closed while the unknown red is triaged. This is deliberately stricter than either extreme: it neither blames the WP nor ignores unexplained red state.

A structured causal verifier that crashes or disagrees with its GitHub conclusion also becomes `INFRA_ERROR`, never a fabricated `FAIL`.

## Derivable metadata moved before Reviewer

`scripts/derive-worker-review-metadata.py` can generate only fields already determined after final bytes are CLEAN and HEAD is known: contract/evidence pointers, Candidate/Frozen SHA equality, frozen/Ready Worker state, pending Reviewer fields and unchanged fail cycle.

It refuses to generate a handoff unless:

- `PREDECESSOR_CONTRACT_CHECK` exists;
- exact Worker pre-review evidence contains `WORKER_PRE_REVIEW: CLEAN`;
- exact 40-char HEAD is supplied.

It cannot invent CLEAN, choose Worker ownership, reset `fail_cycle` or generate a Reviewer verdict. The existing independent handoff lint still validates the result. This moves transcription errors left without creating a self-certifying handoff.

Historical motivation: review `#5273364796` on CTX-01 found frozen candidate bytes unchanged but canonical Ready metadata/process mode missing and Ready gates red. Under CTX-03 this family is `REVIEW_BLOCKED`; metadata can be derived/fixed and gates rerun without spending a semantic FAIL round.

## REVIEW_READY_CLOSED removes a race; it adds no ceremony

The pre-CTX-03 lifecycle could observe GREEN Ready checks and still lose the actual Automation V2 transition or race a later HEAD movement. CTX-03 preserves the required durable `REVIEW_READY` marker and adds an automatic **post-marker observation**:

1. existing Automation V2 posts `State: REVIEW_READY` for exact frozen SHA;
2. repository-owned `issue_comment` workflow receives that marker automatically;
3. workflow re-reads canonical PR/handoff and prerequisite checks;
4. it performs a live HEAD read after marker observation;
5. immediately before persistence it performs one more race check;
6. only then it posts `State: REVIEW_READY_CLOSED` for the same SHA.

No Worker/human posts a second marker, clicks a second Ready action, re-enters metadata or waits for a new semantic approval. `REVIEW_READY_CLOSED` is durable evidence that the already-required terminal ordering was observed, not a new semantic gate.

For the CTX-03 adoption candidate itself the new workflow is not yet on default `main`, so this one candidate uses the exact equivalent already required by its frozen plan: observe the existing real `REVIEW_READY` marker, then perform a final live PR HEAD read without changing repository bytes. Future candidates use the automatic CLOSED projection.

Negative closure controls in `scripts/review-ready-closure.py` prove:

- CLEAN while PR is Draft remains blocked;
- missing/stale Frozen SHA remains blocked;
- handoff lint RED/pending remains blocked;
- freeze validation RED/pending remains blocked;
- every prerequisite gate GREEN but no durable matching `REVIEW_READY` remains blocked;
- wrong-SHA marker remains blocked;
- post-marker HEAD movement remains blocked;
- representative CTX-01 incomplete handoff remains blocked;
- representative CTX-02 missing-predecessor/handoff-lint family remains blocked.

Malformed closure input returns `INFRA_ERROR`; a well-formed but incomplete terminal state returns `REVIEW_BLOCKED`.

## Causal false-green controls preserved or strengthened

| Claim | Independent/derived universe | Causal negative control | Required result |
|---|---|---|---|
| role context ceiling | effective `initial_reads` from accepted role profile | grow a real required source across ceiling | `FAIL` |
| unrelated repo growth should not false-red | same derived read set | add 100KB unrelated file | remains GREEN |
| ceiling increase reviewability | prior config from base ref | increase ceiling without revision + justification | `FAIL` |
| escalation completeness | accepted profile `must_escalate_if` | delete one real predicate from escalation record | `FAIL` |
| PA material-source reachability | production route generator + real PA capsule source selectors | remove canonical PA-03 result from compact route selectors | quality replay RED |
| CITY non-compressible source | production route generator + real CITY capsule mandatory read | remove `CITY_PRODUCT_SEED.md` mandatory read | quality replay RED |
| DocSync current-state consistency | accepted-state index + normal current-state docs | representative accepted CTX-02→CTX-03 reconciliation | contradictory/missing next state RED |
| history separation | accepted profile initial reads | any `Docs/history/**` normal initial read | RED |
| terminal REVIEW_READY transaction | live-state-shaped closure oracle | remove actual marker / move HEAD / break gate | `REVIEW_BLOCKED` |

The quality replay checks routing/discoverability only. It does **not** claim that a model will notice a semantic blocker once the source is open. Independent Reviewer reasoning remains mandatory.

## Historical corpus classification

`HISTORICAL_CLASSIFICATION.json` was re-derived from durable GitHub/repository evidence. The adopted mechanical families are limited to:

- canonical/derivable Ready metadata and handoff coherence;
- durable `REVIEW_READY` plus post-marker live-HEAD closure.

Known semantic/mixed CTX-02 failure families remain owned by accepted CTX-02 causal controls or independent review. Generic natural-language semantic equivalence is explicitly `REJECT` as a new mechanical gate.

## Exact observed run

On candidate `0be2a5cdf97047e11561d71944919c9d27850bc3`, run `35701788029` produced:

```text
context-envelope self-test: PASS
mechanical-verifier-classifier self-test: PASS
review-ready-closure self-test: PASS
derive-worker-review-metadata self-test: PASS
ctx03-quality-replay self-test: PASS
ctx03-process-controls self-test: PASS
ctx03-docsync-history self-test: PASS
CTX_PROCESS_ENVELOPE_OUTCOME: PASS
CTX03_QUALITY_REPLAY: PASS
CTX03_PROCESS_CONTROLS: PASS
CTX03_DOCSYNC_HISTORY: PASS
mechanical decision: PASS
semantic_review_still_required: true
wp_failed_mechanically: false
```

Subsequent Worker pre-review must repeat this suite on the final evidence-bearing SHA. No green in this document is a substitute for that final exact-SHA rerun or independent semantic review.
