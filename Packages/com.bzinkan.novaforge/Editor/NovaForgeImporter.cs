using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using NovaForge.Networking;
using NovaForge.Settings;

namespace NovaForge.Editor
{
    public static class NovaForgeImporter
    {
        private const int PollIntervalMilliseconds = 2000;
        // UPDATE: Increased to 5 minutes (300 seconds) for Meshy
        private const int TimeoutSeconds = 300;

        [Serializable]
        public class StatusResponse
        {
            public string status;
            public string download_url;
            public string error;
            public int progress; // <--- Added to catch Replit's data
        }

        // UPDATE: Added 'onProgress' callback
        public static async Task<StatusResponse> PollAndImport(
            string jobId,
            string outputName,
            NovaForgeConfig config,
            NovaForgeAPIManager apiManager,
            Action<string> onProgress)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                Debug.LogError("[NovaForge] PollAndImport called with empty jobId.");
                return new StatusResponse { status = "failed", error = "Missing job id." };
            }

            string safeName = SanitizeFileName(outputName);
            DateTime startTime = DateTime.UtcNow;

            Debug.Log($"[NovaForge] Polling for output: {jobId}");

            while ((DateTime.UtcNow - startTime).TotalSeconds < TimeoutSeconds)
            {
                string response = await apiManager.GetJobStatus(jobId, config);
                if (string.IsNullOrWhiteSpace(response))
                {
                    await Task.Delay(PollIntervalMilliseconds);
                    continue;
                }

                StatusResponse statusResponse = JsonUtility.FromJson<StatusResponse>(response);
                if (statusResponse == null)
                {
                    await Task.Delay(PollIntervalMilliseconds);
                    continue;
                }

                // --- PROGRESS UPDATE LOGIC ---
                if (statusResponse.status == "processing")
                {
                    // Update the UI via the callback
                    string msg = $"Processing: {statusResponse.progress}%";
                    onProgress?.Invoke(msg);
                }
                // -----------------------------

                if (!string.IsNullOrWhiteSpace(statusResponse.error))
                {
                    return statusResponse;
                }

                if (string.Equals(statusResponse.status, "completed", StringComparison.OrdinalIgnoreCase))
                {
                    onProgress?.Invoke("Downloading..."); // Update UI
                    await DownloadAndImport(statusResponse.download_url, safeName);
                    return statusResponse;
                }

                await Task.Delay(PollIntervalMilliseconds);
            }

            return new StatusResponse { status = "timeout", error = "Timed out waiting for output." };
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
            }
        }

        private static async Task SendRequestAsync(UnityWebRequest request)
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone) await Task.Yield();
        }

        private static string SanitizeFileName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return "NovaForgeOutput";
            foreach (char c in Path.GetInvalidFileNameChars()) rawName = rawName.Replace(c, '_');
            return rawName.Trim();
        }
    }
}
