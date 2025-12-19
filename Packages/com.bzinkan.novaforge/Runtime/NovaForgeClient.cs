using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using NovaForge.Settings;

namespace NovaForge.Runtime
{
    [Serializable]
    public class ForgeRequest
    {
        public string api_key;
        public string prompt;
        public string image_url;
    }

    [Serializable]
    public class ForgeResponse
    {
        public string status;
        public string worker;
        public Dimensions dimensions;
        public string message;
    }

    [Serializable]
    public class Dimensions
    {
        public float height;
        public float width;
        public string category;
    }

    public class NovaForgeClient : MonoBehaviour
    {
        [Header("Configuration")]
        public NovaForgeConfig config;

        [Header("Scene Settings")]
        public GameObject placeholderPrefab;
        public Transform spawnPoint;

        public void SendForgeRequest(string userPrompt)
        {
            if (config == null)
            {
                Debug.LogError("❌ NovaForge: No Configuration assigned!");
                return;
            }
            StartCoroutine(PostRequest(userPrompt));
        }

        IEnumerator PostRequest(string prompt)
        {
            string url = config.saasEndpointURL + "/api/generate";

            ForgeRequest req = new ForgeRequest();
            req.api_key = config.userAuthToken;
            req.prompt = prompt;

            string jsonBody = JsonUtility.ToJson(req);

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"🚀 NovaForge: Contacting Architect for '{prompt}'...");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("❌ NovaForge Error: " + request.error + " | " + request.downloadHandler.text);
            }
            else
            {
                Debug.Log("✅ Architect Blueprints Received.");
                ProcessResponse(request.downloadHandler.text);
            }
        }

        void ProcessResponse(string json)
        {
            ForgeResponse response = JsonUtility.FromJson<ForgeResponse>(json);

            if (response.status == "success")
            {
                Debug.Log($"🔨 Dispatched to: {response.worker}");

                if (placeholderPrefab != null)
                {
                    GameObject obj = Instantiate(placeholderPrefab, spawnPoint.position, Quaternion.identity);
                    obj.name = "Pending_Forge_Asset";

                    if (response.dimensions != null)
                    {
                        float w = response.dimensions.width;
                        float h = response.dimensions.height;
                        obj.transform.localScale = new Vector3(w, h, w);
                        Debug.Log($"📏 Placeholder Scaled to: {h}m Height x {w}m Width");
                    }
                }
            }
            else
            {
                Debug.LogError("Server Message: " + response.message);
            }
        }
    }
}
