namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TerrainSplatEditor : EditorWindow
    {
        private string m_ButtonTitle = string.Empty;
        private int m_Index = -1;
        private float m_Metallic;
        public Texture2D m_NormalMap;
        private Vector2 m_ScrollPosition;
        private float m_Smoothness;
        private Color m_Specular;
        private Terrain m_Terrain;
        public Texture2D m_Texture;
        private Vector2 m_TileOffset;
        private Vector2 m_TileSize;

        public TerrainSplatEditor()
        {
            base.position = new Rect(50f, 50f, 200f, 300f);
            base.minSize = new Vector2(200f, 300f);
        }

        private void ApplyTerrainSplat()
        {
            if ((this.m_Terrain != null) && (this.m_Terrain.terrainData != null))
            {
                SplatPrototype[] splatPrototypes = this.m_Terrain.terrainData.splatPrototypes;
                if (this.m_Index == -1)
                {
                    SplatPrototype[] destinationArray = new SplatPrototype[splatPrototypes.Length + 1];
                    Array.Copy(splatPrototypes, 0, destinationArray, 0, splatPrototypes.Length);
                    this.m_Index = splatPrototypes.Length;
                    splatPrototypes = destinationArray;
                    splatPrototypes[this.m_Index] = new SplatPrototype();
                }
                splatPrototypes[this.m_Index].texture = this.m_Texture;
                splatPrototypes[this.m_Index].normalMap = this.m_NormalMap;
                splatPrototypes[this.m_Index].tileSize = this.m_TileSize;
                splatPrototypes[this.m_Index].tileOffset = this.m_TileOffset;
                splatPrototypes[this.m_Index].specular = this.m_Specular;
                splatPrototypes[this.m_Index].metallic = this.m_Metallic;
                splatPrototypes[this.m_Index].smoothness = this.m_Smoothness;
                this.m_Terrain.terrainData.splatPrototypes = splatPrototypes;
                EditorUtility.SetDirty(this.m_Terrain);
            }
        }

        private void InitializeData(Terrain terrain, int index)
        {
            SplatPrototype prototype;
            this.m_Terrain = terrain;
            this.m_Index = index;
            if (index == -1)
            {
                prototype = new SplatPrototype();
            }
            else
            {
                prototype = this.m_Terrain.terrainData.splatPrototypes[index];
            }
            this.m_Texture = prototype.texture;
            this.m_NormalMap = prototype.normalMap;
            this.m_TileSize = prototype.tileSize;
            this.m_TileOffset = prototype.tileOffset;
            this.m_Specular = prototype.specular;
            this.m_Metallic = prototype.metallic;
            this.m_Smoothness = prototype.smoothness;
        }

        private static bool IsUsingMetallic(Terrain.MaterialType materialType, Material materialTemplate)
        {
            return ((materialType == Terrain.MaterialType.BuiltInStandard) || (((materialType == Terrain.MaterialType.Custom) && (materialTemplate != null)) && materialTemplate.HasProperty("_Metallic0")));
        }

        private static bool IsUsingSmoothness(Terrain.MaterialType materialType, Material materialTemplate)
        {
            return ((materialType == Terrain.MaterialType.BuiltInStandard) || (((materialType == Terrain.MaterialType.Custom) && (materialTemplate != null)) && materialTemplate.HasProperty("_Smoothness0")));
        }

        private static bool IsUsingSpecular(Terrain.MaterialType materialType, Material materialTemplate)
        {
            return ((materialType == Terrain.MaterialType.BuiltInStandard) || (((materialType == Terrain.MaterialType.Custom) && (materialTemplate != null)) && materialTemplate.HasProperty("_Specular0")));
        }

        private void OnGUI()
        {
            EditorGUIUtility.labelWidth = (base.position.width - 64f) - 20f;
            bool flag = true;
            this.m_ScrollPosition = EditorGUILayout.BeginVerticalScrollView(this.m_ScrollPosition, false, GUI.skin.verticalScrollbar, GUI.skin.scrollView, new GUILayoutOption[0]);
            flag &= this.ValidateTerrain();
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            string label = string.Empty;
            float alignmentOffset = 0f;
            switch (this.m_Terrain.materialType)
            {
                case Terrain.MaterialType.BuiltInStandard:
                    label = " Albedo (RGB)\nSmoothness (A)";
                    alignmentOffset = 15f;
                    break;

                case Terrain.MaterialType.BuiltInLegacyDiffuse:
                    label = "\n Diffuse (RGB)";
                    alignmentOffset = 15f;
                    break;

                case Terrain.MaterialType.BuiltInLegacySpecular:
                    label = "Diffuse (RGB)\n   Gloss (A)";
                    alignmentOffset = 12f;
                    break;

                case Terrain.MaterialType.Custom:
                    label = " \n  Splat";
                    alignmentOffset = 0f;
                    break;
            }
            TextureFieldGUI(label, ref this.m_Texture, alignmentOffset);
            TextureFieldGUI("\nNormal", ref this.m_NormalMap, -4f);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            flag &= this.ValidateMainTexture(this.m_Texture);
            if (flag)
            {
                if (IsUsingMetallic(this.m_Terrain.materialType, this.m_Terrain.materialTemplate))
                {
                    EditorGUILayout.Space();
                    float labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 75f;
                    this.m_Metallic = EditorGUILayout.Slider("Metallic", this.m_Metallic, 0f, 1f, new GUILayoutOption[0]);
                    EditorGUIUtility.labelWidth = labelWidth;
                }
                else if (IsUsingSpecular(this.m_Terrain.materialType, this.m_Terrain.materialTemplate))
                {
                    this.m_Specular = EditorGUILayout.ColorField("Specular", this.m_Specular, new GUILayoutOption[0]);
                }
                if (IsUsingSmoothness(this.m_Terrain.materialType, this.m_Terrain.materialTemplate) && !TextureUtil.HasAlphaTextureFormat(this.m_Texture.format))
                {
                    EditorGUILayout.Space();
                    float num4 = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 75f;
                    this.m_Smoothness = EditorGUILayout.Slider("Smoothness", this.m_Smoothness, 0f, 1f, new GUILayoutOption[0]);
                    EditorGUIUtility.labelWidth = num4;
                }
            }
            SplatSizeGUI(ref this.m_TileSize, ref this.m_TileOffset);
            bool flag2 = EditorGUI.EndChangeCheck();
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.FlexibleSpace();
            GUI.enabled = flag;
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MinWidth(100f) };
            if (GUILayout.Button(this.m_ButtonTitle, options))
            {
                this.ApplyTerrainSplat();
                base.Close();
                GUIUtility.ExitGUI();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if ((flag2 && flag) && (this.m_Index != -1))
            {
                this.ApplyTerrainSplat();
            }
        }

        internal static void ShowTerrainSplatEditor(string title, string button, Terrain terrain, int index)
        {
            TerrainSplatEditor window = EditorWindow.GetWindow<TerrainSplatEditor>(true, title);
            window.m_ButtonTitle = button;
            window.InitializeData(terrain, index);
        }

        private static void SplatSizeGUI(ref Vector2 scale, ref Vector2 offset)
        {
            GUILayoutOption option = GUILayout.Width(10f);
            GUILayoutOption option2 = GUILayout.MinWidth(32f);
            GUILayout.Space(6f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { option };
            GUILayout.Label(string.Empty, EditorStyles.miniLabel, options);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { option };
            GUILayout.Label("x", EditorStyles.miniLabel, optionArray2);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { option };
            GUILayout.Label("y", EditorStyles.miniLabel, optionArray3);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Size", EditorStyles.miniLabel, new GUILayoutOption[0]);
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { option2 };
            scale.x = EditorGUILayout.FloatField(scale.x, EditorStyles.miniTextField, optionArray4);
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { option2 };
            scale.y = EditorGUILayout.FloatField(scale.y, EditorStyles.miniTextField, optionArray5);
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            GUILayout.Label("Offset", EditorStyles.miniLabel, new GUILayoutOption[0]);
            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { option2 };
            offset.x = EditorGUILayout.FloatField(offset.x, EditorStyles.miniTextField, optionArray6);
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { option2 };
            offset.y = EditorGUILayout.FloatField(offset.y, EditorStyles.miniTextField, optionArray7);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private static void TextureFieldGUI(string label, ref Texture2D texture, float alignmentOffset)
        {
            GUILayout.Space(6f);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Width(80f) };
            GUILayout.BeginVertical(options);
            GUILayout.Label(label, new GUILayoutOption[0]);
            Type objType = typeof(Texture2D);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.MaxWidth(64f) };
            Rect position = GUILayoutUtility.GetRect(64f, 64f, 64f, 64f, optionArray2);
            position.x += alignmentOffset;
            texture = EditorGUI.DoObjectField(position, position, GUIUtility.GetControlID(0x3042, EditorGUIUtility.native, position), texture, objType, null, null, false) as Texture2D;
            GUILayout.EndVertical();
        }

        private bool ValidateMainTexture(Texture2D tex)
        {
            if (tex == null)
            {
                EditorGUILayout.HelpBox("Assign a tiling texture", MessageType.Warning);
                return false;
            }
            if (tex.wrapMode != TextureWrapMode.Repeat)
            {
                EditorGUILayout.HelpBox("Texture wrap mode must be set to Repeat", MessageType.Warning);
                return false;
            }
            if ((tex.width != Mathf.ClosestPowerOfTwo(tex.width)) || (tex.height != Mathf.ClosestPowerOfTwo(tex.height)))
            {
                EditorGUILayout.HelpBox("Texture size must be power of two", MessageType.Warning);
                return false;
            }
            if (tex.mipmapCount <= 1)
            {
                EditorGUILayout.HelpBox("Texture must have mip maps", MessageType.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateTerrain()
        {
            if ((this.m_Terrain != null) && (this.m_Terrain.terrainData != null))
            {
                return true;
            }
            EditorGUILayout.HelpBox("Terrain does not exist", MessageType.Error);
            return false;
        }
    }
}

