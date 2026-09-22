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
- non-bootstrap history: `Docs/history/CTX_PROCESS_HISTORY.md`;
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

The measurement estimator is deliberately simple and reproducible: `ceil(UTF-8 bytes / 4)` for each unique repository source. This is a provider-neutral context estimate, not a claim about an exact vendor tokenizer.

Provider-token uncertainty is declared separately in `context-envelope.json`. A saving is called **material** only when:

```text
pre_estimate - post_estimate > pre_uncertainty + post_uncertainty
```

Pre/post bytes are always read from the **same checked-out candidate snapshot**. Repository/history growth therefore cannot masquerade as routing improvement.

Dynamic mandatory inputs whose size CTX does not control — live GitHub state, exact PR diff, current Reviewer FAIL, local-execution manifest-named files — remain mandatory but are held outside the static corpus delta. They may not be silently omitted to improve the number.

For each representative route the report records:

- pre-CTX source set;
- post-CTX minimum source set;
- post-CTX materially escalated source set;
- file-level bytes/estimate;
- uncertainty and material-saving verdict.

H1 sources that remain binding because the WP is foundational/local are held constant in both routes. CITY-04's unresolved cross-track gate forces ROADMAP into the post route instead of pretending the blocked route is closed. The cumulative PA route includes all currently accepted PA capsules.

The exact candidate measurement is persisted in Worker evidence. The important result is intentionally non-uniform: PA has a demonstrated material minimum-route reduction; H1's measured minimum reduction does not clear the conservative uncertainty threshold; CITY is larger because its exact non-compressible seed and unresolved gate remain mandatory. CTX-03 records those facts rather than tuning the universe until every route appears cheaper.

## 3. Auditable context escalation

A `CONTEXT_ESCALATIONS` record evaluates every `must_escalate_if` predicate of the effective role profile. A triggered predicate must name at least one existing authoritative source opened because of it. A non-triggered predicate must record why it is non-material.

The checker independently obtains the required predicate universe from the accepted role profile. The escalation record therefore cannot define its own completeness by deleting a predicate from itself.

`CONTEXT_ESCALATIONS` is navigation/audit evidence only. It may not prove a semantic claim or prevent a later Worker/Reviewer from deepening further.

## 4. Cumulative PA decision

CTX-03 measures PA as a cumulative consumer rather than a direct-predecessor happy path. Accepted CTX-02 already provides one capsule per accepted PA result, completion-side discovery, exact disposition status preservation and fail-closed source reconstruction.

The accepted-candidate measurement shows the PA worker/reviewer minimum route falling from an estimate of `41066` to `22855`, a saving of `18211` / `44.3457%`, which clears the deliberately conservative combined uncertainty threshold. When the material-source escalation opens all PA-01..03 authoritative results, the route becomes `43067`, i.e. intentionally not cheaper than pre-CTX. That is correct: escalation buys source fidelity, not token savings.

Decision: **KEEP CTX-02 CAPSULE CHAIN / DO NOT ADD A SECOND PA COMPACT REGISTRY IN CTX-03**. CTX-02 materially closes the dominant cumulative starting-cost problem, and the CTX-03 quality replay proves the authoritative PA-03 result remains reachable on material escalation. Adding another registry would duplicate compact authority without demonstrated need.

## 5. CI-only process envelope

The process envelope is derived from the accepted `initial_reads` of each role profile; the config supplies only deterministic placeholder substitutions used for calibration. It does **not** duplicate the source list.

For every profile:

```text
reviewed ceiling = ceil(calibrated post-CTX baseline * (1 + reviewed headroom))
```

The initial headroom is 20%. The budget file is CI-only and is not a normal Worker/Reviewer read.

Unrelated repository growth outside the derived read set does not affect the estimate. Growth of a required source does. A future ceiling increase must be an explicit diff with both:

- incremented `ceiling_revision`;
- non-empty `ceiling_increase_justification`.

A silent increase caused only by editing another protocol/configuration is rejected. Independent negative controls mutate a real derived required source across the ceiling and separately add unrelated repository growth; only the former is allowed to turn the envelope RED.

## 6. Mechanical false-red audit

CTX-03 explicitly separates mechanical outcomes so protocol/runner noise does not consume independent Reviewer rounds.

The only outcome vocabulary is:

- `FAIL` — a **registered deterministic verifier** causally proved a repository-owned condition required by this WP is false. This means Worker repair before review. It is not an automated semantic Reviewer verdict.
- `REVIEW_BLOCKED` — handoff/freeze/metadata/lifecycle state is incomplete, pending, stale or incoherent. Fix or derive the process state and rerun before starting a Reviewer.
- `NOT_APPLICABLE` — the verifier does not apply to the candidate/mode. Neutral; never synthetic GREEN proof and never FAIL.
- `INFRA_ERROR` — runner/tool/API/checkout or an unclassified/unknown verifier failed without causal evidence that the candidate condition is false. Operationally fail closed, but do not label the WP defective from that alone.
- `PASS` — the registered deterministic condition is satisfied. Semantic independent review remains mandatory.

The canonical registry is `mechanical-verifier-registry.json`. A red verifier/check not registered there **cannot count as WP FAIL** until a reviewed registry amendment defines its exact deterministic claim and failure class. It is not silently ignored either: an unregistered red is `INFRA_ERROR`, keeps `wp_failed_mechanically=false` and blocks independent review until triaged or registered.

