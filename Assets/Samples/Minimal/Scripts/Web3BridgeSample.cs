using System;
using UnityEngine;
using Gamenator.Web3OnboardUnity.Runtime.WebGL;

namespace Gamenator.Web3OnboardUnity.Samples
{
    public class Web3BridgeSample : MonoBehaviour
    {
        [TextArea(4, 8)] public string initOptionsJson = "{\n  \"projectId\": \"YOUR_WC_PROJECT_ID\",\n  \"appMetadata\": { \"name\": \"Web3 Onboard Sample\", \"description\": \"Unity WebGL Dapp\" }\n}";
        [TextArea(3, 8)] public string themeJson = "{\n  \"--w3o-background-color\": \"#1a1d26\",\n  \"--w3o-foreground-color\": \"#242835\"\n}";
        public string messageToSign = "Hello from Unity";
        private string _lastLog = string.Empty;

        private void OnGUI()
        {
            const int pad = 8; int w = 260, h = 28;
            int total = (h + pad) * 5 + pad; // 5 buttons + padding
            int x = (Screen.width - w) / 2;
            int y = (Screen.height - total) / 2;

            if (GUI.Button(new Rect(x, y, w, h), "Initialize")) { Try(() => Web3OnboardJsBridge.Init(initOptionsJson), "Init OK"); } y += h + pad;
            if (GUI.Button(new Rect(x, y, w, h), "Apply Theme")) { Try(() => Web3OnboardJsBridge.ApplyTheme(themeJson), "Theme applied"); } y += h + pad;
            if (GUI.Button(new Rect(x, y, w, h), "Connect")) { Try(Web3OnboardJsBridge.Connect, "Connect called"); } y += h + pad;
            if (GUI.Button(new Rect(x, y, w, h), "Disconnect All")) { Try(Web3OnboardJsBridge.DisconnectAll, "DisconnectAll called"); } y += h + pad;
            if (GUI.Button(new Rect(x, y, w, h), "Sign Message")) { Try(() => Web3OnboardJsBridge.SignMessage(messageToSign, gameObject.name, nameof(OnSignMessageResult)), "Sign called"); } y += h + pad;

            GUI.Label(new Rect(x - 270, y, 800, 200), _lastLog);
        }

        private void Try(Action a, string ok)
        {
            try { a(); _lastLog = ok; } catch (Exception e) { _lastLog = e.Message; }
        }

        private void OnConnected(string address)
        {
            _lastLog = $"Connected: {address}";
            Debug.Log(_lastLog);
        }

        private void OnDisconnected(string _)
        {
            _lastLog = "Disconnected";
            Debug.Log(_lastLog);
        }

        private void OnSigned(string signature)
        {
            _lastLog = $"Signed: {signature}";
            Debug.Log(_lastLog);
        }

        private void OnConnectError(string error)
        {
            _lastLog = $"ConnectError: {error}";
            Debug.LogError(_lastLog);
        }

        private void OnDisconnectError(string error)
        {
            _lastLog = $"DisconnectError: {error}";
            Debug.LogError(_lastLog);
        }

        private void OnSignError(string error)
        {
            _lastLog = $"SignError: {error}";
            Debug.LogError(_lastLog);
        }

        public void OnSignMessageResult(string json)
        {
            Debug.Log("OnSignMessageResult");
            _lastLog = $"SignResult: {json}";
            Debug.Log(_lastLog);
        }
    }
}

