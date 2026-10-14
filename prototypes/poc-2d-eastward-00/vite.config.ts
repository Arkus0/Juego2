import { defineConfig } from 'vite';
// assets/ is framework-independent and served as-is (publicDir)
export default defineConfig({ base: './', publicDir: 'assets', build: { outDir: 'dist', assetsInlineLimit: 0 } });
