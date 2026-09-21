# ADR-H1-002 — Generational Unity projection and explicit reconciliation

Status: ACCEPTED — H1 planning PR `#71`; reviewed candidate `58b0d78a57b8c617d167e6bf286a6cbd29b0612b`; PASS review `#5263596722`; merge `09ce3fb495d331285bef0ab8aebf4c6117c84d57`
Date: 2026-09-20

## Decision

Unity materialization is a separate external-reversible operation after canonical authoring. It builds a staged managed generation, validates and observes it, and publishes a current-generation manifest only after success. Canonical state is never rolled back or rewritten to match a failed Unity projection.

Unity-to-canonical synchronization is explicit: observation may produce a canonical mutation proposal, but only H0 `plan/dry-run/apply` can commit it. No background auto-pull or dual-master merge exists in H1.

## Why

There is no truthful atomic transaction spanning the accepted in-process H0 authority and Unity's asset/scene database. Pretending otherwise would create false provenance or make Unity authoritative. A generation boundary gives a precise active projection, isolates partial output and makes rebuild/drift evidence reviewable.

## Consequences

- canonical-ahead is a valid, diagnosable state after projection failure;
- retries are anchored/idempotent on the full input tuple;
- incomplete staging artifacts are non-authoritative;
- manual edits are drift until explicitly captured through a proposal;
- H1 claims project-checkpoint/rebuild, not general crash/power-loss atomicity.
