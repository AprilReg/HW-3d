# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Unity 6 (6000.4.0f1) project using the Universal Render Pipeline (URP). This is a homework assignment project built on the URP Empty Template.

## Build & Run

This project has no custom build scripts. All building is done through the Unity Editor or Unity's CLI:

- **Open in Unity:** Launch Unity Hub and open `C:\Dev Projects\unity\HW1`
- **CLI build (Windows standalone):**
  ```
  Unity.exe -projectPath "C:\Dev Projects\unity\HW1" -buildWindows64Player "Build/HW1.exe" -quit
  ```
- **Run tests via CLI:**
  ```
  Unity.exe -projectPath "C:\Dev Projects\unity\HW1" -runTests -testPlatform EditMode -quit
  ```

The solution file is `HW1.slnx` — open in Rider or Visual Studio for C# editing.

## Architecture

Minimal URP starter project. The only custom C# code is tutorial scaffolding in `Assets/TutorialInfo/Scripts/`:
- `Readme.cs` — `ScriptableObject` data container for the welcome readme display
- `Editor/ReadmeEditor.cs` — `[CustomEditor(typeof(Readme))]` that auto-shows the readme on first load and renders formatted content with links

Primary scene: `Assets/Scenes/SampleScene.unity`

## Key Packages

| Package | Version | Purpose |
|---|---|---|
| `com.unity.render-pipelines.universal` | 17.4.0 | URP renderer |
| `com.unity.inputsystem` | 1.9.10 | New Input System (configured via `Assets/InputSystem_Actions.inputactions`) |
| `com.unity.ai.navigation` | 2.0.11 | NavMesh / AI navigation |
| `com.unity.test-framework` | 1.6.0 | Unity Test Runner (EditMode/PlayMode) |

## URP Configuration

Two quality tiers are configured under `Assets/Settings/`:
- **PC:** `PC_RPAsset.asset` + `PC_Renderer.asset`
- **Mobile:** `Mobile_RPAsset.asset` + `Mobile_Renderer.asset`

Quality switching is managed via `ProjectSettings/QualitySettings.asset`.
