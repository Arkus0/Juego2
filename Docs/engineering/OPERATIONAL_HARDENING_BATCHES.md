# Operational hardening — three-batch delivery

Status: ACTIVE PROCESS IMPROVEMENT PLAN
Date: 2026-09-22
Authority: process/tooling only; this document does not change any workpack product acceptance criterion.

## Purpose

Deliver the operational improvements found by the September process audit without creating a new multi-WP product track or changing the acceptance contract of work already in flight. The work is grouped into three large PROCESS_ONLY pull requests. Each batch is compatibility-first: add the replacement, prove it, then retire legacy machinery only in the later batch.

The operating model is **remote/chat-first**. Worker, repair, Reviewer and finalization reasoning normally happens in remote role sessions against live GitHub state. A phase is "local" only when the applicable WP explicitly requires machine/editor/toolchain evidence that cannot be truthfully produced remotely; even those WPs may have remote reconstruction, implementation, review or finalization phases around the local evidence step.

Opus, when used for this audit stream, is a **read-only analyst**. It may inspect, compare, challenge and propose improvements, but it does not mutate repository state and does not become the implementation Worker merely because an idea originated in its analysis. Repository mutations remain owned by the explicitly invoked Worker/repair/finalization role under the normal independence rules.

DW-02 and other active work continue under the contract/cycle they started with. Batch A's Worker preflight becomes mandatory only for implementation or repair cycles that first enter `DRAFT + ACTIVE` after Batch A has merged to `main`. A candidate or cycle already active, frozen or in review before that adoption point is not reopened merely to satisfy the new preflight. A later fresh repair cycle begun after adoption is governed by it.

## Batch A — runtime hardening and early defect detection

Owner branch: `process/operational-hardening-a`

- Worker preflight with two equivalent execution paths for post-adoption cycles. Prefer `scripts/worker-preflight.sh` on the clean exact candidate HEAD when the Worker environment can execute the exact SDK from `global.json`. If that environment lacks the exact SDK, use the repository-owned `Worker Candidate Preflight` GitHub Actions run for the canonical PR and exact candidate SHA. The delegated run checks out `pull_request.head.sha`, installs the exact SDK, runs normal restore + Release build/tests + process self-tests, rechecks the live PR head before emitting a durable `PR + SHA + run_id` GREEN receipt, and is cancelled/replaced when the PR advances. A local capability miss is therefore not `NOT_READY` by itself; a missing/mismatched/stale delegated receipt is.
- Canonical Worker/repair/protocol adoption of that preflight for post-adoption cycles, with already-active/frozen cycles grandfathered rather than retroactively reopened. Any candidate mutation invalidates the prior local result and any prior delegated receipt because both are exact-SHA evidence.
- Repository-pinned Stryker.NET runner for targeted, advisory mutation testing. Mutation score is not a WP PASS criterion; surviving mutants in the touched semantic surface are Worker pre-review evidence.
- `Arkus Main Safety` build/test/process safety workflow on pushes to `main` and on pull requests that can affect build/restore/process execution. Its path allow-list explicitly covers current central inputs (`Directory.Build.props`, `Directory.Packages.props`, `NuGet.config`) plus future `Directory.*.props`, `Directory.*.targets`, arbitrary `.props/.targets`, project/solution and NuGet lock/config inputs. `scripts/validate-main-safety-trigger.py` contains positive/negative regression tests and is executed by CI/preflight so this coverage cannot silently shrink.
- `scripts/validate-worker-preflight-context.py` contains negative controls for stale candidate, missing PR identity and cross-repository reuse. The delegated workflow runs those controls before execution and rejects a PR whose live head no longer equals the candidate before issuing GREEN.
- Python bytecode/cache hygiene.
- Claude Code skill adapters that delegate to `.agents/skills` rather than duplicating process authority.
- Clarify that GitHub Actions can enforce CI/protocol/state transitions and may execute the bounded Worker preflight contract, while never impersonating an independent reasoning role session.

No persistent workstation dependency is introduced. Workers that already have the exact SDK use the cheaper local path; chat/cloud Workers without it consume the exact-context delegated run instead. Locked restore remains deferred until reviewed `packages.lock.json` files exist for the complete solution. Branch protection is enabled only after Batch B supplies the stable merge-gate check.

