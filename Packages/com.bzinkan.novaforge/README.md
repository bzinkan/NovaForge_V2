# NovaForge

NovaForge is a deterministic, data-driven Unity package for scene and terrain generation. The repository is structured as a Unity Package Manager (UPM) mono-repo so the actual package lives at `Packages/com.bzinkan.novaforge/`.

## Installation via Git URL
Add the package through Unity's Package Manager using:

```
https://github.com/bzinkan/NovaForge_V2.git?path=Packages/com.bzinkan.novaforge
```

## Package layout

```
NovaForge_V2/
└─ Packages/
   └─ com.bzinkan.novaforge/
      ├─ package.json
      ├─ README.md
      ├─ Runtime/
      │  ├─ NovaForge.Runtime.asmdef
      │  └─ (runtime scripts)
      └─ Editor/
         ├─ NovaForge.Editor.asmdef
         └─ (editor scripts)
```

### Runtime
- **Networking/NovaForgeAPIManager.cs** — Handles async POST requests via `UnityWebRequest` for downstream services.
- **Settings/NovaForgeSettings.cs** — ScriptableObject storage for OpenAI, Meshy, and Leonardo API keys.
- **Models/NovaForgeScene.cs** — Serializable scene and scene-object containers used to exchange generation results.

### Editor
- **NovaForgeWindow.cs** — Opens from `NovaForge/Open Generator` and provides a prompt-driven UI entry point.

## Usage
1. Import the package through UPM using the Git URL above.
2. Create a `NovaForgeSettings` asset via **Create > NovaForge > Settings** and populate API keys.
3. Open **NovaForge > Open Generator** to access the generator window.
