# PROTOTYPE_REPORT — «Del Puente Viejo al Bar»

Estado: **PROTOTIPO NO CANÓNICO** · rama `prototype/demo-puente-bar` · PR en borrador #233, sin merge.
No es evidencia de ningún WP (ni CITY-07, ni ART-01, ni H1/H2). Nada de este documento modifica contratos
aceptados; las recomendaciones de la §7 son **propuestas** para que el propietario decida.

## 1. Qué es y cómo se ejecuta

Demo jugable en tercera persona sobre la semilla CITY-03 más las propuestas P1/P8/P9 del mapa de concepto
(PR #228). La calidad se concentra en el eslabón **Orilla sur → Puente Viejo (X1) → cabeza S02 → subida W12 →
plazuela del Casco → traza B → Bar F01**. Instrucciones de uso: [README.md](README.md).

Todo se genera por código y por capas (`Prototype → Rebuild Scene`), en el orden obligatorio del encargo:
0 terreno · 1 rasantes · 2 superficies · 3 plataformas y cimientos · 4 casas · 5 encuentros · 6 vestido.
El mapeo es el de CITY-04 (`City04GreyboxBuilder.At`): **X = U, Z = V, Y = cota**.

## 2. Packs y piezas usadas (Quaternius, CC0)

| Pack | Piezas |
|---|---|
| Medieval Village MegaKit (proyecto URP del pack) | `Wall_UnevenBrick_{Straight, Window_Wide_Flat, Window_Thin_Round, Window_Wide_Round, Door_Flat, Door_Round}`, `Corner_Exterior_Brick`, `DoorFrame_{Flat,Round}_Brick`, `Door_1..8_{Flat,Round}`, `Window_Wide_Flat1-3`, `Window_Thin_Round1-3`, `WindowShutters_{Wide_Flat,Thin_Round}_{Open,Closed}`, `Balcony_Simple_Straight`, `Roof_RoundTiles_{4,6,8}x{4..14}`, `Roof_Tower_RoundTiles`, `Prop_Chimney/2`, `Prop_Vine1-9`, `Prop_Wagon`, `Prop_WoodenFence_Single`; texturas `T_UnevenBrick`, `T_RoundRocks`, `T_Brick`, `T_Plaster`, `T_WoodTrim` reutilizadas en la geometría propia |
| Nature | `CommonTree_1-5`, `Pine_1-5`, `Bush_Common`, `Bush_Common_Flowers` (hortensias), `Grass_Common_Short/Tall`, `Plant_1`, `Plant_1_Big`, `Plant_7`, `Fern_1`, `Rock_Medium_1-3` |
| Props | `Barrel`, `Barrel_Apples`, `Barrel_Holder`, `Bench`, `Bottle_1`, `Bucket_Wooden_1`, `Candle_1`, `CandleStick`, `Chair_1`, `Crate_Wooden`, `FarmCrate_{Apple,Carrot,Empty}`, `Lantern_Wall`, `Mug`, `Pot_1`, `Stool`, `Table_Large`; en la 2.ª iteración `Chandelier`, `SmallBottle(s)`, `Bag`, `Vase_2`, `Coin_Pile`, `Peg_Rack`, `Table_Knife` |
| Base Characters | cuerpos `Regular_{Male,Female}` y `Teen_{Male,Female}`, texturas de piel y ojos, peinados «Rigged to Head Bone»: base de los 13 vecinos vestidos (§8) |
| Universal Animation Library | clips `Idle_Loop`, `Walk_Loop`, `Jog_Fwd_Loop`, `Idle_Talking_Loop`, `Sitting_*`, `Counter_Idle_Loop`, `Idle_LookAround_Loop`, `Idle_FoldArms_Loop`, `Walk_Carry_Loop` |

**Descartadas a propósito:** todos los `Wall_Plaster_*` y `Roof_Front_*` (llevan entramado de madera, veto de la
Visual Bible), `Wall_WoodBrick/WoodWear/WoodGrid`, `Roof_Spikes*`, `Roof_Wooden*`, armas, pociones, monedas,
`TwistedTree` y las flores saturadas.

## 3. Derivados propios

- **Materiales** (`Assets/_Derived/Materials`, generados por `MatLib.cs`): teja oscura, madera oscura y
  contraventanas verde-azul por remapeo de luminancia de las texturas del kit; piedra, piedra mojada,
  albardilla y empedrado para la geometría propia; materiales URP para Nature/Props; agua.
- **Texturas generadas** (no versionadas): hierba húmeda, tierra, roca y grava tileables.
- **Shader** `Proto/OvercastSky`: cielo nublado con capa de nubes.
- **Geometría propia**: terreno y rasantes, muros de orilla con talud y pretil, muros de contención,
  tapias, zócalos y peldaños, hastiales de piedra que siguen la pendiente real de cada tejado, Puente Viejo
  (arco con dovelas, pretiles, bajada escalonada), pasaderas del vado, rampa del desembarcadero, barca,
  fuentes, puestos de mercado, farolas, ropa tendida, leña, estela de riadas, hornacina, puerta cegada,
  escalera del patio S03, interior y patio del bar, carteles de doble cara, montañas de fondo y caseríos
  lejanos.
- **2.ª iteración** (§8): vecinos vestidos (`Tools/blender/make_townsfolk.py`), 40 carteles pintados
  (`Tools/make_signs.py`), pajares con tablazón, medas, caballones de huerta, hogar con fuego, jamones,
  quesos y hogazas, revocos de cal/ocre/rosa y contraventanas de cuatro colores.

## 4. Huecos del kit (entrada para ART-01)

| Falta | Qué hice en su lugar |
|---|---|
| Tejado de poca pendiente o a cuatro aguas (los del kit rondan los 55°, pico de chalet) | aplané los tejados del kit a ~40° (escala Y 0,55) |
| Muro de revoco **sin** entramado; hastial de piedra | todo en `UnevenBrick`; hastial procedural |
| Teja oscura, madera oscura, contraventana pintada | remapeo de color de las texturas del kit |
| Ventana rectangular estrecha en muro de piedra | solo hay estrecha de medio punto o ancha rectangular |
| Balcón-solana con cortafuegos laterales | `Balcony_Simple` repetido a lo largo de la fachada |
| Puente de piedra, muro de orilla/contención, bordillo, cuneta | geometría propia |
| Farola de pie, fuente, puesto de mercado, barca, hortensia, ropa tendida, carteles | geometría propia o props adaptados |
| **Personajes vestidos** (los Base Characters vienen en ropa interior) | 2.ª iteración: ropa tallada del propio cuerpo en Blender + faldas, mandiles y boinas generados (§8) |
| Revoco liso, pajar de tabla, meda, género de tienda (pan, queso, embutido) | derivados procedurales (§8) |
| Pipeline de importación: Nature/Props en **centímetros** (×100) y con **materiales Standard** extraídos (magenta en URP); los árboles de terreno piden shaders de billboard | escala 0,01 + reasignación a URP en el importador; árboles con `LODGroup` para que Unity no genere billboards |

## 5. Cotas finales

| Punto | Brief | Final | Motivo |
|---|---:|---:|---|
| Lámina Río/Arroyo | 0 | 0 | — |
| W.LANDING | +1 | +1 | — |
| Pasaderas X5 | +0,5 | +0,5 | aguas altas a +1,15 las cubren |
| W.X5 / E.X5 | — | +1,8 | no venía en el brief; sale de W13 E1 |
| W.CASCO | +8 | +8 | — |
| W.X1 (cabeza del puente) | +7,5 | +7,5 | — |
| Lomo del Puente Viejo | ≈ +9 | +8,3 | con +9 la bajada a la orilla sur pasaba del 35 % |
| O.X1 (orilla sur) | +4 | +4,4 | la bajada escalonada del puente acaba a nivel del estribo |
| W.PLAZA | +12 | +12 | — |
| W.SHOP | +12,5 | +12,5 | — |

Pendientes resultantes: W12 casi llana (0,7 %), W05 ~5 %, W13 ~7 %, W06 media 7 % con rellanos cada 12 m
(la rasante es rampa + rellano, sin escalera nueva), W24 «Pasadizo del Arco» **~11 %** (la cota P8 de W.J13 no
casa con E0; se queda como rampa empinada). Senda del Arroyo (P8g) ~13 %.

## 6. Qué funciona y qué no como juego

Build: `Builds/PrototypeDemo/PuenteBar.exe` (Windows, 710 MB tras la 2.ª iteración). Capturas finales en
[`Captures/Final/`](Captures/Final/), tomadas **con el juego en marcha** (`-autocapture`), no en el editor.

**Cómo lo he comprobado.** No he jugado a mano. Todo lo que afirmo sale de la build ejecutada con captura
automática: 13 vistas fijas, el saludo a un NPC, el vado con aguas bajas y altas, y un **recorrido automático** que
conduce al jugador con su `CharacterController` real por el eslabón completo.

| Medida | Resultado |
|---|---|
| Recorrido Orilla sur → Puente Viejo → S02 → W12 → plazuela → traza B → dentro del Bar F01 | **completo, sin atascos**: 134 m en 94,5 s andando (≈ 1,4 m/s) — [`recorrido.txt`](Captures/Final/recorrido.txt) |
| Rendimiento (RTX 3060, 1600×900, vsync) | 1.ª iteración 44–60 fps; 2.ª iteración 42–60 fps, lo más caro sigue siendo el lomo del puente — [`fps.txt`](Captures/Final/fps.txt) |
| Escena | 1.ª iteración: ~100 casas (80 de relleno + 20 singulares), 56 huertas, ~4 900 árboles de bosque, 23 NPCs, 11 secretos. 2.ª: ver §8.3 |

El recorrido automático **encontró dos defectos reales** que ninguna captura mostraba: el muro de orilla y un
murete de borde de plataforma cruzaban la entrada norte del puente (el jugador se quedaba parado). Ambos están
corregidos; el log de atascos dice qué colisionador bloquea y dónde.

### Funciona

- **La lectura del eslabón.** Desde la Orilla sur el Casco se ve alzado sobre sus muros de orilla, con la torre del
  Ayuntamiento como referencia lejana. El puente con escalinata se lee como paso de peatones. W12 comprime entre
  fachadas de piedra y se abre en la plazuela; desde la plazuela se ve el bar con su luz cálida.
- **El bar en tres capas** (sala pública, puerta «PRIVADO» de servicio, patio visto por la rendija del callejón)
  se entiende sin texto. Se entra por la puerta real y el umbral.
- **El vado** muestra su estado: pasaderas a la vista con aguas bajas, agua que las cubre y paso bloqueado con
  aguas altas (tecla F); el HUD lo indica.
- **Los NPCs**, aunque sean maniquíes, dan escala y vida: pasean por las calles y el puente, charlan, están sentados
  o tras la barra, y dicen «¡Hola!» con E.
- **Los secretos** se disparan al acercarse y cuentan en el HUD.
- **Atmósfera**: cielo cubierto, niebla, montañas tipo Picos al fondo, piedra gris, empedrado con canalillo,
  hortensias, ropa tendida.

### No funciona o está a medias

- **Identidad cántabra parcial.** El kit sigue mandando: tejados de ~40° con teja rojiza oscurecida (no la teja
  cántabra), una viga de madera en cada planta que da aire de «medieval de catálogo», y ni solanas ni cortafuegos.
- ~~Solares vacíos~~ → resuelto en la 2.ª iteración (§8).
- **NPCs**: ya vestidos (§8), pero sin rutina: caminan de ida y vuelta por la calle y solo esquivan al jugador, no entre ellos.
- **Locales P9**: además del bar, en la 2.ª iteración se entra en taberna, horno, quesería y tienda; el resto solo tiene fachada.
- **Tejado de la torre**: desde la plaza apenas se ve el remate piramidal por la perspectiva.
- **Sin verificar por una persona**: la sensación de control (andar 1,4 / correr 3,5 m/s), la cámara en los rincones
  estrechos y la colisión en todos los escalones fuera del eslabón probado.
- En las capturas de revisión del **editor** los NPCs salen en pose T; eso solo pasa en el editor, no en el juego.

### Lista de FALLA del encargo

| Criterio | Estado |
|---|---|
| Casa cubo con ventanas o tejado pegados | no aparece: muros por módulos con hueco real, marco y contraventanas |
| Ventana o puerta sin hueco ni marco | no aparece (inserto del kit en hueco real + marco de piedra) |
| Tejado flotando o atravesando el muro | no visto; el tejado apoya en la coronación con hastial de piedra propio |
| Casa sin zócalo ni escalón | no: todas llevan zócalo hasta el terreno más bajo y peldaños |
| Carteles en espejo por detrás | **corregido** (shader de texto con cara trasera oculta; los colgantes tienen texto en cada cara) |
| Suelos coplanarios / z-fighting | evitado con rebajes de terreno bajo el puente, las escaleras y la rampa |
| Colisiones que no coinciden | el recorrido automático detectó dos y están corregidas; fuera del eslabón no se ha medido |
| Árboles de esfera+cilindro, colinas en cúpula, textura de suelo gigante | no (árboles de Nature, relieve con ruido y montañas facetadas; empedrado de 2,2 m) |


## 7. Qué hemos aprendido para el proyecto

### 7.1 Lecciones de construir la demo

1. **Lo que hace que parezca un juego no es el catálogo, es la composición.** Tener las 305 piezas del
   Medieval Village no produjo nada presentable por sí mismo. Lo que convirtió un plano de CITY-03 en un
   pueblo fueron cinco sistemas de composición, todos «compilador de mundo» y ninguno «bridge»:
   - un **marco de cotas compartido** (solver de alturas con calles y plazas fijadas y el resto relajado
     hacia un relieve natural);
   - el **tratamiento del agua** (máscaras exactas → agujeros de terreno + muros de orilla + valla
     invisible), que es lo que da el «pueblo sobre la roca»;
   - **reglas de montaje del kit** (módulos de 2 m, plantas de 3 m, pivote en la cara exterior, insertos
     con la misma transformación que el muro, tejados W×D, esquinales);
   - **evitar solapes** (terreno rebajado bajo puente y escaleras, huecos entre casas para que los aleros
     no se atraviesen);
   - **luz y atmósfera** (cielo nublado, niebla, SSAO, postproceso).
2. **La unidad de autoría útil es el plan, no el objeto de Unity.** ~300 líneas de datos (`Seed.cs`:
   polígonos, calles con cotas, parcelas) + reglas generan ~20 000 objetos de Unity. Editar el plan y
   recompilar (~80 s) fue la forma natural de iterar. Reconciliar 20 000 GameObjects con un estado
   canónico no aporta nada a ese bucle.
3. **La revisión visual automática encontró todos los defectos importantes.** Reconstruir → capturar
   desde 6 cámaras fijas + aérea → mirar contra la lista de FALLA detectó: ruido de textura en rejilla,
   splat perdido por el orden de creación del `TerrainData`, entramado alpino y teja roja del kit,
   césped en mitad del Casco, y la pantalla magenta. Ningún test estructural lo habría visto. Y un
   **recorrido automático** con el controlador real del jugador encontró lo que las capturas no ven: dos muros
   generados que cerraban la entrada del puente. Mirar y recorrer son las dos pruebas que más valor dieron.
4. **El conocimiento del kit se extrae de datos.** Las reglas de montaje (dónde va la ventana respecto al
   muro, a qué altura el tejado, cómo girar una esquina) salieron en minutos de volcar la escena de
   muestra de Quaternius (907 instancias). El «perfil dimensional» de ART-01 puede derivarse
   automáticamente en vez de escribirse a mano.
5. **Los problemas reales de adopción aparecen al usar muchos assets, no cuatro.** Nature/Props vienen
   en centímetros (×100), con materiales Standard extraídos que en URP salen **magenta** y cubrieron la
   pantalla entera; los árboles de terreno exigen shaders de billboard. H1-04 adoptó una muestra de
   4 archivos y no podía ver esto.
6. **Velocidad.** Una sesión produjo un pueblo recorrible de ~100 casas con puente, vado, bar con
   interior y NPCs. En 8 días el repositorio acumula ~2 160 commits, ~52 000 líneas de C# y ~52 500
   líneas de Markdown en 539 documentos, y ninguna escena jugable. El protocolo Worker → Reviewer →
   DocSync tiene sentido para las garantías del kernel H0; aplicado a exploración de contenido es
   desproporcionado.
7. **Deriva documental.** El encabezado del ROADMAP dice que el siguiente WP es H1-03 cuando H1-09 ya está
   aceptado. El volumen documental ya supera la capacidad de mantenerlo coherente.

### 7.2 ¿Hace falta completar H1 entero?

Quedan `H1-10` (checkpoint y reconstrucción limpia), `H1-11` (conformidad amplia con assets reales) y
`H1-GATE`. Recomendación: **no tal y como están planteados.**

- **H1-10** resuelve un problema que desaparece si la escena es un artefacto compilado: la reconstrucción
  limpia es la operación normal. Solo tiene sentido si se mantiene la sincronización bidireccional fina.
- **H1-11** busca lo que esta demo encontró en horas (escala ×100, materiales, billboards). Mejor como una
  lista de **normalizaciones de importación en datos**, ampliada bajo demanda de H2.
- **H1-GATE** podría sustituirse por una **puerta de demo**: una IA, usando solo interfaces públicas, hace
  un cambio significativo en el pueblo (p. ej. añadir una calleja con una tienda) y el resultado pasa la
  revisión visual y un recorrido jugable automático.
- Lo ya aceptado (H0 y H1-00…09) se conserva: identidad lógica, catálogo, diagnósticos y el canal de
  propuestas siguen siendo útiles como base.

### 7.3 Cómo plantearía H2

**H2 = compilador de mundo sobre una especificación canónica compacta.**

1. **Especificación canónica** (en el `WorldState` de H0, con validación, diff y procedencia): marco de
   cotas, red de calles con perfiles, plataformas, parcelas, planes de edificio (tipo, plantas, fachada,
   puerta, tejado), puntos de interés y rutas de NPC. CITY ya tiene todo esto en Markdown; pasa a datos.
2. **Compilador determinista a Unity**, con el constructor por capas de este prototipo como semilla:
   capas 0-6, perfil dimensional del kit derivado de datos, normalizaciones de importación.
3. **Bucle de iteración**: editar la especificación a través del harness → recompilar (idealmente solo la
   región afectada) → capturas fijas + recorrido automático (un bot que anda las rutas y mide tiempos,
   atascos y caídas) → revisión.
4. **Revisión visual como herramienta del harness**: cámaras por zona, lista de FALLA, diff de imagen
   entre versiones.
5. **Ediciones manuales en Unity** como *overrides* explícitos anclados a elementos del plan, en vez de
   reconciliación general.
6. **Arte bajo demanda**: los derivados que esta demo ya necesitó (teja/madera/contraventanas, muros sin
   entramado, hastial de piedra, ropa de NPC) son la primera lista de ART.

Hitos sugeridos: H2-A compilador + especificación del eslabón Puente→Bar · H2-B una IA itera sobre la
especificación · H2-C vida mínima (rutinas y rutas de NPC, interacción) · H2-D puerta jugable.

### 7.4 Infraestructura: qué sobra y qué falta

**Sobra (o está sobredimensionado para esta fase):**
- el protocolo completo Worker/Reviewer/DocSync con recibos por SHA para trabajo de contenido y diseño;
- la planificación documental masiva antes de construir (CITY-00…09, enmiendas P1…P9, ART-01, entradas
  de planificación H2): la demo validó o falsó esa geometría en horas (cotas del puente, W24 al 11 %,
  parcelas P9 pequeñas);
- la sincronización bidireccional fina Unity↔canónico (drift, reconciliación, checkpoint) si el mundo
  se compila.

**Falta:**
- el compilador de mundo y el perfil dimensional del kit derivado de datos;
- revisión visual automatizada y recorridos jugables automáticos;
- un pipeline de importación normalizado (escala, materiales URP, LOD);
- una build jugable continua (build + autocaptura en runtime);
- un único tablero de estado generado desde los WPs, para que el ROADMAP no derive.

### 7.5 ¿Giro de 180°?

Mi recomendación es un giro de **~90°, no de 180°**:

1. **Conservar** H0 (kernel sólido) y lo aceptado de H1.
2. **Declarar H1 suficiente para H2** tras H1-09 (o con un GATE reducido) y mover H1-10/11 a demanda.
3. **Dos carriles de proceso**: *kernel* (protocolo completo, como ahora) y *juego/contenido* (una sesión
   por tarea, commits pequeños, revisión visual + recorrido automático, sin DocSync formal).
4. **H2 empieza portando este prototipo** a compilador sobre especificación canónica.

Esta propuesta **no está aplicada**: no he tocado el ROADMAP ni ningún documento canónico. Si la apruebas,
el siguiente paso es convertir la §7 en una enmienda de proceso/roadmap revisada por el flujo normal.

## 8. Segunda iteración: personas, carteles, pueblo lleno e interiores

Encargo: jugador y vecinos como personas y no maniquíes; más variedad con los assets que ya hay y con
derivados propios coherentes, sin ceñirse tanto al modelo de pueblo; ningún solar vacío (no habrá city
builder); interiores con vida; carteles nuevos.

### 8.1 Qué cambia

| Tema | Antes | Ahora |
|---|---|---|
| Personajes | maniquíes UAL tintados | 13 vecinos vestidos derivados de Base Characters con `Tools/blender/make_townsfolk.py`: la ropa (camisa, jersey, chaleco, chaqueta, pantalón, calzado) se talla del propio cuerpo por hueso dominante, así hereda los pesos y se anima con los clips UAL; faldas, mandiles y boinas son geometría generada y pesada al esqueleto; el pelo es del pack. El jugador es el «Forastero»; camarero, tendera, panadera, quesero… van vestidos según su papel, y las variantes repetidas cambian de tono |
| Carteles | texto 3D con un shader propio | 40 texturas pintadas con `Tools/make_signs.py` (Pillow y tipografías del sistema): colgantes de hierro y madera legibles por los dos lados, rótulos de fachada, placas esmaltadas de calle, placas de piedra, carteles de fiestas, bolos y feria, pizarra del bar |
| Solares | interiores de manzana vacíos y huertas casi desnudas | 48 edificios traseros (cuadra con pajar de tabla y boca de pajar, casa de dos plantas, caseta), una granja en la orilla sur, huertas con caballones y filas densas, frutales, corrales con meda y carro, prados con frutales, arbustos y medas |
| Variedad | todo piedra, contraventanas verdes | ~45 % de las casas revocadas (cal, ocre, rosa gastado) con esquinales y zócalo de piedra; contraventanas verdes, granate, azules o de madera; balcón simple o de cruces; pajares con hastial de tabla |
| Interiores | bar con estanterías vacías y cajas blancas por lámparas | bar con hogar encendido (luz que parpadea), lámparas de hierro, botellas, jamones, carteles, pizarra y dos parroquianos sentados; se entra en la **taberna, el horno, la quesería y la tienda de comestibles**, cada una con mostrador, género (botellas, hogazas, quesos, cajas de fruta, sacos) y su tendero |

Capturas: `Captures/Final/` (las de la 2.ª iteración sustituyen a las anteriores; se añaden el hogar del bar,
los cuatro interiores, la orilla sur y las traseras).

### 8.2 Tres defectos que la primera revisión no vio

1. **El PR no llevaba el generador.** El `.gitignore` del proyecto tenía `[Bb]uild/`, que también excluía
   `Assets/_Prototype/Editor/Build/`: los primeros commits subieron capturas, informe y scripts de runtime, pero
   no el código que construye la escena. Corregido (patrones anclados a la raíz del proyecto) y comprobado con
   `git ls-files`.
2. **Atrezo invisible.** Los modelos de Props traen escala raíz ×100 (están en centímetros) y `Kit.Put` la
   sustituía cuando se le pasaba una escala: botellas, macetas, bancos, cajas del mercado y los faroles de las
   farolas medían milímetros. Ninguna captura lo delataba porque lo que falta no se ve. Ahora la escala se
   multiplica.
3. **Todos los humanoides andaban de espaldas**, también los maniquíes de la primera iteración: el
   importador de clips usaba la orientación original de la raíz, y Quaternius la exporta girada 180°. Con
   maniquíes sin cara no se notaba; con personas saltó a la vista en la primera captura de un tendero. Ahora
   se usa la orientación del cuerpo.

### 8.3 Coste y estado

- Escena: 148 edificios (80 de relleno, 20 singulares, 48 traseros), 50 huertas, ~170 elementos de prado,
  29 NPCs (23 en la calle, 2 sentados en el bar y 4 tenderos).
- Rendimiento (RTX 3060, 1600×900): al añadir los edificios cayó a ~40 fps en las vistas del puente; con
  sombras a 110 m y 3 cascadas, y sin sombras en el atrezo pequeño, queda en 42–60 fps (42 en el lomo del puente, ~49 en la subida W12 y en la aérea, 56–60 en el resto).
- Recorrido automático completo y sin atascos: 134 m en 94,5 s; además, 30 de 30 humanoides mirando hacia donde andan ([`orientacion.txt`](Captures/Final/orientacion.txt)) y ningún render microscópico en la comprobación de presencia de la build.
- Sigue **sin jugarse a mano**.

### 8.4 Qué no funciona todavía

- La ropa es muy ceñida (el cuerpo desplazado unos milímetros y suavizado). Las faldas se aplastan al
  sentarse, por eso los parroquianos sentados llevan pantalón.
- Faldas y mandiles son rígidos, pesados a pelvis y muslos: en una zancada larga la rodilla puede asomar.
- Hogazas, quesos, jamones y el horno son procedurales sencillos: aguantan a distancia de juego, no un
  primer plano.
- Los revocos se aplican sobre el muro de mampostería del kit (el relieve de la piedra sigue debajo): se lee
  como piedra encalada, no como revoco liso.
- Las tiendas no tienen trastienda ni planta alta, y el género no tiene colisión fina.

### 8.5 Lecciones añadidas

1. **Lo que falta no sale en las capturas.** Hacen falta comprobaciones automáticas de presencia y
   orientación: tamaño mínimo de cada prop colocado, personaje mirando hacia donde anda y archivos que la
   build usa pero git no versiona. Las tres se detectan en segundos con una prueba corta; ninguna revisión
   visual las encontró.
2. **Derivar es barato cuando hay pipeline.** Con Blender en modo batch salen 13 personajes vestidos en
   ~1 min desde un solo pack, y se regeneran al cambiar una tabla; los carteles, igual con Pillow. Es la vía
   más rápida para dar identidad sin encargar arte. La lista de ART-01 debería distinguir lo derivable por
   script de lo que hay que modelar.
3. **Rellenar por reglas funciona.** Casas traseras, huertas, frutales y medas colocados por ocupación y
   distancia a la calle llenaron los huecos en una pasada, sin tocar la semilla. No hace falta un city builder
   para que no haya solares: basta una capa de relleno en el compilador de mundo (§7.3).
4. **El rendimiento se mide en cada iteración.** Un 50 % más de edificios costó un 25 % de fps; recortar
   sombras recuperó casi todo.
