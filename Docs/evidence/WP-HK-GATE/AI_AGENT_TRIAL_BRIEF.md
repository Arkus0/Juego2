# WP-HK-GATE — fresh independent AI-agent trial brief

AI_AGENT_TRIAL_STATUS: EXTERNAL_EXACT_SHA_REQUIRED
TRIAL_TRANSPORT: MCP_STDIO
IMPLEMENTATION_SOURCE_ACCESS: FORBIDDEN
RESULT_BINDING: EXACT_CANDIDATE_SHA

## Purpose

This is the mandatory second-client evidence for `WP-HK-GATE`. It must be executed by a **fresh independent AI-agent context** that has not read Arkus implementation source and has not relied on Worker reasoning. The Worker that authored this candidate is therefore ineligible to self-certify this trial.

The trial is not a benchmark of model intelligence. It asks a narrower product question: can an AI client begin from the public MCP surface, discover the contract and complete a representative authoring flow without hidden implementation knowledge?

## Exact-candidate rule

The trial must identify and operate against the exact candidate SHA frozen in PR #64. A transcript from another SHA is invalid. The trial must not modify the candidate branch or tracked files. Post the final transcript/verdict as a PR conversation comment so the evidence can bind to the frozen SHA without changing Git history.

## Allowed information

The agent may receive only:

- repository identity and exact candidate SHA;
- documented build/launch command for the MCP stdio host;
- this brief;
- MCP protocol/tool discovery results returned by the running host.

The agent must **not** read C# implementation files, unit tests, Worker evidence that reveals implementation mechanics, or source-derived capability arguments before discovery. It may use public tool names/schemas/diagnostics after they are returned by MCP discovery.

## Required representative task

Using MCP discovery and schemas as the source of truth, the agent must:

1. enumerate the available public tools/canonical identities and inspect the schemas needed for the task;
2. inspect the current world anchor;
3. author one coherent small market-town slice using generic canonical records: a plaza/root, a market/building contained by it, and an NPC-shaped record contained by the plaza with a typed relation to the market; opaque extension state may be added if the discovered contract makes it natural;
4. use the public planning/dry-run path before the accepted mutation;
5. inspect/query the authored result sufficiently to verify the intended identities, containment and typed relation;
6. intentionally submit one invalid authored change, consume the structured machine-readable diagnostic, repair the request and demonstrate that the invalid attempt did not partially commit;
7. export or otherwise obtain public provenance/snapshot evidence sufficient to identify the resulting accepted state and canonical hash.

The deterministic client already owns exhaustive stale-recovery, 96-operation batching, replay/restart, cross-transport, HK09A/B and endurance coverage. The fresh trial may exercise more, but it does not need to duplicate every deterministic stage to satisfy the second-client requirement.

## Evidence to post on PR #64

The independent agent's PR comment must contain:

- `AI_AGENT_TRIAL_VERDICT: PASS` or `FAIL`;
- `AI_AGENT_TRIAL_TARGET_SHA: <40-hex candidate>`;
- `IMPLEMENTATION_SOURCE_READ: NO`;
- `TRANSPORT: MCP_STDIO`;
- discovered capability/tool identities actually used;
- chronological request/response transcript or a lossless machine-readable attachment/link sufficient to audit the public interaction;
- final world revision/hash;
- invalid-request machine code and repair outcome;
- whether any hidden/private/product-source call was required (`NO` is required for PASS);
- any ambiguity, manual intervention or undocumented assumption encountered.

A PASS is invalid if the agent was preloaded with implementation-derived argument shapes, if the transcript cannot be tied to the exact frozen SHA, or if a human/Worker supplied private intermediate calls.

## Worker handoff

Until a qualifying external transcript exists, the Worker evidence must remain `FOUNDATIONAL_PROOF_VERDICT: NOT_READY` with one unresolved proof obligation. The exact-SHA freeze verifier intentionally fails closed unless PR metadata records the external PASS and links its PR comment.
