# WP-H2F-01 — decision spikes and final stack selection

Status: **Worker evidence, Draft + ACTIVE**, not frozen. Mode: HYBRID. Real Unity 6000.3.24f1 evidence was produced on the owner's workstation in disposable non-keeper workspaces. Baseline `main` `a47f879c`. Contract: `Docs/workpacks/H2F/WP-H2F-01.md`, executed under the owner's ART-01 sequencing amendment (see `PREDECESSOR_CONTRACT_CHECK.md`).

| File | Content |
|---|---|
| `PREDECESSOR_CONTRACT_CHECK.md` | Inherited/owned guarantees, the ART-01 sequencing precondition, reopen conditions |
| `WORKER_PLAN.md` | Spike execution contract and isolation boundary |
| `SPIKE_RESULTS.md` | S01–S07 methods, measurements and verdicts, plus the owner-supplied GC2 candidate |
| `DISPOSITION_MATRIX.csv` | Final disposition for all 56 H2F-00 candidates + GC2 Core (57 rows, no `SPIKE_REQUIRED` left) |
| `BASELINE_INTENT.md` | One coherent recommended baseline + requirements and lifecycle hints for H2F-02 |
| `SPIKE_LEDGER.md` | Workspaces, rebuild recipe, exercised package lock, residue isolation |
| `ACQUISITION_AND_POLICY.md` | Manual/licensed inputs, blockers vs technical results, dependency-policy questions |
| `OWNER_JUDGEMENT.md` | What the owner saw, the verbatim delegation, and how it is applied |
| `SPIKE_INPUT_LOCK.json` | Hash lock for vault inputs beyond ART-01's lock |
| `spike_project/` | Reproducible disposable spike recipe (no vendor bytes) |
| `results/`, `captures/owner/`, `captures/negative/` | Measurements, owner review sheets, negative controls |

## PASS conditions → evidence

| WP condition | Evidence |
|---|---|
| Every H2F-00 `SPIKE_REQUIRED` item affecting CITY-07/H2 has a resolved decision | `DISPOSITION_MATRIX.csv`: S01–S07 rows resolved by real spikes. S08 items (Astra presets, Unity MCP) are resolved as selection/deferral with proof assigned to H2F-03 S08, as accepted in H2F-00. |
| Render/worldbuilding/character choices exercised on representative real assets | The ART-01 structural route rebuilt digest-identically (`results/s01_reproduction.json`), plus owner-vault Quaternius Source; no vendor demos |
| No unexplained overlapping tools for one role | Matrix: ProBuilder/Mixamo/legacy input REDUNDANT; Terrain Tools, Starter Assets and GC2 DEFER with stated reasons; one choice per role in `BASELINE_INTENT.md` |
| Quaternius Source capabilities considered before external tooling | Source URP Shader Graph (S01), Source fences/stairs (S02), UAL1/UAL2 before Mixamo (S07), Source chair for sit anchors |
| Actual Quaternius character animation/retarget fit proven | S07: explicit mapping, pose metrics, root motion, IK contact on the ART clothed citizen |
| Visual decisions receive owner/art-direction inspection | `OWNER_JUDGEMENT.md`: owner inspected the sheets and delegated; choices are reversible defaults re-checked at ART-01 Phase B / CITY-07 |
| Selected tools remain downstream helpers, not canonical authorities | Splines/Terrain/scatter/NavMesh outputs are generated realizations; GC2's parallel runtime was the deciding reason to defer it |
| No keeper CITY-07 content claimed or frozen | All fixtures live in disposable workspaces outside the repository |

## Negative gates checked

- **Not selected from documentation alone.** Every ADOPT_NOW row cites a spike or an unchanged native H2F-00 closure.
- **Not everything installed.** Terrain Tools and ProBuilder were installed only in the spike and not selected.
- **No vendor demo judged.** Every judgement used ART/Quaternius representative content.
- **No keeper geometry mutated.**
- **No feature-rich duplicate selected.** GC2 was deferred despite smoother defaults.

## Freeze preconditions still open

- the ART sequencing amendment merged on `main`;
- ART-01 `PREFOUNDATION_INPUT` published;
- H2F-00 DocSync (#253) merged;
- the owner's answer on GC2 (keep deferred or promote);
- exact-SHA preflight and the strict Worker pre-review.
