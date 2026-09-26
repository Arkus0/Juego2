# WP-HK-05 — Validation + repairable diagnostics

Status: COMPLETE  
Class: FOUNDATIONAL  
Depends on: `WP-HK-02A` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `dbd8121411079f22a01d5cb85345e180ff41f7e2`
Implementation PR: `#26`

Reopen 1 (2026-09-26, owner-waived independent review by owner instruction): WP-H1-GATE fresh-agent trial 2 showed that operation-grammar rejections were not repairable by a fresh public client. The correction adds structured field context and a hint; the grammar, codes and schemas are unchanged. See `Docs/evidence/WP-HK-05/REOPEN_1_GRAMMAR_DIAGNOSTICS.md`.

Reopen 2 (2026-09-26, owner-directed; independent review waived by owner instruction): WP-H1-GATE fresh-agent trial 5 showed that a corrupted extension payload was not repairable by a fresh public client. The correction adds the Base64 failure reason and lengths to the context; the acceptance rule, codes and schemas are unchanged. See `Docs/evidence/WP-HK-05/REOPEN_2_BASE64_DIAGNOSTICS.md`.

Completion:
- Reviewed candidate SHA: `23a9fd4373a803187cd9391b1459cd48975177f6`
- Independent Reviewer verdict: `PASS` (PR review `#5257350871`)
- Exact-SHA candidate observation: GREEN (Actions `35464742544`)
- Exact-SHA freeze validation: GREEN (Actions `35464834162`)
- Merge SHA: `ed65661680aea2a9be79f892c96aa42bf788a842`
- Completed: `2026-09-19`

## Objective

Make invalid world states and invalid authoring intents detectable through stable, machine-actionable diagnostics rather than opaque exceptions.

## Acceptance

- Canonical validator covers structural validity, identity uniqueness, reference integrity and every invariant owned by the micro-world model.
- Validation runs before commit and can also be invoked explicitly against current/proposed state.
- Diagnostics have stable machine code, severity, resource/path context, invariant identity and concise remediation context when safe.
- Multiple independent violations can be reported deterministically without hiding earlier failures behind exceptions.
- Validation result schema is discoverable and versioned.
- Runtime exceptions are not used as the public expected-error contract.
- Validator inventory is mechanically enumerable and mapped to owned invariants; unclassified invariants/rejection sites are zero.
- A known-invalid change cannot enter canonical state through any public mutation route.

## Required negative-conformance tests

RED→GREEN for: validation being skipped on a public mutation path, missing invariant registration, exception escaping as public error, nondeterministic diagnostic order, ambiguous path/context, and mutation route accepting state rejected by explicit validation.

## Accepted result

HK05 now exposes versioned current/proposed validation with stable machine diagnostics, mechanically reconciles the finite invariant inventory, aggregates independent violations deterministically, and rejects invalid candidates before canonical commit. Duplicate object/extension identity is reported at index-addressable locations; secondary diagnostics are deferred only when their own source or dependency traversal genuinely depends on an ambiguous identity representative, so unrelated unique-ID violations remain visible.

Two historical Reviewer FAILs on earlier frozen candidates (`e0c865efd2127ca53d9e25064215e09a4579acd4` and `8c8d8c1a66b4995bbc9f3f933ba1791e49d69e97`) exposed the same aggregate-validation/ambiguous-identity class. The foundational circuit breaker triggered an architecture re-audit before the accepted repair. Preserve that history; do not reopen HK02/HK04 predecessor guarantees without concrete contradictory evidence.

## Forbidden scope

AI natural-language repair generation, Unity validation, final gameplay invariants.

## DoD

Invalid micro-world edits are rejected consistently and a machine client has enough structured information to choose a corrected request; independent PASS.
