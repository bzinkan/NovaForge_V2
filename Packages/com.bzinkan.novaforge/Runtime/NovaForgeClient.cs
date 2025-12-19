using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;
using NovaForge.Settings;

namespace NovaForge.Runtime
{
    [Serializable]
    public class ForgeRequest { public string api_key; public string prompt; public string image_url; }

    [Serializable]
    public class ForgeResponse { public string status; public string worker; public Dimensions dimensions; public string message; }

    [Serializable]
    public class Dimensions { public float height; public float width; public string category; }

    public class NovaForgeClient : MonoBehaviour
    {
        public NovaForgeConfig config;
        public GameObject placeholderPrefab;
        public Transform spawnPoint;

        public void SendForgeRequest(string userPrompt)
        {
            if (config == null) { Debug.LogError("❌ NovaForge: No Config assigned!"); return; }
            StartCoroutine(PostRequest(userPrompt));
        }

        IEnumerator PostRequest(string prompt)
        {
            string url = config.saasEndpointURL + "/api/generate";
            // Uses 'userAuthToken' from config as 'api_key'
            string jsonBody = JsonUtility.ToJson(new ForgeRequest { api_key = config.userAuthToken, prompt = prompt });

            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"🚀 NovaForge: Contacting Architect for '{prompt}'...");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Response: " + request.downloadHandler.text);
                ProcessResponse(request.downloadHandler.text);
            }
            else { Debug.LogError("❌ Error: " + request.error); }
        }

        void ProcessResponse(string json)
        {
            ForgeResponse res = JsonUtility.FromJson<ForgeResponse>(json);
            if (res.status == "success" && placeholderPrefab != null)
            {
                GameObject obj = Instantiate(placeholderPrefab, spawnPoint.position, Quaternion.identity);
                // Apply OpenAI/Gemini Dimensions
                if (res.dimensions != null)
                    obj.transform.localScale = new Vector3(res.dimensions.width, res.dimensions.height, res.dimensions.width);
            }
        }
    }
}
