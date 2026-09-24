# WP-CTX-DW-H1-01 — H1 projection bootstrap + lifecycle

Status: **PLANNED / DORMANT UNTIL PREREQUISITES**  
Class: **NON-PRODUCT-FOUNDATIONAL / CROSS-TRACK PROJECTION**  
Execution: **REMOTE_OK**  
Depends on: accepted `WP-CTX-DW-GATE` + accepted `WP-H1-04`  
Blocks: no H1 product workpack; enables an H1 projection to be classified `USE` when current

## Objective

Materialize the first real accepted H1 catalogue/source universe into DW after `WP-H1-04` has established that universe, without making DW canonical, without changing H1 product semantics and without teaching H0/DW generic code H1-specific meaning.

This workpack exists because `WP-CTX-DW-GATE` proved the lifecycle requirements only with bounded falsification evidence. It did **not** create the actual H1 projection, because the real Quaternius/catalogue universe does not exist until H1-04 is accepted.

## Authority boundary

The authority order remains:

```text
accepted H1-04 source/catalogue authority
        ↓ derived
H1→DW projection
        ↓ selective navigation
CTX-routed Worker/Reviewer consumption
```

The projection is rebuildable context infrastructure. It is never the source used to prove H1-04, H1-05, H1-06 or later H1 product truth.

## Owned guarantees

- an independently enumerated H1-04 source universe exists outside the projection being checked;
- a versioned non-product H1→DW adapter/schema maps only declared H1 facts/relations/provenance into generic DW structures;
- H1/domain semantics remain adapter/consumer-owned and do not become shared H0/DW-kernel semantics;
- completeness is checked against the independent authoritative universe, including represented identities, declared fields, relations, cardinality/targets and provenance;
- every projected record can source-open to exact accepted source/evidence identity plus sufficient fingerprints/hashes and schema/projection version;
- material source identity, accepted H1-04 authority or projection-schema change makes the projection stale until rebuild/revalidation;
- deterministic rebuild from the same accepted universe reproduces the same normalized projection;
- corruption, unilateral projection mutation, missing provenance or a stale fingerprint turns the lifecycle RED rather than remaining self-consistent;
- CTX may classify this exact projection `USE` only while the complete lifecycle evidence is current and GREEN.

## Explicitly not owned

- H1-04 product acceptance or Unity catalogue correctness;
- H1 scene/prefab/component semantics;
- changes to generic DW facts/relations/provenance contracts unless effective evidence proves a generic defect and a separate owner reopens them;
- general arbitrary-domain projection portability;
- H2 public/external knowledge portability;
- the mandatory fresh public-client trial owned by `WP-H1-GATE`.

## Work

1. Freeze the accepted H1-04 authority inputs to this projection by exact accepted identities/fingerprints.
2. Enumerate the authoritative source/catalogue universe independently of any DW output.
3. Define the minimal versioned H1 projection schema and adapter boundary.
4. Project the declared H1 identities/facts/relations/provenance into DW.
5. Build an independent completeness oracle over the authoritative universe and declared projection schema.
6. Prove source-open provenance for represented facts and relations.
7. Prove stale detection under source identity/fingerprint, accepted-authority and projection-schema changes.
8. Prove deterministic rebuild equality from the same accepted universe.
9. Add causal corruption controls that fail when projection content/provenance/currentness diverges unilaterally.
10. Publish one current projection identity plus lifecycle evidence sufficient for CTX routing admission.

## Acceptance

PASS requires all of the following:

- the completeness universe is derived from accepted H1-04 authority, never from the projection under test;
- the normalized projection is deterministically rebuildable from that universe;
- represented facts/relations retain exact source-open provenance;
- stale/corrupt/incomplete projection states are mechanically rejected;
- the shared DW/H0 boundary remains domain-neutral under equivalent/renamed H1 adapter semantics;
- no H1 product oracle or workpack contract is weakened, rewritten or made dependent on DW;
- absence of this projection still leaves later H1 able to proceed through `CTX → authoritative sources`;
- the result does not contaminate the required H1-GATE public-client bootstrap with private Juego2 context.

## Negative gates

FAIL if the projection defines the universe used to prove itself complete, if source and projection can drift while lifecycle remains GREEN, if H1 vocabulary/meaning leaks into the generic DW/H0 kernel, if the adapter becomes a hidden H1 product authority, or if H1-05+ is blocked merely because this non-product projection is unavailable.

## Evidence shape

Evidence should remain deterministic and remote: frozen accepted H1-04 input identities, independent universe enumeration, normalized projection output, completeness comparison, provenance checks, stale/corruption controls and deterministic rebuild equality. Do not rerun H1-04 Unity evidence merely to prove a derived projection.

## PASS consequence

A PASS creates the first H1 projection that CTX is allowed to classify `USE` when material and current. `WP-H1-05` becomes the first eligible real consumer and `WP-H1-06` the second materially different observation. H1 remains free to fall back to authoritative sources at any time.
