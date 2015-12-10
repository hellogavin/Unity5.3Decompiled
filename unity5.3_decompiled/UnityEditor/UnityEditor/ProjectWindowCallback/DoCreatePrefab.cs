namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;

    internal class DoCreatePrefab : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            ProjectWindowUtil.ShowCreatedAsset(PrefabUtility.CreateEmptyPrefab(pathName));
        }
    }
}

