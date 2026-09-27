# WP-HK-05 — Reopen 2: recoverable extension-payload (Base64) diagnostics

Status: **CORRECTION / OWNER-DIRECTED (independent review waived by owner instruction)**

## Trigger

This reopen was triggered by WP-H1-GATE fresh independent public-client AI-agent trial 5. The trial ran on PR `#238`, candidate `3bc9f141eeee0a5c462e8156b387fbc9851ea229`, run `36248237941`, and FAILED. The record is PR comment `#5847067730`.

- The agent had to pass the ~476-character opaque `payloadBase64` returned by `unity.binding.compile` into `authoring.change.*`.
- It retyped the payload as 475 characters, miscounting a run of 71 `A` characters that encodes zero bytes. It received `world.change.invalid_request` "payloadBase64 must use canonical Base64 encoding." with an **empty** context, twice.
- Its own `unity.binding.decode` check was also opaque. The agent stopped without authoring anything.

Trial 4 (run `36246411530`) hit the same failure and recovered only by recompiling.

Owner decision (2026-09-26, after trial 5): do option 2, which is this diagnostic correction, and then option 1, one final trial with a stronger model. The owner directed this correction explicitly, and it follows the waived-review pattern of reopen 1 (`#241`).

## PREDECESSOR_CONTRACT_CHECK

This reopen corrects WP-HK-05 itself. The direct accepted dependency is `WP-HK-02A`, which is COMPLETE.

- The correction consumes only these accepted guarantees:
  - the HK-04 mutation envelope and its `put-extension` payload rule (canonical Base64 only);
  - the H0 `StructuredError` contract;
  - `PortableData`.
- None of them changes.
- WP-HK-05 reopen 1 (`#241`, merge `5d48cb55`) stays in force.
- Reopen condition checked: concrete effective evidence (trials 4 and 5) shows the HK-05 "repairable diagnostics" guarantee does not reach a public client for a corrupted extension payload. No HK-02A/HK-04 guarantee is contradicted.

## Correction

A non-canonical or missing `payloadBase64` in a `put-extension` operation keeps the same machine code, message, path and `retryable=false`. It now also returns `context.reason`:

- `length-not-multiple-of-4`, with `receivedLength` and `lengthRemainder`;
- `invalid-character`, with `firstInvalidIndex`;
- `non-canonical-padding`;
- `missing-or-not-a-string`.

A hint also explains that the payload is an opaque value, to pass exactly as returned by the tool that produced it, and to request again rather than repair by hand.

## Not changed

- The Base64 acceptance rule is unchanged: the same inputs are accepted and rejected.
- Codes, message, path, schemas, canonical state, hashing and journal semantics are unchanged.
- `unity.binding.decode` (H1-01) keeps its own diagnostic.
- Offering compiled extensions by reference, so that no client transcribes payloads at all, is a named future ergonomics decision. It was option 3 of the owner discussion and is not part of this reopen.

## Proof

- `Hk05ValidationDiagnosticsTests.NonCanonicalExtensionPayloadNamesWhyItIsNotCanonicalBase64` covers:
  - a shortened payload (the trial-5 shape), a payload with an invalid character, non-canonical padding (`QR==`) and a missing payload, each with the exact code, message, path, reason, lengths and hint;
  - the error is `PortableData`-valid and valid against the canonical `StructuredError` schema;
  - the canonical payload passes the payload check;
  - the canonical state is unchanged.
- Local `Arkus.Harness.Tests` passes 452/452. Exact-SHA hosted evidence is recorded on the PR.
- The effective public proof is the WP-H1-GATE rerun and the final fresh-agent trial.
