namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(TextMesh)), CanEditMultipleObjects]
    internal class TextMeshInspector : Editor
    {
        private SerializedProperty m_Font;

        private void OnEnable()
        {
            this.m_Font = base.serializedObject.FindProperty("m_Font");
        }

        public override void OnInspectorGUI()
        {
            Font font = !this.m_Font.hasMultipleDifferentValues ? (this.m_Font.objectReferenceValue as Font) : null;
            base.DrawDefaultInspector();
            Font font2 = !this.m_Font.hasMultipleDifferentValues ? (this.m_Font.objectReferenceValue as Font) : null;
            if ((font2 != null) && (font2 != font))
            {
                foreach (TextMesh mesh in base.targets)
                {
                    MeshRenderer component = mesh.GetComponent<MeshRenderer>();
                    if (component != null)
                    {
                        component.sharedMaterial = font2.material;
                    }
                }
            }
        }
    }
}

