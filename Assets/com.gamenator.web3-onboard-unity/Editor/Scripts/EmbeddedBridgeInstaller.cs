using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Gamenator.Web3OnboardUnity.Editor
{
    /// <summary>
    /// Installs the embedded Web3-Onboard JS bridge files into the consuming Unity project.
    /// - Reads a ZIP payload containing one or more *.gz.base64 entries
    /// - For each entry: decodes base64, gunzips, and writes to StreamingAssets/Web3Onboard
    /// </summary>
    public static class EmbeddedBridgeInstaller
    {
        private const string OutputFolderRelative = "Assets/StreamingAssets/Web3Onboard";
        private const string PayloadZipFile = "bridge-payload.zip";

        [MenuItem("Tools/Web3 Onboard/Install Embedded Bridge", priority = 20)]
        public static void InstallEmbeddedBridge()
        {
            try
            {
                string editorDir = Path.GetDirectoryName(GetThisFilePath()) ?? Application.dataPath;
                string embeddedDir = Path.Combine(Directory.GetParent(editorDir)!.FullName, "Embedded");
                string zipPath = Path.Combine(embeddedDir, PayloadZipFile);
                if (!File.Exists(zipPath))
                {
                    Debug.LogError($"Bridge payload not found: {zipPath}");
                    return;
                }

                string destDir = GetAbsolutePathFromAssetsRelative(OutputFolderRelative);
                Directory.CreateDirectory(destDir);

                using var zip = ZipFile.OpenRead(zipPath);
                foreach (var entry in zip.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue; // skip folders
                    var targetName = entry.Name;
                    bool isGzipBase64 = targetName.EndsWith(".gz.base64", StringComparison.OrdinalIgnoreCase);
                    string outName = isGzipBase64 ? targetName.Substring(0, targetName.Length - ".gz.base64".Length) : targetName;

                    using var entryStream = entry.Open();
                    using var ms = new MemoryStream();
                    entryStream.CopyTo(ms);
                    ms.Position = 0;

                    string outputPath = Path.Combine(destDir, outName);

                    if (isGzipBase64)
                    {
                        // decode base64 -> gunzip -> write
                        string base64 = Encoding.UTF8.GetString(ms.ToArray());
                        byte[] gzBytes = Convert.FromBase64String(base64.Replace("\n", string.Empty).Replace("\r", string.Empty));
                        using var gzipMs = new MemoryStream(gzBytes);
                        using var gzip = new GZipStream(gzipMs, CompressionMode.Decompress);
                        using var outMs = new MemoryStream();
                        gzip.CopyTo(outMs);
                        File.WriteAllBytes(outputPath, outMs.ToArray());
                    }
                    else
                    {
                        // raw file in zip
                        File.WriteAllBytes(outputPath, ms.ToArray());
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log($"Installed Web3 bridge files to: {destDir}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to install embedded bridge: {ex}");
            }
        }

        [MenuItem("Tools/Web3 Onboard/Show Bridge SHA256", priority = 21)]
        public static void ShowBridgeSha256()
        {
            try
            {
                string editorDir = Path.GetDirectoryName(GetThisFilePath()) ?? Application.dataPath;
                string embeddedDir = Path.Combine(Directory.GetParent(editorDir)!.FullName, "Embedded");
                string zipPath = Path.Combine(embeddedDir, PayloadZipFile);
                if (!File.Exists(zipPath))
                {
                    Debug.LogError($"Bridge payload not found: {zipPath}");
                    return;
                }

                using var zip = ZipFile.OpenRead(zipPath);
                using var sha = SHA256.Create();
                foreach (var entry in zip.Entries)
                {
                    if (string.IsNullOrEmpty(entry.Name)) continue;
                    using var entryStream = entry.Open();
                    using var ms = new MemoryStream();
                    entryStream.CopyTo(ms);
                    ms.Position = 0;
                    byte[] data;
                    if (entry.Name.EndsWith(".gz.base64", StringComparison.OrdinalIgnoreCase))
                    {
                        string base64 = Encoding.UTF8.GetString(ms.ToArray());
                        byte[] gzBytes = Convert.FromBase64String(base64.Replace("\n", string.Empty).Replace("\r", string.Empty));
                        using var gzipMs = new MemoryStream(gzBytes);
                        using var gzip = new GZipStream(gzipMs, CompressionMode.Decompress);
                        using var outMs = new MemoryStream();
                        gzip.CopyTo(outMs);
                        data = outMs.ToArray();
                    }
                    else
                    {
                        data = ms.ToArray();
                    }

                    byte[] hash = sha.ComputeHash(data);
                    var sb = new StringBuilder(hash.Length * 2);
                    foreach (byte b in hash) sb.Append(b.ToString("x2"));
                    Debug.Log($"{entry.Name} → SHA256: {sb}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to compute bridge SHA256: {ex}");
            }
        }

        private static string GetThisFilePath()
        {
            string[] guids = AssetDatabase.FindAssets("EmbeddedBridgeInstaller t:Script");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("EmbeddedBridgeInstaller.cs"))
                {
                    return Path.GetFullPath(path);
                }
            }
            return Path.Combine(Application.dataPath, "com.gamenator.web3-onboard-unity", "Editor", "Scripts", "EmbeddedBridgeInstaller.cs");
        }

        private static string GetAbsolutePathFromAssetsRelative(string assetsRelativePath)
        {
            // Convert Assets/.. relative path to absolute project path
            string projectRoot = Directory.GetParent(Application.dataPath)!.FullName;
            return Path.Combine(projectRoot, assetsRelativePath);
        }
    }
}


