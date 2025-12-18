using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using NovaForge.Settings;

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

            string jsonData = $"{{\"user_prompt\":\"{EscapeJson(prompt)}\",\"auth_token\":\"{EscapeJson(config.userAuthToken)}\"}}";

            using (UnityWebRequest request = UnityWebRequest.Post(config.saasEndpointURL, jsonData, "application/json"))
            {
                request.downloadHandler = new DownloadHandlerBuffer();

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }

                Debug.LogError($"NovaForge API Error: {request.error} | Response: {request.downloadHandler.text}");
                return null;
            }
        }

        private static string EscapeJson(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }
    }
}