A registered verifier that is allowed to emit candidate `FAIL` must provide a structured outcome. If it merely crashes or returns an unclassified red check, the classifier yields `INFRA_ERROR`, not `FAIL`.

This classification does not soften any semantic/causal gate. It moves obvious mechanical defects left, keeps unknown red state fail-closed without blaming candidate semantics, and preserves the exact independent Reviewer boundary.

## 7. Derivable review metadata

`derive-worker-review-metadata.py` generates the handoff fields that are already mechanically fixed once final bytes are CLEAN and HEAD is known: contract/evidence paths, Candidate/Frozen SHA equality, Ready state values and pending Reviewer fields.

The generator refuses to operate unless the repository-local predecessor check and `WORKER_PRE_REVIEW: CLEAN` evidence exist. It cannot generate CLEAN, choose ownership, alter `fail_cycle`, or produce a Reviewer PASS/FAIL.

The generated block is then validated by the existing independent `validate-worker-handoff.py`. Generation reduces transcription mistakes; validation remains fail closed. A metadata defect is `REVIEW_BLOCKED`; it does not need a semantic Reviewer FAIL to be discovered.

## 8. Transactional REVIEW_READY closure

A Worker may tell the human to start a Reviewer only after the exact terminal invariant is true:

1. final repository/evidence byte mutation precedes the final complete Worker pre-review;
2. exact final evidence says `WORKER_PRE_REVIEW: CLEAN`;
3. Candidate HEAD and Frozen candidate SHA equal the live Ready PR HEAD;
4. canonical Ready metadata is coherent;
5. registered `Worker handoff lint` is PASS;
6. registered `Freeze exact-SHA validation` is PASS;
7. a durable Automation V2 `State: REVIEW_READY` marker targets the same frozen SHA;
8. **after observing that marker**, a final live PR HEAD read still equals that SHA.

After CTX-03 adoption, the `issue_comment` workflow performs steps 7→8 automatically and persists:

```text
ARKUS_AUTOMATION_V2
State: REVIEW_READY_CLOSED
Target SHA: <frozen sha>
```

`REVIEW_READY_CLOSED` is only a durable projection that the already-required terminal invariant was observed. It adds **no new human action** and is not an extra semantic gate. The Worker/human does not post a second comment, click a second transition or re-enter metadata. If the marker is absent/wrong-SHA or HEAD moves, no CLOSED marker appears and the state remains `REVIEW_BLOCKED`.

For the CTX-03 adoption candidate itself, the new `issue_comment` workflow is not yet present on default `main`, so it cannot bootstrap its own event. This candidate satisfies the same accepted invariant directly: existing Automation V2 must persist `REVIEW_READY` for the frozen SHA, then the Worker performs and records the final live HEAD read **without mutating repository bytes**. Once CTX-03 is on `main`, future cycles receive the automatic CLOSED marker.

## 9. Historical handoff controls

The closure oracle reproduces the causal state of:

- the CTX-01 family where substantive bytes were frozen but canonical Ready metadata/check state was incomplete;
- the CTX-02 family where the canonical handoff lint remained red until the real predecessor-check condition was restored;
- the transition-loss case where every prerequisite check is green but no matching durable `REVIEW_READY` exists;
- a post-marker HEAD movement.

In each case CLEAN alone is insufficient. Restoring the real terminal condition, not editing a declaration consumed only by the test, is what turns the control green.

`Docs/evidence/CTX-03/HISTORICAL_CLASSIFICATION.json` separately classifies the reconstructed FAIL/handoff families as mechanical, semantic or mixed and records `ADOPT / DEFER / REJECT`. Generic natural-language semantic equivalence is explicitly rejected as a deterministic gate.

## 10. Structured evidence, DocSync and history

CTX-03 uses small machine-readable rows for naturally tabular process facts and accepted-state navigation. Causal reasoning remains prose when the reasoning itself is evidence.

After adoption, DocSync regenerates the one compact `ACCEPTED_STATE_INDEX.json` projection from authoritative sources, then updates only authoritative/current-state docs whose effective accepted meaning changed. It does not copy the same transition into several current-state narratives merely to preserve chronology. Long accepted closure narrative belongs under exact evidence or `Docs/history/CTX_PROCESS_HISTORY.md`, which is deliberately absent from normal role bootstrap. Historical pointers needed to reconstruct accepted gates/reviews are never deleted.

`ctx03-docsync-history-check.py` proves the representative accepted CTX-02→CTX-03 transition is represented once in the compact CTX row, agrees on the next CTX contract across normal current-state surfaces, preserves reconstruction pointers, and does not leak history into profile `initial_reads`.

A derived current-state row cannot override live GitHub or an exact accepted source. Contradiction triggers reconstruction.

## 11. Adoption and future consumer

This protocol becomes binding only after `WP-CTX-03` receives independent PASS, merges and completes DocSync. It does not retroactively reinterpret accepted CTX-01/02 evidence.

The process-envelope and closure scripts are intentionally repository-generic enough for a future clean consumer repository to adopt before historical context accumulates. CTX-03 does not decide the H2/H3 repository split or move product ownership.
