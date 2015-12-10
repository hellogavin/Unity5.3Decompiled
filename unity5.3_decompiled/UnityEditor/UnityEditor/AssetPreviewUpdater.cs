namespace UnityEditor
{
    using System;
    using System.Reflection;
    using UnityEngine;

    internal class AssetPreviewUpdater
    {
        public static Texture2D CreatePreviewForAsset(Object obj, Object[] subAssets, string assetPath)
        {
            if (obj == null)
            {
                return null;
            }
            Type type = CustomEditorAttributes.FindCustomEditorType(obj, false);
            if (type == null)
            {
                return null;
            }
            MethodInfo method = type.GetMethod("RenderStaticPreview");
            if (method == null)
            {
                Debug.LogError("Fail to find RenderStaticPreview base method");
                return null;
            }
            if (method.DeclaringType == typeof(Editor))
            {
                return null;
            }
            Editor editor = Editor.CreateEditor(obj);
            if (editor == null)
            {
                return null;
            }
            Texture2D textured = editor.RenderStaticPreview(assetPath, subAssets, 0x80, 0x80);
            Object.DestroyImmediate(editor);
            return textured;
        }
    }
}

