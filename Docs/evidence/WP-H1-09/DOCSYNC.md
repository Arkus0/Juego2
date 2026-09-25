# DOCSYNC — WP-H1-09

Status: DOCSYNC_COMPLETE
Workpack: `WP-H1-09 — Drift reconciliation + Unity import proposals`

## Accepted implementation

- Frozen candidate: `8f2103c037d08416d70687d4fb08b3d3c62bceac`
- Canonical implementation PR: `#220`
- Independent PASS review: `#5322464433`
- Implementation merge: `9574f80aa07effc3022a3bb100a516d107965a19`
- Arkus Main Safety: run `36185972420` GREEN
- H1-07 Unity Validation: run `36185972487` GREEN
- H1-08 Unity Validation: run `36185972797` GREEN
- H1-09 Unity Validation: run `36185972628` GREEN
- Arkus Candidate Validation: final rerun `36186462669` GREEN

## Accepted closure

H1-09 now owns the accepted deterministic managed-scope drift oracle, canonical-authoritative rematerialization direction, and allowlisted Unity-edit -> canonical-mutation proposal compiler while consuming H0 commit/CAS/recovery/provenance authority rather than duplicating it.

The final repair closes the effective-drift false-parity class for unsupported managed edits and stable-identity referenced-content drift. A positive, restorative Unity control mutates accepted referenced content while preserving path/GUID/local-file-id, requires `projection.component-reference-content-drift` plus `projection.reconciliation-node-unreadable`, verifies ambiguous/no-parity through the public drift capability, blocks `import-proposal` without a mutation request, restores accepted bytes, and proves clean parity afterward.

The composed valid path is also accepted end to end: effective Unity edit -> public `unity.host.projection.import-proposal` -> ordinary H0 stale recovery/replan -> plan/dry-run/apply -> canonical rebuild/rematerialization -> parity. Valid `drift` and valid `import-proposal` outcomes are equivalent through reference JSONL and MCP.

The authority boundary remains unchanged: canonical -> Unity is authoritative; Unity -> canonical is only an explicit proposal; accepted commit, stale recovery and provenance remain H0-owned. H1-04, H1-05, H1-06, H1-07, H1-08 and HK08B are not reopened by this DocSync.

## Spine consequence

`WP-H1-10 — Project checkpoint + clean Unity reconstruction` is now dependency-valid but remains `PLANNED / NOT_STARTED` until explicitly started.

DOCSYNC_COMPLETE
