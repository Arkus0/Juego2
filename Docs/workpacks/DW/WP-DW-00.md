# WP-DW-00 — Authority-preserving Design World projection contract

Status: PLANNED / NOT_STARTED
Class: FOUNDATIONAL
EXECUTION_REQUIREMENT: REMOTE_OK
Depends on: `WP-HK-GATE` PASS + accepted DW planning PR merge/DocSync
Blocks: `WP-DW-01`

## Objective and central claim

Implement the smallest generic Design World provider/projection boundary needed to represent selected accepted design facts as typed, provenance-bearing, rebuildable Arkus-consumable state without transferring semantic authority away from the accepted source documents or changing H0 meaning.

## WHY_THIS_BOUNDARY

A CITY implementation without an explicit authority/provenance contract could accidentally make a generated index the new truth or bake CITY vocabulary into the kernel. A contract with no executable reference would be speculative. This WP therefore owns the generic projection envelope plus a tiny domain-neutral reference fixture; the first real CITY semantics remain in DW-01.

## Inherited guarantees

Consumes accepted H0 identity/reference/validation/mutation/journal/snapshot/query/transport boundaries and the DW planning architecture. H0 remains canonical for its own authored state and public semantics.

## New guarantees owned

- versioned generic projected-fact/relationship envelope sufficient for DW consumers;
- stable provenance from every proof-relevant projected fact to an accepted source authority anchor;
- explicit projection/schema version and deterministic normalized representation;
- rebuild equality from the same authority inputs and projection version;
- fail-closed stale/missing/ambiguous provenance handling;
- a reference source adapter/fixture demonstrating the contract without CITY/PA vocabulary;
- derived DW state is disposable and cannot become source authority merely because it is queryable.

## Explicitly not re-proved

Full H0 correctness, transport parity, H1 projection semantics, CITY design correctness, PA research correctness, CTX policy or Unity behavior.

## Allowed scope

Generic DW provider/schema/adapter code, domain-neutral reference fixtures, focused tests, architecture reconciliation and evidence. Existing public Arkus contracts may be consumed as-is.

## Forbidden scope

CITY/PA-specific semantics in kernel/public generic contracts, Unity integration, workpack/reviewer process modeling, LLM extraction as authority, H0 oracle weakening, gameplay, art tooling or external product packaging.

## Architecture / authority boundary

Accepted source documents remain authority. DW state is a derived projection whose records retain source lineage. Rebuilding the projection cannot mutate authority inputs. If accepted public Arkus contracts cannot express the required generic envelope without semantic change, this WP stops and produces causal reopen evidence rather than patching H0 privately.

## Acceptance criteria

- projected records carry stable identity, schema/projection version and source provenance sufficient to detect stale/missing/ambiguous authority anchors;
- identical authority inputs + projection version rebuild to equal normalized projected facts/relations and equal relevant digests/results;
- changing a material authority fact or projection rule changes the relevant normalized projection/result;
- deleting derived DW state and rebuilding does not require hidden generated truth;
- authority source bytes/hash are unchanged by projection/query/validation operations;
- no CITY/PA/process vocabulary or type enters H0 kernel dependencies/public semantics;
- reference fixture is usable through the same public surface later consumers will use;
- provenance failure cannot be reported as a valid fact merely because an index row remains.

## Deterministic proof / evidence

A tiny neutral fixture defines an independently enumerated source universe with typed objects, relations and one mechanical invariant. The proof builds the projection, queries/validates it, rebuilds from scratch and compares normalized output. Authority source hash is captured before/after.

## Causal negative-conformance classes

- one material source fact is omitted while the independent source universe remains complete: projection completeness/validation must RED;
- a projected record retains an old source anchor after the source changes: stale provenance must RED;
- a projected fact points ambiguously to two authority locations: it cannot validate as trustworthy;
- projection rule/version changes while cached derived output is reused: staleness/rebuild check must RED;
- derived/index state is deleted and a hidden dependency prevents equal rebuild;
- a domain-specific type/reference is introduced into the generic H0 dependency graph.

## Content-shape probe

Required but domain-neutral. Use only a minimal typed graph sufficient to exercise identity, relation, provenance, rebuild and one invariant. CITY is deliberately excluded until DW-01.

## Dependency / IP implications

No external dependency is expected. Any new runtime/library dependency requires explicit adoption rationale and replacement boundary.

## Residual risks

This contract does not prove real CITY completeness, PA losslessness, context savings, Unity drift or external repository consumption.

## Exact predecessor reopen condition

Reopen H0 only if executable public-surface evidence demonstrates that a generic typed/provenance/rebuild requirement essential to this consumer cannot be expressed without violating an accepted H0 guarantee. Convenience, naming preference or a CITY-specific need is insufficient.

## PASS consequence / next dependency

PASS permits `WP-DW-01`. It does not authorize full CITY/PA conversion, any H0 change, or downstream H2 semantics.
