# Especificación de assets

Todos son PNG RGBA, alfa duro (0/255), cuantizados a `assets/palette/palette.png` (48 colores), 1 px = 1 px lógico. Inventario completo con tamaños, sha1 y procedencia en `assets/manifest.json`.

## Pipeline (`tools/pipeline.py`, reproducible)
1. Keying de magenta (#F800C6): umbral por distancia + spill, apertura morfológica, **erosión de 2 px** a resolución fuente, despill de R/B solo cerca del borde.
2. Reescalado premultiplicado: LANCZOS a 2× seguido de BOX a 1×, y alfa con umbral al 50 %.
3. Paleta compartida y cuantización al color más cercano, sin dithering.
4. Pavimento: recorte interior del 5 %, igualación tonal (media y desviación) contra `cobble_a` y fundido de envoltura de 8 px en ambos ejes (en las transiciones, solo en el eje de recorrido).
5. Las fachadas se cortan en las piezas `roof`, `upper` y `base` por fracciones documentadas.
6. Atlas JSON-hash (compatible con Phaser/TexturePacker), mapa Tiled y manifest.

## Inventario
| Grupo | Archivo | Tamaño |
|---|---|---|
| Tileset | `assets/tilesets/pavement_16.png` | 256×64 (tiles de 16). Fila 0: cobble_a, b, c, moss, puddle, worn_path, gutter, cobble_to_slab. Fila 1: slab_fill (derivado), curb_h (rotado 90°, PROVISIONAL) |
| Fachadas | `assets/facades/residencial_{full,roof,upper,base}.png` | 160×182 (37+82+63) |
| | `assets/facades/chigre_{full,roof,upper,base}.png` | 128×119 (20+32+67) |
| Props | `assets/props/*.png` | barrel_table 17×24, fish_crates 21×20, butane 7×14, stool 11×12, vending 24×36, bicycle 23×20, wall_lantern 10×15, lamp_post 14×50, hydrangea 17×20, bench 25×20 |
| Atlas | `assets/atlas/world_atlas.{png,json}` | 512×512, 18 frames, anchor (0.5, 1) |
| Mapa | `assets/maps/street_00.json` | 30×26 tiles (480×416), Tiled 1.10 |

## Reutilización
- El pavimento sirve para cualquier calle: 3 variantes base, 2 decales (musgo, charco) y 3 transiciones (camino gastado, canaleta, bordillo).
- Fachadas: apilar `base + upper (+upper) + roof`. Cambiando la `base` se varía la planta baja (portal, comercio). El `flipX` es **provisional**, porque revela la repetición.
- Colisión de props: huella de unos 70 % del ancho × 6 px en la línea de pies. Los props de pared no colisionan.

## Cómo abrir en Tiled
Abrir `assets/maps/street_00.json` en Tiled ≥1.10: el tileset está embebido y apunta a `../tilesets/pavement_16.png`. Los objetos llevan las propiedades `frame` y `flipX`, que corresponden a los frames del atlas.
