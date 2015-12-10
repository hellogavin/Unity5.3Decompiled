namespace UnityEditor
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEditor.Sprites;
    using UnityEditorInternal;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(Sprite))]
    internal class SpriteInspector : Editor
    {
        public readonly GUIContent spriteAlignment = EditorGUIUtility.TextContent("Pivot|Sprite pivot point in its localspace. May be used for syncing animation frames of different sizes.");
        public readonly GUIContent[] spriteAlignmentOptions = new GUIContent[] { EditorGUIUtility.TextContent("Center"), EditorGUIUtility.TextContent("Top Left"), EditorGUIUtility.TextContent("Top"), EditorGUIUtility.TextContent("Top Right"), EditorGUIUtility.TextContent("Left"), EditorGUIUtility.TextContent("Right"), EditorGUIUtility.TextContent("Bottom Left"), EditorGUIUtility.TextContent("Bottom"), EditorGUIUtility.TextContent("Bottom Right"), EditorGUIUtility.TextContent("Custom") };

        public static Texture2D BuildPreviewTexture(int width, int height, Sprite sprite, Material spriteRendererMaterial, bool isPolygon)
        {
            if (!ShaderUtil.hardwareSupportsRectRenderTexture)
            {
                return null;
            }
            float num = sprite.rect.width;
            float num2 = sprite.rect.height;
            Texture2D spriteTexture = SpriteUtility.GetSpriteTexture(sprite, false);
            if (!isPolygon)
            {
                PreviewHelpers.AdjustWidthAndHeightForStaticPreview((int) num, (int) num2, ref width, ref height);
            }
            SavedRenderTargetState state = new SavedRenderTargetState();
            RenderTexture temp = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
            RenderTexture.active = temp;
            GL.sRGBWrite = QualitySettings.activeColorSpace == ColorSpace.Linear;
            GL.Clear(true, true, new Color(0f, 0f, 0f, 0f));
            Texture texture = null;
            Vector4 vector = new Vector4(0f, 0f, 0f, 0f);
            bool flag = false;
            bool flag2 = false;
            if (spriteRendererMaterial != null)
            {
                flag = spriteRendererMaterial.HasProperty("_MainTex");
                flag2 = spriteRendererMaterial.HasProperty("_MainTex_TexelSize");
            }
            Material material = null;
            if (spriteRendererMaterial != null)
            {
                if (flag)
                {
                    texture = spriteRendererMaterial.GetTexture("_MainTex");
                    spriteRendererMaterial.SetTexture("_MainTex", spriteTexture);
                }
                if (flag2)
                {
                    vector = spriteRendererMaterial.GetVector("_MainTex_TexelSize");
                    spriteRendererMaterial.SetVector("_MainTex_TexelSize", TextureUtil.GetTexelSizeVector(spriteTexture));
                }
                spriteRendererMaterial.SetPass(0);
            }
            else
            {
                material = new Material(Shader.Find("Hidden/BlitCopy")) {
                    mainTexture = spriteTexture,
                    mainTextureScale = Vector2.one,
                    mainTextureOffset = Vector2.zero
                };
                material.SetPass(0);
            }
            float num3 = sprite.rect.width / sprite.bounds.size.x;
            Vector2[] vertices = sprite.vertices;
            Vector2[] uv = sprite.uv;
            ushort[] triangles = sprite.triangles;
            Vector2 pivot = sprite.pivot;
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Color(new Color(1f, 1f, 1f, 1f));
            GL.Begin(4);
            for (int i = 0; i < sprite.triangles.Length; i++)
            {
                ushort index = triangles[i];
                Vector2 vector3 = vertices[index];
                Vector2 vector4 = uv[index];
                GL.TexCoord(new Vector3(vector4.x, vector4.y, 0f));
                GL.Vertex3(((vector3.x * num3) + pivot.x) / num, ((vector3.y * num3) + pivot.y) / num2, 0f);
            }
            GL.End();
            GL.PopMatrix();
            GL.sRGBWrite = false;
            if (spriteRendererMaterial != null)
            {
                if (flag)
                {
                    spriteRendererMaterial.SetTexture("_MainTex", texture);
                }
                if (flag2)
                {
                    spriteRendererMaterial.SetVector("_MainTex_TexelSize", vector);
                }
            }
            Texture2D textured2 = new Texture2D(width, height, TextureFormat.ARGB32, false) {
                hideFlags = HideFlags.HideAndDontSave
            };
            textured2.ReadPixels(new Rect(0f, 0f, (float) width, (float) height), 0, 0);
            textured2.Apply();
            RenderTexture.ReleaseTemporary(temp);
            state.Restore();
            if (material != null)
            {
                Object.DestroyImmediate(material);
            }
            return textured2;
        }

        public static void DrawPreview(Rect r, Sprite frame, Material spriteRendererMaterial, bool isPolygon)
        {
            if (frame != null)
            {
                float num = Mathf.Min((float) (r.width / frame.rect.width), (float) (r.height / frame.rect.height));
                Rect position = new Rect(r.x, r.y, frame.rect.width * num, frame.rect.height * num) {
                    center = r.center
                };
                Texture2D image = BuildPreviewTexture((int) position.width, (int) position.height, frame, spriteRendererMaterial, isPolygon);
                EditorGUI.DrawTextureTransparent(position, image, ScaleMode.ScaleToFit);
                Vector4 vector = (Vector4) (frame.border * num);
                if (!Mathf.Approximately(vector.sqrMagnitude, 0f))
                {
                    SpriteEditorUtility.BeginLines(new Color(0f, 1f, 0f, 0.7f));
                    SpriteEditorUtility.EndLines();
                }
                Object.DestroyImmediate(image);
            }
        }

        public override string GetInfoString()
        {
            if (this.target == null)
            {
                return string.Empty;
            }
            Sprite target = this.target as Sprite;
            return string.Format("({0}x{1})", (int) target.rect.width, (int) target.rect.height);
        }

        private SpriteMetaData GetMetaData(string name)
        {
            TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.sprite)) as TextureImporter;
            if (atPath == null)
            {
                return new SpriteMetaData();
            }
            if (atPath.spriteImportMode == SpriteImportMode.Single)
            {
                return GetMetaDataInSingleMode(name, atPath);
            }
            return GetMetaDataInMultipleMode(name, atPath);
        }

        private static SpriteMetaData GetMetaDataInMultipleMode(string name, TextureImporter textureImporter)
        {
            SpriteMetaData[] spritesheet = textureImporter.spritesheet;
            for (int i = 0; i < spritesheet.Length; i++)
            {
                if (spritesheet[i].name.Equals(name))
                {
                    return spritesheet[i];
                }
            }
            return new SpriteMetaData();
        }

        private static SpriteMetaData GetMetaDataInSingleMode(string name, TextureImporter textureImporter)
        {
            SpriteMetaData data = new SpriteMetaData {
                border = textureImporter.spriteBorder,
                name = name,
                pivot = textureImporter.spritePivot,
                rect = new Rect(0f, 0f, 1f, 1f)
            };
            TextureImporterSettings dest = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(dest);
            data.alignment = dest.spriteAlignment;
            return data;
        }

        public override bool HasPreviewGUI()
        {
            return (this.target != null);
        }

        public override void OnInspectorGUI()
        {
            bool flag;
            bool flag2;
            bool flag3;
            this.UnifiedValues(out flag, out flag2, out flag3);
            if (flag)
            {
                EditorGUILayout.LabelField("Name", this.sprite.name, new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.LabelField("Name", "-", new GUILayoutOption[0]);
            }
            if (flag2)
            {
                int alignment = this.GetMetaData(this.sprite.name).alignment;
                EditorGUILayout.LabelField(this.spriteAlignment, this.spriteAlignmentOptions[alignment], new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.LabelField(this.spriteAlignment.text, "-", new GUILayoutOption[0]);
            }
            if (flag3)
            {
                Vector4 border = this.GetMetaData(this.sprite.name).border;
                object[] args = new object[] { border.x, border.y, border.z, border.w };
                EditorGUILayout.LabelField("Border", string.Format("L:{0} B:{1} R:{2} T:{3}", args), new GUILayoutOption[0]);
            }
            else
            {
                EditorGUILayout.LabelField("Border", "-", new GUILayoutOption[0]);
            }
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if ((this.target != null) && (Event.current.type == EventType.Repaint))
            {
                bool isPolygon = false;
                TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.sprite)) as TextureImporter;
                if (atPath != null)
                {
                    isPolygon = atPath.spriteImportMode == SpriteImportMode.Polygon;
                }
                DrawPreview(r, this.sprite, null, isPolygon);
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            bool isPolygon = false;
            TextureImporter atPath = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (atPath != null)
            {
                isPolygon = atPath.spriteImportMode == SpriteImportMode.Polygon;
            }
            return BuildPreviewTexture(width, height, this.sprite, null, isPolygon);
        }

        private void UnifiedValues(out bool name, out bool alignment, out bool border)
        {
            name = true;
            alignment = true;
            border = true;
            if (base.targets.Length >= 2)
            {
                TextureImporter atPath = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(this.sprite)) as TextureImporter;
                SpriteMetaData[] spritesheet = atPath.spritesheet;
                string str2 = null;
                int num = -1;
                Vector4? nullable = null;
                for (int i = 0; i < base.targets.Length; i++)
                {
                    Sprite sprite = base.targets[i] as Sprite;
                    for (int j = 0; j < spritesheet.Length; j++)
                    {
                        if (spritesheet[j].name.Equals(sprite.name))
                        {
                            if ((spritesheet[j].alignment != num) && (num > 0))
                            {
                                alignment = false;
                            }
                            else
                            {
                                num = spritesheet[j].alignment;
                            }
                            if ((spritesheet[j].name != str2) && (str2 != null))
                            {
                                name = false;
                            }
                            else
                            {
                                str2 = spritesheet[j].name;
                            }
                            if (((spritesheet[j].border != nullable.GetValueOrDefault()) || !nullable.HasValue) && nullable.HasValue)
                            {
                                border = false;
                            }
                            else
                            {
                                nullable = new Vector4?(spritesheet[j].border);
                            }
                        }
                    }
                }
            }
        }

        private Sprite sprite
        {
            get
            {
                return (this.target as Sprite);
            }
        }
    }
}

