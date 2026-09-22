# DW Design World Architecture — second consumer before H2

Version: 0.4 — accepted plan
Status: ACCEPTED after PR `#117` independent PASS `#5274794434`, merge `b831050e9df8b61b76744e0c5f544bd7ec2d79b5` and post-PASS DocSync

## 1. Definition

A **Design World** is a typed, machine-readable projection of accepted design/research facts that is consumed through Arkus public contracts for deterministic query, validation, diff and provenance-aware retrieval.

It is a second Arkus consumer, not a second authority. In DW v1:

1. accepted CITY/PA source documents remain semantic authority for the facts they own;
2. a projection encodes only explicitly selected, mechanically useful facts and relations;
3. every projected fact used for validation or retrieval retains stable provenance to its accepted source;
4. Arkus may query and validate the projection without learning CITY- or PA-specific semantics in the H0 kernel;
5. projection state is rebuildable from its declared authority inputs and versioned projection rules; and
6. quality/correctness is measured before token or context savings.

DW is not a Markdown replacement, an LLM-generated new source of truth, a workflow/workpack database, a hidden H0 rewrite, or permission to automate creative judgement.

## 2. Motivation and claimed value

The project already contains important design guarantees that are mechanically testable but currently expressed only in prose or JSON explanatory text. A representative example is the CITY binding rule that programme-required access roles may be promoted but never removed by later binding. Today a Worker/Reviewer must reconstruct such relations across documents.

DW tests whether accepted H0 machinery can turn selected design facts into a queryable/failable second consumer so that:

- objective design drift can fail before semantic review;
- Worker/Reviewer context can be retrieved selectively with source provenance rather than by repeatedly loading whole corpora;
- content-shape queries can support art/content planning;
- H0/H1 public boundaries receive pressure from a domain distinct from runtime/Unity projection before H2 freezes a product boundary; and
- later design↔Unity, QA and narrative tooling can build on proven state instead of one-off scripts.

Token reduction is an expected consequence, not the primary acceptance oracle.

## 3. Authority model

| Data / decision | Authority in DW v1 | Consequence |
|---|---|---|
| accepted CITY programme/design facts | accepted CITY source documents/contracts | projection may expose them but cannot silently redefine them |
| accepted PA findings/evidence/dispositions | accepted PA source documents/contracts | projection must preserve finding identity, disposition and source lineage |
| projection schema/rules | reviewed DW provider contract | versioned and inspectable; changing it is not equivalent to changing source truth |
| projection instances/index | derived Design World state | disposable/rebuildable from authority inputs |
| generic identity/mutation/validation/journal/snapshot/query surfaces | accepted Arkus/H0 public contracts | consumed, not reinterpreted |
| Unity effective state | H1 bridge observation once available | not DW authority; later comparable evidence only |
| Worker/Reviewer process state | repository process/protocol | explicitly excluded from DW v1 |
| creative quality/taste/design judgement | human/product owners + semantic review | never reduced to a mechanical invariant merely for automation |

A projection disagreement with its accepted source is a projection defect. It does not transfer authority to the projection.

## 4. Projection model

Every DW projected record that may affect a verdict or retrieval result carries at minimum:

- stable projected identity;
- domain/schema version;
- typed payload/relations;
- source authority identifier/path;
- source anchor sufficient to detect staleness or ambiguity under the accepted source format;
- projection rule/version; and
- deterministic normalized representation used for equality/digest where applicable.

The exact encoding is owned by `WP-DW-00`; this architecture does not prescribe a new H0 canonical type if accepted extensibility already suffices.

Because `WP-DW-00` defines the generic projection/public surface and is foundational, its neutral reference fixture is not sufficient by itself. Before freeze it must also pass one bounded slice of the currently approved Juego2 target through that same surface as the representative content-shape probe required by `FOUNDATIONAL_PROOF_STANDARD.md`. That probe checks only representability, identity/granularity, provenance and DW-00-owned public boundaries; it does not transfer CITY semantic/invariant ownership out of `WP-DW-01`.

### 4.1 Fail-closed provenance

A fact cannot be used as proof merely because it exists in a DW index. Missing, ambiguous, stale or contradictory provenance must be visible and must not silently produce a valid result.

### 4.2 No destructive summarization

DW may provide compact retrieval records, but accepted source facts that affect meaning remain recoverable by provenance. PA dispositions, negative findings, exceptions and counterfactual fixture identities cannot be omitted merely to make a smaller context pack.

## 5. Domain isolation and H0 reopen rule

