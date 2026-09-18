# Automation Bootstrap — Juego2

This file is the minimal entry contract for supported agent backends.

## Human fast path

```text
Ponte a trabajar en Arkus0/Juego2.
```

The actor must then:

1. read `AGENTS.md`;
2. reconstruct current `main`, open PRs and role leases from GitHub;
3. read `Docs/automation/DEPENDENCY_ROUTING.md`;
4. select exactly one first safe + eligible contractual action;
5. choose a backend capable of its evidence class;
6. obey `WORKER_REVIEW_PROTOCOL.md` and `AGENTIC_PROTOCOL_DURABILITY.md`;
7. stop at the next role boundary.

## H0 OpenAI model requirement

For every foundational `HK-*` workpack through `WP-HK-GATE`, OpenAI-backed implementation and independent review use the high-assurance profile:

```text
Model: GPT-5.6 Sol
Reasoning: Extra High / xhigh
```

This requirement applies to:

- `WORKER` implementing an HK workpack;
- `REPAIR_WORKER` repairing an HK Reviewer FAIL;
- `REVIEWER` independently reviewing a frozen HK candidate.

The Reviewer must still run in a fresh independent context. Using the same model does not weaken independence; sharing Worker context does.

Do not silently fall back to Luna, Terra, Medium or High for those roles. If Sol xhigh is unavailable on the selected OpenAI backend, persist/return:

```text
HUMAN_ACTION_REQUIRED: REQUIRED_MODEL_UNAVAILABLE
```

and leave the contractual transition pending.

`FINALIZER`, routine `DOCSYNC`, transition routing and recovery may use a cheaper/faster configuration when their work is mechanically constrained and cannot alter implementation or foundational conclusions.

### Codex Desktop

The repository pins the local Codex default in `.codex/config.toml` to `gpt-5.6-sol` + `xhigh`. Before an HK Worker or Reviewer starts, verify the effective Codex session has not overridden that setting. Worker and Reviewer still require separate contexts.

### Claude Code

Claude Code is a separate backend/model family. The Sol/xhigh requirement does not map onto Claude model names. Claude follows `Docs/automation/CLAUDE_CODE_AUTOMATION.md` and its own strongest-role configuration while preserving the same GitHub contracts and Reviewer independence.

## Setup mode

If the backend has not passed its harmless acceptance test, it is `SETUP/VALIDATING`, not `PRODUCTION_READY`. Setup may configure adapters and run harmless protocol tests but must not pretend automation is durable before proving it.

## Current product boundary

H0 is headless/engine-agnostic. No Unity/editor backend is required until an accepted H1 workpack says otherwise.

## Safety

Never embed credentials in prompts/docs. GitHub/Telegram/provider secrets remain in their native secret stores.
