---
name: validate-workpack
description: Independently review one frozen Juego2 workpack candidate at its exact SHA from a fresh session; classify material PASS or FAIL versus protocol-only blocks without repairing implementation.
---

# validate-workpack

Independently review one frozen Juego2 candidate. Do not edit implementation.

## Authority

Read the exact WP, relevant accepted predecessor contracts/evidence, `AGENTS.md`, and `Docs/engineering/PRODUCT_SHA_CLOSURE.md`. Also read every preregistration, frozen plan, amendment, reviewer-attack surface, stop condition, or other authoritative input that the WP explicitly requires for the claim being reviewed. The amendment governs same-SHA lifecycle classification and Action/DocSync cost. It does not narrow the Reviewer's material causal search.

Accepted predecessor capsules remain governed by `Docs/engineering/CONTEXT_CAPSULE_V1.md`: they are navigation only and never review proof. Escalate to exact authoritative sources whenever the verdict materially depends on them or they are stale/lossy/contradictory.

## Preconditions

- Reviewer is independent from the Worker/repair role.
- The exact `PRODUCT_SHA` is identifiable from current PR HEAD / Frozen candidate identity.
- Required material evidence for that SHA is available.
- A context-bound `REVIEW_READY` marker for that exact SHA is sufficient mechanical handoff. Do not wait for or manufacture a second closure marker.

A malformed lifecycle field does **not** automatically make the product unreviewable when the exact product SHA and evidence identity remain clear. Classify the defect first.

## Independence means independent attack construction

A fresh session is necessary but not sufficient for independence. Worker pre-review, handoff prose, PR conclusions, candidate-authored fixtures and GREEN checks may be evidence, but they must **not define the Reviewer's attack surface**.

Before using the Worker's conclusions to decide PASS/FAIL:

1. Reconstruct a **claim ledger** from the WP plus its required authoritative inputs. Include Acceptance, Definition of Done, negative gates, frozen hypotheses/stop conditions, explicit reviewer-attack surfaces, ownership/trust boundaries and any material universal claim introduced by the candidate.
2. For each material claim, write at least one plausible falsifier that is **not merely a replay of a Worker-authored positive/negative fixture**. Ask: "How could this claim still be false while every supplied fixture and check remains GREEN?"
3. Only then inspect the Worker's fixture set and pre-review conclusions. Candidate evidence may close an independently derived falsifier; it may not substitute for deriving one.

If the required preregistration names a reviewer attack or falsifier class, explicitly close it or FAIL. Do not silently narrow it to the examples the Worker chose to implement.

## Mandatory falsification dimensions

Use judgment, but actively test every dimension that is material to the claim rather than assuming the supplied examples span it:

- **cardinality / scale / composition:** a bounded item or bounded repeated category does not prove the aggregate system is bounded; try many distinct valid items, multiple owners, or composed paths;
- **time / order / future information:** selection, ranking and causality must not depend on facts unavailable at the decision checkpoint; vary only later history and look for retrospective/post-hoc success;
- **causal independence:** paired cases must change only the asserted cause; detect hidden correlated state, candidate-owned expected answers, tautological fixtures and self-fulfilling rules;
- **authority / provenance / epistemics:** privileged truth, hidden lineage, cached summaries or duplicate mutable owners must not leak into the claimed owner;
- **negative / absence claims:** absence in a compact representation, fixture or sampled set is not proof of real absence; seek an independent counterexample/source-open path;
- **quantifier strength:** words and meanings such as `bounded`, `all`, `any`, `never`, `only`, `same`, `independent`, `complete`, `deterministic`, `equivalent` and `no global scan` require evidence at that strength, not a convenient local example;
- **deferred-proof boundary:** a future runtime may own numeric tuning/performance, but a semantic property that is central to the current WP's Acceptance/DoD cannot be deferred merely because its eventual implementation is future work.

These are attack families, not a demand for redundant proof. Run a new targeted probe only when it adds information.

## Review

1. Reconstruct the exact WP claim, accepted direct predecessors, required authoritative inputs and current `PRODUCT_SHA`.
2. Build the candidate-independent claim ledger and falsifiers above **before** accepting the Worker's coverage as sufficient.
3. Consume trustworthy exact-SHA CI/receipts for mechanical facts they already establish. Reviewer independence is independent judgment, not a requirement to rerun an identical build/test/proof command. Run a new targeted probe only when it adds information: to test a causal false-green hypothesis, cover a materially unproved claim, resolve contradictory evidence, or replace evidence whose SHA/provenance cannot be trusted.
4. Challenge code/evidence/behaviour against the WP's actual scope and trust boundary. Search for causal false-greens, omitted in-claim surfaces, invalid evidence binding, regressions and forbidden scope changes.
5. Consume accepted predecessor guarantees unless concrete evidence shows they are false or inapplicable. Do not demand duplicate proof merely for defence in depth.
6. Distinguish material blockers from protocol/administrative defects:
   - **FAIL** only for a material/causal defect in code, required execution, evidence, proof, scope, or integrity severe enough that the reviewed product/evidence identity cannot be trusted.
   - **PROTOCOL_FIX / REVIEW_BLOCKED** for stale/malformed PR body, comments, markers or lifecycle metadata when `PRODUCT_SHA` and material evidence remain unchanged and identifiable.
7. A protocol-only defect must not cause another product campaign. Report the exact metadata repair needed and preserve all material findings/verdict work already completed.
8. A PASS requires more than all supplied fixtures looking coherent: there must be no surviving material falsifier in which the candidate's current evidence can remain GREEN while an in-scope claim is false.
9. When the material claim is sufficiently demonstrated, issue `PASS` bound to exact `PRODUCT_SHA`. When a material blocker exists, issue `FAIL` bound to exact `PRODUCT_SHA` and describe the causal class, not only one example.

## Verdict evidence

The review body should identify the **strongest independently constructed falsifier attempted** and say why it was closed, or state it as the blocker. For a PASS, also identify any material preregistered reviewer-attack surface that required explicit closure. This is evidence that the Reviewer did not merely mirror the Worker's fixture list.

Do not manufacture ceremonial prose when the WP is tiny, but a PASS that only restates GREEN checks and candidate-authored scenarios is not a sufficient independent review.

## After PASS

Use the normal exact-SHA merge/finalization path. Then execute `.agents/skills/update-handoff/SKILL.md` under the Flow Simplification V2 rule: DocSync is a bounded delta and defaults to zero repository commits when no authoritative document meaning changed.

Do not turn post-PASS documentation hygiene into another independent review of the accepted product.
