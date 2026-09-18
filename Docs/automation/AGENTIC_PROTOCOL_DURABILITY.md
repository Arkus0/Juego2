# Agentic Protocol Durability

Version: 1.0 — 2026-09-18

## Purpose

One backend-neutral durable state machine for Juego2 across ChatGPT Work, Codex Desktop/local and Claude Code cloud. GitHub is persistent operational truth; provider sessions/tasks are disposable execution state.

## Minimal entry

On an accepted backend, a human may say:

```text
Ponte a trabajar en Arkus0/Juego2.
```

The backend reconstructs `main`, resolves dependencies, selects exactly one safe eligible WP, and follows Worker → Reviewer → finalization → merge → DocSync. It never invents evidence or skips gates.

## Canonical state machine

```text
DISCOVER
 -> WORKER_ACTIVE
 -> REVIEW_PENDING / FROZEN_FOR_REVIEW
 -> REVIEW_ACTIVE
      FAIL -> REPAIR_PENDING -> REPAIR_ACTIVE -> REVIEW_PENDING
      PASS -> FINALIZATION_PENDING -> FINALIZATION_ACTIVE
      BLOCKED -> durable STOP
 -> MERGED / DOCSYNC_PENDING
 -> DOCSYNC_ACTIVE
 -> DOCSYNC_COMPLETE | DOCSYNC_NOOP
 -> DISCOVER
```

## Interruption invariant

Quota exhaustion, provider/model outage, app crash, task-start failure, host disconnect or lost session are recoverable backpressure. They are never Reviewer FAIL and never increment `fail_cycle`.

Duplicate triggers are expected and must be idempotent. A pending GitHub state remains authoritative until a role actor acquires a lease.

## Actor lease

Before role-specific mutation, persist:

```text
WORK_ACTOR_LEASE_V1
Role: REVIEWER | REPAIR_WORKER | FINALIZER | DOCSYNC
State: ACTIVE
Backend: WORK_MOBILE_CLOUD | CODEX_DESKTOP_LOCAL | CLAUDE_CODE_CLOUD
Target SHA: <exact SHA>
Lease acquired: <ISO-8601>
Lease expires: <ISO-8601>
Run/session reference: <optional>
```

Never steal a live lease. Expired leases may be replaced only after reconstructing GitHub state.

## Role invariants

### Reviewer
Only for exact frozen candidate; fresh independent context; no implementation repair; persists exact-SHA verdict.

### Repair Worker
Only after persisted FAIL; same WP/PR; increments `fail_cycle` only for that independent FAIL; Draft before writes; freezes new exact SHA; never self-reviews.

### Finalizer
Only after exact-SHA PASS; no implementation mutation; merge preflight; human approval stop when repository/account policy requires it.

### DocSync
Only after real merge; reconcile ROADMAP/WP/architecture/evidence/handoff; minimal process-only change or explicit NOOP; next DISCOVER only after completion.

## Recovery reconciler

Production automation must detect pending role states without live leases and resume at most one missing actor. Ambiguous ownership/state => `HUMAN_ACTION_REQUIRED`.

Recommended backstop: every 2 hours plus event triggers where available.

## Backend acceptance

A backend is `PRODUCTION_READY` only after harmless tests prove: correct dependency selection, exact-SHA freeze, independent review, FAIL repair routing, PASS finalization routing, merge→DocSync, interruption persistence, lease recovery and idempotency.
