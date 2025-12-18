using UnityEngine;
using UnityEditor;
using NoveForge.Core;

namespace NoveForge.Editor
{
    public class NoveForgeWindow : EditorWindow
    {
        string userPrompt = "A futuristic cyberpunk alleyway...";

        [MenuItem("NoveForge/Open Generator")]
        public static void ShowWindow()
        {
            GetWindow<NoveForgeWindow>("NoveForge");
        }

        void OnGUI()
        {
            GUILayout.Label("NoveForge Generator", EditorStyles.boldLabel);
            
            GUILayout.Space(10);
            GUILayout.Label("Scene Description:");
            userPrompt = EditorGUILayout.TextArea(userPrompt, GUILayout.Height(60));

            GUILayout.Space(10);
            if (GUILayout.Button("Generate Scene 🚀", GUILayout.Height(40)))
            {
                 Debug.Log($"Generating scene for: {userPrompt}");
                 // Call Orchestrator here
            }
        }
    }
}
