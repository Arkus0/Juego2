# Owner-directed replacement probe v1

Question from the owner on 2026-09-28: can a better AI-to-Unity tool make H0, H1 and Arkus unnecessary for Juego2? Treat replacement as a real possibility. This probe adds decision evidence to the accepted operator WP; it does not itself delete or change production authority.

Use only the same candidate Editor connection and frozen primary agent configuration as the ENV/CHAR/ANIM comparison. Work in the disposable project and use trial-only output. Do not call an Arkus authoring API or use Arkus-generated scene state as the implementation of the probe. An existing Arkus file may be read to understand what the incumbent provides, but the candidate must do the work directly.

## Bounded task

1. From the broad corpus, create a small editable scene/recipe with at least five semantically named objects, a three-level hierarchy, a sibling pair, one cross-reference and one deliberate invalid/missing-asset proposal. Use logical names/IDs that remain understandable without Unity instance IDs or GUIDs.
2. Before changing Unity, show the proposed change set and whether the tool can preview/dry-run it. Apply the valid part and save the scene/recipe. Observe the actual scene through a machine-readable readback and a visual capture; record Console/validation findings.
3. Attempt the invalid proposal. Determine whether the tool rejects it cleanly and whether the valid saved scene remains intact. Record partial-mutation behavior and structured errors if present.
4. Make one controlled Editor-side drift (move one object and remove another), then ask the candidate to detect the difference from its own intended recipe and repair it. Record whether this needs custom code, manual comparison, direct scene inspection or an existing tool primitive.
5. Restart/reload the Editor connection if feasible, then reconstruct or replay the same scene from the saved recipe in a clean scene. Compare object identity, hierarchy, transforms, asset references and cross-reference with the prior machine-readable observation. If a clean project copy is needed, stop and request the Worker to perform the reset as a counted intervention rather than editing outside the trial project.

## Replacement report

For each capability mark `NATIVE`, `CONFIGURED`, `CUSTOM_CODE`, `MANUAL`, `MISSING` or `NOT_TESTED`, with a concrete trace: discoverable authoring contract; logical identity; inspect/readback; dry-run; atomic multi-object change; validation and structured error; provenance/change trace; deterministic replay/reconstruction; drift detection/repair; package and asset reproducibility; use by an external AI agent; and ability to replace the incumbent H0/H1 workflow at acceptable maintenance cost.

Report the smallest viable stack that would replace Arkus, what source of truth it uses, what would be lost, what custom code remains necessary and which claims remain untested. Do not mark a tool inferior solely because it is not Arkus-compatible. Do not claim full replacement from a successful scene screenshot alone.
