using System.Collections.Generic;
using NovaForge.Models;
using UnityEngine;

namespace NovaForge.Core
{
    public static class NovaForgeSceneParser
    {
        public static void ParseAndBuild(string jsonResponse)
        {
            if (string.IsNullOrWhiteSpace(jsonResponse))
            {
                Debug.LogError("NovaForgeSceneParser received empty JSON response.");
                return;
            }

            NovaForgeSceneRecipe sceneRecipe = JsonUtility.FromJson<NovaForgeSceneRecipe>(jsonResponse);
            if (sceneRecipe == null)
            {
                Debug.LogError("NovaForgeSceneParser failed to deserialize JSON response.");
                return;
            }

            ApplyLighting(sceneRecipe.environment?.lighting);

            if (sceneRecipe.objects == null)
            {
                return;
            }

            foreach (ObjectData objectData in sceneRecipe.objects)
            {
                if (objectData == null)
                {
                    continue;
                }

                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = string.IsNullOrWhiteSpace(objectData.id) ? "NovaForgeObject" : objectData.id;

                Transform transform = cube.transform;
                transform.position = ToVector3(objectData.transform?.position, Vector3.zero);
                transform.rotation = Quaternion.Euler(ToVector3(objectData.transform?.rotation, Vector3.zero));
                transform.localScale = ToVector3(objectData.transform?.scale, Vector3.one);
            }
        }

        private static void ApplyLighting(LightingData lighting)
        {
            if (lighting == null)
            {
                return;
            }

            if (TryParseColor(lighting.sky_color, out Color skyColor))
            {
                RenderSettings.ambientSkyColor = skyColor;
            }

            if (TryParseColor(lighting.equator_color, out Color equatorColor))
            {
                RenderSettings.ambientEquatorColor = equatorColor;
            }

            if (TryParseColor(lighting.ground_color, out Color groundColor))
            {
                RenderSettings.ambientGroundColor = groundColor;
            }
        }

        private static bool TryParseColor(string hexColor, out Color color)
        {
            if (!string.IsNullOrWhiteSpace(hexColor))
            {
                return ColorUtility.TryParseHtmlString(hexColor, out color);
            }

            color = default;
            return false;
        }

        private static Vector3 ToVector3(IReadOnlyList<float> values, Vector3 fallback)
        {
            if (values == null || values.Count < 3)
            {
                return fallback;
            }

            return new Vector3(values[0], values[1], values[2]);
        }
    }
}
