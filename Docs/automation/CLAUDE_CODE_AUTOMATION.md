# Claude Code Automation — Juego2

Claude Code may act as Architect, Worker, Reviewer, Finalizer or DocSync only under the same GitHub state machine as every other backend.

## Model policy separation

The OpenAI H0 requirement `GPT-5.6 Sol + Extra High/xhigh` does **not** apply literally to Claude Code. Do not invent a fake equivalence between OpenAI and Anthropic model names or effort controls.

For foundational `HK-*` Worker/Repair Worker/Reviewer roles, Claude Code should use the strongest configured Claude reasoning/coding profile available for that backend. The backend must still satisfy exactly the same WP proof obligations, exact-SHA freeze and independent-review rules.

Claude configuration is maintained independently from `.codex/config.toml` and ChatGPT Work task model settings.

## Invariants

- GitHub is truth; Claude session memory is disposable.
- Exact WP/contract and current `main` are read before work.
- Dependency routing precedes backend selection.
- One live Worker per WP candidate.
- A context that implemented/directed a frozen candidate cannot independently review it.
- Reviewer does not repair implementation.
- Quota/provider interruption is backpressure, not FAIL.
- No cloud backend claims local/editor evidence it did not execute.

## Minimal entry

```text
Ponte a trabajar en Arkus0/Juego2.
```

Reconstruct state, select the first safe eligible contractual node, acquire the appropriate role lease, perform one durable transition, persist the result and stop at the next role boundary.

## Foundational WPs

For HK work, Claude must treat the proof matrix, self-attacks, effective/evaluated behavior and exact-SHA evidence as implementation requirements rather than documentation cleanup.
