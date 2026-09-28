# WP-AI-UNITY-AUTHORING-00 — AI Unity production-operator benchmark

Status: **CONTRACT ACCEPTED FOR EXECUTION / NOT_STARTED**  
Accepted: PR `#274`, candidate `2544307e17419099920139f4543940dd51773bcf`, independent Reviewer PASS `#5336640510`, merge `9646ef769ed65bfbe7a18cc28829bdce46eac36f`.  
Class: PRODUCT TOOLING / VISUAL-PRODUCTION ACCELERATION SPIKE  
Execution: **ISOLATED REAL-UNITY EVIDENCE REQUIRED**; may run before `WP-H2F-GATE` on a disposable branch/worktree/project copy and must not mutate the active H2F candidate.  
Depends on: accepted `WP-H2F-02` baseline + a reconstructable current Unity project + lawful access to the owner/project asset corpus used by the benchmark.  
Does **not** block: `WP-H2F-03`, `WP-H2F-GATE`.  
Feeds, if evidence supports adoption: `WP-ART-CHAR-01/02`, `WP-ART-ANIM-01/02`, `WP-ART-ENV-02`, `WP-ART-URBAN-01`, `WP-ART-03`.

## Question

Can an external AI agent operate the real Unity Editor with enough autonomy, observability and repeatability to become a high-leverage **operator of Juego2's content factories**, rather than merely a code-writing assistant?

This is a productivity/authoring spike, not a mandate to adopt another architecture. It must end in an evidence-backed disposition:

- `ADOPT` — one candidate is suitable as the default visual-production operator;
- `ADOPT_BOUNDED` — useful only for named lanes/tasks (for example ENV + ANIM but not CHAR);
- `TOOL_SOURCE` — the candidate proves valuable capabilities, but Juego2 should expose only a smaller reviewed subset through its own tooling/adapter;
- `REJECT` — measured leverage is insufficient or risk/fragility is too high.

Any of these dispositions may PASS if the evidence is complete and the conclusion follows from it.

## Candidates

Benchmark at minimum:

1. **Unity official MCP / AI Assistant tool surface** available for the pinned Unity generation used by Juego2;
2. **KitWright Unity MCP** (open-source/MIT at planning time) or its current equivalent if the exact package has materially changed before execution;
3. **CoplayDev/unity-mcp (MCP for Unity)** or its current maintained equivalent. At planning time it is MIT, actively maintained and materially comparable: it exposes scene/GameObject and asset operations, script editing/validation, tests, profiling/build/editor controls and supports external clients including Codex/Claude-style agents.

Record exact versions/commits, terms/license, installation/provisioning path, prerequisites, tool exposure, permissions, network/cloud dependence and any project files/settings added by each candidate. A materially changed candidate must be re-audited rather than treated as identical by name.

The mandatory list is a **floor, not a closed allowlist**. At execution time, any newly discovered candidate may be added to the benchmark **without another roadmap amendment** when it is plausibly decision-changing and satisfies all of the following:

- lawful and actually obtainable for the spike;
- exposes a materially comparable real-Unity authoring/observation surface rather than only chat/code generation;
- can be tested under the same isolation, agent-control and evidence rules as the mandatory candidates;
- its inclusion does not itself adopt a production/runtime dependency.

For every such addition, record why it was included and pin its exact identity before running it. A candidate may also be excluded after discovery, but the evidence ledger must state the concrete reason (for example abandoned/unmaintained, incompatible Unity generation, non-comparable capability surface, unacceptable terms, or impossible provisioning). This prevents the benchmark from becoming stale merely because a strong third/fourth candidate appears after this WP was authored.

Do not silently turn any benchmark candidate into a production dependency. Production adoption still requires the handoff and dependency/IP + H1-lifecycle proof defined below.

## Comparison control — freeze the agent/model, not just the MCP

The primary benchmark exists to compare **Unity operator surfaces**, so the agent side must be controlled.

Before the first candidate run, freeze and record a **primary comparison operator configuration** containing at minimum:

- agent/client identity and version;
- model/provider and exact model identifier/version where the provider exposes one;
- reasoning/effort mode and other material inference settings;
- system/worker prompt and benchmark brief version;
- available non-candidate tools/connectors;
- context inputs and starting evidence supplied to the agent;
- approval/permission mode;
- retry/reset policy;
- candidate tool-group exposure policy and any per-candidate unavoidable deviations.

