# Context Envelope v1

Status: **CTX-03 CANDIDATE / PROCESS_ONLY**

## Purpose

Close the CTX programme by measuring context savings and bounding future mandatory-context growth **without reducing semantic/proof quality**. This protocol adds derived process measurements, auditable escalation, deterministic lifecycle closure and mechanical false-red classification. It does not move product/proof authority into compact context or automation.

Machine/evidence surfaces:

- role source: `Docs/engineering/context-bootstrap-profiles.json`;
- envelope/measurement config: `Docs/engineering/context-envelope.json`;
- verifier registry: `Docs/engineering/mechanical-verifier-registry.json`;
- context checker: `scripts/context-envelope-check.py`;
- independent envelope negative controls: `scripts/ctx03-process-controls.py`;
- quality-preservation replay: `scripts/ctx03-quality-replay.py`;
- DocSync/history consistency control: `scripts/ctx03-docsync-history-check.py`;
- mechanical outcome classifier: `scripts/mechanical-verifier-classifier.py`;
- review metadata generator: `scripts/derive-worker-review-metadata.py`;
- post-marker closure oracle: `scripts/review-ready-closure.py`;
- historical classification: `Docs/evidence/CTX-03/HISTORICAL_CLASSIFICATION.json`;
- non-bootstrap history: `Docs/history/CTX_PROCESS_HISTORY.md` + `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md`;
- CI: `.github/workflows/context-envelope-validation.yml`;
- automatic post-marker closure after adoption: `.github/workflows/review-ready-closure.yml`.

All are process/navigation evidence only. Exact workpacks, authoritative sources, code/tests, live GitHub and independent Reviewer judgment keep their existing authority.

## 1. Quality invariant

CTX-03 is successful only when a compact route preserves the same ability to discover, reconstruct and challenge material sources. Token/context savings never justify:

- omitting an authoritative source that a material claim requires;
- converting a derived table/capsule/index into semantic or proof authority;
- suppressing a mandatory escalation;
- replacing independent semantic review with a mechanical PASS;
- weakening accepted CTX-02 omission/substitution/source-consistency controls.

Historical-blocker replay therefore tests **routing and causal reachability**, not whether a particular model happens to notice every semantic problem.

## 2. Same-snapshot measurement

The estimator is deliberately simple and reproducible: `ceil(UTF-8 bytes / 4)` for each unique repository source. This is a provider-neutral context estimate, not a claim about an exact vendor tokenizer.

Provider-token uncertainty is declared separately in `context-envelope.json`. A saving is called **material** only when:

```text
pre_estimate - post_estimate > pre_uncertainty + post_uncertainty
```

Pre/post bytes are always read from the **same checked-out candidate snapshot**. Repository/history growth therefore cannot masquerade as routing improvement.

The representative route universe, canonical profile source, calibration substitutions and pre-CTX direct-predecessor source universe are checker-owned. `context-envelope.json` repeats them only as reviewed assertions. The artifact being measured therefore cannot narrow its own baseline or choose a friendlier route after seeing the result.

Dynamic mandatory inputs whose size CTX does not control — live GitHub state, exact PR diff, current Reviewer FAIL, local-execution manifest-named files — remain mandatory but are held outside the static corpus delta. They may not be silently omitted to improve the number.

H1 sources that remain binding because the WP is foundational/local are held constant in both routes. CITY-04's unresolved cross-track gate forces ROADMAP into the post route. The cumulative PA route includes all currently accepted PA capsules.

The exact candidate measurement is persisted in Worker evidence. The result is intentionally non-uniform: PA has a demonstrated material minimum-route reduction; H1's reduction does not clear the conservative uncertainty threshold; CITY is larger because its exact non-compressible seed and unresolved gate remain mandatory. CTX-03 records those facts rather than tuning the universe until every route appears cheaper.

## 3. Auditable context escalation

A `CONTEXT_ESCALATIONS` record evaluates every `must_escalate_if` predicate of the effective role profile. A triggered predicate must name at least one existing authoritative source opened because of it. A non-triggered predicate must record why it is non-material.

The checker independently obtains the required predicate universe from the canonical accepted role profile. The escalation record therefore cannot define its own completeness by deleting a predicate from itself.

`CONTEXT_ESCALATIONS` is navigation/audit evidence only. It may not prove a semantic claim or prevent a later Worker/Reviewer from deepening further.

## 4. Cumulative PA decision

CTX-03 measures PA as a cumulative consumer rather than a direct-predecessor happy path. Accepted CTX-02 already provides one capsule per accepted PA result, completion-side discovery, exact disposition status preservation and fail-closed source reconstruction.

The hardened same-snapshot measurement shows the PA worker/reviewer minimum route falling from `38389` to `22855`, a saving of `15534` / `40.4647%`, which clears the deliberately conservative combined uncertainty threshold. When material escalation opens the authoritative PA-01..03 results, the route becomes `43067`, intentionally larger than pre-CTX. That is correct: escalation buys source fidelity, not token savings.

Decision: **KEEP CTX-02 CAPSULE CHAIN / DO NOT ADD A SECOND PA COMPACT REGISTRY IN CTX-03**. CTX-02 materially closes the dominant cumulative starting-cost problem, and the CTX-03 quality replay proves the authoritative PA-03 result remains reachable on material escalation. Adding another registry would duplicate compact authority without demonstrated need.

## 5. CI-only process envelope

The process envelope is derived from the accepted `initial_reads` of the canonical role-profile source; the config supplies checker-bound placeholder assertions used for calibration. It does **not** choose or duplicate the source universe.

For every profile:

```text
reviewed ceiling = ceil(calibrated post-CTX baseline * (1 + reviewed headroom))
```

