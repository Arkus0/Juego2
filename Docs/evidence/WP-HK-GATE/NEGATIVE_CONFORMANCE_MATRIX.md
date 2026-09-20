# WP-HK-GATE negative conformance matrix

NEGATIVE_CONTROL_UNIVERSE: 13
GATE_OWNED_RED_CONTROLS: 1
INHERITED_HK10_RED_CONTROLS: 12
KNOWN_UNRESOLVED_FALSE_GREEN_CONTROLS: 0

## Gate-owned causal control

| # | Defect class | Seed | Required RED oracle |
|---|---|---|---|
| G1 | mandatory GATE execution stage silently omitted | in a disposable exact-candidate worktree, remove the actual unfiltered stage-14 `dotnet test` execution command from `scripts/hkgate-observe-exact-sha.sh` while leaving `GateStepUniverse` and its `14-headless-full-validation` label intact | the same `scripts/hkgate-proof-infrastructure-check.sh` oracle used by normal observation must RED specifically with `missing-or-ambiguous-headless-full-validation-execution matches=0`; mutation/setup/infrastructure failure is rejected as non-causal |

Runner: `scripts/hkgate-negative-conformance.sh`.

Normal-path wiring: `scripts/hkgate-observe-exact-sha.sh` invokes `scripts/hkgate-proof-infrastructure-check.sh` before running the deterministic gate and later executes the checked unfiltered headless validation command. The checker identifies the real executable command by its command shape, not by the declarative stage label.

G1 therefore attacks the defect it claims: the execution can disappear while the 14-stage declaration remains unchanged, and the candidate must still go RED. This prevents the self-shrinking proof described by the Reviewer rather than merely protecting a list label.

## Inherited HK10 causal controls re-executed by GATE

GATE runs the accepted `scripts/hk10-negative-conformance.sh` unchanged over the exact candidate. Its twelve seeded defect classes remain:

1. protocol discovery route/definition drift;
2. canonical state/hash order drift;
3. inspection ordering drift;
4. transaction/idempotency receipt bypass;
5. validation suppression;
6. provenance/replay anchor weakening;
7. reference-host frame-ceiling drift;
8. HK08A accepted batch-shape drift;
9. HK08B stale-recovery anchor drift;
10. HK09A elevated-capability admission;
11. HK09B publication-interruption bypass;
12. HK09B bounded-session envelope drift.

Each mutation occurs in a disposable detached worktree, must produce a test RED for the intended causal reason, then restores the exact candidate. A compile/tool failure cannot count as proof.

## Scope / interpretation

GATE does not claim thirteen controls exhaust every conceivable software defect. The bounded universe is the accepted HK10 causal closure plus one new GATE-owned defect class: silent omission of a mandatory readiness execution stage. Other GATE obligations are directly observable integration claims (public transport execution, exact cross-transport semantics, replay/hash equality, content-shape representability and the external AI-agent trial) rather than new product semantics that justify duplicating predecessor mutation machinery.
