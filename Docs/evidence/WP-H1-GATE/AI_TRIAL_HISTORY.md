# WP-H1-GATE — fresh AI-agent trial history and predecessor reopen routing

Gate state: **trial 4 pending on the final frozen SHA**. Trial 1 FAIL led to the WP-H1-04 reopen (merged). Trial 2 FAIL led to the WP-HK-05 reopen (merged). Trial 3 FAIL led to a Gate-owned relay-fidelity correction (see below).

## Trial 1 — FAIL

| Field | Value |
| --- | --- |
| Candidate SHA | `c2dbd318843888af86543e3515a78af07a7ef4a7` |
| Deterministic Gate on the same SHA | `H1-GATE Unity Parity Validation` run `36238102363` **GREEN**: all 17 stages on reference + MCP, verifier and 30 negative controls GREEN. |
| Trial run | `H1-GATE Fresh AI-Agent Trial` run `36238111911`, artifact `h1-gate-ai-trial-c2dbd318…` (transcript + record) |
| Model / route | `openai/gpt-5.6-luna-20260709` via OpenRouter (owner-selected hosted route) |
| Model turns | 7 (the agent stopped by itself; the limit is 90) |
| Agent verdict | `FAIL` |
| Verifier | RED: `TRIAL.required-public-flow-not-exercised` (materialize, apply), `TRIAL.no-rebuild`, `TRIAL.no-closing-evidence`, `TRIAL.verdict-not-pass` |
| Hidden/private calls | none (agent report `NONE`; every call was issued by the model and names a discovered tool) |

The agent recorded these ambiguities in its report, verbatim:

1. "Catalogue queries initially rejected pageSize 100 with catalogue.page-bound."
2. "After correcting pageSize to 64 or 1, catalogue queries consistently returned catalogue.stale-snapshot, including expectedSnapshotToken values 0 and 1, without exposing a usable snapshot token."
3. "The required scene logical identifier arkus.h1-05.scene.potes was not present in the effective catalogue."
4. "No catalogue identifiers could be discovered, so canonical objects and Unity bindings could not be authored."
5. "Observation and checkpoint capture were blocked because no active managed-scene projection existed."

## Classification

The deterministic scenario passed because it was written with implementation knowledge. It omits `expectedSnapshotToken` on the first page, knows the 1..64 bound, and takes the managed scene ID from the `unity.projection.plan` request enum. A fresh public client has none of that knowledge. That gap is exactly what the trial measures. The acceptance criterion "fresh independent AI-agent trial succeeds using public discovery/schemas" is therefore unmet, and the causes are in the discoverable public contract, which the Gate may not change:

| # | Public-surface gap (effective evidence) | Causal owner | Why it is not Gate-owned |
| --- | --- | --- | --- |
| R1 | `unity.host.catalogue.query` publishes `pageSize` as a bare `integer`, but the model enforces `1..64` (`H1CatalogueModel.MaximumPageSize`, `catalogue.page-bound`). The schema does not document the semantics of `expectedSnapshotToken`, which must be the `snapshotToken` of a previous page. | **WP-H1-04** (catalogue public schema) | It changes a public capability schema. |
| R2 | `catalogue.stale-snapshot` returns an empty error context with no current `snapshotToken`. All catalogue errors share one generic `repairHint`, "Repair the reviewed mapping, source input or catalogue reference before materialization." (`H1CatalogueCapability` catch block). For a stale or guessed token or an out-of-range page, that hint misdirects the client towards a mapping repair. | **WP-H1-04** (catalogue structured diagnostics) | It changes public diagnostic semantics. |
| R3 | `unity.binding.compile` requires `binding.targetSceneId` as a logical reference in the catalogue namespace. The only admitted target, the fixed managed scene `arkus.h1-05.scene.potes`, is absent from the effective catalogue (`catalogue.resolve` → `catalogue.missing-reference`). It is discoverable only through the `unity.projection.plan` request enum. | **WP-H1-04 / WP-H1-05** discoverability (the reopen owner triages R3 together with R1–R2) | It changes a public catalogue/projection contract. |
| R4 (likely next blocker, not yet reached by the trial) | A materialize over an invalid canonical binding returns `unity.lifecycle.corrupt-result` with the hint "Repair the host/worker contract mismatch before retrying." (content-shape finding G2). The actionable code (`projection.source-missing` / `projection.reference-missing`) is only on `unity.projection.plan`. Brief step 5 would hit this. | **WP-H1-05 / WP-H1-03A** encoder-rejection mapping (for the reopen owner to triage) | It changes public diagnostic semantics. |

