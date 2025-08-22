# Setup Guide

## Prerequisites
- Unity 2021.3+ with WebGL support
- A browser with a crypto wallet (e.g., MetaMask)

## Install the package (UPM)
Use one of the standard UPM methods:

- Add from Git URL: `https://github.com/v17alya/Web3-Onboard-Unity.git?path=Assets/com.gamenator.web3-onboard-unity#v0.1.0`
- Add from disk: Package Manager → Add package from disk… → select `Assets/com.gamenator.web3-onboard-unity/package.json`
- Edit `Packages/manifest.json` dependencies to include the Git URL above

## Quick start (use the included sample)
1) Import the sample
- Package Manager → Web3 Onboard Unity → Samples → Import “Minimal”

2) Select the WebGL template
- Edit → Project Settings → Player → Resolution and Presentation → Template: `PROJECT:Web3OnboardSample`

3) Configure WalletConnect Project ID
- Open the sample scene and select the `Web3BridgeSample` component
- In `initOptionsJson`, set your `projectId` (and optionally tweak `appMetadata`/theme)

4) Build & run (WebGL)
- File → Build Settings → WebGL → Build or Build And Run
- In the browser, use the top-left buttons (Connect / Disconnect / Sign) to verify the flow

Notes:
- The sample template already includes a prebuilt `web3-onboard-bridge.js` next to `index.html` for immediate testing.
- The template wires `unityInstance` to the bridge after load.

## Alternative: install the embedded bridge via Editor
If you don’t use the sample template and want to install the bridge into your project:
1) Tools → Web3 Onboard → Install Embedded Bridge
- Installs JS files into `Assets/StreamingAssets/Web3Onboard`

2) Use a custom WebGL template
- Ensure your template `index.html` loads the bridge (copy the installed JS near your template or adjust the path accordingly)
- Call `OnboardWalletBridge.init({ projectId, appMetadata, ... })` before using other methods
- After Unity loads, pass the instance: `OnboardWalletBridge.setUnityInstance(unityInstance)`

## Bridge API (global)
- `OnboardWalletBridge.init(options)` — must be called first
- `OnboardWalletBridge.connect()` / `disconnect()`
- `OnboardWalletBridge.signMessage(message)`
- `OnboardWalletBridge.setUnityInstance(unityInstance)`
- `OnboardWalletBridge.applyTheme(themeVars)`

Unity receives callbacks via `SendMessage` to your GameObject (default in sample is `Web3BridgeSample`):
- `OnConnected(address)`
- `OnDisconnected("")`
- `OnSigned(signature)`
- `OnConnectError(error)` / `OnDisconnectError(error)` / `OnSignError(error)`

## Troubleshooting
- Ensure `init(...)` is called before `connect/signMessage`
- Verify `projectId` is valid and a wallet extension is available
- If callbacks do not reach Unity, confirm `setUnityInstance(unityInstance)` runs after player creation, or use the sample’s `Web3BridgeSample` which auto-sets it on Start
