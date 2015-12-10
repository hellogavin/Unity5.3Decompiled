namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEngine;

    internal abstract class WebTemplateManagerBase
    {
        private const float kThumbnailLabelHeight = 20f;
        private const float kThumbnailPadding = 5f;
        private const float kThumbnailSize = 80f;
        private const float kWebTemplateGridPadding = 15f;
        private static Styles s_Styles;
        private GUIContent[] s_TemplateGUIThumbnails;
        private WebTemplate[] s_Templates;

        protected WebTemplateManagerBase()
        {
        }

        private void BuildTemplateList()
        {
            List<WebTemplate> list = new List<WebTemplate>();
            if (Directory.Exists(this.customTemplatesFolder))
            {
                list.AddRange(this.ListTemplates(this.customTemplatesFolder));
            }
            if (Directory.Exists(this.builtinTemplatesFolder))
            {
                list.AddRange(this.ListTemplates(this.builtinTemplatesFolder));
            }
            else
            {
                Debug.LogError("Did not find built-in templates.");
            }
            this.s_Templates = list.ToArray();
            this.s_TemplateGUIThumbnails = new GUIContent[this.s_Templates.Length];
            for (int i = 0; i < this.s_TemplateGUIThumbnails.Length; i++)
            {
                this.s_TemplateGUIThumbnails[i] = this.s_Templates[i].ToGUIContent(this.defaultIcon);
            }
        }

        public void ClearTemplates()
        {
            this.s_Templates = null;
            this.s_TemplateGUIThumbnails = null;
        }

        public int GetTemplateIndex(string path)
        {
            for (int i = 0; i < this.Templates.Length; i++)
            {
                if (path.Equals(this.Templates[i].ToString()))
                {
                    return i;
                }
            }
            return 0;
        }

        private List<WebTemplate> ListTemplates(string path)
        {
            List<WebTemplate> list = new List<WebTemplate>();
            foreach (string str in Directory.GetDirectories(path))
            {
                WebTemplate item = this.Load(str);
                if (item != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        private WebTemplate Load(string path)
        {
            if (!Directory.Exists(path) || (Directory.GetFiles(path, "index.*").Length < 1))
            {
                return null;
            }
            char[] separator = new char[] { '/', '\\' };
            string[] strArray = path.Split(separator);
            WebTemplate template = new WebTemplate {
                m_Name = strArray[strArray.Length - 1]
            };
            if ((strArray.Length > 3) && strArray[strArray.Length - 3].Equals("Assets"))
            {
                template.m_Path = "PROJECT:" + template.m_Name;
            }
            else
            {
                template.m_Path = "APPLICATION:" + template.m_Name;
            }
            string[] files = Directory.GetFiles(path, "thumbnail.*");
            if (files.Length > 0)
            {
                template.m_Thumbnail = new Texture2D(2, 2);
                template.m_Thumbnail.LoadImage(File.ReadAllBytes(files[0]));
            }
            List<string> list = new List<string>();
            Regex regex = new Regex(@"\%UNITY_CUSTOM_([A-Z_]+)\%");
            IEnumerator enumerator = regex.Matches(File.ReadAllText(Directory.GetFiles(path, "index.*")[0])).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Match current = (Match) enumerator.Current;
                    string item = current.Value.Substring("%UNITY_CUSTOM_".Length);
                    item = item.Substring(0, item.Length - 1);
                    if (!list.Contains(item))
                    {
                        list.Add(item);
                    }
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
            template.m_CustomKeys = list.ToArray();
            return template;
        }

        private static string PrettyTemplateKeyName(string name)
        {
            char[] separator = new char[] { '_' };
            string[] strArray = name.Split(separator);
            strArray[0] = UppercaseFirst(strArray[0].ToLower());
            for (int i = 1; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].ToLower();
            }
            return string.Join(" ", strArray);
        }

        public void SelectionUI(SerializedProperty templateProp)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            if (this.TemplateGUIThumbnails.Length < 1)
            {
                GUILayout.Label(EditorGUIUtility.TextContent("No templates found."), new GUILayoutOption[0]);
            }
            else
            {
                int maxRowItems = Mathf.Min((int) Mathf.Max((float) ((Screen.width - 30f) / 80f), (float) 1f), this.TemplateGUIThumbnails.Length);
                int num2 = Mathf.Max((int) Mathf.Ceil(((float) this.TemplateGUIThumbnails.Length) / ((float) maxRowItems)), 1);
                bool changed = GUI.changed;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                templateProp.stringValue = this.Templates[ThumbnailList(GUILayoutUtility.GetRect((float) (maxRowItems * 80f), (float) (num2 * 100f), options), this.GetTemplateIndex(templateProp.stringValue), this.TemplateGUIThumbnails, maxRowItems)].ToString();
                bool flag2 = !changed && GUI.changed;
                bool flag3 = GUI.changed;
                GUI.changed = false;
                foreach (string str in PlayerSettings.templateCustomKeys)
                {
                    string templateCustomValue = PlayerSettings.GetTemplateCustomValue(str);
                    templateCustomValue = EditorGUILayout.TextField(PrettyTemplateKeyName(str), templateCustomValue, new GUILayoutOption[0]);
                    PlayerSettings.SetTemplateCustomValue(str, templateCustomValue);
                }
                if (GUI.changed)
                {
                    templateProp.serializedObject.Update();
                }
                GUI.changed |= flag3;
                if (flag2)
                {
                    GUIUtility.hotControl = 0;
                    GUIUtility.keyboardControl = 0;
                    templateProp.serializedObject.ApplyModifiedProperties();
                    PlayerSettings.templateCustomKeys = this.Templates[this.GetTemplateIndex(templateProp.stringValue)].CustomKeys;
                    templateProp.serializedObject.Update();
                }
            }
        }

        private static int ThumbnailList(Rect rect, int selection, GUIContent[] thumbnails, int maxRowItems)
        {
            int num = 0;
            int index = 0;
            while (index < thumbnails.Length)
            {
                int num3 = 0;
                while ((num3 < maxRowItems) && (index < thumbnails.Length))
                {
                    if (ThumbnailListItem(new Rect(rect.x + (num3 * 80f), rect.y + (num * 100f), 80f, 100f), index == selection, thumbnails[index]))
                    {
                        selection = index;
                    }
                    num3++;
                    index++;
                }
                num++;
            }
            return selection;
        }

        private static bool ThumbnailListItem(Rect rect, bool selected, GUIContent content)
        {
            EventType type = Event.current.type;
            if (type != EventType.MouseDown)
            {
                if (type != EventType.Repaint)
                {
                    return selected;
                }
            }
            else
            {
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (!selected)
                    {
                        GUI.changed = true;
                    }
                    selected = true;
                    Event.current.Use();
                }
                return selected;
            }
            Rect position = new Rect(rect.x + 5f, rect.y + 5f, rect.width - 10f, (rect.height - 20f) - 10f);
            s_Styles.thumbnail.Draw(position, content.image, false, false, selected, selected);
            s_Styles.thumbnailLabel.Draw(new Rect(rect.x, (rect.y + rect.height) - 20f, rect.width, 20f), content.text, false, false, selected, selected);
            return selected;
        }

        private static string UppercaseFirst(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                return string.Empty;
            }
            return (char.ToUpper(target[0]) + target.Substring(1));
        }

        public abstract string builtinTemplatesFolder { get; }

        public abstract string customTemplatesFolder { get; }

        public abstract Texture2D defaultIcon { get; }

        public GUIContent[] TemplateGUIThumbnails
        {
            get
            {
                if ((this.s_Templates == null) || (this.s_TemplateGUIThumbnails == null))
                {
                    this.BuildTemplateList();
                }
                return this.s_TemplateGUIThumbnails;
            }
        }

        public WebTemplate[] Templates
        {
            get
            {
                if ((this.s_Templates == null) || (this.s_TemplateGUIThumbnails == null))
                {
                    this.BuildTemplateList();
                }
                return this.s_Templates;
            }
        }

        private class Styles
        {
            public GUIStyle thumbnail = "IN ThumbnailShadow";
            public GUIStyle thumbnailLabel = "IN ThumbnailSelection";
        }
    }
}