DW deliberately pressure-tests accepted H0 without granting itself permission to patch H0 opportunistically.

A discovered difficulty is classified as one of:

1. **domain need** — express it in the CITY/PA DW provider/schema; H0 stays unchanged;
2. **projection/tooling need** — implement it in DW-owned adapter/index/query tooling if it uses accepted public contracts without changing their meaning;
3. **generic Arkus contradiction/deficiency** — stop the owning DW claim and produce causal evidence for an explicit reviewed kernel delta/reopen decision; or
4. **unsupported ambition** — record as residual/downstream candidate rather than widening the active WP.

A CITY/PA concept, name or rule appearing in kernel semantics is prima facie leakage and fails the owning WP unless an independently reviewed generic abstraction justifies it.

No DW WP may weaken an accepted H0 oracle, alter historical evidence, or call a private implementation route in order to keep the trial green.

## 6. Invariant boundary

DW automates only claims with an independent, deterministic oracle over structured facts.

Suitable examples include:

- required-role subset preservation;
- required anchor/cardinality relations;
- completeness against an independently enumerated declared programme universe;
- typed relation compatibility;
- preservation of accepted PA finding/disposition/fixture links;
- deterministic content-shape counts/buckets whose inputs are explicit.

Not suitable as DW mechanical acceptance without a separately reviewed oracle:

- whether a district is evocative;
- whether an interior is fun;
- whether a narrative beat is emotionally effective;
- whether an art direction is beautiful;
- whether a research conclusion is substantively correct merely because it was encoded.

The invariant executor proves conformance to an accepted rule, not the wisdom of the rule.

## 7. CITY consumer boundary

The first **semantic** consumer is a bounded accepted CITY slice because it contains typed relations and measurable programme constraints while remaining independent of Unity execution. The required DW-00 Juego2 content-shape probe may carry an accepted CITY-shaped record through the generic surface, but it is representability evidence only and cannot claim CITY rule truth.

`WP-DW-01` proves a minimal vertical slice, but every invariant claimed complete uses the **complete accepted source universe relevant to that invariant** — for the initial access-role proof, the full accepted CITY-02 A/B functional-POI universe — plus causal negative controls. `WP-DW-02` then expands only the useful production-facing subset required for real queries/content-shape measurements.

Expected initial families include POI, district, access role, interior/spatial-depth profile, spatial-demand class and required anchor/relationship facts. Exact families remain owned by the workpacks; the architecture does not pre-authorize full CITY conversion.

## 8. PA consumer boundary

PA follows successful CITY proof because typed research composition is semantically denser and more vulnerable to lossy compression.

The PA projection may represent selected findings, failure modes, invariants, evidence references, counterfactual fixtures and dispositions. It must preserve identity and lineage so that a compact query result can route a Worker/Reviewer back to the exact accepted authority rather than replacing it.

PA projection is not permission to infer new research truth from the graph. New research remains PA-owned and reviewed through the existing PA process.

## 9. Structured-context trial

`WP-DW-04` compares two routes over representative already-resolved tasks:

- baseline repository/document context under **accepted CTX-03 rules**; and
- DW structured retrieval plus source-open-on-demand under the same task/oracle.

CTX therefore remains valid and independently useful. DW does not replace or invalidate CTX-01..03; it tests an additional structured retrieval layer against the best accepted CTX baseline rather than against an obsolete, deliberately bloated workflow.

The comparison universe cannot be selected after DW has been tuned. Before route-specific tuning or result observation, DW-04 must durably freeze either the complete eligible historical-task universe plus a deterministic selection/stratification rule, or the exact independently reviewed task manifest. The same pre-tuning freeze also fixes task prompts, accepted authority anchors, expected facts/blockers/verdicts, deterministic scoring rules and matched-run policy. Any later change invalidates prior tuning/results and restarts the trial from a new pre-tuning freeze.

The trial has two jointly necessary proof layers:

1. **structural context proof** — deterministic route assembly/fallback must recover the complete pre-frozen expected material and provenance path; and
2. **actual agent-quality proof** — every selected task must receive paired CTX-vs-DW agent executions under the same declared model/version, system/task prompt, decoding settings, tool permissions, execution budget and run-count policy, with outputs scored deterministically against the pre-frozen expected facts/blockers/verdicts.

Model prose is therefore not the oracle, but model execution is not optional evidence either. A context pack that contains the right fact while the paired DW agent misses the required blocker or returns the wrong required verdict fails the claimed quality-preservation result.

