# WP-H1-UNITY-CI — Post-PASS DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**
DATE: 2026-09-24

## Accepted result

- Exact candidate: `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf`
- Independent Reviewer: **PASS** (`#5299167558`)
- PR: `#166`
- Implementation merge: `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`
- Accepted H1 Unity CI Pilot run: `35936805407` / run #11
- Accepted artifact: `10783780236`

## Acceptance interpretation

The pilot proves a bounded GitHub-hosted Unity execution substrate against the already accepted H1-02 oracle: exact target checkout, Unity `6000.3.24f1 (4e7b9b5b6244)`, clean import without `Library` cache, EditMode `5/5 Passed`, effective coverage disabled, exact known GameCI package-metadata reconciliation, restoration to a tracked tree identical to the target SHA, and retained exact-SHA evidence.

The prior isolation contradiction is closed by an explicit exception limited to `Unity/ArkusUnity/Packages/manifest.json` and `Unity/ArkusUnity/Packages/packages-lock.json`, with exact known JSON deltas only. Any other tracked path/content drift or failure to restore the target tree remains RED.

## Adopted execution policy

GitHub-hosted Unity is now an accepted **optional effective execution substrate** for later H1 workpacks when the claim being proved is fully machine-verifiable and does not materially require human visual inspection, interactive authoring, GPU/appearance judgment, peripherals, or other physical/local-machine state.

For pre-existing H1 planning labels, `LOCAL_UNITY_REQUIRED` remains a conservative statement that effective Unity evidence is required; after this accepted pilot it does not force the owner's physical PC when the relevant proof satisfies the bounded cloud criteria above. `HYBRID` work may likewise combine remote contract tests with GitHub-hosted effective Unity evidence. Local Unity remains valid and becomes mandatory again whenever a claim materially depends on visual/interactive/physical-local evidence.

Each consuming workpack still owns its exact editor/project inputs, causal oracle, failure classification and retained evidence. This DocSync does not silently generalize the H1-02 `5/5` oracle to later workpacks, does not add Unity to Main Safety, and does not permit widening the GameCI package-drift exception without review.

This means H1-03 and H1-03A are eligible to execute end-to-end through Chat/GitHub-hosted infrastructure because their current acceptance claims are machine-verifiable host-policy/process/lifecycle claims rather than visual fidelity claims. Their semantic contracts and dependency order are unchanged.

## DocSync action

`WP-H1-UNITY-CI` is COMPLETE / ACCEPTED. The bounded remote-execution policy above is adopted as process infrastructure only; no H1 product/runtime semantics, H0 authority, CTX↔DW semantics, Unity catalogue behavior or later H1 acceptance oracle is changed.

No unrelated index, capsule, handoff cache or chronology-only documentation is regenerated.

`DOCSYNC_COMPLETE`
