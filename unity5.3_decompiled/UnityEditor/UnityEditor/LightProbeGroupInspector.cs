namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(LightProbeGroup))]
    internal class LightProbeGroupInspector : Editor
    {
        private bool m_EditingProbes;
        private LightProbeGroupEditor m_Editor;
        private bool m_ShouldFocus;

        private void EndEditProbes()
        {
            if (this.m_EditingProbes)
            {
                this.m_Editor.DeselectProbes();
                this.m_EditingProbes = false;
                Tools.s_Hidden = false;
            }
        }

        public bool HasFrameBounds()
        {
            return (this.m_Editor.SelectedCount > 0);
        }

        private void InternalOnSceneView()
        {
            if (EditorGUIUtility.IsGizmosAllowedForObject(this.target))
            {
                if ((SceneView.lastActiveSceneView != null) && this.m_ShouldFocus)
                {
                    this.m_ShouldFocus = false;
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                this.m_Editor.PullProbePositions();
                LightProbeGroup target = this.target as LightProbeGroup;
                if (target != null)
                {
                    if (this.m_Editor.OnSceneGUI(target.transform))
                    {
                        this.StartEditProbes();
                    }
                    else
                    {
                        this.EndEditProbes();
                    }
                }
                this.m_Editor.PushProbePositions();
            }
        }

        public void OnDisable()
        {
            this.EndEditProbes();
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Remove(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Remove(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
            if (this.target != null)
            {
                this.m_Editor.PushProbePositions();
            }
        }

        public void OnEnable()
        {
            this.m_Editor = new LightProbeGroupEditor(this.target as LightProbeGroup);
            this.m_Editor.PullProbePositions();
            this.m_Editor.DeselectProbes();
            this.m_Editor.PushProbePositions();
            SceneView.onSceneGUIDelegate = (SceneView.OnSceneFunc) Delegate.Combine(SceneView.onSceneGUIDelegate, new SceneView.OnSceneFunc(this.OnSceneGUIDelegate));
            Undo.undoRedoPerformed = (Undo.UndoRedoCallback) Delegate.Combine(Undo.undoRedoPerformed, new Undo.UndoRedoCallback(this.UndoRedoPerformed));
        }

        public Bounds OnGetFrameBounds()
        {
            return this.m_Editor.selectedProbeBounds;
        }

        public override void OnInspectorGUI()
        {
            this.m_Editor.PullProbePositions();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button("Add Probe", new GUILayoutOption[0]))
            {
                Vector3 zero = Vector3.zero;
                if (SceneView.lastActiveSceneView != null)
                {
                    zero = SceneView.lastActiveSceneView.pivot;
                    LightProbeGroup target = this.target as LightProbeGroup;
                    if (target != null)
                    {
                        zero = target.transform.InverseTransformPoint(zero);
                    }
                }
                this.StartEditProbes();
                this.m_Editor.DeselectProbes();
                this.m_Editor.AddProbe(zero);
            }
            if (GUILayout.Button("Delete Selected", new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.RemoveSelectedProbes();
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(new GUILayoutOption[0]);
            if (GUILayout.Button("Select All", new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.SelectAllProbes();
            }
            if (GUILayout.Button("Duplicate Selected", new GUILayoutOption[0]))
            {
                this.StartEditProbes();
                this.m_Editor.DuplicateSelectedProbes();
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (this.m_Editor.SelectedCount == 1)
            {
                Vector3 vector2 = this.m_Editor.GetSelectedPositions()[0];
                GUIContent label = new GUIContent("Probe Position", "The local position of this probe relative to the parent group.");
                Vector3 position = EditorGUILayout.Vector3Field(label, vector2, new GUILayoutOption[0]);
                if (position != vector2)
                {
                    this.StartEditProbes();
                    this.m_Editor.UpdateSelectedPosition(0, position);
                }
            }
            this.m_Editor.HandleEditMenuHotKeyCommands();
            this.m_Editor.PushProbePositions();
        }

        public void OnSceneGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                this.InternalOnSceneView();
            }
        }

        public void OnSceneGUIDelegate(SceneView sceneView)
        {
            if (Event.current.type == EventType.Repaint)
            {
                this.InternalOnSceneView();
            }
        }

        private void StartEditProbes()
        {
            if (!this.m_EditingProbes)
            {
                this.m_EditingProbes = true;
                this.m_Editor.SetEditing(true);
                Tools.s_Hidden = true;
                SceneView.RepaintAll();
            }
        }

        private void UndoRedoPerformed()
        {
            this.m_Editor.PullProbePositions();
            this.m_Editor.MarkTetrahedraDirty();
        }
    }
}

