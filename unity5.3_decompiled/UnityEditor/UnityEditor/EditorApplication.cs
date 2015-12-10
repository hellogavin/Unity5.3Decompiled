namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.Scripting;

    public sealed class EditorApplication
    {
        internal static CallbackFunction assetBundleNameChanged;
        internal static CallbackFunction assetLabelsChanged;
        public static CallbackFunction delayCall;
        private static CallbackFunction delayedCallback;
        internal static CallbackFunction globalEventHandler;
        public static CallbackFunction hierarchyWindowChanged;
        public static HierarchyWindowItemCallback hierarchyWindowItemOnGUI;
        public static CallbackFunction modifierKeysChanged;
        public static CallbackFunction playmodeStateChanged;
        public static CallbackFunction projectWindowChanged;
        public static ProjectWindowItemCallback projectWindowItemOnGUI;
        private static float s_DelayedCallbackTime;
        public static CallbackFunction searchChanged;
        public static CallbackFunction update;
        internal static CallbackFunction windowsReordered;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Beep();
        internal static void CallDelayed(CallbackFunction function, float timeFromNow)
        {
            delayedCallback = function;
            s_DelayedCallbackTime = Time.realtimeSinceStartup + timeFromNow;
            update = (CallbackFunction) Delegate.Combine(update, new CallbackFunction(EditorApplication.CheckCallDelayed));
        }

        private static void CheckCallDelayed()
        {
            if (Time.realtimeSinceStartup > s_DelayedCallbackTime)
            {
                update = (CallbackFunction) Delegate.Remove(update, new CallbackFunction(EditorApplication.CheckCallDelayed));
                delayedCallback();
            }
        }

        public static void DirtyHierarchyWindowSorting()
        {
            foreach (SceneHierarchyWindow window in Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.DirtySortingMethods();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ExecuteMenuItem(string menuItemPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool ExecuteMenuItemOnGameObjects(string menuItemPath, GameObject[] objects);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool ExecuteMenuItemWithTemporaryContext(string menuItemPath, Object[] objects);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Exit(int returnValue);
        internal static void Internal_CallAssetBundleNameChanged()
        {
            if (assetBundleNameChanged != null)
            {
                assetBundleNameChanged();
            }
        }

        internal static void Internal_CallAssetLabelsHaveChanged()
        {
            if (assetLabelsChanged != null)
            {
                assetLabelsChanged();
            }
        }

        private static void Internal_CallDelayFunctions()
        {
            CallbackFunction delayCall = EditorApplication.delayCall;
            EditorApplication.delayCall = null;
            if (delayCall != null)
            {
                delayCall();
            }
        }

        [RequiredByNativeCode]
        private static void Internal_CallGlobalEventHandler()
        {
            if (globalEventHandler != null)
            {
                globalEventHandler();
            }
            WindowLayout.MaximizeKeyHandler();
            Event.current = null;
        }

        private static void Internal_CallHierarchyWindowHasChanged()
        {
            if (hierarchyWindowChanged != null)
            {
                hierarchyWindowChanged();
            }
        }

        private static void Internal_CallKeyboardModifiersChanged()
        {
            if (modifierKeysChanged != null)
            {
                modifierKeysChanged();
            }
        }

        private static void Internal_CallProjectWindowHasChanged()
        {
            if (projectWindowChanged != null)
            {
                projectWindowChanged();
            }
        }

        internal static void Internal_CallSearchHasChanged()
        {
            if (searchChanged != null)
            {
                searchChanged();
            }
        }

        private static void Internal_CallUpdateFunctions()
        {
            if (update != null)
            {
                update();
            }
        }

        private static void Internal_CallWindowsReordered()
        {
            if (windowsReordered != null)
            {
                windowsReordered();
            }
        }

        private static void Internal_PlaymodeStateChanged()
        {
            if (playmodeStateChanged != null)
            {
                playmodeStateChanged();
            }
        }

        private static void Internal_RepaintAllViews()
        {
            foreach (GUIView view in Resources.FindObjectsOfTypeAll(typeof(GUIView)))
            {
                view.Repaint();
            }
        }

        private static void Internal_SwitchSkin()
        {
            EditorGUIUtility.Internal_SwitchSkin();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation LoadLevelAdditiveAsyncInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadLevelAdditiveInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern AsyncOperation LoadLevelAsyncInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadLevelInPlayMode(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LockReloadAssemblies();
        [Obsolete("Use EditorSceneManager.MarkSceneDirty or EditorSceneManager.MarkAllScenesDirty")]
        public static void MarkSceneDirty()
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        [Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.EmptyScene)")]
        public static void NewEmptyScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
        }

        [Obsolete("Use EditorSceneManager.NewScene (NewSceneSetup.DefaultGameObjects)")]
        public static void NewScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        }

        public static void OpenProject(string projectPath, params string[] args)
        {
            OpenProjectInternal(projectPath, args);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void OpenProjectInternal(string projectPath, string[] args);
        [Obsolete("Use EditorSceneManager.OpenScene")]
        public static bool OpenScene(string path)
        {
            if (isPlaying)
            {
                throw new InvalidOperationException("EditorApplication.OpenScene() cannot be called when in the Unity Editor is in play mode.");
            }
            return EditorSceneManager.OpenScene(path).IsValid();
        }

        [Obsolete("Use EditorSceneManager.OpenScene")]
        public static void OpenSceneAdditive(string path)
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Exiting playmode.\nOpenSceneAdditive was called at a point where there was no active scene.\nThis usually means it was called in a PostprocessScene function during scene loading or it was called during playmode.\nThis is no longer allowed. Use SceneManager.LoadScene to load scenes at runtime or in playmode.");
            }
            Scene sourceScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
            Scene activeScene = SceneManager.GetActiveScene();
            SceneManager.MergeScenes(sourceScene, activeScene);
        }

        public static void RepaintAnimationWindow()
        {
            foreach (AnimEditor editor in AnimEditor.GetAllAnimationWindows())
            {
                editor.Repaint();
            }
        }

        public static void RepaintHierarchyWindow()
        {
            foreach (SceneHierarchyWindow window in Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow)))
            {
                window.Repaint();
            }
        }

        public static void RepaintProjectWindow()
        {
            foreach (ProjectBrowser browser in ProjectBrowser.GetAllProjectBrowsers())
            {
                browser.Repaint();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ReportUNetWeaver(string filename, string msg, bool isError);
        internal static void RequestRepaintAllViews()
        {
            Internal_RepaintAllViews();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SaveAssets();
        [Obsolete("Use EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo")]
        public static bool SaveCurrentSceneIfUserWantsTo()
        {
            return EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }

        [Obsolete("This function is internal and no longer supported")]
        internal static bool SaveCurrentSceneIfUserWantsToForce()
        {
            return false;
        }

        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene()
        {
            return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), string.Empty, false);
        }

        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene(string path)
        {
            return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, false);
        }

        [Obsolete("Use EditorSceneManager.SaveScene")]
        public static bool SaveScene(string path, bool saveAsCopy)
        {
            return EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), path, saveAsCopy);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetSceneRepaintDirty();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Step();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void UnlockReloadAssemblies();

        public static string applicationContentsPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string applicationPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Use EditorSceneManager to see which scenes are currently loaded")]
        public static string currentScene
        {
            get
            {
                Scene activeScene = SceneManager.GetActiveScene();
                if (activeScene.IsValid())
                {
                    return activeScene.path;
                }
                return string.Empty;
            }
            set
            {
            }
        }

        public static bool isCompiling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool isPlayingOrWillChangePlaymode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isRemoteConnected { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [Obsolete("Use EditorSceneManager to figure out which scene are dirty")]
        public static bool isSceneDirty
        {
            get
            {
                return SceneManager.GetActiveScene().isDirty;
            }
        }

        public static bool isUpdating { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static Object renderSettings { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static Object tagManager { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static double timeSinceStartup { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static string userJavascriptPackagesPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void CallbackFunction();

        public delegate void HierarchyWindowItemCallback(int instanceID, Rect selectionRect);

        public delegate void ProjectWindowItemCallback(string guid, Rect selectionRect);
    }
}

