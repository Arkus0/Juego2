# 01 · ART-GATE — iteración 2 (C2)

**Veredicto: CONDITIONAL.** Es una mejora clara frente a la iteración 1 y la escena ya se puede recorrer para juzgar la dirección artística. Quedan 4 defectos visibles (ver «Problemas abiertos»). **No se avanza a la fase 3 sin aprobación expresa.**
Fórmula de juicio: *Eastward meets Shenmue meets Asturias*. La iteración 1 se conserva en `evidence/iter1/` y `tools/pipeline_v1.py`.

## Cómo inspeccionar
`npm run build && npx vite preview --port 4173` → `http://localhost:4173/` (`?light=0`, `?auto=1`, `?r=webgl`). Teclas: WASD/flechas, L (luz), C (colisiones). Playwright: 0 errores; zoom ×2 a 1280×720 y ×4 a 1920×1080.

## Pavimento: ¿los cobbles nuevos tilean limpios por sí solos? **No.**
Medición objetiva en `evidence/seam_analysis_v2.json` (`tools/seam_analysis.py`):
- La red diagonal tiene un periodo de ~68–70 px en X y ~60–61 px en Y. Es **anisótropa**: los rombos no son cuadrados.
- La autocorrelación en el pico es baja (0,22–0,33): las piedras están pintadas de forma irregular y la red no es regular.
- Con el mejor recorte posible (3 periodos, desplazamiento optimizado), el error de envoltura es **2,2–2,4 veces** la diferencia natural entre columnas vecinas. En los 4 tiles y en los dos ejes se ve costura.
- Cabe solo 1 tile de 236 px útiles, es decir, 3 periodos: no hay margen para elegir un recorte que encaje.

**Solución aplicada:** reconstruir la red. Se genera una red de rombos perfecta (2 periodos por cada metatile de 32 px) y cada piedra se rellena con píxeles pintados del propio `pavement_v2.jpg`. Las juntas se rellenan con piedra vecina y cada piedra lleva borde claro arriba-izquierda y oscuro abajo-derecha. El resultado es **perfectamente tileable**: `evidence/tiling_3x3_*`, `evidence/tiling_mix_*`. Ojo: la forma de cada piedra la da el código; la textura sí es pintada.
- Bordillo horizontal: reconstruido por bandas (2 losas, 3 y 5 piedras por fila), cortando por las juntas detectadas. Tilea en X.
- Losa (`pav_slab`, 64×32): sin verificar como tileable; a la escala de la acera se nota su repetición.

## Cambios de la iteración 2
- `facade2` (ULTRAMARINOS) sustituye a la fachada en espejo: ahora hay **3 fachadas distintas** más una medianera.
- `backwall` va retranqueada detrás del chigre y tapa el hueco de cielo. Una segunda copia cierra la calle por el este.
- La cámara mira 48 px por encima del jugador para que tejados y fondo entren en cuadro.
- Charco y musgo van como **decales** (capa del suelo, por debajo de los actores). El charco se re-colorea para el atardecer, porque reflejaba un cielo azul de mediodía.
- Props reescalados a la referencia humana de 30 px: banco 34×26, máquina 32×44, bici 31×26, barrica 20×28, farola 16×58, etc. (lista en `assets/manifest.json`).
- **Limpieza de propiedad intelectual** (`tools/pipeline.py: ip_repaint`), repintada sobre la imagen fuente:
  - latas con marca → latas genéricas sin texto;
  - bandera y «1992» de la máquina → panel crema liso;
  - escudo y texto municipal de la papelera → verde liso;
  - placa «BUTANO 1992» → placa en blanco.
  Los textos genéricos que quedan («PRENSA TABACOS», cajas de sidra y de pescado) son ilegibles a escala de juego.
- Halos de magenta: color de fondo detectado en cada imagen, erosión de 3 px y despill en el borde. Además, se eliminan los píxeles del contorno que siguen siendo magenta después de reducir.
- Paleta: 48 colores = 39 por median-cut + 6 azules reservados (hortensia) + 3 verdes de musgo reservados.

## Evaluación (a escala real)
| Criterio | Iter 1 | Iter 2 |
|---|---|---|
| Escena navegable | PASS | PASS |
| Lugar asturiano inequívoco | PASS con reservas | **PASS**: galería, chigre, ultramarinos con toldo, ropa tendida, hortensias, bombona naranja, losas y adoquín |
| Pavimento sin seams ni repetición | FAIL | **PASS** en los cobbles (sin rejilla a 1×). **CONDITIONAL** en la losa de acera |
| Piezas recombinables | CONDITIONAL | **PASS**: 4 fachadas en base/upper/roof, decales, 13 props, mapa Tiled |
| Densidad de píxel y escala coherentes | CONDITIONAL | **CONDITIONAL**: los props siguen menos detallados que las fachadas (reducción de ~8× frente a ~3–4×). La bici queda fina y se pierde |
| Sin depender de filtros | PASS | PASS (`frame_1x_luzOFF.png`) |
| Movimiento | NO VALIDADO | **NO VALIDADO a fondo**: sin GPU real; el vídeo headless no permite juzgar el shimmer |

## Problemas abiertos
1. **Escala de la medianera:** sus sillares parecen grandes al lado de las fachadas, y la casa del este se lee como un muro. Haría falta una medianera dibujada a la escala de 30 px.
2. **Luz:** los halos del chigre se ven como manchas difusas sobre la fachada, porque el halo aditivo cae sobre arte que ya tiene luz pintada. Hay que bajarlos o moverlos al suelo.
3. **Charco:** el re-coloreado lo deja algo granulado y marrón. Mejor pedirlo pintado ya de atardecer.
4. **Acera:** la losa de 64 px se repite. La bici y la máquina pierden lectura a escala de juego.
5. El chigre (1,5 plantas) sigue más bajo que las demás fachadas. Es aceptable como edificio singular.
6. Phaser 4.2.1 + WebGL TilemapLayer: sigue el render erróneo en headless, así que se usa Canvas.

## Próximo paso
Esperar la aprobación del usuario tras recorrer la escena. NPC, interior y combate **no** están empezados.
