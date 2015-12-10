namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(EdgeCollider2D))]
    internal class EdgeCollider2DEditor : Collider2DEditorBase
    {
        private PolygonEditorUtility m_PolyUtility = new PolygonEditorUtility();
        private bool m_ShowColliderInfo;

        protected override void OnEditEnd()
        {
            this.m_PolyUtility.StopEditing();
        }

        protected override void OnEditStart()
        {
            this.m_PolyUtility.StartEditing(this.target as Collider2D);
        }

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            base.BeginColliderInspector();
            base.OnInspectorGUI();
            base.EndColliderInspector();
        }

        public void OnSceneGUI()
        {
            if (base.editingCollider)
            {
                this.m_PolyUtility.OnSceneGUI();
            }
        }
    }
}

