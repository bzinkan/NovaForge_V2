using System;
using UnityEngine;

namespace NoveForge.Models
{
    [Serializable]
    public class NoveForgeAsset
    {
        public string assetId;
        public string assetType;
        public string status;
        public string downloadUrl;
        public string prompt;
    }
}
