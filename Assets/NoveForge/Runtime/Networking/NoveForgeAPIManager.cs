using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NoveForge.Networking
{
    public class NoveForgeAPIManager
    {
        public async Task<string> PostAsync(string url, string jsonData, string apiKey)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + apiKey);

                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return request.downloadHandler.text;
                }
                else
                {
                    Debug.LogError($"NoveForge API Error: {request.error}");
                    return null;
                }
            }
        }
    }
}
