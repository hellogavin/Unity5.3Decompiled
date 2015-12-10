namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class GameViewSizesMenuModifyItemUI : FlexibleMenuModifyItemUI
    {
        private GameViewSize m_GameViewSize;
        private static Styles s_Styles;

        private string GetCroppedText(string fullText, float cropWidth, GUIStyle style)
        {
            int numCharactersThatFitWithinWidth = style.GetNumCharactersThatFitWithinWidth(fullText, cropWidth);
            if ((numCharactersThatFitWithinWidth != -1) && ((numCharactersThatFitWithinWidth > 1) && (numCharactersThatFitWithinWidth != fullText.Length)))
            {
                return (fullText.Substring(0, numCharactersThatFitWithinWidth - 1) + "…");
            }
            return fullText;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(230f, 140f);
        }

        public override void OnClose()
        {
            this.m_GameViewSize = null;
            base.OnClose();
        }

        public override void OnGUI(Rect rect)
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GameViewSize other = base.m_Object as GameViewSize;
            if (other == null)
            {
                Debug.LogError("Invalid object");
            }
            else
            {
                if (this.m_GameViewSize == null)
                {
                    this.m_GameViewSize = new GameViewSize(other);
                }
                bool flag = (this.m_GameViewSize.width > 0) && (this.m_GameViewSize.height > 0);
                GUILayout.Space(3f);
                GUILayout.Label((base.m_MenuType != FlexibleMenuModifyItemUI.MenuType.Add) ? s_Styles.headerEdit : s_Styles.headerAdd, EditorStyles.boldLabel, new GUILayoutOption[0]);
                FlexibleMenu.DrawRect(GUILayoutUtility.GetRect((float) 1f, (float) 1f), !EditorGUIUtility.isProSkin ? new Color(0.6f, 0.6f, 0.6f, 1.333f) : new Color(0.32f, 0.32f, 0.32f, 1.333f));
                GUILayout.Space(4f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(90f) };
                GUILayout.Label(s_Styles.optionalText, options);
                GUILayout.Space(10f);
                this.m_GameViewSize.baseText = EditorGUILayout.TextField(this.m_GameViewSize.baseText, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(90f) };
                GUILayout.Label(s_Styles.typeName, optionArray2);
                GUILayout.Space(10f);
                this.m_GameViewSize.sizeType = (GameViewSizeType) EditorGUILayout.Popup((int) this.m_GameViewSize.sizeType, s_Styles.typeNames, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.Width(90f) };
                GUILayout.Label(s_Styles.widthHeightText, optionArray3);
                GUILayout.Space(10f);
                this.m_GameViewSize.width = EditorGUILayout.IntField(this.m_GameViewSize.width, new GUILayoutOption[0]);
                GUILayout.Space(5f);
                this.m_GameViewSize.height = EditorGUILayout.IntField(this.m_GameViewSize.height, new GUILayoutOption[0]);
                GUILayout.EndHorizontal();
                GUILayout.Space(10f);
                float pixels = 10f;
                float cropWidth = rect.width - (2f * pixels);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(pixels);
                GUILayout.FlexibleSpace();
                string displayText = this.m_GameViewSize.displayText;
                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(displayText));
                if (string.IsNullOrEmpty(displayText))
                {
                    displayText = "Result";
                }
                else
                {
                    displayText = this.GetCroppedText(displayText, cropWidth, EditorStyles.label);
                }
                GUILayout.Label(GUIContent.Temp(displayText), EditorStyles.label, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.Space(pixels);
                GUILayout.EndHorizontal();
                GUILayout.Space(5f);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                if (GUILayout.Button(s_Styles.cancel, new GUILayoutOption[0]))
                {
                    base.editorWindow.Close();
                }
                EditorGUI.BeginDisabledGroup(!flag);
                if (GUILayout.Button(s_Styles.ok, new GUILayoutOption[0]))
                {
                    other.Set(this.m_GameViewSize);
                    base.Accepted();
                    base.editorWindow.Close();
                }
                EditorGUI.EndDisabledGroup();
                GUILayout.Space(10f);
                GUILayout.EndHorizontal();
            }
        }

        private class Styles
        {
            public GUIContent cancel = new GUIContent("Cancel");
            public GUIContent headerAdd = new GUIContent("Add");
            public GUIContent headerEdit = new GUIContent("Edit");
            public GUIContent ok = new GUIContent("OK");
            public GUIContent optionalText = new GUIContent("Label");
            public GUIContent typeName = new GUIContent("Type");
            public GUIContent[] typeNames = new GUIContent[] { new GUIContent("Aspect Ratio"), new GUIContent("Fixed Resolution") };
            public GUIContent widthHeightText = new GUIContent("Width & Height");
        }
    }
}

