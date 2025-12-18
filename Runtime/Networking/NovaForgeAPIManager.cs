using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace NovaForge.Networking
{
    /// <summary>
    /// Handles outbound POST requests for NovaForge services.
    /// </summary>
    public class NovaForgeAPIManager
    {
        /// <summary>
        /// Sends an asynchronous POST request with a JSON payload.
        /// </summary>
        /// <param name="endpoint">Fully qualified URL for the request.</param>
        /// <param name="jsonPayload">Serialized JSON payload to send.</param>
        /// <param name="headers">Optional headers to include with the request.</param>
        /// <returns>Response body as a string if successful.</returns>
        /// <exception cref="ArgumentException">Thrown when the endpoint is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the request fails.</exception>
        public async Task<string> PostAsync(string endpoint, string jsonPayload, Dictionary<string, string> headers = null)
        {
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                throw new ArgumentException("Endpoint must be provided", nameof(endpoint));
            }

            using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST);
            var payloadBytes = string.IsNullOrEmpty(jsonPayload) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(payloadBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            await SendRequestAsync(request);

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new InvalidOperationException($"Request failed: {request.responseCode} - {request.error}");
            }

            return request.downloadHandler?.text ?? string.Empty;
        }

        private static Task SendRequestAsync(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<bool>();
            var asyncOp = request.SendWebRequest();
            asyncOp.completed += _ => tcs.TrySetResult(true);
            return tcs.Task;
        }
    }
}
