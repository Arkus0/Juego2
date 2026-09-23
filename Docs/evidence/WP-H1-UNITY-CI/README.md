# WP-H1-UNITY-CI evidence

Pilot state: AWAITING_FIRST_GITHUB_UNITY_RUN

Frozen comparison oracle:

- accepted H1-02 candidate: `d86a08e644f542e9515f5e54fd4061f61e251c70`
- accepted effective Unity: `6000.3.24f1 (4e7b9b5b6244)`
- accepted EditMode result: `5/5` passed
- first cloud run: clean import, no `Library` cache

The authoritative first-run evidence is the GitHub Actions artifact emitted by `H1 Unity CI Pilot`; this file intentionally does not predict the result.

## One-time setup

The pilot workflow is fail-closed until GitHub Actions has these repository secrets:

- `UNITY_LICENSE`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

For Unity Personal, `UNITY_LICENSE` is the contents of the `.ulf` file produced by activating the free Personal license in Unity Hub. The `.ulf` and credentials must never be committed.

Once those secrets exist, re-run the failed `H1 Unity CI Pilot` job on the same pilot PR candidate. No code mutation is required merely to retry credential-backed execution.
