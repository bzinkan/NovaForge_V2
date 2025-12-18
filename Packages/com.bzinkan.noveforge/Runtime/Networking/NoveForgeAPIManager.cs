using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NoveForge.Runtime.Networking
{
    public static class NoveForgeAPIManager
    {
        public static async Task<string> PostAsync(string url, string jsonPayload, string bearerToken)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("URL cannot be null or empty.", nameof(url));
            }

            using UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload ?? string.Empty)),
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {bearerToken}");
            }

            var operation = request.SendWebRequest();
            await AwaitRequest(operation);

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.DataProcessingError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new InvalidOperationException($"Request failed: {request.error}");
            }

            return request.downloadHandler.text;
        }

        private static Task AwaitRequest(UnityWebRequestAsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<bool>();
            operation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
