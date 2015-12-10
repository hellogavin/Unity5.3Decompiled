namespace UnityEngine
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Scripting;

    [StructLayout(LayoutKind.Sequential), UsedByNativeCode]
    public struct UIVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Color32 color;
        public Vector2 uv0;
        public Vector2 uv1;
        public Vector4 tangent;
        private static readonly Color32 s_DefaultColor;
        private static readonly Vector4 s_DefaultTangent;
        public static UIVertex simpleVert;
        static UIVertex()
        {
            s_DefaultColor = new Color32(0xff, 0xff, 0xff, 0xff);
            s_DefaultTangent = new Vector4(1f, 0f, 0f, -1f);
            UIVertex vertex = new UIVertex {
                position = Vector3.zero,
                normal = Vector3.back,
                tangent = s_DefaultTangent,
                color = s_DefaultColor,
                uv0 = Vector2.zero,
                uv1 = Vector2.zero
            };
            simpleVert = vertex;
        }
    }
}

