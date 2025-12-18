using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NovaForge.Runtime
{
    /// <summary>
    /// Provides helpers for sending HTTP requests to NovaForge services.
    /// </summary>
    public static class NovaForgeAPIManager
    {
        /// <summary>
        /// Sends an asynchronous POST request with a JSON payload.
        /// </summary>
        /// <param name="endpoint">The full URL to post to.</param>
        /// <param name="jsonPayload">The JSON payload to send.</param>
        /// <param name="apiKey">API key used for bearer authorization.</param>
        /// <returns>Response body as a string when successful.</returns>
        public static async Task<string> PostAsync(string endpoint, string jsonPayload, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint must be provided", nameof(endpoint));
            }

            if (apiKey == null)
            {
                throw new ArgumentNullException(nameof(apiKey), "API key must be provided");
            }

            using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(string.IsNullOrEmpty(jsonPayload) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(jsonPayload)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            try
            {
                await SendRequestAsync(request);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    LogRequestError(endpoint, request);
                    throw new InvalidOperationException($"Request failed: {request.responseCode} - {request.error}");
                }

                return request.downloadHandler?.text ?? string.Empty;
            }
            catch (Exception ex)
            {
                Debug.LogError($"NovaForge POST request to {endpoint} encountered an exception: {ex.Message}");
                throw;
            }
        }

        private static Task SendRequestAsync(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<bool>();
            var asyncOp = request.SendWebRequest();
            asyncOp.completed += _ => tcs.TrySetResult(true);
            return tcs.Task;
        }

        private static void LogRequestError(string endpoint, UnityWebRequest request)
        {
            var responseBody = request.downloadHandler?.text;
            Debug.LogError($"NovaForge POST request to {endpoint} failed. Status: {request.responseCode}, Error: {request.error}, Response: {responseBody}");
        }
    }
}
