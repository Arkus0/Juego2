# Accepted-Contract Capsule v1

Status: **CTX-02 CANDIDATE / PROCESS_ONLY**

## Purpose

Reduce repeated predecessor prose without moving semantic or proof authority. A capsule is a compact, mechanically checkable navigation layer over an already accepted boundary. It is never acceptance evidence by itself and never overrides live GitHub, an exact workpack, canonical result, proof/evidence, architecture source or concrete contradictory evidence.

Machine schema: `Docs/engineering/context-capsule-schema.json`  
Canonical discoverability index: `Docs/engineering/context-capsules/index.json`  
Mechanical checker: `scripts/context-capsule-check.py`  
Independent representative controls: `scripts/context-capsule-controls.py` + `scripts/context-capsule-omission-controls.py` + `scripts/context-capsule-pa-semantic-controls.py`  
Final circuit-breaker audit: `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`

This protocol becomes binding only after `WP-CTX-02` receives independent PASS, merges and completes DocSync. At that adoption boundary it is a **narrow amendment to predecessor-read mechanics**. It does not amend ownership, proof thresholds, exact-SHA freeze/review, Reviewer independence, reopen rules, product/runtime contracts or finalization.

## 1. Authority, trust boundary and usability

A capsule has exactly one authority marker:

```text
NON_AUTHORITATIVE_NAVIGATION_ONLY
```

A role may use a capsule as the **starting representation** of an accepted predecessor only when:

1. live GitHub does not show the predecessor reopened/revoked/superseded;
2. reviewed candidate SHA, merge SHA and PASS review agree with independent accepted completion metadata;
3. every bound source still has the exact recorded Git blob SHA and is outside the capsule/CTX-02-generated authority layer;
4. checker-owned identity, path, track/mode, coverage and structured-source selectors all match exactly;
5. required source-derived structured coverage checks pass;
6. the CTX-02 independent representative semantic controls for the selected near-term H1/CITY/PA boundaries pass;
7. no mandatory escalation condition is active.

Failure of any condition means:

```text
RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES
```

Missing capsule is never negative evidence and never blocks ordinary source-based reconstruction.

`VALID_NAVIGATION_ONLY` is deliberately **not** a generic semantic-equivalence verdict. CTX-02 separates validation into three classes:

1. **production/source-derived invariants** — exact accepted identity, independent source paths/fingerprints, checker-owned canonical roots/selectors, deterministic structured coverage and exact mandatory-read identities where the contract names them;
2. **representative semantic controls** — a small test-only external oracle validates the material content and material source inventory of the actual H1/CITY/PA representative capsules and defect-injects lossy/invented substitutions;
3. **human/escalation-only judgment** — arbitrary future natural-language completeness/equivalence, coordinated semantic-oracle rewrites and contested interpretation are not auto-proved; material ambiguity or contradiction opens authoritative sources and independent review.

The production checker must never grow a registry of approved natural-language meanings merely to turn class 2/3 into class 1.

### 1.1 Checker-owned selector rule

An audited artifact may repeat canonical selector/configuration values as descriptive/asserted metadata, but it may not choose the universe or oracle used to validate itself. The following are checker-owned or derived by the checker and any disagreement fails closed:

- canonical index path: `Docs/engineering/context-capsules/index.json`;
- canonical protocol path: `Docs/engineering/CONTEXT_CAPSULE_V1.md`;
- capsule path: `Docs/engineering/context-capsules/<capsule_id>.json`;
- exact canonical workpack path derived from `<capsule_id>`;
- track and content mode derived from the capsule/workpack identity;
- PA workpack discovery pattern;
- PA accepted-result template;
- PA disposition result path;
- PA disposition table section, key column and status column.

A caller may not substitute an alternate index for `--audit-index`. An index may not redirect an ID to a shadow capsule. A capsule may not relabel its track/content mode to suppress track-specific checks. A same-name workpack in another directory cannot replace the canonical identity source.

## 2. Contract-scoped freshness, not global-main freshness

CTX-01's accepted-state index is a mutable main-derived navigation projection and therefore uses `docsync-first-parent-v1`. Capsules describe an immutable accepted contract identity.

