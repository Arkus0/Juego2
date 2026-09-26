# WP-H1-GATE — fresh independent AI-agent trial brief

AI_AGENT_TRIAL_STATUS: HOSTED_EXACT_SHA
TRIAL_TRANSPORT: MCP_STDIO
IMPLEMENTATION_SOURCE_ACCESS: NONE (structural: the agent is offered only the MCP tools of the running host)
RESULT_BINDING: EXACT_CANDIDATE_SHA (workflow checkout + artifact digest)

This file is published as the trial agent's instructions. The relay `scripts/h1-gate-ai-trial.py` sends the text
between the two `BRIEF` markers below, verbatim, as the agent's only instructions. Nothing else from the repository is
given to the agent. That excludes this header, all source code, tests, Worker evidence and the deterministic scenario.

The owner selected a hosted route. The relay starts the fixed public host command below on the exact candidate, inside
the pinned Unity image, and forwards **only** tool calls that the model issues. It never supplies an intermediate call.
Every model message, tool call and tool result is recorded in `trial-transcript.jsonl`. The verifier
(`h1-gate-verify.py trial`) and its negative controls check that record.

<!-- BRIEF START -->
You are a fresh, independent evaluator of the Arkus public authoring surface for the Unity engine bridge. You have not
seen any Arkus implementation source, tests or design notes, and you must not assume any. Your only source of truth is
the MCP tool list you are offered and the schemas and results those tools return.

Environment: one public Arkus MCP host is running. It was started with the fixed H1 Unity launch profile
(`Arkus.Harness.Mcp --h1-unity`) on a Unity project that already contains an imported, licensed third-party asset
library. Some tools run work inside a short-lived Unity Editor process. Those calls can take a minute or more, and that
is normal. You cannot start processes, read files or reach Unity except through the offered tools.

Goal: prove, using only the public tools, that a new client can author a small game-world slice canonically and see it
realized in Unity, with truthful diagnostics, and reconstruct it from a checkpoint. Work through these steps and adapt
to what the tools tell you:

1. Discover. Identify the capability families, their schemas and how canonical authoring, Unity catalogue, Unity binding,
   projection and checkpoint capabilities relate. Prefer tools that describe the system when you are unsure.
2. Inspect the Unity side: the project/profile, and the catalogue of available prefabs, assets, materials and animation
   clips. Page through large results rather than guessing identifiers.
3. Author a small bounded slice through canonical authoring. Use only identifiers you discovered. Include at least:
   one root object placed from a catalogue prefab or asset; one child object contained by it; one humanoid object that
   references an animation clip; one object with a renderer material reference; and one canonical reference from one
   object to another. Attach the Unity binding intent through the public binding tools, then commit through the
   canonical plan → dry-run → apply path.
4. Realize the slice in Unity, then inspect the result through the public observation tools. Confirm that it matches
   what you authored.
5. Intentionally break one Unity reference through canonical authoring, for example a catalogue identifier that does
   not exist. Try the projection path again, read the structured diagnostic, and check that nothing false was published
   as the current Unity state. Then repair the reference through canonical authoring and realize the slice again.
6. Create a project checkpoint. Then use the public restore or rebuild capability to reconstruct the Unity projection,
   and confirm the observation afterwards.
7. Finish with a final report.

Rules: use only the offered tools and arguments that satisfy their schemas. Never invent hidden parameters. When a call
fails, use the structured error (machine code, path, repair hint) to decide what to do next. Record any ambiguity or
missing information you had to work around. Keep calls purposeful; you do not need to test every tool.

Final report: when you are done (or blocked), reply with no tool calls and a single fenced ```json block containing:
{"verdict": "PASS" or "FAIL", "canonicalHash": "...", "revision": 0, "journalEntryCount": 0,
 "activeGenerationId": "...", "graphDigest": "...",
 "diagnosticConsumed": {"tool": "...", "machineCode": "..."}, "repaired": true,
 "checkpointId": "...", "rebuildGraphDigest": "...",
 "toolsUsed": ["..."], "ambiguities": ["..."], "manualIntervention": "NONE", "hiddenOrPrivateCalls": "NONE"}
Report PASS only if every step above succeeded through the public tools.
<!-- BRIEF END -->

## Verification

A PASS is accepted only when `h1-gate-verify.py trial` is GREEN on the exact candidate SHA. The verifier requires:

- every tool call was issued by a model message and names an offered, discovered MCP tool;
- the authoring, materialize, observe, checkpoint and restore/rebuild capabilities were exercised;
- at least one structured `projection.*`/`unity.*` diagnostic was returned to and consumed by the agent;
- the closing canonical hash, active generation and observation digest are taken from the recorded tool results, not
  from the agent's claims;
- the artifact digest and candidate SHA match.

The deterministic reference scenario remains the authoritative oracle. The trial is not the deterministic oracle; it
proves the new public-client claim only.
