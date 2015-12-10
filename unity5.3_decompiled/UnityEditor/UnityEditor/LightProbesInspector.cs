namespace UnityEditor
{
    using System;
    using UnityEngine;

    [CustomEditor(typeof(LightProbes))]
    internal class LightProbesInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical(EditorStyles.helpBox, new GUILayoutOption[0]);
            LightProbes target = this.target as LightProbes;
            GUIStyle wordWrappedMiniLabel = EditorStyles.wordWrappedMiniLabel;
            GUILayout.Label("Light probe count: " + target.count, wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.Label("Cell count: " + target.cellCount, wordWrappedMiniLabel, new GUILayoutOption[0]);
            GUILayout.EndVertical();
        }
    }
}

