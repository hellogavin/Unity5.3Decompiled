namespace UnityEngine
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    [UsedByNativeCode]
    public sealed class Camera : Behaviour
    {
        public static CameraCallback onPostRender;
        public static CameraCallback onPreCull;
        public static CameraCallback onPreRender;

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void AddCommandBuffer(CameraEvent evt, CommandBuffer buffer);
        public Matrix4x4 CalculateObliqueMatrix(Vector4 clipPlane)
        {
            Matrix4x4 matrixx;
            INTERNAL_CALL_CalculateObliqueMatrix(this, ref clipPlane, out matrixx);
            return matrixx;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CopyFrom(Camera other);
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Camera.DoClear has been deprecated (UnityUpgradable).", true)]
        public void DoClear()
        {
        }

        [RequiredByNativeCode]
        private static void FireOnPostRender(Camera cam)
        {
            if (onPostRender != null)
            {
                onPostRender(cam);
            }
        }

        [RequiredByNativeCode]
        private static void FireOnPreCull(Camera cam)
        {
            if (onPreCull != null)
            {
                onPreCull(cam);
            }
        }

        [RequiredByNativeCode]
        private static void FireOnPreRender(Camera cam)
        {
            if (onPreRender != null)
            {
                onPreRender(cam);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern int GetAllCameras(Camera[] cameras);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern CommandBuffer[] GetCommandBuffers(CameraEvent evt);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern string[] GetHDRWarnings();
        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property GetScreenHeight() has been deprecated. Use Screen.height instead (UnityUpgradable) -> Screen.height", true)]
        public float GetScreenHeight()
        {
            return 0f;
        }

        [Obsolete("Property GetScreenWidth() has been deprecated. Use Screen.width instead (UnityUpgradable) -> Screen.width", true), EditorBrowsable(EditorBrowsableState.Never)]
        public float GetScreenWidth()
        {
            return 0f;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_CalculateObliqueMatrix(Camera self, ref Vector4 clipPlane, out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern GameObject INTERNAL_CALL_RaycastTry(Camera self, ref Ray ray, float distance, int layerMask, QueryTriggerInteraction queryTriggerInteraction);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern GameObject INTERNAL_CALL_RaycastTry2D(Camera self, ref Ray ray, float distance, int layerMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetAspect(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetFieldOfView(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetProjectionMatrix(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetReplacementShader(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ResetWorldToCameraMatrix(Camera self);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ScreenPointToRay(Camera self, ref Vector3 position, out Ray value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ScreenToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ScreenToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetStereoProjectionMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_SetStereoViewMatrices(Camera self, ref Matrix4x4 leftMatrix, ref Matrix4x4 rightMatrix);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ViewportPointToRay(Camera self, ref Vector3 position, out Ray value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ViewportToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_ViewportToWorldPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_WorldToScreenPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_WorldToViewportPoint(Camera self, ref Vector3 position, out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_backgroundColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_cameraToWorldMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_pixelRect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_projectionMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_rect(out Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_velocity(out Vector3 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_worldToCameraMatrix(out Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_RenderToCubemapRT(RenderTexture cubemap, int faceMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_RenderToCubemapTexture(Cubemap cubemap, int faceMask);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_backgroundColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_pixelRect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_projectionMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_rect(ref Rect value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_worldToCameraMatrix(ref Matrix4x4 value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern bool IsFiltered(GameObject go);
        internal void OnlyUsedForTesting1()
        {
        }

        internal void OnlyUsedForTesting2()
        {
        }

        [ExcludeFromDocs]
        internal GameObject RaycastTry(Ray ray, float distance, int layerMask)
        {
            QueryTriggerInteraction useGlobal = QueryTriggerInteraction.UseGlobal;
            return INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, useGlobal);
        }

        internal GameObject RaycastTry(Ray ray, float distance, int layerMask, [DefaultValue("QueryTriggerInteraction.UseGlobal")] QueryTriggerInteraction queryTriggerInteraction)
        {
            return INTERNAL_CALL_RaycastTry(this, ref ray, distance, layerMask, queryTriggerInteraction);
        }

        internal GameObject RaycastTry2D(Ray ray, float distance, int layerMask)
        {
            return INTERNAL_CALL_RaycastTry2D(this, ref ray, distance, layerMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RemoveAllCommandBuffers();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RemoveCommandBuffer(CameraEvent evt, CommandBuffer buffer);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RemoveCommandBuffers(CameraEvent evt);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Render();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RenderDontRestore();
        [ExcludeFromDocs]
        public bool RenderToCubemap(Cubemap cubemap)
        {
            int faceMask = 0x3f;
            return this.RenderToCubemap(cubemap, faceMask);
        }

        [ExcludeFromDocs]
        public bool RenderToCubemap(RenderTexture cubemap)
        {
            int faceMask = 0x3f;
            return this.RenderToCubemap(cubemap, faceMask);
        }

        public bool RenderToCubemap(Cubemap cubemap, [DefaultValue("63")] int faceMask)
        {
            return this.Internal_RenderToCubemapTexture(cubemap, faceMask);
        }

        public bool RenderToCubemap(RenderTexture cubemap, [DefaultValue("63")] int faceMask)
        {
            return this.Internal_RenderToCubemapRT(cubemap, faceMask);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RenderWithShader(Shader shader, string replacementTag);
        public void ResetAspect()
        {
            INTERNAL_CALL_ResetAspect(this);
        }

        public void ResetFieldOfView()
        {
            INTERNAL_CALL_ResetFieldOfView(this);
        }

        public void ResetProjectionMatrix()
        {
            INTERNAL_CALL_ResetProjectionMatrix(this);
        }

        public void ResetReplacementShader()
        {
            INTERNAL_CALL_ResetReplacementShader(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetStereoProjectionMatrices();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ResetStereoViewMatrices();
        public void ResetWorldToCameraMatrix()
        {
            INTERNAL_CALL_ResetWorldToCameraMatrix(this);
        }

        public Ray ScreenPointToRay(Vector3 position)
        {
            Ray ray;
            INTERNAL_CALL_ScreenPointToRay(this, ref position, out ray);
            return ray;
        }

        public Vector3 ScreenToViewportPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ScreenToViewportPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 ScreenToWorldPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ScreenToWorldPoint(this, ref position, out vector);
            return vector;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetReplacementShader(Shader shader, string replacementTag);
        public void SetStereoProjectionMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
        {
            INTERNAL_CALL_SetStereoProjectionMatrices(this, ref leftMatrix, ref rightMatrix);
        }

        public void SetStereoViewMatrices(Matrix4x4 leftMatrix, Matrix4x4 rightMatrix)
        {
            INTERNAL_CALL_SetStereoViewMatrices(this, ref leftMatrix, ref rightMatrix);
        }

        public void SetTargetBuffers(RenderBuffer colorBuffer, RenderBuffer depthBuffer)
        {
            this.SetTargetBuffersImpl(out colorBuffer, out depthBuffer);
        }

        public void SetTargetBuffers(RenderBuffer[] colorBuffer, RenderBuffer depthBuffer)
        {
            this.SetTargetBuffersMRTImpl(colorBuffer, out depthBuffer);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTargetBuffersImpl(out RenderBuffer color, out RenderBuffer depth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTargetBuffersMRTImpl(RenderBuffer[] color, out RenderBuffer depth);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public static extern void SetupCurrent(Camera cur);
        public Ray ViewportPointToRay(Vector3 position)
        {
            Ray ray;
            INTERNAL_CALL_ViewportPointToRay(this, ref position, out ray);
            return ray;
        }

        public Vector3 ViewportToScreenPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ViewportToScreenPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 ViewportToWorldPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_ViewportToWorldPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 WorldToScreenPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_WorldToScreenPoint(this, ref position, out vector);
            return vector;
        }

        public Vector3 WorldToViewportPoint(Vector3 position)
        {
            Vector3 vector;
            INTERNAL_CALL_WorldToViewportPoint(this, ref position, out vector);
            return vector;
        }

        public RenderingPath actualRenderingPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static Camera[] allCameras { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public static int allCamerasCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float aspect { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color backgroundColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_backgroundColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_backgroundColor(ref value);
            }
        }

        public Matrix4x4 cameraToWorldMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_cameraToWorldMatrix(out matrixx);
                return matrixx;
            }
        }

        public CameraType cameraType { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public CameraClearFlags clearFlags { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool clearStencilAfterLightingPass { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int commandBufferCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int cullingMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Camera current { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float depth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public DepthTextureMode depthTextureMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int eventMask { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("use Camera.farClipPlane instead.")]
        public float far
        {
            get
            {
                return this.farClipPlane;
            }
            set
            {
                this.farClipPlane = value;
            }
        }

        public float farClipPlane { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float fieldOfView { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("use Camera.fieldOfView instead.")]
        public float fov
        {
            get
            {
                return this.fieldOfView;
            }
            set
            {
                this.fieldOfView = value;
            }
        }

        public bool hdr { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Property isOrthoGraphic has been deprecated. Use orthographic (UnityUpgradable) -> orthographic", true), EditorBrowsable(EditorBrowsableState.Never)]
        public bool isOrthoGraphic
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public float[] layerCullDistances { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool layerCullSpherical { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public static Camera main { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        [EditorBrowsable(EditorBrowsableState.Never), Obsolete("Property mainCamera has been deprecated. Use Camera.main instead (UnityUpgradable) -> main", true)]
        public static Camera mainCamera
        {
            get
            {
                return null;
            }
        }

        [Obsolete("use Camera.nearClipPlane instead.")]
        public float near
        {
            get
            {
                return this.nearClipPlane;
            }
            set
            {
                this.nearClipPlane = value;
            }
        }

        public float nearClipPlane { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public OpaqueSortMode opaqueSortMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool orthographic { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float orthographicSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int pixelHeight { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Rect pixelRect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_pixelRect(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_pixelRect(ref value);
            }
        }

        public int pixelWidth { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        internal static int PreviewCullingLayer { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Matrix4x4 projectionMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_projectionMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_projectionMatrix(ref value);
            }
        }

        public Rect rect
        {
            get
            {
                Rect rect;
                this.INTERNAL_get_rect(out rect);
                return rect;
            }
            set
            {
                this.INTERNAL_set_rect(ref value);
            }
        }

        public RenderingPath renderingPath { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float stereoConvergence { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool stereoEnabled { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool stereoMirrorMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float stereoSeparation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int targetDisplay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public RenderTexture targetTexture { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public TransparencySortMode transparencySortMode { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool useOcclusionCulling { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector3 velocity
        {
            get
            {
                Vector3 vector;
                this.INTERNAL_get_velocity(out vector);
                return vector;
            }
        }

        public Matrix4x4 worldToCameraMatrix
        {
            get
            {
                Matrix4x4 matrixx;
                this.INTERNAL_get_worldToCameraMatrix(out matrixx);
                return matrixx;
            }
            set
            {
                this.INTERNAL_set_worldToCameraMatrix(ref value);
            }
        }

        public delegate void CameraCallback(Camera cam);
    }
}

