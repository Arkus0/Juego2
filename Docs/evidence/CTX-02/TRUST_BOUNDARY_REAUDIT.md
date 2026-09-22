# CTX-02 Claim / Trust-Boundary Re-audit

Status: **ACTIVE CIRCUIT-BREAKER EVIDENCE — BEFORE REPAIR**  
PR: `#113`  
Baseline: `f7b4f1e8247dfc927203dca6754b71aaa53938f3`  
Trigger candidate: `66ccbeccf699f98c591809c739a3118f1b376e9c`  
Trigger review: `#5274257119`  
`fail_cycle: 3`

This re-audit is intentionally persisted **before** the cycle-3 repair. It is the design authority for that repair. It does not freeze a candidate and it does not authorize merge/DocSync.

## 1. Circuit-breaker lineage

CTX-02 has three consecutive independent Reviewer FAILs in one general family: false-greens inside the compression/validation boundary itself.

1. review `#5274094804`, candidate `8db17036ad65ca03540f14e71711d806179c3e1c`: an accepted PA capsule could delete the whole structured disposition surface while PA-chain audit remained GREEN;
2. review `#5274163927`, candidate `504b4ff25f67720be9adb6b948dff859413419f3`: `reopen_conditions` / `escalate_if` could contain unusable elements and accepted identity did not necessarily require an explicit PASS verdict;
3. review `#5274257119`, candidate `66ccbeccf699f98c591809c739a3118f1b376e9c`: `exported_guarantees[].statement` or `exclusions_nonclaims[].statement` could be inverted/invented while IDs, source pointers and source fingerprints remained unchanged.

The first two blockers are real regressions that the current head already repairs. They remain part of the required regression suite; the cycle is not reset.

## 2. Exact claim boundary

`WP-CTX-02` does **not** claim to prove all predecessor semantics automatically. It claims a safe, compact navigation layer that:

- binds to an independently accepted predecessor identity;
- binds exact authoritative source bytes;
- preserves material inherited boundary information needed for initial downstream reconstruction;
- detects representative lossy/stale compression before downstream roles rely on it;
- preserves deterministic structured facts when an authoritative source exposes a machine-checkable structure (notably PA dispositions);
- forces authoritative reconstruction / human review when semantic equivalence cannot be established mechanically.

Therefore `VALID_NAVIGATION_ONLY` may mean only:

> mechanical identity/source/structure invariants pass **and** the repository's representative independent semantic controls pass.

It must never mean that arbitrary natural-language source semantics have been proved equivalent by the production checker.

## 3. Oracle classes

Every material field is assigned one of three oracle classes.

### A — production/source-derived invariant

The checker may prove this directly from repository structure or authoritative bytes without interpreting open-ended semantics. Failure is mechanical RED / `RECONSTRUCT_FROM_AUTHORITATIVE_SOURCES`.

### B — representative semantic control, test-only external oracle

Natural-language material cannot be proved generically by the production checker. CTX-02 therefore keeps a **small test-only oracle for the actual representative H1/CITY/PA capsules**. It is independent of the capsule/index and validates material content, not only IDs. Changing the oracle is review-visible test code, not a production semantic registry.

### C — human / escalation-only judgment

The content can be shape-checked mechanically, but truth/completeness for arbitrary future capsules remains human judgment. Any concrete contradiction, suspicious compression or material question escalates to authoritative sources and independent review.

## 4. Field-by-field trust audit

