namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;
    using UnityEditor.SceneManagement;

    internal class DoCreateScene : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            bool createDefaultGameObjects = true;
            if (EditorSceneManager.CreateSceneAsset(pathName, createDefaultGameObjects))
            {
                ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath(pathName, typeof(SceneAsset)));
            }
        }
    }
}

