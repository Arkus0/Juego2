# WP-H1-UNITY-CI evidence

Pilot state: FIRST_GITHUB_UNITY_RUN_COMPLETED / REVIEWER_REPAIR_RERUN_REQUIRED

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

Reviewer #5296184999 found that the implementation was technically bounded but the workpack contract still classified every tracked mutation as isolation FAIL. The repair makes the two-path, exact-delta, restore-to-target exception explicit and sets GameCI `coverageEnabled: false` for parity with H1-02. A fresh GREEN run of the repaired candidate is required before independent acceptance.

The authoritative execution evidence is the GitHub Actions artifact emitted by `H1 Unity CI Pilot`; this README records the state and interpretation but does not replace the artifact or exact-SHA receipt.

## One-time setup

The pilot workflow is fail-closed until GitHub Actions has these repository secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For Unity Personal, `UNITY_LICENSE` is the contents of the `.ulf` file produced by activating the free Personal license in Unity Hub. The `.ulf` and credentials must never be committed.
