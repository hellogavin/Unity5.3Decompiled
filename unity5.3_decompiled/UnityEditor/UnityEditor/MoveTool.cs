namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class MoveTool : ManipulationTool
    {
        private static MoveTool s_Instance;

        public static void OnGUI(SceneView view)
        {
            if (s_Instance == null)
            {
                s_Instance = new MoveTool();
            }
            s_Instance.OnToolGUI(view);
        }

        public override void ToolGUI(SceneView view, Vector3 handlePosition, bool isStatic)
        {
            TransformManipulator.BeginManipulationHandling(false);
            EditorGUI.BeginChangeCheck();
            Vector3 vector = Handles.PositionHandle(handlePosition, Tools.handleRotation);
            if (EditorGUI.EndChangeCheck() && !isStatic)
            {
                Vector3 positionDelta = vector - TransformManipulator.mouseDownHandlePosition;
                ManipulationToolUtility.SetMinDragDifferenceForPos(handlePosition);
                TransformManipulator.SetPositionDelta(positionDelta);
            }
            TransformManipulator.EndManipulationHandling();
        }
    }
}

