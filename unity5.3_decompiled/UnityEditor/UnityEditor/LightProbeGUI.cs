namespace UnityEditor
{
    using System;

    internal class LightProbeGUI
    {
        public void DisplayControls(SceneView sceneView)
        {
            LightmapVisualization.showLightProbeLocations = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Light Probes"), LightmapVisualization.showLightProbeLocations, new GUILayoutOption[0]);
            EditorGUI.indentLevel++;
            LightmapVisualization.showLightProbeCells = EditorGUILayout.Toggle(EditorGUIUtility.TextContent("Show Cells"), LightmapVisualization.showLightProbeCells, new GUILayoutOption[0]);
            EditorGUI.indentLevel--;
        }
    }
}

