namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class BillboardAsset : Object
    {
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MakeMaterialProperties(MaterialPropertyBlock properties, Camera camera);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MakePreviewMesh(Mesh mesh);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void MakeRenderMesh(Mesh mesh, float widthScale, float heightScale, float rotation);

        public float bottom { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float height { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int imageCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public int indexCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public Material material { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int vertexCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float width { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

