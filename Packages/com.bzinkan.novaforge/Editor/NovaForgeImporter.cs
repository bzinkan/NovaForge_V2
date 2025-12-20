using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace NovaForge.Editor
{
    public static class NovaForgeImporter
    {
        private const int PollIntervalMilliseconds = 2000;
        private const int TimeoutSeconds = 60;

        public static async void PollAndImport(string jobId, string outputName)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                Debug.LogError("[NovaForge] PollAndImport called with empty jobId.");
                return;
            }

            string safeName = SanitizeFileName(outputName);
            string url = $"https://novaforge.nyc3.digitaloceanspaces.com/novaforge/outputs/{jobId}.glb";
            DateTime startTime = DateTime.UtcNow;

            Debug.Log($"[NovaForge] Polling for output: {url}");

            while ((DateTime.UtcNow - startTime).TotalSeconds < TimeoutSeconds)
            {
                using (UnityWebRequest headRequest = UnityWebRequest.Head(url))
                {
                    await SendRequestAsync(headRequest);

                    if (headRequest.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("[NovaForge] Output found. Downloading...");
                        await DownloadAndImport(url, safeName);
                        return;
                    }
                }

                await Task.Delay(PollIntervalMilliseconds);
            }

            Debug.LogWarning("[NovaForge] Timed out waiting for output.");
        }

        private static async Task DownloadAndImport(string url, string outputName)
        {
            using (UnityWebRequest getRequest = UnityWebRequest.Get(url))
            {
                await SendRequestAsync(getRequest);

                if (getRequest.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[NovaForge] Download failed: {getRequest.error}");
                    return;
                }

                string directoryPath = Path.Combine(Application.dataPath, "NovaForge", "Generated");
                Directory.CreateDirectory(directoryPath);

                string filePath = Path.Combine(directoryPath, $"{outputName}.glb");
                File.WriteAllBytes(filePath, getRequest.downloadHandler.data);

                AssetDatabase.Refresh();

                string assetPath = $"Assets/NovaForge/Generated/{outputName}.glb";
                GameObject importedAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (importedAsset != null)
                {
                    PrefabUtility.InstantiatePrefab(importedAsset);
                    Selection.activeObject = importedAsset;
                    Debug.Log("[NovaForge] Imported asset instantiated in scene.");
                }
                else
                {
                    Debug.Log("[NovaForge] Imported asset saved. Instantiate manually if needed.");
                }
            }
        }

        private static async Task SendRequestAsync(UnityWebRequest request)
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }
        }

        private static string SanitizeFileName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName))
            {
                return "NovaForgeOutput";
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char invalid in invalidChars)
            {
                rawName = rawName.Replace(invalid.ToString(), "_");
            }

            return rawName.Trim();
        }
    }
}
