# CTX-01 — Context Bootstrap v1 dry-runs and controls

Mode: `PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL`  
Historical baseline: `02016ba5a3d0a344525835652bbd84c9a2e9cc49`  
Reconciled live main used for this run: `3f1247a2289351d32a5d8c1f3bbbcbee32db6983`  
Candidate branch: `process/ctx-01-role-bootstrap`

## Main reconstruction / #107 compatibility

Live GitHub reconstruction shows current `main = 3f1247a2289351d32a5d8c1f3bbbcbee32db6983`, merge of PROCESS_ONLY PR #107. CTX-01 is reconciled with that main and is not behind it.

PR #107 makes PROCESS_ONLY validation truthful: when no canonical product/runtime command executes, Automation V2 emits no synthetic `EXECUTION_RECEIPT_V1`, no `Result: GREEN` execution claim and no fabricated canonical-gate proof. CTX-01's profiles/router/index neither require nor invent such a receipt. Its applicable proof is its own process evidence plus the Worker lifecycle/handoff checks.

## Mechanical checker rerun

Executed the exact candidate `scripts/context-bootstrap-check.py` source in an isolated Python temp directory with the candidate profile/index JSON contents.

Results:

```text
python3 scripts/context-bootstrap-check.py --self-test
exit 0
context-bootstrap self-test: PASS

python3 scripts/context-bootstrap-check.py \
  --current-main-sha 3f1247a2289351d32a5d8c1f3bbbcbee32db6983 \
  --require-fresh
exit 0
state_index: FRESH
mutable_hints_usable: true

python3 scripts/context-bootstrap-check.py \
  --current-main-sha 02016ba5a3d0a344525835652bbd84c9a2e9cc49 \
  --require-fresh
exit 2
state_index: STALE
mutable_hints_usable: false
on_stale: RECONSTRUCT_FROM_LIVE_GITHUB_AND_AUTHORITATIVE_CONTRACTS
```

The built-in self-test also proves missing required profile and missing compact-context file fail closed.

## Representative dry-run A — Worker PA-04

Requested role/task: `Worker WP-PA-04`.

Initial profile selection:

```text
AGENTS.md
Docs/engineering/CONTEXT_BOOTSTRAP_V1.md
Docs/workpacks/PA/WP-PA-04.md
Docs/engineering/WORKER_REVIEW_PROTOCOL.md
live GitHub mutable state
```

Observed authoritative state:

- PA-04 is `RESEARCH / NON-FOUNDATIONAL`, execution `REMOTE_HARVEST`;
- exact WP depends on `WP-PA-03 PASS + merge + DocSync`;
- PR #100 is merged and its final independent review PASS binds frozen candidate `d216f32f0c82bf57c23a3c7ef0c4433cbcbdc927`; merge is `2f3862b601b0521a6a3d5a57afe54f182d037e97`;
- current main/workpack index records PA through PA-03 and PA-04 as the next contract;
- no canonical PA-04 implementation PR is open in live GitHub.

Observable routing decision:

```text
CONTEXT_CLOSED: eligibility/dependency state is closed by exact PA-04 + accepted PA-03 evidence + live GitHub.
```

Initial reads intentionally do **not** include `FOUNDATIONAL_PROOF_STANDARD.md` or full `Docs/ROADMAP.md`. PA-04's own required inputs/donor provenance still have to be opened before implementation; bootstrap savings do not remove workpack-required sources.

Control 1 PASS: unrelated foundational proof material is omitted by default for this non-foundational harvest. If a material later question actually binds a foundational claim, the profile's `foundational_claim` trigger requires escalation before deciding it.

## Representative dry-run B — Worker H1-02

Requested role/task: `Worker WP-H1-02`.

Base profile selection is the Worker pack, then exact-contract triggers add:

```text
Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md
Docs/engineering/FOUNDATIONAL_PROOF_STANDARD.md
accepted direct predecessor evidence for WP-HK-GATE
```

Observed authoritative state:

- H1-02 is `FOUNDATIONAL TOOLCHAIN`, `LOCAL_UNITY_REQUIRED`, `REMOTE_FIRST_STAGED`;
- exact WP depends on accepted `WP-HK-GATE`;
- `Docs/workpacks/H1/README.md` records only H1-00 and H1-01 complete and names H1-02 as the next dependency-valid H1 workpack;
- no canonical H1-02 implementation PR is open in live GitHub.

Observable routing decision:

```text
CONTEXT_CLOSED: H1-02 eligibility is closed, but foundational proof + H1 remote/local overlay are mandatory before planning execution.
```

Control 1 second half PASS: when the exact claim binds foundational proof, the profile loads it. Control 6 PASS: the remote Worker keeps semantic/design/predecessor/pre-review authority; the separate `h1_local_executor` profile remains manifest/anchor/exact-WP-only and returns `REMOTE_DECISION_REQUIRED` for semantic choices.

## Representative dry-run C — Reviewer exact WP

This is a **routing simulation only**, not an independent review or verdict. Candidate used for context selection: future fresh Reviewer of exact `WP-CTX-01` / canonical PR #106 after freeze.

Initial Reviewer pack:

