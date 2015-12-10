namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class PreviewGenerator
    {
        private const string kPreviewBuildFolder = "builds";
        protected static PreviewGenerator s_Instance;

        public byte[] GeneratePreview(string assetPath, int width, int height)
        {
            Object targetObject = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (targetObject == null)
            {
                return null;
            }
            Editor editor = Editor.CreateEditor(targetObject);
            if (editor == null)
            {
                return null;
            }
            Texture2D textured = editor.RenderStaticPreview(assetPath, null, width, height);
            if (textured == null)
            {
                Object.DestroyImmediate(editor);
                return null;
            }
            byte[] buffer = textured.EncodeToPNG();
            Object.DestroyImmediate(textured);
            Object.DestroyImmediate(editor);
            return buffer;
        }

        public static PreviewGenerator GetInstance()
        {
            if (s_Instance == null)
            {
                return new PreviewGenerator();
            }
            return s_Instance;
        }
    }
}

