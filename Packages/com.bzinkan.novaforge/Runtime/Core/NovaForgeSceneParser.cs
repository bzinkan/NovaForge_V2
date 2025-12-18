using UnityEngine;
using NovaForge.Models;

namespace NovaForge.Core
{
    public static class NovaForgeSceneParser
    {
        public static void ParseAndBuild(string jsonResponse)
        {
            // 1. Convert JSON to C# Object
            NovaForgeSceneRecipe recipe = JsonUtility.FromJson<NovaForgeSceneRecipe>(jsonResponse);

            if (recipe == null)
            {
                Debug.LogError("[NovaForge] Failed to parse scene recipe.");
                return;
            }

            Debug.Log($"[NovaForge] Building Scene: {recipe.metadata.world_name}");

            // 2. Apply Lighting
            ApplyLighting(recipe.environment.lighting);

            // 3. Build Terrain (Stub)
            if (recipe.environment.terrain.enabled)
            {
                Debug.Log($"[NovaForge] Terrain requested. Size: {recipe.environment.terrain.size[0]}x{recipe.environment.terrain.size[1]}");
            }

            // 4. Spawn Objects
            foreach (var obj in recipe.objects)
            {
                SpawnPlaceholder(obj);
            }
        }

        private static void ApplyLighting(LightingData data)
        {
            Color sky, equator, ground;
            ColorUtility.TryParseHtmlString(data.sky_color, out sky);
            ColorUtility.TryParseHtmlString(data.equator_color, out equator);
            ColorUtility.TryParseHtmlString(data.ground_color, out ground);

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = sky;
            RenderSettings.ambientEquatorColor = equator;
            RenderSettings.ambientGroundColor = ground;
            RenderSettings.fogDensity = data.fog_density;
            
            Light sun = GameObject.FindObjectOfType<Light>();
            if (sun != null && sun.type == LightType.Directional)
            {
                sun.intensity = data.sun_intensity;
            }
        }

        private static void SpawnPlaceholder(ObjectData objData)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = $"{objData.source}_{objData.id}";
            
            Vector3 pos = new Vector3(objData.transform.position[0], objData.transform.position[1], objData.transform.position[2]);
            Vector3 rot = new Vector3(objData.transform.rotation[0], objData.transform.rotation[1], objData.transform.rotation[2]);
            Vector3 scale = new Vector3(objData.transform.scale[0], objData.transform.scale[1], objData.transform.scale[2]);

            cube.transform.position = pos;
            cube.transform.eulerAngles = rot;
            cube.transform.localScale = scale;
        }
    }
}
