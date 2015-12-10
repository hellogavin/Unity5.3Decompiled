namespace UnityEditorInternal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEngine;
    using UnityEngine.Internal;

    public sealed class InternalEditorUtility
    {
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache3;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map7;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map8;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int AddScriptComponentUnchecked(GameObject gameObject, MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void AddSortingLayer();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void AuxWindowManager_OnAssemblyReload();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern CanAppendBuild BuildCanBeAppended(BuildTarget target, string location);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void BumpMapSettingsFixingWindowReportResult(int result);
        internal static bool BumpMapTextureNeedsFixing(MaterialProperty prop)
        {
            if (prop.type == MaterialProperty.PropType.Texture)
            {
                bool flaggedAsNormal = (prop.flags & MaterialProperty.PropFlags.Normal) != MaterialProperty.PropFlags.None;
                foreach (Material material in prop.targets)
                {
                    if (BumpMapTextureNeedsFixingInternal(material, prop.name, flaggedAsNormal))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool BumpMapTextureNeedsFixingInternal(Material material, string propName, bool flaggedAsNormal);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void CalculateAmbientProbeFromSkybox();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string CalculateHashForObjectsAndDependencies(Object[] objects);
        public static Bounds CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection)
        {
            Bounds bounds;
            INTERNAL_CALL_CalculateSelectionBounds(usePivotOnlyForParticles, onlyUseActiveSelection, out bounds);
            return bounds;
        }

        internal static Bounds CalculateSelectionBoundsInSpace(Vector3 position, Quaternion rotation, bool rectBlueprintMode)
        {
            Quaternion quaternion = Quaternion.Inverse(rotation);
            Vector3 vector = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3[] vectorArray = new Vector3[2];
            foreach (GameObject obj2 in Selection.gameObjects)
            {
                Bounds localBounds = GetLocalBounds(obj2);
                vectorArray[0] = localBounds.min;
                vectorArray[1] = localBounds.max;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Vector3 vector3 = new Vector3(vectorArray[i].x, vectorArray[j].y, vectorArray[k].z);
                            if (rectBlueprintMode && SupportsRectLayout(obj2.transform))
                            {
                                Vector3 localPosition = obj2.transform.localPosition;
                                localPosition.z = 0f;
                                vector3 = obj2.transform.parent.TransformPoint(vector3 + localPosition);
                            }
                            else
                            {
                                vector3 = obj2.transform.TransformPoint(vector3);
                            }
                            vector3 = (Vector3) (quaternion * (vector3 - position));
                            for (int m = 0; m < 3; m++)
                            {
                                vector[m] = Mathf.Min(vector[m], vector3[m]);
                                vector2[m] = Mathf.Max(vector2[m], vector3[m]);
                            }
                        }
                    }
                }
            }
            return new Bounds((Vector3) ((vector + vector2) * 0.5f), vector2 - vector);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool CanConnectToCacheServer();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void ClearSceneLighting();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int CreateScriptableObjectUnchecked(MonoScript script);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern DllType DetectDotNetDll(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int DetermineDepthOrder(Transform lhs, Transform rhs);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DrawSkyboxMaterial(Material mat, Camera cam);
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("use EditorSceneManager.EnsureUntitledSceneHasBeenSaved"), WrapperlessIcall]
        public static extern bool EnsureSceneHasBeenSaved(string operation);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExecuteCommandOnKeyWindow(string commandName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ulong FixCacheServerIntegrityErrors();
        internal static void FixHDRTexture(MaterialProperty prop)
        {
            string assetPath = AssetDatabase.GetAssetPath(prop.textureValue);
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (atPath != null)
            {
                TextureImporterFormat textureFormat = TextureImporterFormat.RGB24;
                atPath.textureFormat = textureFormat;
                foreach (BuildPlayerWindow.BuildPlatform platform in BuildPlayerWindow.GetValidPlatforms())
                {
                    int num;
                    int num2;
                    TextureImporterFormat format2;
                    if (atPath.GetPlatformTextureSettings(platform.name, out num, out format2, out num2))
                    {
                        atPath.SetPlatformTextureSettings(platform.name, num, textureFormat, num2, false);
                    }
                }
                AssetDatabase.ImportAsset(assetPath);
                foreach (Object obj2 in prop.targets)
                {
                    EditorUtility.SetDirty(obj2);
                }
            }
        }

        internal static void FixNormalmapTexture(MaterialProperty prop)
        {
            foreach (Material material in prop.targets)
            {
                FixNormalmapTextureInternal(material, prop.name);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void FixNormalmapTextureInternal(Material material, string propName);
        internal static IEnumerable<string> GetAllScriptGUIDs()
        {
            if (<>f__am$cache2 == null)
            {
                <>f__am$cache2 = asset => IsScriptOrAssembly(asset);
            }
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = asset => AssetDatabase.AssetPathToGUID(asset);
            }
            return AssetDatabase.GetAllAssetPaths().Where<string>(<>f__am$cache2).Select<string, string>(<>f__am$cache3);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetAuthToken();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string[] GetAvailableDiffTools();
        public static Rect GetBoundsOfDesktopAtPoint(Vector2 pos)
        {
            Rect rect;
            INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref pos, out rect);
            return rect;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetClassIDWithoutLoadingObject(int instanceID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetCrashReportFolder();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Resolution GetDesktopResolution();
        public static string GetDisplayStringOfInvalidCharsOfFileName(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return string.Empty;
            }
            string str = new string(Path.GetInvalidFileNameChars());
            string str2 = string.Empty;
            foreach (char ch in filename)
            {
                if ((str.IndexOf(ch) >= 0) && (str2.IndexOf(ch) == -1))
                {
                    if (str2.Length > 0)
                    {
                        str2 = str2 + " ";
                    }
                    str2 = str2 + ch;
                }
            }
            return str2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetEditorAssemblyPath();
        public static string[] GetEditorSettingsList(string prefix, int count)
        {
            ArrayList list = new ArrayList();
            for (int i = 1; i <= count; i++)
            {
                string str = EditorPrefs.GetString(prefix + i, "defaultValue");
                if (str == "defaultValue")
                {
                    break;
                }
                list.Add(str);
            }
            return (list.ToArray(typeof(string)) as string[]);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetEngineAssemblyPath();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetExternalScriptEditor();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetExternalScriptEditorArgs();
        private static bool GetFirstAndLastSelected(List<int> allInstanceIDs, List<int> selectedInstanceIDs, out int firstIndex, out int lastIndex)
        {
            firstIndex = -1;
            lastIndex = -1;
            for (int i = 0; i < allInstanceIDs.Count; i++)
            {
                if (selectedInstanceIDs.Contains(allInstanceIDs[i]))
                {
                    if (firstIndex == -1)
                    {
                        firstIndex = i;
                    }
                    lastIndex = i;
                }
            }
            return ((firstIndex != -1) && (lastIndex != -1));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetFullUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetGameObjectInstanceIDFromComponent(int instanceID);
        public static Texture2D GetIconForFile(string fileName)
        {
            int num = fileName.LastIndexOf('.');
            string key = (num != -1) ? fileName.Substring(num + 1).ToLower() : string.Empty;
            if (key != null)
            {
                int num2;
                if (<>f__switch$map7 == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(0x81);
                    dictionary.Add("boo", 0);
                    dictionary.Add("cginc", 1);
                    dictionary.Add("cs", 2);
                    dictionary.Add("guiskin", 3);
                    dictionary.Add("js", 4);
                    dictionary.Add("mat", 5);
                    dictionary.Add("physicmaterial", 6);
                    dictionary.Add("prefab", 7);
                    dictionary.Add("shader", 8);
                    dictionary.Add("txt", 9);
                    dictionary.Add("unity", 10);
                    dictionary.Add("asset", 11);
                    dictionary.Add("prefs", 11);
                    dictionary.Add("anim", 12);
                    dictionary.Add("meta", 13);
                    dictionary.Add("mixer", 14);
                    dictionary.Add("ttf", 15);
                    dictionary.Add("otf", 15);
                    dictionary.Add("fon", 15);
                    dictionary.Add("fnt", 15);
                    dictionary.Add("aac", 0x10);
                    dictionary.Add("aif", 0x10);
                    dictionary.Add("aiff", 0x10);
                    dictionary.Add("au", 0x10);
                    dictionary.Add("mid", 0x10);
                    dictionary.Add("midi", 0x10);
                    dictionary.Add("mp3", 0x10);
                    dictionary.Add("mpa", 0x10);
                    dictionary.Add("ra", 0x10);
                    dictionary.Add("ram", 0x10);
                    dictionary.Add("wma", 0x10);
                    dictionary.Add("wav", 0x10);
                    dictionary.Add("wave", 0x10);
                    dictionary.Add("ogg", 0x10);
                    dictionary.Add("ai", 0x11);
                    dictionary.Add("apng", 0x11);
                    dictionary.Add("png", 0x11);
                    dictionary.Add("bmp", 0x11);
                    dictionary.Add("cdr", 0x11);
                    dictionary.Add("dib", 0x11);
                    dictionary.Add("eps", 0x11);
                    dictionary.Add("exif", 0x11);
                    dictionary.Add("gif", 0x11);
                    dictionary.Add("ico", 0x11);
                    dictionary.Add("icon", 0x11);
                    dictionary.Add("j", 0x11);
                    dictionary.Add("j2c", 0x11);
                    dictionary.Add("j2k", 0x11);
                    dictionary.Add("jas", 0x11);
                    dictionary.Add("jiff", 0x11);
                    dictionary.Add("jng", 0x11);
                    dictionary.Add("jp2", 0x11);
                    dictionary.Add("jpc", 0x11);
                    dictionary.Add("jpe", 0x11);
                    dictionary.Add("jpeg", 0x11);
                    dictionary.Add("jpf", 0x11);
                    dictionary.Add("jpg", 0x11);
                    dictionary.Add("jpw", 0x11);
                    dictionary.Add("jpx", 0x11);
                    dictionary.Add("jtf", 0x11);
                    dictionary.Add("mac", 0x11);
                    dictionary.Add("omf", 0x11);
                    dictionary.Add("qif", 0x11);
                    dictionary.Add("qti", 0x11);
                    dictionary.Add("qtif", 0x11);
                    dictionary.Add("tex", 0x11);
                    dictionary.Add("tfw", 0x11);
                    dictionary.Add("tga", 0x11);
                    dictionary.Add("tif", 0x11);
                    dictionary.Add("tiff", 0x11);
                    dictionary.Add("wmf", 0x11);
                    dictionary.Add("psd", 0x11);
                    dictionary.Add("exr", 0x11);
                    dictionary.Add("hdr", 0x11);
                    dictionary.Add("3df", 0x12);
                    dictionary.Add("3dm", 0x12);
                    dictionary.Add("3dmf", 0x12);
                    dictionary.Add("3ds", 0x12);
                    dictionary.Add("3dv", 0x12);
                    dictionary.Add("3dx", 0x12);
                    dictionary.Add("blend", 0x12);
                    dictionary.Add("c4d", 0x12);
                    dictionary.Add("lwo", 0x12);
                    dictionary.Add("lws", 0x12);
                    dictionary.Add("ma", 0x12);
                    dictionary.Add("max", 0x12);
                    dictionary.Add("mb", 0x12);
                    dictionary.Add("mesh", 0x12);
                    dictionary.Add("obj", 0x12);
                    dictionary.Add("vrl", 0x12);
                    dictionary.Add("wrl", 0x12);
                    dictionary.Add("wrz", 0x12);
                    dictionary.Add("fbx", 0x12);
                    dictionary.Add("asf", 0x13);
                    dictionary.Add("asx", 0x13);
                    dictionary.Add("avi", 0x13);
                    dictionary.Add("dat", 0x13);
                    dictionary.Add("divx", 0x13);
                    dictionary.Add("dvx", 0x13);
                    dictionary.Add("mlv", 0x13);
                    dictionary.Add("m2l", 0x13);
                    dictionary.Add("m2t", 0x13);
                    dictionary.Add("m2ts", 0x13);
                    dictionary.Add("m2v", 0x13);
                    dictionary.Add("m4e", 0x13);
                    dictionary.Add("m4v", 0x13);
                    dictionary.Add("mjp", 0x13);
                    dictionary.Add("mov", 0x13);
                    dictionary.Add("movie", 0x13);
                    dictionary.Add("mp21", 0x13);
                    dictionary.Add("mp4", 0x13);
                    dictionary.Add("mpe", 0x13);
                    dictionary.Add("mpeg", 0x13);
                    dictionary.Add("mpg", 0x13);
                    dictionary.Add("mpv2", 0x13);
                    dictionary.Add("ogm", 0x13);
                    dictionary.Add("qt", 0x13);
                    dictionary.Add("rm", 0x13);
                    dictionary.Add("rmvb", 0x13);
                    dictionary.Add("wmw", 0x13);
                    dictionary.Add("xvid", 0x13);
                    dictionary.Add("colors", 20);
                    dictionary.Add("gradients", 20);
                    dictionary.Add("curves", 20);
                    dictionary.Add("curvesnormalized", 20);
                    dictionary.Add("particlecurves", 20);
                    dictionary.Add("particlecurvessigned", 20);
                    dictionary.Add("particledoublecurves", 20);
                    dictionary.Add("particledoublecurvessigned", 20);
                    <>f__switch$map7 = dictionary;
                }
                if (<>f__switch$map7.TryGetValue(key, out num2))
                {
                    switch (num2)
                    {
                        case 0:
                            return EditorGUIUtility.FindTexture("boo Script Icon");

                        case 1:
                            return EditorGUIUtility.FindTexture("CGProgram Icon");

                        case 2:
                            return EditorGUIUtility.FindTexture("cs Script Icon");

                        case 3:
                            return EditorGUIUtility.FindTexture("GUISkin Icon");

                        case 4:
                            return EditorGUIUtility.FindTexture("Js Script Icon");

                        case 5:
                            return EditorGUIUtility.FindTexture("Material Icon");

                        case 6:
                            return EditorGUIUtility.FindTexture("PhysicMaterial Icon");

                        case 7:
                            return EditorGUIUtility.FindTexture("PrefabNormal Icon");

                        case 8:
                            return EditorGUIUtility.FindTexture("Shader Icon");

                        case 9:
                            return EditorGUIUtility.FindTexture("TextAsset Icon");

                        case 10:
                            return EditorGUIUtility.FindTexture("SceneAsset Icon");

                        case 11:
                            return EditorGUIUtility.FindTexture("GameManager Icon");

                        case 12:
                            return EditorGUIUtility.FindTexture("Animation Icon");

                        case 13:
                            return EditorGUIUtility.FindTexture("MetaFile Icon");

                        case 14:
                            return EditorGUIUtility.FindTexture("AudioMixerController Icon");

                        case 15:
                            return EditorGUIUtility.FindTexture("Font Icon");

                        case 0x10:
                            return EditorGUIUtility.FindTexture("AudioClip Icon");

                        case 0x11:
                            return EditorGUIUtility.FindTexture("Texture Icon");

                        case 0x12:
                            return EditorGUIUtility.FindTexture("Mesh Icon");

                        case 0x13:
                            return EditorGUIUtility.FindTexture("MovieTexture Icon");

                        case 20:
                            return EditorGUIUtility.FindTexture("ScriptableObject Icon");
                    }
                }
            }
            return EditorGUIUtility.FindTexture("DefaultAsset Icon");
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool GetIsInspectorExpanded(Object obj);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetLayerName(int layer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int[] GetLicenseFlags();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetLicenseInfo();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetLoadedObjectFromInstanceID(int instanceID);
        private static Bounds GetLocalBounds(GameObject gameObject)
        {
            RectTransform component = gameObject.GetComponent<RectTransform>();
            if (component != null)
            {
                return new Bounds((Vector3) component.rect.center, (Vector3) component.rect.size);
            }
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer is MeshRenderer)
            {
                MeshFilter filter = renderer.GetComponent<MeshFilter>();
                if ((filter != null) && (filter.sharedMesh != null))
                {
                    return filter.sharedMesh.bounds;
                }
            }
            if (renderer is SpriteRenderer)
            {
                SpriteRenderer renderer2 = renderer as SpriteRenderer;
                if (renderer2.sprite != null)
                {
                    Bounds bounds = renderer2.sprite.bounds;
                    Vector3 size = bounds.size;
                    size.z = 0f;
                    bounds.size = size;
                    return bounds;
                }
            }
            return new Bounds(Vector3.zero, Vector3.zero);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern MonoIsland[] GetMonoIslands();
        public static List<int> GetNewSelection(int clickedInstanceID, List<int> allInstanceIDs, List<int> selectedInstanceIDs, int lastClickedInstanceID, bool keepMultiSelection, bool useShiftAsActionKey, bool allowMultiSelection)
        {
            List<int> list = new List<int>();
            bool flag = Event.current.shift || (EditorGUI.actionKey && useShiftAsActionKey);
            bool flag2 = EditorGUI.actionKey && !useShiftAsActionKey;
            if (!allowMultiSelection)
            {
                flag = flag2 = false;
            }
            if (flag2)
            {
                list.AddRange(selectedInstanceIDs);
                if (list.Contains(clickedInstanceID))
                {
                    list.Remove(clickedInstanceID);
                    return list;
                }
                list.Add(clickedInstanceID);
                return list;
            }
            if (flag)
            {
                int num;
                int num2;
                int num7;
                int num8;
                if (clickedInstanceID == lastClickedInstanceID)
                {
                    return selectedInstanceIDs;
                }
                if (!GetFirstAndLastSelected(allInstanceIDs, selectedInstanceIDs, out num, out num2))
                {
                    list.Add(clickedInstanceID);
                    return list;
                }
                int num3 = -1;
                int num4 = -1;
                for (int i = 0; i < allInstanceIDs.Count; i++)
                {
                    if (allInstanceIDs[i] == clickedInstanceID)
                    {
                        num3 = i;
                    }
                    if ((lastClickedInstanceID != 0) && (allInstanceIDs[i] == lastClickedInstanceID))
                    {
                        num4 = i;
                    }
                }
                int num6 = 0;
                if (num4 != -1)
                {
                    num6 = (num3 <= num4) ? -1 : 1;
                }
                if (num3 > num2)
                {
                    num7 = num;
                    num8 = num3;
                }
                else if ((num3 >= num) && (num3 < num2))
                {
                    if (num6 > 0)
                    {
                        num7 = num3;
                        num8 = num2;
                    }
                    else
                    {
                        num7 = num;
                        num8 = num3;
                    }
                }
                else
                {
                    num7 = num3;
                    num8 = num2;
                }
                for (int j = num7; j <= num8; j++)
                {
                    list.Add(allInstanceIDs[j]);
                }
                return list;
            }
            if (keepMultiSelection && selectedInstanceIDs.Contains(clickedInstanceID))
            {
                list.AddRange(selectedInstanceIDs);
                return list;
            }
            list.Add(clickedInstanceID);
            return list;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetNoDiffToolsDetectedMessage();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object GetObjectFromInstanceID(int instanceID);
        public static Camera[] GetSceneViewCameras()
        {
            return SceneView.GetAllSceneCameras();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetSortingLayerCount();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool GetSortingLayerLocked(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetSortingLayerName(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern string GetSortingLayerNameFromUniqueID(int id);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern int GetSortingLayerUniqueID(int index);
        public static Vector4 GetSpriteOuterUV(Sprite sprite, bool getAtlasData)
        {
            Vector4 vector;
            INTERNAL_CALL_GetSpriteOuterUV(sprite, getAtlasData, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetUnityBuildBranch();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetUnityCopyright();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetUnityRevision();
        public static Version GetUnityVersion()
        {
            Version version = new Version(GetUnityVersionDigits());
            return new Version(version.Major, version.Minor, version.Build, GetUnityRevision());
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetUnityVersionDate();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern string GetUnityVersionDigits();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasAdvancedLicenseOnBuildTarget(BuildTarget target);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasEduLicense();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasFreeLicense();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasFullscreenCamera();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasPro();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool HasTeamLicense();
        internal static bool HDRTextureNeedsFixing(MaterialProperty prop, out bool canBeFixedAutomatically)
        {
            if (((prop.flags & MaterialProperty.PropFlags.HDR) != MaterialProperty.PropFlags.None) || prop.displayName.Contains("(HDR"))
            {
                Texture textureValue = prop.textureValue;
                if (textureValue != null)
                {
                    TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(textureValue)) as TextureImporter;
                    canBeFixedAutomatically = atPath != null;
                    bool flag2 = TextureUtil.HasAlphaTextureFormat(TextureUtil.GetTextureFormat(textureValue));
                    bool flag3 = TextureUtil.GetUsageMode(textureValue) == TextureUsageMode.RGBMEncoded;
                    if (flag2 && !flag3)
                    {
                        return true;
                    }
                }
            }
            canBeFixedAutomatically = false;
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern DragAndDropVisualMode HierarchyWindowDrag(HierarchyProperty property, bool perform, HierarchyDropMode dropMode);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern DragAndDropVisualMode InspectorWindowDrag(Object[] targets, bool perform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Material[] InstantiateMaterialsInEditMode(Renderer renderer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_CalculateSelectionBounds(bool usePivotOnlyForParticles, bool onlyUseActiveSelection, out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetBoundsOfDesktopAtPoint(ref Vector2 pos, out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetSpriteOuterUV(Sprite sprite, bool getAtlasData, out Vector4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_PassAndReturnColor32(ref Color32 c, out Color32 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_PassAndReturnVector2(ref Vector2 v, out Vector2 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Color[] INTERNAL_CALL_ReadScreenPixel(ref Vector2 pixelPos, int sizex, int sizey);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern DragAndDropVisualMode INTERNAL_CALL_SceneViewDrag(Object dropUpon, ref Vector3 worldPosition, ref Vector2 viewportPosition, bool perform);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetCustomLighting(Light[] lights, ref Color ambient);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetRectTransformTemporaryRect(RectTransform rectTransform, ref Rect rect);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_TransformBounds(ref Bounds b, Transform t, out Bounds value);
        internal static bool IsScriptOrAssembly(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                string key = Path.GetExtension(filename).ToLower();
                if (key != null)
                {
                    int num;
                    if (<>f__switch$map8 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                        dictionary.Add(".cs", 0);
                        dictionary.Add(".js", 0);
                        dictionary.Add(".boo", 0);
                        dictionary.Add(".dll", 1);
                        dictionary.Add(".exe", 1);
                        <>f__switch$map8 = dictionary;
                    }
                    if (<>f__switch$map8.TryGetValue(key, out num))
                    {
                        if (num == 0)
                        {
                            return true;
                        }
                        if (num == 1)
                        {
                            return AssemblyHelper.IsManagedAssembly(filename);
                        }
                    }
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern bool IsSortingLayerDefault(int index);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool IsUnityBeta();
        public static bool IsValidFileName(string filename)
        {
            string str = RemoveInvalidCharsFromFileName(filename, false);
            return (!(str != filename) && !string.IsNullOrEmpty(str));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Assembly LoadAssemblyWrapper(string dllName, string dllLocation);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadDefaultLayout();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern Object[] LoadSerializedFileAndForget(string path);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OnGameViewFocus(bool focus);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OpenEditorConsole();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool OpenFileAtLineExternal(string filename, int line);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void OpenPlayerConsole();
        internal static T ParentHasComponent<T>(Transform trans) where T: Component
        {
            if (trans == null)
            {
                return null;
            }
            T component = trans.GetComponent<T>();
            if (component != null)
            {
                return component;
            }
            return ParentHasComponent<T>(trans.parent);
        }

        public static Color32 PassAndReturnColor32(Color32 c)
        {
            Color32 color;
            INTERNAL_CALL_PassAndReturnColor32(ref c, out color);
            return color;
        }

        public static Vector2 PassAndReturnVector2(Vector2 v)
        {
            Vector2 vector;
            INTERNAL_CALL_PassAndReturnVector2(ref v, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern DragAndDropVisualMode ProjectWindowDrag(HierarchyProperty property, bool perform);
        public static Color[] ReadScreenPixel(Vector2 pixelPos, int sizex, int sizey)
        {
            return INTERNAL_CALL_ReadScreenPixel(ref pixelPos, sizex, sizey);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void RegisterExtensionDll(string dllLocation, string guid);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ReloadWindowLayoutMenu();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RemoveCustomLighting();
        public static string RemoveInvalidCharsFromFileName(string filename, bool logIfInvalidChars)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }
            filename = filename.Trim();
            if (string.IsNullOrEmpty(filename))
            {
                return filename;
            }
            string str = new string(Path.GetInvalidFileNameChars());
            string str2 = string.Empty;
            bool flag = false;
            foreach (char ch in filename)
            {
                if (str.IndexOf(ch) == -1)
                {
                    str2 = str2 + ch;
                }
                else
                {
                    flag = true;
                }
            }
            if (flag && logIfInvalidChars)
            {
                string displayStringOfInvalidCharsOfFileName = GetDisplayStringOfInvalidCharsOfFileName(filename);
                if (displayStringOfInvalidCharsOfFileName.Length > 0)
                {
                    object[] args = new object[] { (displayStringOfInvalidCharsOfFileName.Length <= 1) ? string.Empty : "s", displayStringOfInvalidCharsOfFileName };
                    Debug.LogWarningFormat("A filename cannot contain the following character{0}:  {1}", args);
                }
            }
            return str2;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RepaintAllViews();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RequestScriptReload();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ResetCursor();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RevertFactoryLayoutSettings(bool quitOnCancel);
        [ExcludeFromDocs]
        internal static bool RunningUnderWindows8()
        {
            bool orHigher = true;
            return RunningUnderWindows8(orHigher);
        }

        internal static bool RunningUnderWindows8([DefaultValue("true")] bool orHigher)
        {
            if (Application.platform != RuntimePlatform.WindowsEditor)
            {
                return false;
            }
            OperatingSystem oSVersion = Environment.OSVersion;
            int major = oSVersion.Version.Major;
            int minor = oSVersion.Version.Minor;
            if (orHigher)
            {
                return ((major > 6) || ((major == 6) && (minor >= 2)));
            }
            return ((major == 6) && (minor == 2));
        }

        public static void SaveEditorSettingsList(string prefix, string[] aList, int count)
        {
            int num;
            for (num = 0; num < aList.Length; num++)
            {
                EditorPrefs.SetString(prefix + (num + 1), aList[num]);
            }
            for (num = aList.Length + 1; num <= count; num++)
            {
                EditorPrefs.DeleteKey(prefix + num);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SaveToSerializedFileAndForget(Object[] obj, string path, bool allowTextSerialization);
        public static DragAndDropVisualMode SceneViewDrag(Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, bool perform)
        {
            return INTERNAL_CALL_SceneViewDrag(dropUpon, ref worldPosition, ref viewportPosition, perform);
        }

        public static void SetCustomLighting(Light[] lights, Color ambient)
        {
            INTERNAL_CALL_SetCustomLighting(lights, ref ambient);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetIsInspectorExpanded(Object obj, bool isExpanded);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetPlatformPath(string path);
        public static void SetRectTransformTemporaryRect(RectTransform rectTransform, Rect rect)
        {
            INTERNAL_CALL_SetRectTransformTemporaryRect(rectTransform, ref rect);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetSortingLayerLocked(int index, bool locked);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetSortingLayerName(int index, string name);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetupCustomDll(string dllName, string dllLocation);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetupShaderMenu(Material material);
        public static void ShowGameView()
        {
            WindowLayout.ShowAppropriateViewOnEnterExitPlaymode(true);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ShowPackageManagerWindow();
        internal static bool SupportsRectLayout(Transform tr)
        {
            if ((tr == null) || (tr.parent == null))
            {
                return false;
            }
            return ((tr.GetComponent<RectTransform>() != null) && (tr.parent.GetComponent<RectTransform>() != null));
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SwitchSkinAndRepaintAllViews();
        public static string TextAreaForDocBrowser(Rect position, string text, GUIStyle style)
        {
            bool flag;
            int id = GUIUtility.GetControlID("TextAreaWithTabHandling".GetHashCode(), FocusType.Keyboard, position);
            EditorGUI.RecycledTextEditor editor = EditorGUI.s_RecycledEditor;
            Event current = Event.current;
            if (editor.IsEditingControl(id) && (current.type == EventType.KeyDown))
            {
                if (current.character == '\t')
                {
                    editor.Insert('\t');
                    current.Use();
                    GUI.changed = true;
                    text = editor.text;
                }
                if (current.character == '\n')
                {
                    editor.Insert('\n');
                    current.Use();
                    GUI.changed = true;
                    text = editor.text;
                }
            }
            text = EditorGUI.DoTextField(editor, id, EditorGUI.IndentedRect(position), text, style, null, out flag, false, true, false);
            return text;
        }

        public static string TextifyEvent(Event evt)
        {
            if (evt == null)
            {
                return "none";
            }
            string str = null;
            KeyCode keyCode = evt.keyCode;
            switch (keyCode)
            {
                case KeyCode.Keypad0:
                    str = "[0]";
                    break;

                case KeyCode.Keypad1:
                    str = "[1]";
                    break;

                case KeyCode.Keypad2:
                    str = "[2]";
                    break;

                case KeyCode.Keypad3:
                    str = "[3]";
                    break;

                case KeyCode.Keypad4:
                    str = "[4]";
                    break;

                case KeyCode.Keypad5:
                    str = "[5]";
                    break;

                case KeyCode.Keypad6:
                    str = "[6]";
                    break;

                case KeyCode.Keypad7:
                    str = "[7]";
                    break;

                case KeyCode.Keypad8:
                    str = "[8]";
                    break;

                case KeyCode.Keypad9:
                    str = "[9]";
                    break;

                case KeyCode.KeypadPeriod:
                    str = "[.]";
                    break;

                case KeyCode.KeypadDivide:
                    str = "[/]";
                    break;

                case KeyCode.KeypadMinus:
                    str = "[-]";
                    break;

                case KeyCode.KeypadPlus:
                    str = "[+]";
                    break;

                case KeyCode.KeypadEnter:
                    str = "enter";
                    break;

                case KeyCode.KeypadEquals:
                    str = "[=]";
                    break;

                case KeyCode.UpArrow:
                    str = "up";
                    break;

                case KeyCode.DownArrow:
                    str = "down";
                    break;

                case KeyCode.RightArrow:
                    str = "right";
                    break;

                case KeyCode.LeftArrow:
                    str = "left";
                    break;

                case KeyCode.Insert:
                    str = "insert";
                    break;

                case KeyCode.Home:
                    str = "home";
                    break;

                case KeyCode.End:
                    str = "end";
                    break;

                case KeyCode.PageUp:
                    str = "page up";
                    break;

                case KeyCode.PageDown:
                    str = "page down";
                    break;

                case KeyCode.F1:
                    str = "F1";
                    break;

                case KeyCode.F2:
                    str = "F2";
                    break;

                case KeyCode.F3:
                    str = "F3";
                    break;

                case KeyCode.F4:
                    str = "F4";
                    break;

                case KeyCode.F5:
                    str = "F5";
                    break;

                case KeyCode.F6:
                    str = "F6";
                    break;

                case KeyCode.F7:
                    str = "F7";
                    break;

                case KeyCode.F8:
                    str = "F8";
                    break;

                case KeyCode.F9:
                    str = "F9";
                    break;

                case KeyCode.F10:
                    str = "F10";
                    break;

                case KeyCode.F11:
                    str = "F11";
                    break;

                case KeyCode.F12:
                    str = "F12";
                    break;

                case KeyCode.F13:
                    str = "F13";
                    break;

                case KeyCode.F14:
                    str = "F14";
                    break;

                case KeyCode.F15:
                    str = "F15";
                    break;

                default:
                    if (keyCode == KeyCode.Backspace)
                    {
                        str = "backspace";
                    }
                    else if (keyCode == KeyCode.Return)
                    {
                        str = "return";
                    }
                    else if (keyCode == KeyCode.Escape)
                    {
                        str = "[esc]";
                    }
                    else if (keyCode == KeyCode.Delete)
                    {
                        str = "delete";
                    }
                    else
                    {
                        str = string.Empty + evt.keyCode;
                    }
                    break;
            }
            string str2 = string.Empty;
            if (evt.alt)
            {
                str2 = str2 + "Alt+";
            }
            if (evt.command)
            {
                str2 = str2 + ((Application.platform != RuntimePlatform.OSXEditor) ? "Ctrl+" : "Cmd+");
            }
            if (evt.control)
            {
                str2 = str2 + "Ctrl+";
            }
            if (evt.shift)
            {
                str2 = str2 + "Shift+";
            }
            return (str2 + str);
        }

        public static Bounds TransformBounds(Bounds b, Transform t)
        {
            Bounds bounds;
            INTERNAL_CALL_TransformBounds(ref b, t, out bounds);
            return bounds;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void UpdateSortingLayersOrder();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern ulong VerifyCacheServerIntegrity();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool WiiUSaveStartupScreenToFile(Texture2D image, string path, int outputWidth, int outputHeight);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Xbox360GenerateSPAConfig(string spaPath);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern bool Xbox360SaveSplashScreenToFile(Texture2D image, string spaPath);

        public static float defaultScreenHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float defaultScreenWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float defaultWebScreenHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float defaultWebScreenWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int[] expandedProjectWindowItems { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool ignoreInspectorChanges { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool inBatchMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isApplicationActive { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static bool isHumanControllingUs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string[] layers { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float remoteScreenHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static float remoteScreenWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static string[] sortingLayerNames { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static int[] sortingLayerUniqueIDs { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string[] tags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static string unityPreferencesFolder { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public enum HierarchyDropMode
        {
            kHierarchyDragNormal = 0,
            kHierarchyDropAfterParent = 4,
            kHierarchyDropBetween = 2,
            kHierarchyDropUpon = 1
        }
    }
}

