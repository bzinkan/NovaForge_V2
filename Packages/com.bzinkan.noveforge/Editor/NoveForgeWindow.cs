using UnityEditor;
using UnityEngine;

namespace NoveForge.Editor
{
    public class NoveForgeWindow : EditorWindow
    {
        private string prompt = string.Empty;

        [MenuItem("NoveForge/Open Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<NoveForgeWindow>();
            window.titleContent = new GUIContent("NoveForge Generator");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Prompt", EditorStyles.boldLabel);
            prompt = EditorGUILayout.TextArea(prompt, GUILayout.Height(100));

            if (GUILayout.Button("Generate Scene"))
            {
                GenerateScene();
            }
        }

        private void GenerateScene()
        {
            Debug.Log($"Generate Scene clicked with prompt: {prompt}");
        }
    }
}
