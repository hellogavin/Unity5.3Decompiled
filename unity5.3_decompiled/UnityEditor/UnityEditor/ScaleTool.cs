namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class ScaleTool : ManipulationTool
    {
        private static Vector3 s_CurrentScale = Vector3.one;
        private static ScaleTool s_Instance;

        public static void OnGUI(SceneView view)
        {
            if (s_Instance == null)
            {
                s_Instance = new ScaleTool();
            }
            s_Instance.OnToolGUI(view);
        }

        public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
        {
            Quaternion targetRotation = (Selection.transforms.Length <= 1) ? Tools.handleLocalRotation : Tools.handleRotation;
            TransformManipulator.DebugAlignment(targetRotation);
            if (Event.current.type == EventType.MouseDown)
            {
                s_CurrentScale = Vector3.one;
            }
            EditorGUI.BeginChangeCheck();
            TransformManipulator.BeginManipulationHandling(true);
            s_CurrentScale = Handles.ScaleHandle(s_CurrentScale, handlePosition, targetRotation, HandleUtility.GetHandleSize(handlePosition));
            TransformManipulator.EndManipulationHandling();
            if (EditorGUI.EndChangeCheck() && !isStatic)
            {
                TransformManipulator.SetScaleDelta(s_CurrentScale, targetRotation);
            }
        }
    }
}

