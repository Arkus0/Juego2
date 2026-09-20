# WP-HK-07B representative content-shape probe

## Purpose

HK07B does not redefine authorable-state semantics, but it adds a second public transport projection. The bounded content probe therefore asks one product-facing question: can the same currently approved Potes/Liébana-style authored slice traverse MCP and the accepted JSONL transport without either adapter inventing or losing semantic meaning?

## Bounded slice

`Hk07BMcpConformanceTests.McpAndReferenceTransportRemainEquivalentAcrossAcceptedH0Flows` uses the existing small authored shape:

- `place.plaza` as a place;
- `building.market` contained by that place;
- `npc.ana` contained by the place and referencing the market with `works.at`;
- one opaque `future.social` extension attached to the NPC and depending on the market.

This is intentionally the same kind of bounded Potes/Liébana content used to validate HK07A. HK07B changes only how an external client reaches the accepted semantics.

## Probe path

Two fresh external process clients — JSONL and MCP — independently:

1. discover/use the accepted host;
2. read initial world summary and validation;
3. export the base snapshot;
4. apply the same authored operations with transport-local anchors;
5. inspect the NPC;
6. read provenance journal;
7. export and diff the changed snapshot;
8. bootstrap fresh target processes by snapshot import;
9. replay each transport's journal;
10. compare final snapshots and require no semantic diff.

At every compared step, transport-specific request IDs/framing are removed and the neutral outcome semantics must match.

## Findings/classification

- **Representability:** GREEN. Place containment, object relation and opaque extension traverse MCP without new schemas or MCP-specific canonical fields.
- **Identity/granularity:** GREEN. Canonical IDs and state hashes remain transport-independent; MCP's encoded tool name is framing only.
- **Mutation/provenance/portability:** GREEN. Apply produces persisted canonical state and one journal entry; fresh-process replay reconstructs the same authored result and final diff is empty.
- **Validation/error boundary:** GREEN. Validation remains valid; an invalid canonical object request remains a canonical failure through both transports.
- **Predecessor reopen condition:** none observed. The accepted neutral contract represented every needed request/outcome without amendment.
- **Named future decisions:** transforms, schedules, simulation/live state, compact interaction formats and recovery ergonomics remain outside this probe and belong to later WPs.

## Result

The bounded intended content shape does not expose transport coupling. MCP works as a projection of the same accepted authored semantics rather than requiring a parallel game model.
