namespace UnityEditor
{
    using System;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    [CustomEditor(typeof(TrueTypeFontImporter)), CanEditMultipleObjects]
    internal class TrueTypeFontImporterInspector : AssetImporterInspector
    {
        private static GUIContent[] kCharacterStrings = new GUIContent[] { new GUIContent("Dynamic"), new GUIContent("Unicode"), new GUIContent("ASCII default set"), new GUIContent("ASCII upper case"), new GUIContent("ASCII lower case"), new GUIContent("Custom set") };
        private static int[] kCharacterValues = new int[] { -2, -1, 0, 1, 2, 3 };
        private static GUIContent[] kRenderingModeStrings = new GUIContent[] { new GUIContent("Smooth"), new GUIContent("Hinted Smooth"), new GUIContent("Hinted Raster"), new GUIContent("OS Default") };
        private static int[] kRenderingModeValues;
        private SerializedProperty m_CustomCharacters;
        private string m_DefaultFontNamesString = string.Empty;
        private SerializedProperty m_FallbackFontReferencesArraySize;
        private SerializedProperty m_FontNamesArraySize;
        private string m_FontNamesString = string.Empty;
        private SerializedProperty m_FontRenderingMode;
        private SerializedProperty m_FontSize;
        private bool? m_FormatSupported;
        private SerializedProperty m_IncludeFontData;
        private bool m_ReferencesExpanded;
        private SerializedProperty m_TextureCase;

        static TrueTypeFontImporterInspector()
        {
            int[] numArray1 = new int[4];
            numArray1[1] = 1;
            numArray1[2] = 2;
            numArray1[3] = 3;
            kRenderingModeValues = numArray1;
        }

        [MenuItem("CONTEXT/TrueTypeFontImporter/Create Editable Copy")]
        private static void CreateEditableCopy(MenuCommand command)
        {
            TrueTypeFontImporter context = command.context as TrueTypeFontImporter;
            if (context.fontTextureCase == FontTextureCase.Dynamic)
            {
                EditorUtility.DisplayDialog("Cannot generate editabled font asset for dynamic fonts", "Please reimport the font in a different mode.", "Ok");
            }
            else
            {
                string str = Path.GetDirectoryName(context.assetPath) + "/" + Path.GetFileNameWithoutExtension(context.assetPath);
                EditorGUIUtility.PingObject(context.GenerateEditableFont(GetUniquePath(str + "_copy", "fontsettings")));
            }
        }

        private string GetDefaultFontNames()
        {
            TrueTypeFontImporter target = this.target as TrueTypeFontImporter;
            return target.fontTTFName;
        }

        private string GetFontNames()
        {
            TrueTypeFontImporter target = this.target as TrueTypeFontImporter;
            string defaultFontNamesString = string.Empty;
            string[] fontNames = target.fontNames;
            for (int i = 0; i < fontNames.Length; i++)
            {
                defaultFontNamesString = defaultFontNamesString + fontNames[i];
                if (i < (fontNames.Length - 1))
                {
                    defaultFontNamesString = defaultFontNamesString + ", ";
                }
            }
            if (defaultFontNamesString == string.Empty)
            {
                defaultFontNamesString = this.m_DefaultFontNamesString;
            }
            return defaultFontNamesString;
        }

        private static string GetUniquePath(string basePath, string extension)
        {
            for (int i = 0; i < 0x2710; i++)
            {
                string path = basePath + ((i != 0) ? (string.Empty + i) : string.Empty) + "." + extension;
                if (!File.Exists(path))
                {
                    return path;
                }
            }
            return string.Empty;
        }

        private void OnEnable()
        {
            this.m_FontSize = base.serializedObject.FindProperty("m_FontSize");
            this.m_TextureCase = base.serializedObject.FindProperty("m_ForceTextureCase");
            this.m_IncludeFontData = base.serializedObject.FindProperty("m_IncludeFontData");
            this.m_FontNamesArraySize = base.serializedObject.FindProperty("m_FontNames.Array.size");
            this.m_CustomCharacters = base.serializedObject.FindProperty("m_CustomCharacters");
            this.m_FontRenderingMode = base.serializedObject.FindProperty("m_FontRenderingMode");
            this.m_FallbackFontReferencesArraySize = base.serializedObject.FindProperty("m_FallbackFontReferences.Array.size");
            if (base.targets.Length == 1)
            {
                this.m_DefaultFontNamesString = this.GetDefaultFontNames();
                this.m_FontNamesString = this.GetFontNames();
                this.SetFontNames(this.m_FontNamesString);
            }
        }

