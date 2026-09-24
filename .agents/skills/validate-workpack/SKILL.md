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

## Mandatory adversarial falsification before PASS

A Worker fixture demonstrates the fixture case. It does **not** demonstrate the whole semantic class unless there is a structural reason that generalizes the result. Before issuing PASS, perform a separate red-team pass whose goal is to construct a candidate that would satisfy the Worker's supplied fixtures/checks while still violating the WP's actual claim.

1. Extract the material universal or boundary claims from the WP and candidate. Pay particular attention to words and semantics such as `bounded`, `finite`, `only`, `never`, `all`, `same rules`, `deterministic`, `independent`, `complete`, `exact`, `no other`, ownership boundaries, selector/ranker/router rules and causal claims.
2. Read every preregistered Reviewer attack surface, falsification hypothesis, negative gate, failure mode or equivalent section in the WP/plan/evidence. Treat each as an attack obligation, not as background prose. PASS requires either a concrete falsification attempt or a structural argument showing why the attack class is impossible/inapplicable.
3. For every material universal/boundary claim, invent at least one **novel falsifier not already supplied by the Worker**. Stay inside the WP's claimed scope; do not strengthen the contract merely to create a blocker.
4. Apply quantifier and boundary transformations where relevant:
   - repeated examples -> many distinct examples;
   - bounded element -> potentially unbounded collection;
   - small finite sample -> arbitrary `N` / saturation pressure;
   - ordinary case -> overflow, exhaustion or conflict case;
   - one owner/consumer/path -> competing owners/consumers/paths within scope;
   - one point example -> composition/lifecycle/restart/compaction boundary;
   - fixed present inputs -> future-only or privileged-input perturbation.
5. For selectors, rankers, routers, promotion rules and causal attribution, perform a **causal-time attack**: hold fixed everything legitimately available at the decision checkpoint and vary only information that becomes available later or is outside the declared authority. An earlier decision must not change retroactively because of future fixture needs or privileged hindsight.
6. For evidence-driven claims, ask whether the proof is structurally independent of the expected answer. A fixture/oracle that declares both the relevant inputs and the expected semantic result may be physically separate yet still be causally self-confirming.
7. Maintain a compact falsification ledger while reviewing: `claim -> novel attack -> observed/structural result`. This is review reasoning, not a new repository artifact. Do not require the Worker to manufacture a ceremonial ledger.
8. **PASS is forbidden** when a material universal claim is supported only by Worker-provided examples/fixtures and no independent generalization or novel attack closes the wider class.

The final GitHub verdict should remain concise. For PASS, name the strongest one to three novel falsifiers attempted and why they did not break the claim; for FAIL, report the causal class and the smallest concrete witness. Do not dump the full internal ledger or create protocol churn.

## After PASS

Use the normal exact-SHA merge/finalization path. Then execute `.agents/skills/update-handoff/SKILL.md` under the Flow Simplification V2 rule: DocSync is a bounded delta and defaults to zero repository commits when no authoritative document meaning changed.

Do not turn post-PASS documentation hygiene into another independent review of the accepted product.