Model persistence (7 turns) is a contributing factor, but it does not explain the failure away. R2 actively misdirects any client that passes a token, and R3 requires knowledge that is not in the catalogue.

## Routing

- The owner decided to reopen WP-H1-04 before retrying: a fresh repair Worker for the causal owner, then that owner's own independent review.
- The Gate repairs nothing. Once the reopened predecessor is accepted and merged, the Gate merges the new base, re-runs its deterministic validation on the new exact SHA, and repeats the single hosted trial on the final frozen SHA.
- Trial 1 stays in the record. Any later PASS is reported together with this FAIL.

## WP-H1-04 reopen 1 — resolution

| Field | Value |
| --- | --- |
| PR | `#239` |
| Frozen candidate | `ce3ba74efae28692d886a5e11366c7849695e9f6` |
| Merge | `4ab82fb1c0a0d8654aaa44ebc66452a3507ef347` |
| Review | Owner waived independent review for this correction only. The waiver record is `#5846142155` and the Worker pre-review is `#5846087974`. |
| Exact-SHA evidence | Candidate Validation `36240609178`, Main Safety `36240509832`, and H1-07 `36240509836`, H1-09 `36240509853`, H1-10 `36240509838`, H1-11 `36240509869`, all GREEN. |
| Record | `Docs/evidence/WP-H1-04/REOPEN_1_PUBLIC_DIAGNOSTICS.md` |

How each routed gap was resolved (codes, capabilities, schemas and catalogue content are unchanged):

- **R1**: `catalogue.page-bound` returns `context.minimumPageSize`/`maximumPageSize` (or the offset bounds) and a paging hint. Schema numeric bounds are not expressible in H0 `SchemaNode`, and this remains a named future decision.
- **R2**: `catalogue.stale-snapshot` returns `context.currentSnapshotToken`, `fingerprint` and `restartOffset`, with a hint on the token protocol. Every catalogue error now has its own hint, and the misdirecting mapping hint is kept only for mapping faults.
- **R3**: `catalogue.missing-reference` on the fixed managed scene names its role (`managedProjectionTarget`, `targetFor`: `binding.targetSceneId`, `sceneLogicalId`).
- **R4**: a pre-launch encoder refusal keeps its projection code (`projection.source-missing` / `projection.reference-missing`) instead of `unity.lifecycle.corrupt-result`. Gate S12 now requires those exact codes for materialize, and `projection.source-missing` for a plan-relative observe, on both transports.

The trial brief, protocol, model and route are unchanged from trial 1, so the retry measures the repaired public surface and not a re-tuned prompt.

## Trial 2 — FAIL

| Field | Value |
| --- | --- |
| Candidate SHA | `facfe0ebd3678f18ce35205c24d28714b6ae15a8`, on base `4ab82fb1` with WP-H1-04 reopen 1 |
| Deterministic Gate on the same SHA | `H1-GATE Unity Parity Validation` run `36241021111` **GREEN**: 17 stages on reference + MCP; S12 refusals carry `projection.source-missing` / `projection.reference-missing`; verifier, 20 static and 11 effective negative controls GREEN |
| Trial run | `H1-GATE Fresh AI-Agent Trial` run `36242209280` (transcript sha256 `9de7f9b9…`) |
| Model / route / brief | unchanged from trial 1 (`openai/gpt-5.6-luna-20260709` via OpenRouter; same brief digest) |
| Model turns | 14 (the agent stopped by itself) |
| Agent verdict | `FAIL` |
| Hidden/private calls | none |

The reopened H1-04 diagnostics worked as intended:

- The agent recovered from `catalogue.page-bound` using `context.maximumPageSize`.
- It recovered from `catalogue.stale-snapshot` using `context.currentSnapshotToken`, then paged every catalogue kind.
- It compiled bindings against the fixed managed scene without resolving it in the catalogue.

The new blocker is in canonical authoring, which the H0 kernel owns. The agent authored one `put-object` that also carried the extension fields `owner`, `schemaVersion`, `subjectId`, `dependencies` and `payloadBase64`. Six plan/validate attempts returned `world.change.invalid_request` "put-object contains fields outside its declared grammar." with an empty `context`. Each time, the hint pointed back to `system.describe`. The agent reported, verbatim, that "The public operation schema reported all put-object fields as optional", and that "The detailed typed mutation grammar needed to author objects was not available in a usable form through the returned system.describe response."

