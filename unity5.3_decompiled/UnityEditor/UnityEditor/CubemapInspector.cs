namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Cubemap))]
    internal class CubemapInspector : TextureInspector
    {
        private static readonly string[] kSizes = new string[] { "16", "32", "64", "128", "256", "512", "1024", "2048" };
        private static readonly int[] kSizesValues = new int[] { 0x10, 0x20, 0x40, 0x80, 0x100, 0x200, 0x400, 0x800 };
        private const int kTextureSize = 0x40;
        private Texture2D[] m_Images;

        private void InitTexturesFromCubemap()
        {
            Cubemap target = this.target as Cubemap;
            if (target != null)
            {
                if (this.m_Images == null)
                {
                    this.m_Images = new Texture2D[6];
                }
                for (int i = 0; i < this.m_Images.Length; i++)
                {
                    if ((this.m_Images[i] != null) && !EditorUtility.IsPersistent(this.m_Images[i]))
                    {
                        Object.DestroyImmediate(this.m_Images[i]);
                    }
                    if (TextureUtil.GetSourceTexture(target, (CubemapFace) i) != null)
                    {
                        this.m_Images[i] = TextureUtil.GetSourceTexture(target, (CubemapFace) i);
                    }
                    else
                    {
                        this.m_Images[i] = new Texture2D(0x40, 0x40, TextureFormat.ARGB32, false);
                        this.m_Images[i].hideFlags = HideFlags.HideAndDontSave;
                        TextureUtil.CopyCubemapFaceIntoTexture(target, (CubemapFace) i, this.m_Images[i]);
                    }
                }
            }
        }

        public static Object ObjectField(string label, Object obj, Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray1 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUI.Label(GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, 32f, EditorStyles.label, optionArray1), label, EditorStyles.label);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.Width(64f) };
            Object obj2 = EditorGUI.ObjectField(GUILayoutUtility.GetAspectRect(1f, EditorStyles.objectField, optionArray2), obj, objType, allowSceneObjects);
            GUILayout.EndHorizontal();
            return obj2;
        }

        internal override void OnAssetStoreInspectorGUI()
        {
            this.OnInspectorGUI();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (this.m_Images != null)
            {
                for (int i = 0; i < this.m_Images.Length; i++)
                {
                    if ((this.m_Images[i] != null) && !EditorUtility.IsPersistent(this.m_Images[i]))
                    {
                        Object.DestroyImmediate(this.m_Images[i]);
                    }
                }
            }
            this.m_Images = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.InitTexturesFromCubemap();
        }

        public override void OnInspectorGUI()
        {
            if (this.m_Images == null)
            {
                this.InitTexturesFromCubemap();
            }
            EditorGUIUtility.labelWidth = 50f;
            Cubemap target = this.target as Cubemap;
            if (target != null)
            {
                GUILayout.BeginVertical(new GUILayoutOption[0]);
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.ShowFace("Right\n(+X)", CubemapFace.PositiveX);
                this.ShowFace("Left\n(-X)", CubemapFace.NegativeX);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.ShowFace("Top\n(+Y)", CubemapFace.PositiveY);
                this.ShowFace("Bottom\n(-Y)", CubemapFace.NegativeY);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                this.ShowFace("Front\n(+Z)", CubemapFace.PositiveZ);
                this.ShowFace("Back\n(-Z)", CubemapFace.NegativeZ);
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                EditorGUIUtility.labelWidth = 0f;
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.HelpBox("Lowering face size is a destructive operation, you might need to re-assign the textures later to fix resolution issues. It's preferable to use Cubemap texture import type instead of Legacy Cubemap assets.", MessageType.Warning);
                int gLWidth = TextureUtil.GetGLWidth(target);
                gLWidth = EditorGUILayout.IntPopup("Face size", gLWidth, kSizes, kSizesValues, new GUILayoutOption[0]);
                int num2 = TextureUtil.CountMipmaps(target);
                bool useMipmap = EditorGUILayout.Toggle("MipMaps", num2 > 1, new GUILayoutOption[0]);
                bool linearSampled = TextureUtil.GetLinearSampled(target);
                linearSampled = EditorGUILayout.Toggle("Linear", linearSampled, new GUILayoutOption[0]);
                bool flag3 = TextureUtil.IsCubemapReadable(target);
                flag3 = EditorGUILayout.Toggle("Readable", flag3, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (TextureUtil.ReformatCubemap(ref target, gLWidth, gLWidth, target.format, useMipmap, linearSampled))
                    {
                        this.InitTexturesFromCubemap();
                    }
                    TextureUtil.MarkCubemapReadable(target, flag3);
                    target.Apply();
                }
            }
        }

        private void ShowFace(string label, CubemapFace face)
        {
            Cubemap target = this.target as Cubemap;
            int index = (int) face;
            GUI.changed = false;
            Texture2D textureRef = (Texture2D) ObjectField(label, this.m_Images[index], typeof(Texture2D), false, new GUILayoutOption[0]);
            if (GUI.changed)
            {
                TextureUtil.CopyTextureIntoCubemapFace(textureRef, target, face);
                this.m_Images[index] = textureRef;
            }
        }
    }
}

