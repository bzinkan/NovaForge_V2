using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovaForge.Models
{
    [Serializable]
    public class NovaForgeJob
    {
        public string job_id;
        public string output_name;
        public string output_prefix;
        public TerrainConfig terrain;
        public SkyConfig sky;
        public List<ObjectConfig> objects;

        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
    }

    [Serializable]
    public class TerrainConfig
    {
        public string heightmap_url;
        public float vertical_scale;
    }

    [Serializable]
    public class SkyConfig
    {
        public string hdri_url;
    }

    [Serializable]
    public class ObjectConfig
    {
        public string model_url;
        public Vector3 location;
        public Vector3 rotation;
        public Vector3 scale;
    }
}
