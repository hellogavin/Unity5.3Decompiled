namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Gizmos
    {
        public static void DrawCube(Vector3 center, Vector3 size)
        {
            INTERNAL_CALL_DrawCube(ref center, ref size);
        }

        public static void DrawFrustum(Vector3 center, float fov, float maxRange, float minRange, float aspect)
        {
            INTERNAL_CALL_DrawFrustum(ref center, fov, maxRange, minRange, aspect);
        }

        [ExcludeFromDocs]
        public static void DrawGUITexture(Rect screenRect, Texture texture)
        {
            Material mat = null;
            DrawGUITexture(screenRect, texture, mat);
        }

        public static void DrawGUITexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
        {
            DrawGUITexture(screenRect, texture, 0, 0, 0, 0, mat);
        }

        [ExcludeFromDocs]
        public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
            Material mat = null;
            INTERNAL_CALL_DrawGUITexture(ref screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
        }

        public static void DrawGUITexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
        {
            INTERNAL_CALL_DrawGUITexture(ref screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
        }

        [ExcludeFromDocs]
        public static void DrawIcon(Vector3 center, string name)
        {
            bool allowScaling = true;
            INTERNAL_CALL_DrawIcon(ref center, name, allowScaling);
        }

        public static void DrawIcon(Vector3 center, string name, [DefaultValue("true")] bool allowScaling)
        {
            INTERNAL_CALL_DrawIcon(ref center, name, allowScaling);
        }

        public static void DrawLine(Vector3 from, Vector3 to)
        {
            INTERNAL_CALL_DrawLine(ref from, ref to);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            Vector3 zero = Vector3.zero;
            DrawMesh(mesh, zero, identity, one);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, int submeshIndex)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            Vector3 zero = Vector3.zero;
            INTERNAL_CALL_DrawMesh(mesh, submeshIndex, ref zero, ref identity, ref one);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            DrawMesh(mesh, position, identity, one);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            INTERNAL_CALL_DrawMesh(mesh, submeshIndex, ref position, ref identity, ref one);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            Vector3 one = Vector3.one;
            DrawMesh(mesh, position, rotation, one);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
        {
            Vector3 one = Vector3.one;
            INTERNAL_CALL_DrawMesh(mesh, submeshIndex, ref position, ref rotation, ref one);
        }

        public static void DrawMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
        {
            DrawMesh(mesh, -1, position, rotation, scale);
        }

        public static void DrawMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
        {
            INTERNAL_CALL_DrawMesh(mesh, submeshIndex, ref position, ref rotation, ref scale);
        }

        public static void DrawRay(Ray r)
        {
            DrawLine(r.origin, r.origin + r.direction);
        }

        public static void DrawRay(Vector3 from, Vector3 direction)
        {
            DrawLine(from, from + direction);
        }

        public static void DrawSphere(Vector3 center, float radius)
        {
            INTERNAL_CALL_DrawSphere(ref center, radius);
        }

        public static void DrawWireCube(Vector3 center, Vector3 size)
        {
            INTERNAL_CALL_DrawWireCube(ref center, ref size);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            Vector3 zero = Vector3.zero;
            DrawWireMesh(mesh, zero, identity, one);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh, int submeshIndex)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            Vector3 zero = Vector3.zero;
            INTERNAL_CALL_DrawWireMesh(mesh, submeshIndex, ref zero, ref identity, ref one);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh, Vector3 position)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            DrawWireMesh(mesh, position, identity, one);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position)
        {
            Vector3 one = Vector3.one;
            Quaternion identity = Quaternion.identity;
            INTERNAL_CALL_DrawWireMesh(mesh, submeshIndex, ref position, ref identity, ref one);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            Vector3 one = Vector3.one;
            DrawWireMesh(mesh, position, rotation, one);
        }

        [ExcludeFromDocs]
        public static void DrawWireMesh(Mesh mesh, int submeshIndex, Vector3 position, Quaternion rotation)
        {
            Vector3 one = Vector3.one;
            INTERNAL_CALL_DrawWireMesh(mesh, submeshIndex, ref position, ref rotation, ref one);
        }

        public static void DrawWireMesh(Mesh mesh, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
        {
            DrawWireMesh(mesh, -1, position, rotation, scale);
        }

        public static void DrawWireMesh(Mesh mesh, int submeshIndex, [DefaultValue("Vector3.zero")] Vector3 position, [DefaultValue("Quaternion.identity")] Quaternion rotation, [DefaultValue("Vector3.one")] Vector3 scale)
        {
            INTERNAL_CALL_DrawWireMesh(mesh, submeshIndex, ref position, ref rotation, ref scale);
        }

        public static void DrawWireSphere(Vector3 center, float radius)
        {
            INTERNAL_CALL_DrawWireSphere(ref center, radius);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawCube(ref Vector3 center, ref Vector3 size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawFrustum(ref Vector3 center, float fov, float maxRange, float minRange, float aspect);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawGUITexture(ref Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Material mat);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawIcon(ref Vector3 center, string name, bool allowScaling);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawLine(ref Vector3 from, ref Vector3 to);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawMesh(Mesh mesh, int submeshIndex, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawSphere(ref Vector3 center, float radius);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawWireCube(ref Vector3 center, ref Vector3 size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawWireMesh(Mesh mesh, int submeshIndex, ref Vector3 position, ref Quaternion rotation, ref Vector3 scale);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_DrawWireSphere(ref Vector3 center, float radius);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_color(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_get_matrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_color(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_set_matrix(ref Matrix4x4 value);

        public static Color color
        {
            get
            {
                Color color;
                INTERNAL_get_color(out color);
                return color;
            }
            set
            {
                INTERNAL_set_color(ref value);
            }
        }

        public static Matrix4x4 matrix
        {
            get
            {
                Matrix4x4 matrixx;
                INTERNAL_get_matrix(out matrixx);
                return matrixx;
            }
            set
            {
                INTERNAL_set_matrix(ref value);
            }
        }
    }
}

