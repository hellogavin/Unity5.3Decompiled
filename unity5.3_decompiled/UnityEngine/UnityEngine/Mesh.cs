namespace UnityEngine
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class Mesh : Object
    {
        public Mesh()
        {
            Internal_Create(this);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void AddBlendShapeFrame(string shapeName, float frameWeight, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool CheckCanAccessUVChannel(int channel);
        [ExcludeFromDocs]
        public void Clear()
        {
            bool keepVertexLayout = true;
            this.Clear(keepVertexLayout);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Clear([DefaultValue("true")] bool keepVertexLayout);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void ClearBlendShapes();
        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine)
        {
            bool useMatrices = true;
            bool mergeSubMeshes = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        [ExcludeFromDocs]
        public void CombineMeshes(CombineInstance[] combine, bool mergeSubMeshes)
        {
            bool useMatrices = true;
            this.CombineMeshes(combine, mergeSubMeshes, useMatrices);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void CombineMeshes(CombineInstance[] combine, [DefaultValue("true")] bool mergeSubMeshes, [DefaultValue("true")] bool useMatrices);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Array ExtractListData(object list);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetBlendShapeFrameCount(int shapeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void GetBlendShapeFrameVertices(int shapeIndex, int frameIndex, Vector3[] deltaVertices, Vector3[] deltaNormals, Vector3[] deltaTangents);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern float GetBlendShapeFrameWeight(int shapeIndex, int frameIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetBlendShapeIndex(string blendShapeName);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern string GetBlendShapeName(int shapeIndex);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int[] GetIndices(int submesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern MeshTopology GetTopology(int submesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int[] GetTriangles(int submesh);
        public void GetUVs(int channel, List<Vector2> uvs)
        {
            this.GetUVsImpl<Vector2>(channel, uvs, 2);
        }

        public void GetUVs(int channel, List<Vector3> uvs)
        {
            this.GetUVsImpl<Vector3>(channel, uvs, 3);
        }

        public void GetUVs(int channel, List<Vector4> uvs)
        {
            this.GetUVsImpl<Vector4>(channel, uvs, 4);
        }

        private void GetUVsImpl<T>(int channel, List<T> uvs, int dim)
        {
            if (uvs == null)
            {
                throw new ArgumentException("The result uvs list cannot be null");
            }
            if (this.CheckCanAccessUVChannel(channel))
            {
                int vertexCount = this.vertexCount;
                if (vertexCount > uvs.Capacity)
                {
                    uvs.Capacity = vertexCount;
                }
                ResizeList(uvs, vertexCount);
                this.GetUVsInternal(ExtractListData(uvs), channel, dim);
            }
            else
            {
                uvs.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void GetUVsInternal(Array uvs, int channel, int dim);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void Internal_Create([Writable] Mesh mono);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_bounds(out Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_bounds(ref Bounds value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void MarkDynamic();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void Optimize();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RecalculateBounds();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void RecalculateNormals();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void ResizeList(object list, int size);
        public void SetColors(List<Color> inColors)
        {
            this.SetColorsInternal(inColors);
        }

        public void SetColors(List<Color32> inColors)
        {
            this.SetColors32Internal(inColors);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetColors32Internal(object colors);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetColorsInternal(object colors);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetIndices(int[] indices, MeshTopology topology, int submesh);
        public void SetNormals(List<Vector3> inNormals)
        {
            this.SetNormalsInternal(inNormals);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetNormalsInternal(object normals);
        public void SetTangents(List<Vector4> inTangents)
        {
            this.SetTangentsInternal(inTangents);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTangentsInternal(object tangents);
        public void SetTriangles(List<int> inTriangles, int submesh)
        {
            this.SetTrianglesInternal(inTriangles, submesh);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetTriangles(int[] triangles, int submesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetTrianglesInternal(object triangles, int submesh);
        public void SetUVs(int channel, List<Vector2> uvs)
        {
            this.SetUVsInternal(ExtractListData(uvs), channel, 2, uvs.Count);
        }

        public void SetUVs(int channel, List<Vector3> uvs)
        {
            this.SetUVsInternal(ExtractListData(uvs), channel, 3, uvs.Count);
        }

        public void SetUVs(int channel, List<Vector4> uvs)
        {
            this.SetUVsInternal(ExtractListData(uvs), channel, 4, uvs.Count);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetUVsInternal(Array uvs, int channel, int dim, int arraySize);
        public void SetVertices(List<Vector3> inVertices)
        {
            this.SetVerticesInternal(inVertices);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void SetVerticesInternal(object vertices);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void UploadMeshData(bool markNoLogerReadable);

        public Matrix4x4[] bindposes { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int blendShapeCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public BoneWeight[] boneWeights { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Bounds bounds
        {
            get
            {
                Bounds bounds;
                this.INTERNAL_get_bounds(out bounds);
                return bounds;
            }
            set
            {
                this.INTERNAL_set_bounds(ref value);
            }
        }

        internal bool canAccess { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Color[] colors { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color32[] colors32 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isReadable { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3[] normals { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int subMeshCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector4[] tangents { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int[] triangles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2[] uv { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("Property Mesh.uv1 has been deprecated. Use Mesh.uv2 instead (UnityUpgradable) -> uv2", true), EditorBrowsable(EditorBrowsableState.Never)]
        public Vector2[] uv1
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public Vector2[] uv2 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2[] uv3 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Vector2[] uv4 { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Vector3[] vertices { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

