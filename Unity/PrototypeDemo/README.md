# PrototypeDemo — «Del Puente Viejo al Bar»

Prototipo **no canónico** y desechable. No es evidencia de ningún workpack. Informe: [PROTOTYPE_REPORT.md](PROTOTYPE_REPORT.md).

## Requisitos

- Unity **6000.3.24f1** (misma versión que `Unity/ArkusUnity`), módulo Windows Build Support.
- Packs Quaternius (CC0) en `C:\Juego2-Assets` (Medieval Village, Nature, Props, Animation, Animation2).
- Python 3 para el importador.

## Pasos

1. Copiar los packs al proyecto (quedan fuera de git en `Assets/ThirdParty/`):
   ```bash
   python Unity/PrototypeDemo/Tools/import_thirdparty.py
   ```
2. Configurar el proyecto una vez (URP con SSAO, espacio lineal, Input System, capas):
   ```bash
   Unity/PrototypeDemo/Tools/unity_batch.sh Proto.EditorTools.ProjectSetup.Run setup
   ```
3. Generar la escena por capas (0 terreno … 6 vestido) y las capturas de revisión en `Captures/Layers/L<n>`:
   ```bash
   Unity/PrototypeDemo/Tools/unity_batch.sh Proto.Build.DemoBuild.BatchLayer layer6 -protoLayer 6
   ```
   En el editor: menú **Prototype → Rebuild Scene (all layers)**. La escena `Assets/_Prototype/Scenes/PuenteBar.unity` es salida generada y no se versiona.
4. Build de Windows en `<repo>/Builds/PrototypeDemo/PuenteBar.exe`:
   ```bash
   Unity/PrototypeDemo/Tools/unity_batch.sh Proto.Build.DemoBuild.BatchBuildPlayer build
   ```
5. Capturas con el juego en marcha (NPCs animados), medición de fps y **recorrido automático** del eslabón
   (escribe `fps.txt` y `recorrido.txt`, con el colisionador culpable si el jugador se atasca):
   ```bash
   Builds/PrototypeDemo/PuenteBar.exe -autocapture Unity/PrototypeDemo/Captures/Runtime -screen-width 1600 -screen-height 900
   ```

## Controles

WASD andar · Shift correr · ratón cámara · rueda zoom · **E** saludar · **F** aguas bajas/altas del vado X5 · **R** volver a la Orilla sur · Esc liberar el ratón · H ocultar la ayuda.

## Dónde está cada cosa

| Qué | Dónde |
|---|---|
| Geometría de la semilla, calles con cotas, P8/P9, secretos | `Assets/_Prototype/Editor/Build/Seed.cs` |
| Trazado (casas, huertas, tapias) | `Layout.cs` |
| Relieve + rasantes (solver) | `Heights.cs`, `TerrainBuild.cs`, `Stamps.cs` |
| Muros de orilla, contención, tapias | `WallBuild.cs` |
| Casas con piezas del kit | `HouseBuilder.cs`, `Kit.cs` |
| Puente, vado, bar, Ayuntamiento, secretos | `BridgeBuild.cs`, `Encounters.cs` |
| Vestido, vegetación, fondo | `Dressing.cs`, `Backdrop.cs` |
| Jugador, cámara, HUD, NPCs | `Assets/_Prototype/Runtime/` + `GameplayBuild.cs` |