        public override void OnInspectorGUI()
        {
            if (!this.m_FormatSupported.HasValue)
            {
                this.m_FormatSupported = true;
                foreach (Object obj2 in base.targets)
                {
                    TrueTypeFontImporter importer = obj2 as TrueTypeFontImporter;
                    if ((importer == null) || !importer.IsFormatSupported())
                    {
                        this.m_FormatSupported = false;
                    }
                }
            }
            if (this.m_FormatSupported == false)
            {
                this.ShowFormatUnsupportedGUI();
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_FontSize, new GUILayoutOption[0]);
                if (this.m_FontSize.intValue < 1)
                {
                    this.m_FontSize.intValue = 1;
                }
                if (this.m_FontSize.intValue > 500)
                {
                    this.m_FontSize.intValue = 500;
                }
                EditorGUILayout.IntPopup(this.m_FontRenderingMode, kRenderingModeStrings, kRenderingModeValues, new GUIContent("Rendering Mode"), new GUILayoutOption[0]);
                EditorGUILayout.IntPopup(this.m_TextureCase, kCharacterStrings, kCharacterValues, new GUIContent("Character"), new GUILayoutOption[0]);
                if (!this.m_TextureCase.hasMultipleDifferentValues)
                {
                    if (this.m_TextureCase.intValue != -2)
                    {
                        if (this.m_TextureCase.intValue == 3)
                        {
                            EditorGUI.BeginChangeCheck();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUILayout.PrefixLabel("Custom Chars");
                            EditorGUI.showMixedValue = this.m_CustomCharacters.hasMultipleDifferentValues;
                            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                            string source = EditorGUILayout.TextArea(this.m_CustomCharacters.stringValue, GUI.skin.textArea, options);
                            EditorGUI.showMixedValue = false;
                            GUILayout.EndHorizontal();
                            if (EditorGUI.EndChangeCheck())
                            {
                                source = new string(source.Distinct<char>().ToArray<char>());
                                source = source.Replace("\n", string.Empty).Replace("\r", string.Empty);
                                this.m_CustomCharacters.stringValue = source;
                            }
                        }
                    }
                    else
                    {
                        EditorGUILayout.PropertyField(this.m_IncludeFontData, new GUIContent("Incl. Font Data"), new GUILayoutOption[0]);
                        if (base.targets.Length == 1)
                        {
                            EditorGUI.BeginChangeCheck();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            EditorGUILayout.PrefixLabel("Font Names");
                            GUI.SetNextControlName("fontnames");
                            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MinHeight(32f) };
                            this.m_FontNamesString = EditorGUILayout.TextArea(this.m_FontNamesString, "TextArea", optionArray2);
                            GUILayout.EndHorizontal();
                            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                            GUILayout.FlexibleSpace();
                            EditorGUI.BeginDisabledGroup(this.m_FontNamesString == this.m_DefaultFontNamesString);
                            if (GUILayout.Button("Reset", "MiniButton", new GUILayoutOption[0]))
                            {
                                GUI.changed = true;
                                if (GUI.GetNameOfFocusedControl() == "fontnames")
                                {
                                    GUIUtility.keyboardControl = 0;
                                }
                                this.m_FontNamesString = this.m_DefaultFontNamesString;
                            }
                            EditorGUI.EndDisabledGroup();
                            GUILayout.EndHorizontal();
                            if (EditorGUI.EndChangeCheck())
                            {
                                this.SetFontNames(this.m_FontNamesString);
                            }
                            this.m_ReferencesExpanded = EditorGUILayout.Foldout(this.m_ReferencesExpanded, "References to other fonts in project");
                            if (this.m_ReferencesExpanded)
                            {
                                EditorGUILayout.HelpBox("These are automatically generated by the inspector if any of the font names you supplied match fonts present in your project, which will then be used as fallbacks for this font.", MessageType.Info);
                                if (this.m_FallbackFontReferencesArraySize.intValue > 0)
                                {
                                    SerializedProperty property = this.m_FallbackFontReferencesArraySize.Copy();
                                    while (property.NextVisible(true) && (property.depth == 1))
                                    {
                                        EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
                                    }
                                }
                                else
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    GUILayout.Label("No references to other fonts in project.", new GUILayoutOption[0]);
                                    EditorGUI.EndDisabledGroup();
                                }
                            }
                        }
                    }
                }
                base.ApplyRevertGUI();
            }
        }

        private void SetFontNames(string fontNames)
        {
            string[] strArray;
            if (fontNames == this.m_DefaultFontNamesString)
            {
                strArray = new string[0];
            }
            else
            {
                char[] separator = new char[] { ',' };
                strArray = fontNames.Split(separator);
                for (int k = 0; k < strArray.Length; k++)
                {
                    strArray[k] = strArray[k].Trim();
                }
            }
            this.m_FontNamesArraySize.intValue = strArray.Length;
            SerializedProperty property = this.m_FontNamesArraySize.Copy();
            for (int i = 0; i < strArray.Length; i++)
            {
                property.Next(false);
                property.stringValue = strArray[i];
            }
            Font[] fallbackFontReferences = (this.target as TrueTypeFontImporter).LookupFallbackFontReferences(strArray);
            this.m_FallbackFontReferencesArraySize.intValue = fallbackFontReferences.Length;
            SerializedProperty property2 = this.m_FallbackFontReferencesArraySize.Copy();
            for (int j = 0; j < fallbackFontReferences.Length; j++)
            {
                property2.Next(false);
                property2.objectReferenceValue = fallbackFontReferences[j];
            }
        }

        private void ShowFormatUnsupportedGUI()
        {
            GUILayout.Space(5f);
            EditorGUILayout.HelpBox("Format of selected font is not supported by Unity.", MessageType.Warning);
        }
    }
}

