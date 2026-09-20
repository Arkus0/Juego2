# WP-HK-08A implementation observation

Implementation/test SHA: `300c4b65bcbe1b547837dffc76e241c9500a9e0c`
GitHub Actions run: `35499408516`
Observation artifact: `10601643061`

Canonical command:

`bash scripts/hk08a-verify-exact-sha.sh 300c4b65bcbe1b547837dffc76e241c9500a9e0c`

Observed result after the Reviewer FAIL repair:

- exact checkout and clean pre-state: GREEN;
- locked restore: GREEN;
- Release build: GREEN, 0 warnings / 0 errors;
- focused `Hk08A*`: 13 passed / 0 failed;
- full regression: 173 passed / 0 failed;
- cross-transport conformance: GREEN;
- foundational proof / evidence reconciliation / Worker pre-review gates: GREEN;
- clean post-state: GREEN;
- receipt result: GREEN.

The repair restores `authoring.journal.read@1.0` to the accepted HK06A contract (`{}` -> complete journal) and exposes HK08A pagination only through explicit `authoring.journal.read@2.0` negotiation. The first repair validation (`21b13bf571c7295d2907d972f2eaa95e9a4c5031`, Actions `35499256364`) correctly turned red only because two HK01 independent-route tests still contained hard-coded pre-v2 inventory counts. Those count literals were replaced by definition/route equality plus explicit assertions for both journal versions; the resulting SHA above is fully GREEN.

This is pre-freeze implementation evidence. The later evidence-reconciliation SHA must pass `scripts/hk08a-verify-exact-sha.sh` unchanged before it can become the frozen candidate.
