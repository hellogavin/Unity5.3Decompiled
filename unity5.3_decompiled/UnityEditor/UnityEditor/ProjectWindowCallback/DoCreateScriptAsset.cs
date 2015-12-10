namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;

    internal class DoCreateScriptAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            ProjectWindowUtil.ShowCreatedAsset(ProjectWindowUtil.CreateScriptAssetFromTemplate(pathName, resourceFile));
        }
    }
}