Run every candidate's decision-bearing comparison with that same primary configuration and the same task briefs/counting rules. If a setting cannot be made identical because of candidate architecture, record the deviation and its likely effect before scoring.

Candidate-native assistants, a different model, a tuned prompt, or a higher reasoning budget may be explored in a **secondary capability pass**, but those results must be reported separately and may not be used to declare one MCP/operator surface superior to another in the primary scorecard.

A run with an unrecorded or materially different agent/model/configuration is non-comparable and cannot support `ADOPT` / `ADOPT_BOUNDED`.

## Critical rule — open exploration corpus, not ART-curated corpus

**The benchmark must not be artificially limited to assets already approved by ART.** Its first purpose is to measure the ceiling of the operator.

The agent may inspect/search/use the broadest **lawful and actually available** corpus accessible in the isolated benchmark environment, including where present:

- current admitted Juego2 assets;
- uncurated Quaternius source material;
- historical/previously filtered source assets;
- environment, character, wardrobe, prop and animation material not yet dispositioned by ART;
- reusable components from fantasy/medieval or otherwise mismatched source packs;
- existing project prefabs, materials, textures, meshes, animations and local derivatives;
- lawful owner-provided packages/assets already available for evaluation.

Pack label, current ART rejection, timber percentage, genre origin or lack of prior curation are **not** reasons to hide material from this spike.

However, exploration authority is not production authority. Every benchmark-produced artifact must carry one of:

- `TRIAL_ONLY_UNCURATED` — used to test capability; no production approval implied;
- `PRODUCTION_INPUT_ALREADY_ADMITTED` — source was already allowed by the normal product contracts;
- `CANDIDATE_FOR_LATER_ART_REVIEW` — potentially useful discovery that remains non-authoritative.

No result from this WP may silently upgrade an uncurated asset into `KEEPER_READY`, bypass `DEPENDENCY_IP_POLICY.md`, change the Visual Bible, or override ART/CITY/H2 ownership. Downstream production still performs the normal adoption/visual/provenance review.

The open corpus does **not** authorize purchases, credential sharing, scraping restricted services, unlicensed downloads, or committing licensed/source bytes contrary to their terms. If an asset is locally available but cannot lawfully be committed, evidence may record stable identity/provenance and captures without adding the bytes to git.

## Benchmark A — ENV autonomous scene construction

Give the agent a bounded Juego2-style brief plus access to the open exploration corpus. Do **not** hand it exact prefab paths or a preselected final kit unless discovery failure makes that necessary and the intervention is recorded.

The agent must attempt to:

1. inspect project/scene/package state;
2. discover candidate assets itself;
3. select and combine useful whole assets and/or donor components;
4. create a small third-person street/corner/threshold composition;
5. configure transforms, hierarchy, materials and bounded dressing;
6. configure lighting/presentation enough for visual inspection;
7. preserve or add existing collision/navigation hooks where supported;
8. enter Play Mode or otherwise perform real-Unity validation;
9. capture the result;
10. inspect console/runtime state and its own output;
11. perform at least one evidence-driven correction pass.

The result does not need to be keeper art. This benchmark asks whether the agent can autonomously transform a broad asset corpus into a coherent inspectable scene and repair obvious mistakes.

## Benchmark B — CHAR batch production

Using actual available character/body/rig/wardrobe/hair/accessory material, ask the agent to produce a representative batch of **5–10 visually distinct ordinary NPC presentation candidates**.

The agent must attempt to:

- discover usable body/rig and clothing sources without exact per-item paths supplied;
- combine/reuse/adapt available pieces where technically possible;
- create material/palette/accessory variation where supported;
- configure/validate Humanoid or the accepted equivalent;
- create reusable prefabs/variants rather than scene-only one-offs;
- place the batch together for visual comparison;
- detect and report obvious invalid combinations such as severe clipping, floating pieces, broken scale/rig or missing material state;
- repair or reject at least one bad combination.

The benchmark does **not** require final faces, final Juego2 wardrobe approval, gameplay identity, jobs, schedules or narrative roles.