The trial records at minimum:

- pre-tuning task-universe/selection-freeze anchor and chronology;
- exact model/configuration and matched-run policy used for each pair;
- input/context volume using a reproducible byte/token-count method defined before measurement;
- sources opened;
- deterministic context-package expected facts/blockers recovered;
- scored agent-recovered required facts/blockers/verdicts;
- omissions and false findings;
- provenance/source fallback behavior; and
- whether structured retrieval changed the final scored verdict.

A smaller prompt with a missed blocker is a FAIL. A quality regression cannot be traded for token savings. Because model runs are stochastic, the quality claim is bounded to the exact frozen task universe, model/configuration and run policy actually executed; deterministic expected/scoring manifests remain the oracle, while paired model runs provide the mandatory execution evidence for the agent-quality claim.

DW does not claim a universal percentage reduction from one trial. `WP-DW-GATE` may report measured results only for the declared fixture universe/configuration.

## 10. H1/H2 interlock

DW does not alter the accepted H1 DAG. H1 proceeds independently toward its existing Unity bridge gate.

DW-GATE is planning evidence for H2, not a replacement for H1-GATE. H2 planning should consume both if available:

- H1-GATE answers whether Arkus can drive the accepted Unity projection boundary;
- DW-GATE answers whether Arkus survives a materially different non-runtime consumer and whether structured design retrieval is useful/correct.

If H2 planning begins before DW-GATE, it must not freeze an external/public boundary whose design would make the still-authorized DW second-consumer proof impossible to incorporate without explicit review. The preferred sequence is to complete DW-GATE before final H2 boundary acceptance. The repository top-level milestone/gate authority, `Docs/ROADMAP.md`, records this interlock: DW does not block H1 or by itself authorize H2 implementation, but final H2 public/external-boundary acceptance must consume accepted DW-GATE evidence or explicitly review and disposition the interlock.

DW itself does **not** prove package/version distribution to a fresh external repository; that remains the existing H2 planning question.

## 11. Downstream adoption candidates — explicitly not DW v1 claims

If DW-GATE passes, later owners may evaluate:

- accepted-design ↔ normalized Unity observation drift;
- generated art/content briefs from accepted design queries;
- source-catalogue coverage and content-production planning;
- replay-backed deterministic QA fixtures;
- narrative knowledge-boundary checks where the knowledge model has an independently reviewed oracle; and
- other non-game typed-state consumers.

These are preserved opportunities, not pre-authorized DW implementation. Their causal owner depends on the H1/H2 state that exists when they are planned.

## 12. Process-state exclusion

DW v1 must not model or validate the repository's own Worker/Reviewer/freeze/DocSync/residual-ledger state as a Design World consumer. Doing so before the second-consumer boundary is independently proven would make Arkus load-bearing for the process that validates Arkus and create avoidable circularity.

CTX may consume measured DW results later, but CTX remains the owner of process-context policy.

## 13. Determinism and completeness

For the same accepted authority anchors, DW schema/projection version and declared consumer configuration, a rebuild must yield equal normalized projected facts/relations and deterministic validation/query results.

Completeness-sensitive claims require an independently/effectively justified universe. A registry cannot prove its own completeness. Removing a material source record, required relation, projection rule execution or expected fixture must turn the relevant proof RED rather than merely proving that a row was deleted. For DW-04, the task-selection universe/rule and correctness scorer are likewise external to the result being judged and must be frozen before treatment-specific tuning.

## 14. Security / privacy / external-domain non-claims

DW v1 operates only on repository-owned project material already authorized for this project. It makes no claim for clinical, legal, regulated, enterprise or third-party confidential workloads. Architectural resemblance to those domains is not product readiness.

## 15. Exit boundary

DW ends only when its gate can answer, with exact evidence:

1. did accepted Arkus serve CITY and PA through public generic surfaces without domain leakage into H0;
2. are projected facts provenance-preserving, rebuildable and fail-closed;
3. did at least one useful CITY mechanical invariant catch a causal omission/change;
4. did the PA projection preserve the accepted finding/disposition/fixture universe under causal omission controls;
5. did structured retrieval preserve actual paired-agent review/task correctness under the pre-tuning frozen suite/model/configuration while measuring context cost honestly against accepted CTX;
6. what generic Arkus limitations were discovered and how were they routed; and
7. which consequences, if any, should become binding H2 planning input.

Passing DW proves a bounded second-consumer architecture and measured project utility. It does not prove arbitrary-domain universality.
