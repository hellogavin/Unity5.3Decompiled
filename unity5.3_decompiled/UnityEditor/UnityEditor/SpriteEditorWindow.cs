namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEditor.Sprites;
    using UnityEditorInternal;
    using UnityEngine;

    internal class SpriteEditorWindow : SpriteUtilityWindow
    {
        [CompilerGenerated]
        private static Comparison<Rect> <>f__am$cache13;
        private const int k_PolygonChangeShapeWindowHeight = 0x2d;
        private const int k_PolygonChangeShapeWindowMargin = 0x11;
        private const int k_PolygonChangeShapeWindowWarningHeight = 0x41;
        private const int k_PolygonChangeShapeWindowWidth = 150;
        internal static PrefKey k_SpriteEditorTrim = new PrefKey("Sprite Editor/Trim", "#t");
        private const float k_WarningMessageHeight = 40f;
        private const float k_WarningMessageWidth = 250f;
        private GizmoMode m_GizmoMode;
        public bool m_IgnoreNextPostprocessEvent;
        public Texture2D m_OriginalTexture;
        private Rect m_PolygonChangeShapeWindowRect = new Rect(0f, 17f, 150f, 45f);
        private int m_PolygonSides;
        private SpriteRectCache m_RectsCache;
        public bool m_ResetOnNextRepaint;
        [SerializeField]
        private SpriteRect m_Selected;
        public string m_SelectedAssetPath;
        private bool m_ShowPolygonChangeShapeWindow;
        private SerializedProperty m_SpriteSheetOutline;
        private TextureImporter m_TextureImporter;
        private SerializedObject m_TextureImporterSO;
        private SerializedProperty m_TextureImporterSprites;
        private bool m_TextureIsDirty;
        private const float marginForFraming = 0.05f;
        private const float maxSnapDistance = 14f;
        private static bool[] s_AlphaPixelCache;
        public static SpriteEditorWindow s_Instance;
        public static bool s_OneClickDragStarted = false;

        private static void AcquireOutline(SerializedProperty outlineSP, ref SpriteRect spriteRect)
        {
            for (int i = 0; i < outlineSP.arraySize; i++)
            {
                List<Vector2> item = new List<Vector2>();
                SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
                for (int j = 0; j < arrayElementAtIndex.arraySize; j++)
                {
                    Vector2 vector = arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value;
                    item.Add(vector);
                }
                spriteRect.m_Outline.Add(item);
            }
        }

        private void AddSprite(Rect frame, int alignment, Vector2 pivot, AutoSlicingMethod slicingMethod)
        {
            if (slicingMethod != AutoSlicingMethod.DeleteAll)
            {
                SpriteRect existingOverlappingSprite = this.GetExistingOverlappingSprite(frame);
                if (existingOverlappingSprite != null)
                {
                    if (slicingMethod == AutoSlicingMethod.Smart)
                    {
                        existingOverlappingSprite.m_Rect = frame;
                        existingOverlappingSprite.m_Alignment = (SpriteAlignment) alignment;
                        existingOverlappingSprite.m_Pivot = pivot;
                    }
                }
                else
                {
                    this.AddSprite(frame, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
                }
            }
            else
            {
                this.AddSprite(frame, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
            }
        }

        public SpriteRect AddSprite(Rect rect, int alignment, Vector2 pivot, int colliderAlphaCutoff, float colliderDetail)
        {
            SpriteRect r = new SpriteRect {
                m_Rect = rect,
                m_Alignment = (SpriteAlignment) alignment,
                m_Pivot = pivot
            };
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(this.m_TextureImporter.assetPath);
            r.m_Name = this.GetUniqueName(fileNameWithoutExtension);
            r.m_OriginalName = r.m_Name;
            this.textureIsDirty = true;
            this.m_RectsCache.AddRect(r);
            return r;
        }

        private void ApplyCacheSettingsToInspector(SerializedObject so)
        {
            if ((this.m_TextureImporterSO != null) && (this.m_TextureImporterSO.targetObject == so.targetObject))
            {
                if (so.FindProperty("m_SpriteMode").intValue == this.m_TextureImporterSO.FindProperty("m_SpriteMode").intValue)
                {
                    s_Instance.m_IgnoreNextPostprocessEvent = true;
                }
                else if (this.textureIsDirty && EditorUtility.DisplayDialog("Sprite Editor Window", "You have pending changes in the Sprite Editor Window.\nDo you want to apply these changes?", "Yes", "No"))
                {
                    this.DoApply(so);
                }
            }
        }

        private static void ApplyOutlineChanges(SerializedProperty outlineSP, SpriteRect spriteRect)
        {
            for (int i = 0; i < spriteRect.m_Outline.Count; i++)
            {
                outlineSP.InsertArrayElementAtIndex(i);
                List<Vector2> list = spriteRect.m_Outline[i];
                SerializedProperty arrayElementAtIndex = outlineSP.GetArrayElementAtIndex(i);
                for (int j = 0; j < list.Count; j++)
                {
                    arrayElementAtIndex.InsertArrayElementAtIndex(j);
                    arrayElementAtIndex.GetArrayElementAtIndex(j).vector2Value = list[j];
                }
            }
        }

        public Vector2 ApplySpriteAlignmentToPivot(Vector2 pivot, Rect rect, SpriteAlignment alignment)
        {
            Vector2[] snapPointsArray = this.GetSnapPointsArray(rect);
            if (alignment != SpriteAlignment.Custom)
            {
                Vector2 texturePos = snapPointsArray[(int) alignment];
                return this.ConvertFromTextureToNormalizedSpace(texturePos, rect);
            }
            return pivot;
        }

        private Vector4 ClampSpriteBorder(Vector4 border)
        {
            Rect rect = this.FlipNegativeRect(this.selected.m_Rect);
            float width = rect.width;
            float height = rect.height;
            return new Vector4 { x = Mathf.RoundToInt(Mathf.Clamp(border.x, 0f, Mathf.Min(width - border.z, width))), z = Mathf.RoundToInt(Mathf.Clamp(border.z, 0f, Mathf.Min(width - border.x, width))), y = Mathf.RoundToInt(Mathf.Clamp(border.y, 0f, Mathf.Min(height - border.w, height))), w = Mathf.RoundToInt(Mathf.Clamp(border.w, 0f, Mathf.Min(height - border.y, height))) };
        }

        private Rect ClampSpriteRect(Rect rect)
        {
            Rect rect2 = new Rect {
                xMin = Mathf.Clamp(rect.xMin, 0f, (float) (base.m_Texture.width - 1)),
                yMin = Mathf.Clamp(rect.yMin, 0f, (float) (base.m_Texture.height - 1)),
                xMax = Mathf.Clamp(rect.xMax, 1f, (float) base.m_Texture.width),
                yMax = Mathf.Clamp(rect.yMax, 1f, (float) base.m_Texture.height)
            };
            if (Mathf.RoundToInt(rect2.width) == 0)
            {
                rect2.width = 1f;
            }
            if (Mathf.RoundToInt(rect2.height) == 0)
            {
                rect2.height = 1f;
            }
            return SpriteEditorUtility.RoundedRect(rect2);
        }

        private Vector2 ConvertFromTextureToNormalizedSpace(Vector2 texturePos, Rect rect)
        {
            return new Vector2((texturePos.x - rect.xMin) / rect.width, (texturePos.y - rect.yMin) / rect.height);
        }

        private Texture2D CreateTemporaryDuplicate(Texture2D original, int width, int height)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture || (original == null))
            {
                return null;
            }
            RenderTexture active = RenderTexture.active;
            bool flag = !TextureUtil.GetLinearSampled(original);
            RenderTexture dest = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, !flag ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
            GL.sRGBWrite = flag && (QualitySettings.activeColorSpace == ColorSpace.Linear);
            Graphics.Blit(original, dest);
            GL.sRGBWrite = false;
            RenderTexture.active = dest;
            bool flag2 = (width >= SystemInfo.maxTextureSize) || (height >= SystemInfo.maxTextureSize);
            Texture2D textured = new Texture2D(width, height, TextureFormat.ARGB32, (original.mipmapCount > 1) || flag2);
            textured.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
            textured.Apply();
            RenderTexture.ReleaseTemporary(dest);
            EditorGUIUtility.SetRenderTextureNoViewport(active);
            textured.alphaIsTransparency = original.alphaIsTransparency;
            return textured;
        }

        private void DeterminePolygonSides()
        {
            if (((this.selected != null) && (this.selected.m_Outline != null)) && (this.selected.m_Outline.Count == 1))
            {
                this.m_PolygonSides = this.selected.m_Outline[0].Count;
            }
            else
            {
                this.m_PolygonSides = 0;
            }
        }

        private void DoApply()
        {
            Undo.ClearUndo(this.m_RectsCache);
            this.DoApply(this.m_TextureImporterSO);
            this.m_TextureImporterSO.ApplyModifiedPropertiesWithoutUndo();
            this.m_IgnoreNextPostprocessEvent = true;
            this.DoTextureReimport(this.m_TextureImporter.assetPath);
            this.textureIsDirty = false;
            this.selected = null;
        }

        private void DoApply(SerializedObject so)
        {
            if (this.multipleSprites)
            {
                List<string> list = new List<string>();
                List<string> list2 = new List<string>();
                SerializedProperty property = so.FindProperty("m_SpriteSheet.m_Sprites");
                property.ClearArray();
                for (int i = 0; i < this.m_RectsCache.Count; i++)
                {
                    SpriteRect spriteRect = this.m_RectsCache.RectAt(i);
                    if (string.IsNullOrEmpty(spriteRect.m_Name))
                    {
                        spriteRect.m_Name = "Empty";
                    }
                    if (!string.IsNullOrEmpty(spriteRect.m_OriginalName))
                    {
                        list.Add(spriteRect.m_OriginalName);
                        list2.Add(spriteRect.m_Name);
                    }
                    property.InsertArrayElementAtIndex(i);
                    SerializedProperty arrayElementAtIndex = property.GetArrayElementAtIndex(i);
                    arrayElementAtIndex.FindPropertyRelative("m_Rect").rectValue = spriteRect.m_Rect;
                    arrayElementAtIndex.FindPropertyRelative("m_Border").vector4Value = spriteRect.m_Border;
                    arrayElementAtIndex.FindPropertyRelative("m_Name").stringValue = spriteRect.m_Name;
                    arrayElementAtIndex.FindPropertyRelative("m_Alignment").intValue = (int) spriteRect.m_Alignment;
                    arrayElementAtIndex.FindPropertyRelative("m_Pivot").vector2Value = spriteRect.m_Pivot;
                    ApplyOutlineChanges(arrayElementAtIndex.FindPropertyRelative("m_Outline"), spriteRect);
                }
                if (list.Count > 0)
                {
                    PatchImportSettingRecycleID.PatchMultiple(so, 0xd5, list.ToArray(), list2.ToArray());
                }
            }
            else if (this.m_RectsCache.Count > 0)
            {
                SpriteRect rect2 = this.m_RectsCache.RectAt(0);
                so.FindProperty("m_Alignment").intValue = (int) rect2.m_Alignment;
                so.FindProperty("m_SpriteBorder").vector4Value = rect2.m_Border;
                so.FindProperty("m_SpritePivot").vector2Value = rect2.m_Pivot;
                this.m_SpriteSheetOutline.ClearArray();
                ApplyOutlineChanges(this.m_SpriteSheetOutline, rect2);
            }
        }

        private void DoApplyRevertGUI()
        {
            EditorGUI.BeginDisabledGroup(!this.textureIsDirty);
            if (GUILayout.Button("Revert", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.DoRevert();
            }
            if (GUILayout.Button("Apply", EditorStyles.toolbarButton, new GUILayoutOption[0]))
            {
                this.DoApply();
            }
            EditorGUI.EndDisabledGroup();
        }

        public void DoAutomaticSlicing(int minimumSpriteSize, int alignment, Vector2 pivot, AutoSlicingMethod slicingMethod)
        {
            Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Automatic Slicing");
            if (slicingMethod == AutoSlicingMethod.DeleteAll)
            {
                this.m_RectsCache.ClearAll();
            }
            List<Rect> rects = new List<Rect>(InternalSpriteUtility.GenerateAutomaticSpriteRectangles(base.m_Texture, minimumSpriteSize, 0));
            foreach (Rect rect in this.SortRects(rects))
            {
                this.AddSprite(rect, alignment, pivot, slicingMethod);
            }
            this.selected = null;
            this.textureIsDirty = true;
            base.Repaint();
        }

        private void DoBorderFields()
        {
            EditorGUI.BeginChangeCheck();
            Vector4 vector = this.ClampSpriteBorder(this.selected.m_Border);
            int x = Mathf.RoundToInt(vector.x);
            int w = Mathf.RoundToInt(vector.y);
            int z = Mathf.RoundToInt(vector.z);
            int y = Mathf.RoundToInt(vector.w);
            this.FourIntFields("Border", "L", "T", "R", "B", ref x, ref y, ref z, ref w);
            Vector4 border = new Vector4((float) x, (float) w, (float) z, (float) y);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Border");
                this.textureIsDirty = true;
                this.selected.m_Border = this.ClampSpriteBorder(border);
            }
        }

        private void DoEditingDisabledMessage()
        {
            if (this.IsEditingDisabled())
            {
                GUILayout.BeginArea(this.warningMessageRect);
                EditorGUILayout.HelpBox("Editing is disabled during play mode", MessageType.Warning);
                GUILayout.EndArea();
            }
        }

        public void DoGridSlicing(Vector2 size, Vector2 offset, Vector2 padding, int alignment, Vector2 pivot)
        {
            Rect[] rectArray = InternalSpriteUtility.GenerateGridSpriteRectangles(base.m_Texture, offset, size, padding);
            bool flag = true;
            if ((rectArray.Length > 0x3e8) && !EditorUtility.DisplayDialog("Creating multiple sprites", "Creating " + rectArray.Length + " sprites. \nThis can take up to several minutes.", "Ok", "Cancel"))
            {
                flag = false;
            }
            if (flag)
            {
                Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Grid Slicing");
                this.m_RectsCache.ClearAll();
                foreach (Rect rect in rectArray)
                {
                    this.AddSprite(rect, alignment, pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
                }
                this.selected = null;
                this.textureIsDirty = true;
            }
            base.Repaint();
        }

        private void DoNameField()
        {
            EditorGUI.BeginChangeCheck();
            string name = this.selected.m_Name;
            GUI.SetNextControlName("SpriteName");
            string filename = EditorGUILayout.TextField("Name", name, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Name");
                this.textureIsDirty = true;
                filename = InternalEditorUtility.RemoveInvalidCharsFromFileName(filename, true);
                if (string.IsNullOrEmpty(this.selected.m_OriginalName) && (filename != name))
                {
                    this.selected.m_OriginalName = name;
                }
                if (string.IsNullOrEmpty(filename))
                {
                    filename = name;
                }
                foreach (SpriteRect rect in this.m_RectsCache.m_Rects)
                {
                    if (rect.m_Name == filename)
                    {
                        filename = this.selected.m_OriginalName;
                        break;
                    }
                }
                this.selected.m_Name = filename;
            }
        }

        private void DoPivotFields()
        {
            EditorGUI.BeginChangeCheck();
            this.selected.m_Alignment = (SpriteAlignment) EditorGUILayout.Popup(SpriteUtilityWindow.Styles.s_PivotLabel, (int) this.selected.m_Alignment, SpriteUtilityWindow.Styles.spriteAlignmentOptions, new GUILayoutOption[0]);
            Vector2 pivot = this.selected.m_Pivot;
            Vector2 customOffset = pivot;
            EditorGUI.BeginDisabledGroup(this.selected.m_Alignment != SpriteAlignment.Custom);
            Rect position = GUILayoutUtility.GetRect(322f, 322f, (float) 32f, (float) 32f);
            GUI.SetNextControlName("PivotField");
            customOffset = EditorGUI.Vector2Field(position, "Custom Pivot", pivot);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Pivot");
                this.textureIsDirty = true;
                this.selected.m_Pivot = SpriteEditorUtility.GetPivotValue(this.selected.m_Alignment, customOffset);
            }
        }

        private void DoPolygonChangeShapeWindow()
        {
            if (this.m_ShowPolygonChangeShapeWindow)
            {
                bool flag = false;
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 45f;
                GUILayout.BeginArea(this.m_PolygonChangeShapeWindowRect);
                GUILayout.BeginVertical(GUI.skin.box, new GUILayoutOption[0]);
                Event current = Event.current;
                if ((this.isSidesValid && (current.type == EventType.KeyDown)) && (current.keyCode == KeyCode.Return))
                {
                    flag = true;
                    current.Use();
                }
                EditorGUI.FocusTextInControl("PolygonSidesInput");
                GUI.SetNextControlName("PolygonSidesInput");
                EditorGUI.BeginChangeCheck();
                this.m_PolygonSides = EditorGUILayout.IntField("Sides", this.m_PolygonSides, new GUILayoutOption[0]);
                if (EditorGUI.EndChangeCheck())
                {
                    if (!this.isSidesValid)
                    {
                        this.m_PolygonChangeShapeWindowRect.height = 65f;
                    }
                    else
                    {
                        this.m_PolygonChangeShapeWindowRect.height = 45f;
                    }
                }
                GUILayout.FlexibleSpace();
                if (!this.isSidesValid)
                {
                    EditorGUILayout.HelpBox("Sides can only be either 0 or anything between 3 and 128", MessageType.Warning, true);
                }
                else
                {
                    GUILayout.BeginHorizontal(new GUILayoutOption[0]);
                    GUILayout.FlexibleSpace();
                    EditorGUI.BeginDisabledGroup(!this.isSidesValid);
                    if (GUILayout.Button(new GUIContent("Change", "Change to the new number of sides"), new GUILayoutOption[0]))
                    {
                        flag = true;
                    }
                    EditorGUI.EndDisabledGroup();
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                if (flag)
                {
                    if (this.isSidesValid)
                    {
                        this.GeneratePolygonOutline(this.m_PolygonSides);
                    }
                    this.m_ShowPolygonChangeShapeWindow = false;
                }
                EditorGUIUtility.labelWidth = labelWidth;
                GUILayout.EndArea();
            }
        }

        private void DoPositionField()
        {
            EditorGUI.BeginChangeCheck();
            Rect rect = this.selected.m_Rect;
            int x = Mathf.RoundToInt(rect.x);
            int y = Mathf.RoundToInt(rect.y);
            int z = Mathf.RoundToInt(rect.width);
            int w = Mathf.RoundToInt(rect.height);
            this.FourIntFields("Position", "X", "Y", "W", "H", ref x, ref y, ref z, ref w);
            Rect rect2 = new Rect((float) x, (float) y, (float) z, (float) w);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Change Sprite Position");
                this.textureIsDirty = true;
                this.selected.m_Rect = this.ClampSpriteRect(rect2);
            }
        }

        private void DoRevert()
        {
            this.m_TextureIsDirty = false;
            this.selected = null;
            this.RefreshRects();
            GUI.FocusControl(string.Empty);
        }

        private void DoSelectedFrameInspector()
        {
            if (this.selected != null)
            {
                EditorGUIUtility.wideMode = true;
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 135f;
                GUILayout.BeginArea(this.inspectorRect);
                GUILayout.BeginVertical(new GUIContent("Sprite"), GUI.skin.window, new GUILayoutOption[0]);
                EditorGUI.BeginDisabledGroup(!this.multipleSprites);
                this.DoNameField();
                this.DoPositionField();
                EditorGUI.EndDisabledGroup();
                this.DoBorderFields();
                this.DoPivotFields();
                GUILayout.EndVertical();
                GUILayout.EndArea();
                EditorGUIUtility.labelWidth = labelWidth;
            }
        }

        protected override void DoTextureGUIExtras()
        {
            if (!this.IsEditingDisabled())
            {
                this.HandleGizmoMode();
                if (this.multipleSprites)
                {
                    this.HandleRectCornerScalingHandles();
                }
                this.HandleBorderCornerScalingHandles();
                this.HandleBorderSidePointScalingSliders();
                if (this.multipleSprites)
                {
                    this.HandleRectSideScalingHandles();
                }
                this.HandleBorderSideScalingHandles();
                this.HandlePivotHandle();
                if (this.multipleSprites)
                {
                    this.HandleDragging();
                }
                this.HandleSelection();
                this.HandleFrameSelected();
                if (this.multipleSprites)
                {
                    this.HandleCreate();
                    this.HandleDelete();
                    this.HandleDuplicate();
                }
            }
        }

        public void DoTextureReimport(string path)
        {
            if (this.m_TextureImporterSO != null)
            {
                try
                {
                    AssetDatabase.StartAssetEditing();
                    AssetDatabase.ImportAsset(path);
                }
                finally
                {
                    AssetDatabase.StopAssetEditing();
                }
                this.textureIsDirty = false;
            }
        }

        private void DoToolbarGUI()
        {
            if (this.polygonSprite)
            {
                EditorGUI.BeginDisabledGroup(this.IsEditingDisabled());
                this.m_ShowPolygonChangeShapeWindow = GUILayout.Toggle(this.m_ShowPolygonChangeShapeWindow, "Change Shape", EditorStyles.toolbarButton, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                EditorGUI.BeginDisabledGroup(!this.multipleSprites || this.IsEditingDisabled());
                Rect buttonRect = EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                if (GUILayout.Button("Slice", "toolbarPopup", new GUILayoutOption[0]))
                {
                    SpriteEditorMenu.s_SpriteEditor = this;
                    if (SpriteEditorMenu.ShowAtPosition(buttonRect))
                    {
                        GUIUtility.ExitGUI();
                    }
                }
                EditorGUI.BeginDisabledGroup(this.selected == null);
                if (GUILayout.Button(new GUIContent("Trim", "Trims selected rectangle (T)"), EditorStyles.toolbarButton, new GUILayoutOption[0]) || (string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()) && k_SpriteEditorTrim.activated))
                {
                    Rect rect = this.TrimAlpha(this.selected.m_Rect);
                    if ((rect.width <= 0f) && (rect.height <= 0f))
                    {
                        this.m_RectsCache.RemoveRect(this.selected);
                        this.selected = null;
                    }
                    else
                    {
                        rect = this.ClampSpriteRect(rect);
                        if (this.selected.m_Rect != rect)
                        {
                            this.textureIsDirty = true;
                        }
                        this.selected.m_Rect = rect;
                    }
                    base.Repaint();
                }
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
            }
        }

        protected override void DrawGizmos()
        {
            SpriteEditorUtility.BeginLines(new Color(0f, 0f, 0f, 0.25f));
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                Rect rect = this.m_RectsCache.RectAt(i).m_Rect;
                if (this.m_RectsCache.RectAt(i) != this.selected)
                {
                    SpriteEditorUtility.DrawBox(new Rect(rect.xMin + (1f / base.m_Zoom), rect.yMin + (1f / base.m_Zoom), rect.width, rect.height));
                }
            }
            SpriteEditorUtility.EndLines();
            SpriteEditorUtility.BeginLines(new Color(1f, 1f, 1f, 0.5f));
            for (int j = 0; j < this.m_RectsCache.Count; j++)
            {
                if (this.m_RectsCache.RectAt(j) != this.selected)
                {
                    SpriteEditorUtility.DrawBox(this.m_RectsCache.RectAt(j).m_Rect);
                }
            }
            SpriteEditorUtility.EndLines();
            if (this.polygonSprite)
            {
                for (int m = 0; m < this.m_RectsCache.Count; m++)
                {
                    SpriteRect rect2 = this.m_RectsCache.RectAt(m);
                    if (rect2.m_Outline.Count > 0)
                    {
                        SpriteEditorUtility.BeginLines(new Color(0.75f, 0.75f, 0.75f, 0.75f));
                        for (int n = 0; n < rect2.m_Outline.Count; n++)
                        {
                            int num5 = 0;
                            while (num5 < (rect2.m_Outline[n].Count - 1))
                            {
                                SpriteEditorUtility.DrawLine(rect2.m_Outline[n][num5], rect2.m_Outline[n][num5 + 1]);
                                num5++;
                            }
                            SpriteEditorUtility.DrawLine(rect2.m_Outline[n][num5], rect2.m_Outline[n][0]);
                        }
                        SpriteEditorUtility.EndLines();
                    }
                }
            }
            SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
            for (int k = 0; k < this.m_RectsCache.Count; k++)
            {
                SpriteRect currentRect = this.m_RectsCache.RectAt(k);
                if (this.ShouldDrawBorders(currentRect))
                {
                    Vector4 border = currentRect.m_Border;
                    Rect rect4 = currentRect.m_Rect;
                    SpriteEditorUtility.DrawLine(new Vector3(rect4.xMin + border.x, rect4.yMin), new Vector3(rect4.xMin + border.x, rect4.yMax));
                    SpriteEditorUtility.DrawLine(new Vector3(rect4.xMax - border.z, rect4.yMin), new Vector3(rect4.xMax - border.z, rect4.yMax));
                    SpriteEditorUtility.DrawLine(new Vector3(rect4.xMin, rect4.yMin + border.y), new Vector3(rect4.xMax, rect4.yMin + border.y));
                    SpriteEditorUtility.DrawLine(new Vector3(rect4.xMin, rect4.yMax - border.w), new Vector3(rect4.xMax, rect4.yMax - border.w));
                }
            }
            SpriteEditorUtility.EndLines();
            if (this.ShouldShowRectScaling())
            {
                Rect position = this.selected.m_Rect;
                SpriteEditorUtility.BeginLines(new Color(0f, 0.1f, 0.3f, 0.25f));
                SpriteEditorUtility.DrawBox(new Rect(position.xMin + (1f / base.m_Zoom), position.yMin + (1f / base.m_Zoom), position.width, position.height));
                SpriteEditorUtility.EndLines();
                SpriteEditorUtility.BeginLines(new Color(0.25f, 0.5f, 1f, 0.75f));
                SpriteEditorUtility.DrawBox(position);
                SpriteEditorUtility.EndLines();
            }
        }

        private Rect FlipNegativeRect(Rect rect)
        {
            return new Rect { xMin = Mathf.Min(rect.xMin, rect.xMax), yMin = Mathf.Min(rect.yMin, rect.yMax), xMax = Mathf.Max(rect.xMin, rect.xMax), yMax = Mathf.Max(rect.yMin, rect.yMax) };
        }

        private void FourIntFields(string label, string labelX, string labelY, string labelZ, string labelW, ref int x, ref int y, ref int z, ref int w)
        {
            Rect rect = GUILayoutUtility.GetRect(322f, 322f, (float) 32f, (float) 32f);
            Rect position = rect;
            position.width = EditorGUIUtility.labelWidth;
            position.height = 16f;
            GUI.Label(position, label);
            Rect rect3 = rect;
            rect3.width -= EditorGUIUtility.labelWidth;
            rect3.height = 16f;
            rect3.x += EditorGUIUtility.labelWidth;
            rect3.width /= 2f;
            rect3.width -= 2f;
            EditorGUIUtility.labelWidth = 12f;
            GUI.SetNextControlName("FourIntFields_x");
            x = EditorGUI.IntField(rect3, labelX, x);
            rect3.x += rect3.width + 3f;
            GUI.SetNextControlName("FourIntFields_y");
            y = EditorGUI.IntField(rect3, labelY, y);
            rect3.y += 16f;
            rect3.x -= rect3.width + 3f;
            GUI.SetNextControlName("FourIntFields_z");
            z = EditorGUI.IntField(rect3, labelZ, z);
            rect3.x += rect3.width + 3f;
            GUI.SetNextControlName("FourIntFields_w");
            w = EditorGUI.IntField(rect3, labelW, w);
            EditorGUIUtility.labelWidth = 135f;
        }

        public void GeneratePolygonOutline(int sides)
        {
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                SpriteRect rect = this.m_RectsCache.RectAt(i);
                List<Vector2> item = new List<Vector2>();
                item.AddRange(SpriteUtility.GeneratePolygonOutlineVerticesOfSize(sides, (int) rect.m_Rect.width, (int) rect.m_Rect.height));
                rect.m_Outline.Clear();
                rect.m_Outline.Add(item);
                this.m_TextureIsDirty = true;
            }
            base.Repaint();
        }

        private SpriteRect GetExistingOverlappingSprite(Rect rect)
        {
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                Rect a = this.m_RectsCache.RectAt(i).m_Rect;
                if (this.Overlap(a, rect))
                {
                    return this.m_RectsCache.RectAt(i);
                }
            }
            return null;
        }

        private Texture2D GetSelectedTexture2D()
        {
            Texture2D assetObject = null;
            if (Selection.activeObject is Texture2D)
            {
                assetObject = Selection.activeObject as Texture2D;
            }
            else if (Selection.activeObject is Sprite)
            {
                assetObject = SpriteUtility.GetSpriteTexture(Selection.activeObject as Sprite, false);
            }
            else if (((Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<SpriteRenderer>() != null)) && (Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite != null))
            {
                assetObject = SpriteUtility.GetSpriteTexture(Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite, false);
            }
            if (assetObject != null)
            {
                this.m_SelectedAssetPath = AssetDatabase.GetAssetPath(assetObject);
            }
            return assetObject;
        }

        private Vector2[] GetSnapPointsArray(Rect rect)
        {
            Vector2[] vectorArray = new Vector2[9];
            vectorArray[1] = new Vector2(rect.xMin, rect.yMax);
            vectorArray[2] = new Vector2(rect.center.x, rect.yMax);
            vectorArray[3] = new Vector2(rect.xMax, rect.yMax);
            vectorArray[4] = new Vector2(rect.xMin, rect.center.y);
            vectorArray[0] = new Vector2(rect.center.x, rect.center.y);
            vectorArray[5] = new Vector2(rect.xMax, rect.center.y);
            vectorArray[6] = new Vector2(rect.xMin, rect.yMin);
            vectorArray[7] = new Vector2(rect.center.x, rect.yMin);
            vectorArray[8] = new Vector2(rect.xMax, rect.yMin);
            return vectorArray;
        }

        private string GetUniqueName(string prefix)
        {
            string str;
            bool flag;
            int num = 0;
            do
            {
                str = prefix + "_" + num++;
                flag = false;
                foreach (SpriteRect rect in this.m_RectsCache.m_Rects)
                {
                    if (rect.m_Name == str)
                    {
                        flag = true;
                    }
                }
            }
            while (flag);
            return str;
        }

        public static void GetWindow()
        {
            EditorWindow.GetWindow<SpriteEditorWindow>();
        }

        private void HandleApplyRevertDialog()
        {
            if (this.textureIsDirty && (this.m_TextureImporter != null))
            {
                if (EditorUtility.DisplayDialog("Unapplied import settings", "Unapplied import settings for '" + this.m_TextureImporter.assetPath + "'", "Apply", "Revert"))
                {
                    this.DoApply();
                }
                else
                {
                    this.DoRevert();
                }
            }
        }

        private void HandleBorderCornerScalingHandles()
        {
            if (this.selected != null)
            {
                GUIStyle dragBorderdot = SpriteUtilityWindow.s_Styles.dragBorderdot;
                GUIStyle dragBorderDotActive = SpriteUtilityWindow.s_Styles.dragBorderDotActive;
                Color color = new Color(0f, 1f, 0f);
                Rect rect = new Rect(this.selected.m_Rect);
                Vector4 border = this.selected.m_Border;
                float x = rect.xMin + border.x;
                float num2 = rect.xMax - border.z;
                float y = rect.yMax - border.w;
                float num4 = rect.yMin + border.y;
                EditorGUI.BeginChangeCheck();
                this.HandleBorderPointSlider(ref x, ref y, MouseCursor.ResizeUpLeft, (border.x < 1f) && (border.w < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref num2, ref y, MouseCursor.ResizeUpRight, (border.z < 1f) && (border.w < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref x, ref num4, MouseCursor.ResizeUpRight, (border.x < 1f) && (border.y < 1f), dragBorderdot, dragBorderDotActive, color);
                this.HandleBorderPointSlider(ref num2, ref num4, MouseCursor.ResizeUpLeft, (border.z < 1f) && (border.y < 1f), dragBorderdot, dragBorderDotActive, color);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
                    border.x = x - rect.xMin;
                    border.z = rect.xMax - num2;
                    border.w = rect.yMax - y;
                    border.y = num4 - rect.yMin;
                    this.textureIsDirty = true;
                }
                this.selected.m_Border = this.ClampSpriteBorder(border);
            }
        }

        private void HandleBorderPointSlider(ref float x, ref float y, MouseCursor mouseCursor, bool isHidden, GUIStyle dragDot, GUIStyle dragDotActive, Color color)
        {
            Color color2 = GUI.color;
            if (isHidden)
            {
                GUI.color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                GUI.color = color;
            }
            Vector2 vector = SpriteEditorHandles.PointSlider(new Vector2(x, y), mouseCursor, dragDot, dragDotActive);
            x = vector.x;
            y = vector.y;
            GUI.color = color2;
        }

        private float HandleBorderScaleSlider(float x, float y, float width, float height, bool isHorizontal)
        {
            float num2;
            float fixedWidth = SpriteUtilityWindow.s_Styles.dragBorderdot.fixedWidth;
            Vector2 pos = Handles.matrix.MultiplyPoint((Vector3) new Vector2(x, y));
            EditorGUI.BeginChangeCheck();
            if (isHorizontal)
            {
                Rect cursorRect = new Rect(pos.x - (fixedWidth * 0.5f), pos.y, fixedWidth, height);
                num2 = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeHorizontal, cursorRect).x;
            }
            else
            {
                Rect rect2 = new Rect(pos.x, pos.y - (fixedWidth * 0.5f), width, fixedWidth);
                num2 = SpriteEditorHandles.ScaleSlider(pos, MouseCursor.ResizeVertical, rect2).y;
            }
            if (EditorGUI.EndChangeCheck())
            {
                return num2;
            }
            return (!isHorizontal ? y : x);
        }

        private void HandleBorderSidePointScalingSliders()
        {
            if (this.selected != null)
            {
                GUIStyle dragBorderdot = SpriteUtilityWindow.s_Styles.dragBorderdot;
                GUIStyle dragBorderDotActive = SpriteUtilityWindow.s_Styles.dragBorderDotActive;
                Color color = new Color(0f, 1f, 0f);
                Rect rect = this.selected.m_Rect;
                Vector4 border = this.selected.m_Border;
                float x = rect.xMin + border.x;
                float num2 = rect.xMax - border.z;
                float y = rect.yMax - border.w;
                float num4 = rect.yMin + border.y;
                EditorGUI.BeginChangeCheck();
                float num5 = num4 - ((num4 - y) / 2f);
                float num6 = x - ((x - num2) / 2f);
                float num7 = num5;
                this.HandleBorderPointSlider(ref x, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num5;
                this.HandleBorderPointSlider(ref num2, ref num7, MouseCursor.ResizeHorizontal, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num6;
                this.HandleBorderPointSlider(ref num7, ref y, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
                num7 = num6;
                this.HandleBorderPointSlider(ref num7, ref num4, MouseCursor.ResizeVertical, false, dragBorderdot, dragBorderDotActive, color);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
                    border.x = x - rect.xMin;
                    border.z = rect.xMax - num2;
                    border.w = rect.yMax - y;
                    border.y = num4 - rect.yMin;
                    this.textureIsDirty = true;
                }
                this.selected.m_Border = this.ClampSpriteBorder(border);
            }
        }

        private void HandleBorderSideScalingHandles()
        {
            if (this.selected != null)
            {
                Rect rect = new Rect(this.selected.m_Rect);
                Vector4 border = this.selected.m_Border;
                float x = rect.xMin + border.x;
                float num2 = rect.xMax - border.z;
                float y = rect.yMax - border.w;
                float num4 = rect.yMin + border.y;
                Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
                Vector2 vector3 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
                float width = Mathf.Abs((float) (vector3.x - vector2.x));
                float height = Mathf.Abs((float) (vector3.y - vector2.y));
                EditorGUI.BeginChangeCheck();
                x = this.HandleBorderScaleSlider(x, rect.yMax, width, height, true);
                num2 = this.HandleBorderScaleSlider(num2, rect.yMax, width, height, true);
                y = this.HandleBorderScaleSlider(rect.xMin, y, width, height, false);
                num4 = this.HandleBorderScaleSlider(rect.xMin, num4, width, height, false);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite border");
                    border.x = x - rect.xMin;
                    border.z = rect.xMax - num2;
                    border.w = rect.yMax - y;
                    border.y = num4 - rect.yMin;
                    this.selected.m_Border = this.ClampSpriteBorder(border);
                    this.textureIsDirty = true;
                }
            }
        }

        private void HandleCreate()
        {
            if (!this.MouseOnTopOfInspector() && !Event.current.alt)
            {
                EditorGUI.BeginChangeCheck();
                Rect rect = SpriteEditorHandles.RectCreator((float) base.m_Texture.width, (float) base.m_Texture.height, SpriteUtilityWindow.s_Styles.createRect);
                if ((EditorGUI.EndChangeCheck() && (rect.width > 0f)) && (rect.height > 0f))
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Create sprite");
                    this.selected = this.AddSprite(rect, 0, Vector2.zero, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
                    GUIUtility.keyboardControl = 0;
                }
            }
        }

        private void HandleDelete()
        {
            if (((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand)) && ((Event.current.commandName == "SoftDelete") || (Event.current.commandName == "Delete")))
            {
                if (Event.current.type == EventType.ExecuteCommand)
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Delete sprite");
                    this.m_RectsCache.RemoveRect(this.selected);
                    this.selected = null;
                    this.textureIsDirty = true;
                }
                Event.current.Use();
            }
        }

        private void HandleDragging()
        {
            if ((this.selected != null) && !this.MouseOnTopOfInspector())
            {
                Rect clamp = new Rect(0f, 0f, (float) base.m_Texture.width, (float) base.m_Texture.height);
                EditorGUI.BeginChangeCheck();
                SpriteRect selected = this.selected;
                Rect rect4 = SpriteEditorUtility.ClampedRect(SpriteEditorUtility.RoundedRect(SpriteEditorHandles.SliderRect(this.selected.m_Rect)), clamp, true);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Move sprite");
                    selected.m_Rect = rect4;
                    this.textureIsDirty = true;
                }
            }
        }

        private void HandleDuplicate()
        {
            if (((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand)) && (Event.current.commandName == "Duplicate"))
            {
                if (Event.current.type == EventType.ExecuteCommand)
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Duplicate sprite");
                    this.selected = this.AddSprite(this.selected.m_Rect, (int) this.selected.m_Alignment, this.selected.m_Pivot, this.defaultColliderAlphaCutoff, this.defaultColliderDetail);
                }
                Event.current.Use();
            }
        }

        private void HandleFrameSelected()
        {
            if (((Event.current.type == EventType.ValidateCommand) || (Event.current.type == EventType.ExecuteCommand)) && (Event.current.commandName == "FrameSelected"))
            {
                if (Event.current.type == EventType.ExecuteCommand)
                {
                    if (this.selected == null)
                    {
                        return;
                    }
                    Rect rect = this.selected.m_Rect;
                    float zoom = base.m_Zoom;
                    if (rect.width < rect.height)
                    {
                        zoom = this.m_TextureViewRect.height / (rect.height + (this.m_TextureViewRect.height * 0.05f));
                    }
                    else
                    {
                        zoom = this.m_TextureViewRect.width / (rect.width + (this.m_TextureViewRect.width * 0.05f));
                    }
                    base.m_Zoom = zoom;
                    this.m_ScrollPosition.x = (rect.center.x - (base.m_Texture.width * 0.5f)) * base.m_Zoom;
                    this.m_ScrollPosition.y = ((rect.center.y - (base.m_Texture.height * 0.5f)) * base.m_Zoom) * -1f;
                    base.Repaint();
                }
                Event.current.Use();
            }
        }

        private void HandleGizmoMode()
        {
            if (Event.current.control)
            {
                this.m_GizmoMode = GizmoMode.BorderEditing;
            }
            else
            {
                this.m_GizmoMode = GizmoMode.RectEditing;
            }
            Event current = Event.current;
            if (((current.type == EventType.KeyDown) || (current.type == EventType.KeyUp)) && (((current.keyCode == KeyCode.LeftControl) || (current.keyCode == KeyCode.RightControl)) || ((current.keyCode == KeyCode.LeftAlt) || (current.keyCode == KeyCode.RightAlt))))
            {
                base.Repaint();
            }
        }

        private void HandlePivotHandle()
        {
            if (this.selected != null)
            {
                EditorGUI.BeginChangeCheck();
                SpriteRect selected = this.selected;
                selected.m_Pivot = this.ApplySpriteAlignmentToPivot(selected.m_Pivot, selected.m_Rect, selected.m_Alignment);
                Vector2 pivot = SpriteEditorHandles.PivotSlider(selected.m_Rect, selected.m_Pivot, SpriteUtilityWindow.s_Styles.pivotdot, SpriteUtilityWindow.s_Styles.pivotdotactive);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Move sprite pivot");
                    if (Event.current.control)
                    {
                        this.selected.m_Pivot = this.SnapPivot(pivot);
                    }
                    else
                    {
                        this.selected.m_Pivot = pivot;
                        this.selected.m_Alignment = SpriteAlignment.Custom;
                    }
                    this.textureIsDirty = true;
                }
            }
        }

        private void HandleRectCornerScalingHandles()
        {
            if (this.selected != null)
            {
                GUIStyle dragdot = SpriteUtilityWindow.s_Styles.dragdot;
                GUIStyle dragdotactive = SpriteUtilityWindow.s_Styles.dragdotactive;
                Color white = Color.white;
                Rect rect = new Rect(this.selected.m_Rect);
                float xMin = rect.xMin;
                float xMax = rect.xMax;
                float yMax = rect.yMax;
                float yMin = rect.yMin;
                EditorGUI.BeginChangeCheck();
                this.HandleBorderPointSlider(ref xMin, ref yMax, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
                this.HandleBorderPointSlider(ref xMax, ref yMax, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
                this.HandleBorderPointSlider(ref xMin, ref yMin, MouseCursor.ResizeUpRight, false, dragdot, dragdotactive, white);
                this.HandleBorderPointSlider(ref xMax, ref yMin, MouseCursor.ResizeUpLeft, false, dragdot, dragdotactive, white);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite");
                    rect.xMin = xMin;
                    rect.xMax = xMax;
                    rect.yMax = yMax;
                    rect.yMin = yMin;
                    this.selected.m_Rect = this.ClampSpriteRect(rect);
                    this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
                    this.textureIsDirty = true;
                }
                if (GUIUtility.hotControl == 0)
                {
                    this.selected.m_Rect = this.FlipNegativeRect(this.selected.m_Rect);
                    this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
                }
            }
        }

        private void HandleRectSideScalingHandles()
        {
            if (this.selected != null)
            {
                Rect rect = new Rect(this.selected.m_Rect);
                float xMin = rect.xMin;
                float xMax = rect.xMax;
                float yMax = rect.yMax;
                float yMin = rect.yMin;
                Vector2 vector = Handles.matrix.MultiplyPoint(new Vector3(rect.xMin, rect.yMin));
                Vector2 vector2 = Handles.matrix.MultiplyPoint(new Vector3(rect.xMax, rect.yMax));
                float width = Mathf.Abs((float) (vector2.x - vector.x));
                float height = Mathf.Abs((float) (vector2.y - vector.y));
                EditorGUI.BeginChangeCheck();
                xMin = this.HandleBorderScaleSlider(xMin, rect.yMax, width, height, true);
                xMax = this.HandleBorderScaleSlider(xMax, rect.yMax, width, height, true);
                yMax = this.HandleBorderScaleSlider(rect.xMin, yMax, width, height, false);
                yMin = this.HandleBorderScaleSlider(rect.xMin, yMin, width, height, false);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(this.m_RectsCache, "Scale sprite");
                    rect.xMin = xMin;
                    rect.xMax = xMax;
                    rect.yMax = yMax;
                    rect.yMin = yMin;
                    this.selected.m_Rect = this.ClampSpriteRect(rect);
                    this.selected.m_Border = this.ClampSpriteBorder(this.selected.m_Border);
                    this.textureIsDirty = true;
                }
            }
        }

        private void HandleSelection()
        {
            if ((((Event.current.type == EventType.MouseDown) && (Event.current.button == 0)) && ((GUIUtility.hotControl == 0) && !Event.current.alt)) && !this.MouseOnTopOfInspector())
            {
                SpriteRect selected = this.selected;
                this.selected = this.TrySelect(Event.current.mousePosition);
                if (this.selected != null)
                {
                    s_OneClickDragStarted = true;
                }
                else
                {
                    base.Repaint();
                }
                if ((selected != this.selected) && (this.selected != null))
                {
                    Event.current.Use();
                }
            }
        }

        private void InitializeAnimVariables()
        {
        }

        public void InvalidatePropertiesCache()
        {
            if (this.m_RectsCache != null)
            {
                this.m_RectsCache.ClearAll();
                Object.DestroyImmediate(this.m_RectsCache);
            }
            if (base.m_Texture != null)
            {
                Object.DestroyImmediate(base.m_Texture);
            }
            this.m_OriginalTexture = null;
            this.m_TextureImporter = null;
            this.m_TextureImporterSO = null;
            this.m_TextureImporterSprites = null;
            s_AlphaPixelCache = null;
        }

        public bool IsEditingDisabled()
        {
            return EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private void ModifierKeysChanged()
        {
            if (EditorWindow.focusedWindow == this)
            {
                base.Repaint();
            }
        }

        private bool MouseOnTopOfInspector()
        {
            if (this.selected == null)
            {
                return false;
            }
            Vector2 point = GUIClip.Unclip(Event.current.mousePosition) + new Vector2(0f, -22f);
            return this.inspectorRect.Contains(point);
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            if (this.m_RectsCache != null)
            {
                Undo.ClearUndo(this.m_RectsCache);
            }
            this.HandleApplyRevertDialog();
            this.InvalidatePropertiesCache();
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
            s_Instance = null;
        }

        private void OnEnable()
        {
            base.minSize = new Vector2(360f, 200f);
            base.titleContent = EditorGUIUtility.TextContent("Sprite Editor");
            s_Instance = this;
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Remove(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
            EditorApplication.modifierKeysChanged = (EditorApplication.CallbackFunction) Delegate.Combine(EditorApplication.modifierKeysChanged, new EditorApplication.CallbackFunction(this.ModifierKeysChanged));
            this.Reset();
        }

        private void OnGUI()
        {
            if (this.m_ResetOnNextRepaint || this.selectedTextureChanged)
            {
                this.Reset();
                this.m_ResetOnNextRepaint = false;
            }
            Matrix4x4 matrix = Handles.matrix;
            if (!this.activeTextureSelected)
            {
                EditorGUI.BeginDisabledGroup(true);
                GUILayout.Label(SpriteUtilityWindow.Styles.s_NoSelectionWarning, new GUILayoutOption[0]);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                base.InitStyles();
                Rect rect = EditorGUILayout.BeginHorizontal(GUIContent.none, "Toolbar", new GUILayoutOption[0]);
                this.DoToolbarGUI();
                GUILayout.FlexibleSpace();
                this.DoApplyRevertGUI();
                base.DoAlphaZoomToolbarGUI();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                base.m_TextureViewRect = new Rect(0f, rect.yMax, base.position.width - 16f, (base.position.height - 16f) - rect.height);
                GUILayout.FlexibleSpace();
                base.DoTextureGUI();
                EditorGUILayout.EndHorizontal();
                this.DoPolygonChangeShapeWindow();
                this.DoEditingDisabledMessage();
                this.DoSelectedFrameInspector();
                Handles.matrix = matrix;
            }
        }

        private void OnSelectionChange()
        {
            if ((this.GetSelectedTexture2D() == null) || this.selectedTextureChanged)
            {
                this.HandleApplyRevertDialog();
            }
            this.InvalidatePropertiesCache();
            this.Reset();
            this.UpdateSelectedSprite();
            base.Repaint();
        }

        private bool Overlap(Rect a, Rect b)
        {
            return ((((a.xMin < b.xMax) && (a.xMax > b.xMin)) && (a.yMin < b.yMax)) && (a.yMax > b.yMin));
        }

        private bool PixelHasAlpha(int x, int y)
        {
            if (base.m_Texture == null)
            {
                return false;
            }
            if (s_AlphaPixelCache == null)
            {
                s_AlphaPixelCache = new bool[base.m_Texture.width * base.m_Texture.height];
                Color32[] colorArray = base.m_Texture.GetPixels32();
                for (int i = 0; i < colorArray.Length; i++)
                {
                    s_AlphaPixelCache[i] = colorArray[i].a != 0;
                }
            }
            int index = (y * base.m_Texture.width) + x;
            return s_AlphaPixelCache[index];
        }

        private List<Rect> RectSweep(List<Rect> rects, Rect sweepRect)
        {
            if ((rects == null) || (rects.Count == 0))
            {
                return new List<Rect>();
            }
            List<Rect> list = new List<Rect>();
            foreach (Rect rect in rects)
            {
                if (this.Overlap(rect, sweepRect))
                {
                    list.Add(rect);
                }
            }
            foreach (Rect rect2 in list)
            {
                rects.Remove(rect2);
            }
            if (<>f__am$cache13 == null)
            {
                <>f__am$cache13 = (a, b) => a.x.CompareTo(b.x);
            }
            list.Sort(<>f__am$cache13);
            return list;
        }

        public void RefreshPropertiesCache()
        {
            this.m_OriginalTexture = this.GetSelectedTexture2D();
            if (this.m_OriginalTexture != null)
            {
                this.m_TextureImporter = AssetImporter.GetAtPath(this.m_SelectedAssetPath) as TextureImporter;
                if (this.m_TextureImporter != null)
                {
                    this.m_TextureImporterSO = new SerializedObject(this.m_TextureImporter);
                    this.m_TextureImporterSprites = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Sprites");
                    this.m_SpriteSheetOutline = this.m_TextureImporterSO.FindProperty("m_SpriteSheet.m_Outline");
                    if (this.m_RectsCache != null)
                    {
                        this.selected = (this.m_TextureImporterSprites.arraySize <= 0) ? null : this.m_RectsCache.RectAt(0);
                    }
                    int width = 0;
                    int height = 0;
                    this.m_TextureImporter.GetWidthAndHeight(ref width, ref height);
                    base.m_Texture = this.CreateTemporaryDuplicate(AssetDatabase.LoadMainAssetAtPath(this.m_TextureImporter.assetPath) as Texture2D, width, height);
                    if (base.m_Texture != null)
                    {
                        base.m_Texture.filterMode = FilterMode.Point;
                    }
                }
            }
        }

        private void RefreshRects()
        {
            if (this.m_TextureImporterSprites != null)
            {
                if (this.m_RectsCache != null)
                {
                    this.m_RectsCache.ClearAll();
                    Undo.ClearUndo(this.m_RectsCache);
                    Object.DestroyImmediate(this.m_RectsCache);
                }
                this.m_RectsCache = ScriptableObject.CreateInstance<SpriteRectCache>();
                if (this.multipleSprites)
                {
                    for (int i = 0; i < this.m_TextureImporterSprites.arraySize; i++)
                    {
                        SpriteRect rect;
                        rect = new SpriteRect {
                            m_Rect = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Rect").rectValue,
                            m_Name = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue,
                            m_Alignment = (SpriteAlignment) this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Alignment").intValue,
                            m_Border = this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Border").vector4Value,
                            m_Pivot = SpriteEditorUtility.GetPivotValue(rect.m_Alignment, this.m_TextureImporterSprites.GetArrayElementAtIndex(i).FindPropertyRelative("m_Pivot").vector2Value)
                        };
                        AcquireOutline(this.m_TextureImporterSprites.GetArrayElementAtIndex(0).FindPropertyRelative("m_Outline"), ref rect);
                        this.m_RectsCache.AddRect(rect);
                    }
                }
                else if (this.validSprite)
                {
                    SpriteRect rect2;
                    rect2 = new SpriteRect {
                        m_Rect = new Rect(0f, 0f, (float) base.m_Texture.width, (float) base.m_Texture.height),
                        m_Name = this.m_OriginalTexture.name,
                        m_Alignment = (SpriteAlignment) this.m_TextureImporterSO.FindProperty("m_Alignment").intValue,
                        m_Border = this.m_TextureImporter.spriteBorder,
                        m_Pivot = SpriteEditorUtility.GetPivotValue(rect2.m_Alignment, this.m_TextureImporter.spritePivot)
                    };
                    AcquireOutline(this.m_SpriteSheetOutline, ref rect2);
                    this.m_RectsCache.AddRect(rect2);
                }
                if (this.m_RectsCache.Count > 0)
                {
                    this.selected = this.m_RectsCache.RectAt(0);
                }
            }
        }

        public void Reset()
        {
            this.InvalidatePropertiesCache();
            this.selected = null;
            this.textureIsDirty = false;
            base.m_Zoom = -1f;
            this.RefreshPropertiesCache();
            this.RefreshRects();
            this.m_ShowPolygonChangeShapeWindow = this.polygonSprite;
            if (this.m_ShowPolygonChangeShapeWindow)
            {
                this.DeterminePolygonSides();
            }
            base.Repaint();
        }

        private void SelectSpriteIndex(Sprite sprite)
        {
            if (sprite != null)
            {
                this.selected = null;
                for (int i = 0; i < this.m_RectsCache.Count; i++)
                {
                    if (sprite.rect == this.m_RectsCache.RectAt(i).m_Rect)
                    {
                        this.selected = this.m_RectsCache.RectAt(i);
                        return;
                    }
                }
            }
        }

        private bool ShouldDrawBorders(SpriteRect currentRect)
        {
            return (!Mathf.Approximately(currentRect.m_Border.sqrMagnitude, 0f) || ((currentRect == this.selected) && (this.m_GizmoMode == GizmoMode.BorderEditing)));
        }

        private bool ShouldShowRectScaling()
        {
            return ((this.selected != null) && (this.m_GizmoMode == GizmoMode.RectEditing));
        }

        private Vector2 SnapPivot(Vector2 pivot)
        {
            Rect rect = this.selected.m_Rect;
            Vector2 texturePos = new Vector2(rect.xMin + (rect.width * pivot.x), rect.yMin + (rect.height * pivot.y));
            Vector2[] snapPointsArray = this.GetSnapPointsArray(rect);
            SpriteAlignment custom = SpriteAlignment.Custom;
            float maxValue = float.MaxValue;
            for (int i = 0; i < snapPointsArray.Length; i++)
            {
                Vector2 vector2 = texturePos - snapPointsArray[i];
                float num3 = vector2.magnitude * base.m_Zoom;
                if (num3 < maxValue)
                {
                    custom = (SpriteAlignment) i;
                    maxValue = num3;
                }
            }
            this.selected.m_Alignment = custom;
            return this.ConvertFromTextureToNormalizedSpace(texturePos, rect);
        }

        private List<Rect> SortRects(List<Rect> rects)
        {
            List<Rect> list = new List<Rect>();
            while (rects.Count > 0)
            {
                Rect rect = rects[rects.Count - 1];
                Rect sweepRect = new Rect(0f, rect.yMin, (float) base.m_Texture.width, rect.height);
                List<Rect> collection = this.RectSweep(rects, sweepRect);
                if (collection.Count > 0)
                {
                    list.AddRange(collection);
                }
                else
                {
                    list.AddRange(rects);
                    return list;
                }
            }
            return list;
        }

        public static void TextureImporterApply(SerializedObject so)
        {
            if (s_Instance != null)
            {
                s_Instance.ApplyCacheSettingsToInspector(so);
            }
        }

        private Rect TrimAlpha(Rect rect)
        {
            int xMax = (int) rect.xMax;
            int xMin = (int) rect.xMin;
            int yMax = (int) rect.yMax;
            int yMin = (int) rect.yMin;
            for (int i = (int) rect.yMin; i < ((int) rect.yMax); i++)
            {
                for (int j = (int) rect.xMin; j < ((int) rect.xMax); j++)
                {
                    if (this.PixelHasAlpha(j, i))
                    {
                        xMax = Mathf.Min(xMax, j);
                        xMin = Mathf.Max(xMin, j);
                        yMax = Mathf.Min(yMax, i);
                        yMin = Mathf.Max(yMin, i);
                    }
                }
            }
            if ((xMax <= xMin) && (yMax <= yMin))
            {
                return new Rect((float) xMax, (float) yMax, (float) ((xMin - xMax) + 1), (float) ((yMin - yMax) + 1));
            }
            return new Rect(0f, 0f, 0f, 0f);
        }

        private SpriteRect TrySelect(Vector2 mousePosition)
        {
            float num = 1E+07f;
            SpriteRect rect = null;
            for (int i = 0; i < this.m_RectsCache.Count; i++)
            {
                if (this.m_RectsCache.RectAt(i).m_Rect.Contains(Handles.s_InverseMatrix.MultiplyPoint((Vector3) mousePosition)))
                {
                    if (this.m_RectsCache.RectAt(i) == this.selected)
                    {
                        return this.m_RectsCache.RectAt(i);
                    }
                    float width = this.m_RectsCache.RectAt(i).m_Rect.width;
                    float height = this.m_RectsCache.RectAt(i).m_Rect.height;
                    float num5 = width * height;
                    if (((width > 0f) && (height > 0f)) && (num5 < num))
                    {
                        rect = this.m_RectsCache.RectAt(i);
                        num = num5;
                    }
                }
            }
            return rect;
        }

        private void UndoRedoPerformed()
        {
            Texture2D textured = this.GetSelectedTexture2D();
            if ((textured != null) && (this.m_OriginalTexture != textured))
            {
                this.OnSelectionChange();
            }
            if ((this.m_RectsCache != null) && !this.m_RectsCache.Contains(this.selected))
            {
                this.selected = null;
            }
            base.Repaint();
        }

        private void UpdateSelectedSprite()
        {
            if (Selection.activeObject is Sprite)
            {
                this.SelectSpriteIndex(Selection.activeObject as Sprite);
            }
            else if ((Selection.activeGameObject != null) && (Selection.activeGameObject.GetComponent<SpriteRenderer>() != null))
            {
                Sprite sprite = Selection.activeGameObject.GetComponent<SpriteRenderer>().sprite;
                this.SelectSpriteIndex(sprite);
            }
        }

        private bool activeTextureSelected
        {
            get
            {
                return (((this.m_TextureImporter != null) && (base.m_Texture != null)) && (this.m_OriginalTexture != null));
            }
        }

        private int defaultColliderAlphaCutoff
        {
            get
            {
                return 0xfe;
            }
        }

        private float defaultColliderDetail
        {
            get
            {
                return 0.25f;
            }
        }

        private Rect inspectorRect
        {
            get
            {
                return new Rect(((base.position.width - 330f) - 8f) - 16f, ((base.position.height - 148f) - 8f) - 16f, 330f, 148f);
            }
        }

        private bool isSidesValid
        {
            get
            {
                return ((this.m_PolygonSides == 0) || ((this.m_PolygonSides >= 3) && (this.m_PolygonSides <= 0x80)));
            }
        }

        private bool multipleSprites
        {
            get
            {
                return ((this.m_TextureImporter != null) && (this.m_TextureImporter.spriteImportMode == SpriteImportMode.Multiple));
            }
        }

        public Texture2D originalTexture
        {
            get
            {
                return this.m_OriginalTexture;
            }
        }

        private bool polygonSprite
        {
            get
            {
                return ((this.m_TextureImporter != null) && (this.m_TextureImporter.spriteImportMode == SpriteImportMode.Polygon));
            }
        }

        internal Texture2D previewTexture
        {
            get
            {
                return base.m_Texture;
            }
        }

        internal SpriteRect selected
        {
            get
            {
                if (this.IsEditingDisabled())
                {
                    return null;
                }
                return this.m_Selected;
            }
            set
            {
                if (value != this.m_Selected)
                {
                    this.m_Selected = value;
                }
            }
        }

        public bool selectedTextureChanged
        {
            get
            {
                Texture2D textured = this.GetSelectedTexture2D();
                return ((textured != null) && (this.m_OriginalTexture != textured));
            }
        }

        public bool textureIsDirty
        {
            get
            {
                return this.m_TextureIsDirty;
            }
            set
            {
                this.m_TextureIsDirty = value;
            }
        }

        private bool validSprite
        {
            get
            {
                return ((this.m_TextureImporter != null) && (this.m_TextureImporter.spriteImportMode != SpriteImportMode.None));
            }
        }

        private Rect warningMessageRect
        {
            get
            {
                return new Rect(((base.position.width - 250f) - 8f) - 16f, 24f, 250f, 40f);
            }
        }

        public enum AutoSlicingMethod
        {
            DeleteAll,
            Smart,
            Safe
        }

        private enum GizmoMode
        {
            BorderEditing,
            RectEditing
        }
    }
}

