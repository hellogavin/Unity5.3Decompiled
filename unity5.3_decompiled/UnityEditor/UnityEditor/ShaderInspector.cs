namespace UnityEditor
{
    using System;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CustomEditor(typeof(Shader))]
    internal class ShaderInspector : Editor
    {
        private static readonly int kErrorViewHash = "ShaderErrorView".GetHashCode();
        private static readonly string[] kPropertyTypes = new string[] { "Color: ", "Vector: ", "Float: ", "Range: ", "Texture: " };
        private const float kSpace = 5f;
        private static readonly string[] kTextureTypes = new string[] { "No Texture?: ", "1D texture: ", "Texture: ", "Volume: ", "Cubemap: ", "Any texture: " };
        private Vector2 m_ScrollPosition = Vector2.zero;

        private static string GetPropertyType(Shader s, int index)
        {
            ShaderUtil.ShaderPropertyType propertyType = ShaderUtil.GetPropertyType(s, index);
            if (propertyType == ShaderUtil.ShaderPropertyType.TexEnv)
            {
                return kTextureTypes[(int) ShaderUtil.GetTexDim(s, index)];
            }
            return kPropertyTypes[(int) propertyType];
        }

        public virtual void OnEnable()
        {
            Shader target = this.target as Shader;
            ShaderUtil.FetchCachedErrors(target);
        }

        public override void OnInspectorGUI()
        {
            Shader target = this.target as Shader;
            if (target != null)
            {
                GUI.enabled = true;
                EditorGUI.indentLevel = 0;
                this.ShowShaderCodeArea(target);
                if (target.isSupported)
                {
                    string str;
                    EditorGUILayout.LabelField("Cast shadows", !ShaderUtil.HasShadowCasterPass(target) ? "no" : "yes", new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Render queue", ShaderUtil.GetRenderQueue(target).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("LOD", ShaderUtil.GetLOD(target).ToString(CultureInfo.InvariantCulture), new GUILayoutOption[0]);
                    EditorGUILayout.LabelField("Ignore projector", !ShaderUtil.DoesIgnoreProjector(target) ? "no" : "yes", new GUILayoutOption[0]);
                    switch (target.disableBatching)
                    {
                        case DisableBatchingType.False:
                            str = "no";
                            break;

                        case DisableBatchingType.True:
                            str = "yes";
                            break;

                        case DisableBatchingType.WhenLODFading:
                            str = "when LOD fading is on";
                            break;

                        default:
                            str = "unknown";
                            break;
                    }
                    EditorGUILayout.LabelField("Disable batching", str, new GUILayoutOption[0]);
                    ShowShaderProperties(target);
                }
            }
        }

        internal static void ShaderErrorListUI(Object shader, ShaderError[] errors, ref Vector2 scrollPosition)
        {
            <ShaderErrorListUI>c__AnonStorey92 storey = new <ShaderErrorListUI>c__AnonStorey92 {
                errors = errors
            };
            int length = storey.errors.Length;
            GUILayout.Space(5f);
            GUILayout.Label(string.Format("Errors ({0}):", length), EditorStyles.boldLabel, new GUILayoutOption[0]);
            int controlID = GUIUtility.GetControlID(kErrorViewHash, FocusType.Native);
            float minHeight = Mathf.Min((float) ((length * 20f) + 40f), (float) 150f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(minHeight) };
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUISkin.current.box, options);
            EditorGUIUtility.SetIconSize(new Vector2(16f, 16f));
            float height = Styles.messageStyle.CalcHeight(EditorGUIUtility.TempContent(Styles.errorIcon), 100f);
            Event current = Event.current;
            for (int i = 0; i < length; i++)
            {
                Rect position = EditorGUILayout.GetControlRect(false, height, new GUILayoutOption[0]);
                string message = storey.errors[i].message;
                string platform = storey.errors[i].platform;
                bool flag = storey.errors[i].warning != 0;
                string lastPathNameComponent = FileUtil.GetLastPathNameComponent(storey.errors[i].file);
                int line = storey.errors[i].line;
                if (((current.type == EventType.MouseDown) && (current.button == 0)) && position.Contains(current.mousePosition))
                {
                    GUIUtility.keyboardControl = controlID;
                    if (current.clickCount == 2)
                    {
                        string file = storey.errors[i].file;
                        Object obj2 = !string.IsNullOrEmpty(file) ? AssetDatabase.LoadMainAssetAtPath(file) : null;
                        if (obj2 == null)
                        {
                        }
                        AssetDatabase.OpenAsset(shader, line);
                        GUIUtility.ExitGUI();
                    }
                    current.Use();
                }
                if ((current.type == EventType.ContextClick) && position.Contains(current.mousePosition))
                {
                    <ShaderErrorListUI>c__AnonStorey93 storey2 = new <ShaderErrorListUI>c__AnonStorey93 {
                        <>f__ref$146 = storey
                    };
                    current.Use();
                    GenericMenu menu = new GenericMenu();
                    storey2.errorIndex = i;
                    menu.AddItem(new GUIContent("Copy error text"), false, new GenericMenu.MenuFunction(storey2.<>m__1A7));
                    menu.ShowAsContext();
                }
                if ((current.type == EventType.Repaint) && ((i & 1) == 0))
                {
                    Styles.evenBackground.Draw(position, false, false, false, false);
                }
                Rect rect2 = position;
                rect2.xMin = rect2.xMax;
                if (line > 0)
                {
                    GUIContent content;
                    if (string.IsNullOrEmpty(lastPathNameComponent))
                    {
                        content = EditorGUIUtility.TempContent(line.ToString(CultureInfo.InvariantCulture));
                    }
                    else
                    {
                        content = EditorGUIUtility.TempContent(lastPathNameComponent + ":" + line.ToString(CultureInfo.InvariantCulture));
                    }
                    Vector2 vector = EditorStyles.miniLabel.CalcSize(content);
                    rect2.xMin -= vector.x;
                    GUI.Label(rect2, content, EditorStyles.miniLabel);
                    rect2.xMin -= 2f;
                    if (rect2.width < 30f)
                    {
                        rect2.xMin = rect2.xMax - 30f;
                    }
                }
                Rect rect3 = rect2;
                rect3.width = 0f;
                if (platform.Length > 0)
                {
                    GUIContent content2 = EditorGUIUtility.TempContent(platform);
                    Vector2 vector2 = EditorStyles.miniLabel.CalcSize(content2);
                    rect3.xMin -= vector2.x;
                    Color contentColor = GUI.contentColor;
                    GUI.contentColor = new Color(1f, 1f, 1f, 0.5f);
                    GUI.Label(rect3, content2, EditorStyles.miniLabel);
                    GUI.contentColor = contentColor;
                    rect3.xMin -= 2f;
                }
                Rect rect4 = position;
                rect4.xMax = rect3.xMin;
                GUI.Label(rect4, EditorGUIUtility.TempContent(message, !flag ? Styles.errorIcon : Styles.warningIcon), Styles.messageStyle);
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
            GUILayout.EndScrollView();
        }

        private void ShowCompiledCodeButton(Shader s)
        {
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("Compiled code", EditorStyles.miniButton);
            if ((ShaderUtil.HasShaderSnippets(s) || ShaderUtil.HasSurfaceShaders(s)) || ShaderUtil.HasFixedFunctionShaders(s))
            {
                GUIContent showCurrent = Styles.showCurrent;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                Rect position = GUILayoutUtility.GetRect(showCurrent, EditorStyles.miniButton, options);
                Rect rect2 = new Rect(position.xMax - 16f, position.y, 16f, position.height);
                if (EditorGUI.ButtonMouseDown(rect2, GUIContent.none, FocusType.Passive, GUIStyle.none))
                {
                    PopupWindow.Show(GUILayoutUtility.topLevel.GetLast(), new ShaderInspectorPlatformsPopup(s));
                    GUIUtility.ExitGUI();
                }
                if (GUI.Button(position, showCurrent, EditorStyles.miniButton))
                {
                    ShaderUtil.OpenCompiledShader(s, ShaderInspectorPlatformsPopup.currentMode, ShaderInspectorPlatformsPopup.currentPlatformMask, ShaderInspectorPlatformsPopup.currentVariantStripping == 0);
                    GUIUtility.ExitGUI();
                }
            }
            else
            {
                GUILayout.Button("none (precompiled shader)", GUI.skin.label, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void ShowFixedFunctionShaderButton(Shader s)
        {
            bool flag = ShaderUtil.HasFixedFunctionShaders(s);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("Fixed function", EditorStyles.miniButton);
            if (flag)
            {
                if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s)) != null)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(Styles.showFF, EditorStyles.miniButton, options))
                    {
                        ShaderUtil.OpenGeneratedFixedFunctionShader(s);
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    GUILayout.Button(Styles.builtinShader, GUI.skin.label, new GUILayoutOption[0]);
                }
            }
            else
            {
                GUILayout.Button(Styles.no, GUI.skin.label, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowShaderCodeArea(Shader s)
        {
            ShowSurfaceShaderButton(s);
            ShowFixedFunctionShaderButton(s);
            this.ShowCompiledCodeButton(s);
            this.ShowShaderErrors(s);
        }

        private void ShowShaderErrors(Shader s)
        {
            if (ShaderUtil.GetShaderErrorCount(s) >= 1)
            {
                ShaderErrorListUI(s, ShaderUtil.GetShaderErrors(s), ref this.m_ScrollPosition);
            }
        }

        private static void ShowShaderProperties(Shader s)
        {
            GUILayout.Space(5f);
            GUILayout.Label("Properties:", EditorStyles.boldLabel, new GUILayoutOption[0]);
            int propertyCount = ShaderUtil.GetPropertyCount(s);
            for (int i = 0; i < propertyCount; i++)
            {
                string propertyName = ShaderUtil.GetPropertyName(s, i);
                string str2 = GetPropertyType(s, i) + ShaderUtil.GetPropertyDescription(s, i);
                EditorGUILayout.LabelField(propertyName, str2, new GUILayoutOption[0]);
            }
        }

        private static void ShowSurfaceShaderButton(Shader s)
        {
            bool flag = ShaderUtil.HasSurfaceShaders(s);
            EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
            EditorGUILayout.PrefixLabel("Surface shader", EditorStyles.miniButton);
            if (flag)
            {
                if (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(s)) != null)
                {
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    if (GUILayout.Button(Styles.showSurface, EditorStyles.miniButton, options))
                    {
                        ShaderUtil.OpenParsedSurfaceShader(s);
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    GUILayout.Button(Styles.builtinShader, GUI.skin.label, new GUILayoutOption[0]);
                }
            }
            else
            {
                GUILayout.Button(Styles.no, GUI.skin.label, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndHorizontal();
        }

        [CompilerGenerated]
        private sealed class <ShaderErrorListUI>c__AnonStorey92
        {
            internal ShaderError[] errors;
        }

        [CompilerGenerated]
        private sealed class <ShaderErrorListUI>c__AnonStorey93
        {
            internal ShaderInspector.<ShaderErrorListUI>c__AnonStorey92 <>f__ref$146;
            internal int errorIndex;

            internal void <>m__1A7()
            {
                string message = this.<>f__ref$146.errors[this.errorIndex].message;
                if (!string.IsNullOrEmpty(this.<>f__ref$146.errors[this.errorIndex].messageDetails))
                {
                    message = message + ((string) '\n') + this.<>f__ref$146.errors[this.errorIndex].messageDetails;
                }
                EditorGUIUtility.systemCopyBuffer = message;
            }
        }

        internal class Styles
        {
            public static GUIContent builtinShader = EditorGUIUtility.TextContent("Built-in shader");
            public static Texture2D errorIcon = EditorGUIUtility.LoadIcon("console.erroricon.sml");
            public static GUIStyle evenBackground = "CN EntryBackEven";
            public static GUIStyle messageStyle = "CN StatusInfo";
            public static GUIContent no = EditorGUIUtility.TextContent("no");
            public static GUIContent showCurrent = new GUIContent("Compile and show code | ▾");
            public static GUIContent showFF = EditorGUIUtility.TextContent("Show generated code|Show generated code of a fixed function shader");
            public static GUIContent showSurface = EditorGUIUtility.TextContent("Show generated code|Show generated code of a surface shader");
            public static Texture2D warningIcon = EditorGUIUtility.LoadIcon("console.warnicon.sml");
        }
    }
}

