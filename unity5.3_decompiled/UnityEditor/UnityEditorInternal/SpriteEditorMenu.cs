namespace UnityEditorInternal
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class SpriteEditorMenu : EditorWindow
    {
        private static long s_LastClosedTime;
        private static int s_Selected;
        private static SpriteEditorMenuSetting s_Setting;
        public static SpriteEditorWindow s_SpriteEditor;
        public static SpriteEditorMenu s_SpriteEditorMenu;
        private static Styles s_Styles;
        public readonly string[] slicingMethodOptions = new string[] { EditorGUIUtility.TextContent("Delete Existing|Delete all existing sprite assets before the slicing operation").text, EditorGUIUtility.TextContent("Smart|Try to match existing sprite rects to sliced rects from the slicing operation").text, EditorGUIUtility.TextContent("Safe|Keep existing sprite rects intact").text };
        public readonly string[] spriteAlignmentOptions = new string[] { EditorGUIUtility.TextContent("Center").text, EditorGUIUtility.TextContent("Top Left").text, EditorGUIUtility.TextContent("Top").text, EditorGUIUtility.TextContent("Top Right").text, EditorGUIUtility.TextContent("Left").text, EditorGUIUtility.TextContent("Right").text, EditorGUIUtility.TextContent("Bottom Left").text, EditorGUIUtility.TextContent("Bottom").text, EditorGUIUtility.TextContent("Bottom Right").text, EditorGUIUtility.TextContent("Custom").text };

        private void DetemineGridCellSizeWithCellCount()
        {
            int num = (s_SpriteEditor.previewTexture == null) ? 0x1000 : s_SpriteEditor.previewTexture.width;
            int num2 = (s_SpriteEditor.previewTexture == null) ? 0x1000 : s_SpriteEditor.previewTexture.height;
            s_Setting.gridSpriteSize.x = ((num - s_Setting.gridSpriteOffset.x) - (s_Setting.gridSpritePadding.x * s_Setting.gridCellCount.x)) / s_Setting.gridCellCount.x;
            s_Setting.gridSpriteSize.y = ((num2 - s_Setting.gridSpriteOffset.y) - (s_Setting.gridSpritePadding.y * s_Setting.gridCellCount.y)) / s_Setting.gridCellCount.y;
            s_Setting.gridSpriteSize.x = Mathf.Clamp(s_Setting.gridSpriteSize.x, 1f, (float) num);
            s_Setting.gridSpriteSize.y = Mathf.Clamp(s_Setting.gridSpriteSize.y, 1f, (float) num2);
        }

        private void DoAnalytics()
        {
            Analytics.Event("Sprite Editor", "Slice", "Type", (int) s_Setting.slicingType);
            if (s_SpriteEditor.originalTexture != null)
            {
                Analytics.Event("Sprite Editor", "Slice", "Texture Width", s_SpriteEditor.originalTexture.width);
                Analytics.Event("Sprite Editor", "Slice", "Texture Height", s_SpriteEditor.originalTexture.height);
            }
            if (s_Setting.slicingType == SpriteEditorMenuSetting.SlicingType.Automatic)
            {
                Analytics.Event("Sprite Editor", "Slice", "Auto Slicing Method", s_Setting.autoSlicingMethod);
            }
            else
            {
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Size X", (int) s_Setting.gridSpriteSize.x);
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Size Y", (int) s_Setting.gridSpriteSize.y);
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Offset X", (int) s_Setting.gridSpriteOffset.x);
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Offset Y", (int) s_Setting.gridSpriteOffset.y);
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Padding X", (int) s_Setting.gridSpritePadding.x);
                Analytics.Event("Sprite Editor", "Slice", "Grid Slicing Padding Y", (int) s_Setting.gridSpritePadding.y);
            }
        }

        private void DoAutomaticSlicing()
        {
            s_SpriteEditor.DoAutomaticSlicing(4, s_Setting.spriteAlignment, s_Setting.pivot, (SpriteEditorWindow.AutoSlicingMethod) s_Setting.autoSlicingMethod);
        }

        private void DoGridSlicing()
        {
            if (s_Setting.slicingType == SpriteEditorMenuSetting.SlicingType.GridByCellCount)
            {
                this.DetemineGridCellSizeWithCellCount();
            }
            s_SpriteEditor.DoGridSlicing(s_Setting.gridSpriteSize, s_Setting.gridSpriteOffset, s_Setting.gridSpritePadding, s_Setting.spriteAlignment, s_Setting.pivot);
        }

        private void DoPivotGUI()
        {
            EditorGUI.BeginChangeCheck();
            int spriteAlignment = s_Setting.spriteAlignment;
            spriteAlignment = EditorGUILayout.Popup("Pivot", spriteAlignment, this.spriteAlignmentOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change Alignment");
                s_Setting.spriteAlignment = spriteAlignment;
                s_Setting.pivot = SpriteEditorUtility.GetPivotValue((SpriteAlignment) spriteAlignment, s_Setting.pivot);
            }
            Vector2 pivot = s_Setting.pivot;
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(spriteAlignment != 9);
            pivot = EditorGUILayout.Vector2Field("Custom Pivot", pivot, new GUILayoutOption[0]);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change custom pivot");
                s_Setting.pivot = pivot;
            }
        }

        private void DoSlicing()
        {
            this.DoAnalytics();
            switch (s_Setting.slicingType)
            {
                case SpriteEditorMenuSetting.SlicingType.Automatic:
                    this.DoAutomaticSlicing();
                    break;

                case SpriteEditorMenuSetting.SlicingType.GridByCellSize:
                case SpriteEditorMenuSetting.SlicingType.GridByCellCount:
                    this.DoGridSlicing();
                    break;
            }
        }

        private void Init(Rect buttonRect)
        {
            if (s_Setting == null)
            {
                s_Setting = ScriptableObject.CreateInstance<SpriteEditorMenuSetting>();
            }
            buttonRect = GUIUtility.GUIToScreenRect(buttonRect);
            float y = 145f;
            Vector2 windowSize = new Vector2(300f, y);
            base.ShowAsDropDown(buttonRect, windowSize);
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        private void OnAutomaticGUI()
        {
            float pixels = 38f;
            if ((s_SpriteEditor.originalTexture != null) && TextureUtil.IsCompressedTextureFormat(s_SpriteEditor.originalTexture.format))
            {
                EditorGUILayout.LabelField("To obtain more accurate slicing results, manual slicing is recommended!", s_Styles.notice, new GUILayoutOption[0]);
                pixels -= 31f;
            }
            this.DoPivotGUI();
            EditorGUI.BeginChangeCheck();
            int autoSlicingMethod = s_Setting.autoSlicingMethod;
            autoSlicingMethod = EditorGUILayout.Popup("Method", autoSlicingMethod, this.slicingMethodOptions, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change Slicing Method");
                s_Setting.autoSlicingMethod = autoSlicingMethod;
            }
            GUILayout.Space(pixels);
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            s_LastClosedTime = DateTime.Now.Ticks / 0x2710L;
            s_SpriteEditorMenu = null;
        }

        private void OnGridGUI()
        {
            int max = (s_SpriteEditor.previewTexture == null) ? 0x1000 : s_SpriteEditor.previewTexture.width;
            int num2 = (s_SpriteEditor.previewTexture == null) ? 0x1000 : s_SpriteEditor.previewTexture.height;
            if (s_Setting.slicingType == SpriteEditorMenuSetting.SlicingType.GridByCellCount)
            {
                int num3 = (int) s_Setting.gridCellCount.x;
                int num4 = (int) s_Setting.gridCellCount.y;
                EditorGUI.BeginChangeCheck();
                this.TwoIntFields("Column & Row", "C", "R", ref num3, ref num4);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(s_Setting, "Change column & row");
                    s_Setting.gridCellCount.x = Mathf.Clamp(num3, 1, max);
                    s_Setting.gridCellCount.y = Mathf.Clamp(num4, 1, num2);
                }
            }
            else
            {
                int num5 = (int) s_Setting.gridSpriteSize.x;
                int num6 = (int) s_Setting.gridSpriteSize.y;
                EditorGUI.BeginChangeCheck();
                this.TwoIntFields("Pixel Size", "X", "Y", ref num5, ref num6);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(s_Setting, "Change grid size");
                    s_Setting.gridSpriteSize.x = Mathf.Clamp(num5, 1, max);
                    s_Setting.gridSpriteSize.y = Mathf.Clamp(num6, 1, num2);
                }
            }
            int x = (int) s_Setting.gridSpriteOffset.x;
            int y = (int) s_Setting.gridSpriteOffset.y;
            EditorGUI.BeginChangeCheck();
            this.TwoIntFields("Offset", "X", "Y", ref x, ref y);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change grid offset");
                s_Setting.gridSpriteOffset.x = Mathf.Clamp((float) x, 0f, max - s_Setting.gridSpriteSize.x);
                s_Setting.gridSpriteOffset.y = Mathf.Clamp((float) y, 0f, num2 - s_Setting.gridSpriteSize.y);
            }
            int num9 = (int) s_Setting.gridSpritePadding.x;
            int num10 = (int) s_Setting.gridSpritePadding.y;
            EditorGUI.BeginChangeCheck();
            this.TwoIntFields("Padding", "X", "Y", ref num9, ref num10);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change grid padding");
                s_Setting.gridSpritePadding.x = Mathf.Clamp(num9, 0, max);
                s_Setting.gridSpritePadding.y = Mathf.Clamp(num10, 0, num2);
            }
            this.DoPivotGUI();
            GUILayout.Space(2f);
        }

        private void OnGUI()
        {
            if (s_Styles == null)
            {
                s_Styles = new Styles();
            }
            GUILayout.Space(4f);
            EditorGUIUtility.labelWidth = 124f;
            EditorGUIUtility.wideMode = true;
            GUI.Label(new Rect(0f, 0f, base.position.width, base.position.height), GUIContent.none, s_Styles.background);
            EditorGUI.BeginChangeCheck();
            SpriteEditorMenuSetting.SlicingType slicingType = s_Setting.slicingType;
            slicingType = (SpriteEditorMenuSetting.SlicingType) EditorGUILayout.EnumPopup("Type", slicingType, new GUILayoutOption[0]);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterCompleteObjectUndo(s_Setting, "Change slicing type");
                s_Setting.slicingType = slicingType;
            }
            switch (slicingType)
            {
                case SpriteEditorMenuSetting.SlicingType.Automatic:
                    this.OnAutomaticGUI();
                    break;

                case SpriteEditorMenuSetting.SlicingType.GridByCellSize:
                case SpriteEditorMenuSetting.SlicingType.GridByCellCount:
                    this.OnGridGUI();
                    break;
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Space(EditorGUIUtility.labelWidth + 4f);
            if (GUILayout.Button("Slice", new GUILayoutOption[0]))
            {
                this.DoSlicing();
            }
            GUILayout.EndHorizontal();
        }

        internal static bool ShowAtPosition(Rect buttonRect)
        {
            long num = DateTime.Now.Ticks / 0x2710L;
            if (num < (s_LastClosedTime + 50L))
            {
                return false;
            }
            Event.current.Use();
            if (s_SpriteEditorMenu == null)
            {
                s_SpriteEditorMenu = ScriptableObject.CreateInstance<SpriteEditorMenu>();
            }
            s_SpriteEditorMenu.Init(buttonRect);
            return true;
        }

        private void TwoIntFields(string label, string labelX, string labelY, ref int x, ref int y)
        {
            float minHeight = 16f;
            Rect rect = GUILayoutUtility.GetRect(EditorGUILayout.kLabelFloatMinW, EditorGUILayout.kLabelFloatMaxW, minHeight, minHeight, EditorStyles.numberField);
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
            x = EditorGUI.IntField(rect3, labelX, x);
            rect3.x += rect3.width + 3f;
            y = EditorGUI.IntField(rect3, labelY, y);
            EditorGUIUtility.labelWidth = position.width;
        }

        private void UndoRedoPerformed()
        {
            base.Repaint();
        }

        private class Styles
        {
            public GUIStyle background = "grey_border";
            public GUIStyle notice = new GUIStyle(GUI.skin.label);

            public Styles()
            {
                this.notice.alignment = TextAnchor.MiddleCenter;
                this.notice.wordWrap = true;
            }
        }
    }
}

