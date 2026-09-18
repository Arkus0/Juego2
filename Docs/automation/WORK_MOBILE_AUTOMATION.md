# ChatGPT Work / Mobile Automation — Juego2

Version: 1.1 — 2026-09-18

These are the canonical trigger/condition/prompt contracts for cloud automation. Provider UI configuration is an adapter; GitHub state remains authoritative.

## Required model profile for H0

For every `HK-*` workpack through `WP-HK-GATE`:

- any `WORKER` or `REPAIR_WORKER` task: **GPT-5.6 Sol · Extra High**;
- any independent `REVIEWER` task: **GPT-5.6 Sol · Extra High**.

This is a task configuration requirement, not wording inside the prompt. Do not silently fall back to another model/reasoning level. If the profile is unavailable, leave the GitHub transition pending and persist `HUMAN_ACTION_REQUIRED: REQUIRED_MODEL_UNAVAILABLE`.

Finalizer, routine DocSync and recovery may use a cheaper configuration when mechanically constrained.

## 1. Juego2 Review Ready

**Model:** GPT-5.6 Sol · Extra High for H0.

**Trigger:** pull request marked Ready for review.

**Condition:** PR body has `Worker state: FROZEN_FOR_REVIEW`, `Branch frozen: YES`, an exact 40-char `Frozen candidate SHA`, and no live REVIEWER lease.

**Prompt:**

```text
Trabaja sobre Arkus0/Juego2. Actúa exclusivamente como Reviewer independiente del WP/PR que acaba de quedar Ready. Reconstruye el estado desde GitHub; no dependas de contexto privado del Worker. Verifica HEAD == Frozen candidate SHA, lee AGENTS.md, el WP exacto, WORKER_REVIEW_PROTOCOL.md y el proof standard aplicable. No modifiques implementación. Adquiere la lease REVIEWER, revisa adversarialmente el candidato, persiste PASS/FAIL/BLOCKED/READY_FOR_LOCAL_VALIDATION con el SHA exacto y libera la lease. Si esta sesión participó como Worker del candidato, STOP: HUMAN_ACTION_REQUIRED: NEED_FRESH_REVIEWER.
```

## 2. Juego2 Review Fail Repair

**Model:** GPT-5.6 Sol · Extra High for H0.

**Trigger:** review/comment persists `Reviewer verdict: FAIL`.

**Condition:** no later PASS, same WP unresolved, `fail_cycle < 4`, no live REPAIR_WORKER lease.

**Prompt:**

```text
Trabaja sobre Arkus0/Juego2 como nuevo Worker de reparación del mismo WP que recibió FAIL. Reconstruye el PR y el finding desde GitHub. Adquiere lease REPAIR_WORKER; conserva WP, PR, Worker history, Transfer SHA y fail_cycle; convierte a Draft antes de escribir; corrige sólo los defectos contractuales del FAIL; revalida todo el WP, actualiza evidencia, congela un nuevo SHA exacto, marca FROZEN_FOR_REVIEW/Ready y libera lease. No actúes como Reviewer.
```

## 3. Juego2 Review Pass Finalize

**Trigger:** review/comment persists `Reviewer verdict: PASS`.

**Condition:** `Reviewed candidate SHA == Frozen candidate SHA`, no later implementation mutation, required checks green, no live FINALIZER lease.

**Prompt:**

```text
Trabaja sobre Arkus0/Juego2 como finalizer. Reconstruye desde GitHub. Verifica PASS independiente exact-SHA y merge preflight. Adquiere FINALIZER lease. No modifiques implementación. Realiza sólo finalización documental permitida y merge cuando las reglas/permisos lo autoricen; si requiere aprobación humana, persiste HUMAN_ACTION_REQUIRED sin inventar consentimiento. Libera lease tras transición durable.
```

## 4. Juego2 Post Merge DocSync

**Trigger:** implementation PR merged.

**Condition:** DocSync not complete/NOOP and no live DOCSYNC lease.

**Prompt:**

```text
Trabaja sobre Arkus0/Juego2 como DocSync. Reconstruye el merge real desde GitHub, adquiere DOCSYNC lease y sincroniza sólo superficies documentales afectadas: ROADMAP, WP status, arquitectura/ADR, evidencia y SESSION_HANDOFF. Si no hay cambios, persiste DOCSYNC_NOOP indicando superficies verificadas. No implementes producto ni reclames el siguiente WP. Libera lease y sólo entonces permite nuevo DISCOVER.
```

## 5. Juego2 Recovery Reconciler

**Trigger:** scheduled backstop plus manual run-now.

**Prompt:**

```text
Reconstruye Arkus0/Juego2 desde GitHub y ejecuta una sola recuperación idempotente conforme a AGENTIC_PROTOCOL_DURABILITY.md. Si hay una transición pendiente sin lease viva, reanuda exactamente ese actor. No robes leases vivas, no conviertas quota/provider failure en FAIL y no cierres PRs no terminales. Ambigüedad => HUMAN_ACTION_REQUIRED. Nada pendiente => RECOVERY_NOOP.
```

Recommended backstop: every 2 hours. Event triggers provide responsiveness; reconciler provides durability.
