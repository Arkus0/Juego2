# H1 CTX↔DW selective-adoption plan

Status: **PLANNING INPUT / NON-PRODUCT-SEMANTIC**
Depends on for adoption: accepted `WP-CTX-DW-GATE`
Does not block: `WP-H1-03`, `WP-H1-03A`

## Purpose

Record how an accepted CTX↔DW composition should be introduced into H1 without turning context infrastructure into an artificial product dependency, without assuming H1 projection semantics that DW-GATE did not prove, and without changing any H1 product workpack claim.

## Decision

`WP-H1-03` and `WP-H1-03A` continue on the existing H1 dependency chain. They do not wait for CTX↔DW synergy work.

For those authority/lifecycle workpacks, the expected route is usually:

```text
CTX → H1/H0 authoritative contracts/code/evidence
DW → NOT_MATERIAL unless concrete evidence says otherwise
```

Correct DW abstention is preferred to loading a design corpus that has no causal role in host/workspace or Editor lifecycle correctness.

The two real H1 adoption boundaries are now explicit contracts:

```text
H1-04 PASS
    ↓
WP-CTX-DW-H1-01 — H1 projection bootstrap + lifecycle
    ↓
H1-05 — first eligible real consumer
    ↓
H1-06 — second distinct consumer shape
    ↓
WP-CTX-DW-H1-02 — selective-adoption validation + H1-07+ disposition
```

Neither CTX↔DW H1 workpack blocks H1 product progress. In a strictly sequential owner flow it is recommended to run them at the points above; if unavailable, H1 falls back to authoritative sources.

## Authority split

CTX and DW do not sit in one universal authority ladder.

For represented semantic/content facts:

```text
authoritative accepted source > DW derived projection > CTX semantic summary/navigation
```

For routing/process:

```text
accepted CTX effective read-set / mandatory-read / escalation authority > DW materiality advice
```

DW can improve materiality targeting, but it cannot cancel a CTX-mandated source read or escalation. A material contradiction forces source-open, rebuild where applicable, or fail closed.

## H1-04 — authority and projection bootstrap, not H1-real DW consumption

`WP-H1-04` is the first adoption point for the real Quaternius Source and the H1 catalogue/identity authority. That means the H1-real DW universe does **not** exist as an accepted projection before H1-04 proves and accepts it.

H1-04 therefore runs source-first:

```text
CTX
  → authoritative H1/H0 contracts
  → human-approved Quaternius Source boundary
  → H1-04 deterministic Unity/catalogue evidence
```

H1-04 may consult pre-existing DW information only if independently material, but no `USE` classification may pretend that DW already contains the catalogue/source universe H1-04 is creating. The H1-04 product oracle remains its deterministic Unity/catalogue evidence. CTX↔DW does not change that claim.

## `WP-CTX-DW-H1-01` — post-H1-04 projection lifecycle

After H1-04 PASS and acceptance, `WP-CTX-DW-H1-01` projects that accepted universe into DW as a separate non-product step. It is owned by the CTX↔DW adoption/projection boundary, not by the H1-04 product Worker and not by shared H0/DW kernel semantics.

Before any later H1 task may classify this projection as `USE`, `WP-CTX-DW-H1-01` must establish:

- **independent source universe:** enumerate the accepted H1-04 source-adoption and catalogue/identity authority/evidence independently of the DW output;
- **projection owner/adapter/schema:** map that universe into generic DW facts/relations/provenance while keeping H1/domain meaning consumer-owned;
- **completeness oracle:** compare the independent authoritative universe with projected identities, declared represented fields/relations, cardinality/targets and provenance; the DW projection cannot define the universe used to prove itself complete;
- **provenance:** bind projected records to exact accepted source/evidence identities, hashes/fingerprints and projection/schema version sufficient for source-open;
- **staleness:** a material source identity/fingerprint, accepted H1-04 authority or projection-schema change invalidates currentness until rebuild/revalidation;
- **rebuild:** the same accepted universe reproduces the same normalized projection, while stale/corrupted projection evidence turns RED;
- **routing admission:** `USE` is legal only for the exact current projection whose lifecycle evidence is GREEN.

If that projection is missing, stale or not yet accepted, later H1 work does **not** wait for it. CTX routes directly to authoritative sources and DW becomes `OPTIONAL` or `NOT_MATERIAL`. This preserves H1 progress and prevents context infrastructure from becoming a hidden product prerequisite.

