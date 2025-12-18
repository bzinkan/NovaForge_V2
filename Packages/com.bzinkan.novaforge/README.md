# NovaForge V2

NovaForge V2 is an AI-powered Unity package that orchestrates scene generation inside the editor using services like OpenAI, Meshy, and Leonardo. The repository is structured as a Unity Package Manager (UPM) package so it can be dropped directly into projects.

## Package manifest

Root `package.json` aligns with Unity 2021.3 and identifies the package for UPM consumption as `com.bzinkan.novaforge`.
When cloning or downloading the repository, the folder itself (`NovaForge_V2/`) is the Unity package root that should be added
to your `Packages/` directory or referenced via Git in the Package Manager.

## Project layout

```
/NovaForge_V2
├── package.json
├── Runtime/
│   ├── NovaForge.Runtime.asmdef
│   ├── Networking/
│   ├── Models/
│   └── Settings/
├── Editor/
│   ├── NovaForge.Editor.asmdef
│   └── NovaForgeWindow.cs
├── .gitignore
└── README.md
```

### Runtime
- **Networking/NovaForgeAPIManager.cs** — Handles async POST requests via `UnityWebRequest` for downstream services.
- **Settings/NovaForgeSettings.cs** — ScriptableObject storage for OpenAI, Meshy, and Leonardo API keys.
- **Models/NovaForgeScene.cs** — Serializable scene and scene-object containers used to exchange generation results.

### Editor
- **NovaForgeWindow.cs** — Opens from `NovaForge/Open Generator` and provides a prompt-driven UI entry point.

## Usage
1. Import the package through UPM using the Git URL of this repository.
2. Create a `NovaForgeSettings` asset via **Create > NovaForge > Settings** and populate API keys.
3. Open **NovaForge > Open Generator** to access the generator window.

## Roadmap
- Wire editor actions to the networking layer for live generation calls.
- Expand scene models to mirror service responses.
- Add validation and diagnostics for API connectivity.