```text
AGENTS.md
Docs/engineering/CONTEXT_BOOTSTRAP_V1.md
Docs/engineering/WORKER_REVIEW_PROTOCOL.md
Docs/workpacks/CTX/WP-CTX-01.md
live canonical PR + complete baseline→Frozen diff
accepted direct predecessor evidence: CTX programme plan PR #102 / PASS / merge / DocSync
```

Because CTX-01 is PROCESS_ONLY / NON-PRODUCT-FOUNDATIONAL, foundational proof is not loaded merely because the repository contains foundational history. Full ROADMAP is not initially required unless a material order/cross-track question appears.

Observable routing decision:

```text
CONTEXT_CLOSED_FOR_REVIEW_START: compact state may navigate, but authoritative predecessor evidence and complete frozen diff must be opened independently before any verdict.
```

Control 5 PASS: the Reviewer profile explicitly refuses Worker summary/index as proof. A real accepted example already demonstrates the intended behavior: PA-03's final Reviewer independently reconstructed PA-02 accepted evidence before PASS rather than trusting Worker prose.

## Representative dry-run D — blocked CITY-04 with observable cross-track escalation

Requested role/task: `Worker WP-CITY-04`.

Initial exact WP states:

```text
Depends on: WP-CITY-03 PASS + WP-H1-08 PASS
```

The first dependency is accepted on current main. The second is cross-track and not accepted. Because eligibility crosses CITY→H1, the Worker profile fires the cross-track escalation trigger **before** deciding eligibility.

Observable routing trace:

```text
ESCALATE:CROSS_TRACK_DEPENDENCY
  -> open Docs/workpacks/H1/README.md
  -> deepen to Docs/ROADMAP.md / exact H1 dependency state as needed
  -> observe H1 only through H1-01 accepted; H1-08 is later in the H1 DAG
BLOCKED: WP-H1-08 PASS + merge + DocSync is missing
```

Control 2 PASS: ROADMAP/track deepening is observable rather than hidden behind a false `CONTEXT_CLOSED` decision.

## Required negative/process controls

### C1 — non-foundational default vs later foundational binding

PASS via PA-04 and H1-02 dry-runs above. Selection is claim-bound, not repository-history-bound.

### C2 — cross-track dependency forces deepening

PASS via CITY-04 dry-run above. Exact CITY WP alone exposes a cross-track dependency; the decision records `ESCALATE:CROSS_TRACK_DEPENDENCY` before returning BLOCKED.

### C3 — stale compact/README state cannot override live GitHub

PASS with a concrete stale surface: open PROCESS_ONLY guide PR #86 still describes an old snapshot (`WP-PA-01` available; CITY-05-era state). Current accepted main has PA through PA-03 and CITY through CITY-03 on the accepted non-numeric spine. Context Bootstrap v1 therefore treats the guide as non-authoritative navigation and uses live GitHub + current exact contracts.

A derived index generated from the old historical CTX-01 baseline was also mechanically evaluated against current main and returned `STALE`, exit 2 with `--require-fresh`.

### C4 — state index cannot create product semantics

PASS. The index declares `authority: DERIVED_NAVIGATION_ONLY` and every hint carries source pointers. A hypothetical edited hint such as `H1-08 accepted` cannot establish that fact: current H1 authoritative state says only H1-00/H1-01 are complete, so the contradiction forces authoritative reads and the hint is rejected. The checker intentionally validates freshness/shape, not semantic truth; semantic authority stays outside the projection.

### C5 — Reviewer independent authoritative reads

PASS via Reviewer dry-run above. Reviewer starts from exact WP/live PR/direct predecessor evidence and independently opens original accepted predecessor proof when material.

### C6 — H1 local executor remains narrower

PASS. The `h1_local_executor` profile starts from H1 remote/local protocol → durable anchor → anchored manifest → exact H1 WP → manifest-named executable surfaces. Any required semantic/design/repair decision returns `REMOTE_DECISION_REQUIRED`; it does not inherit the remote Worker profile.

### C7 — generated main SHA mismatch invalidates hints

PASS mechanically:

```text
generated_from_main_sha = 3f1247a2289351d32a5d8c1f3bbbcbee32db6983
current_main_sha        = 02016ba5a3d0a344525835652bbd84c9a2e9cc49
=> STALE / mutable_hints_usable=false / exit 2 with --require-fresh
```

Additionally, transient PR/branch/Ready/HEAD/review/check state is never cached in the reusable index, because those facts can change without `main` moving and therefore cannot be protected by this SHA comparison.

### C8 — missing compact context escalates, never infers from silence

PASS. The checker self-test removes a required Reviewer profile and separately requests a missing compact-context file; both fail closed. Protocol behavior is then to reconstruct from live GitHub + authoritative contracts or stop, never interpret missing data as `not accepted`, `not blocked`, or `safe`.

## Scope check

These runs exercise CTX-01 only: selection, authority, stale detection, escalation and role routing. They introduce no accepted-contract capsules (CTX-02), no structured proof/history migration or measurement baseline (CTX-03), no Automation V2 redesign, no H2/Juego3 work and no new product/runtime semantics.

`CTX01_DRY_RUNS: PASS`
