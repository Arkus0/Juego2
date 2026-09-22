# WP-DW-00 — foundational proof matrix

FOUNDATIONAL_PROOF_VERDICT: NOT_READY
UNRESOLVED_PROOF_OBLIGATIONS: 1
KNOWN_UNDETECTED_DEFECT_CLASSES: 0
TRUST_BOUNDARY: DW-00 owns the generic Design World authority/projection/rebuild/provenance seam and its use of accepted public H0 world-state/inspection surfaces; accepted source semantics, H0 correctness outside the consumed seam, .NET/Git/runner/hash infrastructure and downstream CITY/PA/CTX/Unity semantics are outside this claim.
PROOF_BUDGET_VERDICT: WITHIN_BUDGET

`NOT_READY` currently means only that the final exact-candidate canonical observation and strict exact-HEAD Worker pre-review have not yet closed. The product/proof obligations below are implemented and become READY only after those terminal checks are green.

## Independently defined universes / oracles

- **Neutral completeness universe:** `StaticDesignAuthorityUniverse` is constructed independently of `IDesignAuthorityReader`. `DesignWorldProjector.Build` must resolve every required id; omitting a reader definition while the universe remains complete throws `dw.provenance_missing`.
- **Projection coverage oracle:** validator compares independently supplied expected fact ids with actual projected fact ids; neither the projection nor its index can shrink the expected set.
- **Provenance oracle:** current authority bytes, unique anchor occurrence, full source digest and anchor digest are evaluated by the reader; an existing derived row cannot self-confirm trust.
- **H0 surface oracle:** projected H0 object ids and DW extension subject ids must exactly equal projected fact ids; reverse product dependency is checked independently from H0 source/project files by the canonical observer.
- **Representative product probe:** exact accepted CITY programme bytes are read directly; it is not the neutral completeness universe.

## Acceptance / proof-obligation matrix

