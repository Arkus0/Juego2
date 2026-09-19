# WP-HK-GATE — AI authoring readiness gate

Status: PLANNED  
Class: FOUNDATIONAL GATE  
Depends on: `WP-HK-10`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Prove that Arkus is genuinely ready to be the engine-neutral foundation for AI-driven game development before Unity/gameplay work is allowed to start.

## Gate scenario

From a clean checkout, launch the canonical headless host. A client starts only with the launch instruction and the public bootstrap entrypoint; it must not read C# implementation source to complete the scenario.

The client must:

1. discover the full relevant canonical capability/schema surface;
2. create a representative micro-world containing multiple resources, typed identities and cross-references entirely through harness commands;
3. inspect/query that world and prove it can reconstruct the authorable state needed for safe edits;
4. perform a multi-resource dry-run, inspect its change set, then atomically apply it;
5. submit an intentionally invalid change, receive structured diagnostics, repair the request and succeed;
6. demonstrate stale-revision/conflict rejection rather than silent overwrite;
7. execute at least one batched authoring flow and satisfy the accepted HK-08 interaction budget;
8. export a canonical snapshot and provenance/journal evidence;
9. restart from a clean process, replay/import through the accepted mechanism and obtain the identical canonical final state hash;
10. produce a semantic diff and provenance chain explaining the accepted changes;
11. exercise the same accepted semantic surface through both the deterministic reference transport and the MCP projection and prove contract/result equivalence for the gate flow;
12. run the full validation/test surface headlessly with no manual/editor intervention.

## Two independent clients

The gate requires both:

- a deterministic reference client/script in CI that exercises the exact scenario; and
- one fresh independent AI-agent trial whose transcript/evidence shows it used the public discovered contract rather than private implementation knowledge.

The AI trial must use an accepted standard/client-facing adapter path (normally MCP) for at least the representative authoring flow. The deterministic reference client remains the oracle for reproducible semantic comparison.

The AI trial is review evidence, not a replacement for deterministic CI.

## Product-boundary proof

Gate evidence must also demonstrate:

- canonical semantics do not depend on MCP, Unity, a model vendor or a hosted service;
- transport projections are mechanically checked against the canonical capability universe;
- material external dependencies satisfy the accepted dependency/IP record and are replaceable behind conformance boundaries;
- no completeness claim uses a self-shrinking inventory as its sole universe;
- the H0 architecture leaves a clean engine-bridge boundary for H1 without embedding engine object models into canonical contracts.

## Hard blockers

The gate FAILs if any of these occur:

- direct editing of canonical world persistence or implementation files is required to complete the scenario;
- a public capability is usable but absent/inaccurate in canonical discovery;
- request/success/error schema is missing for a used capability;
- reference and MCP projections disagree semantically on the accepted gate surface;
- invalid state can commit through a public mutation route;
- partial commit, silent stale overwrite, replay/hash divergence or unaudited mutation occurs;
- an error requires reading source code to understand the corrective action;
- hidden Unity/editor/network/model-vendor dependency is required for canonical H0 behaviour;
- the AI workflow degenerates into pathological one-field-per-request authoring contrary to the accepted interaction budget;
- proof infrastructure itself is missing or can omit required obligations from its own universe;
- an external component silently becomes the semantic source of truth for a canonical guarantee;
- `UNRESOLVED_PROOF_OBLIGATIONS != 0` or `KNOWN_UNDETECTED_DEFECT_CLASSES != 0`.

## Evidence

Gate evidence must include exact candidate SHA, clean-checkout commands, canonical discovered capability inventory, transport-conformance results, scenario transcripts, state hashes, semantic diffs, provenance/journal output, interaction metrics, negative-conformance test summary, dependency/IP inventory for material H0 dependencies, reference-client result and independent AI trial transcript.

## PASS consequence

Only after an independent Reviewer PASS on the exact gate SHA may the roadmap author and begin detailed Engine Bridge / Unity-first workpacks.

A PASS does **not** authorize gameplay implementation directly; Unity must first receive its own downstream parity/bridge gate.
