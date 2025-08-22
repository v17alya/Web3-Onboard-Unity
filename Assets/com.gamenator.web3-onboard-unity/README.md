# Web3 Onboard Unity (UPM Package)

Unity WebGL plugin that integrates [Web3-Onboard](https://onboard.blocknative.com/) and exposes a JavaScript bridge plus Editor tooling.

## Features
- Editor installer to unpack embedded bridge payload into `Assets/StreamingAssets/Web3Onboard`
- JavaScript bridge (`window.OnboardWalletBridge`) with `init`, `connect`, `disconnect`, `signMessage`, `setUnityInstance`, `applyTheme`
- C# interop: `.jslib` + `Web3OnboardJsBridge` wrapper
- Sample (`Minimal`) with WebGL template and `Web3BridgeSample` MonoBehaviour

## Installation (Unity Package Manager)
Choose one of the following:

1) Add from Git URL (recommended)
- Open Package Manager → + (Add) → Add package from git URL…
- Use: `https://github.com/v17alya/Web3-Onboard-Unity.git?path=Assets/com.gamenator.web3-onboard-unity#v0.1.0`

2) Add from disk
- Open Package Manager → + (Add) → Add package from disk…
- Select this file: `Assets/com.gamenator.web3-onboard-unity/package.json`

3) Via `Packages/manifest.json`
```json
{
  "dependencies": {
    "com.gamenator.web3-onboard-unity": "https://github.com/v17alya/Web3-Onboard-Unity.git?path=Assets/com.gamenator.web3-onboard-unity#v0.1.0"
  }
}
```

## Getting Started
1) Install the embedded bridge
- Unity menu: Tools → Web3 Onboard → Install Embedded Bridge (writes to `Assets/StreamingAssets/Web3Onboard/`)

2) Use the sample
- Import the `Minimal` sample via Package Manager → Web3 Onboard Unity → Samples → Import
- In Project Settings → Player → WebGL → Resolution and Presentation → Template: select `PROJECT:Web3OnboardSample`
- Open the sample scene and set `projectId` in the component’s `initOptionsJson`
- Build WebGL and test

## JavaScript Bridge API (global `OnboardWalletBridge`)
- `init(options)` — must be called first; supports `projectId`, `chains`, `appMetadata`, `unity`, `theme`
- `connect()` / `disconnect()`
- `signMessage(message)`
- `setUnityInstance(unityInstance)`
- `applyTheme(themeVars)`

## Editor Utilities
- Tools → Web3 Onboard → Install Embedded Bridge
- Tools → Web3 Onboard → Generate Web3Bridge MonoBehaviour
- Tools → Web3 Onboard → Samples → Create Sample Scene / Build Sample (WebGL)

## Requirements
- Unity 2021.3+ (WebGL)
- Modern browser (desktop/mobile)

## License
See `LICENSE` at repo root.
