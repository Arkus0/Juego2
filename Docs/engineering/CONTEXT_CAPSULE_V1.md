# Accepted-Contract Capsule v1

Status: **CTX-02 CANDIDATE / PROCESS_ONLY**

## Purpose

Reduce repeated predecessor prose without moving semantic or proof authority. A capsule is a compact, mechanically checkable navigation layer over an already accepted boundary. It is never acceptance evidence by itself and never overrides live GitHub, an exact workpack, canonical result, proof/evidence, architecture source or concrete contradictory evidence.

Machine schema: `Docs/engineering/context-capsule-schema.json`  
Discoverability index: `Docs/engineering/context-capsules/index.json`  
Mechanical checker: `scripts/context-capsule-check.py`  
Independent representative controls: `scripts/context-capsule-controls.py` + `scripts/context-capsule-omission-controls.py` + `scripts/context-capsule-pa-semantic-controls.py`

This protocol becomes binding only after `WP-CTX-02` receives independent PASS, merges and completes DocSync. At that adoption boundary it is a **narrow amendment to the predecessor-read mechanics** in `AGENTS.md` and `WORKER_REVIEW_PROTOCOL.md` v1.8: a validated capsule may satisfy initial accepted-predecessor reconstruction until an escalation trigger fires. It does not amend ownership, proof thresholds, exact-SHA freeze/review, Reviewer independence, reopen rules or finalization.

## 1. Authority and usability

A capsule has exactly one authority marker:

```text
NON_AUTHORITATIVE_NAVIGATION_ONLY
```

A role may use a capsule as the **starting representation** of an accepted predecessor only when:

1. live GitHub does not show the predecessor reopened/revoked/superseded;
2. the capsule's reviewed candidate SHA, merge SHA and PASS review agree with independent accepted completion metadata;
3. every bound source still has the exact recorded Git blob SHA and is outside the capsule/CTX-02-generated authority layer;
4. required source-derived structured coverage checks pass;
5. the CTX-02 independent representative semantic controls for the selected near-term H1/CITY/PA boundaries pass;
6. no mandatory escalation condition is active.

Failure of any condition means:

```text
RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES
```

Missing capsule is never negative evidence and never blocks ordinary source-based reconstruction.

`VALID_NAVIGATION_ONLY` is deliberately **not** a generic semantic-equivalence verdict. CTX-02 separates validation into three classes:

1. **production/source-derived invariants** — exact accepted identity, independent source paths/fingerprints, deterministic structured coverage and exact mandatory-read identities where the contract names them;
2. **representative semantic controls** — a small test-only external oracle validates the material content of the actual H1/CITY/PA representative capsules and defect-injects lossy/invented substitutions;
3. **human/escalation-only judgment** — arbitrary future natural-language completeness/equivalence and contested interpretation are not auto-proved; material ambiguity or contradiction opens authoritative sources and independent review.

The production checker must never grow a registry of approved natural-language meanings merely to turn class 2/3 into class 1.

## 2. Contract-scoped freshness, not global-main freshness

CTX-01's accepted-state index is a mutable main-derived navigation projection and therefore uses `docsync-first-parent-v1`. Capsules are different: they describe an immutable accepted contract identity.

An unrelated later `main` commit does **not** stale a capsule. A capsule becomes unusable when the accepted predecessor identity is reopened/superseded, a bound authoritative source changes, its exact reviewed/merge identity no longer agrees, its structured coverage becomes lossy, a representative semantic control detects changed material content, or live state contradicts it.

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

The accepted identity source must be the matching external canonical workpack under `Docs/workpacks/**/<capsule_id>.md`. `identity_source`, `authoritative_sources`, `disposition_source` and mandatory source reads may not point back into `Docs/engineering/context-capsules/**`, the capsule index, or CTX-02-generated evidence/protocol and then use those bytes to self-certify predecessor authority.

## 4. Structured PA result chain

The PA track is cumulative by contract: later PA WPs require all earlier accepted findings. CTX-02 therefore explicitly treats the canonical PA result chain as a capsule family.

The **completion side defines the chain universe**. Every canonical `Docs/workpacks/PA/WP-PA-NN.md` whose `Status` is `COMPLETE` independently enters the accepted PA inventory. For each such workpack, the canonical `Docs/research/living-world/results/PA-NN.md` must exist and the capsule index must contain one `WP-PA-NN` capsule pointing to that result. A result file, capsule and index entry may not disappear together and thereby remove the COMPLETE workpack from the universe being proved. Future PA DocSync adds the newly accepted result capsule before capsule-chain coverage may be declared complete.

PA disposition compression is **status-preserving**, not a prose summary. The checker extracts the authoritative disposition table and requires exact key/status coverage. Mixed states such as `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT as authority` and `REJECT baseline` are preserved as distinct source statuses; they may not be flattened to a binary keep/drop decision.

The disposition oracle is valid only because the PA-chain checker independently discovers COMPLETE `WP-PA-NN` workpacks first, derives the required canonical `PA-NN.md` result from that completion-side inventory, and requires `disposition_source.path` and fingerprint to be that same result binding. A different internally coherent table may not certify the accepted PA result.

The representative PA consumer starts from the accepted PA-01 + PA-02 + PA-03 family, so the test-only semantic controls cover material exports/exclusions/reopen/escalation content across **all three current PA capsules**. PA-03 additionally carries the directional-value oracle. This remains bounded representative evidence, not a production registry for arbitrary future PA prose.

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

Production validation rejects collapse or malformed structure. Because a different invented pair can still remain syntactically distinct, the representative PA semantic control independently pins the actual accepted directional values. Arbitrary future directional interpretation remains subject to authoritative-source review.

