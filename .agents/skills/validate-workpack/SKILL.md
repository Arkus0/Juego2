# validate-workpack

Independently review one frozen Juego2 candidate. Do not edit implementation.

Juego2 / Arkus Harness is a game-development and software-verification project. Review is limited to repository-owned game-authoring code, fixtures, tests, CI and documentation. Legacy terms such as `self-attack`, `attack fixture`, `bypass` or `adversarial review` refer only to internal negative/conformance testing; use the neutral terminology defined in `AGENTS.md` for new work.

## Context bootstrap

Start with `Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` and the `reviewer` profile in `Docs/engineering/context-bootstrap-profiles.json`. The profile is a minimum starting pack, never a review ceiling. Full `Docs/ROADMAP.md` is conditional for an exact-WP review unless cross-track/order/gate meaning is not closed by exact/direct sources. `FOUNDATIONAL_PROOF_STANDARD.md` remains mandatory whenever the exact claim binds it. After CTX-02 adoption, `Docs/engineering/CONTEXT_CAPSULE_V1.md` governs validated accepted-contract capsules that may navigate an accepted predecessor boundary, but a capsule is never review proof and never limits independent search. A stale/missing/lossy capsule, stale compact index or Worker summary can only trigger escalation; none is semantic authority.

After CTX-03 adoption, mechanical handoff/envelope results are classified under `Docs/engineering/CONTEXT_ENVELOPE_V1.md`. This classification exists to keep pure protocol/infra defects out of semantic Reviewer rounds; it never narrows the Reviewer's causal search.

## Preconditions

- PR is Ready and `Worker state=FROZEN_FOR_REVIEW`.
- `Branch frozen=YES`.
- Current HEAD equals exact `Frozen candidate SHA`.
- Worker pre-review is CLEAN for the exact final bytes.
- After CTX-03 adoption, a durable Automation V2 `REVIEW_READY_CLOSED` marker targets that exact SHA. For the one-time CTX-03 adoption candidate itself, where the new issue-comment workflow cannot yet exist on default `main`, require the equivalent already-planned invariant: durable `REVIEW_READY` for the frozen SHA plus a post-marker live HEAD confirmation for that same SHA.
- This reviewer/session did not act as Worker or direct implementation of this candidate. Otherwise STOP: `HUMAN_ACTION_REQUIRED: NEED_FRESH_REVIEWER`.

A red mechanical check is not automatically a Reviewer FAIL. Before semantic review begins, classify it as `FAIL | REVIEW_BLOCKED | NOT_APPLICABLE | INFRA_ERROR` using the registered verifier contract. `REVIEW_BLOCKED` and `INFRA_ERROR` stop review without consuming a semantic verdict. An unregistered verifier cannot by itself establish WP failure. A registered mechanical `FAIL` means the Worker should repair before independent review; it is not an independent Reviewer verdict.

## Review

1. Reconstruct current contract and dependencies from repository sources and live state.
2. Before CTX-02 adoption, or when a direct dependency has no valid capsule, independently read its accepted contract/PASS/completion evidence and relevant proof/invariant material. After CTX-02 adoption, a validated capsule + independently confirmed accepted identity may start navigation; open the authoritative source whenever the verdict materially depends on an inherited guarantee, a capsule is lossy/suspect, a source is marked non-compressible, or concrete contradiction could reopen the predecessor.
3. Inspect the Worker's `PREDECESSOR_CONTRACT_CHECK`; verify it is factually consistent, but do not trust it or a Worker-chosen capsule as an authority.
4. Build the inherited/current ownership split: which guarantees are already binding upstream, which guarantees this WP actually owns, and what concrete evidence would be required to reopen an upstream claim.
5. Inspect baseline→Frozen candidate complete diff.
6. Inspect exact-SHA validation/evidence but do not trust Worker conclusions. Mechanical PASS only proves its registered deterministic condition.
7. Reproduce material tests/checks independently where possible.
8. Verify Allowed/Forbidden scope.
9. For foundational WPs, challenge completeness and false-green paths **inside the WP claim and declared trust boundary**. Search independently for omissions not highlighted by the Worker, but do not invent out-of-boundary pathological scenarios merely to force a finding.
10. Before treating an apparent omission as a blocker, check whether a predecessor already owns and has accepted that guarantee. Do not require duplicate proof unless concrete evidence shows the inherited guarantee is inapplicable or false. Capsule acceptance alone never defeats concrete contradictory evidence.
11. Check negative controls are causal rather than incidental compile failures.
12. Check exact-SHA binding of evidence.
13. Search for hidden dual behavior, duplicated truths, unowned in-claim surfaces and undeclared/alternate effective runtime paths.
14. Respect the proof budget: trusted-base pathology scenarios, duplicate re-proof of accepted predecessor guarantees, or unsupported non-canonical paths are residual risks/overdefense unless the WP explicitly owns them or contradictory evidence reopens the causal boundary.

## Verdict

Emit exactly one of `PASS | FAIL | BLOCKED | READY_FOR_LOCAL_VALIDATION`, naming the reviewed SHA.

FAIL includes criterion, observed evidence, expected behavior and minimal correction boundary. If the finding appears predecessor-owned, the FAIL must also state why the accepted predecessor guarantee does not cover the case or what evidence proves that guarantee false. Do not repair the candidate.

Do not use `FAIL` merely to report a mechanical `REVIEW_BLOCKED`, `NOT_APPLICABLE` or `INFRA_ERROR` condition that should have been resolved before semantic review. If such a condition is discovered at review start, stop with `BLOCKED` and name the exact mechanical condition; no semantic verdict is consumed.

A valid review is not required to discover a novel defect. PASS is appropriate when serious independent challenge finds no blocking in-claim defect.
