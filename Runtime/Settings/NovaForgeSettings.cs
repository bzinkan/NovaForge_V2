using UnityEngine;

namespace NovaForge.Settings
{
    /// <summary>
    /// Stores API credentials for NovaForge services.
    /// </summary>
    [CreateAssetMenu(fileName = "NovaForgeSettings", menuName = "NovaForge/Settings", order = 0)]
    public class NovaForgeSettings : ScriptableObject
    {
        [Header("API Keys")]
        [SerializeField] private string openAIKey;
        [SerializeField] private string meshyKey;
        [SerializeField] private string leonardoKey;

        public string OpenAIKey => openAIKey;
        public string MeshyKey => meshyKey;
        public string LeonardoKey => leonardoKey;
    }
}
