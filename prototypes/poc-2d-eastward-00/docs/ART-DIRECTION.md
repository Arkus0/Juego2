# Dirección de arte · POC-2D-EASTWARD-00

## Fórmula obligatoria (regla de estilo del usuario)
> **«Eastward meets Shenmue meets Asturias»**: pixel art denso de Eastward y luz cálida; realismo cotidiano y objetos de época de Shenmue; arquitectura, clima y gente de Asturias/Cimavilla 1992.

Todo asset, prompt de generación y revisión se juzga contra esta fórmula. Toda petición de imagen empieza con ella.

## Gramática visual (original; Eastward solo como listón de calidad)
- **Proyección**: frontal 3/4 «oblicua»: las fachadas se ven de frente (sin paredes laterales en perspectiva), los tejados se ven un poco desde arriba y el suelo es cenital. **Una sola gramática**: nada de cubos isométricos ni laterales en fuga. Por eso se recortó la pared lateral del chigre.
- **Luz**: principal desde arriba-izquierda, cálida de atardecer; secundarias puntuales (escaparates del chigre, farolas). La iluminación de escena es una capa **subordinada**: el arte base debe aguantar sin ella (comparar `frame_1x_luzOFF.png`).
- **Densidad de píxel**: un solo grid lógico (1 px de asset = 1 px lógico). Prohibido mezclar escalas.
- **Contraste focal**: el chigre (verde botella + ámbar) es el foco; la arquitectura residencial va en ocres y piedra gris; el suelo, más apagado que las fachadas.

## Decisiones
| Decisión | Valor |
|---|---|
| grid_tile_size | 16 px (pavimento en metatiles de 32×32 = 2×2) |
| logical_resolution_and_scale | 480×270, escalado **entero** (×2 a 1280×720, ×4 a 1920×1080), `pixelArt`, `roundPixels` |
| actor_sprite_dimensions | humano ≈ 30 px de alto (caja 16×32); puerta residencial 36 px; puerta chigre ≈ 37 px |
| projection_and_anchors | frontal 3/4; ancla **abajo-centro** = línea de pies; depth = y del ancla |
| palette_and_contrast | 48 colores compartidos: 42 median-cut sobre todas las piezas + 6 acentos fríos reservados (azules de hortensia). `assets/palette/` (.png, .gpl). Sin dithering |
| modularity_rules | fachada = base (planta baja, colisión) + upper (pisos repetibles) + roof (capa de oclusión). Props sueltos con ancla de pies |
| layers_and_collision | `ground` (tiles) · `objects` (fachadas, props, luces, triggers) · `collision` (rects) · roof depth 5000 · luz 8000 |

## Rasgos asturianos de la microzona (≥3)
1. **Galería acristalada blanca** sobre fachada de mampostería con revoco ocre desconchado.
2. **Chigre** con frente de madera verde, rótulo «CHIGRE CASA PACHU», botellas de sidra, barricas-mesa y taburetes.
3. **Empedrado húmedo** con musgo, charcos y canaleta; balcón de forja con ropa tendida; **hortensias azules**; cajas de pescado con red (barrio pesquero).

## Rechazos explícitos
Paquete RPG Maker, arquitectura japonesa genérica, collage sin oclusión, densidades de píxel mezcladas, una ilustración panorámica sin piezas.
