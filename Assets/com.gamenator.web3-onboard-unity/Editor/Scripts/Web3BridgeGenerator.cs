using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gamenator.Web3OnboardUnity.Editor
{
#if UNITY_EDITOR
    /// <summary>
    /// Generates a MonoBehaviour script (Web3Bridge.cs) that wraps calls to the WebGL JS bridge.
    /// </summary>
    public static class Web3BridgeGenerator
    {
        private const string DefaultFileName = "Web3Bridge.cs";

        [MenuItem("Tools/Web3 Onboard/Generate Web3Bridge MonoBehaviour", priority = 40)]
        public static void GenerateMonoBehaviour()
        {
            string defaultName = Path.GetFileNameWithoutExtension(DefaultFileName);
            string path = EditorUtility.SaveFilePanelInProject(
                title: "Create Web3Bridge MonoBehaviour",
                defaultName: defaultName,
                extension: "cs",
                message: "This script will call the WebGL bridge and receive callbacks");

            if (string.IsNullOrEmpty(path))
            {
                Debug.Log("Web3Bridge generation cancelled.");
                return;
            }

            string content = GetMonoBehaviourTemplate();
            File.WriteAllText(path, content);
            AssetDatabase.Refresh();
            Debug.Log($"Generated Web3Bridge MonoBehaviour at: {path}");
        }

        private static string GetMonoBehaviourTemplate()
        {
            return
"using System;\n" +
"using UnityEngine;\n" +
"using Gamenator.Web3OnboardUnity.Runtime.WebGL;\n" +
"\n" +
"namespace Web3Onboard.Generated\n" +
"{\n" +
"    /// <summary>\n" +
"    /// Example MonoBehaviour wrapper for window.OnboardWalletBridge via Web3OnboardJsBridge.\n" +
"    /// Add this to a GameObject and wire up calls from your UI.\n" +
"    /// </summary>\n" +
"    public class Web3Bridge : MonoBehaviour\n" +
"    {\n" +
"        /// <summary>Initialize OnboardWalletBridge (must be called before connect/sign).</summary>\n" +
"        public void Initialize(string initOptionsJson)\n" +
"        {\n" +
"            if (string.IsNullOrWhiteSpace(initOptionsJson))\n" +
"            {\n" +
"                Debug.LogWarning(\"[Web3Bridge] initOptionsJson is empty\");\n" +
"                return;\n" +
"            }\n" +
"            try\n" +
"            {\n" +
"                bool ok = Web3OnboardJsBridge.Init(initOptionsJson);\n" +
"                if (!ok) throw new InvalidOperationException(\"Web3Bridge.Init failed\");\n" +
"            }\n" +
"            catch (Exception e)\n" +
"            {\n" +
"                Debug.LogError($\"[Web3Bridge] Initialize exception: {e}\");\n" +
"                throw;\n" +
"            }\n" +
"        }\n" +
"\n" +
"        /// <summary>Apply theme (CSS variables).</summary>\n" +
"        public void ApplyTheme(string themeJson)\n" +
"        {\n" +
"            if (string.IsNullOrWhiteSpace(themeJson))\n" +
"            {\n" +
"                Debug.LogWarning(\"[Web3Bridge] themeJson is empty\");\n" +
"                return;\n" +
"            }\n" +
"            try\n" +
"            {\n" +
"                Web3OnboardJsBridge.ApplyTheme(themeJson);\n" +
"            }\n" +
"            catch (Exception e)\n" +
"            {\n" +
"                Debug.LogError($\"[Web3Bridge] ApplyTheme exception: {e}\");\n" +
"                throw;\n" +
"            }\n" +
"        }\n" +
"\n" +
"        /// <summary>Connect using Onboard.</summary>\n" +
"        public void Connect()\n" +
"        {\n" +
"            try { Web3OnboardJsBridge.Connect(); } catch (Exception e) { Debug.LogError($\"[Web3Bridge] Connect exception: {e}\"); throw; }\n" +
"        }\n" +
"\n" +
"        /// <summary>Disconnect all wallets.</summary>\n" +
"        public void DisconnectAll()\n" +
"        {\n" +
"            try { Web3OnboardJsBridge.DisconnectAll(); } catch (Exception e) { Debug.LogError($\"[Web3Bridge] DisconnectAll exception: {e}\"); throw; }\n" +
"        }\n" +
"\n" +
"        /// <summary>Request a message signature. Result will arrive in OnSignMessageResult(string json).</summary>\n" +
"        public void SignMessage(string message)\n" +
"        {\n" +
"            try { Web3OnboardJsBridge.SignMessage(message, gameObject.name, nameof(OnSignMessageResult)); } catch (Exception e) { Debug.LogError($\"[Web3Bridge] SignMessage exception: {e}\"); throw; }\n" +
"        }\n" +
"\n" +
"        // Private callbacks used by JS SendMessage\n" +
"\n" +
"        private void OnConnected(string address)\n" +
"        {\n" +
"            Debug.Log($\"[Web3Bridge] OnConnected: {address}\");\n" +
"        }\n" +
"\n" +
"        private void OnDisconnected(string _)\n" +
"        {\n" +
"            Debug.Log(\"[Web3Bridge] OnDisconnected\");\n" +
"        }\n" +
"\n" +
"        private void OnSigned(string signature)\n" +
"        {\n" +
"            Debug.Log($\"[Web3Bridge] OnSigned: {signature}\");\n" +
"        }\n" +
"\n" +
"        private void OnConnectError(string error)\n" +
"        {\n" +
"            Debug.LogError($\"[Web3Bridge] OnConnectError: {error}\");\n" +
"        }\n" +
"\n" +
"        private void OnDisconnectError(string error)\n" +
"        {\n" +
"            Debug.LogError($\"[Web3Bridge] OnDisconnectError: {error}\");\n" +
"        }\n" +
"\n" +
"        private void OnSignError(string error)\n" +
"        {\n" +
"            Debug.LogError($\"[Web3Bridge] OnSignError: {error}\");\n" +
"        }\n" +
"\n" +
"        private void OnSignMessageResult(string json)\n" +
"        {\n" +
"            Debug.Log($\"[Web3Bridge] OnSignMessageResult => {json}\");\n" +
"        }\n" +
"    }\n" +
"}\n";
        }
    }
#endif
}