## Batch B — deterministic process automation

Required scope:

- `docsync.py` write/check flow driven by canonical acceptance identity (WP, reviewed candidate SHA, PR, review/verdict, merge identity), never by an editable `Status: PASS` acting as its own oracle.
- A checker-owned generic verifier registry/schema with compatibility execution against existing frozen per-WP verifiers. Legacy verifiers remain available during the equivalence period.
- Standard PA disposition table schema/selector rules so future PA acceptance does not require checker code edits solely to locate the canonical table.
- Safe same-SHA receipt reuse for PR-body edits, with a digest over every mutable input actually consumed by the handoff/checker path.
- One stable pull-request merge-gate status suitable for branch protection, plus deterministic checks that generated DocSync/navigation surfaces have not drifted.
- All new automation must be usable by the normal remote/chat workflow; it may not assume a persistent human workstation or a long-lived local agent.

Exit criterion: the generic path and the legacy path agree on a representative accepted/negative corpus, and branch protection can require the stable merge gate without class-specific false failures.

## Batch C — lifecycle convergence and legacy retirement

Required scope:

- Reviewer launch deduplication bound to `PR + exact SHA`, with expiry/recovery so an abandoned claim cannot deadlock review. Deduplication coordinates disposable remote sessions; it must not introduce a long-lived reviewer daemon.
- Remove consumed conditional-adoption prose from live role context and preserve historical rationale under `Docs/history/` where useful.
- Neutralize legacy active-code test terminology (`self-attack`, attack fixture, bypass naming) without rewriting frozen evidence/history.
- Retire or archive obsolete H0-only workflow/material such as `hk00-ci.yml` and superseded HK-06..09 only after proving no live consumer still depends on the path.
- Remove duplicated legacy verifier/dispatcher machinery only after Batch B equivalence evidence is GREEN; frozen historical SHAs remain valid evidence.

Exit criterion: no live automation depends on the retired paths, reviewer duplicate launch is causally prevented, and all moved/renamed active surfaces remain covered by the normal build/process checks.

## Audit-item ownership

| Audit improvement | Owner |
| --- | --- |
| Pre-review .NET build/test with local-preferred exact-SHA Actions fallback | A |
| Mutation testing for causal test strength | A |
| Mechanical DocSync automation | B |
| Required/stable merge safety + post-merge safety | A + B |
| Generic verifier | B, retirement in C |
| PA selector/schema standardization | B |
| Duplicate Reviewer prevention | C |
| Safe receipt reuse on `edited` | B |
| Remove consumed adoption conditionals | C |
| Claude Code skill availability | A |
| Neutral active-code terminology | C |
| H0 leftovers/history | C |
| Clarify CLAUDE/Actions wording | A |
| Python cache hygiene | A |

No item in this table is silently dropped. An item may be superseded only by an independently reviewed change that documents an equal-or-stronger replacement.

## Safety rules across all three batches

1. Add before delete. Replacement automation coexists with the frozen/legacy path until equivalence is demonstrated.
2. No self-defined completeness. Registry/schema required fields and DocSync acceptance identity are checker-owned.
3. Exact-SHA identity remains mandatory wherever a result claims to validate a candidate. Delegated Worker-preflight evidence additionally binds canonical PR identity and workflow run ID.
4. A body-only edit may reuse computation only when the complete consumed-input digest is unchanged; candidate-SHA changes always invalidate Worker-preflight evidence.
5. Mutation testing is diagnostic until a separate reviewed decision establishes a stable causal gate; no arbitrary mutation-score threshold is introduced here.
6. Active product WPs are not reopened merely because process tooling improved after their current cycle began; new post-adoption cycles use the new tooling prospectively.
7. Remote/chat execution is first-class. Lack of a local .NET SDK selects delegated exact-SHA preflight rather than making the Worker intrinsically unready. Local evidence remains required where the applicable product contract explicitly requires local/editor/toolchain truth.
8. Read-only audit agents do not gain repository mutation authority through authorship of recommendations.
