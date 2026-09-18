# Agent Model Policy — Juego2

Version: 1.1 — 2026-09-18

Repository correctness is defined by contracts/evidence, but H0 deliberately standardizes the OpenAI execution profile for its highest-risk roles.

## H0 OpenAI profile

For every foundational `HK-*` workpack through `WP-HK-GATE`:

| Role | Required OpenAI model | Required reasoning |
|---|---|---|
| Worker | GPT-5.6 Sol | Extra High / xhigh |
| Repair Worker | GPT-5.6 Sol | Extra High / xhigh |
| Independent Reviewer | GPT-5.6 Sol | Extra High / xhigh |

Do not silently downgrade these roles. If the selected OpenAI backend cannot provide this profile, leave the role pending and return `HUMAN_ACTION_REQUIRED: REQUIRED_MODEL_UNAVAILABLE`.

Reviewer independence is contextual, not model-brand diversity: a fresh Sol xhigh session is independent; a different model carrying Worker implementation context is not.

## Why xhigh for H0

HK work is architectural/proof work: completeness arguments, adversarial omission search, deterministic contracts, transactional semantics, evaluated compiler/runtime behavior, fuzz/property closure and exact-SHA evidence. A false PASS has downstream cost far above the extra inference cost.

## Architect

Use a frontier high-reasoning configuration for milestone architecture, dependency resets, foundational contract design and repeated-failure analysis. Architect is not automatically the independent Reviewer of a candidate it materially directed.

## Codex Desktop

Juego2 provides `.codex/config.toml` with:

```toml
model = "gpt-5.6-sol"
model_reasoning_effort = "xhigh"
plan_mode_reasoning_effort = "xhigh"
```

That is the project default for Codex Desktop/CLI. Before HK implementation/review, verify no higher-priority CLI/user override has changed the effective model/effort.

## ChatGPT Work / mobile

Any task that performs `WORKER`, `REPAIR_WORKER` or `REVIEWER` for H0 must be instantiated with GPT-5.6 Sol at Extra High. Finalization/DocSync/recovery may use lower-cost configurations when they are mechanically constrained and cannot change implementation or foundational verdicts.

## Claude Code

Claude Code is configured separately. Do not translate `Sol xhigh` into a fake Claude model name. Claude follows its own backend policy and should use the strongest configured model/reasoning tier for foundational Worker/Reviewer roles while preserving the same independence and proof obligations.

## Routine DocSync / recovery

May use faster models when the transition is mechanically constrained and all authoritative state comes from GitHub.

## Escalation

Even outside H0, escalate to the strongest available reasoning configuration when:

- a foundational claim receives FAIL;
- two repair loops expose the same class;
- contract sources conflict;
- the proposed fix grows syntax-specific exceptions;
- scope/dependency architecture may need reset.
