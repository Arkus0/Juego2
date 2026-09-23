# Pre-result amendment 01

The first published `PRECALIBRATION_FREEZE.json` is preserved in Git commit `091f4e9e813736c7e503e44d88a100b4d5ac1d6d`. Before **any** calibration or acceptance route execution, static inspection found two catalogue mistakes:

1. `A-CITY-02` mislabelled the authoritative X6 edge ledger as §4; the row is §5.3 of the same pinned CITY-01 file. Only the section locator changes.
2. reserve `A-CITY-04` repeated calibration `C-CITY-02`'s arrival-stop semantics. Reserve `A-CITY-04` now covers `loc.ribera.workshop`, an unused source row. The predeclared first-three-per-domain selection is unaffected.

This amend-and-republish precedes all DW-04 model execution. The new commit is the effective `PRECALIBRATION_FREEZE` identity for every later calibration and selection check. The old commit remains visible as superseded history, never an additional task pool or fallback selection. No calibration outcome informed this change.
