using UnityEngine;

namespace NovaForge.Settings
{
    [CreateAssetMenu(fileName = "NovaForgeConfig", menuName = "NovaForge/Config")]
    public class NovaForgeConfig : ScriptableObject
    {
        [Header("Server Configuration")]
        [Tooltip("The URL of your Replit backend. Ensure there is no trailing slash at the end.")]
        // I changed 'Url' to 'URL' below to match your other scripts
        public string saasEndpointURL = "https://novaforge.replit.app";

        [Header("Authentication")]
        [Tooltip("Paste your API Key from the Replit Dashboard here (e.g., nf_live_...).")]
        public string userAuthToken;
    }
}
