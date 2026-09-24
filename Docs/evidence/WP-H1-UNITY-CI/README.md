# WP-H1-UNITY-CI evidence

Pilot state: COMPLETE / ACCEPTED / DOCSYNC_COMPLETE

Frozen comparison oracle:

- accepted H1-02 candidate: `d86a08e644f542e9515f5e54fd4061f61e251c70`
- accepted effective Unity: `6000.3.24f1 (4e7b9b5b6244)`
- accepted EditMode result: `5/5` passed
- cloud import baseline: clean import, no `Library` cache

Observed GitHub-hosted evidence before the reviewer repair:

- H1 Unity CI Pilot run #4 / `35912394723`: GREEN
- effective Unity: `6000.3.24f1 (4e7b9b5b6244)`
- effective EditMode result: `5/5` passed
- deterministic GameCI drift was limited to `Unity/ArkusUnity/Packages/manifest.json` and `Unity/ArkusUnity/Packages/packages-lock.json`, reconciled against the known package injection, then restored
- final tracked tree after restoration: clean
- coverage was enabled by the action default, which is why reviewer #5296184999 required an explicit disabled-coverage repair

Reviewer repair history:

- the workpack makes the two-path, exact-delta, restore-to-target isolation exception explicit;
- repair run #7 / `35936312446` passed `coverageEnabled: false` to the pinned `unity-test-runner` action, but the wrapper translated it to `--no-coverageEnabled`; GameCI CLI `v0.1.69` rejected that argument before Unity launched, so that RED is classified as wrapper/CLI incompatibility rather than product evidence;
- the accepted repair invokes the exact GameCI CLI `v0.1.69` Linux x64 release binary directly, verifies SHA-256 `d847fe7b0131a00c521c51b0e6987b356301bb7121b69d9266c9414e78832329`, and supplies the supported `--coverageEnabled false` boolean form.

## Accepted run

- exact candidate under review: `b49b081a92b088d7b0fd9adce4bd5f26a3b6c1bf`
- H1 Unity CI Pilot run #11 / `35936805407`: GREEN
- exact oracle checkout: `d86a08e644f542e9515f5e54fd4061f61e251c70`
- effective Unity: `6000.3.24f1 (4e7b9b5b6244)`
- runtime coverage: disabled (`coverageEnabled=false`)
- EditMode result: `5/5 Passed`, zero failures
- isolation reconciliation: exact known two-path GameCI package injection, then restoration
- final tracked tree: identical to target SHA
- artifact: `10783780236`, bound to run `35936805407` and candidate HEAD
- Arkus Main Safety run `35936805408`: GREEN
- independent PASS review: `#5299167558`
- PR: `#166`
- implementation merge: `6898250be985ab5d805bbdb129e30c9c6f1f4cdf`

The authoritative execution evidence is the GitHub Actions artifact emitted by `H1 Unity CI Pilot`; this README records the accepted state and interpretation but does not replace the artifact or exact-SHA receipt.

Post-PASS reconciliation: `Docs/evidence/WP-H1-UNITY-CI/DOCSYNC.md`.

## One-time setup

The accepted GitHub-hosted Unity execution path is fail-closed until GitHub Actions has these repository secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For Unity Personal, `UNITY_LICENSE` is the contents of the `.ulf` file produced by activating the free Personal license in Unity Hub. The `.ulf` and credentials must never be committed.