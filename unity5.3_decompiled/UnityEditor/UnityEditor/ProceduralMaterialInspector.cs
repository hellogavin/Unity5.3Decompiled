namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(ProceduralMaterial))]
    internal class ProceduralMaterialInspector : MaterialEditor
    {
        private static string[] kMaxLoadBehaviorStrings = new string[] { "Do nothing", "Do nothing and cache", "Build on level load", "Build on level load and cache", "Bake and keep Substance", "Bake and discard Substance" };
        private static int[] kMaxLoadBehaviorValues = new int[] { 0, 5, 1, 4, 2, 3 };
        private static string[] kMaxTextureSizeStrings = new string[] { "32", "64", "128", "256", "512", "1024", "2048" };
        private static int[] kMaxTextureSizeValues = new int[] { 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800 };
        private static string[] kTextureFormatStrings = new string[] { "Compressed", "Compressed - No Alpha", "RAW", "RAW - No Alpha" };
        private static int[] kTextureFormatValues;
        private bool m_AllowTextureSizeModification;
        private static Dictionary<ProceduralMaterial, float> m_GeneratingSince;
        private static SubstanceImporter m_Importer = null;
        private string m_LastGroup;
        private static ProceduralMaterial m_Material = null;
        private bool m_MightHaveModified;
        protected List<ProceduralPlatformSetting> m_PlatformSettings;
        private bool m_ReimportOnDisable = true;
        private Vector2 m_ScrollPos = new Vector2();
        private static Shader m_ShaderPMaterial = null;
        private bool m_ShowHSLInputs = true;
        private bool m_ShowTexturesSection;
        private Styles m_Styles;
        private static bool m_UndoWasPerformed;

        static ProceduralMaterialInspector()
        {
            int[] numArray1 = new int[4];
            numArray1[1] = 2;
            numArray1[2] = 1;
            numArray1[3] = 3;
            kTextureFormatValues = numArray1;
            m_UndoWasPerformed = false;
            m_GeneratingSince = new Dictionary<ProceduralMaterial, float>();
        }

        internal void Apply()
        {
            foreach (ProceduralPlatformSetting setting in this.m_PlatformSettings)
            {
                setting.Apply();
            }
        }

        public override void Awake()
        {
            base.Awake();
            this.m_ShowTexturesSection = EditorPrefs.GetBool("ProceduralShowTextures", false);
            this.m_ReimportOnDisable = true;
            m_UndoWasPerformed = false;
        }

        public void BuildTargetList()
        {
            List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
            this.m_PlatformSettings = new List<ProceduralPlatformSetting>();
            this.m_PlatformSettings.Add(new ProceduralPlatformSetting(base.targets, string.Empty, BuildTarget.StandaloneWindows, null));
            foreach (BuildPlayerWindow.BuildPlatform platform in validPlatforms)
            {
                this.m_PlatformSettings.Add(new ProceduralPlatformSetting(base.targets, platform.name, platform.DefaultTarget, platform.smallIcon));
            }
        }

        public void DisableReimportOnDisable()
        {
            this.m_ReimportOnDisable = false;
        }

        internal void DisplayRestrictedInspector()
        {
            this.m_MightHaveModified = false;
            if (this.m_Styles == null)
            {
                this.m_Styles = new Styles();
            }
            ProceduralMaterial target = this.target as ProceduralMaterial;
            if (m_Material != target)
            {
                m_Material = target;
                m_ShaderPMaterial = target.shader;
            }
            this.ProceduralProperties();
            GUILayout.Space(15f);
            this.GeneratedTextures();
        }

        internal static void DoObjectPingField(Rect position, Rect dropRect, int id, Object obj, Type objType)
        {
            Event current = Event.current;
            EventType rawType = current.type;
            if ((!GUI.enabled && GUIClip.enabled) && (Event.current.rawType == EventType.MouseDown))
            {
                rawType = Event.current.rawType;
            }
            bool flag = EditorGUIUtility.HasObjectThumbnail(objType) && (position.height > 16f);
            switch (rawType)
            {
                case EventType.MouseDown:
                    if ((Event.current.button == 0) && position.Contains(Event.current.mousePosition))
                    {
                        Object gameObject = obj;
                        Component component = gameObject as Component;
                        if (component != null)
                        {
                            gameObject = component.gameObject;
                        }
                        if (Event.current.clickCount == 1)
                        {
                            GUIUtility.keyboardControl = id;
                            if (gameObject != null)
                            {
                                EditorGUIUtility.PingObject(gameObject);
                            }
                            current.Use();
                        }
                        else if (Event.current.clickCount == 2)
                        {
                            if (gameObject != null)
                            {
                                AssetDatabase.OpenAsset(gameObject);
                                GUIUtility.ExitGUI();
                            }
                            current.Use();
                        }
                    }
                    break;

                case EventType.Repaint:
                {
                    GUIContent content = EditorGUIUtility.ObjectContent(obj, objType);
                    if (flag)
                    {
                        GUIStyle objectFieldThumb = EditorStyles.objectFieldThumb;
                        objectFieldThumb.Draw(position, GUIContent.none, id, DragAndDrop.activeControlID == id);
                        if (obj != null)
                        {
                            EditorGUI.DrawPreviewTexture(objectFieldThumb.padding.Remove(position), content.image);
                        }
                        else
                        {
                            (objectFieldThumb.name + "Overlay").Draw(position, content, id);
                        }
                    }
                    else
                    {
                        EditorStyles.objectField.Draw(position, content, id, DragAndDrop.activeControlID == id);
                    }
                    break;
                }
            }
        }

        private static void ExportBitmaps(ProceduralMaterial material, bool alphaRemap)
        {
            m_Importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material)) as SubstanceImporter;
            m_Importer.ExportBitmaps(material, alphaRemap);
        }

        [MenuItem("CONTEXT/ProceduralMaterial/Export Bitmaps (remapped alpha channels)", false)]
        public static void ExportBitmapsAlphaRemap(MenuCommand command)
        {
            ExportBitmaps(command.context as ProceduralMaterial, true);
        }

        [MenuItem("CONTEXT/ProceduralMaterial/Export Bitmaps (original alpha channels)", false)]
        public static void ExportBitmapsNoAlphaRemap(MenuCommand command)
        {
            ExportBitmaps(command.context as ProceduralMaterial, false);
        }

        private void GeneratedTextures()
        {
            if (base.targets.Length <= 1)
            {
                foreach (ProceduralPropertyDescription description in m_Material.GetProceduralPropertyDescriptions())
                {
                    if (description.name == "$outputsize")
                    {
                        this.m_AllowTextureSizeModification = true;
                        break;
                    }
                }
                string content = "Generated Textures";
                if (ShowIsGenerating(this.target as ProceduralMaterial))
                {
                    content = content + " (Generating...)";
                }
                EditorGUI.BeginChangeCheck();
                this.m_ShowTexturesSection = EditorGUILayout.Foldout(this.m_ShowTexturesSection, content);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool("ProceduralShowTextures", this.m_ShowTexturesSection);
                }
                if (this.m_ShowTexturesSection)
                {
                    this.ShowProceduralTexturesGUI(m_Material);
                    this.ShowGeneratedTexturesGUI(m_Material);
                    if (m_Importer != null)
                    {
                        if (this.HasProceduralTextureProperties(m_Material))
                        {
                            this.OffsetScaleGUI(m_Material);
                        }
                        GUILayout.Space(5f);
                        EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                        this.ShowTextureSizeGUI();
                        EditorGUI.EndDisabledGroup();
                    }
                }
            }
        }

        public override string GetInfoString()
        {
            Texture[] generatedTextures = (this.target as ProceduralMaterial).GetGeneratedTextures();
            if (generatedTextures.Length == 0)
            {
                return string.Empty;
            }
            return (generatedTextures[0].width + "x" + generatedTextures[0].height);
        }

        internal bool HasModified()
        {
            foreach (ProceduralPlatformSetting setting in this.m_PlatformSettings)
            {
                if (setting.HasChanged())
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasProceduralTextureProperties(Material material)
        {
            Shader s = material.shader;
            int propertyCount = ShaderUtil.GetPropertyCount(s);
            for (int i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    string propertyName = ShaderUtil.GetPropertyName(s, i);
                    Texture tex = material.GetTexture(propertyName);
                    if (SubstanceImporter.IsProceduralTextureSlot(material, tex, propertyName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void InputGUI(ProceduralPropertyDescription input)
        {
            float num;
            ProceduralPropertyType type = input.type;
            GUIContent label = new GUIContent(input.label, input.name);
            switch (type)
            {
                case ProceduralPropertyType.Boolean:
                {
                    EditorGUI.BeginChangeCheck();
                    bool flag = EditorGUILayout.Toggle(label, m_Material.GetProceduralBoolean(input.name), new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                        m_Material.SetProceduralBoolean(input.name, flag);
                    }
                    return;
                }
                case ProceduralPropertyType.Float:
                {
                    EditorGUI.BeginChangeCheck();
                    if (!input.hasRange)
                    {
                        num = EditorGUILayout.FloatField(label, m_Material.GetProceduralFloat(input.name), new GUILayoutOption[0]);
                        break;
                    }
                    float minimum = input.minimum;
                    float maximum = input.maximum;
                    num = EditorGUILayout.Slider(label, m_Material.GetProceduralFloat(input.name), minimum, maximum, new GUILayoutOption[0]);
                    break;
                }
                case ProceduralPropertyType.Vector2:
                case ProceduralPropertyType.Vector3:
                case ProceduralPropertyType.Vector4:
                {
                    int num4 = (type != ProceduralPropertyType.Vector2) ? ((type != ProceduralPropertyType.Vector3) ? 4 : 3) : 2;
                    Vector4 proceduralVector = m_Material.GetProceduralVector(input.name);
                    EditorGUI.BeginChangeCheck();
                    if (!input.hasRange)
                    {
                        switch (num4)
                        {
                            case 2:
                                proceduralVector = EditorGUILayout.Vector2Field(input.name, (Vector2) proceduralVector, new GUILayoutOption[0]);
                                break;

                            case 3:
                                proceduralVector = EditorGUILayout.Vector3Field(input.name, (Vector3) proceduralVector, new GUILayoutOption[0]);
                                break;

                            case 4:
                                proceduralVector = EditorGUILayout.Vector4Field(input.name, proceduralVector, new GUILayoutOption[0]);
                                break;
                        }
                    }
                    else
                    {
                        float leftValue = input.minimum;
                        float rightValue = input.maximum;
                        EditorGUILayout.BeginVertical(new GUILayoutOption[0]);
                        EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        GUILayout.Space((float) (EditorGUI.indentLevel * 15));
                        GUILayout.Label(label, new GUILayoutOption[0]);
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.indentLevel++;
                        for (int i = 0; i < num4; i++)
                        {
                            proceduralVector[i] = EditorGUILayout.Slider(new GUIContent(input.componentLabels[i]), proceduralVector[i], leftValue, rightValue, new GUILayoutOption[0]);
                        }
                        EditorGUI.indentLevel--;
                        EditorGUILayout.EndVertical();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                        m_Material.SetProceduralVector(input.name, proceduralVector);
                    }
                    return;
                }
                case ProceduralPropertyType.Color3:
                case ProceduralPropertyType.Color4:
                {
                    EditorGUI.BeginChangeCheck();
                    Color color = EditorGUILayout.ColorField(label, m_Material.GetProceduralColor(input.name), new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                        m_Material.SetProceduralColor(input.name, color);
                    }
                    return;
                }
                case ProceduralPropertyType.Enum:
                {
                    GUIContent[] displayedOptions = new GUIContent[input.enumOptions.Length];
                    for (int j = 0; j < displayedOptions.Length; j++)
                    {
                        displayedOptions[j] = new GUIContent(input.enumOptions[j]);
                    }
                    EditorGUI.BeginChangeCheck();
                    int num9 = EditorGUILayout.Popup(label, m_Material.GetProceduralEnum(input.name), displayedOptions, new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                        m_Material.SetProceduralEnum(input.name, num9);
                    }
                    return;
                }
                case ProceduralPropertyType.Texture:
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Space((float) (EditorGUI.indentLevel * 15));
                    GUILayout.Label(label, new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
                    Rect position = GUILayoutUtility.GetRect((float) 64f, (float) 64f, options);
                    EditorGUI.BeginChangeCheck();
                    Texture2D textured = EditorGUI.DoObjectField(position, position, GUIUtility.GetControlID(0x3042, EditorGUIUtility.native, position), m_Material.GetProceduralTexture(input.name), typeof(Texture2D), null, null, false) as Texture2D;
                    EditorGUILayout.EndHorizontal();
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                        m_Material.SetProceduralTexture(input.name, textured);
                    }
                    return;
                }
                default:
                    return;
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.RecordForUndo(m_Material, m_Importer, "Modified property " + input.name + " for material " + m_Material.name);
                m_Material.SetProceduralFloat(input.name, num);
            }
        }

        private void InputHSLGUI(ProceduralPropertyDescription hInput, ProceduralPropertyDescription sInput, ProceduralPropertyDescription lInput)
        {
            GUILayout.Space(5f);
            this.m_ShowHSLInputs = EditorPrefs.GetBool("ProceduralShowHSL", true);
            EditorGUI.BeginChangeCheck();
            this.m_ShowHSLInputs = EditorGUILayout.Foldout(this.m_ShowHSLInputs, this.m_Styles.hslContent);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("ProceduralShowHSL", this.m_ShowHSLInputs);
            }
            if (this.m_ShowHSLInputs)
            {
                EditorGUI.indentLevel++;
                this.InputGUI(hInput);
                this.InputGUI(sInput);
                this.InputGUI(lInput);
                EditorGUI.indentLevel--;
            }
        }

        protected void InputOptions(ProceduralMaterial material)
        {
            EditorGUI.BeginChangeCheck();
            this.InputsGUI();
            if (EditorGUI.EndChangeCheck())
            {
                material.RebuildTextures();
            }
        }

        private void InputSeedGUI(ProceduralPropertyDescription input)
        {
            Rect controlRect = EditorGUILayout.GetControlRect(new GUILayoutOption[0]);
            EditorGUI.BeginChangeCheck();
            float num = this.RandomIntField(controlRect, this.m_Styles.randomSeedContent, (int) m_Material.GetProceduralFloat(input.name), 0, 0x270f);
            if (EditorGUI.EndChangeCheck())
            {
                this.RecordForUndo(m_Material, m_Importer, "Modified random seed for material " + m_Material.name);
                m_Material.SetProceduralFloat(input.name, num);
            }
        }

        public void InputsGUI()
        {
            List<ProceduralPropertyDescription> list2;
            List<ProceduralPropertyDescription> list5;
            List<string> list = new List<string>();
            Dictionary<string, List<ProceduralPropertyDescription>> dictionary = new Dictionary<string, List<ProceduralPropertyDescription>>();
            Dictionary<string, List<ProceduralPropertyDescription>> dictionary2 = new Dictionary<string, List<ProceduralPropertyDescription>>();
            ProceduralPropertyDescription[] proceduralPropertyDescriptions = m_Material.GetProceduralPropertyDescriptions();
            ProceduralPropertyDescription hInput = null;
            ProceduralPropertyDescription sInput = null;
            ProceduralPropertyDescription lInput = null;
            foreach (ProceduralPropertyDescription description4 in proceduralPropertyDescriptions)
            {
                if (description4.name == "$randomseed")
                {
                    this.InputSeedGUI(description4);
                }
                else if (((description4.name.Length <= 0) || (description4.name[0] != '$')) && m_Material.IsProceduralPropertyVisible(description4.name))
                {
                    string group = description4.group;
                    if ((group != string.Empty) && !list.Contains(group))
                    {
                        list.Add(group);
                    }
                    if (((description4.name == "Hue_Shift") && (description4.type == ProceduralPropertyType.Float)) && (group == string.Empty))
                    {
                        hInput = description4;
                    }
                    if (((description4.name == "Saturation") && (description4.type == ProceduralPropertyType.Float)) && (group == string.Empty))
                    {
                        sInput = description4;
                    }
                    if (((description4.name == "Luminosity") && (description4.type == ProceduralPropertyType.Float)) && (group == string.Empty))
                    {
                        lInput = description4;
                    }
                    if (description4.type == ProceduralPropertyType.Texture)
                    {
                        if (!dictionary2.ContainsKey(group))
                        {
                            dictionary2.Add(group, new List<ProceduralPropertyDescription>());
                        }
                        dictionary2[group].Add(description4);
                    }
                    else
                    {
                        if (!dictionary.ContainsKey(group))
                        {
                            dictionary.Add(group, new List<ProceduralPropertyDescription>());
                        }
                        dictionary[group].Add(description4);
                    }
                }
            }
            bool flag = false;
            if (((hInput != null) && (sInput != null)) && (lInput != null))
            {
                flag = true;
            }
            if (dictionary.TryGetValue(string.Empty, out list2))
            {
                foreach (ProceduralPropertyDescription description5 in list2)
                {
                    if (!flag || (((description5 != hInput) && (description5 != sInput)) && (description5 != lInput)))
                    {
                        this.InputGUI(description5);
                    }
                }
            }
            foreach (string str2 in list)
            {
                ProceduralMaterial target = this.target as ProceduralMaterial;
                string key = target.name + str2;
                GUILayout.Space(5f);
                bool @bool = EditorPrefs.GetBool(key, true);
                EditorGUI.BeginChangeCheck();
                @bool = EditorGUILayout.Foldout(@bool, str2);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(key, @bool);
                }
                if (@bool)
                {
                    List<ProceduralPropertyDescription> list3;
                    List<ProceduralPropertyDescription> list4;
                    EditorGUI.indentLevel++;
                    if (dictionary.TryGetValue(str2, out list3))
                    {
                        foreach (ProceduralPropertyDescription description6 in list3)
                        {
                            this.InputGUI(description6);
                        }
                    }
                    if (dictionary2.TryGetValue(str2, out list4))
                    {
                        GUILayout.Space(2f);
                        foreach (ProceduralPropertyDescription description7 in list4)
                        {
                            this.InputGUI(description7);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            if (flag)
            {
                this.InputHSLGUI(hInput, sInput, lInput);
            }
            if (dictionary2.TryGetValue(string.Empty, out list5))
            {
                GUILayout.Space(5f);
                foreach (ProceduralPropertyDescription description8 in list5)
                {
                    this.InputGUI(description8);
                }
            }
        }

        internal override bool IsEnabled()
        {
            return base.IsOpenForEdit();
        }

        protected void OffsetScaleGUI(ProceduralMaterial material)
        {
            if ((m_Importer != null) && (base.targets.Length <= 1))
            {
                Vector2 materialScale = m_Importer.GetMaterialScale(material);
                Vector2 materialOffset = m_Importer.GetMaterialOffset(material);
                Vector4 scaleOffset = new Vector4(materialScale.x, materialScale.y, materialOffset.x, materialOffset.y);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.Space(10f);
                Rect position = GUILayoutUtility.GetRect(100f, 10000f, (float) 32f, (float) 32f);
                GUILayout.EndHorizontal();
                EditorGUI.BeginChangeCheck();
                scaleOffset = MaterialEditor.TextureScaleOffsetProperty(position, scaleOffset);
                if (EditorGUI.EndChangeCheck())
                {
                    this.RecordForUndo(material, m_Importer, "Modify " + material.name + "'s Tiling/Offset");
                    m_Importer.SetMaterialScale(material, new Vector2(scaleOffset.x, scaleOffset.y));
                    m_Importer.SetMaterialOffset(material, new Vector2(scaleOffset.z, scaleOffset.w));
                }
            }
        }

        internal override void OnAssetStoreInspectorGUI()
        {
            this.DisplayRestrictedInspector();
        }

        public override void OnDisable()
        {
            ProceduralMaterial target = this.target as ProceduralMaterial;
            if (((target != null) && (this.m_PlatformSettings != null)) && this.HasModified())
            {
                string message = "Unapplied import settings for '" + AssetDatabase.GetAssetPath(this.target) + "'";
                if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Revert"))
                {
                    this.Apply();
                    this.ReimportSubstances();
                }
                this.ResetValues();
            }
            if (this.m_ReimportOnDisable)
            {
                this.ReimportSubstancesIfNeeded();
            }
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            base.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        internal override void OnHeaderTitleGUI(Rect titleRect, string header)
        {
            ProceduralMaterial target = this.target as ProceduralMaterial;
            m_Importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.target)) as SubstanceImporter;
            if (m_Importer != null)
            {
                string name = target.name;
                name = EditorGUI.DelayedTextField(titleRect, name, EditorStyles.textField);
                if (name != target.name)
                {
                    if (m_Importer.RenameMaterial(target, name))
                    {
                        AssetDatabase.ImportAsset(m_Importer.assetPath, ImportAssetOptions.ForceUncompressedImport);
                        GUIUtility.ExitGUI();
                    }
                    else
                    {
                        name = target.name;
                    }
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(AnimationMode.InAnimationMode());
            this.m_MightHaveModified = true;
            if (this.m_Styles == null)
            {
                this.m_Styles = new Styles();
            }
            ProceduralMaterial target = this.target as ProceduralMaterial;
            m_Importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.target)) as SubstanceImporter;
            if (m_Importer == null)
            {
                this.DisplayRestrictedInspector();
            }
            else
            {
                if (m_Material != target)
                {
                    m_Material = target;
                    m_ShaderPMaterial = target.shader;
                }
                if (base.isVisible && (target.shader != null))
                {
                    if (m_ShaderPMaterial != target.shader)
                    {
                        m_ShaderPMaterial = target.shader;
                        foreach (ProceduralMaterial material2 in base.targets)
                        {
                            (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material2)) as SubstanceImporter).OnShaderModified(material2);
                        }
                    }
                    if (base.PropertiesGUI())
                    {
                        m_ShaderPMaterial = target.shader;
                        foreach (ProceduralMaterial material3 in base.targets)
                        {
                            (AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material3)) as SubstanceImporter).OnShaderModified(material3);
                        }
                        base.PropertiesChanged();
                    }
                    GUILayout.Space(5f);
                    this.ProceduralProperties();
                    GUILayout.Space(15f);
                    this.GeneratedTextures();
                    EditorGUI.EndDisabledGroup();
                }
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            base.OnPreviewGUI(r, background);
            if (ShowIsGenerating(this.target as ProceduralMaterial) && (r.width > 50f))
            {
                EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Generating...");
            }
        }

        private void ProceduralProperties()
        {
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
            GUILayout.Label("Procedural Properties", EditorStyles.boldLabel, options);
            foreach (ProceduralMaterial material in base.targets)
            {
                if (material.isProcessing)
                {
                    base.Repaint();
                    SceneView.RepaintAll();
                    GameView.RepaintAll();
                    break;
                }
            }
            if (base.targets.Length > 1)
            {
                GUILayout.Label("Procedural properties do not support multi-editing.", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUIUtility.labelWidth = 0f;
                EditorGUIUtility.fieldWidth = 0f;
                if (m_Importer != null)
                {
                    if (!ProceduralMaterial.isSupported)
                    {
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                        GUILayout.Label("Procedural Materials are not supported on " + EditorUserBuildSettings.activeBuildTarget + ". Textures will be baked.", EditorStyles.helpBox, optionArray2);
                    }
                    bool changed = GUI.changed;
                    EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                    EditorGUI.BeginChangeCheck();
                    bool generated = EditorGUILayout.Toggle(this.m_Styles.generateAllOutputsContent, m_Importer.GetGenerateAllOutputs(m_Material), new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_Importer.SetGenerateAllOutputs(m_Material, generated);
                    }
                    EditorGUI.BeginChangeCheck();
                    bool mode = EditorGUILayout.Toggle(this.m_Styles.mipmapContent, m_Importer.GetGenerateMipMaps(m_Material), new GUILayoutOption[0]);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_Importer.SetGenerateMipMaps(m_Material, mode);
                    }
                    EditorGUI.EndDisabledGroup();
                    if (m_Material.HasProceduralProperty("$time"))
                    {
                        EditorGUI.BeginChangeCheck();
                        int num2 = EditorGUILayout.IntField(this.m_Styles.animatedContent, m_Importer.GetAnimationUpdateRate(m_Material), new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            m_Importer.SetAnimationUpdateRate(m_Material, num2);
                        }
                    }
                    GUI.changed = changed;
                }
                this.InputOptions(m_Material);
            }
        }

        internal int RandomIntField(Rect position, int val, int min, int max)
        {
            position.width = (position.width - EditorGUIUtility.fieldWidth) - 5f;
            if (GUI.Button(position, this.m_Styles.randomizeButtonContent, EditorStyles.miniButton))
            {
                val = Random.Range(min, max + 1);
            }
            position.x += position.width + 5f;
            position.width = EditorGUIUtility.fieldWidth;
            val = Mathf.Clamp(EditorGUI.IntField(position, val), min, max);
            return val;
        }

        internal int RandomIntField(Rect position, GUIContent label, int val, int min, int max)
        {
            position = EditorGUI.PrefixLabel(position, 0, label);
            return this.RandomIntField(position, val, min, max);
        }

        protected void RecordForUndo(ProceduralMaterial material, SubstanceImporter importer, string message)
        {
            if (importer != null)
            {
                Object[] objectsToUndo = new Object[] { material, importer };
                Undo.RecordObjects(objectsToUndo, message);
            }
            else
            {
                Undo.RecordObject(material, message);
            }
        }

        public void ReimportSubstances()
        {
            string[] strArray = new string[base.targets.GetLength(0)];
            int num = 0;
            foreach (ProceduralMaterial material in base.targets)
            {
                strArray[num++] = AssetDatabase.GetAssetPath(material);
            }
            for (int i = 0; i < num; i++)
            {
                SubstanceImporter atPath = AssetImporter.GetAtPath(strArray[i]) as SubstanceImporter;
                if ((atPath != null) && EditorUtility.IsDirty(atPath.GetInstanceID()))
                {
                    AssetDatabase.ImportAsset(strArray[i], ImportAssetOptions.ForceUncompressedImport);
                }
            }
        }

        public void ReimportSubstancesIfNeeded()
        {
            if ((this.m_MightHaveModified && !m_UndoWasPerformed) && (!EditorApplication.isPlaying && !InternalEditorUtility.ignoreInspectorChanges))
            {
                this.ReimportSubstances();
            }
        }

        [MenuItem("CONTEXT/ProceduralMaterial/Reset", false, -100)]
        public static void ResetSubstance(MenuCommand command)
        {
            m_Importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(command.context)) as SubstanceImporter;
            m_Importer.ResetMaterial(command.context as ProceduralMaterial);
        }

        internal void ResetValues()
        {
            this.BuildTargetList();
            if (this.HasModified())
            {
                Debug.LogError("Impossible");
            }
        }

        private void ShowAlphaSourceGUI(ProceduralMaterial material, ProceduralTexture tex, ref Rect rect)
        {
            GUIStyle style = "ObjectPickerResultsGridLabel";
            float num = 10f;
            rect.y = rect.yMax + 2f;
            if (m_Importer != null)
            {
                EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
                if ((tex.GetProceduralOutputType() != ProceduralOutputType.Normal) && tex.hasAlpha)
                {
                    rect.height = style.fixedHeight;
                    string[] displayedOptions = new string[] { "Source (A)", "Diffuse (A)", "Normal (A)", "Height (A)", "Emissive (A)", "Specular (A)", "Opacity (A)", "Smoothness (A)", "Amb. Occlusion (A)", "Detail Mask (A)", "Metallic (A)", "Roughness (A)" };
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    int num2 = EditorGUI.Popup(rect, (int) m_Importer.GetTextureAlphaSource(material, tex.name), displayedOptions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.RecordForUndo(material, m_Importer, "Modify " + material.name + "'s Alpha Modifier");
                        m_Importer.SetTextureAlphaSource(material, tex.name, (ProceduralOutputType) num2);
                    }
                    rect.y = rect.yMax + 2f;
                }
                EditorGUI.EndDisabledGroup();
            }
            rect.width += num;
        }

        protected void ShowGeneratedTexturesGUI(ProceduralMaterial material)
        {
            if ((base.targets.Length <= 1) && ((m_Importer == null) || m_Importer.GetGenerateAllOutputs(m_Material)))
            {
                GUIStyle style = "ObjectPickerResultsGridLabel";
                EditorGUILayout.Space();
                GUILayout.FlexibleSpace();
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(((64f + style.fixedHeight) + style.fixedHeight) + 16f) };
                this.m_ScrollPos = EditorGUILayout.BeginScrollView(this.m_ScrollPos, options);
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                float pixels = 10f;
                foreach (Texture texture in material.GetGeneratedTextures())
                {
                    ProceduralTexture texture2 = texture as ProceduralTexture;
                    if (texture2 != null)
                    {
                        GUILayout.Space(pixels);
                        GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Height((64f + style.fixedHeight) + 8f) };
                        GUILayout.BeginVertical(optionArray2);
                        Rect position = GUILayoutUtility.GetRect((float) 64f, (float) 64f);
                        DoObjectPingField(position, position, GUIUtility.GetControlID(0x3042, EditorGUIUtility.native, position), texture2, typeof(Texture));
                        this.ShowAlphaSourceGUI(material, texture2, ref position);
                        GUILayout.EndVertical();
                        GUILayout.Space(pixels);
                        GUILayout.FlexibleSpace();
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
        }

        public static bool ShowIsGenerating(ProceduralMaterial mat)
        {
            if (!m_GeneratingSince.ContainsKey(mat))
            {
                m_GeneratingSince[mat] = 0f;
            }
            if (mat.isProcessing)
            {
                return (Time.realtimeSinceStartup > (m_GeneratingSince[mat] + 0.4f));
            }
            m_GeneratingSince[mat] = Time.realtimeSinceStartup;
            return false;
        }

        protected void ShowProceduralTexturesGUI(ProceduralMaterial material)
        {
            if (base.targets.Length <= 1)
            {
                EditorGUILayout.Space();
                Shader s = material.shader;
                if (s != null)
                {
                    EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.Space(4f);
                    GUILayout.FlexibleSpace();
                    float pixels = 10f;
                    bool flag = true;
                    for (int i = 0; i < ShaderUtil.GetPropertyCount(s); i++)
                    {
                        if (ShaderUtil.GetPropertyType(s, i) != ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            continue;
                        }
                        string propertyName = ShaderUtil.GetPropertyName(s, i);
                        Texture tex = material.GetTexture(propertyName);
                        if (SubstanceImporter.IsProceduralTextureSlot(material, tex, propertyName))
                        {
                            Type type;
                            string propertyDescription = ShaderUtil.GetPropertyDescription(s, i);
                            switch (ShaderUtil.GetTexDim(s, i))
                            {
                                case ShaderUtil.ShaderPropertyTexDim.TexDim2D:
                                    type = typeof(Texture);
                                    break;

                                case ShaderUtil.ShaderPropertyTexDim.TexDim3D:
                                    type = typeof(Texture3D);
                                    break;

                                case ShaderUtil.ShaderPropertyTexDim.TexDimCUBE:
                                    type = typeof(Cubemap);
                                    break;

                                case ShaderUtil.ShaderPropertyTexDim.TexDimAny:
                                    type = typeof(Texture);
                                    break;

                                default:
                                    type = null;
                                    break;
                            }
                            GUIStyle style = "ObjectPickerResultsGridLabel";
                            if (flag)
                            {
                                flag = false;
                            }
                            else
                            {
                                GUILayout.Space(pixels);
                            }
                            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(((72f + style.fixedHeight) + style.fixedHeight) + 8f) };
                            GUILayout.BeginVertical(options);
                            Rect position = GUILayoutUtility.GetRect((float) 72f, (float) 72f);
                            DoObjectPingField(position, position, GUIUtility.GetControlID(0x3042, EditorGUIUtility.native, position), tex, type);
                            this.ShowAlphaSourceGUI(material, tex as ProceduralTexture, ref position);
                            position.height = style.fixedHeight;
                            GUI.Label(position, propertyDescription, style);
                            GUILayout.EndVertical();
                            GUILayout.FlexibleSpace();
                        }
                    }
                    GUILayout.Space(4f);
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public void ShowTextureSizeGUI()
        {
            if (this.m_PlatformSettings == null)
            {
                this.BuildTargetList();
            }
            this.TextureSizeGUI();
        }

        protected void TextureSizeGUI()
        {
            int num = EditorGUILayout.BeginPlatformGrouping(BuildPlayerWindow.GetValidPlatforms().ToArray(), this.m_Styles.defaultPlatform);
            ProceduralPlatformSetting setting = this.m_PlatformSettings[num + 1];
            ProceduralPlatformSetting setting2 = setting;
            bool flag = true;
            if (setting.name != string.Empty)
            {
                EditorGUI.BeginChangeCheck();
                flag = GUILayout.Toggle(setting.overridden, "Override for " + setting.name, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (flag)
                    {
                        setting.SetOverride(this.m_PlatformSettings[0]);
                    }
                    else
                    {
                        setting.ClearOverride(this.m_PlatformSettings[0]);
                    }
                }
            }
            EditorGUI.BeginDisabledGroup(!flag);
            if (!this.m_AllowTextureSizeModification)
            {
                GUILayout.Label("This ProceduralMaterial was published with a fixed size.", EditorStyles.wordWrappedLabel, new GUILayoutOption[0]);
            }
            EditorGUI.BeginDisabledGroup(!this.m_AllowTextureSizeModification);
            EditorGUI.BeginChangeCheck();
            setting2.maxTextureWidth = EditorGUILayout.IntPopup(this.m_Styles.targetWidth.text, setting2.maxTextureWidth, kMaxTextureSizeStrings, kMaxTextureSizeValues, new GUILayoutOption[0]);
            setting2.maxTextureHeight = EditorGUILayout.IntPopup(this.m_Styles.targetHeight.text, setting2.maxTextureHeight, kMaxTextureSizeStrings, kMaxTextureSizeValues, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && setting2.isDefault)
            {
                foreach (ProceduralPlatformSetting setting3 in this.m_PlatformSettings)
                {
                    if (!setting3.isDefault && !setting3.overridden)
                    {
                        setting3.maxTextureWidth = setting2.maxTextureWidth;
                        setting3.maxTextureHeight = setting2.maxTextureHeight;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginChangeCheck();
            int textureFormat = setting2.textureFormat;
            if ((textureFormat < 0) || (textureFormat >= kTextureFormatStrings.Length))
            {
                Debug.LogError("Invalid TextureFormat");
            }
            textureFormat = EditorGUILayout.IntPopup(this.m_Styles.textureFormat.text, textureFormat, kTextureFormatStrings, kTextureFormatValues, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                setting2.textureFormat = textureFormat;
                if (setting2.isDefault)
                {
                    foreach (ProceduralPlatformSetting setting4 in this.m_PlatformSettings)
                    {
                        if (!setting4.isDefault && !setting4.overridden)
                        {
                            setting4.textureFormat = setting2.textureFormat;
                        }
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            setting2.m_LoadBehavior = EditorGUILayout.IntPopup(this.m_Styles.loadBehavior.text, setting2.m_LoadBehavior, kMaxLoadBehaviorStrings, kMaxLoadBehaviorValues, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck() && setting2.isDefault)
            {
                foreach (ProceduralPlatformSetting setting5 in this.m_PlatformSettings)
                {
                    if (!setting5.isDefault && !setting5.overridden)
                    {
                        setting5.m_LoadBehavior = setting2.m_LoadBehavior;
                    }
                }
            }
            GUILayout.Space(5f);
            EditorGUI.BeginDisabledGroup(!this.HasModified());
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Revert", new GUILayoutOption[0]))
            {
                this.ResetValues();
            }
            if (GUILayout.Button("Apply", new GUILayoutOption[0]))
            {
                this.Apply();
                this.ReimportSubstances();
                this.ResetValues();
            }
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            GUILayout.Space(5f);
            EditorGUILayout.EndPlatformGrouping();
            EditorGUI.EndDisabledGroup();
        }

        private Object TextureValidator(Object[] references, Type objType, SerializedProperty property)
        {
            foreach (Object obj2 in references)
            {
                Texture texture = obj2 as Texture;
                if (texture != null)
                {
                    return texture;
                }
            }
            return null;
        }

        public override void UndoRedoPerformed()
        {
            m_UndoWasPerformed = true;
            base.UndoRedoPerformed();
            if (m_Material != null)
            {
                m_Material.RebuildTextures();
            }
            base.Repaint();
        }

        [Serializable]
        protected class ProceduralPlatformSetting
        {
            public Texture2D icon;
            public int m_LoadBehavior;
            public bool m_Overridden;
            public int m_TextureFormat;
            public int maxTextureHeight;
            public int maxTextureWidth;
            public string name;
            public BuildTarget target;
            private Object[] targets;

            public ProceduralPlatformSetting(Object[] objects, string _name, BuildTarget _target, Texture2D _icon)
            {
                this.targets = objects;
                this.m_Overridden = false;
                this.target = _target;
                this.name = _name;
                this.icon = _icon;
                this.m_Overridden = false;
                if (this.name != string.Empty)
                {
                    foreach (ProceduralMaterial material in this.targets)
                    {
                        SubstanceImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material)) as SubstanceImporter;
                        if ((atPath != null) && atPath.GetPlatformTextureSettings(material.name, this.name, out this.maxTextureWidth, out this.maxTextureHeight, out this.m_TextureFormat, out this.m_LoadBehavior))
                        {
                            this.m_Overridden = true;
                            break;
                        }
                    }
                }
                if (!this.m_Overridden && (this.targets.Length > 0))
                {
                    SubstanceImporter importer2 = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.targets[0])) as SubstanceImporter;
                    if (importer2 != null)
                    {
                        importer2.GetPlatformTextureSettings((this.targets[0] as ProceduralMaterial).name, string.Empty, out this.maxTextureWidth, out this.maxTextureHeight, out this.m_TextureFormat, out this.m_LoadBehavior);
                    }
                }
            }

            public void Apply()
            {
                foreach (ProceduralMaterial material in this.targets)
                {
                    SubstanceImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(material)) as SubstanceImporter;
                    if (this.name != string.Empty)
                    {
                        if (this.m_Overridden)
                        {
                            atPath.SetPlatformTextureSettings(material, this.name, this.maxTextureWidth, this.maxTextureHeight, this.m_TextureFormat, this.m_LoadBehavior);
                        }
                        else
                        {
                            atPath.ClearPlatformTextureSettings(material.name, this.name);
                        }
                    }
                    else
                    {
                        atPath.SetPlatformTextureSettings(material, this.name, this.maxTextureWidth, this.maxTextureHeight, this.m_TextureFormat, this.m_LoadBehavior);
                    }
                }
            }

            public void ClearOverride(ProceduralMaterialInspector.ProceduralPlatformSetting master)
            {
                this.m_TextureFormat = master.textureFormat;
                this.maxTextureWidth = master.maxTextureWidth;
                this.maxTextureHeight = master.maxTextureHeight;
                this.m_LoadBehavior = master.m_LoadBehavior;
                this.m_Overridden = false;
            }

            public bool HasChanged()
            {
                ProceduralMaterialInspector.ProceduralPlatformSetting setting = new ProceduralMaterialInspector.ProceduralPlatformSetting(this.targets, this.name, this.target, null);
                return ((((setting.m_Overridden != this.m_Overridden) || (setting.maxTextureWidth != this.maxTextureWidth)) || ((setting.maxTextureHeight != this.maxTextureHeight) || (setting.textureFormat != this.textureFormat))) || (setting.m_LoadBehavior != this.m_LoadBehavior));
            }

            public void SetOverride(ProceduralMaterialInspector.ProceduralPlatformSetting master)
            {
                this.m_Overridden = true;
            }

            public bool isDefault
            {
                get
                {
                    return (this.name == string.Empty);
                }
            }

            public bool overridden
            {
                get
                {
                    return this.m_Overridden;
                }
            }

            public int textureFormat
            {
                get
                {
                    return this.m_TextureFormat;
                }
                set
                {
                    this.m_TextureFormat = value;
                }
            }
        }

        private class Styles
        {
            public GUIContent animatedContent = new GUIContent("Animation update rate", "Set the animation update rate in millisecond");
            public GUIContent defaultPlatform = EditorGUIUtility.TextContent("Default");
            public GUIContent generateAllOutputsContent = new GUIContent("Generate all outputs", "Force the generation of all Substance outputs.");
            public GUIContent hslContent = new GUIContent("HSL Adjustment", "Hue_Shift, Saturation, Luminosity");
            public GUIContent loadBehavior = new GUIContent("Load Behavior");
            public GUIContent mipmapContent = new GUIContent("Generate Mip Maps");
            public GUIContent randomizeButtonContent = new GUIContent("Randomize");
            public GUIContent randomSeedContent = new GUIContent("Random Seed", "$randomseed : the overall random aspect of the texture.");
            public GUIContent targetHeight = new GUIContent("Target Height");
            public GUIContent targetWidth = new GUIContent("Target Width");
            public GUIContent textureFormat = EditorGUIUtility.TextContent("Format");
        }
    }
}

