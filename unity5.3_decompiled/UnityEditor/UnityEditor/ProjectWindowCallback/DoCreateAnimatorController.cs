namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;
    using UnityEditor.Animations;

    internal class DoCreateAnimatorController : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            ProjectWindowUtil.ShowCreatedAsset(AnimatorController.CreateAnimatorControllerAtPath(pathName));
        }
    }
}

