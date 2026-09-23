# WP-H1-UNITY-CI — GitHub-hosted Unity execution pilot

Status: PILOT / NOT ADOPTED
Class: PROCESS_INFRASTRUCTURE / NON-PRODUCT
Execution: GITHUB_HOSTED_UNITY
Blocks: nothing
Blocked by: nothing beyond the already accepted H1-02 oracle

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

`.github/workflows/h1-unity-ci-pilot.yml` executes only the already accepted H1-02 SHA for the PR pilot. A later manual dispatch may target another exact SHA only after this pilot is reviewed.

The first run intentionally disables the Unity `Library` cache. This makes the first observation a clean import/execution test rather than a cache-reuse test.

Pinned execution inputs:

- GitHub-hosted `ubuntu-24.04` runner;
- `game-ci/unity-test-runner` pinned by commit `32e57712352b500e17974b245a6dce9e11a73213`;
- GameCI CLI `v0.1.69`;
- project version read from the checked-out `ProjectVersion.txt`;
- `testMode: editmode`;
- coverage disabled for parity with the bounded H1-02 oracle.

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
5. tracked repository source remains unchanged by execution;
6. test artifacts and an exact-SHA pilot receipt are retained by Actions.

## FAIL / INCONCLUSIVE

Classify causally:

- missing/invalid Unity credentials or unavailable Personal activation: `SETUP_BLOCKED`, not product FAIL;
- GameCI image/tooling cannot execute the pinned Unity patch: infrastructure FAIL;
- Unity launches but the accepted five tests do not all pass: substantive portability FAIL;
- result cannot be bound to the exact H1-02 SHA: evidence FAIL;
- workflow mutates tracked source: isolation FAIL.

Do not repair H1-02 product semantics merely to make the cloud pilot green.

## Adoption consequence

PASS authorizes a separate, explicit process change making GitHub-hosted Unity an optional effective execution substrate for later H1 workpacks when their claims do not require human visual inspection or physical/local-machine state.

PASS does **not**:

- make Unity run on every commit;
- add this lane to Main Safety;
- replace independent Reviewer judgment;
- prove PlayMode, standalone builds, graphics fidelity, scene appearance, animation appearance, GPU behavior or interactive authoring;
- eliminate local/visual Unity execution when a later claim materially requires it.

After PASS, the next optimization may add a `Library` cache and exact-input receipt reuse, but only after the clean pilot remains the baseline oracle.
