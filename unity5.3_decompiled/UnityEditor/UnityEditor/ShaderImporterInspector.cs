namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [CustomEditor(typeof(ShaderImporter))]
    internal class ShaderImporterInspector : AssetImporterInspector
    {
        private List<ShaderUtil.ShaderPropertyTexDim> dimensions = new List<ShaderUtil.ShaderPropertyTexDim>();
        private List<string> displayNames = new List<string>();
        private List<string> propertyNames = new List<string>();
        private List<Texture> textures = new List<Texture>();

        internal override void Apply()
        {
            base.Apply();
            ShaderImporter target = this.target as ShaderImporter;
            if (target != null)
            {
                target.SetDefaultTextures(this.propertyNames.ToArray(), this.textures.ToArray());
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
            }
        }

        private static int GetNumberOfTextures(Shader shader)
        {
            int num = 0;
            int propertyCount = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < propertyCount; i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    num++;
                }
            }
            return num;
        }

        internal override bool HasModified()
        {
            if (base.HasModified())
            {
                return true;
            }
            ShaderImporter target = this.target as ShaderImporter;
            if (target != null)
            {
                Shader s = target.GetShader();
                if (s == null)
                {
                    return false;
                }
                int propertyCount = ShaderUtil.GetPropertyCount(s);
                for (int i = 0; i < propertyCount; i++)
                {
                    string propertyName = ShaderUtil.GetPropertyName(s, i);
                    for (int j = 0; j < this.propertyNames.Count; j++)
                    {
                        if ((this.propertyNames[j] == propertyName) && (this.textures[j] != target.GetDefaultTexture(propertyName)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void OnEnable()
        {
            this.ResetValues();
        }

        internal override void OnHeaderControlsGUI()
        {
            Shader target = this.assetEditor.target as Shader;
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open...", EditorStyles.miniButton, new GUILayoutOption[0]))
            {
                AssetDatabase.OpenAsset(target);
                GUIUtility.ExitGUI();
            }
        }

        public override void OnInspectorGUI()
        {
            ShaderImporter target = this.target as ShaderImporter;
            if (target != null)
            {
                Shader shader = target.GetShader();
                if (shader != null)
                {
                    if (GetNumberOfTextures(shader) != this.propertyNames.Count)
                    {
                        this.ResetValues();
                    }
                    this.ShowDefaultTextures();
                    base.ApplyRevertGUI();
                }
            }
        }

        internal override void ResetValues()
        {
            base.ResetValues();
            this.propertyNames = new List<string>();
            this.displayNames = new List<string>();
            this.textures = new List<Texture>();
            this.dimensions = new List<ShaderUtil.ShaderPropertyTexDim>();
            ShaderImporter target = this.target as ShaderImporter;
            if (target != null)
            {
                Shader s = target.GetShader();
                if (s != null)
                {
                    int propertyCount = ShaderUtil.GetPropertyCount(s);
                    for (int i = 0; i < propertyCount; i++)
                    {
                        if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                        {
                            string propertyName = ShaderUtil.GetPropertyName(s, i);
                            string propertyDescription = ShaderUtil.GetPropertyDescription(s, i);
                            Texture defaultTexture = target.GetDefaultTexture(propertyName);
                            this.propertyNames.Add(propertyName);
                            this.displayNames.Add(propertyDescription);
                            this.textures.Add(defaultTexture);
                            this.dimensions.Add(ShaderUtil.GetTexDim(s, i));
                        }
                    }
                }
            }
        }

        private void ShowDefaultTextures()
        {
            if (this.propertyNames.Count != 0)
            {
                EditorGUILayout.LabelField("Default Maps", EditorStyles.boldLabel, new GUILayoutOption[0]);
                for (int i = 0; i < this.propertyNames.Count; i++)
                {
                    Type type;
                    Texture texture = this.textures[i];
                    Texture texture2 = null;
                    EditorGUI.BeginChangeCheck();
                    switch (this.dimensions[i])
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
                    if (type != null)
                    {
                        string t = !string.IsNullOrEmpty(this.displayNames[i]) ? this.displayNames[i] : ObjectNames.NicifyVariableName(this.propertyNames[i]);
                        texture2 = EditorGUILayout.MiniThumbnailObjectField(GUIContent.Temp(t), texture, type, null, new GUILayoutOption[0]) as Texture;
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        this.textures[i] = texture2;
                    }
                }
            }
        }
    }
}

