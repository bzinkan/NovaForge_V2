using UnityEditor;
using UnityEngine;

namespace NovaForge.Editor
{
    public class NovaForgeAssetProcessor : AssetPostprocessor
    {
        // This runs automatically every time a new .glb is downloaded
        void OnPostprocessModel(GameObject g)
        {
            if (g.name.Contains("Terrain"))
            {
                Renderer[] renderers = g.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    // Forcing the URP Lit shader which handles vertex colors
                    Shader vertexShader = Shader.Find("Universal Render Pipeline/Lit");

                    if (vertexShader != null)
                    {
                        r.sharedMaterial = new Material(vertexShader);
                        r.sharedMaterial.name = "Auto_VertexColor_Material";
                    }
                }
            }
        }
    }
}
