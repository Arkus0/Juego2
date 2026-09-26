# WP-H1-GATE — residual risk and reconciliation summary

The machine-checked reconciliation is authoritative: `RECONCILIATION.json`, checked by `h1-gate-verify.py reconcile`. The checker discovers the inherited universe on its own and fails if any item is missing, unclassified, sourceless or textually changed. This file summarizes that reconciliation for readers.

## Reconciliation totals

| Disposition | Count | Meaning |
| --- | ---: | --- |
| `CARRIED_FORWARD` | 67 | Remains a visible post-H1 non-claim with a named owner (H2/ART/CITY/H0S/product/future). |
| `CLOSED_BY_ACCEPTED_OWNER` | 23 | Closed by an accepted H1 WP (PASS identity in the row), and exercised by the Gate where relevant. |
| `CONFIRMED_BY_GATE` | 16 | Confirmed by Gate evidence on the exact candidate. Examples: R-00A-05 final confirmation; H1-10's residual that its Unity stages bypassed the public host dispatch; the risk rows "second mutation authority", "private route", "partial publication", "manual edits become truth", "checkpoint redefines lineage", "gate invents semantics". |
| `SATISFIED_BY_GATE_EVIDENCE` | 5 | H0S measurements that H1 records without solving them (object count, snapshot size, plan size/duration, catalogue size/duration, drift cost, commit latency/stale rate), in `facts-*.json#h0sMeasurements`. |
| `PROCESS_RECORD` | 10 | Historical process statements (readiness markers, proof-budget notes, derived-projection refresh). |

Total: 119 discovered items plus the mandatory H1-11 owner residual (`.meta` sidecar integrity, PR #236 comment `5845036679`, carried forward as future proof-harness hardening).

The dependency reconciliation lists all 15 accepted H1 workpacks and records how the Gate consumes each one. The stage reconciliation maps all 17 parity-gate stages.

No inherited residual was reclassified to hide it. Mixed items, where part is closed and part remains open, are conservatively `CARRIED_FORWARD`, and the closed part is named in `basis`.

## Gate-specific residuals (outside the claim)

- **Trial model independence** is structural: a different vendor's model receives only the brief and the MCP tool list, with no repository or file access. It is not a benchmark of model quality. One hosted trial is the WP's single required trial. A trial FAIL caused by model capability, rather than by a public-surface defect, is classified in the pre-review and is not a bridge defect.
- **Relay truncation:** tool results longer than 14,000 characters are shortened for the model's context, and the full result is always recorded. Top-level digests precede node arrays in the accepted result shapes, so truncation does not hide closing evidence.
- **Batch-per-operation latency:** every Editor-bound public call launches one short-lived Editor process, per the accepted ADR-H1-004 profile. The Gate records durations (H0S) and makes no latency claim.
- **First-import timeout:** the first public project inspection performs the project's clean import. If it exceeds the 120 s H1 operation ceiling, the Gate retries only that read-only inspection under the accepted lifecycle semantics (operation-status, then a restart). This is recorded in the transcript. No other stage is retried.
- **Stage-16 capture** is the product launcher's own Editor log of the rebuilt real slice being loaded, scanned for owned errors. It is supplementary. No pixel capture is produced on the public path, because the public surface has no rendering capability and the Gate may not add one. The accepted H1-11 rendered capture of the same selected items stays the visual reference.
- **Package universe:** the public surface reports the effective editor identity (S03) and the package fingerprint inside the checkpoint environment digest (S14). S01 checks that the committed package lock is exact. A per-package enumeration capability does not exist and is not added.
- Everything declared in `H1_RISK_AND_RESIDUAL_PLAN.md#residuals-expected-after-h1` stays visible for H2 planning.

## Reopen status

**Reopen triggered (owner decision, 2026-09-26): WP-H1-04.** The deterministic Gate was GREEN, but the first fresh AI-agent trial failed on the public catalogue surface. The causes were an undeclared page bound, a `stale-snapshot` diagnostic with no current token and a misleading repair hint, a managed scene ID that was not discoverable in the catalogue, and a generic encoder-rejection code. Evidence, classification (R1–R4) and routing are in `AI_TRIAL_HISTORY.md`. The Gate repaired none of these.

**Reopen resolved: WP-H1-04 reopen 1.** PR `#239`, frozen candidate `ce3ba74efae28692d886a5e11366c7849695e9f6`, merge `4ab82fb1c0a0d8654aaa44ebc66452a3507ef347`. The owner waived independent review for that correction only. The fix gives structured context and code-specific hints on catalogue errors, and projection codes on pre-launch refusals. It does not change capabilities, schemas, codes or catalogue content. The Gate merged the new base and re-ran its deterministic validation on the new exact SHA. S12 now requires the actionable refusal codes. The trial was then repeated once on the final frozen SHA (`AI_TRIAL_HISTORY.md`). Residual: R1 is solved through diagnostics, not through schema numeric bounds, because `SchemaNode` (H0) cannot express `minimum`/`maximum`. This is a named future H0 schema-expressiveness decision, not a Gate blocker.

**Second reopen: WP-HK-05 (trial 2, R5).** After the H1-04 fix, the fresh agent could page the catalogue but could not recover from an operation-grammar rejection. The published operation schema is a flattened union, and the rejection carried no field context. The fix is WP-HK-05 reopen 1: PR `#241`, frozen candidate `93cdc3b5a6f38190a4ae0b8ba13894a004babe78`, merge `5d48cb55ac2815be417dc2a910734192b6d1543c`, review waived by owner instruction (`#5846410743`). It adds the per-kind allowed, required and unexpected fields and a hint, and it leaves the grammar, codes and schemas unchanged. Residual: a per-kind `oneOf` in the published schema needs H0 `SchemaNode` expressiveness. This is a named future H0 decision, not a Gate blocker.
