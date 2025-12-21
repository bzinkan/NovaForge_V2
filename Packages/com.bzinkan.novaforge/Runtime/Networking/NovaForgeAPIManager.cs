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
                Debug.LogError("NovaForgeConfig is missing. Please assign a config asset.");
                return null;
            }

            string jsonData = $"{{\"prompt\":\"{EscapeJson(prompt)}\",\"api_key\":\"{EscapeJson(config.userAuthToken)}\"}}";

            Debug.Log($"[NovaForge] Transmitting to Replit: {jsonData}");

            using (UnityWebRequest request = new UnityWebRequest(config.saasEndpointURL + "/api/generate", "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[NovaForge] Success: {request.downloadHandler.text}");
                    return request.downloadHandler.text;
                }

                Debug.LogError($"NovaForge API Error: {request.error} | Response: {request.downloadHandler.text}");
                return null;
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
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
