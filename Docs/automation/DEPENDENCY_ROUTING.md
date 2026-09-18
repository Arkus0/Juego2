# Dependency Routing

Version: 1.0 — 2026-09-18

A human target is a goal, not permission to skip prerequisites.

## Authority

Reconstruct the DAG from current `main` immediately before routing. Priority:

1. exact WP dependency declarations;
2. milestone plans when present;
3. `Docs/ROADMAP.md` gates/order;
4. binding architecture/process documents;
5. real accepted/merged GitHub state.

Do not infer dependencies from filename order alone.

## Algorithm

```text
requested target
 -> resolve current contractual DAG
 -> executable now?
      yes: evaluate target
      no: collect unmet transitive prerequisites
 -> identify minimal contractual path
 -> select earliest SAFE + ELIGIBLE pending prerequisite
 -> choose backend for that node
 -> execute normal Worker→Reviewer→merge→DocSync
```

Node states: `UNMET | BLOCKED | OWNED | ELIGIBLE | SATISFIED`.

## H0 route

Unless an accepted later contract changes it:

```text
HK-00 -> HK-01 -> HK-02 -> HK-03 -> HK-04 -> HK-05
      -> HK-06 -> HK-07 -> HK-08 -> HK-09 -> HK-10 -> HK-GATE
```

H1/Unity remains blocked until `HK-GATE` is accepted.

## Ownership / failure

Routing never steals an owned WP. Reviewer FAIL normally keeps the same WP as the unresolved prerequisite and routes to repair, not to a later WP.

Two independent FAILs exposing the same foundational class trigger architecture re-audit under the foundational proof standard.

## Ambiguity

If multiple contractual paths are equivalent and no priority exists:

```text
HUMAN_ACTION_REQUIRED: AMBIGUOUS_DEPENDENCY_ROUTE
```

Never choose by convenience, numeric suffix, model preference or backend availability.

## Re-resolution

Recompute from current `main` after every merge, DocSync, gate, contract change or ownership transfer. Never hardcode `next = X` across accepted transitions.
