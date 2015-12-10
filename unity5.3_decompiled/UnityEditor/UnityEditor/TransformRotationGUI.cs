namespace UnityEditor
{
    using System;
    using UnityEngine;

    [Serializable]
    internal class TransformRotationGUI
    {
        private Vector3 m_EulerAngles;
        private Vector3 m_OldEulerAngles = new Vector3(1000000f, 1E+07f, 1000000f);
        private RotationOrder m_OldRotationOrder = RotationOrder.OrderZXY;
        private SerializedProperty m_Rotation;
        private GUIContent rotationContent = new GUIContent("Rotation", "The local rotation of this Game Object relative to the parent.");
        private static int s_FoldoutHash = "Foldout".GetHashCode();
        private Object[] targets;

        public void OnEnable(SerializedProperty m_Rotation, GUIContent label)
        {
            this.m_Rotation = m_Rotation;
            this.targets = m_Rotation.serializedObject.targetObjects;
            this.m_OldRotationOrder = (this.targets[0] as Transform).rotationOrder;
            this.rotationContent = label;
        }

        public void RotationField()
        {
            this.RotationField(false);
        }

        public void RotationField(bool disabled)
        {
            Transform transform = this.targets[0] as Transform;
            Vector3 localEulerAngles = transform.GetLocalEulerAngles(transform.rotationOrder);
            if (((this.m_OldEulerAngles.x != localEulerAngles.x) || (this.m_OldEulerAngles.y != localEulerAngles.y)) || ((this.m_OldEulerAngles.z != localEulerAngles.z) || (this.m_OldRotationOrder != transform.rotationOrder)))
            {
                this.m_EulerAngles = transform.GetLocalEulerAngles(transform.rotationOrder);
                this.m_OldRotationOrder = transform.rotationOrder;
            }
            bool flag = false;
            bool flag2 = false;
            for (int i = 1; i < this.targets.Length; i++)
            {
                Transform transform2 = this.targets[i] as Transform;
                Vector3 vector2 = transform2.GetLocalEulerAngles(transform2.rotationOrder);
                flag |= ((vector2.x != localEulerAngles.x) || (vector2.y != localEulerAngles.y)) || (vector2.z != localEulerAngles.z);
                flag2 |= transform2.rotationOrder != transform.rotationOrder;
            }
            Rect totalPosition = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight * (!EditorGUIUtility.wideMode ? ((float) 2) : ((float) 1)), new GUILayoutOption[0]);
            GUIContent label = EditorGUI.BeginProperty(totalPosition, this.rotationContent, this.m_Rotation);
            EditorGUI.showMixedValue = flag;
            EditorGUI.BeginChangeCheck();
            int id = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, totalPosition);
            string str = string.Empty;
            if (AnimationMode.InAnimationMode() && (transform.rotationOrder != RotationOrder.OrderZXY))
            {
                if (flag2)
                {
                    str = "Mixed";
                }
                else
                {
                    str = transform.rotationOrder.ToString();
                    str = str.Substring(str.Length - 3);
                }
                label.text = label.text + " (" + str + ")";
            }
            totalPosition = EditorGUI.MultiFieldPrefixLabel(totalPosition, id, label, 3);
            totalPosition.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.BeginDisabledGroup(disabled);
            this.m_EulerAngles = EditorGUI.Vector3Field(totalPosition, GUIContent.none, this.m_EulerAngles);
            EditorGUI.EndDisabledGroup();
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(this.targets, "Inspector");
                foreach (Transform transform3 in this.targets)
                {
                    transform3.SetLocalEulerAngles(this.m_EulerAngles, transform3.rotationOrder);
                    if (transform3.parent != null)
                    {
                        transform3.SendTransformChangedScale();
                    }
                }
                this.m_Rotation.serializedObject.SetIsDifferentCacheDirty();
            }
            EditorGUI.showMixedValue = false;
            if (flag2)
            {
                EditorGUILayout.HelpBox("Transforms have different rotation orders, keyframes saved will have the same value but not the same local rotation", MessageType.Warning);
            }
            EditorGUI.EndProperty();
        }
    }
}

