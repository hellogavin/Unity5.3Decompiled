namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    internal class PopupList : PopupWindowContent
    {
        [CompilerGenerated]
        private static Func<ListElement, string> <>f__am$cacheB;
        private const float frameWidth = 1f;
        private const float gizmoRightAlign = 23f;
        private const float iconRightAlign = 64f;
        private const float k_LineHeight = 16f;
        private const float k_Margin = 10f;
        private const float k_TextFieldHeight = 16f;
        private const float listElementHeight = 18f;
        private InputData m_Data;
        private string m_EnteredText;
        private string m_EnteredTextCompletion;
        private Gravity m_Gravity;
        private Vector2 m_ScreenPos;
        private Vector2 m_ScrollPosition;
        private int m_SelectedCompletionIndex;
        private static EditorGUI.RecycledTextEditor s_RecycledEditor = new EditorGUI.RecycledTextEditor();
        private static Styles s_Styles;
        private static int s_TextFieldHash = s_TextFieldName.GetHashCode();
        private static string s_TextFieldName = "ProjectBrowserPopupsTextField";
        private const float scrollBarWidth = 14f;

        public PopupList(InputData inputData) : this(inputData, null)
        {
        }

        public PopupList(InputData inputData, string initialSelectionLabel)
        {
            this.m_EnteredTextCompletion = string.Empty;
            this.m_EnteredText = string.Empty;
            this.m_Data = inputData;
            this.m_Data.ResetScores();
            this.SelectNoCompletion();
            this.m_Gravity = Gravity.Top;
            if (initialSelectionLabel != null)
            {
                this.m_EnteredTextCompletion = initialSelectionLabel;
                this.UpdateCompletion();
            }
        }

        private void AdjustRecycledEditorSelectionToCompletion()
        {
            if (this.m_EnteredTextCompletion != string.Empty)
            {
                s_RecycledEditor.text = this.m_EnteredTextCompletion;
                EditorGUI.s_OriginalText = this.m_EnteredTextCompletion;
                s_RecycledEditor.cursorIndex = this.m_EnteredText.Length;
                s_RecycledEditor.selectIndex = this.m_EnteredTextCompletion.Length;
            }
        }

        private void ChangeSelectedCompletion(int change)
        {
            int filteredCount = this.m_Data.GetFilteredCount(this.m_EnteredText);
            if ((this.m_SelectedCompletionIndex == -1) && (change < 0))
            {
                this.m_SelectedCompletionIndex = filteredCount;
            }
            int index = (filteredCount <= 0) ? 0 : (((this.m_SelectedCompletionIndex + change) + filteredCount) % filteredCount);
            this.SelectCompletionWithIndex(index);
        }

        private string CurrentDisplayedText()
        {
            return (!(this.m_EnteredTextCompletion != string.Empty) ? this.m_EnteredText : this.m_EnteredTextCompletion);
        }

        private void DrawCustomTextField(EditorWindow editorWindow, Rect windowRect)
        {
            bool flag5;
            if (!this.m_Data.m_AllowCustom)
            {
                return;
            }
            Event current = Event.current;
            bool enableAutoCompletion = this.m_Data.m_EnableAutoCompletion;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            string label = this.CurrentDisplayedText();
            if (current.type == EventType.KeyDown)
            {
                KeyCode keyCode = current.keyCode;
                switch (keyCode)
                {
                    case KeyCode.Backspace:
                        goto Label_0136;

                    case KeyCode.Tab:
                    case KeyCode.Return:
                        break;

                    default:
                        switch (keyCode)
                        {
                            case KeyCode.UpArrow:
                                this.ChangeSelectedCompletion(-1);
                                flag3 = true;
                                goto Label_017A;

                            case KeyCode.DownArrow:
                                this.ChangeSelectedCompletion(1);
                                flag3 = true;
                                goto Label_017A;

                            case KeyCode.None:
                                if ((current.character == ' ') || (current.character == ','))
                                {
                                    flag3 = true;
                                }
                                goto Label_017A;

                            case KeyCode.Space:
                            case KeyCode.Comma:
                                break;

                            case KeyCode.Delete:
                                goto Label_0136;

                            default:
                                goto Label_017A;
                        }
                        break;
                }
                if (label != string.Empty)
                {
                    if (this.m_Data.m_OnSelectCallback != null)
                    {
                        this.m_Data.m_OnSelectCallback(this.m_Data.NewOrMatchingElement(label));
                    }
                    if ((current.keyCode == KeyCode.Tab) || (current.keyCode == KeyCode.Comma))
                    {
                        flag4 = true;
                    }
                    if (this.m_Data.m_CloseOnSelection || (current.keyCode == KeyCode.Return))
                    {
                        flag2 = true;
                    }
                }
                flag3 = true;
            }
            goto Label_017A;
        Label_0136:
            enableAutoCompletion = false;
        Label_017A:
            flag5 = false;
            Rect position = new Rect(windowRect.x + 5f, windowRect.y + ((this.m_Gravity != Gravity.Top) ? ((windowRect.height - 16f) - 5f) : 5f), (windowRect.width - 10f) - 14f, 16f);
            GUI.SetNextControlName(s_TextFieldName);
            EditorGUI.FocusTextInControl(s_TextFieldName);
            int id = GUIUtility.GetControlID(s_TextFieldHash, FocusType.Keyboard, position);
            if (flag3)
            {
                current.Use();
            }
            if (GUIUtility.keyboardControl == 0)
            {
                GUIUtility.keyboardControl = id;
            }
            string str2 = EditorGUI.DoTextField(s_RecycledEditor, id, position, label, s_Styles.customTextField, null, out flag5, false, false, false);
            Rect rect2 = position;
            rect2.x += position.width;
            rect2.width = 14f;
            if ((GUI.Button(rect2, GUIContent.none, !(str2 != string.Empty) ? s_Styles.customTextFieldCancelButtonEmpty : s_Styles.customTextFieldCancelButton) && (str2 != string.Empty)) || flag4)
            {
                string str3 = string.Empty;
                s_RecycledEditor.text = str3;
                str2 = EditorGUI.s_OriginalText = str3;
                s_RecycledEditor.cursorIndex = 0;
                s_RecycledEditor.selectIndex = 0;
                enableAutoCompletion = false;
            }
            if (label != str2)
            {
                this.m_EnteredText = ((0 > s_RecycledEditor.cursorIndex) || (s_RecycledEditor.cursorIndex >= str2.Length)) ? str2 : str2.Substring(0, s_RecycledEditor.cursorIndex);
                if (enableAutoCompletion)
                {
                    this.UpdateCompletion();
                }
                else
                {
                    this.SelectNoCompletion();
                }
            }
            if (flag2)
            {
                editorWindow.Close();
            }
        }

        private void DrawList(EditorWindow editorWindow, Rect windowRect)
        {
            Event current = Event.current;
            int index = -1;
            IEnumerator<ListElement> enumerator = this.m_Data.GetFilteredList(this.m_EnteredText).GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    ListElement element = enumerator.Current;
                    index++;
                    Rect position = new Rect(windowRect.x, ((windowRect.y + 10f) + (index * 16f)) + (((this.m_Gravity != Gravity.Top) || !this.m_Data.m_AllowCustom) ? 0f : 16f), windowRect.width, 16f);
                    EventType type = current.type;
                    switch (type)
                    {
                        case EventType.MouseDown:
                        {
                            if (((Event.current.button == 0) && position.Contains(Event.current.mousePosition)) && element.enabled)
                            {
                                if (this.m_Data.m_OnSelectCallback != null)
                                {
                                    this.m_Data.m_OnSelectCallback(element);
                                }
                                current.Use();
                                if (this.m_Data.m_CloseOnSelection)
                                {
                                    editorWindow.Close();
                                }
                            }
                            continue;
                        }
                        case EventType.MouseMove:
                            break;

                        default:
                        {
                            if (type == EventType.Repaint)
                            {
                                GUIStyle style = !element.partiallySelected ? s_Styles.menuItem : s_Styles.menuItemMixed;
                                bool on = element.selected || element.partiallySelected;
                                bool hasKeyboardFocus = false;
                                bool isHover = index == this.m_SelectedCompletionIndex;
                                bool isActive = on;
                                EditorGUI.BeginDisabledGroup(!element.enabled);
                                GUIContent content = element.m_Content;
                                style.Draw(position, content, isHover, isActive, on, hasKeyboardFocus);
                                EditorGUI.EndDisabledGroup();
                            }
                            continue;
                        }
                    }
                    if (position.Contains(Event.current.mousePosition))
                    {
                        this.SelectCompletionWithIndex(index);
                        current.Use();
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        public virtual float GetWindowHeight()
        {
            int num = (this.m_Data.m_MaxCount != 0) ? this.m_Data.m_MaxCount : this.m_Data.GetFilteredCount(this.m_EnteredText);
            return (((num * 16f) + 20f) + (!this.m_Data.m_AllowCustom ? 0f : 16f));
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(this.GetWindowWidth(), this.GetWindowHeight());
        }

        public virtual float GetWindowWidth()
        {
            return 150f;
        }

        public override void OnClose()
        {
            if (this.m_Data != null)
            {
                this.m_Data.ResetScores();
            }
        }

        public override void OnGUI(Rect windowRect)
        {
            Event current = Event.current;
            if (current.type != EventType.Layout)
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                if ((current.type == EventType.KeyDown) && (current.keyCode == KeyCode.Escape))
                {
                    base.editorWindow.Close();
                    GUIUtility.ExitGUI();
                }
                if (this.m_Gravity == Gravity.Bottom)
                {
                    this.DrawList(base.editorWindow, windowRect);
                    this.DrawCustomTextField(base.editorWindow, windowRect);
                }
                else
                {
                    this.DrawCustomTextField(base.editorWindow, windowRect);
                    this.DrawList(base.editorWindow, windowRect);
                }
                if (current.type == EventType.Repaint)
                {
                    s_Styles.background.Draw(new Rect(windowRect.x, windowRect.y, windowRect.width, windowRect.height), false, false, false, false);
                }
            }
        }

        private void SelectCompletionWithIndex(int index)
        {
            this.m_SelectedCompletionIndex = index;
            this.m_EnteredTextCompletion = string.Empty;
            this.UpdateCompletion();
        }

        private void SelectNoCompletion()
        {
            this.m_SelectedCompletionIndex = -1;
            this.m_EnteredTextCompletion = string.Empty;
            this.AdjustRecycledEditorSelectionToCompletion();
        }

        private void UpdateCompletion()
        {
            if (this.m_Data.m_EnableAutoCompletion)
            {
                if (<>f__am$cacheB == null)
                {
                    <>f__am$cacheB = element => element.text;
                }
                IEnumerable<string> source = this.m_Data.GetFilteredList(this.m_EnteredText).Select<ListElement, string>(<>f__am$cacheB);
                if ((this.m_EnteredTextCompletion != string.Empty) && this.m_EnteredTextCompletion.StartsWith(this.m_EnteredText, StringComparison.OrdinalIgnoreCase))
                {
                    this.m_SelectedCompletionIndex = source.TakeWhile<string>(element => (element != this.m_EnteredTextCompletion)).Count<string>();
                }
                else
                {
                    if (this.m_SelectedCompletionIndex < 0)
                    {
                        this.m_SelectedCompletionIndex = 0;
                    }
                    else if (this.m_SelectedCompletionIndex >= source.Count<string>())
                    {
                        this.m_SelectedCompletionIndex = source.Count<string>() - 1;
                    }
                    this.m_EnteredTextCompletion = source.Skip<string>(this.m_SelectedCompletionIndex).DefaultIfEmpty<string>(string.Empty).FirstOrDefault<string>();
                }
                this.AdjustRecycledEditorSelectionToCompletion();
            }
        }

        public enum Gravity
        {
            Top,
            Bottom
        }

        public class InputData
        {
            [CompilerGenerated]
            private static Func<PopupList.ListElement, float> <>f__am$cache7;
            [CompilerGenerated]
            private static Func<PopupList.ListElement, string> <>f__am$cache8;
            public bool m_AllowCustom;
            public bool m_CloseOnSelection;
            public bool m_EnableAutoCompletion = true;
            public List<PopupList.ListElement> m_ListElements = new List<PopupList.ListElement>();
            public int m_MaxCount;
            public PopupList.OnSelectCallback m_OnSelectCallback;
            public bool m_SortAlphabetically;

            public virtual IEnumerable<PopupList.ListElement> BuildQuery(string prefix)
            {
                <BuildQuery>c__AnonStorey3E storeye = new <BuildQuery>c__AnonStorey3E {
                    prefix = prefix
                };
                if (storeye.prefix == string.Empty)
                {
                    return this.m_ListElements;
                }
                return this.m_ListElements.Where<PopupList.ListElement>(new Func<PopupList.ListElement, bool>(storeye.<>m__5F));
            }

            public void DeselectAll()
            {
                foreach (PopupList.ListElement element in this.m_ListElements)
                {
                    element.selected = false;
                    element.partiallySelected = false;
                }
            }

            public int GetFilteredCount(string prefix)
            {
                IEnumerable<PopupList.ListElement> source = this.BuildQuery(prefix);
                if (this.m_MaxCount > 0)
                {
                    source = source.Take<PopupList.ListElement>(this.m_MaxCount);
                }
                return source.Count<PopupList.ListElement>();
            }

            public IEnumerable<PopupList.ListElement> GetFilteredList(string prefix)
            {
                IEnumerable<PopupList.ListElement> source = this.BuildQuery(prefix);
                if (this.m_MaxCount > 0)
                {
                    if (<>f__am$cache7 == null)
                    {
                        <>f__am$cache7 = element => element.filterScore;
                    }
                    source = source.OrderByDescending<PopupList.ListElement, float>(<>f__am$cache7).Take<PopupList.ListElement>(this.m_MaxCount);
                }
                if (!this.m_SortAlphabetically)
                {
                    return source;
                }
                if (<>f__am$cache8 == null)
                {
                    <>f__am$cache8 = element => element.text.ToLower();
                }
                return source.OrderBy<PopupList.ListElement, string>(<>f__am$cache8);
            }

            public PopupList.ListElement NewOrMatchingElement(string label)
            {
                foreach (PopupList.ListElement element in this.m_ListElements)
                {
                    if (element.text.Equals(label, StringComparison.OrdinalIgnoreCase))
                    {
                        return element;
                    }
                }
                PopupList.ListElement item = new PopupList.ListElement(label, false, -1f);
                this.m_ListElements.Add(item);
                return item;
            }

            public void ResetScores()
            {
                foreach (PopupList.ListElement element in this.m_ListElements)
                {
                    element.ResetScore();
                }
            }

            [CompilerGenerated]
            private sealed class <BuildQuery>c__AnonStorey3E
            {
                internal string prefix;

                internal bool <>m__5F(PopupList.ListElement element)
                {
                    return element.m_Content.text.StartsWith(this.prefix, StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public class ListElement
        {
            public GUIContent m_Content;
            private bool m_Enabled;
            private float m_FilterScore;
            private bool m_PartiallySelected;
            private bool m_Selected;
            private bool m_WasSelected;

            public ListElement(string text) : this(text, false)
            {
            }

            public ListElement(string text, bool selected)
            {
                this.m_Content = new GUIContent(text);
                this.m_Selected = selected;
                this.filterScore = 0f;
                this.m_PartiallySelected = false;
                this.m_Enabled = true;
            }

            public ListElement(string text, bool selected, float score)
            {
                this.m_Content = new GUIContent(text);
                if (!string.IsNullOrEmpty(this.m_Content.text))
                {
                    char[] chArray = this.m_Content.text.ToCharArray();
                    chArray[0] = char.ToUpper(chArray[0]);
                    this.m_Content.text = new string(chArray);
                }
                this.m_Selected = selected;
                this.filterScore = score;
                this.m_PartiallySelected = false;
                this.m_Enabled = true;
            }

            public void ResetScore()
            {
                this.m_WasSelected = this.m_Selected || this.m_PartiallySelected;
            }

            public bool enabled
            {
                get
                {
                    return this.m_Enabled;
                }
                set
                {
                    this.m_Enabled = value;
                }
            }

            public float filterScore
            {
                get
                {
                    return (!this.m_WasSelected ? this.m_FilterScore : float.MaxValue);
                }
                set
                {
                    this.m_FilterScore = value;
                    this.ResetScore();
                }
            }

            public bool partiallySelected
            {
                get
                {
                    return this.m_PartiallySelected;
                }
                set
                {
                    this.m_PartiallySelected = value;
                    if (this.m_PartiallySelected)
                    {
                        this.m_WasSelected = true;
                    }
                }
            }

            public bool selected
            {
                get
                {
                    return this.m_Selected;
                }
                set
                {
                    this.m_Selected = value;
                    if (this.m_Selected)
                    {
                        this.m_WasSelected = true;
                    }
                }
            }

            public string text
            {
                get
                {
                    return this.m_Content.text;
                }
                set
                {
                    this.m_Content.text = value;
                }
            }
        }

        public delegate void OnSelectCallback(PopupList.ListElement element);

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle customTextField = new GUIStyle(EditorStyles.toolbarSearchField);
            public GUIStyle customTextFieldCancelButton = new GUIStyle(EditorStyles.toolbarSearchFieldCancelButton);
            public GUIStyle customTextFieldCancelButtonEmpty = new GUIStyle(EditorStyles.toolbarSearchFieldCancelButtonEmpty);
            public GUIStyle label = "PR Label";
            public GUIStyle menuItem = "MenuItem";
            public GUIStyle menuItemMixed = "MenuItemMixed";
        }
    }
}

