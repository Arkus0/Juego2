# WP-H1-05 — Reopen 1: actionable projection preflight diagnostics

Status: **CORRECTION / OWNER-DIRECTED REOPEN**

## Trigger

This reopen comes from WP-H1-GATE fresh independent public-client AI-agent trial 7, which FAILED.

- Trial: PR `#238`, candidate `fb0933f27cd0141ed4320d022bd683bdd432d846`, run `36281413613`, record `#5851178404`.
- The deterministic Gate was GREEN on the same SHA (run `36280239062`).
- The model, route, brief, protocol, relay and verifier are the same as trial 6, which passed on a byte-identical product surface.

What happened in trial 7:

1. The agent authored its bindings by transcribing `payloadBase64`. It did not use the `documentMutation` that `unity.binding.compile` returns.
2. The transcriptions were valid Base64, so the kernel accepted them.
3. `unity.projection.plan` then refused with `projection.binding-invalid` / `unity.binding.invalid-payload at $.payloadBase64`, with an **empty `context`**.
4. The hint was the generic "Repair the canonical binding or catalogue input before materialization."

A client that has only the public contract could not tell which of its bindings was broken, or how to author one without transcription. It removed and re-added bindings blindly and stopped at FAIL.

Owner decision (2026-09-27, in the WP-H1-GATE repair Worker session): reopen WP-H1-05, then run trial 8 on the Gate's final frozen SHA.

## Accepted guarantee proved inapplicable

WP-H1-05 owns "versioned public projection plan/materialize/observe capabilities". The plan's preflight refusal is accepted as a structured diagnostic. It holds for a client with implementation knowledge, but it is inapplicable to a fresh public client:

- `H1ManagedScenePlan.Build` knew the failing subject, the codec's machine code and path, and, for catalogue failures, the catalogue code and logical ID;
- the public `StructuredError` exposed none of these.

The same gap class was corrected for the catalogue in WP-H1-04 reopen 1 (`#239`).

## Correction

- **`H1ProjectionException`.** It carries optional structured `Context` and an optional code-specific `RepairHint`. The existing two-argument constructor is unchanged.
- **`H1ManagedScenePlan.Build`.**
  - Every per-binding failure now names its canonical `subjectId`.
  - `projection.binding-invalid` adds `bindingCode` and `bindingPath` from the Unity binding codec. Its hint says to recompile with `unity.binding.compile` and apply the returned `documentMutation` (a typed-document `put-extension`) instead of copying `payloadBase64`, then re-plan.
  - Catalogue-mapped failures (`projection.source-*`, `projection.reference-*`, `projection.component-reference-*`) add `catalogueCode` and the catalogue's own context, for example `logicalId` and `kind`.
  - `projection.parent-unbound` adds `parentObjectId`, and `projection.canonical-target-unbound` adds `targetObjectId`.
- **Plan handler.** `H1ManagedScenePlanHandler` returns these facts through `H1ProjectionContract.PreflightError`. It uses the code-specific hint when there is one, and otherwise the accepted default hint.

## Not changed

- No capability, schema, machine code, public message, plan digest, input digest, catalogue content, fingerprint or Unity/Editor code changes.
- The same inputs produce the same successful plan.
- Checkpoint, reconciliation and lifecycle pre-launch refusals are unchanged. The lifecycle refusal already points the client to `unity.projection.plan`, which now carries the facts.
- The H0 kernel is unchanged. An opaque `payloadBase64` that is valid Base64 is still admitted by authoring and caught at projection preflight, which is accepted H0/H1-08 semantics.

## Proof

- `H1ManagedScenePlanTests.Binding_failures_name_the_canonical_subject_and_the_typed_document_repair` covers:
  - a valid-Base64 non-binding payload: `projection.binding-invalid` with the unchanged message, plus `subjectId`, `bindingCode=unity.binding.invalid-payload`, `bindingPath=$.payloadBase64`, and a `documentMutation` hint;
  - `projection.source-missing` with `subjectId`, `catalogueCode` and `logicalId`;
  - `projection.parent-unbound` with `subjectId` and `parentObjectId`;
  - each public preflight error keeps its code and message and is a `PortableData`-valid `StructuredError` that validates against `CanonicalContractSchemas.StructuredError()`;
  - the default hint remains where there is no code-specific hint.
- The existing `H1ManagedScenePlanTests` and `H1CatalogueTests` are unchanged and still pin the codes.
- Exact-SHA hosted evidence is recorded on the PR: Arkus Main Safety, and the H1-05 route `scripts/h1-05-verify-exact-sha.sh`.
- The effective public proof is WP-H1-GATE: its deterministic 17-stage scenario on reference and MCP, and trial 8 on the Gate's final frozen SHA.
