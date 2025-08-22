# Web3-Onboard Bridge (Vite)

This project builds the JS bridge used by the Unity WebGL plugin.

## Contents
- [What do you want to do?](#what-do-you-want-to-do)
- [Prerequisites](#prerequisites)
- [Build the bridge (single-file IIFE)](#build-the-bridge-single-file-iife--alternative-1)
- [Building with chunks (multiple output files)](#building-with-chunks-multiple-output-files--alternative-2)
- [Package for Unity installer (ZIP)](#package-for-unity-installer-zip)
- [Use with the Unity Installer (Install Embedded Bridge)](#use-with-the-unity-installer-install-embedded-bridge)
- [Integrate without packaging (copy to WebGL template)](#integrate-without-packaging-copy-to-webgl-template)
- [Add to WebGL HTML/JS (script usage)](#add-to-webgl-htmljs-script-usage)

## What do you want to do?
- Use the Unity installer (packaged ZIP):
  1) Build the bridge (choose ONE: single-file IIFE or chunked) — see [“Build the bridge (single-file IIFE)](#build-the-bridge-single-file-iife--alternative-1) or [“Building with chunks (multiple output files)”](#building-with-chunks-multiple-output-files--alternative-2)
  2) Package the build into a ZIP — see [“Package for Unity installer (ZIP)”](#package-for-unity-installer-zip)
  3) Copy the ZIP and run the installer — see [“Use with the Unity Installer (Install Embedded Bridge)”](#use-with-the-unity-installer-install-embedded-bridge)
- Integrate without packaging (direct in WebGL template):
  1) Build the bridge — e.g., see [“Build the bridge (single-file IIFE)”](#build-the-bridge-single-file-iife--alternative-1)
  2) Copy the built JS to your WebGL template — see [“Integrate without packaging (copy to WebGL template)”](#integrate-without-packaging-copy-to-webgl-template)
  3) Include and initialize in HTML — see [“Add to WebGL HTML/JS (script usage)”](#add-to-webgl-htmljs-script-usage)

## Prerequisites
- Node.js and npm
- zip and gzip in your shell (macOS/Linux or Git Bash on Windows)

## Build the bridge (single-file IIFE) — alternative 1
```bash
# From repo root (or cd into this folder first)
cd "web3-onboard-bridge"

# Install deps (reproducible)
npm ci || npm install

# Build a single IIFE bundle at dist/web3-onboard-bridge.js
npm run build
```

## Building with chunks (multiple output files) — alternative 2
Need multiple output files instead of a single IIFE?

1) Edit vite.config.js to allow code-splitting and keep predictable names:
- The entry stays `dist/web3-onboard-bridge.js`
- All chunks and assets are emitted under `dist/assets/`
```js
// vite.config.js
import { defineConfig } from 'vite'

export default defineConfig({
  build: {
    sourcemap: false,
    minify: true,
    // assetsDir defaults to 'assets' – chunks will go under dist/assets/
    rollupOptions: {
      input: './src/index.js',
      output: {
        entryFileNames: 'web3-onboard-bridge.js',
        chunkFileNames: 'assets/web3-onboard-bridge.[name].js',
        assetFileNames: 'assets/web3-onboard-bridge.[name][extname]'
      }
    }
  },
  define: { 'process.env': {} }
})
```
2) Build:
```bash
npm run build
```

## Package for Unity installer (ZIP)
The Unity installer expects a ZIP that contains one or more *.gz.base64 files (each is a gzip-compressed, Base64-encoded JS file). Use ONE of the following, depending on your build:

- Single-file build:
```bash
# Encode the single JS file
gzip -c "dist/web3-onboard-bridge.js" | base64 > "dist/web3-onboard-bridge.js.gz.base64"

# Create/refresh the ZIP payload with all encoded files in dist/
zip -j "dist/bridge-payload.zip" "dist/web3-onboard-bridge.js.gz.base64"
```

- Chunked build (encode the entry and all .js files under dist/assets/):
```bash
# Encode entry file
gzip -c "dist/web3-onboard-bridge.js" | base64 > "dist/web3-onboard-bridge.js.gz.base64"

# Encode every chunk .js output as gzip+Base64 into dist/gz/
mkdir -p dist/gz
for f in dist/assets/*.js; do
  [ -f "$f" ] || continue
  base=$(basename "$f")
  gzip -c "$f" | base64 > "dist/gz/${base}.gz.base64"
done

# Create the ZIP payload with all encoded files
zip -j dist/bridge-payload.zip dist/web3-onboard-bridge.js.gz.base64 dist/gz/*.gz.base64
```

### Optional: add an npm packaging script (single-file build)
Add this script to web3-onboard-bridge/package.json:
```json
{
  "scripts": {
    "package:zip": "npm run build && gzip -c dist/web3-onboard-bridge.js | base64 > dist/web3-onboard-bridge.js.gz.base64 && zip -j dist/bridge-payload.zip dist/web3-onboard-bridge.js.gz.base64"
  }
}
```
Then run:
```bash
cd "web3-onboard-bridge"
npm run package:zip
```

## Use with the Unity Installer (Install Embedded Bridge)
```bash
# Copy the ZIP into the plugin package
cp dist/bridge-payload.zip "../Assets/com.gamenator.web3-onboard-unity/Editor/Embedded/bridge-payload.zip"
```
In Unity:
- Tools → Web3 Onboard → Install Embedded Bridge
- The installer unpacks to Assets/StreamingAssets/Web3Onboard/ for WebGL builds

## Integrate without packaging (copy to WebGL template)
Direct integration into a WebGL template (no installer):
```bash
# Build the bridge (single-file recommended)
cd "web3-onboard-bridge"
npm install
npm run build

# Copy the compiled file into your WebGL template folder
# Replace <YourTemplate> with your template name
cp dist/web3-onboard-bridge.js "../Assets/WebGLTemplates/<YourTemplate>/web3-onboard-bridge.js"
```

## Add to WebGL HTML/JS (script usage)
Include the script in your template index.html (ensure it's loaded before any code that calls window.OnboardWalletBridge):
```html
<!-- In Assets/WebGLTemplates/<YourTemplate>/index.html -->
<head>
  <!-- other tags ... -->
  <script src="web3-onboard-bridge.js"></script>
  <script>
    // Required: initialize bridge before use
    OnboardWalletBridge.init({
      // walletConnectProjectId: 'YOUR_WC_ID',
      // chains: [{ id: '0x1', token: 'ETH', label: 'Ethereum', rpcUrl: 'https://rpc.ankr.com/eth' }],
      // appMetadata: { name: 'Your App', description: '...', icon: '<svg></svg>' }
    });

    // Later:
    // OnboardWalletBridge.connect();
    // OnboardWalletBridge.disconnect();
    // OnboardWalletBridge.signMessage('hello');
  </script>
</head>
```

Notes:
- Place the file next to index.html (or update the path accordingly if you put it under a subfolder).
- The bridge exposes a single global: window.OnboardWalletBridge with methods init, connect, disconnect, signMessage, setUnityInstance, and helpers getUnity, sendToUnity, applyTheme.
