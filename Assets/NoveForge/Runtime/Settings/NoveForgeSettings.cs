using UnityEngine;

namespace NoveForge.Settings
{
    [CreateAssetMenu(fileName = "NoveForgeSettings", menuName = "NoveForge/Settings")]
    public class NoveForgeSettings : ScriptableObject
    {
        [Header("API Keys")]
        public string openAIKey;
        public string meshyKey;
        public string leonardoKey;
        
        [Header("Endpoints")]
        public string generationEndpoint;
    }
}
