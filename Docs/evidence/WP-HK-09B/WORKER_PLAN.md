# WP-HK-09B Worker plan and implementation record

Status: REPAIRING_AFTER_REVIEW_FAIL  
Branch: `wp/hk-09b-resource-persistence`  
Baseline SHA: `ada532f99282c96db15813eb17963bc9cb6d08fb`

## PREDECESSOR_CONTRACT_CHECK

### Accepted direct dependency

- Direct predecessor: `WP-HK-09A`.
- Accepted reviewed candidate SHA: `acb1ccc341aec5131dc2ef979bd322e40e208b53`.
- Independent Reviewer verdict: PASS, PR `#54`, review `#5260496498`.
- Exact-SHA freeze validation: GREEN, Actions `35508529666`, artifact `10604463322`.
- Implementation merge SHA: `614ad941881fdefa83fd46a1a8db989cfaba2cbb`.
- Documentation/roadmap reconciliation is present on the baseline `main`; HK09B is the dependency-valid next workpack.

### Inherited guarantees consumed by HK09B

HK09B consumes, rather than re-proves, these accepted guarantees unless concrete effective evidence contradicts them:

1. HK09A: the production H0 host exposes no generic shell/process authority, no protocol-triggered ambient network authority and no caller-selected filesystem path authority.
2. HK09A/HK07A: every conforming transport crosses `arkus.neutral-projection@1`, whose construction enforces `H0HostCapabilityPolicy` below JSONL/MCP; adapters cannot mint a parallel capability or policy registry. HK07A additionally freezes `arkus.reference.jsonl@1` framing at 1,048,576 bytes with `transport.frame_too_large` above that bound.
3. HK04/HK05/HK06A-C: canonical mutation, validation, provenance, snapshot rebase and replay publish only through their accepted authorities and preserve whole-state atomicity/lineage meaning.
4. HK08A: the representative coherent 96-operation mixed-resource edit is one accepted transaction, one revision advance and one HK06A entry; query and journal pages are bounded to accepted deterministic semantics.
5. HK08B: same-lineage recovery remains truthful and bounded; retry is a normal canonical transaction; the five-flow reference client remains within 12 requests, 15,064 serialized response bytes and the coarse 1,480 ms regression guard.

No predecessor reopen condition was observed. The independent review later found one HK09B regression against item 2; that is repaired locally below rather than reopening HK07A.

## Newly owned HK09B guarantee

HK09B owns only the quantitative/resource and persistence-interruption boundary:

- one explicit transport-neutral H0 envelope for request bytes, portable depth, batch count/payload, page size, per-resource relations/payload, canonical world/snapshot size, session growth and execution budget;
- machine-readable discovery through `system.resource-envelope.describe@1.0` and stable resource diagnostics;
- enforcement below transport-specific adapters with equivalent JSONL/MCP meaning for successfully framed canonical requests;
- preservation of the accepted HK08A 96-operation coherent edit without splitting;
- snapshot/import format/version/resource validation before replacement;
- aggregate publication of imported state + fresh local lineage + import receipt + truthful rebase evidence;
- cooperative execution/cancellation checks at existing canonical mutation/rebase/replay publication boundaries; and
- explicit finite session/world ceilings for later HK10 measurement.

## Implemented design

1. `H0ResourceEnvelope` is the single H0 canonical limit vocabulary and machine-readable data source. Canonical metadata references the same constants.
2. `H0ResourcePolicy` measures canonical portable arguments at neutral projection and rejects bytes/depth/batch/page/snapshot violations before semantic dispatch.
3. `WorldResourceLimits` independently validates materialized state for world bytes/resources, per-resource relations and extension payload bytes, so adapter estimates are not semantic authority.
4. Mutation authority enforces bounded session history, stages next state/receipt/journal and performs one publication after `TryBeginPublication("canonical-mutation")`.
5. Snapshot import was moved from a state-then-wrapper receipt seam to one immutable `PortableSessionState` publication containing imported state, fresh mutation lineage, receipt and rebase evidence.
6. Replay continues to execute only through staged canonical HK04 mutations; it now checks the resource budget immediately before its one final aggregate publication.
7. `InvocationResourceBudget` uses a monotonic stopwatch deadline/cancellation and preserves canonical success after a publication has become authoritative.
8. The inherited `arkus.reference.jsonl@1` framing implementation is left untouched. HK09B's canonical argument/world ceilings are tuned below that frozen 1 MiB boundary so valid framed requests reach the same neutral resource semantics exercised by MCP.

