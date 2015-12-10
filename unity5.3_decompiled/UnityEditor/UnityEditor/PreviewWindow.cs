namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class PreviewWindow : InspectorWindow
    {
        [SerializeField]
        private InspectorWindow m_ParentInspectorWindow;

        public override void AddItemsToMenu(GenericMenu menu)
        {
        }

        protected override void CreateTracker()
        {
            base.m_Tracker = this.m_ParentInspectorWindow.GetTracker();
        }

        public override Editor GetLastInteractedEditor()
        {
            return this.m_ParentInspectorWindow.GetLastInteractedEditor();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.m_ParentInspectorWindow.Repaint();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            base.titleContent = EditorGUIUtility.TextContent("Preview");
            base.minSize = new Vector2(260f, 220f);
        }

        protected override void OnGUI()
        {
            if (this.m_ParentInspectorWindow == null)
            {
                base.Close();
                GUIUtility.ExitGUI();
            }
            Editor.m_AllowMultiObjectAccess = true;
            this.CreateTracker();
            this.CreatePreviewables();
            base.AssignAssetEditor(base.m_Tracker.activeEditors);
            IPreviewable[] editorsWithPreviews = base.GetEditorsWithPreviews(base.m_Tracker.activeEditors);
            IPreviewable editorThatControlsPreview = base.GetEditorThatControlsPreview(editorsWithPreviews);
            bool flag = (editorThatControlsPreview != null) && editorThatControlsPreview.HasPreviewGUI();
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(17f) };
            Rect rect2 = EditorGUILayout.BeginHorizontal(GUIContent.none, InspectorWindow.styles.preToolbar, options);
            GUILayout.FlexibleSpace();
            Rect lastRect = GUILayoutUtility.GetLastRect();
            string text = string.Empty;
            if (editorThatControlsPreview != null)
            {
                text = editorThatControlsPreview.GetPreviewTitle().text;
            }
            GUI.Label(lastRect, text, InspectorWindow.styles.preToolbar2);
            if (flag)
            {
                editorThatControlsPreview.OnPreviewSettings();
            }
            EditorGUILayout.EndHorizontal();
            Event current = Event.current;
            if (((current.type == EventType.MouseUp) && (current.button == 1)) && rect2.Contains(current.mousePosition))
            {
                base.Close();
                current.Use();
            }
            else
            {
                Rect position = GUILayoutUtility.GetRect(0f, 10240f, (float) 64f, (float) 10240f);
                if (Event.current.type == EventType.Repaint)
                {
                    InspectorWindow.styles.preBackground.Draw(position, false, false, false, false);
                }
                if ((editorThatControlsPreview != null) && editorThatControlsPreview.HasPreviewGUI())
                {
                    editorThatControlsPreview.DrawPreview(position);
                }
            }
        }

        public void SetParentInspector(InspectorWindow inspector)
        {
            this.m_ParentInspectorWindow = inspector;
        }

        protected override void ShowButton(Rect r)
        {
        }
    }
}

