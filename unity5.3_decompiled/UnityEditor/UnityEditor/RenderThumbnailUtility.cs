namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class RenderThumbnailUtility
    {
        public static Bounds CalculateVisibleBounds(GameObject prefab)
        {
            return prefab.GetComponent<Renderer>().bounds;
        }

        public static Texture2D Render(GameObject prefab)
        {
            if (prefab == null)
            {
                return null;
            }
            if (prefab.GetComponent<Renderer>() == null)
            {
                return null;
            }
            Texture2D textured = new Texture2D(0x40, 0x40) {
                hideFlags = HideFlags.HideAndDontSave,
                name = "Preview Texture"
            };
            RenderTexture temporary = RenderTexture.GetTemporary(textured.width, textured.height);
            GameObject obj2 = new GameObject("Preview") {
                hideFlags = HideFlags.HideAndDontSave
            };
            Camera camera = obj2.AddComponent(typeof(Camera)) as Camera;
            camera.cameraType = CameraType.Preview;
            camera.clearFlags = CameraClearFlags.Color;
            camera.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0f);
            camera.cullingMask = 0;
            camera.enabled = false;
            camera.targetTexture = temporary;
            Light light = obj2.AddComponent(typeof(Light)) as Light;
            light.type = LightType.Directional;
            Bounds bounds = CalculateVisibleBounds(prefab);
            Vector3 vector = new Vector3(0.7f, 0.3f, 0.7f);
            float num = bounds.extents.magnitude * 1.6f;
            obj2.transform.position = bounds.center + ((Vector3) (vector.normalized * num));
            obj2.transform.LookAt(bounds.center);
            camera.nearClipPlane = num * 0.1f;
            camera.farClipPlane = num * 2.2f;
            Camera current = Camera.current;
            camera.RenderDontRestore();
            Light[] lights = new Light[] { light };
            Graphics.SetupVertexLights(lights);
            foreach (Renderer renderer in prefab.GetComponentsInChildren(typeof(Renderer)))
            {
                if (renderer.enabled)
                {
                    Material[] sharedMaterials = renderer.sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        if (sharedMaterials[i] != null)
                        {
                            Material original = sharedMaterials[i];
                            string dependency = ShaderUtil.GetDependency(original.shader, "BillboardShader");
                            if ((dependency != null) && (dependency != string.Empty))
                            {
                                original = Object.Instantiate<Material>(original);
                                original.shader = Shader.Find(dependency);
                                original.hideFlags = HideFlags.HideAndDontSave;
                            }
                            for (int j = 0; j < original.passCount; j++)
                            {
                                if (original.SetPass(j))
                                {
                                    renderer.RenderNow(i);
                                }
                            }
                            if (original != sharedMaterials[i])
                            {
                                Object.DestroyImmediate(original);
                            }
                        }
                    }
                }
            }
            textured.ReadPixels(new Rect(0f, 0f, (float) textured.width, (float) textured.height), 0, 0);
            RenderTexture.ReleaseTemporary(temporary);
            Object.DestroyImmediate(obj2);
            Camera.SetupCurrent(current);
            return textured;
        }
    }
}

