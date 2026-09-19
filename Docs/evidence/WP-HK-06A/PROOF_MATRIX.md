# WP-HK-06A foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-06A/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

Inside the accepted single-process H0 authored-world session, every newly persisted canonical
mutation appends exactly one deterministic, versioned provenance entry at the accepted HK04 commit
boundary. The entry carries the complete normalized accepted request, its identity and canonical
capability version, truthful before/after authored anchors and the affected-resource set.
Non-persisting work does not append. Authored state, idempotency receipts and the journal are
published as one immutable session-state reference while holding the existing commit lock.

Repair cycle 1 strengthens the replay-evidence claim discovered false-green by independent review:
before an entry can be published, HK06A independently re-derives the request fingerprint from the
journal-ready machine-readable envelope itself and requires it to equal the fingerprint of the
parsed/executed request. The envelope's idempotency key and expected revision/hash must also match
the recorded request identity and base anchor. Entry construction then independently verifies the
length-framed entry identity against that bound fingerprint, transition anchors and affected
resources. Therefore a schema-valid serializer drift cannot silently produce replay instructions
that disagree with the mutation actually accepted.

A separate test-owned oracle does not call that production guard: it compares the complete
journalized request against an independently constructed accepted request and compares entry identity
against a clean second execution. This closes the circular-oracle concern without implementing
HK06C replay.

Live/runtime observation remains a separate semantic category. The HK06A stamp contract carries only
the authored base revision/hash; a bounded test-owned scheduled-NPC surrogate changes transient
fields without changing `WorldState`, authored revision/hash or the journal. HK06A makes no
deterministic simulation claim.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive/effective evidence | Causal negative control | Result | Residual risk |
|---|---|---|---|---|---|---|
| every newly committed authored mutation creates one entry | accepted current public canonical-mutation surface | HK06A consumes HK01/HK04 route and commit-authority completeness; append is inside the only accepted apply commit section immediately before the single immutable state publication | two sequential public applies produce sequences 1/2; concurrent stale-writer race produces one accepted state and one matching entry | independent transition audit removes one effective entry and returns `entry-count-mismatch` | PASS | future mutation surfaces must continue to enter this boundary or revise the contract under review |
| only persisted mutations enter successful history | plan, dry-run, inspection, validation, journal read, rejected/malformed/stale apply and idempotent retry | only the final CAS-success block constructs/publishes an entry; receipts return before planning/append; all failure returns precede publication | effective non-persisting routes leave journal unchanged; one accepted apply increments it once; exact retry remains one entry | effective entry supplied to an audit expecting zero persisted transitions turns RED as an extra entry | PASS | policy/audit telemetry outside successful mutation history is not implemented here |
| before/result anchors are truthful | canonical `WorldState` revision/hash at commit | base anchor is checked against current authored state and prior journal tail; result hash is recomputed from the exact immutable candidate; mismatch prevents publication | every entry compared with independently captured before/after `WorldState`; journal `current` equals effective session state | corrupted base hash and result revision each produce distinct audit failures | PASS | cryptographic primitive correctness is trusted per proof standard |
| normalized accepted request is truthful replay evidence | current HK04 mutation grammar; no replay execution | journal-ready envelope is independently interpreted by `WorldProvenanceIntegrity`; its full operation grammar is fingerprinted independently from private parsed operation objects, top-level identity/base fields are reconciled, and a mismatch throws before `AuthoringState` publication | complete object+extension envelope is deeply equal to the test-built accepted request; all four operation kinds survive the effective path unchanged | schema-valid corruption of top identity/base fields and replay-relevant object/extension/remove fields turns production guard RED; separate test-owned audit turns RED for valid `typeId` and payload corruption | PASS | replay compatibility/interpretation belongs to HK06C |
| request identity/version is bound to the transition | `authoring.change.apply@1.0`, request identity and HK06A entry ID | request fingerprint is bound to the normalized envelope; idempotency key is checked separately; entry ID length-frames schema/version, sequence, bound request identity, anchors and sorted resources; constructor independently verifies the formula | two clean sessions executing the same request produce identical entry IDs | canonical-looking 64-hex wrong ID is rejected by production guard and by the test-owned second-run identity oracle | PASS | compatibility across accepted future journal versions belongs to HK06C |
| affected resources agree with accepted mutation | resources named by inherited HK04 semantic change set | HK06A consumes HK04's accepted independent change-coverage guarantee and derives a sorted unique resource set from that covered plan | multi-resource object+extension commit and Potes shop edit expose exact deterministic sets | replacing the set with an invented resource turns audit RED | PASS | field-level semantic diff belongs to HK06B |
| ordering is deterministic | one journal lineage from explicit base anchor | sequence is commit order under the commit lock; entry identity includes sequence and transition/request identity | two clean sessions produce identical ordered IDs and final hash | sequence/identity corruption turns bounded audit RED | PASS | identity is lineage-specific |
| journal schema/version is canonical and discoverable | composed Arkus contract | definition and handler are added through accepted canonical composer; no journal-owned registry exists | `system.describe` exposes `authoring.journal.read@1.0`; success schema validates `arkus.authoring.journal@1` / `arkus.authoring.journal-entry@1` output | inherited route-universe/conformance regression turns RED on definition/handler/discovery drift | PASS | pagination/compact history reads belong to HK08 |
| authored/live boundary is explicit | bounded runtime-observation stamp and test-owned surrogate | observation stamp is a separately named schema containing an authored anchor only; surrogate owns transient step/position and receives no authoring service | two synthetic steps change surrogate values while revision/hash/journal IDs remain unchanged | unsafe test surrogate granted an apply callback turns boundary oracle RED for revision, hash and journal | PASS | gameplay timing, movement meaning and deterministic simulation are deliberately unclaimed |
| accepted predecessor guarantees remain composed | HK01-HK05 integration seams only | new read route uses HK04 attenuation; commit still uses HK04 internal committer and HK05 validation before planning; no second writer/validator/state model added | full regression GREEN on repaired implementation observation | accepted HK04 effective non-mutation oracle includes new journal route and proves it state-neutral | PASS | concrete contrary evidence would reopen predecessor; none observed |
| forbidden scope absent | baseline-to-candidate diff | changes remain limited to provenance/stamp contracts, commit-boundary integration, one read binding, focused tests/scripts/evidence and the bounded internal request-integrity guard | strict diff audit | no semantic diff/snapshot/replay/Unity/scheduler/AI/GUI/cloud/Git-history implementation present | PASS | none |

