namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(MovieImporter))]
    internal class MovieImporterInspector : AssetImporterInspector
    {
        private static GUIContent linearTextureContent = EditorGUIUtility.TextContent("Bypass sRGB Sampling|Texture will not be converted from gamma space to linear when sampled. Enable for IMGUI textures and non-color textures.");
        private float m_duration;
        private bool m_linearTexture;
        private float m_quality;

        internal override void Apply()
        {
            MovieImporter target = this.target as MovieImporter;
            target.quality = this.m_quality;
            target.linearTexture = this.m_linearTexture;
        }

        private double GetAudioBitrateForQuality(double f)
        {
            return (56000.0 + (200000.0 * f));
        }

        private double GetAudioQualityForBitrate(double f)
        {
            return ((f - 56000.0) / 200000.0);
        }

        private double GetVideoBitrateForQuality(double f)
        {
            return (100000.0 + (8000000.0 * f));
        }

        private double GetVideoQualityForBitrate(double f)
        {
            return ((f - 100000.0) / 8000000.0);
        }

        internal override bool HasModified()
        {
            MovieImporter target = this.target as MovieImporter;
            return ((target.quality != this.m_quality) || (target.linearTexture != this.m_linearTexture));
        }

        public override void OnInspectorGUI()
        {
            MovieImporter target = this.target as MovieImporter;
            if (target != null)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                this.m_linearTexture = EditorGUILayout.Toggle(linearTextureContent, this.m_linearTexture, new GUILayoutOption[0]);
                int num = (int) (this.GetVideoBitrateForQuality((double) this.m_quality) + this.GetAudioBitrateForQuality((double) this.m_quality));
                float num2 = (num / 8) * this.m_duration;
                float num3 = 1048576f;
                this.m_quality = EditorGUILayout.Slider("Quality", this.m_quality, 0f, 1f, new GUILayoutOption[0]);
                GUILayout.Label(string.Format("Approx. {0:0.00} " + ((num2 >= num3) ? "MB" : "kB") + ", {1} kbps", num2 / ((num2 >= num3) ? num3 : 1024f), num / 0x3e8), EditorStyles.helpBox, new GUILayoutOption[0]);
                GUILayout.EndVertical();
            }
            base.ApplyRevertGUI();
            MovieTexture texture = this.assetEditor.target as MovieTexture;
            if ((texture != null) && texture.loop)
            {
                EditorGUILayout.Space();
                texture.loop = EditorGUILayout.Toggle("Loop", texture.loop, new GUILayoutOption[0]);
                GUILayout.Label("The Loop setting in the Inspector is obsolete. Use the Scripting API to control looping instead.\n\nThe loop setting will be disabled on next re-import or by disabling it above.", EditorStyles.helpBox, new GUILayoutOption[0]);
            }
        }

        internal override void ResetValues()
        {
            MovieImporter target = this.target as MovieImporter;
            this.m_quality = target.quality;
            this.m_linearTexture = target.linearTexture;
            this.m_duration = target.duration;
        }

        internal override bool showImportedObject
        {
            get
            {
                return false;
            }
        }
    }
}