| Field / surface | Material downstream effect | Independent oracle against omission / reclassification / invention / self-confirmation | Class / required result |
|---|---|---|---|
| `schema`, `authority` | prevents protocol/authority drift | fixed protocol constants in checker | **A** |
| `capsule_id` + index entry | selects predecessor identity and capsule | index/capsule ID equality; duplicate rejection | **A** |
| `track` | activates track-specific rules | fixed enum plus PA/CITY track-specific validation | **A** |
| `content_mode` | selects boundary vs structured-disposition behavior | fixed enum; accepted PA-chain must be `structured_disposition`; CITY must be `boundary_summary` | **A** |
| `accepted_identity.reviewed_candidate_sha` | binds reviewed candidate | parsed from independent completion workpack; optional live accepted-state exact-SHA check | **A** |
| `accepted_identity.merge_sha` | binds accepted merge | parsed from independent completion workpack; optional live accepted-state exact-SHA check | **A** |
| `accepted_identity.review_id` | binds independent PASS review | completion parser must require explicit PASS + review id | **A** |
| `identity_source` | decides what is allowed to certify identity | must be external to capsule/index/CTX-02 evidence, live under `Docs/workpacks/**`, and its basename must match `<capsule_id>.md`; fingerprint recomputed | **A**; closes self-confirmation |
| `authoritative_sources[]` | defines bytes a downstream role may deepen into | source existence + recomputed Git blob SHA + duplicate rejection; source path may not point back into capsule/index/CTX-02 generated evidence | **A** for byte/source independence; source sufficiency remains **B/C** |
| `exported_guarantees[].id` | stable material boundary handle | structural uniqueness in production; representative expected IDs from external test oracle | **A+B** |
| `exported_guarantees[].statement` | directly reconstructs inherited guarantee | representative actual H1/CITY/PA `id -> statement` test oracle, independent from capsule/index; same-ID statement inversion/invention must RED | **B**; arbitrary future semantics remain **C** |
| `exported_guarantees[].source_pointer` | tells reviewer/worker where to deepen | representative oracle pins material source pointer together with statement where used; arbitrary pointer semantics remain reviewable | **B/C** |
| `exclusions_nonclaims[].id` | stable negative-boundary handle | structural uniqueness + representative expected IDs | **A+B** |
| `exclusions_nonclaims[].statement` | prevents overclaim / forbidden inheritance | symmetric representative actual H1/CITY/PA `id -> statement` oracle; same-ID inverted/invented non-claim must RED | **B**; arbitrary future semantics remain **C** |
| `exclusions_nonclaims[].source_pointer` | negative-boundary provenance | representative oracle where material | **B/C** |
| `reopen_conditions[]` | determines when accepted predecessor must be challenged | production requires non-empty strings; representative actual boundary oracle pins material conditions so deletion/substitution cannot silently pass CTX-02 controls | **A** shape + **B** representative semantics; future completeness is **C** |
| `escalate_if[]` | determines when capsule must stop being sufficient | same split as reopen conditions | **A+B+C** |
| `consumer_hints[]` | convenience/discoverability only | shape check; bootstrap/index wiring is independently tested; hints are not proof/ownership authority | **A** shape, otherwise **C/navigation only** |
| `mandatory_source_reads[]` | forces exact non-compressible source read | fingerprint + `noncompressible=true`; CITY boundary must contain the exact `Docs/production/CITY_PRODUCT_SEED.md`, not merely any valid read | **A** |
| `mandatory_source_reads[].reason` | explains why compression stops | non-empty shape; semantic adequacy remains human review | **A** shape + **C** |
| `disposition_source` | defines authoritative PA table used as oracle | accepted PA-chain discovery independently identifies canonical `Docs/research/living-world/results/PA-NN.md`; `disposition_source.path` must equal that discovered result and its blob must match | **A**; closes self-confirmation |
| `dispositions[]` | preserves exact PA state/disposition | exact source-table key/status equality; whole surface required for every discovered accepted PA result | **A** |
| `directional_semantics[].id/forward/reverse` | preserves asymmetric accepted meaning | production proves shape and forward != reverse only; representative PA oracle pins actual `id -> forward/reverse` values so invented-but-distinct directions RED | **A+B**; arbitrary semantic interpretation remains **C** |
| index PA coverage | determines whether accepted cumulative PA chain may claim COMPLETE | accepted universe discovered from canonical result glob + `Status: COMPLETE` workpacks, not from index | **A** |
| index representative H1/CITY entries | discoverability | index/capsule validation plus adoption-wiring controls; semantic sufficiency is challenged by representative oracle | **A+B** |

## 5. Current false-green class and causal closure

The cycle-3 blocker is not “one bad HK statement”. It is the general class:

> a semantic field can retain valid structure/ID/source bytes while its **material value** changes to an opposite or invented value, and the current independent control does not notice because it checks only presence/shape.

Inside CTX-02's claim this class applies to:

- representative `exported_guarantees`;
- representative `exclusions_nonclaims`;
- representative `reopen_conditions`;
- representative `escalate_if`;
- representative `directional_semantics` values.

It does **not** justify a generic production natural-language equivalence engine. The repair must instead extend the existing test-only representative oracle to the actual indexed H1/CITY/PA boundaries and validate material content.

Required defect controls:

