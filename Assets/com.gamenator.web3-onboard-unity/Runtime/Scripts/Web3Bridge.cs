using System;
using UnityEngine;
using Gamenator.Web3OnboardUnity.Runtime.WebGL;

namespace Web3Onboard.Generated
{
    /// <summary>
    /// Example MonoBehaviour wrapper for window.OnboardWalletBridge via Web3OnboardJsBridge.
    /// Add this to a GameObject and wire up calls from your UI.
    /// </summary>
    public class Web3Bridge : MonoBehaviour
    {
        /// <summary>Initialize OnboardWalletBridge (must be called before connect/sign).</summary>
        public void Initialize(string initOptionsJson)
        {
            if (string.IsNullOrWhiteSpace(initOptionsJson))
            {
                Debug.LogWarning("[Web3Bridge] initOptionsJson is empty");
                return;
            }
            try
            {
                bool ok = Web3OnboardJsBridge.Init(initOptionsJson);
                if (!ok) throw new InvalidOperationException("Web3Bridge.Init failed");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Web3Bridge] Initialize exception: {e}");
                throw;
            }
        }

        /// <summary>Apply theme (CSS variables).</summary>
        public void ApplyTheme(string themeJson)
        {
            if (string.IsNullOrWhiteSpace(themeJson))
            {
                Debug.LogWarning("[Web3Bridge] themeJson is empty");
                return;
            }
            try
            {
                Web3OnboardJsBridge.ApplyTheme(themeJson);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Web3Bridge] ApplyTheme exception: {e}");
                throw;
            }
        }

        /// <summary>Connect using Onboard.</summary>
        public void Connect()
        {
            try { Web3OnboardJsBridge.Connect(); } catch (Exception e) { Debug.LogError($"[Web3Bridge] Connect exception: {e}"); throw; }
        }

        /// <summary>Disconnect all wallets.</summary>
        public void DisconnectAll()
        {
            try { Web3OnboardJsBridge.DisconnectAll(); } catch (Exception e) { Debug.LogError($"[Web3Bridge] DisconnectAll exception: {e}"); throw; }
        }

        /// <summary>Request a message signature. Result will arrive in OnSignMessageResult(string json).</summary>
        public void SignMessage(string message)
        {
            try { Web3OnboardJsBridge.SignMessage(message, gameObject.name, nameof(OnSignMessageResult)); } catch (Exception e) { Debug.LogError($"[Web3Bridge] SignMessage exception: {e}"); throw; }
        }

        // Private callbacks used by JS SendMessage

        private void OnConnected(string address)
        {
            Debug.Log($"[Web3Bridge] OnConnected: {address}");
        }

        private void OnDisconnected(string _)
        {
            Debug.Log("[Web3Bridge] OnDisconnected");
        }

        private void OnSigned(string signature)
        {
            Debug.Log($"[Web3Bridge] OnSigned: {signature}");
        }

        private void OnConnectError(string error)
        {
            Debug.LogError($"[Web3Bridge] OnConnectError: {error}");
        }

        private void OnDisconnectError(string error)
        {
            Debug.LogError($"[Web3Bridge] OnDisconnectError: {error}");
        }

        private void OnSignError(string error)
        {
            Debug.LogError($"[Web3Bridge] OnSignError: {error}");
        }

        private void OnSignMessageResult(string json)
        {
            Debug.Log($"[Web3Bridge] OnSignMessageResult => {json}");
        }
    }
}
