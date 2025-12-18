using UnityEngine;
using UnityEditor;
using NovaForge.Networking; // 👈 This enables access to the APIManager
// using NovaForge.Models;  // (Add this later when we implement the models)
using NovaForge.Settings;

namespace NovaForge.Editor
{
    /// <summary>
    /// Primary entry point for generating scenes within the Unity Editor.
    /// </summary>
    public class NovaForgeWindow : EditorWindow
    {
        private string prompt;
        private NovaForgeSettings settings;
        private readonly NovaForgeAPIManager apiManager = new();

        [MenuItem("NovaForge/Open Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<NovaForgeWindow>("NovaForge Generator");
            window.minSize = new Vector2(420, 320);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("NovaForge Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Provide a prompt to generate a 3D scene using your configured APIs.", MessageType.Info);

            using (new EditorGUILayout.VerticalScope("box"))
            {
                settings = (NovaForgeSettings)EditorGUILayout.ObjectField("Settings", settings, typeof(NovaForgeSettings), false);
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Prompt");
                prompt = EditorGUILayout.TextArea(prompt, GUILayout.Height(120));
            }

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(prompt) || settings == null);
            if (GUILayout.Button("Generate Scene"))
            {
                Debug.Log("NovaForge generation triggered.");
                // Future implementation will call apiManager.PostAsync with configured endpoints.
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
