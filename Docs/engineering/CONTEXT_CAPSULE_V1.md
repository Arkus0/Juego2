# Accepted-Contract Capsule v1

Status: **CTX-02 CANDIDATE / PROCESS_ONLY**

## Purpose

Reduce repeated predecessor prose without moving semantic or proof authority. A capsule is a compact, mechanically checkable navigation layer over an already accepted boundary. It is never acceptance evidence by itself and never overrides live GitHub, an exact workpack, canonical result, proof/evidence, architecture source or concrete contradictory evidence.

Machine schema: `Docs/engineering/context-capsule-schema.json`  
Discoverability index: `Docs/engineering/context-capsules/index.json`  
Mechanical checker: `scripts/context-capsule-check.py`

This protocol becomes binding only after `WP-CTX-02` receives independent PASS, merges and completes DocSync. At that adoption boundary it is a **narrow amendment to the predecessor-read mechanics** in `AGENTS.md` and `WORKER_REVIEW_PROTOCOL.md` v1.8: a validated capsule may satisfy initial accepted-predecessor reconstruction until an escalation trigger fires. It does not amend ownership, proof thresholds, exact-SHA freeze/review, Reviewer independence, reopen rules or finalization.

## 1. Authority and usability

A capsule has exactly one authority marker:

```text
NON_AUTHORITATIVE_NAVIGATION_ONLY
```

A role may use a capsule as the **starting representation** of an accepted predecessor only when:

1. live GitHub does not show the predecessor reopened/revoked/superseded;
2. the capsule's reviewed candidate SHA, merge SHA and PASS review agree with independent accepted completion metadata;
3. every bound source still has the exact recorded Git blob SHA;
4. required structured coverage checks pass;
5. no mandatory escalation condition is active.

Failure of any condition means:

```text
RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES
```

Missing capsule is never negative evidence and never blocks ordinary source-based reconstruction.

## 2. Contract-scoped freshness, not global-main freshness

CTX-01's accepted-state index is a mutable main-derived navigation projection and therefore uses `docsync-first-parent-v1`. Capsules are different: they describe an immutable accepted contract identity.

An unrelated later `main` commit does **not** stale a capsule. A capsule becomes unusable when the accepted predecessor identity is reopened/superseded, a bound authoritative source changes, its exact reviewed/merge identity no longer agrees, its structured coverage becomes lossy, or live state contradicts it.

This distinction is deliberate. Requiring `capsule.source_main_sha == current main` would destroy reuse and recreate the token problem CTX-02 exists to solve.

## 3. Minimum capsule contents

Every capsule binds:

- exact reviewed candidate SHA, merge SHA and independent PASS review;
- an independent completion/identity source;
- exact authoritative source paths plus Git blob fingerprints;
- exported guarantees relevant to downstream ownership;
- explicit exclusions/non-claims;
- concrete predecessor-reopen conditions;
- mandatory escalation triggers;
- consumer hints only as navigation.

A capsule may add structured disposition coverage, directional/asymmetric semantics and mandatory non-compressible reads.

It may not cache transient PR/check/branch state.

## 4. Structured PA result chain

The PA track is cumulative by contract: later PA WPs require all earlier accepted findings. CTX-02 therefore explicitly treats the canonical PA result chain as a capsule family.

For each accepted `Docs/research/living-world/results/PA-NN.md` whose matching `WP-PA-NN` is `COMPLETE`, the capsule index must contain one `WP-PA-NN` capsule pointing to that result. Future PA DocSync adds the newly accepted capsule before capsule-chain coverage may be declared complete.

PA disposition compression is **status-preserving**, not a prose summary. The checker extracts the authoritative disposition table and requires exact key/status coverage. Mixed states such as `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT as authority` and `REJECT baseline` are preserved as distinct source statuses; they may not be flattened to a binary keep/drop decision.

Exclusions are first-class contract material. In particular, PA-03's rejection of default global/N-hop social target discovery carries the inherited PA-02 bounded-discovery guarantee and is protected exactly like a positive adoption.

If a later consumer needs the exact recommendation, rationale, scenario or nuance behind a row, it follows the exact source pointer. The capsule reduces initial reconstruction; it does not claim to contain the full research dossier.

## 5. Asymmetry survives compression

Directional semantics must remain directional. A capsule that carries accepted asymmetric meaning records the forward and reverse expressions separately with `must_remain_distinct=true`.

For PA-03:

```text
trust(A -> B) != trust(B -> A)
affinity(A -> B) != affinity(B -> A)
fear(A -> B) != fear(B -> A)
```

