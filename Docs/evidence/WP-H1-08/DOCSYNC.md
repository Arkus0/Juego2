# DOCSYNC — WP-H1-08

Status: DOCSYNC_COMPLETE
Workpack: `WP-H1-08 — Unity validation + repairable diagnostics`

## Accepted implementation

- Frozen candidate: `8b7ea7b9ad74d67c5b300180b79833fe89350f4d`
- Canonical implementation PR: `#216`
- Independent PASS review: `#5319609389`
- Implementation merge: `f22fa99e4427849b8a7202bf24bbae83fb90ee83`
- Arkus Main Safety: run `36152105677` GREEN
- H1-07 Unity Validation: run `36152105495` GREEN
- H1-08 Unity Validation: run `36152105554` GREEN
- Arkus Candidate Validation: run `36155260107` GREEN

## Accepted closure

H1-08 now owns the accepted finite Unity validation inventory, deterministic structured aggregation, truthful proposed/current/effective phase separation, publication blocking, and the H1-owned finite-transform invariant while consuming the accepted HK05 and H1-04/05/06/07 causal authorities.

The final repair closed the remaining expected-invalidity lifecycle hole structurally: canonical `projection.*` invalidity reached during effective materialization is converted into an `H1ValidationResult` at the registered post-materialization boundary rather than escaping with `validation = null`. The effective negative `Materialize_InvalidActiveManifest_IsStructuredAndDoesNotStageOrPublish` proves materialize A -> corrupt active `current.json` -> materialize valid B returns structured `projection.manifest-invalid`, does not publish a replacement manifest, and does not stage a new managed generation.

Earlier accepted repairs remain part of the frozen candidate: proposed/materialize structural preconditions are aligned; current validation derives effective identity/drift from active manifest observation; malformed/missing active state yields structured validation; and the non-finite negative exercises the actual finite predicate rather than a direct diagnostic sentinel.

No H1-04, H1-05, H1-06 or H1-07 guarantee is reopened by this DocSync.

## Spine consequence

`WP-H1-09 — Drift reconciliation + Unity import proposals` is now dependency-valid but remains `PLANNED / NOT_STARTED` until explicitly started.

H1-08 also satisfies the H1-side prerequisite for `WP-CITY-04`; CITY-04 still requires its own accepted CITY predecessor state and explicit start.

DOCSYNC_COMPLETE
