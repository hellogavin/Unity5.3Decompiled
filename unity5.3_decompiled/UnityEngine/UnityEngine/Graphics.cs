namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;

    public sealed class Graphics
    {
        [ExcludeFromDocs]
        public static void Blit(Texture source, Material mat)
        {
            int pass = -1;
            Blit(source, mat, pass);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void Blit(Texture source, RenderTexture dest);
        public static void Blit(Texture source, Material mat, [DefaultValue("-1")] int pass)
        {
            Internal_BlitMaterial(source, null, mat, pass, false);
        }

        [ExcludeFromDocs]
        public static void Blit(Texture source, RenderTexture dest, Material mat)
        {
            int pass = -1;
            Blit(source, dest, mat, pass);
        }

        public static void Blit(Texture source, RenderTexture dest, Material mat, [DefaultValue("-1")] int pass)
        {
            Internal_BlitMaterial(source, dest, mat, pass, true);
        }

        public static void BlitMultiTap(Texture source, RenderTexture dest, Material mat, params Vector2[] offsets)
        {
            Internal_BlitMultiTap(source, dest, mat, offsets);
        }

        internal static void CheckLoadActionValid(RenderBufferLoadAction load, string bufferType)
        {
            if ((load != RenderBufferLoadAction.Load) && (load != RenderBufferLoadAction.DontCare))
            {
                object[] args = new object[] { bufferType };
                throw new ArgumentException(UnityString.Format("Bad {0} LoadAction provided.", args));
            }
        }

        internal static void CheckStoreActionValid(RenderBufferStoreAction store, string bufferType)
        {
            if ((store != RenderBufferStoreAction.Store) && (store != RenderBufferStoreAction.DontCare))
            {
                object[] args = new object[] { bufferType };
                throw new ArgumentException(UnityString.Format("Bad {0} StoreAction provided.", args));
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ClearRandomWriteTargets();
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, int materialIndex)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation)
        {
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            Camera camera = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Method DrawMesh has been deprecated. Use Graphics.DrawMeshNow instead (UnityUpgradable) -> DrawMeshNow(*)", true)]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            Camera camera = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            int submeshIndex = 0;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            MaterialPropertyBlock properties = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
        {
            bool receiveShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            Transform probeAnchor = null;
            bool receiveShadows = true;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties)
        {
            bool receiveShadows = true;
            bool castShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows)
        {
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, !castShadows ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            Transform probeAnchor = null;
            DrawMesh(mesh, matrix, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, bool castShadows)
        {
            bool receiveShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows)
        {
            Transform probeAnchor = null;
            bool receiveShadows = true;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor);
        }

        public static void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor)
        {
            Internal_DrawMeshMatrixArguments arguments = new Internal_DrawMeshMatrixArguments {
                matrix = matrix,
                layer = layer,
                submeshIndex = submeshIndex,
                castShadows = (int) castShadows,
                receiveShadows = !receiveShadows ? 0 : 1,
                reflectionProbeAnchorInstanceID = (probeAnchor == null) ? 0 : probeAnchor.GetInstanceID()
            };
            Internal_DrawMeshMatrix(ref arguments, properties, material, mesh, camera);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, [DefaultValue("null")] Camera camera, [DefaultValue("0")] int submeshIndex, [DefaultValue("null")] MaterialPropertyBlock properties, [DefaultValue("true")] bool castShadows, [DefaultValue("true")] bool receiveShadows)
        {
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, !castShadows ? ShadowCastingMode.Off : ShadowCastingMode.On, receiveShadows);
        }

        [ExcludeFromDocs]
        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, bool receiveShadows)
        {
            Transform probeAnchor = null;
            DrawMesh(mesh, position, rotation, material, layer, camera, submeshIndex, properties, castShadows, receiveShadows, probeAnchor);
        }

        public static void DrawMesh(Mesh mesh, Vector3 position, Quaternion rotation, Material material, int layer, Camera camera, int submeshIndex, MaterialPropertyBlock properties, ShadowCastingMode castShadows, [DefaultValue("true")] bool receiveShadows, [DefaultValue("null")] Transform probeAnchor)
        {
            Internal_DrawMeshTRArguments arguments = new Internal_DrawMeshTRArguments {
                position = position,
                rotation = rotation,
                layer = layer,
                submeshIndex = submeshIndex,
                castShadows = (int) castShadows,
                receiveShadows = !receiveShadows ? 0 : 1,
                reflectionProbeAnchorInstanceID = (probeAnchor == null) ? 0 : probeAnchor.GetInstanceID()
            };
            Internal_DrawMeshTR(ref arguments, properties, material, mesh, camera);
        }

        public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix)
        {
            Internal_DrawMeshNow2(mesh, matrix, -1);
        }

        public static void DrawMeshNow(Mesh mesh, Matrix4x4 matrix, int materialIndex)
        {
            Internal_DrawMeshNow2(mesh, matrix, materialIndex);
        }

        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation)
        {
            Internal_DrawMeshNow1(mesh, position, rotation, -1);
        }

        public static void DrawMeshNow(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
            Internal_DrawMeshNow1(mesh, position, rotation, materialIndex);
        }

        [ExcludeFromDocs]
        public static void DrawProcedural(MeshTopology topology, int vertexCount)
        {
            int instanceCount = 1;
            DrawProcedural(topology, vertexCount, instanceCount);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DrawProcedural(MeshTopology topology, int vertexCount, [DefaultValue("1")] int instanceCount);
        [ExcludeFromDocs]
        public static void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs)
        {
            int argsOffset = 0;
            DrawProceduralIndirect(topology, bufferWithArgs, argsOffset);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void DrawProceduralIndirect(MeshTopology topology, ComputeBuffer bufferWithArgs, [DefaultValue("0")] int argsOffset);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void DrawTexture(ref InternalDrawTextureArguments arguments);
        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture)
        {
            Material mat = null;
            DrawTexture(screenRect, texture, mat);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, [DefaultValue("null")] Material mat)
        {
            DrawTexture(screenRect, texture, 0, 0, 0, 0, mat);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
            Material mat = null;
            DrawTexture(screenRect, texture, leftBorder, rightBorder, topBorder, bottomBorder, mat);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
        {
            DrawTexture(screenRect, texture, new Rect(0f, 0f, 1f, 1f), leftBorder, rightBorder, topBorder, bottomBorder, mat);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder)
        {
            Material mat = null;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, mat);
        }

        [ExcludeFromDocs]
        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color)
        {
            Material mat = null;
            DrawTexture(screenRect, texture, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, color, mat);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, [DefaultValue("null")] Material mat)
        {
            Color32 color;
            InternalDrawTextureArguments arguments = new InternalDrawTextureArguments {
                screenRect = screenRect,
                texture = texture,
                sourceRect = sourceRect,
                leftBorder = leftBorder,
                rightBorder = rightBorder,
                topBorder = topBorder,
                bottomBorder = bottomBorder
            };
            color = new Color32 {
                r = color.g = color.b = (byte) (color.a = 0x80)
            };
            arguments.color = color;
            arguments.mat = mat;
            DrawTexture(ref arguments);
        }

        public static void DrawTexture(Rect screenRect, Texture texture, Rect sourceRect, int leftBorder, int rightBorder, int topBorder, int bottomBorder, Color color, [DefaultValue("null")] Material mat)
        {
            InternalDrawTextureArguments arguments = new InternalDrawTextureArguments {
                screenRect = screenRect,
                texture = texture,
                sourceRect = sourceRect,
                leftBorder = leftBorder,
                rightBorder = rightBorder,
                topBorder = topBorder,
                bottomBorder = bottomBorder,
                color = color,
                mat = mat
            };
            DrawTexture(ref arguments);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void ExecuteCommandBuffer(CommandBuffer buffer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetActiveColorBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void GetActiveDepthBuffer(out RenderBuffer res);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_BlitMaterial(Texture source, RenderTexture dest, Material mat, int pass, bool setRT);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_BlitMultiTap(Texture source, RenderTexture dest, Material mat, Vector2[] offsets);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DrawMeshNow1(Mesh mesh, ref Vector3 position, ref Quaternion rotation, int materialIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Internal_DrawMeshNow2(Mesh mesh, ref Matrix4x4 matrix, int materialIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_DrawMeshMatrix(ref Internal_DrawMeshMatrixArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);
        private static void Internal_DrawMeshNow1(Mesh mesh, Vector3 position, Quaternion rotation, int materialIndex)
        {
            INTERNAL_CALL_Internal_DrawMeshNow1(mesh, ref position, ref rotation, materialIndex);
        }

        private static void Internal_DrawMeshNow2(Mesh mesh, Matrix4x4 matrix, int materialIndex)
        {
            INTERNAL_CALL_Internal_DrawMeshNow2(mesh, ref matrix, materialIndex);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_DrawMeshTR(ref Internal_DrawMeshTRArguments arguments, MaterialPropertyBlock properties, Material material, Mesh mesh, Camera camera);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetMRTFullSetup(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face, RenderBufferLoadAction[] colorLoadSA, RenderBufferStoreAction[] colorStoreSA, RenderBufferLoadAction depthLoad, RenderBufferStoreAction depthStore);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetMRTSimple(RenderBuffer[] colorSA, out RenderBuffer depth, int mip, CubemapFace face);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetNullRT();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetRandomWriteTargetBuffer(int index, ComputeBuffer uav);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetRandomWriteTargetRT(int index, RenderTexture uav);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_SetRTSimple(out RenderBuffer color, out RenderBuffer depth, int mip, CubemapFace face);
        public static void SetRandomWriteTarget(int index, ComputeBuffer uav)
        {
            Internal_SetRandomWriteTargetBuffer(index, uav);
        }

        public static void SetRandomWriteTarget(int index, RenderTexture uav)
        {
            Internal_SetRandomWriteTargetRT(index, uav);
        }

        public static void SetRenderTarget(RenderTargetSetup setup)
        {
            SetRenderTargetImpl(setup);
        }

        public static void SetRenderTarget(RenderTexture rt)
        {
            SetRenderTargetImpl(rt, 0, CubemapFace.Unknown);
        }

        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, 0, CubemapFace.Unknown);
        }

        public static void SetRenderTarget(RenderTexture rt, int mipLevel)
        {
            SetRenderTargetImpl(rt, mipLevel, CubemapFace.Unknown);
        }

        public static void SetRenderTarget(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer)
        {
            SetRenderTargetImpl(colorBuffers, depthBuffer, 0, CubemapFace.Unknown);
        }

        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, CubemapFace.Unknown);
        }

        public static void SetRenderTarget(RenderTexture rt, int mipLevel, CubemapFace face)
        {
            SetRenderTargetImpl(rt, mipLevel, face);
        }

        public static void SetRenderTarget(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
        {
            SetRenderTargetImpl(colorBuffer, depthBuffer, mipLevel, face);
        }

        internal static void SetRenderTargetImpl(RenderTargetSetup setup)
        {
            if (setup.color.Length == 0)
            {
                throw new ArgumentException("Invalid color buffer count for SetRenderTarget");
            }
            if (setup.color.Length != setup.colorLoad.Length)
            {
                throw new ArgumentException("Color LoadAction and Buffer arrays have different sizes");
            }
            if (setup.color.Length != setup.colorStore.Length)
            {
                throw new ArgumentException("Color StoreAction and Buffer arrays have different sizes");
            }
            foreach (RenderBufferLoadAction action in setup.colorLoad)
            {
                CheckLoadActionValid(action, "Color");
            }
            foreach (RenderBufferStoreAction action2 in setup.colorStore)
            {
                CheckStoreActionValid(action2, "Color");
            }
            CheckLoadActionValid(setup.depthLoad, "Depth");
            CheckStoreActionValid(setup.depthStore, "Depth");
            if ((setup.cubemapFace < CubemapFace.Unknown) || (setup.cubemapFace > CubemapFace.NegativeZ))
            {
                throw new ArgumentException("Bad CubemapFace provided");
            }
            Internal_SetMRTFullSetup(setup.color, out setup.depth, setup.mipLevel, setup.cubemapFace, setup.colorLoad, setup.colorStore, setup.depthLoad, setup.depthStore);
        }

        internal static void SetRenderTargetImpl(RenderTexture rt, int mipLevel, CubemapFace face)
        {
            if (rt != null)
            {
                SetRenderTargetImpl(rt.colorBuffer, rt.depthBuffer, mipLevel, face);
            }
            else
            {
                Internal_SetNullRT();
            }
        }

        internal static void SetRenderTargetImpl(RenderBuffer colorBuffer, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
        {
            RenderBuffer color = colorBuffer;
            RenderBuffer depth = depthBuffer;
            Internal_SetRTSimple(out color, out depth, mipLevel, face);
        }

        internal static void SetRenderTargetImpl(RenderBuffer[] colorBuffers, RenderBuffer depthBuffer, int mipLevel, CubemapFace face)
        {
            RenderBuffer depth = depthBuffer;
            Internal_SetMRTSimple(colorBuffers, out depth, mipLevel, face);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal static extern void SetupVertexLights(Light[] lights);

        public static RenderBuffer activeColorBuffer
        {
            get
            {
                RenderBuffer buffer;
                GetActiveColorBuffer(out buffer);
                return buffer;
            }
        }

        public static RenderBuffer activeDepthBuffer
        {
            get
            {
                RenderBuffer buffer;
                GetActiveDepthBuffer(out buffer);
                return buffer;
            }
        }

        [Obsolete("Property deviceName has been deprecated. Use SystemInfo.graphicsDeviceName instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceName", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static string deviceName
        {
            get
            {
                return SystemInfo.graphicsDeviceName;
            }
        }

        [Obsolete("Property deviceVendor has been deprecated. Use SystemInfo.graphicsDeviceVendor instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVendor", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static string deviceVendor
        {
            get
            {
                return SystemInfo.graphicsDeviceVendor;
            }
        }

        [Obsolete("Property deviceVersion has been deprecated. Use SystemInfo.graphicsDeviceVersion instead (UnityUpgradable) -> UnityEngine.SystemInfo.graphicsDeviceVersion", true), EditorBrowsable(EditorBrowsableState.Never)]
        public static string deviceVersion
        {
            get
            {
                return SystemInfo.graphicsDeviceVersion;
            }
        }
    }
}