1. keep guarantee ID + source pointer + authoritative fingerprints unchanged, invert/invent the guarantee `statement`, and require representative control RED;
2. symmetric exclusion/non-claim mutation with ID + source pointer + fingerprints preserved, require RED;
3. mutate one representative reopen condition to a materially opposite/invented non-empty string, require RED;
4. mutate one representative escalation trigger similarly, require RED;
5. mutate one PA directional value to a different but still distinct expression, require RED.

The production `--audit-index` path is not expected to semantically reject items 1–5 by itself. CTX-02 validation is the **conjunction** of production audit plus independent controls. This separation is deliberate and documented.

## 6. Self-confirmation audit

### Accepted identity

A fingerprint alone is insufficient if the capsule is allowed to choose a generated/self-authored identity source. The checker must require an external canonical workpack identity source (`Docs/workpacks/**/<capsule_id>.md`) and parse explicit COMPLETE + reviewed candidate + merge + PASS review from that source.

### Authoritative sources

A bound source must not point back into `Docs/engineering/context-capsules/**`, the capsule index, or CTX-02's own generated evidence and then claim that as predecessor authority. Byte-integrity checks remain useful only after this source-independence boundary holds.

### PA dispositions

The table comparison is a strong production oracle only if the table comes from the **canonical accepted PA result discovered independently**. The accepted PA-chain rule must therefore require `disposition_source.path == discovered result path`, not merely require some structured table plus a separate authoritative source list entry.

### Mandatory CITY read

`mandatory_source_reads` must not prove itself by containing an arbitrary valid file. For the representative CITY boundary the protocol explicitly names `Docs/production/CITY_PRODUCT_SEED.md`; production validation must enforce that exact path.

## 7. What remains necessarily human

CTX-02 cannot mechanically prove that:

- every future natural-language guarantee is a complete and correct paraphrase of all relevant source nuance;
- every future exclusion captures every possible overclaim;
- every reopen/escalation list is semantically exhaustive;
- a source pointer is the best interpretation of a contested source passage;
- an apparently contradictory later fact actually reopens a predecessor rather than being owned downstream.

Those cases must remain fail-safe by process: if material, contradictory, ambiguous or suspicious, open authoritative sources and require independent Reviewer judgment. The capsule never limits review.

## 8. Repair scope authorized by this re-audit

The next repair may change only what follows from the boundary above:

- production checker: source-independence constraints, canonical PA disposition-source binding, exact CITY mandatory-read binding, and any minimal shape enforcement needed by those invariants;
- independent controls: actual indexed representative semantic oracle for H1/CITY/PA plus causal mutation controls for guarantee, exclusion, reopen, escalation and directional semantic value;
- protocol/evidence text: make the A/B/C split explicit so `VALID_NAVIGATION_ONLY` is not misread as generic semantic proof;
- existing regression controls from fail cycles 1 and 2 remain mandatory.

Forbidden repair expansion:

- no production registry mapping all semantic IDs to approved prose;
- no NLP/fuzzy semantic-equivalence checker;
- no attempt to parse and prove arbitrary narrative meaning;
- no bulk migration of historical capsules;
- no product/runtime semantic change.

## 9. Pre-repair proof-budget decision

`PROOF_BUDGET_PRE_REPAIR: WITHIN_BUDGET_IF_BOUNDED_AS_ABOVE`

Rationale:

- three production additions are small source-identity invariants with deterministic authoritative oracles;
- the semantic repair extends one existing **test-only** representative control rather than creating a production semantic authority;
- representative coverage remains H1 + CITY + one complex PA boundary, while PA dispositions continue to use their existing source-derived table oracle for all accepted PA results;
- unresolved arbitrary natural-language equivalence stays with escalation + independent Reviewer judgment.

If implementation requires a general semantic registry or source-language theorem machinery, this proof budget is exceeded and CTX-02 must stop/re-scope instead of adding that machinery.

## 10. Gate before any new freeze

No new freeze is permitted until all are true:

- this re-audit remains durable in history;
- repair follows sections 5–8 rather than a single Reviewer example;
- required causal controls RED on injected defects and GREEN on baseline;
- fail-cycle 1 and 2 regressions remain GREEN;
- complete baseline→candidate diff is reviewed;
- complete Worker pre-review is rerun and persisted CLEAN;
- final `PROOF_BUDGET` explicitly confirms the implemented solution stayed within the bounded design;
- only then may exact-SHA freeze + fresh independent Reviewer begin.
