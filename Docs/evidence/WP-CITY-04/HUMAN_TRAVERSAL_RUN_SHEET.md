# CITY-04 human traversal run sheet

Status: unexecuted timing template. The 2026-09-25 human assessment is recorded in `OWNER_GREYBOX_FEEDBACK.md`; it does not fill the runs or imply a measured PASS. Timing is deferred by the owner pending an explicit accepted contract amendment.

Open `Unity/ArkusUnity/Assets/Arkus/CITY/City04Greybox.unity` in Unity `6000.3.24f1` and enter Play mode. After the owner feedback, `WASD` walks at 3.5 m/s and Shift moves at 5.0 m/s; hold right mouse to look. Keys `1`–`9` place the probe at O.X1, W.X1, W.CASCO, F01 bar, W.PLAZA, W.SHOP, W.LANDING, W.X5 and E.X5. `T` resets the leg timer; `L` writes `CITY04_HUMAN_LEG` with elapsed seconds and end coordinates to the Unity Console. `Z` starts the micro.A proxy; `X` starts micro.B; `B` blocks/opens micro.A; `G` changes X5 availability; `Q` shows/hides the busy market proxy.

Before each timed leg, state the start/end anchors, scenario state, whether Shift was used, and any collision or detour. Save the Console log or transcribe the `CITY04_HUMAN_LEG` line. Record the actual human observer, date, exact Git SHA and effective Unity editor/version. The proxy and markers are spatial props; they claim no NPC, market, access-policy or Living World runtime.

## Required nine runs

| # | Run and setup | Time / notes / observed defects |
|---|---|---|
| 1 | Cross the seed casually with no objective; start at O.X1 or W.SHOP, choose freely. Note perceived scale and route choice without map overlay. | **MISSING** |
| 2 | At W.CASCO, follow proxy route A with `Z`; note split and rejoin. | **MISSING** |
| 3 | Repeat with proxy route B using `X`; compare distinctness and whether S03 is mistakenly used as a public shortcut. | **MISSING** |
| 4 | Travel X1 bridge ↔ F01 bar ↔ W.PLAZA/S01; note grade, bridge constraint, sightlines and times. | **MISSING** |
| 5 | Approach/leave `seam.port_landing` at W.LANDING; confirm no dry Puerto continuation, only a future across-water socket. | **MISSING** |
| 6 | Approach/leave residential/rural continuation via O.X1 and upper visual seam; judge whether expansion can join without moving retained anchors. | **MISSING** |
| 7 | Press `B` to block micro.A and use micro.B. Toggle `G` to close X5 and verify no other Arroyo crossing appears. | **MISSING** |
| 8 | Compare quiet W13/S03 with `Q` quiet against busy S01/W04 with `Q` busy. Note whether ordinary space remains useful. | **MISSING** |
| 9 | At F01/F02/F03, test at least one service/private or semi-private threshold; distinguish it from the public entrance without treating it as an ordinary public shortcut. | **MISSING** |

## Timed segments and concatenations

Record seconds, route, direction, X5 state, observed width/grade/obstacle and comparison to `LOCAL_GREYBOX_OBSERVATION.md`. Walk W04, W05, W06, W12, W13, X1 and available X5 separately. Also time O.X1→W.PLAZA via X1/W12/W05 (CITY-01 3.75 min), W.SHOP→W.LANDING via W04/W05/W06 (4.50 min), and W.CASCO→E.X5 via W13/X5 when available (1.40 min). The scene's hard seed does not include the absent full-city links.

After traversal, record: Río/Arroyo apparent widths and bank clearances; W04 and historic-lane clear widths; W06/W13 grades; X1 pedestrian approach and X5 foot-only/closure read; S01 apron versus always-clear route; F01/F02/F03 threshold separation; S03 boundary; all five expansion seams; accidental crossing/shortcut search; and any F01–F08/S01–S03 envelope problem. For each real defect state the observation, expected CITY-03/CITY-01/02/05/06 constraint, causal owner and whether CITY-04 geometry can repair it without semantic drift.

Final human spatial verdict: **MISSING** (`PASS` / `REVISE` only after these observations). Observer/signature: **MISSING**. Candidate SHA tested: **MISSING**.
