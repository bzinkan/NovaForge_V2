using UnityEngine;

namespace NoveForge.Runtime.Settings
{
    [CreateAssetMenu(fileName = "NoveForgeSettings", menuName = "NoveForge/Settings", order = 0)]
    public class NoveForgeSettings : ScriptableObject
    {
        public string openAIKey;
        public string meshyKey;
        public string leonardoKey;
    }
}