The H0 durability claim remains `process-local-checkpoint`; the envelope explicitly advertises `powerLossDurabilityClaimed=false`. No WAL, `fsync`, distributed storage, multi-process writer coordination or crash-recovery architecture was added.

## Numeric envelope

The H0 ceilings are documented in `RESOURCE_ENVELOPE.md`. Most importantly, `maximumOperations=96` is derived from the accepted HK08A coherent product-shaped intent rather than the previous provisional 64 cap. The HK09B Potes probe independently repeats 72 objects + 24 extensions as one public transaction and checkpoint/rebase flow.

For transport compatibility, the inherited JSONL frame remains 1 MiB; canonical arguments are capped at 896 KiB and canonical world/snapshot bytes at 640 KiB. The latter base64-expands to at most 873,816 characters, leaving 43,688 bytes inside the canonical-argument ceiling for import metadata before framing overhead. These values keep the advertised neutral envelope traversable through JSONL @1 rather than widening @1 in place.

HK08B's accepted 12-request / 15,064-byte / 1,480-ms interaction guards remain separate regression budgets. HK09B's 5-second execution ceiling is an outer fail-closed publication budget, not a relaxation of HK08B performance expectations.

## Convergence and regression repair

The implementation first reached focused HK09B GREEN while full regression exposed four stale inherited assertions introduced by the new legitimate base read capability `system.resource-envelope.describe@1.0`:

- HK01 base inventory assumed exactly one base definition;
- HK01 synthetic/scoped discovery assumed the old total counts;
- HK04 effective non-writer-route self-check had no request vector for the new read route.

Those tests were repaired at their actual invariants: explicit complete capability identities and exact effective non-writer route coverage. No production semantics were weakened.

The first frozen candidate `dcea0901a4fe78549d81271a7f192bbae77e58b7` then received independent Reviewer FAIL in review `#5261028914`: HK09B had widened `arkus.reference.jsonl@1` from 1 MiB to 2 MiB, changed the inherited oversized-frame error to `resource.request_bytes_exceeded`, and moved the HK07A regression oracle with the implementation.

The repair is deliberately local:

- commit `6b35791049bbfe4a3fcda7f65562337671504d95` restores the HK07A `ReferenceTransport.cs` and `Hk07AReferenceTransportTests.cs` blobs exactly to the accepted baseline, preserving 1 MiB + `transport.frame_too_large`;
- commit `dadb0168ab014cb5155aef28315fd6dc7a8a2dba` lowers the new neutral envelope to 896 KiB canonical arguments and 640 KiB canonical world/snapshot bytes, with the machine-readable frame compatibility value at 1 MiB;
- the external-process HK09B conformance test still sends the neutral byte-limit negative through both real JSONL and MCP, so a regression that again places the canonical byte test beyond JSONL framing will turn the comparison red.

Post-review validation is required before this branch may be frozen again. The original pre-FAIL run `35521120979` is historical evidence only and is not sufficient for refreeze.

## Proof/evidence record

- `PROOF_MATRIX.md` — complete acceptance mapping and independent/effective universes.
- `NEGATIVE_CONFORMANCE_MATRIX.md` — required oversize/depth/batch/page/execution/import/interruption/transport controls.
- `RESOURCE_ENVELOPE.md` — enforced ceilings, HK07A compatibility derivation and HK08A/HK08B justification.
- `PERSISTENCE_INTERRUPTION.md` — mutation/import/replay stage/publish proof at the claimed durability level.
- `CONTENT_SHAPE_PROBE.md` — approved `Docs/art/VISUAL_BIBLE.md` Potes slice, 96 operations, checkpoint rebase.
- `RESIDUAL_RISK.md` — trust boundary, non-claims and concrete reopen conditions.
- `WORKER_PRE_REVIEW.md` — strict Worker pre-review; must be refreshed after the reviewer repair is GREEN.

## Freeze condition

Refreeze is allowed only after the post-review evidence-bearing HEAD receives GREEN canonical observation and a refreshed Worker pre-review is CLEAN. The PR body must then record that exact SHA as both Candidate HEAD and Frozen candidate, set `Branch frozen: YES`, and require `scripts/hk09b-verify-exact-sha.sh <SHA>` GREEN in the exact-SHA freeze job. No Worker commit may follow that freeze; a fresh independent Reviewer must then decide PASS/FAIL.

FOUNDATIONAL_PROOF_VERDICT: REPAIR_VALIDATION_PENDING  
UNRESOLVED_PROOF_OBLIGATIONS: 1  
KNOWN_UNDETECTED_DEFECT_CLASSES: 0  
TRUST_BOUNDARY: Docs/evidence/WP-HK-09B/RESIDUAL_RISK.md  
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