An unrelated later `main` commit does **not** stale a capsule. A capsule becomes unusable when the accepted predecessor identity is reopened/superseded, a bound authoritative source changes, its reviewed/merge identity no longer agrees, its structured coverage becomes lossy, a checker-owned selector no longer matches, a representative semantic control detects changed material content, or live state contradicts it.

Requiring `capsule.source_main_sha == current main` would destroy reuse and recreate the token problem CTX-02 exists to solve.

## 3. Minimum capsule contents and canonical identity

Every capsule binds:

- exact reviewed candidate SHA, merge SHA and independent PASS review;
- an independent completion/identity source;
- exact authoritative source paths plus Git blob fingerprints;
- exported guarantees relevant to downstream ownership;
- explicit exclusions/non-claims;
- concrete predecessor-reopen conditions;
- mandatory escalation triggers;
- consumer hints only as navigation.

A capsule may add structured disposition coverage, directional/asymmetric semantics and mandatory non-compressible reads. It may not cache transient PR/check/branch state.

The identity source must equal the checker-derived canonical workpack path for the capsule ID, not merely a same-named file somewhere under `Docs/workpacks/**`. `identity_source`, `authoritative_sources`, `disposition_source` and mandatory source reads may not point back into `Docs/engineering/context-capsules/**`, the capsule index, or CTX-02-generated evidence/protocol and then use those bytes to self-certify predecessor authority.

For the representative boundaries, the independent test-only oracle additionally pins the material authoritative-source inventory needed for safe navigation. Removing one such source while keeping the remaining fingerprints valid is a semantic-control RED, not silent narrowing.

## 4. Structured PA result chain

The PA track is cumulative by contract: later PA WPs require all earlier accepted findings. CTX-02 therefore treats the canonical PA result chain as a capsule family.

The **completion side defines the chain universe** using checker-owned conventions:

```text
workpack discovery = Docs/workpacks/PA/WP-PA-[0-9][0-9].md
canonical result   = Docs/research/living-world/results/PA-{NN}.md
```

The similarly named fields in `index.json` are assertions/documentation only. They must equal those checker-owned constants exactly before discovery begins. A narrow selector, broad selector, selector matching nothing, or alternate result template is an automatic RED.

Every canonical `WP-PA-NN.md` whose `Status` is `COMPLETE` independently enters the accepted PA inventory. For each such workpack, the canonical `PA-NN.md` result must exist and the canonical index must contain one `WP-PA-NN` capsule at `Docs/engineering/context-capsules/WP-PA-NN.json`. A result file, capsule, index entry and index selector may not disappear/change together and thereby remove the COMPLETE workpack from the universe being proved.

PA disposition compression is **status-preserving**, not a prose summary. The checker extracts the authoritative disposition table and requires exact key/status coverage. Mixed states such as `ADOPT ... / LATER ...`, `LATER / non-authoritative`, `REJECT as authority` and `REJECT baseline` remain distinct source statuses.

The table oracle itself is checker-owned. For current accepted PA capsules the canonical selectors are:

| Capsule | Section | Key column | Status column |
|---|---|---:|---:|
| `WP-PA-01` | `## 3. Donor → Juego2 disposition` | 0 | 2 |
| `WP-PA-02` | `## 3. Donor → Juego2 mechanism disposition` | 0 | 2 |
| `WP-PA-03` | `## 4. Minimal relationship vocabulary recommendation` | 0 | 1 |

`disposition_source.path` must be the checker-derived canonical `PA-NN.md`, its fingerprint must equal that canonical source binding, and its section/key/status selector must equal the checker-owned selector. A different internally coherent table inside the same result may not certify completeness.

A future COMPLETE PA result does **not** become capsule-chain COMPLETE merely because a capsule supplies a new section/column selector. DocSync must add a reviewed checker-owned selector in the checker first. Until then the capsule fails closed to `RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`; the accepted result itself remains authoritative and valid.

The representative PA consumer starts from PA-01 + PA-02 + PA-03, so test-only semantic controls cover material exports/exclusions/reopen/escalation content across all three current capsules. PA-03 additionally carries the directional-value oracle. This remains bounded representative evidence, not a production semantic registry for arbitrary future PA prose.

