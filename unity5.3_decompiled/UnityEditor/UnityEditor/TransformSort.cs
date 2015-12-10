namespace UnityEditor
{
    using UnityEngine;

    public class TransformSort : BaseHierarchySort
    {
        private readonly GUIContent m_Content = new GUIContent(EditorGUIUtility.FindTexture("DefaultSorting"), "Transform Child Order");

        public override GUIContent content
        {
            get
            {
                return this.m_Content;
            }
        }
    }
}

