namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    internal class Collider2DEditorBase : ColliderEditorBase
    {
        private SerializedProperty m_Density;
        private readonly AnimBool m_ShowDensity = new AnimBool();

        protected void BeginColliderInspector()
        {
            base.serializedObject.Update();
            EditorGUI.BeginDisabledGroup(base.targets.Length > 1);
            base.InspectorEditButtonGUI();
            EditorGUI.EndDisabledGroup();
        }

        protected void CheckColliderErrorState()
        {
            switch ((this.target as Collider2D).errorState)
            {
                case ColliderErrorState2D.NoShapes:
                    EditorGUILayout.HelpBox("The collider did not create any collision shapes as they all failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;

                case ColliderErrorState2D.RemovedShapes:
                    EditorGUILayout.HelpBox("The collider created collision shape(s) but some were removed as they failed verification.  This could be because they were deemed too small or the vertices were too close.  Vertices can also become close under certain rotations or very small scaling.", MessageType.Warning);
                    break;
            }
        }

        protected void EndColliderInspector()
        {
            base.serializedObject.ApplyModifiedProperties();
        }

        public override void OnDisable()
        {
            this.m_ShowDensity.valueChanged.RemoveListener(new UnityAction(this.Repaint));
            base.OnDisable();
        }

        public override void OnEnable()
        {
            base.OnEnable();
            this.m_Density = base.serializedObject.FindProperty("m_Density");
            Collider2D target = this.target as Collider2D;
            this.m_ShowDensity.value = (target.attachedRigidbody != null) && target.attachedRigidbody.useAutoMass;
            this.m_ShowDensity.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        internal override void OnForceReloadInspector()
        {
            base.OnForceReloadInspector();
            if (base.editingCollider)
            {
                base.ForceQuitEditMode();
            }
        }

        public override void OnInspectorGUI()
        {
            Collider2D target = this.target as Collider2D;
            base.serializedObject.Update();
            this.m_ShowDensity.target = (target.attachedRigidbody != null) && target.attachedRigidbody.useAutoMass;
            if (EditorGUILayout.BeginFadeGroup(this.m_ShowDensity.faded))
            {
                EditorGUILayout.PropertyField(this.m_Density, new GUILayoutOption[0]);
            }
            EditorGUILayout.EndFadeGroup();
            base.serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
            this.CheckColliderErrorState();
            Effector2DEditor.CheckEffectorWarnings(this.target as Collider2D);
        }
    }
}

