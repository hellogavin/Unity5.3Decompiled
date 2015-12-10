namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class StandardShaderGUI : ShaderGUI
    {
        private MaterialProperty albedoColor;
        private MaterialProperty albedoMap;
        private MaterialProperty alphaCutoff;
        private MaterialProperty blendMode;
        private MaterialProperty bumpMap;
        private MaterialProperty bumpScale;
        private MaterialProperty detailAlbedoMap;
        private MaterialProperty detailMask;
        private MaterialProperty detailNormalMap;
        private MaterialProperty detailNormalMapScale;
        private MaterialProperty emissionColorForRendering;
        private MaterialProperty emissionMap;
        private MaterialProperty heightMap;
        private MaterialProperty heigtMapScale;
        private ColorPickerHDRConfig m_ColorPickerHDRConfig = new ColorPickerHDRConfig(0f, 99f, 0.01010101f, 3f);
        private bool m_FirstTimeApply = true;
        private MaterialEditor m_MaterialEditor;
        private WorkflowMode m_WorkflowMode;
        private MaterialProperty metallic;
        private MaterialProperty metallicMap;
        private MaterialProperty occlusionMap;
        private MaterialProperty occlusionStrength;
        private MaterialProperty smoothness;
        private MaterialProperty specularColor;
        private MaterialProperty specularMap;
        private MaterialProperty uvSetSecondary;

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material.HasProperty("_Emission"))
            {
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));
            }
            base.AssignNewShaderToMaterial(material, oldShader, newShader);
            if ((oldShader != null) && oldShader.name.Contains("Legacy Shaders/"))
            {
                BlendMode opaque = BlendMode.Opaque;
                if (oldShader.name.Contains("/Transparent/Cutout/"))
                {
                    opaque = BlendMode.Cutout;
                }
                else if (oldShader.name.Contains("/Transparent/"))
                {
                    opaque = BlendMode.Fade;
                }
                material.SetFloat("_Mode", (float) opaque);
                Material[] mats = new Material[] { material };
                this.DetermineWorkflow(MaterialEditor.GetMaterialProperties(mats));
                MaterialChanged(material, this.m_WorkflowMode);
            }
        }

        private void BlendModePopup()
        {
            EditorGUI.showMixedValue = this.blendMode.hasMixedValue;
            BlendMode floatValue = (BlendMode) ((int) this.blendMode.floatValue);
            EditorGUI.BeginChangeCheck();
            floatValue = (BlendMode) EditorGUILayout.Popup(Styles.renderingMode, (int) floatValue, Styles.blendNames, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                this.m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
                this.blendMode.floatValue = (float) floatValue;
            }
            EditorGUI.showMixedValue = false;
        }

        internal void DetermineWorkflow(MaterialProperty[] props)
        {
            if ((ShaderGUI.FindProperty("_SpecGlossMap", props, false) != null) && (ShaderGUI.FindProperty("_SpecColor", props, false) != null))
            {
                this.m_WorkflowMode = WorkflowMode.Specular;
            }
            else if ((ShaderGUI.FindProperty("_MetallicGlossMap", props, false) != null) && (ShaderGUI.FindProperty("_Metallic", props, false) != null))
            {
                this.m_WorkflowMode = WorkflowMode.Metallic;
            }
            else
            {
                this.m_WorkflowMode = WorkflowMode.Dielectric;
            }
        }

        private void DoAlbedoArea(Material material)
        {
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.albedoText, this.albedoMap, this.albedoColor);
            if (((int) material.GetFloat("_Mode")) == 1)
            {
                this.m_MaterialEditor.ShaderProperty(this.alphaCutoff, Styles.alphaCutoffText.text, 3);
            }
        }

        private void DoEmissionArea(Material material)
        {
            float maxColorComponent = this.emissionColorForRendering.colorValue.maxColorComponent;
            bool flag = !this.HasValidEmissiveKeyword(material);
            bool flag2 = maxColorComponent > 0f;
            bool flag3 = this.emissionMap.textureValue != null;
            this.m_MaterialEditor.TexturePropertyWithHDRColor(Styles.emissionText, this.emissionMap, this.emissionColorForRendering, this.m_ColorPickerHDRConfig, false);
            if (((this.emissionMap.textureValue != null) && !flag3) && (maxColorComponent <= 0f))
            {
                this.emissionColorForRendering.colorValue = Color.white;
            }
            if (flag2)
            {
                EditorGUI.BeginDisabledGroup(!ShouldEmissionBeEnabled(this.emissionColorForRendering.colorValue));
                this.m_MaterialEditor.LightmapEmissionProperty(3);
                EditorGUI.EndDisabledGroup();
            }
            if (flag)
            {
                EditorGUILayout.HelpBox(Styles.emissiveWarning.text, MessageType.Warning);
            }
        }

        private void DoSpecularMetallicArea()
        {
            if (this.m_WorkflowMode == WorkflowMode.Specular)
            {
                if (this.specularMap.textureValue == null)
                {
                    this.m_MaterialEditor.TexturePropertyTwoLines(Styles.specularMapText, this.specularMap, this.specularColor, Styles.smoothnessText, this.smoothness);
                }
                else
                {
                    this.m_MaterialEditor.TexturePropertySingleLine(Styles.specularMapText, this.specularMap);
                }
            }
            else if (this.m_WorkflowMode == WorkflowMode.Metallic)
            {
                if (this.metallicMap.textureValue == null)
                {
                    this.m_MaterialEditor.TexturePropertyTwoLines(Styles.metallicMapText, this.metallicMap, this.metallic, Styles.smoothnessText, this.smoothness);
                }
                else
                {
                    this.m_MaterialEditor.TexturePropertySingleLine(Styles.metallicMapText, this.metallicMap);
                }
            }
        }

        public void FindProperties(MaterialProperty[] props)
        {
            this.blendMode = ShaderGUI.FindProperty("_Mode", props);
            this.albedoMap = ShaderGUI.FindProperty("_MainTex", props);
            this.albedoColor = ShaderGUI.FindProperty("_Color", props);
            this.alphaCutoff = ShaderGUI.FindProperty("_Cutoff", props);
            this.specularMap = ShaderGUI.FindProperty("_SpecGlossMap", props, false);
            this.specularColor = ShaderGUI.FindProperty("_SpecColor", props, false);
            this.metallicMap = ShaderGUI.FindProperty("_MetallicGlossMap", props, false);
            this.metallic = ShaderGUI.FindProperty("_Metallic", props, false);
            if ((this.specularMap != null) && (this.specularColor != null))
            {
                this.m_WorkflowMode = WorkflowMode.Specular;
            }
            else if ((this.metallicMap != null) && (this.metallic != null))
            {
                this.m_WorkflowMode = WorkflowMode.Metallic;
            }
            else
            {
                this.m_WorkflowMode = WorkflowMode.Dielectric;
            }
            this.smoothness = ShaderGUI.FindProperty("_Glossiness", props);
            this.bumpScale = ShaderGUI.FindProperty("_BumpScale", props);
            this.bumpMap = ShaderGUI.FindProperty("_BumpMap", props);
            this.heigtMapScale = ShaderGUI.FindProperty("_Parallax", props);
            this.heightMap = ShaderGUI.FindProperty("_ParallaxMap", props);
            this.occlusionStrength = ShaderGUI.FindProperty("_OcclusionStrength", props);
            this.occlusionMap = ShaderGUI.FindProperty("_OcclusionMap", props);
            this.emissionColorForRendering = ShaderGUI.FindProperty("_EmissionColor", props);
            this.emissionMap = ShaderGUI.FindProperty("_EmissionMap", props);
            this.detailMask = ShaderGUI.FindProperty("_DetailMask", props);
            this.detailAlbedoMap = ShaderGUI.FindProperty("_DetailAlbedoMap", props);
            this.detailNormalMapScale = ShaderGUI.FindProperty("_DetailNormalMapScale", props);
            this.detailNormalMap = ShaderGUI.FindProperty("_DetailNormalMap", props);
            this.uvSetSecondary = ShaderGUI.FindProperty("_UVSec", props);
        }

        private bool HasValidEmissiveKeyword(Material material)
        {
            if (!material.IsKeywordEnabled("_EMISSION") && ShouldEmissionBeEnabled(this.emissionColorForRendering.colorValue))
            {
                return false;
            }
            return true;
        }

        private static void MaterialChanged(Material material, WorkflowMode workflowMode)
        {
            SetupMaterialWithBlendMode(material, (BlendMode) ((int) material.GetFloat("_Mode")));
            SetMaterialKeywords(material, workflowMode);
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
        {
            this.FindProperties(props);
            this.m_MaterialEditor = materialEditor;
            Material target = materialEditor.target as Material;
            this.ShaderPropertiesGUI(target);
            if (this.m_FirstTimeApply)
            {
                SetMaterialKeywords(target, this.m_WorkflowMode);
                this.m_FirstTimeApply = false;
            }
        }

        private static void SetKeyword(Material m, string keyword, bool state)
        {
            if (state)
            {
                m.EnableKeyword(keyword);
            }
            else
            {
                m.DisableKeyword(keyword);
            }
        }

        private static void SetMaterialKeywords(Material material, WorkflowMode workflowMode)
        {
            SetKeyword(material, "_NORMALMAP", (material.GetTexture("_BumpMap") != null) || ((bool) material.GetTexture("_DetailNormalMap")));
            if (workflowMode == WorkflowMode.Specular)
            {
                SetKeyword(material, "_SPECGLOSSMAP", (bool) material.GetTexture("_SpecGlossMap"));
            }
            else if (workflowMode == WorkflowMode.Metallic)
            {
                SetKeyword(material, "_METALLICGLOSSMAP", (bool) material.GetTexture("_MetallicGlossMap"));
            }
            SetKeyword(material, "_PARALLAXMAP", (bool) material.GetTexture("_ParallaxMap"));
            SetKeyword(material, "_DETAIL_MULX2", (material.GetTexture("_DetailAlbedoMap") != null) || ((bool) material.GetTexture("_DetailNormalMap")));
            bool state = ShouldEmissionBeEnabled(material.GetColor("_EmissionColor"));
            SetKeyword(material, "_EMISSION", state);
            MaterialGlobalIlluminationFlags globalIlluminationFlags = material.globalIlluminationFlags;
            if ((globalIlluminationFlags & (MaterialGlobalIlluminationFlags.BakedEmissive | MaterialGlobalIlluminationFlags.RealtimeEmissive)) != MaterialGlobalIlluminationFlags.None)
            {
                globalIlluminationFlags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                if (!state)
                {
                    globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                }
                material.globalIlluminationFlags = globalIlluminationFlags;
            }
        }

        public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Opaque:
                    material.SetOverrideTag("RenderType", string.Empty);
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;

                case BlendMode.Cutout:
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 0);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 0x992;
                    break;

                case BlendMode.Fade:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", 5);
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 0xbb8;
                    break;

                case BlendMode.Transparent:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", 1);
                    material.SetInt("_DstBlend", 10);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 0xbb8;
                    break;
            }
        }

        public void ShaderPropertiesGUI(Material material)
        {
            EditorGUIUtility.labelWidth = 0f;
            EditorGUI.BeginChangeCheck();
            this.BlendModePopup();
            GUILayout.Label(Styles.primaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.DoAlbedoArea(material);
            this.DoSpecularMetallicArea();
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.normalMapText, this.bumpMap, (this.bumpMap.textureValue == null) ? null : this.bumpScale);
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.heightMapText, this.heightMap, (this.heightMap.textureValue == null) ? null : this.heigtMapScale);
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.occlusionText, this.occlusionMap, (this.occlusionMap.textureValue == null) ? null : this.occlusionStrength);
            this.DoEmissionArea(material);
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.detailMaskText, this.detailMask);
            EditorGUI.BeginChangeCheck();
            this.m_MaterialEditor.TextureScaleOffsetProperty(this.albedoMap);
            if (EditorGUI.EndChangeCheck())
            {
                this.emissionMap.textureScaleAndOffset = this.albedoMap.textureScaleAndOffset;
            }
            EditorGUILayout.Space();
            GUILayout.Label(Styles.secondaryMapsText, EditorStyles.boldLabel, new GUILayoutOption[0]);
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.detailAlbedoText, this.detailAlbedoMap);
            this.m_MaterialEditor.TexturePropertySingleLine(Styles.detailNormalMapText, this.detailNormalMap, this.detailNormalMapScale);
            this.m_MaterialEditor.TextureScaleOffsetProperty(this.detailAlbedoMap);
            this.m_MaterialEditor.ShaderProperty(this.uvSetSecondary, Styles.uvSetLabel.text);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj2 in this.blendMode.targets)
                {
                    MaterialChanged((Material) obj2, this.m_WorkflowMode);
                }
            }
        }

        private static bool ShouldEmissionBeEnabled(Color color)
        {
            return (color.maxColorComponent > 0.0003921569f);
        }

        public enum BlendMode
        {
            Opaque,
            Cutout,
            Fade,
            Transparent
        }

        private static class Styles
        {
            public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");
            public static GUIContent alphaCutoffText = new GUIContent("Alpha Cutoff", "Threshold for alpha cutoff");
            public static readonly string[] blendNames = Enum.GetNames(typeof(StandardShaderGUI.BlendMode));
            public static GUIContent detailAlbedoText = new GUIContent("Detail Albedo x2", "Albedo (RGB) multiplied by 2");
            public static GUIContent detailMaskText = new GUIContent("Detail Mask", "Mask for Secondary Maps (A)");
            public static GUIContent detailNormalMapText = new GUIContent("Normal Map", "Normal Map");
            public static GUIContent emissionText = new GUIContent("Emission", "Emission (RGB)");
            public static GUIContent emissiveColorWarning = new GUIContent("Ensure emissive color is non-black for emission to have effect.");
            public static GUIContent emissiveWarning = new GUIContent("Emissive value is animated but the material has not been configured to support emissive. Please make sure the material itself has some amount of emissive.");
            public static string emptyTootip = string.Empty;
            public static GUIContent heightMapText = new GUIContent("Height Map", "Height Map (G)");
            public static GUIContent metallicMapText = new GUIContent("Metallic", "Metallic (R) and Smoothness (A)");
            public static GUIContent normalMapText = new GUIContent("Normal Map", "Normal Map");
            public static GUIContent occlusionText = new GUIContent("Occlusion", "Occlusion (G)");
            public static GUIStyle optionsButton = "PaneOptions";
            public static string primaryMapsText = "Main Maps";
            public static string renderingMode = "Rendering Mode";
            public static string secondaryMapsText = "Secondary Maps";
            public static GUIContent smoothnessText = new GUIContent("Smoothness", string.Empty);
            public static GUIContent specularMapText = new GUIContent("Specular", "Specular (RGB) and Smoothness (A)");
            public static GUIContent uvSetLabel = new GUIContent("UV Set");
            public static GUIContent[] uvSetOptions = new GUIContent[] { new GUIContent("UV channel 0"), new GUIContent("UV channel 1") };
            public static string whiteSpaceString = " ";
        }

        private enum WorkflowMode
        {
            Specular,
            Metallic,
            Dielectric
        }
    }
}

