namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;

    internal class DoCreateNewAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
            ProjectWindowUtil.FrameObjectInProjectWindow(instanceId);
        }
    }
}

