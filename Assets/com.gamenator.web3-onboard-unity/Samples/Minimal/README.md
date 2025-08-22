# Minimal Sample — Web3-Onboard Unity

This sample demonstrates a minimal end-to-end setup of the Web3-Onboard bridge in a Unity WebGL project.

## What’s included
- A Unity package in this sample’s root containing:
  - A WebGL template (`Web3OnboardSample`) with `index.html` that initializes `OnboardWalletBridge`, wires the Unity instance after load, and includes a prebuilt `web3-onboard-bridge.js` for immediate testing
  - A ready sample scene with a `Web3BridgeSample` MonoBehaviour (buttons to Initialize, Apply Theme, Connect, Disconnect, Sign) and Unity-side logging of callbacks

## Setup
1) Import the sample Unity package (scene + template + scripts)
- Double-click the Unity package in this sample’s root to import it into your project

2) Select the WebGL template in Project Settings
- Open: Edit → Project Settings → Player → Resolution and Presentation
- Set Template to: `PROJECT:Web3OnboardSample`

3) Configure WalletConnect Project ID
- Open the imported sample scene and select the `Web3BridgeSample` component
- In the component, set `initOptionsJson` to include your `projectId`:
```json
{
  "projectId": "YOUR_WC_PROJECT_ID",
  "appMetadata": { "name": "Web3 Onboard Sample", "description": "Unity WebGL Dapp" }
}
```

Optionally, you can customize the theme via `themeJson`.

Also update the WebGL template `index.html` (loaded by the sample template):
- Replace `YOUR_WC_PROJECT_ID` with your WalletConnect Project ID
- Replace `YOUR_INFURA_PROJECT_ID` in the Ethereum Mainnet RPC (`https://mainnet.infura.io/v3/YOUR_INFURA_PROJECT_ID`) or remove that chain block if you don’t use Infura/Alchemy

## Build & Test
1) Build for WebGL
- File → Build Settings:
  - Add/select the imported sample scene
  - Switch Platform to WebGL
- Edit → Project Settings → Player → WebGL → Publishing Settings:
  - Compression Format: Disabled (no Gzip/Brotli)
- Build or Build And Run

2) Interact
- In the browser, use the top-left buttons (Connect / Disconnect / Sign)
- The `Web3BridgeSample` will also log Unity-side callbacks

## Notes
- The template’s `index.html` calls `OnboardWalletBridge.init({ projectId, appMetadata })` and, after Unity loads, passes the `unityInstance` via `OnboardWalletBridge.setUnityInstance(unityInstance)`.
- This sample is intended to verify the integration using the prebuilt `web3-onboard-bridge.js` included with the template — no separate Vite build is required here.
- Ensure the bridge JS is loaded before any code that calls `OnboardWalletBridge`.
