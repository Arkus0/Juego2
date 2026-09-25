# WP-CTX-DW-H1-02 — selective-adoption validation + H1-07+ disposition

Status: **COMPLETE / ACCEPTED**  
Class: **NON-PRODUCT-FOUNDATIONAL / CROSS-TRACK ADOPTION VALIDATION**  
Execution: **REMOTE_OK**  
Depends on: accepted `WP-CTX-DW-H1-01` + accepted `WP-H1-05` + accepted `WP-H1-06`  
Blocks: no H1 product workpack; informs bounded `USE / OPTIONAL / NOT_MATERIAL` routing for `WP-H1-07+`

Baseline SHA: `159b85f8352ca5df2aa5b3c714ad5dc94b76d64d`.

Accepted candidate: `a6f9a3a82dfaed51ca4dc20a70967aa07b201cdb`  
Canonical implementation PR: `#203`  
Independent PASS review: `#5313739292`  
Implementation merge: `e6b29a3bd1db8065119b5d292173e45443c247ef`  
Arkus Candidate Validation: `#2096` / run `36098000952` GREEN  
Arkus Main Safety: `#639` / run `36097756860` GREEN  
Binding DocSync: `Docs/evidence/WP-CTX-DW-H1-02/DOCSYNC.md`

## Objective

Validate whether the accepted H1→DW projection is actually useful across the two materially different accepted H1 consumer shapes (`WP-H1-05` managed-scene publication and `WP-H1-06` asset/prefab realization) without hiding blockers, source authority or CTX-mandated reads, then publish bounded routing guidance for H1-07+.

This workpack evaluates accepted artifacts. It does **not** create duplicate shadow H1-05/H1-06 Workers, rerun physical Unity merely to manufacture context metrics, or expand the H1-04 projection to cover later product semantics.

## Authority boundary

For represented H1 catalogue/source facts:

```text
accepted H1-04 authority
        ↓ derived/current
accepted WP-CTX-DW-H1-01 projection
        ↓ selective navigation only
CTX-routed Worker/Reviewer
```

For product truth:

```text
H1 workpack contract + accepted product evidence/code/oracle
        > DW routing/navigation advice
```

For process/routing:

```text
accepted CTX mandatory-read / effective-read / escalation authority
        > DW materiality advice
```

A `USE` classification never makes DW product-authoritative and never cancels an authoritative source-open or a CTX-mandated read.

## Owned guarantees

- consume the accepted H1-05 and H1-06 artifacts as the two real observation shapes;
- verify the exact accepted H1-01 projection lifecycle/identity is current before any `USE` admission;
- derive routing from capabilities actually represented by that projection, not from H1/domain labels or free text;
- bind each canonical claim ID to an independently verifier-owned semantic contract rather than allowing the evaluated fixture to define its own requirements or mandatory reads;
- distinguish full coverage (`USE`), partial navigation value (`OPTIONAL`) and no material projection value (`NOT_MATERIAL`);
- preserve every product-evidence / mandatory-read source required by the active claim;
- require source-open provenance for every admitted projected fact used by the observation;
- record bounded navigation/context effects without turning token savings into product acceptance evidence;
- publish an H1-07+ disposition that remains claim-shaped rather than a blanket H1 default;
- keep the mandatory H1-GATE fresh public-client trial private-context free.

## Explicitly not owned

- H1-04 source/catalogue correctness or source adoption;
- H1-05 scene publication semantics;
- H1-06 prefab/source-derived relationship equality;
- H1-07 component adapter completeness or field round-trip truth;
- expansion of the accepted H1-01 projection schema merely to improve adoption scores;
- generic arbitrary-domain CTX↔DW portability;
- H2 public/external knowledge portability;
- physical Unity re-execution of accepted H1-05/H1-06.

## Work

