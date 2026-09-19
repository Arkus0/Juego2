# WP-HK-06A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-06A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted single-process H0 authored-world session, every newly persisted canonical
mutation appends exactly one deterministic, versioned provenance entry at the accepted HK04 commit
boundary. The entry carries the normalized accepted request, its identity and canonical capability
version, truthful before/after authored anchors and the affected-resource set. Non-persisting work
does not append. Authored state, idempotency receipts and the journal are published as one immutable
session-state reference while holding the existing commit lock.

Live/runtime observation is a separate semantic category. The HK06A stamp contract carries only the
authored base revision/hash; a bounded test-owned scheduled-NPC surrogate changes transient fields
without changing `WorldState`, authored revision/hash or the journal. HK06A makes no deterministic
simulation claim.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| every newly committed authored mutation creates one entry | accepted current public canonical-mutation surface | HK06A consumes HK01/HK04's accepted route and commit-authority completeness; append is inside the only accepted apply commit section, immediately before the single immutable state publication | two sequential public applies produce sequences 1/2; a concurrent stale-writer race produces one accepted state and one matching entry | independent audit removes one effective entry and returns `entry-count-mismatch` | PASS | future mutation surfaces must continue to enter this boundary or revise the contract under review |
| only persisted mutations enter successful history | plan, dry-run, inspection, validation, journal read, rejected/malformed/stale apply and idempotent retry | only the final CAS-success block constructs/publishes an entry; receipts return before planning/append; all failure returns precede publication | effective non-persisting routes leave journal unchanged; one accepted apply increments it once; exact retry remains one entry | effective entry supplied to an audit expecting zero persisted transitions turns RED as an extra entry | PASS | policy/audit telemetry outside successful mutation history is not implemented here |
| before/result anchors are truthful | canonical `WorldState` revision/hash at commit | base anchor is checked against the current authored state and prior journal tail; result hash is recomputed from the exact immutable candidate; mismatch returns structured failure before publication | every entry compared with independently captured before/after `WorldState`; journal `current` equals effective session state | corrupted base hash and result revision each produce distinct audit failures | PASS | cryptographic primitive correctness is trusted per the proof standard |
| request identity/version and replay evidence are complete for HK06A | current HK04 mutation envelope and `authoring.change.apply@1.0` | entry stores idempotency key, canonical request fingerprint, capability name/version and the full normalized mutation envelope accepted by the existing parser | schema-valid journal exposes both operations, anchors and canonical Base64 payload from the accepted request | identity/hash syntax and schema validation reject synthetic corruptions; wrong entry ID turns audit RED | PASS | replay compatibility/interpretation belongs to HK06C |
| affected resources agree with accepted mutation | resources named by inherited HK04 semantic change set | HK06A consumes HK04's accepted independent change-coverage guarantee and derives a sorted unique resource set from that covered plan; test expectation is independent/manual | multi-resource object+extension commit and Potes shop edit expose exact deterministic sets | replacing the set with an invented resource turns audit RED | PASS | field-level semantic diff belongs to HK06B; journal owns affected resource identity only |
| ordering and entry identity are deterministic | one journal lineage from explicit base anchor | sequence is commit order under the commit lock; entry ID hashes a length-framed versioned identity containing sequence, request fingerprint, anchors and sorted resources | two clean sessions executing the same requests produce identical ordered IDs and final hash | invalid/noncanonical entry ID is rejected by the independent audit | PASS | identity is lineage-specific; cross-version compatibility belongs to HK06C |
| journal schema/version is canonical and discoverable | composed Arkus contract | definition and handler are added through the accepted canonical composer; no journal-owned registry exists | `system.describe` exposes `authoring.journal.read@1.0`; its success schema validates effective output carrying `arkus.authoring.journal@1` and `arkus.authoring.journal-entry@1` | inherited route-universe/conformance regression would turn RED if definition/handler/discovery drift | PASS | pagination/compact history reads belong to HK08 |
| authored/live boundary is explicit | bounded runtime-observation stamp and test-owned surrogate | observation stamp is a separately named schema containing an authored anchor only; surrogate owns transient step/position and receives no authoring service | two synthetic steps change surrogate values while revision/hash/journal IDs remain byte-semantically unchanged | unsafe test surrogate granted an apply callback turns the boundary oracle RED for revision, hash and journal | PASS | gameplay timing, movement meaning and deterministic simulation are deliberately unclaimed |
| accepted predecessor guarantees remain composed | HK01-HK05 integration seams only | new read route uses HK04 attenuation; commit still uses HK04 internal committer and HK05 validation before planning; no second writer/validator/state model added | full 115-test regression GREEN on implementation observation | accepted HK04 effective non-mutation oracle includes the new journal route and proves it state-neutral | PASS | concrete contrary evidence would reopen the relevant predecessor, none observed |
| forbidden scope absent | baseline-to-candidate diff | changes are limited to provenance/stamp contracts, transaction-boundary integration, one read binding, focused tests/scripts/evidence | strict diff audit | no diff/snapshot/replay/Unity/scheduler/AI/GUI/cloud/Git-history implementation present | PASS | none |

## Independent/effective universes

1. **Mutation universe:** accepted HK01/HK04 composed public route/commit boundary. HK06A consumes it
   rather than asking the journal to decide which mutations exist.
2. **Effective persisted state:** accepted HK02 canonical `WorldState` revision and content hash,
   captured independently before and after public dispatch.
3. **Journal publication:** one immutable `AuthoringState` reference contains current state,
   idempotency receipts and ordered entries. Readers and writers use the same lock.
4. **Truthfulness oracle:** test-owned expected transitions are built from independently captured
   states and manually named representative resource sets, not from journal output.
5. **Authored/live oracle:** the test-owned runtime surrogate changes its own fields; the oracle
   observes only effective canonical state/hash/revision and journal IDs.
6. **Public schema universe:** inherited canonical composition/discovery reconciles the new
   definition and route; effective output is validated by the discovered definition's schema.

## Observation receipt before evidence reconciliation

Implementation SHA `611ec0ea06f4b4cd5a2a2fca149d89f6a5e44b9e` passed candidate observation
Actions `35467452640` on Ubuntu 24.04 with pinned .NET SDK 8.0.425:

- Release build: 0 warnings / 0 errors;
- focused `Hk06A*`: 5/5 GREEN;
- full regression: 115/115 GREEN;
- exact-SHA clean-before/clean-after receipt: GREEN.

Subsequent evidence and concurrency-regression changes require a fresh exact-SHA observation before
freeze; this earlier run is implementation feedback, not the final receipt.

## Proof-budget verdict

HK06A adds one product journal model, one canonical read route, a small runtime-observation stamp and
six focused tests after the final concurrency integration. Negative controls reuse one compact
test-owned audit and one boundary oracle; they do not introduce a generalized verifier, another
semantic registry or duplicate HK01-HK05 proof machinery. Every control maps directly to an HK06A
acceptance clause or required defect class.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
