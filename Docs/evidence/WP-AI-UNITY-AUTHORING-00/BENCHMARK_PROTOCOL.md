# AI Unity operator benchmark protocol v2

Frozen before the first candidate run. Version 2 adds the owner's explicit replacement question from 2026-09-28; no candidate had begun its agent run when this was added. All candidate results use this protocol unless a deviation is recorded before scoring.

## Primary comparison operator

| Setting | Frozen value |
|---|---|
| Client | Codex CLI `0.156.1`, noninteractive `codex exec --json --ephemeral` |
| Provider/model | OpenAI via the owner's existing Codex sign-in; explicit model ID `gpt-6-sol` |
| Reasoning | `model_reasoning_effort=xhigh`; no candidate-specific extra budget or model |
| Prompt/brief | This protocol v2, `PRIMARY_OPERATOR_BRIEF.md` v2 and `REPLACEMENT_PROBE.md` v1; identical task prose for every candidate except the name of its Editor connection |
| Starting context | Same clean Juego2 Unity project from `main` `e8652d1cbd4d4eb77b4545c79c34e807b4ab4c73`, Unity `6000.3.24f1`, same copied broad lawful corpus inventory, same baseline package/settings state; candidate package is the only intended difference |
| Non-candidate tools | Codex CLI built-in local file/shell inspection and patch tools, no other MCP server and no additional asset-generation service; shell may inspect files but Unity authoring, observation and validation must use the candidate Editor surface |
| Filesystem/approval | CLI `workspace-write` rooted at the disposable project, `approval_policy=never`, no writes outside the trial project; candidate Editor commands follow the candidate's own permission settings |
| Permission rule | Discovery starts with broad code execution disabled or gated where practical. If the candidate needs such a tool for a task, record the reason and every use as rescue/high-authority operation. Do not hand a candidate unrestricted filesystem/network authority through the project. |
| Reset/retry | One fresh imported project per candidate from the same baseline. A failed operation may be retried once after observing the error. A candidate may perform its own diagnosis/correction; worker intervention is recorded. Do not supply exact asset paths until discovery has failed and the hint is logged. |
| Secondary pass | Candidate-native assistant, another model, tuned prompt or extra tool exposure, if tried, receives a separate log and cannot change the primary comparison score. |

CLI version and model ID are explicit. The provider does not expose a more granular model build identifier to this CLI; do not infer one. Each run records the exact executable version, command line, installed candidate package identity and project manifest fingerprint.

## Common briefs

**ENV:** Inspect the project and asset corpus, discover fitting pieces without a supplied prefab list, and build a small third-person Juego2-style street corner with a meaningful threshold. Combine whole assets and useful donor components as appropriate. Configure hierarchy, transforms, materials, bounded dressing, lighting, existing collision/navigation hooks, and a camera. Run Play Mode or another actual Unity validation, capture the result, inspect Console/runtime state, diagnose one visible or functional problem and correct/re-observe it. Trial art is allowed and must be labelled `TRIAL_ONLY_UNCURATED` or `CANDIDATE_FOR_LATER_ART_REVIEW` unless its source was already admitted.

**CHAR:** Discover available bodies, rigs, wardrobe, hair and accessories. Produce five visually distinct ordinary NPC presentation candidates as reusable prefabs/variants, place them together, validate Humanoid or a justified equivalent, inspect clipping/scale/rig/material failures, and repair or reject at least one bad combination. No final art, jobs, identity or gameplay roles.

**ANIM:** Discover a bounded mixed set of locomotion, ambient/acting, object and reaction clips and a real available character base. Inspect import/rig state, configure retarget/loop/root-motion choices and a minimum Animator/runtime presentation, play and capture the clips, identify one bad retarget/import case, and repair, adapt or reject it based on visual/runtime evidence.

For all three, the required loop is `inspect -> plan -> act in Unity -> run/observe -> diagnose -> correct -> re-observe`. The operator must report what it could not do instead of inventing screenshots or success.

**REPLACEMENT:** Execute the separate, bounded `REPLACEMENT_PROBE.md` after ENV/CHAR/ANIM. It tests whether the candidate can supply a practical AI-to-Unity authoring, inspection, validation, recovery and reconstruction workflow *without* Arkus. H0/H1 are the incumbent capabilities to compare, not an assumed architecture to preserve. Report any lost guarantee and the effort to replace it, but do not make Arkus compatibility a scoring prerequisite.

## Counting and scoring

An **agent action** is one tool call that reads or mutates Unity. An **owner/worker manual action** is one click/keystroke sequence that advances the scene beyond routine package launch, counted separately from an agent action. An **exact-path hint** gives a file/prefab path the operator failed to discover. A **rescue tool/script** is new code or a non-candidate service added solely to make the task work. A **recovery** is an editor restart, domain reload or manual repair caused by the trial. An **unsafe action prevented** is a proposed operation rejected or blocked by isolation/permission controls. An **agent visual correction** requires the agent itself to observe the defect and choose the change; a human-identified correction is separate. Count each event in a timestamped operation ledger, including zero counts.

Score each criterion per candidate from `0` (absent/failed), `1` (only with substantial rescue), `2` (partial with ordinary intervention), `3` (independent for the bounded brief), to `4` (independent, observed, repeatable and recoverable). Record the underlying calls/captures for each score. Criteria are those named in the WP comparative scorecard plus the separately reported replacement criteria; tool count is descriptive only. An `ADOPT` or `ADOPT_BOUNDED` operator conclusion additionally requires its production-constrained replay and like-for-like current-workflow baseline, irrespective of raw score.

The corpus and trial output are exploratory. No trial package or uncurated source is added to the production project by this WP. A recommendation to retire H0/H1/Arkus is a possible outcome of the replacement study, but a production architecture change still needs its own explicit reviewed decision after this experiment.
