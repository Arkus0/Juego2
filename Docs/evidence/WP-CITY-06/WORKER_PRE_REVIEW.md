# WP-CITY-06 — Strict Worker pre-review

WP: `WP-CITY-06 — Interiors + layered discovery`  
Contract: `Docs/workpacks/CITY/WP-CITY-06.md`  
Baseline SHA: `632a63c089f844313e567046b3df541e435d75d4`  
PR: `#90`  
Pre-review semantic candidate HEAD before this report: `abb8d424164c2ac9d7aea747a00a06bfc284b7b8`  
Worker: `ChatGPT GPT-5.6 Sol`  
fail_cycle: **0**

`WORKER_PRE_REVIEW: CLEAN`  
`WORKER_PRE_REVIEW_FINDINGS_FIXED: 2`  
`WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-CITY-06/WORKER_PRE_REVIEW.md`

This is Worker quality-gate evidence only. It is not an independent PASS and does not authorize merge.

## 1. Surfaces reviewed

The strict challenge re-read and cross-checked:

- `Docs/workpacks/CITY/WP-CITY-06.md`;
- `Docs/workpacks/CITY/README.md`;
- accepted `Docs/production/CITY_LOCATION_PROGRAMME.md` (CITY-02);
- accepted `Docs/production/CITY_ENVIRONMENT_GRAMMAR.md` + discovery-requirements projection (CITY-05);
- relevant accepted CITY-00/CITY-01 geography/access invariants inherited through those owners;
- ART authority boundary;
- accepted PA programme only as a future-owner map;
- `Docs/engineering/WORKER_REVIEW_PROTOCOL.md` v1.7;
- the complete PR #90 baseline→candidate diff existing before this report:
  - `Docs/production/CITY_INTERIORS_DISCOVERY.md`;
  - `Docs/evidence/WP-CITY-06/WORKER_PLAN.md`;
  - `Docs/evidence/WP-CITY-06/INTERIOR_COVERAGE_AUDIT.md`;
  - `Docs/evidence/WP-CITY-06/DISCOVERY_CAUSALITY_AUDIT.md`;
  - `Docs/evidence/WP-CITY-06/DEPTH_SHELL_COMPATIBILITY_AUDIT.md`.

No code, Unity, asset, catalogue or runtime files are changed.

## 2. Predecessor-contract check

`Docs/evidence/WP-CITY-06/WORKER_PLAN.md` reconstructs the direct accepted predecessor:

- CITY-05 candidate `10d1528b0354b16a614fb10a3933a25b32f15f28`;
- independent PASS review `#5267776704`;
- implementation merge `47909a72eb6d38332f62e9c01426c8cd40e1863b`;
- DocSync merge `cff6d4d0d40786dd1c002a8cc46e768b478ee3cc`;
- residual reconciliation / Worker baseline `632a63c089f844313e567046b3df541e435d75d4`.

The check correctly consumes rather than re-proves:

- CITY-00 geography/crossing truth;
- CITY-01 movement/access graph;
- CITY-02 37-row place programme, A–D/S/I orthogonality, I-depth backlog and capacity boundary;
- CITY-05 exterior shell/family mappings and fail-closed access-role binding;
- ART setting/visual authority.

Concrete reopen conditions are recorded for CITY-05/02/01/00/ART rather than weakening predecessors locally.

`PREDECESSOR_CONTRACT_CHECK: VALID`

## 3. Findings discovered and repaired before freeze

### Finding 1 — `S2..S4` coverage was semantically present but not exhaustively audited

The first candidate allocated all 14 `I1..I3` interiors and named exterior-led second layers, but the WP wording is stronger: it owns interior/access expectations for every CITY-02 `S2..S4` location.

A Reviewer could reasonably ask whether an omitted `S2/S3 I0` place sat outside the asserted coverage universe.

Repair:

- added `DEPTH_SHELL_COMPATIBILITY_AUDIT.md`;
- independently reconstructed the accepted `S2..S4` universe as **18** rows;
- proved `1×S4 + 7×S3 + 10×S2 = 18/18` covered;
- showed that the four `I0` members are deliberately exterior-led rather than silently promoted:
  - `loc.casco.shared_court`;
  - `loc.plaza.market`;
  - `loc.barrio.lavadero`;
  - `loc.ensanche.shared_garden`.

Negative control: making shared court enterable merely because it is S3 fails the S/I orthogonality boundary.

Disposition: **FIXED at evidence/coverage boundary; no product predecessor changed.**

### Finding 2 — CITY-05 shell compatibility was too implicit

The first candidate preserved all required access roles but largely relied on prose that the new interior families sat “behind” accepted shells. The WP acceptance explicitly requires detailed interior promises to remain compatible with CITY-05 shells.