The initial headroom is 20%. The budget file is CI-only and is not a normal Worker/Reviewer read.

Unrelated repository growth outside the derived read set does not affect the estimate. Growth of a required source does. A future ceiling increase must be an explicit diff with both an incremented `ceiling_revision` and non-empty `ceiling_increase_justification`.

Independent controls attack route removal, profile removal, profile-source redirection, calibration-placeholder redirection, real required-source growth and mandatory-escalation omission. The measured artifact cannot choose its own completeness universe.

## 6. Mechanical false-red audit

CTX-03 explicitly separates mechanical outcomes so protocol/runner noise does not consume independent Reviewer rounds.

- `FAIL` — a **registered deterministic verifier** causally proved a repository-owned condition required by this WP is false. Worker repair before review; never an automated semantic Reviewer verdict.
- `REVIEW_BLOCKED` — handoff/freeze/metadata/lifecycle state is incomplete, pending, stale or incoherent. Fix/derive it and rerun before Reviewer.
- `NOT_APPLICABLE` — verifier does not apply. Neutral; never synthetic GREEN proof and never FAIL.
- `INFRA_ERROR` — runner/tool/API/checkout or unknown/unclassified verifier failed without causal candidate-defect evidence. Fail closed operationally, but do not label the WP defective.
- `PASS` — registered deterministic condition satisfied. Semantic independent review remains mandatory.

The canonical registry is `mechanical-verifier-registry.json`. A red check not registered there cannot count as WP FAIL; it becomes operational `INFRA_ERROR` until triaged or explicitly registered. Any registered verifier capable of emitting WP `FAIL` must require a structured outcome and may not claim semantic authority. A crash therefore cannot become a candidate FAIL merely because GitHub paints a check red.

## 7. Derivable review metadata

`derive-worker-review-metadata.py` derives only fields mechanically fixed once final bytes are CLEAN and HEAD is known. It preserves non-derivable lineage (`Baseline SHA`, Worker identity/history, Transfer SHA, prior reviewed SHA, `fail_cycle`) from the existing canonical handoff instead of accepting caller-selected replacements.

The generator refuses to operate unless the repository-local predecessor check and `WORKER_PRE_REVIEW: CLEAN` evidence exist. It cannot generate CLEAN, choose ownership, reset repair history or produce a Reviewer verdict. The existing independent `validate-worker-handoff.py` remains the oracle.

## 8. Transactional REVIEW_READY closure

A Worker may tell the human to start a Reviewer only after:

1. final repository/evidence byte mutation precedes final complete Worker pre-review;
2. exact evidence says `WORKER_PRE_REVIEW: CLEAN`;
3. Candidate HEAD and Frozen candidate SHA equal live Ready PR HEAD;
4. canonical Ready metadata is coherent;
5. registered `Worker handoff lint` is PASS;
6. registered `Freeze exact-SHA validation` is PASS;
7. durable Automation V2 `State: REVIEW_READY` targets the same frozen SHA;
8. **after observing that marker**, a final live PR HEAD read still equals that SHA.

After CTX-03 adoption, the `issue_comment` workflow performs steps 7→8 automatically and persists `State: REVIEW_READY_CLOSED` for the same SHA. CLOSED is only durable evidence that the already-required terminal invariant was observed. It adds **no human action**: no second comment, button, metadata entry or semantic approval.

For the CTX-03 adoption candidate itself, that new workflow is not yet on default `main`, so it cannot bootstrap its own event. This candidate satisfies the same invariant directly: existing Automation V2 must persist `REVIEW_READY`, then the Worker performs/records the final live HEAD read without mutating repository bytes.

## 9. Historical handoff controls

The closure oracle reproduces:

- CTX-01 frozen bytes with incomplete canonical Ready metadata/check state;
- CTX-02 handoff-lint/predecessor-check incompleteness;
- every prerequisite check GREEN but no matching durable `REVIEW_READY`;
- wrong-SHA marker;
- post-marker HEAD movement.

In each case CLEAN alone is insufficient. `HISTORICAL_CLASSIFICATION.json` separately classifies reconstructed FAIL/handoff families as mechanical, semantic or mixed and records `ADOPT / DEFER / REJECT`. Generic natural-language semantic equivalence is explicitly rejected as a deterministic gate.

## 10. Structured evidence, DocSync and history

CTX-03 uses machine-readable rows for naturally tabular process facts and keeps causal reasoning as prose where the reasoning itself is evidence.

After adoption, DocSync regenerates the one compact `ACCEPTED_STATE_INDEX.json` projection from authoritative sources, then updates only current-state docs whose effective accepted meaning changed. It does not copy the same transition into several current-state narratives merely to preserve chronology. Accepted closure chronology lives under exact evidence or `Docs/history/**`, deliberately outside normal role bootstrap.

ROADMAP v1.33 demonstrates the separation: verbose H0 accepted PR/review/action closure narrative moved to `Docs/history/ROADMAP_ACCEPTED_CLOSURES.md`, while current H0 state, ordering, architecture consequences and gates remain in ROADMAP. `ctx03-docsync-history-check.py` protects both sides: normal bootstrap must not pull history back in, and representative exact historical reconstruction pointers must remain present.

A derived current-state row cannot override live GitHub or an exact accepted source. Contradiction triggers reconstruction.

## 11. Adoption and future consumer

This protocol becomes binding only after `WP-CTX-03` receives independent PASS, merges and completes DocSync. It does not retroactively reinterpret accepted CTX-01/02 evidence.

The process-envelope and closure primitives are intentionally repository-generic enough for a future clean consumer repository to adopt before historical context accumulates. CTX-03 does not decide the H2/H3 repository split or move product ownership.
