# Predeclared diagnostic performance oracle

Status: **declared before any H2F-03 capture.** This file is committed before the first profiler run. Git history is the proof of order. After a capture exists, this file changes only to record a context value that is measured (marked *measured*), never a threshold.

Contract: `Docs/workpacks/H2F/WP-H2F-03.md` § Performance sanity. Profiling is diagnostic. **No FPS, frame-time, draw-call, triangle or memory number is a PASS/FAIL threshold.** Ordinary numbers are residual/optimization evidence.

## Comparison context

| Field | Declared value |
|---|---|
| Machine | AMD Ryzen 7 5700X (8 cores / 16 threads), NVIDIA GeForce RTX 3060 (driver 32.0.15.9186), 32 GB RAM, Windows 11 Pro 10.0.26200 (owner workstation) |
| Unity mode | Unity Editor 6000.3.24f1 (4e7b9b5b6244), windowed, **Play Mode in the Editor** (not a player build). Editor overhead is included in every number. |
| Fixture revision | the candidate SHA and `Unity/ArkusUnity` tree recorded in `results/profile_*.json` (`candidateSha`, `unityTree`) |
| Resolution | the player camera renders the capture attempt into a fixed 1280×720 target. The Game view size of the clean attempts is recorded as *measured*. |
| Quality / URP tier | `High` quality level → `J2_URP_High` + `J2_Renderer_High` (SSAO + Decals). Recorded from `QualitySettings` and `GraphicsSettings` at runtime. |
| Traversal | the scripted route in `FIXTURE_BRIEF.md` § Route: player walks the street, the tee junction and the side lane, returns to the door threshold, interacts twice, looks through the interior window, and ends by the pond. The NPC walks its NavMesh loop at the same time. The same route and input sequence every attempt. |
| Warm-up | enter Play Mode, wait 3.0 s idle, then start the route. Frames of the first 1.0 s of the route are excluded from summaries but kept in the raw series. |
| Capture | whole route (≈ 60–120 s depending on walking speed); recorders sample every frame |
| Attempts | A = capture attempt (continuous frames + profile); B and C = clean attempts (profile only, no frame capture), each from a fresh Play Mode entry. Pathology 3/5 is judged on B and C. |
| GPU timing | `FrameTimingManager` GPU frame time where the platform reports it; recorded as available/unavailable per attempt. Editor Play Mode GPU numbers are indicative only. |

## Recorded metrics (all diagnostic)

- CPU: main-thread frame time, render-thread time (ProfilerRecorder `Main Thread`, `Render Thread` where available), `Time.unscaledDeltaTime`.
- GPU: `FrameTimingManager` GPU frame time where available.
- Scene complexity: `Draw Calls Count`, `Batches Count`, `SetPass Calls Count`, `Triangles Count`, `Vertices Count`.
- Memory: `Total Used Memory`, `Total Reserved Memory`, `GC Reserved Memory`, `System Used Memory`, `GC Allocated In Frame`.
- Per series: count, min, median, p95, max, plus the ten worst frames with route position.
- Errors: every `Error`, `Exception` and `Assert` log message during the attempt, with the first stack line.

## Enumerated foundation pathologies (the only performance FAIL causes)

Copied from the contract. A pathology must name the causal capability and the reproduction step.

1. Crash, Editor/player termination, out-of-memory, unrecoverable device/render failure or hard hang during the bounded traversal.
2. A selected foundation capability emits an uncaught exception/error on the normal fixture path, and the same causal error reproduces on a second run.
3. A selected subsystem enters a runaway/repeating error or allocation condition that prevents completing the same bounded traversal twice without restart/repair.
4. A selected capability cannot run in the frozen render/platform/project configuration it was adopted for, so representative fixture functionality is unavailable.
5. A deterministic foundation-induced stall or resource exhaustion severe enough that the bounded fixture cannot complete its prescribed continuous traversal on two clean attempts.

Not pathologies unless they reproduce on the warmed traversal **and** meet one of the cases above: one-off import spike, shader compilation, domain reload, first-run bake, background import, isolated GC spike.

## Mechanical evaluation

`results/profile_summary.json` records, per attempt: `completed` (route finished), `errors` (list), `crashed`/`hung` (driver watchdog), and the metric summaries. The pathology decision is:

- 1 → any attempt did not finish because the Editor terminated or the watchdog fired;
- 2 → the same error signature (message without numbers + first stack frame) in attempt A or B and again in a later attempt, attributed to a selected capability;
- 3 → B or C did not complete, or an error signature repeats ≥ 100 times within one attempt;
- 4 → a required fixture function reported unavailable (URP not active, decal/SSAO feature missing, NavMesh absent, GC2 player not driven, IK not solving, water/window shader unsupported);
- 5 → B or C exceeded the route watchdog (3× the capture attempt duration) without a crash.

Everything else is `DIAGNOSTIC_RESIDUAL`.
