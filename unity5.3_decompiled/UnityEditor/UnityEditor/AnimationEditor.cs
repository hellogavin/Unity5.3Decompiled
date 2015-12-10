namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(Animation)), CanEditMultipleObjects]
    internal class AnimationEditor : Editor
    {
        private BoxEditor m_BoxEditor = new BoxEditor(false, s_BoxHash);
        private int m_PrePreviewAnimationArraySize = -1;
        private static int s_BoxHash = "AnimationBoundsEditorHash".GetHashCode();

        internal override void OnAssetStoreInspectorGUI()
        {
            this.OnInspectorGUI();
        }

        public void OnDisable()
        {
            this.m_BoxEditor.OnDisable();
        }

        public void OnEnable()
        {
            this.m_PrePreviewAnimationArraySize = -1;
            this.m_BoxEditor.OnEnable();
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            SerializedProperty property = base.serializedObject.FindProperty("m_Animation");
            EditorGUILayout.PropertyField(property, true, new GUILayoutOption[0]);
            int objectReferenceInstanceIDValue = property.objectReferenceInstanceIDValue;
            SerializedProperty property2 = base.serializedObject.FindProperty("m_Animations");
            int arraySize = property2.arraySize;
            if (ObjectSelector.isVisible && (this.m_PrePreviewAnimationArraySize == -1))
            {
                this.m_PrePreviewAnimationArraySize = arraySize;
            }
            if (this.m_PrePreviewAnimationArraySize != -1)
            {
                int num3 = (arraySize <= 0) ? -1 : property2.GetArrayElementAtIndex(arraySize - 1).objectReferenceInstanceIDValue;
                if (num3 != objectReferenceInstanceIDValue)
                {
                    property2.arraySize = this.m_PrePreviewAnimationArraySize;
                }
                if (!ObjectSelector.isVisible)
                {
                    this.m_PrePreviewAnimationArraySize = -1;
                }
            }
            string[] propertyToExclude = new string[] { "m_Animation", "m_UserAABB" };
            Editor.DrawPropertiesExcluding(base.serializedObject, propertyToExclude);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