| # | Public-surface gap (effective evidence) | Causal owner | Why it is not Gate-owned |
| --- | --- | --- | --- |
| R5 | The published operation item is one flattened union of the four operation kinds: only `kind` is required, and there is no per-kind grammar, because `SchemaNode` has no `oneOf`. The per-kind grammar rejection in `WorldMutationService.ParseOperation` did not name the kind's allowed, required or offending fields. | **WP-HK-05** (validation + repairable diagnostics) | It changes H0 public diagnostic semantics. |

Relay note (Gate-owned, not causal): the relay shows the model at most 14,000 characters of each tool result, while `system.describe` is about 141,000 characters. Even the full `system.describe` carries only the same flattened schema. The truncation therefore hid no per-kind grammar, and the protocol stays unchanged.

## WP-HK-05 reopen 1

The owner instructed the Worker to do whatever is needed to finish the Gate, and waived independent review for the predecessor correction. This second causal reopen was handled the same way, and its record says so explicitly.

- PR: `#241`, frozen candidate `93cdc3b5a6f38190a4ae0b8ba13894a004babe78`, merge `5d48cb55ac2815be417dc2a910734192b6d1543c`. Record: `Docs/evidence/WP-HK-05/REOPEN_1_GRAMMAR_DIAGNOSTICS.md`.
- Review: waived by owner instruction (record `#5846410743`). Worker pre-review: `#5846390197`.
- Exact-SHA evidence, all GREEN: Candidate Validation `36243056710`, Main Safety `36243025358`, H1-07 `36243025401`.
- The same PR restored the HK-05 exact-SHA route in CI, which was already RED because it required a fully clean tree.
- A per-kind grammar violation now returns `context.operationKind`, `allowedFields`, `requiredFields` and `unexpectedFields`, plus a hint that object and extension data are separate `put-object` and `put-extension` operations. The grammar, code, message, path and schemas are unchanged.
- The Gate repairs nothing itself. After the merge, it re-runs its deterministic validation on the new exact SHA and repeats the trial once on the final frozen SHA, with the same brief, protocol, model and route.

## Trial 3 — FAIL

| Field | Value |
| --- | --- |
| Candidate SHA | `84e6eda62f71870beaffc79ed019bcb94a0ceb37`, on base `5d48cb55` with WP-H1-04 and WP-HK-05 reopen 1 |
| Deterministic Gate on the same SHA | run `36243305078` **GREEN**: 17 stages on reference + MCP; the S06 grammar probe is repairable on both transports; 20 static and 12 effective negative controls GREEN |
| Trial run | run `36244597630` |
| Model / route / brief | unchanged (`openai/gpt-5.6-luna-20260709` via OpenRouter; same brief digest) |
| Model turns | 11 (the agent stopped by itself) |
| Agent verdict | `FAIL` |
| Hidden/private calls | none |

Both predecessor corrections reached the agent:

- It recovered the catalogue through `maximumPageSize` and `currentSnapshotToken`.
- It fixed a binding dependency assertion by itself.
- Every operation-grammar rejection now named `allowedFields`, `requiredFields` and `unexpectedFields`, for example `put-object` unexpected `[dependencies, owner, payloadBase64, schemaVersion, subjectId]`.

The agent still re-sent the same unexpected properties with empty or zero values three times, and then stopped. It reported, verbatim: "The public authoring operation schema exposes all operation fields as required, but the server rejects those fields for put-object and put-extension according to their narrower grammars."

## Classification of trial 3: Gate-owned relay fidelity

The host does **not** publish those fields as required: the operation item requires only `kind`, and every other property is optional. Across trials 1–3, the model supplied **every** optional property of **every** tool with a zero value:

- `offset: 0` and `expectedSnapshotToken: 0` on first catalogue pages; this is also the proximate trigger of trial 1's `stale-snapshot` loop;
- `cursor: ""` and all empty filters on `world.object.query`, which returned `world.invalid_cursor`;
- all union properties on every mutation operation.

This is the behavioural signature of strict function calling, which turns every property into a required one. It also matches the agent's own description of the schema.

The relay never declared `strict`, so the model's view of optionality depended on the provider's default. Faithful presentation of the public schema is the relay's job, and the relay is Gate-owned. So this is a **Gate-owned harness defect, not a product reopen**.

Correction:

- `AI_TRIAL_PROTOCOL.json` now carries `toolStrict: false`, and the relay declares `strict: false` explicitly on every offered tool.
- The relay also records the provider and served model per turn.
- The brief, model, route, limits and verifier are unchanged.

The two predecessor reopens stand on their own merits. Their diagnostics were not repairable from the published contract; the trial evidence and the deterministic S06/S12 probes show that. The retry measures the same fresh agent with the host schema presented faithfully.
