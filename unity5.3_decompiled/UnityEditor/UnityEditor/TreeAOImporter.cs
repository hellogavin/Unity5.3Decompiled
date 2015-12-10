namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TreeAOImporter : AssetPostprocessor
    {
        private void OnPostprocessModel(GameObject root)
        {
            if (base.assetPath.ToLower().IndexOf("ambient-occlusion") != -1)
            {
                foreach (MeshFilter filter in root.GetComponentsInChildren(typeof(MeshFilter)))
                {
                    if (filter.sharedMesh != null)
                    {
                        Mesh sharedMesh = filter.sharedMesh;
                        TreeAO.CalcSoftOcclusion(sharedMesh);
                        Bounds bounds = sharedMesh.bounds;
                        Color[] colors = sharedMesh.colors;
                        Vector3[] vertices = sharedMesh.vertices;
                        Vector4[] tangents = sharedMesh.tangents;
                        if (colors.Length == 0)
                        {
                            colors = new Color[sharedMesh.vertexCount];
                            for (int m = 0; m < colors.Length; m++)
                            {
                                colors[m] = Color.white;
                            }
                        }
                        float b = 0f;
                        for (int i = 0; i < tangents.Length; i++)
                        {
                            b = Mathf.Max(tangents[i].w, b);
                        }
                        float num5 = 0f;
                        for (int j = 0; j < colors.Length; j++)
                        {
                            Vector2 vector = new Vector2(vertices[j].x, vertices[j].z);
                            num5 = Mathf.Max(vector.magnitude, num5);
                        }
                        for (int k = 0; k < colors.Length; k++)
                        {
                            Vector2 vector2 = new Vector2(vertices[k].x, vertices[k].z);
                            float num9 = vector2.magnitude / num5;
                            float num10 = (vertices[k].y - bounds.min.y) / bounds.size.y;
                            colors[k].a = ((num10 * num9) * 0.6f) + (num10 * 0.5f);
                        }
                        sharedMesh.colors = colors;
                    }
                }
            }
        }
    }
}

