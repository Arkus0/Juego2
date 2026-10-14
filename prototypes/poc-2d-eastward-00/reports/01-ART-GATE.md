# 01 · ART-GATE (C2)

**Veredicto: CONDITIONAL (no aprobado para la fase 3).** Las fachadas se acercan a la ambición; el pavimento y la escala de los props **no**. Hace falta otra iteración de arte (con las imágenes nuevas pedidas abajo) antes de construir nada encima.

Fórmula de juicio: *Eastward meets Shenmue meets Asturias*.

## Qué se puede inspeccionar
- Escena navegable en localhost: `npm run build && npx vite preview --port 4173` → `http://localhost:4173/`. Teclas: WASD/flechas, `L` (luz), `C` (colisiones). Parámetros: `?light=0`, `?auto=1` (recorrido automático), `?r=webgl`.
- La valida Playwright: 0 errores de consola o página, carga en 1280×720 (zoom ×2) y 1920×1080 (zoom ×4).
- Recorrido grabado: `evidence/recorrido_auto.webm` (19 s, 1280×720) y `evidence/recorrido_frames.gif`.
- Sprites aislados: `evidence/isolated_sprites_1x.png` / `_x3.png` (la barra gris = humano de 30 px). Atlas: `evidence/atlas_1x.png` / `_x2.png`.
- Montaje a 1× real (sin escalar): `evidence/frame_1x_luzON.png`, `evidence/frame_1x_luzOFF.png`.
- Dos tamaños de ventana: `evidence/shot_{1280x720,1920x1080}_luz{ON,OFF}_{spawn,chigre}.png`.
- Tiling: `evidence/tiling_3x3_*.png` (cada metatile en 3×3), `evidence/tiling_mix_*.png` (segunda combinación aleatoria).
- Bug de WebGL: `evidence/bug_webgl_tilemap_1280x720.png`.

## Evaluación por criterio (a escala real de juego)
| Criterio | Resultado | Diagnóstico honesto |
|---|---|---|
| Escena navegable, no render | **PASS** | Tilemap Tiled + objetos + colisiones + cámara con seguimiento; el jugador es una cápsula PROVISIONAL. |
| Lugar inequívocamente asturiano | **PASS (con reservas)** | Galería blanca, mampostería con revoco ocre, chigre verde con sidra, hortensias y cajas de pescado. La bombona **azul** no es de 1992 en España (era naranja). La máquina de refrescos tiene un aire algo japonés. |
| Arte original, atractivo, ambición Eastward | **CONDITIONAL** | Las **fachadas** tienen detalle y carácter: es lo mejor del set. Los **props** quedan pequeños y menos definidos (se reducen ~10× frente a ~3,75× las fachadas), así que la densidad de detalle no es homogénea. |
| Las piezas encajan y se recombinan | **CONDITIONAL** | Las fachadas se apilan (base, upper, roof) con anclas de pies. Pero solo hay **1 fachada residencial**: la segunda es la misma en espejo, y la repetición se ve. El chigre tiene 1,5 plantas frente a 2,5 de la residencial, sin edificio de fondo: queda un hueco de «cielo» liso. |
| Calidad sin depender de filtros | **PASS** | La versión sin luz se sostiene. La luz es un velo cálido más halos aditivos en bandas. |
| Pavimento tileable sin seams ni repetición | **FAIL** | A 1× se ve la **rejilla de 32 px**: la banda de fundido de 8 px emborrona las juntas. El charco es un cuadrado evidente. La acera de losas (`slab_fill`) es plana y borrosa. El bordillo horizontal está rotado 90°, con la luz incoherente. |
| Proyección única | **PASS tras corrección** | Se recortaron la pared lateral 3D y el faldón de tejado del chigre. El borde izquierdo de su tejado queda cortado en recto. |
| Coherencia de luz | **PASS (aprox.)** | Fuentes con luz arriba-izquierda; el chigre trae su propia luz interior, lo que es coherente con la escena. |
| Movimiento / cámara | **NO VALIDADO a fondo** | El vídeo headless a unos 25 fps no basta para juzgar el *shimmer* subpíxel. Se usan zoom entero y `roundPixels`, pero la cámara con lerp 0,12 puede producir vibración: hay que revisarlo en un monitor real. A 1280×720, el zoom ×2 deja bandas negras (960×540). |

## Problemas abiertos
1. Pavimento: cambiar el fundido por un recorte en juntas, o pedir tiles nativos de 32 px pensados como tileables. También faltan un decal de charco orgánico (con alfa) y un bordillo horizontal nativo.
2. Falta una **segunda fachada residencial real** y una **fachada de fondo/medianera** detrás del chigre.
3. Props: pedirlos a escala coherente y con más masa: banco a ~24 px de alto y bici a ~22 px, dibujados para esa escala y no reducidos 10×. Bombona naranja de época.
4. Halo residual de despill en algunos bordes (hortensia).
5. Phaser 4.2.1 WebGL TilemapLayer renderiza mal en headless/SwiftShader. Se usa Canvas; queda pendiente comprobarlo en GPU real.
6. Los binarios no van en el repositorio como PNG (limitación del conector): van en un bundle base64 que se desempaqueta con `node tools/unpack_assets.mjs`.

## Estimación
Una iteración más de arte: unas 1–2 h de pipeline, más el tiempo de generación de 5–6 imágenes nuevas. Después, nuevo ART-GATE.

## Próximo paso autorizado
**Ninguno automático.** Se espera la aprobación o las instrucciones del usuario. No se han creado NPC, interior ni combate.
