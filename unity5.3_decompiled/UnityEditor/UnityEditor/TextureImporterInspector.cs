namespace UnityEditor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(TextureImporter))]
    internal class TextureImporterInspector : AssetImporterInspector
    {
        [CompilerGenerated]
        private static Func<GUIContent, string> <>f__am$cache47;
        private static readonly TextureImporterFormat[] kFormatsWithCompressionSettings = new TextureImporterFormat[] { 
            TextureImporterFormat.DXT1Crunched, TextureImporterFormat.DXT5Crunched, TextureImporterFormat.PVRTC_RGB2, TextureImporterFormat.PVRTC_RGB4, TextureImporterFormat.PVRTC_RGBA2, TextureImporterFormat.PVRTC_RGBA4, TextureImporterFormat.ATC_RGB4, TextureImporterFormat.ATC_RGBA8, TextureImporterFormat.ETC_RGB4, TextureImporterFormat.ETC2_RGB4, TextureImporterFormat.ETC2_RGB4_PUNCHTHROUGH_ALPHA, TextureImporterFormat.ETC2_RGBA8, TextureImporterFormat.ASTC_RGB_4x4, TextureImporterFormat.ASTC_RGB_5x5, TextureImporterFormat.ASTC_RGB_6x6, TextureImporterFormat.ASTC_RGB_8x8, 
            TextureImporterFormat.ASTC_RGB_10x10, TextureImporterFormat.ASTC_RGB_12x12, TextureImporterFormat.ASTC_RGBA_4x4, TextureImporterFormat.ASTC_RGBA_5x5, TextureImporterFormat.ASTC_RGBA_6x6, TextureImporterFormat.ASTC_RGBA_8x8, TextureImporterFormat.ASTC_RGBA_10x10, TextureImporterFormat.ASTC_RGBA_12x12
         };
        private static readonly string[] kMaxTextureSizeStrings = new string[] { "32", "64", "128", "256", "512", "1024", "2048", "4096", "8192" };
        private static readonly int[] kMaxTextureSizeValues = new int[] { 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800, 0x1000, 0x2000 };
        private static readonly int[] kNormalFormatsValueWeb = new int[] { 12, 0x1d, 2, 5 };
        private static readonly int[] kTextureFormatsValueAndroid = new int[] { 
            10, 12, 0x1c, 0x1d, 0x22, 0x2d, 0x2e, 0x2f, 30, 0x1f, 0x20, 0x21, 0x23, 0x24, 0x30, 0x31, 
            50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 7, 3, 1, 13, 4
         };
        private static readonly int[] kTextureFormatsValueBB10 = new int[] { 0x22, 30, 0x1f, 0x20, 0x21, 7, 3, 1, 13, 4 };
        private static readonly int[] kTextureFormatsValueGLESEmu = new int[] { 
            0x22, 30, 0x1f, 0x20, 0x21, 0x2d, 0x2e, 0x2f, 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 
            0x38, 0x39, 0x3a, 0x3b, 7, 3, 1, 13, 5
         };
        private static readonly int[] kTextureFormatsValueiPhone = new int[] { 30, 0x1f, 0x20, 0x21, 7, 3, 1, 13, 4 };
        private static readonly int[] kTextureFormatsValueSTV = new int[] { 0x22, 7, 3, 1, 13, 4 };
        private static readonly int[] kTextureFormatsValueTizen = new int[] { 0x22, 7, 3, 1, 13, 4 };
        private static readonly int[] kTextureFormatsValueWeb = new int[] { 10, 12, 0x1c, 0x1d, 7, 3, 1, 2, 5 };
        private static readonly int[] kTextureFormatsValueWiiU = new int[] { 10, 12, 7, 1, 4, 13 };
        private SerializedProperty m_Alignment;
        private SerializedProperty m_AlphaIsTransparency;
        private SerializedProperty m_Aniso;
        private SerializedProperty m_BorderMipMap;
        private SerializedProperty m_ConvertToNormalMap;
        private SerializedProperty m_CubemapConvolution;
        private SerializedProperty m_CubemapConvolutionExponent;
        private SerializedProperty m_CubemapConvolutionSteps;
        private readonly GUIContent m_EmptyContent = new GUIContent(" ");
        private SerializedProperty m_EnableMipMap;
        private SerializedProperty m_FadeOut;
        private SerializedProperty m_FilterMode;
        private readonly int[] m_FilterModeOptions;
        private SerializedProperty m_GenerateCubemap;
        private SerializedProperty m_GenerateMipsInLinearSpace;
        private SerializedProperty m_GrayscaleToAlpha;
        private SerializedProperty m_HeightScale;
        private string m_ImportWarning;
        private SerializedProperty m_IsReadable;
        private SerializedProperty m_Lightmap;
        private SerializedProperty m_LinearTexture;
        private SerializedProperty m_MipMapFadeDistanceEnd;
        private SerializedProperty m_MipMapFadeDistanceStart;
        private SerializedProperty m_MipMapMode;
        private SerializedProperty m_NormalMap;
        private SerializedProperty m_NormalMapFilter;
        private SerializedProperty m_NPOTScale;
        [SerializeField]
        protected List<PlatformSetting> m_PlatformSettings;
        private SerializedProperty m_RGBM;
        private SerializedProperty m_SeamlessCubemap;
        private readonly AnimBool m_ShowBumpGenerationSettings = new AnimBool();
        private readonly AnimBool m_ShowConvolutionCubeMapSettings = new AnimBool();
        private readonly AnimBool m_ShowCookieCubeMapSettings = new AnimBool();
        private readonly AnimBool m_ShowGenericSpriteSettings = new AnimBool();
        private readonly AnimBool m_ShowManualAtlasGenerationSettings = new AnimBool();
        private SerializedProperty m_SpriteExtrude;
        private SerializedProperty m_SpriteMeshType;
        private SerializedProperty m_SpriteMode;
        private SerializedProperty m_SpritePackingTag;
        private SerializedProperty m_SpritePivot;
        private SerializedProperty m_SpritePixelsToUnits;
        private readonly int[] m_TextureFormatValues;
        private SerializedProperty m_TextureType;
        private readonly int[] m_TextureTypeValues = new int[] { 0, 1, 2, 8, 7, 3, 4, 6, 5 };
        private SerializedProperty m_WrapMode;
        private static string[] s_NormalFormatStringsAll;
        private static string[] s_NormalFormatStringsWeb;
        private static int[] s_NormalFormatsValueAll;
        private static Styles s_Styles;
        private static string[] s_TextureFormatStringsAll;
        private static string[] s_TextureFormatStringsAndroid;
        private static string[] s_TextureFormatStringsBB10;
        private static string[] s_TextureFormatStringsGLESEmu;
        private static string[] s_TextureFormatStringsiPhone;
        private static string[] s_TextureFormatStringsSTV;
        private static string[] s_TextureFormatStringsTizen;
        private static string[] s_TextureFormatStringsWeb;
        private static string[] s_TextureFormatStringsWiiU;
        private static int[] s_TextureFormatsValueAll;

        public TextureImporterInspector()
        {
            int[] numArray1 = new int[4];
            numArray1[1] = 1;
            numArray1[2] = 2;
            numArray1[3] = 4;
            this.m_TextureFormatValues = numArray1;
            this.m_FilterModeOptions = (int[]) Enum.GetValues(typeof(FilterMode));
        }

        private void AdvancedGUI()
        {
            TextureImporter target = this.target as TextureImporter;
            if (target != null)
            {
                int height = 0;
                int width = 0;
                target.GetWidthAndHeight(ref width, ref height);
                bool disabled = IsPowerOfTwo(height) && IsPowerOfTwo(width);
                EditorGUI.BeginDisabledGroup(disabled);
                this.EnumPopup(this.m_NPOTScale, typeof(TextureImporterNPOTScale), s_Styles.npot);
                EditorGUI.EndDisabledGroup();
                EditorGUI.BeginDisabledGroup(!disabled && (this.m_NPOTScale.intValue == 0));
                this.CubemapMappingGUI(true);
                EditorGUI.EndDisabledGroup();
                this.ToggleFromInt(this.m_IsReadable, s_Styles.readWrite);
                AdvancedTextureType normalMap = AdvancedTextureType.Default;
                if (this.m_NormalMap.intValue > 0)
                {
                    normalMap = AdvancedTextureType.NormalMap;
                }
                else if (this.m_Lightmap.intValue > 0)
                {
                    normalMap = AdvancedTextureType.LightMap;
                }
                EditorGUI.BeginChangeCheck();
                string[] displayedOptions = new string[] { "Default", "Normal Map", "Lightmap" };
                normalMap = (AdvancedTextureType) EditorGUILayout.Popup("Import Type", (int) normalMap, displayedOptions, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    switch (normalMap)
                    {
                        case AdvancedTextureType.Default:
                            this.m_NormalMap.intValue = 0;
                            this.m_Lightmap.intValue = 0;
                            this.m_ConvertToNormalMap.intValue = 0;
                            break;

                        case AdvancedTextureType.NormalMap:
                            this.m_NormalMap.intValue = 1;
                            this.m_Lightmap.intValue = 0;
                            this.m_LinearTexture.intValue = 1;
                            this.m_RGBM.intValue = 0;
                            break;

                        case AdvancedTextureType.LightMap:
                            this.m_NormalMap.intValue = 0;
                            this.m_Lightmap.intValue = 1;
                            this.m_ConvertToNormalMap.intValue = 0;
                            this.m_LinearTexture.intValue = 1;
                            this.m_RGBM.intValue = 0;
                            break;
                    }
                }
                EditorGUI.indentLevel++;
                switch (normalMap)
                {
                    case AdvancedTextureType.NormalMap:
                        EditorGUI.BeginChangeCheck();
                        this.BumpGUI();
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.SyncPlatformSettings();
                        }
                        break;

                    case AdvancedTextureType.Default:
                        this.ToggleFromInt(this.m_GrayscaleToAlpha, s_Styles.generateAlphaFromGrayscale);
                        this.DoAlphaIsTransparencyGUI();
                        this.ToggleFromInt(this.m_LinearTexture, s_Styles.linearTexture);
                        EditorGUILayout.Popup(this.m_RGBM, s_Styles.rgbmEncodingOptions, s_Styles.rgbmEncoding, new GUILayoutOption[0]);
                        this.SpriteGUI(false);
                        break;
                }
                EditorGUI.indentLevel--;
                this.ToggleFromInt(this.m_EnableMipMap, s_Styles.generateMipMaps);
                if (this.m_EnableMipMap.boolValue && !this.m_EnableMipMap.hasMultipleDifferentValues)
                {
                    EditorGUI.indentLevel++;
                    this.ToggleFromInt(this.m_GenerateMipsInLinearSpace, s_Styles.mipMapsInLinearSpace);
                    this.ToggleFromInt(this.m_BorderMipMap, s_Styles.borderMipMaps);
                    EditorGUILayout.Popup(this.m_MipMapMode, s_Styles.mipMapFilterOptions, s_Styles.mipMapFilter, new GUILayoutOption[0]);
                    this.ToggleFromInt(this.m_FadeOut, s_Styles.mipmapFadeOutToggle);
                    if (this.m_FadeOut.intValue > 0)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUI.BeginChangeCheck();
                        float intValue = this.m_MipMapFadeDistanceStart.intValue;
                        float maxValue = this.m_MipMapFadeDistanceEnd.intValue;
                        EditorGUILayout.MinMaxSlider(s_Styles.mipmapFadeOut, ref intValue, ref maxValue, 0f, 10f, new GUILayoutOption[0]);
                        if (EditorGUI.EndChangeCheck())
                        {
                            this.m_MipMapFadeDistanceStart.intValue = Mathf.RoundToInt(intValue);
                            this.m_MipMapFadeDistanceEnd.intValue = Mathf.RoundToInt(maxValue);
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        internal override void Apply()
        {
            SpriteEditorWindow.TextureImporterApply(base.serializedObject);
            base.Apply();
            this.SyncPlatformSettings();
            foreach (PlatformSetting setting in this.m_PlatformSettings)
            {
                setting.Apply();
            }
        }

        private void ApplySettingsToTexture()
        {
            foreach (AssetImporter importer in base.targets)
            {
                Texture tex = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture;
                if (this.m_Aniso.intValue != -1)
                {
                    TextureUtil.SetAnisoLevelNoDirty(tex, this.m_Aniso.intValue);
                }
                if (this.m_FilterMode.intValue != -1)
                {
                    TextureUtil.SetFilterModeNoDirty(tex, (FilterMode) this.m_FilterMode.intValue);
                }
                if (this.m_WrapMode.intValue != -1)
                {
                    TextureUtil.SetWrapModeNoDirty(tex, (TextureWrapMode) this.m_WrapMode.intValue);
                }
            }
            SceneView.RepaintAll();
        }

        private void BeginToggleGroup(SerializedProperty property, GUIContent label)
        {
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int num = !EditorGUILayout.BeginToggleGroup(label, property.intValue > 0) ? 0 : 1;
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = num;
            }
            EditorGUI.showMixedValue = false;
        }

        public virtual void BuildTargetList()
        {
            BuildPlayerWindow.BuildPlatform[] buildPlayerValidPlatforms = GetBuildPlayerValidPlatforms();
            this.m_PlatformSettings = new List<PlatformSetting>();
            this.m_PlatformSettings.Add(new PlatformSetting(string.Empty, BuildTarget.StandaloneWindows, this));
            foreach (BuildPlayerWindow.BuildPlatform platform in buildPlayerValidPlatforms)
            {
                this.m_PlatformSettings.Add(new PlatformSetting(platform.name, platform.DefaultTarget, this));
            }
        }

        private static string[] BuildTextureStrings(int[] texFormatValues)
        {
            string[] strArray = new string[texFormatValues.Length];
            for (int i = 0; i < texFormatValues.Length; i++)
            {
                int num2 = texFormatValues[i];
                int num3 = num2;
                switch ((num3 + 5))
                {
                    case 0:
                    {
                        strArray[i] = "Automatic Crunched";
                        continue;
                    }
                    case 2:
                    {
                        strArray[i] = "Automatic Truecolor";
                        continue;
                    }
                    case 3:
                    {
                        strArray[i] = "Automatic 16 bits";
                        continue;
                    }
                    case 4:
                    {
                        strArray[i] = "Automatic Compressed";
                        continue;
                    }
                }
                strArray[i] = " " + TextureUtil.GetTextureFormatString((TextureFormat) num2);
            }
            return strArray;
        }

        private void BumpGUI()
        {
            this.ToggleFromInt(this.m_ConvertToNormalMap, s_Styles.generateFromBump);
            this.m_ShowBumpGenerationSettings.target = this.m_ConvertToNormalMap.intValue > 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowBumpGenerationSettings.faded))
            {
                EditorGUILayout.Slider(this.m_HeightScale, 0f, 0.3f, s_Styles.bumpiness, new GUILayoutOption[0]);
                EditorGUILayout.Popup(this.m_NormalMapFilter, s_Styles.bumpFilteringOptions, s_Styles.bumpFiltering, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void CacheSerializedProperties()
        {
            this.m_TextureType = base.serializedObject.FindProperty("m_TextureType");
            this.m_GrayscaleToAlpha = base.serializedObject.FindProperty("m_GrayScaleToAlpha");
            this.m_ConvertToNormalMap = base.serializedObject.FindProperty("m_ConvertToNormalMap");
            this.m_NormalMap = base.serializedObject.FindProperty("m_ExternalNormalMap");
            this.m_HeightScale = base.serializedObject.FindProperty("m_HeightScale");
            this.m_NormalMapFilter = base.serializedObject.FindProperty("m_NormalMapFilter");
            this.m_GenerateCubemap = base.serializedObject.FindProperty("m_GenerateCubemap");
            this.m_SeamlessCubemap = base.serializedObject.FindProperty("m_SeamlessCubemap");
            this.m_BorderMipMap = base.serializedObject.FindProperty("m_BorderMipMap");
            this.m_NPOTScale = base.serializedObject.FindProperty("m_NPOTScale");
            this.m_IsReadable = base.serializedObject.FindProperty("m_IsReadable");
            this.m_LinearTexture = base.serializedObject.FindProperty("m_LinearTexture");
            this.m_RGBM = base.serializedObject.FindProperty("m_RGBM");
            this.m_EnableMipMap = base.serializedObject.FindProperty("m_EnableMipMap");
            this.m_MipMapMode = base.serializedObject.FindProperty("m_MipMapMode");
            this.m_GenerateMipsInLinearSpace = base.serializedObject.FindProperty("correctGamma");
            this.m_FadeOut = base.serializedObject.FindProperty("m_FadeOut");
            this.m_MipMapFadeDistanceStart = base.serializedObject.FindProperty("m_MipMapFadeDistanceStart");
            this.m_MipMapFadeDistanceEnd = base.serializedObject.FindProperty("m_MipMapFadeDistanceEnd");
            this.m_Lightmap = base.serializedObject.FindProperty("m_Lightmap");
            this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
            this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
            this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
            this.m_CubemapConvolution = base.serializedObject.FindProperty("m_CubemapConvolution");
            this.m_CubemapConvolutionSteps = base.serializedObject.FindProperty("m_CubemapConvolutionSteps");
            this.m_CubemapConvolutionExponent = base.serializedObject.FindProperty("m_CubemapConvolutionExponent");
            this.m_SpriteMode = base.serializedObject.FindProperty("m_SpriteMode");
            this.m_SpritePackingTag = base.serializedObject.FindProperty("m_SpritePackingTag");
            this.m_SpritePixelsToUnits = base.serializedObject.FindProperty("m_SpritePixelsToUnits");
            this.m_SpriteExtrude = base.serializedObject.FindProperty("m_SpriteExtrude");
            this.m_SpriteMeshType = base.serializedObject.FindProperty("m_SpriteMeshType");
            this.m_Alignment = base.serializedObject.FindProperty("m_Alignment");
            this.m_SpritePivot = base.serializedObject.FindProperty("m_SpritePivot");
            this.m_AlphaIsTransparency = base.serializedObject.FindProperty("m_AlphaIsTransparency");
        }

        private void CookieGUI()
        {
            CookieMode spot;
            EditorGUI.BeginChangeCheck();
            if (this.m_BorderMipMap.intValue > 0)
            {
                spot = CookieMode.Spot;
            }
            else if (this.m_GenerateCubemap.intValue != 0)
            {
                spot = CookieMode.Point;
            }
            else
            {
                spot = CookieMode.Directional;
            }
            spot = (CookieMode) EditorGUILayout.Popup(s_Styles.cookieType, (int) spot, s_Styles.cookieOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.SetCookieMode(spot);
            }
            this.m_ShowCookieCubeMapSettings.target = spot == CookieMode.Point;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowCookieCubeMapSettings.faded))
            {
                this.CubemapMappingGUI(false);
            }
            EditorGUILayout.EndFadeGroup();
            this.ToggleFromInt(this.m_GrayscaleToAlpha, s_Styles.generateAlphaFromGrayscale);
        }

        private static bool CountImportersWithAlpha(Object[] importers, out int count)
        {
            try
            {
                count = 0;
                foreach (Object obj2 in importers)
                {
                    if ((obj2 as TextureImporter).DoesSourceTextureHaveAlpha())
                    {
                        count++;
                    }
                }
                return true;
            }
            catch
            {
                count = importers.Length;
                return false;
            }
        }

        private void CubemapGUI()
        {
            this.CubemapMappingGUI(false);
        }

        private void CubemapMappingGUI(bool advancedMode)
        {
            EditorGUI.showMixedValue = this.m_GenerateCubemap.hasMultipleDifferentValues || this.m_SeamlessCubemap.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int count = !advancedMode ? 1 : 0;
            List<GUIContent> list = new List<GUIContent>(s_Styles.cubemapOptions);
            list.RemoveRange(0, count);
            List<int> list2 = new List<int>(s_Styles.cubemapValues);
            list2.RemoveRange(0, count);
            int num2 = EditorGUILayout.IntPopup(s_Styles.cubemap, this.m_GenerateCubemap.intValue, list.ToArray(), list2.ToArray(), new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_GenerateCubemap.intValue = num2;
            }
            EditorGUI.BeginDisabledGroup(num2 == 0);
            if (advancedMode)
            {
                EditorGUI.indentLevel++;
            }
            if (advancedMode)
            {
                EditorGUILayout.IntPopup(this.m_CubemapConvolution, s_Styles.cubemapConvolutionOptions, s_Styles.cubemapConvolutionValues.ToArray<int>(), s_Styles.cubemapConvolution, new GUILayoutOption[0]);
            }
            else
            {
                this.ToggleFromInt(this.m_CubemapConvolution, s_Styles.cubemapConvolutionSimple);
                if (this.m_CubemapConvolution.intValue != 0)
                {
                    this.m_CubemapConvolution.intValue = 1;
                }
            }
            if (advancedMode)
            {
                this.m_ShowConvolutionCubeMapSettings.target = this.m_CubemapConvolution.intValue == 1;
                if (EditorGUILayout.BeginFadeGroup(this.m_ShowConvolutionCubeMapSettings.faded))
                {
                    EditorGUI.indentLevel++;
                    this.m_CubemapConvolutionSteps.intValue = EditorGUILayout.IntField(s_Styles.cubemapConvolutionSteps, this.m_CubemapConvolutionSteps.intValue, new GUILayoutOption[0]);
                    this.m_CubemapConvolutionExponent.floatValue = EditorGUILayout.FloatField(s_Styles.cubemapConvolutionExp, this.m_CubemapConvolutionExponent.floatValue, new GUILayoutOption[0]);
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
            }
            this.ToggleFromInt(this.m_SeamlessCubemap, s_Styles.seamlessCubemap);
            if (advancedMode)
            {
                EditorGUI.indentLevel--;
            }
            EditorGUI.EndDisabledGroup();
            EditorGUI.showMixedValue = false;
        }

        private void DoAlphaIsTransparencyGUI()
        {
            int num;
            bool flag = CountImportersWithAlpha(base.targets, out num);
            if (this.m_GrayscaleToAlpha.boolValue || (num == base.targets.Length))
            {
                EditorGUI.BeginDisabledGroup(!flag);
                this.ToggleFromInt(this.m_AlphaIsTransparency, s_Styles.alphaIsTransparency);
                EditorGUI.EndDisabledGroup();
            }
        }

        private int EditCompressionQuality(BuildTarget target, int compression)
        {
            if (((target == BuildTarget.iOS) || (target == BuildTarget.Android)) || (((target == BuildTarget.BlackBerry) || (target == BuildTarget.Tizen)) || (target == BuildTarget.SamsungTV)))
            {
                int selectedIndex = 1;
                if (compression == 0)
                {
                    selectedIndex = 0;
                }
                else if (compression == 100)
                {
                    selectedIndex = 2;
                }
                switch (EditorGUILayout.Popup(s_Styles.compressionQuality, selectedIndex, s_Styles.mobileCompressionQualityOptions, new GUILayoutOption[0]))
                {
                    case 0:
                        return 0;

                    case 1:
                        return 50;

                    case 2:
                        return 100;
                }
                return 50;
            }
            compression = EditorGUILayout.IntSlider(s_Styles.compressionQualitySlider, compression, 0, 100, new GUILayoutOption[0]);
            return compression;
        }

        private void EnumPopup(SerializedProperty property, Type type, GUIContent label)
        {
            EditorGUILayout.IntPopup(property, EditorGUIUtility.TempContent(Enum.GetNames(type)), Enum.GetValues(type) as int[], label, new GUILayoutOption[0]);
        }

        public static BuildPlayerWindow.BuildPlatform[] GetBuildPlayerValidPlatforms()
        {
            List<BuildPlayerWindow.BuildPlatform> validPlatforms = BuildPlayerWindow.GetValidPlatforms();
            List<BuildPlayerWindow.BuildPlatform> list2 = new List<BuildPlayerWindow.BuildPlatform>();
            foreach (BuildPlayerWindow.BuildPlatform platform in validPlatforms)
            {
                list2.Add(platform);
            }
            return list2.ToArray();
        }

        private TextureImporterSettings GetSerializedPropertySettings()
        {
            return this.GetSerializedPropertySettings(new TextureImporterSettings());
        }

        private TextureImporterSettings GetSerializedPropertySettings(TextureImporterSettings settings)
        {
            if (!this.m_GrayscaleToAlpha.hasMultipleDifferentValues)
            {
                settings.grayscaleToAlpha = this.m_GrayscaleToAlpha.intValue > 0;
            }
            if (!this.m_ConvertToNormalMap.hasMultipleDifferentValues)
            {
                settings.convertToNormalMap = this.m_ConvertToNormalMap.intValue > 0;
            }
            if (!this.m_NormalMap.hasMultipleDifferentValues)
            {
                settings.normalMap = this.m_NormalMap.intValue > 0;
            }
            if (!this.m_HeightScale.hasMultipleDifferentValues)
            {
                settings.heightmapScale = this.m_HeightScale.floatValue;
            }
            if (!this.m_NormalMapFilter.hasMultipleDifferentValues)
            {
                settings.normalMapFilter = (TextureImporterNormalFilter) this.m_NormalMapFilter.intValue;
            }
            if (!this.m_GenerateCubemap.hasMultipleDifferentValues)
            {
                settings.generateCubemap = (TextureImporterGenerateCubemap) this.m_GenerateCubemap.intValue;
            }
            if (!this.m_CubemapConvolution.hasMultipleDifferentValues)
            {
                settings.cubemapConvolution = (TextureImporterCubemapConvolution) this.m_CubemapConvolution.intValue;
            }
            if (!this.m_CubemapConvolutionSteps.hasMultipleDifferentValues)
            {
                settings.cubemapConvolutionSteps = this.m_CubemapConvolutionSteps.intValue;
            }
            if (!this.m_CubemapConvolutionExponent.hasMultipleDifferentValues)
            {
                settings.cubemapConvolutionExponent = this.m_CubemapConvolutionExponent.floatValue;
            }
            if (!this.m_SeamlessCubemap.hasMultipleDifferentValues)
            {
                settings.seamlessCubemap = this.m_SeamlessCubemap.intValue > 0;
            }
            if (!this.m_BorderMipMap.hasMultipleDifferentValues)
            {
                settings.borderMipmap = this.m_BorderMipMap.intValue > 0;
            }
            if (!this.m_NPOTScale.hasMultipleDifferentValues)
            {
                settings.npotScale = (TextureImporterNPOTScale) this.m_NPOTScale.intValue;
            }
            if (!this.m_IsReadable.hasMultipleDifferentValues)
            {
                settings.readable = this.m_IsReadable.intValue > 0;
            }
            if (!this.m_LinearTexture.hasMultipleDifferentValues)
            {
                settings.linearTexture = this.m_LinearTexture.intValue > 0;
            }
            if (!this.m_RGBM.hasMultipleDifferentValues)
            {
                settings.rgbm = (TextureImporterRGBMMode) this.m_RGBM.intValue;
            }
            if (!this.m_EnableMipMap.hasMultipleDifferentValues)
            {
                settings.mipmapEnabled = this.m_EnableMipMap.intValue > 0;
            }
            if (!this.m_GenerateMipsInLinearSpace.hasMultipleDifferentValues)
            {
                settings.generateMipsInLinearSpace = this.m_GenerateMipsInLinearSpace.intValue > 0;
            }
            if (!this.m_MipMapMode.hasMultipleDifferentValues)
            {
                settings.mipmapFilter = (TextureImporterMipFilter) this.m_MipMapMode.intValue;
            }
            if (!this.m_FadeOut.hasMultipleDifferentValues)
            {
                settings.fadeOut = this.m_FadeOut.intValue > 0;
            }
            if (!this.m_MipMapFadeDistanceStart.hasMultipleDifferentValues)
            {
                settings.mipmapFadeDistanceStart = this.m_MipMapFadeDistanceStart.intValue;
            }
            if (!this.m_MipMapFadeDistanceEnd.hasMultipleDifferentValues)
            {
                settings.mipmapFadeDistanceEnd = this.m_MipMapFadeDistanceEnd.intValue;
            }
            if (!this.m_Lightmap.hasMultipleDifferentValues)
            {
                settings.lightmap = this.m_Lightmap.intValue > 0;
            }
            if (!this.m_SpriteMode.hasMultipleDifferentValues)
            {
                settings.spriteMode = this.m_SpriteMode.intValue;
            }
            if (!this.m_SpritePixelsToUnits.hasMultipleDifferentValues)
            {
                settings.spritePixelsPerUnit = this.m_SpritePixelsToUnits.floatValue;
            }
            if (!this.m_SpriteExtrude.hasMultipleDifferentValues)
            {
                settings.spriteExtrude = (uint) this.m_SpriteExtrude.intValue;
            }
            if (!this.m_SpriteMeshType.hasMultipleDifferentValues)
            {
                settings.spriteMeshType = (SpriteMeshType) this.m_SpriteMeshType.intValue;
            }
            if (!this.m_Alignment.hasMultipleDifferentValues)
            {
                settings.spriteAlignment = this.m_Alignment.intValue;
            }
            if (!this.m_SpritePivot.hasMultipleDifferentValues)
            {
                settings.spritePivot = this.m_SpritePivot.vector2Value;
            }
            if (!this.m_WrapMode.hasMultipleDifferentValues)
            {
                settings.wrapMode = (TextureWrapMode) this.m_WrapMode.intValue;
            }
            if (!this.m_FilterMode.hasMultipleDifferentValues)
            {
                settings.filterMode = (FilterMode) this.m_FilterMode.intValue;
            }
            if (!this.m_Aniso.hasMultipleDifferentValues)
            {
                settings.aniso = this.m_Aniso.intValue;
            }
            if (!this.m_AlphaIsTransparency.hasMultipleDifferentValues)
            {
                settings.alphaIsTransparency = this.m_AlphaIsTransparency.intValue > 0;
            }
            return settings;
        }

        internal override bool HasModified()
        {
            if (base.HasModified())
            {
                return true;
            }
            foreach (PlatformSetting setting in this.m_PlatformSettings)
            {
                if (setting.HasChanged())
                {
                    return true;
                }
            }
            return false;
        }

        private void ImageGUI()
        {
            TextureImporter target = this.target as TextureImporter;
            if (target != null)
            {
                this.ToggleFromInt(this.m_GrayscaleToAlpha, s_Styles.generateAlphaFromGrayscale);
                this.DoAlphaIsTransparencyGUI();
            }
        }

        internal static bool IsGLESMobileTargetPlatform(BuildTarget target)
        {
            return ((((target == BuildTarget.iOS) || (target == BuildTarget.Android)) || ((target == BuildTarget.BlackBerry) || (target == BuildTarget.Tizen))) || (target == BuildTarget.SamsungTV));
        }

        private static bool IsPowerOfTwo(int f)
        {
            return ((f & (f - 1)) == 0);
        }

        private static TextureImporterFormat MakeTextureFormatHaveAlpha(TextureImporterFormat format)
        {
            switch (format)
            {
                case TextureImporterFormat.RGB16:
                    return TextureImporterFormat.ARGB16;

                case TextureImporterFormat.DXT1:
                    return TextureImporterFormat.DXT5;

                case TextureImporterFormat.PVRTC_RGB2:
                    return TextureImporterFormat.PVRTC_RGBA2;

                case TextureImporterFormat.PVRTC_RGB4:
                    return TextureImporterFormat.PVRTC_RGBA4;

                case TextureImporterFormat.RGB24:
                    return TextureImporterFormat.ARGB32;
            }
            return format;
        }

        public void OnDisable()
        {
            base.OnDisable();
        }

        public virtual void OnEnable()
        {
            this.CacheSerializedProperties();
            this.m_ShowBumpGenerationSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCookieCubeMapSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowCookieCubeMapSettings.value = (this.textureType == TextureImporterType.Cookie) && (this.m_GenerateCubemap.intValue != 0);
            this.m_ShowConvolutionCubeMapSettings.value = (this.m_CubemapConvolution.intValue == 1) && (this.m_GenerateCubemap.intValue != 0);
            this.m_ShowGenericSpriteSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowManualAtlasGenerationSettings.valueChanged.AddListener(new UnityAction(this.Repaint));
            this.m_ShowGenericSpriteSettings.value = this.m_SpriteMode.intValue != 0;
            this.m_ShowManualAtlasGenerationSettings.value = this.m_SpriteMode.intValue == 2;
        }

        public override void OnInspectorGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            bool enabled = GUI.enabled;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.textureTypeHasMultipleDifferentValues;
            int num = EditorGUILayout.IntPopup(s_Styles.textureType, (int) this.textureType, s_Styles.textureTypeOptions, this.m_TextureTypeValues, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_TextureType.intValue = num;
                TextureImporterSettings serializedPropertySettings = this.GetSerializedPropertySettings();
                serializedPropertySettings.ApplyTextureType(this.textureType, true);
                this.SetSerializedPropertySettings(serializedPropertySettings);
                this.SyncPlatformSettings();
                this.ApplySettingsToTexture();
            }
            if (!this.textureTypeHasMultipleDifferentValues)
            {
                switch (this.textureType)
                {
                    case TextureImporterType.Image:
                        this.ImageGUI();
                        break;

                    case TextureImporterType.Bump:
                        this.BumpGUI();
                        break;

                    case TextureImporterType.Cubemap:
                        this.CubemapGUI();
                        break;

                    case TextureImporterType.Cookie:
                        this.CookieGUI();
                        break;

                    case TextureImporterType.Advanced:
                        this.AdvancedGUI();
                        break;

                    case TextureImporterType.Sprite:
                        this.SpriteGUI();
                        break;
                }
            }
            EditorGUILayout.Space();
            this.PreviewableGUI();
            this.SizeAndFormatGUI();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            base.ApplyRevertGUI();
            GUILayout.EndHorizontal();
            this.UpdateImportWarning();
            if (this.m_ImportWarning != null)
            {
                EditorGUILayout.HelpBox(this.m_ImportWarning, MessageType.Warning);
            }
            GUI.enabled = enabled;
        }

        private void PreviewableGUI()
        {
            EditorGUI.BeginChangeCheck();
            if ((((this.textureType != TextureImporterType.GUI) && (this.textureType != TextureImporterType.Sprite)) && ((this.textureType != TextureImporterType.Cubemap) && (this.textureType != TextureImporterType.Cookie))) && (this.textureType != TextureImporterType.Lightmap))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
                TextureWrapMode selected = (TextureWrapMode) this.m_WrapMode.intValue;
                if (selected == ~TextureWrapMode.Repeat)
                {
                    selected = TextureWrapMode.Repeat;
                }
                selected = (TextureWrapMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), selected, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_WrapMode.intValue = (int) selected;
                }
                if (((this.m_NPOTScale.intValue == 0) && (selected == TextureWrapMode.Repeat)) && !ShaderUtil.hardwareSupportsFullNPOT)
                {
                    bool flag = false;
                    foreach (Object obj2 in base.targets)
                    {
                        int width = -1;
                        int height = -1;
                        ((TextureImporter) obj2).GetWidthAndHeight(ref width, ref height);
                        if (!Mathf.IsPowerOfTwo(width) || !Mathf.IsPowerOfTwo(height))
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag)
                    {
                        EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("Graphics device doesn't support Repeat wrap mode on NPOT textures. Falling back to Clamp.").text, MessageType.Warning, true);
                    }
                }
            }
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
            FilterMode intValue = (FilterMode) this.m_FilterMode.intValue;
            if (intValue == ~FilterMode.Point)
            {
                if (((this.m_FadeOut.intValue > 0) || (this.m_ConvertToNormalMap.intValue > 0)) || (this.m_NormalMap.intValue > 0))
                {
                    intValue = FilterMode.Trilinear;
                }
                else
                {
                    intValue = FilterMode.Bilinear;
                }
            }
            intValue = (FilterMode) EditorGUILayout.IntPopup(s_Styles.filterMode, (int) intValue, s_Styles.filterModeOptions, this.m_FilterModeOptions, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_FilterMode.intValue = (int) intValue;
            }
            if (((intValue != FilterMode.Point) && ((this.m_EnableMipMap.intValue > 0) || (this.textureType == TextureImporterType.Advanced))) && ((this.textureType != TextureImporterType.Sprite) && (this.textureType != TextureImporterType.Cubemap)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
                int num4 = this.m_Aniso.intValue;
                if (num4 == -1)
                {
                    num4 = 1;
                }
                num4 = EditorGUILayout.IntSlider("Aniso Level", num4, 0, 0x10, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    this.m_Aniso.intValue = num4;
                }
                TextureInspector.DoAnisoGlobalSettingNote(num4);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ApplySettingsToTexture();
            }
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.CacheSerializedProperties();
            this.BuildTargetList();
            this.ApplySettingsToTexture();
            SelectMainAssets(base.targets);
        }

        public static void SelectMainAssets(Object[] targets)
        {
            ArrayList list = new ArrayList();
            foreach (AssetImporter importer in targets)
            {
                Texture texture = AssetDatabase.LoadMainAssetAtPath(importer.assetPath) as Texture;
                if (texture != null)
                {
                    list.Add(texture);
                }
            }
            Selection.objects = list.ToArray(typeof(Object)) as Object[];
        }

        private void SetCookieMode(CookieMode cm)
        {
            switch (cm)
            {
                case CookieMode.Spot:
                    this.m_BorderMipMap.intValue = 1;
                    this.m_WrapMode.intValue = 1;
                    this.m_GenerateCubemap.intValue = 0;
                    break;

                case CookieMode.Directional:
                    this.m_BorderMipMap.intValue = 0;
                    this.m_WrapMode.intValue = 0;
                    this.m_GenerateCubemap.intValue = 0;
                    break;

                case CookieMode.Point:
                    this.m_BorderMipMap.intValue = 0;
                    this.m_WrapMode.intValue = 1;
                    this.m_GenerateCubemap.intValue = 1;
                    break;
            }
        }

        private void SetSerializedPropertySettings(TextureImporterSettings settings)
        {
            this.m_GrayscaleToAlpha.intValue = !settings.grayscaleToAlpha ? 0 : 1;
            this.m_ConvertToNormalMap.intValue = !settings.convertToNormalMap ? 0 : 1;
            this.m_NormalMap.intValue = !settings.normalMap ? 0 : 1;
            this.m_HeightScale.floatValue = settings.heightmapScale;
            this.m_NormalMapFilter.intValue = (int) settings.normalMapFilter;
            this.m_GenerateCubemap.intValue = (int) settings.generateCubemap;
            this.m_CubemapConvolution.intValue = (int) settings.cubemapConvolution;
            this.m_CubemapConvolutionSteps.intValue = settings.cubemapConvolutionSteps;
            this.m_CubemapConvolutionExponent.floatValue = settings.cubemapConvolutionExponent;
            this.m_SeamlessCubemap.intValue = !settings.seamlessCubemap ? 0 : 1;
            this.m_BorderMipMap.intValue = !settings.borderMipmap ? 0 : 1;
            this.m_NPOTScale.intValue = (int) settings.npotScale;
            this.m_IsReadable.intValue = !settings.readable ? 0 : 1;
            this.m_EnableMipMap.intValue = !settings.mipmapEnabled ? 0 : 1;
            this.m_LinearTexture.intValue = !settings.linearTexture ? 0 : 1;
            this.m_RGBM.intValue = (int) settings.rgbm;
            this.m_MipMapMode.intValue = (int) settings.mipmapFilter;
            this.m_GenerateMipsInLinearSpace.intValue = !settings.generateMipsInLinearSpace ? 0 : 1;
            this.m_FadeOut.intValue = !settings.fadeOut ? 0 : 1;
            this.m_MipMapFadeDistanceStart.intValue = settings.mipmapFadeDistanceStart;
            this.m_MipMapFadeDistanceEnd.intValue = settings.mipmapFadeDistanceEnd;
            this.m_Lightmap.intValue = !settings.lightmap ? 0 : 1;
            this.m_SpriteMode.intValue = settings.spriteMode;
            this.m_SpritePixelsToUnits.floatValue = settings.spritePixelsPerUnit;
            this.m_SpriteExtrude.intValue = (int) settings.spriteExtrude;
            this.m_SpriteMeshType.intValue = (int) settings.spriteMeshType;
            this.m_Alignment.intValue = settings.spriteAlignment;
            this.m_WrapMode.intValue = (int) settings.wrapMode;
            this.m_FilterMode.intValue = (int) settings.filterMode;
            this.m_Aniso.intValue = settings.aniso;
            this.m_AlphaIsTransparency.intValue = !settings.alphaIsTransparency ? 0 : 1;
        }

        protected void SizeAndFormatGUI()
        {
            BuildPlayerWindow.BuildPlatform[] platforms = GetBuildPlayerValidPlatforms().ToArray<BuildPlayerWindow.BuildPlatform>();
            GUILayout.Space(10f);
            int num = EditorGUILayout.BeginPlatformGrouping(platforms, s_Styles.defaultPlatform);
            PlatformSetting setting = this.m_PlatformSettings[num + 1];
            if (!setting.isDefault)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = setting.overriddenIsDifferent;
                bool overridden = EditorGUILayout.ToggleLeft("Override for " + setting.name, setting.overridden, new GUILayoutOption[0]);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    setting.SetOverriddenForAll(overridden);
                    this.SyncPlatformSettings();
                }
            }
            bool disabled = !setting.isDefault && !setting.allAreOverridden;
            EditorGUI.BeginDisabledGroup(disabled);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = setting.overriddenIsDifferent || setting.maxTextureSizeIsDifferent;
            int maxTextureSize = EditorGUILayout.IntPopup(s_Styles.maxSize.text, setting.maxTextureSize, kMaxTextureSizeStrings, kMaxTextureSizeValues, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                setting.SetMaxTextureSizeForAll(maxTextureSize);
                this.SyncPlatformSettings();
            }
            int[] second = null;
            string[] strArray = null;
            bool flag3 = false;
            int selectedValue = 0;
            bool flag4 = false;
            int num4 = 0;
            bool flag5 = false;
            for (int i = 0; i < base.targets.Length; i++)
            {
                TextureImporter importer = base.targets[i] as TextureImporter;
                TextureImporterType tType = !this.textureTypeHasMultipleDifferentValues ? this.textureType : importer.textureType;
                TextureImporterSettings settings = setting.GetSettings(importer);
                int num6 = (int) setting.textureFormats[i];
                int num7 = num6;
                if (!setting.isDefault && (num6 < 0))
                {
                    num7 = (int) TextureImporter.SimpleToFullTextureFormat2((TextureImporterFormat) num7, tType, settings, importer.DoesSourceTextureHaveAlpha(), importer.IsSourceTextureHDR(), setting.m_Target);
                }
                if (settings.normalMap && !IsGLESMobileTargetPlatform(setting.m_Target))
                {
                    num7 = (int) MakeTextureFormatHaveAlpha((TextureImporterFormat) num7);
                }
                TextureImporterType type2 = tType;
                if (type2 != TextureImporterType.Cookie)
                {
                    if (type2 == TextureImporterType.Advanced)
                    {
                        goto Label_020E;
                    }
                    goto Label_0404;
                }
                int[] first = new int[1];
                string[] strArray2 = new string[] { "8 Bit Alpha" };
                num6 = 0;
                goto Label_045B;
            Label_020E:
                num6 = num7;
                if (IsGLESMobileTargetPlatform(setting.m_Target))
                {
                    if (s_TextureFormatStringsiPhone == null)
                    {
                        s_TextureFormatStringsiPhone = BuildTextureStrings(kTextureFormatsValueiPhone);
                    }
                    if (s_TextureFormatStringsAndroid == null)
                    {
                        s_TextureFormatStringsAndroid = BuildTextureStrings(kTextureFormatsValueAndroid);
                    }
                    if (setting.m_Target == BuildTarget.iOS)
                    {
                        first = kTextureFormatsValueiPhone;
                        strArray2 = s_TextureFormatStringsiPhone;
                    }
                    else if (setting.m_Target == BuildTarget.SamsungTV)
                    {
                        if (s_TextureFormatStringsSTV == null)
                        {
                            s_TextureFormatStringsSTV = BuildTextureStrings(kTextureFormatsValueSTV);
                        }
                        first = kTextureFormatsValueSTV;
                        strArray2 = s_TextureFormatStringsSTV;
                    }
                    else
                    {
                        first = kTextureFormatsValueAndroid;
                        strArray2 = s_TextureFormatStringsAndroid;
                    }
                }
                else if (!settings.normalMap)
                {
                    if (s_TextureFormatStringsAll == null)
                    {
                        s_TextureFormatStringsAll = BuildTextureStrings(TextureFormatsValueAll);
                    }
                    if (s_TextureFormatStringsWiiU == null)
                    {
                        s_TextureFormatStringsWiiU = BuildTextureStrings(kTextureFormatsValueWiiU);
                    }
                    if (s_TextureFormatStringsGLESEmu == null)
                    {
                        s_TextureFormatStringsGLESEmu = BuildTextureStrings(kTextureFormatsValueGLESEmu);
                    }
                    if (s_TextureFormatStringsWeb == null)
                    {
                        s_TextureFormatStringsWeb = BuildTextureStrings(kTextureFormatsValueWeb);
                    }
                    if (setting.isDefault)
                    {
                        first = TextureFormatsValueAll;
                        strArray2 = s_TextureFormatStringsAll;
                    }
                    else if (setting.m_Target == BuildTarget.WiiU)
                    {
                        first = kTextureFormatsValueWiiU;
                        strArray2 = s_TextureFormatStringsWiiU;
                    }
                    else if (setting.m_Target == BuildTarget.StandaloneGLESEmu)
                    {
                        first = kTextureFormatsValueGLESEmu;
                        strArray2 = s_TextureFormatStringsGLESEmu;
                    }
                    else
                    {
                        first = kTextureFormatsValueWeb;
                        strArray2 = s_TextureFormatStringsWeb;
                    }
                }
                else
                {
                    if (s_NormalFormatStringsAll == null)
                    {
                        s_NormalFormatStringsAll = BuildTextureStrings(NormalFormatsValueAll);
                    }
                    if (s_NormalFormatStringsWeb == null)
                    {
                        s_NormalFormatStringsWeb = BuildTextureStrings(kNormalFormatsValueWeb);
                    }
                    if (setting.isDefault)
                    {
                        first = NormalFormatsValueAll;
                        strArray2 = s_NormalFormatStringsAll;
                    }
                    else
                    {
                        first = kNormalFormatsValueWeb;
                        strArray2 = s_NormalFormatStringsWeb;
                    }
                }
                goto Label_045B;
            Label_0404:
                first = this.m_TextureFormatValues;
                if (<>f__am$cache47 == null)
                {
                    <>f__am$cache47 = content => content.text;
                }
                strArray2 = s_Styles.textureFormatOptions.Select<GUIContent, string>(<>f__am$cache47).ToArray<string>();
                if (num6 >= 0)
                {
                    num6 = (int) TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat) num6);
                }
                num6 = -1 - num6;
            Label_045B:
                if (i == 0)
                {
                    second = first;
                    strArray = strArray2;
                    selectedValue = num6;
                    num4 = num7;
                }
                else
                {
                    if (num6 != selectedValue)
                    {
                        flag4 = true;
                    }
                    if (num7 != num4)
                    {
                        flag5 = true;
                    }
                    if (!first.SequenceEqual<int>(second) || !strArray2.SequenceEqual<string>(strArray))
                    {
                        flag3 = true;
                        break;
                    }
                }
            }
            EditorGUI.BeginDisabledGroup(flag3 || (strArray.Length == 1));
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = flag3 || flag4;
            selectedValue = EditorGUILayout.IntPopup(s_Styles.textureFormat, selectedValue, EditorGUIUtility.TempContent(strArray), second, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (this.textureType != TextureImporterType.Advanced)
            {
                selectedValue = -1 - selectedValue;
            }
            if (EditorGUI.EndChangeCheck())
            {
                setting.SetTextureFormatForAll((TextureImporterFormat) selectedValue);
                this.SyncPlatformSettings();
            }
            EditorGUI.EndDisabledGroup();
            if ((num4 == -5) || (!flag5 && ArrayUtility.Contains<TextureImporterFormat>(kFormatsWithCompressionSettings, (TextureImporterFormat) num4)))
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = setting.overriddenIsDifferent || setting.compressionQualityIsDifferent;
                int quality = this.EditCompressionQuality(setting.m_Target, setting.compressionQuality);
                EditorGUI.showMixedValue = false;
                if (EditorGUI.EndChangeCheck())
                {
                    setting.SetCompressionQualityForAll(quality);
                    this.SyncPlatformSettings();
                }
            }
            if ((TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat) selectedValue) == TextureImporterFormat.AutomaticCrunched) && (TextureImporter.FullToSimpleTextureFormat((TextureImporterFormat) num4) != TextureImporterFormat.AutomaticCrunched))
            {
                EditorGUILayout.HelpBox("Crunched is not supported on this platform. Falling back to 'Compressed'.", MessageType.Warning);
            }
            if ((setting.overridden && (setting.m_Target == BuildTarget.Android)) && ((selectedValue == -1) && (setting.importers.Length > 0)))
            {
                TextureImporter importer2 = setting.importers[0];
                EditorGUI.BeginChangeCheck();
                bool flag = GUILayout.Toggle(importer2.GetAllowsAlphaSplitting(), s_Styles.etc1Compression, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (TextureImporter importer3 in setting.importers)
                    {
                        importer3.SetAllowsAlphaSplitting(flag);
                    }
                    setting.SetChanged();
                }
            }
            if ((!setting.overridden && (setting.m_Target == BuildTarget.Android)) && ((setting.importers.Length > 0) && setting.importers[0].GetAllowsAlphaSplitting()))
            {
                setting.importers[0].SetAllowsAlphaSplitting(false);
                setting.SetChanged();
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndPlatformGrouping();
        }

        private void SpriteGUI()
        {
            this.SpriteGUI(true);
        }

        private void SpriteGUI(bool showMipMapControls)
        {
            EditorGUI.BeginChangeCheck();
            if (this.textureType == TextureImporterType.Advanced)
            {
                int[] optionValues = new int[4];
                optionValues[1] = 1;
                optionValues[2] = 2;
                optionValues[3] = 3;
                EditorGUILayout.IntPopup(this.m_SpriteMode, s_Styles.spriteModeOptionsAdvanced, optionValues, s_Styles.spriteMode, new GUILayoutOption[0]);
            }
            else
            {
                int[] numArray2 = new int[] { 1, 2, 3 };
                EditorGUILayout.IntPopup(this.m_SpriteMode, s_Styles.spriteModeOptions, numArray2, s_Styles.spriteMode, new GUILayoutOption[0]);
            }
            if (EditorGUI.EndChangeCheck())
            {
                GUIUtility.keyboardControl = 0;
            }
            EditorGUI.indentLevel++;
            this.m_ShowGenericSpriteSettings.target = this.m_SpriteMode.intValue != 0;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowGenericSpriteSettings.faded))
            {
                EditorGUILayout.PropertyField(this.m_SpritePackingTag, s_Styles.spritePackingTag, new GUILayoutOption[0]);
                EditorGUILayout.PropertyField(this.m_SpritePixelsToUnits, s_Styles.spritePixelsPerUnit, new GUILayoutOption[0]);
                if (this.textureType == TextureImporterType.Advanced)
                {
                    int[] numArray3 = new int[2];
                    numArray3[1] = 1;
                    EditorGUILayout.IntPopup(this.m_SpriteMeshType, s_Styles.spriteMeshTypeOptions, numArray3, s_Styles.spriteMeshType, new GUILayoutOption[0]);
                    EditorGUILayout.IntSlider(this.m_SpriteExtrude, 0, 0x20, s_Styles.spriteExtrude, new GUILayoutOption[0]);
                }
                if (this.m_SpriteMode.intValue == 1)
                {
                    EditorGUILayout.Popup(this.m_Alignment, s_Styles.spriteAlignmentOptions, s_Styles.spriteAlignment, new GUILayoutOption[0]);
                    if (this.m_Alignment.intValue == 9)
                    {
                        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                        EditorGUILayout.PropertyField(this.m_SpritePivot, this.m_EmptyContent, new GUILayoutOption[0]);
                        GUILayout.EndHorizontal();
                    }
                }
                EditorGUI.BeginDisabledGroup(base.targets.Length != 1);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Sprite Editor", new GUILayoutOption[0]))
                {
                    if (this.HasModified())
                    {
                        string message = ("Unapplied import settings for '" + ((TextureImporter) this.target).assetPath + "'.\n") + "Apply and continue to sprite editor or cancel.";
                        if (EditorUtility.DisplayDialog("Unapplied import settings", message, "Apply", "Cancel"))
                        {
                            base.ApplyAndImport();
                            SpriteEditorWindow.GetWindow();
                            GUIUtility.ExitGUI();
                        }
                    }
                    else
                    {
                        SpriteEditorWindow.GetWindow();
                    }
                }
                GUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndFadeGroup();
            EditorGUI.indentLevel--;
            if (showMipMapControls)
            {
                this.ToggleFromInt(this.m_EnableMipMap, s_Styles.generateMipMaps);
            }
        }

        private void SyncPlatformSettings()
        {
            foreach (PlatformSetting setting in this.m_PlatformSettings)
            {
                setting.Sync();
            }
        }

        private void ToggleFromInt(SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            int num = !EditorGUILayout.Toggle(label, property.intValue > 0, new GUILayoutOption[0]) ? 0 : 1;
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                property.intValue = num;
            }
        }

        private void UpdateImportWarning()
        {
            TextureImporter target = this.target as TextureImporter;
            this.m_ImportWarning = (target == null) ? null : target.GetImportWarnings();
        }

        private static int[] NormalFormatsValueAll
        {
            get
            {
                bool flag = false;
                bool flag2 = false;
                bool flag3 = false;
                bool flag4 = false;
                bool flag5 = false;
                foreach (BuildPlayerWindow.BuildPlatform platform in GetBuildPlayerValidPlatforms())
                {
                    switch (platform.DefaultTarget)
                    {
                        case BuildTarget.iOS:
                        {
                            flag2 = true;
                            flag = true;
                            continue;
                        }
                        case BuildTarget.Android:
                        {
                            flag2 = true;
                            flag3 = true;
                            flag = true;
                            flag4 = true;
                            flag5 = true;
                            continue;
                        }
                        case BuildTarget.StandaloneGLESEmu:
                        {
                            flag2 = true;
                            flag = true;
                            flag4 = true;
                            flag5 = true;
                            continue;
                        }
                        case BuildTarget.BlackBerry:
                            flag2 = true;
                            flag = true;
                            break;

                        case BuildTarget.Tizen:
                            goto Label_0097;
                    }
                    continue;
                Label_0097:
                    flag = true;
                }
                List<int> list = new List<int>();
                int[] collection = new int[] { -1, 12 };
                list.AddRange(collection);
                if (flag2)
                {
                    list.AddRange(new int[] { 30, 0x1f, 0x20, 0x21 });
                }
                if (flag3)
                {
                    int[] numArray2 = new int[] { 0x23, 0x24 };
                    list.AddRange(numArray2);
                }
                if (flag)
                {
                    int[] numArray3 = new int[] { 0x22 };
                    list.AddRange(numArray3);
                }
                if (flag4)
                {
                    int[] numArray4 = new int[] { 0x2d, 0x2e, 0x2f };
                    list.AddRange(numArray4);
                }
                if (flag5)
                {
                    list.AddRange(new int[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b });
                }
                list.AddRange(new int[] { -2, 2, 13, -3, 4, -5, 0x1d });
                s_NormalFormatsValueAll = list.ToArray();
                return s_NormalFormatsValueAll;
            }
        }

        internal override bool showImportedObject
        {
            get
            {
                return false;
            }
        }

        private static int[] TextureFormatsValueAll
        {
            get
            {
                if (s_TextureFormatsValueAll == null)
                {
                    bool flag = false;
                    bool flag2 = false;
                    bool flag3 = false;
                    bool flag4 = false;
                    bool flag5 = false;
                    foreach (BuildPlayerWindow.BuildPlatform platform in GetBuildPlayerValidPlatforms())
                    {
                        BuildTarget defaultTarget = platform.DefaultTarget;
                        switch (defaultTarget)
                        {
                            case BuildTarget.iOS:
                            {
                                flag2 = true;
                                continue;
                            }
                            case BuildTarget.Android:
                            {
                                flag2 = true;
                                flag = true;
                                flag3 = true;
                                flag4 = true;
                                flag5 = true;
                                continue;
                            }
                            case BuildTarget.StandaloneGLESEmu:
                            {
                                flag2 = true;
                                flag = true;
                                flag4 = true;
                                flag5 = true;
                                continue;
                            }
                            default:
                            {
                                if (defaultTarget != BuildTarget.BlackBerry)
                                {
                                    if (defaultTarget == BuildTarget.Tizen)
                                    {
                                        break;
                                    }
                                    if (defaultTarget == BuildTarget.SamsungTV)
                                    {
                                        goto Label_00B5;
                                    }
                                }
                                else
                                {
                                    flag2 = true;
                                    flag = true;
                                }
                                continue;
                            }
                        }
                        flag = true;
                        continue;
                    Label_00B5:
                        flag = true;
                    }
                    List<int> list = new List<int>();
                    int[] collection = new int[] { -1, 10, 12 };
                    list.AddRange(collection);
                    if (flag)
                    {
                        list.Add(0x22);
                    }
                    if (flag2)
                    {
                        list.AddRange(new int[] { 30, 0x1f, 0x20, 0x21 });
                    }
                    if (flag3)
                    {
                        int[] numArray2 = new int[] { 0x23, 0x24 };
                        list.AddRange(numArray2);
                    }
                    if (flag4)
                    {
                        int[] numArray3 = new int[] { 0x2d, 0x2e, 0x2f };
                        list.AddRange(numArray3);
                    }
                    if (flag5)
                    {
                        list.AddRange(new int[] { 0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b });
                    }
                    list.AddRange(new int[] { -2, 7, 2, 13, -3, 3, 1, 5, 4, -5, 0x1c, 0x1d });
                    s_TextureFormatsValueAll = list.ToArray();
                }
                return s_TextureFormatsValueAll;
            }
        }

        private TextureImporterType textureType
        {
            get
            {
                if (this.textureTypeHasMultipleDifferentValues)
                {
                    return ~TextureImporterType.Image;
                }
                if (this.m_TextureType.intValue < 0)
                {
                    return (this.target as TextureImporter).textureType;
                }
                return (TextureImporterType) this.m_TextureType.intValue;
            }
        }

        private bool textureTypeHasMultipleDifferentValues
        {
            get
            {
                if (this.m_TextureType.hasMultipleDifferentValues)
                {
                    return true;
                }
                if (this.m_TextureType.intValue < 0)
                {
                    TextureImporterType textureType = (this.target as TextureImporter).textureType;
                    foreach (Object obj2 in base.targets)
                    {
                        if ((obj2 as TextureImporter).textureType != textureType)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private enum AdvancedTextureType
        {
            Default,
            NormalMap,
            LightMap
        }

        private enum CookieMode
        {
            Spot,
            Directional,
            Point
        }

        [Serializable]
        protected class PlatformSetting
        {
            [CompilerGenerated]
            private static Func<Object, TextureImporter> <>f__am$cacheD;
            [SerializeField]
            private int m_CompressionQuality;
            [SerializeField]
            private bool m_CompressionQualityIsDifferent;
            [SerializeField]
            private bool m_HasChanged;
            [SerializeField]
            private TextureImporter[] m_Importers;
            [SerializeField]
            private TextureImporterInspector m_Inspector;
            [SerializeField]
            private int m_MaxTextureSize;
            [SerializeField]
            private bool m_MaxTextureSizeIsDifferent;
            [SerializeField]
            private bool m_Overridden;
            [SerializeField]
            private bool m_OverriddenIsDifferent;
            [SerializeField]
            public BuildTarget m_Target;
            [SerializeField]
            private TextureImporterFormat[] m_TextureFormatArray;
            [SerializeField]
            private bool m_TextureFormatIsDifferent;
            [SerializeField]
            public string name;

            public PlatformSetting(string name, BuildTarget target, TextureImporterInspector inspector)
            {
                this.name = name;
                this.m_Target = target;
                this.m_Inspector = inspector;
                this.m_Overridden = false;
                if (<>f__am$cacheD == null)
                {
                    <>f__am$cacheD = new Func<Object, TextureImporter>(TextureImporterInspector.PlatformSetting.<PlatformSetting>m__12F);
                }
                this.m_Importers = inspector.targets.Select<Object, TextureImporter>(<>f__am$cacheD).ToArray<TextureImporter>();
                this.m_TextureFormatArray = new TextureImporterFormat[this.importers.Length];
                for (int i = 0; i < this.importers.Length; i++)
                {
                    TextureImporterFormat textureFormat;
                    int maxTextureSize;
                    int compressionQuality;
                    bool flag;
                    TextureImporter importer = this.importers[i];
                    if (!this.isDefault)
                    {
                        flag = importer.GetPlatformTextureSettings(name, out maxTextureSize, out textureFormat, out compressionQuality);
                    }
                    else
                    {
                        flag = true;
                        maxTextureSize = importer.maxTextureSize;
                        textureFormat = importer.textureFormat;
                        compressionQuality = importer.compressionQuality;
                    }
                    this.m_TextureFormatArray[i] = textureFormat;
                    if (i == 0)
                    {
                        this.m_Overridden = flag;
                        this.m_MaxTextureSize = maxTextureSize;
                        this.m_CompressionQuality = compressionQuality;
                    }
                    else
                    {
                        if (flag != this.m_Overridden)
                        {
                            this.m_OverriddenIsDifferent = true;
                        }
                        if (maxTextureSize != this.m_MaxTextureSize)
                        {
                            this.m_MaxTextureSizeIsDifferent = true;
                        }
                        if (compressionQuality != this.m_CompressionQuality)
                        {
                            this.m_CompressionQualityIsDifferent = true;
                        }
                        if (textureFormat != this.m_TextureFormatArray[0])
                        {
                            this.m_TextureFormatIsDifferent = true;
                        }
                    }
                }
                this.Sync();
            }

            [CompilerGenerated]
            private static TextureImporter <PlatformSetting>m__12F(Object x)
            {
                return (x as TextureImporter);
            }

            public void Apply()
            {
                for (int i = 0; i < this.importers.Length; i++)
                {
                    int maxTextureSize;
                    TextureImporter importer = this.importers[i];
                    int compressionQuality = -1;
                    bool overridden = false;
                    if (this.isDefault)
                    {
                        maxTextureSize = importer.maxTextureSize;
                    }
                    else
                    {
                        TextureImporterFormat format;
                        overridden = importer.GetPlatformTextureSettings(this.name, out maxTextureSize, out format, out compressionQuality);
                    }
                    if (!overridden)
                    {
                        maxTextureSize = importer.maxTextureSize;
                    }
                    if (!this.m_MaxTextureSizeIsDifferent)
                    {
                        maxTextureSize = this.m_MaxTextureSize;
                    }
                    if (!this.m_CompressionQualityIsDifferent)
                    {
                        compressionQuality = this.m_CompressionQuality;
                    }
                    if (!this.isDefault)
                    {
                        if (!this.m_OverriddenIsDifferent)
                        {
                            overridden = this.m_Overridden;
                        }
                        bool allowsAlphaSplit = false;
                        allowsAlphaSplit = importer.GetAllowsAlphaSplitting();
                        if (overridden)
                        {
                            importer.SetPlatformTextureSettings(this.name, maxTextureSize, this.m_TextureFormatArray[i], compressionQuality, allowsAlphaSplit);
                        }
                        else
                        {
                            importer.ClearPlatformTextureSettings(this.name);
                        }
                    }
                    else
                    {
                        importer.maxTextureSize = maxTextureSize;
                        importer.textureFormat = this.m_TextureFormatArray[i];
                        importer.compressionQuality = compressionQuality;
                    }
                }
            }

            private bool GetOverridden(TextureImporter importer)
            {
                int num;
                TextureImporterFormat format;
                if (!this.m_OverriddenIsDifferent)
                {
                    return this.m_Overridden;
                }
                return importer.GetPlatformTextureSettings(this.name, out num, out format);
            }

            public TextureImporterSettings GetSettings(TextureImporter importer)
            {
                TextureImporterSettings dest = new TextureImporterSettings();
                importer.ReadTextureSettings(dest);
                this.m_Inspector.GetSerializedPropertySettings(dest);
                return dest;
            }

            public virtual bool HasChanged()
            {
                return this.m_HasChanged;
            }

            public virtual void SetChanged()
            {
                this.m_HasChanged = true;
            }

            public void SetCompressionQualityForAll(int quality)
            {
                this.m_CompressionQuality = quality;
                this.m_CompressionQualityIsDifferent = false;
                this.m_HasChanged = true;
            }

            public void SetMaxTextureSizeForAll(int maxTextureSize)
            {
                this.m_MaxTextureSize = maxTextureSize;
                this.m_MaxTextureSizeIsDifferent = false;
                this.m_HasChanged = true;
            }

            public void SetOverriddenForAll(bool overridden)
            {
                this.m_Overridden = overridden;
                this.m_OverriddenIsDifferent = false;
                this.m_HasChanged = true;
            }

            public void SetTextureFormatForAll(TextureImporterFormat format)
            {
                for (int i = 0; i < this.m_TextureFormatArray.Length; i++)
                {
                    this.m_TextureFormatArray[i] = format;
                }
                this.m_TextureFormatIsDifferent = false;
                this.m_HasChanged = true;
            }

            public bool SupportsFormat(TextureImporterFormat format, TextureImporter importer)
            {
                int[] kTextureFormatsValueiPhone;
                TextureImporterSettings settings = this.GetSettings(importer);
                BuildTarget target = this.m_Target;
                switch (target)
                {
                    case BuildTarget.iOS:
                        kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueiPhone;
                        break;

                    case BuildTarget.Android:
                        kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueAndroid;
                        break;

                    case BuildTarget.StandaloneGLESEmu:
                        kTextureFormatsValueiPhone = !settings.normalMap ? TextureImporterInspector.kTextureFormatsValueGLESEmu : TextureImporterInspector.kNormalFormatsValueWeb;
                        break;

                    case BuildTarget.SamsungTV:
                        kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueSTV;
                        break;

                    case BuildTarget.WiiU:
                        kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueWiiU;
                        break;

                    default:
                        if (target == BuildTarget.BlackBerry)
                        {
                            kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueBB10;
                        }
                        else if (target == BuildTarget.Tizen)
                        {
                            kTextureFormatsValueiPhone = TextureImporterInspector.kTextureFormatsValueTizen;
                        }
                        else
                        {
                            kTextureFormatsValueiPhone = !settings.normalMap ? TextureImporterInspector.kTextureFormatsValueWeb : TextureImporterInspector.kNormalFormatsValueWeb;
                        }
                        break;
                }
                return kTextureFormatsValueiPhone.Contains((int) format);
            }

            public void Sync()
            {
                if (!this.isDefault && (!this.m_Overridden || this.m_OverriddenIsDifferent))
                {
                    TextureImporterInspector.PlatformSetting setting = this.m_Inspector.m_PlatformSettings[0];
                    this.m_MaxTextureSize = setting.m_MaxTextureSize;
                    this.m_MaxTextureSizeIsDifferent = setting.m_MaxTextureSizeIsDifferent;
                    this.m_TextureFormatArray = (TextureImporterFormat[]) setting.m_TextureFormatArray.Clone();
                    this.m_TextureFormatIsDifferent = setting.m_TextureFormatIsDifferent;
                    this.m_CompressionQuality = setting.m_CompressionQuality;
                    this.m_CompressionQualityIsDifferent = setting.m_CompressionQualityIsDifferent;
                }
                TextureImporterType textureType = this.m_Inspector.textureType;
                for (int i = 0; i < this.importers.Length; i++)
                {
                    TextureImporter importer = this.importers[i];
                    TextureImporterSettings settings = this.GetSettings(importer);
                    if (textureType == TextureImporterType.Advanced)
                    {
                        if (this.isDefault)
                        {
                            continue;
                        }
                        if (!this.SupportsFormat(this.m_TextureFormatArray[i], importer))
                        {
                            this.m_TextureFormatArray[i] = TextureImporter.FullToSimpleTextureFormat(this.m_TextureFormatArray[i]);
                        }
                        if (this.m_TextureFormatArray[i] < ~TextureImporterFormat.AutomaticCompressed)
                        {
                            this.m_TextureFormatArray[i] = TextureImporter.SimpleToFullTextureFormat2(this.m_TextureFormatArray[i], textureType, settings, importer.DoesSourceTextureHaveAlpha(), importer.IsSourceTextureHDR(), this.m_Target);
                        }
                    }
                    else if (this.m_TextureFormatArray[i] >= ~TextureImporterFormat.AutomaticCompressed)
                    {
                        this.m_TextureFormatArray[i] = TextureImporter.FullToSimpleTextureFormat(this.m_TextureFormatArray[i]);
                    }
                    if (settings.normalMap && !TextureImporterInspector.IsGLESMobileTargetPlatform(this.m_Target))
                    {
                        this.m_TextureFormatArray[i] = TextureImporterInspector.MakeTextureFormatHaveAlpha(this.m_TextureFormatArray[i]);
                    }
                }
                this.m_TextureFormatIsDifferent = false;
                foreach (TextureImporterFormat format in this.m_TextureFormatArray)
                {
                    if (format != this.m_TextureFormatArray[0])
                    {
                        this.m_TextureFormatIsDifferent = true;
                    }
                }
            }

            public bool allAreOverridden
            {
                get
                {
                    return (this.isDefault || (this.m_Overridden && !this.m_OverriddenIsDifferent));
                }
            }

            public int compressionQuality
            {
                get
                {
                    return this.m_CompressionQuality;
                }
            }

            public bool compressionQualityIsDifferent
            {
                get
                {
                    return this.m_CompressionQualityIsDifferent;
                }
            }

            public TextureImporter[] importers
            {
                get
                {
                    return this.m_Importers;
                }
            }

            public bool isDefault
            {
                get
                {
                    return (this.name == string.Empty);
                }
            }

            public int maxTextureSize
            {
                get
                {
                    return this.m_MaxTextureSize;
                }
            }

            public bool maxTextureSizeIsDifferent
            {
                get
                {
                    return this.m_MaxTextureSizeIsDifferent;
                }
            }

            public bool overridden
            {
                get
                {
                    return this.m_Overridden;
                }
            }

            public bool overriddenIsDifferent
            {
                get
                {
                    return this.m_OverriddenIsDifferent;
                }
            }

            public bool textureFormatIsDifferent
            {
                get
                {
                    return this.m_TextureFormatIsDifferent;
                }
            }

            public TextureImporterFormat[] textureFormats
            {
                get
                {
                    return this.m_TextureFormatArray;
                }
            }
        }

        private class Styles
        {
            public readonly GUIContent alphaIsTransparency;
            public readonly GUIContent borderMipMaps;
            public readonly GUIContent bumpFiltering = EditorGUIUtility.TextContent("Filtering");
            public readonly GUIContent[] bumpFilteringOptions = new GUIContent[] { EditorGUIUtility.TextContent("Sharp"), EditorGUIUtility.TextContent("Smooth") };
            public readonly GUIContent bumpiness = EditorGUIUtility.TextContent("Bumpiness");
            public readonly GUIContent compressionQuality;
            public readonly GUIContent compressionQualitySlider;
            public readonly GUIContent[] cookieOptions = new GUIContent[] { EditorGUIUtility.TextContent("Spotlight"), EditorGUIUtility.TextContent("Directional"), EditorGUIUtility.TextContent("Point") };
            public readonly GUIContent cookieType = EditorGUIUtility.TextContent("Light Type");
            public readonly GUIContent cubemap = EditorGUIUtility.TextContent("Mapping");
            public readonly GUIContent cubemapConvolution = EditorGUIUtility.TextContent("Convolution Type");
            public readonly GUIContent cubemapConvolutionExp;
            public readonly GUIContent[] cubemapConvolutionOptions = new GUIContent[] { EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("Specular (Glossy Reflection)|Convolve cubemap for specular reflections with varying smoothness (Glossy Reflections)."), EditorGUIUtility.TextContent("Diffuse (Irradiance)|Convolve cubemap for diffuse-only reflection (Irradiance Cubemap).") };
            public readonly GUIContent cubemapConvolutionSimple;
            public readonly GUIContent cubemapConvolutionSteps;
            public readonly int[] cubemapConvolutionValues;
            public readonly GUIContent[] cubemapOptions = new GUIContent[] { EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("Auto"), EditorGUIUtility.TextContent("6 Frames Layout (Cubic Environment)|Texture contains 6 images arranged in one of the standard cubemap layouts - cross or sequence (+x,-x, +y, -y, +z, -z). Texture can be in vertical or horizontal orientation."), EditorGUIUtility.TextContent("Latitude-Longitude Layout (Cylindrical)|Texture contains an image of a ball unwrapped such that latitude and longitude are mapped to horizontal and vertical dimensions (as on a globe)."), EditorGUIUtility.TextContent("Mirrored Ball (Spheremap)|Texture contains an image of a mirrored ball.") };
            public readonly int[] cubemapValues = new int[] { 0, 6, 5, 2, 1 };
            public readonly GUIContent defaultPlatform;
            public readonly GUIContent etc1Compression;
            public readonly GUIContent filterMode = EditorGUIUtility.TextContent("Filter Mode");
            public readonly GUIContent[] filterModeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Point (no filter)"), EditorGUIUtility.TextContent("Bilinear"), EditorGUIUtility.TextContent("Trilinear") };
            public readonly GUIContent generateAlphaFromGrayscale = EditorGUIUtility.TextContent("Alpha from Grayscale|Generate texture's alpha channel from grayscale.");
            public readonly GUIContent generateBumpmap = EditorGUIUtility.TextContent("Create bump map");
            public readonly GUIContent generateCubemap;
            public readonly GUIContent generateFromBump = EditorGUIUtility.TextContent("Create from Grayscale|The grayscale of the image is used as a heightmap for generating the normal map.");
            public readonly GUIContent generateMipMaps;
            public readonly GUIContent lightmap;
            public readonly GUIContent linearTexture;
            public readonly GUIContent maxSize;
            public readonly GUIContent mipmapFadeOut;
            public readonly GUIContent mipmapFadeOutToggle;
            public readonly GUIContent mipMapFilter;
            public readonly GUIContent[] mipMapFilterOptions;
            public readonly GUIContent mipMapsInLinearSpace;
            public readonly GUIContent[] mobileCompressionQualityOptions;
            public readonly GUIContent normalmap;
            public readonly GUIContent npot;
            public readonly GUIContent readWrite;
            public readonly GUIContent rgbmEncoding;
            public readonly GUIContent[] rgbmEncodingOptions;
            public readonly GUIContent seamlessCubemap;
            public readonly GUIContent spriteAlignment;
            public readonly GUIContent[] spriteAlignmentOptions;
            public readonly GUIContent spriteExtrude;
            public readonly GUIContent spriteMeshType;
            public readonly GUIContent[] spriteMeshTypeOptions;
            public readonly GUIContent spriteMode;
            public readonly GUIContent[] spriteModeOptions;
            public readonly GUIContent[] spriteModeOptionsAdvanced;
            public readonly GUIContent spritePackingTag;
            public readonly GUIContent spritePixelsPerUnit;
            public readonly GUIContent textureFormat;
            public readonly GUIContent[] textureFormatOptions;
            public readonly GUIContent textureType = EditorGUIUtility.TextContent("Texture Type|What will this texture be used for?");
            public readonly GUIContent[] textureTypeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Texture|Texture is a normal image such as a diffuse texture or other."), EditorGUIUtility.TextContent("Normal map|Texture is a bump or normal map."), EditorGUIUtility.TextContent("Editor GUI and Legacy GUI|Texture is used for a GUI element."), EditorGUIUtility.TextContent("Sprite (2D and UI)|Texture is used for a sprite."), EditorGUIUtility.TextContent("Cursor|Texture is used for a cursor."), EditorGUIUtility.TextContent("Cubemap|Texture is a cubemap."), EditorGUIUtility.TextContent("Cookie|Texture is a cookie you put on a light."), EditorGUIUtility.TextContent("Lightmap|Texture is a lightmap."), EditorGUIUtility.TextContent("Advanced|All settings displayed.") };

            public Styles()
            {
                int[] numArray1 = new int[3];
                numArray1[1] = 1;
                numArray1[2] = 2;
                this.cubemapConvolutionValues = numArray1;
                this.cubemapConvolutionSimple = EditorGUIUtility.TextContent("Glossy Reflection");
                this.cubemapConvolutionSteps = EditorGUIUtility.TextContent("Steps|Number of smoothness steps represented in mip maps of the cubemap.");
                this.cubemapConvolutionExp = EditorGUIUtility.TextContent("Exponent|Defines smoothness curve (x^exponent) for convolution.");
                this.seamlessCubemap = EditorGUIUtility.TextContent("Fixup Edge Seams|Enable if this texture is used for glossy reflections.");
                this.maxSize = EditorGUIUtility.TextContent("Max Size|Textures larger than this will be scaled down.");
                this.textureFormat = EditorGUIUtility.TextContent("Format");
                this.textureFormatOptions = new GUIContent[] { EditorGUIUtility.TextContent("Compressed"), EditorGUIUtility.TextContent("16 bits"), EditorGUIUtility.TextContent("Truecolor"), EditorGUIUtility.TextContent("Crunched") };
                this.defaultPlatform = EditorGUIUtility.TextContent("Default");
                this.mipmapFadeOutToggle = EditorGUIUtility.TextContent("Fadeout Mip Maps");
                this.mipmapFadeOut = EditorGUIUtility.TextContent("Fade Range");
                this.readWrite = EditorGUIUtility.TextContent("Read/Write Enabled|Enable to be able to access the raw pixel data from code.");
                this.rgbmEncoding = EditorGUIUtility.TextContent("Encode as RGBM|Encode texture as RGBM (for HDR textures).");
                this.rgbmEncodingOptions = new GUIContent[] { EditorGUIUtility.TextContent("Auto"), EditorGUIUtility.TextContent("On"), EditorGUIUtility.TextContent("Off"), EditorGUIUtility.TextContent("Encoded") };
                this.generateMipMaps = EditorGUIUtility.TextContent("Generate Mip Maps");
                this.mipMapsInLinearSpace = EditorGUIUtility.TextContent("In Linear Space|Perform mip map generation in linear space.");
                this.linearTexture = EditorGUIUtility.TextContent("Bypass sRGB Sampling|Texture will not be converted from gamma space to linear when sampled. Enable for IMGUI textures and non-color textures.");
                this.borderMipMaps = EditorGUIUtility.TextContent("Border Mip Maps");
                this.mipMapFilter = EditorGUIUtility.TextContent("Mip Map Filtering");
                this.mipMapFilterOptions = new GUIContent[] { EditorGUIUtility.TextContent("Box"), EditorGUIUtility.TextContent("Kaiser") };
                this.normalmap = EditorGUIUtility.TextContent("Normal Map|Enable if this texture is a normal map baked out of a 3D package.");
                this.npot = EditorGUIUtility.TextContent("Non Power of 2|How non-power-of-two textures are scaled on import.");
                this.generateCubemap = EditorGUIUtility.TextContent("Generate Cubemap");
                this.lightmap = EditorGUIUtility.TextContent("Lightmap|Enable if this is a lightmap (best if stored in EXR format).");
                this.compressionQuality = EditorGUIUtility.TextContent("Compression Quality");
                this.compressionQualitySlider = EditorGUIUtility.TextContent("Compression Quality|Use the slider to adjust compression quality from 0 (Fastest) to 100 (Best)");
                this.mobileCompressionQualityOptions = new GUIContent[] { EditorGUIUtility.TextContent("Fast"), EditorGUIUtility.TextContent("Normal"), EditorGUIUtility.TextContent("Best") };
                this.spriteMode = EditorGUIUtility.TextContent("Sprite Mode");
                this.spriteModeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Single"), EditorGUIUtility.TextContent("Multiple"), EditorGUIUtility.TextContent("Polygon") };
                this.spriteModeOptionsAdvanced = new GUIContent[] { EditorGUIUtility.TextContent("None"), EditorGUIUtility.TextContent("Single"), EditorGUIUtility.TextContent("Multiple"), EditorGUIUtility.TextContent("Polygon") };
                this.spriteMeshTypeOptions = new GUIContent[] { EditorGUIUtility.TextContent("Full Rect"), EditorGUIUtility.TextContent("Tight") };
                this.spritePackingTag = EditorGUIUtility.TextContent("Packing Tag|Tag for the Sprite Packing system.");
                this.spritePixelsPerUnit = EditorGUIUtility.TextContent("Pixels Per Unit|How many pixels in the sprite correspond to one unit in the world.");
                this.spriteExtrude = EditorGUIUtility.TextContent("Extrude Edges|How much empty area to leave around the sprite in the generated mesh.");
                this.spriteMeshType = EditorGUIUtility.TextContent("Mesh Type|Type of sprite mesh to generate.");
                this.spriteAlignment = EditorGUIUtility.TextContent("Pivot|Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
                this.spriteAlignmentOptions = new GUIContent[] { EditorGUIUtility.TextContent("Center"), EditorGUIUtility.TextContent("Top Left"), EditorGUIUtility.TextContent("Top"), EditorGUIUtility.TextContent("Top Right"), EditorGUIUtility.TextContent("Left"), EditorGUIUtility.TextContent("Right"), EditorGUIUtility.TextContent("Bottom Left"), EditorGUIUtility.TextContent("Bottom"), EditorGUIUtility.TextContent("Bottom Right"), EditorGUIUtility.TextContent("Custom") };
                this.alphaIsTransparency = EditorGUIUtility.TextContent("Alpha Is Transparency");
                this.etc1Compression = EditorGUIUtility.TextContent("Compress using ETC1 (split alpha channel)|This texture will be placed in an atlas that will be compressed using ETC1 compression, provided that the Texture Compression for Android build settings is set to 'ETC (default)'.");
            }
        }
    }
}

