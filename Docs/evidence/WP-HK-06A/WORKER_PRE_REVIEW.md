# WP-HK-06A Worker pre-review — repair cycle 1

WORKER_PRE_REVIEW: CLEAN
WORKER_PRE_REVIEW_FINDINGS_FIXED: 1
WORKER_PRE_REVIEW_EVIDENCE: Docs/evidence/WP-HK-06A
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

Baseline: `bc6241d2db2b6e15f1a9ac78c673f7ef0735ffff`
Reviewer-failed candidate: `046134fd3acd9641798dbad180406688fb5d31aa`
Repair implementation SHA: `e78e587dd098f765d4266bc9d61511118faab7d2`
Reviewer FAIL: review `5257604438`

Actions run `35468947971` observed the repaired implementation SHA on Ubuntu 24.04 with pinned .NET
SDK 8.0.425: Release build 0 warnings / 0 errors; focused `Hk06A*` 9/9 GREEN; full regression
119/119 GREEN; clean-before/clean-after exact-SHA observation receipt GREEN. Artifact `10592091971`
contains the receipt and observation log.

This report is the final evidence reconciliation write for repair cycle 1. Its resulting branch SHA
must receive a fresh canonical exact-SHA verification unchanged before freeze; the implementation
observation above is not reused as the frozen-candidate receipt.

## Contract and predecessor re-check

The repair pre-review re-read `WP-HK-06A`, the HK06A→HK06B→HK06C split and downstream HK07A
consumption boundary, together with `FOUNDATIONAL_PROOF_STANDARD.md`, `WORKER_REVIEW_PROTOCOL.md`
v1.6 and the exact-SHA execution protocol.

The direct accepted predecessor remains HK05 reviewed candidate
`23a9fd4373a803187cd9391b1459cd48975177f6`, independent PASS review `5257350871`, implementation
merge `ed65661680aea2a9be79f892c96aa42bf788a842` and DocSync merge
`2be48337721406eb77b6b66b31f7fc738d9ba08f`. `WORKER_PLAN.md` contains the mandatory predecessor
contract check from before implementation.

HK06A continues to consume, without duplicate proof, HK01 canonical route/discovery closure,
HK02/HK02A state/hash identity, HK03 read neutrality, HK04 sole commit authority/atomicity/CAS/
idempotency/change coverage and HK05 pre-commit validation. The independent FAIL did not contradict
those guarantees. The defect was solely in HK06A's new claim that its machine-readable normalized
request is truthful future replay evidence.

## Reviewer finding and causal repair

The failed candidate parsed/executed one mutation representation and then separately serialized a
journal `request`. Its request fingerprint came from the parsed mutation, while the journal body came
from `NormalizedOperationData`. A schema-valid serializer defect (for example another valid `typeId`)
could therefore leave persisted state, anchors, affected resources and fingerprint correct while the
journal carried false replay instructions.

Repair cycle 1 fixes the class, not the example:

- `WorldProvenanceIntegrity` independently interprets the journal-ready machine-readable request;
- it re-derives the complete v2 semantic fingerprint across all four operation kinds and requires it
  to equal the fingerprint of the parsed/executed request;
- journal idempotency key and expected revision/hash must independently agree with request identity
  and the base anchor;
- `WorldMutationProvenanceEntry` refuses construction if that binding fails, before the immutable
  `AuthoringState` publication;
- entry identity is independently recomputed from the bound request identity, transition anchors,
  sequence, schema/version and affected resources; a different but syntactically canonical 64-hex ID
  is rejected;
- focused self-attacks corrupt schema-valid replay-relevant fields across `put-object`,
  `remove-object`, `put-extension` and `remove-extension`, plus top-level identity/base fields, and
  require the production guard to turn RED;
- a separate test-owned oracle compares the complete journal request recursively against the request
  constructed independently before dispatch. Valid `typeId` and Base64 payload corruption turn this
  oracle RED, and a canonical-looking wrong entry ID turns an independent second-run identity oracle
  RED.

No replay execution was implemented. HK06C remains the owner of replay interpretation/compatibility.

## Complete baseline-to-candidate diff audit