## Benchmark C — ANIM retarget + runtime validation

Using real available animation sources and at least one actual accepted-or-trial character base, ask the agent to process a bounded mixed set of locomotion + acting/ambient/object/reaction clips.

The agent must attempt to:

- discover candidate clips;
- inspect import/rig settings;
- configure Humanoid/retarget mapping where appropriate;
- distinguish root-motion/in-place/loop needs;
- create or update the minimum Animator/runtime presentation needed for the test;
- run the clips in Unity;
- capture evidence;
- detect/report at least one deliberately selected or naturally occurring bad-retarget/import case;
- repair, adapt or reject it rather than accepting Unity's technical import success as visual success.

## Observation + self-correction requirements

A candidate is not useful merely because it can write C# or instantiate objects. The benchmark must show whether it can close the loop:

`inspect -> plan -> act in Unity -> run/observe -> diagnose -> correct -> re-observe`

Record whether each candidate can access, without manual copy/paste where its design claims support:

- hierarchy/scene state;
- assets and project metadata;
- components/serialized values;
- prefab state;
- Animator/animation state;
- compilation status and Console output;
- Play Mode;
- screenshots/Game/Scene view captures;
- input/runtime simulation where available;
- profiler/performance evidence where available;
- Undo/recovery/change trace.

## Intervention accounting

For every benchmark keep an operation/intervention log separating:

1. agent actions;
2. owner/worker manual clicks;
3. exact-path hints supplied after failed discovery;
4. custom scripts/tools written only to rescue the benchmark;
5. restarts/recompiles/recovery steps;
6. irreversible or unsafe actions prevented;
7. visual corrections the agent discovered itself versus corrections supplied by a human.

The key productivity oracle is not wall-clock time. It is whether the operator converts ordinary production into **brief + review** rather than repeated low-level Unity manipulation.

## Comparative scorecard

Score all candidates with evidence, not impressions, on:

- project-state comprehension;
- asset discovery across the open corpus;
- scene/prefab authoring breadth;
- character assembly usefulness;
- animation/Animator usefulness;
- Play Mode/runtime control;
- visual/readback observability;
- self-correction quality;
- deterministic/repeatable operations;
- recovery after script/domain reload;
- Undo/change trace and damage containment;
- amount of custom rescue tooling required;
- compatibility with Codex/Claude-style external agents;
- licensing/cost/cloud/account burden;
- H1 lifecycle/reconstructability impact if later adopted;
- fit with Arkus authority rather than replacement of Arkus.

A high raw tool count is not itself a PASS signal.

## Architecture boundary

If adopted, the tool is an **Editor operator**, not canonical game-state authority.

```text
Arkus / accepted product contracts
            -> CITY / ART / GC2 briefs + recipes
            -> AI production operator
            -> Unity Editor / project
            -> evidence + candidate content
            -> normal validation / adoption / keeper gates
```

The operator may accelerate physical authoring, discovery and validation. It may not silently own persistent gameplay identity, replace H0/H1 authority, invent canonical semantics, bypass H1 materialize/reconcile guarantees, or turn raw plugin/editor state into hidden product truth.

## Isolation and damage containment

Because these candidates can mutate the Editor directly, execution must use an isolated branch/worktree/project copy or equivalent disposable environment. At minimum:

- snapshot/freeze starting git SHA and Unity/package versions;
- preserve a clean rebuild path;
- no credentials in repo/prompts/logs;
- no destructive filesystem scope outside the benchmark project;
- inspect candidate safety/permission configuration before broad tool exposure;
- keep broad `execute_code`/equivalent escape hatches disabled or approval-gated where practical during initial discovery, then test them explicitly only if needed;
- record generated/modified files and package manifest changes;
- prove the benchmark can be discarded without affecting the active H2F candidate.

## Production-constrained replay + current-workflow baseline

The open-corpus pass measures ceiling. Adoption additionally requires proof that the operator works under the real product constraints rather than only in a permissive sandbox.

For any final disposition of `ADOPT` or `ADOPT_BOUNDED`:

1. replay at least one successful representative task for **every lane named in the proposed adoption** using only currently admitted production inputs and the then-current binding ART/CITY rules applicable to that task;
2. keep the same frozen primary agent/model/configuration and intervention counting rules used by the comparison unless a documented product constraint forces a deviation;
3. do not weaken ART/CITY acceptance, hand-pick exact asset paths, or add rescue tooling merely to make the candidate pass;
4. run a like-for-like **current Juego2 workflow baseline** for the same constrained brief without the candidate operator, recording the same categories of manual intervention, exact-path assistance, rescue scripts/tools, recovery operations and human visual corrections;
5. compare operator replay versus baseline explicitly. An adoption claim must show a material reduction in low-level manual manipulation and/or rescue burden while preserving the required evidence/quality boundary.

For `ADOPT_BOUNDED`, evidence outside the adopted lane is informative but cannot substitute for the required constrained replay inside each adopted lane.

`TOOL_SOURCE` and `REJECT` may PASS without this constrained replay if the evidence already supports those non-adoption conclusions, but the report must say that no production-operator adoption is being claimed.

## PASS acceptance contract

Mandatory evidence:

1. exact candidate/version/provisioning/terms ledger, including every execution-time candidate added or excluded under the material-candidate rule;
2. isolated-environment proof and starting SHA;
3. frozen primary agent/client/model/configuration ledger plus any unavoidable per-candidate deviations;
4. open-corpus inventory boundary stating what the agent could see;
5. ENV benchmark log + before/after/correction evidence;
6. CHAR batch benchmark log + group inspection evidence;
7. ANIM benchmark log + good/bad retarget evidence;
8. manual-intervention and rescue-tooling counts under explicit counting rules;
9. comparative scorecard based on the frozen primary comparison, with secondary different-model/native-agent experiments clearly separated;
10. lifecycle/reconstructability impact assessment;
11. risk/residual ledger;
12. final `ADOPT | ADOPT_BOUNDED | TOOL_SOURCE | REJECT` disposition with named downstream consequences;
13. for `ADOPT` / `ADOPT_BOUNDED`, production-constrained replay evidence for every adopted lane plus the like-for-like current-workflow baseline and explicit manual-intervention/rescue-tooling comparison;
14. if adopted in any form, a precise handoff identifying which later WP owns production integration and which package/tool state must be dependency/IP + H1-lifecycle proven before keeper use.

## FAIL conditions

FAIL if any of the following is true:

- the agent is pre-fed exact asset paths/hand-selected final assemblies so heavily that autonomous discovery is not measured;
- ART curation is used to hide difficult/unclassified material and therefore inflate the apparent success rate;
- a materially comparable execution-time candidate is silently ignored without a recorded exclusion reason;
- the primary candidate comparison uses materially different models/agent configurations, or fails to record those controls, yet treats the result as an MCP/operator-surface comparison;
- a flashy screenshot substitutes for operation logs, runtime/console evidence and repeatability;
- a candidate requires substantial bespoke rescue tooling yet is declared a productivity win without accounting for it;
- `ADOPT` / `ADOPT_BOUNDED` is declared without the required production-constrained replay and current-workflow intervention/rescue baseline for every adopted lane;
- uncurated benchmark output is silently promoted to production authority;
- licensed/proprietary bytes are committed unlawfully;
- the spike mutates or destabilizes the active H2F candidate;
- adoption is declared from tool-count marketing rather than Juego2 benchmark evidence;
- Arkus/H1 authority is replaced implicitly rather than through a separately reviewed architecture decision.

## Non-claims

This WP does not build the final scenario factory, final NPC factory, final animation library, procedural city generation, final visual identity, Living World behavior, dialogue, combat or the shipping content set. It only determines whether an AI-controlled Unity operator can materially accelerate those later factories and, if so, within what bounded role.

## Planning-time external anchors

These are discovery anchors only; execution must re-check current versions/terms:

- Unity official AI/MCP overview: https://unity.com/blog/unity-ai-mcp-how-to-get-started
- Unity AI Assistant package/docs: https://docs.unity.com/en-us/engine/6000.6/manual/packages-list/packages-all/pack-safe/com-unity-ai-assistant
- KitWright Unity MCP: https://github.com/kitwright/unity-mcp
- CoplayDev MCP for Unity: https://github.com/CoplayDev/unity-mcp