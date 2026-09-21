# WP-CITY-02 — Post-PASS DocSync

DOCSYNC_STATUS: **DOCSYNC_COMPLETE**  
MODE: **PROCESS_ONLY / DOCUMENTATION_ONLY**  
DATE: 2026-09-21

## Accepted result

- Frozen candidate SHA: `ddcb22d9a2dfcf68054db2b762fc7ff7aed68ae8`
- Independent Reviewer verdict: **PASS**
- Review: `#5266113192`
- PR: `#81`
- Merge commit: `1bdb7b6e914692493b17a9d2215d881dfe326cb0`

## Accepted repair history

Cycle 0 candidate `5ae61e0863bf1480e616275377306f5df940aeb5` failed independent review `#5265810937` because the CITY-00 Q6 capacity conclusion used `area / A/B handle count` plus interior-depth distribution even though programme handles do not encode footprint/frontage/open-space demand.

The accepted cycle-1 candidate replaces that false-green oracle with a coarse falsifiable programme-capacity bound in `Docs/evidence/WP-CITY-02/CAPACITY_SANITY.md`: every A/B place receives a role-based `T/S/M/L` spatial-demand envelope, upper bounds are charged per accepted dense part, hard ordinary/quiet reserve and a separate 15% uncommitted/circulation margin are preserved, and class-ceiling/programme-growth/part-cap/reserve-borrowing breaches explicitly reopen Q6. Exact parcel/site/shell composition remains downstream in CITY-05.

## DocSync actions

1. Marked `WP-CITY-02` COMPLETE and recorded the accepted candidate, review and merge.
2. Advanced the CITY spine to `CITY-00 ✅ -> CITY-01 ✅ -> CITY-02 ✅ -> CITY-05`.
3. Updated the root workpack index so `WP-CITY-05` is the next dependency-valid CITY workpack.
4. Preserved `Docs/production/CITY_LOCATION_PROGRAMME.md` and the accepted CITY-00/CITY-01 semantic owners exactly as reviewed; this DocSync adds no place, route, capacity class, parcel, interior or runtime semantics.
5. Reconciled the accepted residual boundary in `Docs/engineering/RESIDUAL_LEDGER.md`: exact parcel/frontage/shell composition, detailed interiors/discovery, retained-seed selection, realized traversal/reactive-density measurement and runtime social/product semantics remain downstream.

## Boundary

This DocSync is documentation-only. It does not reopen the accepted CITY-02 programme, change the 37-row place ledger, change A–D/S0–S4/I0–I3 assignments, alter the Q6 capacity arithmetic, parcelize CITY-05, design CITY-06 interiors, select CITY-03, authorize CITY-04 Unity work, or create Living World/runtime semantics.

The accepted Q6 condition remains fail-closed: if downstream composition discovers that a programmed place cannot truthfully fit within its accepted `T/S/M/L` ceiling, or would require consuming the hard ordinary/quiet reserve, Q6 must reopen rather than being hidden by DocSync.

## Next action

Next CITY workpack: `WP-CITY-05 — Streets, parcels + reusable building families`.

`WP-CITY-05` is dependency-valid after this DocSync but is not active until a human starts its Worker.

`DOCSYNC_COMPLETE`
