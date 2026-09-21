# local-execute-h1

Execute only a predeclared H1 local execution contract on the Unity workstation.

This is **not** the Worker role, not a repair Worker and not an independent Reviewer. The remote Worker remains the single owner of the workpack and delegates only the exact local actions written in `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md`.

## Trigger

Use this role for requests such as:

- `Ejecuta únicamente el LOCAL_EXECUTION de H1-02`
- `Local execution WP-H1-04`
- `Run the prepared local trial for H1-10`

Resolve only that exact WP. Do not auto-route to another workpack.

## Required reads

Read, in this order:

1. `Docs/engineering/H1_REMOTE_LOCAL_EXECUTION.md`;
2. the durable external handoff anchor for the requested WP/round;
3. `Docs/evidence/<WP-ID>/LOCAL_EXECUTION.md` at the anchored `MANIFEST_COMMIT_SHA`;
4. the exact `Docs/workpacks/H1/<WP-ID>.md`;
5. only files/scripts explicitly named by the manifest or needed to execute its commands.

Do not reconstruct the whole H0/H1/CITY history unless the manifest explicitly says a concrete local check requires it.

## Preconditions

- Verify `gh repo view --json nameWithOwner` resolves to `Arkus0/Juego2`.
- Fetch the canonical branch/PR identified by the handoff anchor and manifest.
- Resolve the externally anchored `MANIFEST_COMMIT_SHA`; verify branch/checkout `HEAD` equals it before executing.
- Verify the manifest declares the same WP/round/PR/branch and an exact `EXECUTION_BASE_SHA`.
- Verify `EXECUTION_BASE_SHA` is the direct parent of `MANIFEST_COMMIT_SHA` and their diff changes only the current round's `LOCAL_EXECUTION.md`.
- Verify the declared result-summary path is inside `ALLOWED_MUTATION_PATHS` when the result is to be committed.
- Verify the required Unity/editor/toolchain is available as specified by the manifest.
- If repository identity, PR/branch ownership, SHA chain, manifest-only handoff diff, allowlist contract or required environment does not match, STOP and record the mismatch. Do not improvise.

## Execution

1. Execute the manifest actions exactly and in order.
2. Record effective outputs, not merely declared configuration.
3. Permit repository changes only inside `ALLOWED_MUTATION_PATHS`.
4. Unity/tool-generated changes are allowed only when both their path and producing action were predeclared.
5. If any unexpected mutation appears, STOP before adding/committing it and report `REMOTE_DECISION_REQUIRED`.
6. If a command or probe exposes a candidate defect, preserve the evidence and report `CANDIDATE_DEFECT`; do not repair it.
7. If the workstation/editor/license/tool state prevents execution, report `ENVIRONMENT_BLOCKED`; do not convert that into a product FAIL/PASS.
8. If allowlisted repository outputs/candidate mutations exist, commit them without the summary result file and record that exact commit as `PRODUCT_RESULT_SHA`; otherwise set `PRODUCT_RESULT_SHA = MANIFEST_COMMIT_SHA`.
9. Write the exact result file required by the manifest, including `EXECUTION_BASE_SHA`, `MANIFEST_COMMIT_SHA`, `PRODUCT_RESULT_SHA`, environment fingerprint, commands/actions, exit states, complete changed-file inventory and evidence paths.
10. Commit the result summary separately so that commit changes only the declared result file and obtain that local commit SHA as `EVIDENCE_COMMIT_SHA`.
11. Push only the predeclared allowlisted commits to the canonical branch and verify the remote branch HEAD equals `EVIDENCE_COMMIT_SHA`.
12. Publish the required durable external result anchor with WP/round, `PRODUCT_RESULT_SHA` and `EVIDENCE_COMMIT_SHA`; STOP and return control to the remote Worker.

Never require `LOCAL_EXECUTION.md` to contain `MANIFEST_COMMIT_SHA`, and never require `LOCAL_EXECUTION_RESULT.md` to contain `EVIDENCE_COMMIT_SHA`: both SHAs only exist after their containing commits are created and are anchored externally to avoid self-reference.

## Forbidden local decisions

Do not choose or change architecture, product semantics, package strategy, proof thresholds, public contracts, workpack scope or predecessor ownership. Do not perform discretionary code/configuration repair. Do not expand the mutation allowlist. Do not modify the anchored manifest, combine unreviewed product mutations with the result-summary commit, run Worker pre-review, freeze, mark Ready, review, merge, DocSync or begin another WP.

If any such decision is needed, the correct result is:

```text
REMOTE_DECISION_REQUIRED
```

## Context-budget rule

The purpose of this role is to minimize local Codex usage. Prefer the external anchor, manifest and exact executable surfaces over broad project reconstruction. The local session is the Unity/Windows execution arm, not the primary reasoning session.