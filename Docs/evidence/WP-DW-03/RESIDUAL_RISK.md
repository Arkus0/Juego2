# WP-DW-03 — Residual risk and final circuit-breaker audit

RESIDUAL_RISK_VERDICT: ACCEPTABLE_WITHIN_CLAIM
UNRESOLVED_IN_BOUNDARY_RISKS: 0
PREDECESSOR_REOPEN_CONDITION_TRIGGERED: NO

## Residual risks outside the DW-03 claim

1. **Future PA evolution.** A new PA or an intentional revision of PA-01..05 may require a new entity/field/relation shape. DW-03 deliberately fails closed rather than pretending v1 is universal.
2. **Unstructured narrative.** Prose outside `SOURCE_MANIFEST_V1.md` is not first-class query data in v1. It remains authoritative and source-open through provenance; DW-03 does not claim the compact corpus substitutes for rereading source when a consumer needs material outside the declared suite.
3. **Same-authority navigation is not proof attribution.** `same-authority-evidence` and `same-authority-fixture` make accepted material reachable; they do not assert that every evidence row or fixture individually proves every finding.
4. **Trusted primitives.** Git blob identity, cryptographic hashes, filesystem/source reads and inherited Design World normalization are trusted infrastructure.
5. **No context-efficiency claim.** The existence of a compact deterministic index does not prove token savings, model quality or optimal agent retrieval. Those measurements belong to DW-04.
6. **No runtime claim.** Research fixtures and projected PA semantics do not prove Unity/runtime implementation, persistence, performance or final H0 architecture.

These are explicit boundaries, not known false-green classes inside the declared DW-03 guarantee.

## Final adversarial circuit-breaker

### Can a source element disappear and reduce the expected universe at the same time?

**No within the accepted v1 universe.** Six accepted semantic authorities are pinned by Git blob identity before production parsing, and the semantic oracle separately pins/reconstructs them. A source edit cannot be accepted as the same universe merely because the parser now sees fewer rows.

### Can a disposition or negative finding be lost while IDs/counts still look plausible?

**No.** Exact material/disposition fields and disposition flags are compared source-side. Controls remove an entire disposition surface, weaken one disposition behind the same identity, and remove a hard negative/failure-mode record; all are semantic RED even with a self-consistent generic projection.

### Can a relation change and auto-confirm producer + generic validator?

**No for the claimed PA relations.** The source oracle reconstructs exact relation name/cardinality/target multisets independently. Separate controls remove, rename, retarget and add a second valid target while the generic projection remains GREEN where the defect is PA-specific.

### Can source schema change while preserving positions and stay GREEN?

**No.** Accepted bytes are pinned and table/header structures are reviewed explicitly. Focused rename and reorder controls fail closed before projection.

### Can the semantic oracle remain in the repository but be bypassed by production?

**No.** `ProductionBuildRouteActuallyInvokesSemanticOracle` injects a sentinel oracle and requires the actual `BuildAndValidate()` route to invoke it and fail. Removing only that invocation breaks the suite even if direct oracle tests still exist.

### Can values remain equal while provenance is falsified?

**No.** Authority id, source path, full source anchor, source digest and anchor digest are independently compared. The forged-provenance control preserves values/identity and still produces semantic RED.

### Can the frozen suite cover only the easy cases?

**No.** The suite was frozen before implementation verdict and traverses all five PA families plus findings, dispositions, evidence, fixtures, invariants/failure families and negative/rejected/deferred material. The causal controls target every required loss class rather than only positive lookup paths.

### Can a downstream consumer retrieve the material information DW-03 claims without rereading the entire PA corpus?

**Yes for the declared structured guarantee.** Query records expose complete adopted material text, exact disposition, typed relations and source-open provenance, and the compact index is deterministic. A consumer still returns to the accepted source when it needs intentionally unmodeled narrative; this preserves rather than erases the authority boundary.

### Is DW-03 claiming work that actually belongs to DW-04?

**No.** DW-03 proves fidelity/losslessness and useful deterministic retrieval. It makes no token-budget, context-window, model-quality or agent-efficiency claim.

### Can an unreviewed future PA silently become authoritative?

**No.** A synthetic PA-06 projected record can satisfy the self-confirming generic path but the source oracle rejects it as `pa.semantic_fact_unexpected`. Future extension requires ordinary PA acceptance and explicit reviewed projection extension.

## Reopen conditions

- Reopen **DW-00** only for a demonstrated generic provenance/rebuild limitation not repairable downstream.
- Reopen **DW-01/DW-02** only if concrete evidence disproves the inherited semantic-oracle/schema-binding guarantee on the effective path.
- Reopen **accepted PA semantics** only through PA evidence rules if the accepted source itself is contradictory.
- Otherwise, future defects in this representation are **DW-03** defects and do not justify rewriting accepted authority for convenience.

## Audit conclusion

After source-universe, schema, content, negative-knowledge, relation, provenance, determinism, compactness, future-extension and wiring attacks, no known in-boundary defect class remains undetected by the repository-backed proof suite. Lifecycle freeze and independent Reviewer acceptance remain required; this document does not self-accept the workpack.