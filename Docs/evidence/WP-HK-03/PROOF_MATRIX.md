# WP-HK-03 foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: READY
UNRESOLVED_PROOF_OBLIGATIONS: 0
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: Docs/evidence/WP-HK-03/RESIDUAL_RISK.md
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

## Central claim

For the finite canonical state model accepted by HK02, the HK03 six-command `world.*@1.0` surface is sufficient to reconstruct all current authorable semantics through the single HK01 canonical contract inventory while keeping every large output path bounded, deterministic and revision/hash anchored. No mutation or second semantic state model is introduced.

Pre-freeze implementation observation:

- candidate SHA `05e25995d5c0ad91954bded22ab65e7c49097ea2`
- GitHub Actions run `35438474195`
- Release build: 0 warnings / 0 errors
- `Hk03InspectionTests`: 7/7 GREEN
- `Hk03SelfAttackTests`: 7/7 GREEN
- full regression: 70/70 GREEN

The eventual frozen candidate is re-executed by `scripts/hk03-verify-exact-sha.sh`; the exact-SHA Actions check is PR metadata and therefore cannot be embedded into the same immutable candidate without changing its SHA.

## Proof obligations

| Proof obligation | Claim/trust-boundary scope | Completeness argument | Positive evidence | Negative control / attack | Result | Residual risk |
|---|---|---|---|---|---|---|
| public reads cover summary, lookup, relationships and filtered reconstruction | current HK02 semantic state only | independent HK02 property universe maps to summary, object pages, relationship pages, extension descriptors/chunks in `COMPLETENESS_MAP.md` | `SummaryAndObjectLookupBindEverySuccessToRevisionAndHash`, `ObjectQueryIsFilteredProjectedPaginatedAndCanonicalOrder`, `ReferenceQueryProvidesDeterministicRelationshipSurface`, extension reconstruction test | `HiddenAuthorableStateMutantBreaksIndependentReadReconstructionHash`; reflection surface must match independently enumerated expected state properties | PASS | future state fields deliberately turn proof red until mapped |
| query/filter language is explicit, typed, bounded and non-executable | six HK03 schemas + semantic selector validation | only finite object/reference/extension filter records, projection enum, stable-token identifiers, integer limits/cursors; no expression/path/evaluator input exists | canonical discovery schema conformance; filtered query positive | `SelectorCannotEscapeDeclaredStableTokenGrammar`; `UnboundedDeclaredLimitsFailClosed` | PASS | current generic schema vocabulary lacks numeric min/max facets; runtime semantic validation owns those bounds |
| deterministic pagination/cursors stable on unchanged revision | object/reference/extension query routes | each result set is canonically sorted; cursor binds command + normalized selector/projection/limit fingerprint + revision/hash + offset | repeated read/cursor equality; canonical-order positive | `NondeterministicQueryOrderMutantCannotLeakInputOrdering`; `StaleRevisionAndCursorAreRejectedAfterSourceAdvances` | PASS | cursor is a continuation token, not an authorization credential |
| every successful state description identifies revision/hash | all six public reads | every success is produced through one `Success(snapshot, payload)` helper adding `worldId/schemaVersion/revision/hash`; summary provides the initial anchor | summary/object/read repeatability positives | stale revision/hash/cursor controls prove the anchor cannot silently describe a different state | PASS | malformed requests rejected before a successful state description do not claim to describe a world |
| missing resources, invalid selectors and stale cursors are structured | state-dependent HK03 handlers | all semantic failures are canonical `StructuredError`; generic request-schema failures remain HK01 structured contract errors | missing object + stale revision positive assertions | invalid selector, invalid limit, stale cursor causal controls | PASS | none inside claim |
| no authorable state needed for safe later mutation is hidden | current accepted HK02 public semantic surface | reconstruction uses only public discovered reads, then canonical HK02 hash equality acts as effective semantic oracle; reflection prevents self-shrinking surface | full micro-world public-read reconstruction | hidden extension mutant changes hash; public-property universe growth turns oracle red | PASS | future semantic model additions require explicit proof update |
| large results are bounded without second truth | six response paths | summary scalar; object get fixed row; object query <=100 fixed rows; references <=100 rows/page; extensions <=100 descriptors/page; payload <=768 bytes/chunk; all project canonical HK02 data | pagination/projection/chunk positives | 130-edge nested-output attack + declared-limit attacks | PASS | query evaluation may scan/sort full in-memory state; output, not asymptotic CPU, is HK03 claim |
| reads are side-effect free and repeatable | Authoring inspection service + canonical definitions | handlers delegate pure reads; definitions declare read-only/deterministic/idempotent; canonical state hash remains unchanged across repeated reads | `ReadsRemainSideEffectFreeAndRepeatableAgainstSameState` | reorder/stale controls challenge hidden mutable/order context | PASS | multi-call snapshot lease belongs to later transaction work |
| discovered public reads cannot silently escape canonical schemas | production runtime route universe | HK01 `RouteUniverse` reflects concrete handler attributes independently of HK03 definition list; conformance compares routes, definitions, dispatcher/discovery | `CanonicalDiscoveryPublishesAllWorldReadsWithSchemas`; expanded HK01 route-universe regression | `EveryEffectiveWorldReadIsDiscoveredAndMissingSchemaMutantTurnsRed`; initial CI actually failed when six routes existed outside old composition | PASS | trusted .NET reflection/effective assembly behavior per foundational standard |
| forbidden scope absent | candidate diff from HK03 baseline | changed product files are Authoring inspection + Runtime canonical bindings/composition; no mutation/apply, Unity, gameplay or NL evaluator added | baseline-to-candidate changed-file audit | Worker pre-review scope audit | PASS | none |

## Independent/effective universes

Two non-circular universes protect the completeness claims:

1. **State universe** — reflection over the accepted HK02 public state-bearing property surface is compared with an explicit expected semantic inventory. It does not derive completeness from HK03 command definitions.
2. **Public route universe** — HK01 `RouteUniverse` reflects effective concrete `ICanonicalCapabilityHandler` routes from production assemblies independently of the definition registry and `system.describe`. The canonical conformance oracle then requires route/definition/discovery agreement.

The reconstruction hash is an evaluated semantic oracle over the accepted HK02 canonical codec. Thus deleting an HK03 mapping while leaving state present, or adding an effective public handler without its schema/definition, cannot make the corresponding proof obligation disappear with the object.

## Proof-budget reconciliation

HK03 adds no third-party dependency and no general-purpose proof framework. The support machinery is limited to:

- one independent state-surface/reconstruction oracle for the central completeness claim;
- the existing HK01 independent route-universe oracle, updated rather than bypassed;
- seven causal controls covering the six mandatory WP defect classes plus the concretely observed nested-relationship unbounded-path variant;
- one exact-SHA observation/verification wrapper following the established HK sequence.

The only material mid-implementation proof expansion accompanied a real product correction: removal of the unbounded nested-reference object response and migration of relationship output to the paginated canonical relationship route. There have not been two consecutive proof-only expansion cycles. The proof remains simpler than the behavior it protects and is proportionate to a foundational completeness claim.

## Reconciliation verdict

All acceptance obligations are PASS inside the stated finite trust boundary. No current semantic inventory entry is unmapped, no effective world route lacks canonical schema/definition coverage, every mandatory causal defect class is exercised, and no known in-boundary false-green class remains undetected.
