using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using NovaForge.Settings;
using System.Text;

// 👇 THIS NAMESPACE MUST MATCH WHAT YOUR WINDOW SCRIPT EXPECTS
namespace NovaForge.Networking 
{
    public class NovaForgeAPIManager
    {
        public async Task<string> RequestSceneGeneration(string prompt, NovaForgeConfig config)
        {
            if (config == null)
            {
                Debug.LogError("NovaForgeConfig is missing.");
                return null;
            }

            string url = config.saasEndpointURL.TrimEnd('/') + "/api/generate";
            string jsonData = "{"
                + "\"prompt\":\"" + EscapeJson(prompt) + "\","
                + "\"api_key\":\"" + EscapeJson(config.userAuthToken) + "\","
                + "\"image_url\": null"
                + "}";

            Debug.Log($"[NovaForge] Transmitting to: {url}");
            Debug.Log($"[NovaForge] Payload: {jsonData}");

            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[NovaForge] FAILED. Code: {request.responseCode}");
                    Debug.LogError($"[NovaForge] Error: {request.error}");
                    Debug.LogError($"[NovaForge] Response Body: {request.downloadHandler.text}");
                    return null;
                }

                Debug.Log($"[NovaForge] Success: {request.downloadHandler.text}");
                return request.downloadHandler.text;
            }
        }

        public async Task<string> GetJobStatus(string jobId, NovaForgeConfig config)
        {
            string url = $"{config.saasEndpointURL}/api/status/{jobId}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                    return request.downloadHandler.text;

                Debug.LogError($"[NovaForge] Status Check Failed: {request.error}");
                return null;
            }
        }

        private static string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            return value
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
