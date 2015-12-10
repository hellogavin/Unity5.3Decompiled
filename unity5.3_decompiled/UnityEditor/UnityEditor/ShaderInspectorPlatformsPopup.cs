namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class ShaderInspectorPlatformsPopup : PopupWindowContent
    {
        private const float kFrameWidth = 1f;
        private const float kSeparatorHeight = 6f;
        private readonly Shader m_Shader;
        private static int s_CurrentMode = -1;
        private static int s_CurrentPlatformMask = -1;
        private static int s_CurrentVariantStripping = -1;
        internal static readonly string[] s_PlatformModes = new string[] { "Current graphics device", "Current build platform", "All platforms", "Custom:" };
        private static int[] s_ShaderPlatformIndices;
        private static string[] s_ShaderPlatformNames;

        public ShaderInspectorPlatformsPopup(Shader shader)
        {
            this.m_Shader = shader;
            InitializeShaderPlatforms();
        }

        private void DoCustomPlatformBit(Rect rect, int index)
        {
            EditorGUI.BeginChangeCheck();
            int num = ((int) 1) << s_ShaderPlatformIndices[index];
            bool flag = (currentPlatformMask & num) != 0;
            flag = GUI.Toggle(rect, flag, EditorGUIUtility.TempContent(s_ShaderPlatformNames[index]), Styles.menuItem);
            if (EditorGUI.EndChangeCheck())
            {
                if (flag)
                {
                    currentPlatformMask |= num;
                }
                else
                {
                    currentPlatformMask &= ~num;
                }
                currentMode = 3;
            }
        }

        private void DoOneMode(Rect rect, int index)
        {
            EditorGUI.BeginChangeCheck();
            GUI.Toggle(rect, currentMode == index, EditorGUIUtility.TempContent(s_PlatformModes[index]), Styles.menuItem);
            if (EditorGUI.EndChangeCheck())
            {
                currentMode = index;
            }
        }

        private void DoShaderVariants(EditorWindow caller, ref Rect drawPos)
        {
            EditorGUI.BeginChangeCheck();
            bool usedBySceneOnly = GUI.Toggle(drawPos, currentVariantStripping == 1, EditorGUIUtility.TempContent("Skip unused shader_features"), Styles.menuItem);
            drawPos.y += 16f;
            if (EditorGUI.EndChangeCheck())
            {
                currentVariantStripping = !usedBySceneOnly ? 0 : 1;
            }
            drawPos.y += 6f;
            int comboCount = ShaderUtil.GetComboCount(this.m_Shader, usedBySceneOnly);
            string text = !usedBySceneOnly ? (comboCount + " variants total") : (comboCount + " variants included");
            Rect position = drawPos;
            position.x += Styles.menuItem.padding.left;
            position.width -= Styles.menuItem.padding.left + 4;
            GUI.Label(position, text);
            position.xMin = position.xMax - 40f;
            if (GUI.Button(position, "Show", EditorStyles.miniButton))
            {
                ShaderUtil.OpenShaderCombinations(this.m_Shader, usedBySceneOnly);
                caller.Close();
                GUIUtility.ExitGUI();
            }
        }

        private void Draw(EditorWindow caller, float listElementWidth)
        {
            Rect rect = new Rect(0f, 0f, listElementWidth, 16f);
            for (int i = 0; i < s_PlatformModes.Length; i++)
            {
                this.DoOneMode(rect, i);
                rect.y += 16f;
            }
            Color color = GUI.color;
            if (currentMode != 3)
            {
                GUI.color *= new Color(1f, 1f, 1f, 0.7f);
            }
            rect.xMin += 16f;
            for (int j = 0; j < s_ShaderPlatformNames.Length; j++)
            {
                this.DoCustomPlatformBit(rect, j);
                rect.y += 16f;
            }
            GUI.color = color;
            rect.xMin -= 16f;
            this.DrawSeparator(ref rect);
            this.DoShaderVariants(caller, ref rect);
        }

        private void DrawSeparator(ref Rect rect)
        {
            GUI.Label(new Rect(rect.x + 5f, rect.y + 3f, rect.width - 10f, 3f), GUIContent.none, Styles.separator);
            rect.y += 6f;
        }

        public override Vector2 GetWindowSize()
        {
            int num = (s_PlatformModes.Length + s_ShaderPlatformNames.Length) + 2;
            float num2 = (num * 16f) + 18f;
            return new Vector2(210f, num2 + 2f);
        }

        private static void InitializeShaderPlatforms()
        {
            if (s_ShaderPlatformNames == null)
            {
                int availableShaderCompilerPlatforms = ShaderUtil.GetAvailableShaderCompilerPlatforms();
                List<string> list = new List<string>();
                List<int> list2 = new List<int>();
                for (int i = 0; i < 0x20; i++)
                {
                    if ((availableShaderCompilerPlatforms & (((int) 1) << i)) != 0)
                    {
                        list.Add(((ShaderUtil.ShaderCompilerPlatformType) i).ToString());
                        list2.Add(i);
                    }
                }
                s_ShaderPlatformNames = list.ToArray();
                s_ShaderPlatformIndices = list2.ToArray();
            }
        }

        public override void OnGUI(Rect rect)
        {
            if ((this.m_Shader != null) && (Event.current.type != EventType.Layout))
            {
                this.Draw(base.editorWindow, rect.width);
                if (Event.current.type == EventType.MouseMove)
                {
                    Event.current.Use();
                }
                if ((Event.current.type == EventType.KeyDown) && (Event.current.keyCode == KeyCode.Escape))
                {
                    base.editorWindow.Close();
                    GUIUtility.ExitGUI();
                }
            }
        }

        public static int currentMode
        {
            get
            {
                if (s_CurrentMode < 0)
                {
                    s_CurrentMode = EditorPrefs.GetInt("ShaderInspectorPlatformMode", 1);
                }
                return s_CurrentMode;
            }
            set
            {
                s_CurrentMode = value;
                EditorPrefs.SetInt("ShaderInspectorPlatformMode", value);
            }
        }

        public static int currentPlatformMask
        {
            get
            {
                if (s_CurrentPlatformMask < 0)
                {
                    s_CurrentPlatformMask = EditorPrefs.GetInt("ShaderInspectorPlatformMask", 0x3ffff);
                }
                return s_CurrentPlatformMask;
            }
            set
            {
                s_CurrentPlatformMask = value;
                EditorPrefs.SetInt("ShaderInspectorPlatformMask", value);
            }
        }

        public static int currentVariantStripping
        {
            get
            {
                if (s_CurrentVariantStripping < 0)
                {
                    s_CurrentVariantStripping = EditorPrefs.GetInt("ShaderInspectorVariantStripping", 1);
                }
                return s_CurrentVariantStripping;
            }
            set
            {
                s_CurrentVariantStripping = value;
                EditorPrefs.SetInt("ShaderInspectorVariantStripping", value);
            }
        }

        private class Styles
        {
            public static readonly GUIStyle menuItem = "MenuItem";
            public static readonly GUIStyle separator = "sv_iconselector_sep";
        }
    }
}

