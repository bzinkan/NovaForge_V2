using UnityEngine;

namespace NovaForge.Settings
{
    [CreateAssetMenu(fileName = "NovaForgeConfig", menuName = "NovaForge/Config", order = 0)]
    public class NovaForgeConfig : ScriptableObject
    {
        [Tooltip("The URL of your NovaForge Replit Server")]
        public string saasEndpointURL;

        [Tooltip("Your unique SaaS API Token")]
        public string userAuthToken;

        public bool useDebugServer = false;
    }
}
