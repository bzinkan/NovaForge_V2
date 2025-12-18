using System;
using System.Collections.Generic;
using UnityEngine;

namespace NoveForge.Runtime.Models
{
    [Serializable]
    public class NoveForgeScene
    {
        public List<SceneObject> objects = new List<SceneObject>();
    }

    [Serializable]
    public class SceneObject
    {
        public Vector3 position;
        public Vector3 rotation;
    }
}
