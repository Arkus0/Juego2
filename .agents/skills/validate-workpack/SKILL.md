---
name: validate-workpack
description: Independently review one frozen Juego2 workpack candidate at its exact SHA from a fresh session; classify material PASS or FAIL versus protocol-only blocks without repairing implementation.
---

# validate-workpack

Independently review one frozen Juego2 candidate. Do not edit implementation.

## Authority

Read the exact WP, relevant accepted predecessor contracts/evidence, `AGENTS.md`, and `Docs/engineering/PRODUCT_SHA_CLOSURE.md`. The amendment governs same-SHA lifecycle classification and Action/DocSync cost. It does not narrow the Reviewer's material causal search.

Accepted predecessor capsules remain governed by `Docs/engineering/CONTEXT_CAPSULE_V1.md`: they are navigation only and never review proof. Escalate to exact authoritative sources whenever the verdict materially depends on them or they are stale/lossy/contradictory.

## Preconditions

- Reviewer is independent from the Worker/repair role.
- The exact `PRODUCT_SHA` is identifiable from current PR HEAD / Frozen candidate identity.
- Required material evidence for that SHA is available.
- A context-bound `REVIEW_READY` marker for that exact SHA is sufficient mechanical handoff. Do not wait for or manufacture a second closure marker.

A malformed lifecycle field does **not** automatically make the product unreviewable when the exact product SHA and evidence identity remain clear. Classify the defect first.

## Review

1. Reconstruct the exact WP claim, accepted direct predecessors and current `PRODUCT_SHA`.
2. Consume trustworthy exact-SHA CI/receipts for mechanical facts they already establish. Reviewer independence is independent judgment, not a requirement to rerun an identical build/test/proof command. Run a new targeted probe only when it adds information: to test a causal false-green hypothesis, cover a materially unproved claim, resolve contradictory evidence, or replace evidence whose SHA/provenance cannot be trusted.
3. Challenge code/evidence/behaviour against the WP's actual scope and trust boundary. Search for causal false-greens, omitted in-claim surfaces, invalid evidence binding, regressions and forbidden scope changes.
4. Consume accepted predecessor guarantees unless concrete evidence shows they are false or inapplicable. Do not demand duplicate proof merely for defence in depth.
5. Distinguish material blockers from protocol/administrative defects:
   - **FAIL** only for a material/causal defect in code, required execution, evidence, proof, scope, or integrity severe enough that the reviewed product/evidence identity cannot be trusted.
   - **PROTOCOL_FIX / REVIEW_BLOCKED** for stale/malformed PR body, comments, markers or lifecycle metadata when `PRODUCT_SHA` and material evidence remain unchanged and identifiable.
6. A protocol-only defect must not cause another product campaign. Report the exact metadata repair needed and preserve all material findings/verdict work already completed.
7. When the material claim is sufficiently demonstrated, issue `PASS` bound to exact `PRODUCT_SHA`. When a material blocker exists, issue `FAIL` bound to exact `PRODUCT_SHA` and describe the causal class, not only one example.

## After PASS

Use the normal exact-SHA merge/finalization path. Then execute `.agents/skills/update-handoff/SKILL.md` under the Flow Simplification V2 rule: DocSync is a bounded delta and defaults to zero repository commits when no authoritative document meaning changed.

Do not turn post-PASS documentation hygiene into another independent review of the accepted product.
