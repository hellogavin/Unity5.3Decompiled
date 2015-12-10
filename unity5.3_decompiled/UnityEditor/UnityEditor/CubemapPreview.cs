namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class CubemapPreview
    {
        private float m_Intensity = 1f;
        private Mesh m_Mesh;
        [SerializeField]
        private float m_MipLevel;
        public Vector2 m_PreviewDir = new Vector2(0f, 0f);
        [SerializeField]
        private PreviewType m_PreviewType;
        private PreviewRenderUtility m_PreviewUtility;

        public float GetMipLevelForRendering(Texture texture)
        {
            return Mathf.Min(this.m_MipLevel, (float) TextureUtil.CountMipmaps(texture));
        }

        private void InitPreview()
        {
            if (this.m_PreviewUtility == null)
            {
                PreviewRenderUtility utility = new PreviewRenderUtility {
                    m_CameraFieldOfView = 15f
                };
                this.m_PreviewUtility = utility;
                this.m_Mesh = PreviewRenderUtility.GetPreviewSphere();
            }
        }

        public void OnDisable()
        {
            if (this.m_PreviewUtility != null)
            {
                this.m_PreviewUtility.Cleanup();
                this.m_PreviewUtility = null;
            }
        }

        public void OnPreviewGUI(Texture t, Rect r, GUIStyle background)
        {
            if (t != null)
            {
                if (!ShaderUtil.hardwareSupportsRectRenderTexture)
                {
                    if (Event.current.type == EventType.Repaint)
                    {
                        EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 40f), "Cubemap preview requires\nrender texture support");
                    }
                }
                else
                {
                    this.m_PreviewDir = PreviewGUI.Drag2D(this.m_PreviewDir, r);
                    if (Event.current.type == EventType.Repaint)
                    {
                        this.InitPreview();
                        this.m_PreviewUtility.BeginPreview(r, background);
                        this.RenderCubemap(t, this.m_PreviewDir, 6f);
                        Texture image = this.m_PreviewUtility.EndPreview();
                        GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
                        if (this.mipLevel != 0f)
                        {
                            EditorGUI.DropShadowLabel(new Rect(r.x, r.y, r.width, 20f), "Mip " + this.mipLevel);
                        }
                    }
                }
            }
        }

        public void OnPreviewSettings(Object[] targets)
        {
            if (ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                GUI.enabled = true;
                this.InitPreview();
                bool flag = true;
                bool flag2 = true;
                bool flag3 = false;
                int a = 8;
                foreach (Texture texture in targets)
                {
                    a = Mathf.Max(a, TextureUtil.CountMipmaps(texture));
                    Cubemap cubemap = texture as Cubemap;
                    if (cubemap != null)
                    {
                        TextureFormat format = cubemap.format;
                        if (!TextureUtil.IsAlphaOnlyTextureFormat(format))
                        {
                            flag2 = false;
                        }
                        if (TextureUtil.HasAlphaTextureFormat(format) && (TextureUtil.GetUsageMode(texture) == TextureUsageMode.Default))
                        {
                            flag3 = true;
                        }
                    }
                    else
                    {
                        flag3 = true;
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    this.m_PreviewType = PreviewType.Alpha;
                    flag = false;
                }
                else if (!flag3)
                {
                    this.m_PreviewType = PreviewType.RGB;
                    flag = false;
                }
                if (flag)
                {
                    GUIContent[] contentArray = new GUIContent[] { Styles.RGBIcon, Styles.alphaIcon };
                    int previewType = (int) this.m_PreviewType;
                    if (GUILayout.Button(contentArray[previewType], Styles.preButton, new GUILayoutOption[0]))
                    {
                        this.m_PreviewType = (PreviewType) (++previewType % contentArray.Length);
                    }
                }
                GUI.enabled = a != 1;
                GUILayout.Box(Styles.smallZoom, Styles.preLabel, new GUILayoutOption[0]);
                GUI.changed = false;
                GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(64f) };
                this.m_MipLevel = Mathf.Round(GUILayout.HorizontalSlider(this.m_MipLevel, (float) (a - 1), 0f, Styles.preSlider, Styles.preSliderThumb, options));
                GUILayout.Box(Styles.largeZoom, Styles.preLabel, new GUILayoutOption[0]);
                GUI.enabled = true;
            }
        }

        private void RenderCubemap(Texture t, Vector2 previewDir, float previewDistance)
        {
            bool fog = RenderSettings.fog;
            Unsupported.SetRenderSettingsUseFogNoDirty(false);
            this.m_PreviewUtility.m_Camera.transform.position = (Vector3) (-Vector3.forward * previewDistance);
            this.m_PreviewUtility.m_Camera.transform.rotation = Quaternion.identity;
            Quaternion q = Quaternion.Euler(previewDir.y, 0f, 0f) * Quaternion.Euler(0f, previewDir.x, 0f);
            Material mat = EditorGUIUtility.LoadRequired("Previews/PreviewCubemapMaterial.mat") as Material;
            mat.mainTexture = t;
            mat.SetMatrix("_CubemapRotation", Matrix4x4.TRS(Vector3.zero, q, Vector3.one));
            float mipLevelForRendering = this.GetMipLevelForRendering(t);
            mat.SetFloat("_Mip", mipLevelForRendering);
            mat.SetFloat("_Alpha", (this.m_PreviewType != PreviewType.Alpha) ? 0f : 1f);
            mat.SetFloat("_Intensity", this.m_Intensity);
            this.m_PreviewUtility.DrawMesh(this.m_Mesh, Vector3.zero, q, mat, 0);
            this.m_PreviewUtility.m_Camera.Render();
            Unsupported.SetRenderSettingsUseFogNoDirty(fog);
        }

        public Texture2D RenderStaticPreview(Texture t, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            this.InitPreview();
            this.m_PreviewUtility.BeginStaticPreview(new Rect(0f, 0f, (float) width, (float) height));
            Vector2 previewDir = new Vector2(0f, 0f);
            this.RenderCubemap(t, previewDir, 5.3f);
            return this.m_PreviewUtility.EndStaticPreview();
        }

        public void SetIntensity(float intensity)
        {
            this.m_Intensity = intensity;
        }

        public float mipLevel
        {
            get
            {
                return this.m_MipLevel;
            }
            set
            {
                this.m_MipLevel = value;
            }
        }

        private enum PreviewType
        {
            RGB,
            Alpha
        }

        private static class Styles
        {
            public static GUIContent alphaIcon = EditorGUIUtility.IconContent("PreTextureAlpha");
            public static GUIContent largeZoom = EditorGUIUtility.IconContent("PreTextureMipMapHigh");
            public static GUIStyle preButton = "preButton";
            public static GUIStyle preLabel = "preLabel";
            public static GUIStyle preSlider = "preSlider";
            public static GUIStyle preSliderThumb = "preSliderThumb";
            public static GUIContent RGBIcon = EditorGUIUtility.IconContent("PreTextureRGB");
            public static GUIContent smallZoom = EditorGUIUtility.IconContent("PreTextureMipMapLow");
        }
    }
}

