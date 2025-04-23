import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import laravel from "laravel-vite-plugin";
import path from "path";
import { mkdirSync } from "fs";
import svgr from "vite-plugin-svgr";
import dts from "vite-plugin-dts";
import eslint from 'vite-plugin-eslint';

const outDir = "../wwwroot/build";

mkdirSync(outDir, { recursive: true });

export default defineConfig({
  plugins: [
   laravel({
    input: ["src/App.tsx"],
    publicDirectory: outDir,
    refresh: true,
   }),
   react(),
   svgr({
     svgrOptions: {
       icon: true,
     },
   }),
   dts(),
   eslint(),
  ],
  resolve: {
   alias: {
    "@": path.resolve(__dirname, "src"),
   },
  },
  build: {
   outDir,
   emptyOutDir: true,
  }
});