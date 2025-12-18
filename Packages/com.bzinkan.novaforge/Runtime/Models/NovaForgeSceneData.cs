using System;
using System.Collections.Generic;
using UnityEngine;

namespace NovaForge.Models
{
    [Serializable]
    public class NovaForgeSceneRecipe
    {
        public string novaforge_version;
        public string scene_id;
        public Metadata metadata;
        public EnvironmentData environment;
        public List<ObjectData> objects;
    }

    [Serializable]
    public class Metadata
    {
        public string world_name;
        public int generation_seed;
    }

    [Serializable]
    public class EnvironmentData
    {
        public LightingData lighting;
        public TerrainData terrain;
    }

    [Serializable]
    public class LightingData
    {
        public string ambient_mode;
        public string sky_color;
        public string equator_color;
        public string ground_color;
        public float sun_intensity;
        public float fog_density;
    }

    [Serializable]
    public class TerrainData
    {
        public bool enabled;
        public float[] size; // [width, length, height]
        public string heightmap_url;
    }

    [Serializable]
    public class ObjectData
    {
        public string id;
        public string source; // "MESHY", "LEONARDO", etc.
        public string asset_type;
        public TransformData transform;
    }

    [Serializable]
    public class TransformData
    {
        public float[] position;
        public float[] rotation;
        public float[] scale;
    }
}
