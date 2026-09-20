# WP-HK-09B Worker plan and implementation record

Status: READY_FOR_FREEZE  
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
2. HK09A/HK07A: every conforming transport crosses `arkus.neutral-projection@1`, whose construction enforces `H0HostCapabilityPolicy` below JSONL/MCP; adapters cannot mint a parallel capability or policy registry.
3. HK04/HK05/HK06A-C: canonical mutation, validation, provenance, snapshot rebase and replay publish only through their accepted authorities and preserve whole-state atomicity/lineage meaning.
4. HK08A: the representative coherent 96-operation mixed-resource edit is one accepted transaction, one revision advance and one HK06A entry; query and journal pages are bounded to accepted deterministic semantics.
5. HK08B: same-lineage recovery remains truthful and bounded; retry is a normal canonical transaction; the five-flow reference client remains within 12 requests, 15,064 serialized response bytes and the coarse 1,480 ms regression guard.

No predecessor reopen condition was observed.

## Newly owned HK09B guarantee

HK09B owns only the quantitative/resource and persistence-interruption boundary:

- one explicit transport-neutral H0 envelope for request bytes, portable depth, batch count/payload, page size, per-resource relations/payload, canonical world/snapshot size, session growth and execution budget;
- machine-readable discovery through `system.resource-envelope.describe@1.0` and stable resource diagnostics;
- enforcement below transport-specific adapters with equivalent JSONL/MCP meaning;
- preservation of the accepted HK08A 96-operation coherent edit without splitting;
- snapshot/import format/version/resource validation before replacement;
- aggregate publication of imported state + fresh local lineage + import receipt + truthful rebase evidence;
- cooperative execution/cancellation checks at existing canonical mutation/rebase/replay publication boundaries; and
- explicit finite session/world ceilings for later HK10 measurement.

## Implemented design

1. `H0ResourceEnvelope` is the single H0 limit vocabulary and machine-readable data source. Canonical metadata references the same constants.
2. `H0ResourcePolicy` measures canonical portable arguments at neutral projection and rejects bytes/depth/batch/page/snapshot violations before semantic dispatch.
3. `WorldResourceLimits` independently validates materialized state for world bytes/resources, per-resource relations and extension payload bytes, so adapter estimates are not semantic authority.
4. Mutation authority enforces bounded session history, stages next state/receipt/journal and performs one publication after `TryBeginPublication("canonical-mutation")`.
5. Snapshot import was moved from a state-then-wrapper receipt seam to one immutable `PortableSessionState` publication containing imported state, fresh mutation lineage, receipt and rebase evidence.
6. Replay continues to execute only through staged canonical HK04 mutations; it now checks the resource budget immediately before its one final aggregate publication.
7. `InvocationResourceBudget` uses a monotonic stopwatch deadline/cancellation and preserves canonical success after a publication has become authoritative.
8. Reference JSONL framing was aligned so successfully framed requests reach the same neutral resource depth/byte semantics exercised by MCP.

The H0 durability claim remains `process-local-checkpoint`; the envelope explicitly advertises `powerLossDurabilityClaimed=false`. No WAL, `fsync`, distributed storage, multi-process writer coordination or crash-recovery architecture was added.

## Frozen numeric envelope

The final H0 ceilings are documented in `RESOURCE_ENVELOPE.md`. Most importantly, `maximumOperations=96` is derived from the accepted HK08A coherent product-shaped intent rather than the previous provisional 64 cap. The HK09B Potes probe independently repeats 72 objects + 24 extensions as one public transaction and checkpoint/rebase flow.

HK08B's accepted 12-request / 15,064-byte / 1,480-ms interaction guards remain separate regression budgets. HK09B's 5-second execution ceiling is an outer fail-closed publication budget, not a relaxation of HK08B performance expectations.

## Convergence and regression repair

The implementation first reached focused HK09B GREEN while full regression exposed four stale inherited assertions introduced by the new legitimate base read capability `system.resource-envelope.describe@1.0`:

- HK01 base inventory assumed exactly one base definition;
- HK01 synthetic/scoped discovery assumed the old total counts;
- HK04 effective non-writer-route self-check had no request vector for the new read route.

Those tests were repaired at their actual invariants: explicit complete capability identities and exact effective non-writer route coverage. No production semantics were weakened.

Implementation checkpoint `e370606e4278da3e08602a3167c1cb6513bea93d` then passed Actions run `35521120979`: Release build 0 warnings / 0 errors, focused HK09B 9/9, full regression 205/205, candidate clean before and after.

## Proof/evidence record

- `PROOF_MATRIX.md` — complete acceptance mapping and independent/effective universes.
- `NEGATIVE_CONFORMANCE_MATRIX.md` — required oversize/depth/batch/page/execution/import/interruption/transport controls.
- `RESOURCE_ENVELOPE.md` — enforced ceilings and HK08A/HK08B justification.
- `PERSISTENCE_INTERRUPTION.md` — mutation/import/replay stage/publish proof at the claimed durability level.
- `CONTENT_SHAPE_PROBE.md` — approved `Docs/art/VISUAL_BIBLE.md` Potes slice, 96 operations, checkpoint rebase.
- `RESIDUAL_RISK.md` — trust boundary, non-claims and concrete reopen conditions.
- `WORKER_PRE_REVIEW.md` — strict Worker pre-review CLEAN, three material convergence findings fixed.

## Freeze condition

The next/last Worker action is metadata-only freeze: wait for canonical observation to be GREEN on this final evidence-bearing HEAD, record that exact SHA in PR #56 as both Candidate HEAD and Frozen candidate, set `Branch frozen: YES`, and require the exact-SHA freeze job to run `scripts/hk09b-verify-exact-sha.sh <SHA>` GREEN. No Worker commit may follow that freeze; a fresh independent Reviewer must then decide PASS/FAIL.

FOUNDATIONAL_PROOF_VERDICT: READY  
UNRESOLVED_PROOF_OBLIGATIONS: 0  
KNOWN_UNDETECTED_DEFECT_CLASSES: 0  
TRUST_BOUNDARY: Docs/evidence/WP-HK-09B/RESIDUAL_RISK.md  
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
