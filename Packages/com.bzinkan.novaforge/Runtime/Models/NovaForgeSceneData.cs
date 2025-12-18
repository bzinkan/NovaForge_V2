using System;
using System.Collections.Generic;

namespace NovaForge.Models
{
    [Serializable]
    public class NovaForgeSceneRecipe
    {
        public string novaforge_version;
        public MetadataData metadata;
        public EnvironmentData environment;
        public List<ObjectData> objects = new();
    }

    [Serializable]
    public class MetadataData
    {
        public string world_name;
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
        public string sky_color;
        public string equator_color;
        public string ground_color;
        public float sun_intensity;
    }

    [Serializable]
    public class TerrainData
    {
        public bool enabled;
        public float[] size;
    }

    [Serializable]
    public class ObjectData
    {
        public string id;
        public string source;
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
