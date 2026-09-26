# Product work execution policy — proportional proof after the foundations

Status: **PROPOSED / PROCESS_ONLY**  
Date: 2026-09-25  
Scope: future non-foundational product/research work. This policy does not weaken or reinterpret any accepted H0/H1/DW foundational guarantee.

## 1. Why this policy exists

Juego2 used the same Worker → freeze → exact-SHA → independent Reviewer discipline to build a strong H0/H1 foundation. That standard is appropriate when a false PASS can corrupt authority, determinism, persistence, replay, reconciliation or the engine boundary.

The same ceremony is not automatically appropriate for visual composition, level iteration or bounded product research. Those activities need fast iteration and direct product evidence. The project therefore separates proof rigor by the risk of the claim rather than by habit.

## 2. PASS-before-work rule

Every new executable workpack MUST define its acceptance contract before implementation/research begins.

The contract must state, in one reviewable place:

- the positive claim being proved;
- exact mandatory positive evidence;
- negative gates / forbidden false-PASS states;
- explicit non-claims;
- allowed residuals;
- the evaluation method: deterministic test, human visual/play inspection, research argument, or a named combination;
- the causal predecessors whose accepted guarantees are consumed rather than re-proved.

Worker and Reviewer use the same acceptance contract.

A Reviewer may add a blocker outside the predeclared checklist only when it demonstrates one of:

1. a concrete false PASS under the written claim;
2. a direct contradiction of an accepted binding predecessor;
3. missing contractual reachability that lets the Worker legally skip a mandatory requirement;
4. evidence that the acceptance oracle itself cannot prove the claim it says it proves.

A useful improvement, speculative edge case or broader defence that does not meet one of those conditions becomes a residual/backlog item rather than a surprise blocker.

## 3. Execution classes

### `FOUNDATION_STRICT`

Use for H0, foundational H1/DW work and any later work that changes canonical authority, determinism, transaction/replay semantics, persistence identity, public protocol compatibility or equivalent platform-level guarantees.

Binding process remains the accepted foundational proof standard: exact candidate identity where required, causal negative controls, independent review and the accepted gate/recovery rules.

This policy does not modify `FOUNDATION_STRICT`.

### `PRODUCT_CHECKPOINT`

Default for CITY keeper work, ART production, visual/interactive H2 work and other bounded playable product work.

Rules:

- local Worker iteration between checkpoints is intentionally cheap;
- no independent review is required for each click, mesh adjustment, asset swap, dressing pass or local legal geometry edit;
- the WP defines 2–3 meaningful checkpoints at most unless a causal reason requires more;
- final acceptance uses the smallest combination of deterministic affected-invariant checks and direct human third-person/play evidence that proves the product claim;
- exact-SHA proof infrastructure is not required merely because Git is used; a stable reviewed candidate is sufficient unless the WP explicitly owns a reproducibility/identity claim;
- accepted predecessor tests are not replayed wholesale: only concretely affected conclusions are revalidated;
- visual taste is owner/art-direction authority. Reviewer checks conformance, false-PASS risk and evidence completeness; it does not silently replace the approved art direction with its own preference.

Recommended checkpoints:

1. target/baseline checkpoint;
2. structural/playable checkpoint;
3. final acceptance candidate.

### `RESEARCH_BATCH`

Default for non-foundational PA/product research where several serial questions are tightly related and have no independent runtime authority.

Rules:

- preserve each original research question, acceptance claim, provenance and result artefact;
- multiple related research units may be executed in one Worker branch/PR and receive one independent batch review;
- the batch reviewer verifies every included unit against its prewritten PASS, plus cross-unit consistency;
- do not require freeze/review/merge/DocSync between internal units unless a later unit would become invalid if the earlier answer changed materially;
- one accepted batch gets one DocSync closure;
- runtime/Unity claims remain deferred to their future product owners.

## 4. Non-canonical prototype lane

`PROTOTYPE / NON_CANONICAL` work is allowed for product discovery.

It may use shortcuts, temporary geometry, proxy behaviours and direct Unity/editor experimentation, provided that:

- it is visibly labelled non-canonical;
- it cannot satisfy keeper/gate acceptance;
- `PROXY_VISUAL` / `COVERAGE_BLOCKED` remain truthful states rather than disguised success;
- any successful lesson promoted into keeper work is re-expressed through the correct accepted owner/contract.

A prototype is evidence for a design decision, not authority over the shipping world.

## 5. Review circuit breaker

For `PRODUCT_CHECKPOINT` and `RESEARCH_BATCH`, after a FAIL the repair cycle is bounded to the causal blockers in that review.

A subsequent Reviewer should not search for a new family of defences unless the repair itself changed the claim or exposed a direct new contradiction. New non-causal improvements go to residuals. This prevents whack-a-mole while preserving the ability to reject a real false PASS.

## 6. DocSync policy

DocSync happens at accepted contract/checkpoint boundaries, not after every internal iteration.

- `FOUNDATION_STRICT`: unchanged existing rules.
- `PRODUCT_CHECKPOINT`: DocSync after final WP acceptance or a deliberately accepted intermediate product contract.
- `RESEARCH_BATCH`: one DocSync after the accepted batch; internal result files remain individually addressable.

## 7. Local vs hosted evidence

Use hosted/automated evidence when the claim is machine-verifiable and all lawful inputs are available.

Physical/local Unity or human inspection remains required when acceptance materially depends on appearance, feel, camera readability, third-person scale, animation presentation, interactive traversal or source bytes unavailable to the runner.

Do not convert a visual/product judgement into a synthetic automated oracle merely to avoid local evidence.

## 8. Definition of success

This policy succeeds if non-foundational work becomes materially faster without creating a path for infrastructure guarantees or keeper readiness to PASS on weaker evidence.
