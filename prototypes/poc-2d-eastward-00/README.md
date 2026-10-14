# POC-2D-EASTWARD-00 (experimento, pivot 2D NO aprobado)
Prueba aislada de dirección de arte 2D (Phaser 4.2.1 + TypeScript + Vite). No toca la versión Unity.
Fórmula de estilo: **Eastward meets Shenmue meets Asturias**.

```
npm install
node tools/unpack_assets.mjs   # solo si clonas desde GitHub (PNG van en assets.bundle.b64.json)
npm run check                  # verifica rutas de manifest/atlas/mapa
npm run build && npx vite preview --port 4173
# regenerar arte (requiere source/*.jpg, que no están en el repo): pip install pillow numpy scipy && npm run assets
```
Controles: WASD/flechas, L luz, C colisiones. `?auto=1` recorrido automático.
Estado: fases 0–2 hechas; ver `reports/01-ART-GATE.md` (CONDITIONAL). Fases 3+ esperan aprobación.
