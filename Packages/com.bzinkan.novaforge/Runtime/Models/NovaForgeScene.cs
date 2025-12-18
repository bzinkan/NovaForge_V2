using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovaForge.Models
{
    /// <summary>
    /// Represents an object placed within a procedurally generated scene.
    /// </summary>
    [Serializable]
    public class SceneObject
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale = Vector3.one;
    }

    /// <summary>
    /// Container for scene metadata and contents.
    /// </summary>
    [Serializable]
    public class NovaForgeScene
    {
        public string sceneName;
        public List<SceneObject> objects = new();
    }
}
