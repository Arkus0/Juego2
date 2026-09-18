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

## Setup mode

If the backend has not passed its harmless acceptance test, it is `SETUP/VALIDATING`, not `PRODUCTION_READY`. Setup may configure adapters and run harmless protocol tests but must not pretend automation is durable before proving it.

## Current product boundary

H0 is headless/engine-agnostic. No Unity/editor backend is required until an accepted H1 workpack says otherwise.

## Safety

Never embed credentials in prompts/docs. GitHub/Telegram/provider secrets remain in their native secret stores.
