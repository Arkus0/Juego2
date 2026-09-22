# WP-PA-04 — Capsule DocSync repair evidence

Date: 2026-09-22  
Class: PROCESS_ONLY corrective DocSync evidence  
Source DocSync: PR `#131` / merge `58e417f357caac387f84333766f61c2fb4d9f461`

## Trigger

PA-05 Worker PR `#132`, exact candidate `18d70fe95eab311c301659ace49135ebb4f96796`, ran Context Capsule Validation `35757146872` / job `106845659910`.

The checker self-test and independent mutation controls passed, but the canonical accepted-chain audit failed with:

```text
context-capsule: FAIL: accepted PA result-chain coverage gap: WP-PA-04; missing capsule forces authoritative source reconstruction and DocSync cannot claim capsule coverage complete
```

This is a predecessor/DocSync defect, not a PA-05 product-semantic failure.

## Root cause

PR #131 correctly marked `WP-PA-04` COMPLETE after its independent PASS and merge, but did not extend the accepted CTX-02 capsule chain.

The accepted CTX-02 policy is fail-closed:

- every COMPLETE `WP-PA-NN` independently enters the accepted PA chain universe;
- its canonical `PA-NN.md` result must have one indexed capsule;
- every accepted PA capsule requires a checker-owned disposition selector so the capsule cannot define its own completeness oracle;
- missing capsule/selector means reconstruct from authoritative sources and capsule coverage is not COMPLETE.

Therefore the `DOCSYNC_COMPLETE` claim from PR #131 was incomplete specifically for this navigation projection surface.

## Corrective scope

This repair changes only:

1. `Docs/engineering/context-capsules/WP-PA-04.json`
   - exact accepted identity from `WP-PA-04`;
   - exact canonical PA-04 result blob binding;
   - exported boundary guarantees/exclusions as navigation only;
   - complete structured coverage of `## 11. Juego2 disposition table`;
   - no semantic authority granted by the capsule.
2. `Docs/engineering/context-capsules/index.json`
   - add the canonical `WP-PA-04` entry.
3. `scripts/context-capsule-check.py`
   - add the checker-owned PA-04 selector:
     - section `## 11. Juego2 disposition table`;
     - key column `0`;
     - status column `1`.

No PA-04 research semantics, workpack acceptance bytes, runtime code, Unity code, PA-05 candidate bytes, H0/H1/CITY/DW semantics or frozen plans are changed.

## Exact authority bindings

Accepted PA-04 identity:

- reviewed candidate: `5d38ea38b983cd5227f57afa1d880d24746f9249`;
- independent PASS review: `#5280879115`;
- research merge: `d6041b719292f24c4481dea28727e2cfd5f7ed5b`.

Bound source bytes at repair baseline:

- `Docs/workpacks/PA/WP-PA-04.md` blob `16d277bc931c29d4114a8f20d5b70c90d224cdcc`;
- `Docs/research/living-world/results/PA-04.md` blob `d065241d5dd4ed663dd686fa288e6d8a43e1dc46`.

## Required validation

Before this repair may unblock PA-05:

- `python3 scripts/context-capsule-check.py --self-test` must PASS;
- independent capsule mutation/omission/semantic controls must PASS;
- `python3 scripts/context-capsule-check.py --audit-index --repo-root .` must discover PA-01..04 and report COMPLETE coverage;
- normal repository process checks must be GREEN;
- a fresh independent Reviewer must review the exact repair candidate because the checker-owned selector is new process authority.

The repair is not allowed to auto-accept PA-05 or substitute for PA-05's later independent semantic review.

## PA-05 effect

PA-05 PR #132 remains Draft and unfrozen while this defect is open. After an independently reviewed repair is merged, the PA-05 Worker must reconcile to the new `main`, update its predecessor/baseline evidence, rerun exact-HEAD pre-review and only then freeze a PA-05 candidate for independent review.
