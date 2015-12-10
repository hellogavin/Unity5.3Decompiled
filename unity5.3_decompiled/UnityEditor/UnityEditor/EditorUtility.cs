namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.Scripting.Compilers;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class EditorUtility
    {
        [Obsolete("Use BuildPipeline.BuildAssetBundle instead")]
        public static bool BuildResourceFile(Object[] selection, string pathName)
        {
            return BuildPipeline.BuildAssetBundle(null, selection, pathName, BuildAssetBundleOptions.CompleteAssets);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearProgressBar();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object[] CollectDeepHierarchy(Object[] roots);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object[] CollectDependencies(Object[] roots);
        public static string[] CompileCSharp(string[] sources, string[] references, string[] defines, string outputFile)
        {
            return MonoCSharpCompiler.Compile(sources, references, defines, outputFile);
        }

        private static void CompressTexture(Texture2D texture, TextureFormat format)
        {
            CompressTexture(texture, format, TextureCompressionQuality.Normal);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CompressTexture(Texture2D texture, TextureFormat format, int quality);
        public static void CompressTexture(Texture2D texture, TextureFormat format, TextureCompressionQuality quality)
        {
            CompressTexture(texture, format, (int) quality);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void CopySerialized(Object source, Object dest);
        [Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
        public static Object CreateEmptyPrefab(string path)
        {
            return PrefabUtility.CreateEmptyPrefab(path);
        }

        public static GameObject CreateGameObjectWithHideFlags(string name, HideFlags flags, params Type[] components)
        {
            GameObject obj2 = Internal_CreateGameObjectWithHideFlags(name, flags);
            obj2.AddComponent(typeof(Transform));
            foreach (Type type in components)
            {
                obj2.AddComponent(type);
            }
            return obj2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool DisplayCancelableProgressBar(string title, string info, float progress);
        internal static void DisplayCustomMenu(Rect position, string[] options, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            bool[] separator = new bool[options.Length];
            DisplayCustomMenuWithSeparators(position, options, separator, selected, callback, userData);
        }

        public static void DisplayCustomMenu(Rect position, GUIContent[] options, int selected, SelectMenuItemFunction callback, object userData)
        {
            int[] numArray = new int[] { selected };
            string[] strArray = new string[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                strArray[i] = options[i].text;
            }
            DisplayCustomMenu(position, strArray, numArray, callback, userData);
        }

        internal static void DisplayCustomMenu(Rect position, string[] options, bool[] enabled, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            bool[] separator = new bool[options.Length];
            DisplayCustomMenuWithSeparators(position, options, enabled, separator, selected, callback, userData);
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            int[] enabled = new int[options.Length];
            int[] numArray2 = new int[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                enabled[i] = 1;
                numArray2[i] = 0;
            }
            Internal_DisplayCustomMenu(position, options, enabled, numArray2, selected, callback, userData);
            ResetMouseDown();
        }

        internal static void DisplayCustomMenuWithSeparators(Rect position, string[] options, bool[] enabled, bool[] separator, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            int[] numArray = new int[options.Length];
            int[] numArray2 = new int[options.Length];
            for (int i = 0; i < options.Length; i++)
            {
                numArray[i] = !enabled[i] ? 0 : 1;
                numArray2[i] = !separator[i] ? 0 : 1;
            }
            Internal_DisplayCustomMenu(position, options, numArray, numArray2, selected, callback, userData);
            ResetMouseDown();
        }

        [ExcludeFromDocs]
        public static bool DisplayDialog(string title, string message, string ok)
        {
            string cancel = string.Empty;
            return DisplayDialog(title, message, ok, cancel);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool DisplayDialog(string title, string message, string ok, [DefaultValue("\"\"")] string cancel);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int DisplayDialogComplex(string title, string message, string ok, string cancel, string alt);
        internal static void DisplayObjectContextMenu(Rect position, Object context, int contextUserData)
        {
            Object[] objArray1 = new Object[] { context };
            DisplayObjectContextMenu(position, objArray1, contextUserData);
        }

        internal static void DisplayObjectContextMenu(Rect position, Object[] context, int contextUserData)
        {
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            Internal_DisplayObjectContextMenu(position, context, contextUserData);
            ResetMouseDown();
        }

        public static void DisplayPopupMenu(Rect position, string menuItemPath, MenuCommand command)
        {
            if (((menuItemPath == "CONTEXT") || (menuItemPath == "CONTEXT/")) || (menuItemPath == @"CONTEXT\"))
            {
                bool flag = false;
                if (command == null)
                {
                    flag = true;
                }
                if ((command != null) && (command.context == null))
                {
                    flag = true;
                }
                if (flag)
                {
                    Debug.LogError("DisplayPopupMenu: invalid arguments: using CONTEXT requires a valid MenuCommand object. If you want a custom context menu then try using the GenericMenu.");
                    return;
                }
            }
            Vector2 vector = GUIUtility.GUIToScreenPoint(new Vector2(position.x, position.y));
            position.x = vector.x;
            position.y = vector.y;
            Internal_DisplayPopupMenu(position, menuItemPath, (command != null) ? command.context : null, (command != null) ? command.userData : 0);
            ResetMouseDown();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DisplayProgressBar(string title, string info, float progress);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool ExtractOggFile(Object obj, string path);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use AssetDatabase.LoadAssetAtPath"), WrapperlessIcall]
        public static extern Object FindAsset(string path, Type type);
        [Obsolete("Use PrefabUtility.FindPrefabRoot")]
        public static GameObject FindPrefabRoot(GameObject source)
        {
            return PrefabUtility.FindPrefabRoot(source);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FocusProjectWindow();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ForceRebuildInspectors();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ForceReloadInspectors();
        public static string FormatBytes(int bytes)
        {
            return FormatBytes((long) bytes);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string FormatBytes(long bytes);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetActiveNativePlatformSupportModuleName();
        [Obsolete("Use AssetDatabase.GetAssetPath")]
        public static string GetAssetPath(Object asset)
        {
            return AssetDatabase.GetAssetPath(asset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetDirtyIndex(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetInvalidFilenameChars();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetObjectEnabled(Object target);
        [Obsolete("Use PrefabUtility.GetPrefabParent")]
        public static Object GetPrefabParent(Object source)
        {
            return PrefabUtility.GetPrefabParent(source);
        }

        [Obsolete("Use PrefabUtility.GetPrefabType")]
        public static PrefabType GetPrefabType(Object target)
        {
            return PrefabUtility.GetPrefabType(target);
        }

        internal static void InitInstantiatedPreviewRecursive(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;
            go.layer = Camera.PreviewCullingLayer;
            IEnumerator enumerator = go.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    InitInstantiatedPreviewRecursive(current.gameObject);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object InstanceIDToObject(int instanceID);
        internal static GameObject InstantiateForAnimatorPreview(Object original)
        {
            if (original == null)
            {
                throw new ArgumentException("The prefab you want to instantiate is null.");
            }
            GameObject go = InstantiateRemoveAllNonAnimationComponents(original, Vector3.zero, Quaternion.identity) as GameObject;
            InitInstantiatedPreviewRecursive(go);
            Animator component = go.GetComponent<Animator>();
            if (component != null)
            {
                component.enabled = false;
                component.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                component.logWarnings = false;
                component.fireEvents = false;
            }
            return go;
        }

        [Obsolete("Use PrefabUtility.InstantiatePrefab")]
        public static Object InstantiatePrefab(Object target)
        {
            return PrefabUtility.InstantiatePrefab(target);
        }

        internal static Object InstantiateRemoveAllNonAnimationComponents(Object original, Vector3 position, Quaternion rotation)
        {
            if (original == null)
            {
                throw new ArgumentException("The prefab you want to instantiate is null.");
            }
            return Internal_InstantiateRemoveAllNonAnimationComponentsSingle(original, position, rotation);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DisplayCustomMenu(ref Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, SelectMenuItemFunction callback, object userData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref Rect position, Object[] context, int contextUserData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DisplayPopupMenu(ref Rect position, string menuItemPath, Object context, int contextUserData);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Object INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(Object data, ref Vector3 pos, ref Quaternion rot);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern GameObject Internal_CreateGameObjectWithHideFlags(string name, HideFlags flags);
        private static void Internal_DisplayCustomMenu(Rect screenPosition, string[] options, int[] enabled, int[] separator, int[] selected, SelectMenuItemFunction callback, object userData)
        {
            INTERNAL_CALL_Internal_DisplayCustomMenu(ref screenPosition, options, enabled, separator, selected, callback, userData);
        }

        internal static void Internal_DisplayObjectContextMenu(Rect position, Object[] context, int contextUserData)
        {
            INTERNAL_CALL_Internal_DisplayObjectContextMenu(ref position, context, contextUserData);
        }

        internal static void Internal_DisplayPopupMenu(Rect position, string menuItemPath, Object context, int contextUserData)
        {
            INTERNAL_CALL_Internal_DisplayPopupMenu(ref position, menuItemPath, context, contextUserData);
        }

        private static Object Internal_InstantiateRemoveAllNonAnimationComponentsSingle(Object data, Vector3 pos, Quaternion rot)
        {
            return INTERNAL_CALL_Internal_InstantiateRemoveAllNonAnimationComponentsSingle(data, ref pos, ref rot);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern string Internal_SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Internal_UpdateAllMenus();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void Internal_UpdateMenuTitleForLanguage(SystemLanguage newloc);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string InvokeDiffTool(string leftTitle, string leftFile, string rightTitle, string rightFile, string ancestorTitle, string ancestorFile);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsDirty(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsPersistent(Object target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void LoadPlatformSupportModuleNativeDllInternal(string target);
        public static bool LoadWindowLayout(string path)
        {
            bool newProjectLayoutWasCreated = false;
            return WindowLayout.LoadWindowLayout(path, newProjectLayoutWasCreated);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int NaturalCompare(string a, string b);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int NaturalCompareObjectNames(Object a, Object b);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string OpenFilePanel(string title, string directory, string extension);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string OpenFilePanelWithFilters(string title, string directory, string[] filters);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string OpenFolderPanel(string title, string folder, string defaultName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OpenWithDefaultApp(string fileName);
        [Obsolete("Use PrefabUtility.CreateEmptyPrefab")]
        public static bool ReconnectToLastPrefab(GameObject go)
        {
            return PrefabUtility.ReconnectToLastPrefab(go);
        }

        [Obsolete("Use PrefabUtility.ReplacePrefab")]
        public static GameObject ReplacePrefab(GameObject go, Object targetPrefab)
        {
            return PrefabUtility.ReplacePrefab(go, targetPrefab, ReplacePrefabOptions.Default);
        }

        [Obsolete("Use PrefabUtility.ReplacePrefab")]
        public static GameObject ReplacePrefab(GameObject go, Object targetPrefab, ReplacePrefabOptions options)
        {
            return PrefabUtility.ReplacePrefab(go, targetPrefab, options);
        }

        internal static void ResetMouseDown()
        {
            Tools.s_ButtonDown = -1;
            GUIUtility.hotControl = 0;
        }

        [Obsolete("Use PrefabUtility.ResetToPrefabState")]
        public static bool ResetToPrefabState(Object source)
        {
            return PrefabUtility.ResetToPrefabState(source);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RevealInFinder(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string SaveBuildPanel(BuildTarget target, string title, string directory, string defaultName, string extension, out bool updateExistingBuild);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string SaveFilePanel(string title, string directory, string defaultName, string extension);
        public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message)
        {
            return Internal_SaveFilePanelInProject(title, defaultName, extension, message, "Assets");
        }

        public static string SaveFilePanelInProject(string title, string defaultName, string extension, string message, string path)
        {
            return Internal_SaveFilePanelInProject(title, defaultName, extension, message, path);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string SaveFolderPanel(string title, string folder, string defaultName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetCameraAnimateMaterials(Camera camera, bool animate);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetDirty(Object target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetObjectEnabled(Object target, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetSelectedWireframeHidden(Renderer renderer, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead"), WrapperlessIcall]
        public static extern void UnloadUnusedAssets();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Use EditorUtility.UnloadUnusedAssetsImmediate instead")]
        public static extern void UnloadUnusedAssetsIgnoreManagedReferences();
        public static void UnloadUnusedAssetsImmediate()
        {
            UnloadUnusedAssetsImmediateInternal(true);
        }

        public static void UnloadUnusedAssetsImmediate(bool ignoreReferencesFromScript)
        {
            UnloadUnusedAssetsImmediateInternal(ignoreReferencesFromScript);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void UnloadUnusedAssetsImmediateInternal(bool ignoreReferencesFromScript);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool WarnPrefab(Object target, string title, string warning, string okButton);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool WSACreateTestCertificate(string path, string publisher, string password, bool overwrite);

        public static bool audioMasterMute { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        internal static bool audioProfilingEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public delegate void SelectMenuItemFunction(object userData, string[] options, int selected);
    }
}