## First eligible real consumers

### H1-05 — managed scenes

H1-05 is the first eligible real H1 consumer after the post-H1-04 projection exists and is current. DW may help navigate catalogue/design relations when material, but the H1-05 product oracle and source authority remain unchanged.

If the projection is not ready or not material, H1-05 proceeds source-first.

### H1-06 — assets/prefabs

H1-06 is the second planned observation point because source-asset, prefab and managed-derivative relationships form a materially different graph from H1-04 inventory/identity and H1-05 scene use.

A useful H1-05 observation is not enough to assume H1-06 usefulness; route by claim evidence and current projection state.

## `WP-CTX-DW-H1-02` — selective-adoption validation

After H1-05 and H1-06 have each produced accepted product evidence, `WP-CTX-DW-H1-02` consumes those real observations and decides what the H1-07+ routing defaults should be.

It must not create duplicate shadow Workers or replay H1-05/H1-06 solely to manufacture adoption metrics. It evaluates accepted artifacts from the two materially different consumer shapes, preserves blocker/source discoverability, verifies source-open provenance and records bounded context/cognitive-load effects.

Its valid outcome may be selective: some H1 claims can be `USE`, others `OPTIONAL`, and others `NOT_MATERIAL`. It may not force broad DW adoption merely because the projection exists.

## Later H1

From H1-07 onward, CTX classifies DW as `USE`, `OPTIONAL` or `NOT_MATERIAL` for the active claim, informed by `WP-CTX-DW-H1-02` once accepted.

The classification is routing advice only. It cannot shrink CTX mandatory reads, and a Worker or Reviewer may always open deeper authoritative sources.

`USE` requires a current projection lifecycle for the exact universe consumed. Otherwise the route falls back to authority rather than silently creating or repairing projection machinery inside the H1 product workpack.

## Lightweight adoption observations

Normal H1 work may record, when materially useful:

- CTX→DW classification used;
- exact projection identity/currentness when `USE` is selected;
- authoritative source escalations;
- whether DW materially shortened navigation to a fact/relation/provenance anchor;
- material blockers discovered by Worker or Reviewer;
- obvious cases where DW correctly abstained;
- obvious context/cognitive-load changes.

Do not create duplicate shadow Workers or replay every workpack solely to obtain adoption metrics.

A CTX↔DW defect found during H1 belongs to the causal routing/projection owner unless the evidence actually falsifies an H1 guarantee. Do not patch H1 semantics merely to make context infrastructure pass.

## H1-GATE — preserve the required fresh public-client trial

The mandatory fresh independent AI-agent trial owned by `WP-H1-GATE` must remain a clean test of the public H1/Arkus discovery boundary.

Its bootstrap remains the accepted H1-GATE one: fixed public launch-profile instructions, public MCP discovery and returned schemas, no implementation-source/private product call. Do **not** pre-seed that trial with Juego2-private CTX capsules, DW projections, proving-ground history or routing knowledge that a public client would not have.

CTX↔DW may be observed separately as a composition/usability probe, including against the same accepted product surface, but that observation:

- is not the required H1-GATE fresh-agent trial;
- is not part of H1-GATE's deterministic or agent acceptance oracle;
- cannot repair a failure of public discovery/schemas;
- cannot justify a private path or internal Juego2 knowledge in the mandatory trial.

If a future H2/public-product plan chooses to publish and version CTX/DW as part of an external consumer boundary, that is a separate reviewed portability claim. The H2 planner must author the appropriate `CTX-DW-H2-*` workpack at that time; do not pre-author it before the H1 adoption evidence exists.

## Non-claims

This plan does not:

- add a dependency edge from CTX↔DW to H1-03 or H1-03A;
- make H1-04 depend on a pre-existing H1 catalogue projection;
- block H1-05/H1-06 if `WP-CTX-DW-H1-01` is unavailable;
- make H1-07+ product progress depend on `WP-CTX-DW-H1-02`;
- change H1 architecture, Unity authority or workpack acceptance criteria;
- assert that DW is useful for every H1 claim;
- authorize domain knowledge to become H0/H1/DW-kernel generic semantics;
- let DW routing advice override CTX mandatory reads/escalation;
- contaminate the required H1-GATE public-client trial with private Juego2 context;
- make context/token savings an H1 product PASS criterion.
