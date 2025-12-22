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
        // FIX: Set to 1 hour. We trust Replit to tell us if it failed.
        private const int TimeoutSeconds = 3600; 

        [Serializable]
        public class StatusResponse
        {
            public string status;
            public string download_url;
            public string error;
            public int progress;
        }

        public static async Task<StatusResponse> PollAndImport(
            string jobId,
            string outputName,
            NovaForgeConfig config,
            NovaForgeAPIManager apiManager,
            Action<string, float> onProgress)
        {
            if (string.IsNullOrWhiteSpace(jobId))
                return new StatusResponse { status = "failed", error = "Missing job id." };

            string safeName = SanitizeFileName(outputName);
            DateTime startTime = DateTime.UtcNow;

            Debug.Log($"[NovaForge] Polling started for: {jobId}");

            while ((DateTime.UtcNow - startTime).TotalSeconds < TimeoutSeconds)
            {
                // Calculate elapsed time for UI display
                double elapsed = (DateTime.UtcNow - startTime).TotalSeconds;

                string response = await apiManager.GetJobStatus(jobId, config);
                if (string.IsNullOrWhiteSpace(response))
                {
                    await Task.Delay(PollIntervalMilliseconds);
                    continue;
                }

                StatusResponse statusResponse = JsonUtility.FromJson<StatusResponse>(response);
                if (statusResponse != null)
                {
                    // 1. REPLIT SAYS: PROCESSING
                    if (statusResponse.status == "processing")
                    {
                        float percent = statusResponse.progress / 100f;
                        string msg = $"Server is working... {statusResponse.progress}% ({elapsed:F0}s)";
                        onProgress?.Invoke(msg, percent);
                    }
                    
                    // 2. REPLIT SAYS: COMPLETED
                    else if (statusResponse.status == "completed")
                    {
                        onProgress?.Invoke("Downloading...", 1.0f);
                        await DownloadAndImport(statusResponse.download_url, safeName);
                        return statusResponse;
                    }

                    // 3. REPLIT SAYS: FAILED (This is what you asked for!)
                    else if (statusResponse.status == "failed" || !string.IsNullOrWhiteSpace(statusResponse.error))
                    {
                        return statusResponse;
                    }
                }

                await Task.Delay(PollIntervalMilliseconds);
            }

            return new StatusResponse { status = "timeout", error = "Client timed out (1 hour limit reached)." };
        }

        private static async Task DownloadAndImport(string url, string outputName)
        {
            using (UnityWebRequest getRequest = UnityWebRequest.Get(url))
            {
                var op = getRequest.SendWebRequest();
                while (!op.isDone) await Task.Yield();

                if (getRequest.result == UnityWebRequest.Result.Success)
                {
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
                        Debug.Log("[NovaForge] Asset imported successfully!");
                    }
                }
                else { Debug.LogError($"[NovaForge] Download failed: {getRequest.error}"); }
            }
        }

        private static string SanitizeFileName(string rawName)
        {
            if (string.IsNullOrWhiteSpace(rawName)) return "NovaForgeOutput";
            foreach (char c in Path.GetInvalidFileNameChars()) rawName = rawName.Replace(c, '_');
            return rawName.Trim();
        }
    }
}
