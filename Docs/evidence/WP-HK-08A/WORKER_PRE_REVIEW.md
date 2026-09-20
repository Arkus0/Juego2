# WP-HK-08A Worker pre-review

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 3
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-08A/WORKER_PRE_REVIEW.md

## Candidate challenged

- WP: `WP-HK-08A`.
- Baseline: `13db4f890f4b6fe917fdf0d0c4e99cecfeed368d`.
- Branch: `wp/hk-08a-efficient-interaction`.
- Direct predecessor: accepted `WP-HK-07B`; predecessor reconstruction is recorded in `WORKER_PLAN.md`.
- Final implementation/test SHA challenged here: `7102330e9a3030d8f4f333d2715a8633312eeb7b`.
- GitHub Actions observation: `35498188203`; artifact `10600817518`.

This is Worker quality-gate evidence only. It is not an independent Reviewer verdict.

## Scope and predecessor check

The diff was challenged against the literal HK08A contract and the accepted HK03→HK07B architecture. The implementation does not add a second mutation authority, validator, provenance journal, discovery registry or transport-specific semantic model:

- batching continues through the accepted HK04 transaction/commit authority;
- candidate validation remains HK05/accepted world validation;
- provenance remains the complete HK06A journal authority, with HK08A providing only a bounded public read view;
- compact object reads reuse HK03 field projection;
- JSONL and MCP continue to project the same canonical definitions and neutral dispatch;
- stale-CAS mutation recovery, repair prioritization and final budgets remain absent and owned by HK08B/HK09B.

No predecessor reopen condition was observed.

## Finding 1 — cursor offset context was editable without an integrity guard

The first green implementation observation (`98b4696fe33f2cd26631249a058305993ec12a75`) had a real HK08A-owned false green. Its cursor was canonical Base64 framing over revision/hash/base/count/limit/offset, but a client could decode it, change the offset while keeping the same valid journal anchor, re-encode it and cause a gap/duplicate while the stale-anchor checks still passed.

This directly contradicted HK08A's pagination completeness claim, so the candidate was **not** frozen despite green CI.

Repair:

- `IntegrityBoundWorldProvenanceService` now wraps every public continuation cursor in a deterministic integrity envelope before transport projection;
- the envelope covers the complete underlying cursor context and rejects altered/corrupted context as `world.provenance.invalid_cursor` before paging resumes;
- this is framing integrity only, not a security/authentication claim;
- `AlteredCursorOffsetFailsClosedInsteadOfSkippingJournalEntries` reproduces the original false-green class by changing only the inner offset while retaining the old checksum; the altered cursor fails closed, while the unchanged cursor resumes at pageOffset 1 / sequence 2.

## Finding 2 — ordinary modify evidence was too weak for the anti-chattiness claim

The initial external flow performed an object update, but only one effective property changed. That was enough for transport parity but not a strong demonstration of the explicit HK08A rule that an ordinary coherent update must not require one public call per property.

Repair:

- `OneCoherentMutationRequestUpdatesSeveralObjectPropertiesTogether` updates type, container and references in one `put-object` operation / one canonical request;
- the resulting canonical plan reports a single `update` resource change containing all three changed fields;
- the persisted result advances one revision and appends one provenance entry;
- the independent shape oracle still turns red for the artificial one-request-per-property pattern.

No new field-setter API or high-level planner was introduced.

## Finding 3 — operation failure was not sufficient proof that global validation remains in the batch path

The first atomic negative used a later `remove-object` for a missing resource. That proves no partial commit after an operation-level failure, but a path that accidentally omitted candidate-level HK05/world validation could still pass that test.

Repair:

- `ParsedBatchThatViolatesWorldValidationCannotPublishOrAppendProvenance` uses two operations that both parse/apply locally;
- the resulting candidate contains a missing containment target, so the accepted global validator is the reason the batch must be rejected;
- state revision/hash/objects and journal remain unchanged.

This closes the exact "batch path omitting validation" defect class without duplicating the validator.

## Complete diff / false-green challenge

The final diff was challenged for:

- keeping the old 64-operation ceiling while claiming the representative intent is atomic;
- implementing a hidden multi-batch transaction or committing halves;
- applying earlier operations when a later operation or global validation fails;
- reporting success without one matching provenance entry;
- returning a partial journal page under the accepted complete HK06A schema and thereby confusing replay;
- duplicate/missing sequence across pages;
- stale anchor/cursor continuation after authored state changes;
- altered cursor offset producing a gap while the same anchor remains current;
- compact reads removing the required authored-world anchor;
- introducing a transport-local cost or batching registry;
- JSONL/MCP drift for batch, page, compact or discovery semantics;
- claiming anti-chattiness while only proving one changed property;
- silently pulling stale-CAS recovery, resource quotas, multi-agent orchestration or engine/gameplay semantics into HK08A.

No such blocker remains in the implementation/test SHA.

## Product-shape recheck

`Docs/art/VISUAL_BIBLE.md` is the approved-draft source used only for scale/context: fictional Potes/Liébana, modular reuse and an H2 hero target of at most roughly 120 meshes. The 96-operation probe deliberately stays below that future hero-scale envelope while crossing the old 64 ceiling. Its 72 objects + 24 opaque extensions are fixture records, not a new canonical town/Unity schema. `CONTENT_SHAPE_PROBE.md` records the assumptions and future-owner classifications.

## Green observation after all findings

Exact implementation/test SHA `7102330e9a3030d8f4f333d2715a8633312eeb7b` passed:

- locked restore: GREEN;
- Release build: 0 warnings / 0 errors;
- focused `Hk08A*`: 13/13 GREEN;
- full regression: 173/173 GREEN;
- candidate clean before/after: YES;
- Actions run `35498188203`: GREEN;
- artifact `10600817518`.

## Proof-budget conclusion

All three pre-review additions close explicit HK08A acceptance gaps or a concrete false green found in the candidate. None introduces generic hardening machinery. The remaining named risks belong to HK08B/HK09B or later host/engine work. Adding further proof for hostile cursor fabrication, final performance limits or stale mutation recovery would expand the claim rather than converge it.

PROOF_BUDGET_VERDICT: WITHIN_BUDGET

No known in-boundary blocker remains. The resulting evidence-reconciliation SHA is eligible for exact-SHA verification and freeze; a fresh independent Reviewer is still required after that handoff.
