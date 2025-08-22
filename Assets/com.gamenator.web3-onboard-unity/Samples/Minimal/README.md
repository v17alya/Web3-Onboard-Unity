# Minimal Sample — Web3-Onboard Unity

This sample demonstrates a minimal end-to-end setup of the Web3-Onboard bridge in a Unity WebGL project.

## What’s included
- WebGL template: `Assets/com.gamenator.web3-onboard-unity/Samples/Minimal/WebGLTemplates/Web3OnboardSample/`
  - Contains `index.html` that initializes `OnboardWalletBridge` and wires Unity instance after load
  - Includes a prebuilt `web3-onboard-bridge.js` for immediate testing
- Sample script: `Assets/com.gamenator.web3-onboard-unity/Samples/Minimal/Scripts/Web3BridgeSample.cs`
  - Simple OnGUI with buttons to Initialize, Apply Theme, Connect, Disconnect, Sign
  - Logs Unity callbacks from the JS bridge

## Setup
1) Select the sample WebGL template in Unity
- Open: Edit → Project Settings → Player → Resolution and Presentation
- Set Template to: `PROJECT:Web3OnboardSample`

2) Configure WalletConnect Project ID
- Open a scene and add the component `Web3BridgeSample` to any GameObject
- In the component, set `initOptionsJson` to include your `projectId`:
```json
{
  "projectId": "YOUR_WC_PROJECT_ID",
  "appMetadata": { "name": "Web3 Onboard Sample", "description": "Unity WebGL Dapp" }
}
```

Optionally, you can customize the theme via `themeJson`.

## Build & Test
1) Build for WebGL
- File → Build Settings → Select your scene → Switch Platform (WebGL) → Build or Build And Run

2) Interact
- In the browser, use the top-left buttons (Connect / Disconnect / Sign)
- The `Web3BridgeSample` will also log Unity-side callbacks

## Notes
- The template’s `index.html` calls `OnboardWalletBridge.init({ projectId, appMetadata })` and, after Unity loads, passes the `unityInstance` via `OnboardWalletBridge.setUnityInstance(unityInstance)`.
- This sample is intended to verify the integration using the prebuilt `web3-onboard-bridge.js` included with the template — no separate Vite build is required here.
- Ensure the bridge JS is loaded before any code that calls `OnboardWalletBridge`.
