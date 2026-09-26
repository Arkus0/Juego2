# ART-01 neutral/massing checkpoint — 2026-09-26

> **Superseded (2026-09-26) by `STRUCTURAL_CHECKPOINT.md`.** Retained as history of Codex's `290c487` iteration. The same structural audit, run on that geometry, recorded 569/1449 failures (`UNITY_STRUCTURAL_AUDIT_BASELINE_290c487.json`). These include door-module base voids, 0.13–0.22 m corner notches, a 0.2 m eave slit at gable ends, 0.17 m recessed gables, shutter overhang/clash, a buried drain, invisible water, route narrowing and floating plinths/kerbs. All of them are resolved in the current candidate. The images in `massing_checkpoint/` show the superseded geometry and are not current evidence.

Frozen before the fresh-author smoke test. Unity `6000.3.24f1`, built-in renderer; `Art01BenchmarkBuilder.Build` and `CaptureNeutral` both exited 0. The neutral renderer replaced all ordinary materials with one grey Standard material and hid props/vegetation. The four retained captures in `massing_checkpoint/` show:

- `neutral_puente_s02.png`: 5.5 m bridgehead-to-2.8 m W12 width transition, parapet/coping, real two-storey mass, supported roof/eave and plinth;
- `neutral_w12_casco.png`: human-height compression between two differentiated facades and a reveal toward F01;
- `neutral_f01_exterior.png`: gable/roof closure, 0.44 m structural render-host apertures, open stone doorway, supported threshold canopy/posts, interrupted plinth and named street/edge transition;
- `neutral_f01_threshold.png`: doorway and landing depth, no opaque wall across the public entry.

The neutral view is deliberately unflattering: it demonstrates the cut facades and meeting geometry without tile, greenery or prop polish. The remaining visible simple solids are declared structural plinths, support posts/feet, kerbs, retaining wall, steps/floor or furniture. They do **not** serve as a universal building shell with windows or a roof attached. The caps/road/threshold still receive independent product inspection in the final checkpoint; this neutral pass alone does not mark ART-01 ready.

The pre-author effective audit `UNITY_BENCHMARK_AUDIT.json` was green with 351 tagged pieces, a single street `MeshCollider` and downward support hits at W12 road `0.055 m`, F01 first step `0.15 m`, F01 landing `0.30 m`, and interior floor `0.30 m`. That audit checks geometry/support, not aesthetics or independent author usability.
