namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(SpriteRenderer))]
    internal class SpriteRendererEditor : RendererEditorBase
    {
        private SerializedProperty m_Color;
        private GUIContent m_FlipLabel = EditorGUIUtility.TextContent("Flip");
        private SerializedProperty m_FlipX;
        private SerializedProperty m_FlipY;
        private SerializedProperty m_Material;
        private GUIContent m_MaterialStyle = EditorGUIUtility.TextContent("Material");
        private SerializedProperty m_Sprite;
        private static Texture2D s_WarningIcon;

        private void CheckForErrors()
        {
            bool flag;
            if (this.IsMaterialTextureAtlasConflict())
            {
                ShowError("Material has CanUseSpriteAtlas=False tag. Sprite texture has atlasHint set. Rendering artifacts possible.");
            }
            if (!this.DoesMaterialHaveSpriteTexture(out flag))
            {
                ShowError("Material does not have a _MainTex texture property. It is required for SpriteRenderer.");
            }
            else if (flag)
            {
                ShowError("Material texture property _MainTex has offset/scale set. It is incompatible with SpriteRenderer.");
            }
        }

        private bool DoesMaterialHaveSpriteTexture(out bool tiled)
        {
            tiled = false;
            Material sharedMaterial = (this.target as SpriteRenderer).sharedMaterial;
            if (sharedMaterial == null)
            {
                return true;
            }
            if (sharedMaterial.HasProperty("_MainTex"))
            {
                Vector2 textureOffset = sharedMaterial.GetTextureOffset("_MainTex");
                Vector2 textureScale = sharedMaterial.GetTextureScale("_MainTex");
                if (((textureOffset.x != 0f) || (textureOffset.y != 0f)) || ((textureScale.x != 1f) || (textureScale.y != 1f)))
                {
                    tiled = true;
                }
            }
            return sharedMaterial.HasProperty("_MainTex");
        }

        private void FlipToggle(Rect r, string label, SerializedProperty property)
        {
            bool boolValue = property.boolValue;
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            boolValue = EditorGUI.ToggleLeft(r, label, boolValue);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Edit Constraints");
                property.boolValue = boolValue;
            }
            EditorGUI.showMixedValue = false;
        }

        private void FlipToggles()
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x211c, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, this.m_FlipLabel);
            position.width = 30f;
            this.FlipToggle(position, "X", this.m_FlipX);
            position.x += 30f;
            this.FlipToggle(position, "Y", this.m_FlipY);
            GUILayout.EndHorizontal();
        }

        private bool IsMaterialTextureAtlasConflict()
        {
            Material sharedMaterial = (this.target as SpriteRenderer).sharedMaterial;
            if ((sharedMaterial != null) && (sharedMaterial.GetTag("CanUseSpriteAtlas", false).ToLower() == "false"))
            {
                Sprite objectReferenceValue = this.m_Sprite.objectReferenceValue as Sprite;
                TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(objectReferenceValue)) as TextureImporter;
                if ((atPath.spritePackingTag != null) && (atPath.spritePackingTag.Length > 0))
                {
                    return true;
                }
            }
            return false;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Sprite = base.serializedObject.FindProperty("m_Sprite");
            this.m_Color = base.serializedObject.FindProperty("m_Color");
            this.m_FlipX = base.serializedObject.FindProperty("m_FlipX");
            this.m_FlipY = base.serializedObject.FindProperty("m_FlipY");
            this.m_Material = base.serializedObject.FindProperty("m_Materials.Array");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            EditorGUILayout.PropertyField(this.m_Sprite, new GUILayoutOption[0]);
            EditorGUILayout.PropertyField(this.m_Color, true, new GUILayoutOption[0]);
            this.FlipToggles();
            if (this.m_Material.arraySize == 0)
            {
                this.m_Material.InsertArrayElementAtIndex(0);
            }
            Rect position = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, (float) 16f, (float) 16f);
            EditorGUI.showMixedValue = this.m_Material.hasMultipleDifferentValues;
            Object objectReferenceValue = this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue;
            Object obj3 = EditorGUI.ObjectField(position, this.m_MaterialStyle, objectReferenceValue, typeof(Material), false);
            if (obj3 != objectReferenceValue)
            {
                this.m_Material.GetArrayElementAtIndex(0).objectReferenceValue = obj3;
            }
            EditorGUI.showMixedValue = false;
            base.RenderSortingLayerFields();
            this.CheckForErrors();
            base.serializedObject.ApplyModifiedProperties();
        }

        private static void ShowError(string error)
        {
            if (s_WarningIcon == null)
            {
                s_WarningIcon = EditorGUIUtility.LoadIcon("console.warnicon");
            }
            GUIContent content = new GUIContent(error) {
                image = s_WarningIcon
            };
            GUILayout.Space(5f);
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            GUILayout.Label(content, EditorStyles.wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
        }
    }
}

