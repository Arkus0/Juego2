# WP-HK-02A negative-conformance matrix

All controls are repository-local software conformance tests in `Hk02AObjectScopedExtensionTests` or inherited exact-surface tests extended by this candidate.

| Required defect class | Causal control | Why it turns RED for the intended defect | Result on observed SHA `f3c9a8f…` |
|---|---|---|---|
| serializer/hash omits `SubjectId` | `CanonicalCodecCoversSubjectAndDependenciesButIgnoresInputOrdering` | two otherwise equal valid states differ only by subject; equal hashes expose omission | GREEN |
| serializer/hash omits dependency | same test | two otherwise equal valid states differ only by dependency target; equal hashes expose omission | GREEN |
| uniqueness still owner/version only | `CompositeIdentityAllowsSameOwnerVersionOnDifferentSubjects` | a valid two-subject state would be rejected or collapse if subject were absent from identity | GREEN |
| dangling subject accepted | `InvalidSubjectDependencyAndDuplicateEdgesFailClosed` plus subject case of `RemovingObjectReferencedByExtensionCannotReachApply` | direct construction and transactional candidate validation both require the subject object | GREEN |
| dangling/duplicate dependency accepted | same validation test plus dependency case of removal theory | missing targets and duplicate `(kind,target)` edges must throw; target deletion must not commit | GREEN |
| HK03 reconstruction omits subject/dependencies | `InspectionExposesCompositeAddressAndReconstructsExactHash` and inherited `PublicInspectionCanReconstructExactCanonicalHash` | public descriptor/read data is used to rebuild state; any omitted semantic field changes the exact hash | GREEN |
| HK04 extension key/remove ignores subject | `MutationAddressesSubjectAndReportsDependencyEffects` | two same-owner/version subjects are put, one is removed, and the other must remain | GREEN |
| HK04 change coverage omits dependency effects | `IndependentCoverageOracleDetectsOmittedDependencyEffects` | a real dependency replacement is compared with an empty declared change set; field/add/remove mismatches must appear | GREEN |
| dependency input ordering leaks into identity/hash | corrected reverse-input branch in codec test | dependency order is reversed before immutable constructor copy; canonical bytes/hash must remain equal | GREEN |
| public semantic identity grows outside proof inventory | extended HK02/HK03 reflection inventories | adding/changing public identity inputs/properties without updating proof makes exact property/constructor sets fail | GREEN |

The focused class is executed by `scripts/hk02a-observe-exact-sha.sh`; the complete regression runs immediately afterward. Final evidence-bearing SHA execution is required before freeze.

## Repair cycle 1 — reviewer-found false-green classes

| Defect class | Effective control and outcome on implementation `2cb7a3d` | Failure condition |
|---|---|---|
| Global and object subject `"-"` fingerprint collide for put-extension | `GlobalAndDashSubjectHaveDistinctIdempotencyFingerprints(false)`, Actions `35456331653` GREEN | Same-key replay instead of `world.change.idempotency_conflict`, or a second commit, turns RED. |
| Global and object subject `"-"` fingerprint collide for remove-extension | `GlobalAndDashSubjectHaveDistinctIdempotencyFingerprints(true)`, same GREEN run | Same-key replay, unintended second remove, or failure to remove scoped resource with a distinct key turns RED. |
| HK03 extension dependency pages silently omit page two | `SecondDependencyPageIsRequiredForExactCanonicalReconstruction`, same GREEN run | Missing `nextDependencyOffset`, second page, any of 101 edges, or exact reconstructed hash turns RED. A 100-edge-only reconstructed world is explicitly compared and has a different hash. |

The pre-repair frozen candidate `9d0dc373...` was reviewed FAIL despite GREEN historical tests; these controls are owned by the repaired candidate. Do not reinterpret earlier 8/8 or 95/95 as covering these classes. The existing bounded object-reference test remains inherited rather than copied.