Collapsing either direction to a mirrored/symmetric representation is a validation failure.

## 6. CITY is boundary-only

CTX-02's acceptance requires a representative CITY consumer, but CITY production specifications are not capsule material.

`WP-CITY-03` therefore has only a **boundary capsule**: it tells CITY-04 which accepted owner/boundary it inherits and where the exact source lives. `Docs/production/CITY_PRODUCT_SEED.md` is explicitly `noncompressible=true` and remains a mandatory exact read for construction/measurement.

A CITY capsule must fail validation if it omits its non-compressible source. No capsule may substitute a short seed summary for geometry, route, parcel, scenario, seam or measurement specification.

## 7. H1 boundary coverage

Near-term H1 uses a capsule for the accepted HK-GATE boundary actually consumed by H1-02. CTX-02 does not bulk-migrate H0 history.

The capsule is sufficient to navigate the inherited/current ownership split; exact H0 proof is opened only when the current H1 claim, a contradiction or a reopen question makes it material. Its navigation pointers include the accepted gate verdict, proof matrix and residual-risk evidence so deepening does not require rediscovery.

## 8. Worker predecessor reconstruction

After adoption, a Worker may begin predecessor reconstruction from validated capsules plus the exact current WP and independently confirmed accepted dependency identity instead of mechanically rereading every historical narrative.

The Worker still records the normal `PREDECESSOR_CONTRACT_CHECK`. It must name:

- capsule(s) used;
- independently confirmed accepted identity;
- inherited guarantees consumed;
- current-WP-owned guarantees;
- escalations to original sources that were required;
- concrete reopen conditions.

A capsule never weakens mandatory proof, local-execution or architecture reads when the exact claim makes them material. A source explicitly marked `noncompressible=true` is always read by the consumer that needs that material detail.

## 9. Reviewer independence

A Reviewer may use capsules to navigate but never as proof authority or as a limit on review.

The Reviewer independently checks live accepted state and capsule validity. For a material inherited guarantee, lossy-summary suspicion, apparent predecessor-owned blocker or concrete contradiction, the Reviewer opens the authoritative source and challenges the boundary there.

Concrete contradictory evidence can reopen a predecessor even when a capsule says `ACCEPTED`. The checker supports an external accepted-state input; `state != ACCEPTED` or exact-SHA mismatch fails closed.

## 10. DocSync and future PA results

After a later PA result independently passes and merges, DocSync:

1. updates the accepted PA workpack completion metadata;
2. creates/updates the one capsule for that accepted result from the exact accepted identity/result;
3. binds reviewed SHA, merge SHA, PASS review and source blob SHA;
4. preserves the full disposition key/status set where present;
5. updates the capsule index;
6. runs `python3 scripts/context-capsule-check.py --self-test`, `python3 scripts/context-capsule-controls.py`, and `python3 scripts/context-capsule-check.py --audit-index --repo-root .` when capsule mechanics/coverage are affected;
7. only then may it claim accepted PA capsule-chain coverage complete.

If capsule production fails, DocSync may still reconstruct truth from authoritative sources but must not claim capsule coverage complete or silently omit the accepted result.

## 11. Fail-closed controls

The checker self-test and independent CTX-02 control harness exercise at least:

- stale reviewed-candidate SHA mismatch;
- changed source bytes / blob fingerprint mismatch;
- one material positive exported guarantee omitted while another remains;
- one material exclusion omitted while another remains;
- omitted material `REJECT` disposition;
- `LATER` reclassified as adopted/rejected;
- A→B/B→A collapse;
- predecessor external state changed to `REOPENED`;
- live accepted exact-SHA mismatch;
- CITY boundary capsule without mandatory non-compressible seed source.

The repository audit additionally checks every indexed representative capsule and discovers the accepted PA result-chain universe from repository state rather than trusting the capsule index to define its own completeness. The independent omission oracle is test-only evidence and is not promoted to a production semantic registry.

## 12. Adoption boundary

Before CTX-02 independent PASS + merge + successful DocSync, existing CTX-01/Worker/Reviewer predecessor-read rules remain authoritative and capsules in this candidate are evidence only. CTX-02 itself therefore used the pre-CTX-02 full predecessor reconstruction path.

After adoption, the capsule start path described here supersedes only the unconditional **initial-read list** in older Worker/Reviewer instructions. Capsules reduce initial context but do not change product/runtime semantics, proof thresholds, Reviewer independence, exact-SHA review, predecessor reopen rules or the H1 local-executor boundary. Any ambiguity about whether compression is safe resolves toward the authoritative source, never toward the capsule.
