namespace UnityEditor.NScreen
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEngine;

    internal class NScreenManager : ScriptableSingleton<NScreenManager>
    {
        [CompilerGenerated]
        private static Func<EditorWindow, bool> <>f__am$cache3;
        [SerializeField]
        private bool m_BuildOnPlay = true;
        [SerializeField]
        private int m_LatestId;
        [SerializeField]
        private int m_SelectedSizeIndex;

        static NScreenManager()
        {
            EditorApplication.playmodeStateChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.playmodeStateChanged, new EditorApplication.CallbackFunction(NScreenManager.PlayModeStateChanged));
        }

        internal static void Build()
        {
            string[] array = new string[EditorBuildSettings.scenes.Length];
            int index = 0;
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    array[index] = EditorBuildSettings.scenes[i].path;
                    index++;
                }
            }
            Array.Resize<string>(ref array, index);
            Directory.CreateDirectory("Temp/NScreen");
            ResolutionDialogSetting displayResolutionDialog = PlayerSettings.displayResolutionDialog;
            bool runInBackground = PlayerSettings.runInBackground;
            bool defaultIsFullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;
            PlayerSettings.runInBackground = true;
            PlayerSettings.defaultIsFullScreen = false;
            try
            {
                if (IntPtr.Size == 4)
                {
                    BuildPipeline.BuildPlayer(array, "Temp/NScreen/NScreen.app", BuildTarget.StandaloneOSXIntel, BuildOptions.CompressTextures);
                }
                else
                {
                    BuildPipeline.BuildPlayer(array, "Temp/NScreen/NScreen.app", BuildTarget.StandaloneOSXIntel64, BuildOptions.CompressTextures);
                }
            }
            finally
            {
                PlayerSettings.displayResolutionDialog = displayResolutionDialog;
                PlayerSettings.runInBackground = runInBackground;
                PlayerSettings.defaultIsFullScreen = defaultIsFullScreen;
            }
        }

        internal int GetNewId()
        {
            return ++this.m_LatestId;
        }

        internal static void Init()
        {
            RemoteGame window = (RemoteGame) EditorWindow.GetWindow(typeof(RemoteGame));
            if (EditorApplication.isPlaying && !window.IsRunning())
            {
                window.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                window.StartGame();
            }
        }

        internal static void OpenAnotherWindow()
        {
            RemoteGame game = ScriptableObject.CreateInstance<RemoteGame>();
            foreach (ContainerWindow window in ContainerWindow.windows)
            {
                foreach (View view in window.mainView.allChildren)
                {
                    DockArea area = view as DockArea;
                    if (area != null)
                    {
                        if (<>f__am$cache3 == null)
                        {
                            <>f__am$cache3 = pane => pane.GetType() == typeof(RemoteGame);
                        }
                        if (area.m_Panes.Any<EditorWindow>(<>f__am$cache3))
                        {
                            area.AddTab(game);
                            break;
                        }
                    }
                }
            }
            game.Show();
            if (EditorApplication.isPlaying)
            {
                game.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                game.StartGame();
            }
        }

        internal static void PlayModeStateChanged()
        {
            if (!EditorApplication.isPaused)
            {
                if ((!EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode) && ((Resources.FindObjectsOfTypeAll<RemoteGame>().Length > 0) && ScriptableSingleton<NScreenManager>.instance.BuildOnPlay))
                {
                    Build();
                }
                if (EditorApplication.isPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    StartAll();
                }
                else if (EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    StopAll();
                }
                else if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    RepaintAllGameViews();
                }
            }
        }

        internal static void RepaintAllGameViews()
        {
            foreach (RemoteGame game in Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.Repaint();
                game.GameViewAspectWasChanged();
            }
        }

        internal void ResetIds()
        {
            this.m_LatestId = 0;
        }

        internal static void StartAll()
        {
            ScriptableSingleton<NScreenManager>.instance.ResetIds();
            foreach (RemoteGame game in Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.id = ScriptableSingleton<NScreenManager>.instance.GetNewId();
                game.StartGame();
            }
        }

        internal static void StopAll()
        {
            foreach (RemoteGame game in Resources.FindObjectsOfTypeAll<RemoteGame>())
            {
                game.StopGame();
            }
        }

        internal bool BuildOnPlay
        {
            get
            {
                return (this.m_BuildOnPlay || !this.HasBuild);
            }
            set
            {
                this.m_BuildOnPlay = value;
            }
        }

        internal bool HasBuild
        {
            get
            {
                return Directory.Exists("Temp/NScreen/NScreen.app");
            }
        }

        internal int SelectedSizeIndex
        {
            get
            {
                return this.m_SelectedSizeIndex;
            }
            set
            {
                this.m_SelectedSizeIndex = value;
            }
        }
    }
}