Exclusions are first-class contract material. In particular, PA-03's rejection of default global/N-hop social target discovery carries the inherited PA-02 bounded-discovery guarantee and is protected like a positive adoption.

If a later consumer needs the exact recommendation, rationale, scenario or nuance behind a row, it follows the exact source pointer. The capsule reduces initial reconstruction; it does not claim to contain the full research dossier.

## 5. Asymmetry survives compression

Directional semantics must remain directional. A capsule that carries accepted asymmetric meaning records forward and reverse separately with `must_remain_distinct=true`.

For PA-03:

```text
trust(A -> B) != trust(B -> A)
affinity(A -> B) != affinity(B -> A)
fear(A -> B) != fear(B -> A)
```

Production validation rejects malformed/collapsed structure. Because an invented pair can still remain syntactically distinct, the representative PA semantic control independently pins the actual accepted directional values. Arbitrary future directional interpretation remains subject to authoritative-source review.

## 6. CITY is boundary-only

CTX-02's acceptance requires a representative CITY consumer, but CITY production specifications are not capsule material.

`WP-CITY-03` therefore has only a boundary capsule. Its canonical ID derives `track=CITY` and `content_mode=boundary_summary`; the capsule may not relabel itself to disable CITY checks. `Docs/production/CITY_PRODUCT_SEED.md` is explicitly `noncompressible=true` and remains a mandatory exact read for construction/measurement.

The checker requires that exact path for the representative `WP-CITY-03` boundary. An arbitrary different valid/fingerprinted file cannot satisfy the obligation. No capsule may substitute a short seed summary for geometry, route, parcel, scenario, seam or measurement specification.

## 7. H1 boundary coverage

Near-term H1 uses a capsule for the accepted HK-GATE boundary actually consumed by H1-02. CTX-02 does not bulk-migrate H0 history.

The capsule is sufficient to navigate the inherited/current ownership split; exact H0 proof is opened when the current H1 claim, contradiction or reopen question makes it material. Its independently pinned navigation inventory includes the accepted gate contract, verdict, proof matrix and residual-risk evidence so deepening does not require rediscovery.

The representative H1 control validates material `id -> statement + source pointer` content and the expected material source inventory, not only IDs. Keeping IDs/fingerprints while inverting prose or silently deleting a proof pointer is therefore a control failure even though production code intentionally does not attempt generic prose equivalence.

## 8. Worker predecessor reconstruction

After adoption, a Worker may begin predecessor reconstruction from validated capsules plus the exact current WP and independently confirmed accepted dependency identity instead of mechanically rereading every historical narrative.

The Worker still records the normal `PREDECESSOR_CONTRACT_CHECK`, naming capsule(s) used, independently confirmed identity, inherited guarantees, current-WP-owned guarantees, required source escalations and concrete reopen conditions.

A capsule never weakens mandatory proof, local-execution or architecture reads when the exact claim makes them material. A source explicitly marked `noncompressible=true` is always read by the consumer that needs that material detail.

## 9. Reviewer independence and human escalation

A Reviewer may use capsules to navigate but never as proof authority or as a limit on review.

For a material inherited guarantee, lossy-summary suspicion, apparent predecessor-owned blocker, coordinated change to capsule and semantic-control fixture, or concrete contradiction, the Reviewer opens the original authoritative source and challenges the boundary there.

Concrete contradictory evidence can reopen a predecessor even when a capsule says `ACCEPTED`. The checker supports an external accepted-state input; `state != ACCEPTED` or exact-SHA mismatch fails closed when that external state is supplied.

A representative semantic GREEN is evidence that selected CTX-02 examples survived known compression-loss classes. It does not prove arbitrary future prose exhaustiveness and does not authorize a Reviewer to stop challenging source semantics.

## 10. DocSync and future PA results

After a later PA result independently passes and merges, DocSync:

1. updates the accepted PA workpack completion metadata;
2. creates/updates its capsule from the exact accepted identity/result;
3. binds reviewed SHA, merge SHA, PASS review and source blob SHA;
4. preserves the full disposition key/status set where present;
5. adds/updates a reviewed checker-owned canonical disposition selector before claiming capsule coverage for a new PA ID;
6. updates the canonical capsule index without changing its checker-owned discovery/template values;
7. runs the full CTX-02 validation surface:

```bash
python3 scripts/context-capsule-check.py --self-test
python3 scripts/context-capsule-controls.py
python3 scripts/context-capsule-omission-controls.py
python3 scripts/context-capsule-pa-semantic-controls.py
python3 scripts/context-capsule-check.py --audit-index --repo-root .
```

8. only then may it claim accepted PA capsule-chain coverage complete.

If capsule production, checker-owned selector adoption or any required independent control fails, DocSync may still reconstruct truth from authoritative sources but must not claim capsule coverage complete or silently omit the accepted result.

Future capsules whose natural-language semantics are not covered by the representative test oracle remain usable only under the class-3 rule: material uncertainty/escalation requires authoritative reconstruction and independent judgment.

## 11. Fail-closed controls

The checker self-test and independent CTX-02 control harnesses exercise at least:

- stale reviewed-candidate SHA mismatch;
- review id with non-PASS verdict;
- identity source redirected to a same-name but non-canonical workpack;
- authoritative source redirected into capsule/CTX-02 generated evidence;
- changed source bytes / blob fingerprint mismatch;
- representative material source omitted while remaining sources stay valid;
- one material positive guarantee omitted while another remains, with production shape/source GREEN and independent semantic oracle RED;
- symmetric material exclusion omission;
- same-ID/pointer/fingerprint guarantee or exclusion statement inversion;
- representative reopen/escalation substitution with opposite non-empty prose;
- PA-01/02/03 semantic fixtures across the cumulative representative PA surface;
- COMPLETE `WP-PA-04` retained while canonical result/capsule/index entry are jointly absent;
- the same PA-04 omission plus narrowed `workpack_glob` attempting to hide it;
- PA workpack selector narrowed, broadened or changed to match nothing;
- PA result template redirected to an alternate tree;
- alternate index supplied to `--audit-index`;
- canonical index entry redirected to an alternate valid-looking capsule file;
- `WP-CITY-03` relabelled to another track while its mandatory seed read is deleted;
- content mode changed to disable the canonical track behavior;
- omitted material PA disposition and status reclassification;
- PA disposition source rebound to another result;
- PA table section or status column redirected to a different valid-looking table/column;
- A→B/B→A collapse and changed-but-still-distinct representative direction;
- predecessor external state changed to `REOPENED` and live accepted SHA mismatch;
- CITY boundary capsule without the exact mandatory non-compressible `CITY_PRODUCT_SEED.md` source;
- workflow/role guide that claims the “full CTX-02 validation surface” while omitting any canonical command.

Integration-class mutations use the real production CLI path `--audit-index`; controls are not allowed to prove only that a helper mutation occurred.

The repository audit discovers the PA chain from checker-owned COMPLETE-workpack conventions, not from result files, capsules or index-controlled selectors. The independent representative oracles are test-only evidence; production audit + independent controls together form CTX-02's mechanical surface, and unresolved semantics stay with escalation + independent Reviewer judgment.

The complete trust-boundary, invariant, mutation and Reviewer pre-mortem matrix is durable at `Docs/evidence/CTX-02/FINAL_CIRCUIT_BREAKER_AUDIT.md`.

## 12. Adoption boundary

Before CTX-02 independent PASS + merge + successful DocSync, existing CTX-01/Worker/Reviewer predecessor-read rules remain authoritative and capsules in this candidate are evidence only. CTX-02 itself therefore used the pre-CTX-02 full predecessor reconstruction path.

After adoption, the capsule start path supersedes only the unconditional initial-read list in older Worker/Reviewer instructions. Capsules reduce initial context but do not change product/runtime semantics, proof thresholds, Reviewer independence, exact-SHA review, predecessor reopen rules or the H1 local-executor boundary. Any ambiguity about whether compression is safe resolves toward authoritative sources, never toward the capsule.
