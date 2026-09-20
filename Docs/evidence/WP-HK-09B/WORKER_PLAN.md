# WP-HK-09B Worker plan and implementation record

Status: IMPLEMENTING  
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
4. HK08A: the representative coherent 96-operation mixed-resource edit is one accepted transaction, one revision advance and one HK06A entry; query and journal pages are bounded to the accepted deterministic page semantics.
5. HK08B: same-lineage recovery remains truthful and bounded; retry is a normal canonical transaction; the five-flow reference client remains within 12 requests, 15,064 serialized response bytes and the coarse 1,480 ms regression guard.

The materially relied-on transitive evidence is the accepted HK08A 96-operation content shape and HK08B public-process benchmark. HK09B may bound their resource cost but may not split the coherent edit, fabricate recovery history, or weaken atomicity/validation/provenance.

### Guarantees newly owned by HK09B

HK09B owns only the quantitative/resource and persistence-interruption boundary:

- one explicit transport-neutral H0 envelope for request bytes, portable nesting, batch operation count/payload, page size, authored-state/snapshot size, per-resource reference/dependency count and execution budget;
- machine-readable discovery of that envelope and stable resource-limit diagnostics;
- enforcement below transport-specific adapters, with equivalent JSONL/MCP meaning;
- proof that the accepted HK08A 96-operation edit and HK08B workflow fit without splitting or semantic weakening;
- snapshot/import format/version and size validation before canonical replacement;
- one aggregate publication boundary for imported state, import idempotency receipt and truthful rebase evidence;
- cooperative execution/cancellation checks at canonical mutation/rebase/replay publication, without a second mutation authority; and
- a bounded H0 session envelope that HK10 can exercise for growth/endurance evidence.

### Guarantees intentionally not re-proved

HK09B will rerun the complete regression suite but will not duplicate HK09A shell/network/filesystem/type-activation proof, HK07 transport-inventory completeness, HK04 hidden-writer completeness, HK06 replay truth or HK08 recovery ancestry proof. A desire for defence in depth is not a reopen condition.

### Concrete predecessor reopen conditions

A predecessor is reopened only if HK09B finds concrete evidence that:

- a valid accepted HK08A/HK08B public flow cannot fit any proportionate bounded envelope without being split or semantically weakened;
- the accepted neutral-projection path is not shared by an effective JSONL/MCP invocation, making transport-neutral enforcement impossible;
- a canonical mutation/rebase/replay publication path can change authoritative state outside the accepted aggregate authority; or
- the accepted HK09A policy boundary itself provides one of the forbidden host powers.

No predecessor reopen condition was observed before implementation.

## Initial implementation observations

1. The reference JSONL adapter already has an unadvertised 1 MiB frame cap and parser depth 256, while MCP reaches the same neutral projection after SDK framing; these adapter-local constants are not yet one resource semantic.
2. Mutation count (96) and query/journal page size (100) already exist as scattered service constants. Extension payload bytes, dependency/reference counts, total authored-state bytes and snapshot bytes remain effectively unbounded.
3. `NeutralProjectionService` serializes dispatch but only applies caller timeout/cancellation before canonical dispatch. Post-admission execution currently has no H0 budget.
4. HK04 mutation publication already swaps one immutable aggregate containing state, mutation receipts and journal. Replay stages a complete session and swaps it once. Both are suitable publication boundaries for a budget/interruption checkpoint.
5. Snapshot import swaps the authored session inside the raw importer, but keyed import receipts and rebase evidence are assembled in a separate wrapper afterward. HK09B must close this real publication seam so interruption cannot separate authoritative state from accepted import evidence.

## Planned bounded design

1. Define one transport-neutral H0 resource-envelope contract and expose it through canonical discovery rather than through adapter-only documentation.
2. Enforce portable request byte/depth limits at neutral projection and retain only framing-level early rejection in adapters.
3. Reconcile existing operation/page limits with the envelope, then add batch payload, per-resource relation, authored-state and snapshot constraints before expensive materialization/publication.
4. Apply one fixed H0 execution budget below transports. Read-only work may return a resource failure after bounded evaluation; mutation/rebase/replay must check the same budget immediately before their accepted aggregate publication and preserve canonical success once publication is authoritative.
5. Move snapshot state + import receipt + rebase evidence into one immutable session-root publication. Inject interruption at the real validate/stage/publish boundary and prove the previous state, journal and accepted receipts remain unchanged.
6. Preserve the accepted process-local checkpoint/snapshot durability claim. Do not add a WAL, `fsync`, distributed storage, multi-process writer coordination or automatic crash recovery.

Numeric values are frozen only after measuring the accepted HK08A/HK08B representative shapes. Existing constants are inputs, not authority.

## Proof boundary and budget

Trust boundary: repository-owned canonical contract, neutral projection, JSONL/MCP adapters and process-local authored session publication, under the default trusted Git/.NET/OS/runtime base. H0 claims atomic in-process aggregate publication and fail-closed rejected/interrupted staging; it does not claim power-loss durability, WAL recovery, external storage correctness, arbitrary large-world scale or multi-process coordination.

The proof budget is limited to the nine negative classes named by `WP-HK-09B`, one approved Juego2 content-shape probe, transport equivalence for changed resource errors and the actual mutation/rebase/replay publication seams. No generic resource-governance framework or OS-level crash model will be introduced.

FOUNDATIONAL_PROOF_VERDICT: NOT_READY  
UNRESOLVED_PROOF_OBLIGATIONS: 9  
KNOWN_UNDETECTED_DEFECT_CLASSES: 1  
TRUST_BOUNDARY: this file; final residual audit pending  
PROOF_BUDGET_VERDICT: WITHIN_BUDGET
