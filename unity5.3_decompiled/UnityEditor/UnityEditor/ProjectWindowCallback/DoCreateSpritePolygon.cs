namespace UnityEditor.ProjectWindowCallback
{
    using System;
    using UnityEditor;
    using UnityEditor.Sprites;

    internal class DoCreateSpritePolygon : EndNameEditAction
    {
        public int sides;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            bool flag = false;
            if (this.sides < 0)
            {
                this.sides = 5;
                flag = true;
            }
            SpriteUtility.CreateSpritePolygonAssetAtPath(pathName, this.sides);
            if (flag)
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(pathName);
                SpriteEditorWindow.GetWindow();
            }
        }
    }
}

