namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Rendering;

    internal class AddShaderVariantWindow : EditorWindow
    {
        [CompilerGenerated]
        private static Func<string, string> <>f__am$cache5;
        private const float kMargin = 2f;
        private const float kMinWindowHeight = 264f;
        private const float kMinWindowWidth = 400f;
        private const float kMiscUIHeight = 120f;
        private const float kSeparatorHeight = 3f;
        private const float kSpaceHeight = 6f;
        private List<string> m_AvailableKeywords;
        private PopupData m_Data;
        private List<int> m_FilteredVariants;
        private List<string> m_SelectedKeywords;
        private List<int> m_SelectedVariants;

        public AddShaderVariantWindow()
        {
            base.position = new Rect(100f, 100f, 600f, 396f);
            base.minSize = new Vector2(400f, 264f);
            base.wantsMouseMove = true;
        }

        private void ApplyKeywordFilter()
        {
            this.m_FilteredVariants.Clear();
            this.m_AvailableKeywords.Clear();
            for (int i = 0; i < this.m_Data.keywords.Length; i++)
            {
                bool flag = true;
                for (int j = 0; j < this.m_SelectedKeywords.Count; j++)
                {
                    if (!this.m_Data.keywords[i].Contains<string>(this.m_SelectedKeywords[j]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    this.m_FilteredVariants.Add(i);
                    foreach (string str in this.m_Data.keywords[i])
                    {
                        if (!this.m_AvailableKeywords.Contains(str) && !this.m_SelectedKeywords.Contains(str))
                        {
                            this.m_AvailableKeywords.Add(str);
                        }
                    }
                }
            }
            this.m_AvailableKeywords.Sort();
        }

        private float CalcVerticalSpaceForKeywords()
        {
            return Mathf.Floor((base.position.height - 120f) / 4f);
        }

        private float CalcVerticalSpaceForVariants()
        {
            return ((base.position.height - 120f) / 2f);
        }

        private void Draw(Rect windowRect)
        {
            Rect rect = new Rect(2f, 2f, windowRect.width - 4f, 16f);
            this.DrawSectionHeader(ref rect, "Pick shader keywords to narrow down variant list:", false);
            this.DrawKeywordsList(ref rect, this.m_AvailableKeywords, true);
            this.DrawSectionHeader(ref rect, "Selected keywords:", true);
            this.DrawKeywordsList(ref rect, this.m_SelectedKeywords, false);
            this.DrawSectionHeader(ref rect, "Shader variants with these keywords (click to select):", true);
            if (this.m_FilteredVariants.Count > 0)
            {
                int b = (int) (this.CalcVerticalSpaceForVariants() / 16f);
                for (int i = 0; i < Mathf.Min(this.m_FilteredVariants.Count, b); i++)
                {
                    int index = this.m_FilteredVariants[i];
                    PassType type = (PassType) this.m_Data.types[index];
                    bool flag = this.m_SelectedVariants.Contains(index);
                    string text = type.ToString() + " " + string.Join(" ", this.m_Data.keywords[index]).ToLowerInvariant();
                    bool flag2 = GUI.Toggle(rect, flag, text, Styles.sMenuItem);
                    rect.y += rect.height;
                    if (flag2 && !flag)
                    {
                        this.m_SelectedVariants.Add(index);
                    }
                    else if (!flag2 && flag)
                    {
                        this.m_SelectedVariants.Remove(index);
                    }
                }
                if (this.m_FilteredVariants.Count > b)
                {
                    GUI.Label(rect, string.Format("[{0} more variants skipped]", this.m_FilteredVariants.Count - b), EditorStyles.miniLabel);
                    rect.y += rect.height;
                }
            }
            else
            {
                GUI.Label(rect, "No variants with these keywords");
                rect.y += rect.height;
            }
            rect.y = ((windowRect.height - 2f) - 6f) - 16f;
            rect.height = 16f;
            EditorGUI.BeginDisabledGroup(this.m_SelectedVariants.Count == 0);
            if (GUI.Button(rect, string.Format("Add {0} selected variants", this.m_SelectedVariants.Count)))
            {
                Undo.RecordObject(this.m_Data.collection, "Add variant");
                for (int j = 0; j < this.m_SelectedVariants.Count; j++)
                {
                    int num5 = this.m_SelectedVariants[j];
                    ShaderVariantCollection.ShaderVariant variant = new ShaderVariantCollection.ShaderVariant(this.m_Data.shader, (PassType) this.m_Data.types[num5], this.m_Data.keywords[num5]);
                    this.m_Data.collection.Add(variant);
                }
                base.Close();
                GUIUtility.ExitGUI();
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawKeywordsList(ref Rect rect, List<string> keywords, bool clickingAddsToSelected)
        {
            rect.height = this.CalcVerticalSpaceForKeywords();
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = k => !string.IsNullOrEmpty(k) ? k.ToLowerInvariant() : "<no keyword>";
            }
            List<string> items = keywords.Select<string, string>(<>f__am$cache5).ToList<string>();
            GUI.BeginGroup(rect);
            Rect rect2 = new Rect(4f, 0f, rect.width, rect.height);
            List<Rect> list2 = EditorGUIUtility.GetFlowLayoutedRects(rect2, EditorStyles.miniButton, 2f, 2f, items);
            for (int i = 0; i < items.Count; i++)
            {
                if (this.KeywordButton(list2[i], items[i], rect.size))
                {
                    if (clickingAddsToSelected)
                    {
                        this.m_SelectedKeywords.Add(keywords[i]);
                        this.m_SelectedKeywords.Sort();
                    }
                    else
                    {
                        this.m_SelectedKeywords.Remove(keywords[i]);
                    }
                    this.ApplyKeywordFilter();
                    GUIUtility.ExitGUI();
                }
            }
            GUI.EndGroup();
            rect.y += rect.height;
        }

        private void DrawSectionHeader(ref Rect rect, string titleString, bool separator)
        {
            rect.y += 6f;
            if (separator)
            {
                rect.height = 3f;
                GUI.Label(rect, GUIContent.none, Styles.sSeparator);
                rect.y += rect.height;
            }
            rect.height = 16f;
            GUI.Label(rect, titleString);
            rect.y += rect.height;
        }

        private void Initialize(PopupData data)
        {
            this.m_Data = data;
            this.m_SelectedKeywords = new List<string>();
            this.m_AvailableKeywords = new List<string>();
            this.m_SelectedVariants = new List<int>();
            this.m_AvailableKeywords.Sort();
            this.m_FilteredVariants = new List<int>();
            this.ApplyKeywordFilter();
        }

        private bool KeywordButton(Rect buttonRect, string k, Vector2 areaSize)
        {
            Color color = GUI.color;
            if (buttonRect.yMax > areaSize.y)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.4f);
            }
            bool flag = GUI.Button(buttonRect, EditorGUIUtility.TempContent(k), EditorStyles.miniButton);
            GUI.color = color;
            return flag;
        }

        public void OnGUI()
        {
            if (((this.m_Data == null) || (this.m_Data.shader == null)) || (this.m_Data.collection == null))
            {
                base.Close();
                GUIUtility.ExitGUI();
            }
            else if (Event.current.type != EventType.Layout)
            {
                Rect windowRect = new Rect(0f, 0f, base.position.width, base.position.height);
                this.Draw(windowRect);
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
            }
        }

        public static void ShowAddVariantWindow(PopupData data)
        {
            AddShaderVariantWindow window = EditorWindow.GetWindow<AddShaderVariantWindow>(true, "Add shader " + data.shader.name + " variants to collection");
            window.Initialize(data);
            window.m_Parent.window.m_DontSaveToLayout = true;
        }

        internal class PopupData
        {
            public ShaderVariantCollection collection;
            public string[][] keywords;
            public Shader shader;
            public int[] types;
        }

        private class Styles
        {
            public static readonly GUIStyle sMenuItem = "MenuItem";
            public static readonly GUIStyle sSeparator = "sv_iconselector_sep";
        }
    }
}

