namespace UnityEditor
{
    using System;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    using UnityEngine.Events;

    [CanEditMultipleObjects, CustomEditor(typeof(Rigidbody2D))]
    internal class Rigidbody2DEditor : Editor
    {
        private const int k_ToggleOffset = 30;
        private SerializedProperty m_Constraints;
        private static readonly GUIContent m_FreezePositionLabel = new GUIContent("Freeze Position");
        private static readonly GUIContent m_FreezeRotationLabel = new GUIContent("Freeze Rotation");
        private SerializedProperty m_Mass;
        private readonly AnimBool m_ShowMass = new AnimBool();
        private SerializedProperty m_UseAutoMass;

        private void ConstraintToggle(Rect r, string label, RigidbodyConstraints2D value, int bit)
        {
            bool flag = (value & (((int) 1) << bit)) != RigidbodyConstraints2D.None;
            EditorGUI.showMixedValue = (this.m_Constraints.hasMultipleDifferentValuesBitwise & (((int) 1) << bit)) != 0;
            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            flag = EditorGUI.ToggleLeft(r, label, flag);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(base.targets, "Edit Constraints2D");
                this.m_Constraints.SetBitAtIndexForAllTargetsImmediate(bit, flag);
            }
            EditorGUI.showMixedValue = false;
        }

        public void OnDisable()
        {
            this.m_ShowMass.valueChanged.RemoveListener(new UnityAction(this.Repaint));
        }

        public void OnEnable()
        {
            Rigidbody2D target = this.target as Rigidbody2D;
            this.m_UseAutoMass = base.serializedObject.FindProperty("m_UseAutoMass");
            this.m_Mass = base.serializedObject.FindProperty("m_Mass");
            this.m_Constraints = base.serializedObject.FindProperty("m_Constraints");
            this.m_ShowMass.value = !target.useAutoMass;
            this.m_ShowMass.valueChanged.AddListener(new UnityAction(this.Repaint));
        }

        public override void OnInspectorGUI()
        {
            Rigidbody2D target = this.target as Rigidbody2D;
            base.serializedObject.Update();
            this.m_ShowMass.target = !target.useAutoMass;
            EditorGUILayout.PropertyField(this.m_UseAutoMass, new GUILayoutOption[0]);
            bool enabled = GUI.enabled;
            GUI.enabled = !target.useAutoMass;
            EditorGUILayout.PropertyField(this.m_Mass, new GUILayoutOption[0]);
            GUI.enabled = enabled;
            base.serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            this.m_Constraints.isExpanded = EditorGUILayout.Foldout(this.m_Constraints.isExpanded, "Constraints");
            GUILayout.EndHorizontal();
            RigidbodyConstraints2D intValue = (RigidbodyConstraints2D) this.m_Constraints.intValue;
            if (this.m_Constraints.isExpanded)
            {
                EditorGUI.indentLevel++;
                this.ToggleFreezePosition(intValue, m_FreezePositionLabel, 0, 1);
                this.ToggleFreezeRotation(intValue, m_FreezeRotationLabel, 2);
                EditorGUI.indentLevel--;
            }
            if (intValue == RigidbodyConstraints2D.FreezeAll)
            {
                EditorGUILayout.HelpBox("Rather than turning on all constraints, you may want to consider removing the Rigidbody2D component which makes any colliders static.  This gives far better performance overall.", MessageType.Info);
            }
        }

        private void ToggleFreezePosition(RigidbodyConstraints2D constraints, GUIContent label, int x, int y)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x1c3f, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, label);
            position.width = 30f;
            this.ConstraintToggle(position, "X", constraints, x);
            position.x += 30f;
            this.ConstraintToggle(position, "Y", constraints, y);
            GUILayout.EndHorizontal();
        }

        private void ToggleFreezeRotation(RigidbodyConstraints2D constraints, GUIContent label, int z)
        {
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, EditorGUILayout.kLabelFloatMaxW, 16f, 16f, EditorStyles.numberField);
            int id = GUIUtility.GetControlID(0x1c3f, FocusType.Keyboard, position);
            position = EditorGUI.PrefixLabel(position, id, label);
            position.width = 30f;
            this.ConstraintToggle(position, "Z", constraints, z);
            GUILayout.EndHorizontal();
        }
    }
}

