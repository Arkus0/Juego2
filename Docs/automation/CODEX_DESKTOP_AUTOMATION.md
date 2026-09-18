# Codex Desktop Automation — Juego2

Codex Desktop/local is the preferred backend when a WP requires real local tools or later Unity/editor evidence. During H0 it is simply another Worker/Reviewer execution backend and does not get special architectural authority.

## H0 model profile

For every `HK-*` workpack through `WP-HK-GATE`, Codex Desktop/CLI must use:

```text
Model: gpt-5.6-sol
Reasoning effort: xhigh
```

The repository-level `.codex/config.toml` pins this as the project default. Before an HK Worker or Reviewer begins, verify the effective session configuration (for example via Codex status/model controls) has not been overridden by a higher-priority CLI or local setting.

Do not silently downgrade an H0 Worker/Repair Worker/Reviewer. If Sol xhigh cannot be used, leave the role pending and return `HUMAN_ACTION_REQUIRED: REQUIRED_MODEL_UNAVAILABLE`.

Routine DocSync/recovery may explicitly use a cheaper configuration when they cannot modify implementation or foundational verdicts.

## Rules

- Reconstruct from GitHub before acting.
- Resolve the contractual WP first, backend second.
- Respect leases and one-Worker-per-WP ownership.
- Worker and independent Reviewer must use separate contexts even when both use Sol xhigh.
- Local availability is evidence capability, not permission to skip review.
- Host asleep/offline is recoverable backpressure, never contractual FAIL.
- Never claim LOCAL evidence unless the actual required toolchain/editor executed it.

## Minimal entry

```text
Ponte a trabajar en Arkus0/Juego2.
```

Apply `AGENTIC_PROTOCOL_DURABILITY.md` + `DEPENDENCY_ROUTING.md` and execute at most the first safe eligible transition.

## Later Unity boundary

When H1 exists, LOCAL-UNITY WPs may route here only after the exact WP defines the required Unity/editor evidence. H0 must not assume Unity exists.
