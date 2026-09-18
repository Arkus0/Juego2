---
description: Revisa adversarialmente un candidato congelado de Juego2 sin editarlo.
agent: reviewer
---

Usa la skill `validate-workpack` sobre el PR/WP indicado en `$ARGUMENTS`. Reconstruye todo desde GitHub, verifica independencia y SHA congelado exacto, intenta falsar el candidato y emite PASS/FAIL/BLOCKED/READY_FOR_LOCAL_VALIDATION sin corregir implementación.
