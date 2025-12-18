using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// 👇 THIS NAMESPACE MUST MATCH WHAT YOUR WINDOW SCRIPT EXPECTS
namespace NovaForge.Networking 
{
    public class NovaForgeAPIManager
    {
        // A generic POST method to handle your AI calls
        public async Task<string> PostAsync(string url, string jsonData, string apiKey)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                
                request.SetRequestHeader("Content-Type", "application/json");
                
                // Only add Auth header if a key is provided
                if (!string.IsNullOrEmpty(apiKey))
                {
                    request.SetRequestHeader("Authorization", "Bearer " + apiKey);
                }

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"NovaForge API Error: {request.error} | Response: {request.downloadHandler.text}");
                    return null;
                }
            }
        }
    }
}
