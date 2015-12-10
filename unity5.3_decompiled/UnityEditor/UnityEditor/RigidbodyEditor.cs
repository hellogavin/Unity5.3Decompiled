namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(Rigidbody))]
    internal class RigidbodyEditor : Editor
    {
        private SerializedProperty m_Constraints;
        private static GUIContent m_FreezePositionLabel = new GUIContent("Freeze Position");
        private static GUIContent m_FreezeRotationLabel = new GUIContent("Freeze Rotation");

        private void ConstraintToggle(Rect r, string label, RigidbodyConstraints value, int bit)
        {
            bool flag = (value & (((int) 1) << bit)) != RigidbodyConstraints.None;
            EditorGUI.showMixedValue = (this.m_Constraints.hasMultipleDifferentValuesBitwise & (((int) 1) << bit)) != 0;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            flag = EditorGUI.ToggleLeft(r, label, flag);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Edit Constraints");
                this.m_Constraints.SetBitAtIndexForAllTargetsImmediate(bit, flag);
            }
            EditorGUI.showMixedValue = false;
        }

        public void OnEnable()
        {
            this.m_Constraints = base.serializedObject.FindProperty("m_Constraints");
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.m_Constraints.isExpanded = EditorGUILayout.Foldout(this.m_Constraints.isExpanded, "Constraints");
            GUILayout.EndHorizontal();
            base.serializedObject.Update();
            RigidbodyConstraints intValue = (RigidbodyConstraints) this.m_Constraints.intValue;
            if (this.m_Constraints.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.ToggleBlock(intValue, m_FreezePositionLabel, 1, 2, 3);
                this.ToggleBlock(intValue, m_FreezeRotationLabel, 4, 5, 6);
                EditorGUI.indentLevel--;
            }
        }

        private void ToggleBlock(RigidbodyConstraints constraints, GUIContent label, int x, int y, int z)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x1c3f, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, label);
            position.width = 30f;
            this.ConstraintToggle(position, "X", constraints, x);
            position.x += 30f;
            this.ConstraintToggle(position, "Y", constraints, y);
            position.x += 30f;
            this.ConstraintToggle(position, "Z", constraints, z);
            GUILayout.EndHorizontal();
        }
    }
}

