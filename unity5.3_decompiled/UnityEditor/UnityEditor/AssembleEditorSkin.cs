namespace UnityEditor
{
    using System;

    internal class AssembleEditorSkin : EditorWindow
    {
        public static void DoIt()
        {
            EditorApplication.ExecuteMenuItem("Tools/Regenerate Editor Skins Now");
        }

        private static void RegenerateAllIconsWithMipLevels()
        {
            GenerateIconsWithMipLevels.GenerateAllIconsWithMipLevels();
        }

        private static void RegenerateSelectedIconsWithMipLevels()
        {
            GenerateIconsWithMipLevels.GenerateSelectedIconsWithMips();
        }
    }
}

