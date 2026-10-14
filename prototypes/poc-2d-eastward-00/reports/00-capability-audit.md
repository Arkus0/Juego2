# 00 · Auditoría de capacidades (C0)

Fecha: 2026-10-14 · Entorno: box Linux compartido (Grok Bot), sin GPU física.

| Capacidad | Estado | Evidencia / nota |
|---|---|---|
| Repo `Arkus0/Juego2` | OK lectura/escritura vía conector GitHub (MCP `push_files`, `create_branch`) | `gh` CLI **no** autenticado. El conector solo empuja **texto**: los PNG/JPG binarios no se pueden subir tal cual (ver «Bloqueos»). |
| Rama aislada | OK | `poc-2d-eastward-00`, trabajo solo en `prototypes/poc-2d-eastward-00/`. Ningún archivo Unity tocado. |
| Node / npm | OK | Node 20.19.2, npm 9.2 |
| Phaser | OK | `phaser@4.2.1` (última estable v4 en npm). **Hallazgo**: con renderer WebGL (SwiftShader headless) el `TilemapLayer` dibuja tiles erróneos (`evidence/bug_webgl_tilemap_1280x720.png`); con Canvas es correcto. Se usa Canvas por defecto (`?r=webgl` para comparar). No verificado en GPU real. |
| TypeScript + Vite | OK | TypeScript 7.0.2, Vite 8.3.4; `npm run build` sin errores. |
| Tiled | Programa **no instalado**. Se genera JSON Tiled 1.10 (tileset embebido) abrible en Tiled. No se construye editor propio. |
| Navegador de pruebas | OK | Playwright 1.63 + Chromium headless (capturas, vídeo webm). |
| Herramientas gráficas | Python PIL + numpy + scipy (pipeline por script). Sin Aseprite/GIMP. |
| Generación de imagen | **El subagente no puede generar imágenes.** El agente principal sí (generación IA de Grok Bot). Fuentes en `source/*.jpg` (provenance: IA original, sin copiar assets de juegos). |
| Referencias fotográficas de Cimavilla | Existe `/workspace/cimavilla_lab/...` (texturas, `cimavilla.jpg`) **sin licencia verificada** → NO usadas. |
| Preview web | Solo **localhost** (`vite preview` :4173). No se despliega nada público por instrucción. |

## Vías de creación artística
1. **Elegida**: imágenes IA originales sobre magenta → keying → reescalado → paleta compartida → piezas modulares (pipeline reproducible `tools/pipeline.py`).
2. Pixel art procedural por código: **descartada** como arte final (la propia brief lo prohíbe; experiencia previa: queda genérico).
3. Artista humano / pixel-artist: fuera de alcance sin autorización de gasto.

## Bloqueos, costes, licencias
- **Binarios en GitHub**: el conector no sube PNG. Solución provisional: `assets.bundle.b64.json` (PNG en base64, texto) + `tools/unpack_assets.mjs`. Las JPG fuente (~1,6 MB) **no** se suben; quedan en el box (`/workspace/juego2_poc/.../source/`).
- Coste monetario: 0. Sin compras.
- Licencia: arte generado para el proyecto; ningún asset de Eastward/Alundra/SEGA usado ni calcado.
- Riesgo: la IA no garantiza una gramática de proyección consistente (el chigre vino con pared lateral 3D → recortada).

**Conclusión C0**: existe una vía creíble (IA + pipeline), con la dependencia de que el agente principal genere las imágenes que se piden.
