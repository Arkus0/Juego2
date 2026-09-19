# validate-workpack

Independently review one frozen Juego2 candidate. Do not edit implementation.

## Preconditions

- PR is Ready and `Worker state=FROZEN_FOR_REVIEW`.
- `Branch frozen=YES`.
- Current HEAD equals exact `Frozen candidate SHA`.
- This reviewer/session did not act as Worker or direct implementation of this candidate. Otherwise STOP: `HUMAN_ACTION_REQUIRED: NEED_FRESH_REVIEWER`.

## Review

1. Reconstruct current contract and dependencies from repository sources.
2. Read the current WP's direct accepted dependency contract(s), accepted PASS/completion evidence and relevant proof/invariant material independently of the Worker summary.
3. Inspect the Worker's `PREDECESSOR_CONTRACT_CHECK`; verify it is factually consistent, but do not trust it as an authority.
4. Build the inherited/current ownership split: which guarantees are already binding upstream, which guarantees this WP actually owns, and what concrete evidence would be required to reopen an upstream claim.
5. Inspect baseline→Frozen candidate complete diff.
6. Inspect exact-SHA validation/evidence but do not trust Worker conclusions.
7. Reproduce material tests/checks independently where possible.
8. Verify Allowed/Forbidden scope.
9. For foundational WPs, challenge completeness and false-green paths **inside the WP claim and declared trust boundary**. Search independently for omissions not highlighted by the Worker, but do not invent out-of-boundary attacks merely to force a finding.
10. Before treating an apparent omission as a blocker, check whether a predecessor already owns and has accepted that guarantee. Do not require duplicate proof unless concrete evidence shows the inherited guarantee is inapplicable or false.
11. Check negative controls are causal rather than incidental compile failures.
12. Check exact-SHA binding of evidence.
13. Search for hidden dual behavior, duplicated truths, unowned in-claim surfaces and effective/runtime bypasses.
14. Respect the proof budget: trusted-base subversion, duplicate re-proof of accepted predecessor guarantees, or unsupported non-canonical paths are residual risks/overdefense unless the WP explicitly owns them or contradictory evidence reopens the causal boundary.

## Verdict

Emit exactly one of `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION`, naming the reviewed SHA.

FAIL includes criterion, observed evidence, expected behavior and minimal correction boundary. If the finding appears predecessor-owned, the FAIL must also state why the accepted predecessor guarantee does not cover the case or what evidence proves that guarantee false. Do not repair the candidate.

A valid review is not required to discover a novel defect. PASS is appropriate when serious independent falsification finds no blocking in-claim defect.
