using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Gamenator.Web3OnboardUnity.Runtime.WebGL
{
    /// <summary>
    /// Thin C# wrapper over the WebGL .jslib interop for calling window.OnboardWalletBridge API.
    /// Non-MonoBehaviour by design.
    /// </summary>
    public static class Web3OnboardJsBridge
    {
        public enum BridgeLogLevel
        {
            None = 0,
            Error = 1,
            Warning = 2,
            Info = 3,
            Verbose = 4
        }

        /// <summary>
        /// Controls verbosity for internal logs.
        /// </summary>
        public static BridgeLogLevel LogLevel { get; set; } = BridgeLogLevel.Info;

        // --- Native bindings (available in WebGL builds) ---
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern int W3O_Init(string optionsJson);
        [DllImport("__Internal")] private static extern void W3O_Connect();
        [DllImport("__Internal")] private static extern void W3O_DisconnectAll();
        [DllImport("__Internal")] private static extern void W3O_ApplyTheme(string themeJson);
        [DllImport("__Internal")] private static extern void W3O_SignMessage(string message, string callbackGameObject, string callbackMethod);
        [DllImport("__Internal")] private static extern void W3O_SetUnityInstance(string gameObjectName);
        [DllImport("__Internal")] private static extern void W3O_SendToUnity(string goName, string method, string payload);
#endif

        /// <summary>
        /// Initialize the JS bridge by forwarding options JSON to window.OnboardWalletBridge.init.
        /// Returns true if initialized, false otherwise.
        /// </summary>
        /// <param name="optionsJson">JSON serialized options object for Onboard initialization.</param>
        public static bool Init(string optionsJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(optionsJson))
                {
                    throw new ArgumentException("optionsJson cannot be null or empty", nameof(optionsJson));
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                return W3O_Init(optionsJson) == 1;
#else
                LogInfo("Init is a no-op in Editor/Non-WebGL.");
                return true;
#endif
            }
            catch (Exception e)
            {
                LogError($"Init exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Connect wallets via Onboard.
        /// </summary>
        public static void Connect()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_Connect();
#else
                LogInfo("Connect is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"Connect exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Disconnect all connected wallets.
        /// </summary>
        public static void DisconnectAll()
        {
            try
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_DisconnectAll();
#else
                LogInfo("DisconnectAll is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"DisconnectAll exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Apply theme variables (JSON object of CSS variables -> values).
        /// </summary>
        /// <param name="themeJson">JSON serialized dictionary of CSS variable names to values.</param>
        public static void ApplyTheme(string themeJson)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(themeJson))
                {
                    throw new ArgumentException("themeJson cannot be null or empty", nameof(themeJson));
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_ApplyTheme(themeJson);
#else
                LogInfo("ApplyTheme is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"ApplyTheme exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Request a message signature. The result will be sent back to Unity via SendMessage
        /// to callbackGameObject.callbackMethod as a JSON string payload.
        /// </summary>
        /// <param name="message">Message to be signed by the connected wallet.</param>
        /// <param name="callbackGameObject">Target GameObject name for SendMessage callback.</param>
        /// <param name="callbackMethod">Method name on the target GameObject to receive the JSON payload.</param>
        public static void SignMessage(string message, string callbackGameObject, string callbackMethod)
        {
            try
            {
                if (string.IsNullOrEmpty(message)) throw new ArgumentException("message cannot be null or empty", nameof(message));
                if (string.IsNullOrEmpty(callbackGameObject)) throw new ArgumentException("callbackGameObject cannot be null or empty", nameof(callbackGameObject));
                if (string.IsNullOrEmpty(callbackMethod)) throw new ArgumentException("callbackMethod cannot be null or empty", nameof(callbackMethod));

#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_SignMessage(message ?? string.Empty, callbackGameObject ?? string.Empty, callbackMethod ?? string.Empty);
#else
                LogInfo("SignMessage is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"SignMessage exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Provide the active Unity GameObject name so the JS bridge can SendMessage back.
        /// </summary>
        /// <param name="gameObjectName">The GameObject name that should receive JS callbacks.</param>
        public static void SetUnityInstance(string gameObjectName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(gameObjectName))
                {
                    throw new ArgumentException("gameObjectName cannot be null or empty", nameof(gameObjectName));
                }

#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_SetUnityInstance(gameObjectName);
#else
                LogInfo($"SetUnityInstance('{gameObjectName}') is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"SetUnityInstance exception: {e}");
                throw;
            }
        }

        /// <summary>
        /// Helper to forward a payload to Unity via JS (falls back to SendMessage if available).
        /// </summary>
        /// <param name="goName">GameObject name to receive the message.</param>
        /// <param name="method">Method name to invoke on the target GameObject.</param>
        /// <param name="payload">Arbitrary payload string (often JSON).</param>
        public static void SendToUnity(string goName, string method, string payload)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(goName)) throw new ArgumentException("goName cannot be null or empty", nameof(goName));
                if (string.IsNullOrWhiteSpace(method)) throw new ArgumentException("method cannot be null or empty", nameof(method));

#if UNITY_WEBGL && !UNITY_EDITOR
                W3O_SendToUnity(goName, method, payload ?? string.Empty);
#else
                LogInfo($"SendToUnity('{goName}', '{method}', '{payload}') is a no-op in Editor/Non-WebGL.");
#endif
            }
            catch (Exception e)
            {
                LogError($"SendToUnity exception: {e}");
                throw;
            }
        }

        private static void LogInfo(string message)
        {
            if (LogLevel < BridgeLogLevel.Info) return;
            Debug.Log($"[Web3OnboardJsBridge] {message}");
        }

        private static void LogWarning(string message)
        {
            if (LogLevel < BridgeLogLevel.Warning) return;
            Debug.LogWarning($"[Web3OnboardJsBridge] {message}");
        }

        private static void LogError(string message)
        {
            if (LogLevel < BridgeLogLevel.Error) return;
            Debug.LogError($"[Web3OnboardJsBridge] {message}");
        }
    }
}