1. Freeze accepted identities for H1-01 projection lifecycle, H1-05 and H1-06.
2. Exercise the accepted projection itself against representative catalogue/source queries used by both accepted consumer shapes.
3. Bind each canonical claim ID to verifier-owned capability requirements and exact accepted claim anchors outside the evaluated fixture.
4. Preserve claim-specific mandatory authority/evidence reads from verifier-owned obligations, then audit them against reads actually executed by the route.
5. Record bounded navigation byte/read effects for admitted `USE` queries.
6. Prove `USE` becomes inadmissible when lifecycle currentness is false.
7. Prove adding domain labels/free text or undeclared synthetic capabilities cannot manufacture `USE`.
8. Prove partial projection coverage cannot close a product claim or hide its product oracle.
9. Publish and mechanically verify the exact per-claim H1-07+ disposition, not only aggregate class counts.
10. Add exact-SHA remote verification and adversarial controls, including semantic claim swapping/relabeling.

## Classification contract

For one canonical claim ID, let `R` be its verifier-owned material capability requirements and `P` the capabilities actually represented by the current accepted H1 projection. `R` is **not** read from `ADOPTION_CASES.json` or any expected-result field in the evaluated fixture.

- `USE`: lifecycle is current, `R` is non-empty, and every requirement in `R` is represented by `P`. Projected answers must still source-open to accepted authority.
- `OPTIONAL`: some but not all requirements in `R` are represented by `P`, or a would-be `USE` route loses lifecycle currentness. DW may help navigate but cannot close the claim.
- `NOT_MATERIAL`: no material requirement in `R` is represented, or the canonical claim contract explicitly isolates the claim from private context (the mandatory H1-GATE public-client trial).

The classification does not depend on workpack name, domain label, descriptive prose, fixture requirements, fixture mandatory-read lists or fixture expected results.

## Acceptance

PASS requires all of the following:

- both H1-05 and H1-06 are consumed from accepted evidence, not replayed shadow executions;
- at least one real accepted observation demonstrates useful source-open `USE` navigation and at least one demonstrates correct abstention/partial coverage;
- `USE` is mechanically gated by the exact current H1-01 lifecycle identity;
- H1-05 scene/publication truth and H1-06 relationship-multiset truth remain outside the projection and remain discoverable through their accepted evidence;
- canonical claim requirements and mandatory-read obligations are independent of the evaluated fixture;
- effective reads are produced by actual claim-anchor/source-open operations and then compared against the independent mandatory-read obligation;
- the exact eight claim IDs map to `3 USE / 2 OPTIONAL / 3 NOT_MATERIAL` with the published identity-specific disposition; aggregate `3/2/3` alone is insufficient;
- changing or swapping fixture-declared requirements/expected results cannot redefine claim semantics and must turn the verifier RED;
- bounded context/navigation measurements are reported as routing evidence only, never product correctness;
- H1-07 guidance is selective: catalogue/source/component-schema inventory may use current DW navigation, mixed component/reference claims remain partial, and effective adapter/field/runtime truth stays source/product authoritative;
- H1-GATE's mandatory fresh public-client trial is classified `NOT_MATERIAL` for Juego2-private CTX/DW context;
- causal controls turn RED for stale lifecycle, hidden mandatory reads, capability inflation, broad H1-label-based adoption and semantic claim relabeling;
- no accepted H1 product dependency is rewritten to depend on this workpack or DW.

## Negative gates

FAIL if the evaluation invents duplicate H1-05/H1-06 executions, if a fixture can declare its own coverage result or semantic requirement/mandatory-read set, if `USE` remains legal on stale lifecycle, if DW advice can remove a mandatory authority/product read, if product-only semantics are treated as projection-covered, if domain labels/free text manufacture adoption, if H1-07+ receives blanket `USE`, if exact claim identities can be semantically swapped while preserving aggregate class counts, or if private CTX/DW context is allowed into the mandatory H1-GATE fresh-client trial.

## Evidence shape

Remote deterministic evidence only: accepted predecessor identity check, one focused test over the real H1-01 projection, accepted H1-05/H1-06/H1-07 claim anchors, verifier-owned canonical claim contracts, capability-based routing, actual source-open/mandatory-read audit, bounded navigation measurements, exact H1-07+ disposition and adversarial controls.

## PASS consequence

H1-07+ may consume the published claim-shaped routing defaults when convenient. H1 product progress remains independent: a missing/stale/unhelpful projection falls back to authoritative sources rather than blocking the product WP.
