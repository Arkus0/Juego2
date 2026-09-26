# WP-H1-GATE — fresh AI-agent trial history and predecessor reopen routing

Gate state: **REOPEN RESOLVED — trial 2 on the final frozen SHA**. Owner decision (2026-09-26): reopen WP-H1-04 before any further trial. The reopen is merged (see below).

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
