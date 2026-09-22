# Análisis CTX / presupuesto de proceso / H2-H3 — NON-CONTRACTUAL

Re-medido en `main` = `f7b4f1e8247dfc927203dca6754b71aaa53938f3` (2026-09-22).
Artefacto anterior: `TOKEN_BUDGET_BASELINE.md` en `claude/token-savings-project-owqkzj`,
pinneado a `a57afa3d3fc60b3e1c59d04149fe982f387c2048`.

**Input de analista externo.** No define contrato, no vincula ningún workpack, no
debilita ningún criterio de aceptación, y **no es evidencia aceptada**. No ha pasado
revisión independiente. Donde parezca entrar en conflicto con `AGENTS.md`,
`Docs/ROADMAP.md`, `Docs/workpacks/**` o evidencia aceptada, ganan esos.

La sesión que produjo este documento no actuó como Worker ni Reviewer de ningún
workpack y no debe actuar como Reviewer de nada que se derive de él.

Tokenizador: el de Claude incluido en `anthropic==0.21.3`. Proxy cercano, no exacto.
**Trata toda cifra como ±10%.** Reproducible con `scripts/token-budget/`.

---

## Veredicto

El plan CTX es correcto pero **insuficiente**, por una razón estructural:

> CTX cambia el **intercepto** una vez. Nada en CTX-01/02/03 cambia la **pendiente**.

CTX-01 ahorra 16.430 tokens en el mejor caso. El crecimiento restante hasta H4 añade
~41.944, es decir **2,6× todo el ahorro de CTX-01**. Sin un presupuesto por rol que
falle cerrado en CI, CTX se devuelve solo — y ya empezó a devolverse mientras se
ejecutaba.

### Estado de las cifras del brief

| cifra | estado | valor en `f7b4f1e` |
|---|---|---:|
| Worker foundational 50.414 | **caducada** | 53.382 (+2.968) |
| Reviewer foundational 83.091 | **caducada** | 85.565 (+2.474) |
| +427 tokens de boot por WP | **confirmada exacta** | +427 (32 revisiones de ROADMAP) |
| +4.325 por fase aceptada | **confirmada** | +4.560 en la ventana 20→21 sep |
| arquitectura vinculante 6.826/fase | **confirmada** | 6.826 |
| cadena PA-13 ~87.000 | **confirmada** | 87.372 (proyección plana) |
| corpus producción CITY ~68.000 | **confirmada** | 67.509 |
| evidencia CITY aceptada ~110.000 | **confirmada** | 110.249 |
| cápsula HK-GATE 496 vs 12.709 | no re-derivada | — |
| tasa de reparación 0,92 | no re-derivada | — |

### Dos hechos nuevos, no medidos antes

- Entre `a57afa3` y `f7b4f1e` se **añadieron 49.691 tokens y se borraron 1.390**
  (un único prompt de handoff recortado). El corpus vinculante es estrictamente
  aditivo hasta ahora.
- El conjunto de boot pasó de **17.937 a 22.497 (+25,4%) con 19 workpacks aceptados
  constantes**: crecimiento con cero trabajo de producto aceptado, durante el propio
  esfuerzo de ahorro.

---

## (a) CTX-01 no entrega un número, entrega un rango sin auditar

CTX-01 es enrutamiento, no borrado. No elimina nada; **permite saltar**. El ahorro
depende de si los predicados `must_escalate_if` se disparan, y eso lo decide la
propia sesión que se beneficia de saltar.

Arranque en frío del Worker en WP-H1-02, medido en `f7b4f1e`:

| escenario | tokens | vs 53.382 |
|---|---:|---:|
| reglas pre-CTX | 53.382 | — |
| CTX-01 **MIN** (pack del perfil, sin escalar) | 36.952 | −30,8% |
| CTX-01 **ESC** (escalan arquitectura + hk00) | 47.222 | −11,5% |

Reviewer con el diff medio del candidato (32.968, se lee entero — cerrado):
85.565 → **69.135 MIN / 79.405 ESC** (−19,2% / −7,2%).

Esto explica la discrepancia del borrador superseded: el ~8% era la columna ESC del
Reviewer y el ~20% la MIN. Ambos eran correctos; medían escenarios distintos sin
decirlo.