## 6. CITY is boundary-only

CTX-02's acceptance requires a representative CITY consumer, but CITY production specifications are not capsule material.

`WP-CITY-03` therefore has only a **boundary capsule**: it tells CITY-04 which accepted owner/boundary it inherits and where the exact source lives. `Docs/production/CITY_PRODUCT_SEED.md` is explicitly `noncompressible=true` and remains a mandatory exact read for construction/measurement.

The production checker requires that exact path for the representative `WP-CITY-03` boundary; an arbitrary different valid/non-compressible file cannot satisfy the obligation. No capsule may substitute a short seed summary for geometry, route, parcel, scenario, seam or measurement specification.

## 7. H1 boundary coverage

Near-term H1 uses a capsule for the accepted HK-GATE boundary actually consumed by H1-02. CTX-02 does not bulk-migrate H0 history.

The capsule is sufficient to navigate the inherited/current ownership split; exact H0 proof is opened only when the current H1 claim, a contradiction or a reopen question makes it material. Its navigation pointers include the accepted gate verdict, proof matrix and residual-risk evidence so deepening does not require rediscovery.

The representative H1 control validates actual material `id -> statement + source pointer` content, not only statement IDs. Keeping an ID and valid source fingerprints while inverting/inventing the statement is therefore a control failure even though the production checker intentionally does not attempt generic prose equivalence.

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

A representative semantic GREEN is evidence that the selected CTX-02 examples survived known compression-loss classes. It does not authorize a Reviewer to stop challenging source semantics when the current claim makes them material.

## 10. DocSync and future PA results

After a later PA result independently passes and merges, DocSync:

1. updates the accepted PA workpack completion metadata;
2. creates/updates the one capsule for that accepted result from the exact accepted identity/result;
3. binds reviewed SHA, merge SHA, PASS review and source blob SHA;
4. preserves the full disposition key/status set where present;
5. updates the capsule index;
6. runs `python3 scripts/context-capsule-check.py --self-test`, `python3 scripts/context-capsule-controls.py`, `python3 scripts/context-capsule-omission-controls.py`, `python3 scripts/context-capsule-pa-semantic-controls.py`, and `python3 scripts/context-capsule-check.py --audit-index --repo-root .` when capsule mechanics/coverage are affected;
7. only then may it claim accepted PA capsule-chain coverage complete.

If capsule production or any required independent control fails, DocSync may still reconstruct truth from authoritative sources but must not claim capsule coverage complete or silently omit the accepted result.

Future capsules whose natural-language semantics are not covered by the representative test oracle remain usable only under the class-3 rule: any material uncertainty/escalation requires authoritative reconstruction and independent judgment. CTX-02 does not require extending a production semantic registry.

## 11. Fail-closed controls

The checker self-test and independent CTX-02 control harnesses exercise at least:

- stale reviewed-candidate SHA mismatch;
- completion identity source redirected away from the canonical external workpack;
- authoritative source redirected back into capsule/CTX-02 generated evidence;
- changed source bytes / blob fingerprint mismatch;
- one material positive exported guarantee omitted while another remains, with production shape/source validation GREEN and the real representative semantic oracle RED;
- one material exclusion omitted while another remains, with the same representative semantic oracle RED;
- representative guarantee statement inverted/invented while ID, source pointer and source fingerprints remain unchanged;
- symmetric representative exclusion/non-claim statement inversion/invention;
- representative reopen-condition and escalation-trigger substitution with structurally valid but materially opposite/invented text;
- PA-01/PA-02/PA-03 actual semantic fixtures across the representative cumulative PA start surface;
- a COMPLETE `WP-PA-04` retained while its canonical result and capsule are jointly absent, which must RED from the completion-side PA universe;
- omitted material `REJECT` disposition;
- `LATER` reclassified as adopted/rejected;
- PA disposition source rebound to a different internally coherent result table;
- A→B/B→A collapse;
- representative directional value changed to a different but still structurally asymmetric expression;
- predecessor external state changed to `REOPENED`;
- live accepted exact-SHA mismatch;
- CITY boundary capsule without the exact mandatory non-compressible `CITY_PRODUCT_SEED.md` source.

The repository audit additionally checks every indexed representative capsule and discovers the accepted PA result-chain universe from COMPLETE canonical PA workpacks rather than trusting either result files or the capsule index to define their own completeness.

The independent representative semantic oracles are test-only evidence for the selected H1/CITY boundaries and the current PA-01/02/03 cumulative representative chain. They validate actual material content rather than only IDs, but they are **not** imported by the production checker, do not become semantic authority, and do not claim generic natural-language proof. Production audit + independent controls together form CTX-02's mechanical validation surface; unresolved semantics stay with escalation + independent Reviewer judgment.

The circuit-breaker trust analysis that defines this split is durable at `Docs/evidence/CTX-02/TRUST_BOUNDARY_REAUDIT.md`.

## 12. Adoption boundary

Before CTX-02 independent PASS + merge + successful DocSync, existing CTX-01/Worker/Reviewer predecessor-read rules remain authoritative and capsules in this candidate are evidence only. CTX-02 itself therefore used the pre-CTX-02 full predecessor reconstruction path.

After adoption, the capsule start path described here supersedes only the unconditional **initial-read list** in older Worker/Reviewer instructions. Capsules reduce initial context but do not change product/runtime semantics, proof thresholds, Reviewer independence, exact-SHA review, predecessor reopen rules or the H1 local-executor boundary. Any ambiguity about whether compression is safe resolves toward the authoritative source, never toward the capsule.
