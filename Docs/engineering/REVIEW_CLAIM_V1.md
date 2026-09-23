# Reviewer claim lease v1

Status: prospective Batch C process contract.
Authority: Reviewer concurrency only; never semantic review authority.

## Purpose

Prevent two independent Reviewer sessions from spending a full review on the same PR + exact candidate SHA while preserving recovery if a Reviewer session disappears.

## Claim request

For candidates that enter Reviewer after Batch C is accepted and merged, a fresh independent Reviewer posts exactly:

```text
ARKUS_AUTOMATION_V2
State: REVIEW_CLAIM_REQUEST
Target SHA: <exact 40-hex candidate SHA>
```

The Reviewer does not choose a Claim ID. `Arkus Reviewer Claim` serializes requests per PR and derives the Claim ID from the request-comment ID.

Only a `github-actions[bot]` marker is authoritative:

- `REVIEW_CLAIMED`: this session may continue only when `Claim ID` equals its own request-comment ID, the target SHA still equals live PR HEAD, and the lease is unexpired.
- `REVIEW_CLAIM_DENIED`: another unexpired lease already owns the same exact PR+SHA; stop without performing a duplicate semantic review.

Human-authored or agent-authored lookalike `REVIEW_CLAIMED` markers are ignored by the policy oracle.

## Lease and recovery

A granted lease lasts 90 minutes. Expired leases are ignored automatically by later requests. A claim for another SHA never blocks the current candidate. Therefore an abandoned Reviewer session cannot deadlock review and no background cleanup daemon is required.

After publishing a verdict, or when abandoning a live claimed review, the session should post:

```text
ARKUS_AUTOMATION_V2
State: REVIEW_CLAIM_RELEASE_REQUEST
Claim ID: <granted Claim ID>
Target SHA: <reviewed exact SHA>
```

The workflow emits `REVIEW_CLAIM_RELEASED` only when that Claim ID is still the active exact-SHA lease. A stale or mismatched release fails closed.

## Boundaries

- Claim ownership proves only concurrency ownership. It does not establish independence, PASS, FAIL, evidence validity, proof completeness or any workpack guarantee.
- A denied or expired claim is not a Reviewer verdict and consumes no semantic review round.
- The lease does not mutate candidate bytes.
- Reviewer independence remains a session/process obligation under `WORKER_REVIEW_PROTOCOL.md`.
- Batch C itself is reviewed under the pre-C protocol. Requiring the claim workflow to review the candidate that introduces that workflow would be a circular adoption dependency.
- Adoption is prospective: the first candidate entering Reviewer after Batch C merge must use this protocol.