## Independent/effective universes

1. **Mutation universe:** accepted HK01/HK04 composed public route/commit boundary; HK06A consumes it.
2. **Effective persisted state:** accepted HK02 canonical `WorldState` revision/content hash captured before and after public dispatch.
3. **Journal publication:** one immutable `AuthoringState` reference contains current state, receipts and ordered entries; readers and writers use the same lock.
4. **Transition truthfulness oracle:** test-owned expected transitions are built from independently captured states and manually named representative resource sets.
5. **Accepted-request oracle:** test-owned request data is constructed before dispatch and compared recursively with the full journal `request`; it is not derived from journal output or the production integrity guard.
6. **Request-binding guard:** production independently interprets the machine-readable envelope and reconciles it with parsed-request fingerprint/idempotency/base anchor before publication.
7. **Identity oracle:** a second clean execution supplies the independently expected deterministic entry ID; a canonical-looking wrong value is rejected.
8. **Authored/live oracle:** the test-owned runtime surrogate changes only its own fields while the oracle observes effective canonical state/hash/revision and journal IDs.
9. **Public schema universe:** inherited canonical composition/discovery reconciles the new definition/route; effective output is validated by the discovered schema.

## Repair implementation observation

Implementation SHA `e78e587dd098f765d4266bc9d61511118faab7d2` passed candidate observation
Actions `35468947971` on Ubuntu 24.04 with pinned .NET SDK 8.0.425:

- Release build: 0 warnings / 0 errors;
- focused `Hk06A*`: 9/9 GREEN;
- full regression: 119/119 GREEN;
- exact-SHA clean-before/clean-after observation receipt: GREEN;
- artifact `10592091971` contains the observation log and receipt.

This evidence reconciliation commit is documentation-only. Its resulting exact branch SHA must
receive a fresh canonical exact-SHA verification unchanged before freeze; the implementation
observation above is strong feedback but is not reused as the final frozen-candidate receipt.

## Proof-budget verdict

The repair adds one internal fail-closed integrity guard at the existing provenance-entry boundary,
two focused production-guard self-attack tests and one independent accepted-envelope audit test.
This support directly answers a concrete Reviewer-proven false-green class. It does not add a replay
engine, generalized event store, second state model, semantic registry or duplicate predecessor proof.
The focused HK06A suite is 9 tests and the full regression is 119 tests.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.
