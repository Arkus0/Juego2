# WP-DW-05 — Worker plan

Baseline: `18d4aa33be526c65a53796aa305a56fe8df15d12`

## Decision posture

This workpack is a falsification stress. The Worker will not optimize for PASS and will not repair a demonstrated generic public-seam contradiction inside DW-05. Conversely, fixture/schema/test code and probe-specific glue are allowed and are not treated as architectural failure merely because they are new.

## Probe shape

Use a tiny synthetic **observatory calibration graph**, deliberately unrelated to CITY programme/content and PA research/disposition shapes:

- `observatory.north` (`observatory`) with integer elevation and boolean remote fields;
- `detector.spectro` (`detector`) with decimal wavelength-min/max fields;
- `calibration.lamp-a` (`calibration-source`) with string mode field;
- relations `houses` and `calibrated-by` connect the three facts.

The source is an in-test frozen authority text. Probe-specific definitions/universe/query glue live only in tests. The public production seam remains `StaticDesignAuthorityUniverse` + `AnchoredTextAuthorityReader` + `DesignWorldProjector` + public `DesignWorldProjection`/`WorldState` + `DesignWorldProjectionValidator`/`DesignWorldProjectionDiff`.

## Proof slices

1. **Positive neutral projection/query/rebuild:** build from the frozen neutral authority, query by public typed fact/field/relation surfaces, validate provenance, rebuild, compare digest/normalization/diff and verify authority bytes unchanged.
2. **Causal invalid mutation:** remove a required source anchor/relation-bearing fact while keeping the independent neutral universe fixed; require RED through the existing generic authority/projection/validation path. Also exercise stale authority bytes against a previously built projection and require provenance RED.
3. **Leakage/dependency audit:** inspect generic DW contract/projection files, direct project references and the accepted H0 `Arkus.Game.World` dependency closure for CITY/PA vocabulary or references. The checker itself is test-owned; an injected synthetic forbidden dependency/reference must make it RED.
4. **Residual reconciliation:** independently inventory predecessor residual statements/evidence, reconcile them to a machine-readable closure manifest with exactly one classification and causal owner, and prove omission/misclassification controls RED.
5. **H0 semantic-change guard:** pin the baseline blob identities for the generic DW public seam and H0 world project consumed by the probe; candidate work may add tests/evidence only. Any candidate mutation to those accepted files is out of plan unless a separately accepted reopen/delta exists.
6. **H2 input report:** record exercised capabilities, bounded consequences, optional opportunities and rejected/premature ideas. Preserve design↔Unity drift, generated art briefs, content-catalogue coverage, replay QA and narrative knowledge checks as downstream candidates only.

## Stop/adverse conditions

Stop repairing and record a material limitation/reopen proposal if the neutral shape cannot pass without changing/widening the generic public contract, adding a new shared abstraction/capability required for the probe, changing H0 semantics, importing CITY/PA knowledge or weakening provenance/rebuild. Implementation defects that contradict already-promised behavior are classified separately and may be repaired only when the repair is within DW-05 allowed scope and does not cross those boundaries.

## Expected write set

- `tests/Arkus.Harness.Tests/Dw05GenericBoundaryStressTests.cs`
- `Docs/evidence/WP-DW-05/*`

No `src/` file, accepted predecessor contract, CITY/PA source/oracle or H0 semantic file is planned to change.
