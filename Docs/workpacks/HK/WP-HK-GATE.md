# WP-HK-GATE — AI authoring readiness gate

Status: COMPLETE  
Class: FOUNDATIONAL GATE  
Depends on: `WP-HK-10` ✅ COMPLETE  
Binding proof standard: `Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md`
Baseline SHA: `290f92e9c21f1e454e0d6924b29f4d778b9875f8`
Implementation PR: `#64`

Completion:
- Reviewed candidate SHA: `0fa3d4fb039a3d0049cea0f3eed1c83128ec7dcd`
- Independent Reviewer verdict: `PASS`
- Reviewer evidence: PR review `#5261636151`
- Deterministic exact-SHA observation: GREEN (`Arkus Candidate Validation` run `35533486939`)
- Exact-SHA freeze validation: GREEN (`Arkus Candidate Validation` run `35534660950`)
- Independent AI-agent MCP trial: PASS (PR comment `#5752332211`)
- Merge SHA: `048d2e449d5ff81e8bcc35bc15664ea4ceec18ca`
- Completed: `2026-09-20`
- Repair cycles: `1`; prior frozen SHA `2c0df70c1ec8245999e4d144e710816d2f7eb335` failed review `#5261512538` because residual reconciliation did not consume the full accepted HK10 universe and the GATE-owned omission control protected only a declarative label rather than the real stage-14 execution. The accepted repair closes both proof-boundary defects without adding product semantics.

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
6. demonstrate same-lineage stale-revision/conflict rejection rather than silent overwrite, then use the accepted HK08B machine-readable recovery context to re-plan against the new base and successfully retry without full-world reconstruction in the ordinary representative case;
7. execute at least one HK08A batched authoring flow representing one coherent multi-resource design intent, keep that intent within one accepted atomic transaction inside the HK09B resource envelope, and satisfy the accepted HK08B interaction budget;
8. export a canonical snapshot and provenance/journal evidence;
9. restart from a clean process, replay/import through the accepted mechanism and obtain the identical canonical final state hash;
10. produce a semantic diff and provenance chain explaining the accepted changes;
11. exercise the same accepted semantic surface, including HK08A batch/compact/pagination behaviour and HK08B conflict-recovery/repair behaviour, through both the deterministic reference transport and the MCP projection and prove contract/result equivalence for the gate flow;
12. demonstrate the accepted HK09A capability boundary from the public host path and the accepted HK09B resource/persistence limits without relying on transport-specific exceptions;
13. run the bounded long-session/endurance case accepted by HK10 and remain inside the declared HK09B H0 resource envelope;
14. run the full validation/test surface headlessly with no manual/editor intervention.

## Two independent clients

The gate requires both:

- a deterministic reference client/script in CI that exercises the exact scenario; and
- one fresh independent AI-agent trial whose transcript/evidence shows it used the public discovered contract rather than private implementation knowledge.

The AI trial must use an accepted standard/client-facing adapter path (normally MCP) for at least the representative authoring flow. The deterministic reference client remains the oracle for reproducible semantic comparison.

The AI trial is review evidence, not a replacement for deterministic CI.

## Product-boundary proof

Gate evidence must also demonstrate:

- canonical semantics do not depend on MCP, Unity, a model vendor or a hosted service;
- transport projections are mechanically checked against the canonical capability universe, including interaction primitives added by HK08A/HK08B;
- material external dependencies satisfy the accepted dependency/IP record and are replaceable behind conformance boundaries;
- no completeness claim uses a self-shrinking inventory as its sole universe;
- the H0 architecture leaves a clean engine-bridge boundary for H1 without embedding engine object models into canonical contracts;
- whole-world CAS remains an explicit H0 concurrency boundary: the gate proves deterministic stale-writer rejection plus cheap structured recovery, not per-resource locking, automatic merging, multi-process writer coordination or multi-agent throughput;
- HK09A host-power policy is enforced below transports, while HK09B limits/persistence integrity apply equivalently to the accepted client paths.

## Hard blockers

