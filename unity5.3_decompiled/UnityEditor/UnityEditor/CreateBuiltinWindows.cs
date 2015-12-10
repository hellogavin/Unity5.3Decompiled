namespace UnityEditor
{
    using System;

    internal class CreateBuiltinWindows
    {
        [MenuItem("Window/Animation %6", false, 0x7d6)]
        private static void ShowAnimationWindow()
        {
            EditorWindow.GetWindow<AnimationWindow>();
        }

        [MenuItem("Window/Audio Mixer %8", false, 0x7d8)]
        private static void ShowAudioMixer()
        {
            AudioMixerWindow.Create();
        }

        [MenuItem("Window/Console %#c", false, 0x898)]
        private static void ShowConsole()
        {
            EditorWindow.GetWindow<ConsoleWindow>();
        }

        [MenuItem("Window/Game %2", false, 0x7d1)]
        private static void ShowGameView()
        {
            EditorWindow.GetWindow<GameView>();
        }

        [MenuItem("Window/Inspector %3", false, 0x7d2)]
        private static void ShowInspector()
        {
            EditorWindow.GetWindow<InspectorWindow>();
        }

        [MenuItem("Window/Hierarchy %4", false, 0x7d3)]
        private static void ShowNewHierarchy()
        {
            EditorWindow.GetWindow<SceneHierarchyWindow>();
        }

        private static void ShowProfilerWindow()
        {
            EditorWindow.GetWindow<ProfilerWindow>();
        }

        [MenuItem("Window/Project %5", false, 0x7d4)]
        private static void ShowProject()
        {
            EditorWindow.GetWindow<ProjectBrowser>();
        }

        [MenuItem("Window/Scene %1", false, 0x7d0)]
        private static void ShowSceneView()
        {
            EditorWindow.GetWindow<SceneView>();
        }

        [MenuItem("Window/Sprite Packer", false, 0x7de)]
        private static void ShowSpritePackerWindow()
        {
            EditorWindow.GetWindow<PackerWindow>();
        }

        private static void ShowVersionControl()
        {
            if (EditorSettings.externalVersionControl == ExternalVersionControl.AssetServer)
            {
                ASEditorBackend.DoAS();
            }
            else
            {
                EditorWindow.GetWindow<WindowPending>();
            }
        }
    }
}

