# WP-HK-08A implementation observation

Implementation/test SHA: `7102330e9a3030d8f4f333d2715a8633312eeb7b`
GitHub Actions run: `35498188203`
Observation artifact: `10600817518`

Canonical command:

`bash scripts/hk08a-observe-exact-sha.sh 7102330e9a3030d8f4f333d2715a8633312eeb7b`

Observed result:

- exact checkout and clean pre-state: GREEN;
- locked restore: GREEN;
- Release build: GREEN, 0 warnings / 0 errors;
- focused `Hk08A*`: 13 passed / 0 failed;
- full regression: 173 passed / 0 failed;
- clean post-state: GREEN;
- receipt result: GREEN.

The observation is pre-freeze implementation evidence. The later documentation/evidence reconciliation SHA must pass `scripts/hk08a-verify-exact-sha.sh` unchanged before it can become the frozen candidate.
