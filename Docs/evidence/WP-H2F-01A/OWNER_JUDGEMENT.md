# Owner judgement — S06 amendment scope

Date: 2026-09-27, during the Worker session. The owner was shown `captures/owner/S06_amendment_camera_route.png`:

- same route, same GC2 body, same Juego2 input;
- top row: the H2F-01 Cinemachine 3 camera;
- bottom row: the GC2 Third Person shot.

The Worker's summary was that route metrics are equal and the only visible difference is at the Bar F01 threshold: Cinemachine presses against the lintel (the H2F-01 known residual), while the GC2 shot frames the interior better.

Question asked (verbatim, Spanish):

> Los probes C02–C07 respaldan una enmienda S06 del controlador: cuerpo del jugador = GC2 Character (con el input de Juego2 y rendimiento de ruta idéntico al controlador mínimo), porque así jugador y NPC comparten interacción, pasos, look-at, gestos y ragdoll sin código a medida. En la cámara, las métricas son iguales. La única diferencia visible es en el umbral del bar: Cinemachine se pega al dintel (el residual ya aceptado de H2F-01) y la cámara de GC2 encuadra mejor el interior. ¿Qué incluyo en la enmienda?

Options offered:

1. **Solo controlador** — the Worker's recommendation.
2. **Controlador + cámara GC2.**
3. **Sin enmienda.**

**Owner answer: "Controlador + cámara GC2".**

## How it is applied

- The S06 amendment covers the player **controller** (GC2 Character body) **and** the player **camera** (GC2 Main Camera + Third Person shot). Input is unchanged: the Juego2 Input System action map remains the only input source. GC2 reads it for movement **and** for camera orbit/zoom.
- The owner's choice is a feel judgement. After the question, the Worker added a rendered body-visibility measurement to the route so the judgement rests on data as well as the sheet: the share of screen pixels that change when the body is hidden, sampled every 0.5 s. C07 re-runs the full composition with the amended camera. The numbers are in `PROBE_RESULTS.md`.
- The Worker's recommendation (controller only) is kept on record. The amendment document states the extra cost the camera change brings: GC2 camera state, and the displaced Cinemachine player-camera role.
- The independent Reviewer judges the amendment against the WP rule: explicit, evidence-backed, and naming what it displaces, the measured benefit, the new authority/lifecycle burden and the H2F-02 handoff.
