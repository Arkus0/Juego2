# validate-workpack

Independently review one frozen Juego2 candidate. Do not edit implementation.

## Preconditions

- PR is Ready and `Worker state=FROZEN_FOR_REVIEW`.
- `Branch frozen=YES`.
- Current HEAD equals exact `Frozen candidate SHA`.
- This reviewer/session did not act as Worker or direct implementation of this candidate. Otherwise STOP: `HUMAN_ACTION_REQUIRED: NEED_FRESH_REVIEWER`.

## Review

1. Reconstruct current contract and dependencies from repository sources.
2. Inspect baseline→Frozen candidate complete diff.
3. Inspect exact-SHA validation/evidence but do not trust Worker conclusions.
4. Reproduce material tests/checks independently where possible.
5. Verify Allowed/Forbidden scope.
6. For foundational WPs, challenge completeness and false-green paths **inside the WP claim and declared trust boundary**. Search independently for omissions not highlighted by the Worker, but do not invent out-of-boundary attacks merely to force a finding.
7. Check negative controls are causal rather than incidental compile failures.
8. Check exact-SHA binding of evidence.
9. Search for hidden dual behavior, duplicated truths, unowned in-claim surfaces and effective/runtime bypasses.
10. Respect the proof budget: trusted-base subversion or unsupported non-canonical paths are residual risks unless the WP explicitly owns them.

## Verdict

Emit exactly one of `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION`, naming the reviewed SHA.

FAIL includes criterion, observed evidence, expected behavior and minimal correction boundary. Do not repair the candidate.

A valid review is not required to discover a novel defect. PASS is appropriate when serious independent falsification finds no blocking in-claim defect.
