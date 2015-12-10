namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class IconSelector : EditorWindow
    {
        private GUIContent[] m_LabelIcons;
        private GUIContent[] m_LabelLargeIcons;
        private GUIContent[] m_LargeIcons;
        private MonoScriptIconChangedCallback m_MonoScriptIconChangedCallback;
        private GUIContent m_NoneButtonContent;
        private bool m_ShowLabelIcons;
        private GUIContent[] m_SmallIcons;
        private Texture2D m_StartIcon;
        private static Styles m_Styles;
        private Object m_TargetObject;
        private static int s_HashIconSelector = "IconSelector".GetHashCode();
        private static IconSelector s_IconSelector = null;
        private static long s_LastClosedTime = 0L;
        private static int s_LastInstanceID = -1;

        private IconSelector()
        {
            base.hideFlags = HideFlags.DontSave;
        }

        private void CloseWindow()
        {
            base.Close();
            GUI.changed = true;
            GUIUtility.ExitGUI();
        }

        private Texture2D ConvertLargeIconToSmallIcon(Texture2D largeIcon, ref bool isLabelIcon)
        {
            if (largeIcon == null)
            {
                return null;
            }
            isLabelIcon = true;
            for (int i = 0; i < this.m_LabelLargeIcons.Length; i++)
            {
                if (this.m_LabelLargeIcons[i].image == largeIcon)
                {
                    return (Texture2D) this.m_LabelIcons[i].image;
                }
            }
            isLabelIcon = false;
            for (int j = 0; j < this.m_LargeIcons.Length; j++)
            {
                if (this.m_LargeIcons[j].image == largeIcon)
                {
                    return (Texture2D) this.m_SmallIcons[j].image;
                }
            }
            return largeIcon;
        }

        private Texture2D ConvertSmallIconToLargeIcon(Texture2D smallIcon, bool labelIcon)
        {
            if (labelIcon)
            {
                for (int j = 0; j < this.m_LabelIcons.Length; j++)
                {
                    if (this.m_LabelIcons[j].image == smallIcon)
                    {
                        return (Texture2D) this.m_LabelLargeIcons[j].image;
                    }
                }
                return smallIcon;
            }
            for (int i = 0; i < this.m_SmallIcons.Length; i++)
            {
                if (this.m_SmallIcons[i].image == smallIcon)
                {
                    return (Texture2D) this.m_LargeIcons[i].image;
                }
            }
            return smallIcon;
        }

        private void DoButton(GUIContent content, Texture2D selectedIcon, bool labelIcon)
        {
            int controlID = GUIUtility.GetControlID(s_HashIconSelector, FocusType.Keyboard);
            if (content.image == selectedIcon)
            {
                Rect position = GUILayoutUtility.topLevel.PeekNext();
                float num2 = 2f;
                position.x -= num2;
                position.y -= num2;
                position.width = selectedIcon.width + (2f * num2);
                position.height = selectedIcon.height + (2f * num2);
                GUI.Label(position, GUIContent.none, !labelIcon ? m_Styles.selection : m_Styles.selectionLabel);
            }
            if (EditorGUILayout.IconButton(controlID, content, GUIStyle.none, new GUILayoutOption[0]))
            {
                Texture2D icon = this.ConvertSmallIconToLargeIcon((Texture2D) content.image, labelIcon);
                EditorGUIUtility.SetIconForObject(this.m_TargetObject, icon);
                EditorUtility.ForceReloadInspectors();
                AnnotationWindow.IconChanged();
                if (Event.current.clickCount == 2)
                {
                    this.CloseWindow();
                }
            }
        }

        private void DoTopSection(bool anySelected)
        {
            Rect position = new Rect(6f, 4f, 110f, 20f);
            GUI.Label(position, "Select Icon");
            EditorGUI.BeginDisabledGroup(!anySelected);
            Rect rect2 = new Rect(93f, 6f, 43f, 12f);
            if (GUI.Button(rect2, this.m_NoneButtonContent, m_Styles.noneButton))
            {
                EditorGUIUtility.SetIconForObject(this.m_TargetObject, null);
                EditorUtility.ForceReloadInspectors();
                AnnotationWindow.IconChanged();
            }
            EditorGUI.EndDisabledGroup();
        }

        private GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
        {
            GUIContent[] contentArray = new GUIContent[count];
            for (int i = 0; i < count; i++)
            {
                contentArray[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
            }
            return contentArray;
        }

        private void Init(Object targetObj, Rect activatorRect, bool showLabelIcons)
        {
            this.m_TargetObject = targetObj;
            this.m_StartIcon = EditorGUIUtility.GetIconForObject(this.m_TargetObject);
            this.m_ShowLabelIcons = showLabelIcons;
            Rect buttonRect = GUIUtility.GUIToScreenRect(activatorRect);
            GUIUtility.keyboardControl = 0;
            this.m_LabelLargeIcons = this.GetTextures("sv_label_", string.Empty, 0, 8);
            this.m_LabelIcons = this.GetTextures("sv_icon_name", string.Empty, 0, 8);
            this.m_SmallIcons = this.GetTextures("sv_icon_dot", "_sml", 0, 0x10);
            this.m_LargeIcons = this.GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 0x10);
            this.m_NoneButtonContent = EditorGUIUtility.IconContent("sv_icon_none");
            this.m_NoneButtonContent.text = "None";
            float x = 140f;
            float y = 86f;
            if (this.m_ShowLabelIcons)
            {
                y = 126f;
            }
            base.ShowAsDropDown(buttonRect, new Vector2(x, y));
        }

        private void OnDisable()
        {
            this.SaveIconChanges();
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_IconSelector = null;
        }

        private void OnEnable()
        {
        }

        internal void OnGUI()
        {
            if (m_Styles == null)
            {
                m_Styles = new Styles();
            }
            if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
            {
                this.CloseWindow();
            }
            Texture2D iconForObject = EditorGUIUtility.GetIconForObject(this.m_TargetObject);
            bool isLabelIcon = false;
            if (Event.current.type == EventType.Repaint)
            {
                iconForObject = this.ConvertLargeIconToSmallIcon(iconForObject, ref isLabelIcon);
            }
            Event current = Event.current;
            EventType type = current.type;
            GUI.BeginGroup(new Rect(0f, 0f, base.position.width, base.position.height), m_Styles.background);
            this.DoTopSection(iconForObject != null);
            GUILayout.Space(22f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(1f);
            GUI.enabled = false;
            GUILayout.Label(string.Empty, m_Styles.seperator, new GUILayoutOption[0]);
            GUI.enabled = true;
            GUILayout.Space(1f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
            if (this.m_ShowLabelIcons)
            {
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(6f);
                for (int k = 0; k < (this.m_LabelIcons.Length / 2); k++)
                {
                    this.DoButton(this.m_LabelIcons[k], iconForObject, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(6f);
                for (int m = this.m_LabelIcons.Length / 2; m < this.m_LabelIcons.Length; m++)
                {
                    this.DoButton(this.m_LabelIcons[m], iconForObject, true);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(1f);
                GUI.enabled = false;
                GUILayout.Label(string.Empty, m_Styles.seperator, new GUILayoutOption[0]);
                GUI.enabled = true;
                GUILayout.Space(1f);
                GUILayout.EndHorizontal();
                GUILayout.Space(3f);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(9f);
            for (int i = 0; i < (this.m_SmallIcons.Length / 2); i++)
            {
                this.DoButton(this.m_SmallIcons[i], iconForObject, false);
            }
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(9f);
            for (int j = this.m_SmallIcons.Length / 2; j < this.m_SmallIcons.Length; j++)
            {
                this.DoButton(this.m_SmallIcons[j], iconForObject, false);
            }
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(6f);
            GUI.backgroundColor = new Color(1f, 1f, 1f, 0.7f);
            bool flag2 = false;
            int controlID = GUIUtility.GetControlID(s_HashIconSelector, FocusType.Keyboard);
            if (GUILayout.Button(EditorGUIUtility.TempContent("Other..."), new GUILayoutOption[0]))
            {
                GUIUtility.keyboardControl = controlID;
                flag2 = true;
            }
            GUI.backgroundColor = new Color(1f, 1f, 1f, 1f);
            GUI.EndGroup();
            if (flag2)
            {
                ObjectSelector.get.Show(this.m_TargetObject, typeof(Texture2D), null, false);
                ObjectSelector.get.objectSelectorID = controlID;
                GUI.backgroundColor = new Color(1f, 1f, 1f, 0.7f);
                current.Use();
                GUIUtility.ExitGUI();
            }
            EventType type2 = type;
            if ((type2 == EventType.ExecuteCommand) && (((current.commandName == "ObjectSelectorUpdated") && (ObjectSelector.get.objectSelectorID == controlID)) && (GUIUtility.keyboardControl == controlID)))
            {
                Texture2D currentObject = ObjectSelector.GetCurrentObject() as Texture2D;
                EditorGUIUtility.SetIconForObject(this.m_TargetObject, currentObject);
                GUI.changed = true;
                current.Use();
            }
        }

        private void SaveIconChanges()
        {
            if (EditorGUIUtility.GetIconForObject(this.m_TargetObject) != this.m_StartIcon)
            {
                MonoScript targetObject = this.m_TargetObject as MonoScript;
                if (targetObject != null)
                {
                    if (this.m_MonoScriptIconChangedCallback != null)
                    {
                        this.m_MonoScriptIconChangedCallback(targetObject);
                    }
                    else
                    {
                        MonoImporter.CopyMonoScriptIconToImporters(targetObject);
                    }
                }
            }
        }

        internal static void SetMonoScriptIconChangedCallback(MonoScriptIconChangedCallback callback)
        {
            if (s_IconSelector != null)
            {
                s_IconSelector.m_MonoScriptIconChangedCallback = callback;
            }
            else
            {
                Debug.Log("ERROR: setting callback on hidden IconSelector");
            }
        }

        internal static bool ShowAtPosition(Object targetObj, Rect activatorRect, bool showLabelIcons)
        {
            int instanceID = targetObj.GetInstanceID();
            long num2 = DateTime.Now.Ticks / 0x2710L;
            bool flag = num2 < (s_LastClosedTime + 50L);
            if ((instanceID == s_LastInstanceID) && flag)
            {
                return false;
            }
            Event.current.Use();
            s_LastInstanceID = instanceID;
            if (s_IconSelector == null)
            {
                s_IconSelector = ScriptableObject.CreateInstance<IconSelector>();
            }
            s_IconSelector.Init(targetObj, activatorRect, showLabelIcons);
            return true;
        }

        public delegate void MonoScriptIconChangedCallback(MonoScript monoScript);

        private class Styles
        {
            public GUIStyle background = "sv_iconselector_back";
            public GUIStyle noneButton = "sv_iconselector_button";
            public GUIStyle selection = "sv_iconselector_selection";
            public GUIStyle selectionLabel = "sv_iconselector_labelselection";
            public GUIStyle seperator = "sv_iconselector_sep";
        }
    }
}

