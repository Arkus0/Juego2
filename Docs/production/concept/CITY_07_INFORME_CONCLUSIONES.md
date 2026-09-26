# Informe de conclusiones — CITY-07, mapa de juego y estado visual

Fecha: 2026-09-25  
Estado: **NO CANÓNICO**. Es un informe de sesión de diseño y no sustituye a ningún WP, enmienda aceptada ni revisión independiente.  
PR asociado: [#228](https://github.com/Arkus0/Juego2/pull/228), rama `claude/city-07-game-map-eohu64`  
Base de `main` consultada: `2aefda4` (CITY-04 aceptado)

---

## 1. Resumen ejecutivo

1. **El distrito semilla (CITY-03) es correcto pero escaso para un videojuego.** Sus calles, puente, vado, anclas y solares aguantan, pero solo abre 3 edificios y deja media cuña sin acceso.
2. **Se puede densificar sin romper la semántica aceptada**, en tres capas:
   - relleno de casas cerradas y mobiliario, que CITY-07 puede hacer ya;
   - callejas nuevas (**P8**, enmienda a CITY-01 y CITY-03);
   - locales pequeños nuevos (**P9**, enmienda a CITY-02 y CITY-06).
3. **La torre del Ayuntamiento (P1)** es el hito que convierte el distrito en un mapa que se recuerda. Necesita enmienda a CITY-05.
4. **El setting (Potes / Liébana ficticio) tiene recorrido.** El problema no es el lugar sino la densidad de cosas que hacer. No recomiendo cambiarlo ahora.
5. **Problema real detectado: la escena de prueba de Astra son cubos y planos con texturas.** La causa de fondo es que casi no hay kit de construcción admitido y el que existe (Medieval Village) choca con la biblia visual.
6. **El roadmap no lo arregla solo.** Falta escribir **WP-ART-01** (adaptación Quaternius → Cantabria), cuya condición de arranque ya se ha cumplido. Si no se escribe antes de CITY-07, el keeper se construirá con el kit equivocado.

---

## 2. Diagnóstico del distrito semilla

Toda la geometría de esta sección sale de `CITY_PRODUCT_SEED.md` §2–4.

| Métrica | Valor |
|---|---|
| Superficie de la semilla | 0,0448 km² (dentro de la banda aceptada 0,03–0,06) |
| Aristas públicas representadas | W04, W05, W06, W12, W13, X1, X5, más el bucle A/B del Casco |
| Cuña a ≤15 m de una ruta pública | ~33 % |
| Cuña a ≤25 m de una ruta pública | ~49 % |
| Edificios comprometidos | 8 frentes (F01–F08) y 3 espacios abiertos (S01–S03) |
| Locales en los que se entra | 3: bar (I3), Ayuntamiento (I2), tienda (I1) |
| Calle con algo abierto a ≤25 m | 27 % |

**Conclusión:** la semilla demuestra la topología (río, puente, vado, niveles) pero tiene dos vacíos:

- **Edificios:** es fácil de resolver en CITY-07.
- **Calles:** hace falta una enmienda aguas arriba.

La sensación de "pueblo pequeño y vacío" del greybox de CITY-04 (desviación D02) viene de ahí.

---

## 3. Mapa de juego de concepto

Archivos en `Docs/production/concept/`:

- `city07_game_map.html`: mapa interactivo con capas;
- `CITY_07_GAME_MAP_CONCEPT.md`: memoria completa;
- tres capturas y `city07_geometry_check.py`, la validación geométrica con shapely.

### 3.1 Qué contiene

- **Densidad media.** Casas cerradas solo con fachada a la calle y huertas con tapia detrás. El primer intento, con todo lleno de casas, resultó demasiado denso.
- **Mobiliario:** fuentes, farolas, bancos, hortensias, ropa tendida, leña, pozo, bolos y barcas.
- **11 secretos que siguen las reglas de CITY-06:** no hay cofres; lo que se descubre es el propio espacio.

| Estado | Secretos |
|---|---|
| Ya aceptados (3) | patio trasero del bar, tablón del Ayuntamiento, escalera del patio S03 |
| CITY-07 puede hacerlos (5) | marcas de riada, puerta cegada, hornacina, rendija al río, barca varada |
| Dependen de P8 (3) | pozo de la huerta, mirador del Arroyo, la Era alta |

### 3.2 Posibilidades como videojuego

| Situación | Dónde |
|---|---|
| Base / casa | bar F01 y plazuela del Casco |
| Multitud | Plaza y mercado |
| Seguir o buscar | bucle A/B, Pasadizo, Horno |
| Persecución | Cuesta, W12 y la vuelta de los Tintes |
| Enfrentamiento | Puente Viejo |
| Conversación | Mirador del Arroyo, Rincón del Tinte, desembarcadero |
| Observación | lomo del puente, Calleja Alta detrás de la torre |
| Juego | La Era alta (bolos) |
| Esperar un cambio de estado | el vado, que solo existe con aguas bajas |

Otros rasgos que aporta el mapa:

- orientación en tres escalas: torre, río y plaza, y hitos locales;
- contraste entre zonas tranquilas y animadas;
- ganchos para sistemas futuros: crecidas, mercado, archivo municipal y barca X6.

### 3.3 Eslabón recomendado para CITY-07

`Orilla sur → Puente Viejo → cabeza del puente S02 → subida W12 → plazuela del Casco → traza B → Bar F01`

- **No necesita ninguna enmienda.**
- Termina en el único interior héroe.
- Cubre las escenas SCN-04, 05, 06 y 07 de CITY-03.
- No toca el rincón reservado a CITY-08 (`W04 + F03 + F07 + S01`).

---

## 4. Enmiendas propuestas (PR #228, estado **PROPUESTO**)

| Enmienda | Dueño que la aprueba | Qué hace | Efecto |
|---|---|---|---|
| **P1** `CITY_P1_CIVIC_TOWER_AMENDMENT.md` | CITY-05 | Torre única de 24–32 m dentro del solar F02, con teja oscura y sin almenas. No añade accesos ni interior. | Hito que orienta todo el distrito |
| **P8** `CITY_P8_LANES_AMENDMENT.md` | CITY-01 + CITY-03 | 8 cruces, partición de W05/W12/W13 manteniendo sus tiempos, aristas W18–W26 y E06, y plazuelas S04–S07 | Bucles reales; la cuña deja de tener bloques inaccesibles |
| **P9** `CITY_P9_PLACES_AMENDMENT.md` | CITY-02 + CITY-06 (y filas en CITY-03/05) | 11 locales I1 en los solares F09–F19, más la tintorería exterior S04 | Locales: de 3 a 15. Calle con algo abierto a ≤25 m: del 27 % al 77 % |

Toda la geometría está validada contra los polígonos exactos de la semilla:

- dentro del contorno duro;
- fuera del agua y de las orillas no edificables (3 m y 2 m);
- sin pisar los solares ni las calles existentes;
- sin solaparse entre sí;
- dentro de las bandas de parcela de CITY-05.

Cada enmienda necesita revisión independiente, merge y DocSync. Esta sesión **no** debe ser su revisor.

---

## 5. Comparación con Kamurocho (Yakuza)

| Aspecto | Kamurocho | Este distrito |
|---|---|---|
| Cosas que hacer por metro de calle | muy alta | baja; con P9, media-alta |
| Calles fáciles de aprender en bucle | cuadrícula con nombre | solo con P8 |
| Hito | Millennium Tower | la torre (P1) |
| Geografía y desniveles | llano, bordes artificiales | río, puente, vado, cuesta: **mejor** |
| Contraste tranquilo / animado | intenso en todas partes | **mejor**, por diseño |
| Secretos con sentido | historias secundarias | espaciales (CITY-06); a desarrollar por PA |

**Lección que conviene importar:** la densidad de cosas que hacer, que se consigue con P9.  
**Lo que no conviene importar:** la saturación permanente. Este pueblo funciona mejor al estilo Shenmue que como el Tokio nocturno.

---

## 6. Setting: mantener o cambiar

**Recomendación: mantener el Potes / Liébana ficticio.**

- Es un escenario único: casi no hay juegos en la España rural de montaña.
- Encaja con la referencia de "sensación de vida" de la biblia visual.
- Puede tener un ciclo de estados propio: día de mercado, fiestas y la fiesta del orujo, peregrinos del Camino Lebaniego, turismo, crecidas y temporadas de huerta y alambique.
- **Solo tendría sentido cambiar** si la fantasía principal fuera combate callejero, vida nocturna urbana o crimen organizado. En ese caso habría que cambiar **ya**: se rehace la cadena CITY y la biblia visual, pero no H1.

Prueba práctica para decidir en la demo de CITY-07: desde cualquier punto de la calle, ¿hay a la vista al menos una cosa que hacer o descubrir?

---

## 7. Escena de prueba de Astra (fotos del 2026-09-25)

### 7.1 Qué se ve

- Muros de caja sin grosor; un lateral es un plano blanco sin textura.
- Ventanas planas sin hueco ni marco; la puerta es un agujero sin jamba.
- El tejado va suelto y no casa con el muro; el porche es un tejado sobre dos postes.
- No hay zócalo ni bordillo; todo sale de un plano único con un tiling de piedra enorme.
- Árboles de esfera más cilindro, uno de ellos atravesando otro cilindro; colinas en cúpula.
- Cartel de una sola cara: desde detrás se lee en espejo.
- Sol duro, cielo azul plano y césped saturado.
- Casas de entramado con tejado de pico y teja terracota.
- El personaje es el maniquí de pruebas, sin ropa.

**Veredicto:** son cubos y planos con assets pegados encima. Es exactamente el fallo que la enmienda de CITY-07 describe como *transcribir el greybox*.

### 7.2 Choques con lo aceptado

- **Biblia visual:** veta el entramado alpino, los tejados de chalet, el terracota por defecto, el cielo azul plano y la fantasía medieval. Pide teja oscura mojada, piedra gris, cielo nublado y verde apagado.
- **La Taberna del Puente es F09 de P9, que no está aceptada.** Además, CITY-07 no puede empezar hasta que pase H1-GATE. La escena solo vale como prueba desechable.
- **El texto de Astra menciona el paquete "Nature".** Cualquier import fuera de lo admitido tiene que pasar por la admisión de assets (`DEPENDENCY_IP_POLICY`).

### 7.3 Causa raíz

H1-04 solo admitió **una fachada** (`Wall_Plaster_Window_Wide_Flat.fbx`), un material y la biblioteca de animaciones del **Medieval Village MegaKit**. Sin un kit modular, cualquiera acaba improvisando con primitivas, y el pack de origen tiene justo el estilo vetado.

---

## 8. ¿Lo arregla seguir el roadmap?

**En parte, no de forma automática.**

| Paso | ¿Ayuda al aspecto? |
|---|---|
| H1-09, H1-10 | No; son el puente con Unity y la reconstrucción |
| H1-11 | Poco: 10–15 piezas más del mismo pack medieval, como prueba del puente. Prohíbe expresamente producir calles o el pueblo. |
| H1-GATE | No; es la prueba de preparación |
| CITY-07 | Sí en la composición, **pero compone con el kit que haya** |
| **WP-ART-01** | **Sí: es la pieza que falta.** Decide qué assets se quedan, cuáles se adaptan a lo cántabro y cuáles se crean, y fija los criterios visuales. Su condición de arranque (greybox de CITY-04) **ya se ha cumplido**, pero el WP **no está escrito**. |
| WP-ART-02 | Cierre visual, pero va **después** de CITY-07 |

**Riesgo:** si CITY-07 arranca sin ART-01, el keeper saldrá con el kit medieval-alpino y habrá que rehacerlo en ART-02.

---

## 9. Recomendaciones ordenadas

1. **Revisar el PR #228** con un revisor independiente y decidir P1, P8 y P9 por separado en el DocSync.
2. **Escribir WP-ART-01 ya**, en paralelo con H1-09 → H1-GATE, e incluir:
   - la lista mínima del kit para el eslabón del Puente Viejo al bar (~40–60 piezas: Quaternius tal cual / adaptada / nueva);
   - la paleta y las reglas de la biblia visual como criterios de aceptación;
   - la admisión de cualquier pack nuevo (por ejemplo Nature) bajo `DEPENDENCY_IP_POLICY`.
3. **Añadir una dependencia blanda en CITY-07:** el keeper se compone con el kit adaptado por ART-01.
4. **Tratar la escena de Astra como prueba desechable.** Si se sigue explorando, dar prioridad a un pase de estilo (teja oscura, piedra gris, cielo nublado, laderas rocosas) y a los fallos técnicos (cartel de dos caras, tiling del suelo, árbol atravesado).
5. **Definir con PA los estados del pueblo** (mercado, fiesta, noche, crecida). Son lo que da vida sin convertir el pueblo en un Kamurocho de neón.
6. **En la demo de CITY-07**, medir RD-1 (destinos por traversal), RD-4 (minutos tranquilos) y la "prueba Kamurocho".

---

## 10. Límites de este informe

- Toda la geometría es de planificación; no hay anchos, pendientes, visuales ni tiempos medidos.
- Las fotos de Astra se han valorado de forma visual, sin acceso a la escena ni a sus archivos.
- Esta sesión diseñó el concepto y las enmiendas, así que no debe actuar como revisor independiente de ellos ni de un candidato de CITY-07 construido a partir de ellos.
