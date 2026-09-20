# WP-HK-09B residual-risk audit

## Trust boundary

HK09B claims bounded H0 request/session resources and fail-closed **process-local** authoritative publication. It trusts the default foundational base: exact Git checkout, pinned .NET SDK/runtime/compiler/MSBuild/NuGet behavior, normal documented process/thread/lock/reference-assignment semantics, CI runner/OS/filesystem infrastructure and standard hash primitives.

Inside the claim are:

- portable request byte/depth admission;
- mutation operation/payload/relation bounds;
- page-size admission;
- materialized canonical world/snapshot limits;
- bounded session transaction/import-receipt growth;
- cooperative execution/cancellation checks at canonical dispatch/publication seams;
- atomic in-process publication of mutation state+receipt+journal, snapshot state+lineage+receipt+rebase evidence, and replay state+receipts+journal;
- reference JSONL/MCP equivalence for resource semantics;
- preservation of the accepted 96-operation coherent authoring shape.

## Non-blocking residuals

### Power loss / process crash durability

`system.resource-envelope.describe@1.0` states `powerLossDurabilityClaimed=false`. HK09B therefore does not prove recovery from process kill, kernel/filesystem failure, torn disk write or machine power loss. No WAL/fsync/storage engine exists in H0. This is intentionally outside the accepted claim, not an undetected defect.

### Cooperative rather than preemptive execution limit

The 5 s execution budget is checked through `InvocationResourceBudget` and before authoritative publication; read-only dispatch also checks before returning when no publication committed. It is not a scheduler that forcibly aborts arbitrary managed CPU at exactly 5000 ms. A pathological calculation can consume CPU until the next cooperative check, but it cannot truthfully publish a new canonical aggregate after the publication check has failed. Hard real-time/preemptive execution is outside H0.

### Adapter framing versus canonical request bytes

Reference JSONL has a larger 2 MiB framing ceiling while canonical argument bytes are limited to 1792 KiB. Malformed, incomplete or impossible transport frames may be rejected before a neutral canonical request exists. HK09B claims semantic equivalence for successfully framed canonical requests and verifies representative bytes/depth/page failures end-to-end; it does not require all malformed transport syntax to share canonical diagnostics.

### Base64 early estimates

Neutral admission estimates decoded base64 volume to reject clearly oversized payloads early. Semantic authority does not rest on that estimate: canonical parsers decode/validate payloads and `WorldResourceLimits` checks the materialized state before publication. Invalid encoding remains a schema/parse failure rather than being trusted because the estimate was small.

### Future production scale

The H0 world/session ceilings are finite proof/operability bounds, not throughput or scale promises for a complete shipped game. HK10 owns bounded long-session measurement. H1/H2 may provide evidence that specific ceilings require revision; such a change must preserve atomicity rather than silently splitting coherent intents.

### Multi-process/distributed writers

H0 retains one process-local canonical authority and whole-world revision+hash CAS semantics. File/network/distributed storage, multiple writers, per-resource locks/CAS and distributed transactions remain outside HK09B and were explicitly forbidden by the WP.

### Final engine/editor persistence

Unity/editor asset databases, scene serialization and engine-specific crash recovery are H1+ bridge concerns. HK09B proves the engine-neutral canonical/session publication seam only.

## Reopen conditions

HK09B should be reopened (rather than merely tuned downstream) if evidence shows any of the following within the accepted H0 claim:

1. a valid accepted 96-operation coherent HK08A intent is rejected or split by the resource layer;
2. a rejected/expired/interrupted writer changes canonical revision/hash, HK06A journal, idempotency receipt or rebase evidence;
3. imported state can become visible separately from its fresh lineage/receipt/evidence aggregate;
4. replay can publish staged state separately from staged provenance;
5. a valid framed canonical request produces materially different resource semantics between reference JSONL and MCP;
6. the public machine-readable envelope diverges from the actually enforced H0 limits;
7. an HK10 bounded-session measurement demonstrates that the declared session ceiling prevents an accepted H0 workflow rather than merely bounding deferred production scale.

## Blocking audit

No currently known residual inside the declared HK09B claim can materially falsify the acceptance criteria while the focused tests, inherited authority regressions, exact-SHA observation and evidence gates remain GREEN.

KNOWN_UNDETECTED_DEFECT_CLASSES: 0
UNRESOLVED_PROOF_OBLIGATIONS: 0
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