| Proof obligation | Claim / trust-boundary scope | Completeness argument | Positive evidence | Causal negative control / defect injection | Result | Residual risk |
|---|---|---|---|---|---|---|
| Stable projected identity, schema/rule version and typed fact/relation envelope | DW-owned generic consumer types | every expected id is independently enumerated; values and relations are normalized deterministically | `NeutralFixtureRebuildsDeterministicallyThroughPublicH0InspectionSurface`; representative probe tests | identity mismatch / relation target outside universe are fail-closed in projector; omission test exercises missing expected id | IMPLEMENTED | Full domain-specific schemas are downstream. |
| Provenance detects missing authority | DW source-adapter/projection seam | independent universe remains complete while reader entry is omitted | normal neutral + representative builds | `IndependentUniverseMakesMaterialProjectionOmissionCausallyRed` | GREEN in focused test once exact observation closes | No full Markdown discovery claim. |
| Provenance detects stale authority | DW provenance trust decision | validation resolves each projected anchor against current authority bytes, not cached index | normal rebuild validates | `StaleAuthorityBytesInvalidatePreviouslyProjectedProvenance` | GREEN in focused test once exact observation closes | Whole-document digest is conservative. |
| Provenance detects ambiguous authority | DW provenance trust decision | exact source anchor must occur exactly once | unique anchors validate | `AmbiguousAuthorityAnchorCannotValidateAsTrustworthy`; `DuplicateRepresentativeAnchorFailsClosedRatherThanTrustingAnIndexRow` | GREEN in focused test once exact observation closes | Source formats requiring richer anchors can add provider-specific readers. |
| Identical authority + projection version rebuilds equally | DW derived-state/rebuild claim | deterministic sorted ids/fields/relations + length-prefixed normalization + canonical H0 serializer | neutral build twice compares normalized text, digest, canonical H0 bytes and empty diff; representative rebuild/diff test repeats on real source | hidden-derived-state case rebuilds with new reader/projector and no cached projection | IMPLEMENTED | Trusted hash/runtime behavior remains outside claim. |
| Material authority/projection-rule change changes derived result | DW causal-change claim | fact fingerprints and projection digest include typed values/relations/provenance and version | normal equal rebuild | `MaterialAuthorityChangeChangesNormalizedProjectionAndDiff`; representative rule-only field change changes digest + fact diff | IMPLEMENTED | Semantic wisdom of a source change is not judged. |
| Cached state from old projection rule/version cannot validate | DW staleness claim | expected projection version is supplied independently at validation | same-version validation | `ReusingDerivedStateAcrossProjectionRuleVersionIsCausallyRed` | IMPLEMENTED | Migration policy is future work. |
| Derived DW state is disposable and cannot become authority | DW authority boundary | rebuild requires only source reader + independent universe + version; no index is input | normal build and representative probe | `DeletingAllDerivedStateStillRebuildsFromAuthorityOnly` | IMPLEMENTED | Persistence/index implementation is not required in DW-00. |
| Authority source bytes unchanged by projection/query/validation | DW source-authority preservation | reader is immutable over supplied text; product probe also compares repository file bytes before/after | both representative probe tests | any source mutation would fail byte equality | IMPLEMENTED | External filesystem corruption is trusted-base/out of boundary. |
| No CITY/PA semantics enter H0; dependency remains downstream | H0 boundary consumed by DW | DesignWorld project references only `Arkus.Game.World`; canonical observer scans H0 dirs for reverse DesignWorld reference and generic consumer source for domain-specific implementation vocabulary | `DesignWorldIsAConsumerAndCannotBecomeAH0Dependency` | observer fails if reverse H0 dependency or CITY/PA-specific generic implementation vocabulary is introduced | IMPLEMENTED | Domain payload values may naturally contain source-domain text; H0 semantics do not. |
| Same public surface is usable by later consumers | DW-to-H0 public seam | derived facts are materialized only as public `WorldState` objects/references/extensions; query uses `WorldInspectionService` | neutral `Summary`/`QueryObjects`, representative `QueryReferences` | validator fails object/extension identity-surface mismatch | IMPLEMENTED | Mutation of source authority is intentionally not exposed through DW. |
| Representative approved Juego2 shape fits owned boundary | foundational content-shape probe | exact accepted CITY row, bounded to identity/granularity/provenance; separate from neutral universe proof | `CONTENT_SHAPE_PROBE.md` + two representative tests | duplicate real-shape anchor fails; projection-rule change changes representative diff | IMPLEMENTED | CITY semantic/invariant truth stays DW-01/02. |
| Exact-SHA canonical execution observes locked restore, build, focused tests, boundary guards and full regression | final execution evidence | `scripts/arkus-observe-exact-sha.sh` routes `WP-DW-00` to `scripts/dw00-observe-exact-sha.sh` on exact clean SHA | pending final workflow receipt | exact-SHA mismatch/dirty state/locked graph/boundary/test failure makes command nonzero | PENDING TERMINAL CHECK | This is the one unresolved proof obligation. |

## Named causal negative-conformance classes

1. **Self-shrinking projection completeness:** keep independent universe complete, omit material reader fact -> `dw.provenance_missing` RED.
2. **Stale source anchor/index row:** project old bytes, validate against changed bytes -> `dw.provenance_stale` RED.
3. **Ambiguous authority location:** duplicate anchor -> `dw.provenance_ambiguous` RED.
4. **Stale cached derived output after rule-version change:** validate old projection with new expected version -> `dw.projection_version_stale` RED.
5. **Hidden generated truth:** discard all derived output and instantiate a fresh reader/projector -> equal canonical rebuild required.
6. **Domain leakage / authority inversion through dependency graph:** reverse H0 dependency or CITY/PA-specific generic implementation vocabulary -> canonical observer RED.
7. **Material change invisibility:** authority/projection value change -> projection digest and fact diff must change.

## Final closure rule

After all repository/evidence bytes are complete, the Worker stops writers, reads the exact final HEAD, runs/observes canonical exact-SHA validation, inspects the complete baseline→candidate diff and performs the strict Worker pre-review. Only then may this matrix be changed to `READY / 0 / 0` and the final CLEAN readiness record be persisted externally as the CTX-03 GitHub PR comment.