The gate FAILs if any of these occur:

- direct editing of canonical world persistence or implementation files is required to complete the scenario;
- a public capability is usable but absent/inaccurate in canonical discovery;
- request/success/error schema is missing for a used capability;
- reference and MCP projections disagree semantically on the accepted gate surface, including HK08A/HK08B-added interaction semantics;
- invalid state can commit through a public mutation route;
- partial commit, silent stale overwrite, replay/hash divergence or unaudited mutation occurs;
- the representative same-lineage stale conflict can only be recovered by discarding the plan and reloading/reconstructing the complete world rather than using bounded HK08B recovery context;
- the representative coherent multi-resource authoring intent must be split into independently persisted halves solely because the accepted HK08A batch shape or HK09B limit is too small without evidence;
- an error requires reading source code to understand the corrective action;
- an undeclared shell/process/network/filesystem authority or transport-specific privilege contradicts the accepted HK09A boundary;
- an oversized/over-budget or interrupted HK09B operation can partially publish canonical state/evidence;
- hidden Unity/editor/network/model-vendor dependency is required for canonical H0 behaviour;
- the AI workflow degenerates into pathological one-field-per-request authoring contrary to the accepted HK08B interaction budget;
- the bounded HK10 long-session/endurance case exceeds an accepted HK09B H0 resource limit without an explicit resolved policy;
- proof infrastructure itself is missing or can omit required obligations from its own universe;
- an external component silently becomes the semantic source of truth for a canonical guarantee;
- `UNRESOLVED_PROOF_OBLIGATIONS != 0` or `KNOWN_UNDETECTED_DEFECT_CLASSES != 0`.

## Evidence

Gate evidence must include exact candidate SHA, clean-checkout commands, canonical discovered capability inventory, transport-conformance results, scenario transcripts, state hashes, semantic diffs, provenance/journal output, stale-conflict recovery transcript/anchors, HK08A batch metrics for the representative coherent edit, HK08B interaction metrics/budgets, HK09A capability-boundary evidence, HK09B limit/persistence evidence, bounded-session resource metrics, negative-conformance test summary, dependency/IP inventory for material H0 dependencies, reference-client result and independent AI trial transcript.

## Accepted gate result

The accepted candidate closes the full 14-stage readiness scenario on the exact frozen SHA through the public reference path, re-executes the accepted cross-transport, interaction-budget, host-authority, resource/persistence, endurance and full-regression surfaces, and preserves the HK01→HK10 semantic ownership boundaries rather than adding a new product subsystem at the gate.

The external AI-agent trial independently launched the exact-SHA self-contained MCP artifact, performed public `tools/list` discovery before product calls, derived argument shapes from returned schemas, completed plan/dry-run/apply and public inspection, consumed a structured invalid-state diagnostic, demonstrated no partial commit, repaired the request, validated the result and closed with snapshot/journal evidence without implementation-source access or hidden/private product calls.

The accepted repair also closes the two earlier Reviewer proof blockers: `RESIDUAL_RISK.md` consumes all 53 HK10 handoff rows and exact-SHA verification checks ID + classification equality against the predecessor handoff; G1 removes the real unfiltered stage-14 execution while preserving its declarative label and must RED through the same executable-wiring oracle used on the normal path. Effective foundational proof is `READY`, with zero unresolved proof obligations and zero known undetected in-boundary defect classes.

## PASS consequence

Independent Reviewer PASS on the exact gate SHA has now been obtained. H0 is complete and the roadmap may author detailed Engine Bridge / Unity-first workpacks.

A PASS does **not** authorize gameplay implementation directly; Unity must first receive its own downstream parity/bridge gate.

Per-resource concurrency, automatic disjoint-write merge, multi-process writer coordination and multi-agent scheduling remain post-GATE product work unless the gate itself produced measured evidence that one of them is required for the representative single-client authoring contract. The accepted GATE produced no such blocker. The post-GATE H0S scale/concurrency track in `Docs/ROADMAP.md` remains the default home for measured follow-up and may run in parallel with H1.
