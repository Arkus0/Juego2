# PA pending-research batch execution amendment

Status: **PROPOSED / PROCESS_ONLY**  
Date: 2026-09-25

## Purpose

Reduce review/merge/DocSync overhead for the remaining non-foundational PA research without dropping any accepted research question, acceptance criterion, result artefact, provenance or deferred-proof boundary.

This amendment changes **execution packaging**, not the semantic meaning of PA-07..14.

## Supersession rule

`WP-PA-07` through `WP-PA-13` remain binding research specifications/inputs, but after this amendment is accepted they are **not individually executed as seven separate Worker -> review -> merge -> DocSync cycles**.

They are executed through three batch workpacks:

```text
PA-B1 = PA-07 + PA-08 + PA-09
PA-B2 = PA-10 + PA-11 + PA-12
PA-B3 = PA-13 + H3/H4 pre-integration handoff
```

`PA-14` remains deferred until H2-GATE and the prior PA research are accepted.

Each original canonical result file remains separate:

```text
Docs/research/living-world/results/PA-07.md
...
Docs/research/living-world/results/PA-13.md
```

The batch does not collapse findings into one opaque dossier.

## Review rule

One independent review evaluates every included original PA unit against its already-written PASS/negative gates **plus** cross-unit consistency.

A unit may be `PASS` while another unit in the same candidate has a blocker, but the batch cannot be accepted until all included units pass. Repairs are causal and bounded under `PRODUCT_EXECUTION_POLICY.md`.

No exact-SHA/foundational negative-conformance ceremony is introduced merely because the research is batched.

## Batch dependencies

- `PA-B1` depends on accepted PA-01..06.
- `PA-B2` depends on accepted `PA-B1`.
- `PA-B3` depends on accepted `PA-B2`.
- `PA-14` depends on accepted `PA-B3` + accepted H2-GATE.

## Non-claims

Batching does not claim runtime implementation, Unity proof, tuning or performance. H3/H4+ still own empirical/runtime validation.
