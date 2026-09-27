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

Second owner decision (same day): the reopen is **probed with the fresh-agent trial before it merges**, in an agile loop. Exploratory copies of this PR's commits go onto the Gate branch, and the trial runs there. Each failed probe names the next public-surface obstacle, which is corrected here and probed again. Probe records are exploratory; the Gate's own trial still runs once, on its final frozen SHA.

### Probe 1 (Gate run `36303087443`, exploratory copy `279bde65` of `7cb371d3`)

- **The `binding-invalid` correction worked.** The refusal named `subjectId=slice-root`. The agent recompiled, applied the typed `document`, and the plan advanced.
- **`projection.parent-unbound`.** The facts were present, but the hint was the generic default. The agent tried to contain its objects in the scene by creating a canonical object named after the scene. That object was unbound, and it took four attempts to recover.
- **`projection.component-target-missing` from materialize, with an empty `context` and a generic hint.** The agent had put a `renderer` on the UAL1 humanoid prefab. That prefab owns a `SkinnedMeshRenderer`, not a `MeshRenderer`. Unity's preflight tests renderer references on a synthetic probe, so the refusal surfaced only inside the staged scene, with the scene as its resource. In addition, the host ignored the Unity validation diagnostic in the reply. The agent could not tell which subject or component was wrong, and stopped at FAIL.

### Probe 2 (Gate run `36304041997`, exploratory copy of `da72a70b`)

The run stopped at turn 9, before any H1-05 surface was reached. The agent authored its bindings by transcribing `payloadBase64` again, and the transcription was not valid Base64 (321 characters, invalid character at index 92).

The kernel's HK-05 reopen-2 refusal gave the reason, and its hint said "pass it exactly as returned … request it again". It never mentioned the typed `document` form, which needs no transcription. The agent resent the same transcription and stopped at FAIL.

This refusal is the one step before `projection.binding-invalid`: a transcription that is invalid Base64 is caught here, and a valid but wrong one is caught at plan. Only the plan refusal pointed to `documentMutation`.

### Probe 3 (Gate run `36304524613`, exploratory copy of `a3a20d53`)

The agent gave up at turn 23. It authored only with transcribed `payloadBase64`, and each transcription happened to be valid Base64, so the kernel's Base64 refusal never fired. Three refusals cost it turns without saying what to repair:

1. **`unity.binding.compile` → `unity.binding.canonical-dependency-mismatch`**, with an empty context and a generic hint (WP-H1-01 surface). The agent had guessed a `containment` dependency. It misread the refusal and recompiled two bindings for the other catalogued scene.
2. **`projection.scene-out-of-scope`**, with only `subjectId` and the generic hint. The refusal named neither the binding's scene nor the managed scene.
3. **`projection.source-missing` for `quaternius.mediewal.prefab.prop-vine1`.** One flipped character of a transcribed payload (`…bWVkaWV3YWw…` instead of `…bWVkaWV2YWw…`) had changed the `logicalId`, and the payload was still valid Base64 and a valid binding. The hint was generic, so the agent re-transcribed, then stopped at a correct `binding-invalid` refusal.

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
- **Probe 1 corrections:**
  - `projection.parent-unbound` and `projection.canonical-target-unbound` have code-specific hints. For `parent-unbound`: the managed scene is not a canonical object, so either bind the container in the same scene or clear `containerId` to realize the subject at the scene root. For `canonical-target-unbound`: bind the target, or remove the `canonical-link`.
  - **Unity preflight (`unity.plan.component`, a per-subject check).** For a prefab source with a `renderer` component, it applies the renderer adapter's own target rule (`H1ComponentProjection.ValidateRendererTarget`: exactly one owned `MeshRenderer`, at most one material slot) to the source prefab, before staging. The existing codes `projection.component-target-missing`, `projection.component-target-cardinality` and `projection.component-material-slot-cardinality` are therefore reported with the subject as `canonicalResource` and its source as `logicalAsset`. A mesh `asset` source always realizes one `MeshRenderer`, so it is not affected.
  - **Materialize/observe refusals (`H1ManagedSceneExecutor.WorkerFailure`).** They carry the facts of the Unity validation diagnostic that has the reply's code: `subjectId` (when it is not the scene), `sourceLogicalId`, `invariantId`, `phase` and `validationContext`. They also carry a code-specific hint for the renderer target and material-slot codes and for `projection.active-scene-missing`; otherwise they keep the accepted default hint.

- **Probe 2 correction (kernel hint, disclosed cross-WP touch; see `PREDECESSOR_CONTRACT_CHECK.md`).** The hint of the canonical-Base64 refusal of `authoring.change.*` now says:
  - `payloadBase64` must never be retyped;
  - when the producing tool also returned the same `put-extension` as a typed document (for example a `documentMutation`), send that operation with `document`;
  - otherwise pass the payload exactly as returned, or request it again.

  The machine code, message, path and context are unchanged. So is the accepted payload semantics.

