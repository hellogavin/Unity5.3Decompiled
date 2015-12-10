namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    internal class GenericInspector : Editor
    {
        private AudioFilterGUI m_AudioFilterGUI;

        internal override bool GetOptimizedGUIBlock(bool isDirty, bool isVisible, out OptimizedGUIBlock block, out float height)
        {
            bool flag = base.GetOptimizedGUIBlockImplementation(isDirty, isVisible, out block, out height);
            if (((this.target is MonoBehaviour) && AudioUtil.HaveAudioCallback(this.target as MonoBehaviour)) && (AudioUtil.GetCustomFilterChannelCount(this.target as MonoBehaviour) > 0))
            {
                return false;
            }
            if (this.IsMissingMonoBehaviourTarget())
            {
                return false;
            }
            return flag;
        }

        private bool IsMissingMonoBehaviourTarget()
        {
            return ((this.target.GetType() == typeof(MonoBehaviour)) || (this.target.GetType() == typeof(ScriptableObject)));
        }

        public bool MissingMonoBehaviourGUI()
        {
            base.serializedObject.Update();
            SerializedProperty property = base.serializedObject.FindProperty("m_Script");
            if (property == null)
            {
                return false;
            }
            EditorGUILayout.PropertyField(property, new GUILayoutOption[0]);
            MonoScript objectReferenceValue = property.objectReferenceValue as MonoScript;
            bool flag = true;
            if ((objectReferenceValue != null) && objectReferenceValue.GetScriptTypeWasJustCreatedFromComponentMenu())
            {
                flag = false;
            }
            if (flag)
            {
                EditorGUILayout.HelpBox(EditorGUIUtility.TextContent("The associated script can not be loaded.\nPlease fix any compile errors\nand assign a valid script.").text, MessageType.Warning, true);
            }
            if (base.serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.ForceRebuildInspectors();
            }
            return true;
        }

        public override void OnInspectorGUI()
        {
            if (!this.IsMissingMonoBehaviourTarget() || !this.MissingMonoBehaviourGUI())
            {
                base.OnInspectorGUI();
                if (((this.target is MonoBehaviour) && AudioUtil.HaveAudioCallback(this.target as MonoBehaviour)) && (AudioUtil.GetCustomFilterChannelCount(this.target as MonoBehaviour) > 0))
                {
                    if (this.m_AudioFilterGUI == null)
                    {
                        this.m_AudioFilterGUI = new AudioFilterGUI();
                    }
                    this.m_AudioFilterGUI.DrawAudioFilterGUI(this.target as MonoBehaviour);
                }
            }
        }

        internal override bool OnOptimizedInspectorGUI(Rect contentRect)
        {
            return base.OptimizedInspectorGUIImplementation(contentRect);
        }
    }
}

