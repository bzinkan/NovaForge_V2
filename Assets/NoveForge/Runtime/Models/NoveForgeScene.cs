using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoveForge.Models
{
    [Serializable]
    public class NoveForgeScene
    {
        public string sceneName;
        public string environmentType;
        public List<SceneObject> objects;

        [Serializable]
        public class SceneObject
        {
            public string name;
            public string assetId;
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale = Vector3.one;
        }
    }
}