Repair:

- added a **14/14** place-by-place mapping from each inherited `I1..I3` row back to its accepted CITY-05 shell/building posture;
- verified CITY-06 does not require a new exterior family, route, parcel class or automatic extra storey;
- added causal controls against hero-floor invention, I1 role→room multiplication, rural-shell urbanisation and I0 room rescue;
- kept metric/Unity fit honestly deferred; realized contradiction triggers predecessor reopen rather than role weakening.

Disposition: **FIXED at evidence/compatibility boundary; no exterior semantic owner changed.**

## 4. Acceptance challenge

### A. Interior-depth allocation

Recomputed inherited interior backlog:

```text
I3: 1
I2: 6
I1: 7
TOTAL I1-I3: 14
```

Candidate allocates **14/14**.

Key checks:

- `loc.casco.bar` remains the only I3/hero commitment;
- every I2 gets multiple meaningful zones/thresholds but no mandatory basement/roof/secret room;
- every I1 remains one primary meaningful enclosed zone plus at most bounded support treatment;
- role multiplicity does not become room multiplicity;
- `loc.ribera.service_yard` remains exterior-led with one shallow support room maximum sufficient;
- I0 remains no enclosed-interior promise.

Result: **CLEAN**.

### B. Required access roles

Cross-checked the 14 interior rows against accepted CITY-05 role sets.

The candidate never substitutes:

- vertical for semi-private;
- storage/staff for private;
- rear for service;
- court/landing for semi-private;
- physical walkability for ordinary-public route authority.

Controls specifically reject the previously sensitive CITY-05 families:

- everyday shop without required service;
- bar without required semi-private;
- service yard publicised for convenience.

Result: **CLEAN**.

### C. Reuse versus bespoke/hero inflation

Candidate defines eight bounded topology/use families and one bar-only hero overlay. The same social family serves Puerto worker social and the bar base; shallow retail/work-support families cover multiple places. Single-current-use families remain topology categories with reviewed promotion rules rather than narrative one-offs.

No rule creates one custom room graph per POI, and hero treatment is an overlay rather than a second architectural universe.

Result: **CLEAN**.

### D. Major-district second-layer coverage

All nine major district families have at least one grounded second layer:

1. Casco Viejo;
2. Plaza/Ayuntamiento;
3. Calle Mayor;
4. Barrio Alto;
5. Ensanche;
6. Ribera/Talleres;
7. Puerto Fluvial;
8. Entrada/Carretera;
9. La Vega.

A layer may be access hierarchy, material/work provenance, social/temporal context, institutional/historical context or rare cultural context. No district is required to have underground/roof/secret geometry.

Result: **CLEAN**.

### E. Authored / systemic / hybrid truth boundary

Candidate introduces explicit route states:

- `AUTHORED_SPATIAL_NOW`;
- `FUTURE_OWNER_CONDITIONAL`;
- `NOT_A_ROUTE`.

Systemic examples name future owner categories from the accepted PA plan but never treat those plans/results as runtime proof.

Negative controls reject:

- duplicate presentations counted as independent routes;
- “a neighbour tells you” with no causal owner;
- PA plan/PA research acceptance reclassified as implemented runtime;
- hidden engine truth exposed as actor/player knowledge;
- publicising service/private access merely to increase route count.

`MULTI_ROUTE_READY` is explicitly preproduction readiness, not gameplay proof.

Result: **CLEAN**.

### F. Contract-requested discovery motifs

All requested motifs have bounded ownership:

- follow actor → future daily-life/agency runtime;
- invitation → future social/access semantics;
- key/access → threshold reserved, literal key/inventory mechanics **not required** here;
- overheard information → future knowledge/rumour/investigation/dialogue semantics;
- document → authored surface may exist, information truth/effect later-owned;
- schedule change → future daily-life/consequence owner;
- municipal consequence → future governance owner;
- return-after-change → future memory/material/player-agency/governance owner depending on cause.

No motif needs quest scripting or a privileged player-only state path.

Result: **CLEAN**.

### G. Quiet / ordinary content

The candidate explicitly permits quiet places to remain ordinary and rejects mandatory clue/event generation on:

- Vega quiet paseo;
- Barrio lavadero;
- upper residential frontage;
- ordinary Puerto sheds;
- scenic D/S0 fabric.

“Second layer” is not used as a secret quota.

Result: **CLEAN**.

### H. Rare martial/cinema strand

Candidate makes this strand optional and non-load-bearing:

- zero opportunities in the retained seed is valid and neutral;
- if present, at most one primary opportunity in the selected seed;
- it attaches to an otherwise coherent ordinary place;
- it cannot explain governance, economy, crossings, every family or every secret;
- missing it cannot break city coherence;
- final narrative/cultural truth remains later-owned.

Negative controls reject universal hidden-society explanation and district-wide exotic theme replacement.

Result: **CLEAN**.

### I. CITY-03 handoff

CITY-03 receives comparison constraints, not a preselected seed:

- included I1–I3 commitments and interior families;
- exterior-led second layers;
- required role sets;
- authored versus future-conditional route status;
- ordinary/quiet no-secret content;
- optional extraordinary-strand posture;
- predecessor reopen signals;
- minimum selected-seed depth mix.

The candidate does not draw the seed boundary, choose candidate geography or write CITY-03's scenario pack.

Result: **CLEAN**.

## 5. Negative-gate challenge summary

The candidate fails closed against the WP's named negative gates:

| Negative class | Candidate barrier |
|---|---|
| every building needs a secret | I0 protection + secret-quota rejection |
| hidden-room quantity used as quality | explicit hidden-room/loot-room negative rules |
| systemic discovery without real owner | future-owner conditional taxonomy + owner table |
| martial/cinema overwhelms setting | zero-valid, max-one-primary-seed, non-load-bearing rules |
| quest/dialogue/belief runtime authored here | outside-claim + rejection rules |
| access roles weakened for discovery | inherited-role invariant + access-laundering controls |
| seed chosen first then content invented | CITY-03 comparison contract + seed-gaming rejection |
| asset geometry silently expands interior scope | I0 asset-room negative control |

No remaining material in-claim blocker was found.

## 6. Concurrent-main audit

While CITY-06 was Draft, `main` advanced from baseline `632a63c089f844313e567046b3df541e435d75d4` to `3688b7b9a27355b0fda160c20e57a385e40c6814` through acceptance of `WP-PA-01`.

Exact baseline→main compare at pre-review:

- 5 commits ahead;
- changed files are only:
  - `Docs/evidence/WP-PA-01/HANDOFF.md`;
  - `Docs/evidence/WP-PA-01/WORKER_PLAN.md`;
  - `Docs/evidence/WP-PA-01/WORKER_PRE_REVIEW.md`;
  - `Docs/research/living-world/results/PA-01.md`.

No CITY-00/01/02/05 semantic owner, CITY-06 contract, ART input or candidate path changed. PR #90 remains mergeable.

The accepted PA-01 result is research/product input, not implemented runtime. CITY-06 deliberately uses PA identifiers only as future causal-owner categories, so PA-01 acceptance does not make `follow actor` or schedule-based discovery `AUTHORED_SPATIAL_NOW`.

No semantic rebase is required. Freeze remains tied to the exact branch SHA after final handoff rather than chasing unrelated parallel-track commits.

## 7. Scope / proof-budget check

CITY-06 is non-foundational. No foundational proof matrix or content-shape probe is required.

Evidence added is bounded to acceptance surfaces:

- predecessor/claim split;
- interior coverage;
- depth/shell compatibility;
- discovery causality;
- strict Worker pre-review;
- handoff.

The two findings fixed added direct acceptance evidence rather than generic hardening machinery.

`PROOF_BUDGET_VERDICT: NOT_APPLICABLE_NON_FOUNDATIONAL / BOUNDED_EVIDENCE`

## 8. Residuals intentionally not hardened here

Remain later-owned:

- exact retained-seed choice/boundary;
- exact room dimensions and metric shell fit;
- Unity geometry, collision/navmesh, lighting and performance;
- asset/prefab inventory and canonical native locators;
- implemented public discovery/catalogue capability;
- runtime schedules, access permissions, keys/inventory, beliefs/knowledge, dialogue, relationships, material simulation, governance, memory/consequence and save semantics;
- final named documents/lore/backstories/quests;
- empirical proof of systemic/hybrid routes;
- CITY-07 realization and CITY-08 authoring/reuse proof.

These are not converted into CITY-06 blockers absent concrete contradiction with the accepted planning claim.

## 9. Worker verdict

```text
WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 2
KNOWN_BLOCKING_DEFECTS: 0
PREDECESSOR_REOPEN_TRIGGERED: NO
CANDIDATE_SCOPE: WITHIN_WP
READY_TO_WRITE_FINAL_HANDOFF: YES
```

After the handoff file is committed, its containing exact 40-character SHA must be read from PR #90, recorded in PR metadata as `Candidate HEAD SHA` and `Frozen candidate SHA`, and the PR may then be marked Ready for a **fresh independent Reviewer**. No Worker branch writes are permitted after that freeze.