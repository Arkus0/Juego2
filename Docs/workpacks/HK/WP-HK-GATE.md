# WP-HK-GATE — AI authoring readiness gate

Status: PLANNED  
Class: FOUNDATIONAL GATE  
Depends on: `WP-HK-10`  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`

## Objective

Prove that the harness is genuinely ready to be the foundation for AI-driven game development before Unity/gameplay work is allowed to start.

## Gate scenario

From a clean checkout, launch the canonical headless host. A client starts only with the launch instruction and the public protocol bootstrap entrypoint; it must not read C# implementation source to complete the scenario.

The client must:

1. discover the full relevant capability/schema surface;
2. create a representative micro-world containing multiple resources, typed identities and cross-references entirely through harness commands;
3. inspect/query that world and prove it can reconstruct the authorable state needed for safe edits;
4. perform a multi-resource dry-run, inspect its change set, then atomically apply it;
5. submit an intentionally invalid change, receive structured diagnostics, repair the request and succeed;
6. demonstrate stale-revision/conflict rejection rather than silent overwrite;
7. execute at least one batched authoring flow and satisfy the accepted HK-08 interaction budget;
8. export a canonical snapshot and provenance/journal evidence;
9. restart from a clean process, replay/import through the accepted mechanism and obtain the identical canonical final state hash;
10. produce a semantic diff and provenance chain explaining the accepted changes;
11. run the full validation/test surface headlessly with no manual/editor intervention.

## Two independent clients

The gate requires both:

- a deterministic reference client/script in CI that exercises the exact scenario; and
- one fresh independent AI-agent trial whose transcript/evidence shows it used the public discovered contract rather than private implementation knowledge.

The AI trial is review evidence, not a replacement for deterministic CI.

## Hard blockers

The gate FAILs if any of these occur:

- direct editing of canonical world persistence or implementation files is required to complete the scenario;
- a public capability is usable but absent/inaccurate in discovery;
- request/success/error schema is missing for a used command;
- invalid state can commit through a public mutation route;
- partial commit, silent stale overwrite, replay/hash divergence or unaudited mutation occurs;
- an error requires reading source code to understand the corrective action;
- hidden Unity/editor/network dependency is required;
- the AI workflow degenerates into pathological one-field-per-request authoring contrary to the accepted interaction budget;
- proof infrastructure itself is missing or bypassable;
- `UNRESOLVED_PROOF_OBLIGATIONS != 0` or `KNOWN_UNDETECTED_DEFECT_CLASSES != 0`.

## Evidence

Gate evidence must include exact candidate SHA, clean-checkout commands, discovered capability inventory, scenario transcript, state hashes, semantic diffs, provenance/journal output, interaction metrics, adversarial test summary, reference-client result and independent AI trial transcript.

## PASS consequence

Only after an independent Reviewer PASS on the exact gate SHA may the roadmap author and begin detailed Unity Bridge workpacks.

A PASS does **not** authorize gameplay implementation directly; Unity must first receive its own downstream parity/bridge gate.
