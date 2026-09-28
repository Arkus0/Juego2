# Game Creator Hub — auditoría de reutilización para Juego2

Fecha de observación: **28 de septiembre de 2026**. Estado: **discovery; no adopción ni instalación**. Fuente primaria de cada extensión: su ficha autenticada y su código visible en [Game Creator Hub](https://gamecreator.io/hub). La ficha, versión, autor, enlace, dependencia, evidencia, tipo, riesgo y recomendación de las **77 extensiones abiertas** están en [GC2_HUB_EXTENSION_CATALOG.csv](GC2_HUB_EXTENSION_CATALOG.csv). Las **643 tarjetas distintas por nombre y versión** vistas durante el rastreo, incluidas las descartadas superficialmente, figuran en [GC2_HUB_COVERAGE_INVENTORY.csv](GC2_HUB_COVERAGE_INVENTORY.csv).

## 1. Executive Summary

**El Hub sí importa a Juego2, pero como biblioteca de piezas pequeñas y código fuente adaptable, no como solución terminada de ciudad viva.** Los mayores retornos están en pasar `Self/Target` entre acciones reutilizables, consultas sobre grupos, eventos alrededor de módulos que se adopten por razones de producto y un posible importador de diálogo. No se encontró un planificador de rutinas de NPC, simulación fuera de pantalla, identidad persistente o gestor robusto de multitudes listo para integrar. Esos límites siguen perteneciendo al diseño GC2/Unity + autoridad persistente Arkus que establece el roadmap vigente.

El triage de 77 fuentes: **9 USE, 7 ADAPT, 37 WATCH, 9 IGNORE y 15 REJECT**. Son recomendaciones **condicionadas a la versión efectiva de GC2 y al workpack correspondiente**; USE significa “buen candidato para una prueba de adopción”, no paquete ya admitido. De las 77, 50 son Core-only por imports/tipos observados, 24 requieren un módulo oficial, 2 un tercero y 1 combina Dialogue con código no incluido. Los contadores de módulos más abajo excluyen IGNORE/REJECT.

El ahorro **no está medido** en este proyecto. Como hipótesis de planificación, escoger 5–10 piezas apropiadas en varios WPs podría evitar unas **10–30 jornadas de prototipos, pegamento y repetición de autoría**, con riesgo de integración y depuración que puede reducir ese ahorro. No sería honesto traducirlo a líneas de código ni descontarlo de la producción de escenarios, animación, escritura o la residencia persistente de 60–100 NPC. Antes de contabilizar el ahorro, medir el mismo caso con la solución nativa y con la extensión, incluida la preparación de contenido por un autor nuevo.

**Decisión de producto:** incorporar una comprobación breve “GC2 nativo → Hub → adaptar fuente → código propio” al inicio de cada WP que vaya a añadir una mecánica GC2. En particular, hacerlo en GC2-DIALOGUE-00, GC2-02/03/04/05/06 y GC2-08. Mantener en esos WPs sus gates de licencia, versión exacta, H1/H2F y autoridad Arkus. Este discovery **no modifica ni desbloquea** los WPs aceptados; la enmienda de contratos debe pasar revisión independiente antes de ser binding.

## 2. Metodología

1. Sesión iniciada por el owner en el navegador. Navegación de solo lectura; ninguna descarga, instalación, compra, cambio de cuenta o ejecución Unity.
2. Inventario inicial de 150 tarjetas; 40 búsquedas temáticas sobre NPC, navegación, proximidad, Behavior, Perception, interacción, Dialogue, Quests, variables, guardado, escenas, cámaras, animación, audio, Inventory, tiendas, tiempo, pooling, autoría y diagnóstico. Deduplicación por nombre + versión: 643 tarjetas observadas; 615 nombres distintos.
3. Selección de 77 fichas relevantes o ejemplares de riesgo; lectura de metadatos, dependencias declaradas, parámetros y código fuente visible. Para otras 566 tarjetas solo consta el texto de la tarjeta, no un juicio de código.
4. Contraste con documentación oficial de GC2 de [On Interval](https://docs.gamecreator.io/gamecreator/visual-scripting/triggers/events/lifecycle/on-interval/), [Move To](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/characters/navigation/move-to/), [Collect Characters](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/variables/collect-characters/), [Run Actions](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/visual-scripting/run-actions/), [Loop List](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/variables/loop-list/), [Save & Load](https://docs.gamecreator.io/gamecreator/advanced/save-load-game/), [Load Scene](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/scenes/load-scene/), [Perception Sight](https://docs.gamecreator.io/perception/sensors/sight/) y [Behavior](https://docs.gamecreator.io/behavior/).
5. Clasificación por **evidencia del badge Dependencies o del source**, nunca por el título aislado. `CONFIRMED` indica dependencia declarada por la ficha; `INFERRED_FROM_SOURCE` señala namespace/tipo/import, incluso para Core-only; `UNKNOWN` se reserva a evidencia insuficiente. Una ficha sin badge no prueba ausencia de módulo. `Game.DialogueActors` aparece importado pero no se ha establecido dónde obtenerlo ni su licencia.

Las fichas muestran fechas relativas (“hace 3 meses”, etc.); el CSV conserva esa forma observada. Un badge con versión es evidencia de **requisito declarado por el autor**, no garantía de compatibilidad con la instalación actual. No se compiló ni ejecutó ningún source.

## 3. Estado del catálogo auditado y límites

| Cobertura | Resultado |
|---|---:|
| Tarjetas de portada | 150 |
| Consultas del buscador | 40 |
| Tarjetas únicas nombre + versión observadas | 643 |
| Nombres únicos | 615 |
| Fichas abiertas y código visible inspeccionado | 77 |
| Fichas con prueba Unity/versión efectiva | 0 |

La portada presenta los tipos Instructions, Conditions y Events; las fichas abiertas abarcan Lifecycle, Logic, AI, NPCs, Navigation, Variables, Storage, Dialogue, Quests, Perception, Behavior, Inventory, Scenes, Cameras, Audio, Physics y Tooling. El buscador devuelve hasta **150 coincidencias** para términos amplios como “variable” y “time”; mezcla resultados por coincidencias imprecisas y puede tardar en actualizar. No encontré un contador global ni una paginación verificable que demuestre el total del Hub. Por tanto, **643 observadas no equivale a “todo el Hub”**. Las fichas no abiertas quedan como inventario de triage, sin dependencia ni calidad inventadas.

Consultas reproducibles: `routine, schedule, tick, navigation, proximity, radius, state machine, behavior, perception, interaction, dialogue, quest, investigation, evidence, variable, save, scene, camera, animation, audio, inventory, shop, minigame, time, calendar, npc, wander, waypoint, crowd, pool, optimized, lod, stream, import, author, debug, merchant, door, zone, signal`. `npc` devolvió cero pese a tarjetas NPC halladas en otras consultas: la búsqueda es inconsistente y un cero **no prueba ausencia**.

## 4. Top findings

- [Run Actions with Args](https://gamecreator.io/hub/OMhE9O5rN06g15fuJvpt) (**Core; USE**) agrega `Self` y `Target` parametrizados a la ejecución de Actions. GC2 [Run Actions nativo](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/visual-scripting/run-actions/) expone el componente y la espera, pero no estos dos parámetros en su ficha. Favorece acciones compartidas entre testigo, objeto y jugador.
- [For Each in Radius](https://gamecreator.io/hub/B0zttlE8GcXMrElOtYd5) (**Core; ADAPT**) ejecuta Actions sobre targets espaciales sin preparar una lista. Es una mejora de authoring sobre coleccionar/iterar para ciertos casos, pero usa `Collider[128]`, `OverlapSphereNonAlloc`, `HashSet`/`List` **static** y `await` durante la iteración: puede truncar y mezclar ejecuciones simultáneas. Copiar la idea tras corregir propiedad de buffers, tope y reentrancia.
- [Build Dialogue From Text](https://gamecreator.io/hub/ObFYrBKs5XOdSJ8r5ibF) (**Dialogue + `Game.DialogueActors`; ADAPT condicionado**) transforma texto indentado con opciones en nodos. El source importa `GameCreator.Runtime.Dialogue` y `Game.DialogueActors`, escribe `Node.m_Acting` y `Acting.m_Actor` mediante reflection, borra nodos existentes y busca skin vía `AssetDatabase` solo en Editor. Gran potencial para el escritor, pero **no es un paquete autocontenido ni una vía de producción inmediata**. Requiere disponer lícitamente de Dialogue 2 y resolver el source de actores, compilación, builds y roundtrip.
- [Preload Scene](https://gamecreator.io/hub/AO3J4xiD3qZIe5YLjJJ3) + [Activate Preloaded Scene](https://gamecreator.io/hub/QEC2LOG5V7OAmbuadvmG) (**Core; ADAPT**) aportan precarga con activación diferida. `PreloadedScene` es una sola `AsyncOperation static`, de modo que el protocolo no sirve tal cual para varias escenas o cancelaciones y no sustituye [Load Scene nativo](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/scenes/load-scene/) en transiciones simples.
- [Wander Prefered Paths](https://gamecreator.io/hub/qKncnMkkyxzBQf16LFfk) y [Move to Random Marker](https://gamecreator.io/hub/vMGN5lYfvHlWb53e0ndy) (**Core; ADAPT**) pueden ayudar a montar rutas legibles. La primera ordena aleatoriamente rutas y espera colecciones manuales de GameObjects; la segunda fija distancia 4 en `MoveToMarker`. Ninguna representa un horario, memoria ni avance fuera de pantalla.
- [On Update Rate](https://gamecreator.io/hub/nVlBink5S47a1rWCR5nF) (**Core; WATCH, LOW VALUE**) permite Hz y catch-up con máximo por frame, pero recibe `OnUpdate` cada frame. GC2 Core ya tiene [On Interval](https://docs.gamecreator.io/gamecreator/visual-scripting/triggers/events/lifecycle/on-interval/) con Time Mode e Interval. Usar primero el nativo; el catch-up solo si la semántica exige recuperar pasos perdidos.
- [Manage NPC Visibility and Pooling](https://gamecreator.io/hub/VM7Fw6aHFuEau3Nc03l7) (**Core; REJECT directo**) declara “alpha untested”, ejecuta lógica por frame con NavMesh y raycasts y activa/desactiva objetos sin prueba de identidad persistente. No resuelve el contrato de residencia GC2-06.
- [Trigger At Specific Time](https://gamecreator.io/hub/c1jgU1m4kjJirL5a82qV) (**Core; REJECT para horarios**) lee `DateTime.Now` del dispositivo en `OnUpdate`, no la hora del mundo del juego. Es una falsa solución para rutinas.

## 5. Core-only winners

**Adopción candidata inmediata, una vez llegue el WP:** `Run Actions with Args`, `Switch (String)` y `Set Audio Mixer Group Volume`. **Source reutilizable tras adaptar:** `For Each in Radius`, `Preload Scene`/`Activate Preloaded Scene`, `Wander Prefered Paths` y `Move to Random Marker`. **Probar solo ante caso concreto:** `Check Object in Range`, `Text with Dialog System`, `Change Interact Radius`, `Set Character Interaction Mode`, `Force Third Person Alignment`, `Play Gesture (Extended)`.

La clase CORE_ONLY en el CSV significa que el **source visible** solo importa tipos GC2 Core/Unity conocidos y no muestra otro requisito; no es una compilación probada. TextMeshPro y UnityEditor son paquetes habituales de Unity, pero la plantilla con UnityEditor necesita ensamblado Editor. Para `Wander` se necesita NavMesh Agent de Unity; para `Preload Scene`, escenas configuradas en el proyecto.

## 6. GC2 MODULE DEPENDENCY MATRIX

Conteos sobre fichas **relevantes** (`USE`/`ADAPT`/`WATCH`); un mismo paquete puede aparecer en un módulo y en MULTI-MODULE. Los `IGNORE`/`REJECT` se conservan en el CSV pero no inflan el argumento de compra.

| Módulo o clase | N.º | Extensiones relevantes | Aumenta el valor de adoptar el módulo |
|---|---:|---|---|
| **CORE ONLY** | 32 | Destacan `Run Actions with Args`, `For Each in Radius`, precarga, `Wander Prefered Paths`; lista completa filtrable en CSV | No supone compra |
| **DIALOGUE** | 3 | `Build Dialogue From Text`, `Clear Dialogue Visits`, `Is Dialogue UI Open` | **Moderado y condicionado**: el importador sería valioso tras reparar source; la razón primaria debe ser la presentación/conversación de Juego2 |
| **PERCEPTION** | 5 | `NPC Sees Player in Zone`, `On Feel (Duration)`, `Save Can See Result`, `Set Feel Radius`, `On Luminance Compare` | **Moderado** para persecución/reacciones; el módulo nativo [ya controla Sight por intervalo](https://docs.gamecreator.io/perception/sensors/sight/), más decisivo que los wrappers |
| **BEHAVIOR** | 2 | `Add Goal (Property)`, `Flee` | **Pequeño** por Hub; el valor depende de necesitar su [State Machine/GOAP/Utility/BT](https://docs.gamecreator.io/behavior/) en actores concretos |
| **INVENTORY** | 4 | `On Buy from Merchant`, `On Sell from Merchant`, `On Open Target Bag UI`, `On Close Target Bag UI` | **Moderado** si GC2-04 adopta objetos portátiles y GC2-06 tiendas; no comprar para estos eventos solamente |
| **QUESTS** | 4 | `Has Quest Started`, `Is Quest Tracked`, `On Any Task Activate From Quest`, `On Any Task Completed From Quest` | **Moderado** para subhistorias si la gestión nativa de [Quests](https://docs.gamecreator.io/quests/) aporta al producto; Arkus conserva consecuencias |
| **ABILITIES** | 1 | `Enter Reactive State` | **Nulo ahora**: una instrucción no justifica módulo |
| **MULTI-MODULE / módulo + source adicional** | 1 | `Build Dialogue From Text` = Dialogue + `Game.DialogueActors` (origen pendiente) | Aumenta interés de un spike, no justifica compra |
| **THIRD PARTY** | 2 | `Load Scene with Transition` = Transitions Plus 4.0.0; `Run State Machine Runner Node Advanced` = NinjutsuGames.StateMachine | No justifica compra/integración; evaluar solo necesidad específica |

Quests, Inventory, Dialogue y Behavior son nombres **exactos** vistos en badges y/o source; también se observó el módulo **Abilities**. Shooter/Melee aparecen en otras tarjetas de la cobertura amplia, pero no entre las extensiones aquí consideradas valiosas para la fase actual y por eso no tienen fila artificial. No se halló requisito de módulo GC2 adicional en las 50 fuentes Core-only abiertas. Un badge ausente y un import ausente no demuestran compatibilidad universal.

## 7. Third-party dependency findings

- `Game.DialogueActors` es un namespace importado por Build Dialogue From Text. **Desconocemos si es código del autor, otro paquete o un namespace faltante**. Antes de compilar, localizar fuente/licencia/ensamblado; no copiar una implementación sustituta presuponiendo la API.
- `Transitions Plus 4.0.0` está declarado por el Hub en Load Scene with Transition. Es presentación; GC2 Core carga escenas y el roadmap ya tiene lenguaje visual. WATCH.
- `NinjutsuGames.StateMachine.Runtime` está en el source de Run State Machine Runner Node Advanced. Es un State Machine externo, **no es Behavior 2**. WATCH por dependencia adicional y solapamiento.

## 8. COMPOSABLE STACKS

| Stack y dependencias | Resultado de juego | Ahorro posible y condición | Riesgo |
|---|---|---|---|
| Core `On Interval` nativo + `Check Object in Range` o `Collect Characters` nativo + `Run Actions with Args` | Interacciones locales escalonadas por proximidad | Medio: diseñador monta reacción sin código nuevo | El evento de radio inspeccionado sigue comprobando por frame; no fingir simulation LOD |
| Core `Wander Prefered Paths` + `Move to Random Marker` + Variables/Marker + reloj de juego **por definir** | Rutina visible de GC2-05 con dos POI | Medio/alto en authoring de rutas tras adaptación | No hay scheduler listo; actor fuera de pantalla se representa mediante estado/POI Arkus, no GameObjects ocultos activos |
| Core `For Each in Radius` adaptado + `Switch (String)` + `Run Actions with Args` | Evento sistémico sobre grupo cercano | Alto para eventos escritos por diseñadores | Buffer 128/reentrancia/ejecución async; presupuestar distancia y frecuencia |
| Dialogue 2 **solo si admitido** + `Build Dialogue From Text` adaptado + Actors + Variables/Arkus | Importación de escenas de diálogo redactadas en texto | Potencial alto para el escritor, no cuantificado | Falta `Game.DialogueActors`; reflection, destrucción de nodos, validación de ramas y skin en build |
| Perception 2 **solo si admitido** + Sight nativo con intervalo + `Save Can See Result` + `NPC Sees Player in Zone` adaptado | Testigo/guardia que ve una intrusión o sospechoso | Medio para GC2-08 | La conciencia depende de sensores configurados; no escanear todos los NPC por frame |
| Core `Preload Scene` + `Activate Preloaded Scene` adaptados + Load Scene/Entries nativos | Transición entre interiores o distritos | Medio si la precarga realmente evita pausas | Singleton `AsyncOperation`, activación, cancelación y restore de identidad; benchmark de memoria |
| Inventory 2 **solo si admitido** + eventos de bolsa/merchant + Core Actions con Args + recibo Arkus | Tienda o prueba portátil con resultado duradero | Medio para GC2-04/06 | Item runtime/Bag no se convierte en identidad ni verdad de investigación Arkus |
| Quests 2 **solo si admitido** + eventos de tareas + `Has Quest Started` + recibo Arkus | Subhistoria con progreso visible y consecuencias | Medio para GC2-03+ | Evitar doble progreso tras reload; no confundir Journal con autoridad causal |

Las filas son **composiciones propuestas**, no integración comprobada. Un stack no reduce el gate de licencia o versión del módulo del que dependa.

## 9. Performance-related findings

| Fuente | Evidencia del código visible | Decisión |
|---|---|---|
| For Each in Radius | `Collider[128]`, `Physics.OverlapSphereNonAlloc`, aviso al llenarse; `s_Targets` y `s_ProcessedRoots` static; `await m_Actions.Run` | ADAPT; buffers por ejecución, resultado de overflow explícito, snapshot de targets y prueba de dos invocaciones simultáneas |
| Gather Target Candidates In Range | `Collider[30]` static y `OverlapSphereNonAlloc`; añade Combat targets | IGNORE para multitudes; dominio de combate distinto |
| Multiple NPCs See Player in Zone | `GameObject.FindGameObjectsWithTag` durante comprobaciones | REJECT directo con muchos NPC |
| Track by Layers Optimized | `FindObjectsByType<GameObject>` global al refrescar caché estática | REJECT como “optimización” sin política de invalidación |
| Vision Detection | `OnFixedUpdate`, `FindGameObjectWithTag`, arrays de vértices/triángulos y raycasts | REJECT; usar Perception si se adopta |
| Simple Field Of View | `Physics.OverlapSphere` con asignación y raycast; un target | IGNORE para percepción escalada |
| Manage NPC Visibility and Pooling | Alpha no probado, `OnUpdate`, `NavMesh.SamplePosition`, raycasts y pool visual | REJECT directo; no cumple residencia GC2-06 |
| Play Speech | `new float[channels]` en ruta `OnAudioFilterRead` al cambiar canales | WATCH; probar voz/timbre y callback, no requerimiento H2 |

El Core [On Interval](https://docs.gamecreator.io/gamecreator/visual-scripting/triggers/events/lifecycle/on-interval/) y Perception [Sight/Feel por intervalo](https://docs.gamecreator.io/perception/sensors/feel/) merecen prioridad frente a nuevos loops por NPC. “Optimized” en el título no es benchmark. Medir actores simultáneos, cadencia, alloc/frame, consultas físicas y continuidad al atravesar seam en GC2-06.

## 10. Narrative/Dialogue findings

La oportunidad real de Build Dialogue From Text es que el escritor redacte líneas, hablantes y elecciones en texto y vea una escena jugable. Su versión 0.0.1 genera el árbol **en runtime**, borra `RootIds` anteriores y accede a campos privados de actores. Para convertirse en pipeline de producción tendría que producir/validar assets o una representación estable con IDs, errores de parsing, traducción, diff, revisión de ramas y prueba en build sin `AssetDatabase`. El texto de ejemplo de la ficha no es evidencia de que el writer workflow funcione en Juego2.

`Text with Dialog System` es una opción Core/TMP para frases secuenciales si la estrategia de GC2-DIALOGUE-00 no adopta Dialogue 2; requiere comparar el diseño final de ART-UI-01. `Clear Dialogue Visits` e `Is Dialogue UI Open` requieren Dialogue; `Change Dialogue Node Duration` usa reflection y no debe instalarse sin reemplazar esa vía. GC2-03 conserva el **único** punto de adopción Dialogue en GC2-DIALOGUE-00; este informe no lo modifica.

## 11. NPC/AI findings

El Hub aporta waypoints, percepción contextual, condiciones y wrappers de Behavior, pero **no apareció un sistema listo de horario de ciudad ni un scheduler de residencia** en las búsquedas realizadas. `Wander Prefered Paths` ayuda a authored paths, mientras `Trigger At Specific Time` usa la hora real del dispositivo y debe excluirse de rutinas. `Is Current State` llega a `Processor.m_Graph` privado; `Add Goal (Property)` exige Behavior 2 y solo añade valor incremental a su Add Goal nativo. `Flee` también usa Behavior 2; evaluarlo cuando exista persecución real.

Perception 2 tiene sensores nativos con intervalo y awareness; sus cinco wrappers relevantes no justifican comprarlo por sí mismos. Valorar el módulo por testigos reactivos y persecución GC2-08, con densidad y coste medidos. El actor que sale de la residencia activa en GC2-06 debe avanzar su horario/POI abstracto sin Animator/NavMesh vivos y rematerializarse con identidad y consecuencia intactas. Ningún paquete auditado demuestra esa semántica.

## 12. World/scene findings

El [Load Scene de Core](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/scenes/load-scene/) ya soporta modo Single/Additive, async y Scene Entries. El par Preload/Activate solo añade valor si se necesita precarga con activación diferida y supera mediciones. Su `AsyncOperation static` requiere owner por escena y cleanup. `Save and Load Scene by Layers` recorre GameObjects y crea un sistema paralelo de archivo: REJECT por identidad, restauración y autoridad. `Load Game (No Scene Reload, Reflection)` atraviesa internals del save manager: REJECT aunque parezca resolver viajes de distrito. `Load Scene with Transition` necesita Transitions Plus externo; WATCH por estética, no por streaming.

## 13. Tooling/authoring findings

`Run Actions with Args` ofrece la mejora de autoría más simple y verificable: una sola acción reusable con dos roles explícitos. `Switch (String)` compacta un branching moderado. `Advanced Instruction Template` contiene UI Editor y runtime en el source mostrado; solo sirve como referencia tras separar ensamblados. `Instructions Container` y `Loop List Get Index` deben demostrar ventaja concreta frente a Actions y [Loop List nativos](https://docs.gamecreator.io/gamecreator/visual-scripting/actions/instructions/variables/loop-list/). El catálogo no muestra un importador documental probado para Quests ni un debugger de rutina citywide.

## 14. Risks y reglas de adopción

- **Versiones:** badges antiguos mencionan Quests 2.0.1/2.3.8, Inventory 2.0.1, Dialogue 2.0.6/2.2.8, Behavior 2.0.1 y Abilities 2.0.1. Son mínimos declarados por autores, no pin del proyecto. Se necesita compilación y juego contra versión exacta admitida.
- **Reflection privada:** Build Dialogue From Text, Change Dialogue Node Duration, Is Current State, Run All Conditions, Set NavMesh Agent Priority y restauradores selectivos son frágiles ante updates. No hacerlos dependencia keeper sin API pública o fork reparado y fixture específico.
- **Frecuencia y reentrancia:** evitar `FindObjects`/Physics por actor y frame; evitar buffers globales compartidos con `await`; emitir diagnóstico de overflow.
- **Editor/build:** source con `UnityEditor` o `AssetDatabase` precisa separar Editor del runtime y ejecutar build; no presuponer que la ficha del Hub entrega un UPM válido.
- **Autoridad y persistencia:** GC2/Unity gestiona interacción, cámara y navegación local; Arkus conserva identidad/factos persistentes según los WPs vigentes. Un plugin de save, bolsa o Quest no reemplaza esa frontera.
- **IP/provisión:** el Hub enseña código; no da por sí solo derecho a incluir módulos de pago o source externo faltante. Aplicar `Docs/engineering/DEPENDENCY_IP_POLICY.md` y el gate H1/H2F pertinente antes de incorporar bytes.

## 15. Recommended adoption order

1. **GC2-00 / GC2-02:** comparar `Run Actions with Args` y `Switch (String)` con Core en una acción testigo. Si simplifican authoring y compilan con la versión admitida, adoptar source aislado.
2. **GC2-DIALOGUE-00:** decidir presentación Dialogue 2 por producto. Hacer spike de Build Dialogue From Text **solo** si existe paquete lícito y se resuelve `Game.DialogueActors`; si no, probar Core/TMP sin bloquear H2.
3. **GC2-04:** si Inventory 2 supera su decisión propia, evaluar eventos de Bag por actor; los eventos Merchant esperan una tienda real de GC2-06.
4. **GC2-05:** prototipar dos POI y cambio por interacción con `Wander Prefered Paths`/`Move to Random Marker` adaptados, usando On Interval nativo y reloj de juego explícito. No integrar pooling visual como atajo.
5. **GC2-06:** comparar `For Each in Radius` corregido y Core Collect Characters; medir residencia y el par de precarga solo si el bloque tiene seam real.
6. **GC2-08:** evaluar Perception 2 por persecución y testigos. Después considerar `Save Can See Result`, `On Feel (Duration)` y zona con intervalos medidos.
7. Quests y Behavior solo si subhistorias/IA concreta justifican sus módulos, no por el número de paquetes en el Hub.

Para formalizar la regla de reutilización en el roadmap, añadir en cada WP aplicable una **nota de decisión de implementación** de una página: necesidad del caso, alternativa GC2 nativa, candidatos Hub con versión/dependencia exacta, evaluación de source, coste de authoring + runtime, decisión USE/ADAPT/IGNORE y responsable de autoridad. Es una comprobación acotada al mecanismo que se pretende escribir, **no una auditoría completa del Hub repetida en cada WP**. Dejar intactas las decisiones de compra/adopción que ya tienen dueño exclusivo.

## 16. Packages to avoid

| Paquete | Motivo causal |
|---|---|
| [Manage NPC Visibility and Pooling](https://gamecreator.io/hub/VM7Fw6aHFuEau3Nc03l7) | Alpha no probado; no simula identidad ni horario |
| [Trigger At Specific Time](https://gamecreator.io/hub/c1jgU1m4kjJirL5a82qV) | Reloj del sistema, no reloj del juego |
| [Load Game (No Scene Reload, Reflection)](https://gamecreator.io/hub/jmyhJNqT2IKfFYMXetIs) | Internals del save manager y estado potencialmente inconsistente |
| [Load Name Variables data from GC2 Save](https://gamecreator.io/hub/S9ty7EeSeaDIEJiZ1gvN) / [Load All selected...](https://gamecreator.io/hub/baEjwD88cx7yJeFVy4t7) | Reflection sobre almacenamiento interno; Core ya guarda variables |
| [Vision Detection](https://gamecreator.io/hub/P37AWWGAF3zPhxDDvDqV) / [Track by Layers Optimized](https://gamecreator.io/hub/HGUozuNHIFA47LawVu8t) | Búsqueda global/consulta física recurrente sin escala probada |
| [Change Dialogue Node Duration](https://gamecreator.io/hub/EVkGbDJyAROkQhFM6WYY) / [Is Current State](https://gamecreator.io/hub/t6TTdBXKIDyeeOk3wwty) | Reflection sobre estados privados |
| [Save and Load Scene by Layers](https://gamecreator.io/hub/H42GYUnT3XLyOSNHJeG9) | Sistema paralelo incompatible con identidad/persistencia exigidas |
| [Generate Quest (Detailed)](https://gamecreator.io/hub/bzXp3snWuLQS9ndGvqym) | Genera texto, no una Quest jugable ni una subhistoria escrita |

REJECT aquí significa “no usar tal cual”; un patrón pequeño de source puede inspirar una nueva solución solo después de justificarla. `Move To`, `On Update Rate`, `Collect Characters [Optimized]` y `Quest Filter` son más bien wrappers de bajo valor frente al nativo, no fallos críticos.

## 17. Unknowns / verificación futura

1. Total real del Hub, fichas anteriores no alcanzadas por 40 búsquedas, categorías adicionales, bugs de búsqueda y autores con catálogo propio. Inventario amplio, **cobertura total no demostrada**.
2. Compatibilidad de cada fuente con la versión exacta GC2/Unity de Juego2; requisitos de asmdef, URP, Input System y licencia de cada module/source. Cero paquetes descargados/compilados.
3. Origen y disponibilidad de `Game.DialogueActors`; viabilidad de importar diálogos en editor con IDs estables, validación, localización y builds.
4. Rendimiento real del buffer/reentrancia, consulta Perception y resident set con un perfil representativo de NPC; no se dispone de benchmark del Hub en Juego2.
5. Reloj diegético y autoridad del scheduler, reglas de progresión sin escena, spawn/POI y seam cuando se sigue físicamente a un NPC.
6. Si adoptar Dialogue 2, Perception 2, Inventory 2, Quests 2 o Behavior 2 produce ahorro neto frente a Core/local para casos jugables; cada decisión sigue en su WP.
7. Si GC2 Core añadió después funciones que vuelven redundantes algunos source antiguos; contrastar APIs efectivas antes de instalar.

### Lectura del catálogo

Filtrar `Recommendation` para priorizar y `DependencyClass`/`RequiredModules` para compras potenciales. `Confidence=CONFIRMED` se reserva a badge explícito; `INFERRED_FROM_SOURCE` exige compilación futura. `Value` expresa encaje en Juego2 y `EffortSaved` estima ahorro relativo si funciona; no son benchmarks de tiempo. `SourceChars` documenta que el código fue visible, no su calidad. El inventario amplio separado solo demuestra que se vio una tarjeta: `DetailOpened=no` nunca autoriza dependencia ni adopción.

## 18. TOP LISTS

### TOP 20 HUB EXTENSIONS FOR JUEGO2

La posición es **prioridad de evaluación para Juego2**, no aprobación de instalación; los módulos indicados siguen bajo sus WPs de adopción. Los enlaces y metadatos completos están en el CSV.

| # | Extensión | Dependencia; decisión | Motivo |
|---:|---|---|---|
| 1 | Run Actions with Args | Core; USE | Reutilizar Actions con Self/Target definidos por diseñador |
| 2 | For Each in Radius | Core; ADAPT | Eventos para conjuntos cercanos sin lista manual; corregir buffer/reentrancia |
| 3 | Build Dialogue From Text | Dialogue + source adicional; ADAPT | Pipeline de escritura de mayor potencial; resolver actor source y reflection |
| 4 | Preload Scene | Core; ADAPT | Precarga diferida cuando un seam real la justifique |
| 5 | Activate Preloaded Scene | Core; ADAPT | Segunda mitad del seam; coordinación de escena faltante |
| 6 | Wander Prefered Paths | Core; ADAPT | Rutas de POI authorables para NPC; no es scheduler |
| 7 | Move to Random Marker | Core; ADAPT | Variante rápida de ruta/espera; eliminar distancia literal |
| 8 | Switch (String) | Core; USE | Compacta pequeñas bifurcaciones de estado |
| 9 | On Feel (Duration) | Perception; WATCH | Dwell temporal para reacción contextual |
| 10 | Save Can See Result | Perception; WATCH | Lleva resultado sensorial a lógica de evento |
| 11 | NPC Sees Player in Zone | Perception; ADAPT | Zona de sospecha/intrusión; ajustar frecuencia |
| 12 | Set Feel Radius | Perception; WATCH | Modula sensor por situación |
| 13 | Check Object in Range | Core; WATCH | Proximidad legible para pocos actores, no bucle masivo |
| 14 | Set Character Interaction Mode | Core; WATCH | Contexto de interacción durante investigación |
| 15 | Text with Dialog System | Core/TMP; WATCH | Alternativa de UI ligera si Dialogue no se adopta |
| 16 | On Open Target Bag UI | Inventory; USE | Evento de interfaz ligado a bolsa concreta |
| 17 | On Buy from Merchant | Inventory; USE | Punto de enlace entre tienda y consecuencias |
| 18 | On Any Task Completed From Quest | Quests; USE | Punto de enlace de subhistoria y hecho duradero |
| 19 | Has Quest Started | Quests; WATCH | Condición útil para ramas tras una misión |
| 20 | Set Audio Mixer Group Volume | Core/Unity Audio; USE | Transición de ambientes y mezcla sin código específico |

### TOP CORE-ONLY EXTENSIONS

1. **Run Actions with Args:** argumentos explícitos que no figuran en Run Actions nativo.
2. **For Each in Radius:** authoring grupal útil tras reparar source.
3. **Preload Scene + Activate Preloaded Scene:** pareja con valor si hay activación diferida real.
4. **Wander Prefered Paths + Move to Random Marker:** bloques de rutas, con agenda externa.
5. **Switch (String):** ramificación compacta y reusable.
6. **Check Object in Range:** umbral de proximidad para pocos NPC.
7. **Text with Dialog System:** presentación ligera condicionada al lenguaje de UI.
8. **Set Audio Mixer Group Volume:** ambiente/audio de distrito con transición.

**On Update Rate no entra**: Core On Interval ya cubre la necesidad ordinaria; su catch-up es un caso especial, no un ganador general.

### TOP EXTENSIONS REQUIRING A GC2 MODULE

1. **Build Dialogue From Text** — Dialogue 2 y código externo por resolver; mayor ahorro de narrativa si se repara.
2. **On Feel (Duration)** — Perception 2; añade duración a una sensación.
3. **Save Can See Result** — Perception 2; integra visión con variables.
4. **NPC Sees Player in Zone** — Perception 2; vigilancia de un espacio concreto.
5. **On Open/Close Target Bag UI** — Inventory 2; contexto de objeto/bolsa precisa.
6. **On Buy/Sell from Merchant** — Inventory 2; conexiones de tienda.
7. **On Any Task Activate/Completed From Quest** — Quests 2; hitos de subhistorias.
8. **Has Quest Started** — Quests 2; memoria de inicio para una rama.

Behavior 2 aporta menos por Hub (`Flee`, `Add Goal (Property)`); su compra se juzga por AI nativa en un caso jugable.

### TOP AUTHORING TOOLS

`Build Dialogue From Text` (guion a árbol, tras reparar); `Run Actions with Args` (acciones reusable); `Switch (String)` (ramas); `Wander Prefered Paths` (rutas); `Instructions Container` (composición, comparar con Actions nativo); `Advanced Instruction Template` (plantilla editor, separar ensamblados). Ordenados por potencial de ahorro para escritores/diseñadores, con las condiciones técnicas indicadas.

### TOP NPC/SIMULATION TOOLS

`For Each in Radius` (grupo espacial), `Wander Prefered Paths` (rutas), `Move to Random Marker` (puntos de paseo), `On Feel (Duration)` (persistencia sensorial), `Save Can See Result` (visibilidad), `NPC Sees Player in Zone` (alerta localizada), `Check Object in Range` (umbral simple). **Ninguno es simulation LOD completo**: residencia, reloj y estado persistente siguen siendo trabajo de GC2-05/06.

### TOP “ADAPT THE SOURCE” CANDIDATES

1. **For Each in Radius:** buffers por invocación, overflow visible y async seguro.
2. **Build Dialogue From Text:** dependencia de actores, API pública y pipeline de autoría validado.
3. **Preload Scene + Activate Preloaded Scene:** handles por escena, cancelación y orden de activación.
4. **Wander Prefered Paths:** separar elección de ruta de reloj/POI persistente.
5. **Move to Random Marker:** configurar distancia, cancelación y destino real.
6. **NPC Sees Player in Zone:** usar eventos/sensores e intervalo en lugar de polling redundante.

### TOP PACKAGES TO AVOID

`Manage NPC Visibility and Pooling` (alpha sin semántica de identidad); `Trigger At Specific Time` (reloj del dispositivo); `Load Game (No Scene Reload, Reflection)` y los dos `Load ... Name Variables ... Save` (internals del guardado); `Vision Detection` y `Track by Layers Optimized` (consultas globales recurrentes); `Change Dialogue Node Duration` e `Is Current State` (campos privados); `Save and Load Scene by Layers` (persistencia paralela). El apartado 16 contiene enlaces y causalidad de cada uno.
