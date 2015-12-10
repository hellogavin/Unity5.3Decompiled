namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class GL
    {
        public const int LINES = 1;
        public const int QUADS = 7;
        public const int TRIANGLE_STRIP = 5;
        public const int TRIANGLES = 4;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Begin(int mode);
        [ExcludeFromDocs]
        public static void Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor)
        {
            float depth = 1f;
            Clear(clearDepth, clearColor, backgroundColor, depth);
        }

        public static void Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor, [DefaultValue("1.0f")] float depth)
        {
            Internal_Clear(clearDepth, clearColor, backgroundColor, depth);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearWithSkybox(bool clearDepth, Camera camera);
        public static void Color(UnityEngine.Color c)
        {
            INTERNAL_CALL_Color(ref c);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void End();
        public static Matrix4x4 GetGPUProjectionMatrix(Matrix4x4 proj, bool renderIntoTexture)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_GetGPUProjectionMatrix(ref proj, renderIntoTexture, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Color(ref UnityEngine.Color c);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_GetGPUProjectionMatrix(ref Matrix4x4 proj, bool renderIntoTexture, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_Clear(bool clearDepth, bool clearColor, ref UnityEngine.Color backgroundColor, float depth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_LoadProjectionMatrix(ref Matrix4x4 mat);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MultiTexCoord(int unit, ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_MultMatrix(ref Matrix4x4 mat);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_TexCoord(ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Vertex(ref Vector3 v);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Viewport(ref Rect pixelRect);
        private static void Internal_Clear(bool clearDepth, bool clearColor, UnityEngine.Color backgroundColor, float depth)
        {
            INTERNAL_CALL_Internal_Clear(clearDepth, clearColor, ref backgroundColor, depth);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_modelview(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_modelview(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void InvalidateState();
        [MethodImpl(MethodImplOptions.InternalCall), Obsolete("IssuePluginEvent(eventID) is deprecated. Use IssuePluginEvent(callback, eventID) instead."), WrapperlessIcall]
        public static extern void IssuePluginEvent(int eventID);
        public static void IssuePluginEvent(IntPtr callback, int eventID)
        {
            if (callback == IntPtr.Zero)
            {
                throw new ArgumentException("Null callback specified.");
            }
            IssuePluginEventInternal(callback, eventID);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void IssuePluginEventInternal(IntPtr callback, int eventID);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadIdentity();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadOrtho();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void LoadPixelMatrix();
        public static void LoadPixelMatrix(float left, float right, float bottom, float top)
        {
            LoadPixelMatrixArgs(left, right, bottom, top);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void LoadPixelMatrixArgs(float left, float right, float bottom, float top);
        public static void LoadProjectionMatrix(Matrix4x4 mat)
        {
            INTERNAL_CALL_LoadProjectionMatrix(ref mat);
        }

        public static void MultiTexCoord(int unit, Vector3 v)
        {
            INTERNAL_CALL_MultiTexCoord(unit, ref v);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MultiTexCoord2(int unit, float x, float y);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void MultiTexCoord3(int unit, float x, float y, float z);
        public static void MultMatrix(Matrix4x4 mat)
        {
            INTERNAL_CALL_MultMatrix(ref mat);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PopMatrix();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void PushMatrix();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void RenderTargetBarrier();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall, Obsolete("Use invertCulling property")]
        public static extern void SetRevertBackfacing(bool revertBackFaces);
        public static void TexCoord(Vector3 v)
        {
            INTERNAL_CALL_TexCoord(ref v);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void TexCoord2(float x, float y);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void TexCoord3(float x, float y, float z);
        public static void Vertex(Vector3 v)
        {
            INTERNAL_CALL_Vertex(ref v);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Vertex3(float x, float y, float z);
        public static void Viewport(Rect pixelRect)
        {
            INTERNAL_CALL_Viewport(ref pixelRect);
        }

        public static bool invertCulling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Matrix4x4 modelview
        {
            get
            {
                Matrix4x4 matrixx;
                INTERNAL_get_modelview(out matrixx);
                return matrixx;
            }
            set
            {
                INTERNAL_set_modelview(ref value);
            }
        }

        public static bool sRGBWrite { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static bool wireframe { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

