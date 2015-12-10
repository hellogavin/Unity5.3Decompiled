namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(AudioLowPassFilter)), CanEditMultipleObjects]
    internal class AudioLowPassFilterInspector : Editor
    {
        private SerializedProperty m_LowpassLevelCustomCurve;
        private SerializedProperty m_LowpassResonanceQ;

        private void OnEnable()
        {
            this.m_LowpassResonanceQ = base.serializedObject.FindProperty("m_LowpassResonanceQ");
            this.m_LowpassLevelCustomCurve = base.serializedObject.FindProperty("lowpassLevelCustomCurve");
        }

        public override void OnInspectorGUI()
        {
            base.serializedObject.Update();
            AudioSourceInspector.AnimProp(new GUIContent("Cutoff Frequency"), this.m_LowpassLevelCustomCurve, 22000f, 0f, true);
            EditorGUILayout.PropertyField(this.m_LowpassResonanceQ, new GUILayoutOption[0]);
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}

