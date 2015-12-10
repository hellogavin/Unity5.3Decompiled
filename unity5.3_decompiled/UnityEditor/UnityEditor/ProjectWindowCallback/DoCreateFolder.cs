namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    internal class DoCreateFolder : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            ProjectWindowUtil.ShowCreatedAsset(AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.CreateFolder(Path.GetDirectoryName(pathName), Path.GetFileName(pathName))), typeof(Object)));
        }
    }
}

