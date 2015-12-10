namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(Texture2D))]
    internal class TextureInspector : Editor
    {
        protected SerializedProperty m_Aniso;
        private CubemapPreview m_CubemapPreview = new CubemapPreview();
        protected SerializedProperty m_FilterMode;
        [SerializeField]
        private float m_MipLevel;
        [SerializeField]
        protected Vector2 m_Pos;
        private bool m_ShowAlpha;
        protected SerializedProperty m_WrapMode;
        private static Styles s_Styles;

        internal static void DoAnisoGlobalSettingNote(int anisoLevel)
        {
            if (anisoLevel > 1)
            {
                if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
                {
                    EditorGUILayout.HelpBox("Anisotropic filtering is disabled for all textures in Quality Settings.", MessageType.Info);
                }
                else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
                {
                    EditorGUILayout.HelpBox("Anisotropic filtering is enabled for all textures in Quality Settings.", MessageType.Info);
                }
            }
        }

        protected void DoAnisoLevelSlider()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_Aniso.hasMultipleDifferentValues;
            int intValue = this.m_Aniso.intValue;
            intValue = EditorGUILayout.IntSlider("Aniso Level", intValue, 0, 0x10, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_Aniso.intValue = intValue;
            }
            DoAnisoGlobalSettingNote(intValue);
        }

        protected void DoFilterModePopup()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_FilterMode.hasMultipleDifferentValues;
            FilterMode intValue = (FilterMode) this.m_FilterMode.intValue;
            intValue = (FilterMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Filter Mode"), intValue, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_FilterMode.intValue = (int) intValue;
            }
        }

        protected void DoWrapModePopup()
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = this.m_WrapMode.hasMultipleDifferentValues;
            TextureWrapMode intValue = (TextureWrapMode) this.m_WrapMode.intValue;
            intValue = (TextureWrapMode) EditorGUILayout.EnumPopup(EditorGUIUtility.TempContent("Wrap Mode"), intValue, new GUILayoutOption[0]);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
            {
                this.m_WrapMode.intValue = (int) intValue;
            }
        }

        private void DrawRect(Rect rect)
        {
            GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0f));
            GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0f));
            GL.Vertex(new Vector3(rect.xMax, rect.yMin, 0f));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0f));
            GL.Vertex(new Vector3(rect.xMax, rect.yMax, 0f));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0f));
            GL.Vertex(new Vector3(rect.xMin, rect.yMax, 0f));
            GL.Vertex(new Vector3(rect.xMin, rect.yMin, 0f));
        }

        public override string GetInfoString()
        {
            Texture target = this.target as Texture;
            Texture2D t = this.target as Texture2D;
            TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(target)) as TextureImporter;
            string str = target.width.ToString() + "x" + target.height.ToString();
            if (QualitySettings.desiredColorSpace == ColorSpace.Linear)
            {
                str = str + " " + TextureUtil.GetTextureColorSpaceString(target);
            }
            bool flag = true;
            bool flag2 = (atPath != null) && atPath.qualifiesForSpritePacking;
            bool flag3 = IsNormalMap(target);
            bool flag4 = TextureUtil.DoesTextureStillNeedToBeCompressed(AssetDatabase.GetAssetPath(target));
            bool flag5 = (t != null) && TextureUtil.IsNonPowerOfTwo(t);
            TextureFormat textureFormat = TextureUtil.GetTextureFormat(target);
            flag = !flag4;
            if (flag5)
            {
                str = str + " (NPOT)";
            }
            if (flag4)
            {
                str = str + " (Not yet compressed)";
            }
            else if (!flag3)
            {
                if (flag2)
                {
                    TextureFormat format2;
                    ColorSpace space;
                    int num;
                    atPath.ReadTextureImportInstructions(EditorUserBuildSettings.activeBuildTarget, out format2, out space, out num);
                    string str2 = str;
                    string[] textArray1 = new string[] { str2, "\n ", TextureUtil.GetTextureFormatString(textureFormat), "(Original) ", TextureUtil.GetTextureFormatString(format2), "(Atlas)" };
                    str = string.Concat(textArray1);
                }
                else
                {
                    str = str + "  " + TextureUtil.GetTextureFormatString(textureFormat);
                }
            }
            else
            {
                TextureFormat format3 = textureFormat;
                switch (format3)
                {
                    case TextureFormat.ARGB4444:
                        str = str + "  Nm 16 bit";
                        goto Label_01E0;

                    case TextureFormat.ARGB32:
                        str = str + "  Nm 32 bit";
                        goto Label_01E0;
                }
                if (format3 == TextureFormat.DXT5)
                {
                    str = str + "  DXTnm";
                }
                else
                {
                    str = str + "  " + TextureUtil.GetTextureFormatString(textureFormat);
                }
            }
        Label_01E0:
            if (flag)
            {
                str = str + "\n" + EditorUtility.FormatBytes(TextureUtil.GetStorageMemorySize(target));
            }
            if (TextureUtil.GetUsageMode(target) != TextureUsageMode.AlwaysPadded)
            {
                return str;
            }
            int gLWidth = TextureUtil.GetGLWidth(target);
            int gLHeight = TextureUtil.GetGLHeight(target);
            if ((target.width == gLWidth) && (target.height == gLHeight))
            {
                return str;
            }
            return (str + string.Format("\nPadded to {0}x{1}", gLWidth, gLHeight));
        }

        public float GetMipLevelForRendering()
        {
            if (this.target == null)
            {
                return 0f;
            }
            if (this.IsCubemap())
            {
                return this.m_CubemapPreview.GetMipLevelForRendering(this.target as Texture);
            }
            return Mathf.Min(this.m_MipLevel, (float) (TextureUtil.CountMipmaps(this.target as Texture) - 1));
        }

        public override bool HasPreviewGUI()
        {
            return (this.target != null);
        }

        private bool IsCubemap()
        {
            RenderTexture target = this.target as RenderTexture;
            if ((target != null) && target.isCubemap)
            {
                return true;
            }
            Cubemap cubemap = this.target as Cubemap;
            return (cubemap != null);
        }

        public static bool IsNormalMap(Texture t)
        {
            TextureUsageMode usageMode = TextureUtil.GetUsageMode(t);
            return ((usageMode == TextureUsageMode.NormalmapPlain) || (usageMode == TextureUsageMode.NormalmapDXT5nm));
        }

        private float Log2(float x)
        {
            return (float) (Math.Log((double) x) / Math.Log(2.0));
        }

        protected virtual void OnDisable()
        {
            this.m_CubemapPreview.OnDisable();
        }

        protected virtual void OnEnable()
        {
            this.m_WrapMode = base.serializedObject.FindProperty("m_TextureSettings.m_WrapMode");
            this.m_FilterMode = base.serializedObject.FindProperty("m_TextureSettings.m_FilterMode");
            this.m_Aniso = base.serializedObject.FindProperty("m_TextureSettings.m_Aniso");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            this.DoWrapModePopup();
            this.DoFilterModePopup();
            this.DoAnisoLevelSlider();
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                background.Draw(r, false, false, false, false);
            }
            Texture target = this.target as Texture;
            RenderTexture texture2 = target as RenderTexture;
            if (texture2 != null)
            {
                if (!SystemInfo.SupportsRenderTextureFormat(texture2.format))
                {
                    return;
                }
                texture2.Create();
            }
            if (this.IsCubemap())
            {
                this.m_CubemapPreview.OnPreviewGUI(target, r, background);
            }
            else
            {
                int num = Mathf.Max(target.width, 1);
                int num2 = Mathf.Max(target.height, 1);
                float mipLevelForRendering = this.GetMipLevelForRendering();
                float num4 = Mathf.Min(Mathf.Min((float) (r.width / ((float) num)), (float) (r.height / ((float) num2))), 1f);
                Rect viewRect = new Rect(r.x, r.y, num * num4, num2 * num4);
                PreviewGUI.BeginScrollView(r, this.m_Pos, viewRect, "PreHorizontalScrollbar", "PreHorizontalScrollbarThumb");
                float mipMapBias = target.mipMapBias;
                TextureUtil.SetMipMapBiasNoDirty(target, mipLevelForRendering - this.Log2(((float) num) / viewRect.width));
                FilterMode filterMode = target.filterMode;
                TextureUtil.SetFilterModeNoDirty(target, FilterMode.Point);
                if (this.m_ShowAlpha)
                {
                    EditorGUI.DrawTextureAlpha(viewRect, target);
                }
                else
                {
                    Texture2D textured = target as Texture2D;
                    if ((textured != null) && textured.alphaIsTransparency)
                    {
                        EditorGUI.DrawTextureTransparent(viewRect, target);
                    }
                    else
                    {
                        EditorGUI.DrawPreviewTexture(viewRect, target);
                    }
                }
                if ((viewRect.width > 32f) && (viewRect.height > 32f))
                {
                    TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(target)) as TextureImporter;
                    SpriteMetaData[] dataArray = (atPath == null) ? null : atPath.spritesheet;
                    if ((dataArray != null) && (atPath.spriteImportMode == SpriteImportMode.Multiple))
                    {
                        Rect outScreenRect = new Rect();
                        Rect outSourceRect = new Rect();
                        GUI.CalculateScaledTextureRects(viewRect, ScaleMode.StretchToFill, ((float) target.width) / ((float) target.height), ref outScreenRect, ref outSourceRect);
                        int width = target.width;
                        int height = target.height;
                        atPath.GetWidthAndHeight(ref width, ref height);
                        float num8 = ((float) target.width) / ((float) width);
                        HandleUtility.ApplyWireMaterial();
                        GL.PushMatrix();
                        GL.MultMatrix(Handles.matrix);
                        GL.Begin(1);
                        GL.Color(new Color(1f, 1f, 1f, 0.5f));
                        foreach (SpriteMetaData data in dataArray)
                        {
                            Rect rect = data.rect;
                            Rect rect5 = new Rect {
                                xMin = outScreenRect.xMin + (outScreenRect.width * ((rect.xMin / ((float) target.width)) * num8)),
                                xMax = outScreenRect.xMin + (outScreenRect.width * ((rect.xMax / ((float) target.width)) * num8)),
                                yMin = outScreenRect.yMin + (outScreenRect.height * (1f - ((rect.yMin / ((float) target.height)) * num8))),
                                yMax = outScreenRect.yMin + (outScreenRect.height * (1f - ((rect.yMax / ((float) target.height)) * num8)))
                            };
                            this.DrawRect(rect5);
                        }
                        GL.End();
                        GL.PopMatrix();
                    }
                }
                TextureUtil.SetMipMapBiasNoDirty(target, mipMapBias);
                TextureUtil.SetFilterModeNoDirty(target, filterMode);
                this.m_Pos = PreviewGUI.EndScrollView();
                if (mipLevelForRendering != 0f)
                {
                    EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + mipLevelForRendering);
                }
            }
        }

        public override void OnPreviewSettings()
        {
            if (this.IsCubemap())
            {
                this.m_CubemapPreview.OnPreviewSettings(base.targets);
            }
            else
            {
                if (s_Styles == null)
                {
                    s_Styles = new Styles();
                }
                Texture target = this.target as Texture;
                bool flag = true;
                bool flag2 = false;
                bool flag3 = true;
                int a = 1;
                if ((this.target is Texture2D) || (this.target is ProceduralTexture))
                {
                    flag2 = true;
                    flag3 = false;
                }
                foreach (Texture texture2 in base.targets)
                {
                    TextureFormat format = (TextureFormat) 0;
                    bool flag4 = false;
                    if (texture2 is Texture2D)
                    {
                        format = (texture2 as Texture2D).format;
                        flag4 = true;
                    }
                    else if (texture2 is ProceduralTexture)
                    {
                        format = (texture2 as ProceduralTexture).format;
                        flag4 = true;
                    }
                    if (flag4)
                    {
                        if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
                        {
                            flag2 = false;
                        }
                        if (TextureUtil.HasAlphaTextureFormat(format) && (TextureUtil.GetUsageMode(texture2) == TextureUsageMode.Default))
                        {
                            flag3 = true;
                        }
                    }
                    a = Mathf.Max(a, TextureUtil.CountMipmaps(texture2));
                }
                if (flag2)
                {
                    this.m_ShowAlpha = true;
                    flag = false;
                }
                else if (!flag3)
                {
                    this.m_ShowAlpha = false;
                    flag = false;
                }
                if (flag && !IsNormalMap(target))
                {
                    this.m_ShowAlpha = GUILayout.Toggle(this.m_ShowAlpha, !this.m_ShowAlpha ? s_Styles.RGBIcon : s_Styles.alphaIcon, s_Styles.previewButton, new GUILayoutOption[0]);
                }
                GUI.enabled = a != 1;
                GUILayout.Box(s_Styles.smallZoom, s_Styles.previewLabel, new GUILayoutOption[0]);
                GUI.changed = false;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(64f) };
                this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float) (a - 1), 0f, s_Styles.previewSlider, s_Styles.previewSliderThumb, options));
                GUILayout.Box(s_Styles.largeZoom, s_Styles.previewLabel, new GUILayoutOption[0]);
                GUI.enabled = true;
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            Texture2D textured;
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            Texture target = this.target as Texture;
            if (this.IsCubemap())
            {
                return this.m_CubemapPreview.RenderStaticPreview(target, width, height);
            }
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if ((atPath != null) && (atPath.spriteImportMode == SpriteImportMode.Polygon))
            {
                Sprite sprite = subAssets[0] as Sprite;
                if (sprite != null)
                {
                    return SpriteInspector.BuildPreviewTexture(width, height, sprite, null, true);
                }
            }
            PreviewHelpers.AdjustWidthAndHeightForStaticPreview(target.width, target.height, ref width, ref height);
            RenderTexture active = RenderTexture.active;
            Rect rawViewportRect = ShaderUtil.rawViewportRect;
            bool flag = !TextureUtil.GetLinearSampled(target);
            RenderTexture dest = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, !flag ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
            Material materialForSpecialTexture = EditorGUI.GetMaterialForSpecialTexture(target);
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            if (materialForSpecialTexture != null)
            {
                if (Unsupported.IsDeveloperBuild())
                {
                    materialForSpecialTexture = new Material(materialForSpecialTexture);
                }
                Graphics.Blit(target, dest, materialForSpecialTexture);
            }
            else
            {
                Graphics.Blit(target, dest);
            }
            GL.sRGBWrite = false;
            RenderTexture.active = dest;
            Texture2D textured2 = this.target as Texture2D;
            if ((textured2 != null) && textured2.alphaIsTransparency)
            {
                textured = new Texture2D(width, height, TextureFormat.ARGB32, false);
            }
            else
            {
                textured = new Texture2D(width, height, TextureFormat.RGB24, false);
            }
            textured.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
            textured.Apply();
            RenderTexture.ReleaseTemporary(dest);
            EditorGUIUtility.SetRenderTextureNoViewport(active);
            ShaderUtil.rawViewportRect = rawViewportRect;
            if ((materialForSpecialTexture != null) && Unsupported.IsDeveloperBuild())
            {
                Object.DestroyImmediate(materialForSpecialTexture);
            }
            return textured;
        }

        internal void SetCubemapIntensity(float intensity)
        {
            if (this.m_CubemapPreview != null)
            {
                this.m_CubemapPreview.SetIntensity(intensity);
            }
        }

        public float mipLevel
        {
            get
            {
                if (this.IsCubemap())
                {
                    return this.m_CubemapPreview.mipLevel;
                }
                return this.m_MipLevel;
            }
            set
            {
                this.m_CubemapPreview.mipLevel = value;
                this.m_MipLevel = value;
            }
        }

        private class Styles
        {
            public GUIContent alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
            public GUIContent largeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
            public GUIStyle previewButton = "preButton";
            public GUIStyle previewLabel = new GUIStyle("preLabel");
            public GUIStyle previewSlider = "preSlider";
            public GUIStyle previewSliderThumb = "preSliderThumb";
            public GUIContent RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
            public GUIContent smallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");

            public Styles()
            {
                this.previewLabel.alignment = TextAnchor.UpperCenter;
            }
        }
    }
}

