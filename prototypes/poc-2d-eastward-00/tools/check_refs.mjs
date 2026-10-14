// verifies every file referenced by manifest, atlas and map exists and dimensions match
import fs from 'fs'; import path from 'path';
const R = path.resolve(path.dirname(new URL(import.meta.url).pathname), '..');
let bad = 0; const ok = (c, m) => { if (!c) { bad++; console.log('FAIL', m); } };
const png = f => { const b = fs.readFileSync(f); return { w: b.readUInt32BE(16), h: b.readUInt32BE(20) }; };
const man = JSON.parse(fs.readFileSync(`${R}/assets/manifest.json`));
for (const a of man.assets) { const f = `${R}/${a.file}`; ok(fs.existsSync(f), a.file); if (a.file.endsWith('.png') && fs.existsSync(f)) { const d = png(f); ok(d.w === a.w && d.h === a.h, `${a.file} size`); } }
const at = JSON.parse(fs.readFileSync(`${R}/assets/atlas/world_atlas.json`)); const ad = png(`${R}/assets/atlas/${at.meta.image}`);
for (const [n, fr] of Object.entries(at.frames)) ok(fr.frame.x + fr.frame.w <= ad.w && fr.frame.y + fr.frame.h <= ad.h, `atlas frame ${n}`);
const map = JSON.parse(fs.readFileSync(`${R}/assets/maps/street_00.json`));
for (const t of map.tilesets) ok(fs.existsSync(path.resolve(`${R}/assets/maps`, t.image)), `tileset ${t.image}`);
for (const o of map.layers.find(l => l.name === 'objects').objects) { const fr = o.properties?.find(p => p.name === 'frame')?.value; if (fr && o.type === 'facade') { for (const p of ['base', 'upper', 'roof']) ok(at.frames[`${fr}_${p}`], `frame ${fr}_${p}`); } else if (fr) ok(at.frames[fr], `frame ${fr}`); }
console.log(bad ? `${bad} problems` : `refs OK (${man.assets.length} manifest entries, ${Object.keys(at.frames).length} atlas frames)`); process.exit(bad ? 1 : 0);
