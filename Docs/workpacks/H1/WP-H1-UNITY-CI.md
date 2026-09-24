# WP-H1-UNITY-CI — GitHub-hosted Unity execution pilot

Status: COMPLETE / ACCEPTED
Class: PROCESS_INFRASTRUCTURE / NON-PRODUCT
Execution: GITHUB_HOSTED_UNITY
Blocks: nothing
Blocked by: nothing beyond the already accepted H1-02 oracle
Acceptance: exact candidate `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf`; independent PASS review `#5299167558`; PR `#166`; implementation merge `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`.
Post-PASS reconciliation: `Docs/evidence/WP-H1-UNITY-CI/DOCSYNC.md`.

## Objective

Test one bounded infrastructure claim before changing H1 execution policy:

> Can the already accepted H1-02 Unity project execute its effective EditMode proof on a GitHub-hosted runner, from an exact repository SHA, with enough evidence to make cloud Unity a valid optional execution substrate for later H1 workpacks?

This pilot is deliberately not an H1 product workpack and does not reopen H1-02. It does not block H1-03 or H1-03A.

## Frozen oracle

Use the accepted H1-02 frozen candidate:

`d86a08e644f542e9515f5e54fd4061f61e251c70`

Accepted local evidence records:

- Unity `6000.3.24f1 (4e7b9b5b6244)`;
- project `Unity/ArkusUnity`;
- EditMode suite `5/5` passed;
- Force Text and Visible Meta Files effective;
- Built-in render pipeline retained;
- `packages-lock.json` present;
- repository-owned Unity `.cs` / `.asmdef` files have visible `.meta` files.

The cloud pilot must not weaken or rewrite that oracle to obtain GREEN.

## Pilot execution

`.github/workflows/h1-unity-ci-pilot.yml` executes only the already accepted H1-02 SHA for the PR pilot. Manual dispatch may target another exact SHA only under the accepted bounded execution policy recorded by the post-PASS DocSync; the consuming workpack remains responsible for its own exact-SHA oracle and proof requirements.

The first run intentionally disables the Unity `Library` cache. This makes the first observation a clean import/execution test rather than a cache-reuse test.

Pinned execution inputs:

- GitHub-hosted `ubuntu-24.04` runner;
- GameCI CLI `v0.1.69`, Linux x64 release binary SHA-256 `d847fe7b0131a00c521c51b0e6987b356301bb7121b69d9266c9414e78832329`;
- project version read from the checked-out `ProjectVersion.txt`;
- `testPlatforms: editmode`;
- `coverageEnabled: false` for parity with the bounded H1-02 oracle.

The workflow invokes the pinned GameCI CLI directly rather than routing this boolean through `game-ci/unity-test-runner@32e57712352b500e17974b245a6dce9e11a73213`. That wrapper translates `coverageEnabled: false` into `--no-coverageEnabled`, which GameCI CLI `v0.1.69` rejects. Direct invocation uses the CLI's supported `--coverageEnabled false` form, preserving the same GameCI Docker execution path while making the disabled-coverage requirement effective instead of declarative.

## Bounded GameCI package-metadata exception

The isolation rule permits exactly one transient, tool-generated exception during the pinned GameCI execution. GameCI may modify only these two tracked paths:

- `Unity/ArkusUnity/Packages/manifest.json`;
- `Unity/ArkusUnity/Packages/packages-lock.json`.

That exception is valid only when the workflow proves all of the following in the same run:

1. the pre-Unity checkout is clean and exactly equals the frozen target SHA;
2. the complete set of tracked paths changed by GameCI is exactly the two paths above;
3. `manifest.json` differs from the target only by the known GameCI Linux package injection pinned in the workflow;
4. `packages-lock.json` differs from the target only by the corresponding known GameCI lock entries pinned in the workflow;
5. any other tracked path or any other content delta is RED as an isolation failure;
6. both files are restored from the exact target SHA after reconciliation; and
7. the tracked working tree is then identical to the target SHA before evidence is accepted.

This is not permission for product/source drift and does not generalize to later GameCI versions, package deltas, paths, or workpacks. A changed CLI binary/hash or changed package injection requires a new explicit review rather than silently widening the exception.

## License boundary

Unity activation is an execution prerequisite, not product authority. The workflow expects GitHub Actions secrets named:

- `UNITY_LICENSE`;
- `UNITY_EMAIL`;
- `UNITY_PASSWORD`.

For Unity Personal, current GameCI guidance is to activate a Personal license through Unity Hub, locate the resulting `.ulf`, and store its contents plus Unity account credentials as GitHub Actions secrets. No license material may be committed to the repository or uploaded as ordinary build evidence.

Missing secrets are a setup failure, not evidence against Arkus or H1.

## PASS

The pilot may be called PASS only if one GitHub-hosted execution against the frozen H1-02 SHA proves all of the following:

1. exact checkout equals the accepted H1-02 candidate;
2. effective Unity editor test reports the pinned `6000.3.24f1` version;
3. the existing EditMode suite executes in Unity and returns exactly `5/5` passed with zero failures;
4. the effective tests still prove Force Text, Visible Meta Files, Built-in render pipeline, package lock presence, and visible-meta coverage because those are the accepted H1-02 tests, not duplicated workflow assertions;
5. coverage is effectively disabled for the cloud execution;
6. tracked repository state remains unchanged by execution except for the bounded transient GameCI package-metadata exception above, whose exact two-path delta must be reconciled and restored so the final tracked tree is identical to the frozen target SHA;
7. test artifacts and an exact-SHA pilot receipt are retained by Actions.

## FAIL / INCONCLUSIVE

Classify causally:

- missing/invalid Unity credentials or unavailable Personal activation: `SETUP_BLOCKED`, not product FAIL;
- GameCI image/tooling cannot execute the pinned Unity patch: infrastructure FAIL;
- Unity launches but the accepted five tests do not all pass: substantive portability FAIL;
- coverage is enabled despite the pinned disabled setting: execution-contract FAIL;
- result cannot be bound to the exact H1-02 SHA: evidence FAIL;
- any tracked mutation outside the bounded two-path exception, any mismatch from its exact known package delta, or any failure to restore a tracked tree identical to the target SHA: isolation FAIL.

Do not repair H1-02 product semantics merely to make the cloud pilot green.

## Adoption consequence

PASS authorized a separate, explicit process change making GitHub-hosted Unity an optional effective execution substrate for later H1 workpacks when their claims do not require human visual inspection or physical/local-machine state. The post-PASS DocSync linked above enacts that bounded process change.

This acceptance does **not**:

- make Unity run on every commit;
- add this lane to Main Safety;
- replace independent Reviewer judgment;
- prove PlayMode, standalone builds, graphics fidelity, scene appearance, animation appearance, GPU behavior or interactive authoring;
- eliminate local/visual Unity execution when a later claim materially requires it.

After PASS, a later optimization may add a `Library` cache and exact-input receipt reuse, but only after the clean pilot remains the baseline oracle.