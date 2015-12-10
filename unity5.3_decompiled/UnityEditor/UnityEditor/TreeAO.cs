namespace UnityEditor
{
    using System;
    using UnityEngine;

    internal class TreeAO
    {
        private static Vector3[] directions;
        private static bool kDebug;
        private const int kWorkLayer = 0x1d;
        private const float occlusion = 0.5f;

        public static void CalcSoftOcclusion(Mesh mesh)
        {
            GameObject obj2 = new GameObject("Test") {
                layer = 0x1d
            };
            obj2.AddComponent<MeshFilter>().mesh = mesh;
            obj2.AddComponent<MeshCollider>();
            if (directions == null)
            {
                InitializeDirections();
            }
            Vector4[] vectorArray = new Vector4[directions.Length];
            for (int i = 0; i < directions.Length; i++)
            {
                vectorArray[i] = new Vector4(GetWeight(1, directions[i]), GetWeight(2, directions[i]), GetWeight(3, directions[i]), GetWeight(0, directions[i]));
            }
            Vector3[] vertices = mesh.vertices;
            Vector4[] vectorArray3 = new Vector4[vertices.Length];
            float num2 = 0f;
            for (int j = 0; j < vertices.Length; j++)
            {
                Vector4 zero = Vector4.zero;
                Vector3 v = obj2.transform.TransformPoint(vertices[j]);
                for (int m = 0; m < directions.Length; m++)
                {
                    float p = CountIntersections(v, obj2.transform.TransformDirection(directions[m]), 3f);
                    p = Mathf.Pow(0.5f, p);
                    zero += (Vector4) (vectorArray[m] * p);
                }
                zero = (Vector4) (zero / ((float) directions.Length));
                num2 += zero.w;
                vectorArray3[j] = zero;
            }
            num2 /= (float) vertices.Length;
            for (int k = 0; k < vertices.Length; k++)
            {
                vectorArray3[k].w -= num2;
            }
            mesh.tangents = vectorArray3;
            Object.DestroyImmediate(obj2);
        }

        private static int CountIntersections(Vector3 v, Vector3 dist, float length)
        {
            v += (Vector3) (dist * 0.01f);
            if (!kDebug)
            {
                return (Physics.RaycastAll(v, dist, length, 0x20000000).Length + Physics.RaycastAll(v + ((Vector3) (dist * length)), -dist, length, 0x20000000).Length);
            }
            RaycastHit[] hitArray = Physics.RaycastAll(v, dist, length, 0x20000000);
            int num = hitArray.Length;
            float distance = 0f;
            if (num > 0)
            {
                distance = hitArray[hitArray.Length - 1].distance;
            }
            hitArray = Physics.RaycastAll(v + ((Vector3) (dist * length)), -dist, length, 0x20000000);
            if (hitArray.Length > 0)
            {
                float num3 = length - hitArray[0].distance;
                if (num3 > distance)
                {
                    distance = num3;
                }
            }
            return (num + hitArray.Length);
        }

        private static float GetWeight(int coeff, Vector3 dir)
        {
            switch (coeff)
            {
                case 0:
                    return 0.5f;

                case 1:
                    return (0.5f * dir.x);

                case 2:
                    return (0.5f * dir.y);

                case 3:
                    return (0.5f * dir.z);
            }
            Debug.Log("Only defined up to 3");
            return 0f;
        }

        public static void InitializeDirections()
        {
            float z = (1f + Mathf.Sqrt(5f)) / 2f;
            directions = new Vector3[60];
            directions[0] = new Vector3(0f, 1f, 3f * z);
            directions[1] = new Vector3(0f, 1f, -3f * z);
            directions[2] = new Vector3(0f, -1f, 3f * z);
            directions[3] = new Vector3(0f, -1f, -3f * z);
            directions[4] = new Vector3(1f, 3f * z, 0f);
            directions[5] = new Vector3(1f, -3f * z, 0f);
            directions[6] = new Vector3(-1f, 3f * z, 0f);
            directions[7] = new Vector3(-1f, -3f * z, 0f);
            directions[8] = new Vector3(3f * z, 0f, 1f);
            directions[9] = new Vector3(3f * z, 0f, -1f);
            directions[10] = new Vector3(-3f * z, 0f, 1f);
            directions[11] = new Vector3(-3f * z, 0f, -1f);
            int offset = 12;
            offset = PermuteCuboid(directions, offset, 2f, 1f + (2f * z), z);
            offset = PermuteCuboid(directions, offset, 1f + (2f * z), z, 2f);
            offset = PermuteCuboid(directions, offset, z, 2f, 1f + (2f * z));
            offset = PermuteCuboid(directions, offset, 1f, 2f + z, 2f * z);
            offset = PermuteCuboid(directions, offset, 2f + z, 2f * z, 1f);
            offset = PermuteCuboid(directions, offset, 2f * z, 1f, 2f + z);
            for (int i = 0; i < directions.Length; i++)
            {
                directions[i] = directions[i].normalized;
            }
        }

        private static int PermuteCuboid(Vector3[] dirs, int offset, float x, float y, float z)
        {
            dirs[offset] = new Vector3(x, y, z);
            dirs[offset + 1] = new Vector3(x, y, -z);
            dirs[offset + 2] = new Vector3(x, -y, z);
            dirs[offset + 3] = new Vector3(x, -y, -z);
            dirs[offset + 4] = new Vector3(-x, y, z);
            dirs[offset + 5] = new Vector3(-x, y, -z);
            dirs[offset + 6] = new Vector3(-x, -y, z);
            dirs[offset + 7] = new Vector3(-x, -y, -z);
            return (offset + 8);
        }
    }
}

