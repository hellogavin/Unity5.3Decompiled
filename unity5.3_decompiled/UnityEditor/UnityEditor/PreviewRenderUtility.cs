namespace UnityEditor
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Rendering;

    public class PreviewRenderUtility
    {
        public Camera m_Camera;
        public float m_CameraFieldOfView;
        public Light[] m_Light;
        internal RenderTexture m_RenderTexture;
        private SavedRenderTargetState m_SavedState;
        private Rect m_TargetRect;

        public PreviewRenderUtility() : this(false)
        {
        }

        public PreviewRenderUtility(bool renderFullScene)
        {
            this.m_CameraFieldOfView = 15f;
            this.m_Light = new Light[2];
            Type[] components = new Type[] { typeof(Camera) };
            this.m_Camera = EditorUtility.CreateGameObjectWithHideFlags("PreRenderCamera", HideFlags.HideAndDontSave, components).GetComponent<Camera>();
            this.m_Camera.cameraType = CameraType.Preview;
            this.m_Camera.fieldOfView = this.m_CameraFieldOfView;
            this.m_Camera.enabled = false;
            this.m_Camera.clearFlags = CameraClearFlags.Depth;
            this.m_Camera.farClipPlane = 10f;
            this.m_Camera.nearClipPlane = 2f;
            this.m_Camera.backgroundColor = new Color(0.1921569f, 0.1921569f, 0.1921569f, 1f);
            this.m_Camera.renderingPath = RenderingPath.Forward;
            this.m_Camera.useOcclusionCulling = false;
            if (!renderFullScene)
            {
                Handles.SetCameraOnlyDrawMesh(this.m_Camera);
            }
            for (int i = 0; i < 2; i++)
            {
                Type[] typeArray2 = new Type[] { typeof(Light) };
                this.m_Light[i] = EditorUtility.CreateGameObjectWithHideFlags("PreRenderLight", HideFlags.HideAndDontSave, typeArray2).GetComponent<Light>();
                this.m_Light[i].type = LightType.Directional;
                this.m_Light[i].intensity = 1f;
                this.m_Light[i].enabled = false;
            }
            this.m_Light[0].color = SceneView.kSceneViewFrontLight;
            this.m_Light[1].transform.rotation = Quaternion.Euler(340f, 218f, 177f);
            this.m_Light[1].color = (Color) (new Color(0.4f, 0.4f, 0.45f, 0f) * 0.7f);
        }

        public void BeginPreview(Rect r, GUIStyle previewBackground)
        {
            this.InitPreview(r);
            if ((previewBackground != null) && (previewBackground != GUIStyle.none))
            {
                Graphics.DrawTexture(previewBackground.overflow.Add(new Rect(0f, 0f, (float) this.m_RenderTexture.width, (float) this.m_RenderTexture.height)), previewBackground.normal.background, new Rect(0f, 0f, 1f, 1f), previewBackground.border.left, previewBackground.border.right, previewBackground.border.top, previewBackground.border.bottom, new Color(0.5f, 0.5f, 0.5f, 1f), null);
            }
        }

        public void BeginStaticPreview(Rect r)
        {
            this.InitPreview(r);
            Color color = new Color(0.3215686f, 0.3215686f, 0.3215686f, 1f);
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, true, true);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            Graphics.DrawTexture(new Rect(0f, 0f, (float) this.m_RenderTexture.width, (float) this.m_RenderTexture.height), texture);
            Object.DestroyImmediate(texture);
        }

        public void Cleanup()
        {
            if (this.m_Camera != null)
            {
                Object.DestroyImmediate(this.m_Camera.gameObject, true);
            }
            if (this.m_RenderTexture != null)
            {
                Object.DestroyImmediate(this.m_RenderTexture);
                this.m_RenderTexture = null;
            }
            foreach (Light light in this.m_Light)
            {
                if (light != null)
                {
                    Object.DestroyImmediate(light.gameObject, true);
                }
            }
        }

        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex)
        {
            this.DrawMesh(mesh, matrix, mat, subMeshIndex, null);
        }

        public void DrawMesh(Mesh mesh, Matrix4x4 matrix, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
        {
            Graphics.DrawMesh(mesh, matrix, mat, 1, this.m_Camera, subMeshIndex, customProperties);
        }

        public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex)
        {
            this.DrawMesh(mesh, pos, rot, mat, subMeshIndex, null);
        }

        public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties)
        {
            Graphics.DrawMesh(mesh, pos, rot, mat, 1, this.m_Camera, subMeshIndex, customProperties);
        }

        public void DrawMesh(Mesh mesh, Vector3 pos, Quaternion rot, Material mat, int subMeshIndex, MaterialPropertyBlock customProperties, Transform probeAnchor)
        {
            Graphics.DrawMesh(mesh, pos, rot, mat, 1, this.m_Camera, subMeshIndex, customProperties, ShadowCastingMode.Off, false, probeAnchor);
        }

        public void EndAndDrawPreview(Rect r)
        {
            Texture image = this.EndPreview();
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            GUI.DrawTexture(r, image, ScaleMode.StretchToFill, false);
            GL.sRGBWrite = false;
        }

        public Texture EndPreview()
        {
            this.m_SavedState.Restore();
            return this.m_RenderTexture;
        }

        public Texture2D EndStaticPreview()
        {
            RenderTexture dest = RenderTexture.GetTemporary((int) this.m_TargetRect.width, (int) this.m_TargetRect.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            Graphics.Blit(this.m_RenderTexture, dest);
            GL.sRGBWrite = false;
            RenderTexture.active = dest;
            Texture2D textured = new Texture2D((int) this.m_TargetRect.width, (int) this.m_TargetRect.height, TextureFormat.RGB24, false, true);
            textured.ReadPixels(new Rect(0f, 0f, this.m_TargetRect.width, this.m_TargetRect.height), 0, 0);
            textured.Apply();
            RenderTexture.ReleaseTemporary(dest);
            this.m_SavedState.Restore();
            return textured;
        }

        internal static Mesh GetPreviewSphere()
        {
            GameObject obj2 = (GameObject) EditorGUIUtility.LoadRequired("Previews/PreviewMaterials.fbx");
            obj2.SetActive(false);
            IEnumerator enumerator = obj2.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.name == "sphere")
                    {
                        return current.GetComponent<MeshFilter>().sharedMesh;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return null;
        }

        public float GetScaleFactor(float width, float height)
        {
            float a = Mathf.Max(Mathf.Min((float) (width * 2f), (float) 1024f), width) / width;
            float b = Mathf.Max(Mathf.Min((float) (height * 2f), (float) 1024f), height) / height;
            return (Mathf.Min(a, b) * EditorGUIUtility.pixelsPerPoint);
        }

        private void InitPreview(Rect r)
        {
            this.m_TargetRect = r;
            int width = (int) r.width;
            int height = (int) r.height;
            if (((this.m_RenderTexture == null) || (this.m_RenderTexture.width != width)) || (this.m_RenderTexture.height != height))
            {
                if (this.m_RenderTexture != null)
                {
                    Object.DestroyImmediate(this.m_RenderTexture);
                    this.m_RenderTexture = null;
                }
                float scaleFactor = this.GetScaleFactor((float) width, (float) height);
                this.m_RenderTexture = new RenderTexture((int) (width * scaleFactor), (int) (height * scaleFactor), 0x10, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
                this.m_RenderTexture.hideFlags = HideFlags.HideAndDontSave;
                this.m_Camera.targetTexture = this.m_RenderTexture;
            }
            float num4 = (this.m_RenderTexture.width > 0) ? Mathf.Max((float) 1f, (float) (((float) this.m_RenderTexture.height) / ((float) this.m_RenderTexture.width))) : 1f;
            this.m_Camera.fieldOfView = (Mathf.Atan(num4 * Mathf.Tan((this.m_CameraFieldOfView * 0.5f) * 0.01745329f)) * 57.29578f) * 2f;
            this.m_SavedState = new SavedRenderTargetState();
            EditorGUIUtility.SetRenderTextureNoViewport(this.m_RenderTexture);
            GL.LoadOrtho();
            GL.LoadPixelMatrix(0f, (float) this.m_RenderTexture.width, (float) this.m_RenderTexture.height, 0f);
            ShaderUtil.rawViewportRect = new Rect(0f, 0f, (float) this.m_RenderTexture.width, (float) this.m_RenderTexture.height);
            ShaderUtil.rawScissorRect = new Rect(0f, 0f, (float) this.m_RenderTexture.width, (float) this.m_RenderTexture.height);
            GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
        }
    }
}

