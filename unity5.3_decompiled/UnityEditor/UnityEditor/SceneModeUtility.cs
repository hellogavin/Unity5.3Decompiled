namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class SceneModeUtility
    {
        private static Type s_FocusType;
        private static SceneHierarchyWindow s_HierarchyWindow;
        private static GUIContent s_NoneButtonContent;
        private static Styles s_Styles;

        public static GameObject[] GetObjects(Object[] gameObjects, bool includeChildren)
        {
            List<GameObject> arr = new List<GameObject>();
            if (!includeChildren)
            {
                foreach (GameObject obj2 in gameObjects)
                {
                    arr.Add(obj2);
                }
            }
            else
            {
                foreach (GameObject obj3 in gameObjects)
                {
                    GetObjectsRecurse(obj3.transform, arr);
                }
            }
            return arr.ToArray();
        }

        private static void GetObjectsRecurse(Transform root, List<GameObject> arr)
        {
            arr.Add(root.gameObject);
            IEnumerator enumerator = root.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    GetObjectsRecurse(current, arr);
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

        public static T[] GetSelectedObjectsOfType<T>(out GameObject[] gameObjects, params Type[] types) where T: Object
        {
            if (types.Length == 0)
            {
                types = new Type[] { typeof(T) };
            }
            List<GameObject> list = new List<GameObject>();
            List<T> list2 = new List<T>();
            foreach (Transform transform in Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab))
            {
                foreach (Type type in types)
                {
                    Object component = transform.gameObject.GetComponent(type);
                    if (component != null)
                    {
                        list.Add(transform.gameObject);
                        list2.Add((T) component);
                        break;
                    }
                }
            }
            gameObjects = list.ToArray();
            return list2.ToArray();
        }

        public static Type SearchBar(params Type[] types)
        {
            if (s_NoneButtonContent == null)
            {
                s_NoneButtonContent = EditorGUIUtility.IconContent("sv_icon_none");
                s_NoneButtonContent.text = "None";
            }
            if ((s_FocusType != null) && ((s_HierarchyWindow == null) || (s_HierarchyWindow.m_SearchFilter != ("t:" + s_FocusType.Name))))
            {
                s_FocusType = null;
            }
            GUILayout.Label("Scene Filter:", new GUILayoutOption[0]);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            if (TypeButton(EditorGUIUtility.TempContent("All", AssetPreview.GetMiniTypeThumbnail(typeof(GameObject))), s_FocusType == null, styles.typeButton))
            {
                SearchForType(null);
            }
            for (int i = 0; i < types.Length; i++)
            {
                Type type = types[i];
                Texture2D image = null;
                if (type == typeof(Renderer))
                {
                    image = EditorGUIUtility.IconContent("MeshRenderer Icon").image as Texture2D;
                }
                else if (type == typeof(Terrain))
                {
                    image = EditorGUIUtility.IconContent("Terrain Icon").image as Texture2D;
                }
                else
                {
                    image = AssetPreview.GetMiniTypeThumbnail(type);
                }
                if (TypeButton(EditorGUIUtility.TempContent(ObjectNames.NicifyVariableName(type.Name) + "s", image), type == s_FocusType, styles.typeButton))
                {
                    SearchForType(type);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return s_FocusType;
        }

        public static void SearchForType(Type type)
        {
            Object[] objArray = Resources.FindObjectsOfTypeAll(typeof(SceneHierarchyWindow));
            SceneHierarchyWindow window = (objArray.Length <= 0) ? null : (objArray[0] as SceneHierarchyWindow);
            if (window != null)
            {
                s_HierarchyWindow = window;
                if ((type == null) || (type == typeof(GameObject)))
                {
                    s_FocusType = null;
                    window.ClearSearchFilter();
                }
                else
                {
                    s_FocusType = type;
                    if (window.searchMode == SearchableEditorWindow.SearchMode.Name)
                    {
                        window.searchMode = SearchableEditorWindow.SearchMode.All;
                    }
                    window.SetSearchFilter("t:" + type.Name, window.searchMode, false);
                    window.hasSearchFilterFocus = true;
                }
            }
            else
            {
                s_FocusType = null;
            }
        }

        public static bool SetStaticFlags(Object[] targetObjects, int changedFlags, bool flagValue)
        {
            bool flag = changedFlags == -1;
            StaticEditorFlags flags = !flag ? ((StaticEditorFlags) ((int) Enum.Parse(typeof(StaticEditorFlags), changedFlags.ToString()))) : ((StaticEditorFlags) 0);
            GameObjectUtility.ShouldIncludeChildren children = GameObjectUtility.DisplayUpdateChildrenDialogIfNeeded(targetObjects.OfType<GameObject>(), "Change Static Flags", !flag ? ("Do you want to " + (!flagValue ? "disable" : "enable") + " the " + ObjectNames.NicifyVariableName(flags.ToString()) + " flag for all the child objects as well?") : ("Do you want to " + (!flagValue ? "disable" : "enable") + " the static flags for all the child objects as well?"));
            if (children == GameObjectUtility.ShouldIncludeChildren.Cancel)
            {
                GUIUtility.ExitGUI();
                return false;
            }
            GameObject[] objects = GetObjects(targetObjects, children == GameObjectUtility.ShouldIncludeChildren.IncludeChildren);
            Undo.RecordObjects(objects, "Change Static Flags");
            foreach (GameObject obj2 in objects)
            {
                int staticEditorFlags = (int) GameObjectUtility.GetStaticEditorFlags(obj2);
                staticEditorFlags = !flagValue ? (staticEditorFlags & ~changedFlags) : (staticEditorFlags | changedFlags);
                GameObjectUtility.SetStaticEditorFlags(obj2, (StaticEditorFlags) staticEditorFlags);
            }
            return true;
        }

        public static bool StaticFlagField(string label, SerializedProperty property, int flag)
        {
            bool flag2 = (property.intValue & flag) != 0;
            bool flag3 = (property.hasMultipleDifferentValuesBitwise & flag) != 0;
            EditorGUI.showMixedValue = flag3;
            EditorGUI.BeginChangeCheck();
            bool flagValue = EditorGUILayout.Toggle(label, flag2, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                if (!SetStaticFlags(property.serializedObject.targetObjects, flag, flagValue))
                {
                    return (flag2 && !flag3);
                }
                return flagValue;
            }
            EditorGUI.showMixedValue = false;
            return (flagValue && !flag3);
        }

        private static bool TypeButton(GUIContent label, bool selected, GUIStyle style)
        {
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            bool flag = GUILayout.Toggle(selected, label, style, new GUILayoutOption[0]);
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return (flag && (flag != selected));
        }

        private static Styles styles
        {
            get
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                return s_Styles;
            }
        }

        private class Styles
        {
            public GUIStyle typeButton = "SearchModeFilter";
        }
    }
}