The complete baseline→repair diff was inspected rather than only the last commits. Product changes
remain confined to the existing HK06A surfaces: authored anchors/runtime-observation stamp, versioned
journal/entry model, canonical read binding and transaction-boundary provenance integration. The
repair adds only the internal request/identity integrity guard at that provenance-entry boundary.

Focused tests/evidence now include the original journal, persisted-only, concurrency,
content-shape/authored-live checks plus the repair's semantic-corruption and independent-envelope
oracles. No new package or external authority was introduced.

The diff contains no semantic diff, snapshot export/import, journal replay, Unity serialization,
gameplay clock/scheduler/AI, deterministic simulation, GUI history browser, cloud persistence or Git
history as source of truth.

## Strict in-claim challenge

### Replay-envelope truthfulness

The repaired path was challenged against the exact false-green class from review and equivalent
variants. Changing valid object ID/type/container/reference data, extension owner/version/subject/
dependency/payload, remove-operation identities, expected revision/hash or idempotency key while
holding the accepted parsed fingerprint fixed makes provenance construction fail closed. The guard
uses the machine-readable envelope rather than private parsed operation objects, so it is capable of
detecting drift in the serializer it is checking.

The independent test-owned oracle additionally proves that a mutually wrong production serializer
and production guard cannot define the expected request by themselves: expected data is created in
the test before dispatch and deeply compared with public journal output. That oracle also uses a
second clean execution to establish expected deterministic entry identity instead of merely checking
hash syntax.

### Atomicity and predecessor composition

The guard runs while the accepted HK04 commit lock is held and before the new immutable
`AuthoringState` is assigned. Therefore an invariant failure cannot publish state without provenance
or provenance without state. No second writer, validator, CAS, receipt store or state model was
added. The existing concurrent-writer and non-persisting-path regressions remain GREEN.

### Public contract and downstream boundary

The public journal schema/version is unchanged: the repair strengthens the truthfulness invariant of
`arkus.authoring.journal-entry@1`; it does not redefine HK06A output after the fact or push format
repair into HK06C. `authoring.journal.read@1.0` remains canonical/discoverable through the accepted
composer and read-only attenuation path.

HK06B can therefore consume truthful history/anchors without taking ownership of request semantics,
and HK06C can consume the already-versioned HK06A journal rather than repairing or redefining it.

### Authored/live boundary

The repair does not touch the authored/live split. The bounded Potes observation surrogate remains
outside mutation authority, is stamped with the authored base and can change transient fields without
canonical revision/hash/history churn; the unsafe authority-granted surrogate still turns the
boundary oracle RED.

## Findings

`WORKER_PRE_REVIEW_FINDINGS_FIXED: 1` counts the concrete independent-Reviewer blocker that initiated
repair cycle 1: schema-valid normalized replay-envelope drift was not causally bound to the accepted
mutation. It is repaired at the provenance boundary with both a production fail-closed invariant and
an independent test-owned oracle.

No additional material in-claim blocker was found during the repair pre-review. No accepted
predecessor guarantee was reopened because no concrete evidence made one false or inapplicable.

Out-of-boundary items remain those already recorded in `RESIDUAL_RISK.md`: process durability,
snapshot/history policy, replay compatibility/interpretation, semantic diff, pagination and future
runtime-observation payload/simulation semantics.

## Proof budget and handoff readiness

The repair adds one bounded internal integrity guard plus three focused tests (two production-guard
self-attacks and one independent accepted-envelope audit). This is proportional to a concrete
Reviewer-proven false-green class and does not introduce a generalized replay verifier, event store,
second semantic registry, second state model or duplicate predecessor proof.

`PROOF_BUDGET_VERDICT: WITHIN_BUDGET`.

No known in-claim blocker remains. `WORKER_PRE_REVIEW: CLEAN` applies to the repaired content above.
After this evidence-only commit receives GREEN canonical exact-SHA verification, that exact SHA may
be recorded as Candidate/Frozen SHA, the branch may be frozen and the PR marked Ready for a fresh
independent Reviewer. This Worker does not issue the independent PASS/FAIL verdict.
