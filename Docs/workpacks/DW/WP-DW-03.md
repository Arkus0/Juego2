# WP-DW-03 — PA typed corpus and lossless provenance projection

Status: PLANNED / NOT_STARTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-DW-02` PASS + merge + DocSync; `WP-PA-05` PASS + merge + DocSync
Blocks: `WP-DW-04`

## Objective and central claim

Prove that the accepted local PA-01..05 corpus can be projected as typed findings/evidence/dispositions/fixtures/relations and queried compactly without losing material accepted meaning or replacing the source research as authority.

## WHY_THIS_BOUNDARY

CITY mainly stresses programme entities, typed relations and counts. PA stresses accumulated findings, negative knowledge, dispositions, evidence and counterfactual links where lossy compression can create a false green. A separate WP lets Reviewer reject PA losslessness without reopening a correct CITY consumer.

## Inherited guarantees

Consumes DW-00 generic projection/provenance/rebuild semantics, the domain-isolation evidence from DW-01/02 and accepted PA-01..05 local source truth. Existing PA ownership/review semantics remain unchanged.

## New guarantees owned

- versioned PA projection vocabulary for selected finding, failure-mode, invariant, evidence, fixture and disposition records;
- stable identity and provenance for every projected PA record/relation;
- explicit preservation of accepted dispositions, including negative/rejected/later/baseline meanings present in the source corpus;
- lossless source-to-projection completeness for the declared PA-01..05 universe;
- deterministic typed queries by domain/failure family/disposition/fixture/evidence relation;
- source-open-on-demand pointers from compact query results;
- a forward-extension rule for newly accepted PA work that does not pre-accept future research.

## Explicitly not re-proved

Substantive correctness of accepted PA research, future PA-06..14 findings, CTX context policy, model reasoning quality, CITY semantics or H0 correctness beyond the consumed seam.

## Allowed scope

PA projection/schema/provider, source manifests, deterministic query/validation tooling, focused losslessness tests and evidence.

## Forbidden scope

New PA research conclusions, destructive summaries as authority, auto-adopting future PA output, changing accepted dispositions, modeling Worker/Reviewer process state, H0 domain-specific changes or claiming token savings before DW-04 measures them.

## Architecture / authority boundary

Accepted PA documents remain authority. Projection records are indexes/views with provenance. A compact query result may select relevant records, but omission from a query/index cannot rewrite or erase an accepted finding/disposition/fixture. Completeness is checked against an independent source-owned manifest/universe.

## Acceptance criteria

- a reviewed source manifest enumerates the complete declared PA-01..05 projection universe independently of projected rows;
- every projected finding preserves stable identity, source provenance and every material relation selected by the schema;
- dispositions are represented explicitly and cannot disappear merely because the entire structured disposition surface is omitted;
- counterfactual fixture/evidence relations remain linked and queryable where accepted source truth defines them;
- queries over disposition/domain/failure family/fixture/evidence return exact expected sets against a source-side oracle;
- clean rebuild from the same accepted PA anchors yields equal normalized projection/query output;
- omission of one material finding, one disposition surface or one required fixture/evidence relation makes the semantic completeness oracle RED;
- compact results provide enough source identity to reopen exact accepted evidence rather than relying on paraphrase;
- PA-specific types/rules remain outside H0 kernel/public generic semantics.

## Deterministic proof / evidence

Build the projection from the accepted PA-01..05 source manifest, execute a frozen query suite and compare exact IDs/relations/dispositions with an independently derived expected set. Rebuild from scratch and compare. Synthetic omission controls mutate the effective projection path and must fail the semantic oracle/pipeline.

## Causal negative-conformance classes

- one accepted finding omitted while the source manifest still contains it;
- the entire structured disposition surface for one source/workpack is removed;
- one ADOPT/LATER/REJECT/baseline disposition is changed or lost;
- a required counterfactual fixture/evidence edge is removed while record identity remains;
- a compact query drops a negative/rejected record because it is considered less relevant;
- provenance is stale/ambiguous while the text/ID happens to look plausible;
- the forward-extension path accepts an unreviewed future PA record as if already authoritative.

## Content-shape probe

Required. PA-01..05 are the initial complete local accepted corpus for the declared schema, not a cherry-picked positive subset. The schema may deliberately exclude source material that has no claimed structured meaning, but every exclusion class must be explicit and independently reviewable.

## Dependency / IP implications

Repository-owned research material only. No external corpus is adopted by this WP.

## Residual risks

A faithful PA-01..05 projection does not prove future PA research can always use the same schema unchanged, nor that an LLM using compact retrieval will maintain quality. DW-04 owns the latter; future schema pressure is routed by the forward-extension rule.

## Exact predecessor reopen condition

Reopen DW-00 only if PA demonstrates a generic provenance/rebuild limitation; reopen the PA DW provider/schema for PA-specific representational defects. Accepted PA source research reopens only under PA's own evidence rules, not because projection is inconvenient.

## PASS consequence / next dependency

PASS permits `WP-DW-04` once CTX-03 is accepted. Newly accepted PA work may be projected through the reviewed extension rule, but remains non-authoritative until its ordinary PA PASS + merge + DocSync exists.
