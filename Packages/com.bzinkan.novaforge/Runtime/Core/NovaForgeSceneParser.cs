using UnityEngine;
using NovaForge.Models;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace NovaForge.Core
{
    public static class NovaForgeSceneParser
    {
        public static async void ParseAndBuild(string jsonResponse)
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

            // 3. Build Terrain
            if (recipe.environment.terrain.enabled)
            {
                await BuildTerrainAsync(recipe.environment.terrain);
            }

            // 4. Spawn Objects
            if (recipe.objects != null)
            {
                foreach (var obj in recipe.objects)
                {
                    SpawnPlaceholder(obj);
                }
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
            RenderSettings.fog = true;
            
            Light sun = GameObject.FindObjectOfType<Light>();
            if (sun != null && sun.type == LightType.Directional)
            {
                sun.intensity = data.sun_intensity;
            }
        }

        private static async Task BuildTerrainAsync(TerrainData data)
        {
            Debug.Log("[NovaForge] Downloading Heightmap...");
            
            // 1. Download the Texture
            Texture2D heightmapTexture = await DownloadTextureAsync(data.heightmap_url);
            if (heightmapTexture == null) return;

            // 2. Create Terrain Data
            UnityEngine.TerrainData terrainData = new UnityEngine.TerrainData();
            
            // Unity Terrain Size: [Width (x), Height (y - altitude), Length (z)]
            // JSON sends [Width, Length, Altitude]
            terrainData.heightmapResolution = 513; // Standard Unity resolution
            terrainData.size = new Vector3(data.size[0], data.size[2], data.size[1]);

            // 3. Apply Heights
            float[,] heights = GenerateHeightsFromTexture(heightmapTexture, 513);
            terrainData.SetHeights(0, 0, heights);

            // 4. Spawn the GameObject
            GameObject terrainObj = UnityEngine.Terrain.CreateTerrainGameObject(terrainData);
            terrainObj.name = "NovaForge Terrain";
            terrainObj.transform.position = Vector3.zero;
        }

        private static async Task<Texture2D> DownloadTextureAsync(string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone) await Task.Yield();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return DownloadHandlerTexture.GetContent(request);
                }
                else
                {
                    Debug.LogError($"[NovaForge] Heightmap Download Failed: {request.error}");
                    return null;
                }
            }
        }

        private static float[,] GenerateHeightsFromTexture(Texture2D texture, int resolution)
        {
            float[,] heights = new float[resolution, resolution];
            
            // Resize logic would go here, but for now we sample the center
            // Simple approach: Map texture pixels to height array
            for (int x = 0; x < resolution; x++)
            {
                for (int y = 0; y < resolution; y++)
                {
                    // Calculate normalized UV coordinates (0.0 to 1.0)
                    float u = (float)x / (resolution - 1);
                    float v = (float)y / (resolution - 1);
                    
                    // Sample the texture pixel (Grayscale value = Height)
                    Color pixel = texture.GetPixelBilinear(u, v);
                    heights[y, x] = pixel.grayscale;
                }
            }
            return heights;
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
