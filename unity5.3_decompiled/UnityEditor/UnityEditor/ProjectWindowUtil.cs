namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEditor.ProjectWindowCallback;
    using UnityEditorInternal;
    using UnityEngine;

    public class ProjectWindowUtil
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map1F;
        internal static string k_DraggingFavoriteGenericData = "DraggingFavorite";
        internal static int k_FavoritesStartInstanceID = 0x3b9aca00;
        internal static string k_IsFolderGenericData = "IsFolder";

        private static void CreateAnimatorController()
        {
            Texture2D image = EditorGUIUtility.IconContent("AnimatorController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAnimatorController>(), "New Animator Controller.controller", image, null);
        }

        public static void CreateAsset(Object asset, string pathName)
        {
            StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), ScriptableObject.CreateInstance<DoCreateNewAsset>(), pathName, AssetPreview.GetMiniThumbnail(asset), null);
        }

        private static void CreateAudioMixer()
        {
            Texture2D image = EditorGUIUtility.IconContent("AudioMixerController Icon").image as Texture2D;
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateAudioMixer>(), "NewAudioMixer.mixer", image, null);
        }

        public static void CreateFolder()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateFolder>(), "New Folder", EditorGUIUtility.IconContent(EditorResourcesUtility.emptyFolderIconName).image as Texture2D, null);
        }

        [MenuItem("Assets/Create/GUI Skin", false, 0x259)]
        public static void CreateNewGUISkin()
        {
            GUISkin dest = ScriptableObject.CreateInstance<GUISkin>();
            GUISkin builtinResource = Resources.GetBuiltinResource(typeof(GUISkin), "GameSkin/GameSkin.guiskin") as GUISkin;
            if (builtinResource != null)
            {
                EditorUtility.CopySerialized(builtinResource, dest);
            }
            else
            {
                Debug.LogError("Internal error: unable to load builtin GUIskin");
            }
            CreateAsset(dest, "New GUISkin.guiskin");
        }

        public static void CreatePrefab()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreatePrefab>(), "New Prefab.prefab", EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D, null);
        }

        public static void CreateScene()
        {
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScene>(), "New Scene.unity", EditorGUIUtility.FindTexture("SceneAsset Icon"), null);
        }

        private static void CreateScriptAsset(string templatePath, string destName)
        {
            if (templatePath.ToLower().Contains("test"))
            {
                string uniquePathNameAtSelectedPath = AssetDatabase.GetUniquePathNameAtSelectedPath(destName);
                if (!uniquePathNameAtSelectedPath.ToLower().Contains("/editor/"))
                {
                    uniquePathNameAtSelectedPath = uniquePathNameAtSelectedPath.Substring(0, (uniquePathNameAtSelectedPath.Length - destName.Length) - 1);
                    string path = Path.Combine(uniquePathNameAtSelectedPath, "Editor");
                    if (!Directory.Exists(path))
                    {
                        AssetDatabase.CreateFolder(uniquePathNameAtSelectedPath, "Editor");
                    }
                    uniquePathNameAtSelectedPath = Path.Combine(path, destName).Replace(@"\", "/");
                }
                destName = uniquePathNameAtSelectedPath;
            }
            Texture2D icon = null;
            string extension = Path.GetExtension(destName);
            if (extension != null)
            {
                int num;
                if (<>f__switch$map1F == null)
                {
                    Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                    dictionary.Add(".js", 0);
                    dictionary.Add(".cs", 1);
                    dictionary.Add(".boo", 2);
                    dictionary.Add(".shader", 3);
                    <>f__switch$map1F = dictionary;
                }
                if (<>f__switch$map1F.TryGetValue(extension, out num))
                {
                    switch (num)
                    {
                        case 0:
                            icon = EditorGUIUtility.IconContent("js Script Icon").image as Texture2D;
                            goto Label_0193;

                        case 1:
                            icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
                            goto Label_0193;

                        case 2:
                            icon = EditorGUIUtility.IconContent("boo Script Icon").image as Texture2D;
                            goto Label_0193;

                        case 3:
                            icon = EditorGUIUtility.IconContent("Shader Icon").image as Texture2D;
                            goto Label_0193;
                    }
                }
            }
            icon = EditorGUIUtility.IconContent("TextAsset Icon").image as Texture2D;
        Label_0193:
            StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<DoCreateScriptAsset>(), destName, icon, templatePath);
        }

        internal static Object CreateScriptAssetFromTemplate(string pathName, string resourceFile)
        {
            string fullPath = Path.GetFullPath(pathName);
            StreamReader reader = new StreamReader(resourceFile);
            string input = reader.ReadToEnd();
            reader.Close();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
            input = Regex.Replace(input, "#NAME#", fileNameWithoutExtension);
            string replacement = Regex.Replace(fileNameWithoutExtension, " ", string.Empty);
            input = Regex.Replace(input, "#SCRIPTNAME#", replacement);
            if (char.IsUpper(replacement, 0))
            {
                replacement = char.ToLower(replacement[0]) + replacement.Substring(1);
                input = Regex.Replace(input, "#SCRIPTNAME_LOWER#", replacement);
            }
            else
            {
                replacement = "my" + char.ToUpper(replacement[0]) + replacement.Substring(1);
                input = Regex.Replace(input, "#SCRIPTNAME_LOWER#", replacement);
            }
            bool flag = true;
            bool throwOnInvalidBytes = false;
            UTF8Encoding encoding = new UTF8Encoding(flag, throwOnInvalidBytes);
            bool append = false;
            StreamWriter writer = new StreamWriter(fullPath, append, encoding);
            writer.Write(input);
            writer.Close();
            AssetDatabase.ImportAsset(pathName);
            return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
        }

        private static void CreateSpritePolygon(int sides)
        {
            string str = string.Empty;
            int num = sides;
            switch (num)
            {
                case 0:
                    str = "Square";
                    break;

                case 3:
                    str = "Triangle";
                    break;

                case 4:
                    str = "Diamond";
                    break;

                case 6:
                    str = "Hexagon";
                    break;

                default:
                    if (num == 0x2a)
                    {
                        str = "Everythingon";
                    }
                    else if (num == 0x80)
                    {
                        str = "Circle";
                    }
                    else
                    {
                        str = "Polygon";
                    }
                    break;
            }
            Texture2D image = EditorGUIUtility.IconContent("Sprite Icon").image as Texture2D;
            DoCreateSpritePolygon endAction = ScriptableObject.CreateInstance<DoCreateSpritePolygon>();
            endAction.sides = sides;
            StartNameEditingIfProjectWindowExists(0, endAction, str + ".png", image, null);
        }

        internal static void DuplicateSelectedAssets()
        {
            AssetDatabase.Refresh();
            Object[] objects = Selection.objects;
            bool flag = true;
            foreach (Object obj2 in objects)
            {
                AnimationClip clip = obj2 as AnimationClip;
                if (((clip == null) || ((clip.hideFlags & HideFlags.NotEditable) == HideFlags.None)) || !AssetDatabase.Contains(clip))
                {
                    flag = false;
                }
            }
            ArrayList list = new ArrayList();
            bool flag2 = false;
            if (flag)
            {
                foreach (Object obj3 in objects)
                {
                    AnimationClip source = obj3 as AnimationClip;
                    if ((source != null) && ((source.hideFlags & HideFlags.NotEditable) != HideFlags.None))
                    {
                        string path = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(AssetDatabase.GetAssetPath(obj3)), source.name) + ".anim");
                        AnimationClip dest = new AnimationClip();
                        EditorUtility.CopySerialized(source, dest);
                        AssetDatabase.CreateAsset(dest, path);
                        list.Add(path);
                    }
                }
            }
            else
            {
                foreach (Object obj4 in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
                {
                    string assetPath = AssetDatabase.GetAssetPath(obj4);
                    string newPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                    if (newPath.Length != 0)
                    {
                        flag2 |= !AssetDatabase.CopyAsset(assetPath, newPath);
                    }
                    else
                    {
                        flag2 |= true;
                    }
                    if (!flag2)
                    {
                        list.Add(newPath);
                    }
                }
            }
            AssetDatabase.Refresh();
            Object[] objArray6 = new Object[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                objArray6[i] = AssetDatabase.LoadMainAssetAtPath(list[i] as string);
            }
            Selection.objects = objArray6;
        }

        internal static void EndNameEditAction(UnityEditor.ProjectWindowCallback.EndNameEditAction action, int instanceId, string pathName, string resourceFile)
        {
            pathName = AssetDatabase.GenerateUniqueAssetPath(pathName);
            if (action != null)
            {
                action.Action(instanceId, pathName, resourceFile);
                action.CleanUp();
            }
        }

        internal static void FrameObjectInProjectWindow(int instanceID)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.FrameObject(instanceID, false);
            }
        }

        internal static string GetActiveFolderPath()
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists == null)
            {
                return "Assets";
            }
            return projectBrowserIfExists.GetActiveFolderPath();
        }

        public static int[] GetAncestors(int instanceID)
        {
            int num2;
            List<int> list = new List<int>();
            int mainAssetInstanceID = AssetDatabase.GetMainAssetInstanceID(AssetDatabase.GetAssetPath(instanceID));
            if (mainAssetInstanceID != instanceID)
            {
                list.Add(mainAssetInstanceID);
            }
            for (string str = GetContainingFolder(AssetDatabase.GetAssetPath(mainAssetInstanceID)); !string.IsNullOrEmpty(str); str = GetContainingFolder(AssetDatabase.GetAssetPath(num2)))
            {
                num2 = AssetDatabase.GetMainAssetInstanceID(str);
                list.Add(num2);
            }
            return list.ToArray();
        }

        public static string[] GetBaseFolders(string[] folders)
        {
            if (folders.Length <= 1)
            {
                return folders;
            }
            List<string> list = new List<string>();
            List<string> list2 = new List<string>(folders);
            for (int i = 0; i < list2.Count; i++)
            {
                char[] trimChars = new char[] { '/' };
                list2[i] = list2[i].Trim(trimChars);
            }
            list2.Sort();
            for (int j = 0; j < list2.Count; j++)
            {
                if (!list2[j].EndsWith("/"))
                {
                    list2[j] = list2[j] + "/";
                }
            }
            string item = list2[0];
            list.Add(item);
            for (int k = 1; k < list2.Count; k++)
            {
                if (list2[k].IndexOf(item, StringComparison.Ordinal) != 0)
                {
                    list.Add(list2[k]);
                    item = list2[k];
                }
            }
            for (int m = 0; m < list.Count; m++)
            {
                char[] chArray2 = new char[] { '/' };
                list[m] = list[m].Trim(chArray2);
            }
            return list.ToArray();
        }

        public static string GetContainingFolder(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                char[] trimChars = new char[] { '/' };
                path = path.Trim(trimChars);
                int length = path.LastIndexOf("/", StringComparison.Ordinal);
                if (length != -1)
                {
                    return path.Substring(0, length);
                }
            }
            return null;
        }

        internal static Object[] GetDragAndDropObjects(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            List<Object> list = new List<Object>(selectedInstanceIDs.Count);
            if (selectedInstanceIDs.Contains(draggedInstanceID))
            {
                for (int i = 0; i < selectedInstanceIDs.Count; i++)
                {
                    Object objectFromInstanceID = InternalEditorUtility.GetObjectFromInstanceID(selectedInstanceIDs[i]);
                    if (objectFromInstanceID != null)
                    {
                        list.Add(objectFromInstanceID);
                    }
                }
            }
            else
            {
                Object item = InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        internal static string[] GetDragAndDropPaths(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            List<string> list = new List<string>();
            foreach (int num in selectedInstanceIDs)
            {
                if (AssetDatabase.IsMainAsset(num))
                {
                    string item = AssetDatabase.GetAssetPath(num);
                    list.Add(item);
                }
            }
            string assetPath = AssetDatabase.GetAssetPath(draggedInstanceID);
            if (string.IsNullOrEmpty(assetPath))
            {
                return new string[0];
            }
            if (list.Contains(assetPath))
            {
                return list.ToArray();
            }
            return new string[] { assetPath };
        }

        private static ProjectBrowser GetProjectBrowserIfExists()
        {
            return ProjectBrowser.s_LastInteractedProjectBrowser;
        }

        internal static bool IsFavoritesItem(int instanceID)
        {
            return (instanceID >= k_FavoritesStartInstanceID);
        }

        public static bool IsFolder(int instanceID)
        {
            return AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(instanceID));
        }

        public static void ShowCreatedAsset(Object o)
        {
            Selection.activeObject = o;
            if (o != null)
            {
                FrameObjectInProjectWindow(o.GetInstanceID());
            }
        }

        internal static void StartDrag(int draggedInstanceID, List<int> selectedInstanceIDs)
        {
            DragAndDrop.PrepareStartDrag();
            string title = string.Empty;
            if (IsFavoritesItem(draggedInstanceID))
            {
                DragAndDrop.SetGenericData(k_DraggingFavoriteGenericData, draggedInstanceID);
                DragAndDrop.objectReferences = new Object[0];
            }
            else
            {
                bool flag = IsFolder(draggedInstanceID);
                DragAndDrop.objectReferences = GetDragAndDropObjects(draggedInstanceID, selectedInstanceIDs);
                DragAndDrop.SetGenericData(k_IsFolderGenericData, !flag ? string.Empty : "isFolder");
                string[] dragAndDropPaths = GetDragAndDropPaths(draggedInstanceID, selectedInstanceIDs);
                if (dragAndDropPaths.Length > 0)
                {
                    DragAndDrop.paths = dragAndDropPaths;
                }
                if (DragAndDrop.objectReferences.Length > 1)
                {
                    title = "<Multiple>";
                }
                else
                {
                    title = ObjectNames.GetDragAndDropTitle(InternalEditorUtility.GetObjectFromInstanceID(draggedInstanceID));
                }
            }
            DragAndDrop.StartDrag(title);
        }

        public static void StartNameEditingIfProjectWindowExists(int instanceID, UnityEditor.ProjectWindowCallback.EndNameEditAction endAction, string pathName, Texture2D icon, string resourceFile)
        {
            ProjectBrowser projectBrowserIfExists = GetProjectBrowserIfExists();
            if (projectBrowserIfExists != null)
            {
                projectBrowserIfExists.Focus();
                projectBrowserIfExists.BeginPreimportedNameEditing(instanceID, endAction, pathName, icon, resourceFile);
                projectBrowserIfExists.Repaint();
            }
            else
            {
                if (!pathName.StartsWith("assets/", StringComparison.CurrentCultureIgnoreCase))
                {
                    pathName = "Assets/" + pathName;
                }
                EndNameEditAction(endAction, instanceID, pathName, resourceFile);
                Selection.activeObject = EditorUtility.InstanceIDToObject(instanceID);
            }
        }
    }
}

