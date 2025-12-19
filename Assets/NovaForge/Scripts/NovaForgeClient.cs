using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

[Serializable]
public class ForgeRequest
{
    public string api_key;
    public string prompt;
    public string image_url; // Optional: for image-to-3d
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
    // REPLACE with your actual Replit URL (e.g. https://novaforge.username.replit.app)
    public string saasUrl = "https://YOUR-REPL-NAME.replit.app";
    // REPLACE with an API Key from your Replit Database
    public string apiKey = "nf_live_YOUR_KEY_HERE";

    [Header("Scene Settings")]
    public GameObject placeholderPrefab; // Assign a generic Cube/Wireframe here
    public Transform spawnPoint;

    // Call this from your UI Input Field
    public void SendForgeRequest(string userPrompt)
    {
        StartCoroutine(PostRequest(userPrompt));
    }

    IEnumerator PostRequest(string prompt)
    {
        string url = saasUrl + "/api/generate";

        // 1. Create JSON Payload
        ForgeRequest req = new ForgeRequest();
        req.api_key = apiKey;
        req.prompt = prompt;

        string jsonBody = JsonUtility.ToJson(req);

        // 2. Setup Web Request
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"🚀 NovaForge: Contacting Architect for '{prompt}'...");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ NovaForge Error: " + request.error);
        }
        else
        {
            // 3. Handle Response
            Debug.Log("✅ Architect Blueprints Received: " + request.downloadHandler.text);
            ProcessResponse(request.downloadHandler.text);
        }
    }

    void ProcessResponse(string json)
    {
        // Parse the JSON from Replit
        ForgeResponse response = JsonUtility.FromJson<ForgeResponse>(json);

        if (response.status == "success")
        {
            Debug.Log($"🔨 Dispatched to: {response.worker}");

            // 4. SPAWN & SCALE PLACEHOLDER
            // We don't have the model yet (it takes time), but we know the SIZE!
            if (placeholderPrefab != null)
            {
                GameObject obj = Instantiate(placeholderPrefab, spawnPoint.position, Quaternion.identity);
                obj.name = "Pending_Forge_Asset";

                // --- 🌟 THE MAGIC LINE 🌟 ---
                // Applies the exact dimensions calculated by OpenAI
                float w = response.dimensions.width;
                float h = response.dimensions.height;

                // Unity Default Cube is 1x1x1 meters.
                // Scaling it to (w, h, w) makes it the real-world size.
                obj.transform.localScale = new Vector3(w, h, w);

                Debug.Log($"📏 Placeholder Scaled to: {h}m Height x {w}m Width");
            }
        }
        else
        {
            Debug.LogError("Server Message: " + response.message);
        }
    }
}
