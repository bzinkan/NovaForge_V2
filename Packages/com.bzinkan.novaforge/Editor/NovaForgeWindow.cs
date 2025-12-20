using System;
using UnityEditor;
using UnityEngine;
using NovaForge.Networking; // Connects to your API Manager
using NovaForge.Settings;
// using NovaForge.Models; // We'll uncomment this when we handle the response

namespace NovaForge.Editor
{
    public class NovaForgeWindow : EditorWindow
    {
        // 1. UI Variables
        string userPrompt = "A futuristic city with neon lights..."; // Default text
        string statusMessage = "Ready to create.";
        bool isProcessing = false;

        public NovaForgeConfig config;

        readonly NovaForgeAPIManager apiManager = new NovaForgeAPIManager();

        [Serializable]
        private class GenerationResponse
        {
            public string job_id;
        }

        // 2. Add the Menu Item
        [MenuItem("NovaForge/Open Generator ✨")]
        public static void ShowWindow()
        {
            // Opens the window and gives it a title
            GetWindow<NovaForgeWindow>("NovaForge");
        }

        // 3. Draw the User Interface
        void OnGUI()
        {
            // -- Header --
            GUILayout.Space(10);
            GUILayout.Label("NovaForge V2 Generator", EditorStyles.boldLabel);
            GUILayout.Space(5);

            config = (NovaForgeConfig)EditorGUILayout.ObjectField("Config", config, typeof(NovaForgeConfig), false);

            GUILayout.Space(10);

            // -- Prompt Input --
            GUILayout.Label("Describe your scene:", EditorStyles.label);
            
            // Create a scrollable text area with a fixed height
            GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
            userPrompt = EditorGUILayout.TextArea(userPrompt, textAreaStyle, GUILayout.Height(80));

            GUILayout.Space(15);

            // -- Generate Button --
            // If we are processing, disable the button so they can't spam click
            EditorGUI.BeginDisabledGroup(isProcessing);
            
            if (GUILayout.Button("Generate Scene 🚀", GUILayout.Height(40)))
            {
                TriggerGeneration();
            }
            
            EditorGUI.EndDisabledGroup();

            // -- Status Bar --
            GUILayout.Space(10);
            GUILayout.Label($"Status: {statusMessage}", EditorStyles.helpBox);
        }

        // 4. The Trigger Logic
        private async void TriggerGeneration()
        {
            if (config == null)
            {
                statusMessage = "Please assign a NovaForge Config asset.";
                return;
            }

            isProcessing = true;
            statusMessage = "Connecting to NovaForge Cloud...";
            
            // Just a debug log for now to prove the UI works
            Debug.Log($"[NovaForge] Sending prompt: {userPrompt}");

            string response = await apiManager.RequestSceneGeneration(userPrompt, config);

            if (string.IsNullOrEmpty(response))
            {
                statusMessage = "Generation failed. Check console for errors.";
                isProcessing = false;
                return;
            }

            GenerationResponse generationResponse = JsonUtility.FromJson<GenerationResponse>(response);
            if (generationResponse == null || string.IsNullOrEmpty(generationResponse.job_id))
            {
                statusMessage = "Generation response missing job id.";
                isProcessing = false;
                return;
            }

            statusMessage = "Generation started. Polling for output...";
            NovaForgeImporter.PollAndImport(generationResponse.job_id, userPrompt);

            isProcessing = false;
        }
    }
}