- **Probe 3 corrections:**
  - `projection.scene-out-of-scope` carries `targetSceneId` and `managedSceneId`. Its hint says to recompile with the managed scene and apply the `documentMutation`.
  - The catalogue-mapped `projection.source-missing`, `projection.reference-missing` and `projection.component-reference-missing` get a hint:
    - check the `logicalId` against the catalogue;
    - one changed character of a transcribed payload can change it;
    - recompile (compile validates catalogue references) and apply the `documentMutation`.

    Other catalogue failures keep the default hint.
  - **Compile (disclosed WP-H1-01 touch; see `PREDECESSOR_CONTRACT_CHECK.md`).** `unity.binding.canonical-dependency-mismatch` and `unity.binding.catalogue-dependency-mismatch` carry `derivedCanonicalDependencies` / `derivedCatalogueDependencies` in their context. Their hints say the assertion is optional (omit it, or set it to exactly the derived list), and that containment comes from `containerId`, not from the binding. `UnityBindingException` gains optional `Context` and `RepairHint`, which the provider passes through. Every other binding refusal keeps its empty context and its accepted hint.

## Not changed

- No capability, schema, machine code, public message, plan digest, input digest, catalogue content or fingerprint changes.
- The same inputs produce the same successful plan. On the Unity side, the only change is the renderer target preflight above. It enforces the rule that materialization already enforced, earlier, and adds no invariant ID and no new code. A plan that materializes today also passes it.
- Checkpoint, reconciliation and lifecycle pre-launch refusals are unchanged. The lifecycle refusal already points the client to `unity.projection.plan`, which now carries the facts.
- The H0 kernel's behavior is unchanged; only the repair-hint text of its Base64 refusal changes. An opaque `payloadBase64` that is valid Base64 is still admitted by authoring and caught at projection preflight, which is accepted H0/H1-08 semantics.

## Proof

- `H1ManagedScenePlanTests.Binding_failures_name_the_canonical_subject_and_the_typed_document_repair` covers:
  - a valid-Base64 non-binding payload: `projection.binding-invalid` with the unchanged message, plus `subjectId`, `bindingCode=unity.binding.invalid-payload`, `bindingPath=$.payloadBase64`, and a `documentMutation` hint;
  - `projection.source-missing` with `subjectId`, `catalogueCode` and `logicalId`;
  - `projection.parent-unbound` with `subjectId` and `parentObjectId`;
  - each public preflight error keeps its code and message and is a `PortableData`-valid `StructuredError` that validates against `CanonicalContractSchemas.StructuredError()`;
  - the default hint remains where there is no code-specific hint;
  - `projection.parent-unbound` and `projection.canonical-target-unbound` carry their code-specific hints and facts.
- `H1ManagedScenePlanTests.Editor_refusals_carry_the_unity_diagnostic_subject_and_a_code_specific_repair` deserializes the Editor reply shape (`H1SceneProjection.ValidationReply`) and checks three cases:
  - a `component-target-missing` preflight diagnostic yields `subjectId`, `sourceLogicalId`, `invariantId`, `phase`, `validationContext` and the renderer hint;
  - a scene-scoped `active-scene-missing` yields no `subjectId` and gets its own hint;
  - a reply without a matching diagnostic keeps the empty context and the default hint.

  All three are valid `StructuredError`s.
- `Hk05ValidationDiagnosticsTests.NonCanonicalExtensionPayloadNamesWhyItIsNotCanonicalBase64` still pins the refusal's code, message, path, context and "exactly as returned". It now also requires the typed-document direction (`documentMutation`, and "send that operation with document instead of payloadBase64").
- Probe 3:
  - `H1ManagedScenePlanTests` covers `scene-out-of-scope`, with its facts and hint, and the catalogue-missing hint. `source-wrong-type` keeps the default hint.
  - `H1UnityAuthoringProducerTests.CallerMaintainedDependencyTruthFailsClosedOnOmissionContradictionAndDuplicates` covers both mismatch refusals: they carry the derived dependencies and hints and are valid `StructuredError`s.
- The existing `H1ManagedScenePlanTests` and `H1CatalogueTests` still pin the codes.
- Exact-SHA hosted evidence is recorded on the PR: Arkus Main Safety, and the H1-05 route `scripts/h1-05-verify-exact-sha.sh`.
- The effective public proof is WP-H1-GATE: its deterministic 17-stage scenario on reference and MCP, and trial 8 on the Gate's final frozen SHA.
