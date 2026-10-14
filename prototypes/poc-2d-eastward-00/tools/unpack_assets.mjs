// Restores binary PNGs from assets.bundle.b64.json (GitHub connector could only push text)
import fs from 'fs'; import path from 'path';
const R = path.resolve(path.dirname(new URL(import.meta.url).pathname), '..');
const b = JSON.parse(fs.readFileSync(`${R}/assets.bundle.b64.json`));
for (const [f, d] of Object.entries(b)) { fs.mkdirSync(path.dirname(`${R}/${f}`), { recursive: true }); fs.writeFileSync(`${R}/${f}`, Buffer.from(d, 'base64')); }
console.log('restored', Object.keys(b).length, 'files');