**Para WP-H1-02 en concreto, la columna probable es ESC.** El WP declara una sección
«Architecture / authority boundary», hereda garantías HK00/HK00A y posee los registros
de adopción Unity dependency/IP. Son tres disparadores independientes de *«the current
claim binds an architecture/proof source outside the initial pack»*.

### El problema real no es el tamaño del rango, es que no deja rastro

Los 10.270 tokens de diferencia los resuelve un juicio no auditado, y **no sobrevive
ningún artefacto de esa decisión a la revisión**. Un Reviewer no puede saber si el
Worker escaló arquitectura o decidió que no era material. Es un hueco de proceso nuevo
que CTX-01 introduce y que el lint de handoff — correctamente estrecho — no cubre.

Sugerencia barata para CTX-03, no para reabrir CTX-01: que el
`PREDECESSOR_CONTRACT_CHECK` (ya obligatorio, ya se persiste) incluya una línea
mecánica `CONTEXT_ESCALATIONS:` enumerando qué predicados se dispararon y cuáles se
declararon no materiales. Coste ~40 tokens. Convierte el rango en dato medible y le da
a CTX-03 la base empírica que su propio contrato pide («measure representative
bootstrap/context cost before and after CTX»).

### Un defecto menor, verificable

`Docs/engineering/CONTEXT_BOOTSTRAP_V1.md` sigue diciendo en su cabecera
`Status: **CTX-01 REPAIR CANDIDATE / PROCESS_ONLY**`, y su §9 dice que el protocolo
«becomes binding only after this repaired WP-CTX-01 receives fresh independent PASS,
merges, and completes DocSync». Eso ya ocurrió (PR #111, #112). El documento que debe
ser vinculante se declara a sí mismo no vinculante en `main`. Es trabajo de DocSync,
no de workpack.

---

## (b) CTX-02 nombra a PA como consumidor, pero la cápsula no encaja en su forma

Sí: PA-09..14 son las sesiones más caras del proyecto, por bastante margen.
**WP-PA-13 arranca en 124.396 tokens** post-CTX-01, 2,6× la sesión foundational del
kernel.

| componente de WP-PA-13 | tokens |
|---|---:|
| «all accepted PA-01..12 findings» (proyección plana) | 87.372 |
| boot + skill + overhead post-CTX-01 | 15.327 |
| evidencia del predecesor directo (PA-03 como media) | 8.440 |
| ambos amendments transversales | 7.707 |
| PA_ROADMAP + PA/README | 4.937 |
| WP-PA-13 | 613 |
| **total** | **124.396** |

La cadena es acumulativa **por contrato**, no por costumbre: cada WP-PA-0N declara
«accepted PA-01..0(N-1) findings» en sus Required inputs, y PA-13 declara «**all**
accepted PA-01..12 findings». Los resultados además crecen: 3.444 → 5.735 → 7.281
(+1.919 de media por WP). Si esa pendiente continúa en vez de aplanarse, la cadena no
son 87.372 sino 167.982.

### Por qué la cápsula de CTX-02 no lo resuelve

CTX-02 sí nombra PA («representative H1, CITY and PA consumers»). Pero su esquema de
cápsula es *«exported guarantees, explicit exclusions/non-claims, concrete reopen
conditions»*. Eso modela una **frontera de contrato heredado**. Lo que PA-13 consume no
es una frontera: es un **corpus que debe componer** — su objetivo literal es «Compose
the failure findings accumulated across PA-01..12».

No se puede componer lo que se ha comprimido a una lista de garantías. Y el control de
CTX-02 — «omitir una garantía exportada material y probar que la validación detecta la
pérdida» — no tiene análogo para hallazgos de investigación, donde «material» no está
definido.

Estimación: CTX-02 tal como está escrito reduce PA-13 en la evidencia del predecesor
directo, ~8.440 de 124.396 = **6,8%**. Deja intacto el término dominante.

### Lo que sí encajaría, y que CTX-02 no contempla

Los ficheros de resultado PA ya tienen una estructura muy regular por **rol** de
sección — Boundary, Exact provenance audited, Canonical Juego2 finding,
escenarios/controles negativos, deferred proof, residual ownership, accepted
disposition — pero los **nombres y números derivan** entre resultados (§8 es «Deferred
empirical proof» en PA-01 y «counterfactual fixtures» en PA-03). Por eso hoy ningún
consumidor puede direccionarlos mecánicamente y todos leen el fichero entero.

El bloque más grande es siempre el de escenarios/fixtures, y es el que más crece:
570 → 883 → 1.543 tokens (PA-01/02/03). PA-13 no lo necesita: necesita hallazgo
canónico, modos de fallo y disposición. En PA-03 eso son 1.336 de 7.281 = **18%**.

Un **esquema de resultado con ids de sección normativos**, más la posibilidad de que un
WP consumidor declare qué secciones consume, convertiría los 87.372 en ~25.000. Son
**~62.000 tokens en una sola sesión**, casi 4× todo lo que CTX-01 entrega en una sesión
foundational.

Esto es materia de CTX-03 («introduce structured representations for naturally tabular
evidence»), no de CTX-02. Pero CTX-03 depende de CTX-02, y CTX-02 tal como está
redactado no ataca la cadena PA.

> **Recomendación:** añadir a los Required inputs de CTX-02 la obligación de medir la
> cadena PA y declarar explícitamente si la cápsula la cubre o la deriva a CTX-03. Sin
> eso, la secuencia acordada llega a CTX-03 sin haber mirado el mayor coste del
> proyecto.

---

## (c) Las zonas nunca analizadas, medidas

**Amendments PA.** Los dos transversales (5.702 + 2.005 = **7.707**) son vinculantes
para *todo* WP de PA según `Docs/workpacks/PA/README.md`. Los específicos (PA-09 4.667,
PA-12 6.653) solo atan su propio WP. La cifra de ~21.000 del brief mezcla ambos: el
coste realmente recurrente son 7.707 por sesión PA. Con 11 WPs de PA restantes × 2
sesiones = **~170.000 tokens** de relectura de los mismos dos documentos. Candidato
claro a cápsula, y este sí tiene forma de garantía: es un contrato transversal, no un
corpus a componer.

**Arquitectura H1 + ADRs.** 6.826 en total. Nunca sale del set foundational; se relee
en cada WP de H1, Worker y Reviewer. Con 13 WPs de H1 restantes × 2 = **~177.000**. Es
el mejor candidato a cápsula del repo: contenido estable, forma de garantía,
consumidores múltiples.

Pero ojo con el invariante: una cápsula de arquitectura que el Reviewer acepte sin
abrir el ADR es exactamente la degradación de revisión que no se quiere, y **se parece
a un éxito**. El control de CTX-02 es necesario aquí y probablemente insuficiente.

**Documentos de protocolo.** Crecimiento en la ventana medida:

| documento | antes | ahora | delta |
|---|---:|---:|---:|
| `implement-workpack/SKILL.md` | 986 | 1.615 | +63,8% |
| `AUTOMATION_V2.md` | 2.384 | 3.251 | +36,4% |
| `WORKER_REVIEW_PROTOCOL.md` | 4.143 | 5.084 | +22,7% |
| `Docs/workpacks/README.md` | 2.357 | 2.821 | +19,7% |

Ninguno de estos crecimientos pasó por un presupuesto. Son los documentos que **toda**
sesión lee, así que su crecimiento se multiplica por cada rol y cada WP restante. Es el
multiplicador más alto del repo y el único sin dueño.

---

## (d) El process-envelope: CTX cambia el intercepto, no la pendiente

Este es el hallazgo principal, y responde a la pregunta 1 con un no.

CTX-01/02/03 son reducciones **de una sola vez**. El crecimiento es **recurrente** y no
tiene tope. Proyección con 65 ficheros de workpack existentes, 19 aceptados, H2 sin
workpacks todavía:

| término | tokens |
|---|---:|
| ~46 WPs planificados restantes × 427 | 19.642 |
| pasos de fase H2 + H3 (2 × 4.325) | 8.650 |
| arquitectura vinculante H2 + H3 (2 × 6.826) | 13.652 |
| **crecimiento de boot restante hasta H4** | **~41.944** |

CTX-01 ahorra 16.430 en el mejor caso. El crecimiento restante añade ~41.944, **2,6×
todo el ahorro**. En el escenario MIN el Worker foundational en H4 sigue aterrizando en
~78.896, un 48% por encima del ESC de hoy.

Y ya se está devolviendo. Medido de punta a punta desde `a57afa3`: el peor caso mejoró
solo 3.192 tokens (50.414 → 47.222, −6,3%), porque **aproximadamente la mitad del
ahorro de peor caso de CTX-01 se consumió con crecimiento que aterrizó mientras CTX-01
estaba en vuelo**. El brief lo intuía con ~10.970; la cifra vinculante que mido en la
misma ventana es de ese orden (~13.500 de documentos vinculantes nuevos, de los cuales
~7.400 atan a todos los roles).

### Propuesta

Un `process-envelope` medido y con dueño, que **falle cerrado en CI**. La
infraestructura ya existe: `candidate-validation.yml` ya ejecuta
`validate-worker-handoff.py` sobre el SHA congelado, ya falla cerrado y ya tiene
`--self-test`.

La propuesta **no** es ampliar el lint de handoff — su alcance estrecho está cerrado y
debe seguir así, sin juicio semántico. Es un script hermano con el mismo cableado:

1. Un fichero de presupuesto versionado: un techo de tokens por perfil de rol.
2. El check tokeniza el set de lectura derivado del perfil en el SHA congelado y falla
   si excede el techo.
3. Subir el techo es un cambio de fichero revisado, con justificación, no un efecto
   colateral silencioso de añadir un párrafo a un protocolo.

Esto no toca el rigor: no borra evidencia, no comprime nada, no toca la revisión. Solo
hace que **añadir contexto vinculante sea una decisión visible** en vez de un impuesto
invisible. Es lo único de esta lista que cambia la pendiente.

Aviso honesto sobre su propio coste: el fichero de presupuesto y su documentación son
ellos mismos tokens nuevos. Debería ser un JSON pequeño que ninguna sesión de rol lee
— solo CI — o el remedio entra en la enfermedad.

---

## (e) El gate pre-freeze y el 0,92

De acuerdo con la intuición del brief: **el multiplicador 0,92 vale más que cualquier
reducción de tamaño de sesión**. Una reparación foundational cuesta un Worker de
reparación más un Reviewer fresco. Bajar 0,92 a 0,80 ahorra más que todo CTX junto.

Pero el invariante bidireccional es fácil de romper sin darse cuenta:

- Un gate que atrape defectos **mecánicos** baja 0,92 de forma **sana**: mueve el
  hallazgo de «Reviewer lo encontró» a «CI lo encontró», más barato, sin perder el
  hallazgo.
- Un gate que haga que los Workers **escriban para el gate** baja 0,92 de forma
  **enferma**, y es indistinguible del caso anterior si solo se mira la métrica.

Por eso el control negativo que propone el brief — debe ponerse en rojo al quitar la
ejecución real, no la declaración — es exactamente el correcto: es un control de
segundo orden sobre el propio gate. Mantenerlo.

Lo que añadiría: **clasificar los 11 FAILs históricos antes de construir nada**.
¿Cuántos habría atrapado un gate mecánico? Si la respuesta es 2 de 11, el gate no es la
palanca y conviene saberlo antes. Esa clasificación es barata y no la he hecho: no
re-derivé el 0,92 en esta pasada.

De acuerdo con aparcarlo hasta CTX-03, con una condición: que CTX-03 reciba la
clasificación de FAILs como input, no solo mediciones de contexto. Su contrato ya pide
«record whether any FAIL/review quality signal regressed during the CTX rollout», que
es la mitad defensiva de lo mismo.

---

## (f) H2/H3: los cinco criterios son correctos, y faltan tres

La dirección me parece acertada y estructuralmente sólida. El repo ya la registra como
señal no vinculante en `Docs/workpacks/H2/FUTURE_PLANNING_SIGNAL.md`, que hace bien en
decidir en vez de asumir.

El criterio más fuerte de los cinco es el cuarto: **un repo consumidor externo que
arranca solo con artefactos públicos**. Es el único falsable de verdad. Los otros
cuatro son declarativos y se pueden satisfacer sobre el papel; ese no.

El matiz acordado — H3 hereda invariantes de proceso, no el corpus — es exactamente la
distinción correcta, y este análisis la respalda con números: **el corpus es lo que
crece a +427/WP; los invariantes no crecen**.

### Lo que añadiría

**1. Un presupuesto de arranque para el repo cliente, fijado en H2.** Si H3 arranca sin
techo, reproduce la curva de H0/H1/H2 desde cero y en dos meses está donde está Juego2
hoy. La plantilla de repo-cliente que H2 debe producir debería incluir el check de
presupuesto de (d), no solo el lint y los invariantes. Es el único momento barato para
ponerlo: antes de que exista corpus que proteger.

**2. Un criterio de salida sobre la propia frontera de contexto.** Los cinco criterios
prueban que el consumidor puede *usar* Arkus sin el histórico. Ninguno prueba cuánto le
cuesta. Propongo medirlo: *el arranque en frío de una sesión de rol en el repo
consumidor no excede N tokens, y ninguna lectura obligatoria apunta dentro de Juego2*.
Eso convierte «no hereda el histórico» de intención en aserción mecánica, y usa la
misma maquinaria que ya existe.

**3. Decidir qué pasa con PA y CITY.** Son investigación y producción de contenido, no
kernel. Si H3 es un repo nuevo que solo aplica el harness, ¿dónde viven PA-01..14 y el
corpus CITY? No son artefactos públicos versionados de Arkus, pero tampoco son
histórico desechable: son los requisitos del juego. Los criterios de salida propuestos
no los nombran, y son ~178.000 tokens de contenido aceptado. **Este es el hueco más
grande que veo en (f).**

---

## Reproducir las mediciones

```bash
pip install tokenizers
pip download anthropic==0.21.3 --no-deps -d /tmp/a
unzip -o -j /tmp/a/*.whl anthropic/tokenizer.json -d scripts/token-budget/
git fetch --unshallow origin main          # growth.py necesita el histórico completo
python3 scripts/token-budget/measure.py    # antes (sets PRE-CTX, no editar)
python3 scripts/token-budget/after.py      # después (lee los perfiles de rol de CTX-01)
python3 scripts/token-budget/growth.py     # curva de crecimiento sobre el histórico
```

`measure.py` tiene los sets de lectura PRE-CTX hardcodeados: es deliberado, es el lado
«antes», y no se tocaron. `after.py` es nuevo y lee los perfiles de rol derivados que
CTX-01 hizo autoritativos (`Docs/engineering/context-bootstrap-profiles.json`), así que
ambos lados siguen siendo comparables.

---

## Límites de este análisis

Qué **no** re-derivé, y que por tanto no debería citarse desde aquí como medido:

- La tasa de reparación **0,92** (11 FAILs en 12 WPs). La arrastro del artefacto
  anterior.
- La **cápsula HK-GATE de 496 tokens**. No la reconstruí; es el dato que hace plausible
  toda la línea de CTX-02 y merece verificación independiente.
- El diff medio del candidato (**32.968**). Lo trato como constante fija porque está
  cerrado.

Debilidades metodológicas:

- Las columnas **MIN/ESC** son mi lectura de qué predicados de escalado dispara
  WP-H1-02. Es un juicio, no una medición. Otra persona podría situar la línea en otro
  sitio. Que el juicio sea necesario *es* el hallazgo de (a).
- La proyección de la cadena PA asume que los resultados se aplanan en el tamaño de
  PA-03. Los tres puntos medidos sugieren crecimiento lineal continuado, que daría
  167.982 en vez de 87.372. Tres puntos no son una tendencia.
- Tokenizador proxy, **±10%**. Ninguna conclusión aquí depende de márgenes menores.
- La proyección a H4 asume 46 WPs restantes contando ficheros de workpack existentes.
  H2 no tiene workpacks todavía, así que es un suelo, no una estimación central.

Qué **no** propongo: no propongo tocar el diff del candidato, ni la independencia del
Reviewer, ni el alcance del lint de handoff, ni paralelizar CTX-02 con CTX-01, ni meter
nada de esto en `RESIDUAL_LEDGER.md`. Estas ideas son backlog CTX no vinculante, o
input para CTX-03.
